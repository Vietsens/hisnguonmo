/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.ConnectionTest.ADO;
using Inventec.Common.Adapter;
using Inventec.Core;
using LIS.EFMODEL.DataModels;
using LIS.Filter;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ConnectionTest
{
    public partial class UC_ConnectionTest : HIS.Desktop.Utility.UserControlBase
    {
        #region NhomXN - Declare

        /// <summary>Sentinel key cho nhóm "Khác" (dịch vụ không có PARENT_ID).</summary>
        private const long SERVICE_GROUP_KHAC_KEY = -1L;

        /// <summary>Map SAMPLE_ID -> tập key nhóm cha (PARENT_ID hoặc SERVICE_GROUP_KHAC_KEY).</summary>
        private Dictionary<long, HashSet<long>> _groupKeysBySampleId = new Dictionary<long, HashSet<long>>();

        /// <summary>Các key nhóm đang được tích chọn — giữ nguyên qua các lần reload mẫu.</summary>
        private HashSet<long> _selectedServiceGroupKeys = new HashSet<long>();

        /// <summary>Các key nhóm hiện có trong dropdown (theo dữ liệu mẫu trong phiên).</summary>
        private HashSet<long> _availableServiceGroupKeys = new HashSet<long>();

        /// <summary>Chặn EditValueChanged fire khi đang nạp/rebuild items theo code.</summary>
        private bool _isLoadingServiceGroup = false;

        #endregion

        #region NhomXN - Build + Filter

        /// <summary>
        /// Nạp dịch vụ XN của các mẫu trong phiên, dựng danh sách nhóm cha (+ "Khác"),
        /// giữ nguyên các nhóm đang chọn rồi áp dụng lại filter.
        /// Gọi ở cuối FillDataToGridSample.
        /// </summary>
        internal void LoadServiceGroupFilter()
        {
            try
            {
                _groupKeysBySampleId = new Dictionary<long, HashSet<long>>();
                _availableServiceGroupKeys = new HashSet<long>();

                List<LisSampleADO> samples = lstSampleAll;
                if (samples == null || samples.Count == 0)
                {
                    RebuildServiceGroupItems(new List<KeyValuePair<long, string>>());
                    ApplyServiceGroupFilter();
                    return;
                }

                // 1. Lấy dịch vụ của tất cả mẫu đang hiển thị trong 1 lần gọi.
                CommonParam param = new CommonParam();
                LisSampleServiceViewFilter filter = new LisSampleServiceViewFilter();
                filter.SAMPLE_IDs = samples.Select(o => o.ID).Distinct().ToList();
                List<V_LIS_SAMPLE_SERVICE> sampleServices = new BackendAdapter(param)
                    .Get<List<V_LIS_SAMPLE_SERVICE>>(
                        "api/LisSampleService/GetView", ApiConsumers.LisConsumer, filter, param);
                if (sampleServices == null)
                {
                    sampleServices = new List<V_LIS_SAMPLE_SERVICE>();
                }

                // 2. SERVICE_CODE -> V_HIS_SERVICE (cache RAM, lookup O(1)).
                Dictionary<string, V_HIS_SERVICE> serviceByCode = new Dictionary<string, V_HIS_SERVICE>();
                Dictionary<long, V_HIS_SERVICE> serviceById = new Dictionary<long, V_HIS_SERVICE>();
                foreach (var sv in BackendDataWorker.Get<V_HIS_SERVICE>())
                {
                    if (sv == null) continue;
                    if (!string.IsNullOrEmpty(sv.SERVICE_CODE) && !serviceByCode.ContainsKey(sv.SERVICE_CODE))
                    {
                        serviceByCode.Add(sv.SERVICE_CODE, sv);
                    }
                    if (!serviceById.ContainsKey(sv.ID))
                    {
                        serviceById.Add(sv.ID, sv);
                    }
                }

                // 3. SAMPLE_ID -> tập key nhóm; đồng thời gom danh sách nhóm xuất hiện.
                Dictionary<long, string> groupNameByKey = new Dictionary<long, string>();
                bool hasKhac = false;
                foreach (var ss in sampleServices)
                {
                    if (ss == null || ss.SAMPLE_ID == 0) continue;

                    long groupKey = SERVICE_GROUP_KHAC_KEY;
                    V_HIS_SERVICE service = null;
                    if (!string.IsNullOrEmpty(ss.SERVICE_CODE))
                    {
                        serviceByCode.TryGetValue(ss.SERVICE_CODE, out service);
                    }
                    if (service != null && service.PARENT_ID.HasValue)
                    {
                        groupKey = service.PARENT_ID.Value;
                        if (!groupNameByKey.ContainsKey(groupKey))
                        {
                            V_HIS_SERVICE parent = null;
                            serviceById.TryGetValue(groupKey, out parent);
                            groupNameByKey[groupKey] = parent != null ? parent.SERVICE_NAME : ss.SERVICE_NAME;
                        }
                    }
                    else
                    {
                        hasKhac = true;
                    }

                    HashSet<long> keys;
                    if (!_groupKeysBySampleId.TryGetValue(ss.SAMPLE_ID, out keys))
                    {
                        keys = new HashSet<long>();
                        _groupKeysBySampleId.Add(ss.SAMPLE_ID, keys);
                    }
                    keys.Add(groupKey);
                    _availableServiceGroupKeys.Add(groupKey);
                }

                // 4. Sắp xếp nhóm cha theo tên, "Khác" xuống cuối.
                List<KeyValuePair<long, string>> orderedGroups = groupNameByKey
                    .OrderBy(o => o.Value)
                    .Select(o => new KeyValuePair<long, string>(o.Key, o.Value))
                    .ToList();
                if (hasKhac)
                {
                    orderedGroups.Add(new KeyValuePair<long, string>(
                        SERVICE_GROUP_KHAC_KEY, Resources.ResourceMessage.NhomXNKhac));
                }

                // 5. Giữ lại các nhóm đang chọn còn tồn tại sau reload.
                _selectedServiceGroupKeys = new HashSet<long>(
                    _selectedServiceGroupKeys.Where(k => _availableServiceGroupKeys.Contains(k)));

                RebuildServiceGroupItems(orderedGroups);
                ApplyServiceGroupFilter();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Dựng lại các mục checkbox trong dropdown, giữ trạng thái tích theo _selectedServiceGroupKeys.</summary>
        private void RebuildServiceGroupItems(List<KeyValuePair<long, string>> orderedGroups)
        {
            try
            {
                _isLoadingServiceGroup = true;
                cboServiceGroup.Properties.Items.BeginUpdate();
                cboServiceGroup.Properties.Items.Clear();
                foreach (var g in orderedGroups)
                {
                    CheckState state = _selectedServiceGroupKeys.Contains(g.Key)
                        ? CheckState.Checked : CheckState.Unchecked;
                    cboServiceGroup.Properties.Items.Add(new CheckedListBoxItem(g.Key, g.Value, state));
                }
                cboServiceGroup.Properties.Items.EndUpdate();
                // Cập nhật lại text hiển thị theo các mục đang tích.
                cboServiceGroup.RefreshEditValue();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            finally
            {
                _isLoadingServiceGroup = false;
            }
        }

        /// <summary>
        /// Áp dụng filter OR lên gridControlSample.
        /// Bỏ tích tất cả / Tất cả đều tích -> hiển thị toàn bộ mẫu.
        /// </summary>
        private void ApplyServiceGroupFilter()
        {
            try
            {
                List<LisSampleADO> samples = lstSampleAll;
                if (samples == null)
                {
                    return;
                }

                bool noneSelected = _selectedServiceGroupKeys.Count == 0;
                bool allSelected = _availableServiceGroupKeys.Count > 0
                    && _selectedServiceGroupKeys.Count >= _availableServiceGroupKeys.Count
                    && _availableServiceGroupKeys.All(k => _selectedServiceGroupKeys.Contains(k));

                List<LisSampleADO> dataSource;
                if (noneSelected || allSelected)
                {
                    dataSource = samples;
                }
                else
                {
                    dataSource = samples.Where(s =>
                    {
                        HashSet<long> keys;
                        return _groupKeysBySampleId.TryGetValue(s.ID, out keys)
                            && keys.Any(k => _selectedServiceGroupKeys.Contains(k));
                    }).ToList();
                }

                gridViewSample.BeginUpdate();
                try
                {
                    gridControlSample.DataSource = dataSource;
                }
                finally
                {
                    gridViewSample.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>User tích/bỏ tích nhóm XN -> cập nhật danh sách chọn và lọc lại.</summary>
        private void cboServiceGroup_EditValueChanged(object sender, EventArgs e)
        {
            if (_isLoadingServiceGroup) return;
            try
            {
                _selectedServiceGroupKeys = new HashSet<long>();
                foreach (CheckedListBoxItem item in cboServiceGroup.Properties.Items)
                {
                    if (item.CheckState == CheckState.Checked && item.Value != null)
                    {
                        _selectedServiceGroupKeys.Add(Convert.ToInt64(item.Value));
                    }
                }
                ApplyServiceGroupFilter();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion
    }
}

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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.Utility;
using HIS.Desktop.Plugins.BrowseExportTicket.ADO;
using HIS.Desktop.Plugins.BrowseExportTicket.Resources;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.BrowseExportTicket
{
    public partial class frmBrowseExportTicket : HIS.Desktop.Utility.FormBase
    {
        private void btnTestServiceReq_Click(object sender, EventArgs e)
        {
            try
            {
                OpenAttachTestServicePopup(true, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnAttachTestService_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                VHisBloodADO blood = gridViewExMestBlood.GetFocusedRow() as VHisBloodADO;
                if (blood == null)
                {
                    return;
                }
                OpenAttachTestServicePopup(false, blood);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridControlExpMestBlood_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button != MouseButtons.Right)
                {
                    return;
                }
                DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo hitInfo = gridViewExMestBlood.CalcHitInfo(e.Location);
                if (!hitInfo.InRow || hitInfo.RowHandle < 0)
                {
                    return;
                }
                gridViewExMestBlood.FocusedRowHandle = hitInfo.RowHandle;
                popupMenuBlood.ShowPopup(barManager1, gridControlExpMestBlood.PointToScreen(e.Location));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Mở popup đính kèm dịch vụ xét nghiệm.
        /// isGeneral = true: từ nút (chọn y lệnh chung — duyệt từng loại máu).
        /// isGeneral = false: từ chuột phải (chọn từng dịch vụ — theo túi máu đang chọn).
        /// </summary>
        private void OpenAttachTestServicePopup(bool isGeneral, VHisBloodADO targetBlood)
        {
            try
            {
                if (ChmsExpMest == null || !ChmsExpMest.TDL_TREATMENT_ID.HasValue || ChmsExpMest.TDL_TREATMENT_ID.Value <= 0)
                {
                    return;
                }

                if (dicBloodAdo == null || dicBloodAdo.Count == 0)
                {
                    MessageManager.Show(ResourceMessage.ChuaCoTuiMauDeDinhKemDichVuXetNghiem);
                    return;
                }

                if (!isGeneral && targetBlood == null)
                {
                    return;
                }

                List<long> preCheckedSereServIds = new List<long>();
                if (isGeneral)
                {
                    foreach (var item in dicBloodAdo.Select(o => o.Value))
                    {
                        if (item.AttachSereServIds != null && item.AttachSereServIds.Count > 0)
                        {
                            preCheckedSereServIds.AddRange(item.AttachSereServIds);
                        }
                    }
                }
                else if (targetBlood.AttachSereServIds != null && targetBlood.AttachSereServIds.Count > 0)
                {
                    preCheckedSereServIds.AddRange(targetBlood.AttachSereServIds);
                }
                preCheckedSereServIds = preCheckedSereServIds.Distinct().ToList();

                using (frmAttachTestService frm = new frmAttachTestService(ChmsExpMest.TDL_TREATMENT_ID.Value, preCheckedSereServIds))
                {
                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        ProcessAttachTestServices(frm.SelectedTestServices, isGeneral, targetBlood);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Gom nhóm dịch vụ xét nghiệm đi kèm theo thiết lập HIS_SERVICE_FOLLOW.
        /// - Chọn y lệnh chung (isGeneral): duyệt từng loại máu; không thiết lập -> bỏ qua; có thiết lập -> chỉ gắn DV đi kèm.
        /// - Chọn từng dịch vụ (isGeneral=false): theo DV túi máu đang chọn; không thiết lập -> gắn toàn bộ; có thiết lập -> chỉ gắn DV đi kèm.
        /// </summary>
        private void ProcessAttachTestServices(List<AttachTestServiceADO> checkedTests, bool isGeneral, VHisBloodADO targetBlood)
        {
            try
            {
                if (checkedTests == null)
                {
                    checkedTests = new List<AttachTestServiceADO>();
                }

                List<VHisBloodADO> bloodBags;
                if (isGeneral)
                {
                    bloodBags = dicBloodAdo.Select(o => o.Value).ToList();
                }
                else
                {
                    bloodBags = targetBlood != null ? new List<VHisBloodADO> { targetBlood } : new List<VHisBloodADO>();
                }

                if (bloodBags.Count == 0)
                {
                    return;
                }

                List<long> bloodServiceIds = bloodBags.Select(o => o.SERVICE_ID).Distinct().ToList();
                Dictionary<long, HashSet<long>> followByService = LoadServiceFollow(bloodServiceIds);

                foreach (var blood in bloodBags)
                {
                    HashSet<long> followSet = null;
                    bool hasFollow = followByService.TryGetValue(blood.SERVICE_ID, out followSet) && followSet != null && followSet.Count > 0;

                    List<AttachTestServiceADO> toAttach;
                    if (hasFollow)
                    {
                        toAttach = checkedTests.Where(o => followSet.Contains(o.SERVICE_ID)).ToList();
                    }
                    else if (isGeneral)
                    {
                        continue;
                    }
                    else
                    {
                        toAttach = checkedTests;
                    }

                    // Thay thế (không cộng dồn) để bỏ tích = gỡ đính kèm của túi máu này
                    List<AttachTestServiceADO> distinctAttach = toAttach.GroupBy(o => o.SERE_SERV_ID).Select(g => g.First()).ToList();
                    blood.AttachSereServs = distinctAttach;
                    blood.AttachSereServIds = distinctAttach.Select(o => o.SERE_SERV_ID).ToList();
                    blood.AttachTestServiceNames = String.Join(", ", distinctAttach.Select(o => o.TDL_SERVICE_NAME));
                }

                FillDataToGridExpMestBlood();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Lấy thiết lập dịch vụ đi kèm: SERVICE_ID (dịch vụ máu) -> tập FOLLOW_ID (dịch vụ xét nghiệm đi kèm).
        /// </summary>
        private Dictionary<long, HashSet<long>> LoadServiceFollow(List<long> serviceIds)
        {
            Dictionary<long, HashSet<long>> result = new Dictionary<long, HashSet<long>>();
            try
            {
                if (serviceIds == null || serviceIds.Count == 0)
                {
                    return result;
                }

                HisServiceFollowViewFilter filter = new HisServiceFollowViewFilter();
                filter.SERVICE_IDs = serviceIds;
                var follows = new BackendAdapter(new CommonParam()).Get<List<V_HIS_SERVICE_FOLLOW>>("api/HisServiceFollow/GetView", ApiConsumers.MosConsumer, filter, null);

                if (follows != null && follows.Count > 0)
                {
                    foreach (var item in follows)
                    {
                        if (item.IS_ACTIVE != IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                        {
                            continue;
                        }
                        HashSet<long> set;
                        if (!result.TryGetValue(item.SERVICE_ID, out set))
                        {
                            set = new HashSet<long>();
                            result[item.SERVICE_ID] = set;
                        }
                        set.Add(item.FOLLOW_ID);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}

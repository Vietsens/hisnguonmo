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
using DevExpress.XtraEditors;
using DevExpress.XtraTab;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.BedRoomPartial.ADO;
using HIS.Desktop.Plugins.BedRoomPartial.Base;
using HIS.Desktop.Utility;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.BedRoomPartial
{
    /// <summary>
    /// Task 57971 - actions of the CLS group tabs: edit / delete confirmation, flags for the "Kết quả CLS" screen,
    /// "Thu gọn", anticipate columns, catalog lookup and dispose (tab building: UCBedRoomPartial__Pluss__ClsTab.cs).
    /// </summary>
    public partial class UCBedRoomPartial : UserControlBase
    {
        private void EditFromClsTab(string groupCode, SereServADO data)
        {
            try
            {
                if (this.ConfirmClsTabWholeRequest(groupCode, data))
                    this.Edit_Click(data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void DeleteFromClsTab(string groupCode, SereServADO data)
        {
            try
            {
                if (this.ConfirmClsTabWholeRequest(groupCode, data))
                    this.Delete_Click(data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Edit / Delete always act on the whole service request. From a group tab, when the request also has services
        /// of other groups (e.g. X-ray + CT in one diagnostic imaging request), ask before going on.
        /// The check fails closed: an unexpected error stops the action instead of skipping the question.
        /// </summary>
        private bool ConfirmClsTabWholeRequest(string groupCode, SereServADO data)
        {
            bool result = true;
            try
            {
                HashSet<string> groups;
                if (data == null || !data.SERVICE_REQ_ID.HasValue || this.clsGroupsByServiceReq == null
                    || !this.clsGroupsByServiceReq.TryGetValue(data.SERVICE_REQ_ID.Value, out groups))
                    return true;

                List<string> otherGroupNames = this.clsTabPages
                    .Where(o => o.GroupCode != groupCode && groups.Contains(o.GroupCode))
                    .Select(o => o.FullName)
                    .ToList();
                if (otherGroupNames.Count == 0)
                    return true;

                string serviceReqCode = !String.IsNullOrEmpty(data.SERVICE_REQ_CODE) ? data.SERVICE_REQ_CODE : data.SERVICE_CODE;
                string message = String.Format(Resources.ResourceMessage.PhieuConDichVuThuocNhomKhac, serviceReqCode, String.Join(", ", otherGroupNames));
                result = XtraMessageBox.Show(message, Resources.ResourceMessage.ThongBao, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>
        /// Flags added to the "Kết quả CLS" call: show the result tabs and open the tab of the group the user is on.
        /// </summary>
        private List<string> GetClsTabFlagsForSubclinical()
        {
            List<string> result = new List<string>();
            try
            {
                if (!this.IsClsTabEnabled)
                    return result;
                result.Add(ClsTabGroupStore.ARG__CLS_TAB);
                XtraTabPage selected = this.xtraTabControl1.SelectedTabPage;
                ClsTabPage tab = selected != null ? this.clsTabPages.FirstOrDefault(o => o.Page == selected) : null;
                if (tab != null)
                    result.Add(ClsTabGroupStore.ARG__CLS_TAB_INIT_PREFIX + tab.GroupCode);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>
        /// "Thu gọn / Chi tiết" also applies to the group tabs.
        /// </summary>
        private void ExpandClsTabTrees(bool isExpand)
        {
            try
            {
                foreach (ClsTabPage tab in this.clsTabPages)
                {
                    tab.TreeUc.Expand(isExpand);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetClsTabAnticipateColumnsVisible()
        {
            try
            {
                foreach (ClsTabPage tab in this.clsTabPages)
                {
                    tab.TreeUc.SetAnticipateColumnsVisible(this.isShowAnticipateByUseDate);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Diagnostic imaging services of the catalog by ID and by code (only these need the DIIM type).
        /// Rebuilt only when the cached catalog list is replaced.
        /// </summary>
        private void EnsureClsServiceLookup()
        {
            try
            {
                List<V_HIS_SERVICE> services = BackendDataWorker.Get<V_HIS_SERVICE>();
                if (services != null && Object.ReferenceEquals(services, this.clsServiceSource) && this.clsServiceById != null)
                    return;

                this.clsServiceSource = services;
                this.clsServiceById = new Dictionary<long, V_HIS_SERVICE>();
                this.clsServiceByCode = new Dictionary<string, V_HIS_SERVICE>();
                if (services == null)
                    return;
                foreach (V_HIS_SERVICE service in services)
                {
                    if (service == null || service.SERVICE_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA)
                        continue;
                    if (!this.clsServiceById.ContainsKey(service.ID))
                        this.clsServiceById[service.ID] = service;
                    if (!String.IsNullOrEmpty(service.SERVICE_CODE) && !this.clsServiceByCode.ContainsKey(service.SERVICE_CODE))
                        this.clsServiceByCode[service.SERVICE_CODE] = service;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Releases the group tabs (called from ProcessDisposeModuleDataAfterClose).
        /// </summary>
        private void DisposeClsTabs()
        {
            try
            {
                if (this.xtraTabControl1 != null)
                {
                    this.xtraTabControl1.SelectedPageChanged -= new TabPageChangedEventHandler(this.xtraTabControl1_SelectedPageChanged);
                    this.xtraTabControl1.MouseDown -= new MouseEventHandler(this.xtraTabControl1_MouseDown);
                }
                foreach (ClsTabPage tab in this.clsTabPages)
                {
                    try
                    {
                        if (tab.TreeUc != null)
                            tab.TreeUc.DisposeData();
                    }
                    catch (Exception exTab)
                    {
                        Inventec.Common.Logging.LogSystem.Warn(exTab);
                    }
                    tab.TreeUc = null;
                    tab.Page = null;
                }
                this.clsTabPages = new List<ClsTabPage>();
                this.clsGroupsByServiceReq = null;
                this.clsServiceById = null;
                this.clsServiceByCode = null;
                this.clsServiceSource = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

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
using ACS.EFMODEL.DataModels;
using DevExpress.XtraTreeList.Nodes;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.TrackingCreate.ADO;
using HIS.Desktop.Plugins.TrackingCreate.Base;
using HIS.Desktop.Plugins.TrackingCreate.Resources;
using HIS.Desktop.Utility;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TrackingCreate
{
    /// <summary>
    /// Task 59656: a treatment sheet only records orders requested by its creator
    /// (key HIS.Desktop.Plugins.Tracking.AssignToOwnTrackingOption). Orders already attached to this sheet are kept
    /// whoever requested them (the backend treats the sent list as the final list). Key off: nothing here runs.
    /// </summary>
    public partial class frmTrackingCreateNew : FormBase
    {
        /// <summary>Value of HIS.Desktop.Plugins.Tracking.AssignToOwnTrackingOption (read with the other keys in Load).</summary>
        private string trackingOwnerOptionCFG;

        private EnumTrackingOwnerOption GetTrackingOwnerOption()
        {
            return TrackingOwnerChecker.ParseOption(this.trackingOwnerOptionCFG);
        }

        /// <summary>Owner of the sheet: creator of the saved sheet being edited, else the logged-in user (new sheet).</summary>
        private string GetTrackingOwnerLoginName()
        {
            if (this.currentTracking != null && !String.IsNullOrWhiteSpace(this.currentTracking.CREATOR))
                return this.currentTracking.CREATOR;
            return this.loginName;
        }

        /// <summary>"loginname - username" for messages.</summary>
        private string GetUserDisplayForTrackingOwner(string loginName)
        {
            string result = loginName ?? "";
            try
            {
                if (!String.IsNullOrWhiteSpace(loginName))
                {
                    var acsUser = BackendDataWorker.Get<ACS_USER>().FirstOrDefault(o => TrackingOwnerChecker.IsOwner(o.LOGINNAME, loginName));
                    if (acsUser != null && !String.IsNullOrWhiteSpace(acsUser.USERNAME))
                        result = acsUser.LOGINNAME + " - " + acsUser.USERNAME;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>Called after the order tree is built: flag nodes of orders not attached to this sheet and requested by another user.</summary>
        private void MarkRequestOfOtherUser(List<TreeSereServADO> nodes)
        {
            try
            {
                if (nodes == null || this.GetTrackingOwnerOption() == EnumTrackingOwnerOption.None || this.rsServiceReq == null)
                    return;
                string owner = this.GetTrackingOwnerLoginName();
                long currentTrackingId = this.currentTracking != null ? this.currentTracking.ID : 0;
                Dictionary<long, HIS_SERVICE_REQ> dicServiceReq = new Dictionary<long, HIS_SERVICE_REQ>();
                foreach (var serviceReq in this.rsServiceReq)
                {
                    if (serviceReq != null && !dicServiceReq.ContainsKey(serviceReq.ID))
                        dicServiceReq.Add(serviceReq.ID, serviceReq);
                }

                int count = 0;
                foreach (var node in nodes)
                {
                    HIS_SERVICE_REQ serviceReq = null;
                    long serviceReqId = node.SERVICE_REQ_ID ?? 0;
                    if (serviceReqId > 0 && dicServiceReq.TryGetValue(serviceReqId, out serviceReq))
                    {
                        bool isAttached = currentTrackingId > 0 && serviceReq.TRACKING_ID == currentTrackingId;
                        node.IsRequestOfOtherUser = !isAttached && !TrackingOwnerChecker.IsOwner(serviceReq.REQUEST_LOGINNAME, owner);
                        if (node.IsRequestOfOtherUser)
                            count++;
                    }
                }
                Inventec.Common.Logging.LogSystem.Info("MarkRequestOfOtherUser___owner=" + owner + "; trackingId=" + currentTrackingId + "; nodes=" + count);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Automatic checking (load, New, "Chi lay YL tu BB") never pre-checks orders of other users.</summary>
        private void UncheckRequestOfOtherUserNodes(TreeListNodes treeListNodes)
        {
            try
            {
                if (treeListNodes == null || this.GetTrackingOwnerOption() == EnumTrackingOwnerOption.None)
                    return;
                foreach (TreeListNode node in treeListNodes)
                {
                    var data = this.treeListServiceReq.GetDataRecordByNode(node) as TreeSereServADO;
                    if (data != null && data.IsRequestOfOtherUser)
                    {
                        node.UncheckAll();
                        this.CheckNodesParent(node);
                    }
                    else if (node.HasChildren)
                    {
                        this.UncheckRequestOfOtherUserNodes(node.Nodes);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>Block mode: orders of other users cannot be checked by hand either.</summary>
        private bool IsTrackingOwnerBlockedNode(TreeSereServADO data)
        {
            return data != null && data.IsRequestOfOtherUser && this.GetTrackingOwnerOption() == EnumTrackingOwnerOption.Block;
        }

        /// <summary>Before calling api/HisTracking/Create|Update. Returns false when saving must stop.</summary>
        private bool CheckTrackingOwnerBeforeSave()
        {
            bool result = true;
            try
            {
                EnumTrackingOwnerOption option = this.GetTrackingOwnerOption();
                if (option == EnumTrackingOwnerOption.None)
                    return true;

                List<TreeSereServADO> checkeds = this.GetListCheck();
                List<long> otherIds = checkeds.Where(o => o.IsRequestOfOtherUser && (o.SERVICE_REQ_ID ?? 0) > 0)
                    .Select(o => o.SERVICE_REQ_ID.Value).Distinct().ToList();
                if (otherIds.Count == 0)
                    return true;

                string owner = this.GetTrackingOwnerLoginName();
                List<string> lines = new List<string>();
                foreach (long serviceReqId in otherIds)
                {
                    var serviceReq = this.rsServiceReq != null ? this.rsServiceReq.FirstOrDefault(o => o.ID == serviceReqId) : null;
                    string code = serviceReq != null ? serviceReq.SERVICE_REQ_CODE : serviceReqId.ToString();
                    string requestUser = serviceReq != null
                        ? (String.IsNullOrWhiteSpace(serviceReq.REQUEST_USERNAME) ? serviceReq.REQUEST_LOGINNAME : serviceReq.REQUEST_LOGINNAME + " - " + serviceReq.REQUEST_USERNAME)
                        : "";
                    lines.Add(String.Format(ResourceMessage.TrackingOwner__YLenhNguoiKhacChiDinh, code, requestUser));
                }

                bool isBlock = (option == EnumTrackingOwnerOption.Block);
                string ownerDisplay = this.GetUserDisplayForTrackingOwner(owner);
                string footer = String.Format(isBlock ? ResourceMessage.TrackingOwner__ChanLuuToDieuTri : ResourceMessage.TrackingOwner__CanhBaoLuuToDieuTri, ownerDisplay);
                string message = TrackingOwnerChecker.JoinLines(lines, footer);
                Inventec.Common.Logging.LogSystem.Info("CheckTrackingOwnerBeforeSave___option=" + option + "; owner=" + owner + "; otherServiceReqIds=" + String.Join(",", otherIds));

                WaitingManager.Hide();
                if (isBlock)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(message, ResourceMessage.ThongBao, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                result = DevExpress.XtraEditors.XtraMessageBox.Show(message, ResourceMessage.ThongBao, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
                if (result)
                    WaitingManager.Show();
            }
            catch (Exception ex)
            {
                //Fail-open: an unexpected error here must not stop saving the treatment sheet
                result = true;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}

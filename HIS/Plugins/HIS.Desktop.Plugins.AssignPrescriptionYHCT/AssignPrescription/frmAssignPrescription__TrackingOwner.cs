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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.Base;
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.Config;
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.Resources;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignPrescriptionYHCT.AssignPrescription
{
    /// <summary>
    /// Task 59656: orders of the ordering user may only be attached to treatment sheets created by that user
    /// (key HIS.Desktop.Plugins.Tracking.AssignToOwnTrackingOption). Key off: nothing here changes the behavior.
    /// </summary>
    public partial class frmAssignPrescription : HIS.Desktop.Utility.FormBase
    {
        private EnumTrackingOwnerOption GetTrackingOwnerOption()
        {
            return TrackingOwnerChecker.ParseOption(HisConfigCFG.AssignToOwnTrackingOption);
        }

        /// <summary>Ordering user resolved like SaveAbstract fills RequestLoginname (cboUser, else the backend uses the logged-in user).</summary>
        private string GetRequestLoginNameForTrackingOwner()
        {
            string result = null;
            try
            {
                if (this.cboUser.EditValue != null)
                    result = this.cboUser.EditValue.ToString();
                if (String.IsNullOrWhiteSpace(result))
                    result = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
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

        private static TrackingOwnerChecker.Item ToTrackingOwnerItem(HIS_TRACKING tracking)
        {
            return new TrackingOwnerChecker.Item() { Id = tracking.ID, TrackingTime = tracking.TRACKING_TIME, Creator = tracking.CREATOR };
        }

        /// <summary>Opened by the treatment sheet screen while its sheet is not saved yet (CreateOption = 2): the orders
        /// are attached when that sheet is saved, so "no own sheet" must not be reported here.</summary>
        private bool IsCalledFromUnsavedTrackingCreate()
        {
            return TrackingOwnerChecker.IsCalledFromTrackingCreate(this.processDataResult) && this.tracking == null;
        }

        /// <summary>Check run before saving. Returns false when saving must stop.</summary>
        private bool CheckTrackingOwnerBeforeSave()
        {
            bool result = true;
            try
            {
                EnumTrackingOwnerOption option = this.GetTrackingOwnerOption();
                if (option == EnumTrackingOwnerOption.None)
                    return true;
                //Several patients in one save: sheets are chosen per patient by the default rule, nothing to check here
                if (this.bIsSelectMultiPatientProcessing)
                    return true;
                //Sheet combo hidden (prescription without treatment sheet) or disabled
                if (this.lciPhieuDieuTri.Visibility != DevExpress.XtraLayout.Utils.LayoutVisibility.Always || !this.cboPhieuDieuTri.Enabled)
                    return true;

                string requestLoginName = this.GetRequestLoginNameForTrackingOwner();
                List<TrackingOwnerChecker.Item> selecteds = new List<TrackingOwnerChecker.Item>();
                if (this.cboPhieuDieuTri.EditValue != null)
                {
                    long trackingId = Inventec.Common.TypeConvert.Parse.ToInt64(this.cboPhieuDieuTri.EditValue.ToString());
                    var tracking = this.trackingADOs != null ? this.trackingADOs.FirstOrDefault(o => o.ID == trackingId) : null;
                    if (tracking == null)
                    {
                        Inventec.Common.Logging.LogSystem.Warn("CheckTrackingOwnerBeforeSave: khong tim thay to dieu tri dang chon trong danh sach, bo qua kiem tra___trackingId=" + trackingId);
                        return true;
                    }
                    selecteds.Add(ToTrackingOwnerItem(tracking));
                }

                //Edit: keep the sheet the order already had (data before the key was turned on), check only a new sheet
                bool isEdit = this.oldServiceReq != null && this.oldServiceReq.ID > 0;
                if (isEdit && this.oldServiceReq.TRACKING_ID.HasValue)
                    selecteds.RemoveAll(o => o.Id == this.oldServiceReq.TRACKING_ID.Value);

                List<long> instructionTimes = (this.ucDateProcessor != null && this.ucDate != null) ? this.ucDateProcessor.GetValue(this.ucDate) : null;
                if (instructionTimes == null || instructionTimes.Count == 0)
                    instructionTimes = this.intructionTimeSelecteds;
                List<TrackingOwnerChecker.Item> availables = this.trackingADOs != null ? this.trackingADOs.Select(o => ToTrackingOwnerItem(o)).ToList() : new List<TrackingOwnerChecker.Item>();
                bool isCheckMissing = !isEdit && !this.IsCalledFromUnsavedTrackingCreate();
                var check = TrackingOwnerChecker.Check(requestLoginName, instructionTimes, selecteds, availables, true, isCheckMissing);
                Inventec.Common.Logging.LogSystem.Info("CheckTrackingOwnerBeforeSave___option=" + option + "; requestLoginName=" + requestLoginName
                    + "; selected=" + String.Join(",", selecteds.Select(o => o.Id + ":" + o.Creator))
                    + "; otherOwner=" + check.OtherOwnerTrackings.Count + "; missingDates=" + String.Join(",", check.MissingDates)
                    + "; isEdit=" + isEdit + "; isCheckMissing=" + isCheckMissing);
                if (!check.HasViolation)
                    return true;

                result = this.ShowTrackingOwnerMessage(check, option, requestLoginName);
            }
            catch (Exception ex)
            {
                //Fail-open: an unexpected error here must not stop the prescription
                result = true;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        /// <summary>Block: show and return false. Warning: ask, return true when the user chooses Yes.</summary>
        private bool ShowTrackingOwnerMessage(TrackingOwnerChecker.Result check, EnumTrackingOwnerOption option, string requestLoginName)
        {
            bool isBlock = (option == EnumTrackingOwnerOption.Block);
            string message = TrackingOwnerChecker.BuildMessage(check, this.GetUserDisplayForTrackingOwner(requestLoginName), this.GetUserDisplayForTrackingOwner,
                ResourceMessage.TrackingOwner__ToDieuTriCuaNguoiKhac, ResourceMessage.TrackingOwner__NguoiChiDinhChuaCoToDieuTri,
                isBlock ? ResourceMessage.TrackingOwner__ChanLuu : ResourceMessage.TrackingOwner__CanhBaoTiepTuc);
            if (isBlock)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show(message,
                    HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return DevExpress.XtraEditors.XtraMessageBox.Show(message,
                HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao),
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }
    }
}

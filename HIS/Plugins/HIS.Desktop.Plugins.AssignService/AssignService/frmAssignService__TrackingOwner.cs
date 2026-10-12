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
using HIS.Desktop.Plugins.AssignService.ADO;
using HIS.Desktop.Plugins.AssignService.Base;
using HIS.Desktop.Plugins.AssignService.Config;
using HIS.Desktop.Plugins.AssignService.Resources;
using HIS.Desktop.Utilities.Extensions;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignService.AssignService
{
    /// <summary>
    /// Task 59656: orders of the ordering user may only be attached to treatment sheets created by that user
    /// (key HIS.Desktop.Plugins.Tracking.AssignToOwnTrackingOption). Key off: nothing here runs.
    /// </summary>
    public partial class frmAssignService : HIS.Desktop.Utility.FormBase
    {
        private EnumTrackingOwnerOption GetTrackingOwnerOption()
        {
            return TrackingOwnerChecker.ParseOption(HisConfigCFG.AssignToOwnTrackingOption);
        }

        /// <summary>Ordering user resolved exactly like ProcessServiceReqSDO fills RequestLoginName.</summary>
        private string GetRequestLoginNameForTrackingOwner()
        {
            string result = null;
            try
            {
                if (this.cboUser.EditValue != null)
                {
                    string value = this.cboUser.EditValue.ToString();
                    var acsUser = BackendDataWorker.Get<ACS_USER>().FirstOrDefault(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && o.LOGINNAME.Equals(value));
                    if (acsUser != null)
                        result = acsUser.LOGINNAME;
                }
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
            return TrackingOwnerChecker.IsCalledFromTrackingCreate(this.processDataResult)
                && (this.workingAssignServiceADO == null || this.workingAssignServiceADO.Tracking == null);
        }

        /// <summary>Check run before saving (single patient). Returns false when saving must stop.</summary>
        private bool CheckTrackingOwnerBeforeSave()
        {
            bool result = true;
            try
            {
                EnumTrackingOwnerOption option = this.GetTrackingOwnerOption();
                if (option == EnumTrackingOwnerOption.None)
                    return true;
                //Combo disabled by EnableCboTracking: this treatment type does not use treatment sheets
                if (!this.cboTracking.Enabled)
                    return true;

                string requestLoginName = this.GetRequestLoginNameForTrackingOwner();
                List<TrackingOwnerChecker.Item> selecteds = new List<TrackingOwnerChecker.Item>();
                bool isOneTrackingForAllTimes = true;
                //Same source as ProcessServiceReqSDO: multi-selection (one sheet per date) first, then the single value
                GridCheckMarksSelection gridCheckMark = this.cboTracking.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null && gridCheckMark.SelectedCount > 0)
                {
                    isOneTrackingForAllTimes = false;
                    foreach (TrackingAdo rv in gridCheckMark.Selection)
                    {
                        if (rv != null)
                            selecteds.Add(ToTrackingOwnerItem(rv));
                    }
                }
                else if (this.cboTracking.EditValue != null && !String.IsNullOrEmpty(this.cboTracking.EditValue.ToString()))
                {
                    long trackingId = Inventec.Common.TypeConvert.Parse.ToInt64(this.cboTracking.EditValue.ToString());
                    var tracking = this.trackingAdos != null ? this.trackingAdos.FirstOrDefault(o => o.ID == trackingId) : null;
                    if (tracking == null)
                    {
                        Inventec.Common.Logging.LogSystem.Warn("CheckTrackingOwnerBeforeSave: khong tim thay to dieu tri dang chon trong danh sach, bo qua kiem tra___trackingId=" + trackingId);
                        return true;
                    }
                    selecteds.Add(ToTrackingOwnerItem(tracking));
                }

                List<TrackingOwnerChecker.Item> availables = this.trackingAdos != null ? this.trackingAdos.Select(o => ToTrackingOwnerItem(o)).ToList() : new List<TrackingOwnerChecker.Item>();
                bool isCheckMissing = !this.IsCalledFromUnsavedTrackingCreate();
                var check = TrackingOwnerChecker.Check(requestLoginName, this.intructionTimeSelecteds, selecteds, availables, isOneTrackingForAllTimes, isCheckMissing);
                Inventec.Common.Logging.LogSystem.Info("CheckTrackingOwnerBeforeSave___option=" + option + "; requestLoginName=" + requestLoginName
                    + "; selected=" + String.Join(",", selecteds.Select(o => o.Id + ":" + o.Creator))
                    + "; otherOwner=" + check.OtherOwnerTrackings.Count + "; missingDates=" + String.Join(",", check.MissingDates)
                    + "; isCheckMissing=" + isCheckMissing);
                if (!check.HasViolation)
                    return true;

                result = this.ShowTrackingOwnerMessage(check, option, requestLoginName);
            }
            catch (Exception ex)
            {
                //Fail-open: an unexpected error here must not stop the ordering
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

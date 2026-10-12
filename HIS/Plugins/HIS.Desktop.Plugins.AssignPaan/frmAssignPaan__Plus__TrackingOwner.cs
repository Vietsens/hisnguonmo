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
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.Plugins.AssignPaan.ADO;
using HIS.Desktop.Plugins.AssignPaan.Base;
using HIS.Desktop.Plugins.AssignPaan.Config;
using HIS.Desktop.Plugins.AssignPaan.Resources;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignPaan
{
    /// <summary>
    /// Task 59656: orders of the ordering user may only be attached to treatment sheets created by that user
    /// (key HIS.Desktop.Plugins.Tracking.AssignToOwnTrackingOption). Key off: nothing here changes the behavior.
    /// </summary>
    public partial class frmAssignPaan : HIS.Desktop.Utility.FormBase
    {
        private EnumTrackingOwnerOption GetTrackingOwnerOption()
        {
            return TrackingOwnerChecker.ParseOption(HisConfigs.Get<string>(TrackingOwnerChecker.CONFIG_KEY));
        }

        /// <summary>Ordering user resolved like ProcessSave fills RequestLoginName (cboUsername when ShowRequestUser = 1,
        /// otherwise the backend uses the logged-in user).</summary>
        private string GetRequestLoginNameForTrackingOwner()
        {
            string result = null;
            try
            {
                if (AppConfig.ShowRequestUser == "1" && this.cboUsername.EditValue != null)
                {
                    long userId = Inventec.Common.TypeConvert.Parse.ToInt64(this.cboUsername.EditValue.ToString());
                    var user = BackendDataWorker.Get<ACS_USER>().FirstOrDefault(o => o.ID == userId);
                    if (user != null)
                        result = user.LOGINNAME;
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

        private static TrackingOwnerChecker.Item ToTrackingOwnerItem(V_HIS_TRACKING tracking)
        {
            return new TrackingOwnerChecker.Item() { Id = tracking.ID, TrackingTime = tracking.TRACKING_TIME, Creator = tracking.CREATOR };
        }

        private long GetInstructionTimeForTrackingOwner()
        {
            if (this.dtInstructionTime.EditValue == null || this.dtInstructionTime.DateTime == DateTime.MinValue)
                return 0;
            return Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(this.dtInstructionTime.DateTime) ?? 0;
        }

        /// <summary>Default sheet when the key is on: the sheet passed by the caller if the ordering user created it,
        /// otherwise the latest sheet of the ordering user on the instruction date in the working department, otherwise none.</summary>
        private void SetDefaultOwnTrackingCombo(List<TrackingAdo> trackingAdoList)
        {
            try
            {
                string owner = this.GetRequestLoginNameForTrackingOwner();
                if (this.currentTracking != null && TrackingOwnerChecker.IsOwner(this.currentTracking.CREATOR, owner))
                {
                    this.cboTracking.EditValue = this.currentTracking.ID;
                    return;
                }
                long departmentId = HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetWorkPlace(this.currentModule).DepartmentId;
                var departmentTrackings = trackingAdoList != null ? trackingAdoList.Where(o => o.DEPARTMENT_ID == departmentId).Select(o => ToTrackingOwnerItem(o)) : null;
                var own = TrackingOwnerChecker.GetLatestOwnTracking(departmentTrackings, this.GetInstructionTimeForTrackingOwner(), owner);
                this.cboTracking.EditValue = own != null ? (object)own.Id : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Opened by the treatment sheet screen while its sheet is not saved yet (CreateOption = 2): the order
        /// is attached when that sheet is saved, so "no own sheet" must not be reported here.</summary>
        private bool IsCalledFromUnsavedTrackingCreate()
        {
            return TrackingOwnerChecker.IsCalledFromTrackingCreate(this.delegateActionSave) && this.currentTracking == null;
        }

        /// <summary>"No own sheet" is only reported for treatment types that use treatment sheets (in-patient, out-patient treatment).</summary>
        private bool IsTreatmentTypeUsingTracking()
        {
            return this.currentPatientTypeAlter != null
                && (this.currentPatientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU
                    || this.currentPatientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU);
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
                if (!this.cboTracking.Enabled)
                    return true;

                string requestLoginName = this.GetRequestLoginNameForTrackingOwner();
                List<TrackingOwnerChecker.Item> selecteds = new List<TrackingOwnerChecker.Item>();
                if (this.cboTracking.EditValue != null)
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

                List<long> instructionTimes = new List<long>() { this.GetInstructionTimeForTrackingOwner() };
                List<TrackingOwnerChecker.Item> availables = this.trackingAdos != null ? this.trackingAdos.Select(o => ToTrackingOwnerItem(o)).ToList() : new List<TrackingOwnerChecker.Item>();
                bool isCheckMissing = this.IsTreatmentTypeUsingTracking() && !this.IsCalledFromUnsavedTrackingCreate();
                var check = TrackingOwnerChecker.Check(requestLoginName, instructionTimes, selecteds, availables, true, isCheckMissing);
                Inventec.Common.Logging.LogSystem.Info("CheckTrackingOwnerBeforeSave___option=" + option + "; requestLoginName=" + requestLoginName
                    + "; selected=" + String.Join(",", selecteds.Select(o => o.Id + ":" + o.Creator))
                    + "; otherOwner=" + check.OtherOwnerTrackings.Count + "; missingDates=" + String.Join(",", check.MissingDates)
                    + "; isCheckMissing=" + isCheckMissing);
                if (!check.HasViolation)
                    return true;

                bool isBlock = (option == EnumTrackingOwnerOption.Block);
                string message = TrackingOwnerChecker.BuildMessage(check, this.GetUserDisplayForTrackingOwner(requestLoginName), this.GetUserDisplayForTrackingOwner,
                    ResourceMessageLang.TrackingOwner__ToDieuTriCuaNguoiKhac, ResourceMessageLang.TrackingOwner__NguoiChiDinhChuaCoToDieuTri,
                    isBlock ? ResourceMessageLang.TrackingOwner__ChanLuu : ResourceMessageLang.TrackingOwner__CanhBaoTiepTuc);
                if (isBlock)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(message,
                        HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao),
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                result = DevExpress.XtraEditors.XtraMessageBox.Show(message,
                    HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
            }
            catch (Exception ex)
            {
                //Fail-open: an unexpected error here must not stop the order
                result = true;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}

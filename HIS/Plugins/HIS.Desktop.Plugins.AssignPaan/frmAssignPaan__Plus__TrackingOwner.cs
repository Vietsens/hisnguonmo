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
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.AssignPaan
{
    /// <summary>
    /// Task 59656 (revised 14/10/2026 after QA): key HIS.Desktop.Plugins.Tracking.AssignToOwnTrackingOption = 1 makes the
    /// "Treatment sheet" combo show only sheets created by the ordering user and preselect that user's latest sheet of the
    /// instruction date; changing the ordering user filters and selects again. Key off: nothing here changes the behavior.
    /// </summary>
    public partial class frmAssignPaan : HIS.Desktop.Utility.FormBase
    {
        /// <summary>Ordering user the treatment sheet combo is currently filtered for (key on), null when not filtered.</summary>
        private string trackingOwnerLoginName;

        private EnumTrackingOwnerOption GetTrackingOwnerOption()
        {
            EnumTrackingOwnerOption result = EnumTrackingOwnerOption.None;
            try
            {
                result = TrackingOwnerChecker.ParseOption(HisConfigs.Get<string>(TrackingOwnerChecker.CONFIG_KEY));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
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

        /// <summary>Data source of the treatment sheet combo: key on -> only the sheets created by the ordering user,
        /// otherwise the full list unchanged. Remembers the ordering user the combo is filtered for.</summary>
        private List<TrackingAdo> GetTrackingDataSourceByOwner(List<TrackingAdo> trackingAdoList)
        {
            List<TrackingAdo> result = trackingAdoList;
            try
            {
                this.trackingOwnerLoginName = null;
                if (this.GetTrackingOwnerOption() == EnumTrackingOwnerOption.OwnOnly)
                {
                    this.trackingOwnerLoginName = this.GetRequestLoginNameForTrackingOwner();
                    result = TrackingOwnerChecker.FilterOwn(trackingAdoList, o => o.CREATOR, this.trackingOwnerLoginName);
                }
            }
            catch (Exception ex)
            {
                result = trackingAdoList;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
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

        /// <summary>Ordering user changed (key on): filter the already loaded full sheet list (trackingAdos, no API call)
        /// again for the new ordering user and select that user's latest sheet of the instruction date (empty if none).</summary>
        private void ReloadTrackingComboByOwner()
        {
            try
            {
                if (this.GetTrackingOwnerOption() != EnumTrackingOwnerOption.OwnOnly)
                    return;
                //Sheets not loaded yet: LoadDataToTrackingCombo filters them itself when it runs
                if (this.trackingAdos == null)
                    return;
                string owner = this.GetRequestLoginNameForTrackingOwner();
                //Same ordering user as the current filter (e.g. default user set while loading the form): keep list and selection
                if (TrackingOwnerChecker.IsOwner(owner, this.trackingOwnerLoginName))
                    return;

                this.cboTracking.Properties.DataSource = this.GetTrackingDataSourceByOwner(this.trackingAdos);
                this.SetDefaultOwnTrackingCombo(this.trackingAdos);
                Inventec.Common.Logging.LogSystem.Debug("ReloadTrackingComboByOwner___owner=" + owner + "; trackingId=" + this.cboTracking.EditValue);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

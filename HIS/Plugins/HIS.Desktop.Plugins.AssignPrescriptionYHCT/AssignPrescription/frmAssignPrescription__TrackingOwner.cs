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
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.ADO;
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.Base;
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.Config;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.AssignPrescriptionYHCT.AssignPrescription
{
    /// <summary>
    /// Task 59656 (revised 14/10/2026): key HIS.Desktop.Plugins.Tracking.AssignToOwnTrackingOption = 1 makes the "Treatment sheet"
    /// combo show only the sheets created by the ordering user (cboUser) and preselect that user's latest sheet of the instruction
    /// date; changing the ordering user filters and preselects again. trackingADOs keeps the full list loaded by LoadDataTracking,
    /// only the combo data source is filtered. Editing an order keeps the order's sheet in the combo even if another user created it.
    /// Key empty/other: nothing here changes the behavior.
    /// </summary>
    public partial class frmAssignPrescription : HIS.Desktop.Utility.FormBase
    {
        private EnumTrackingOwnerOption GetTrackingOwnerOption()
        {
            EnumTrackingOwnerOption result = EnumTrackingOwnerOption.None;
            try
            {
                result = TrackingOwnerChecker.ParseOption(HisConfigCFG.AssignToOwnTrackingOption);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
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

        /// <summary>Editing an order: the sheet the order is attached to, null otherwise.</summary>
        private long? GetEditTrackingIdForTrackingOwner()
        {
            long? result = null;
            try
            {
                if (this.actionType == GlobalVariables.ActionEdit && this.oldServiceReq != null)
                    result = this.oldServiceReq.TRACKING_ID;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>Data source of the treatment sheet combo. Key 1: sheets created by the ordering user plus the sheet of the
        /// edited order. Key off: the full list (same object as before).</summary>
        private List<TrackingADO> GetTrackingADOsForCombo()
        {
            List<TrackingADO> result = this.trackingADOs;
            try
            {
                if (this.trackingADOs == null || this.GetTrackingOwnerOption() != EnumTrackingOwnerOption.OwnOnly)
                    return result;

                string requestLoginName = this.GetRequestLoginNameForTrackingOwner();
                long? editTrackingId = this.GetEditTrackingIdForTrackingOwner();
                List<TrackingADO> ownTrackings = TrackingOwnerChecker.FilterOwn(this.trackingADOs, o => o.CREATOR, requestLoginName);
                if (editTrackingId.HasValue && !ownTrackings.Any(o => o.ID == editTrackingId.Value))
                {
                    TrackingADO editTracking = this.trackingADOs.FirstOrDefault(o => o != null && o.ID == editTrackingId.Value);
                    if (editTracking != null)
                    {
                        ownTrackings.Add(editTracking);
                        ownTrackings = ownTrackings.OrderByDescending(o => o.TRACKING_TIME).ToList();
                    }
                }
                Inventec.Common.Logging.LogSystem.Debug("GetTrackingADOsForCombo___requestLoginName=" + requestLoginName + "; all=" + this.trackingADOs.Count
                    + "; shown=" + ownTrackings.Count + "; editTrackingId=" + editTrackingId);
                result = ownTrackings;
            }
            catch (Exception ex)
            {
                //Fail-open: show the full list
                result = this.trackingADOs;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>Called when the ordering user changes (cboUser, txtLoginName). Key 1, after the form is loaded: filter the combo
        /// for the new ordering user (reusing the loaded list, no API call) and preselect that user's sheet again.</summary>
        private void ReloadTrackingComboByRequestUser()
        {
            try
            {
                if (this.isNotLoadWhileChangeInstructionTimeInFirst
                    || this.GetTrackingOwnerOption() != EnumTrackingOwnerOption.OwnOnly
                    || this.trackingADOs == null
                    || this.lciPhieuDieuTri.Visibility != DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                    return;

                string requestLoginName = this.GetRequestLoginNameForTrackingOwner();
                long? selectedTrackingId = this.GetDefaultTrackingIdForTrackingOwner(this.GetTrackingADOsForCombo(), requestLoginName);
                this.InitComboTracking(this.cboPhieuDieuTri);
                this.cboPhieuDieuTri.EditValue = selectedTrackingId;
                this.cboPhieuDieuTri.Properties.Buttons[1].Visible = (this.cboPhieuDieuTri.EditValue != null);
                Inventec.Common.Logging.LogSystem.Info("ReloadTrackingComboByRequestUser___requestLoginName=" + requestLoginName + "; selectedTrackingId=" + selectedTrackingId);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Sheet preselected for the ordering user (same rule as LoadDataTracking), null = none. Several instruction dates:
        /// none. Editing an order: keep the selected sheet while it is still in the combo (the order's sheet always is). Sheet passed in
        /// by the treatment sheet screen: kept when the ordering user created it. Otherwise the ordering user's latest sheet of the
        /// instruction date in the current department.</summary>
        private long? GetDefaultTrackingIdForTrackingOwner(List<TrackingADO> trackingForCombo, string requestLoginName)
        {
            long? result = null;
            try
            {
                if (trackingForCombo == null || trackingForCombo.Count == 0)
                    return null;
                if (this.ucDateProcessor.GetChkMultiDateState(this.ucDate))
                    return null;

                if (this.actionType == GlobalVariables.ActionEdit && this.cboPhieuDieuTri.EditValue != null)
                {
                    long currentTrackingId = Inventec.Common.TypeConvert.Parse.ToInt64(this.cboPhieuDieuTri.EditValue.ToString());
                    if (trackingForCombo.Any(o => o.ID == currentTrackingId))
                        return currentTrackingId;
                }

                if (this.tracking != null && TrackingOwnerChecker.IsOwner(this.tracking.CREATOR, requestLoginName)
                    && trackingForCombo.Any(o => o.ID == this.tracking.ID))
                    return this.tracking.ID;

                List<long> instructionTimes = this.ucDateProcessor.GetValue(this.ucDate);
                long instructionTime = (instructionTimes != null && instructionTimes.Count > 0) ? instructionTimes.Max() : 0;
                long departmentId = HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetDepartmentId(this.currentModule.RoomTypeId);
                TrackingOwnerChecker.Item latest = TrackingOwnerChecker.GetLatestOwnTracking(
                    trackingForCombo.Where(o => o.DEPARTMENT_ID == departmentId)
                        .Select(o => new TrackingOwnerChecker.Item() { Id = o.ID, TrackingTime = o.TRACKING_TIME, Creator = o.CREATOR }),
                    instructionTime, requestLoginName);
                if (latest != null)
                    result = latest.Id;
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
    }
}

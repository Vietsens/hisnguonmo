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
using HIS.Desktop.Plugins.AssignPrescriptionKidney.ADO;
using HIS.Desktop.Plugins.AssignPrescriptionKidney.Base;
using HIS.Desktop.Plugins.AssignPrescriptionKidney.Config;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.AssignPrescriptionKidney.AssignPrescription
{
    /// <summary>
    /// Task 59656: key HIS.Desktop.Plugins.Tracking.AssignToOwnTrackingOption = 1 - the "Treatment sheet" combo only shows
    /// sheets created by the ordering user (the sheet of the prescription being edited is kept) and the latest own sheet of
    /// the instruction date is selected; changing the ordering user filters and selects again. Key off: nothing here runs.
    /// </summary>
    public partial class frmAssignPrescription : HIS.Desktop.Utility.FormBase
    {
        /// <summary>All treatment sheets loaded by LoadDataTracking (unfiltered), so a change of the ordering user only
        /// filters again without calling the API. Only filled when the key is on.</summary>
        private List<TrackingADO> trackingADOsAll;

        /// <summary>Ordering user the sheet combo is currently filtered for.</summary>
        private string trackingOwnerFilteredLoginName;

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

        /// <summary>Ordering user resolved exactly like SaveAbstract fills RequestLoginname (txtLoginName, else the logged-in user).</summary>
        private string GetRequestLoginNameForTrackingOwner()
        {
            string result = null;
            try
            {
                result = this.txtLoginName.Text;
                if (String.IsNullOrEmpty(result))
                    result = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>Sheet of the prescription being edited (kept in the list and selected even if another user created it), null otherwise.</summary>
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

        /// <summary>Sheets of the ordering user, plus the sheet of the prescription being edited. Newest first like the full list.</summary>
        private List<TrackingADO> FilterTrackingByOwner(List<TrackingADO> allTrackings, string requestLoginName)
        {
            List<TrackingADO> result = allTrackings;
            try
            {
                if (allTrackings == null)
                    return null;
                result = TrackingOwnerChecker.FilterOwn(allTrackings, o => o.CREATOR, requestLoginName);
                long? editTrackingId = this.GetEditTrackingIdForTrackingOwner();
                if (editTrackingId.HasValue && !result.Any(o => o.ID == editTrackingId.Value))
                {
                    var editTracking = allTrackings.FirstOrDefault(o => o.ID == editTrackingId.Value);
                    if (editTracking != null)
                    {
                        result.Add(editTracking);
                        result = result.OrderByDescending(o => o.TRACKING_TIME).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                result = allTrackings;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>Latest sheet of the ordering user on the instruction date in the working department (same scope as the
        /// IsDefaultTracking default), null if none or several instruction dates are chosen.</summary>
        private TrackingADO GetLatestOwnTrackingOfInstructionDate(string requestLoginName)
        {
            TrackingADO result = null;
            try
            {
                if (this.trackingADOsAll == null || this.ucDateProcessor.GetChkMultiDateState(this.ucDate))
                    return null;
                List<long> instructionTimes = this.ucDateProcessor.GetValue(this.ucDate);
                if (instructionTimes == null || !instructionTimes.Any())
                    return null;

                long departmentId = WorkPlace.GetDepartmentId(this.currentModule.RoomTypeId);
                var items = this.trackingADOsAll
                    .Where(o => o.DEPARTMENT_ID == departmentId)
                    .Select(o => new TrackingOwnerChecker.Item() { Id = o.ID, TrackingTime = o.TRACKING_TIME, Creator = o.CREATOR });
                var latest = TrackingOwnerChecker.GetLatestOwnTracking(items, instructionTimes.Max(), requestLoginName);
                if (latest != null)
                    result = this.trackingADOsAll.FirstOrDefault(o => o.ID == latest.Id);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>Key 1: keep only the sheets of the ordering user in trackingADOs and choose the sheet to select.
        /// Edit: the current sheet stays selected when it is still listed (the sheet of the order always is), no auto select.
        /// Otherwise: the latest own sheet of the instruction date, empty if none. The caller binds the combo afterwards.</summary>
        private void ApplyTrackingOwnerFilter(string requestLoginName)
        {
            try
            {
                if (this.trackingADOsAll == null)
                    return;

                this.trackingOwnerFilteredLoginName = requestLoginName;
                this.trackingADOs = this.FilterTrackingByOwner(this.trackingADOsAll, requestLoginName);

                long? selectedTrackingId = null;
                if (this.actionType == GlobalVariables.ActionEdit)
                {
                    long currentTrackingId = this.cboPhieuDieuTri.EditValue != null ? Inventec.Common.TypeConvert.Parse.ToInt64(this.cboPhieuDieuTri.EditValue.ToString()) : 0;
                    long? editTrackingId = this.GetEditTrackingIdForTrackingOwner();
                    if (currentTrackingId > 0
                        && (this.trackingADOs.Any(o => o.ID == currentTrackingId) || (editTrackingId.HasValue && editTrackingId.Value == currentTrackingId)))
                        selectedTrackingId = currentTrackingId;
                }
                else
                {
                    var latestOwnTracking = this.GetLatestOwnTrackingOfInstructionDate(requestLoginName);
                    if (latestOwnTracking != null)
                        selectedTrackingId = latestOwnTracking.ID;
                }

                this.cboPhieuDieuTri.EditValue = selectedTrackingId;
                this.cboPhieuDieuTri.Properties.Buttons[1].Visible = selectedTrackingId.HasValue;
                Inventec.Common.Logging.LogSystem.Info("ApplyTrackingOwnerFilter___requestLoginName=" + requestLoginName + "; actionType=" + this.actionType
                    + "; all=" + this.trackingADOsAll.Count + "; own=" + this.trackingADOs.Count + "; selectedTrackingId=" + selectedTrackingId);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>The user changed the ordering user (cboUser / txtLoginName). Key 1: filter the already loaded sheets again
        /// for the new user and select his latest sheet of the instruction date, without calling the API.</summary>
        private void ReloadTrackingByRequestUser(string requestLoginName)
        {
            try
            {
                if (this.GetTrackingOwnerOption() != EnumTrackingOwnerOption.OwnOnly)
                    return;
                if (String.IsNullOrWhiteSpace(requestLoginName) || this.trackingADOsAll == null)
                    return;
                if (TrackingOwnerChecker.IsOwner(this.trackingOwnerFilteredLoginName, requestLoginName))
                    return;

                this.ApplyTrackingOwnerFilter(requestLoginName);
                this.InitComboTracking(this.cboPhieuDieuTri);
                this.cboPhieuDieuTri.Properties.Buttons[1].Visible = (this.cboPhieuDieuTri.EditValue != null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

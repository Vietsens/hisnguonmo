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
using HIS.Desktop.Plugins.AssignPrescriptionPK.ADO;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Base;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Config;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.AssignPrescriptionPK.AssignPrescription
{
    /// <summary>
    /// Task 59656 (revised 14/10/2026): key HIS.Desktop.Plugins.Tracking.AssignToOwnTrackingOption = 1 makes the "Treatment sheet"
    /// combo show only the sheets created by the ordering user and select that user's latest sheet of the instruction date.
    /// Editing an order keeps its current sheet. Nothing is checked when saving. Key off: nothing here changes the behavior.
    /// </summary>
    public partial class frmAssignPrescription : HIS.Desktop.Utility.FormBase
    {
        /// <summary>Ordering user the treatment sheet combo was last filtered for (key 1), so only a real change of the ordering user re-filters it.</summary>
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

        /// <summary>Editing an order: its current treatment sheet stays in the combo even if another user created it.</summary>
        private bool IsTrackingKeptForEdit(long trackingId)
        {
            bool result = false;
            try
            {
                result = this.actionType == GlobalVariables.ActionEdit
                    && this.oldServiceReq != null
                    && this.oldServiceReq.TRACKING_ID.HasValue
                    && this.oldServiceReq.TRACKING_ID.Value == trackingId;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>
        /// Sheets bound to the treatment sheet combo. Key off: all loaded sheets (this.trackingADOs, behavior unchanged).
        /// Key 1: only the sheets created by the ordering user, plus the sheet of the order being edited.
        /// this.trackingADOs always keeps the full list, so the combo can be filtered again without calling the API.
        /// </summary>
        private List<TrackingADO> GetTrackingADOsForCombo()
        {
            List<TrackingADO> result = this.trackingADOs;
            try
            {
                if (this.trackingADOs == null || this.GetTrackingOwnerOption() != EnumTrackingOwnerOption.OwnOnly)
                    return result;

                string requestLoginName = this.GetRequestLoginNameForTrackingOwner();
                result = TrackingOwnerChecker.FilterOwn(this.trackingADOs, o => o.CREATOR, requestLoginName);
                TrackingADO keptForEdit = this.trackingADOs.FirstOrDefault(o => o != null && this.IsTrackingKeptForEdit(o.ID));
                if (keptForEdit != null && !result.Contains(keptForEdit))
                {
                    result.Add(keptForEdit);
                    result = result.OrderByDescending(o => o.TRACKING_TIME).ToList();
                }
                this.trackingOwnerFilteredLoginName = requestLoginName;
                Inventec.Common.Logging.LogSystem.Debug("GetTrackingADOsForCombo: chi hien to dieu tri cua nguoi chi dinh___requestLoginName=" + requestLoginName
                    + "; all=" + this.trackingADOs.Count + "; shown=" + result.Count + "; keptForEdit=" + (keptForEdit != null ? keptForEdit.ID.ToString() : ""));
            }
            catch (Exception ex)
            {
                result = this.trackingADOs;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>
        /// Key 1: the ordering user was changed (cboUser / txtLoginName) after the sheets were loaded. Filters the combo again for the
        /// new user from the loaded sheets (no API call) and selects that user's latest sheet of the instruction date (empty if none).
        /// Editing keeps the order's sheet. Several instruction dates: InitComboTracking ticks the user's latest sheet of each date.
        /// </summary>
        private void ReloadTrackingComboByRequestUser()
        {
            try
            {
                if (this.GetTrackingOwnerOption() != EnumTrackingOwnerOption.OwnOnly)
                    return;
                //Still loading (LoadDataTracking filters with the ordering user itself) or no sheet loaded
                if (this.isInitTracking || this.trackingADOs == null || this.trackingADOs.Count == 0)
                    return;

                string requestLoginName = this.GetRequestLoginNameForTrackingOwner();
                //A change is real once txtLoginName and cboUser show the same account (typing in txtLoginName is not a change yet)
                string userComboLoginName = this.cboUser.EditValue != null ? this.cboUser.EditValue.ToString() : null;
                if (!TrackingOwnerChecker.IsOwner(userComboLoginName, requestLoginName)
                    || TrackingOwnerChecker.IsOwner(this.trackingOwnerFilteredLoginName, requestLoginName))
                    return;

                Inventec.Common.Logging.LogSystem.Info("ReloadTrackingComboByRequestUser: doi nguoi chi dinh, loc lai to dieu tri___from=" + this.trackingOwnerFilteredLoginName
                    + "; to=" + requestLoginName + "; isMultiDate=" + this.chkMultiIntructionTime.Checked + "; actionType=" + this.actionType);
                long? currentTrackingId = (this.cboPhieuDieuTri.EditValue != null) ? (long?)Inventec.Common.TypeConvert.Parse.ToInt64(this.cboPhieuDieuTri.EditValue.ToString()) : null;
                this.InitComboTracking(this.cboPhieuDieuTri);
                if (this.chkMultiIntructionTime.Checked || !this.cboPhieuDieuTri.Enabled)
                    return;

                this.cboPhieuDieuTri.EditValue = this.GetTrackingIdToSelectForRequestUser(this.GetTrackingADOsForCombo(), currentTrackingId, requestLoginName);
                this.cboPhieuDieuTri.Properties.Buttons[1].Visible = (this.cboPhieuDieuTri.EditValue != null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Sheet to select after the ordering user changed (one instruction date). New order: the user's latest sheet of the instruction
        /// date, null if none. Editing: the current sheet while it is still shown, else the order's own sheet, else null.
        /// </summary>
        private long? GetTrackingIdToSelectForRequestUser(List<TrackingADO> shownTrackings, long? currentTrackingId, string requestLoginName)
        {
            long? result = null;
            try
            {
                if (shownTrackings == null || shownTrackings.Count == 0)
                    return null;

                if (this.actionType == GlobalVariables.ActionEdit)
                {
                    if (currentTrackingId.HasValue && shownTrackings.Exists(o => o != null && o.ID == currentTrackingId.Value))
                        return currentTrackingId;
                    TrackingADO keptForEdit = shownTrackings.FirstOrDefault(o => o != null && this.IsTrackingKeptForEdit(o.ID));
                    return keptForEdit != null ? (long?)keptForEdit.ID : null;
                }

                List<TrackingOwnerChecker.Item> items = shownTrackings.Where(o => o != null)
                    .Select(o => new TrackingOwnerChecker.Item() { Id = o.ID, TrackingTime = o.TRACKING_TIME, Creator = o.CREATOR }).ToList();
                TrackingOwnerChecker.Item latest = null;
                if (this.intructionTimeSelecteds != null)
                {
                    foreach (long instructionTime in this.intructionTimeSelecteds)
                    {
                        TrackingOwnerChecker.Item item = TrackingOwnerChecker.GetLatestOwnTracking(items, instructionTime, requestLoginName);
                        if (item != null && (latest == null || item.TrackingTime > latest.TrackingTime))
                            latest = item;
                    }
                }
                result = (latest != null) ? (long?)latest.Id : null;
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

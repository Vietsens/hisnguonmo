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
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignService.ADO;
using HIS.Desktop.Plugins.AssignService.Base;
using HIS.Desktop.Plugins.AssignService.Config;
using HIS.Desktop.Utilities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.AssignService.AssignService
{
    /// <summary>
    /// Task 59656 (revised 14/10/2026 after QA): key HIS.Desktop.Plugins.Tracking.AssignToOwnTrackingOption = 1 makes the
    /// "Treatment sheet" combo list only the sheets created by the ordering user and preselect that user's latest sheet of
    /// the instruction date (FillDataToTrackingCombo); changing the ordering user re-filters the sheets already loaded.
    /// Nothing is checked when saving. Key off: nothing here runs, the combo behaves as before.
    /// </summary>
    public partial class frmAssignService : HIS.Desktop.Utility.FormBase
    {
        /// <summary>Ordering user the treatment sheet combo was last filtered for (key on); null when the list is not filtered.</summary>
        string trackingOwnerFilterLoginName;

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

        /// <summary>
        /// Sheets created by the ordering user, loaded order (newest first) kept. keepTrackingIds (sheets attached to the saved
        /// order being edited) stay listed even when created by someone else. Unexpected error: the list is returned unfiltered.
        /// </summary>
        private List<TrackingAdo> FilterTrackingAdosByOwner(List<TrackingAdo> trackingAdos, string requestLoginName, List<long> keepTrackingIds)
        {
            List<TrackingAdo> result = trackingAdos;
            try
            {
                if (keepTrackingIds == null || keepTrackingIds.Count == 0)
                {
                    result = TrackingOwnerChecker.FilterOwn(trackingAdos, o => o.CREATOR, requestLoginName);
                }
                else if (trackingAdos != null)
                {
                    HashSet<long> keepIds = new HashSet<long>(keepTrackingIds);
                    result = trackingAdos.Where(o => o != null && (keepIds.Contains(o.ID) || TrackingOwnerChecker.IsOwner(o.CREATOR, requestLoginName))).ToList();
                }
            }
            catch (Exception ex)
            {
                result = trackingAdos;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>Selected sheets, same source as ProcessServiceReqSDO: check marks (multi-date) first, then the single value.</summary>
        private List<long> GetSelectedTrackingIds()
        {
            List<long> result = new List<long>();
            try
            {
                GridCheckMarksSelection gridCheckMark = this.cboTracking.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null && gridCheckMark.SelectedCount > 0)
                {
                    foreach (TrackingAdo rv in gridCheckMark.Selection)
                    {
                        if (rv != null)
                            result.Add(rv.ID);
                    }
                }
                else if (this.cboTracking.EditValue != null && !String.IsNullOrEmpty(this.cboTracking.EditValue.ToString()))
                {
                    result.Add(Inventec.Common.TypeConvert.Parse.ToInt64(this.cboTracking.EditValue.ToString()));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>
        /// Called after the user changed the ordering user (txtLoginName Enter, cboUser popup / Enter). Key on: re-filter the sheets
        /// already loaded (no API call) for the new ordering user and preselect that user's latest sheet of the instruction date
        /// (none if the user has no sheet). Saved order being edited (ActionEdit): the attached sheet(s) stay listed and selected.
        /// </summary>
        private void ReloadTrackingComboByRequestUser()
        {
            try
            {
                if (this.GetTrackingOwnerOption() != EnumTrackingOwnerOption.OwnOnly)
                    return;
                //Sheets not loaded yet or being (re)loaded (form load, patient change): that load filters for the current ordering user itself
                if (this.trackings == null || this.isInitTracking)
                    return;
                string requestLoginName = this.GetRequestLoginNameForTrackingOwner();
                if (TrackingOwnerChecker.IsOwner(requestLoginName, this.trackingOwnerFilterLoginName))
                    return;

                Inventec.Common.Logging.LogSystem.Info("ReloadTrackingComboByRequestUser___from=" + this.trackingOwnerFilterLoginName + "; to=" + requestLoginName + "; actionType=" + this.actionType);
                if (this.actionType == GlobalVariables.ActionEdit)
                {
                    this.RefilterTrackingComboKeepSelection(requestLoginName);
                }
                else
                {
                    this.FillDataToTrackingCombo();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Saved order being edited (ActionEdit): only the list is re-filtered for the new ordering user; the selected sheet(s)
        /// stay in the list and stay selected, no default selection and no ICD reload.
        /// </summary>
        private void RefilterTrackingComboKeepSelection(string requestLoginName)
        {
            bool isInitTrackingOld = this.isInitTracking;
            try
            {
                List<long> selectedIds = this.GetSelectedTrackingIds();
                List<TrackingAdo> trackingAll = this.trackings.Select(o => new TrackingAdo(o)).ToList();
                List<TrackingAdo> trackingFiltered = this.FilterTrackingAdosByOwner(trackingAll, requestLoginName, selectedIds);
                this.trackingAdos = trackingFiltered;
                this.trackingOwnerFilterLoginName = requestLoginName;

                //Single date: EditValue (id of the attached sheet) is kept and still found in the new list
                this.cboTracking.Properties.DataSource = trackingFiltered;
                GridCheckMarksSelection gridCheckMark = this.cboTracking.Properties.Tag as GridCheckMarksSelection;
                if (this.chkMultiIntructionTime.Checked && gridCheckMark != null)
                {
                    //Check marks hold the row objects of the old list: mark the same sheets again in the new list,
                    //with the same guard as the multi-date load (cboTracking_EditValueChanged skipped)
                    this.isInitTracking = true;
                    HashSet<long> selectedIdSet = new HashSet<long>(selectedIds);
                    gridCheckMark.SelectAll(trackingFiltered.Where(o => selectedIdSet.Contains(o.ID)).ToList());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            finally
            {
                this.isInitTracking = isInitTrackingOld;
            }
        }
    }
}

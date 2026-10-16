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
using HIS.Desktop.Plugins.AssignPrescriptionCLS.ADO;
using HIS.Desktop.Plugins.AssignPrescriptionCLS.Base;
using HIS.Desktop.Plugins.AssignPrescriptionCLS.Config;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.AssignPrescriptionCLS.AssignPrescription
{
    /// <summary>
    /// Task 59656 (revised 14/10/2026 after QA): key HIS.Desktop.Plugins.Tracking.AssignToOwnTrackingOption = 1 makes the
    /// "Treatment sheet" combo list only the sheets created by the ordering user (cboUser/txtLoginName) and select that
    /// user's latest sheet of the instruction date; changing the ordering user filters and selects again.
    /// Edit mode keeps the sheet the order already has (even when created by someone else).
    /// trackingADOs always keeps the full list loaded by InitComboPhieuDieuTri, the filter only changes the combo data source.
    /// Key off: nothing here changes the behavior.
    /// </summary>
    public partial class frmAssignPrescription : HIS.Desktop.Utility.FormBase
    {
        private EnumTrackingOwnerOption GetTrackingOwnerOption()
        {
            try
            {
                return TrackingOwnerChecker.ParseOption(HisConfigCFG.AssignToOwnTrackingOption);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return EnumTrackingOwnerOption.None;
            }
        }

        /// <summary>Key = 1: the sheet combo only lists the ordering user's sheets.</summary>
        private bool IsTrackingOwnOnly()
        {
            try
            {
                return this.GetTrackingOwnerOption() == EnumTrackingOwnerOption.OwnOnly;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return false;
            }
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

        /// <summary>Ordering user the sheets are filtered/selected for. cboUser is read first because its EditValueChanged
        /// fires before txtLoginName is synchronized (txtLoginName Enter, cboUser popup).</summary>
        private string GetOrderingUserForTracking()
        {
            string result = null;
            try
            {
                result = this.cboUser.EditValue != null && !String.IsNullOrWhiteSpace(this.cboUser.EditValue.ToString())
                    ? this.cboUser.EditValue.ToString()
                    : this.GetRequestLoginNameForTrackingOwner();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>Edit mode: sheet of the order being edited, kept in the list even when created by someone else.</summary>
        private long? GetEditOrderTrackingId()
        {
            try
            {
                if (this.actionType == GlobalVariables.ActionEdit && this.oldServiceReq != null)
                    return this.oldServiceReq.TRACKING_ID;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return null;
        }

        /// <summary>Data source of cboPhieuDieuTri: the full list (trackingADOs) when the key is off, otherwise the ordering
        /// user's sheets plus, in edit mode, the sheet of the order being edited (newest first, like trackingADOs).</summary>
        private List<TrackingADO> GetTrackingDataSourceForCombo()
        {
            List<TrackingADO> result = this.trackingADOs;
            try
            {
                if (this.trackingADOs == null || !this.IsTrackingOwnOnly())
                    return this.trackingADOs;

                string orderingUser = this.GetOrderingUserForTracking();
                result = TrackingOwnerChecker.FilterOwn(this.trackingADOs, o => o.CREATOR, orderingUser);
                long? editTrackingId = this.GetEditOrderTrackingId();
                if (editTrackingId.HasValue && !result.Any(o => o.ID == editTrackingId.Value))
                {
                    TrackingADO editTracking = this.trackingADOs.FirstOrDefault(o => o.ID == editTrackingId.Value);
                    if (editTracking != null)
                    {
                        result.Add(editTracking);
                        result = result.OrderByDescending(o => o.TRACKING_TIME).ToList();
                    }
                }
                Inventec.Common.Logging.LogSystem.Debug("GetTrackingDataSourceForCombo___orderingUser=" + orderingUser
                    + "; editTrackingId=" + editTrackingId + "; total=" + this.trackingADOs.Count + "; shown=" + result.Count);
            }
            catch (Exception ex)
            {
                result = this.trackingADOs;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        /// <summary>Key = 1: rebinds the sheet combo for the current ordering user, reusing the loaded list (no API call).
        /// The caller then selects the default sheet (SetDefaultComboPhieuDieuTri).</summary>
        private void ReloadComboPhieuDieuTriByOrderingUser()
        {
            try
            {
                if (this.trackingADOs == null || !this.IsTrackingOwnOnly())
                    return;
                this.cboPhieuDieuTri.Properties.DataSource = this.GetTrackingDataSourceForCombo();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Key = 1, edit mode, ordering user changed: keep the selected sheet when it is still listed, otherwise fall back to the
        /// sheet of the order being edited, so saving the edit does not detach the order from its sheet. Returns false when the
        /// caller must apply the normal default selection (key off or not editing).
        /// </summary>
        private bool KeepEditTrackingSelection()
        {
            try
            {
                if (!this.IsTrackingOwnOnly() || this.actionType != GlobalVariables.ActionEdit)
                    return false;
                List<TrackingADO> listed = this.cboPhieuDieuTri.Properties.DataSource as List<TrackingADO>;
                long? selectedId = this.cboPhieuDieuTri.EditValue != null ? (long?)Inventec.Common.TypeConvert.Parse.ToInt64(this.cboPhieuDieuTri.EditValue.ToString()) : null;
                if (selectedId.HasValue && listed != null && listed.Any(o => o.ID == selectedId.Value))
                    return true;
                long? editTrackingId = this.GetEditOrderTrackingId();
                this.cboPhieuDieuTri.EditValue = (editTrackingId.HasValue && listed != null && listed.Any(o => o.ID == editTrackingId.Value)) ? (object)editTrackingId.Value : null;
                return true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return false;
        }

        /// <summary>Latest sheet of the ordering user on the instruction date, null when none (a sheet of another user is not kept).</summary>
        private long? GetDefaultOwnTrackingId()
        {
            try
            {
                if (this.trackingADOs == null || this.InstructionTime <= 0)
                    return null;
                string orderingUser = this.GetOrderingUserForTracking();
                var own = TrackingOwnerChecker.GetLatestOwnTracking(this.trackingADOs.Select(o => ToTrackingOwnerItem(o)), this.InstructionTime, orderingUser);
                return own != null ? (long?)own.Id : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
        }

        private static TrackingOwnerChecker.Item ToTrackingOwnerItem(HIS_TRACKING tracking)
        {
            return new TrackingOwnerChecker.Item() { Id = tracking.ID, TrackingTime = tracking.TRACKING_TIME, Creator = tracking.CREATOR };
        }
    }
}

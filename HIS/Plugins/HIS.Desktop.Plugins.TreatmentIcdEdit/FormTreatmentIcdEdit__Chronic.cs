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
using HIS.Desktop.LocalStorage.HisConfig;
using Inventec.Desktop.Common.LanguageManager;
using System;

namespace HIS.Desktop.Plugins.TreatmentIcdEdit
{
    /// <summary>
    /// vCong58919 - "Mãn tính" checkbox for the treatment being edited.
    /// Shown only when config of task 54036 (chronic - change treatment type) is on.
    /// Sends HisTreatmentCommonInfoUpdateSDO.IsChronic (bool?) only when the user changed it;
    /// backend writes HIS_TREATMENT.IS_CHRONIC of this treatment only (no patient flag, no treatment type change).
    /// </summary>
    public partial class FormTreatmentIcdEdit : HIS.Desktop.Utility.FormBase
    {
        /// <summary>Config of task 54036, extended by task 58919. "1" = on.</summary>
        private const string CHRONIC_CHANGE_TREATMENT_TYPE_OPTION_KEY = "MOS.HIS_TREATMENT.FINISH.CHRONIC_CHANGE_TREATMENT_TYPE_OPTION";
        private const string CONFIG_VALUE_ON = "1";

        /// <summary>IS_CHRONIC stored as NUMBER: 1 = chronic.</summary>
        private const short CHRONIC_TRUE = 1;

        /// <summary>Chronic flag of the treatment when the form was loaded, to detect user changes.</summary>
        private bool chronicOriginalValue;

        private bool IsChronicByTreatmentOn()
        {
            try
            {
                return HisConfigs.Get<string>(CHRONIC_CHANGE_TREATMENT_TYPE_OPTION_KEY) == CONFIG_VALUE_ON;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return false;
        }

        /// <summary>Shows/hides the chronic checkbox and fills it from the treatment. Called once in Load.</summary>
        private void InitChronicControl()
        {
            try
            {
                bool isOn = IsChronicByTreatmentOn();
                lciChronic.Visibility = isOn ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                if (!isOn)
                    return;

                lciChronic.Text = Inventec.Common.Resource.Get.Value("FormTreatmentIcdEdit.lciChronic.Text", Resources.ResourceLanguageManager.LanguageFormTreatmentIcdEdit, LanguageManager.GetCulture());
                chkChronic.ToolTip = Inventec.Common.Resource.Get.Value("FormTreatmentIcdEdit.chkChronic.ToolTip", Resources.ResourceLanguageManager.LanguageFormTreatmentIcdEdit, LanguageManager.GetCulture());

                this.chronicOriginalValue = this.currentVHisTreatment != null && this.currentVHisTreatment.IS_CHRONIC == CHRONIC_TRUE;
                chkChronic.Checked = this.chronicOriginalValue;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Value for HisTreatmentCommonInfoUpdateSDO.IsChronic: null = no change (config off, or checkbox unchanged).
        /// </summary>
        private bool? GetChronicValueToSave()
        {
            try
            {
                if (lciChronic.Visibility != DevExpress.XtraLayout.Utils.LayoutVisibility.Always || !IsChronicByTreatmentOn())
                    return null;
                if (chkChronic.Checked == this.chronicOriginalValue)
                    return null;
                return chkChronic.Checked;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return null;
        }
    }
}

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
using Inventec.Desktop.Common.LanguageManager;
using MOS.EFMODEL.DataModels;
using System;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisIcd
{
    /// <summary>
    /// 55058 - "Benh man tinh" checkbox (HIS_ICD.IS_CHRONIC).
    /// Every access to IS_CHRONIC goes through a NoInlining accessor that only runs when the
    /// runtime MOS.EFMODEL has the column, so an older EFMODEL just hides the feature
    /// instead of breaking the whole form (MissingMethodException at JIT time).
    /// </summary>
    public partial class frmHisIcd
    {
        private static bool? isChronicFieldSupported;

        /// <summary>
        /// True when the loaded MOS.EFMODEL has HIS_ICD.IS_CHRONIC and V_HIS_ICD.IS_CHRONIC.
        /// </summary>
        private static bool IsChronicFieldSupported
        {
            get
            {
                if (!isChronicFieldSupported.HasValue)
                {
                    try
                    {
                        isChronicFieldSupported = typeof(HIS_ICD).GetProperty("IS_CHRONIC") != null
                            && typeof(V_HIS_ICD).GetProperty("IS_CHRONIC") != null;
                        if (!isChronicFieldSupported.Value)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("MOS.EFMODEL chua co HIS_ICD.IS_CHRONIC/V_HIS_ICD.IS_CHRONIC -> an checkbox Benh man tinh");
                        }
                    }
                    catch (Exception ex)
                    {
                        isChronicFieldSupported = false;
                        Inventec.Common.Logging.LogSystem.Warn(ex);
                    }
                }
                return isChronicFieldSupported.Value;
            }
        }

        /// <summary>
        /// Hide checkbox + grid column when the runtime EFMODEL does not support IS_CHRONIC.
        /// </summary>
        private void InitChronicControl()
        {
            try
            {
                if (!IsChronicFieldSupported)
                {
                    this.layoutControlItem30.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    this.grdColIsChronic.Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetCaptionChronic()
        {
            try
            {
                this.chkIsChronic.Properties.Caption = Inventec.Common.Resource.Get.Value("frmHisIcd.chkIsChronic.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsChronic.ToolTip = Inventec.Common.Resource.Get.Value("frmHisIcd.chkIsChronic.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColIsChronic.Caption = Inventec.Common.Resource.Get.Value("frmHisIcd.grdColIsChronic.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColIsChronic.ToolTip = Inventec.Common.Resource.Get.Value("frmHisIcd.grdColIsChronic.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Value for grid unbound column IS_CHRONIC_CHK.
        /// </summary>
        private bool IsChronicIcd(V_HIS_ICD data)
        {
            try
            {
                return data != null && IsChronicFieldSupported && GetIsChronic(data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return false;
        }

        private void FillChronicToEditor(V_HIS_ICD data)
        {
            try
            {
                chkIsChronic.Checked = IsChronicIcd(data);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateChronicToDTO(HIS_ICD currentDTO)
        {
            try
            {
                if (currentDTO != null && IsChronicFieldSupported)
                {
                    SetIsChronic(currentDTO, chkIsChronic.Checked);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static bool GetIsChronic(V_HIS_ICD data)
        {
            return data.IS_CHRONIC == 1;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void SetIsChronic(HIS_ICD dto, bool isChronic)
        {
            dto.IS_CHRONIC = isChronic ? (short?)1 : null;
        }

        private void chkIsChronic_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Space)
                {
                    chkIsChronic.Checked = !chkIsChronic.Checked;
                }
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.ActionType == GlobalVariables.ActionAdd)
                    {
                        btnAdd.Focus();
                    }
                    else
                    {
                        btnEdit.Focus();
                    }
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

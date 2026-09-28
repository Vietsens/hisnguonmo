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
using System.Reflection;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisIcd
{
    /// <summary>
    /// 55058 - "Benh man tinh" checkbox (HIS_ICD.IS_CHRONIC).
    /// IS_CHRONIC is read/written through cached PropertyInfo so the plugin builds and runs with
    /// both the old MOS.EFMODEL (no column -> checkbox and grid column are hidden) and the new one.
    /// </summary>
    public partial class frmHisIcd
    {
        private const string IS_CHRONIC_PROPERTY = "IS_CHRONIC";

        private static bool isChronicPropertyLoaded;
        private static PropertyInfo icdChronicProperty;
        private static PropertyInfo viewIcdChronicProperty;

        /// <summary>
        /// True when the loaded MOS.EFMODEL has HIS_ICD.IS_CHRONIC and V_HIS_ICD.IS_CHRONIC.
        /// </summary>
        private static bool IsChronicFieldSupported
        {
            get
            {
                if (!isChronicPropertyLoaded)
                {
                    isChronicPropertyLoaded = true;
                    try
                    {
                        icdChronicProperty = typeof(HIS_ICD).GetProperty(IS_CHRONIC_PROPERTY);
                        viewIcdChronicProperty = typeof(V_HIS_ICD).GetProperty(IS_CHRONIC_PROPERTY);
                        if (icdChronicProperty == null || viewIcdChronicProperty == null)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("MOS.EFMODEL chua co HIS_ICD.IS_CHRONIC/V_HIS_ICD.IS_CHRONIC -> an checkbox Benh man tinh (55058)");
                        }
                    }
                    catch (Exception ex)
                    {
                        icdChronicProperty = null;
                        viewIcdChronicProperty = null;
                        Inventec.Common.Logging.LogSystem.Warn(ex);
                    }
                }
                return icdChronicProperty != null && viewIcdChronicProperty != null;
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
        /// V_HIS_ICD.IS_CHRONIC == 1. Used by the grid unbound column and the editor.
        /// </summary>
        private bool IsChronicIcd(V_HIS_ICD data)
        {
            try
            {
                if (data != null && IsChronicFieldSupported)
                {
                    short? value = viewIcdChronicProperty.GetValue(data, null) as short?;
                    return value == 1;
                }
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
                    icdChronicProperty.SetValue(currentDTO, chkIsChronic.Checked ? (short?)1 : null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
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

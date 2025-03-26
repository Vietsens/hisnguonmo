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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.LocalStorage.BackendData;
using DevExpress.XtraEditors;
using Inventec.Common.Controls.PopupLoader;
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.DelegateRegister;
using HIS.Desktop.Utility;
using SDA.EFMODEL.DataModels;
using HIS.UC.PlusInfo.ADO;
using HIS.UC.PlusInfo.Validate;
using DevExpress.XtraEditors.DXErrorProvider;
using Inventec.Desktop.Common.LanguageManager;
using HIS.UC.PlusInfo.ShareMethod;
using System.Resources;

namespace HIS.UC.PlusInfo.Design
{
    public partial class UCCMND : UserControlBase
    {
        #region Declare

        DelegateNextControl dlgFocusNextUserControl;
        public bool val = false;

        #endregion

        #region Contructor - Load

        public UCCMND()
            : base("UCPlusInfo", "UCCMND")
        {
            try
            {
                InitializeComponent();

                this.ValidRelativeCMNDNumberLenght(false);
                this.txtCMND.TabIndex = this.TabIndex;
                //this.SetCaptionByLanguageKey();
                SetCaptionByLanguageKeyNew();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UCCMND_Load(object sender, EventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                this.lciCMND.Text = Inventec.Common.Resource.Get.Value("UCPlusInfo.UCCMND", HIS.UC.PlusInfo.ShareMethod.ResourceLanguageManager.ResourceUCPlusInfo, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện UCCMND
        /// </summary>
        private void SetCaptionByLanguageKeyNew()
        {
            try
            {
                ////Khoi tao doi tuong resource
                ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.UC.PlusInfo.Resources.Lang", typeof(UCCMND).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCCMND.layoutControl1.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciCMND.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCCMND.lciCMND.OptionsToolTip.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciCMND.Text = Inventec.Common.Resource.Get.Value("UCCMND.lciCMND.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        #endregion

        #region Focus

        internal void FocusNextControl(DelegateNextControl _dlgFocusNextControl)
        {
            try
            {
                if (_dlgFocusNextControl != null)
                    this.dlgFocusNextUserControl = _dlgFocusNextControl;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void FocusControl()
        {
            try
            {
                this.txtCMND.Focus();
                this.txtCMND.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Get - Set Value

        internal string GetValue()
        {
            try
            {
                //if (!String.IsNullOrEmpty(this.txtCMND.Text))
                return this.txtCMND.Text;
            }
            catch (Exception ex)
            {
                return null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return null;
        }

        internal void SetValue(string CMNDNumber)
        {
            try
            {
                this.txtCMND.Text = CMNDNumber;
                IList<Control> invalidControls = this.dxValidationProviderCMND.GetInvalidControls();
                for (int i = invalidControls.Count - 1; i >= 0; i--)
                {
                    this.dxValidationProviderCMND.RemoveControlError(invalidControls[i]);
                }
                //this.txtCMND.TabIndex = this.TabIndex;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal void RefreshUserControl(bool loadDataAgain)
        {
            try
            {
                if (loadDataAgain)
                {
                    this.txtCMND.Text = "";
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(this.dxValidationProviderCMND, this.dxErrorProviderControl);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn("Tiep don: UCPlusInfo/UCCommuneNow/RefreshUserControl: \n" + ex);
            }
        }

        #endregion

        #region Validate

        internal bool ValidateRequiredField()
        {
            bool valid = true;
            try
            {
                if (!this.dxValidationProviderCMND.Validate())
                {
                    IList<Control> invalidControls = this.dxValidationProviderCMND.GetInvalidControls();
                    for (int i = invalidControls.Count - 1; i >= 0; i--)
                    {
                        Inventec.Common.Logging.LogSystem.Debug((i == 0 ? "InvalidControls:" : "") + "" + invalidControls[i].Name + ",");
                    }
                    valid = false;
                }
            }
            catch (Exception ex)
            {
                valid = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }

        private void ValidRelativeCMNDNumberLenght(bool IsRequired)
        {
            try
            {
                Validate_CMND_Control oDobDateRule = new Validate_CMND_Control();
                oDobDateRule.txtCMND = this.txtCMND;
                oDobDateRule.IsRequired = IsRequired;
                oDobDateRule.ErrorType = ErrorType.Warning;
                this.dxValidationProviderCMND.SetValidationRule(this.txtCMND, oDobDateRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        internal void ReloadValidFeild(bool IsReload)
        {

            try
            {
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(this.dxValidationProviderCMND, this.dxErrorProviderControl);
                if (IsReload)
                {
                    lciCMND.AppearanceItemCaption.ForeColor = Color.Maroon;
                }
                else
                {
                    lciCMND.AppearanceItemCaption.ForeColor = Color.Black;
                }
                ValidRelativeCMNDNumberLenght(IsReload);
            }

            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        #endregion

        private void txtCMND_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (this.dlgFocusNextUserControl != null)
                        this.dlgFocusNextUserControl(this.TabIndex, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtCMND_Validated(object sender, EventArgs e)
        {
            try
            {
                bool validPatientInfo = dxValidationProviderCMND.Validate();
                this.val = validPatientInfo;
                if (!validPatientInfo)
                {
                    IList<Control> invalidControls = dxValidationProviderCMND.GetInvalidControls();
                    for (int i = invalidControls.Count - 1; i >= 0; i--)
                    {
                        Inventec.Common.Logging.LogSystem.Info((i == 0 ? "InvalidControls:" : "") + "" + invalidControls[i].Name + ",");
                    }
                    return;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        internal void DisposeControl()
        {
            try
            {
                val = false;
                dlgFocusNextUserControl = null;
                this.txtCMND.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.txtCMND_KeyDown);
                this.txtCMND.Validated -= new System.EventHandler(this.txtCMND_Validated);
                this.Load -= new System.EventHandler(this.UCCMND_Load);
                dxErrorProviderControl = null;
                dxValidationProviderCMND = null;
                lciCMND = null;
                txtCMND = null;
                layoutControlGroup1 = null;
                layoutControl1 = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}

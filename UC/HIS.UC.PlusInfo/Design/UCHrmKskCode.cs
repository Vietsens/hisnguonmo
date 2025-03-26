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
using Inventec.Core;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using Inventec.Desktop.Common.LanguageManager;
using HIS.UC.PlusInfo.ShareMethod;
using System.Resources;

namespace HIS.UC.PlusInfo.Design
{
    public partial class UCHrmKskCode : UserControlBase
    {
        #region Declare

        DelegateNextControl dlgFocusNextUserControl;
        int positionHandleControl = -1;
        bool isValid;
        bool isVisible;
        public bool val = true;

        #endregion

        #region Contructor - Load

        public UCHrmKskCode()
            : base("UCPlusInfo", "UCHrmKskCode")
        {
            try
            {
                InitializeComponent();

                this.txtHrmKskCode.TabIndex = this.TabIndex;
                this.ValidHrmKskCode();
                SetCaptionByLanguageKeyNew();
                //this.SetCaptionByLanguageKey();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UCHrmKskCode_Load(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện UCHrmKskCode
        /// </summary>
        private void SetCaptionByLanguageKeyNew()
        {
            try
            {
                ////Khoi tao doi tuong resource
                ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.UC.PlusInfo.Resources.Lang", typeof(UCHrmKskCode).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCHrmKskCode.layoutControl1.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciHrmKskCode.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCHrmKskCode.lciHrmKskCode.OptionsToolTip.ToolTip", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciHrmKskCode.Text = Inventec.Common.Resource.Get.Value("UCHrmKskCode.lciHrmKskCode.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
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
                this.lciHrmKskCode.Text = Inventec.Common.Resource.Get.Value("UCPlusInfo.UCHrmKskCode", HIS.UC.PlusInfo.ShareMethod.ResourceLanguageManager.ResourceUCPlusInfo, LanguageManager.GetCulture());
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

        #endregion

        #region Event Control

        private void txtHrmKskCode_KeyDown(object sender, KeyEventArgs e)
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

        #endregion

        #region Validate

        private void ValidHrmKskCode()
        {
            try
            {
                if (isValid)
                {
                    lciHrmKskCode.AppearanceItemCaption.ForeColor = Color.Maroon;
                    Valid_HrmKskCode_Control oDateRule = new Valid_HrmKskCode_Control();
                    oDateRule.txtHrmKskCode = txtHrmKskCode;
                    oDateRule.ErrorType = ErrorType.Warning;
                    this.dxValidationProvider1.SetValidationRule(txtHrmKskCode, oDateRule);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal bool ValidateRequiredField()
        {
            bool valid = true;
            try
            {
                if (!this.dxValidationProvider1.Validate())
                {
                    IList<Control> invalidControls = this.dxValidationProvider1.GetInvalidControls();
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

        #endregion

        #region Get - Set Value

        internal string GetValue()
        {
            try
            {
                if (!String.IsNullOrEmpty(this.txtHrmKskCode.Text))
                    return this.txtHrmKskCode.Text;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return null;
        }

        internal void SetValue(string hrmKskCode)
        {
            try
            {
                this.txtHrmKskCode.Text = hrmKskCode;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion


        internal void SetVisible(bool show)
        {
            try
            {
                this.isVisible = show;
                if (!show)
                {
                    lciHrmKskCode.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
                else
                {
                    lciHrmKskCode.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal bool GetVisible()
        {
            bool show = false;
            try
            {
                if (this.isVisible && this.isValid)
                {
                    show = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return show;
        }

        internal void RefreshData()
        {
            try
            {
                txtHrmKskCode.Text = "";
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(this.dxValidationProvider1, this.dxErrorProvider1);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void ResetRequiredField()
        {
            try
            {
                Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(this.dxValidationProvider1, this.dxErrorProvider1);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void ReloadValidation(bool _isValid)
        {
            try
            {
                this.isValid = _isValid;
                if (this.isValid)
                {
                    ValidHrmKskCode();
                }
                else
                {
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(this.dxValidationProvider1, this.dxErrorProvider1);
                    this.lciHrmKskCode.AppearanceItemCaption.ForeColor = Color.Black;
                    this.dxValidationProvider1.SetValidationRule(txtHrmKskCode, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        internal void DisposeControl()
        {
            try
            {
                val = false;
                isVisible = false;
                isValid = false;
                positionHandleControl = 0;
                dlgFocusNextUserControl = null;
                this.txtHrmKskCode.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.txtHrmKskCode_KeyDown);
                this.Load -= new System.EventHandler(this.UCHrmKskCode_Load);
                dxErrorProvider1 = null;
                dxValidationProvider1 = null;
                lciHrmKskCode = null;
                txtHrmKskCode = null;
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

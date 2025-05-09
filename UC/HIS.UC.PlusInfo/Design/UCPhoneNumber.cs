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
using Inventec.Desktop.Common.LanguageManager;
using HIS.UC.PlusInfo.Validate;
using HIS.UC.PlusInfo.ShareMethod;
using System.Resources;

namespace HIS.UC.PlusInfo.Design
{
    public partial class UCPhoneNumber : UserControlBase
    {
        #region Declare

        DelegateNextControl dlgFocusNextUserControl;

        #endregion

        #region Contructor - Load

        public UCPhoneNumber()
        {
            try
            {
                InitializeComponent(); 
                
                this.SetCaptionByLanguageKeyNew();
                this.txtPhoneNumber.TabIndex = this.TabIndex;
                SetValidate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UCPhoneNumber_Load(object sender, EventArgs e)
        {
            try
            {
                //if (this.dlgFocusNextUserControl != null)
                //    this.dlgFocusNextUserControl(this.TabIndex, null);               
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện UCPhoneNumber
        /// </summary>
        private void SetCaptionByLanguageKeyNew()
        {
            try
            {
                ////Khoi tao doi tuong resource
                ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.UC.PlusInfo.Resources.Lang", typeof(UCPhoneNumber).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCPhoneNumber.layoutControl1.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPhoneNumber.Text = Inventec.Common.Resource.Get.Value("UCPhoneNumber.lciPhoneNumber.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
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
                this.lciPhoneNumber.Text = Inventec.Common.Resource.Get.Value("UCPlusInfo.UCPhoneNumber", HIS.UC.PlusInfo.ShareMethod.ResourceLanguageManager.ResourceUCPlusInfo, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Event Control

        private void txtPhoneNumber_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.dlgFocusNextUserControl(this.TabIndex, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }            
        }

        private void txtPhoneNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '/'))
                {
                    e.Handled = true;
                }
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

        #region Get - Set Value

        internal string GetValue()
        {
            try
            {
                if (!String.IsNullOrEmpty(this.txtPhoneNumber.Text))
                    return this.txtPhoneNumber.Text;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return null;
        }

        internal void SetValue(string MotherName)
        {
            try
            {
                this.txtPhoneNumber.Text = MotherName;
                //this.txtPhoneNumber.TabIndex = this.TabIndex;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        private void SetValidate()
        {
            try
            {
                Valid_Number_Control validate = new Valid_Number_Control();
                validate.txtEdit = txtPhoneNumber;
                validate.ErrorText = "Sai định dạng. Chỉ cho phép nhập số";
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(txtPhoneNumber, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal bool ValidateRequiredField()
        {
            bool result = true;
            try
            {
                positionHandle = -1;
                result = dxValidationProvider1.Validate();
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        internal void ResetRequiredField()
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

        int positionHandle = -1;

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo viewInfo = edit.GetViewInfo() as DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
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
                dlgFocusNextUserControl = null;
                positionHandle = 0;
                this.txtPhoneNumber.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.txtPhoneNumber_KeyDown);
                this.txtPhoneNumber.KeyPress -= new System.Windows.Forms.KeyPressEventHandler(this.txtPhoneNumber_KeyPress);
                this.dxValidationProvider1.ValidationFailed -= new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider1_ValidationFailed);
                this.Load -= new System.EventHandler(this.UCPhoneNumber_Load);
                dxErrorProvider1 = null;
                dxValidationProvider1 = null;
                lciPhoneNumber = null;
                txtPhoneNumber = null;
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

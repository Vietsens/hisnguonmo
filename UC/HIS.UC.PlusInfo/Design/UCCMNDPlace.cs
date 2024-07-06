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
using Inventec.Desktop.Common.LanguageManager;
using HIS.UC.PlusInfo.ShareMethod;
using System.Resources;

namespace HIS.UC.PlusInfo.Design
{
    public partial class UCCMNDPlace : UserControlBase
    {
        #region Declare

        DelegateNextControl dlgFocusNextUserControl;

        #endregion

        #region Contructor - Load

        public UCCMNDPlace()
        {
            try
            {
                InitializeComponent();

                this.txtCMNDPlace.TabIndex = this.TabIndex;
                SetMaxlength(this.txtCMNDPlace, 100);
                SetCaptionByLanguageKey();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UCCMNDPlace_Load(object sender, EventArgs e)
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
                ////Khoi tao doi tuong resource
                ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.UC.PlusInfo.Resources.Lang", typeof(UCCMNDPlace).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCCMNDPlace.layoutControl1.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciCMNDPlace.Text = Inventec.Common.Resource.Get.Value("UCCMNDPlace.lciCMNDPlace.Text", ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
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
                if (!String.IsNullOrEmpty(this.txtCMNDPlace.Text))
                    return this.txtCMNDPlace.Text;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return null;
        }

        internal void SetValue(string CMNDPlace)
        {
            try
            {
                this.txtCMNDPlace.Text = CMNDPlace;
                //this.txtCMNDPlace.TabIndex = this.TabIndex;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Event

        private void txtCMNDPlace_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (this.dlgFocusNextUserControl != null)
                    this.dlgFocusNextUserControl(this.TabIndex, null);
            }
        }

        #endregion

        private void SetMaxlength(BaseEdit control, int maxlenght)
        {
            try
            {
                Inventec.Desktop.Common.Controls.ValidationRule.ControlMaxLengthValidationRule validate = new Inventec.Desktop.Common.Controls.ValidationRule.ControlMaxLengthValidationRule();
                validate.editor = control;
                validate.maxLength = maxlenght;
                validate.IsRequired = false;
                validate.ErrorText = string.Format(ResourceMessage.NhapQuaKyTuChoPhep, maxlenght);
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validate);
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
                this.txtCMNDPlace.KeyDown -= new System.Windows.Forms.KeyEventHandler(this.txtCMNDPlace_KeyDown);
                this.dxValidationProvider1.ValidationFailed -= new DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventHandler(this.dxValidationProvider1_ValidationFailed);
                this.Load -= new System.EventHandler(this.UCCMNDPlace_Load);
                dxErrorProvider1 = null;
                dxValidationProvider1 = null;
                lciCMNDPlace = null;
                txtCMNDPlace = null;
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
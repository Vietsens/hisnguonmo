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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.Plugins.Library.RegisterConfig;
using HIS.Desktop.Plugins.Register.ValidationRule;
using HIS.Desktop.Utility;
using Inventec.Desktop.Common.Controls.ValidationRule;
using System;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.Register.Run
{
    public partial class UCRegister : UserControlBase
    {
        private void ValidateForm()
        {
            try
            {
                //Set custom error type icon for txtPatientDob control     
                this.ValidationSingleControl(this.dtIntructionTime, this.dxValidationProviderControl);
                this.ValidateLookupWithTextEdit(cboGender, this.txtGenderCode, this.dxValidationProviderControl);
                if (lciCboCareer.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                {
                    this.ValidateGridLookupWithTextEdit(this.cboCareer, this.txtCareerCode, this.dxValidationProviderPlusInfomation);
                    //this.ValidateCareer(this.dxValidationProviderPlusInfomation);
                }
                this.ValidateGridLookupWithTextEdit(this.cboPatientType, this.txtPatientTypeCode, this.dxValidationProviderControl);
                this.ValidationPatientNameControl();
                this.ValidationPatientDobControl();
                this.ValidationIntructionTimeControl();
                ValidateTreatmentType();
                ValidControlRequired(txtReasonVV);
                if (HisConfigCFG.MustHaveNCSInfoForChild)
                {
                    if (this.lcitxtHomePerson.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                    {
                        this.ValidationSingleControl(this.txtHomePerson, this.dxValidationProviderPlusInfomation, "", ValidHomePerson);
                    }
                    //if (this.lcitxtCorrelated.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                    //{
                    //    this.ValidationSingleControl(this.txtCorrelated, this.dxValidationProviderPlusInfomation, "", ValidCorrelated);
                    //}
                    //if (this.lcitxtRelativeAddress.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                    //{
                    //    this.ValidationSingleControl(this.txtRelativeAddress, this.dxValidationProviderPlusInfomation, "", ValidRelativeAddress);
                    //}
                    //if (this.lciRelativeCMNDNumber.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always)
                    //{
                    //    this.ValidationSingleControl(this.txtRelativeCMNDNumber, this.dxValidationProviderPlusInfomation, "", ValidRelativeCMNDNumber);
                    //}
                }

                if (HisConfigCFG.IsSyncHID)
                {
                    //this.lciProvinceKS.AppearanceItemCaption.ForeColor = (HisConfigCFG.IsSyncHID ? Color.Maroon : Color.Black);
                    //this.ValidateGridLookupWithTextEdit(cboProvinceKS, this.txtProvinceCodeKS, this.dxValidationProviderControl);
                    //this.ValidationSingleControl(this.txtRelativeCMNDNumber, this.dxValidationProviderControl, "", ValidCmnd);
                }

                ValidateMaxlengthTextEdit(this.txtAddress, 200);
                ValidateMaxlengthTextEdit(this.txtHomePerson, 100);
                ValidateMaxlengthTextEdit(this.txtCorrelated, 50);
                ValidateMaxlengthTextEdit(this.txtRelativeAddress, 200);
                ValidateMaxlengthTextEdit(this.txtRelativeAddress, 200);
                ValidateMaxlengthTextEdit(this.txtPhone, 12);
                ValidateMaxlengthTextEdit(this.txtRelativeCMNDNumber, 12);
                ValidateMaxlength(this.txtNote, 1000);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidControlRequired(TextEdit control)
        {
            try
            {
                ControlEditValidationRule validate = new ControlEditValidationRule();
                validate.editor = control;
                validate.ErrorText = "Thiếu trường dữ liệu bắt buộc";
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProviderControl.SetValidationRule(control, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool ValidCmnd()
        {
            try
            {
                if (!String.IsNullOrEmpty(txtRelativeCMNDNumber.Text) && (txtRelativeCMNDNumber.Text.Trim().Length != 9 && txtRelativeCMNDNumber.Text.Trim().Length != 12))
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return false;
        }

        void DXErrorProvider_GetErrorIcon(GetErrorIconEventArgs e)
        {
            if (e.ErrorType == ErrorType.Critical)
            {
                //e.ErrorIcon = ImageLoader.Instance.GetImageInfo("INT_ErrorIcon").Image;
            }
        }

        bool ValidHomePerson()
        {
            bool success = true;
            try
            {
                if (this.CheckIsChild() && String.IsNullOrEmpty(this.txtHomePerson.Text))
                {
                    success = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return success;
        }

        bool ValidCorrelated()
        {
            bool success = true;
            try
            {
                if (this.CheckIsChild() && String.IsNullOrEmpty(this.txtCorrelated.Text))
                {
                    success = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return success;
        }

        bool ValidRelativeAddress()
        {
            bool success = true;
            try
            {
                if (this.CheckIsChild() && String.IsNullOrEmpty(this.txtRelativeAddress.Text))
                {
                    success = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return success;
        }

        bool ValidRelativeCMNDNumber()
        {
            bool success = true;
            try
            {
                if (this.CheckIsChild() && String.IsNullOrEmpty(this.txtRelativeCMNDNumber.Text))
                {
                    success = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return success;
        }

        bool CheckIsChild()
        {
            bool success = false;
            try
            {
                if (this.dtPatientDob.EditValue != null && this.dtPatientDob.DateTime != DateTime.MinValue)
                {
                    DateTime dtNgSinh = this.dtPatientDob.DateTime;
                    success = MOS.LibraryHein.Bhyt.BhytPatientTypeData.IsChild(dtNgSinh);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return success;
        }

        void ValidationPatientNameControl()
        {
            try
            {
                PatientName__ValidationRule oPatientNameRule = new PatientName__ValidationRule();
                oPatientNameRule.txtPatientName = this.txtPatientName;
                oPatientNameRule.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                oPatientNameRule.ErrorType = ErrorType.Warning;
                this.dxValidationProviderControl.SetValidationRule(this.txtPatientName, oPatientNameRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //void ValidationProvinceKSControl()
        //{
        //    try
        //    {
        //        ProvinceKS__ValidationRule oPatientNameRule = new ProvinceKS__ValidationRule();
        //        oPatientNameRule.txtProvinceCodeKS = this.txtProvinceCodeKS;
        //        oPatientNameRule.cboProvinceKS = this.cboProvinceKS;
        //        oPatientNameRule.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
        //        oPatientNameRule.ErrorType = ErrorType.Warning;
        //        this.dxValidationProviderControl.SetValidationRule(this.txtProvinceCodeKS, oPatientNameRule);
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        private void ValidateTreatmentType()
        {
            try
            {
                TreatmentType__ValidationRule validRule = new TreatmentType__ValidationRule();
                validRule.cboTreatmentType = this.cboTreatmentType;
                validRule.ErrorText = ResourceMessage.TruongDuLieuBatBuoc;
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderControl.SetValidationRule(this.cboTreatmentType, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ValidationPatientDobControl()
        {
            try
            {
                PatientDob__ValidationRule oDobDateRule = new PatientDob__ValidationRule();
                oDobDateRule.txtDob = this.txtPatientDob;
                oDobDateRule.dtDob = this.dtPatientDob;
                oDobDateRule.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                oDobDateRule.ErrorType = ErrorType.Warning;
                this.dxValidationProviderControl.SetValidationRule(this.txtPatientDob, oDobDateRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ValidationIntructionTimeControl()
        {
            try
            {
                IntructionTime__ValidationRule timeRule = new IntructionTime__ValidationRule();
                timeRule.txtIntructionTime = this.txtIntructionTime;
                timeRule.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                timeRule.ErrorType = ErrorType.Warning;
                this.dxValidationProviderControl.SetValidationRule(this.txtIntructionTime, timeRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateLookupWithTextEdit(LookUpEdit cbo, TextEdit textEdit, DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProviderEditor)
        {
            try
            {
                LookupEditWithTextEditValidationRule validRule = new LookupEditWithTextEditValidationRule();
                validRule.txtTextEdit = textEdit;
                validRule.cbo = cbo;
                validRule.ErrorText = ResourceMessage.TruongDuLieuBatBuoc;
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditor.SetValidationRule(textEdit, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateHrmKskCode(DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProviderEditor)
        {
            try
            {
                HrmKskCode__ValidationRule validRule = new HrmKskCode__ValidationRule();
                validRule.txtKskCode = this.txtKskCode;
                validRule.ErrorText = ResourceMessage.TruongDuLieuBatBuoc;
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditor.SetValidationRule(this.txtKskCode, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateGridLookupWithTextEdit(GridLookUpEdit cbo, TextEdit textEdit, DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProviderEditor)
        {
            try
            {
                GridLookupEditWithTextEditValidationRule validRule = new GridLookupEditWithTextEditValidationRule();
                validRule.txtTextEdit = textEdit;
                validRule.cbo = cbo;
                validRule.ErrorText = ResourceMessage.TruongDuLieuBatBuoc;
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditor.SetValidationRule(textEdit, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationSingleControl(BaseEdit control, DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProviderEditor)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = ResourceMessage.TruongDuLieuBatBuoc;
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditor.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationSingleControl(Control control, DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProviderEditor, string messageErr, IsValidControl isValidControl)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                if (isValidControl != null)
                {
                    validRule.isUseOnlyCustomValidControl = true;
                    validRule.isValidControl = isValidControl;
                }
                if (!String.IsNullOrEmpty(messageErr))
                    validRule.ErrorText = messageErr;
                else
                    validRule.ErrorText = ResourceMessage.TruongDuLieuBatBuoc;
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditor.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dxValidationProviderPlusInfomation_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (this.positionHandlePlusInfoControl == -1)
                {
                    this.positionHandlePlusInfoControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        string name = edit.Name;
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (this.positionHandlePlusInfoControl > edit.TabIndex)
                {
                    this.positionHandlePlusInfoControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        string name = edit.Name;
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dxValidationProviderControl_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (this.positionHandleControl == -1)
                {
                    this.positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        string name = edit.Name;
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (this.positionHandleControl > edit.TabIndex)
                {
                    this.positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        string name = edit.Name;
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateMaxlengthTextEdit(DevExpress.XtraEditors.TextEdit txtEdit, int maxlength, bool isVali = false)
        {
            try
            {
                TextEditMaxLengthValidationRule _rule = new TextEditMaxLengthValidationRule();
                _rule.txtEdit = txtEdit;
                _rule.maxlength = maxlength;
                _rule.isVali = isVali;
                _rule.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                _rule.ErrorType = ErrorType.Warning;
                this.dxValidationProviderControl.SetValidationRule(txtEdit, _rule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidateMaxlength(BaseEdit control, int maxlenght)
        {
            try
            {
                ControlMaxLengthValidationRule validate = new ControlMaxLengthValidationRule();
                validate.editor = control;
                validate.maxLength = maxlenght;
                validate.IsRequired = false;
                validate.ErrorText = string.Format("Nhập quá kí tự cho phép ({0})", maxlenght);
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProviderControl.SetValidationRule(control, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ValidateComboHosspitalizeReason()
        {
            try
            {
                Inventec.Desktop.Common.Controls.ValidationRule.ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = this.cboHosReason;
                validRule.ErrorText = ResourceMessage.TruongDuLieuBatBuoc;
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderControl.SetValidationRule(this.cboHosReason, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

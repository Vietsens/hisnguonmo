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
using HIS.Desktop.LibraryMessage;
using Inventec.Desktop.Common.Controls.ValidationRule;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.PatientUpdate
{
    public partial class frmPatientUpdate : HIS.Desktop.Utility.FormBase
    {
        private void ValidateForm(bool isChild)
        {
            try
            {
                ValidationSingleControl(txtPatientName, 101, true);
                ValidationSingleControl(cboGender1);
                ValidationSingleText(dtPatientDob);
                //ValidateMaxlengthMemoEdit(txtQuaTrinh, 3000);
                ValidateMaxlengthMemoEdit(txtTienSuBenh, 3000);
                ValidateMaxlengthMemoEdit(txtTienSuGiaDinh, 3000);
                ValidateMaxlengthMemoEdit(txtNote, 1000);
                ValidateMaxlengthTaxCode();
                ValidateMaxlengthOtherAddress();
                ValidationSingleControl(txtHTAddress, 200, false);
                ValidationSingleControl(txtFatherName, 100, false);
                ValidationSingleControl(txtMotherCareer, 200, false);
                ValidationSingleControl(txtMotherEducationalLevel, 200, false);
                ValidationSingleControl(txtFatherCareer, 200, false);
                ValidationSingleControl(txtFatherEducationalLevel, 200, false);
                ValidationSingleControl(txtMotherName, 100, false);
                ValidationSingleControl(txtContact, 200, false);
                ValidationSingleControl(txtRelativeMobile, 12, false);
                ValidationSingleControl(txtRelativePhone, 12, false);

                ValidateExactLengthTextEdit(txtSocialInsuranceNumber, 10);
                ValidateCMTCCCD(txtCCCD_CMTNumber);
                ValidatePatientDob();
                ValidationTheBhyt();
                ValidControlDate(txtCCCD_CMTDate);
                ValidationSingleControl(txtAddress, 200, false);
                ValidationSingleControl(txtCCCD_CMTPlace, 100, false);
                ValidationSingleControl(txtEmail, 100, false);
                ValidationSingleControl(txtAccountNumber, 50, false);
                ValidationSingleControl(txtPatientStoreCode, 20, false);
                ValidationSingleControl(txtPersonFamily, 100, false);
                ValidationSingleControl(txtRelation, 50, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidatePatientDob()
        {
            Valid_PatientDob_Control icdMainRule = new Valid_PatientDob_Control();
            icdMainRule.dtPatientDob = this.dtPatientDob;
            icdMainRule.txtPatientDob = this.txtPatientDob;
            icdMainRule.ErrorText = MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
            icdMainRule.ErrorType = ErrorType.Warning;
            dxValidationProvider1.SetValidationRule(txtPatientDob, icdMainRule);
        }

        private void ValidControlDate(DateEdit control)
        {
            try
            {
                Dob__ValidationRule dobRule = new Dob__ValidationRule();
                dobRule.txtPeopleDob = control;
                dxValidationProvider1.SetValidationRule(control, dobRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidateLookupWithTextEdit(LookUpEdit cbo, TextEdit textEdit)
        {
            try
            {
                LookupEditWithTextEditValidationRule validRule = new LookupEditWithTextEditValidationRule();
                validRule.txtTextEdit = textEdit;
                validRule.cbo = cbo;
                validRule.ErrorText = MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(textEdit, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateMaxlengthMemoEdit(MemoEdit txt, int maxLength)
        {
            try
            {
                ValidateMaxlength validRule = new ValidateMaxlength();
                validRule.mme = txt;
                validRule.maxLength = maxLength;
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(txt, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateExactLengthTextEdit(TextEdit txt, int exactLength)
        {
            try
            {
                ValidateMaxlength validRule = new ValidateMaxlength();
                validRule.txt = txt;
                validRule.exactLength = exactLength;
                validRule.ErrorText = "Mã BHXH phải nhập đủ 10 ký tự";
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(txt, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void ValidateCMTCCCD(TextEdit txt)
        {
            try
            {
                ValidateCMTCCCD validRule = new ValidateCMTCCCD();
                validRule.txtCmndNumber = txt;
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(txt, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateMaxlengthTaxCode()
        {
            try
            {
                ValidateMaxlengthTaxCode _Rule = new ValidateMaxlengthTaxCode();
                _Rule.txt = txtTaxCode;
                _Rule.maxLength = 14;
                _Rule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(txtTaxCode, _Rule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateMaxlengthOtherAddress()
        {
            try
            {
                ValidateMaxlengthTaxCode _Rule = new ValidateMaxlengthTaxCode();
                _Rule.txt = txtOtherAddress;
                _Rule.maxLength = 500;
                _Rule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(txtOtherAddress, _Rule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateGridLookupWithTextEdit(GridLookUpEdit cbo, TextEdit textEdit)
        {
            try
            {
                GridLookupEditWithTextEditValidationRule validRule = new GridLookupEditWithTextEditValidationRule();
                validRule.txtTextEdit = textEdit;
                validRule.cbo = cbo;
                validRule.ErrorText = MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(textEdit, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationSingleText(TextEdit text)
        {
            try
            {
                txtDOBValidate validRule = new txtDOBValidate();
                validRule.txtdob = text;
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(text, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationSingleControl(BaseEdit control)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;

                validRule.ErrorText = MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationTheBhyt()
        {
            try
            {

                ValidateTheBhyt validRule = new ValidateTheBhyt();
                validRule.theBhyt = txtTheBHYT;
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(txtTheBHYT, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationSingleControl(BaseEdit control, int maxlength, bool requied)
        {
            try
            {

                ControlMaxLengthValidationRule validRule = new ControlMaxLengthValidationRule();
                validRule.editor = control;
                validRule.maxLength = maxlength;
                validRule.IsRequired = requied;
                validRule.ErrorText = MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandleControlPatientInfo == -1)
                {
                    positionHandleControlPatientInfo = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControlPatientInfo > edit.TabIndex)
                {
                    positionHandleControlPatientInfo = edit.TabIndex;
                    if (edit.Visible)
                    {
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
    }
}

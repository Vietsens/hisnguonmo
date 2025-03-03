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
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Config;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Resources;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Validate.ValidateRule;
using HIS.Desktop.Utility.ValidateRule;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LibraryMessage;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AssignPrescriptionPK.AssignPrescription
{
    public partial class frmAssignPrescription : HIS.Desktop.Utility.FormBase
    {
        private void dxValidationProviderMaterialTypeTSD_ValidationFailed(object sender, ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandleControl == -1)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControl > edit.TabIndex)
                {
                    positionHandleControl = edit.TabIndex;
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

                if (positionHandleControl == -1)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControl > edit.TabIndex)
                {
                    positionHandleControl = edit.TabIndex;
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

        private async Task ValidateForm()
        {
            try
            {
                this.ValidationSingleControl(this.cboMediStockExport, this.dxValidationProviderControl, "", this.ValidMediStockExpMest);

                this.dxValidProviderBoXung.SetValidationRule(txtMedicineTypeOther, null);
                this.dxValidProviderBoXung.SetValidationRule(txtUnitOther, null);
                this.dxValidProviderBoXung.SetValidationRule(txtMediMatyForPrescription, null);
                this.dxValidProviderBoXung.SetValidationRule(spinAmount, null);
                this.dxValidProviderBoXung.SetValidationRule(memHtu, null);
                this.ValidationSingleControl(this.spinKidneyCount, this.dxValidProviderBoXung, LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc), this.ValidKidneyShift);
                if (txtMedicineTypeOther.Enabled && String.IsNullOrEmpty(txtMediMatyForPrescription.Text))
                {
                    this.ValidationSingleControl(this.txtMedicineTypeOther, this.dxValidProviderBoXung);
                }
                if (txtUnitOther.Enabled && String.IsNullOrEmpty(txtMediMatyForPrescription.Text))
                {
                    this.ValidationSingleControl(this.txtUnitOther, this.dxValidProviderBoXung);
                }
                if (String.IsNullOrEmpty(txtMediMatyForPrescription.Text) && txtMedicineTypeOther.Enabled == false)
                {
                    this.ValidationSingleControl(this.txtMediMatyForPrescription, this.dxValidProviderBoXung);
                }

                if (this.spinAmount.Enabled && String.IsNullOrEmpty(this.spinAmount.Text))
                {
                    this.ValidationSingleControl(this.spinAmount, this.dxValidProviderBoXung, null, this.ValidAmount);
                }

                this.ValidateGridLookupWithTextEdit(this.cboUser, this.txtLoginName, this.dxValidationProviderControl);
                this.ValidateTutorial();
                this.ValidControlProvisionalDiagnosis();
                this.ValdateSecondaryIcd();

                this.dxValidationProviderControl.SetValidationRule(txtInteractionReason, null);
                if (txtInteractionReason.Enabled)
                {
                    this.ValidationSingleControl(this.txtInteractionReason, this.dxValidationProviderControl);
                }

                this.dxValidationProviderControl.SetValidationRule(txtAdvise, null);
                this.ValidMaxLengthControl(this.txtAdvise, false, 1024);
                this.ValidationMaxLengthControl(this.memHtu, 1024, this.dxValidProviderBoXung);
                if (HisConfigCFG.IsRequiredHtu == "1")
                {
                    this.ValidationSingleControl(this.cboHtu, this.dxValidProviderBoXung);
                    lciHtu.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ValidMaxLengthControl(BaseEdit txt, bool IsRequired, int maxlength)
        {

            try
            {
                ValidateMaxLength valid = new ValidateMaxLength();
                valid.maxLength = maxlength;
                valid.textEdit = txt;
                valid.IsRequired = IsRequired;
                this.dxValidationProviderControl.SetValidationRule(txt, valid);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        private void ValdateSecondaryIcd()
        {
            try
            {
                BenhPhuValidationRule mainRule = new BenhPhuValidationRule();
                mainRule.maBenhPhuTxt = txtIcdSubCode;
                mainRule.tenBenhPhuTxt = txtIcdText;
                mainRule.getIcdMain = this.GetIcdMainCode;
                mainRule.ErrorType = ErrorType.Warning;
                this.dxValidationProviderControl.SetValidationRule(txtIcdSubCode, mainRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidControlProvisionalDiagnosis()
        {
            try
            {
                CommonValidateMaxLength rule = new CommonValidateMaxLength();
                rule.textEdit = txtProvisionalDiagnosis;
                rule.maxLength = 1000;
                dxValidationProviderControl.SetValidationRule(txtProvisionalDiagnosis, rule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidateTutorial()
        {
            ControlMaxLengthValidationRule validate = new ControlMaxLengthValidationRule();
            validate.editor = txtTutorial;
            validate.maxLength = 1000;
            validate.IsRequired = false;
            validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
            this.dxValidationProviderControl.SetValidationRule(txtTutorial, validate);
        }

        private void ValidateLookupWithTextEdit(LookUpEdit cbo, TextEdit textEdit, DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProviderEditor)
        {
            try
            {
                LookupEditWithTextEditValidationRule validRule = new LookupEditWithTextEditValidationRule();
                validRule.txtTextEdit = textEdit;
                validRule.cbo = cbo;
                validRule.ErrorText = MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditor.SetValidationRule(textEdit, validRule);
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
                validRule.ErrorText = MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
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
                validRule.ErrorText = MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditor.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ValidationMaxLengthControl(BaseEdit control, int maxLength, DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProviderEditor)
        {
            try
            {

                Inventec.Desktop.Common.Controls.ValidationRule.ControlMaxLengthValidationRule icdMainRule = new Inventec.Desktop.Common.Controls.ValidationRule.ControlMaxLengthValidationRule();
                icdMainRule.editor = control;
                icdMainRule.maxLength = maxLength;
                icdMainRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditor.SetValidationRule(control, icdMainRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationSingleControl(BaseEdit control, DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProviderEditor, string messageErr, IsValidControl isValidControl)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                if (isValidControl != null)
                {
                    validRule.isValidControl = isValidControl;
                    validRule.isUseOnlyCustomValidControl = true;
                }
                if (!String.IsNullOrEmpty(messageErr))
                    validRule.ErrorText = messageErr;
                else
                    validRule.ErrorText = MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditor.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task ValidateBosung()
        {
            try
            {
                this.ValidationSingleControl(this.cboMedicineUseForm, this.dxValidProviderBoXung__DuongDung, ResourceMessage.BenhNhanDoiTuongTTBhytBatBuocPhaiNhapDuongDung, this.ValidDuongDung);

                if (HisConfigCFG.ObligateIcd == GlobalVariables.CommonStringTrue)
                {
                    ValidationICD(10, 500, true);
                }
                else
                {
                    ValidationSingleControlWithMaxLength(txtIcdCode, false, 10);
                    ValidationSingleControlWithMaxLength(txtIcdMainText, false, 500);
                }
                ValidationSingleControlWithMaxLength(txtIcdCodeCause, false, 10);
                ValidationSingleControlWithMaxLength(txtIcdMainTextCause, false, 500);
                if (HisConfigCFG.IsTrackingRequired)
                {
                    ValidationSingleControl(cboPhieuDieuTri, dxValidationProviderControl, Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc), ValidTracking);
                    this.lciPhieuDieuTri.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
                }

                if (HisConfigCFG.IsReasonRequired && actionType == GlobalVariables.ActionEdit)
                {
                    //ValidationSingleControl(cboExpMestReason, dxValidationProviderControl, Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc), ValidExpMestReason);
                    this.lciExpMestReason.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
                    //this.cboExpMestReason.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        bool ValidExpMestReason()
        {
            bool valid = true;
            try
            {
                if (lciExpMestReason.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always && cboExpMestReason.EditValue == null)
                {
                    valid = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        bool ValidTracking()
        {
            bool valid = true;
            try
            {
                if (lciPhieuDieuTri.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always && cboPhieuDieuTri.EditValue == null)
                {
                    valid = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        bool ValidMediStockExpMest()
        {
            bool valid = true;
            try
            {
                //if (currentMediStock == null || currentMediStock.Count == 0)
                //{
                //    valid = false;
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        bool ValidAmount()
        {
            bool valid = true;
            try
            {
                if (GetAmount() <= 0)
                {
                    valid = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        bool ValidDuongDung()
        {
            bool valid = true;
            try
            {
                if (currentHisPatientTypeAlter != null
                    && currentHisPatientTypeAlter.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT
                    && Inventec.Common.TypeConvert.Parse.ToInt64((cboMedicineUseForm.EditValue ?? "0").ToString()) == 0)
                {
                    valid = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        bool ValidKidneyShift()
        {
            bool valid = true;
            try
            {
                if (chkPreKidneyShift.Checked)
                {
                    valid = spinKidneyCount.Value > 0;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        private void dxValidProviderBoXung__DuongDung_ValidationFailed(object sender, ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandle__DuongDung == -1)
                {
                    positionHandle__DuongDung = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandle__DuongDung > edit.TabIndex)
                {
                    positionHandle__DuongDung = edit.TabIndex;
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

        private void dxValidProviderBoXung__MedicinePage_ValidationFailed(object sender, ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
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

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
using HIS.Desktop.ADO;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.ADO;
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.Config;
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.Resources;
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.ValidateRule;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.LibraryMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignPrescriptionYHCT.AssignPrescription
{
    public partial class frmAssignPrescription : HIS.Desktop.Utility.FormBase
    {
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

        private void ValidateForm()
        {
            try
            {
                //this.ValidationSingleControl(this.dtInstructionTime, this.dxValidationProviderControl);
                this.ValidationSingleControl(this.cboMediStockExport, this.dxValidationProviderControl, "", this.ValidMediStockExpMest);

                this.dxValidProviderBoXung.SetValidationRule(txtMedicineTypeOther, null);
                this.dxValidProviderBoXung.SetValidationRule(txtUnitOther, null);
                this.dxValidProviderBoXung.SetValidationRule(txtMediMatyForPrescription, null);
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

                this.ValidateGridLookupWithTextEdit(this.cboUser, this.txtLoginName, this.dxValidationProviderControl);
                this.ValidationSingleControl(txtLadder, this.dxValidationProviderControl);
                //this.ValidationSingleControl(txtHuongDan, this.dxValidationProviderControl);

                if (HisConfigCFG.IsReasonRequired && actionType == GlobalVariables.ActionEdit)
                {
                    //ValidationSingleControl(cboExpMestReason, dxValidationProviderControl, Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc), ValidExpMestReason);
                    this.lciExpMestReason.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
                    //this.cboExpMestReason.Properties.Buttons[1].Visible = false;
                }

                this.dxValidationProviderControl.SetValidationRule(txtAdvise, null);
                this.ValidMaxLengthControl(this.txtAdvise, false, 1024);
                ValidateHtu();
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
        private void ValidateHtu()
        {
            ControlMaxLengthValidationRule validate = new ControlMaxLengthValidationRule();
            validate.editor = memHtu;
            validate.maxLength = 1024;
            validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
            this.dxValidProviderBoXung.SetValidationRule(memHtu, validate);
        }
        private void ValidateTutorial()
        {
            ControlMaxLengthValidationRule validate = new ControlMaxLengthValidationRule();
            validate.editor = txtHuongDan;
            validate.maxLength = 1000;
            validate.IsRequired = true;
            validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
            this.dxValidProviderBoXung.SetValidationRule(txtHuongDan, validate);
        }
        private void ValidateMaxLengthControl(BaseControl control, bool IsRequire, int maxLength)
        {
            ControlMaxLengthValidationRule validate = new ControlMaxLengthValidationRule();
            validate.editor = control;
            validate.maxLength = maxLength;
            validate.IsRequired = IsRequire;
            validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
            this.dxValidationProviderControl.SetValidationRule(control, validate);
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
                validRule.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
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

        private void ValidateBosung()
        {
            try
            {
                this.ValidationSingleControl(this.spinAmount, this.dxValidProviderBoXung, null, this.ValidAmount);
                this.ValidationSingleControl(this.cboMedicineUseForm, this.dxValidProviderBoXung__DuongDung, ResourceMessage.BenhNhanDoiTuongTTBhytBatBuocPhaiNhapDuongDung, this.ValidDuongDung);
                

                this.ValidateTutorial();

                if (HisConfigCFG.IsTrackingRequired)
                {
                    ValidationSingleControl(cboPhieuDieuTri, dxValidationProviderControl, Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc), ValidTracking);
                    this.lciPhieuDieuTri.AppearanceItemCaption.ForeColor = System.Drawing.Color.Maroon;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
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
                    && Inventec.Common.TypeConvert.Parse.ToInt64((cboMedicineUseForm.EditValue ?? "0").ToString()) == 0
                     && (currentMedicineTypeADOForEdit.DO_NOT_REQUIRED_USE_FORM ?? -1) != RequiredUseFormCFG.DO_NOT_REQUIRED)
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

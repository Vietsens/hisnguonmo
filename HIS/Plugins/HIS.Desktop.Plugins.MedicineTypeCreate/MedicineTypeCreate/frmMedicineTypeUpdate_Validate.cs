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
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using Inventec.Common.Logging;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.Plugins.MedicineTypeCreate.Validtion;
using HIS.Desktop.LocalStorage.BackendData;

namespace HIS.Desktop.Plugins.MedicineTypeCreate.MedicineTypeCreate
{
    public partial class frmMedicineTypeCreate : HIS.Desktop.Utility.FormBase
    {
        private void ValidataForm()
        {
            try
            {
                ValidationControlTextEditMedicineTypeCodeName();
                ValidateLookupWithTextEdit(cboServiceUnit, txtServiceUnitCode);
                ValidateLookupWithTextEdit(cboMedicineUseForm, txtMedicineUseFormCode);
                ValidationControlSpinNotVatAndBlack(spinImpPrice);
                ValidationControlSpinVatBlack(spinImpVatRatio);
                ValidationControlSpinNotVatAndBlack(spinInternalPrice);
                ValidationControlSpinNotVatAndBlack(spinLastExpPrice);
                ValidationControlSpinNotVatAndBlack(spinLastExpVatRatio);
                ValidationControlSpinNotVatAndBlack(spinUseOnDay);
                ValidationControlAgeMonth(spinAgeFrom, spinAgeTo);
                ValidationImpUnitConverRatio(spUnitConvertRatio, cboImpUnit);
                ValidateHeinLimitRatioOld();
                ValidateHeinLimitRatio();
                ValidMaxlengthtxtRegisterNumber();
                ValidMaxlengthtxtTcyNumOrder();
                ValidMaxlengthtxtBytNumOrder();
                ValidMaxlengthtxtPackingTypeCode();
                ValidMaxlengthtxtConcentra();
                ValidMaxlengthtxtTCCL();
                ValidMaxlengthtxtTutorial();
                ValidControlMedicineNationalCode();
                ValidationControlSpinNotVatAndBlack(spinNumOrder);
                ValidatecboMedicineLine();
                //qtcode
                //ValidatecboDosageForm();
                //qtcode
                ValidMaxlengthtxtActiveIngrBhytCode();
                ValidMaxlengthtxtActiveIngrBhytName();
                ValidMaxlengthTxtDescription();
                ValidMaxlengthTxtContentWarning();
                ValidMaxlength();
                ValidMaxlengthTxtContraindication();
                ValidMaxlengthTxtRecordTransaction();
                ValidationControlMaxLength(this.txtScientificName, 500, false);
                ValidationControlMaxLength(this.txtPreprocessing, 1000, false);
                ValidationControlMaxLength(this.txtContentWarning, 2000, false);
                ValidationControlMaxLength(this.txtProcessing, 1000, false);
                ValidationControlMaxLength(this.txtUsedPart, 500, false);
                ValidationControlMaxLength(this.txtDistributedAmount, 500, false);
                ValidationControlMaxLength(this.txtOTHER_PAY_SOURCE, 200, false);
                ValidationControlMaxLength(this.memoContainer, 2000, false);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidMaxlength()
        {
            try
            {
                ValidateMaxLength validateMaxLength = new ValidateMaxLength();
                validateMaxLength.memoEdit = memoContainer;
                validateMaxLength.maxLength = 2000;
                dxValidationMedicineType.SetValidationRule(memoContainer, validateMaxLength);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidMaxlengthTxtContentWarning()
        {
            try
            {
                ValidateMaxLength validateMaxLength = new ValidateMaxLength();
                validateMaxLength.textEdit = txtContentWarning;
                validateMaxLength.maxLength = 2000;
                dxValidationMedicineType.SetValidationRule(txtContentWarning, validateMaxLength);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ValidatecboMedicineLine()
        {
            try
            {
                ValidateCombox vali = new ValidateCombox();
                vali.gridLockup = cboMedicineLine;
                vali.ErrorType = ErrorType.Warning;
                vali.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                dxValidationMedicineType.SetValidationRule(cboMedicineLine, vali);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
        //qtcode
        void ValidatecboDosageForm(bool isValid)
        {
            try
            {
                ValidateCombox vali = new ValidateCombox();
                vali.gridLockup = cboDosageForm;
                vali.ErrorType = ErrorType.Warning;
                vali.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                dxValidationMedicineType.SetValidationRule(cboDosageForm, isValid ? vali : new ValidateCombox());

            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
        //qtcode
        void ValidatecboMedicineUseForm(bool isValid)
        {
            try
            {
                lciMedicineUseForm.AppearanceItemCaption.ForeColor = isValid ? Color.Maroon : Color.Black;
                ValidComboUseMedicine validRule = new ValidComboUseMedicine();
                validRule.txtTextEdit = txtMedicineUseFormCode;
                validRule.cbo = cboMedicineUseForm;
                validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationMedicineType.SetValidationRule(txtMedicineUseFormCode, isValid ? validRule : new ValidComboUseMedicine());
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        void ValidMaxlengthTxtDescription()
        {
            try
            {
                ValidateMaxLength validateMaxLength = new ValidateMaxLength();
                validateMaxLength.textEdit = txtDescription;
                validateMaxLength.maxLength = 500;
                dxValidationMedicineType.SetValidationRule(txtDescription, validateMaxLength);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ValidMaxlengthTxtContraindication()
        {
            try
            {
                ValidateMaxLength validateMaxLength = new ValidateMaxLength();
                validateMaxLength.textEdit = txtContraindication;
                validateMaxLength.maxLength = 4000;
                dxValidationMedicineType.SetValidationRule(txtContraindication, validateMaxLength);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ValidMaxlengthTxtRecordTransaction()
        {
            try
            {
                ValidateMaxLength validateMaxLength = new ValidateMaxLength();
                validateMaxLength.textEdit = txtRecordingTransaction;
                validateMaxLength.maxLength = 20;
                dxValidationMedicineType.SetValidationRule(txtRecordingTransaction, validateMaxLength);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ValidMaxlengthtxtActiveIngrBhytCode()
        {
            try
            {
                ValidateMaxLength validateMaxLength = new ValidateMaxLength();
                validateMaxLength.textEdit = txtActiveIngrBhytCode;
                validateMaxLength.maxLength = 500;
                validateMaxLength.ErrorType = ErrorType.Warning;
                dxValidationMedicineType.SetValidationRule(txtActiveIngrBhytCode, validateMaxLength);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ValidMaxlengthtxtActiveIngrBhytName()
        {
            try
            {
                ValidateMaxLength validateMaxLength = new ValidateMaxLength();
                validateMaxLength.textEdit = txtActiveIngrBhytName;
                validateMaxLength.maxLength = 500;
                validateMaxLength.ErrorType = ErrorType.Warning;
                dxValidationMedicineType.SetValidationRule(txtActiveIngrBhytCode, validateMaxLength);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ValidMaxlengthtxtRegisterNumber()
        {
            try
            {
                ValidateMaxLength validateMaxLength = new ValidateMaxLength();
                validateMaxLength.textEdit = txtRegisterNumber;
                validateMaxLength.maxLength = 500;
                validateMaxLength.ErrorType = ErrorType.Warning;
                dxValidationMedicineType.SetValidationRule(txtRegisterNumber, validateMaxLength);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ValidMaxlengthtxtTcyNumOrder()
        {
            try
            {
                ValidateMaxLength validateMaxLength = new ValidateMaxLength();
                validateMaxLength.textEdit = txtTcyNumOrder;
                validateMaxLength.maxLength = 20;
                validateMaxLength.ErrorType = ErrorType.Warning;
                dxValidationMedicineType.SetValidationRule(txtTcyNumOrder, validateMaxLength);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ValidMaxlengthtxtBytNumOrder()
        {
            try
            {
                ValidateMaxLength validateMaxLength = new ValidateMaxLength();
                validateMaxLength.textEdit = txtBytNumOrder;
                validateMaxLength.maxLength = 50;
                validateMaxLength.ErrorType = ErrorType.Warning;
                dxValidationMedicineType.SetValidationRule(txtBytNumOrder, validateMaxLength);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ValidMaxlengthtxtPackingTypeCode()
        {
            try
            {
                ValidateMaxLength validateMaxLength = new ValidateMaxLength();
                validateMaxLength.textEdit = txtPackingTypeCode;
                validateMaxLength.maxLength = 300;
                validateMaxLength.ErrorType = ErrorType.Warning;
                dxValidationMedicineType.SetValidationRule(txtPackingTypeCode, validateMaxLength);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ValidMaxlengthtxtConcentra()
        {
            try
            {
                ValidateMaxLength validateMaxLength = new ValidateMaxLength();
                validateMaxLength.textEdit = txtConcentra;
                validateMaxLength.maxLength = 1000;
                validateMaxLength.ErrorType = ErrorType.Warning;
                dxValidationMedicineType.SetValidationRule(txtConcentra, validateMaxLength);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ValidMaxlengthtxtTCCL()
        {
            try
            {
                ValidateMaxLength validateMaxLength = new ValidateMaxLength();
                validateMaxLength.textEdit = txtTCCL;
                validateMaxLength.maxLength = 1000;
                validateMaxLength.ErrorType = ErrorType.Warning;
                dxValidationMedicineType.SetValidationRule(txtTCCL, validateMaxLength);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ValidMaxlengthtxtTutorial()
        {
            try
            {
                ValidateMaxLength validateMaxLength = new ValidateMaxLength();
                validateMaxLength.textEdit = txtTutorial;
                validateMaxLength.maxLength = 2000;
                validateMaxLength.ErrorType = ErrorType.Warning;
                dxValidationMedicineType.SetValidationRule(txtTutorial, validateMaxLength);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ValidateHeinLimitRatio()
        {
            try
            {
                HeinLimitRatioValidationRule heinLimitRatioValidationRule = new HeinLimitRatioValidationRule();
                heinLimitRatioValidationRule.txtHeinLimitRatio = this.txtHeinLimitRatio;
                heinLimitRatioValidationRule.ErrorText = "Giá trị trong khoảng 0 - 100";
                heinLimitRatioValidationRule.ErrorType = ErrorType.Warning;
                dxValidationMedicineType.SetValidationRule(this.txtHeinLimitRatio, heinLimitRatioValidationRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ValidateHeinLimitRatioOld()
        {
            try
            {
                HeinLimitRatioValidationRule heinLimitRatioOldValidationRule = new HeinLimitRatioValidationRule();
                heinLimitRatioOldValidationRule.txtHeinLimitRatio = this.txtHeinLimitRatioOld;
                heinLimitRatioOldValidationRule.ErrorText = "Giá trị trong khoảng 0 - 100";
                heinLimitRatioOldValidationRule.ErrorType = ErrorType.Warning;
                dxValidationMedicineType.SetValidationRule(this.txtHeinLimitRatioOld, heinLimitRatioOldValidationRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ValidControlMedicineNationalCode()
        {
            try
            {
                MedicineNationalCodeValidationRule rule = new MedicineNationalCodeValidationRule();
                rule.txtMedicineNationalCode = txtMedicineNationalCode;
                dxValidationMedicineType.SetValidationRule(txtMedicineNationalCode, rule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidateLookupWithTextEdit(GridLookUpEdit cbo, TextEdit textEdit)
        {
            try
            {
                GridLookupEditWithTextEditValidationRule validRule = new GridLookupEditWithTextEditValidationRule();
                validRule.txtTextEdit = textEdit;
                validRule.cbo = cbo;
                validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationMedicineType.SetValidationRule(textEdit, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationControlTextEditMedicineTypeCodeName()
        {
            try
            {
                ValidMaxlengthTxtMedicineTypeCodeName validRule = new ValidMaxlengthTxtMedicineTypeCodeName();
                validRule.txtMedicineTypeCode = txtMedicineTypeCode;
                validRule.txtMedicineTypeName = txtMedicineTypeName;
                validRule.isValidCode = (this.ActionType != HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd);
                validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationMedicineType.SetValidationRule(txtMedicineTypeCode, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationControlSpinNotVatAndBlack(SpinEdit control)
        {
            try
            {
                SpinNotVatAndBlack validRule = new SpinNotVatAndBlack();
                validRule.spin = control;
                validRule.ErrorType = ErrorType.Warning;
                dxValidationMedicineType.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationControlAgeMonth(SpinEdit controlFrom, SpinEdit controlTo)
        {
            try
            {
                ValidationAgeMonth validRule = new ValidationAgeMonth();
                validRule.spinAgeFrom = controlFrom;
                validRule.spinAgeTo = controlTo;
                validRule.ErrorType = ErrorType.Warning;
                dxValidationMedicineType.SetValidationRule(controlFrom, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationImpUnitConverRatio(SpinEdit sp, GridLookUpEdit cbo)
        {
            try
            {
                ValidationImpUnitConverRatio validRule = new ValidationImpUnitConverRatio();
                validRule.spinEdit = sp;
                validRule.ComboBox = cbo;
                validRule.ErrorType = ErrorType.Warning;
                dxValidationMedicineType.SetValidationRule(sp, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationControlSpinVatBlack(SpinEdit control)
        {
            try
            {
                SpinVatBlack validRule = new SpinVatBlack();
                validRule.spin = control;
                validRule.ErrorType = ErrorType.Warning;
                dxValidationMedicineType.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationControlSpinVatRed(SpinEdit control)
        {
            try
            {
                SpinVatRed validRule = new SpinVatRed();
                validRule.spin = control;
                validRule.ErrorType = ErrorType.Warning;
                dxValidationMedicineType.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationControlMaxLength(BaseEdit control, int? maxLength, bool IsRequest = false)
        {
            try
            {
                ControlMaxLengthValidationRule validate = new ControlMaxLengthValidationRule();
                validate.editor = control;
                validate.maxLength = maxLength;
                validate.IsRequired = IsRequest;
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationMedicineType.SetValidationRule(control, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

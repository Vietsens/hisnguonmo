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
using HIS.Desktop.Plugins.MaterialTypeCreate.Validtion;


namespace HIS.Desktop.Plugins.MaterialTypeCreate.MaterialTypeCreate
{
    public partial class frmMaterialTypeCreate : HIS.Desktop.Utility.FormBase
    {
        private void ValidataForm()
        {

            ValidateLookupWithTextEdit(cboServiceUnit, txtServiceUnitCode);
            ValidationControlSpinNotVatAndBlack(spinAlertExpiredDate);
            ValidationControlSpinNotVatAndBlack(spinAlertMinInStock);
            ValidationControlSpinNotVatAndBlack(spinAlertMaxInPrescription);
            ValidationControlSpinNotVatAndBlack(spinImpPrice);
            ValidationControlSpinNotVatAndBlack(spinLastExpPrice);
            ValidationControlSpinNotVatAndBlack(spinLastExpVatPrice);
            ValidationControlSpinVatBlack(spinImpVatRatio);
            ValidationControlSpinVatBlack(spinLastExpVatPrice);
            ValidationControlSpinNotVatAndBlack(spinInternalPrice);
            ValidationControlSpinNotVatAndBlack(spinNumOrder);
            ValidateHeinServiceTypeBhyt();
            ValidateHeinLimitRatioRange();
            ValidateHeinLimitRatioOldRange();
            ValidMaxlengthTxtPackingTypeName();
            ValidMaxlengthTxtConcentra();
            ValidMaxlengthtxtMaterialGroupBHYT();
            ValidationControlMaxReuseCount(chkMaxReuseCount, spinMaxReuseCount);
            ValidationControlMaxReuseCount(chkMaxReuseCount, spPricingMaxReuseCount);
            ValidationControlFilmSize(ChkIsFilm, cboFileSize);
            ValidationControlTextEditMaterialypeCodeName();
            validatemaxl(txtHeinOrder, 20, false);
            ValidateTxtHeinLimitPrice(txtHeinLimitPrice);
            ValidateTxtHeinLimitPrice(txtHienLimitPriceOld);
            validatemaxl(txtRecordTransation, 20, false);
            ValidMaxlengthTxtIsSupported();
            ValidMaxlengthTxtBhytCodes();
            ValidMaxlengthTxtModel();
            ValidationImpUnitConverRatio(spUnitConvertRatio, cboImpUnit);
        }

       

        private void ValidationImpUnitConverRatio(SpinEdit sp, GridLookUpEdit cbo)
        {
            try
            {
                ValidationImpUnitConverRatio validRule = new ValidationImpUnitConverRatio();
                validRule.spinEdit = sp;
                validRule.ComboBox = cbo;
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(sp, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                dxValidationProvider1.SetValidationRule(textEdit, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ValidateHeinServiceTypeBhyt()
        {
            try
            {
                HeinServiceTypeBhytValidationRule heinServiceTypeBhytValidationRule = new HeinServiceTypeBhytValidationRule();
                heinServiceTypeBhytValidationRule.txtHeinServiceTypeBhytCode = this.txtHeinServiceBhytCode;
                heinServiceTypeBhytValidationRule.txtHeinServiceTypeBhytName = this.txtHeinServiceBhytName;
                heinServiceTypeBhytValidationRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(this.txtHeinServiceBhytCode, heinServiceTypeBhytValidationRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ValidMaxlengthTxtPackingTypeName()
        {
            try
            {
                ValidateMaxLength validateMaxLength = new ValidateMaxLength();
                validateMaxLength.textEdit = txtPackingTypeCode;
                validateMaxLength.maxLength = 300;
                dxValidationProvider1.SetValidationRule(txtPackingTypeCode, validateMaxLength);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ValidMaxlengthTxtIsSupported()
        {
            try
            {
                ValidateMaxLength validateMaxLength = new ValidateMaxLength();
                validateMaxLength.textEdit = txtIsSupported;
                validateMaxLength.maxLength = 200;
                dxValidationProvider1.SetValidationRule(txtIsSupported, validateMaxLength);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        void ValidMaxlengthTxtBhytCodes()
        {
            try
            {
                ValidateMaxLength validateMaxLength = new ValidateMaxLength();
                validateMaxLength.textEdit = txtBhytWhiteListCodes;
                validateMaxLength.maxLength = 500;
                dxValidationProvider1.SetValidationRule(txtBhytWhiteListCodes, validateMaxLength);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ValidMaxlengthTxtModel()
        {
            try
            {
                ValidateMaxLength validateMaxLength = new ValidateMaxLength();
                validateMaxLength.textEdit = txtModel;
                validateMaxLength.maxLength = 250;
                dxValidationProvider1.SetValidationRule(txtModel, validateMaxLength);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        void ValidMaxlengthTxtConcentra()
        {
            try
            {
                ValidateMaxLength validateMaxLength = new ValidateMaxLength();
                validateMaxLength.textEdit = txtConcentra;
                validateMaxLength.maxLength = 500;
                dxValidationProvider1.SetValidationRule(txtConcentra, validateMaxLength);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        void ValidMaxlengthtxtMaterialGroupBHYT()
        {
            try
            {
                ValidateMaxLength validateMaxLength = new ValidateMaxLength();
                validateMaxLength.textEdit = txtMaterialGroupBHYT;
                validateMaxLength.maxLength = 500;
                dxValidationProvider1.SetValidationRule(txtMaterialGroupBHYT, validateMaxLength);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        void ValidateHeinLimitRatioRange()
        {
            try
            {
                HeinLimitRatioRangeValidationRule heinLimitRatioValidationRule = new HeinLimitRatioRangeValidationRule();
                heinLimitRatioValidationRule.txtHeinLimitRatio = this.txtHeinLimitRatio;
                heinLimitRatioValidationRule.ErrorText = "Giá trị trong khoảng 0 - 100";
                heinLimitRatioValidationRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(this.txtHeinLimitRatio, heinLimitRatioValidationRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ValidateHeinLimitRatioOldRange()
        {
            try
            {
                HeinLimitRatioRangeValidationRule heinLimitRatioOldValidationRule = new HeinLimitRatioRangeValidationRule();
                heinLimitRatioOldValidationRule.txtHeinLimitRatio = this.txtHeinLimitRatioOld;
                heinLimitRatioOldValidationRule.ErrorText = "Giá trị trong khoảng 0 - 100";
                heinLimitRatioOldValidationRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(this.txtHeinLimitRatioOld, heinLimitRatioOldValidationRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationControlTextEdit(TextEdit control)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validRule);
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
                dxValidationProvider1.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ValidationControlMaxReuseCount(CheckEdit check, SpinEdit controlSpin)
        {
            try
            {
                MaxReuseCountValidation validRule = new MaxReuseCountValidation();
                validRule.spin = controlSpin;
                validRule.checkReuse = check;
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(controlSpin, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ValidationControlFilmSize(CheckEdit check, GridLookUpEdit cbofile)
        {
            try
            {
                MaxFileSize validRule = new MaxFileSize();
                validRule.spin = cbofile;
                validRule.checkReuse = check;
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(cbofile, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationControlSpinNotVatAndRed(SpinEdit control)
        {
            try
            {
                SpinNotVatAndRed validRule = new SpinNotVatAndRed();
                validRule.spin = control;
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validRule);
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
                dxValidationProvider1.SetValidationRule(control, validRule);
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
                dxValidationProvider1.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ValidationControlTextEditMaterialypeCodeName()
        {
            try
            {
                ValidMaxlengthTxtMaterialTypeCodeName validRule = new ValidMaxlengthTxtMaterialTypeCodeName();
                validRule.txtMaterialTypeTypeCode = txtMedicineTypeCode;
                validRule.txtMaterialTypeTypeName = txtMedicineTypeName;
                validRule.isValidCode = (this.ActionType != HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionAdd);
                validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(txtMedicineTypeCode, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidateTxtHeinLimitPrice(TextEdit control)
        {
            try
            {
                HeinLimitPriceValidationRule vali = new HeinLimitPriceValidationRule();
                vali.txtHeinLimitPrice = control;
                vali.txtHeinLimitRatio = txtHeinLimitRatio;
                vali.txtHeinLimitRatioOld = txtHeinLimitRatioOld;
                vali.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, vali);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void validatemaxl(BaseEdit control, int maxlength, bool isRequired)
        {

            try
            {
                Inventec.Desktop.Common.Controls.ValidationRule.ControlMaxLengthValidationRule validRule = new Inventec.Desktop.Common.Controls.ValidationRule.ControlMaxLengthValidationRule();
                validRule.editor = control;
                validRule.maxLength = maxlength;
                validRule.IsRequired = isRequired;
                validRule.ErrorText = "Vượt quá độ dài cho phép (" + maxlength + ")";
                validRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

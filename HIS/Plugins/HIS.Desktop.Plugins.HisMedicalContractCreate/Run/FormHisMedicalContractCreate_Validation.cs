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
using HIS.Desktop.Plugins.HisMedicalContractCreate.Validation;
using HIS.Desktop.Utility;
using Inventec.Desktop.Common.Controls.ValidationRule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisMedicalContractCreate.Run
{
    public partial class FormHisMedicalContractCreate : FormBase
    {
        private void ValidControl()
        {
            try
            {
                ValidateMedicalContract();
                ValidateMetyMaty();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidateMedicalContract()
        {
            try
            {
                ControlMaxLengthValidationRule contractCodeValidate = new ControlMaxLengthValidationRule();
                contractCodeValidate.editor = txtMedicalContractCode;
                contractCodeValidate.maxLength = 50;
                contractCodeValidate.IsRequired = true;
                contractCodeValidate.ErrorText = Resources.ResourceLanguageManager.ThieuTruongDuLieuBatBuoc;
                contractCodeValidate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProviderContract.SetValidationRule(txtMedicalContractCode, contractCodeValidate);

                ControlEditValidationRule supplierValidate = new ControlEditValidationRule();
                supplierValidate.editor = cboSupplier;
                supplierValidate.ErrorText = Resources.ResourceLanguageManager.ThieuTruongDuLieuBatBuoc;
                supplierValidate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProviderContract.SetValidationRule(cboSupplier, supplierValidate);

                Validation.ValidTimeValidationRule validTimeValidate = new Validation.ValidTimeValidationRule();
                validTimeValidate.dtValidFromDate = dtValidFromDate;
                validTimeValidate.dtValidToDate = dtValidToDate;
                validTimeValidate.ErrorText = Resources.ResourceLanguageManager.HieuLucDenLonHonHieuLucTu;
                validTimeValidate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProviderContract.SetValidationRule(dtValidFromDate, validTimeValidate);

                ControlMaxLengthValidationRule ContractNameValidate = new ControlMaxLengthValidationRule();
                ContractNameValidate.editor = txtMedicalContractName;
                ContractNameValidate.maxLength = 200;
                ContractNameValidate.IsRequired = false;
                ContractNameValidate.ErrorText = string.Format(Resources.ResourceLanguageManager.NhapQuaMaxlength, "200");
                ContractNameValidate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProviderContract.SetValidationRule(txtMedicalContractName, ContractNameValidate);

                ControlMaxLengthValidationRule ventureAgreeningValidate = new ControlMaxLengthValidationRule();
                ventureAgreeningValidate.editor = txtVentureAgreening;
                ventureAgreeningValidate.maxLength = 500;
                ventureAgreeningValidate.IsRequired = false;
                ventureAgreeningValidate.ErrorText = string.Format(Resources.ResourceLanguageManager.NhapQuaMaxlength, "500");
                ventureAgreeningValidate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProviderContract.SetValidationRule(txtVentureAgreening, ventureAgreeningValidate);

                ControlMaxLengthValidationRule noteValidate = new ControlMaxLengthValidationRule();
                noteValidate.editor = txtNote;
                noteValidate.maxLength = 4000;
                noteValidate.IsRequired = false;
                noteValidate.ErrorText = string.Format(Resources.ResourceLanguageManager.NhapQuaMaxlength, "4000");
                noteValidate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProviderContract.SetValidationRule(txtNote, noteValidate);

                ControlMaxLengthValidationRule QDThauValidate = new ControlMaxLengthValidationRule();
                QDThauValidate.editor = txtQDThau;
                QDThauValidate.maxLength = 50;
                QDThauValidate.IsRequired = false;
                QDThauValidate.ErrorText = string.Format(Resources.ResourceLanguageManager.NhapQuaMaxlength, "50");
                QDThauValidate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProviderContract.SetValidationRule(txtQDThau, QDThauValidate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidateMetyMaty()
        {
            try
            {
                PriceAmountValidationRule amountValidate = new PriceAmountValidationRule();
                amountValidate.spinImpPrice = spAmount;
                amountValidate.Minvalue = 1;
                amountValidate.ErrorText = Resources.ResourceLanguageManager.ThieuTruongDuLieuBatBuoc;
                amountValidate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProviderMetyMate.SetValidationRule(spAmount, amountValidate);

                ImpVatRatioValidationRule impVatValidate = new ImpVatRatioValidationRule();
                impVatValidate.spinImpVatRatio = spImpVat;
                impVatValidate.ErrorText = Resources.ResourceLanguageManager.ThieuTruongDuLieuBatBuoc;
                impVatValidate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProviderMetyMate.SetValidationRule(spImpVat, impVatValidate);

                ControlMaxLengthValidationRule registerNumberValidate = new ControlMaxLengthValidationRule();
                registerNumberValidate.editor = txtRegisterNumber;
                registerNumberValidate.maxLength = 500;
                registerNumberValidate.IsRequired = false;
                registerNumberValidate.ErrorText = string.Format(Resources.ResourceLanguageManager.NhapQuaMaxlength, "4000");
                registerNumberValidate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProviderContract.SetValidationRule(txtRegisterNumber, registerNumberValidate);

                ControlMaxLengthValidationRule concentraValidate = new ControlMaxLengthValidationRule();
                concentraValidate.editor = txtConcentra;
                concentraValidate.maxLength = 1000;
                concentraValidate.IsRequired = false;
                concentraValidate.ErrorText = string.Format(Resources.ResourceLanguageManager.NhapQuaMaxlength, "4000");
                concentraValidate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProviderContract.SetValidationRule(txtConcentra, concentraValidate);

                ControlMaxLengthValidationRule QDThauValidate = new ControlMaxLengthValidationRule();
                QDThauValidate.editor = txtQDThau;
                QDThauValidate.maxLength = 50;
                QDThauValidate.IsRequired = false;
                QDThauValidate.ErrorText = string.Format(Resources.ResourceLanguageManager.NhapQuaMaxlength, "50");
                QDThauValidate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProviderContract.SetValidationRule(txtQDThau, QDThauValidate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

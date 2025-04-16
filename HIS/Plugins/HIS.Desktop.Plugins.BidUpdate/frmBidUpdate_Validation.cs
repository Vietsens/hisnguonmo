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
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.BidUpdate.Validation;
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

namespace HIS.Desktop.Plugins.BidUpdate
{
    public partial class frmBidUpdate : HIS.Desktop.Utility.FormBase
    {
        private void ValidControlLeft()
        {
            try
            {
                ValidAmount();
                ValidImpPrice();
                ValidImpVatRatio();
                ValidBidGroupCode();
                ValidSupplier();
                //ValidCboDosageForm();
                ValidBidPackage(txtBidPackageCode);
                ValidMaxlengthRegisterNumber();
                ValidMaxlengthConcentra();
                ValidMaxlengthActiveBhyt();
                ValidMaxlengthDosageForm();
                ValidMaxlengthTenTT();
                ValidMaxlengthTenBhyt();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void ValidMaxlengthRegisterNumber()
        {
            try
            {
                ValidateMaxLength validateMaxLength = new ValidateMaxLength();
                validateMaxLength.textEdit = txtRegisterNumber;
                validateMaxLength.maxLength = 500;
                validateMaxLength.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProviderLeft.SetValidationRule(txtRegisterNumber, validateMaxLength);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ValidMaxlengthConcentra()
        {
            try
            {
                ValidateMaxLength validateMaxLength = new ValidateMaxLength();
                validateMaxLength.textEdit = txtConcentra;
                validateMaxLength.maxLength = 1000;
                validateMaxLength.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProviderLeft.SetValidationRule(txtConcentra, validateMaxLength);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ValidMaxlengthActiveBhyt()
        {
            try
            {
                ValidateMaxLength validateMaxLength = new ValidateMaxLength();
                validateMaxLength.textEdit = txtActiveBhyt;
                validateMaxLength.maxLength = 1000;
                validateMaxLength.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProviderLeft.SetValidationRule(txtActiveBhyt, validateMaxLength);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ValidMaxlengthDosageForm()
        {
            try
            {

                CboDosageFormValidationRule cboDosageFormValidation = new CboDosageFormValidationRule();
                cboDosageFormValidation.cboDosageForm = cboDosageForm;
                cboDosageFormValidation.maxLength = 1024;
                cboDosageFormValidation.ErrorText = Resources.ResourceMessage.ThieuTruongDuLieuBatBuoc;
                cboDosageFormValidation.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProviderLeft.SetValidationRule(cboDosageForm, cboDosageFormValidation);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ValidMaxlengthTenBhyt()
        {
            try
            {
                ValidateMaxLength validateMaxLength = new ValidateMaxLength();
                validateMaxLength.textEdit = txtTenBHYT;
                validateMaxLength.maxLength = 1500;
                validateMaxLength.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProviderLeft.SetValidationRule(txtTenBHYT, validateMaxLength);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void ValidMaxlengthTenTT()
        {
            try
            {
                ValidateMaxLength validateMaxLength = new ValidateMaxLength();
                validateMaxLength.textEdit = txtTenTT;
                validateMaxLength.maxLength = 1500;
                validateMaxLength.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProviderLeft.SetValidationRule(txtTenTT, validateMaxLength);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidBidGroupCode()
        {
            try
            {
                BidGroupCodeValidationRule BidGroupCodeValidationRule = new BidGroupCodeValidationRule();
                BidGroupCodeValidationRule.txtBidGroupCode = txtBidGroupCode;
                //BidGroupCodeValidationRule.ErrorText = Resources.ResourceMessage.ThieuTruongDuLieuBatBuoc;
                BidGroupCodeValidationRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProviderLeft.SetValidationRule(txtBidGroupCode, BidGroupCodeValidationRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlRight()
        {
            try
            {
                ValidBidName();
                ValidBidNumber();
                ValidYearQD();
                ValidBID();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidBidPackage(TextEdit control)
         {
            try
            {
                BidPackageValidate bidPackageValidate = new BidPackageValidate();
                bidPackageValidate.xtraTabControl = xtraTabControl1;
                bidPackageValidate.txtBidPackage = control;
                bidPackageValidate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProviderLeft.SetValidationRule(control, bidPackageValidate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidMaxlength(Control control, int maxLength, bool isRequired)
        {
            try
            {
                ControlMaxLengthValidationRule bidPackageValidate = new ControlMaxLengthValidationRule();
                bidPackageValidate.editor = control;
                bidPackageValidate.maxLength = maxLength;
                bidPackageValidate.IsRequired = isRequired;
                bidPackageValidate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProviderLeft.SetValidationRule(control, bidPackageValidate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidImpVatRatio()
        {
            try
            {
                ImpVatRatioValidationRule impVatRatioValidationRule = new ImpVatRatioValidationRule();
                impVatRatioValidationRule.spinImpVatRatio = spinImpVat;
                impVatRatioValidationRule.ErrorText = Resources.ResourceMessage.ThieuTruongDuLieuBatBuoc;
                impVatRatioValidationRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProviderLeft.SetValidationRule(spinImpVat, impVatRatioValidationRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidBidNumOrder()
        {
            try
            {
                BidNumOrderValidationRule bidNumOrderValidationRule = new BidNumOrderValidationRule();
                bidNumOrderValidationRule.txtBidNumOrder = txtBidNumOrder;
                //bidNumOrderValidationRule.ErrorText = Resources.ResourceMessage.ThieuTruongDuLieuBatBuoc;
                bidNumOrderValidationRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProviderLeft.SetValidationRule(txtBidNumOrder, bidNumOrderValidationRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidSupplier()
        {
            try
            {
                CboSupplierValidationRule cboSupplierValidationRule = new CboSupplierValidationRule();
                cboSupplierValidationRule.txtSupplierCode = txtSupplierCode;
                cboSupplierValidationRule.cboSupplier = cboSupplier;
                cboSupplierValidationRule.ErrorText = Resources.ResourceMessage.ThieuTruongDuLieuBatBuoc;
                cboSupplierValidationRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProviderLeft.SetValidationRule(txtSupplierCode, cboSupplierValidationRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidAmount()
        {
            try
            {
                AmountValidationRule amountValidationRule = new AmountValidationRule();
                amountValidationRule.spinAmount = spinAmount;
                amountValidationRule.ErrorText = Resources.ResourceMessage.ThieuTruongDuLieuBatBuoc;
                amountValidationRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProviderLeft.SetValidationRule(spinAmount, amountValidationRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidImpPrice()
        {
            try
            {
                ImpPriceValidationRule impPriceValidationRule = new ImpPriceValidationRule();
                impPriceValidationRule.spinImpPrice = spinImpPrice;
                impPriceValidationRule.ErrorText = Resources.ResourceMessage.ThieuTruongDuLieuBatBuoc;
                impPriceValidationRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProviderLeft.SetValidationRule(spinImpPrice, impPriceValidationRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidYearQD()
        {
            try
            {
                BidYearValidationRule singleControl = new BidYearValidationRule();
                singleControl.txtBidYear = txtBidYear;
                singleControl.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProviderRight.SetValidationRule(txtBidYear, singleControl);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void ValidBidName()
        {
            try
            {
                BidNameValidationRule bidNameValidationRule = new BidNameValidationRule();
                bidNameValidationRule.txtBidName = txtBidName;
                bidNameValidationRule.ErrorText = Resources.ResourceMessage.ThieuTruongDuLieuBatBuoc;
                bidNameValidationRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProviderRight.SetValidationRule(txtBidName, bidNameValidationRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ValidCboDosageForm()
        {
            try
            {
                CboDosageFormValidationRule cboDosageFormValidation = new CboDosageFormValidationRule();
                cboDosageFormValidation.cboDosageForm = cboDosageForm;
                cboDosageFormValidation.ErrorText = Resources.ResourceMessage.ThieuTruongDuLieuBatBuoc;
                cboDosageFormValidation.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProviderLeft.SetValidationRule(cboDosageForm, cboDosageFormValidation);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidBidNumber()
        {
            try
            {
                BidNumberValidationRule bidNumberValidationRule = new BidNumberValidationRule();
                bidNumberValidationRule.txtBidNumber = txtBidNumber;
                bidNumberValidationRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProviderRight.SetValidationRule(txtBidNumber, bidNumberValidationRule);


                validatetxtbid vali = new validatetxtbid();
                vali.txtBid = txtBID;
                vali.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProviderRight.SetValidationRule(txtBID, vali);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidBID()
        {
            try
            {
                BidValidationRule bidValidationRule = new BidValidationRule();
                bidValidationRule.txtBID = txtBID;
                bidValidationRule.ErrorText = Resources.ResourceMessage.ThieuTruongDuLieuBatBuoc;
                bidValidationRule.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProviderRight.SetValidationRule(txtBID, bidValidationRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinAmount_InvalidValue(object sender, InvalidValueExceptionEventArgs e)
        {
            try
            {
                if ((int)spinAmount.Value < 0)
                {
                    spinAmount.Value = 0;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinAmount_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                if (spinAmount.EditValue != null && (spinAmount.Value < 0 || spinAmount.Value >= 10000000000000000000)) e.Cancel = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinImpVat_InvalidValue(object sender, InvalidValueExceptionEventArgs e)
        {
            try
            {
                if ((int)spinImpVat.Value < 0)
                {
                    spinImpVat.Value = 0;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinImpVat_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                if (spinImpVat.EditValue != null && (spinImpVat.Value < 0 || spinImpVat.Value >= 100)) e.Cancel = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinImpPrice_InvalidValue(object sender, InvalidValueExceptionEventArgs e)
        {
            try
            {
                if ((int)spinImpPrice.Value < 0)
                {
                    spinImpPrice.Value = 0;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinImpPrice_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                if (spinImpPrice.EditValue != null && (spinImpPrice.Value < 0 || spinImpPrice.Value >= 10000000000000000000)) e.Cancel = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

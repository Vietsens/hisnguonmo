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
using HIS.Desktop.Plugins.PharmacyCashier.Validation;
using HIS.Desktop.Utility;
using Inventec.Desktop.Common.Controls.ValidationRule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.PharmacyCashier
{
    public partial class frmPharmacyCashier : FormBase
    {
        private void ValidControl()
        {
            try
            {
                ValidControlPatientType();
                ValidControlIntructionTime();
                ValidControlCashierRoom();
                ValidControlTransactionTime();
                ValidControlTotalPrice();
                ValidControlInvoiceAccountBookService();
                ValidControlRecieptAccountBookService();
                ValidControlComboWithText(txtPayFormCode, cboPayForm, dxValidationProviderSave);
                ValidControlComboWithText(txtPayFormCode, cboPayForm, dxValidationProviderPrintInvoice);
                ValidControlComboWithText(txtInvoicePresAccountBookCode, cboInvoicePresAccountBook, dxValidationProviderPrintInvoice);
                ValidationControlMaxLength(txtPatientName, 150, dxValidationProviderSave, true);
                ValidationControlMaxLength(txtPatientAddress, 600, dxValidationProviderSave);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidationControlMaxLength(BaseEdit control, int? maxLength, DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dx, [Optional] bool IsRequest)
        {
            ControlMaxLengthValidationRule validate = new ControlMaxLengthValidationRule();
            validate.editor = control;
            validate.maxLength = maxLength;
            validate.IsRequired = IsRequest;
            validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
            dx.SetValidationRule(control, validate);
        }

        private void ValidControlPatientName()
        {
            try
            {
                PatientInfoValidationRule rule = new PatientInfoValidationRule();
                rule.txtPatientCode = txtPatientCode;
                rule.txtPatientName = txtPatientName;
                rule.checkIsVisitor = checkIsVisitor;
                this.dxValidationProviderSave.SetValidationRule(txtPatientName, rule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidControlIntructionTime()
        {
            try
            {
                ControlEditValidationRule controlEdit = new ControlEditValidationRule();
                controlEdit.editor = dtIntructionTime;
                controlEdit.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.HeThongTBTruongDuLieuBatBuocPhaiNhap);
                controlEdit.ErrorType = ErrorType.Warning;
                this.dxValidationProviderSave.SetValidationRule(dtIntructionTime, controlEdit);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidControlTransactionTime()
        {
            try
            {
                ControlEditValidationRule controlEdit = new ControlEditValidationRule();
                controlEdit.editor = dtTransactionTime;
                controlEdit.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.HeThongTBTruongDuLieuBatBuocPhaiNhap);
                controlEdit.ErrorType = ErrorType.Warning;
                this.dxValidationProviderSave.SetValidationRule(dtTransactionTime, controlEdit);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidControlPatientType()
        {
            try
            {
                PatientTypeValidationRule patientTypeRule = new PatientTypeValidationRule();
                patientTypeRule.cboPatientType = cboPatientType;
                dxValidationProviderSave.SetValidationRule(cboPatientType, patientTypeRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlCashierRoom()
        {
            try
            {
                ControlEditValidationRule controlEdit = new ControlEditValidationRule();
                controlEdit.editor = cboCashierRoom;
                controlEdit.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.HeThongTBTruongDuLieuBatBuocPhaiNhap);
                controlEdit.ErrorType = ErrorType.Warning;
                this.dxValidationProviderSave.SetValidationRule(cboCashierRoom, controlEdit);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidControlTotalPrice()
        {
            try
            {
                TotalPriceValidationRule rule = new TotalPriceValidationRule();
                rule.spinTotalPrice = spinTotalPrice;
                dxValidationProviderSave.SetValidationRule(spinTotalPrice, rule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlComboWithText(TextEdit txt, LookUpEdit cbo, DXValidationProvider dxValidationProvider)
        {
            try
            {
                LookupEditWithTextEditValidationRule controlEdit = new LookupEditWithTextEditValidationRule();
                controlEdit.txtTextEdit = txt;
                controlEdit.cbo = cbo;
                controlEdit.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.HeThongTBTruongDuLieuBatBuocPhaiNhap);
                controlEdit.ErrorType = ErrorType.Warning;
                dxValidationProvider.SetValidationRule(txt, controlEdit);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidControlInvoiceAccountBookService()
        {
            try
            {
                InvoiceAccountBookServiceValidationRule rule = new InvoiceAccountBookServiceValidationRule();
                rule.frmMain = this;
                rule.txtInvoiceAccountBookService = txtInvoiceServiceAccountBookCode;
                rule.cboInvoiceAccountBookService = cboInvoiceServiceAccountBook;
                dxValidationProviderSave.SetValidationRule(txtInvoiceServiceAccountBookCode, rule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlRecieptAccountBookService()
        {
            try
            {
                RecieptAccountBookServiceValidationRule rule = new RecieptAccountBookServiceValidationRule();
                rule.frmMain = this;
                rule.txtRecieptAccountBookService = txtReceiptAccountBookCode;
                rule.cboRecieptAccountBookService = cboReceiptAccountBook;
                dxValidationProviderSave.SetValidationRule(txtReceiptAccountBookCode, rule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ResetControlValidError()
        {
            try
            {
                dxValidationProviderSave.RemoveControlError(cboPatientType);
                dxValidationProviderSave.RemoveControlError(dtIntructionTime);
                dxValidationProviderSave.RemoveControlError(cboCashierRoom);
                dxValidationProviderSave.RemoveControlError(dtTransactionTime);
                dxValidationProviderSave.RemoveControlError(spinTotalPrice);
                dxValidationProviderSave.RemoveControlError(txtReceiptAccountBookCode);
                dxValidationProviderSave.RemoveControlError(txtInvoiceServiceAccountBookCode);
                dxValidationProviderSave.RemoveControlError(txtPayFormCode);
                dxValidationProviderSave.RemoveControlError(txtPatientAddress);
                dxValidationProviderSave.RemoveControlError(txtPatientName);
                dxValidationProviderPrintInvoice.RemoveControlError(txtInvoicePresAccountBookCode);
                dxValidationProviderPrintInvoice.RemoveControlError(txtPayFormCode);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

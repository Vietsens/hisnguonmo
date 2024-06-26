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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using Inventec.Desktop.Common.Controls.ValidationRule;
using HIS.Desktop.Plugins.InvoiceBook.Popup.CreateInvoice.Validation;
using Inventec.Desktop.Common.LibraryMessage;


namespace HIS.Desktop.Plugins.InvoiceBook
{
    public partial class frmCreateInvoice
    {
        public void ValidationControlTextEdit(BaseEdit control, IsValidControl isValidControl)
        {
            try
            {
                var validRule = new ControlEditValidationRule
                {
                    editor = control,
                    isValidControl = isValidControl,
                    ErrorText = HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc),
                    ErrorType = ErrorType.Warning
                };
                dxValidationProvider1.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void ValidVat()
        {
            try
            {
                ValidateSpinVAT vatRule = new ValidateSpinVAT();
               vatRule.spinVatRatio = spinVatRatio;
               vatRule.ErrorText = MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
               vatRule.ErrorType = ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(spinVatRatio, vatRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidChietKhau()
        {
            try
            {
                ValidateSpinChietKhau ChietKhauRule = new ValidateSpinChietKhau();
                ChietKhauRule.spinExemption = spinExemption;
                ChietKhauRule.ErrorText = MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                ChietKhauRule.ErrorType = ErrorType.Warning;
                this.dxValidationProvider1.SetValidationRule(spinExemption, ChietKhauRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        public bool ValidTxtPayFormCode()
        {
            var isValid = true;
            try
            {
                if (string.IsNullOrEmpty(txtPayFormCode.Text))
                    isValid = false;
            }
            catch (Exception ex)
            {
                isValid = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return isValid;
        }



        public bool ValidCboPayForm()
        {
            var isValid = true;
            try
            {
                if (cboPayForm.EditValue == null || string.IsNullOrEmpty(txtPayFormCode.Text))
                    isValid = false;
            }
            catch (Exception ex)
            {
                isValid = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return isValid;
        }

        public bool ValidDtInvoiceTime()
        {
            var isValid = true;
            try
            {
                if (dtInvoiceTime == null)
                    isValid = false;
                if (dtInvoiceTime != null && dtInvoiceTime.EditValue == null)
                    isValid = false;
            }
            catch (Exception ex)
            {
                isValid = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return isValid;
        }

        public bool ValidSpinExemption()
        {
            var isValid = true;
            try
            {
                if (txtBuyerName == null)
                    isValid = false;
                if (txtBuyerName != null && string.IsNullOrEmpty(txtBuyerName.Text))
                    isValid = false;
            }
            catch (Exception ex)
            {
                isValid = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return isValid;
        }

        public bool ValidTxtBuyerName()
        {
            var isValid = true;
            try
            {

            }
            catch (Exception ex)
            {
                isValid = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return isValid;
        }

        public bool ValidTxtBuyerTaxCode()
        {
            var isValid = true;
            try
            {

            }
            catch (Exception ex)
            {
                isValid = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return isValid;
        }

        public bool ValidTxtBuyerAccountNumber()
        {
            var isValid = true;
            try
            {

            }
            catch (Exception ex)
            {
                isValid = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return isValid;
        }

        public bool ValidTxtBuyerOrganization()
        {
            var isValid = true;
            try
            {

            }
            catch (Exception ex)
            {
                isValid = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return isValid;
        }



        public bool ValidTxtDescription()
        {
            var isValid = true;
            try
            {

            }
            catch (Exception ex)
            {
                isValid = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return isValid;
        }

        public bool ValidxtDescription()
        {
            var isValid = true;
            try
            {

            }
            catch (Exception ex)
            {
                isValid = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return isValid;
        }

        public bool ValidTxtSellerName()
        {
            var isValid = true;
            try
            {
                if (txtSellerName == null)
                    isValid = false;
                if (txtSellerName != null && string.IsNullOrEmpty(txtSellerName.Text))
                    isValid = false;
            }
            catch (Exception ex)
            {
                isValid = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return isValid;
        }



        public bool ValidTxtSellerTaxCode()
        {
            var isValid = true;
            try
            {

            }
            catch (Exception ex)
            {
                isValid = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return isValid;
        }

        public bool ValidTxtSellerAccountNumber()
        {
            var isValid = true;
            try
            {

            }
            catch (Exception ex)
            {
                isValid = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return isValid;
        }

        public bool ValidTxtSellerPhone()
        {
            var isValid = true;
            try
            {

            }
            catch (Exception ex)
            {
                isValid = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return isValid;
        }

        public bool ValidTxtSellerAddress()
        {
            var isValid = true;
            try
            {

            }
            catch (Exception ex)
            {
                isValid = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return isValid;
        }


    }
}

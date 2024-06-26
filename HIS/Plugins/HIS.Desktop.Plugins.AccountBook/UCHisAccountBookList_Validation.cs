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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.XtraEditors.DXErrorProvider;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.Plugins.HisAccountBookList.Validation;

namespace HIS.Desktop.Plugins.HisAccountBookList
{
    public partial class UCHisAccountBookList : HIS.Desktop.Utility.UserControlBase
    {
        private void ValidControls()
        {
            try
            {
                ValidAccountBookCode();
                ValidAccountBookName();
                ValidCountNumber();
                ValidFromNumOrder();
                ValidAccountBookType();
                ValidTemplate();
                ValidSymbol();
                ValidCheck();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidAccountBookCode()
        {
            try
            {
                CodeValidationRule CodeRule = new CodeValidationRule();
                CodeRule.txtAccountBookCode = txtAccountBookCode;
                CodeRule.ErrorText = ResourceMessage.TruongDuLieuBatBuocVaCoDoDaiToiDa;
                CodeRule.ErrorType = ErrorType.Warning;
                this.dxValidationProvider.SetValidationRule(txtAccountBookCode, CodeRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidAccountBookName()
        {
            try
            {
                NameValidationRule NameRule = new NameValidationRule();
                NameRule.txtAccountBookName = txtAccountBookName;
                NameRule.ErrorText = ResourceMessage.TXT_ACCBOOK_NAME__ERROR;
                NameRule.ErrorType = ErrorType.Warning;
                this.dxValidationProvider.SetValidationRule(txtAccountBookName, NameRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidCountNumber()
        {
            try
            {
                CountValidationRule CountRule = new CountValidationRule();
                CountRule.spnCount = spinCount;
                CountRule.ErrorText = ResourceMessage.TruongDuLieuBatBuoc;
                CountRule.ErrorType = ErrorType.Warning;
                this.dxValidationProvider.SetValidationRule(spinCount, CountRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidFromNumOrder()
        {
            try
            {
                FromValidationRule FromRule = new FromValidationRule();
                FromRule.spnFromNumberOrder = spinFromNumberOrder;
                FromRule.ErrorText = ResourceMessage.TruongDuLieuBatBuoc;
                FromRule.ErrorType = ErrorType.Warning;
                this.dxValidationProvider.SetValidationRule(spinFromNumberOrder, FromRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidAccountBookType()
        {
            try
            {
                TypeValidationRule TypeRule = new TypeValidationRule();
                TypeRule.chkForBill = chkForBill;
                TypeRule.chkForDeposit = chkForDeposit;
                TypeRule.chkForRepay = chkForRepay;
                TypeRule.chkForDebt = chkForDebt;
                TypeRule.chkForOtherSale = chkForOtherSale;
                TypeRule.txtSymbolCode = txtSymbolCode;
                TypeRule.txtTemplateCode = txtTemplateCode;
                TypeRule.ErrorText = ResourceMessage.TruongDuLieuBatBuoc;
                TypeRule.ErrorType = ErrorType.Warning;
                this.dxValidationProvider.SetValidationRule(chkForDeposit, TypeRule);
                this.dxValidationProvider.SetValidationRule(chkForBill, TypeRule);
                this.dxValidationProvider.SetValidationRule(chkForRepay, TypeRule);
                this.dxValidationProvider.SetValidationRule(chkForDebt, TypeRule);
                this.dxValidationProvider.SetValidationRule(chkForOtherSale, TypeRule);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidTemplate()
        {
            try
            {
                TemplateValidationRule Rule = new TemplateValidationRule();
                Rule.txtTemplateCode = txtTemplateCode;
                Rule.txtSymbolCode = txtSymbolCode;
                Rule.cboEInvoiceSys = cboEInvoiceSys;
                Rule.ErrorText = ResourceMessage.TXT_TEMPLATE__ERROR;
                Rule.ErrorType = ErrorType.Warning;
                this.dxValidationProvider.SetValidationRule(txtTemplateCode, Rule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidSymbol()
        {
            try
            {
                SymbolValidationRule Rule = new SymbolValidationRule();
                Rule.txtTemplateCode = txtTemplateCode;
                Rule.txtSymbolCode = txtSymbolCode;
                Rule.cboEInvoiceSys = cboEInvoiceSys;
                Rule.ErrorText = ResourceMessage.TXT_SYMBOL__ERROR;
                Rule.ErrorType = ErrorType.Warning;
                this.dxValidationProvider.SetValidationRule(txtSymbolCode, Rule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidCheck()
        {
            try
            {
                checkValidationRule checkRule = new checkValidationRule();
                checkRule.chkForDeposit = chkForDeposit;
                checkRule.chkForRepay = chkForRepay;
                checkRule.chkForBill = chkForBill;
                checkRule.chkForDebt = chkForDebt;
                checkRule.chkForOtherSale = chkForOtherSale;
                checkRule.txtSymbolCode = txtSymbolCode;
                checkRule.txtTemplateCode = txtTemplateCode;
                checkRule.ErrorText = ResourceMessage.TruongDuLieuBatBuoc;
                checkRule.ErrorType = ErrorType.Warning;
                this.dxValidationProvider.SetValidationRule(chkForBill, checkRule);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinCount_InvalidValue(object sender, DevExpress.XtraEditors.Controls.InvalidValueExceptionEventArgs e)
        {
            try
            {
                if ((int)spinCount.Value < 0)
                {
                    spinCount.Value = 0;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinCount_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                if (spinCount.EditValue != null && (spinCount.Value < 0 || spinCount.Value > 1000000000000000000)) e.Cancel = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinFromNumberOrder_InvalidValue(object sender, DevExpress.XtraEditors.Controls.InvalidValueExceptionEventArgs e)
        {
            try
            {
                if ((int)spinFromNumberOrder.Value < 0)
                {
                    spinFromNumberOrder.Value = 0;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinFromNumberOrder_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                if (spinFromNumberOrder.EditValue != null && (spinFromNumberOrder.Value < 0 || spinFromNumberOrder.Value > 1000000000000000000)) e.Cancel = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTemplateCode_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                CheckReadOnly();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSymbolCode_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                CheckReadOnly();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CheckReadOnly()
        {
            try
            {
                if (this.ActionType == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionView || this.ActionType == HIS.Desktop.LocalStorage.LocalData.GlobalVariables.ActionEdit)
                {
                    chkNotGenOrder.ReadOnly = true;
                    chkNotGenOrder.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

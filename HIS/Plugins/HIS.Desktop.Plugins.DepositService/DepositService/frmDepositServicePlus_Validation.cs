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
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.DepositService.DepositService.Validtion;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.Message;
using System;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.DepositService.DepositService
{
    public partial class frmDepositService : HIS.Desktop.Utility.FormBase
    {

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo viewInfo = edit.GetViewInfo() as DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidControls()
        {
            try
            {
                ValidateGridLookupWithTextEdit(cboAccountBook, txtAccountBookCode);
                //ValidateGridLookupWithTextEdit(cboPayForm, txtPayFormCode);
                ValidationSingleControl(cboPayForm);
                ValidControlTransactionTime();
                ValidControlDescription();
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
                TransactionTimeValidationRule amountRule = new TransactionTimeValidationRule();
                amountRule.dtTransactionTime = dtTransactionTime;
                dxValidationProvider1.SetValidationRule(dtTransactionTime, amountRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlDescription()
        {
            try
            {
                Inventec.Desktop.Common.Controls.ValidationRule.ControlMaxLengthValidationRule validate = new Inventec.Desktop.Common.Controls.ValidationRule.ControlMaxLengthValidationRule();
                validate.editor = this.txtDescription;
                validate.maxLength = 2000;
                validate.IsRequired = false;
                validate.ErrorText = string.Format("Nhập quá ký tự cho phép {0}", 2000);
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(this.txtDescription, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidateGridLookupWithTextEdit(GridLookUpEdit cbo, TextEdit textEdit)
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

        private void ValidationSingleControl(BaseEdit control)
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

        private void ValidationControlSpinEdit(SpinEdit control)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuKhongNhanGiaTriAm);
                validRule.isValidControl = IsValidSpinAmount;
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool IsValidSpinAmount()
        {
            if (this.txtAmount != null && (decimal)(this.txtAmount.Tag) < 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void spinAmount_InvalidValue(object sender, DevExpress.XtraEditors.Controls.InvalidValueExceptionEventArgs e)
        {
            //try
            //{
            //    if ((Double)spinAmount.Value < 0)
            //    {
            //        spinAmount.Value = 0;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Warn(ex);
            //}

        }


        bool CheckValidForSave(CommonParam param)
        {
            bool valid = true;
            this.positionHandle = -1;
            try
            {
                valid = valid && (dxValidationProvider1.Validate());
                valid = valid && (this.hisDepositSDO != null);
                if (valid)
                    Inventec.Common.Logging.LogSystem.Warn("Gia tri cua this.hisDepositSDO.SereServDeposits " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.hisDepositSDO.SereServDeposits), this.hisDepositSDO.SereServDeposits));
                if (this.hisDepositSDO.SereServDeposits == null || this.hisDepositSDO.SereServDeposits.Count <= 0)
                {
                    WaitingManager.Hide();
                    param.Messages.Add("Bạn chưa chọn dịch vụ");
                    MessageManager.Show(param, false);
                    txtAccountBookCode.SelectAll();
                    txtAccountBookCode.Focus();
                    valid = false;
                }
                if (this.hisDepositSDO.Transaction != null && this.hisDepositSDO.Transaction.PAY_FORM_ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TMCK && spinTransferAmount.EditValue != null && spinTransferAmount.Value > this.hisDepositSDO.Transaction.AMOUNT)
                {
                    WaitingManager.Hide();
                    param.Messages.Add(String.Format("Số tiền chuyển khoản [{0}] lớn hơn số tiền tạm ứng của bệnh nhân [{1}]", Inventec.Common.Number.Convert.NumberToStringRoundAuto(spinTransferAmount.Value, ConfigApplications.NumberSeperator), Inventec.Common.Number.Convert.NumberToStringRoundAuto(this.hisDepositSDO.Transaction.AMOUNT, ConfigApplications.NumberSeperator)));
                    MessageManager.Show(param, false);
                    valid = false;
                }
                else if (this.hisDepositSDO.Transaction != null && this.hisDepositSDO.Transaction.PAY_FORM_ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TMQT && spinTransferAmount.EditValue != null && spinTransferAmount.Value > this.hisDepositSDO.Transaction.AMOUNT)
                {
                    WaitingManager.Hide();
                    param.Messages.Add(String.Format("Số tiền quẹt thẻ [{0}] lớn hơn số tiền tạm ứng của bệnh nhân [{1}]", Inventec.Common.Number.Convert.NumberToStringRoundAuto(spinTransferAmount.Value, ConfigApplications.NumberSeperator), Inventec.Common.Number.Convert.NumberToStringRoundAuto(this.hisDepositSDO.Transaction.AMOUNT, ConfigApplications.NumberSeperator)));
                    MessageManager.Show(param, false);
                    valid = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }
    }
}

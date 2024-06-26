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
using HIS.Desktop.Plugins.RepayService.RepayService.Validtion;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.RepayService.RepayService
{
    public partial class frmRepayService : HIS.Desktop.Utility.FormBase
    {
        private void ValidControls()
        {
            try
            {
                ValidControlAccountBook();
                ValidControlPayForm();
                ValidControlTransactionTime();
                ValidControlRepayReason();
                ValidControlDescription();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidControlRepayReason()
        {
            try
            {
                RepayReasonValidationRule repayReasonRule = new RepayReasonValidationRule();
                repayReasonRule.txtRepayReasonCode = txtRepayReason;
                repayReasonRule.cboRepayReason = cboRepayReason;
                repayReasonRule.isRequired = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<long>("HIS.Desktop.Plugins.Repay.Is_Required_Repay_Reason");
                dxValidationProvider1.SetValidationRule(txtRepayReason, repayReasonRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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

        private void ValidControlAccountBook()
        {
            try
            {
                AccountBookValidationRule accountBookRule = new AccountBookValidationRule();
                accountBookRule.cboAccountBook = cboAccountBook;
                dxValidationProvider1.SetValidationRule(cboAccountBook, accountBookRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlPayForm()
        {
            try
            {
                PayFormValidationRule payFormRule = new PayFormValidationRule();
                payFormRule.cboPayForm = cboPayForm;
                dxValidationProvider1.SetValidationRule(cboPayForm, payFormRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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
            if (this.spinAmount != null && this.spinAmount.Tag != null && (decimal)(this.spinAmount.Tag) < 0)
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
                valid = valid && (this.hisTransactionRepaySDO != null);
                if (valid)
                {
                    if (this.hisTransactionRepaySDO.SereServDepositIds == null || this.hisTransactionRepaySDO.SereServDepositIds.Count <= 0)
                    {
                        WaitingManager.Hide();
                        param.Messages.Add("Bạn chưa chọn dịch vụ");
                        MessageManager.Show(param, false);
                        valid = false;
                    }

                    if (this.hisTransactionRepaySDO.Transaction.PAY_FORM_ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TMCK && spinTransferAmount.EditValue != null)
                    {

                        if (spinTransferAmount.Value > this.hisTransactionRepaySDO.Transaction.AMOUNT)
                        {
                            WaitingManager.Hide();
                            param.Messages.Add(String.Format(Resources.ResourceMessageLang.SoTienChuyenKhoanLonHonSoTienHoanUng, Inventec.Common.Number.Convert.NumberToStringRoundAuto(spinTransferAmount.Value, ConfigApplications.NumberSeperator), Inventec.Common.Number.Convert.NumberToStringRoundAuto(this.hisTransactionRepaySDO.Transaction.AMOUNT, ConfigApplications.NumberSeperator)));
                            MessageManager.Show(param, false);
                            return false;
                        }
                    }
                    else if (this.hisTransactionRepaySDO.Transaction.PAY_FORM_ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TMQT && spinTransferAmount.EditValue != null)
                    {

                        if (spinTransferAmount.Value > this.hisTransactionRepaySDO.Transaction.AMOUNT)
                        {
                            WaitingManager.Hide();
                            param.Messages.Add(String.Format(Resources.ResourceMessageLang.SoTienQuetTheLonHonSoTienHoanUng, Inventec.Common.Number.Convert.NumberToStringRoundAuto(spinTransferAmount.Value, ConfigApplications.NumberSeperator), Inventec.Common.Number.Convert.NumberToStringRoundAuto(this.hisTransactionRepaySDO.Transaction.AMOUNT, ConfigApplications.NumberSeperator)));
                            MessageManager.Show(param, false);
                            return false;
                        }

                    }
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

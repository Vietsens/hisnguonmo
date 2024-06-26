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
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Repay.Repay
{
    public partial class frmRepay : DevExpress.XtraEditors.XtraForm
    {
        private void ValidControls()
        {
            try
            {
                ValidateGridLookupWithTextEdit(cboAccountBook, txtAccountBookCode);
                ValidateGridLookupWithTextEdit(cboPayForm, txtPayFormCode);
                ValidationControlSpinEdit(spinAmount);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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

        private void ValidationControlSpinEdit(SpinEdit control)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuKhongNhanGiaTriAm);
                validRule.isValidControl = isValidSpinAmount;
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validRule);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool isValidSpinAmount()
        {
            if (this.spinAmount != null && this.spinAmount.Value < 0)
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


        bool CheckValidForSave()
        {
            bool valid = true;
            this.positionHandle = -1;
            try
            {
                valid = valid && (this.ActionType == GlobalVariables.ActionAdd || this.ActionType == GlobalVariables.ActionEdit);
                valid = valid && (dxValidationProvider1.Validate());
                valid = valid && (this.HisRepaySDO != null);
                if (valid)
                    if (this.HisRepaySDO.DereDetailIds == null || this.HisRepaySDO.DereDetailIds.Count <= 0)
                    {
                        MessageManager.Show("Thiếu trường dữ liệu bắt buộc, vui lòng kiểm tra lại.");
                        txtAccountBookCode.SelectAll();
                        txtAccountBookCode.Focus();
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

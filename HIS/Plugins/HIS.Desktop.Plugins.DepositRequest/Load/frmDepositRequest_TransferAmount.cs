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
using HIS.Desktop.Plugins.DepositRequest.Config;
using HIS.Desktop.Plugins.DepositRequest.Validtion;
using HIS.Desktop.Utility;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.DepositRequest
{
    public partial class UCDepositRequest : UserControlBase
    {
        /// <summary>
        /// Viec 54923: chi hien o "So tien CK" khi hinh thuc thanh toan la "Tien mat/Chuyen khoan" (ma 03).
        /// Cach lam bam theo man "Tam ung" (frmDepositService.CheckPayFormTienMatChuyenKhoan).
        /// </summary>
        private void CheckPayFormTienMatChuyenKhoan(HIS_PAY_FORM payForm)
        {
            try
            {
                if (payForm != null && payForm.ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TMCK)
                {
                    this.lciTransferAmount.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    dxValidationProvider.RemoveControlError(spinTransferAmount);
                    ValidControlTransferAmount(true);
                    lciTransferAmount.AppearanceItemCaption.ForeColor = Color.Maroon;
                    lciTransferAmount.AppearanceItemCaption.Options.UseForeColor = true;
                    lciTransferAmount.Enabled = true;
                }
                else
                {
                    this.lciTransferAmount.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    dxValidationProvider.RemoveControlError(spinTransferAmount);
                    ValidControlTransferAmount(false);
                    lciTransferAmount.Enabled = false;
                }
                spinTransferAmount.EditValue = 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlTransferAmount(bool IsRequiredField)
        {
            try
            {
                SpinTranferAmountValidationRule rule = new SpinTranferAmountValidationRule();
                rule.spinTranferAmount = spinTransferAmount;
                rule.isRequiredPin = IsRequiredField;
                dxValidationProvider.SetValidationRule(spinTransferAmount, rule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private HIS_PAY_FORM GetCurrentPayForm()
        {
            HIS_PAY_FORM result = null;
            try
            {
                if (cboPayForm.EditValue != null && ListPayForm != null)
                {
                    long payFormId = Inventec.Common.TypeConvert.Parse.ToInt64(cboPayForm.EditValue.ToString());
                    result = ListPayForm.FirstOrDefault(o => o.ID == payFormId);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void cboPayForm_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                CheckPayFormTienMatChuyenKhoan(GetCurrentPayForm());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Viec 54923: chi cho sua o "Số tiền" khi bat key config va yeu cau tam ung chua thu tien.
        /// </summary>
        private void ApplyEditAmountState(V_HIS_DEPOSIT_REQ data)
        {
            try
            {
                bool allowEdit = HisConfigCFG.IsAllowEditAmount && data != null && data.DEPOSIT_ID == null;
                txtAmount.Properties.ReadOnly = !allowEdit;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinTransferAmount_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSavePrint.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

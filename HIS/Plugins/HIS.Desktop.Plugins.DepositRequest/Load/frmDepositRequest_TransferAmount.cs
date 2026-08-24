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
using HIS.Desktop.LocalStorage.ConfigApplication;
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
        /// Viec 54923: enable/disable o "So tien CK" theo hinh thuc thanh toan,
        /// flow chuan lay tu man Xuat hoa don ban thuoc (frmMedicineSaleBill.cboPayFrom_EditValueChanged):
        /// o LUON HIEN, mac dinh disable; PAY_FORM_CODE = 03 (Tien mat/Chuyen khoan) -> enable, nhan "So tien CK";
        /// PAY_FORM_CODE = 06 (Tien mat/Quet the) -> enable, doi nhan "So tien QT";
        /// cac hinh thuc con lai -> disable, nhan den. Khong bat buoc nhap.
        /// </summary>
        private void CheckPayFormTienMatChuyenKhoan(HIS_PAY_FORM payForm)
        {
            try
            {
                lciTransferAmount.Enabled = false;
                spinTransferAmount.Enabled = false;
                lciTransferAmount.Text = "Số tiền CK:";
                lciTransferAmount.OptionsToolTip.ToolTip = "Số tiền chuyển khoản";
                lciTransferAmount.AppearanceItemCaption.ForeColor = Color.Black;
                lciTransferAmount.AppearanceItemCaption.Options.UseForeColor = true;

                dxErrorProvider.SetError(spinTransferAmount, string.Empty);
                spinTransferAmount.EditValue = null;

                if (payForm != null && payForm.PAY_FORM_CODE == "03")
                {
                    lciTransferAmount.Enabled = true;
                    spinTransferAmount.Enabled = true;
                    lciTransferAmount.AppearanceItemCaption.ForeColor = Color.Maroon;
                    lciTransferAmount.Text = "Số tiền CK:";
                    lciTransferAmount.OptionsToolTip.ToolTip = "Số tiền chuyển khoản";
                }
                else if (payForm != null && payForm.PAY_FORM_CODE == "06")
                {
                    lciTransferAmount.Enabled = true;
                    spinTransferAmount.Enabled = true;
                    lciTransferAmount.AppearanceItemCaption.ForeColor = Color.Maroon;
                    lciTransferAmount.Text = "Số tiền QT:";
                    lciTransferAmount.OptionsToolTip.ToolTip = "Số tiền quẹt thẻ";
                }

                UpdateCanThuLabel();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Viec 54923: Can thu = So tien - So tien nhap o box So tien CK/QT (flow chuan MedicineSaleBill.UpdateCanThuLabel).
        /// </summary>
        private void UpdateCanThuLabel()
        {
            try
            {
                decimal total = 0;
                TryGetEditAmount(out total);

                decimal transfer = 0;
                if (spinTransferAmount.Enabled && spinTransferAmount.EditValue != null)
                {
                    decimal.TryParse(spinTransferAmount.EditValue.ToString(), out transfer);
                }

                decimal canThu = total - transfer;
                if (canThu < 0)
                    canThu = 0;

                lblCanThu.Text = Inventec.Common.Number.Convert.NumberToString(canThu, ConfigApplications.NumberSeperator);
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

        private void spinTransferAmount_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                //Xoa canh bao cu khi nguoi dung sua lai so tien
                dxErrorProvider.SetError(spinTransferAmount, string.Empty);
                UpdateCanThuLabel();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtAmount_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                UpdateCanThuLabel();
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

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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ExpMestChmsCreate.Validation
{
    class ExpAmountValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.SpinEdit spinExpAmount;

        // PTTK 36619 (BV HAGL): Func để check user đã nhập AMOUNT_TRANSFER > 0 trên grid (batch mode).
        // Khi batch mode active → bypass validation spinExpAmount = 0.
        internal Func<bool> hasGridTransferRowFunc;

        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool valid = false;
            try
            {
                if (spinExpAmount == null) return valid;

                // PTTK 36619: nếu user đã nhập số lượng trực tiếp trên grid → bỏ qua validation
                if (hasGridTransferRowFunc != null && hasGridTransferRowFunc())
                {
                    return true;
                }

                if (spinExpAmount.Enabled && spinExpAmount.Value <= 0)
                {
                    ErrorText = Base.ResourceMessageLang.SoLuongXuatPhaiLonHonKhong;
                    ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                    return valid;
                }
                valid = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }
    }
}

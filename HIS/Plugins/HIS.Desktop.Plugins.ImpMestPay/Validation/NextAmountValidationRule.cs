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

namespace HIS.Desktop.Plugins.ImpMestPay.Validation
{
    class NextAmountValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.SpinEdit spinNextAmount;
        internal DevExpress.XtraEditors.SpinEdit spinAmount;
        internal DevExpress.XtraEditors.LabelControl lblRemainAmount;

        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool valid = false;
            try
            {
                if (spinAmount == null || lblRemainAmount == null) return valid;
                if (spinNextAmount.EditValue != null && spinNextAmount.Value > 0)
                {
                    decimal remainAmount = Inventec.Common.TypeConvert.Parse.ToDecimal(lblRemainAmount.Text);
                    if ((remainAmount - spinAmount.Value) < spinNextAmount.Value)
                    {
                        ErrorText = "Số tiền hẹn không được lớn hơn số tiền còn lại - số tiền thanh toán";
                        ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                        return valid;
                    }
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

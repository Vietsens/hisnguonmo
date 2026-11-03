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
using HIS.Desktop.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.PregnancyRest.Validation
{
    class ValidateHeinCardNumber : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.TextEdit txtControl;
        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool success = false;
            try
            {
                if (txtControl == null) return success;
                //So the BHYT chi duoc 15 hoac 17 ky tu (cong BHXH tu choi MA_THE sai do dai).
                //Dem tren so the da bo dau "-", "_", dau cach - dung gia tri se luu xuong DB.
                string heinCardNumber = HeinCardHelper.TrimHeinCardNumber(txtControl.Text);
                if (!string.IsNullOrEmpty(heinCardNumber) && heinCardNumber.Length != 15 && heinCardNumber.Length != 17)
                {
                    base.ErrorText = "Số thẻ BHYT phải có độ dài 15 hoặc 17 ký tự";
                    base.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                    return success;
                }
                success = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return success;
        }
    }
}

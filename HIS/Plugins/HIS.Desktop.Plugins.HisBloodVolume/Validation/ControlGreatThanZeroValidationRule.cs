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
using Inventec.Common.Logging;
using Inventec.Desktop.Common.Controls.ValidationRule;
using System;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisBloodVolume
{
    public class ControlGreatThanZeroOrLessThanThoundValidationRule : ValidationRule
    {

        internal DevExpress.XtraEditors.SpinEdit spin;

        public override bool Validate(Control control, object value)
        {
            bool valid = false;
            try
            {
                if (spin.Value <= 0 && spin.EditValue != null)
                {
                    this.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuKhongNhanGiaTriAm);
                    return valid;
                }
                if (spin.EditValue != null && spin.Value>999)
                {
                    this.ErrorText = "Trường dữ liệu chỉ được phép bé hơn 1000";
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

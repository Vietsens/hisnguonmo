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
using HIS.Desktop.LibraryMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HIS.Desktop.Plugins.InfusionCreate.Validation
{
    class TimeValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
     internal DevExpress.XtraEditors.DateEdit DateEdit1;
     internal DevExpress.XtraEditors.DateEdit DateEdit2;

        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool valid = false;
            try
            {
             if (DateEdit1 == null) return valid;
             if (DateEdit2 == null) return valid;
             if (DateEdit1.EditValue == null || DateEdit2.EditValue == null || Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateEdit2.DateTime)
 < Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(DateEdit1.DateTime))
                {
                    ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.NguoiDungNhapNgayKhongHopLe);
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

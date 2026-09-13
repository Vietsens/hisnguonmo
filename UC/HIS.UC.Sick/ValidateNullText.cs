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
using System.Windows.Forms;

namespace HIS.UC.Sick
{
    class ValidateNullText : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.TextEdit txt;

        public override bool Validate(Control control, object value)
        {
            bool valid = false;
            try
            {
                if (txt == null) return valid;
                if (String.IsNullOrEmpty(txt.Text.Trim()))
                {
                    this.ErrorText = "Trường dữ liệu bắt buộc";
                    this.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                    return valid;
                }
                                //Bo chan do dai co dinh cua ma BHXH: trong thoi gian chuyen doi, ma dinh danh y te
                //co the la 10 so (ma so BHXH cu) hoac 12 so (so DDCN/CCCD); du lieu cu con ban ghi
                //dai hon nen chi chan khi vuot do dai toi da cua cot.
                if (!String.IsNullOrEmpty(txt.Text.Trim()) && txt.Text.Trim().Length > 20)
                {
                    this.ErrorText = "Dữ liệu vượt quá độ dài cho phép 20 ký tự";
                    this.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
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

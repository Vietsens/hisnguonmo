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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.InfantInformation.Validation
{
    class ValidateMaxlength : ValidationRule
    {
        internal DevExpress.XtraEditors.BaseControl textEdit;
        internal int maxLength;
        internal bool isValid;
        public override bool Validate(Control control, object value)
        {
            bool valid = false;
            try
            {
                if (textEdit == null) return valid;
                if(isValid && String.IsNullOrEmpty(textEdit.Text.Trim()))
                {
                    this.ErrorText = "Trường dữ liệu vượt quá bắt buộc";
                    this.ErrorType = ErrorType.Warning;
                    return valid;
                }
                if (!String.IsNullOrEmpty(textEdit.Text.Trim()) && Inventec.Common.String.CountVi.Count(textEdit.Text.Trim()) > maxLength)
                {
                    this.ErrorText = "Trường dữ liệu vượt quá " + maxLength + " ký tự";
                    this.ErrorType = ErrorType.Warning;
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

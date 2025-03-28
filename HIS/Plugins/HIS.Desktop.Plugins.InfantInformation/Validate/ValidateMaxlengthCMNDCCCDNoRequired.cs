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
using DevExpress.XtraEditors.DXErrorProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.InfantInformation.Validate
{
    class ValidateMaxlengthCMNDCCCDNoRequired : ValidationRule
    {
        internal DevExpress.XtraEditors.BaseControl textEdit;

        public override bool Validate(Control control, object value)
        {
            bool valid = false;
            try
            {
                //if (textEdit == null) return valid;

                if (!String.IsNullOrEmpty(textEdit.Text) && ((textEdit.Text.Length < 9) || ((textEdit.Text.Length > 9) && (textEdit.Text.Length < 12)) || (textEdit.Text.Length > 12)))
                {

                    this.ErrorText = "Dữ liệu không đúng định dạng. CMND/CCCD phải có 9 hoặc 12 ký tự";
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

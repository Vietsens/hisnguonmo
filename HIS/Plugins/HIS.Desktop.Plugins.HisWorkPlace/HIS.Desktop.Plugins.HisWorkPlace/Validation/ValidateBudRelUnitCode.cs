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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors.DXErrorProvider;

namespace HIS.Desktop.Plugins.HisWorkPlace.Validtion
{
    public class ValidateBudRelUnitCode : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.BaseEdit textEdit;

        public override bool Validate(Control control, object value)
        {
            bool valid = false;
            try
            {
                if (textEdit == null) return valid;

                // Không b?t bu?c nh?p
                if (String.IsNullOrWhiteSpace(textEdit.Text))
                {
                    valid = true;
                    return valid;
                }

                string text = textEdit.Text.Trim();

                // Ki?m tra ?? dài t?i ?a 7 ký t?
                if (text.Length > 7)
                {
                    this.ErrorText = "V??t quá ?? dài t?i ?a (7)";
                    this.ErrorType = ErrorType.Warning;
                    return valid;
                }

                // Ki?m tra ch? ch?a s?
                if (!Regex.IsMatch(text, @"^\d+$"))
                {
                    this.ErrorText = "Ch? ???c nh?p s?";
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

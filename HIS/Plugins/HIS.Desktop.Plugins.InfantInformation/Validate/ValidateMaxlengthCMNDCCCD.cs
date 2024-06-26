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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.InfantInformation.Validation
{
    class ValidateMaxlengthCMNDCCCD : ValidationRule
    {
        internal DevExpress.XtraEditors.BaseControl textEdit;
        internal bool isValid;
        public override bool Validate(Control control, object value)
        {
            bool valid = false;
            try
            {
                if (textEdit == null) return valid;
                if (String.IsNullOrEmpty(textEdit.Text.Trim()) && isValid)
                {
                    this.ErrorText = "Trường dữ liệu bắt buộc";
                    this.ErrorType = ErrorType.Warning;
                    return valid;
                }
               
                if (!String.IsNullOrEmpty(textEdit.Text.Trim()) && IsNumber(textEdit.Text.Trim()) && textEdit.Text.Trim().Length == 9)
                {
                    //La CMND
                    return true;

                }
                else if (!String.IsNullOrEmpty(textEdit.Text.Trim()) && IsNumber(textEdit.Text.Trim()) && textEdit.Text.Trim().Length == 12)
                {
                    //La CCCD
                    return true;
                }
                else if (!String.IsNullOrEmpty(textEdit.Text.Trim()) && IsValid(textEdit.Text.Trim()) && textEdit.Text.Trim().Length < 10)
                {
                    //La passport
                    return true;
                }
                else if (!String.IsNullOrEmpty(textEdit.Text.Trim()))
                {
                    this.ErrorText = "CMND/CCCD/Hộ chiếu không đúng định dạng";
                    this.ErrorType = ErrorType.Warning;
                    return false;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return false;
        }

        private bool IsNumber(string pText)
        {
            Regex regex = new Regex(@"^\d+$");
            Inventec.Common.Logging.LogSystem.Debug("regex.IsMatch(pText)_" + regex.IsMatch(pText));
            return regex.IsMatch(pText);
        }

        private bool IsValid(string txtCMND)
        {
            bool valid = false;
            var txt = txtCMND;
            int countNumber = 0;
            int total = txt.Length;
            for (int i = 0; i < txt.Length; i++)
            {
                if (IsNumber(txt[i].ToString()))
                {
                    countNumber++;
                }
            }
            if (countNumber == 0)
            {
                valid = false;
            }
            else if (countNumber != 0 && countNumber < total)
            {
                valid = true;
            }
            return valid;
        }

        private bool CheckIsNumber(string pValue)
        {
            foreach (Char c in pValue)
            {
                if (!Char.IsDigit(c))
                    return false;
            }
            return true;
        }
    }
}

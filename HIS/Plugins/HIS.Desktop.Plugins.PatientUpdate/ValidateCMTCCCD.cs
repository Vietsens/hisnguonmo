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
using HIS.Desktop.LibraryMessage;
using System.Text.RegularExpressions;
namespace HIS.Desktop.Plugins.PatientUpdate
{
    class ValidateCMTCCCD : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        private const int MIN_LENGTH = 6;
        private const int MAX_LENGTH = 12;

        internal DevExpress.XtraEditors.TextEdit txtCmndNumber;

        public override bool Validate(Control control, object value)
        {
            bool valid = false;
            try
            {
                if (txtCmndNumber == null) return valid;
                if (!String.IsNullOrWhiteSpace(txtCmndNumber.Text))
                {
                    string cmndNumber = txtCmndNumber.Text.Trim();
                    Int64 k;
                    bool isNumeric = Int64.TryParse(cmndNumber, out k);

                    if (isNumeric == false && Regex.IsMatch(cmndNumber, @"^[\p{L}]+$"))
                    {
                        ErrorText = "CMND/CCCD/Hộ chiếu không đúng định dạng";
                        ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                        return valid;
                    }

                    if (isNumeric == false && !Regex.IsMatch(cmndNumber, @"^[0-9a-zA-Z]+$"))
                    {
                        ErrorText = "CMND/CCCD/Hộ chiếu không đúng định dạng";
                        ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                        return valid;
                    }

                    if (cmndNumber.Length < MIN_LENGTH || cmndNumber.Length > MAX_LENGTH)
                    {
                        ErrorText = string.Format("CMND/CCCD/Hộ chiếu phải từ {0} đến {1} ký tự", MIN_LENGTH, MAX_LENGTH);
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

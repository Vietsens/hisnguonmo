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

namespace HIS.Desktop.Plugins.MaterialTypeCreate.Validtion
{
    class HeinServiceTypeBhytValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.TextEdit txtHeinServiceTypeBhytCode;
        internal DevExpress.XtraEditors.TextEdit txtHeinServiceTypeBhytName;

        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool valid = false;
            try
            {
                if (!String.IsNullOrEmpty(txtHeinServiceTypeBhytCode.Text) && !String.IsNullOrEmpty(txtHeinServiceTypeBhytName.Text))
                {
                    if (Inventec.Common.String.CountVi.Count(txtHeinServiceTypeBhytCode.Text) > 100 && Inventec.Common.String.CountVi.Count(txtHeinServiceTypeBhytName.Text) > 500)
                    {
                        this.ErrorText = "Độ dài mã vượt quá " + 100 + "||" + "Độ dài tên vượt quá " + 500;
                        return valid;
                    }
                    else
                    {
                        if (Inventec.Common.String.CountVi.Count(txtHeinServiceTypeBhytCode.Text) > 100)
                        {
                            this.ErrorText = "Độ dài mã vượt quá " + 100;
                            return valid;
                        }
                        else if (Inventec.Common.String.CountVi.Count(txtHeinServiceTypeBhytName.Text) > 500)
                        {
                            this.ErrorText = "Độ dài tên vượt quá " + 500;
                            return valid;
                        }
                    }
                    
                }
                else if ((!String.IsNullOrEmpty(txtHeinServiceTypeBhytCode.Text) && String.IsNullOrEmpty(txtHeinServiceTypeBhytName.Text)))
                {
                    ErrorText = "Nhập mã BHYT thì cần nhập tên BHYT.";
                    ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                    return valid;
                }
                else if ((String.IsNullOrEmpty(txtHeinServiceTypeBhytCode.Text) && !String.IsNullOrEmpty(txtHeinServiceTypeBhytName.Text)))
                {
                    ErrorText = "Nhập tên BHYT thì cần nhập mã BHYT.";
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

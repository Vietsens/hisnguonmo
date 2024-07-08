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

namespace His.UC.UCHein.Design.TemplateHeinBHYT1.ValidationRule
{
    class TemplateHeinBHYT1__Address__ValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.TextEdit txtAddress;
        public override bool Validate(Control control, object value)
        {
            bool valid = false;
            try
            {
                if (txtAddress == null) return valid;
                if (txtAddress.Enabled && (String.IsNullOrEmpty(txtAddress.Text.Trim())))
                    return valid;

                if (txtAddress.Enabled && txtAddress != null && !string.IsNullOrEmpty(txtAddress.Text) && Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(txtAddress.Text, 500))
                {
                    this.ErrorText = "Trường dữ liệu vượt quá độ dài (" + 500 + " kí tự)";
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

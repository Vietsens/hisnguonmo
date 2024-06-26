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

namespace HIS.Desktop.Plugins.ImpMestCreate.Validation
{
    class TxtSeriNumberValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.ButtonEdit txtText;

        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool vaild = false;
            try
            {
                if (txtText == null) return vaild;
                if (txtText.Enabled)
                {
                    if (string.IsNullOrEmpty(txtText.Text)) return vaild;
                    if (Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(txtText.Text, 50))
                    {
                        this.ErrorText = "Trường dữ liệu vượt quá maxlength( " + 50 + " kí tự)";
                        return vaild;
                    }
                }
                vaild = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return vaild;
        }
    }
}

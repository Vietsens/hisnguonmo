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
using DevExpress.XtraEditors.DXErrorProvider;


namespace HIS.Desktop.Plugins.HisMediRecordType.Validates
{
    class ValidateMaxLength_GroupCode : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.TextEdit textedit;
        internal int? maxlength;
        public override bool Validate(Control control, object value)
        {
            bool validate = false;
            try
            {
                if (textedit == null) return validate;
                if (!String.IsNullOrEmpty(textedit.Text) && Inventec.Common.String.CountVi.Count(textedit.Text) > maxlength)
                {
                    this.ErrorText = "Trường dữ liệu vượt quá ký tự cho phép (" + maxlength + ")";
                    this.ErrorType = ErrorType.Warning;
                    return validate;
                }
                validate = true;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return validate;

        }
    }
}

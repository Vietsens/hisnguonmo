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
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisBranch
{
    class ValidateMaxLength : ValidationRule
    {
        internal TextEdit textEdit;
        internal int? maxLength;
        public override bool Validate(Control control, object value)
        {
            bool flag = false;
            bool result;
            try
            {
                if (this.textEdit == null)
                {
                    result = flag;
                    return result;
                }
                if (!string.IsNullOrEmpty(this.textEdit.Text) && Encoding.UTF8.GetByteCount(this.textEdit.Text) > this.maxLength)
                {
                    base.ErrorText = "Vượt quá độ dài cho phép (" + this.maxLength + ")";
                    base.ErrorType = ErrorType.Warning;
                    result = flag;
                    return result;
                }
                flag = true;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            result = flag;
            return result;
        }
    }
}

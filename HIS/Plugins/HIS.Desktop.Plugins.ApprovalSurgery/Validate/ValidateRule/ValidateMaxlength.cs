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

namespace HIS.Desktop.Plugins.ApprovalSurgery.Validate.ValidateRule
{
  public  class ValidateMaxLength : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.BaseEdit textEdit;
        internal int? maxLength;
        internal bool isRequired = false;

        public override bool Validate(Control control, object value)
        {
            bool valid = false;
            try
            {
                if (textEdit == null) return valid;

                if (isRequired && String.IsNullOrWhiteSpace(textEdit.Text))
                {
                    this.ErrorText = "Trường dữ liệu bắt buộc nhập";
                    this.ErrorType = ErrorType.Warning;
                    return valid;
                }
                if (maxLength != null && !String.IsNullOrEmpty(textEdit.Text) && Inventec.Common.String.CountVi.Count(textEdit.Text) > maxLength)
                {
                    this.ErrorText = "Vượt quá độ dài " + maxLength + " ký tự!";
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

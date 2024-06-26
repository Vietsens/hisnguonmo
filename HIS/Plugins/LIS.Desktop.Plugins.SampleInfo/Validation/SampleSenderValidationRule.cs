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
using System.Threading.Tasks;

namespace LIS.Desktop.Plugins.SampleInfo.Validation
{
    class SampleSenderValidationRule : ValidationRule
    {
        internal DevExpress.XtraEditors.TextEdit textEdit;
        internal int maxlength;
        internal bool isRequired = false ;

        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool valid = false;
            try
            {
                if (textEdit == null) return false;
                if (isRequired && (string.IsNullOrEmpty(textEdit.Text)))
                {
                    this.ErrorText = HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                    return false;
                }
                if (maxlength > 0 && !string.IsNullOrEmpty(textEdit.Text) && Inventec.Common.String.CheckString.IsOverMaxLengthUTF8(textEdit.Text, maxlength))
                {
                    this.ErrorText = "Trường dữ liệu vượt quá số ký tự tối đa( " + maxlength + " kí tự) có thể lưu!";
                    return false;
                }
                valid = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                valid = false;
            }
            return valid;
        }
    }
}

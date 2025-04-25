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

namespace HIS.Desktop.Plugins.ServiceReqList
{
    class ValidateMaxLength : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.MemoEdit memoEdit;
        internal int? maxLength;

        public override bool Validate(Control control, object value)
        {
            bool valid = false;
            try
            {
                if (memoEdit == null) return valid;

                if (String.IsNullOrEmpty(memoEdit.Text))
                {

                    this.ErrorText = "Trường dữ liệu bắt buộc";
                    return valid;
                }

                if (!String.IsNullOrEmpty(memoEdit.Text) && Encoding.UTF8.GetByteCount(memoEdit.Text) > maxLength)
                {
                    this.ErrorText = "Trường dữ liệu vượt quá ký tự cho phép";
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
    class ValidateMaxLength2 : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal int? maxLength;

        public override bool Validate(Control control, object value)
        {
            try
            {
                var memo = control as DevExpress.XtraEditors.MemoEdit;

               
                if (memo == null || !memo.Visible)
                    return true;

                string text = memo.Text?.Trim() ?? "";

                if (!string.IsNullOrEmpty(text) && Encoding.UTF8.GetByteCount(text) > maxLength)
                {
                    this.ErrorText = "Trường dữ liệu vượt quá ký tự cho phép";
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                this.ErrorText = "Lỗi kiểm tra dữ liệu";
                return false;
            }
        }
    }

}

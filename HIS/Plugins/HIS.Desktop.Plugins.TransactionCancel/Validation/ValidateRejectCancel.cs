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

namespace HIS.Desktop.Plugins.TransactionCancel
{
    class ValidateRejectCancel : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.MemoEdit memoEdit;

        public override bool Validate(Control control, object value)
        {
            bool valid = false;
            try
            {
                if (memoEdit == null) return valid;

                if (String.IsNullOrEmpty(memoEdit.Text.Trim()))
                {
                    this.ErrorText = "Bắt buộc nhập \"Lý do từ chối hủy\"";
                    return valid;
                }
                else if (Encoding.UTF8.GetByteCount(memoEdit.Text) > 500)
                {
                    this.ErrorText = "Trường dữ liệu vượt quá 500 ký tự";
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

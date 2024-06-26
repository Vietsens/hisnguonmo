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

namespace HIS.Desktop.Utility.ValidateRule
{
    public class CommonNumberEditValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        public DevExpress.XtraEditors.BaseEdit numberEdit;
        public bool isValidNull = true;
        public override bool Validate(Control control, object value)
        {
            bool valid = false;
            try
            {
                if (numberEdit == null) return valid;
                if (isValidNull && (numberEdit == null || string.IsNullOrEmpty(numberEdit.Text)))
                {
                    this.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                    return valid;
                }

                if ((!string.IsNullOrEmpty(numberEdit.Text)) && (numberEdit.Text.Length < 9 || numberEdit.Text.Length > 12))
                {
                    this.ErrorText = "Số điện thoại không hợp lệ";
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

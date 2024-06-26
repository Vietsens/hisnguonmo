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
using HIS.Desktop.LibraryMessage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ImportBlood.Validation
{
    class CMNDNumberValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.TextEdit txtCMND;
        internal bool isValid;
        public override bool Validate(Control control, object value)
        {
            bool valid = false;
            try
            {
                if (isValid && String.IsNullOrEmpty(txtCMND.Text))
                {
                    this.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                    return valid;
                }
                if (!String.IsNullOrEmpty(txtCMND.Text) && ((txtCMND.Text.Trim().Length != 9 && txtCMND.Text.Trim().Length != 12)
                                                            || (txtCMND.Text.Trim().Length == 9 && !txtCMND.Text.Trim().All(char.IsLetterOrDigit))
                                                            || (txtCMND.Text.Trim().Length == 12 && !txtCMND.Text.Trim().All(char.IsDigit))))
                {
                    this.ErrorText = "Độ dài thông tin phải là 9(ký tự chữ, số) hoặc 12(ký tự số)";
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

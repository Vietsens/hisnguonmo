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

namespace Inventec.Desktop.Common.Controls.ValidationRule
{
    public class ControlMaxLengthValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        public object editor;
        public IsValidControl isValidControl;
        public bool isUseOnlyCustomValidControl = false;
        public int? maxLength;
        public bool IsRequired { get; set; }// tiennv

        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool valid = false;
            try
            {
                if (editor == null) return valid;

                if (!isUseOnlyCustomValidControl)
                {
                    if (editor is DevExpress.XtraEditors.TextEdit)
                    {
                        if ((IsRequired && String.IsNullOrEmpty(((DevExpress.XtraEditors.TextEdit)editor).Text)))
                        {
                            this.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                            return valid;
                        }

                        if (!String.IsNullOrEmpty(((DevExpress.XtraEditors.TextEdit)editor).Text.Trim()) && Encoding.UTF8.GetByteCount(((DevExpress.XtraEditors.TextEdit)editor).Text.Trim()) > maxLength)
                        {
                            this.ErrorText = "Trường dữ liệu vượt quá ký tự cho phép";
                            return valid;
                        }
                    }
                    else if (editor is DevExpress.XtraEditors.MemoEdit)
                    {
                        if ((IsRequired && String.IsNullOrEmpty(((DevExpress.XtraEditors.MemoEdit)editor).Text)))
                        {
                            this.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                            return valid;
                        }

                        if (!String.IsNullOrEmpty(((DevExpress.XtraEditors.MemoEdit)editor).Text.Trim()) && Encoding.UTF8.GetByteCount(((DevExpress.XtraEditors.MemoEdit)editor).Text.Trim()) > maxLength)
                        {
                            this.ErrorText = "Trường dữ liệu vượt quá ký tự cho phép";
                            return valid;
                        }
                    }
                    else if (editor is DevExpress.XtraEditors.SpinEdit)
                    {
                        if ((IsRequired && String.IsNullOrEmpty(((DevExpress.XtraEditors.SpinEdit)editor).Text)))
                        {
                            this.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                            return valid;
                        }

                        if (!String.IsNullOrEmpty(((DevExpress.XtraEditors.SpinEdit)editor).Text.Trim()) && Encoding.UTF8.GetByteCount(((DevExpress.XtraEditors.SpinEdit)editor).Text.Trim()) > maxLength)
                        {
                            this.ErrorText = "Trường dữ liệu vượt quá ký tự cho phép";
                            return valid;
                        }
                    }
                }

                if (isValidControl != null && !isValidControl())
                {
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

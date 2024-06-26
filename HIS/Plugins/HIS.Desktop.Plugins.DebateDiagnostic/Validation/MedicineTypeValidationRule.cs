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
using Inventec.Desktop.Common.Controls.ValidationRule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.DebateDiagnostic
{
    internal class CustomControlValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal object editor;
        internal IsValidControl validEditorReference;
        internal IsValidControl isValidControl;
        internal bool isUseOnlyCustomValidControl = false;
        internal int? maxLength;
        internal bool IsRequired { get; set; }
        internal string messageError;

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
                        if (IsRequired && ((validEditorReference == null && String.IsNullOrEmpty(((DevExpress.XtraEditors.TextEdit)editor).Text)) || (validEditorReference != null && validEditorReference() == false)))
                        {
                            this.ErrorText = !String.IsNullOrEmpty(messageError) ? messageError : Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                            return valid;
                        }

                        if (!String.IsNullOrEmpty(((DevExpress.XtraEditors.TextEdit)editor).Text.Trim()) && Encoding.UTF8.GetByteCount(((DevExpress.XtraEditors.TextEdit)editor).Text.Trim()) > maxLength)
                        {
                            this.ErrorText = "Nhập quá kí tự cho phép [" + maxLength + "]";
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

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
using HIS.Desktop.Plugins.TrackingCreate.Resources;
using HIS.Desktop.LibraryMessage;

namespace HIS.Desktop.Plugins.TrackingCreate
{
    class MemoEditValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.MemoEdit txtTextEdit;

        public override bool Validate(Control control, object value)
        {
            bool valid = false;
            try
            {
                if (txtTextEdit == null)
                {
                    return valid;
                }
                if (String.IsNullOrWhiteSpace(txtTextEdit.Text))
                {
                    this.ErrorText = MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                    this.ErrorType = ErrorType.Warning;
                    return valid;
                }

                if (!String.IsNullOrEmpty(txtTextEdit.Text) && Encoding.UTF8.GetByteCount(txtTextEdit.Text) > 4000)
                {
                    this.ErrorText = string.Format(ResourceMessage.NhapQuaMaxlength, 4000);
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

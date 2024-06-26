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

namespace HIS.Desktop.Plugins.Library.TreatmentEndTypeExt.Validation
{
    class ValidateWorkPlace : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.TextEdit txtWorkPlace;
        internal DevExpress.XtraEditors.GridLookUpEdit cboWorkPlace;

        public override bool Validate(Control control, object value)
        {
            bool valid = false;
            try
            {
                if (!String.IsNullOrEmpty(txtWorkPlace.Text) || cboWorkPlace.EditValue != null)
                    return true;
                else if (!String.IsNullOrEmpty(txtWorkPlace.Text) && Encoding.UTF8.GetByteCount(txtWorkPlace.Text) > 100)
                    this.ErrorText = "Vượt quá độ dài cho phép (" + 100 + ")";
                else
                    this.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }
    }
}

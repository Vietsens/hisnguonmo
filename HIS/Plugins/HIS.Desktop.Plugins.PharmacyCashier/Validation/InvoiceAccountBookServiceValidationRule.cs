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

namespace HIS.Desktop.Plugins.PharmacyCashier.Validation
{
    class InvoiceAccountBookServiceValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal frmPharmacyCashier frmMain;
        internal DevExpress.XtraEditors.TextEdit txtInvoiceAccountBookService;
        internal DevExpress.XtraEditors.LookUpEdit cboInvoiceAccountBookService;

        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool valid = false;
            try
            {
                if (frmMain == null || txtInvoiceAccountBookService == null || cboInvoiceAccountBookService == null) return valid;
                if ((frmMain.listSereServAdo != null && frmMain.listSereServAdo.Any(a => a.IsInvoiced))
                    && (String.IsNullOrEmpty(txtInvoiceAccountBookService.Text) || cboInvoiceAccountBookService.EditValue == null))
                {
                    ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                    ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
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

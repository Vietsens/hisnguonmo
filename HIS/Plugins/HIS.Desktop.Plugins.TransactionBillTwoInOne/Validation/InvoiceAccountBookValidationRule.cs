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
using HIS.Desktop.Plugins.TransactionBillTwoInOne.ADO;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.TransactionBillTwoInOne.Validation
{
    class InvoiceAccountBookValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal List<VHisSereServADO> listData = new List<VHisSereServADO>();
        internal DevExpress.XtraEditors.TextEdit txtInvoiceAccountBookCode;
        internal DevExpress.XtraEditors.TextEdit cboInvoiceAccountBook;
        internal DevExpress.XtraEditors.CheckEdit checkNotInvoice;

        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool valid = false;
            try
            {
                if (txtInvoiceAccountBookCode == null || cboInvoiceAccountBook == null || checkNotInvoice == null) return valid;

                if (listData != null && listData.Count > 0 && (!checkNotInvoice.Checked))
                {
                    if (String.IsNullOrEmpty(txtInvoiceAccountBookCode.Text) || cboInvoiceAccountBook.EditValue == null)
                    {
                        ErrorText = Base.ResourceMessageLang.TruongDuLieuBatBuoc;
                        ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                        return valid;
                    }
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

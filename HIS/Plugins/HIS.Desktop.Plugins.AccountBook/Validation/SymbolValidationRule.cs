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

namespace HIS.Desktop.Plugins.HisAccountBookList.Validation
{
    class SymbolValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.TextEdit txtTemplateCode;
        internal DevExpress.XtraEditors.TextEdit txtSymbolCode;
        internal DevExpress.XtraEditors.GridLookUpEdit cboEInvoiceSys;

        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool valid = false;
            try
            {
                if (txtTemplateCode != null && txtSymbolCode != null && cboEInvoiceSys != null)
                {
                    if (!String.IsNullOrEmpty(txtSymbolCode.Text))
                    {
                        if (Inventec.Common.String.CountVi.Count(txtSymbolCode.Text) > 8)
                        {
                            this.ErrorText = "Độ dài mẫu số vượt quá " + 8 + " ký tự.";
                            return valid;
                        }
                    }
                    if (!String.IsNullOrEmpty(txtTemplateCode.Text) && String.IsNullOrEmpty(txtSymbolCode.Text)) return valid;
                    if (cboEInvoiceSys.EditValue != null && String.IsNullOrEmpty(txtSymbolCode.Text)
                        && Inventec.Common.TypeConvert.Parse.ToInt64(cboEInvoiceSys.EditValue.ToString()) != IMSys.DbConfig.HIS_RS.HIS_EINVOICE_TYPE.ID__BKAV) return valid;
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

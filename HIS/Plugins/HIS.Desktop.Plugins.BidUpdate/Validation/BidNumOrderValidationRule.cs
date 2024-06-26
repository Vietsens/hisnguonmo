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

namespace HIS.Desktop.Plugins.BidUpdate.Validation
{
    class BidNumOrderValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.TextEdit txtBidNumOrder;
        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool valid = false;
           
            try
            {
                if (txtBidNumOrder == null) return valid;

                if (string.IsNullOrEmpty(txtBidNumOrder.Text))
                {
                    this.ErrorText = "Trường dữ liệu bắt buộc";
                    return valid;
                }

                if (!string.IsNullOrEmpty(txtBidNumOrder.Text) && Encoding.UTF8.GetByteCount(txtBidNumOrder.Text.Trim()) > 50)
                {
                    this.ErrorText = "Trường dữ liệu nhập quá giới hạn";
                    return valid;
                }
                valid = true;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }
    }
}

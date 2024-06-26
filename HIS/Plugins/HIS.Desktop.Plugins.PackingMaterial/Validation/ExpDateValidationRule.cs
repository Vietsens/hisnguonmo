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
using DevExpress.XtraEditors.DXErrorProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.PackingMaterial.Validation
{
    class ExpDateValidationRule : ValidationRule
    {
        internal DevExpress.XtraEditors.DateEdit dtExpDate;

        public override bool Validate(Control control, object value)
        {
            bool valid = true;
            try
            {
                if (dtExpDate == null)
                    return false;
                if (dtExpDate.EditValue != null && dtExpDate.DateTime != DateTime.MinValue && dtExpDate.DateTime < DateTime.Now.Date)
                {
                    ErrorText = "Hạn sử dụng không được nhỏ hơn ngày hiện tại";
                    ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                    valid = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }
    }
}

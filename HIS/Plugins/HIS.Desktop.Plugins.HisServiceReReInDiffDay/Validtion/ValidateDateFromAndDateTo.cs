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
using DevExpress.XtraEditors.DXErrorProvider;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisServiceReReInDiffDay.Validtion
{
    class ValidateDateFromAndDateTo : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.GridLookUpEdit cboFrom;
        internal DevExpress.XtraEditors.GridLookUpEdit cboTo;

        public override bool Validate(Control control, object value)
        {
            bool valid = false;
            try
            {
                if (cboFrom == null || cboFrom == null) return valid;

                if (cboFrom.EditValue == null || cboTo.EditValue == null)
                {
                    valid = true;
                    return valid;
                }

                if (cboFrom.EditValue != null && cboTo.EditValue != null)
                {
                    long timeFrom = long.Parse(cboFrom.EditValue.ToString());
                    long timeTo = long.Parse(cboTo.EditValue.ToString());
                    if (timeFrom > timeTo)
                    {
                        this.ErrorText = "Ngày chỉ định từ bé hơn hoặc bằng ngày chỉ định đến";
                        this.ErrorType = ErrorType.Warning;
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

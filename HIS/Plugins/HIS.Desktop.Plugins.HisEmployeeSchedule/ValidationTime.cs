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

namespace HIS.Desktop.Plugins.HisEmployeeSchedule
{
    class ValidationTime : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.TimeSpanEdit tmFrom;
        internal DevExpress.XtraEditors.TimeSpanEdit tmTo;

        public override bool Validate(Control control, object value)
        {
            bool valid = false;
            try
            {
                if (tmFrom == null || tmTo == null) return valid;

                tmFrom.DeselectAll();
                tmTo.DeselectAll();

                if (tmFrom.EditValue == null || tmTo.EditValue == null)
                {
                    valid = true;
                    return valid;
                }

                if (tmFrom.EditValue != null && tmTo.EditValue != null)
                {
                    if (tmFrom.TimeSpan.Hours > tmTo.TimeSpan.Hours)
                    {
                        this.ErrorText = "Giờ từ nhỏ hơn giờ đến";
                        this.ErrorType = ErrorType.Warning;
                        return valid;
                    }
                    else if (tmFrom.TimeSpan.Hours == tmTo.TimeSpan.Hours && tmFrom.TimeSpan.Minutes >= tmTo.TimeSpan.Minutes)
                    {
                        this.ErrorText = "Giờ từ nhỏ hơn giờ đến";
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

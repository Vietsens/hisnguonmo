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
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAR.Desktop.Plugins.SarReportType.Validation
{
    class ControlTwoHourValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal GridLookUpEdit HourFrom;
        internal GridLookUpEdit HourTo;

        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool valid = false;
            try
            {
                if (HourFrom == null) return valid;
                if (HourTo == null) return valid;
                if (HourFrom.EditValue != null && HourTo.EditValue != null)
                {
                    if (long.Parse(HourFrom.EditValue.ToString()) > long.Parse(HourTo.EditValue.ToString()))
                    {
                        return valid;
                    }
                }
                else if (HourFrom.EditValue != null && HourTo.EditValue == null)
                {
                    return valid;
                }
                else if (HourFrom.EditValue == null && HourTo.EditValue != null)
                {
                    return valid;
                }

                valid = true;
            }
            catch (Exception ex)
            {
                valid = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return valid;
        }
    }
}

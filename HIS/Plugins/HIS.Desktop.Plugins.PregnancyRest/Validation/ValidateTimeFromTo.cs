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

namespace HIS.Desktop.Plugins.PregnancyRest.Validation
{
    class ValidateTimeFromAndTo : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.DateEdit dtTimeFrom;
        internal DevExpress.XtraEditors.DateEdit dtTimeTo;

        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool success = false;
            try
            {
                if (dtTimeFrom == null || dtTimeTo == null) return success;

                if (dtTimeFrom.EditValue == null || dtTimeFrom.DateTime == DateTime.MinValue)
                    return success;

                if (dtTimeTo.EditValue != null && dtTimeTo.DateTime != DateTime.MinValue && dtTimeTo.DateTime < dtTimeFrom.DateTime)
                {
                    this.ErrorText = "Thời gian từ không được lớn hơn thời gian đến";
                    return success;
                }
                success = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return success;
        }
    }
}

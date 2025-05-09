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

namespace HIS.Desktop.Plugins.ServiceExecute.Validation
{
    class BeginTimeValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.DateEdit dtBeginTime;
        internal DevExpress.XtraEditors.DateEdit dtEndTime;
        internal long IntructionTime;

        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool vaild = false;
            try
            {
                if (dtBeginTime == null) return vaild;

                if (dtBeginTime.EditValue != null)
                {
                    if (Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtBeginTime.DateTime) < IntructionTime)
                    {
                        this.ErrorText = "Thời gian bắt đầu không được nhỏ hơn thời gian y lệnh";
                        return vaild;
                    }

                    if (dtEndTime != null && dtEndTime.EditValue != null && Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtBeginTime.DateTime) > Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtEndTime.DateTime))
                    {
                        this.ErrorText = "Thời gian bắt đầu không được lớn hơn thời gian kết thúc";
                        return vaild;
                    }
                }

                vaild = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return vaild;
        }
    }
}

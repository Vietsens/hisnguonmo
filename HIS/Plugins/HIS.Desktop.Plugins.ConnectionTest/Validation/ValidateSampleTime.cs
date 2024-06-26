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
using HIS.Desktop.Plugins.ConnectionTest.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.ConnectionTest.Validation
{
    class ValidateSampleTime : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.DateEdit dtTime;
        internal long intructionTime;

        public override bool Validate(Control control, object value)
        {
            bool valid = false;
            try
            {
                if (dtTime == null && dtTime == null) return valid;
                if ((HisConfigCFG.StartTimeMustBeGreaterThanInstructionTime == "1" || HisConfigCFG.StartTimeMustBeGreaterThanInstructionTime == "2") && dtTime.EditValue != null && dtTime.DateTime != DateTime.MinValue && Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtTime.DateTime) < intructionTime)
                {
                    ErrorText = string.Format("Thời gian duyệt mẫu không được nhỏ hơn thời gian y lệnh: {0}.", Inventec.Common.DateTime.Convert.TimeNumberToTimeString(intructionTime));
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

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

namespace HIS.Desktop.Plugins.RehaServiceReqExecute
{
    class RehaTrain__TrainTimeValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.DateEdit dtTrainTime;

        public override bool Validate(Control control, object value)
        {
            bool valid = false;
            try
            {
                if (dtTrainTime == null) return valid;
                if (String.IsNullOrEmpty(dtTrainTime.Text))
                    return valid;
                string date = string.Format("{0:00}", Inventec.Common.TypeConvert.Parse.ToInt32(dtTrainTime.Text)) + "" + string.Format("{0:00}", Inventec.Common.TypeConvert.Parse.ToInt32(dtTrainTime.Text)) + "" + string.Format("{0:00}", Inventec.Common.TypeConvert.Parse.ToInt32(dtTrainTime.Text)) + "000000";
                //string date = Inventec.Common.DateTime.Convert.SystemDateTimeToDateString(dtRequestDate_ExamPage);
                valid = Inventec.Common.DateTime.Check.IsValidTime(date);
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

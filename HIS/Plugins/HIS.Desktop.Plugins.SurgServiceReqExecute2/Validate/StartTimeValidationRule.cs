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

namespace HIS.Desktop.Plugins.SurgServiceReqExecute2.Validate
{
    class StartTimeValidationRule :
DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.DateEdit startTime;
        internal DevExpress.XtraEditors.DateEdit finishTime;
        internal long instructionTime;
        //internal long treatmentOutTime;
        internal bool keyCheck;//#19893
        //internal bool keyCheckStatsTime;//#20201

        public override bool Validate(Control control, object value)
        {
            bool valid = true;
            try
            {
                if (startTime.EditValue == null)
                {
                    this.ErrorText = "Trường dữ liệu bắt buộc nhập";
                    return false;
                }
                List<string> errMess = new List<string>();
                long timeStart = startTime.EditValue != null ? Inventec.Common.TypeConvert.Parse.ToInt64(startTime.DateTime.ToString("yyyyMMddHHmm") + "00") : 0;
                if (!keyCheck)
                    if (timeStart < instructionTime)
                    {
                        errMess.Add("Thời gian bắt đầu phải lớn hơn thời gian y lệnh");
                        valid = false;
                    }
                if (finishTime.EditValue != null && startTime.DateTime > finishTime.DateTime)
                {
                    errMess.Add("Thời gian bắt đầu không được lớn hơn thời gian kết thúc");
                    valid = false;
                }
                ////#20113
                //if (!keyCheckStatsTime)
                //{
                //    if (startTime.EditValue != null && startTime.DateTime > DateTime.Now)
                //    {
                //        errMess.Add("Thời gian kết thúc không được lớn hơn thời gian hệ thống");
                //        valid = false;
                //    }
                //}

                //if (treatmentOutTime > 0 && timeStart > treatmentOutTime)
                //{
                //    errMess.Add(String.Format("Thời gian bắt đầu không được lớn hơn thời gian ra viện"));
                //    valid = false;
                //}

                if (!valid)
                {
                    this.ErrorText = String.Join(";", errMess);
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

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

namespace HIS.UC.FormType.TimeFromTo.Validation
{
    class ToTimeValidationRule : DevExpress.XtraEditors.DXErrorProvider.ValidationRule
    {
        internal DevExpress.XtraEditors.DateEdit dtToTime;
        internal DevExpress.XtraEditors.DateEdit dtFromTime;
        public override bool Validate(System.Windows.Forms.Control control, object value)
        {
            bool valid = false;
            try
            {
                if (dtToTime == null) return valid;
                if (dtToTime.EditValue == null || dtToTime.DateTime == System.DateTime.MinValue) return valid;
                if (dtFromTime.EditValue != null && dtFromTime.DateTime != System.DateTime.MinValue)
                {
                    if (dtFromTime.DateTime > dtToTime.DateTime)
                    {
                        this.ErrorText = "Thời gian không hợp lệ.";
                        return valid;
                    }

                    //System.DateTime WarningTimeSaFrom = new System.DateTime(System.DateTime.Now.Year, System.DateTime.Now.Month, System.DateTime.Now.Day, 8, 00, 00);
                    //System.DateTime WarningTimeSaTo = new System.DateTime(System.DateTime.Now.Year, System.DateTime.Now.Month, System.DateTime.Now.Day, 11, 30, 00);

                    //System.DateTime WarningTimeChFrom = new System.DateTime(System.DateTime.Now.Year, System.DateTime.Now.Month, System.DateTime.Now.Day, 14, 00, 00);
                    //System.DateTime WarningTimeChTo = new System.DateTime(System.DateTime.Now.Year, System.DateTime.Now.Month, System.DateTime.Now.Day, 11, 30, 00);

                    //if ((dtToTime.DateTime - dtFromTime.DateTime).TotalDays > 25 &&
                    //    ((System.DateTime.Now >= WarningTimeSaFrom && System.DateTime.Now <= WarningTimeSaTo) ||
                    //    (System.DateTime.Now >= WarningTimeChFrom && System.DateTime.Now <= WarningTimeChTo)))
                    //{
                    //    this.ErrorText = "Thời gian lấy báo cáo lớn vui lòng tạo báo cáo ngoài giờ cao điểm";
                    //    return valid;
                    //}
                }

                valid = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                valid = false;
            }
            return valid;
        }
    }
}

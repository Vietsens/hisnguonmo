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

namespace HIS.Desktop.Plugins.RegisterVaccination
{
    class CalculatePatientAge
    {
        internal class AgeObject
        {
            public AgeObject() { }
            public int AgeType { get; set; }
            public string OutDate { get; set; }
            public System.DateTime OutDateDate { get; set; }

        }

        internal static AgeObject Calculate(long dob)
        {
            AgeObject result = new AgeObject();
            try
            {
                System.DateTime dtNgSinh = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(dob).Value;
                result.OutDateDate = dtNgSinh;
                if (dtNgSinh == System.DateTime.MinValue) throw new ArgumentNullException("dtNgSinh");

                TimeSpan diff__hour = (System.DateTime.Now - dtNgSinh);
                TimeSpan diff__month = (System.DateTime.Now.Date - dtNgSinh.Date);

                //- Dưới 24h: tính chính xác đến giờ.
                double hour = diff__hour.TotalHours;
                if (hour < 24)
                {
                    result.AgeType = 4;
                    result.OutDate = ((int)hour + "");
                }
                else
                {
                    long tongsogiay__hour = diff__hour.Ticks;
                    System.DateTime newDate__hour = new System.DateTime(tongsogiay__hour);
                    int month__hour = ((newDate__hour.Year - 1) * 12 + newDate__hour.Month - 1);
                    if (month__hour == 0)
                    {
                        //Nếu Bn trên 24 giờ và dưới 1 tháng tuổi => hiển thị "xyz ngày tuổi"
                        result.AgeType = 3;
                        result.OutDate = ((int)diff__month.TotalDays + "");
                    }
                    else
                    {
                        long tongsogiay = diff__month.Ticks;
                        System.DateTime newDate = new System.DateTime(tongsogiay);
                        int month = ((newDate.Year - 1) * 12 + newDate.Month - 1);
                        if (month == 0)
                        {
                            //Nếu Bn trên 24 giờ và dưới 1 tháng tuổi => hiển thị "xyz ngày tuổi"
                            result.OutDate = ((int)diff__month.TotalDays + "");
                            result.AgeType = 3;
                        }
                        else
                        {
                            //- Dưới 72 tháng tuổi: tính chính xác đến tháng như hiện tại
                            if (month < 72)
                            {
                                result.OutDate = (month + "");
                                result.AgeType = 2;
                            }
                            //- Trên 72 tháng tuổi: tính chính xác đến năm: tuổi= năm hiện tại - năm sinh
                            else
                            {
                                int year = System.DateTime.Now.Year - dtNgSinh.Year;
                                result.OutDate = (year + "");
                                result.AgeType = 1;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
    }
}

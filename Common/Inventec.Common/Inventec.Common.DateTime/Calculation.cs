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

namespace Inventec.Common.DateTime
{
    public class Calculation
    {
        public static int DifferenceMonth(long timeBefore, long timeAfter)
        {
            int result = -1;
            try
            {
                System.DateTime? dateBefore = Convert.TimeNumberToSystemDateTime(timeBefore);
                System.DateTime? dateAfter = Convert.TimeNumberToSystemDateTime(timeAfter);
                if (dateBefore != null && dateAfter != null)
                {
                    result = ((dateAfter.Value.Year - dateBefore.Value.Year) * 12) + dateAfter.Value.Month - dateBefore.Value.Month;
                }
            }
            catch (Exception ex)
            {
                result = -1;
            }
            return result;
        }

        public static int DifferenceDate(long timeBefore, long timeAfter)
        {
            int result = -1;
            try
            {
                System.DateTime? dateBefore = Convert.TimeNumberToSystemDateTime(timeBefore);
                System.DateTime? dateAfter = Convert.TimeNumberToSystemDateTime(timeAfter);
                if (dateBefore != null && dateAfter != null)
                {
                    TimeSpan difference = dateAfter.Value - dateBefore.Value;
                    result = (int)difference.TotalDays;
                }
            }
            catch (Exception ex)
            {
                result = -1;
            }
            return result;
        }

        public enum UnitDifferenceTime
        {
            DAY,
            HOUR,
            MINUTE,
            SECOND,
            MILISECOND,
        }

        public static int DifferenceTime(long timeBefore, long timeAfter, UnitDifferenceTime unit)
        {
            int result = -1;
            try
            {
                System.DateTime? dateBefore = Convert.TimeNumberToSystemDateTime(timeBefore);
                System.DateTime? dateAfter = Convert.TimeNumberToSystemDateTime(timeAfter);
                if (dateBefore != null && dateAfter != null)
                {
                    TimeSpan difference = dateAfter.Value - dateBefore.Value;
                    switch (unit)
                    {
                        case UnitDifferenceTime.DAY:
                            result = (int)difference.TotalDays;
                            break;
                        case UnitDifferenceTime.HOUR:
                            result = (int)difference.TotalHours;
                            break;
                        case UnitDifferenceTime.MINUTE:
                            result = (int)difference.TotalMinutes;
                            break;
                        case UnitDifferenceTime.SECOND:
                            result = (int)difference.TotalSeconds;
                            break;
                        case UnitDifferenceTime.MILISECOND:
                            result = (int)difference.TotalMilliseconds;
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                result = -1;
            }
            return result;
        }

        public static int Age(long dob)
        {
            int result = -1;
            try
            {
                System.DateTime? sysDob = Convert.TimeNumberToSystemDateTime(dob);
                if (sysDob != null)
                {
                    System.DateTime today = System.DateTime.Today;
                    result = today.Year - sysDob.Value.Year;
                    if (today < sysDob.Value.AddYears(result))
                    {
                        result--;
                    }
                }
            }
            catch (Exception ex)
            {
                result = -1;
            }
            return result;
        }

        public static long? Add(long time, double value, UnitDifferenceTime unit)
        {
            long? result = null;
            try
            {
                System.DateTime? sysDateTime = Convert.TimeNumberToSystemDateTime(time);
                if (sysDateTime != null)
                {
                    switch (unit)
                    {
                        case UnitDifferenceTime.DAY:
                            sysDateTime = sysDateTime.Value.AddDays(value);
                            break;
                        case UnitDifferenceTime.HOUR:
                            sysDateTime = sysDateTime.Value.AddHours(value);
                            break;
                        case UnitDifferenceTime.MINUTE:
                            sysDateTime = sysDateTime.Value.AddMinutes(value);
                            break;
                        case UnitDifferenceTime.SECOND:
                            sysDateTime = sysDateTime.Value.AddSeconds(value);
                            break;
                        case UnitDifferenceTime.MILISECOND:
                            sysDateTime = sysDateTime.Value.AddMilliseconds(value);
                            break;
                        default:
                            break;
                    }
                    result = Convert.SystemDateTimeToTimeNumber(sysDateTime.Value);
                }
            }
            catch (Exception ex)
            {
                result = null;
            }
            return result;
        }

        public static string AgeCaption(long dob)
        {
            string result = System.String.Empty;
            try
            {
                //string caption__Tuoi = "tuổi";
                string caption__ThangTuoi = "tháng tuổi";
                string caption__GioTuoi = "giờ tuổi";
                if (dob > 0)
                {
                    System.DateTime dtNgSinh = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(dob).Value;
                    if (dtNgSinh == System.DateTime.MinValue) throw new ArgumentNullException("dtNgSinh");

                    TimeSpan diff__hour = (System.DateTime.Now - dtNgSinh);
                    TimeSpan diff__month = (System.DateTime.Now.Date - dtNgSinh.Date);

                    //- Dưới 24h: tính chính xác đến giờ.
                    double hour = diff__hour.TotalHours;
                    if (hour < 24)
                    {
                        result = ((int)hour + " " + caption__GioTuoi);
                    }
                    else
                    {
                        long tongsogiay = diff__month.Ticks;
                        System.DateTime newDate = new System.DateTime(tongsogiay);
                        int month = ((newDate.Year - 1) * 12 + newDate.Month - 1);

                        //- Dưới 72 tháng tuổi: tính chính xác đến tháng như hiện tại
                        if (month < 72)
                        {
                            result = (month + " " + caption__ThangTuoi);
                        }
                        //- Trên 72 tháng tuổi: tính chính xác đến năm: tuổi= năm hiện tại - năm sinh
                        else
                        {
                            int year = System.DateTime.Now.Year - dtNgSinh.Year;
                            result = year + " ";//+ caption__Tuoi);
                        }
                    }
                }
            }
            catch (Exception)
            {
                result = System.String.Empty;
            }
            return result;
        }

        /// <summary>
        /// Tính tuổi đến thời gian
        /// Tính tuổi theo năm
        /// </summary>
        /// <param name="dob">ngày tháng năm sinh</param>
        /// <param name="inTime">thời điểm cần tính tuổi</param>
        /// <returns></returns>
        public static int Age(long dob, long inTime)
        {
            int result = -1;
            try
            {
                System.DateTime? sysDob = Convert.TimeNumberToSystemDateTime(dob);
                System.DateTime? sysIn = Convert.TimeNumberToSystemDateTime(inTime);
                if (sysDob.HasValue)
                {
                    if (sysIn.HasValue)
                    {
                        result = sysIn.Value.Year - sysDob.Value.Year;
                        if (sysIn.Value < sysDob.Value.AddYears(result))
                        {
                            result--;
                        }
                    }
                    else
                    {
                        System.DateTime today = System.DateTime.Today;
                        result = today.Year - sysDob.Value.Year;
                        if (today < sysDob.Value.AddYears(result))
                        {
                            result--;
                        }
                    }
                }
            }
            catch (Exception)
            {
                result = -1;
            }
            return result;
        }

        /// <summary>
        /// Công thức tính tuổi theo biểu mẫu excel
        /// </summary>
        /// <param name="dob">Ngày tháng năm sinh</param>
        /// <param name="caption__Tuoi">tuổi </param>
        /// <param name="caption__ThangTuoi">tháng tuổi</param>
        /// <param name="caption__NgayTuoi">ngày tuổi</param>
        /// <param name="caption__GioTuoi">giờ tuổi</param>
        /// <param name="time">thời điểm tính tuổi</param>
        /// <returns></returns>
        public static string AgeString(long dob, string caption__Tuoi, string caption__ThangTuoi, string caption__NgayTuoi, string caption__GioTuoi, long? time = null)
        {
            string result = string.Empty;
            try
            {
                string tuoi = !string.IsNullOrWhiteSpace(caption__Tuoi) ? caption__Tuoi : "tuổi";
                string thangTuoi = !string.IsNullOrWhiteSpace(caption__ThangTuoi) ? caption__ThangTuoi : "tháng tuổi";
                string ngayTuoi = !string.IsNullOrWhiteSpace(caption__NgayTuoi) ? caption__NgayTuoi : "ngày tuổi";
                string gioTuoi = !string.IsNullOrWhiteSpace(caption__GioTuoi) ? caption__GioTuoi : "giờ tuổi";

                if (dob > 0)
                {
                    System.DateTime dtNgSinh = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(dob).Value;
                    if (dtNgSinh == System.DateTime.MinValue) throw new ArgumentNullException("dtNgSinh");

                    System.DateTime dtNow = System.DateTime.Now;

                    if (time.HasValue && time.Value > 0)
                    {
                        dtNow = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(time.Value).Value;
                        if (dtNow == System.DateTime.MinValue) throw new ArgumentNullException("dtNow");
                    }

                    TimeSpan diff__hour = (dtNow - dtNgSinh);
                    TimeSpan diff__month = (dtNow.Date - dtNgSinh.Date);

                    //- Dưới 24h: tính chính xác đến giờ.
                    double hour = diff__hour.TotalHours;
                    if (hour < 24)
                    {
                        result = ((int)hour + " " + gioTuoi);
                    }
                    else
                    {
                        long tongsogiay__hour = diff__hour.Ticks;
                        System.DateTime newDate__hour = new System.DateTime(tongsogiay__hour);
                        int month__hour = ((newDate__hour.Year - 1) * 12 + newDate__hour.Month - 1);
                        if (!string.IsNullOrWhiteSpace(ngayTuoi) && month__hour == 0)
                        {
                            //Nếu Bn trên 24 giờ và dưới 1 tháng tuổi => hiển thị "xyz ngày tuổi"
                            result = ((int)diff__month.TotalDays + " " + ngayTuoi);
                        }
                        else
                        {
                            long tongsogiay = diff__month.Ticks;
                            System.DateTime newDate = new System.DateTime(tongsogiay);
                            int month = ((newDate.Year - 1) * 12 + newDate.Month - 1);
                            if (month == 0)
                            {
                                //Nếu Bn trên 24 giờ và dưới 1 tháng tuổi => hiển thị "xyz ngày tuổi"
                                result = ((int)diff__month.TotalDays + " " + ngayTuoi);
                            }
                            else
                            {
                                //- Dưới 72 tháng tuổi: tính chính xác đến tháng như hiện tại
                                if (month < 72)
                                {
                                    result = (month + " " + thangTuoi);
                                }
                                //- Trên 72 tháng tuổi: tính chính xác đến năm: tuổi= năm hiện tại - năm sinh
                                else
                                {
                                    int year = System.DateTime.Now.Year - dtNgSinh.Year;
                                    result = (year + " " + tuoi);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = string.Empty;
            }
            return result;
        }
    }
}

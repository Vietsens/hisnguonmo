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
using System.Text;

namespace Inventec.Common.DateTime
{
    public class Convert
    {
        private const string DATE_SEPARATE = "Ngày     tháng     năm       ";
        private const string MONTH_SEPARATE = "Tháng     năm       ";
    
        public static string TimeNumberToMonthString(long time)
        {
            string result = null;
            try
            {
                string temp = time.ToString();
                if (temp != null && temp.Length >= 8)
                {
                    result = new StringBuilder().Append(temp.Substring(4, 2)).Append("/").Append(temp.Substring(0, 4)).ToString();
                }
            }
            catch (Exception ex)
            {
                result = null;
            }
            return result;
        }

        public static string TimeNumberToDateString(long time)
        {
            string result = null;
            try
            {
                result = TimeNumberToDateString(time.ToString());
            }
            catch (Exception ex)
            {
                result = null;
            }
            return result;
        }

        public static string TimeNumberToDateString(string time)
        {
            string result = null;
            try
            {
                if (time != null && time.Length >= 8)
                {
                    result = new StringBuilder().Append(time.Substring(6, 2)).Append("/").Append(time.Substring(4, 2)).Append("/").Append(time.Substring(0, 4)).ToString();
                }
            }
            catch (Exception ex)
            {
                result = null;
            }
            return result;
        }

        public static string TimeNumberToTimeString(long time)
        {
            string result = null;
            try
            {
                string temp = time.ToString();
                if (temp != null && temp.Length >= 14)
                {
                    result = new StringBuilder().Append(temp.Substring(6, 2)).Append("/").Append(temp.Substring(4, 2)).Append("/").Append(temp.Substring(0, 4)).Append(" ").Append(temp.Substring(8, 2)).Append(":").Append(temp.Substring(10, 2)).Append(":").Append(temp.Substring(12, 2)).ToString();
                }
            }
            catch (Exception ex)
            {
                result = null;
            }
            return result;
        }

        public static string TimeNumberToTimeStringWithoutSecond(long time)
        {
            string result = null;
            try
            {
                string temp = time.ToString();
                if (temp != null && temp.Length >= 14)
                {
                    result = new StringBuilder().Append(temp.Substring(6, 2)).Append("/").Append(temp.Substring(4, 2)).Append("/").Append(temp.Substring(0, 4)).Append(" ").Append(temp.Substring(8, 2)).Append(":").Append(temp.Substring(10, 2)).ToString();
                }
            }
            catch (Exception ex)
            {
                result = null;
            }
            return result;
        }

        public static string SystemDateTimeToDateString(System.DateTime? dateTime)
        {
            string result = null;
            try
            {
                if (dateTime.HasValue)
                {
                    long? time = SystemDateTimeToTimeNumber(dateTime);
                    if (time.HasValue)
                    {
                        result = TimeNumberToDateString(time.Value);
                    }
                }
                else
                {
                    result = DATE_SEPARATE;
                }
            }
            catch (Exception ex)
            {
                result = DATE_SEPARATE;
            }
            return result;
        }

        public static string TimeNumberToDateStringSeparateString(long time)
        {
            string result = null;
            try
            {
                string temp = time.ToString();
                if (temp != null && temp.Length >= 14)
                {
                    result = new StringBuilder().Append("Ngày ").Append(temp.Substring(6, 2)).Append(" tháng ").Append(temp.Substring(4, 2)).Append(" năm ").Append(temp.Substring(0, 4)).ToString();
                }
            }
            catch (Exception ex)
            {
                result = null;
            }
            return result;
        }


        public static string SystemDateTimeToDateSeparateString(System.DateTime? dateTime)
        {
            string result = DATE_SEPARATE;
            try
            {
                if (dateTime.HasValue)
                {
                    result = new StringBuilder().Append("Ngày ").Append(dateTime.Value.Day).Append(" tháng ").Append(dateTime.Value.Month).Append(" năm ").Append(dateTime.Value.Year).ToString();
                }
                else
                {
                    result = DATE_SEPARATE;
                }
            }
            catch (Exception ex)
            {
                result = DATE_SEPARATE;
            }
            return result;
        }

        public static string SystemDateTimeToTimeSeparateString(System.DateTime? dateTime)
        {
            string result = DATE_SEPARATE;
            try
            {
                if (dateTime.HasValue)
                {
                    result = new StringBuilder().Append("Ngày ").Append(dateTime.Value.Day).Append(" tháng ").Append(dateTime.Value.Month).Append(" năm ").Append(dateTime.Value.Year).Append(" ").Append(dateTime.Value.Hour).Append(" giờ ").Append(dateTime.Value.Minute).Append(" phút ").Append(dateTime.Value.Second).Append(" giây ").ToString();
                }
                else
                {
                    result = DATE_SEPARATE;
                }
            }
            catch (Exception ex)
            {
                result = DATE_SEPARATE;
            }
            return result;
        }

        public static string SystemDateTimeToMonthSeparateString(System.DateTime? dateTime)
        {
            string result = MONTH_SEPARATE;
            try
            {
                if (dateTime.HasValue)
                {
                    result = new StringBuilder().Append("Tháng ").Append(dateTime.Value.Month).Append(" năm ").Append(dateTime.Value.Year).ToString();
                }
                else
                {
                    result = MONTH_SEPARATE;
                }
            }
            catch (Exception ex)
            {
                result = MONTH_SEPARATE;
            }
            return result;
        }

        public static long? SystemDateTimeToTimeNumber(System.DateTime? dateTime)
        {
            long? result = null;
            try
            {
                if (dateTime.HasValue)
                {
                    result = long.Parse(dateTime.Value.ToString("yyyyMMddHHmmss"));
                }
            }
            catch (Exception ex)
            {
                result = null;
            }
            return result;
        }

        public static System.DateTime? TimeNumberToSystemDateTime(long time)
        {
            System.DateTime? result = null;
            try
            {
                if (time > 0)
                {
                    result = System.DateTime.ParseExact(time.ToString(), "yyyyMMddHHmmss",
                                       System.Globalization.CultureInfo.InvariantCulture);
                }
            }
            catch (Exception ex)
            {
                result = null;
            }
            return result;
        }

        public static System.DateTime? TimeNumberToSystemDateTimeUTC(long time)
        {
            System.DateTime? result = null;
            try
            {
                if (time > 0)
                {
                    System.DateTime date = System.DateTime.ParseExact(time.ToString(), "yyyyMMddHHmmss",
                                       System.Globalization.CultureInfo.InvariantCulture);
                    result = System.DateTime.SpecifyKind(date, DateTimeKind.Utc);
                }
            }
            catch (Exception ex)
            {
                result = null;
            }
            return result;
        }
    }
}

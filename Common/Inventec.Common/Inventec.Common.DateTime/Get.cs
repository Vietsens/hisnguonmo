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
    public class Get
    {
        private const string START_TIME = "000000";
        private const string END_TIME = "235959";

        public static long? Now()
        {
            long? result = null;
            try
            {
                result = Int64.Parse(System.DateTime.Now.ToString("yyyyMMddHHmmss"));
            }
            catch (Exception ex)
            {
                result = null;
            }
            return result;
        }

        public static string NowAsTimeString()
        {
            string result = null;
            try
            {
                long? now = Now();
                if (now.HasValue)
                {
                    result = Convert.TimeNumberToTimeString(now.Value);
                }
            }
            catch (Exception ex)
            {
                result = null;
            }
            return result;
        }

        public static string NowAsDateString()
        {
            string result = null;
            try
            {
                long? now = Now();
                if (now.HasValue)
                {
                    result = Convert.TimeNumberToDateString(now.Value);
                }
            }
            catch (Exception ex)
            {
                result = null;
            }
            return result;
        }

        public static string NowAsDateSeparateString()
        {
            string result = null;
            try
            {
                result = Convert.SystemDateTimeToDateSeparateString(System.DateTime.Now);
            }
            catch (Exception ex)
            {

                result = null;
            }
            return result;
        }

        public static string NowAsMonthString()
        {
            string result = null;
            try
            {
                long? now = Now();
                if (now.HasValue)
                {
                    result = Convert.TimeNumberToMonthString(now.Value);
                }
            }
            catch (Exception ex)
            {

                result = null;
            }
            return result;
        }

        public static string NowAsMonthSeparateString()
        {
            string result = null;
            try
            {
                result = Convert.SystemDateTimeToMonthSeparateString(System.DateTime.Now);
            }
            catch (Exception ex)
            {

                result = null;
            }
            return result;
        }

        public static long? StartDay()
        {
            long? result = null;
            try
            {
                result = Int64.Parse(System.DateTime.Now.ToString("yyyyMMdd") + START_TIME);
            }
            catch (Exception ex)
            {

                result = null;
            }
            return result;
        }

        public static long? StartDay(long timeNumber)
        {
            long? result = null;
            try
            {
                System.DateTime datetime = Convert.TimeNumberToSystemDateTime(timeNumber).Value;
                result = Int64.Parse(datetime.ToString("yyyyMMdd") + START_TIME);
            }
            catch (Exception ex)
            {

                result = null;
            }
            return result;
        }

        public static long? EndDay(long timeNumber)
        {
            long? result = null;
            try
            {
                System.DateTime datetime = Convert.TimeNumberToSystemDateTime(timeNumber).Value;
                result = Int64.Parse(datetime.ToString("yyyyMMdd") + END_TIME);
            }
            catch (Exception ex)
            {

                result = null;
            }
            return result;
        }

        public static long? EndDay()
        {
            long? result = null;
            try
            {
                result = Int64.Parse(System.DateTime.Now.ToString("yyyyMMdd") + END_TIME);
            }
            catch (Exception ex)
            {

                result = null;
            }
            return result;
        }

        public static long? StartWeek()
        {
            long? result = null;
            try
            {
                System.DateTime now = System.DateTime.Now;
                int delta = DayOfWeek.Monday - now.DayOfWeek;
                System.DateTime monday = now.AddDays(delta);
                result = Int64.Parse(monday.ToString("yyyyMMdd") + START_TIME);
            }
            catch (Exception ex)
            {

                result = null;
            }
            return result;
        }

        public static long? EndWeek()
        {
            long? result = null;
            try
            {
                System.DateTime now = System.DateTime.Now;
                int delta = DayOfWeek.Sunday - now.DayOfWeek;
                System.DateTime sunday = now.AddDays(delta);
                result = Int64.Parse(sunday.ToString("yyyyMMdd") + END_TIME);
            }
            catch (Exception ex)
            {

                result = null;
            }
            return result;
        }

        public static long? StartMonth()
        {
            long? result = null;
            try
            {
                System.DateTime now = System.DateTime.Now;
                var startOfMonth = new System.DateTime(now.Year, now.Month, 1);
                result = Int64.Parse(startOfMonth.ToString("yyyyMMdd") + START_TIME);
            }
            catch (Exception ex)
            {

                result = null;
            }
            return result;
        }

        public static long? EndMonth()
        {
            long? result = null;
            try
            {
                System.DateTime now = System.DateTime.Now;
                var lastDayOfMonth = System.DateTime.DaysInMonth(now.Year, now.Month);
                result = Int64.Parse((new System.DateTime(now.Year,now.Month,lastDayOfMonth)).ToString("yyyyMMdd") + END_TIME);
            }
            catch (Exception ex)
            {

                result = null;
            }
            return result;
        }

        public static long? StartMonth(System.DateTime now)
        {
            long? result = null;
            try
            {
                var startOfMonth = new System.DateTime(now.Year, now.Month, 1);
                result = Int64.Parse(startOfMonth.ToString("yyyyMMdd") + START_TIME);
            }
            catch (Exception ex)
            {

                result = null;
            }
            return result;
        } 
		
		public static long? EndMonth(System.DateTime now)
        {
            long? result = null;
            try
            {
                System.DateTime nextMonth = now.AddMonths(1);
                var lastDayOfMonth = System.DateTime.DaysInMonth(now.Year, now.Month);
                result = Int64.Parse((new System.DateTime(now.Year, now.Month, lastDayOfMonth)).ToString("yyyyMMdd") + END_TIME);
            }
            catch (Exception ex)
            {

                result = null;
            }
            return result;
        }

        //public static long? EndMonth(System.DateTime now)
        //{
          //  long? result = null;
            //try
            //{
            //    var lastDayOfMonth = System.DateTime.DaysInMonth(now.Year, now.Month);
           //     result = Int64.Parse(lastDayOfMonth.ToString("yyyyMMdd") + END_TIME);
          //  }
           // catch (Exception ex)
           // {

           //     result = null;
          //  }
          //  return result;
       // }

        public static System.DateTime? StartWeekSystemDateTime()
        {
            System.DateTime? result = null;
            try
            {
                System.DateTime now = System.DateTime.Now;
                int delta = DayOfWeek.Monday - now.DayOfWeek;
                System.DateTime monday = now.AddDays(delta);
                long time = 0;
                Int64.TryParse(monday.ToString("yyyyMMdd") + START_TIME, out time);
                result = Convert.TimeNumberToSystemDateTime(time);
            }
            catch (Exception ex)
            {

                result = null;
            }
            return result;
        }

        public static System.DateTime? EndWeekSystemDateTime()
        {
            System.DateTime? result = null;
            try
            {
                System.DateTime now = System.DateTime.Now;
                int delta = DayOfWeek.Sunday - now.DayOfWeek;
                System.DateTime monday = now.AddDays(delta);
                long time = 0;
                Int64.TryParse(monday.ToString("yyyyMMdd") + END_TIME, out time);
                result = Convert.TimeNumberToSystemDateTime(time);
            }
            catch (Exception ex)
            {

                result = null;
            }
            return result;
        }

        public static System.DateTime? StartMonthSystemDateTime()
        {
            System.DateTime? result = null;
            try
            {
                System.DateTime now = System.DateTime.Now;
                var startOfMonth = new System.DateTime(now.Year, now.Month, 1);
                long time = 0;
                Int64.TryParse(startOfMonth.ToString("yyyyMMdd") + START_TIME, out time);
                result = Convert.TimeNumberToSystemDateTime(time);
            }
            catch (Exception ex)
            {

                result = null;
            }
            return result;
        }

        public static System.DateTime? EndMonthSystemDateTime()
        {
            System.DateTime? result = null;
            try
            {
                System.DateTime now = System.DateTime.Now;
                var lastDayOfMonth = System.DateTime.DaysInMonth(now.Year, now.Month);
                long time = 0;
                Int64.TryParse(lastDayOfMonth.ToString("yyyyMMdd") + END_TIME, out time);
                result = Convert.TimeNumberToSystemDateTime(time);
            }
            catch (Exception ex)
            {

                result = null;
            }
            return result;
        }

        public static System.DateTime? EndMonthSystemDateTime(System.DateTime? time)
        {
            System.DateTime? result = null;
            try
            {
                if (time.HasValue)
                {
                    var lastDayOfMonth = System.DateTime.DaysInMonth(time.Value.Year, time.Value.Month);
                    long temp = 0;
                    Int64.TryParse(lastDayOfMonth.ToString("yyyyMMdd") + END_TIME, out temp);
                    result = Convert.TimeNumberToSystemDateTime(temp);
                }
            }
            catch (Exception ex)
            {

                result = null;
            }
            return result;
        }

        /// <summary>
        /// Muc dich tim ngay nay cua thang tiep theo. Vd: 31/10 --> 31/12.
        /// Khong the su dung ham AddMonths cua .NET vi khong chinh xac 31/10 --> 30/11.
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static System.DateTime? SameDayOfNextMonth(System.DateTime? time)
        {
            System.DateTime? result = null;
            try
            {
                if (time.HasValue)
                {
                    int second = time.Value.Second;
                    int minute = time.Value.Minute;
                    int hour = time.Value.Hour;
                    int day = time.Value.Day;
                    int month = time.Value.Month;
                    int year = time.Value.Year;

                    bool found = false;
                    int loop = 0;
                    do
                    {
                        month++;
                        if (month > 12)
                        {
                            month = 1;
                            year++;
                        }
                        try
                        {
                            result = new System.DateTime(year, month, day, hour, minute, second);
                            found = true;
                        }
                        catch (Exception)
                        {
                            result = null;
                            found = false;
                        }
                        loop++;
                    } while (!(found || loop > 5)); //loop > 5 ma van chua xac dinh duoc ngay thi dung lai (co sai sot gi do o day)
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

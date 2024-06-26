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
using Inventec.Common.Logging;
using System;

namespace HIS.Desktop.Utilities
{
    public class GlobalReportQuery
    {
        #region Report Param constan
        /// <summary>
        /// Hiển thị theo định dạng (Tháng 10 năm 2015)Inventec.Desktop.Base.GlobalReportQuery
        /// </summary>
        /// <returns></returns>

        public static string GetCurrentMonthSeparate()
        {
            string result = "";
            try
            {
                string month = string.Format("{0:00}", DateTime.Now.Month);
                string strThang = "Tháng";
                string strNam = "năm";
                result = string.Format(strThang + " {0} " + strNam + " {1}", DateTime.Now.Month, DateTime.Now.Year);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result = "";
            }
            return result;
        }

        /// <summary>
        /// Hiển thị theo định dạng (10/2015)
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentMonth()
        {
            string result = "";
            try
            {
                result = DateTime.Now.ToString("MM/yyyy");
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result = "";
            }
            return result;
        }

        /// <summary>
        /// Hiển thị theo định dạng (2015)
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentYear()
        {
            string result = "";
            try
            {
                result = DateTime.Now.ToString("yyyy");
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result = "";
            }
            return result;
        }

        /// <summary>
        /// Hiển thị theo định dạng (Ngày 12 tháng 10 năm 2015)
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentDateSeparate()
        {
            string result = "";
            try
            {
                string month = string.Format("{0:00}", DateTime.Now.Month);
                string day = string.Format("{0:00}", DateTime.Now.Day);
                string strNgay = "Ngày";
                string strThang = "tháng";
                string strNam = "năm";
                result = string.Format(strNgay + " {0} " + strThang + " {1} " + strNam + " {2}", DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result = "";
            }
            return result;
        }

        /// <summary>
        /// Hiển thị định dạng 23:59 Ngày 12 tháng 10 năm 2015
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentDateSeparateFullTime()
        {
            string result = "";
            try
            {
                string minute = string.Format("{0:00}", DateTime.Now.Minute);
                string hours = string.Format("{0:00}", DateTime.Now.Hour);
                string month = string.Format("{0:00}", DateTime.Now.Month);
                string day = string.Format("{0:00}", DateTime.Now.Day);
                string strNgay = "ngày";
                string strThang = "tháng";
                string strNam = "năm";
                result = string.Format("{0}" + ":" + "{1} " + strNgay + " {2} " + strThang + " {3} " + strNam + " {4}", hours, minute, day, month, DateTime.Now.Year);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result = "";
            }
            return result;
        }

        /// <summary>
        /// Hiển thị định dạng 23:59 Ngày 12 tháng 10 năm 2015
        /// </summary>
        /// <returns></returns>
        public static string GetTimeSeparateFromTime(long intructionTime)
        {
            string result = "";
            try
            {
                if (intructionTime > 0)
                {
                    string timeStr = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(intructionTime);
                    string year = timeStr.Substring(6, 4);
                    string month = timeStr.Substring(3, 2);
                    string date = timeStr.Substring(0, 2);
                    string hour = timeStr.Substring(11, 2);
                    string minute = timeStr.Substring(14, 2);
                    string second = timeStr.Substring(17, 2);
                    string strNgay = "ngày";
                    string strThang = "tháng";
                    string strNam = "năm";
                    result = string.Format("{0}" + ":" + "{1} " + strNgay + " {2} " + strThang + " {3} " + strNam + " {4}", hour, minute, date, month, year);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result = "";
            }
            return result;
        }

        /// <summary>
        /// Hiển thị định dạng Ngày 12 tháng 10 năm 2015
        /// </summary>
        /// <returns></returns>
        public static string GetDateSeparateFromTime(long intructionTime)
        {
            string result = "";
            try
            {
                if (intructionTime > 0)
                {
                    string timeStr = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(intructionTime);
                    string year = timeStr.Substring(6, 4);
                    string month = timeStr.Substring(3, 2);
                    string date = timeStr.Substring(0, 2);
                    string strNgay = "ngày";
                    string strThang = "tháng";
                    string strNam = "năm";
                    result = string.Format(strNgay + " {0} " + strThang + " {1} " + strNam + " {2}", date, month, year);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result = "";
            }
            return result;
        }

        /// <summary>
        /// Hiển thị theo định dạng (12/10/2015)
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentDate()
        {
            string result = "";
            try
            {
                string month = string.Format("{0:00}", DateTime.Now.Month);
                string day = string.Format("{0:00}", DateTime.Now.Day);
                string strNgay = "";
                string strThang = "/";
                string strNam = "/";
                result = string.Format(strNgay + " {0} " + strThang + " {1} " + strNam + " {2}", DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result = "";
            }
            return result;
        }

        /// <summary>
        /// Hiển thị theo định dạng (12/10/2015 16:43:12)
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentTime()
        {
            string result = "";
            try
            {
                result = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(Inventec.Common.DateTime.Get.Now().ToString()));
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result = "";
            }
            return result;
        }               
        #endregion
    }
}

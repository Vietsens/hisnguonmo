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
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MPS
{
    class GlobalQuery
    {
        /// <summary>
        /// Hiển thị định dạng 23:59 Ngày 12 tháng 10 năm 2015
        /// </summary>
        /// <returns></returns>
        internal static string GetCurrentTimeSeparateBeginTime(System.DateTime now)
        {
            string result = "";
            try
            {
                if (now != DateTime.MinValue)
                {
                    string month = string.Format("{0:00}", now.Month);
                    string day = string.Format("{0:00}", now.Day);
                    string hour = string.Format("{0:00}", now.Hour);
                    string hours = string.Format("{0:00}", now.Hour);
                    string minute = string.Format("{0:00}", now.Minute);
                    string strNgay = "ngày";
                    string strThang = "tháng";
                    string strNam = "năm";
                    result = string.Format("{0}" + ":" + "{1} " + strNgay + " {2} " + strThang + " {3} " + strNam + " {4}", hours, minute, now.Day, now.Month, now.Year);
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
        /// Hiển thị định dạng 12/10/2015 23:51:22
        /// </summary>
        /// <returns></returns>
        internal static string GetCurrentTimeSeparate(System.DateTime now)
        {
            string result = "";
            try
            {
                string minute = string.Format("{0:00}", now.Minute);
                string hours = string.Format("{0:00}", now.Hour);
                string month = string.Format("{0:00}", now.Month);
                string day = string.Format("{0:00}", now.Day);
                string strNgay = "ngày";
                string strThang = "tháng";
                string strNam = "năm";
                result = string.Format("{0}" + "/" + "{1}" + "/" + "{2}" + " " + "{3}" + ":" + "{4}" + ":{5}", day, month, now.Year, hours, minute, now.Second);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result = "";
            }
            return result;
        }

        //internal static void AddObjectKeyIntoListkey<T>(T data, List<MPS.Core.KeyValue> keyValues)
        //{
        //    try
        //    {
        //        AddObjectKeyIntoListkey(data, keyValues, true);
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        //internal static void AddObjectKeyIntoListkey<T>(T data, List<MPS.Core.KeyValue> keyValues, bool isOveride)
        //{
        //    try
        //    {
        //        PropertyInfo[] pis = typeof(T).GetProperties();
        //        if (pis != null && pis.Length > 0)
        //        {
        //            foreach (var pi in pis)
        //            {
        //                var searchKey = keyValues.SingleOrDefault(o => o.KEY == pi.Name);
        //                if (searchKey == null)
        //                {
        //                    keyValues.Add(new Core.KeyValue(pi.Name, (data != null ? pi.GetValue(data) : null)));
        //                }
        //                else
        //                {
        //                    if (isOveride)
        //                        searchKey.VALUE = (data != null ? pi.GetValue(data) : null);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        //internal static void AddSingeKey(List<MPS.Core.KeyValue> keyValues, string key, object value, bool isOveride)
        //{
        //    try
        //    {
        //        var searchKey = keyValues.SingleOrDefault(o => o.KEY == key);
        //        if (searchKey == null)
        //        {
        //            keyValues.Add(new Core.KeyValue(key, value));
        //        }
        //        else
        //        {
        //            if (isOveride)
        //                searchKey.VALUE = value;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        //internal static void AddSingeKey(List<MPS.Core.KeyValue> keyValues, string key, object value)
        //{
        //    try
        //    {
        //        AddSingeKey(keyValues, key, value, false);
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        internal static string TrimHeinCardNumber(string heinCardNumber)
        {
            string result = "";
            try
            {
                result = System.Text.RegularExpressions.Regex.Replace(heinCardNumber, @"[-,_ ]|[_]{2}|[_]{3}|[_]{4}|[_]{5}", "").ToUpper();
            }
            catch
            {

            }

            return result;
        }

        public static string SetHeinCardNumberDisplayByNumber(string heinCardNumber)
        {
            string result = "";
            try
            {
                if (!String.IsNullOrWhiteSpace(heinCardNumber) && heinCardNumber.Length == 15)
                {
                    string separateSymbol = "-";
                    result = new StringBuilder().Append(heinCardNumber.Substring(0, 2)).Append(separateSymbol).Append(heinCardNumber.Substring(2, 1)).Append(separateSymbol).Append(heinCardNumber.Substring(3, 2)).Append(separateSymbol).Append(heinCardNumber.Substring(5, 2)).Append(separateSymbol).Append(heinCardNumber.Substring(7, 3)).Append(separateSymbol).Append(heinCardNumber.Substring(10, 5)).ToString();
                }
                else
                {
                    result = heinCardNumber;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                result = heinCardNumber;
            }
            return result;
        }
    }
}

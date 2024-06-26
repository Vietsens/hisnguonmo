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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.CallPatientTypeAlter
{
 public class HelpUltils
 {
  public static string CalculateAgeFromYear(long ageYearNumber)
  {
   string tuoi = "";
   try
   {
    DateTime dtNgSinh = (DateTime)Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(ageYearNumber);

    //New
    TimeSpan diff = DateTime.Now - dtNgSinh;
    long tongsogiay = diff.Ticks;
    if (tongsogiay < 0)
    {
     //
     tuoi = "";
     return "";
    }
    DateTime newDate = new DateTime(tongsogiay);

    int nam = newDate.Year - 1;
    int thang = newDate.Month - 1;
    int ngay = newDate.Day - 1;
    int gio = newDate.Hour;
    int phut = newDate.Minute;
    int giay = newDate.Second;

    long age = 0;
    if (nam > 0)
    {
     //Năm
     age = nam;
     tuoi = string.Format("{0:00}", age + " tuổi");
    }
    else
    {
     if (thang > 0)
     {
      //Tháng
      age = thang;
      tuoi = string.Format("{0:00}", age + " tháng");
     }
     else
     {
      if (ngay > 0)
      {
       //Ngày
       age = ngay;
       tuoi = string.Format("{0:00}", age + " ngày");
      }
      else
      {
       //Giờ
       age = gio;
       tuoi = string.Format("{0:00}", age + " giờ");
      }
     }
    }

    // tuoi = string.Format("{0:00}", age);
   }
   catch (Exception ex)
   {
    Inventec.Common.Logging.LogSystem.Warn(ex);
    tuoi = "";
   }
   return tuoi;
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

  public static string TrimHeinCardNumber(string chucodau)
  {
   string result = "";
   try
   {
    result = System.Text.RegularExpressions.Regex.Replace(chucodau, @"[-,_ ]|[_]{2}|[_]{3}|[_]{4}|[_]{5}", "").ToUpper();
   }
   catch (Exception ex)
   {

   }

   return result;
  }

  public static DateTime? ConvertDateStringToSystemDate(string date)
  {
   DateTime? result = DateTime.MinValue;
   try
   {
    if (!String.IsNullOrEmpty(date))
    {
     date = date.Replace(" ", "");
     int day = Int16.Parse(date.Substring(0, 2));
     int month = Int16.Parse(date.Substring(3, 2));
     int year = Int16.Parse(date.Substring(6, 4));
     result = new DateTime(year, month, day);
    }
   }
   catch (Exception ex)
   {
    result = null;
   }

   return result;
  }

  public static DateTime? ConvertDateTimeStringToSystemTime(string datetime)
  {
   DateTime? result = DateTime.MinValue;
   try
   {
    if (!String.IsNullOrEmpty(datetime))
    {
     //datetime = datetime.Replace("", "");
     int day = Int16.Parse(datetime.Substring(0, 2));
     int month = Int16.Parse(datetime.Substring(3, 2));
     int year = Int16.Parse(datetime.Substring(6, 4));
     int hour = Int16.Parse(datetime.Substring(11, 2));
     int minute = Int16.Parse(datetime.Substring(14, 2));
     result = new DateTime(year, month, day, hour, minute, 0);
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

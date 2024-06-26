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

namespace HIS.Desktop.Utility
{
    public class DateTimeHelper
    {
        public static DateTime? ConvertDateStringToSystemDate(string date)
        {
            DateTime? result = null;
            try
            {
                if (!String.IsNullOrEmpty(date))
                {
                    date = date.Replace(" ", "");
                    if (date.Length == 4)
                    {
                        int year = Int16.Parse(date);
                        result = new DateTime(year, 01, 01);
                    }
                    else if (date.Length == 7)
                    {
                        int month = Int16.Parse(date.Substring(0, 2));
                        int year = Int16.Parse(date.Substring(3, 4));
                        result = new DateTime(year, month, 01);
                    }
                    else
                    {
                        int day = Int16.Parse(date.Substring(0, 2));
                        int month = Int16.Parse(date.Substring(3, 2));
                        int year = Int16.Parse(date.Substring(6, 4));
                        result = new DateTime(year, month, day);
                    }
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
            DateTime? result = null;
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

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
using Inventec.Common.TypeConvert;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000272
{
    class AgeUtil
    {
        internal static string CalculateFullAge(long ageNumber)
        {
            string result;
            try
            {
                DateTime d = Parse.ToDateTime(Inventec.Common.DateTime.Convert.TimeNumberToTimeString(ageNumber));
                long ticks = (DateTime.Now - d).Ticks;
                if (ticks < 0L)
                {
                    result = "";
                }
                else
                {
                    DateTime dateTime = new DateTime(ticks);
                    int num = dateTime.Year - 1;
                    int num2 = dateTime.Month - 1;
                    int num3 = dateTime.Day - 1;
                    int hour = dateTime.Hour;
                    int minute = dateTime.Minute;
                    int second = dateTime.Second;
                    string str;
                    string str2;
                    if (num > 0)
                    {
                        str = num.ToString();
                        str2 = "Tuổi";
                    }
                    else if (num2 > 0)
                    {
                        str = num2.ToString();
                        str2 = "Tháng";
                    }
                    else if (num3 > 0)
                    {
                        str = num3.ToString();
                        str2 = "ngày";
                    }
                    else
                    {
                        str = "";
                        str2 = "Giờ";
                    }
                    result = str + " " + str2;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
                result = "";
            }
            return result;
        }
    }
}

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

namespace MPS.Processor.Mps000047
{
    public class AgeUtil
    {
        public static string CalculateFullAge(long ageNumber)
        {
            string result = "";
            try
            {
                string text = "tuổi";
                string text2 = "tháng tuổi";
                string text3 = "ngày tuổi";
                string text4 = "giờ tuổi";
                if (ageNumber > 0)
                {
                    DateTime value = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(ageNumber).Value;
                    if (value == DateTime.MinValue)
                    {
                        throw new ArgumentNullException("dtNgSinh");
                    }

                    TimeSpan timeSpan = DateTime.Now - value;
                    TimeSpan timeSpan2 = DateTime.Now.Date - value.Date;
                    double totalHours = timeSpan.TotalHours;
                    if (totalHours < 24.0)
                    {
                        result = (int)totalHours + " " + text4;
                    }
                    else
                    {
                        long ticks = timeSpan.Ticks;
                        DateTime dateTime = new DateTime(ticks);
                        if ((dateTime.Year - 1) * 12 + dateTime.Month - 1 == 0)
                        {
                            result = (int)timeSpan2.TotalDays + " " + text3;
                        }
                        else
                        {
                            long ticks2 = timeSpan2.Ticks;
                            DateTime dateTime2 = new DateTime(ticks2);
                            int num = (dateTime2.Year - 1) * 12 + dateTime2.Month - 1;
                            if (num == 0)
                            {
                                result = (int)timeSpan2.TotalDays + " " + text3;
                            }
                            else if (num < 72)
                            {
                                result = num + " " + text2;
                            }
                            else
                            {
                                int num2 = DateTime.Now.Year - value.Year;
                                result = num2 + " " + text;
                            }
                        }
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
                return "";
            }
        }
    }
}

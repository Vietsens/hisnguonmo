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
using System.Globalization;

namespace Inventec.Common.Calculate
{
    public class Calculation
    {
        public static decimal MucLocCauThan(long patientDob, decimal weight, decimal height, decimal resultTestIndex, bool isMale)
        {
            decimal result = 0;
            try
            {
                var age = Age(patientDob);
                if (age >= 17)
                {
                    result = isMale ? (140 - age) * weight / (resultTestIndex * 72) : (140 - age) * weight * (decimal)0.85f / (resultTestIndex * 72);
                }
                else
                {
                    decimal schwartzConstant = 0;
                    if (13 <= age && age <= 17)
                    {
                        schwartzConstant = isMale ? (decimal)0.70f : (decimal)0.55f;
                    }
                    else if (1 <= age && age <= 12)
                    {
                        schwartzConstant = (decimal)0.55f;
                    }
                    else if (age < 1)
                    {
                        int monthOld = DifferenceDate(patientDob, (long)SystemDateTimeToTimeNumber(DateTime.Now)) / 30;
                        decimal standardWeight = GetStandardWeight(monthOld, isMale);
                        schwartzConstant = weight < standardWeight ? (decimal)0.33f : (decimal)0.45f;
                    }
                    result = schwartzConstant * height / resultTestIndex;
                }
            }
            catch (Exception ex)
            {
                result = -1;
            }
            return result;
        }

        public static string MucLocCauThanCrCleGFR(long patientDob, decimal weight, decimal height, decimal resultTestIndex, bool isMale)
        {
            string result = null;

            try
            {
                var age = Age(patientDob);
                var number = MucLocCauThan(patientDob, weight, height, resultTestIndex, isMale);
                number = Math.Round(number, 4); 

                if (age >= 17)
                {
                    result = string.Format("CrCl: {0} ml/phút", number);
                }
                else
                {
                    result = string.Format("eGFR: {0} ml/phút/1,73 m2", number);
                }
            }
            catch (Exception)
            {
                result = null;
            }
            return result;
        }
        private static System.DateTime? TimeNumberToSystemDateTime(long time)
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

        private static long? SystemDateTimeToTimeNumber(System.DateTime? dateTime)
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

        private static int Age(long dob)
        {
            int result = -1;
            try
            {
                System.DateTime? sysDob = TimeNumberToSystemDateTime(dob);
                if (sysDob != null)
                {
                    System.DateTime today = System.DateTime.Today;
                    result = today.Year - sysDob.Value.Year;
                    //if (today < sysDob.Value.AddYears(result))
                    //{
                    //    result--;
                    //}
                }
            }
            catch (Exception ex)
            {
                result = -1;
            }
            return result;
        }

        private static int DifferenceDate(long timeBefore, long timeAfter)
        {
            int result = -1;
            try
            {
                System.DateTime? dateBefore = TimeNumberToSystemDateTime(timeBefore);
                System.DateTime? dateAfter = TimeNumberToSystemDateTime(timeAfter);
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

        private static decimal GetStandardWeight(int monthOld, bool isMale)
        {
            decimal rs = 0;
            try
            {
                switch (monthOld)
                {
                    case 0:
                        rs = isMale ? (decimal)2.5f : (decimal)2.4f;
                        break;
                    case 1:
                        rs = isMale ? (decimal)3.4f : (decimal)3.2f;
                        break;
                    case 2:
                        rs = isMale ? (decimal)4.3f : (decimal)3.9f;
                        break;
                    case 3:
                        rs = isMale ? (decimal)5.0f : (decimal)4.5f;
                        break;
                    case 4:
                        rs = isMale ? (decimal)5.6f : (decimal)5.0f;
                        break;
                    case 5:
                        rs = isMale ? (decimal)6.0f : (decimal)5.4f;
                        break;
                    case 6:
                        rs = isMale ? (decimal)6.4f : (decimal)5.7f;
                        break;
                    case 7:
                        rs = isMale ? (decimal)6.7f : (decimal)6.0f;
                        break;
                    case 8:
                        rs = isMale ? (decimal)6.9f : (decimal)6.3f;
                        break;
                    case 9:
                        rs = isMale ? (decimal)7.1f : (decimal)6.5f;
                        break;
                    case 10:
                        rs = isMale ? (decimal)7.4f : (decimal)6.7f;
                        break;
                    case 11:
                        rs = isMale ? (decimal)7.6f : (decimal)6.9f;
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                rs = 0;
            }
            return rs;
        }

    }
}

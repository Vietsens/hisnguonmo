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

namespace HIS.Desktop.Plugins.ImportBlood.Helpers
{
    public class DateTimeUtil
    {
        public static bool IsValidDateStr(string date)
        {
            bool result = false;
            try
            {
                System.DateTime temp = System.DateTime.ParseExact(date, "dd/MM/yyyy",
                                       System.Globalization.CultureInfo.InvariantCulture);
                result = true;
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        public static System.DateTime? DateStrToSystemDateTime(string date)
        {
            System.DateTime? result = null;
            try
            {
                if (String.IsNullOrWhiteSpace(date))
                    return null;
                result = System.DateTime.ParseExact(date, "dd/MM/yyyy",System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                result = null;
            }
            return result;
        }

        public static bool IsValidDateTimeStr(string date)
        {
            bool result = false;
            try
            {
                System.DateTime temp = System.DateTime.ParseExact(date, "dd/MM/yyyy HH:mm:ss",
                                       System.Globalization.CultureInfo.InvariantCulture);
                result = true;
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        public static System.DateTime? DateTimeStrToSystemDateTime(string date)
        {
            System.DateTime? result = null;
            try
            {
                if (String.IsNullOrWhiteSpace(date))
                    return null;
                result = System.DateTime.ParseExact(date, "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                result = null;
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
    }
}

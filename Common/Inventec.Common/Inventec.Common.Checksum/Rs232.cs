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

namespace Inventec.Common.Checksum
{
    public class Rs232
    {
        public static int? Calc(string text)
        {
            int? result = null;
            try
            {
                if (!String.IsNullOrEmpty(text))
                {
                    int temp = 0;
                    byte[] byteArr = System.Text.Encoding.ASCII.GetBytes(text);
                    foreach (byte b in byteArr)
                    {
                        temp += b;
                    }
                    if (temp > 0) result = temp;
                }
            }
            catch (Exception)
            {
                result = null;
            }
            return result;
        }

        public static object Check(string message)
        {
            object result = null;
            try
            {
                int checksum = Inventec.Common.TypeConvert.Parse.ToInt32(message.Substring(0, message.IndexOf(",")).Replace(",", ""));

                string dataTransfer = message.Substring(message.IndexOf(",") + 1);
                if (checksum == Calc(dataTransfer))
                    result = dataTransfer;
            }
            catch (Exception)
            {
                result = null;
            }
            return result;
        }
    }
}

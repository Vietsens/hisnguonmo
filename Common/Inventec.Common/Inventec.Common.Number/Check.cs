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
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Inventec.Common.Number
{
    public class Check
    {
        /// <summary>
        /// Kiem tra 1 chuoi co chi bao gom ky tu so (co the co 0 o dau)
        /// </summary>
        /// <param groupCode="message"></param>
        /// <returns>
        ///     true - hop le
        ///     false - khong hop le
        /// </returns>
        public static bool IsNumber(string message)
        {
            bool result = true;
            try
            {
                char[] varChar = message.ToCharArray();
                int i = 0;
                int val = 0;
                while (i < varChar.Length)
                {
                    val = System.Convert.ToInt32(varChar[i]);
                    if (val >= 48 && val <= 57) // 0 -> 9
                    {
                        i++;
                    }
                    else
                    {
                        break;
                    }
                }
                if (i < varChar.Length)
                {
                    result = false;//chua ky tu ko cho phep
                }
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        public static bool IsDecimal(string message)
        {
            bool result = false;
            try
            {
                Regex regex = new Regex(@"^[0-9]*\,?[0-9]+$");
                if (regex.IsMatch(message))
                {
                    if (message.First() == ',' || message.Last() == ',')
                    {
                        result = false;
                    }
                    else
                    {
                        result = true;
                    }
                }
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }
    }
}

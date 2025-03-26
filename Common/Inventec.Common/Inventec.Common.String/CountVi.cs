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

namespace Inventec.Common.String
{
    /// <summary>
    /// Với các kí tự từ 0(0x00) ~ 127(0x7F) (tức là của ASCII - có 128 kí tự) : vẫn là 1 byte. : 0x*0vvvvvvv
    /// Với các kí tự từ **0x80 ~ 0x7FF* (có 1920 kí tự) sẽ dùng 2 byte : 0x*110vvvvv 0x10vvvvvv
    /// Với các kí tự từ **0x800 ~ 0xFFFF* (có 63488 kí tự) sẽ dùng 3 byte : 0x*1110vvvv 0x10vvvvvv 0x10*vvvvvv
    /// </summary>
    public class CountVi
    {
        /// <summary>
        /// dem do dai chuoi. dem ky tu tieng viet
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int? Count(string viString)
        {
            int? result = null;
            try
            {
                if (!string.IsNullOrWhiteSpace(viString))
                {
                    Dictionary<char, List<char>> dicChar = new Dictionary<char, List<char>>();

                    foreach (char item in viString)
                    {
                        if (!dicChar.ContainsKey(item))
                            dicChar[item] = new List<char>();
                        dicChar[item].Add(item);
                    }

                    int countt = 0;
                    foreach (var item in dicChar)
                    {
                        var numUtf = char.ConvertToUtf32(item.Key.ToString(), 0);
                        if (numUtf >= 0x00 && numUtf <= 0x7F)
                        {
                            countt += 1 * (item.Value.Count);
                        }
                        else if (numUtf >= 0x80 && numUtf <= 0x7FF)
                        {
                            countt += 2 * (item.Value.Count);
                        }
                        else if (numUtf >= 0x800 && numUtf <= 0xFFFF)
                        {
                            countt += 3 * (item.Value.Count);
                        }
                    }

                    result = countt;
                }
                else
                {
                    result = 0;
                }
            }
            catch (Exception)
            {
                result = null;
            }
            return result;
        }

        public static string SubStringVi(string data, int length)
        {
            return SubStringVi(data, 0, length);
        }

        public static string SubStringVi(string data, int startIndex, int length)
        {
            string result = "";
            try
            {
                if (!string.IsNullOrWhiteSpace(data))
                {
                    var value = data.Substring(startIndex);

                    List<char> listValue = new List<char>();
                    int totalLenght = 0;
                    for (int i = 0; i < value.Length; i++)
                    {
                        int len = 0;
                        var numUtf = char.ConvertToUtf32(value[i].ToString(), 0);
                        if (numUtf >= 0x00 && numUtf <= 0x7F)
                        {
                            len = 1;
                        }
                        else if (numUtf >= 0x80 && numUtf <= 0x7FF)
                        {
                            len = 2;
                        }
                        else if (numUtf >= 0x800 && numUtf <= 0xFFFF)
                        {
                            len = 3;
                        }

                        if (len + totalLenght <= length)
                        {
                            listValue.Add(value[i]);
                            totalLenght += len;
                        }
                        else
                        {
                            break;
                        }
                    }
                    result = string.Join("", listValue);
                }
            }
            catch (Exception)
            {
                result = data;
            }
            return result;
        }
    }
}

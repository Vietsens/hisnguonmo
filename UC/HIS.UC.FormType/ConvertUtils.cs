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

namespace HIS.UC.FormType
{
    internal class ConvertUtils
    {
        internal static object ConvertToObjectFilter(object data)
        {
            string nullString = "null";
            object result = null;
            try
            {
               
                if (data is int)
                {
                    result = (int)data;
                }
                else if (data is int?)
                {
                    if (data != null)
                        result = (int)data;
                    else
                        result = nullString;
                }
                else if (data is short)
                {
                    result = (short)data;
                }
                else if (data is short?)
                {
                    if (data != null)
                        result = (short)data;
                    else
                        result = nullString;
                }
                else if (data is long)
                {
                    result = (long)data;
                }
                else if (data is long?)
                {
                    if (data != null)
                        result = (long)data;
                    else
                        result = nullString;
                }
                else if (data is decimal)
                {
                    result = (decimal)data;
                }
                else if (data is decimal?)
                {
                    if (data != null)
                        result = (decimal)data;
                    else
                        result = nullString;
                }
                else if (data is bool)
                {
                    if ((bool)data == true) data = "true"; else data = "false";
                    result = data;
                }
                else if (data is bool?)
                {
                    if (data != null)
                    {
                        if ((bool)data == true) data = "true"; else data = "false";
                        result = data;
                    }
                    else
                        result = nullString;
                }
                else
                {
                    if (data != null)
                        result = (string)data;
                    else
                        result = nullString;
                }
            }
            catch (Exception ex)
            {

            }
            return result;
        }
    }
}

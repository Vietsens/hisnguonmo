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
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.WordContent
{
    public class TemplateKeyProcessor
    {
        public static void AddKeyIntoDictionaryPrint<T>(T data, Dictionary<string, object> singleValueDictionary, bool isAllowOverrideValue = false)
        {
            try
            {
                PropertyInfo[] pis = typeof(T).GetProperties();
                if (pis != null && pis.Length > 0)
                {
                    foreach (var pi in pis)
                    {
                        if (!singleValueDictionary.ContainsKey(pi.Name))
                        {
                            singleValueDictionary.Add(pi.Name, pi.GetValue(data) != null ? pi.GetValue(data) : "");
                        }
                        else if (isAllowOverrideValue)
                        {
                            singleValueDictionary[pi.Name] = pi.GetValue(data) != null ? pi.GetValue(data) : "";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public static void SetSingleKey(Dictionary<string, object> singleValueDictionary, string key, object value, bool isAllowOverrideValue = true)
        {
            try
            {
                if (!singleValueDictionary.ContainsKey(key))
                {
                    singleValueDictionary.Add(key, value);
                }
                else if (isAllowOverrideValue)
                {
                    singleValueDictionary[key] = value;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

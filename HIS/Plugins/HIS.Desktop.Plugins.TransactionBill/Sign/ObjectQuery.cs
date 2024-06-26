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

namespace HIS.Desktop.Plugins.TransactionBill.Sign
{
    class ObjectQuery
    {
        internal static void AddObjectKeyIntoListkey<T>(T data, Dictionary<string, object> keyValues)
        {
            try
            {
                AddObjectKeyIntoListkey(data, keyValues, true);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal static void AddObjectKeyIntoListkey<T>(T data, Dictionary<string, object> keyValues, bool isOveride)
        {
            try
            {
                PropertyInfo[] pis = typeof(T).GetProperties();
                if (pis != null && pis.Length > 0)
                {
                    foreach (var pi in pis)
                    {
                        var searchKey = keyValues.SingleOrDefault(o => o.Key == pi.Name);
                        if (searchKey.Key == null)
                        {
                            keyValues.Add(pi.Name, (data != null ? pi.GetValue(data) : null));
                        }
                        else
                        {
                            if (isOveride)
                                keyValues[searchKey.Key] = (data != null ? pi.GetValue(data) : null);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

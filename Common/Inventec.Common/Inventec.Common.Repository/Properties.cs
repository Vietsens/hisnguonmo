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
using System.Reflection;

namespace Inventec.Common.Repository
{
    public static class Properties
    {
        private static Dictionary<Type, PropertyInfo[]> dic = new Dictionary<Type, PropertyInfo[]>();

        private static List<Type> listAllowType = new List<Type>() { typeof(string), typeof(long), typeof(Nullable<long>), typeof(decimal), typeof(Nullable<decimal>), typeof(short), typeof(Nullable<short>), typeof(bool), typeof(Nullable<bool>), typeof(int), typeof(Nullable<int>), typeof(double), typeof(Nullable<double>), typeof(float), typeof(Nullable<float>), typeof(DateTime), typeof(Nullable<DateTime>) };

        public static PropertyInfo[] Get<T>() where T : class
        {
            Type type = typeof(T);
            PropertyInfo[] result = null;
            //khi map su dung thread se bi tinh trang TryGetValue false va add nhieu lan
            lock (dic)
            {
                if (!dic.TryGetValue(type, out result))
                {
                    result = type.GetProperties();
                    List<PropertyInfo> listTemp = new List<PropertyInfo>();
                    foreach (var pi in result)
                    {
                        if (listAllowType.Contains(pi.PropertyType))
                        {
                            listTemp.Add(pi);
                        }
                    }
                    result = listTemp.ToArray();
                    dic.Add(type, result);
                }
            }
            return result;
        }

        public static PropertyInfo[] Get(Type type)
        {
            PropertyInfo[] result = null;
            //khi map su dung thread se bi tinh trang TryGetValue false va add nhieu lan
            lock (dic)
            {
                if (!dic.TryGetValue(type, out result))
                {
                    result = type.GetProperties();
                    List<PropertyInfo> listTemp = new List<PropertyInfo>();
                    foreach (var pi in result)
                    {
                        if (listAllowType.Contains(pi.PropertyType))
                        {
                            listTemp.Add(pi);
                        }
                    }
                    result = listTemp.ToArray();
                    dic.Add(type, result);
                }
            }
            return result;
        }
    }
}

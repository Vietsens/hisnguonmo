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

namespace Inventec.Common.LocalStorage.SdaConfig
{
    public class SdaConfigs
    {
        private static Dictionary<string, object> dic = new Dictionary<string, object>();
        private static Object thisLock = new Object();

        /// <summary>
        /// Lay du lieu cua cau hinh tren sdaconfig theo chuoi key
        /// </summary>
        /// <typeparam name="T">(Gia tri cua mot trong các kieu du lieu: string, int, long, decimal, List of string)</typeparam>
        /// <param name="key">Mot chuoi key trong ConfigKeys tuong ung voi key cau hinh tren sdaconfig</param>
        /// <returns>value</returns>
        public static T Get<T>(string key)
        {
            T result = default(T);
            Type type = typeof(T);
            object data = null;
            lock (thisLock)
            {
                if (!dic.TryGetValue(key, out data))
                {
                    data = ConfigUtil.GetStrConfig(key);
                    dic.Add(key, data);
                }
                if (type == typeof(List<string>))
                {
                    data = ConfigUtil.GetStrConfigs(key);
                }
                else
                {
                    data = ConfigUtil.GetStrConfig(key);
                }
                result = (T)System.Convert.ChangeType(data, typeof(T));
            }
            return result;
        }
    }
}

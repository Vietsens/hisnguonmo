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

namespace Inventec.Common.Repository
{
    public static class Worker
    {
        private static Dictionary<Type, object> dic = new Dictionary<Type, object>();
        private static Object thisLock = new Object();

        public static object Get<T>() where T : class
        {
            Type type = typeof(T);
            object result = null;
            lock (thisLock)
            {
                if (!dic.TryGetValue(type, out result))
                {
                    result = Activator.CreateInstance(type);
                    dic.Add(type, result);
                }
            }
            return result;
        }
    }
}

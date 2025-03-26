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

namespace Inventec.Common.RedisCache
{
    public interface ICacheProvider
    {
        void LoadIntoCache<T>();

        void LoadIntoCache<T>(string key);

        List<T> GetList<T>();

        List<T> GetList<T>(string key);

        bool CreateList<T>(string key, List<T> values);

        bool CreateOrUpdateList<T>(string key, List<T> values);

        bool Remove(string key);

        bool IsInCache(string key);
    }
}

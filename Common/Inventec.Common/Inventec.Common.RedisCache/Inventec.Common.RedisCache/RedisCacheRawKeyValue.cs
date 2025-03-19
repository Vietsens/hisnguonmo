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
using ServiceStack.Redis;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.RedisCache
{
    public class RedisCacheRawKeyValue : RedisCacheBase
    {
        public RedisCacheRawKeyValue()
            : base()
        {
        }

        public RedisCacheRawKeyValue(bool isUseRamSaveTemp)
            : base(isUseRamSaveTemp)
        {
        }

        public RedisCacheRawKeyValue(string host, bool isUseRamSaveTemp)
            : base(host, isUseRamSaveTemp)
        {
        }

        public RedisCacheRawKeyValue(string host, bool isUseRamSaveTemp, bool isUsePreNamespaceByFolder, string preNamespaceFolder)
            : base(host, isUseRamSaveTemp, isUsePreNamespaceByFolder, preNamespaceFolder)
        {
        }

        public void LoadIntoCache<T>(string key)
        {
            _cache[GetKey(key)] = Get<T>(GetKey(key));
        }

        public void LoadIntoCache<T>()
        {
            LoadIntoCache<T>(typeof(T).ToString());
        }

        public bool Set<T>(string key, T values)
        {
            bool isSuccess = false;
            key = GetKey(key);
            using (RedisClient redisClient = GetRedisClient())
            {
                if (ConstainByKey(key))
                {
                    redisClient.Remove(key);
                }
                isSuccess = redisClient.Set(key, values);

                //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("values.GetType()=", values.GetType().ToString()));
                if (this.isUseRamSaveTemp)
                {
                    if (!_cache.ContainsKey(key))
                    {
                        _cache.Add(key, values);
                    }
                    else
                        _cache[key] = values;
                }
                isSuccess = true;
            }

            return isSuccess;
        }

        public T Get<T>(string key)
        {
            key = GetKey(key);
            if (this.isUseRamSaveTemp)
            {
                object list;
                //Inventec.Common.Logging.LogSystem.Debug("Get. 1");
                if (_cache.TryGetValue(key, out list))
                {
                    //Inventec.Common.Logging.LogSystem.Debug("Get. 2");
                    if (list != null && (list as System.Collections.IList).Count > 0)
                    {
                        //Inventec.Common.Logging.LogSystem.Debug("Get. 3");
                        //Inventec.Common.Logging.LogSystem.Debug("Get. 4 typeof(T)=" + typeof(T).ToString());
                        //Inventec.Common.Logging.LogSystem.Debug("Get. 4 typeof(list)=" + list.GetType().ToString());
                        return (T)list;
                    }
                }
            }

            using (RedisClient redisClient = GetRedisClient())
            {
                //Inventec.Common.Logging.LogSystem.Debug("Get. 5");
                return redisClient.Get<T>(key);
            }
        }

        public T GetRaw<T>(string rawKey)
        {
            if (this.isUseRamSaveTemp)
            {
                object list;
                if (_cache.TryGetValue(rawKey, out list))
                {
                    if (list != null && (list as System.Collections.IList).Count > 0)
                    {                      
                        return (T)list;
                    }
                }
            }

            using (RedisClient redisClient = GetRedisClient())
            {
                return redisClient.Get<T>(rawKey);
            }
        }

        public void Delete<T>()
        {
            Delete(typeof(T).ToString());
        }

        public void Delete(string key)
        {
            key = GetKey(key);
            DeleteByKey(key);
            if (this.isUseRamSaveTemp && _cache.ContainsKey(key))
            {
                _cache.Remove(key);
            }
        }

        public bool DeleteRaw(string rawKey)
        {
            bool success = false;
            using (RedisClient redisClient = GetRedisClient())
            {
                success = redisClient.Remove(rawKey);
                if (this.isUseRamSaveTemp && _cache.ContainsKey(rawKey))
                {
                    _cache.Remove(rawKey);
                }
            }
            return success;
        }

        public bool ConstainKey(string key)
        {
            key = GetKey(key);
            return ConstainByKey(key);
        }
        
        public List<string> GetAllKeys(string partten)
        {
            List<string> keys = new List<string>();
            using (RedisClient redisClient = GetRedisClient())
            {
                keys = redisClient.GetAllKeys();
                if (!String.IsNullOrEmpty(partten))
                {
                    keys = keys != null ? keys.Where(k => k.Contains(partten)).ToList() : null;
                }
                return keys;
            }
        }
    }
}

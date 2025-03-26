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
using ServiceStack.Redis.Generic;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.RedisCache
{
    public class RedisCacheIRedisList : RedisCacheBase
    {
        public RedisCacheIRedisList()
            : base()
        {
        }

        public RedisCacheIRedisList(bool isUseRamSaveTemp)
            : base(isUseRamSaveTemp)
        {
        }

        public RedisCacheIRedisList(string host, bool isUseRamSaveTemp)
            : base(host, isUseRamSaveTemp)
        {
        }

        public RedisCacheIRedisList(string host, bool isUseRamSaveTemp, bool isUsePreNamespaceByFolder, string preNamespaceFolder)
            : base(host, isUseRamSaveTemp, isUsePreNamespaceByFolder, preNamespaceFolder)
        {
        }

        public void LoadIntoCache<T>(string key)
        {
            _cache[GetKey(key, true, true)] = Get<T>(GetKey(key, true, true));
        }

        public void LoadIntoCache<T>()
        {
            LoadIntoCache<T>(typeof(T).ToString());
        }

        public bool Set<T>(string key, List<T> values)
        {
            bool isSuccess = false;
            key = GetKey(key, true, true);
            if (values != null && values.Count > 0)
            {
                using (RedisClient redisClient = GetRedisClient())
                {
                    IRedisTypedClient<T> redisT = redisClient.As<T>();
                    IRedisList<T> iRDListOld = redisT.Lists[key];

                    iRDListOld.AddRange(values);

                    if (this.isUseRamSaveTemp)
                    {
                        if (!_cache.ContainsKey(key))
                        {
                            _cache.Add(key, iRDListOld.ToList());
                        }
                        else
                            _cache[key] = iRDListOld.ToList();
                    }
                    isSuccess = true;
                }
            }

            return isSuccess;
        }

        public bool SetWithSync<T>(string key, List<T> values)
        {
            bool isSuccess = false;
            key = GetKey(key, true, true);
            using (RedisClient redisClient = GetRedisClient())
            {
                IRedisTypedClient<T> redisT = redisClient.As<T>();
                IRedisList<T> iRDListOld = redisT.Lists[key];
                List<T> listOld = iRDListOld.ToList();

                List<T> inserteds = new List<T>();
                if (listOld != null && listOld.Count > 0)
                {
                    foreach (var item in values)
                    {
                        T singleElement = listOld.FirstOrDefault(ph => Predicates.GetPropertyValue<T>(ph, "ID") == Predicates.GetPropertyValue<T>(item, "ID"));

                        if (singleElement != null)
                        {
                            listOld.Remove(singleElement);
                        }

                        inserteds.Add(item);
                    }
                }
                else
                {
                    inserteds.AddRange(values);
                }

                if (inserteds != null && inserteds.Count > 0)
                {
                    iRDListOld.Clear();
                    iRDListOld.AddRange(inserteds);
                }

                if (this.isUseRamSaveTemp)
                {
                    if (!_cache.ContainsKey(key))
                    {
                        _cache.Add(key, inserteds);
                    }
                    else
                        _cache[key] = inserteds;
                }
                isSuccess = true;
            }

            return isSuccess;
        }

        public List<T> Get<T>(string key)
        {
            key = GetKey(key, true, true);
            if (this.isUseRamSaveTemp && _cache.ContainsKey(key))
            {
                object list;
                if (_cache.TryGetValue(key, out list))
                {
                    if (list != null)
                    {
                        return (List<T>)list;
                    }
                }
            }

            using (RedisClient redisClient = GetRedisClient())
            {
                IRedisTypedClient<T> redisT = redisClient.As<T>();
                IRedisList<T> iRDListOld = redisT.Lists[key];
                return iRDListOld.ToList();
            }
        }

        public List<T> GetRaw<T>(string rawKey)
        {
            if (this.isUseRamSaveTemp && _cache.ContainsKey(rawKey))
            {
                object list;
                if (_cache.TryGetValue(rawKey, out list))
                {
                    if (list != null)
                    {
                        return (List<T>)list;
                    }
                }
            }

            using (RedisClient redisClient = GetRedisClient())
            {
                IRedisTypedClient<T> redisT = redisClient.As<T>();
                IRedisList<T> iRDListOld = redisT.Lists[rawKey];
                return iRDListOld.ToList();
            }
        }

        public void Delete(string key)
        {
            using (RedisClient redisClient = GetRedisClient())
            {
                key = GetKey(key, true, true);
                redisClient.Remove(key);
                if (this.isUseRamSaveTemp && _cache.ContainsKey(key))
                {
                    _cache.Remove(key);
                }
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

        public bool ConstainKey<T>(string key)
        {
            using (RedisClient redisClient = GetRedisClient())
            {
                key = GetKey(key, true, true);
                IRedisTypedClient<T> redisT = redisClient.As<T>();
                IRedisList<T> iRDListOld = redisT.Lists[key];
                return (iRDListOld != null && iRDListOld.Count > 0);
            }
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

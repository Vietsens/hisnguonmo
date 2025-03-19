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
    public class RedisCacheUrnEntry : RedisCacheBase
    {
        public RedisCacheUrnEntry()
            : base()
        {
        }

        public RedisCacheUrnEntry(bool isUseRamSaveTemp)
            : base(isUseRamSaveTemp)
        {
        }

        public RedisCacheUrnEntry(string host, bool isUseRamSaveTemp)
            : base(host, isUseRamSaveTemp)
        {
        }

        public RedisCacheUrnEntry(string host, bool isUseRamSaveTemp, bool isUsePreNamespaceByFolder, string preNamespaceFolder)
            : base(host, isUseRamSaveTemp, isUsePreNamespaceByFolder, preNamespaceFolder)
        {
        }

        public void LoadIntoCache<T>(string key)
        {
            _cache[GetKey(key, false, true)] = Get<T>(GetKey(key, false, true));
        }

        public void LoadIntoCache<T>()
        {
            LoadIntoCache<T>(typeof(T).ToString());
        }

        public bool Set<T>(string key, List<T> values)
        {
            bool isSuccess = false;
            key = GetKey(key, false, true);
            if (values != null && values.Count > 0)
            {
                using (RedisClient redisClient = GetRedisClient())
                {
                    IRedisTypedClient<T> redisT = redisClient.As<T>();

                    values.ForEach(o => redisT.SetEntry(String.Format(formatTypeId, key, Predicates.GetPropertyValue<T>(o, "ID")), o));

                    if (this.isUseRamSaveTemp)
                    {
                        if (!_cache.ContainsKey(key))
                        {
                            _cache.Add(key, Get<T>(key));
                        }
                        else
                            _cache[key] = Get<T>(key);
                    }
                    isSuccess = true;
                }
            }

            return isSuccess;
        }

        public bool Set<T>(string key, List<T> values, string propertyName)
        {
            bool isSuccess = false;
            key = GetKey(key, false, true);
            if (values != null && values.Count > 0)
            {
                using (RedisClient redisClient = GetRedisClient())
                {
                    IRedisTypedClient<T> redisT = redisClient.As<T>();

                    values.ForEach(o => redisT.SetEntry(String.Format(formatTypeId, key, Predicates.GetPropertyValue<T>(o, propertyName)), o));

                    if (this.isUseRamSaveTemp)
                    {
                        if (!_cache.ContainsKey(key))
                        {
                            _cache.Add(key, Get<T>(key));
                        }
                        else
                            _cache[key] = Get<T>(key);
                    }
                    isSuccess = true;
                }
            }

            return isSuccess;
        }

        public bool SetWithSync<T>(string key, List<T> values, string propertyName)
        {
            bool isSuccess = false;
            key = GetKey(key, false, true);
            using (RedisClient redisClient = new RedisClient())
            {
                IRedisTypedClient<T> redisT = redisClient.As<T>();
                List<T> iRDListSync = Get<T>(key);

                List<T> inserteds = new List<T>();
                if (iRDListSync != null && iRDListSync.Count > 0)
                {
                    foreach (var item in values)
                    {
                        T singleElement = iRDListSync.FirstOrDefault(ph => Predicates.GetPropertyValue<T>(ph, propertyName) == Predicates.GetPropertyValue<T>(item, propertyName));

                        if (singleElement != null)
                        {
                            redisT.RemoveEntry(String.Format(formatTypeId, key, Predicates.GetPropertyValue<T>(item, propertyName)));
                            iRDListSync.Remove(singleElement);
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
                    inserteds.ForEach(o => redisT.SetEntry(String.Format(formatTypeId, key, Predicates.GetPropertyValue<T>(o, propertyName)), o));

                    iRDListSync.AddRange(inserteds);
                }

                if (this.isUseRamSaveTemp)
                {
                    if (!_cache.ContainsKey(key))
                    {
                        _cache.Add(key, iRDListSync);
                    }
                    else
                        _cache[key] = iRDListSync;
                }
                isSuccess = true;
            }

            return isSuccess;
        }

        public List<T> Get<T>(string key)
        {
            key = GetKey(key, false, true);
            if (this.isUseRamSaveTemp && _cache.ContainsKey(key))
            {
                object list;
                if (_cache.TryGetValue(key, out list))
                {
                    if (list != null && (list as System.Collections.IList).Count > 0)
                    {
                        return (List<T>)list;
                    }
                }
            }

            using (RedisClient redisClient = GetRedisClient())
            {
                var liveKeys = redisClient.SearchKeys(String.Format("{0}:*", key)).ToList();
                return redisClient.GetValues<T>(liveKeys);
            }
        }

        public List<T> GetRaw<T>(string rawKey)
        {
            if (this.isUseRamSaveTemp && _cache.ContainsKey(rawKey))
            {
                object list;
                if (_cache.TryGetValue(rawKey, out list))
                {
                    if (list != null && (list as System.Collections.IList).Count > 0)
                    {
                        return (List<T>)list;
                    }
                }
            }

            using (RedisClient redisClient = GetRedisClient())
            {
                var liveKeys = redisClient.SearchKeys(String.Format("{0}:*", rawKey)).ToList();
                return redisClient.GetValues<T>(liveKeys);
            }
        }

        public void Delete(string key)
        {
            key = GetKey(key, false, true);
            using (RedisClient redisClient = new RedisClient())
            {
                var liveKeys = redisClient.SearchKeys(String.Format("{0}:*", key)).ToList();
                if (liveKeys != null && liveKeys.Count > 0)
                {
                    redisClient.RemoveAll(liveKeys);
                    if (this.isUseRamSaveTemp && _cache.ContainsKey(key))
                    {
                        _cache.Remove(key);
                    }
                }
            }
        }

        public void DeleteById(string keyId)
        {
            keyId = GetKey(keyId, false, true);
            using (RedisClient redisClient = new RedisClient())
            {
                var liveKeys = redisClient.SearchKeys(String.Format("{0}", keyId)).ToList();
                if (liveKeys != null && liveKeys.Count > 0)
                {
                    redisClient.RemoveAll(liveKeys);
                    if (this.isUseRamSaveTemp && _cache.ContainsKey(keyId))
                    {
                        _cache.Remove(keyId);
                    }
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

        public bool ConstainKey(string key)
        {
            key = GetKey(key, false, true);
            using (RedisClient redisClient = new RedisClient())
            {
                var liveKeys = redisClient.SearchKeys(String.Format("{0}:*", key)).ToList();
                return (liveKeys != null && liveKeys.Count > 0);
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

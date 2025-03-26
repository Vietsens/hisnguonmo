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
using ServiceStack.Redis;
using System.Collections.Concurrent;
using ServiceStack.Redis.Generic;
using System.Linq.Expressions;

namespace Inventec.Common.RedisCache
{
    public class RedisCacheProvider : ICacheProvider
    {
        readonly static IDictionary<string, object> _cache = new ConcurrentDictionary<string, object>();
        bool isUseRamSaveTemp;

        public RedisCacheProvider()
        {
        }

        public RedisCacheProvider(bool isUseRamSaveTemp)
        {
            this.isUseRamSaveTemp = isUseRamSaveTemp;
        }

        public void LoadIntoCache<T>(string key)
        {
            _cache[key] = GetList<T>(key);
        }

        public void LoadIntoCache<T>()
        {
            LoadIntoCache<T>(typeof(T).ToString());
        }

        public List<T> GetList<T>()
        {
            return GetList<T>(typeof(T).ToString());
        }

        public List<T> GetList<T>(string key)
        {
            if (this.isUseRamSaveTemp)
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

            using (RedisClient redisClient = new RedisClient())
            {
                var liveKeys = redisClient.SearchKeys(String.Format("urn:{0}:*", key)).ToList();
                List<T> listsurn1 = redisClient.GetValues<T>(liveKeys);

                redisClient.RemoveAll(liveKeys);
            }
        }

        public bool CreateList<T>(string key, List<T> values)
        {
            bool isSuccess = false;
            using (RedisClient redisClient = new RedisClient())
            {
                IRedisTypedClient<T> redisT = redisClient.As<T>();
                IRedisList<T> iRDListOld = redisT.Lists[key];

                if (values != null && values.Count > 0)
                {
                    iRDListOld.AddRange(values);
                }

                if (this.isUseRamSaveTemp)
                {
                    if (!_cache.ContainsKey(key))
                    {
                        _cache.Add(key, iRDListOld.ToList());
                    }
                    else
                        _cache[key] = iRDListOld.ToList();
                }
                //redis.StoreAll(value);
                isSuccess = true;
            }

            return isSuccess;
        }

        public bool CreateOrUpdateList<T>(string key, List<T> values)
        {
            bool isSuccess = false;
            using (RedisClient redisClient = new RedisClient())
            {
                IRedisTypedClient<T> redisT = redisClient.As<T>();
                IRedisList<T> iRDListOld = redisT.Lists[key];
                List<T> inserteds = new List<T>();
                if (iRDListOld != null && iRDListOld.Count > 0)
                {
                    foreach (var item in values)
                    {
                        T singleElement = iRDListOld.FirstOrDefault(ph => Predicates.GetPropertyValue<T>(ph, "ID") == Predicates.GetPropertyValue<T>(item, "ID"));
                        //T singleElement = redisT.GetValue(GetId<T>(item).ToString());

                        if (singleElement != null)
                        {
                            iRDListOld.Remove(singleElement);
                            //redisT.SetEntry(tFive.Id.ToString(), tFive);
                        }

                        inserteds.Add(singleElement);
                    }
                }
                else
                {
                    inserteds.AddRange(values);
                }

                if (inserteds != null && inserteds.Count > 0)
                {
                    iRDListOld.AddRange(inserteds);
                }

                if (this.isUseRamSaveTemp)
                {
                    if (!_cache.ContainsKey(key))
                    {
                        _cache.Add(key, iRDListOld.ToList());
                    }
                    else
                        _cache[key] = iRDListOld.ToList();
                }
                //redis.StoreAll(value);
                isSuccess = true;
            }

            return isSuccess;
        }

        public bool SetEntry<T>(string key, string pattern, List<T> values)
        {
            bool isSuccess = false;
            using (RedisClient redisClient = new RedisClient())
            {
                IRedisTypedClient<T> redisT = redisClient.As<T>();
                //List<T> iRDListOld = redisT.GetAll() as List<T>;
                var liveKeys = redisClient.SearchKeys(String.Format("urn:{0}:*", key)).ToList();
                List<T> iRDListOld = redisClient.GetValues<T>(liveKeys);

                //var liveSetyKeys = redisClient.SearchKeys(String.Format(pattern, key)).ToList();
                values.ForEach(o => redisT.SetEntry(String.Format(pattern, Predicates.GetPropertyValue<T>(o, "ID")), o));
                if (iRDListOld == null) iRDListOld = new List<T>();

                if (values != null && values.Count > 0)
                {
                    iRDListOld.AddRange(values);
                }

                if (this.isUseRamSaveTemp)
                {
                    if (!_cache.ContainsKey(key))
                    {
                        _cache.Add(key, iRDListOld.ToList());
                    }
                    else
                        _cache[key] = iRDListOld.ToList();
                }
                //redis.StoreAll(value);
                isSuccess = true;
            }

            return isSuccess;
        }

        static bool Save<T>(string key, List<T> value)
        {
            bool isSuccess = false;
            using (RedisClient redisClient = new RedisClient())
            {
                IRedisTypedClient<T> redis = redisClient.As<T>();
                if (redisClient.ContainsKey(key))
                    redisClient.Remove(key);

                redis.StoreAll(value);
                isSuccess = true;
            }

            return isSuccess;
        }

        static T Get<T>(string key)
        {
            using (RedisClient redisClient = new RedisClient())
            {
                return redisClient.Get<T>(key);
            }
        }

        public bool Remove(string key)
        {
            bool removed = false;

            using (RedisClient client = new RedisClient())
            {
                removed = client.Remove(key);

                if (removed && this.isUseRamSaveTemp)
                {
                    _cache.Remove(key);
                }
            }

            return removed;
        }

        public bool IsInCache(string key)
        {
            bool isInCache = false;

            using (RedisClient client = new RedisClient())
            {
                isInCache = client.ContainsKey(key);
            }

            return isInCache;
        }
    }
}

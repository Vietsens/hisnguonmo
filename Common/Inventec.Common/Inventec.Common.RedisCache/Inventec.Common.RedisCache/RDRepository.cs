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
    public class RDRepository
    {
        private static readonly PooledRedisClientManager m = new PooledRedisClientManager();

        readonly static IDictionary<Type, object> _cache = new ConcurrentDictionary<Type, object>();

        /// <summary>
        /// Load {T} into object cache from Data Store.
        /// </summary>
        /// <typeparam name="T">class</typeparam>
        public static void LoadIntoCache<T>()
        {
            _cache[typeof(T)] = GetAll<T>();
        }

        public static bool StoreAll<T>(List<T> entity)
        {
            object list;
            if (!_cache.TryGetValue(typeof(T), out list))
            {
                _cache.Add(typeof(T), entity);
            }
            else
                _cache[typeof(T)] = entity;

            return Save<T>(typeof(T).ToString(), entity);
        }

        /// <summary>
        /// Find All {T}
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>List<T></returns>
        public static List<T> All<T>()
        {
            return GetAll<T>();
        }

        #region Redis Commands

        public static void DeleteAll<T>()
        {
            using (RedisClient redisClient = new RedisClient())
            {
                redisClient.Remove(typeof(T).ToString());
            }
        }

        private static List<T> GetAll<T>()
        {
            object list;
            if (_cache.TryGetValue(typeof(T), out list))
            {
                if (list != null && (list as System.Collections.IList).Count > 0)
                {
                    return (List<T>)list;
                }
            }

            if (list == null || (list as System.Collections.IList).Count == 0)
            {
                using (RedisClient redisClient = new RedisClient())
                {
                    IRedisTypedClient<T> redis = redisClient.As<T>();
                    return redis.Lists[typeof(T).ToString()].ToList();
                    //return redisClient.Get<T>(typeof(T).ToString());
                }
            }
            return default(List<T>);
        }

        ///// <summary>
        ///// Add single {T} into cache and Data Store.
        ///// </summary>
        ///// <typeparam name="T">class</typeparam>
        ///// <param name="entity">class object</param>
        //public static void Create<T>(T entity) where T : class
        //{
        //    List<object> list;
        //    if (!_cache.TryGetValue(typeof(T), out list))
        //    {
        //        list = new List<object>();
        //    }
        //    list.Add(entity);
        //    _cache[typeof(T)] = list;
        //    RedisStore<T>(entity);
        //}

        ///// <summary>
        ///// Find Single {T} in object cache.
        ///// </summary>
        ///// <typeparam name="T">class</typeparam>
        ///// <param name="predicate">linq statement</param>
        ///// <returns></returns>
        //public static T Read<T>(Func<T, bool> predicate) where T : class
        //{
        //    List<object> list;
        //    if (_cache.TryGetValue(typeof(T), out list))
        //    {
        //        return list.Cast<T>().Where(predicate).FirstOrDefault();
        //    }
        //    return null;
        //}

        ///// <summary>
        ///// Tries to update or Add entity to object cache and Data Store.
        ///// </summary>
        ///// <typeparam name="T">class</typeparam>
        ///// <param name="predicate">linq expression</param>
        ///// <param name="entity">entity</param>
        //public static void Update<T>(Func<T, bool> predicate, T entity) where T : class
        //{
        //    List<object> list;
        //    if (_cache.TryGetValue(typeof(T), out list))
        //    {
        //        // Look for old entity.
        //        var e = list.Cast<T>().Where(predicate).FirstOrDefault();

        //        if (e != null)
        //        {
        //            list.Remove(e);
        //        }

        //        // Regardless if object existed or not we add it to our Cache / Data Store.
        //        list.Add(entity);
        //        _cache[typeof(T)] = list;
        //        RedisStore<T>(entity);
        //    }
        //}

        ///// <summary>
        ///// Delete single {T} from cache and Data Store.
        ///// </summary>
        ///// <typeparam name="T">class</typeparam>
        ///// <param name="entity">class object</param>
        //public static void Delete<T>(T entity) where T : class
        //{
        //    List<object> list;
        //    if (_cache.TryGetValue(typeof(T), out list))
        //    {
        //        list.Remove(entity);
        //        _cache[typeof(T)] = list;

        //        RedisDelete<T>(entity);
        //    }
        //}

        ///// <summary>
        ///// Find List<T>(predicate) in cache.
        ///// </summary>
        ///// <typeparam name="T">class</typeparam>
        ///// <param name="predicate">linq statement</param>
        ///// <returns></returns>
        //public static List<T> FindBy<T>(Func<T, bool> predicate) where T : class
        //{
        //    List<object> list;
        //    if (_cache.TryGetValue(typeof(T), out list))
        //    {
        //        return list.Cast<T>().Where(predicate).ToList();
        //    }
        //    return new List<T>();
        //}

        //public static long Next<T>() where T : class
        //{
        //    using (var ctx = m.GetClient())
        //        return ctx.As<T>().GetNextSequence();
        //}

        //private static void RedisDelete<T>(T entity) where T : class
        //{
        //    using (var ctx = m.GetClient())
        //        ctx.As<T>().Delete(entity);
        //}

        //private static T RedisFind<T>(long id) where T : class
        //{
        //    using (var ctx = m.GetClient())
        //        return ctx.As<T>().GetById(id);
        //}

        //private static void RedisStore<T>(T entity) where T : class
        //{
        //    using (var ctx = m.GetClient())
        //    {
        //        if (typeof(T) == typeof(MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY))
        //        {
        //            ctx.Store<MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY>(entity as MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY);
        //        }
        //        else if (typeof(T) == typeof(MOS.EFMODEL.DataModels.V_HIS_SERVICE))
        //        {
        //            ctx.Store<MOS.EFMODEL.DataModels.V_HIS_SERVICE>(entity as MOS.EFMODEL.DataModels.V_HIS_SERVICE);
        //        }
        //        else if (typeof(T) == typeof(MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE))
        //        {
        //            ctx.Store<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE>(entity as MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE);
        //        }
        //        else if (typeof(T) == typeof(MOS.EFMODEL.DataModels.V_HIS_MATERIAL_TYPE))
        //        {
        //            ctx.Store<MOS.EFMODEL.DataModels.V_HIS_MATERIAL_TYPE>(entity as MOS.EFMODEL.DataModels.V_HIS_MATERIAL_TYPE);
        //        }
        //        else if (typeof(T) == typeof(MOS.EFMODEL.DataModels.HIS_ICD))
        //        {
        //            ctx.Store<MOS.EFMODEL.DataModels.HIS_ICD>(entity as MOS.EFMODEL.DataModels.HIS_ICD);
        //        }
        //        else if (typeof(T) == typeof(MOS.EFMODEL.DataModels.HIS_MEDI_ORG))
        //        {
        //            ctx.Store<MOS.EFMODEL.DataModels.HIS_MEDI_ORG>(entity as MOS.EFMODEL.DataModels.HIS_MEDI_ORG);
        //        }
        //    }
        //}

        //private static void RedisStoreAll<T>(List<T> entitys)
        //{
        //    using (var ctx = m.GetClient())
        //    {
        //        if (typeof(T) == typeof(MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY))
        //        {
        //            ctx.StoreAll<MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY>(entitys.Cast<MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY>());
        //        }
        //        else if (typeof(T) == typeof(MOS.EFMODEL.DataModels.V_HIS_SERVICE))
        //        {
        //            ctx.StoreAll<MOS.EFMODEL.DataModels.V_HIS_SERVICE>(entitys.Cast<MOS.EFMODEL.DataModels.V_HIS_SERVICE>());
        //        }
        //        else if (typeof(T) == typeof(MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE))
        //        {
        //            ctx.StoreAll<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE>(entitys.Cast<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE>());
        //        }
        //        else if (typeof(T) == typeof(MOS.EFMODEL.DataModels.V_HIS_MATERIAL_TYPE))
        //        {
        //            ctx.StoreAll<MOS.EFMODEL.DataModels.V_HIS_MATERIAL_TYPE>(entitys.Cast<MOS.EFMODEL.DataModels.V_HIS_MATERIAL_TYPE>());
        //        }
        //        else if (typeof(T) == typeof(MOS.EFMODEL.DataModels.HIS_ICD))
        //        {
        //            ctx.StoreAll<MOS.EFMODEL.DataModels.HIS_ICD>(entitys.Cast<MOS.EFMODEL.DataModels.HIS_ICD>());
        //        }
        //        else if (typeof(T) == typeof(MOS.EFMODEL.DataModels.HIS_MEDI_ORG))
        //        {
        //            ctx.StoreAll<MOS.EFMODEL.DataModels.HIS_MEDI_ORG>(entitys.Cast<MOS.EFMODEL.DataModels.HIS_MEDI_ORG>());
        //        }
        //    }
        //}

        static bool AddOrUpdate<T>(string key, List<T> value)
        {
            bool isSuccess = false;
            using (RedisClient redisClient = new RedisClient())
            {
                IRedisTypedClient<T> redisT = redisClient.As<T>();                 
                foreach (var item in redisT.Lists[typeof(T).ToString()].ToList())
                {
                    //var client = new ServiceStack.ServiceClient.Web.JsonServiceClient("");
                    //List<Todo> all = client.Get(new Todos());           // Count = 0


                    T tFive = redisT.GetValue(5.ToString());
                    if (tFive == null)
                    {
                        // make a small delay
                        //Thread.Sleep(5000);
                        //tFive = new T
                        //{
                        //    Id = 5,
                        //    Manufacturer = "Motorolla",
                        //    Model = "xxxxx",
                        //    Owner = new Person
                        //    {
                        //        Id = 1,
                        //        Age = 90,
                        //        Name = "OldOne",
                        //        Profession = "sportsmen",
                        //        Surname = "OldManSurname"
                        //    }
                        //};
                        //redisT.SetEntry(tFive.Id.ToString(), tFive);
                    }
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

        public static bool ConstainKey(string key)
        {
            using (RedisClient redisClient = new RedisClient())
            {
                return redisClient.ContainsKey(key);
            }
        }
        #endregion
    }
}

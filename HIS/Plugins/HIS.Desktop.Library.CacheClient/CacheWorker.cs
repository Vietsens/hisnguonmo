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
using HIS.Desktop.Library.CacheClient.Redis;
using HIS.Desktop.Library.CacheClient.Sqlites;
using Inventec.Common.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Library.CacheClient
{
    public class CacheWorker
    {
        internal RedisWorker RedisWorker { get { return (RedisWorker)Worker.Get<RedisWorker>(); } }
        internal SqliteWorker SqliteWorker { get { return (SqliteWorker)Worker.Get<SqliteWorker>(); } }

        public CacheWorker() { }

        public void InitDataCacheLocal()
        {
            switch (SerivceConfig.CacheType)
            {
                case 1:
                    SqliteWorker.InitDataCacheLocal();
                    break;
                case 2:
                    if (!IsExistRedisService(RedisConstans.RedisServiceName, Environment.MachineName))
                    {
                        //new RedisProcess().AutoInstallAndStartService();
                    }
                    break;
                default:
                    break;
            }
        }

        public bool ValidTable<T>(string dataKey)
        {
            return SqliteWorker.ValidTable<T>(dataKey);
        }

        public List<T> Get<T>()
        {
            List<T> rs = null;
            try
            {
                return Get<T>(typeof(T).ToString());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return rs;
        }

        public List<T> Get<T>(string key)
        {
            List<T> rs = null;
            try
            {
                switch (SerivceConfig.CacheType)
                {
                    case 1:
                        rs = SqliteWorker.Get<T>(key, "");
                        break;
                    case 2:
                        rs = RedisWorker.Get<T>(key);
                        break;
                    default:
                        break;
                }
                if (rs == null || rs.Count == 0)
                {
                    Inventec.Common.Logging.LogSystem.Info("Get du lieu tu cache khong co du lieu, CacheType = " + SerivceConfig.CacheType + "____RedisSaveType=" + SerivceConfig.RedisSaveType + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rs), rs) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => key), key));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return rs;
        }

        public List<CacheStoreStateRDO> GetState()
        {
            List<CacheStoreStateRDO> rsState = null;
            try
            {
                rsState = RedisWorker.GetState(typeof(CacheStoreStateRDO).ToString());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return rsState;
        }

        public bool Set<T>(List<T> value)
        {
            return Set<T>(value, typeof(T).ToString());
        }

        public bool Set<T>(List<T> value, string key)
        {
            return Set<T>(value, key, true);
        }

        public bool Set<T>(List<T> value, string key, bool isWaitingSync)
        {
            bool rs = false;
            try
            {
                if (value == null || value.Count == 0) throw new ArgumentNullException("data is null");
                switch (SerivceConfig.CacheType)
                {
                    case 1:
                        SqliteWorker.Set<T>(value, key, isWaitingSync);
                        rs = true;
                        break;
                    case 2:
                        RedisWorker.Set<T>(value, key, isWaitingSync);
                        rs = true;
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return rs;
        }

        public bool Delete<T>()
        {
            return Delete(typeof(T).ToString());
        }

        public bool Delete(string dataKey)
        {
            bool success = false;
            try
            {
                switch (SerivceConfig.CacheType)
                {
                    case 1:
                        success = SqliteWorker.Delete(dataKey, "");
                        break;
                    case 2:
                        success = RedisWorker.Delete(dataKey);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return success;
        }

        public bool DeleteWithState(string dataKey)
        {
            bool success = false;
            try
            {
                success = Delete(dataKey);
                switch (SerivceConfig.CacheType)
                {
                    case 1:
                        success = SqliteWorker.Delete("Library.CacheClient.Sqlites.SHC_SYNC", SerivceConfig.KEY + " = " + dataKey) && success;
                        break;
                    case 2:
                        RedisWorker.ResetState(dataKey);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return success;
        }

        public bool DeleteState(string dataKey)
        {
            bool success = false;
            try
            {
                switch (SerivceConfig.CacheType)
                {
                    case 1:
                        success = SqliteWorker.Delete("Library.CacheClient.Sqlites.SHC_SYNC", SerivceConfig.KEY + " = " + dataKey);
                        break;
                    case 2:
                        RedisWorker.ResetState(dataKey);
                        success = true;
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return success;
        }

        public bool SyncState(string dataKey)
        {
            bool success = false;
            try
            {
                switch (SerivceConfig.CacheType)
                {
                    case 1:
                        success = SqliteWorker.Delete("Library.CacheClient.Sqlites.SHC_SYNC", SerivceConfig.KEY + " = " + dataKey);
                        break;
                    case 2:
                        RedisWorker.SyncState(dataKey);
                        success = true;
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return success;
        }

        public bool IsExistRedisService(string serviceName, string machineName)
        {
            return RedisWorker.IsExistRedisService(serviceName, machineName);
        }

        public string GetLastModifyTimeInDB(string key)
        {
            string modifyTime = "";
            try
            {
                switch (SerivceConfig.CacheType)
                {
                    case 1:
                        string shcCahceType = typeof(SHC_SYNC).ToString();
                        string tableNameSync = shcCahceType.Substring(shcCahceType.LastIndexOf(".") + 1);
                        var syncDatas = SqliteWorker.Get<SHC_SYNC>(shcCahceType, "KEY = '" + tableNameSync + "'");
                        if (syncDatas != null && syncDatas.Count > 0)
                        {
                            var syncData = syncDatas.FirstOrDefault(o => o.KEY == key);
                            if (syncData != null)
                            {
                                modifyTime = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(syncData.LAST_DB_MODIFY_TIME);
                            }
                        }
                        break;
                    case 2:
                        var cacheStoreStateRDOs = RedisWorker.Get<CacheStoreStateRDO>(typeof(CacheStoreStateRDO).ToString());
                        if (cacheStoreStateRDOs != null && cacheStoreStateRDOs.Count > 0)
                        {
                            var syncData = cacheStoreStateRDOs.FirstOrDefault(o => o.Key == key);
                            if (syncData != null)
                            {
                                modifyTime = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(syncData.LastDBModifyTime);
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return modifyTime;
        }

        public string GetLastModifyTimeSync(string key)
        {
            string modifyTime = "";
            try
            {
                switch (SerivceConfig.CacheType)
                {
                    case 1:
                        string shcCahceType = typeof(SHC_SYNC).ToString();
                        string tableNameSync = shcCahceType.Substring(shcCahceType.LastIndexOf(".") + 1);
                        var syncDatas = SqliteWorker.Get<SHC_SYNC>(shcCahceType, "KEY = '" + tableNameSync + "'");
                        if (syncDatas != null && syncDatas.Count > 0)
                        {
                            var syncData = syncDatas.FirstOrDefault(o => o.KEY == key);
                            if (syncData != null)
                            {
                                modifyTime = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(syncData.LAST_SYNC_MODIFY_TIME);
                            }
                        }
                        break;
                    case 2:
                        var cacheStoreStateRDOs = RedisWorker.Get<CacheStoreStateRDO>(typeof(CacheStoreStateRDO).ToString());
                        if (cacheStoreStateRDOs != null && cacheStoreStateRDOs.Count > 0)
                        {
                            var syncData = cacheStoreStateRDOs.FirstOrDefault(o => o.Key == key);
                            if (syncData != null)
                            {
                                modifyTime = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(syncData.LastSyncRamModifyTime);
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return modifyTime;
        }

        public bool TruncateAll()
        {
            bool success = false;
            try
            {
                switch (SerivceConfig.CacheType)
                {
                    case 1:
                        var path = System.IO.Path.Combine(Application.StartupPath, @"DB\HIS\HIS.SqliteDBLocal.db");//"DB\HIS\HIS.SqliteDBLocal.db"
                        if (System.IO.File.Exists(path))
                        {
                            File.Delete(path);
                        }
                        success = true;
                        break;
                    case 2:
                        CacheMonitorGet cacheMonitorGet = new CacheMonitorGet();
                        var listCache = cacheMonitorGet.Get();
                        if (listCache != null && listCache.Count > 0)
                        {
                            foreach (var item in listCache)
                            {
                                RedisWorker.Delete(item.CacheMonitorKeyCode);
                                RedisWorker.ResetState(item.CacheMonitorKeyCode);
                            }
                            success = true;
                        }


                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return success;
        }
    }
}

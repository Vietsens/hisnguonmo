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
using Inventec.Common.RedisCache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace HIS.Desktop.Library.CacheClient.Redis
{
    public class RedisWorker
    {
        public RedisWorker() { }

        internal List<T> Get<T>()
        {
            return Get<T>(typeof(T).ToString());
        }

        internal List<T> Get<T>(string dataKey)
        {
            List<T> rs = null;
            try
            {
                if (!IsExistRedisService(RedisConstans.RedisServiceName, Environment.MachineName)) return null;

                try
                {
                    //if (new CacheMonitorGet().IsExistsCode(dataKey))
                    //{
                    switch (SerivceConfig.RedisSaveType)
                    {
                        case RedisSaveType.RawKeyValue:
                            RedisCacheRawKeyValue redisCacheRawKeyValue = new RedisCacheRawKeyValue("", false, true, SerivceConfig.PreNamespaceFolder);
                            rs = redisCacheRawKeyValue.Get<List<T>>(dataKey);
                            break;
                        case RedisSaveType.IRedisList:
                            RedisCacheIRedisList redisCacheIRedisList = new RedisCacheIRedisList("", false, true, SerivceConfig.PreNamespaceFolder);
                            rs = redisCacheIRedisList.Get<T>(dataKey);
                            break;
                        case RedisSaveType.Urn:
                            RedisCacheUrnEntry redisCacheUrnEntry = new RedisCacheUrnEntry("", false, true, SerivceConfig.PreNamespaceFolder);
                            rs = redisCacheUrnEntry.Get<T>(dataKey);
                            break;
                        default:
                            break;
                    }

                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                    //}
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataKey), dataKey));
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return rs;
        }

        internal List<CacheStoreStateRDO> GetState(string dataKey)
        {
            List<CacheStoreStateRDO> rs = null;
            try
            {
                RedisCacheUrnEntry redisCacheUrnEntry = new RedisCacheUrnEntry("", false, true, SerivceConfig.PreNamespaceFolder);
                rs = redisCacheUrnEntry.Get<CacheStoreStateRDO>(dataKey);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return rs;
        }

        internal void Set<T>(List<T> datas)
        {
            Set<T>(datas, typeof(T).ToString());
        }

        internal void Set<T>(List<T> datas, string dataKey)
        {
            Set<T>(datas, dataKey, true);
        }

        internal void Set<T>(List<T> datas, string dataKey, bool? isWaitingSync)
        {
            try
            {
                bool isOK = Set<T>(datas, dataKey, isWaitingSync, SerivceConfig.RedisSaveType);
                if (!isOK)
                {
                    Inventec.Common.Logging.LogSystem.Info("RedisWorker.Set fail. Loi khi luu du lieu vao Redis cache ____"
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataKey), dataKey)
                    + "____" + Inventec.Common.Logging.LogUtil.TraceData("count", datas.Count));
                }
                else
                {
                    SyncState<T>(dataKey, isWaitingSync, datas);

                    Inventec.Common.Logging.LogSystem.Info("RedisWorker.Set success. Luu du lieu vao Redis cache thanh cong ____"
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataKey), dataKey)
                    + "____" + Inventec.Common.Logging.LogUtil.TraceData("count", datas.Count));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal bool Set<T>(List<T> datas, string dataKey, bool? isWaitingSync, RedisSaveType redisSaveType)
        {
            bool success = false;
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("RedisWorker.Set 1 ...");
                if (datas == null || datas.Count == 0) throw new ArgumentNullException("datas is null");

                if (!IsExistRedisService(RedisConstans.RedisServiceName, Environment.MachineName))
                {
                    Inventec.Common.Logging.LogSystem.Info("Redis service not install on computer " + Environment.MachineName);
                    return false;
                }
                Inventec.Common.Logging.LogSystem.Debug("RedisWorker.Set 2 ...");
                switch (redisSaveType)
                {
                    case RedisSaveType.RawKeyValue:
                        RedisCacheRawKeyValue redisCacheRawKeyValue = new RedisCacheRawKeyValue("", false, true, SerivceConfig.PreNamespaceFolder);
                        success = redisCacheRawKeyValue.Set<List<T>>(dataKey, datas);
                        break;
                    case RedisSaveType.IRedisList:
                        RedisCacheIRedisList redisCacheIRedisList = new RedisCacheIRedisList("", false, true, SerivceConfig.PreNamespaceFolder);
                        success = redisCacheIRedisList.Set<T>(dataKey, datas);
                        break;
                    case RedisSaveType.Urn:
                        RedisCacheUrnEntry redisCacheUrnEntry = new RedisCacheUrnEntry("", false, true, SerivceConfig.PreNamespaceFolder);
                        success = redisCacheUrnEntry.Set<T>(dataKey, datas);
                        break;
                    default:
                        break;
                }
                Inventec.Common.Logging.LogSystem.Debug("RedisWorker.Set 3 ...");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return success;
        }

        internal bool SyncState<T>(string dataKey, bool? isWaitingSync, List<T> datas)
        {
            bool success = false;
            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => isWaitingSync), isWaitingSync));
            if (isWaitingSync.HasValue && dataKey != typeof(CacheStoreStateRDO).ToString())
            {
                long modifyTimeNew = (Utils.GetModifyTimeMax<T>(datas) ?? 0);

                List<CacheStoreStateRDO> cacheStoreStateRDOs = new List<CacheStoreStateRDO>();
                cacheStoreStateRDOs.Add(new CacheStoreStateRDO() { Key = dataKey, IsWaitingSync = isWaitingSync.Value, LastDBModifyTime = modifyTimeNew, LastSyncRamModifyTime = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss")) });

                RedisCacheUrnEntry redisCacheUrnEntry = new RedisCacheUrnEntry("", false, true, SerivceConfig.PreNamespaceFolder);
                success = redisCacheUrnEntry.Set<CacheStoreStateRDO>(typeof(CacheStoreStateRDO).ToString(), cacheStoreStateRDOs, "Key");
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataKey), dataKey) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => modifyTimeNew), modifyTimeNew) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => success), success));
            }
            return success;
        }

        internal bool SyncState(string dataKey)
        {
            bool success = false;
            List<CacheStoreStateRDO> cacheStoreStateRDOs = new List<CacheStoreStateRDO>();
            RedisCacheUrnEntry redisCacheUrnEntry = new RedisCacheUrnEntry("", false, true, SerivceConfig.PreNamespaceFolder);
            var cacheStoreStateAlls = redisCacheUrnEntry.Get<CacheStoreStateRDO>(typeof(CacheStoreStateRDO).ToString());
            if (cacheStoreStateAlls != null && cacheStoreStateAlls.Count > 0)
            {
                var cacheStoreStateOne = cacheStoreStateAlls.FirstOrDefault(o => o.Key == dataKey);
                if (cacheStoreStateOne != null)
                {
                    cacheStoreStateOne.IsWaitingSync = false;
                    cacheStoreStateOne.LastSyncRamModifyTime = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
                    
                    cacheStoreStateRDOs.Add(cacheStoreStateOne);
                    success = redisCacheUrnEntry.SetWithSync<CacheStoreStateRDO>(typeof(CacheStoreStateRDO).ToString(), cacheStoreStateRDOs, "Key");
                }
            }

            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataKey), dataKey) + "__" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => success), success));

            return success;
        }

        internal void ResetState(string dataKey)
        {
            RedisCacheUrnEntry redisCacheUrnEntry = new RedisCacheUrnEntry("", false, true, SerivceConfig.PreNamespaceFolder);

            redisCacheUrnEntry.DeleteById(String.Format("{0}:{1}", typeof(CacheStoreStateRDO).ToString(), dataKey));
        }

        internal bool Delete(string dataKey)
        {
            bool success = false;
            try
            {
                switch (SerivceConfig.RedisSaveType)
                {
                    case RedisSaveType.RawKeyValue:
                        RedisCacheRawKeyValue redisCacheRawKeyValue = new RedisCacheRawKeyValue("", false, true, SerivceConfig.PreNamespaceFolder);
                        redisCacheRawKeyValue.Delete(dataKey);
                        success = true;
                        break;
                    case RedisSaveType.IRedisList:
                        RedisCacheIRedisList redisCacheIRedisList = new RedisCacheIRedisList("", false, true, SerivceConfig.PreNamespaceFolder);
                        redisCacheIRedisList.Delete(dataKey);
                        success = true;
                        break;
                    case RedisSaveType.Urn:
                        RedisCacheUrnEntry redisCacheUrnEntry = new RedisCacheUrnEntry("", false, true, SerivceConfig.PreNamespaceFolder);
                        redisCacheUrnEntry.Delete(dataKey);
                        success = true;
                        break;
                    default:
                        break;
                }

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dataKey), dataKey));
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return success;
        }

        internal bool IsExistRedisService(string serviceName, string machineName)
        {
            try
            {
                bool success = false;
                ServiceController[] services = ServiceController.GetServices(machineName);
                var service = services.FirstOrDefault(s => s.ServiceName == serviceName);
                success = (service != null);
                if (success && service.Status != ServiceControllerStatus.Running)
                {
                    System.Diagnostics.ProcessStartInfo proc = new System.Diagnostics.ProcessStartInfo();
                    proc.WorkingDirectory = @"C:\Windows\System32";
                    proc.FileName = @"C:\Windows\System32\cmd.exe";
                    proc.Verb = "runas";
                    proc.UseShellExecute = true;
                    proc.CreateNoWindow = false;
                    proc.LoadUserProfile = true;
                    proc.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                    proc.Arguments = "/C sc start " + RedisConstans.RedisServiceName;
                    System.Diagnostics.Process.Start(proc);
                }
                return success;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return false;
        }
    }
}

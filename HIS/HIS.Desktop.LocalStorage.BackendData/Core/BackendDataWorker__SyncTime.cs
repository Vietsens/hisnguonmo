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
using HIS.Desktop.Library.CacheClient;
using HIS.Desktop.Library.CacheClient.Redis;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Common.Logging;
using Inventec.Common.Repository;
using Inventec.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace HIS.Desktop.LocalStorage.BackendData
{
    public partial class BackendDataWorker
    {
        public static string SetSyncTime<T>(string timeSync)
        {
            string result = "";
            try
            {
                Type type = typeof(T);
                result = (!String.IsNullOrEmpty(timeSync) ? timeSync : DateTime.Now.ToString("yyyyMMddHHmmss"));
                if (dicTimeSync.ContainsKey(type))
                {
                    string outValue = null;
                    if (!dicTimeSync.TryRemove(type, out outValue))
                    {
                        LogSystem.Info("Khong Remove duoc cau hinh trong dictionary dicTimeSync Key: " + type.ToString());
                        dicTimeSync.TryUpdate(type, timeSync, null);
                    }
                    else
                    {
                        if (!dicTimeSync.TryAdd(type, timeSync))
                        {
                            LogSystem.Info("Khong Add duoc cau hinh vao dictionary dicTimeSync Key: " + type.ToString());
                        }
                    }
                }
                else
                {
                    if (!dicTimeSync.TryAdd(type, result))
                    {
                        LogSystem.Info("Khong Add duoc cau hinh vao dictionary dicTimeSync Key: " + type.ToString());
                    }
                }

                //Type type = typeof(T);
                //var ts = dicTimeSync.FirstOrDefault(o => o.DataType == type);
                //if (ts == null || ts.Data == null)
                //{
                //    if (!String.IsNullOrEmpty(timeSync))
                //        result = timeSync;
                //    else
                //        result = DateTime.Now.ToString("yyyyMMddHHmmss");
                //    dicTimeSync.Add(new TimeSyncADO(type, result));
                //}
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return "";
        }

        public static string SetSyncTime(Type type, string timeSync)
        {
            string result = "";
            try
            {
                result = (!String.IsNullOrEmpty(timeSync) ? timeSync : DateTime.Now.ToString("yyyyMMddHHmmss"));
                if (dicTimeSync.ContainsKey(type))
                {
                    string outValue = null;
                    if (!dicTimeSync.TryRemove(type, out outValue))
                    {
                        LogSystem.Info("Khong Remove duoc cau hinh trong dictionary Key: " + type.ToString());
                        dicTimeSync.TryUpdate(type, timeSync, null);
                    }
                    else
                    {
                        if (!dicTimeSync.TryAdd(type, timeSync))
                        {
                            LogSystem.Info("Khong Add duoc cau hinh vao dictionary Key: " + type.ToString());
                        }
                    }
                }
                else
                {
                    if (!dicTimeSync.TryAdd(type, result))
                    {
                        LogSystem.Info("Khong Add duoc cau hinh vao dictionary Key: " + type.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        public static string GetSyncTime<T>() where T : class
        {
            string result = "";
            try
            {
                Type type = typeof(T);
                dicTimeSync.TryGetValue(type, out result);
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return "";
        }

        public static string GetSyncTime(Type type)
        {
            string result = "";
            try
            {
                dicTimeSync.TryGetValue(type, out result);
                if (String.IsNullOrEmpty(result))
                {
                    Inventec.Common.Logging.LogSystem.Warn("type = " + type.ToString() + " GetSyncTime not found");
                }
            }
            catch (Exception ex)
            {
                result = "";
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

    }
}

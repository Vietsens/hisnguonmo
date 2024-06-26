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
using HIS.Desktop.LocalStorage.BackendData.Core.Reset;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Common.Repository;
using Inventec.Core;
using RDCACHE.SDO;
using System;
using System.Collections.Concurrent;

namespace HIS.Desktop.LocalStorage.BackendData
{
    class BackendDataReset
    {
        internal CacheMonitorGet CacheMonitorGet { get { return (CacheMonitorGet)Worker.Get<CacheMonitorGet>(); } }
        internal CacheMonitorSync CacheMonitorSync { get { return (CacheMonitorSync)Worker.Get<CacheMonitorSync>(); } }
        internal CacheWorker CacheWorker { get { return (CacheWorker)Worker.Get<CacheWorker>(); } }

        public BackendDataReset() { }

        internal bool Reset(Type type, ConcurrentDictionary<Type, object> dic, ConcurrentDictionary<Type, string> dicTimeSync)
        {
            bool result = true;
            try
            {
                string dataKey = type.ToString();
                if (dic.ContainsKey(type))
                {
                    object outValue = null;
                    if (!dic.TryRemove(type, out outValue))
                    {
                        result = false;
                        LogSystem.Info("Khong Remove duoc cau hinh trong dictionary dic dataKey: " + dataKey);
                    }
                    else
                    {
                        result = true;
                    }
                }

                if (dicTimeSync.ContainsKey(type))
                {
                    string outValue = null;
                    if (!dicTimeSync.TryRemove(type, out outValue))
                    {
                        LogSystem.Info("Khong Remove duoc cau hinh trong dictionary dicTimeSync dataKey: " + dataKey);
                    }
                }

                if (CacheMonitorGet.IsExistsCode(dataKey))
                {
                    CacheWorker.DeleteWithState(dataKey);

                    //Call api reset & reload data in cache server redis nếu có cấu hình sử dụng cache server
                    //if (Core.CFG.HisConfigCFG.IsUseRedisCacheServer)
                    //{
                    //    try
                    //    {
                    //        CommonParam param = new CommonParam();
                    //        RdCacheSDO cacheSDO = new RDCACHE.SDO.RdCacheSDO();
                    //        cacheSDO.BRANCH_ID = BranchDataWorker.GetCurrentBranchId();
                    //        string uri = GenerateUri.GetTruncateUriByType(dataKey);
                    //        bool truncateState = new BackendAdapter(param).Post<bool>(uri, HIS.Desktop.ApiConsumer.ApiConsumers.RdCacheConsumer, cacheSDO, param);
                    //        LogSystem.Info(String.Format("Call api {0} : {1} ", uri, truncateState));
                    //    }
                    //    catch (Exception exx)
                    //    {
                    //        Inventec.Common.Logging.LogSystem.Warn(exx);
                    //    }
                    //}
                }

                this.ResetExtDataRelaytion(type);

                Inventec.Common.Logging.LogSystem.Info("Reset data" + dataKey + " in cache & ram" + result);
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        void ResetExtDataRelaytion(Type type)
        {
            try
            {
                if (type is MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY)
                {
                    BranchDataWorker.ResetServicePaty();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}

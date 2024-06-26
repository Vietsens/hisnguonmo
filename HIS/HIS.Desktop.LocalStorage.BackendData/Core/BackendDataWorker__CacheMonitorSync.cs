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
        public static void CacheMonitorSyncExecute<T>() where T : class
        {
            try
            {
                Type type = typeof(T);
                CacheMonitorSync.Create(type.ToString(), false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public static void CacheMonitorSyncExecute<T>(bool isReload) where T : class
        {
            try
            {
                Type type = typeof(T);
                CacheMonitorSync.Create(type.ToString(), isReload);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public static void CacheMonitorSyncExecute(Type type)
        {
            try
            {
                CacheMonitorSync.Create(type.ToString(), false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public static void CacheMonitorSyncExecute(Type type, bool isReload)
        {
            try
            {
                CacheMonitorSync.Create(type.ToString(), isReload);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public static void CacheMonitorSyncExecute(string dataKey, bool isReload)
        {
            try
            {
                CacheMonitorSync.Create(dataKey, isReload);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

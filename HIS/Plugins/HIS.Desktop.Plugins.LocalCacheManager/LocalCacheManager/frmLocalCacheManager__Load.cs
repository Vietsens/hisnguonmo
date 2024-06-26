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
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Logging;
using Inventec.Desktop.Common.Message;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.LocalCacheManager
{
    public partial class frmLocalCacheManager : HIS.Desktop.Utility.FormBase
    {
        List<HIS.Desktop.Library.CacheClient.Sqlites.SHC_SYNC> syncDatas;
        List<HIS.Desktop.Library.CacheClient.Redis.CacheStoreStateRDO> cacheStoreStateRDOs;

        string GetLastModifyTimeSync(string key)
        {
            string modifyTime = "";
            try
            {
                if (CacheMonitorGet.IsExistsCode(key))
                {
                    switch (SerivceConfig.CacheType)
                    {
                        case 1:
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
                            if (cacheStoreStateRDOs != null && cacheStoreStateRDOs.Count > 0)
                            {
                                var syncData = cacheStoreStateRDOs.FirstOrDefault(o => o.Key == key);
                                if (syncData != null)
                                {
                                    modifyTime = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(syncData.LastSyncRamModifyTime);
                                }
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return modifyTime;
        }

        string GetLastModifyTimeInDB(string key)
        {
            string modifyTime = "";
            try
            {
                if (CacheMonitorGet.IsExistsCode(key))
                {
                    switch (SerivceConfig.CacheType)
                    {
                        case 1:
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
                            if (cacheStoreStateRDOs != null && cacheStoreStateRDOs.Count > 0)
                            {
                                var syncData = cacheStoreStateRDOs.FirstOrDefault(o => o.Key == key);
                                if (syncData != null)
                                {
                                    modifyTime = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(syncData.LastDBModifyTime);
                                }
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return modifyTime;
        }

        private void LoadData()
        {
            try
            {
                this.cacheDatas = new List<CacheData>();
                var allDatas = BackendDataWorker.GetAll().Where(o => o.Key.ToString() != "HIS.Desktop.LocalStorage.BackendData.ADO.AgeADO" && o.Key.ToString() != "HIS.Desktop.LocalStorage.BackendData.ADO.CommuneADO").ToList();
                switch (SerivceConfig.CacheType)
                {
                    case 1:
                        this.syncDatas = CacheWorker.Get<HIS.Desktop.Library.CacheClient.Sqlites.SHC_SYNC>();
                        break;
                    case 2:
                        this.cacheStoreStateRDOs = CacheWorker.Get<HIS.Desktop.Library.CacheClient.Redis.CacheStoreStateRDO>();
                        break;
                }
                foreach (var item in allDatas)
                {
                    CacheData cacheData = new CacheData();
                    cacheData.ObjectName = item.Key.ToString();
                    cacheData.ObjectType = item.Key;
                    //cacheData.LastModifyTimeSync = GetLastModifyTimeSync(item.Key.ToString());
                    //cacheData.LastModifyTimeInDB = GetLastModifyTimeInDB(item.Key.ToString());
                    cacheData.LastModifyTimeInRam = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(Inventec.Common.TypeConvert.Parse.ToInt64(BackendDataWorker.GetSyncTime(item.Key)));
                    var captionData = HIS.Desktop.XmlRamMonitor.RamMonitorKeyStore.GetByCode(item.Key.ToString());
                    if (captionData != null && !String.IsNullOrEmpty(captionData.RamMonitorKeyName))
                    {
                        cacheData.Description = captionData.RamMonitorKeyName;
                    }

                    //var captionDataCache = HIS.Desktop.XmlCacheMonitor.CacheMonitorKeyStore.GetByCode(item.Key.ToString());
                    //if (captionDataCache != null)
                    //{
                    //    cacheData.ISTL = (cacheData.ISTL != "1" ? String.Empty : captionDataCache.IsReload);
                    //}
                    this.cacheDatas.Add(cacheData);
                }

                gridView1.BeginUpdate();
                gridView1.GridControl.DataSource = this.GetDataSourceWithFilter();
                gridView1.EndUpdate();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private List<CacheData> GetDataSourceWithFilter()
        {
            return this.cacheDatas.Where(o =>
                    ((o.ObjectName ?? "").ToLower()).Contains(this.txtKeyword.Text.ToLower())
                    || (o.Description ?? "").ToLower().Contains(this.txtKeyword.Text.ToLower())).OrderByDescending(o => o.LastModifyTimeInRam).ThenBy(o => o.ObjectName).ToList();
        }

    }
}

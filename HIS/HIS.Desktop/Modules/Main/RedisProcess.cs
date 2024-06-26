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
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.Location;
using Inventec.Common.Logging;
using Inventec.Common.Repository;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Linq;

namespace HIS.Desktop.Modules.Main
{
    class RedisProcess : ProcessBase
    {
        internal CacheWorker CacheWorker { get { return (CacheWorker)Worker.Get<CacheWorker>(); } }

        internal RedisProcess() { }

        internal bool Sync()
        {
            bool success = true;
            //List<CacheStoreStateRDO> redisStoreStateProcessed = new List<CacheStoreStateRDO>();

            var dataStoreStates = CacheWorker.GetState().Where(o => o.IsWaitingSync).ToList();
            if (dataStoreStates != null && dataStoreStates.Count > 0)
            {
                foreach (var dataSS in dataStoreStates)
                {
                    Type t = GetTypeByName(dataSS.Key);
                    long lastModifyTime = 0;
                    IList dataSave = GetDataByKey(t, dataSS.Key);

                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => t), t));
                    //Update dữ liệu từ redis local vào RAM
                    if (t != null && dataSave != null && dataSave.Count > 0)
                    {
                        success = (BackendDataWorker.UpdateToRam(t, dataSave, lastModifyTime));
                        if (success)
                        {
                            Inventec.Common.Logging.LogSystem.Info(dataSS.Key + " sync du lieu tu redis cache local vao RAM thanh cong.____DataCount = " + (dataSave != null ? dataSave.Count : 0));

                            ResetDataExt(dataSS.Key);
                        }
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Info(dataSS.Key + " map to Type fail, sync data to ram fail");
                    }

                    CacheWorker.SyncState(dataSS.Key);
                }

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
            else
            {
                Inventec.Common.Logging.LogSystem.Info("Khog co du lieu can dong bo tu cache vao RAM");
            }

            return success;
        }

        IList GetDataByKey(Type t, string key)
        {
            IList dataSave = null;
            try
            {
                if (t == typeof(MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY))
                {
                    dataSave = CacheWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY>();
                }
                else if (t == typeof(MOS.EFMODEL.DataModels.V_HIS_SERVICE))
                {
                    dataSave = CacheWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE>();
                }
                else if (t == typeof(MOS.EFMODEL.DataModels.V_HIS_SERVICE_ROOM))
                {
                    dataSave = CacheWorker.Get<MOS.EFMODEL.DataModels.V_HIS_SERVICE_ROOM>();
                }
                else if (t == typeof(MOS.EFMODEL.DataModels.HIS_MEDI_ORG))
                {
                    dataSave = CacheWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDI_ORG>();
                }
                else if (t == typeof(MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE))
                {
                    dataSave = CacheWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE>();
                }
                else if (t == typeof(MOS.EFMODEL.DataModels.V_HIS_MATERIAL_TYPE))
                {
                    dataSave = CacheWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MATERIAL_TYPE>();
                }
                else if (t == typeof(MOS.EFMODEL.DataModels.HIS_ICD))
                {
                    dataSave = CacheWorker.Get<MOS.EFMODEL.DataModels.HIS_ICD>();
                }
                else if (t == typeof(MOS.EFMODEL.DataModels.HIS_CACHE_MONITOR))
                {
                    dataSave = CacheWorker.Get<MOS.EFMODEL.DataModels.HIS_CACHE_MONITOR>();
                }
                else if (t == typeof(SDA.EFMODEL.DataModels.SDA_ETHNIC))
                {
                    dataSave = CacheWorker.Get<SDA.EFMODEL.DataModels.SDA_ETHNIC>();
                }
                else if (t == typeof(SDA.EFMODEL.DataModels.V_SDA_COMMUNE))
                {
                    dataSave = CacheWorker.Get<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>();
                }
                else if (t == typeof(SDA.EFMODEL.DataModels.SDA_ETHNIC))
                {
                    dataSave = CacheWorker.Get<SDA.EFMODEL.DataModels.SDA_ETHNIC>();
                }
                else if (t == typeof(SDA.EFMODEL.DataModels.SDA_NATIONAL))
                {
                    dataSave = CacheWorker.Get<SDA.EFMODEL.DataModels.SDA_NATIONAL>();
                }
                else if (t == typeof(SDA.EFMODEL.DataModels.V_SDA_DISTRICT))
                {
                    dataSave = CacheWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>();
                }
                else if (t == typeof(SDA.EFMODEL.DataModels.V_SDA_PROVINCE))
                {
                    dataSave = CacheWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>();
                }
                else
                {
                    bool successDelLostData = CacheWorker.SyncState(key);

                    Inventec.Common.Logging.LogSystem.Info("Xoa dong du lieu dong bo thua (xoa key " + key + " trong bang CacheStoreStateRDO) khong co trong danh sach du lieu hop le luu vao cache____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => successDelLostData), successDelLostData));
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return dataSave;
        }

        internal bool AutoInstallAndStartService()
        {
            try
            {
                if (CacheWorker.IsExistRedisService(RedisConstans.RedisServiceName, Environment.MachineName))
                    return true;

                LogSystem.Info("Service windows " + RedisConstans.RedisServiceName + " cache not found -- > Install Redis cache service to computer " + Environment.MachineName);


                string sourcePath = Path.Combine(Path.Combine(Path.Combine(Path.Combine(ApplicationStoreLocation.ApplicationDirectory, "Integrate"), "Cache"), "Setup"), "Redis");
                if (!Directory.Exists(sourcePath)) return true;

                //Process p = new Process();
                System.Diagnostics.ProcessStartInfo proc = new System.Diagnostics.ProcessStartInfo();
                proc.WorkingDirectory = @"C:\Windows\System32";
                proc.FileName = @"C:\Windows\System32\cmd.exe";
                proc.Verb = "runas";
                proc.UseShellExecute = true;
                proc.CreateNoWindow = false;
                proc.LoadUserProfile = true;
                proc.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                //string createService = "/C sc create " + RedisConstans.RedisServiceName + " obj= \"NT AUTHORITY\\NetworkService\\\" start= auto DisplayName= Redis binpath= \"" + servicePath + "\\redis-server.exe\" --service-run \"" + servicePath + "\\redis.windows-service.conf\"";
                string createService = "";
                if (Environment.Is64BitOperatingSystem)
                {
                    createService = "/C msiexec /quiet /i \"" + sourcePath + "\\x64\\Redis-Windows-x64.msi\"";

                    Inventec.Common.Logging.LogSystem.Info(createService);

                    proc.Arguments = createService;
                    Process.Start(proc);
                }
                else
                {
                    string servicePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), RedisConstans.RedisServiceName);

                    //Now Create all of the directories
                    if (!Directory.Exists(servicePath))
                        Directory.CreateDirectory(servicePath);
                    sourcePath = sourcePath + "\\x86";
                    //Copy all the files & Replaces any files with the same name
                    foreach (string newPath in Directory.GetFiles(sourcePath, "*.*",
                        SearchOption.AllDirectories))
                        File.Copy(newPath, newPath.Replace(sourcePath, servicePath), true);

                    createService = "/C cd " + servicePath + " & service-install.bat";

                    Inventec.Common.Logging.LogSystem.Info(createService);

                    proc.Arguments = createService;
                    Process.Start(proc);
                }

                //string startService = "/C sc start " + RedisConstans.RedisServiceName;
                //proc.Arguments = startService;
                //Process.Start(proc);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
            return true;
        }
    }
}

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
using DevExpress.XtraBars.Ribbon;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Logging;
using System;
using System.Linq;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace HIS.Desktop.Modules.Main
{
    public partial class frmMain : RibbonForm
    {
        short isUseCacheLocal = 0;
        const int timeSyncByCacheMonitorDefault = 180000;

        private void RunSyncBackendDataToLocal()
        {
            try
            {
                this.isUseCacheLocal = ConfigApplicationWorker.Get<short>(AppConfigKeys.CONFIG_KEY__HIS_DESKTOP__IS_USE_CACHE_LOCAL);
                int timeSyncSqliteToRam = ConfigApplicationWorker.Get<int>(AppConfigKeys.CONFIG_KEY__HIS_DESKTOP__TIME_CACHE_SYNC_TO_RAM);
                if (this.isUseCacheLocal == 1 || this.isUseCacheLocal == 2)
                {
                    int timeSyncByCacheMonitor = ConfigApplicationWorker.Get<int>(AppConfigKeys.CONFIG_KEY__HIS_DESKTOP__CACHE_LOCAL_TIME_SYNC_BY_CACHE_MONITOR);
                    int timeSyncAll = ConfigApplicationWorker.Get<int>(AppConfigKeys.CONFIG_KEY__HIS_DESKTOP__CACHE_LOCAL_TIME_SYNC_ALL);

                    Random rnd = new Random();
                    if (timeSyncAll > 0)
                    {
                        int numrd__timeSyncAll = (rnd.Next((int)(2 * timeSyncAll / 3), timeSyncAll));//Ramdom sinh thời gian để các máy client hạn chế việc gọi server cùng 1 lúc
                        Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => numrd__timeSyncAll), numrd__timeSyncAll));
                        timerSyncAll = new System.Windows.Forms.Timer();
                        timerSyncAll.Interval = numrd__timeSyncAll;
                        timerSyncAll.Enabled = true;
                        timerSyncAll.Tick += timerSyncAll_Tick;
                        timerSyncAll.Start();
                    }

                    if (timeSyncByCacheMonitor > 0)
                    {
                        int numrd__timeSyncByCacheMonitor = (rnd.Next((int)(2 * timeSyncByCacheMonitor / 3), timeSyncByCacheMonitor));//Ramdom sinh thời gian để các máy client hạn chế việc gọi server cùng 1 lúc
                        Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => numrd__timeSyncByCacheMonitor), numrd__timeSyncByCacheMonitor));
                        timerByCacheMonitor = new System.Windows.Forms.Timer();
                        timerByCacheMonitor.Interval = numrd__timeSyncByCacheMonitor;
                        timerByCacheMonitor.Enabled = true;
                        timerByCacheMonitor.Tick += timerSyncByCacheMonitor_Tick;
                        timerByCacheMonitor.Start();
                    }

                    timeSyncSqliteToRam = (timeSyncSqliteToRam == 0 ? timeSyncByCacheMonitorDefault : timeSyncSqliteToRam);

                    System.Windows.Forms.Timer timerSyncToRAM = new System.Windows.Forms.Timer();
                    timerSyncToRAM.Interval = timeSyncSqliteToRam;
                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => timeSyncSqliteToRam), timeSyncSqliteToRam));
                    timerSyncToRAM.Enabled = true;
                    timerSyncToRAM.Tick += timerSyncToRAM_Tick;
                    timerSyncToRAM.Start();

                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void timerSyncAll_Tick(object sender, EventArgs e)
        {
            TheadProcessSyncAllData(1);
        }

        private void timerSyncByCacheMonitor_Tick(object sender, EventArgs e)
        {
            TheadProcessSyncAllData(0);
        }

        private void TheadProcessSyncAllData(object isReloadAllCache)
        {
            try
            {
                Thread thread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(ProcessSyncAllData));
                try
                {
                    thread.Start(isReloadAllCache);
                }
                catch (Exception ex)
                {
                    LogSystem.Error(ex);
                    thread.Abort();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void ProcessSyncAllData(object isReloadAllCache)
        {
            try
            {
                if (!CloseAllApp.IsProcessOpen("HLS.WCFClient"))
                {
                    var tc = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetTokenData();
                    string cmdLn = "";
                    cmdLn += "|AcsBaseUri|" + ConfigSystems.URI_API_ACS;
                    cmdLn += "|SdaBaseUri|" + ConfigSystems.URI_API_SDA;
                    cmdLn += "|SarBaseUri|" + ConfigSystems.URI_API_SAR;
                    cmdLn += "|MosBaseUri|" + ConfigSystems.URI_API_MOS;
                    cmdLn += "|ApplicationCode|" + GlobalVariables.APPLICATION_CODE;
                    cmdLn += "|TokenCode|" + (tc != null ? tc.TokenCode : "");
                    cmdLn += "|CacheType|" + this.isUseCacheLocal;
                    cmdLn += "|RedisSaveType|" + (int)HIS.Desktop.Library.CacheClient.SerivceConfig.RedisSaveType;
                    cmdLn += "|PreNamespaceFolder|" + HIS.Desktop.Library.CacheClient.SerivceConfig.PreNamespaceFolder;
                    cmdLn += "|isReloadAllCache|" + isReloadAllCache;
                    Inventec.Common.Logging.LogSystem.Info("cmdLn = " + cmdLn);

                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.FileName = Application.StartupPath + @"\HLS.WCFClient.exe";
                    startInfo.Arguments = cmdLn;                    
                    Process.Start(startInfo);
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Info("Service HLS.WCFClient dang chay, khong the khoi tao moi");
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void timerSyncToRAM_Tick(object sender, EventArgs e)
        {
            TheadProcessSyncToRAM();
        }

        private void TheadProcessSyncToRAM()
        {
            try
            {
                Task.Factory.StartNew(ProcessSyncToRAM);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            //try
            //{
            //    Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(ProcessSyncToRAM));
            //    try
            //    {
            //        thread.Start();
            //    }
            //    catch (Exception ex)
            //    {
            //        LogSystem.Error(ex);
            //        thread.Abort();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    LogSystem.Warn(ex);
            //}
        }

        private void ProcessSyncToRAM()
        {
            try
            {
                LogSystem.Info("Begin ProcessSyncToRAM");
                long useCacheLocal = ConfigApplicationWorker.Get<long>(AppConfigKeys.CONFIG_KEY__HIS_DESKTOP__IS_USE_CACHE_LOCAL);
                switch (useCacheLocal)
                {
                    case 1:
                        new SqliteProcess().Sync();
                        break;
                    case 2:
                        new RedisProcess().Sync();
                        break;
                    default:
                        break;
                }

                Inventec.Common.Logging.LogSystem.Debug("Tổng số dữ liệu đang lưu BackendDataWorker trong cache RAM local Dictionary.count=" + BackendDataWorker.GetAll().Count);
                Inventec.Common.Logging.LogSystem.Debug("Chi tiết các dữ liệu đang lưu BackendDataWorker trong cache RAM local: " + String.Join(", ", BackendDataWorker.GetAll().Keys.Select(k => k.ToString()).ToArray()));

                LogSystem.Info("End ProcessSyncToRAM");
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

    }
}

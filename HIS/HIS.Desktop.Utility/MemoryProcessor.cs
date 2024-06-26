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
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Utility
{
    public class MemoryProcessor
    {
        static ConcurrentDictionary<string, TimerModel> TimerStores = new ConcurrentDictionary<string, TimerModel>();
        public static void DisposeForm(Form extenceInstance)
        {
            try
            {
                if (extenceInstance != null)
                {
                    extenceInstance.Dispose();
                    extenceInstance = null;
                }
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
            catch (Exception exx)
            {
                Inventec.Common.Logging.LogSystem.Warn(exx);
            }
        }

        public static void CalculateMemoryRam()
        {
            try
            {
                //long startBytes = GC.GetTotalMemory(true);
                //decimal memoryUsageusers = (decimal)(startBytes) / (1024 * 1024);
                Process proc = Process.GetCurrentProcess();
                Inventec.Common.Logging.LogSession.Debug(String.Format("{0}____{1}____{2}", GlobalVariables.APPLICATION_CODE, GlobalString.VersionApp, "Dung lượng RAM phần mềm HIS đang chạy là: " + ((decimal)proc.PrivateMemorySize64 / (1024 * 1024)) + "MB"));
            }
            catch (Exception exx)
            {
                Inventec.Common.Logging.LogSystem.Warn(exx);
            }
        }

        public static void CalculateMemoryRam(string description)
        {
            try
            {
                Process proc = Process.GetCurrentProcess();
                Inventec.Common.Logging.LogSession.Debug(String.Format("{0}____{1}____{2}", GlobalVariables.APPLICATION_CODE, GlobalString.VersionApp, (!String.IsNullOrEmpty(description) ? description : "Dung lượng RAM phần mềm HIS đang chạy là:") + ((decimal)proc.PrivateMemorySize64 / (1024 * 1024)) + "MB"));
            }
            catch (Exception exx)
            {
                Inventec.Common.Logging.LogSystem.Warn(exx);
            }
        }

        public static void CalculateMemoryRam(string description, string logtype)
        {
            try
            {
                if (logtype == "DEBUG")
                {
                    Process proc = Process.GetCurrentProcess();
                    Inventec.Common.Logging.LogSession.Debug(String.Format("{0}____{1}____{2}", GlobalVariables.APPLICATION_CODE, GlobalString.VersionApp, (!String.IsNullOrEmpty(description) ? description : "Dung lượng RAM phần mềm HIS đang chạy là:") + ((decimal)proc.PrivateMemorySize64 / (1024 * 1024)) + "MB"));
                }
                else if (logtype == "INFO")
                {
                    string extInfo = String.Format("GDI: {0} - UserOBJ: {1} - HandleCount: {2} - GDIProcessHandleQuota: {3} - USERProcessHandleQuota: {4}", MemoryManagement.GetGuiResourcesGDICount(), MemoryManagement.GetGuiResourcesUserCount(), MemoryManagement.GetHandleCount(), MemoryManagement.GDIProcessHandleQuota(), MemoryManagement.USERProcessHandleQuota());

                    Process proc = Process.GetCurrentProcess();
                    Inventec.Common.Logging.LogSession.Info(String.Format("{0}____{1}____{2}____{3}____{4}____{5}____{6}", GlobalVariables.APPLICATION_CODE, GlobalString.VersionApp, (!String.IsNullOrEmpty(description) ? description : "") + ((decimal)proc.PrivateMemorySize64 / (1024 * 1024)), Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName(), StringUtil.GetIpLocal(), StringUtil.CustomerCode, extInfo));

                }
            }
            catch (Exception exx)
            {
                Inventec.Common.Logging.LogSystem.Warn(exx);
            }
        }

        internal static void RegisterTimer(string moduleLink, string timerKeyName, int timeInterval, Action timerProcess)
        {
            try
            {
                RegisterTimer(moduleLink, timerKeyName, timeInterval, timerProcess, false);
            }
            catch (Exception exx)
            {
                Inventec.Common.Logging.LogSystem.Warn(exx);
            }
        }

        internal static void RegisterTimer(string moduleLink, string timerKeyName, int timeInterval, Action timerProcess, bool isRepeat)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => moduleLink), moduleLink)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => timerKeyName), timerKeyName)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => timeInterval), timeInterval));
                System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
                timer.Interval = timeInterval;
                timer.Tag = String.Format("{0}__{1}", moduleLink, timerKeyName);
                timer.Enabled = false;
                timer.Tick += TimerProcess_ActionTick;

                TimerStores.TryAdd(String.Format("{0}__{1}", moduleLink, timerKeyName), new TimerModel()
                {
                    KeyName = timerKeyName,
                    ModuleLink = moduleLink,
                    TimerInstance = timer,
                    ActionHandler = timerProcess,
                    IsRepeat = isRepeat
                });

                //timer.Start();
            }
            catch (Exception exx)
            {
                Inventec.Common.Logging.LogSystem.Warn(exx);
            }
        }

        static void TimerProcess_ActionTick(object sender, EventArgs e)
        {
            try
            {
                string timerKeyName = "";
                if (sender is System.Windows.Forms.Timer && (sender as System.Windows.Forms.Timer).Tag != null)
                {
                    timerKeyName = (sender as System.Windows.Forms.Timer).Tag as string;
                }
                if (TimerStores.ContainsKey(timerKeyName))
                {
                    TimerModel timerModel = TimerStores[timerKeyName];
                    if (timerModel != null && timerModel.ActionHandler != null)
                    {
                        var watch = System.Diagnostics.Stopwatch.StartNew();
                        timerModel.ActionHandler();

                        watch.Stop();
                        Inventec.Common.Logging.LogTime.Info(String.Format("{0}____{1}____{2}____{3}____{4}____{5}____{6}____{7}", GlobalVariables.APPLICATION_CODE, GlobalString.VersionApp, (double)((double)watch.ElapsedMilliseconds / (double)1000), timerModel.ModuleLink, "Thời gian xử lý của timer(" + timerModel.KeyName + ")", Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName(), StringUtil.GetIpLocal(), StringUtil.CustomerCode));

                        if (!timerModel.IsRepeat)
                        {
                            timerModel.TimerInstance.Enabled = false;
                            timerModel.TimerInstance.Stop();
                            timerModel.TimerInstance.Dispose();
                            timerModel.TimerInstance = null;
                            TimerModel timerModelOut;
                            TimerStores.TryRemove(timerKeyName, out timerModelOut);
                        }
                    }
                }
            }
            catch (Exception exx)
            {
                Inventec.Common.Logging.LogSystem.Warn(exx);
            }
        }

        public static void StartTimer(string moduleLinkKey, string timerKeyName)
        {
            try
            {
                if (TimerStores.ContainsKey(String.Format("{0}__{1}", moduleLinkKey, timerKeyName)))
                {
                    TimerModel timerModel = TimerStores[String.Format("{0}__{1}", moduleLinkKey, timerKeyName)];
                    if (timerModel != null && timerModel.ActionHandler != null)
                    {
                        Inventec.Common.Logging.LogTime.Debug(timerModel.ModuleLink + timerModel.KeyName + ": Trước khi gọi StartTimer");
                        timerModel.TimerInstance.Enabled = true;
                        timerModel.TimerInstance.Start();
                        Inventec.Common.Logging.LogTime.Debug(timerModel.ModuleLink + timerModel.KeyName + ": Sau khi gọi StartTimer");
                    }
                }
            }
            catch (Exception exx)
            {
                Inventec.Common.Logging.LogSystem.Warn(exx);
            }
        }

        public static void StopTimer(string moduleLinkKey, string timerKeyName)
        {
            try
            {
                if (TimerStores.ContainsKey(String.Format("{0}__{1}", moduleLinkKey, timerKeyName)))
                {
                    TimerModel timerModel = TimerStores[String.Format("{0}__{1}", moduleLinkKey, timerKeyName)];
                    if (timerModel != null)
                    {
                        Inventec.Common.Logging.LogTime.Debug(timerModel.ModuleLink + timerModel.KeyName + ": Trước khi gọi StopTimer");
                        timerModel.TimerInstance.Enabled = false;
                        timerModel.TimerInstance.Stop();
                        Inventec.Common.Logging.LogTime.Debug(timerModel.ModuleLink + timerModel.KeyName + ": Sau khi gọi StopTimer");
                    }
                }
            }
            catch (Exception exx)
            {
                Inventec.Common.Logging.LogSystem.Warn(exx);
            }
        }

        public static void DisposeTimer(string moduleLinkKey, string timerKeyName)
        {
            try
            {
                if (TimerStores.ContainsKey(String.Format("{0}__{1}", moduleLinkKey, timerKeyName)))
                {
                    TimerModel timerModel = TimerStores[String.Format("{0}__{1}", moduleLinkKey, timerKeyName)];
                    if (timerModel != null)
                    {
                        Inventec.Common.Logging.LogTime.Debug(timerModel.ModuleLink + timerModel.KeyName + ": Trước khi gọi DisposeTimer");
                        timerModel.TimerInstance.Enabled = false;
                        timerModel.TimerInstance.Stop();
                        timerModel.TimerInstance.Dispose();
                        timerModel.TimerInstance = null;
                        TimerModel timerModelOut;
                        bool success = TimerStores.TryRemove(String.Format("{0}__{1}", moduleLinkKey, timerKeyName), out timerModelOut);
                        Inventec.Common.Logging.LogTime.Debug(timerModel.ModuleLink + timerModel.KeyName + ": Sau khi gọi DisposeTimer: " + success);

                    }
                }
            }
            catch (Exception exx)
            {
                Inventec.Common.Logging.LogSystem.Warn(exx);
            }
        }

        public static void DisposeTimerByModuleLink(string moduleLinkKey)
        {
            try
            {
                var timeKeyAccepts = (TimerStores != null && TimerStores.Count > 0) ? TimerStores.Keys.Where(o => o.StartsWith(moduleLinkKey)).ToList() : null;
                if (timeKeyAccepts != null && timeKeyAccepts.Count > 0)
                {
                    foreach (var itemKey in timeKeyAccepts)
                    {
                        if (TimerStores.ContainsKey(itemKey))
                        {
                            TimerModel timerModel = TimerStores[itemKey];
                            if (timerModel != null)
                            {
                                Inventec.Common.Logging.LogTime.Debug(timerModel.ModuleLink + timerModel.KeyName + ": Trước khi gọi DisposeTimerByModuleLink");
                                timerModel.TimerInstance.Enabled = false;
                                timerModel.TimerInstance.Stop();
                                timerModel.TimerInstance.Dispose();
                                timerModel.TimerInstance = null;
                                TimerModel timerModelOut;
                                bool success = TimerStores.TryRemove(itemKey, out timerModelOut);
                                Inventec.Common.Logging.LogTime.Debug(timerModel.ModuleLink + timerModel.KeyName + ": Sau khi gọi DisposeTimerByModuleLink: " + success);
                            }
                        }
                    }
                }
            }
            catch (Exception exx)
            {
                Inventec.Common.Logging.LogSystem.Warn(exx);
            }
        }
    }

    public class TimerModel
    {
        public string ModuleLink { get; set; }
        public string KeyName { get; set; }
        public System.Windows.Forms.Timer TimerInstance { get; set; }
        public Action ActionHandler { get; set; }
        public bool IsRepeat { get; set; }
        public TimerModel()
        {
        }

    }
    public class GlobalString
    {
        static String versionApp;
        public static String VersionApp
        {
            get
            {
                if (String.IsNullOrEmpty(versionApp))
                {
                    versionApp = System.IO.File.Exists(System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, "readme.txt")) ? System.IO.File.ReadAllText(System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, "readme.txt")) : "";
                }
                return versionApp;
            }
            set
            {
                versionApp = value;
            }
        }
    }
}

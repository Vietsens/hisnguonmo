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
using AutoMapper;
using DevExpress.Skins;
using DevExpress.UserSkins;
using HIS.Desktop.ApplicationFont;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Modules.Login;
using Inventec.Aup.Utility;
using Inventec.Common.Logging;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Core;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Security.Permissions;
using System.Windows.Forms;

namespace HIS.Desktop
{
    static class Program
    {
        static string updater = Application.StartupPath + @"\\Integrate\\Aup\\Inventec.AUS.exe";
        static string updaterV2 = Application.StartupPath + @"\\Integrate\\Aup\\Inventec.AutoUpdater.exe";
        static string processToEnd = (ConfigurationManager.AppSettings["Inventec.Desktop.Execute"] ?? "").ToString();
        static string postProcess = Application.StartupPath + @"\" + processToEnd + ".exe";
        const string command = "updated";
        const string commandAUS = "updatedAUS";
        static bool updatedForAUS = false;
        static bool updated = false;
        static readonly bool IsRunUnitTest = ((ConfigurationManager.AppSettings["HIS.Desktop.IsRunUnitTest"] ?? "") == "1");
        static readonly string AupVersion = (ConfigurationManager.AppSettings["HIS.Desktop.AupVersion"] ?? "");

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
        static void Main()
        {
            //Always at least try to let our application code handle the exception.
            //Setting this to "catch" means the Application.ThreadException event
            //will fire first, essentially causing the app to crash right away and shut down.
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException);
            Platform.PluginManager.PluginLoaded += new EventHandler<PluginLoadedEventArgs>(OnPluginProgress);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            log4net.Config.DOMConfigurator.Configure();
            
            try
            {
                
                CloseAllApp.CloseAllApps();
                ApplicationFontWorker.ChangeFontSize(ApplicationFontWorker.GetFontSize());
                Mapper.AssertConfigurationIsValid();//Check mapper
                MessageBoxManager.Register(); //Dang ky su dung
                LanguageManager.Init();
                OfficeSkins.Register();
                BonusSkins.Register();
                SkinManager.EnableFormSkins();
                HIS.Desktop.Base.ResouceManager.InitResourceLanguageManager();
                Inventec.Common.WebApiClient.IpAddressUtils.InitialIpAddressLocal();
                frmLoadConfigSystem frm = new frmLoadConfigSystem();
                frm.ShowDialog();
                if (!IsRunUnitTest && !String.IsNullOrEmpty(Inventec.Aup.Utility.AupConstant.BASE_URI))
                {
                    UnpackCommandline();
                    if (!updatedForAUS && !updated)
                        RunAutoUpdateAup();
                    if (!updated)
                        RunAutoUpdate();
                }
                else
                {
                    LogSystem.Info("Dang chay che do unit test hoac dia chi uri cua backend AUP khong duoc cau hinh____khong the kiem tra duoc phien ban moi____" + Inventec.Aup.Utility.AupConstant.BASE_URI);
                }

                LogSystem.Info("Application_Start. Time=" + DateTime.Now.ToString("yyyyMMddhhmmss"));
                InitialExtAssemble();
                // Add the event handler for handling UI thread exceptions to the event.
                //Application.ThreadException += GlobalThreadExceptionHandler;

                // Add the event handler for handling non-UI thread exceptions to the event. 
                //AppDomain.CurrentDomain.UnhandledException += GlobalUnhandledExceptionHandler;

                GlobalVariables.IsLostToken = true;
                if (IsRunUnitTest)
                {
                    LogSystem.Info("Application run with unit test config.");
                    Application.Run(new HIS.Desktop.Modules.Main.frmMain());
                }
                else
                {
                    LogSystem.Info("Application run normal.");
                    Application.Run(new MyApplicationContext());
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error("Application_Start error. Message=" + ex.Message + ". Time=" + DateTime.Now.ToString("yyyyMMddhhmmss"));
            }
        }

        static void InitialExtAssemble()
        {
            try
            {
                
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        static void InitialConfig()
        {
            try
            {
                var watch1 = System.Diagnostics.Stopwatch.StartNew();
                //Load cau hinh trong file SystemConfig client
                HIS.Desktop.LocalStorage.ConfigSystem.Load.Init();
                HIS.Desktop.LocalStorage.HisConfig.ConfigLoader.Refresh();
                watch1.Stop();
                Inventec.Common.Logging.LogAction.Info(String.Format("{0}____{1}____{2}____{3}____{4}____{5}____{6}____{7}", "HIS", HIS.Desktop.Utility.GlobalString.VersionApp, (double)((double)watch1.ElapsedMilliseconds / (double)1000), "HIS.Desktop", "ConfigLoader:Refresh(HisConfig)", "userapp", HIS.Desktop.Utility.StringUtil.GetIpLocal(), HIS.Desktop.Utility.StringUtil.CustomerCode));
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private static void GlobalUnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                Exception ex = default(Exception);
                ex = (Exception)e.ExceptionObject;
                LogSystem.Error(ex.Message + "\n" + ex.StackTrace);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private static void GlobalThreadExceptionHandler(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            try
            {
                Exception ex = default(Exception);
                ex = e.Exception;
                LogSystem.Error(ex.Message + "\n" + ex.StackTrace);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        static void RunAutoUpdateAup()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("RunAutoUpdateAup.1");
                string aupVersion = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HisConfigKeys.CONFIG_KEY__RUN_AUP_VERSION);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => aupVersion), aupVersion));
                if (aupVersion == "v2" || AupVersion == "v2")
                {
                    //TODO
                    Inventec.Aup.Client.VersionV2_0 versionV2 = new Inventec.Aup.Client.VersionV2_0();
                    versionV2.Update(AupConstant.BASE_URI, "Upload/" + processToEnd + "/AUPAutoupdateService.xml", "", System.IO.Path.Combine(System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, "Integrate"), "Aup"));
                }
                else
                {
                    HIS.Desktop.Modules.WaitingForm.frmUgradeAUS frmUgrade = new HIS.Desktop.Modules.WaitingForm.frmUgradeAUS();
                    frmUgrade.ShowDialog();
                }
                Inventec.Common.Logging.LogSystem.Info("RunAutoUpdateAup.2");
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        static void RunAutoUpdate()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("RunAutoUpdate.1");
                string aupVersion = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HisConfigKeys.CONFIG_KEY__RUN_AUP_VERSION);
                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => aupVersion), aupVersion));
                if (aupVersion == "v2" || AupVersion == "v2")
                {
                    ProcessUpdateWithAUPv2();
                }
                else
                {
                    HIS.Desktop.Modules.WaitingForm.frmUgrade frmUgrade = new HIS.Desktop.Modules.WaitingForm.frmUgrade();
                    frmUgrade.ShowDialog();
                }
                Inventec.Common.Logging.LogSystem.Info("RunAutoUpdate.2");
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        static void ProcessUpdateWithAUPv2()
        {
            try
            {
                string cmdLnUpdate = "";

                cmdLnUpdate += "|exePath|" + (HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory);
                cmdLnUpdate += "|cfgServerConfigUrl|" + "Upload/" + processToEnd + "/AutoupdateService.xml";
                cmdLnUpdate += "|cfgAupUri|" + AupConstant.BASE_URI;
                cmdLnUpdate += "|preThreadName|" + processToEnd;
                cmdLnUpdate += "|command|" + command;

                Inventec.Common.Logging.LogSystem.Info("Call exe execute updater for app: " + updaterV2 + "____cmdLnUpdate = " + cmdLnUpdate);
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = updaterV2;
                startInfo.Arguments = "\"" + cmdLnUpdate + "\"";
                Process.Start(startInfo);

                //Inventec.Common.Logging.LogSystem.Info("Application.Exit");
                //Application.Exit();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private static void UnpackCommandline()
        {
            try
            {
                string cmdLn = "";
                string fileZipName = "";
                string postProcessCommand = "";

                foreach (string arg in Environment.GetCommandLineArgs())
                {
                    cmdLn += arg;
                }
                if (cmdLn.IndexOf('|') == -1)
                {
                    return;
                }

                string[] tmpCmd = cmdLn.Split('|');

                for (int i = 1; i < tmpCmd.GetLength(0); i++)
                {
                    if (tmpCmd[i] == "fileZipName") fileZipName = tmpCmd[i + 1];
                    if (tmpCmd[i] == "command") postProcessCommand = tmpCmd[i + 1].Replace("/", "");
                    i++;
                }

                if (!String.IsNullOrEmpty(postProcessCommand))
                {
                    updated = (postProcessCommand == command);
                    updatedForAUS = (postProcessCommand == commandAUS);

                    FileDataResult fileDataResult = new Inventec.Aup.Utility.FileDataResult();
                    fileDataResult.AppCode = GlobalVariables.APPLICATION_CODE;
                    fileDataResult.ZipFileUpdate = fileZipName;
                    Inventec.Aup.Client.VersionV2 version = new Inventec.Aup.Client.VersionV2();
                    version.CleanZipFile(fileDataResult);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private static void OnPluginProgress(object sender, PluginLoadedEventArgs e)
        {
            Platform.CheckForNullReference(e, "e");
            //if (e != null && !String.IsNullOrEmpty(e.Message))
            //{
            //    Inventec.Common.Logging.LogSystem.Warn(e.Message);
            //}
#if !MONO
            //SplashScreenManager.SetStatus(e.Message);

            if (e.PluginAssembly != null)
            {
                //SplashScreenManager.AddAssemblyIcon(e.PluginAssembly);
            }
#endif
        }
    }
}

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
using ACS.SDO;
using DevExpress.XtraBars.Ribbon;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.Global.ADO;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Modules.Main
{
    public partial class frmMain : RibbonForm
    {
        int timerConnectServerCFG = 0;
        private void RunCheckConnectServer()
        {
            try
            {
                string checktimeConnectServerCFG = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HisConfigKeys.CONFIG_KEY__TIMER_AUTO_CHECK_CONNECT_SERVER);

                if (!String.IsNullOrEmpty(checktimeConnectServerCFG))
                {
                    this.timerConnectServerCFG = Convert.ToInt32(checktimeConnectServerCFG);
                }

                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => timerConnectServerCFG), timerConnectServerCFG) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => checktimeConnectServerCFG), checktimeConnectServerCFG));
                if (timerConnectServerCFG > 0)
                {
                    LoadDataConnectServer();

                    System.Windows.Forms.Timer timerConnectServer = new System.Windows.Forms.Timer();
                    timerConnectServer.Interval = (this.timerConnectServerCFG * 1000 * 60);
                    timerConnectServer.Enabled = true;
                    timerConnectServer.Tick += TimerConnectServer_Tick;
                    timerConnectServer.Start();

                    foreach (var item in GlobalVariables.ListServerInfoADO)
                    {
                        PingAsync(item);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void TimerConnectServer_Tick(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("TimerConnectServer_Tick.Begin");
                foreach (var item in GlobalVariables.ListServerInfoADO)
                {
                    PingAsync(item);
                }
                Inventec.Common.Logging.LogSystem.Debug("TimerConnectServer_Tick.End");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataConnectServer()
        {
            try
            {
                if (GlobalVariables.ListServerInfoADO == null)
                    GlobalVariables.ListServerInfoADO = new List<LocalStorage.Global.ADO.ServerInfoADO>();

                string filePath = new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath;
                
                string url = "";
                string[] str = filePath.Split('\\');

                for (int i = 0; i < str.Count(); i++)
                {
                    if (i < str.Count() - 1)
                    {
                        if (i != (str.Count() - 2))
                        {
                            url += str[i] + "\\";
                        }
                        else
                            url += str[i];
                    }
                }
                
                string pathXmlFile = System.IO.Path.Combine(url, @"ConfigSystem.xml");
                var ApplicationXml = new Inventec.Common.XmlConfig.XmlApplicationConfig(pathXmlFile, "vi");
                var xml = ApplicationXml.GetElements();

                if (xml != null && xml.Count > 0)
                {
                    foreach (var item in xml)
                    {
                        if (item.KeyCode == "KEY__MOS_BASE_URI" ||
                            item.KeyCode == "KEY__SDA_BASE_URI" ||
                            item.KeyCode == "KEY__SAR_BASE_URI" ||
                            item.KeyCode == "KEY__MRS_BASE_URI" ||
                            item.KeyCode == "KEY__ACS_BASE_URI" ||
                            item.KeyCode == "KEY__FSS_BASE_URI" ||
                            item.KeyCode == "KEY__LIS_BASE_URI" ||
                            item.KeyCode == "KEY__SCN_BASE_URI")
                        {
                            var ado = new ServerInfoADO();
                            string serverAddress = (item.Value ?? "").ToString();
                            if (!String.IsNullOrEmpty(serverAddress))
                            {
                                if (serverAddress.Contains(":"))
                                {
                                    var arrSCF = serverAddress.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                                    if (arrSCF != null && arrSCF.Count() >= 3)
                                    {
                                        ado.ServerAddress = serverAddress.Substring(0, serverAddress.LastIndexOf(':'));
                                    }
                                    else
                                    {
                                        ado.ServerAddress = serverAddress;
                                    }
                                }
                                else
                                    ado.ServerAddress = serverAddress;

                                if (!GlobalVariables.ListServerInfoADO.Exists(k => k.ServerAddress == ado.ServerAddress))
                                {
                                    GlobalVariables.ListServerInfoADO.Add(ado);
                                }
                            }
                        }
                    }
                }

                ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
                fileMap.ExeConfigFilename = @"HIS.exe.config";

                Configuration configSave = ConfigurationManager.OpenMappedExeConfiguration(fileMap,
                ConfigurationUserLevel.None);

                if (configSave.AppSettings != null && configSave.AppSettings.Settings.Count > 0)
                {
                    foreach (string key in configSave.AppSettings.Settings.AllKeys)
                    {
                        if (key == "Inventec.Token.ClientSystem.Acs.Base.Uri" ||
                            key == "fss.uri.base" ||
                            key == "Aup.uri.base" ||
                            key == "His.EventLog.Sda"
                           )
                        {
                            var ado = new ServerInfoADO();
                            string serverAddress = (configSave.AppSettings.Settings[key].Value ?? "").ToString();
                            if (!String.IsNullOrEmpty(serverAddress))
                            {
                                if (serverAddress.Contains(":"))
                                {
                                    var arrSCF = serverAddress.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                                    if (arrSCF != null && arrSCF.Count() >= 3)
                                    {
                                        ado.ServerAddress = serverAddress.Substring(0, serverAddress.LastIndexOf(':'));
                                    }
                                    else
                                    {
                                        ado.ServerAddress = serverAddress;
                                    }
                                }
                                else
                                    ado.ServerAddress = serverAddress;

                                if (!GlobalVariables.ListServerInfoADO.Exists(k => k.ServerAddress == ado.ServerAddress))
                                {
                                    GlobalVariables.ListServerInfoADO.Add(ado);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task<bool> PingAsync(ServerInfoADO serverInfoADO)
        {
            bool success = false;
            try
            {               
                string hostUrl = serverInfoADO != null ? serverInfoADO.ServerAddress : "";
                if (!String.IsNullOrEmpty(hostUrl))
                {
                    var fullUrl = new Uri(hostUrl);
                    var host = fullUrl.Host;

                    Ping ping = new Ping();

                    PingReply result = await ping.SendPingAsync(host);

                    serverInfoADO.LastPingTime = Inventec.Common.DateTime.Get.Now();
                    if (result != null)
                    {
                        serverInfoADO.IPStatus = result.Status;
                        if (result.Status != IPStatus.Success)
                        {
                            serverInfoADO.Description = Inventec.Common.Resource.Get.Value("KetNoiServerKhongOnDinh", Resources.ResourceLanguageManager.LanguageFrmMain, LanguageManager.GetCulture()); //"Kết nối server không ổn định";
                            UpdateStatusServerConnect(serverInfoADO);
                        }
                        else
                        {
                            serverInfoADO.Description = Inventec.Common.Resource.Get.Value("KetNoiServerOnDinh", Resources.ResourceLanguageManager.LanguageFrmMain, LanguageManager.GetCulture());// "Kết nối server ổn định";
                        }
                    }
                    else
                    {
                        serverInfoADO.IPStatus = null;
                        serverInfoADO.Description = Inventec.Common.Resource.Get.Value("MatKetNoiServer", Resources.ResourceLanguageManager.LanguageFrmMain, LanguageManager.GetCulture());//"Mất kết nối đến server...";
                        UpdateStatusServerConnect(serverInfoADO);
                    }

                    success = (result.Status == IPStatus.Success);

                    if (!GlobalVariables.ListServerInfoADO.Exists(k => k.LastPingTime != null && !String.IsNullOrEmpty(k.ServerAddress) && k.IPStatus != IPStatus.Success))
                    {
                        UpdateStatusServerConnect(new ServerInfoADO() { Description = Inventec.Common.Resource.Get.Value("KetNoiServerOnDinh", Resources.ResourceLanguageManager.LanguageFrmMain, LanguageManager.GetCulture()), IPStatus = IPStatus.Success });
                    }
                }
            }
            catch (Exception ex)
            {
                try
                {
                    serverInfoADO.Description = Inventec.Common.Resource.Get.Value("MatKetNoiServer", Resources.ResourceLanguageManager.LanguageFrmMain, LanguageManager.GetCulture());
                    serverInfoADO.IPStatus = IPStatus.Unknown;
                    UpdateStatusServerConnect(serverInfoADO);
                }
                catch { }

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return success;
        }

        void UpdateStatusServerConnect(ServerInfoADO serverInfoADO)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate
                        {
                            UpdateServerConnectStatusInForm(serverInfoADO);
                        }));
                }
                else
                {
                    UpdateServerConnectStatusInForm(serverInfoADO);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateServerConnectStatusInForm(ServerInfoADO serverInfoADO)
        {
            try
            {
                bsiServerConnectStatus.Caption = serverInfoADO != null ? serverInfoADO.Description : "";
                if (serverInfoADO == null || serverInfoADO.IPStatus == null || serverInfoADO.IPStatus != IPStatus.Success)
                {
                    bsiServerConnectStatus.ItemAppearance.Normal.ForeColor = System.Drawing.Color.Red;
                    bsiServerConnectStatus.ItemAppearance.Normal.Font = new Font(bsiServerConnectStatus.ItemAppearance.Normal.Font, FontStyle.Bold);
                }
                else
                {
                    bsiServerConnectStatus.ItemAppearance.Normal.ForeColor = System.Drawing.Color.Black;
                    bsiServerConnectStatus.ItemAppearance.Normal.Font = new Font(bsiServerConnectStatus.ItemAppearance.Normal.Font, FontStyle.Regular);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
           
        }
    }
}

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
using HIS.Desktop.LocalStorage.Location;
using Inventec.Aup.Utility;
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Modules.WaitingForm
{
    public partial class frmUpgradeOption : Form
    {
        string updater = Application.StartupPath + @"\\Integrate\\Aup\\Inventec.AUS.exe";
        string updaterV2 = Application.StartupPath + @"\\Integrate\\Aup\\Inventec.AutoUpdater.exe";
        string processToEnd = (ConfigurationManager.AppSettings["Inventec.Desktop.Execute"] ?? "").ToString();
        string postProcess = Application.StartupPath + @"\" + (ConfigurationManager.AppSettings["Inventec.Desktop.Execute"] ?? "").ToString() + ".exe";
        const string command = "updated";
        const string commandAUS = "updatedAUS";
        string AupVersion = (ConfigurationManager.AppSettings["HIS.Desktop.AupVersion"] ?? "");

        public frmUpgradeOption()
        {
            InitializeComponent();
        }

        private void btnUpgradeHand_Click(object sender, EventArgs e)
        {
            try
            {
                if (File.Exists(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, "readmebk.txt")))
                {
                    File.Delete(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, "readmebk.txt"));
                }
                if (File.Exists(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, "versionlog.txt")))
                {
                    System.IO.File.WriteAllText(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, "versionlog.txt"), "");
                }

                if (File.Exists(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, "AutoUpdater.config")))
                {
                    File.Delete(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, "AutoUpdater.config"));
                }

                if (!String.IsNullOrEmpty(Inventec.Aup.Utility.AupConstant.BASE_URI))
                    RunAutoUpdate();
                else
                {
                    MessageBox.Show("Không có cấu hình địa chỉ backend AUP, không thể thực hiện kiểm tra phiên bản mới");
                    LogSystem.Info("Dang chay che do unit test hoac dia chi uri cua backend AUP khong duoc cau hinh____khong the kiem tra duoc phien ban moi____" + Inventec.Aup.Utility.AupConstant.BASE_URI);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void RunAutoUpdate()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("Người dùng click vào nâng cấp thủ công, hệ thống bắt đầu kiểm tra phiên bản mới");
                Inventec.Common.Logging.LogSystem.Info("RunAutoUpdate.1");
                string aupVersion = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HisConfigKeys.CONFIG_KEY__RUN_AUP_VERSION);
                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => aupVersion), aupVersion));
                if ((!String.IsNullOrEmpty(aupVersion) && aupVersion == "v2") || (AupVersion == "v2"))
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

        void ProcessUpdateWithAUPv2()
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

        private void frmUpgradeOption_Load(object sender, EventArgs e)
        {
            try
            {
                string readmeContent = "", readmeBKContent = "";
                if (File.Exists(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, "readme.txt")))
                {
                    readmeContent = System.IO.File.ReadAllText(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, "readme.txt"));
                }
                if (File.Exists(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, "readmebk.txt")))
                {
                    readmeBKContent = System.IO.File.ReadAllText(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, "readmebk.txt"));
                }

                lblStatus.Text = String.Format(lblStatus.Text, readmeContent, readmeBKContent);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

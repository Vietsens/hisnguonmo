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
using Inventec.Aup.Client;
using Inventec.Aup.Utility;
using Inventec.Common.Logging;
using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;
using Inventec.Desktop.Common.LocalStorage.Location;
using System.Configuration;

namespace Inventec.Desktop.Plugins.Updater
{
    public partial class frmVersionUpdate : DevExpress.XtraEditors.XtraForm
    {
        static string updater = Application.StartupPath + @"\Inventec.AUS.exe";
        static string processToEnd = ConfigurationManager.AppSettings["Inventec.Desktop.Execute"];
        static string postProcess = Application.StartupPath + @"\" + processToEnd + ".exe";
        const string command = "updated";
        static FileDataResult fileDataResult;

        public frmVersionUpdate()
        {
            InitializeComponent();
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void bbtnUpdate_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                simpleButton1_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                if (InstallUpdateRestart())
                {
                    string cmdLn = "";

                    cmdLn += "|downloadFile|" + fileDataResult.FileZipName;
                    cmdLn += "|URL|" + AupConstant.BASE_URI;
                    cmdLn += "|destinationFolder|" + "" + Application.StartupPath + "\\";
                    cmdLn += "|processToEnd|" + processToEnd;
                    cmdLn += "|postProcess|" + postProcess;
                    cmdLn += "|command|" + command;

                    Inventec.Common.Logging.LogSystem.Debug("cmdLn = " + cmdLn);

                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.FileName = updater;
                    startInfo.Arguments = cmdLn;
                    Process.Start(startInfo);

                    Application.Exit();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void frmVersionUpdate_Load(object sender, EventArgs e)
        {
            try
            {
                CheckVersion();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        static bool CheckUpdateInfo()
        {
            bool result = false;
            try
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(frmUpdatingForm));
                fileDataResult = Inventec.Aup.Client.Version.CheckForUpdate(VersionUpdateConfig.APPLICATION_CODE, VersionUpdateConfig.ApplicationDirectory);
                if (fileDataResult != null
                    && fileDataResult.IsUpdate)
                {
                    result = true;
                }
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
            }
            catch (FolderContainerException ex)
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
            }
            catch (Exception ex)
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                LogSystem.Error(ex);
            }
            return result;
        }

        static bool InstallUpdateRestart()
        {
            bool result = false;
            try
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                DevExpress.XtraSplashScreen.SplashScreenManager.ShowForm(typeof(frmUpdatingForm));
                fileDataResult = Inventec.Aup.Client.Version.Update(VersionUpdateConfig.APPLICATION_CODE, VersionUpdateConfig.ApplicationDirectory);
                if (fileDataResult != null
                    && fileDataResult.IsUpdate
                    && !String.IsNullOrEmpty(fileDataResult.FileZipName))
                {
                    result = true;
                }
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
            }
            catch (FolderContainerException ex)
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
            }
            catch (Exception ex)
            {
                DevExpress.XtraSplashScreen.SplashScreenManager.CloseForm(false);
                LogSystem.Error(ex);
            }
            return result;
        }

        void CheckVersion()
        {
            try
            {
                if (CheckUpdateInfo())
                {
                    lblState.Text = "Tìm thấy bản nâng cấp cho phần mềm, chọn Nâng cấp để tiến hành cài đặt. Lưu ý: phần mềm sẽ bị khởi động lại.    An update package is available, choose Update to setup it. Note: software will be restarted.";
                    btnUpdate.Enabled = true;
                }
                else
                {
                    lblState.Text = "Không tìm thấy bản cập nhật nào.    No update is available.";
                    btnUpdate.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnCheck_Click(object sender, EventArgs e)
        {
            try
            {
                CheckVersion();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}

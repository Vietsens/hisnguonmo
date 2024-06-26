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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Inventec.Aup.Utility;
using System.Configuration;
using Inventec.Common.Logging;
using Inventec.Desktop.Common.Message;
using System.Diagnostics;
using Inventec.Aup.Client;
using HIS.Desktop.Resources;
using System.IO;

namespace HIS.Desktop.Modules.WaitingForm
{
    public partial class frmUgradeAUS : DevExpress.XtraEditors.XtraForm
    {
        static string updater = Application.StartupPath + (ConfigurationManager.AppSettings["Integrate.UpdateOfAup.Inventec.AUS"] ?? @"\Integrate\UpdateAup\Inventec.AUS.exe").ToString();
        static string processToEnd = (ConfigurationManager.AppSettings["Inventec.Desktop.Execute"] ?? "").ToString();
        static string postProcess = HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory + @"\" + processToEnd + ".exe";
        const string command = "updatedAUS";
        const string spacialSeperator = "##%%%%##";
        const string spacialSeperator2 = "##%%%%####%%%%##";
        const string spacialSeperator3 = "##%%%%####%%%%####%%%%##";
        static FileDataResult fileDataResult;

        public frmUgradeAUS()
        {
            InitializeComponent();
        }

        private void frmUgrade_Load(object sender, EventArgs e)
        {
            try
            {
                lblDescription.Text = ResourceCommon.DangKiemTraPhienBanCapNhat;

                timer1.Enabled = true;
                timer1.Start();
            }
            catch (Exception ex)
            {
                LogSystem.Info("Application_Start error. Message=" + ex.Message + ". Time=" + DateTime.Now.ToString("yyyyMMddhhmmss"));
            }
        }

        bool CheckUpdateInfo()
        {
            bool result = false;
            try
            {
                WaitingManager.Show();
                string appFolderConstain = "";
                if (File.Exists(System.IO.Path.GetFullPath(System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, "Integrate\\Aup\\versionAUPlog.txt"))))
                {
                    appFolderConstain = System.IO.Path.GetFullPath(System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, "Integrate\\Aup"));
                }
                else
                {
                    appFolderConstain = System.IO.Path.GetFullPath(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory);
                }

                Inventec.Aup.Client.VersionV2 version = new Inventec.Aup.Client.VersionV2();
                fileDataResult = version.UpdateSpecify("readme.txt", "versionAUPlog.txt", HIS.Desktop.LocalStorage.LocalData.GlobalVariables.APPLICATION_CODE, appFolderConstain);
                Inventec.Common.Logging.LogSystem.Debug("** frmUgrade AUS CheckUpdateInfo ** fileDataResult: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => fileDataResult), fileDataResult));
                if (fileDataResult == null || !fileDataResult.IsUpdate)
                {
                    Inventec.Common.Logging.LogSystem.Debug("Upgrade run not found new version!" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => fileDataResult), fileDataResult));
                    WaitingManager.Hide();
                    this.Close();
                }
                else if (fileDataResult.IsUpdate && (HasDeleteFiles(fileDataResult.FileDeletes) || !String.IsNullOrEmpty(fileDataResult.FileZipName) || !String.IsNullOrEmpty(fileDataResult.ZipFileUpdate)))
                {
                    result = true;
                    string cmdLnUpdate = "";
                    if (!String.IsNullOrEmpty(fileDataResult.ZipFileUpdate))
                    {
                        cmdLnUpdate += "|downloadFile|" + fileDataResult.ZipFileUpdate;                       
                    }
                    cmdLnUpdate += "|URL|" + AupConstant.BASE_URI;
                    cmdLnUpdate += ("|destinationFolder|" + "" + ProcessSpacePath(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory) + "\\");
                    cmdLnUpdate += "|processToEnd|" + processToEnd;
                    cmdLnUpdate += "|postProcess|" + ProcessSpacePath(postProcess);
                    cmdLnUpdate += "|command|" + command;
                    cmdLnUpdate += "|version|" + fileDataResult.VersionServer;
                    cmdLnUpdate += "|spacialPath|" + appFolderConstain;

                    // process delete files
                    if (fileDataResult.FileDeletes != null && fileDataResult.FileDeletes.Count > 0)
                    {
                        cmdLnUpdate += "|deleteFiles|";
                        foreach (var item in fileDataResult.FileDeletes)
                        {
                            cmdLnUpdate += item + ";";
                        }
                    }

                    Inventec.Common.Logging.LogSystem.Info("Call exe execute updater for AUS: " + updater + "____cmdLnUpdate = " + cmdLnUpdate);

                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.FileName = updater;
                    startInfo.Arguments = cmdLnUpdate;//"\"" +  +"\"";
                    Process.Start(startInfo);
                    WaitingManager.Hide();

                    Inventec.Common.Logging.LogSystem.Info("Application.Exit");
                    try
                    {
                        CloseAllApp.CloseAllApps();
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error(ex);
                    }
                    Application.Exit();
                }
                else
                {
                    WaitingManager.Hide();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                result = false;
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
            return result;
        }

        private bool HasDeleteFiles(List<string> deleteFiles)
        {
            bool result = false;
            try
            {
                if (deleteFiles != null && deleteFiles.Count > 0)
                {
                    foreach (var pathFile in deleteFiles)
                    {
                        try
                        {
                            string fullFileName = Path.Combine(Application.StartupPath, pathFile.Trim('\\'));
                            Inventec.Common.Logging.LogSystem.Info("HasDeleteFiles:path=" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => fullFileName), fullFileName));
                            if (File.Exists(fullFileName))
                            {
                                return true;
                            }
                        }
                        catch (Exception exx)
                        {
                            Inventec.Common.Logging.LogSystem.Warn(exx.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex.Message);
            }
            return result;
        }

        string ProcessSpacePath(string path)
        {
            return path.Replace("   ", spacialSeperator3).Replace("  ", spacialSeperator2).Replace(" ", spacialSeperator);
        }

        void RunAutoUpdate()
        {
            try
            {
                CheckUpdateInfo();
                this.Close();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                timer1.Enabled = false;
                timer1.Stop();
                RunAutoUpdate();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
    }
}

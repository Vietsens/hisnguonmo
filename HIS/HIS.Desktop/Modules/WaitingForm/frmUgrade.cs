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
    public partial class frmUgrade : DevExpress.XtraEditors.XtraForm
    {
        static string updater = Application.StartupPath + (ConfigurationManager.AppSettings["Integrate.Aup.Inventec.AUS"] ?? @"\Integrate\Aup\Inventec.AUS.exe").ToString();
        static string processToEnd = (ConfigurationManager.AppSettings["Inventec.Desktop.Execute"] ?? "").ToString();
        static string postProcess = HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory + @"\" + processToEnd + ".exe";
        const string command = "updated";
        const string spacialSeperator = "##%%%%##";
        const string spacialSeperator2 = "##%%%%####%%%%##";
        const string spacialSeperator3 = "##%%%%####%%%%####%%%%##";
        static FileDataResult fileDataResult;

        public frmUgrade()
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

        string ProcessSpacePath(string path)
        {
            return path.Replace("   ", spacialSeperator3).Replace("  ", spacialSeperator2).Replace(" ", spacialSeperator);
        }

        bool CheckUpdateInfo()
        {
            bool result = false;
            try
            {
                WaitingManager.Show();
                Inventec.Aup.Client.VersionV2 version = new Inventec.Aup.Client.VersionV2();
                fileDataResult = version.Update("readme.txt", "versionlog.txt", HIS.Desktop.LocalStorage.LocalData.GlobalVariables.APPLICATION_CODE, HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory);
                Inventec.Common.Logging.LogSystem.Debug("**frmUgrade CheckUpdateInfo ** fileDataResult: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => fileDataResult), fileDataResult));
                if (fileDataResult == null
     || !fileDataResult.IsUpdate)
                {
                    Inventec.Common.Logging.LogSystem.Debug("Upgrade HIS run not found new version!" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => fileDataResult), fileDataResult));
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
                    cmdLnUpdate += "|UseCompressZip|1";
                    // process delete files
                    if (fileDataResult.FileDeletes != null && fileDataResult.FileDeletes.Count > 0)
                    {
                        cmdLnUpdate += "|deleteFiles|";
                        string deleteFileCompres = "";
                        foreach (var item in fileDataResult.FileDeletes)
                        {
                            deleteFileCompres += item + ";";
                        }
                        cmdLnUpdate += Inventec.Common.String.StringCompressor.CompressString(deleteFileCompres);
                    }

                    Inventec.Common.Logging.LogSystem.Info("Call exe execute updater for app: " + updater + "____cmdLnUpdate = " + cmdLnUpdate);
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.FileName = updater;
                    startInfo.Arguments = "\"" + cmdLnUpdate + "\"";
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

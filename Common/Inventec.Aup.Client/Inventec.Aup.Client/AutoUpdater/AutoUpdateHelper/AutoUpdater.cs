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
/*****************************************************************
 * Copyright (C) Knights Warrior Corporation. All rights reserved.
 * 
 * Author:   圣殿骑士（Knights Warrior） 
 * Email:    KnightsWarrior@msn.com
 * Website:  http://www.cnblogs.com/KnightsWarrior/       http://knightswarrior.blog.51cto.com/
 * Create Date:  5/8/2010 
 * Usage:
 *
 * RevisionHistory
 * Date         Author               Description
 * 
*****************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;

namespace Inventec.Aup.Client.AutoUpdater
{
    #region The delegate
    public delegate void ShowHandler();
    #endregion

    public class AutoUpdater : IAutoUpdater
    {
        #region The private fields
        public Config config = null;
        private bool bNeedRestart = false;
        private bool bDownload = false;
        List<DownloadFileInfo> downloadFileListTemp = null;
        private string postProcess;
        private string preThreadName;
        private string cfgAupUri;
        Dictionary<string, RemoteFile> listRemotFile;
        List<DownloadFileInfo> downloadList;
        string exePath = "", preCmd = "";
        #endregion

        #region The public event
        public event ShowHandler OnShow;
        #endregion

        #region The constructor of AutoUpdater
        public AutoUpdater(string cfgAupUri, string cfgServerConfigUrl = "", string preThreadName = "", string exePath = "", string preCmd = "")
        {
            this.exePath = exePath;
            this.preCmd = preCmd;
            this.cfgAupUri = cfgAupUri;
            if (!String.IsNullOrEmpty(this.cfgAupUri) && !this.cfgAupUri.EndsWith("/"))
            {
                this.cfgAupUri = this.cfgAupUri + "/";
            }
            this.preThreadName = preThreadName;
            this.postProcess = !String.IsNullOrEmpty(this.preThreadName) ? System.IO.Path.Combine(this.exePath, String.Format("{0}.exe", this.preThreadName)) : "";
            if (!String.IsNullOrEmpty(exePath))
                CommonUnitity.SystemBinUrl = exePath;
            config = Config.LoadConfig(Path.Combine((!String.IsNullOrEmpty(this.exePath) ? this.exePath : AppDomain.CurrentDomain.BaseDirectory), (!String.IsNullOrEmpty(this.preThreadName) ? ConstFile.FILENAME : ConstFile.AUP_FILENAME)));
            if (!String.IsNullOrEmpty(cfgServerConfigUrl) && !String.IsNullOrEmpty(cfgAupUri))
                config.ServerUrl = String.Format("{0}{1}", cfgAupUri, cfgServerConfigUrl);

            Inventec.Common.Logging.LogSystem.Info("cfgServerUrl=" + cfgServerConfigUrl + ", cfgAupUri=" + cfgAupUri + ", exePath=" + exePath + ", postProcess=" + postProcess + ", preThreadName=" + preThreadName);
        }
        #endregion

        #region The public method
        private bool CheckNewVersion()
        {
            Inventec.Common.Logging.LogSystem.Debug("CheckNewVersion.1");
            bool result = false;
            if (!config.Enabled)
                return result;

            Inventec.Common.Logging.LogSystem.Debug("CheckNewVersion.2");

            this.listRemotFile = ParseRemoteXml(config.ServerUrl);
            this.downloadList = new List<DownloadFileInfo>();

            if (config.UpdateFileList == null || config.UpdateFileList.Count == 0)
            {
                //trường hợp client chạy lần đầu tiên chưa có file chứa thông tin phiên bản của toàn bộ phần mềm AutoUpdater.config
                syncFileClientAndServer();
            }

            foreach (LocalFile file in config.UpdateFileList)
            {
                if (listRemotFile.ContainsKey(file.Path))
                {
                    RemoteFile rf = listRemotFile[file.Path];
                    //Version v1 = new Version(rf.LastVer);
                    //Version v2 = new Version(file.LastVer);
                    //if (v1 > v2)
                    string v1 = rf.Verison;
                    string v2 = file.Version;
                    if (v1 != v2)
                    {
                        downloadList.Add(new DownloadFileInfo(rf.Url, rf.Path, rf.LastVer, rf.Size, rf.Verison));
                        file.Path = rf.Path;
                        file.LastVer = rf.LastVer;
                        file.Size = rf.Size;
                        file.Version = rf.Verison;
                        if (rf.NeedRestart)
                            bNeedRestart = true;

                        bDownload = true;
                    }

                    listRemotFile.Remove(file.Path);
                }
            }
            result = ((listRemotFile != null && listRemotFile.Count > 0) || downloadList.Count > 0);

            return result;
        }

        private void syncFileClientAndServer()
        {
            config.UpdateFileList = new UpdateFileList();
            if (!String.IsNullOrEmpty(this.exePath))
            {
                DirectoryInfo dicInfo = new DirectoryInfo(this.exePath);
                PopuAllDirectory(dicInfo);
            }
        }

        private void PopuAllDirectory(DirectoryInfo dicInfo)
        {
            foreach (FileInfo f in dicInfo.GetFiles())
            {
                //排除当前目录中生成xml文件的工具文件
                if (
                    //f.Name != "CreateXmlTools.exe" 
                    //&& f.Name != "AutoupdateService.xml" 
                    //&& !f.Name.Contains("CreateXmlTools")
                    //&& f.Name != "web.config"
                    !f.FullName.Contains("\\Logs\\")
                    && !f.FullName.Contains("\\.svn\\")
                    && !f.FullName.Contains("\\~$")
                    //&& !f.Name.EndsWith(".config")
                    )
                {
                    string path = f.FullName.Replace(this.exePath, "").Replace("\\", "/");
                    string filePath = string.Empty;
                    if (path != string.Empty)
                    {
                        filePath = path.TrimStart('/').TrimEnd('/');
                    }
                    //var fullFilePath = folderPath + f.Name;

                    string extension = Path.GetExtension(f.FullName);
                    if (!String.IsNullOrEmpty(extension) && extension == ".config")
                    {
                        filePath = filePath.Replace(".config", ".config_");
                    }

                    var curChildElemCFG = (listRemotFile.ContainsKey(filePath) ? listRemotFile[filePath] : null);
                    if (curChildElemCFG != null)
                    {
                        var size = curChildElemCFG.Size;
                        var hashCode = curChildElemCFG.Hash;
                        var curFileHash = Config.GetFileHash(f.FullName);
                        LocalFile localFile = new LocalFile()
                        {
                            LastVer = FileVersionInfo.GetVersionInfo(f.FullName).FileVersion,
                            Path = filePath,
                            Size = size,
                            Version = curChildElemCFG.Verison//Guid.NewGuid().ToString()
                        };
                        if (f.Length.ToString() != size.ToString() || curFileHash != hashCode)
                        {
                            localFile.Version = Guid.NewGuid().ToString();
                        }
                        config.UpdateFileList.Add(localFile);
                    }
                }
            }

            foreach (DirectoryInfo di in dicInfo.GetDirectories())
                PopuAllDirectory(di);
        }

        public void Update()
        {
            if (!CheckNewVersion())
            {
                if (!File.Exists(Path.Combine((!String.IsNullOrEmpty(this.exePath) ? this.exePath : AppDomain.CurrentDomain.BaseDirectory), (!String.IsNullOrEmpty(this.preThreadName) ? ConstFile.FILENAME : ConstFile.AUP_FILENAME))))
                {
                    config.SaveConfig(Path.Combine(this.exePath, (!String.IsNullOrEmpty(this.preThreadName) ? ConstFile.FILENAME : ConstFile.AUP_FILENAME)));
                    Inventec.Common.Logging.LogSystem.Debug("File: " + Path.Combine((!String.IsNullOrEmpty(this.exePath) ? this.exePath : AppDomain.CurrentDomain.BaseDirectory), (!String.IsNullOrEmpty(this.preThreadName) ? ConstFile.FILENAME : ConstFile.AUP_FILENAME)) +" not exists==> auto create file config");
                }
                Inventec.Common.Logging.LogSystem.Info("Kiem tra phien ban moi ==> khong tim thay co phien ban moi==>");
                Application.Exit();
                return;
            }
            if (!String.IsNullOrEmpty(preThreadName))
            {
                ProcessKillPreApp();
            }

            foreach (RemoteFile file in listRemotFile.Values)
            {
                downloadList.Add(new DownloadFileInfo(file.Url, file.Path, file.LastVer, file.Size, file.Verison));
                bDownload = true;
                config.UpdateFileList.Add(new LocalFile(file.Path, file.LastVer, file.Size, file.Verison));
                if (file.NeedRestart)
                    bNeedRestart = true;
            }

            downloadFileListTemp = downloadList;
            Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => bDownload), bDownload)
                + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => downloadList), downloadList));
            if (bDownload)
            {
                OperProcess op = new OperProcess();
                op.InitUpdateEnvironment();
                DownloadConfirm dc = new DownloadConfirm(downloadList);

                if (this.OnShow != null)
                    this.OnShow();
                StartDownload(downloadList);
            }
        }

        public void RollBack()
        {
            foreach (DownloadFileInfo file in downloadFileListTemp)
            {
                string tempUrlPath = CommonUnitity.GetFolderUrl(file);
                string oldPath = string.Empty;
                try
                {
                    if (!string.IsNullOrEmpty(tempUrlPath))
                    {
                        oldPath = Path.Combine(CommonUnitity.SystemBinUrl + tempUrlPath.Substring(1), file.FileName);
                    }
                    else
                    {
                        oldPath = Path.Combine(CommonUnitity.SystemBinUrl, file.FileName);
                    }

                    if (oldPath.EndsWith("_"))
                        oldPath = oldPath.Substring(0, oldPath.Length - 1);

                    MoveFolderToOld(oldPath + ".old", oldPath);

                }
                catch (Exception ex)
                {
                    //log the error message,you can use the application's log code
                }
            }
        }

        void ProcessKillPreApp()
        {
            var processHLS = System.Diagnostics.Process.GetProcesses().Where(o => o.ProcessName.Contains("HLS.WCFClient")).ToList();
            if (processHLS != null && processHLS.Count() > 0)
            {
                for (int i = 0; i < processHLS.Count(); i++)
                {
                    try
                    {
                        processHLS[i].Kill();
                    }
                    catch (Exception ex)
                    {
                        //LogSystem.Debug(ex);
                    }
                }
            }

            string exeNameSignProcessor = "EMR.SignProcessor";
            var processSignService = System.Diagnostics.Process.GetProcesses().Where(o => o.ProcessName == exeNameSignProcessor || o.ProcessName == String.Format("{0}.exe", exeNameSignProcessor) || o.ProcessName == String.Format("{0} (32 bit)", exeNameSignProcessor) || o.ProcessName == String.Format("{0}.exe (32 bit)", exeNameSignProcessor)).ToList();
            if (processSignService != null && processSignService.Count() > 0)
            {
                for (int i = 0; i < processSignService.Count(); i++)
                {
                    try
                    {
                        processSignService[i].Kill();
                    }
                    catch (Exception ex)
                    {
                        //LogSystem.Warn(ex);
                    }
                }
            }

            var processNotify = System.Diagnostics.Process.GetProcesses().Where(o => o.ProcessName.Contains("HIS.Desktop.Notify")).ToList();
            if (processNotify != null && processNotify.Count() > 0)
            {
                for (int i = 0; i < processNotify.Count(); i++)
                {
                    try
                    {
                        processNotify[i].Kill();
                    }
                    catch (Exception ex)
                    {
                        //LogSystem.Debug(ex);
                    }
                }
            }


            var processPreApp = System.Diagnostics.Process.GetProcesses().Where(o => o.ProcessName.Equals(preThreadName) || o.ProcessName.Equals(preThreadName + ".exe") || o.ProcessName.Equals(preThreadName + ".exe(32-bit)") || o.ProcessName.Equals(preThreadName + ".exe (32-bit)")).ToList();
            if (processPreApp != null && processPreApp.Count() > 0)
            {
                for (int i = 0; i < processPreApp.Count(); i++)
                {
                    try
                    {
                        processPreApp[i].Kill();
                    }
                    catch (Exception ex)
                    {
                        //LogSystem.Debug(ex);
                    }
                }
            }
        }

        #endregion

        #region The private method
        string newfilepath = string.Empty;
        private void MoveFolderToOld(string oldPath, string newPath)
        {
            if (File.Exists(oldPath) && File.Exists(newPath))
            {
                System.IO.File.Copy(oldPath, newPath, true);
            }
        }

        private void StartDownload(List<DownloadFileInfo> downloadList)
        {
            bool success = true;
            DownloadProgress dp = new DownloadProgress(downloadList, config, postProcess);
            if (dp.ShowDialog() == DialogResult.OK)
            {
                ////
                //if (DialogResult.Cancel == dp.ShowDialog())
                //{
                //    success = false;
                //    //return;
                //}


                if (!String.IsNullOrEmpty(this.postProcess))
                {
                    PostFinalCommand();
                }

                if (success)
                {
                    //Update successfully
                    config.SaveConfig(Path.Combine(this.exePath, (!String.IsNullOrEmpty(this.preThreadName) ? ConstFile.FILENAME : ConstFile.AUP_FILENAME)));
                }
            }

            Application.Exit();
        }

        private void PostFinalCommand()
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = postProcess;
                startInfo.Arguments = ("|command" + "|" + "updated|" + (!String.IsNullOrEmpty(this.preCmd) ? "preCmd|" + this.preCmd + "|" : ""));
                Process.Start(startInfo);
                //Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => postProcessFile), postProcessFile) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => postProcessCommand), postProcessCommand));
            }
            catch (Exception ex)
            {
                //Inventec.Common.Logging.LogSystem.Warn("PostDownload error: " + ex.Message + ". postProcessFile = " + postProcessFile + ". postProcessCommand = " + postProcessCommand);
            }
        }

        private Dictionary<string, RemoteFile> ParseRemoteXml(string xml)
        {
            var xmlStr = string.Empty;
            try
            {
                WebClient client = new WebClient();
                if (!string.IsNullOrEmpty(config.PassWord) && !string.IsNullOrEmpty(config.UserName))
                {
                    client.Credentials = new NetworkCredential(config.UserName, config.PassWord);
                }
                else
                {
                    client.Credentials = new NetworkCredential();
                }
                xmlStr = client.DownloadString(config.ServerUrl);
            }
            catch (Exception ex)
            {

            }

            XmlDocument document = new XmlDocument();
            //document.Load(xml);
            if (!string.IsNullOrEmpty(xmlStr))
            {
                document.LoadXml(xmlStr);
            }
            else
            {
                document.Load(xml);
            }

            Dictionary<string, RemoteFile> list = new Dictionary<string, RemoteFile>();
            foreach (XmlNode node in document.DocumentElement.ChildNodes)
            {
                if (!node.Attributes["path"].Value.EndsWith(String.Format(".{0}", ConstFile.CONFIGFILE)))
                {
                    list.Add(node.Attributes["path"].Value, new RemoteFile(node, this.cfgAupUri));
                }
            }

            return list;
        }
        #endregion

    }

}

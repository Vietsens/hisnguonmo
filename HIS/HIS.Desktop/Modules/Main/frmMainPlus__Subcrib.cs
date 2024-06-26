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
using DevExpress.XtraTab;
using HIS.Desktop.Base;
using Inventec.Common.Logging;
using Inventec.Desktop.Core.Actions;
using Inventec.Desktop.Common.Modules;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.XtraBars;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Repository;
using System.Threading.Tasks;
using Inventec.Core;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.LocalData;
using System.Linq;
using System.Threading;
using MOS.EFMODEL.DataModels;
using SDA.EFMODEL.DataModels;
using HIS.Desktop.ModuleExt;
using System.Net;
using System.Net.Sockets;
using HIS.Desktop.LocalStorage.ConfigSystem;
using System.Text;
using HIS.Desktop.Library.CacheClient;
using HIS.Desktop.LocalStorage.HisConfig;
using Inventec.Desktop.Common.Message;
using System.IO;
using HIS.Desktop.LocalStorage.Location;

namespace HIS.Desktop.Modules.Main
{
    public partial class frmMain : RibbonForm
    {
        Socket _client;
        EndPoint _remoteEndPoint;
        byte[] _data;
        int _recv;
        Boolean _isReceivingStarted = false;
        const string ChanelHisProName = "HISPROCHANEL";
        const string SubscribeCommand = "Subscribe";
        const string UnSubscribeCommand = "UnSubscribe";
        const string DeleteDataCacheCommand = "TruncateCache";
        const string RestartAppCommand = "RestartApp";
        const string UploadLogCommand = "UploadLog";

        private void InitSubscriber()
        {
            string[] psArr = null;
            try
            {
                string serviceInfo = HisConfigs.Get<String>("HIS.Desktop.PubSubServerInfo");
                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => serviceInfo), serviceInfo));
                if (!String.IsNullOrEmpty(serviceInfo))
                {
                    psArr = serviceInfo.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                    if (psArr != null && psArr.Count() == 3)
                    {
                        IPAddress serverIPAddress = IPAddress.Parse(psArr[0]);
                        int serverPort = Convert.ToInt32(psArr[1]);

                        _client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                        _remoteEndPoint = new IPEndPoint(serverIPAddress, serverPort);

                        string clientIps = Inventec.Common.WebApiClient.IpAddressUtils.GetIpAddressLocal();
                        Inventec.Common.Logging.LogSystem.Info(SubscribeCommand + " chanel " + ChanelHisProName + " success____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => psArr), psArr) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => clientIps), clientIps));

                        string message = SubscribeCommand + "," + ChanelHisProName;
                        _client.SendTo(Encoding.ASCII.GetBytes(message), _remoteEndPoint);

                        if (_isReceivingStarted == false)
                        {
                            _isReceivingStarted = true;
                            _data = new byte[1024];
                            Thread thread1 = new Thread(new ThreadStart(ReceiveDataFromServer));
                            thread1.IsBackground = true;
                            thread1.Start();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                try
                {
                    Inventec.Common.Logging.LogSystem.Debug(SubscribeCommand + " chanel " + ChanelHisProName + " fail____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => psArr), psArr));
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                    _client = null;
                }
                catch
                {
                }
            }
        }

        void ReceiveDataFromServer()
        {
            try
            {
                EndPoint publisherEndPoint = _client.LocalEndPoint;
                while (true)
                {
                    _recv = _client.ReceiveFrom(_data, ref publisherEndPoint);
                    string msg = Encoding.ASCII.GetString(_data, 0, _recv) + "," + publisherEndPoint.ToString();
                    this.Invoke(new ReceiveSubcribDelegate(ReceiveSubcrib), msg);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public delegate void ReceiveSubcribDelegate(string message);
        public void ReceiveSubcrib(string message)
        {
            try
            {
                string[] messageParts = message.Split(",".ToCharArray());
                string chanel = messageParts[0];
                switch (chanel)
                {
                    case ChanelHisProName:
                        string messageP = messageParts[1];
                        string localIP = messageParts.Length > 2 ? messageParts[2] : "";
                        string ipLans = Inventec.Common.WebApiClient.IpAddressUtils.GetIpAddressLocal();
                        Inventec.Common.Logging.LogSystem.Info("Run Subscribe " + ChanelHisProName + " success |" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => messageP), messageP) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => localIP), localIP) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ipLans), ipLans));

                        if (messageP == DeleteDataCacheCommand || messageP == RestartAppCommand)
                        {
                            var arrlocalIPReturnByService = !String.IsNullOrEmpty(localIP) ? localIP.Split(new string[] { "," }, StringSplitOptions.None) : null;
                            if (arrlocalIPReturnByService != null && arrlocalIPReturnByService.Count() > 0 && ipLans.Contains(arrlocalIPReturnByService[0]))
                            {
                                Inventec.Common.Logging.LogSystem.Info("Máy trạm tạo lệnh pulish sẽ được bỏ qua không chạy lệnh supcript____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => arrlocalIPReturnByService), arrlocalIPReturnByService)
                                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ipLans), ipLans));
                                return;
                            }
                        }

                        if (messageP == DeleteDataCacheCommand)
                        {
                            CacheWorker cacheWorker = new CacheWorker();
                            if (cacheWorker.TruncateAll())
                            {
                                if (MessageBox.Show("Bạn cần thực hiện đăng nhập lại để tải lại dữ liệu cache", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information) == DialogResult.OK)
                                {
                                    //GlobalVariables.IsLostToken = true;
                                    //this.Close();
                                    this.LogoutAndResetToDefault();
                                }
                            }

                            Inventec.Common.Logging.LogSystem.Info("ReceiveDataFromServer " + ChanelHisProName + " command:|" + DeleteDataCacheCommand + " success");
                        }
                        else if (messageP == RestartAppCommand)
                        {
                            MessageManager.Show(HIS.Desktop.Resources.ResourceCommon.BanVuiLongThoatPhanMemRaVaoLaiDeCapNhat);

                            Inventec.Common.Logging.LogSystem.Info("ReceiveDataFromServer " + ChanelHisProName + " command:|" + RestartAppCommand + " success");
                            System.Diagnostics.Process.Start(Application.ExecutablePath);

                            GlobalVariables.isLogouter = true;
                            GlobalVariables.IsLostToken = true;
                            //close this one
                            System.Diagnostics.Process.GetCurrentProcess().Kill();
                        }
                        else if (messageP.StartsWith(UploadLogCommand))
                        {
                            string[] messagePParts = messageP.Split(new string[] { "__" }, StringSplitOptions.RemoveEmptyEntries);
                            Inventec.Common.Logging.LogSystem.Info("ReceiveDataFromServer " + ChanelHisProName + " command:|" + UploadLogCommand + " begin");
                            Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => messagePParts), messagePParts));
                            if (messagePParts != null && messagePParts.Length > 1 && UploadLogCommand == messagePParts[0] && ("," + ipLans + ",").Contains("," + messagePParts[1] + ","))
                            {
                                Inventec.Common.Logging.LogSystem.Info("ReceiveDataFromServer " + ChanelHisProName + " command:|" + UploadLogCommand + " .1");
                                List<FileHolder> files = new List<FileHolder>();
                                List<string> fileFDeletes = new List<string>();

                                files = UploadLogApp(ref fileFDeletes);

                                List<FileHolder> files1 = UploadLogWindows(ref fileFDeletes);
                                if (files1 != null && files1.Count > 0)
                                {
                                    files.AddRange(files1);
                                }

                                Inventec.Common.Logging.LogSystem.Info("ReceiveDataFromServer " + ChanelHisProName + " command:|" + UploadLogCommand + " .2");
                                if (files != null && files.Count > 0)
                                {
                                    var fileResults = Inventec.Fss.Client.FileUpload.UploadFile(GlobalVariables.APPLICATION_CODE, "LogClient__" + System.Environment.MachineName + "", files, true, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_FSS);
                                    Inventec.Common.Logging.LogSystem.Info("Send file to fss:" + HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_FSS + "____command:" + UploadLogCommand + "____fileLogs.Count:" + files.Count);

                                    if (fileFDeletes != null && fileFDeletes.Count > 0)
                                    {
                                        foreach (var fdel in fileFDeletes)
                                        {
                                            try
                                            {
                                                File.Delete(fdel);
                                            }
                                            catch
                                            { }
                                        }
                                    }
                                    files = null;
                                }
                                Inventec.Common.Logging.LogSystem.Info("ReceiveDataFromServer " + ChanelHisProName + " command:|" + UploadLogCommand + " .3");

                            }
                            Inventec.Common.Logging.LogSystem.Info("ReceiveDataFromServer " + ChanelHisProName + " command:|" + UploadLogCommand + " end");
                        }

                        break;
                    default:
                        break;
                }

                Inventec.Common.Logging.LogSystem.Info("ReceiveSubcrib;" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => messageParts), messageParts));

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        List<FileHolder> UploadLogWindows(ref List<string> fileFDeletes)
        {
            List<FileHolder> files = new List<FileHolder>();
            try
            {
                System.Diagnostics.EventLog[] eventLogs = System.Diagnostics.EventLog.GetEventLogs();
                foreach (System.Diagnostics.EventLog elog in eventLogs)
                {

                    Console.WriteLine();
                    Console.WriteLine("{0}:", elog.LogDisplayName);
                    Console.WriteLine("  Log name = \t\t {0}", elog.Log);
                    Console.WriteLine("  Number of event log entries = {0}", elog.Entries.Count.ToString());

                    string applicationExe = "HIS.exe";
                    string contentEventLogWindows = "";
                    foreach (System.Diagnostics.EventLogEntry itemEntry in elog.Entries)
                    {
                        if (!String.IsNullOrEmpty(itemEntry.Message) && itemEntry.Message.Contains(applicationExe) && itemEntry.TimeWritten.ToString("yyyyMMdd") == DateTime.Now.ToString("yyyyMMdd"))
                        {
                            contentEventLogWindows += itemEntry.Message + "\r\n";
                            Console.WriteLine("  Log itemEntry = \t {0}", itemEntry.ToString());
                        }
                    }

                    if (!String.IsNullOrEmpty(contentEventLogWindows))
                    {
                        string filePathCopy = GenerateTempFileWithin(".txt", "");
                        Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => filePathCopy), filePathCopy)
                            + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => contentEventLogWindows), contentEventLogWindows));
                        File.WriteAllText(filePathCopy, contentEventLogWindows);

                        fileFDeletes.Add(filePathCopy);

                        FileHolder fileHolder = new FileHolder();
                        fileHolder.Content = new MemoryStream(File.ReadAllBytes(filePathCopy));
                        fileHolder.FileName = "EventLogWindows.txt";
                        files.Add(fileHolder);
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return files;
        }

        List<FileHolder> UploadLogApp(ref List<string> fileFDeletes)
        {
            List<FileHolder> files = new List<FileHolder>();
            try
            {
                DirectoryInfo dicInfo = new DirectoryInfo(Path.Combine(ApplicationStoreLocation.ApplicationStartupPath, "Logs"));
                var fileSystems = Directory.GetFiles(dicInfo.FullName, "LogSystem.txt*").ToList();
                if (fileSystems != null && fileSystems.Count > 0)
                {
                    var listFileSysInDays = (fileSystems != null && fileSystems.Count() > 0) ? fileSystems.Where(o => CheckFileLogNameInDay(o, "LogSystem.txt")).OrderByDescending(o => o).ToList() : null;
                    if (listFileSysInDays != null && listFileSysInDays.Count > 0)
                    {

                        foreach (string file in listFileSysInDays)
                        {
                            string filePathCopy = GenerateTempFileWithin(".txt", "");
                            File.Copy(file, filePathCopy);
                            fileFDeletes.Add(filePathCopy);

                            FileHolder fileHolder = new FileHolder();
                            fileHolder.Content = new MemoryStream(File.ReadAllBytes(filePathCopy));
                            fileHolder.FileName = Path.GetFileName(file);
                            files.Add(fileHolder);
                        }
                    }
                }

                var fileActions = Directory.GetFiles(dicInfo.FullName, "LogAction.txt*").ToList();
                if (fileActions != null && fileActions.Count > 0)
                {
                    var listFileActionInDays = (fileActions != null && fileActions.Count() > 0) ? fileActions.Where(o => CheckFileLogNameInDay(o, "LogAction.txt")).OrderByDescending(o => o).ToList() : null;
                    if (listFileActionInDays != null && listFileActionInDays.Count > 0)
                    {

                        foreach (string file in listFileActionInDays)
                        {
                            string filePathCopy = GenerateTempFileWithin(".txt", "");
                            File.Copy(file, filePathCopy);
                            fileFDeletes.Add(filePathCopy);

                            FileHolder fileHolder = new FileHolder();
                            fileHolder.Content = new MemoryStream(File.ReadAllBytes(filePathCopy));
                            fileHolder.FileName = Path.GetFileName(file);
                            files.Add(fileHolder);
                        }
                    }
                }

                var fileSessions = Directory.GetFiles(dicInfo.FullName, "LogSession.txt*").ToList();
                if (fileSessions != null && fileSessions.Count > 0)
                {
                    var listFileSessionInDays = (fileSessions != null && fileSessions.Count() > 0) ? fileSessions.Where(o => CheckFileLogNameInDay(o, "LogSession.txt")).OrderByDescending(o => o).ToList() : null;
                    if (listFileSessionInDays != null && listFileSessionInDays.Count > 0)
                    {

                        foreach (string file in listFileSessionInDays)
                        {
                            string filePathCopy = GenerateTempFileWithin(".txt", "");
                            File.Copy(file, filePathCopy);
                            fileFDeletes.Add(filePathCopy);

                            FileHolder fileHolder = new FileHolder();
                            fileHolder.Content = new MemoryStream(File.ReadAllBytes(filePathCopy));
                            fileHolder.FileName = Path.GetFileName(file);
                            files.Add(fileHolder);
                        }
                    }
                }

                var fileTimes = Directory.GetFiles(dicInfo.FullName, "LogTime.txt*").ToList();
                if (fileTimes != null && fileTimes.Count > 0)
                {
                    var listFileTimeInDays = (fileTimes != null && fileTimes.Count() > 0) ? fileTimes.Where(o => CheckFileLogNameInDay(o, "LogTime.txt")).OrderByDescending(o => o).ToList() : null;
                    if (listFileTimeInDays != null && listFileTimeInDays.Count > 0)
                    {

                        foreach (string file in listFileTimeInDays)
                        {
                            string filePathCopy = GenerateTempFileWithin(".txt", "");
                            File.Copy(file, filePathCopy);
                            fileFDeletes.Add(filePathCopy);

                            FileHolder fileHolder = new FileHolder();
                            fileHolder.Content = new MemoryStream(File.ReadAllBytes(filePathCopy));
                            fileHolder.FileName = Path.GetFileName(file);
                            files.Add(fileHolder);
                        }
                    }
                }

                var fileHLSLogSystems = Directory.GetFiles(dicInfo.FullName, "HLSLogSystem.txt*").ToList();
                if (fileHLSLogSystems != null && fileHLSLogSystems.Count > 0)
                {
                    var listFileHLSLogSystemInDays = (fileHLSLogSystems != null && fileHLSLogSystems.Count() > 0) ? fileHLSLogSystems.Where(o => CheckFileLogNameInDay(o, "HLSLogSystem.txt")).OrderByDescending(o => o).ToList() : null;
                    if (listFileHLSLogSystemInDays != null && listFileHLSLogSystemInDays.Count > 0)
                    {

                        foreach (string file in listFileHLSLogSystemInDays)
                        {
                            string filePathCopy = GenerateTempFileWithin(".txt", "");
                            File.Copy(file, filePathCopy);
                            fileFDeletes.Add(filePathCopy);

                            FileHolder fileHolder = new FileHolder();
                            fileHolder.Content = new MemoryStream(File.ReadAllBytes(filePathCopy));
                            fileHolder.FileName = Path.GetFileName(file);
                            files.Add(fileHolder);
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => fileHolder.FileName), fileHolder.FileName));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

            return files;
        }

        string GenerateTempFileWithin(string ext = "", string filename = "")
        {
            try
            {
                string pathFolderTemp = Path.Combine(Path.Combine(ApplicationStoreLocation.ApplicationStartupPath, "temp"), DateTime.Now.ToString("ddMMyyyy"));
                if (!Directory.Exists(pathFolderTemp))
                {
                    Directory.CreateDirectory(pathFolderTemp);
                }
                return Path.Combine(pathFolderTemp, (String.IsNullOrEmpty(filename) ? Guid.NewGuid().ToString() : filename) + (String.IsNullOrEmpty(ext) ? ".txt" : ext));
            }
            catch (IOException exception)
            {
                Console.WriteLine("Error create temp file: " + exception.Message);
                return "";
            }
        }

        bool CheckFileLogNameInDay(string filename, string logTypeName = "")
        {
            bool valid = false;
            try
            {
                if (!String.IsNullOrEmpty(filename))
                {
                    var arrFileName = filename.Contains(logTypeName) ? filename.Split(new string[] { (String.IsNullOrEmpty(logTypeName) ? "LogSystem.txt" : logTypeName) }, StringSplitOptions.RemoveEmptyEntries) : null;
                    if (arrFileName != null && arrFileName.Length == 1 || (arrFileName.Length == 2 && arrFileName[1].Length < 10))
                    {
                        valid = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return valid;
        }

        private void UnSubcribChanel()
        {
            try
            {
                if (_client != null)
                {
                    string message = UnSubscribeCommand + "," + ChanelHisProName;
                    _client.SendTo(Encoding.ASCII.GetBytes(message), _remoteEndPoint);
                    Inventec.Common.Logging.LogSystem.Info("Run UnSubscribe " + ChanelHisProName + " success |" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => message), message) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => _remoteEndPoint), _remoteEndPoint.ToString()));
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

    }
}

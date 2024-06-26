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
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;
using DevExpress.XtraTab;
using DevExpress.XtraTab.ViewInfo;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.ModuleExt;
using HIS.Desktop.Plugins.Library.ConnectBloodPressure;
using HIS.Desktop.Resources;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Common.RichEditor;
using Inventec.Common.ThreadCustom;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using Inventec.Desktop.Common.Modules;
using Inventec.Desktop.Common.Theme;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using RDCACHE.SDO;
using SDA.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;
using HIS.Desktop.Modules.CheckServerConnect;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using MOS.SDO;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.Utilities.RemoteSupport;
using System.Net;
using System.Management;
using System.Net.Sockets;
using HIS.Desktop.Utilities;
using System.Threading.Tasks;
using System.Runtime;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using SDA.Filter;

namespace HIS.Desktop.Modules.Main
{
    public partial class frmMain : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        #region Declare variable
        private const string BTN000006 = "HIS000006";
        private const string BTN000008 = "HIS000008";
        private const string BTN000009 = "HIS000009";
        private const string BTN000029 = "HIS000029";
        System.Windows.Forms.Timer timerSyncAll;
        System.Windows.Forms.Timer timerByCacheMonitor;
        System.Windows.Forms.Timer timerLogVplus;
        string imageCollection16Path = "Img\\Icon\\16";
        string imageCollection32Path = "Img\\Icon\\32";
        const string extensionAllows = ".png|.jpg|.bmp|.jpeg";
        PopupMenuProcessor popupMenuProcessor;

        System.Drawing.Font fontGeneral;
        System.Drawing.Font fontBoldGeneral;
        #endregion

        #region OnLoad
        public frmMain()
        {
            try
            {
                InitializeComponent();
                try
                {
                    string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                }
                catch (Exception ex)
                {
                    LogSystem.Warn(ex);
                }

                InitImageCollection();
                if (!IsRunUnitTest)
                {
                    InitMultipleThread();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Fatal(ex);
            }
        }

        private void InitImageCollection()
        {
            try
            {
                string imageCollection16FullPath = Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, imageCollection16Path);
                if (Directory.Exists(imageCollection16FullPath))
                {
                    // This path is a directory
                    ProcessDirectory(imageCollection16FullPath, imageCollection16);
                }
                string imageCollection32FullPath = Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, imageCollection32Path);
                if (Directory.Exists(imageCollection32FullPath))
                {
                    // This path is a directory
                    ProcessDirectory(imageCollection32FullPath, imageCollection32);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        // Process all files in the directory passed in, recurse on any directories 
        // that are found, and process the files they contain.
        public void ProcessDirectory(string targetDirectory, DevExpress.Utils.ImageCollection imageCollection)
        {
            try
            {
                // Process the list of files found in the directory.
                string[] fileEntries = Directory.GetFiles(targetDirectory);
                foreach (string fileName in fileEntries)
                {
                    try
                    {
                        string extension = Path.GetExtension(fileName).ToLower();
                        if (!extensionAllows.Contains(extension))
                            continue;

                        imageCollection.AddImage(Image.FromFile(fileName), fileName.Substring(fileName.LastIndexOf("\\") + 1));
                    }
                    catch (Exception exx)
                    {
                        LogSystem.Warn("Tai file icon tu folder local vao ram that bai, ___" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => fileName), fileName), exx);
                    }
                }

                // Recurse into subdirectories of this directory.
                string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
                foreach (string subdirectory in subdirectoryEntries)
                    ProcessDirectory(subdirectory, imageCollection);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            try
            {
                if (IsRunUnitTest)
                {
                    timerRunUnitTest = new System.Windows.Forms.Timer();
                    timerRunUnitTest.Interval = 1000;
                    timerRunUnitTest.Enabled = true;
                    timerRunUnitTest.Tick += timerForUnitTest_Tick;
                    timerRunUnitTest.Start();
                }
                else
                {
                    GetSdaLicense();
                    LoadDataAsync();
                    RunPubSub();

                    RunCheckTokenTimeout();
                    RunSyncBackendDataToLocal();
                    RunLogAction();
                    RunLogService();
                    RunNotify();
                    RunCheckConnectServer();
                    InitSubscriber();//TODO
                    InitDefaultSelectRoom();
                    InitWcfAssignPrescriptionByCFG();
                    DevExpress.XtraEditors.XtraMessageBox.AllowHtmlText = true;
                    SetDefaultTabpageFocus();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        int w;
        string MessageLicense = null;
        Dictionary<string, string> dicAppCode = new Dictionary<string, string>();
        private async void GetSdaLicense()
        {

            try
            {
                lblLicense.Visibility = BarItemVisibility.Never;
                List<SDA_LICENSE> licenseExpires = null;
                List<SDA_LICENSE> licenseAlExpired = null;
                dicAppCode.Add("00001", "Thanh toán viện phí sử dụng mã QR tích hợp ngân hàng");
                dicAppCode.Add("00002", "Ký số, ký điện tử");
                dicAppCode.Add("LIS", "Quản lý thông tin xét nghiệm");
                dicAppCode.Add("EMR", "Quản lý và lưu trữ bệnh án điện tử");
                Task task = new Task(() =>
                {
                    CommonParam paramCommon = new CommonParam();
                    SdaLicenseFilter filter = new SdaLicenseFilter();
                    filter.IS_ACTIVE = 1;
                    var result = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<SDA_LICENSE>>("api/SdaLicense/Get", ApiConsumers.SdaConsumer, filter, paramCommon);
                    if (result != null && result.Count > 0)
                    {
                        licenseExpires = result.Where(o => o.EXPIRED_DATE != null && o.EXPIRED_DATE <= Int64.Parse(DateTime.Now.ToString("yyyyMMdd"))).ToList();
                        licenseAlExpired = result.Where(o => o.EXPIRED_DATE != null && Int64.Parse(DateTime.Now.ToString("yyyyMMdd")) < o.EXPIRED_DATE && o.EXPIRED_DATE <= Int64.Parse(DateTime.Now.AddDays(15).ToString("yyyyMMdd"))).ToList();
                    }
                }
                );
                task.Start();
                await task;
                List<string> lstMes = new List<string>();
                if (licenseExpires != null && licenseExpires.Count > 0)
                {
                    List<string> lstCon = new List<string>();
                    foreach (var item in licenseExpires)
                    {
                        lstCon.Add(string.Format("Tính năng {0} đã hết hạn sử dụng", dicAppCode[item.APP_CODE]));
                    }
                    lstMes.Add(string.Format("{0}", string.Join(". ", lstCon)));
                    DevExpress.XtraEditors.XtraMessageBox.Show(string.Join(". ", string.Format("Tính năng {0} đã hết hạn sử dụng", string.Join(", ", licenseExpires.Select(o => dicAppCode[o.APP_CODE])))) + ". Vui lòng thanh toán phí dịch vụ để gia hạn sử dụng.", HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                }
                if (licenseAlExpired != null && licenseAlExpired.Count > 0)
                {
                    List<string> lstCon = new List<string>();
                    foreach (var item in licenseAlExpired)
                    {
                        lstCon.Add(string.Format("Tính năng {0} sẽ hết hạn sử dụng vào ngày {1}", dicAppCode[item.APP_CODE], Inventec.Common.DateTime.Convert.TimeNumberToDateString(item.EXPIRED_DATE ?? 0)));
                    }
                    lstMes.Add(string.Format("{0}", string.Join(". ", lstCon)));
                    DevExpress.XtraEditors.XtraMessageBox.Show(string.Join(". ", lstCon) + ". Vui lòng thanh toán phí dịch vụ và gia hạn sử dụng để không bị gián đoạn hoạt động.", HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                }
                if(lstMes != null && lstMes.Count > 0)
                {
                    MessageLicense = string.Join(". ", lstMes);
                    lblLicense.Visibility = BarItemVisibility.Always;
                    lblLicense.Caption = MessageLicense;
                    var w = Convert.ToDecimal(lblLicense.Font.Size);
                    if ((Convert.ToDecimal(this.MessageLicense.Length) * (decimal)0.7 * w) > lblLicense.Size.Width)
                    {
                        MessageLicense += "            ";
                        timerEffectText.Start();
                    }else
                    {
                        lblLicense.Size = new Size((int)(Convert.ToDecimal(this.MessageLicense.Length) * (decimal)0.7 * w) + 30, lblLicense.Size.Height);
                    }    
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void RunPubSub()
        {
            try
            {
                HIS.Desktop.LocalStorage.PubSub.PubSubAction.frmShowNotifi = this;
                HIS.Desktop.LocalStorage.PubSub.PubSubAction.IntPubSubClient();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void RunLogService()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("RunLogService.1");
                var time = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HisConfigKeys.CONFIG_KEY__LOG_TIMER);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => time), time));
                if (string.IsNullOrEmpty(time))
                    return;
                string pathLogVPlus = Application.StartupPath + @"\Integrate\LOG.VPlus";
                string pathLogVPlusExe = Application.StartupPath + @"\Integrate\LOG.VPlus\LOG.VPlus.exe";
                if (!Directory.Exists(pathLogVPlus))
                {
                    LogSystem.Warn(string.Format("Không tồn tại đường dẫn {0}", pathLogVPlus));
                    return;
                }
                if (!File.Exists(pathLogVPlusExe))
                {
                    LogSystem.Warn(string.Format("Không tồn tại file cài {0}", pathLogVPlusExe));
                    return;
                }
                Inventec.Common.Logging.LogSystem.Debug("RunLogService.2");
                InitDefaultBranch();
                timerLogVplus = new System.Windows.Forms.Timer();
                timerLogVplus.Interval = Int32.Parse(time);
                timerLogVplus.Enabled = true;
                timerLogVplus.Tick += timerForLogVplus_Tick;
                timerLogVplus.Start();
                Inventec.Common.Logging.LogSystem.Debug("RunLogService.3");
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void timerForLogVplus_Tick(object sender, EventArgs e)
        {
            try
            {
                Task.Factory.StartNew(CallExeLOGNewThread);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CallExeLOGNewThread()
        {
            try
            {
                if (!CloseAllApp.IsProcessOpen("LOG.VPlus"))
                {
                    string customerCode = "";
                    string customerName = "";
                    string stt_sản_phẩm = "";
                    string stt_phần_mềm = "";
                    string customerInfo = HisConfigs.Get<string>(HisConfigKeys.CONFIG_KEY__VPLUS_CUSTOMER_INFO);
                    string syncVlog_IsUAT = HisConfigs.Get<string>(HisConfigKeys.CONFIG_KEY__SyncVlog_IsUAT);
                    if (!String.IsNullOrEmpty(customerInfo))
                    {
                        var cusInfoArr = customerInfo.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                        if (cusInfoArr != null && cusInfoArr.Length > 2)
                        {
                            customerCode = cusInfoArr[0];
                            customerName = cusInfoArr[1];
                            stt_sản_phẩm = cusInfoArr[2];
                            stt_phần_mềm = cusInfoArr[3];
                        }
                    }
                    var tc = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetTokenData();
                    string cmdLn = "";
                    cmdLn += "|customerCode|" + customerCode;
                    cmdLn += "|customerName|" + customerName;
                    cmdLn += "|stt_sản_phẩm|" + stt_sản_phẩm;
                    cmdLn += "|stt_phần_mềm|" + stt_phần_mềm;
                    cmdLn += "|Version|" + (File.Exists(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, "readme.txt")) ? System.IO.File.ReadAllText(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, "readme.txt")) : "");
                    cmdLn += "|IP|" + StringUtil.GetIpLocal();
                    cmdLn += "|Loginname|" + (tc != null ? (tc.User.LoginName) : "");
                    cmdLn += "|IsRunFromHis|" + true;
                    cmdLn += "|PathLogsHis|" + Application.StartupPath + @"\Logs";
                    cmdLn += "|HeinCode|" + (syncVlog_IsUAT == "1" ? "00000" : (BranchDataWorker.Branch != null ? BranchDataWorker.Branch.HEIN_MEDI_ORG_CODE : ""));
                    Inventec.Common.Logging.LogSystem.Debug("cmdLn Send LOG.VPlus= " + cmdLn);

                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.FileName = Application.StartupPath + @"\Integrate\LOG.VPlus\LOG.VPlus.exe";
                    startInfo.Arguments = "\"" + cmdLn + "\"";
                    Process.Start(startInfo);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        /// <summary>
        /// Close main form 
        /// extit application and stop process in computer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("frmMain_FormClosing.1");
                var watch = System.Diagnostics.Stopwatch.StartNew();
                long startBytes = GC.GetTotalMemory(true);
                decimal memoryUsageusers = (decimal)(startBytes) / (1024 * 1024);
                Process proc = Process.GetCurrentProcess();

                Inventec.Common.Logging.LogSystem.Debug("frmMain_FormClosing____Dung luong PM( GC.GetTotalMemory):" + ((decimal)startBytes / (1024 * 1024)) + "MB");
                Inventec.Common.Logging.LogSystem.Debug("frmMain_FormClosing____Dung luong PM(proc.PrivateMemorySize64):" + ((decimal)proc.PrivateMemorySize64 / (1024 * 1024)) + "MB");

                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => GlobalVariables.isLogouter), GlobalVariables.isLogouter));
                if (!GlobalVariables.isLogouter)
                {
                    if (MessageBox.Show(HIS.Desktop.Resources.ResourceCommon.ThongBaoBanCoMuonThoatPhanMem, HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {

                        ResetClientCacheFile();

                        GlobalVariables.isLogouter = true;
                        GlobalVariables.IsLostToken = true;


                        long tuDongLogoutKhiTatPhanMem = ConfigApplicationWorker.Get<long>(AppConfigKeys.CONFIG_KEY__HIS_DESKTOP__AUTO_TOKEN_LOGOUT_WHILE_CLOSE_APPLICATION);
                        if (tuDongLogoutKhiTatPhanMem == 1)
                        {
                            if (!Inventec.Desktop.Common.Token.TokenManager.Logout())
                                Inventec.Common.Logging.LogSystem.Warn(TabWorker.formClosing);
                        }
                        else
                        {
                            this.CallRemoveTokenData();
                        }
                        MPS.ProcessorBase.PrintLog.PrintLogProcess.CreateSarPrintLog(true);

                        // Thời gian kết thúc
                        watch.Stop();
                        Inventec.Common.Logging.LogAction.Info(String.Format("{0}____{1}____{2}____{3}____{4}____{5}____{6}____{7}", GlobalVariables.APPLICATION_CODE, GlobalString.VersionApp, (double)((double)watch.ElapsedMilliseconds / (double)1000), "HIS.Desktop", "CloseApp", Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName(), StringUtil.GetIpLocal(), StringUtil.CustomerCode));

                        try
                        {
                            UnSubcribChanel();

                            HIS.Desktop.LocalStorage.PubSub.PubSubAction.DisposePubSub();

                            CloseAllApp.CloseAllApps();

                            Application.Exit();
                            System.Diagnostics.Process.GetCurrentProcess().Kill();
                        }
                        catch (Exception ex)
                        {
                            LogSystem.Debug("Kill process HIS.Desktop.Notify | HLS.WCFClient that bai. ", ex);
                        }
                    }
                    else
                    {
                        e.Cancel = true;
                    }
                }
                Inventec.Common.Logging.LogSystem.Info("frmMain_FormClosing.2");
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, Keys keyData)
        {
            try
            {
                if (keyData == Keys.Escape)
                {
                    if (MessageBox.Show(HIS.Desktop.Resources.ResourceCommon.BanCoChacChanMuonThoatCuaSoDangLamViec, HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        HIS.Desktop.ModuleExt.TabControlBaseProcess.CloseSelectedTabPage(SessionManager.GetTabControlMain());
                    }
                    return true;
                }
                else if (keyData == Keys.Space && ShortcutReplace.IsUseShortcutReplaceKeyBase)
                {
                    Control c = Control.FromHandle(msg.HWnd);
                    if (ShortcutReplace.ReplaceValue(c)) return true;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        #endregion

        #region Private Mothod

        private void ResetClientCacheFile()
        {
            try
            {
                string pathDel = Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationDirectory, "temp");
                if (Directory.Exists(pathDel))
                {
                    System.IO.DirectoryInfo di = new DirectoryInfo(pathDel);
                    foreach (FileInfo file in di.GetFiles())
                    {
                        try
                        {
                            File.SetAttributes(file.FullName, FileAttributes.Normal);
                            file.Delete();
                        }
                        catch (Exception exx1)
                        {
                            Inventec.Common.Logging.LogSystem.Warn(exx1);
                        }
                    }
                    foreach (DirectoryInfo dir in di.GetDirectories())
                    {
                        foreach (FileInfo file in dir.GetFiles())
                        {
                            try
                            {
                                File.SetAttributes(file.FullName, FileAttributes.Normal);
                                file.Delete();
                            }
                            catch (Exception exx1)
                            {
                                Inventec.Common.Logging.LogSystem.Warn(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => file.FullName), file.FullName));
                                Inventec.Common.Logging.LogSystem.Warn(exx1);
                            }
                        }
                        try
                        {
                            dir.Delete(true);
                        }
                        catch (Exception exx1)
                        {
                            Inventec.Common.Logging.LogSystem.Warn(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => dir), dir));
                            Inventec.Common.Logging.LogSystem.Warn(exx1);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void FrdLoadFactory()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("FrdLoadFactory .1");
                FRD.FrdProcessor process = new FRD.FrdProcessor();//load dll FRD
                process.Initialize();
                Inventec.Common.Logging.LogSystem.Debug("FrdLoadFactory .2");
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void MpsPrinterLoadFactory()
        {
            try
            {
                MPS.ProcessorBase.PrintConfig.ShowModulePrintLog = CallModuleShowPrintLog;
                Inventec.Common.FlexCelPrint.Ado.SupportAdo.RemoteSupport = (() => barBtnSendReflection_ItemClick(null, null));
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void CallModuleShowPrintLog(string printTypeCode, string uniqueCode)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(printTypeCode) && !String.IsNullOrWhiteSpace(uniqueCode))
                {
                    //goi modul
                    HIS.Desktop.ADO.PrintLogADO ado = new HIS.Desktop.ADO.PrintLogADO(printTypeCode, uniqueCode);

                    List<object> listArgs = new List<object>();
                    listArgs.Add(ado);

                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("Inventec.Desktop.Plugins.PrintLog", 0, 0, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GenerateMenuInTopBar()
        {
            try
            {
                //Load dong menu
                new Base.Menu().GenerateMenu(this.ribbonMain, imageCollection32, imageCollection16);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void InitWcfAssignPrescriptionByCFG()
        {
            try
            {
                HIS.Desktop.Plugins.Library.IntegrateAssignPrescription.IntegrateAssignPrescriptionProcesser integrateAssignPrescriptionProcesser = new Plugins.Library.IntegrateAssignPrescription.IntegrateAssignPrescriptionProcesser();
                integrateAssignPrescriptionProcesser.OpenHost();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void CloseHostWcfAssignPrescriptionByCFG()
        {
            try
            {
                HIS.Desktop.Plugins.Library.IntegrateAssignPrescription.IntegrateAssignPrescriptionProcesser integrateAssignPrescriptionProcesser = new Plugins.Library.IntegrateAssignPrescription.IntegrateAssignPrescriptionProcesser();
                integrateAssignPrescriptionProcesser.CloseHost();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Kiểm tra nếu tài khoản của người dùng đăng nhập chỉ có một phòng làm việc thì tự động chọn phòng làm việc đó luôn, ngược lại vẫn phải chọn phòng làm việc
        /// </summary>
        private void InitDefaultSelectRoom()
        {
            try
            {
                CommonParam param = new CommonParam();
                var rooms = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_ROOM>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                HisUserRoomViewFilter userRoomViewFilter = new MOS.Filter.HisUserRoomViewFilter();
                userRoomViewFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                userRoomViewFilter.LOGINNAME = loginName;
                var userRoomByUsers = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_USER_ROOM>>(HisRequestUriStore.HIS_USER_ROOM_GETVIEW, ApiConsumers.MosConsumer, userRoomViewFilter, param);
                long currentBranhId = BranchDataWorker.GetCurrentBranchId();
                if (currentBranhId > 0)
                {
                    userRoomByUsers = userRoomByUsers.Where(o => o.BRANCH_ID == currentBranhId).ToList();
                }
                if (userRoomByUsers != null && userRoomByUsers.Count > 0)
                {
                    if (userRoomByUsers.Count == 1)
                        ChoiceDefaultRoom(userRoomByUsers[0]);
                    else
                        ShowModuleSelectedRoom();

                    //làm việc tại 1 phòng hoặc chọn 1 phòng
                    ProcessAutoOpenModuleLink();
                }
                else
                {
                    WaitingManager.Hide();
                    DevExpress.XtraEditors.XtraMessageBox.Show(ResourceCommon.TaiKhoanChuaDuocCauHinhTaiKhoanPhongCuaChiNhanh, HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                    return;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void ChoiceDefaultRoom(V_HIS_USER_ROOM room)
        {
            try
            {
                CommonParam param = new CommonParam();
                List<long> roomIdCheckeds = new List<long>();
                if (room != null)
                {
                    WaitingManager.Show();
                    roomIdCheckeds.Add(room.ROOM_ID);
                    WorkPlace.WorkPlaceSDO = new BackendAdapter(param).Post<List<MOS.SDO.WorkPlaceSDO>>(HisRequestUriStore.TOKEN__UPDATE_WORK_PLACE_LIST, ApiConsumer.ApiConsumers.MosConsumer, roomIdCheckeds, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    if (WorkPlace.WorkPlaceSDO != null && WorkPlace.WorkPlaceSDO.Count > 0)
                    {
                        GlobalVariables.CurrentRoomTypeIds = WorkPlace.WorkPlaceSDO.Select(o => o.RoomTypeId).ToList();
                        GlobalVariables.CurrentRoomTypeId = WorkPlace.WorkPlaceSDO.FirstOrDefault().RoomTypeId;
                        var roomTypes = BackendDataWorker.Get<HIS_ROOM_TYPE>().Where(o => GlobalVariables.CurrentRoomTypeIds.Contains(o.ID)).ToList();
                        if (roomTypes != null && roomTypes.Count > 0)
                        {
                            GlobalVariables.CurrentRoomTypeCode = roomTypes[0].ROOM_TYPE_CODE;
                            GlobalVariables.CurrentRoomTypeCodes = roomTypes.Select(o => o.ROOM_TYPE_CODE).ToList();
                        }
                        if (BranchDataWorker.GetCurrentBranchId() == 0 || BranchDataWorker.GetCurrentBranchId() != WorkPlace.WorkPlaceSDO[0].BranchId)
                        {
                            //Luu chi nhanh dang chon vao registry
                            HIS.Desktop.LocalStorage.BackendData.BranchDataWorker.ChangeBranch(WorkPlace.GetBranchId());
                        }

                        RichEditorConfig.WorkingRoom = BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.ID == room.ROOM_ID).FirstOrDefault();
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => RichEditorConfig.WorkingRoom), RichEditorConfig.WorkingRoom));
                        RefeshReference();
                        WaitingManager.Hide();
                    }
                    else
                    {
                        if (param.Messages.Count == 0)
                            param.Messages.Add(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBXuatHienExceptionChuaKiemDuocSoat));

                        WaitingManager.Hide();
                        MessageManager.Show(param, false);
                        GlobalVariables.CurrentRoomTypeId = 0;
                        GlobalVariables.CurrentRoomTypeIds = null;
                        GlobalVariables.CurrentRoomTypeCodes = null;
                        GlobalVariables.CurrentRoomTypeCode = "";
                        WorkPlace.WorkPlaceSDO = null;
                        WorkPlace.WorkInfoSDO = null;
                        LogSystem.Warn("Goi api Token.UpdateWorkplace khong thanh cong" + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => room), room));
                    }
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }

        private void ProcessAuthoriButton()
        {
            try
            {
                var btton = GlobalVariables.AcsAuthorizeSDO.ControlInRoles.FirstOrDefault(o => o.CONTROL_CODE == BTN000006);
                this.bbtnResetServerConfigCache.Visibility = (btton != null ? BarItemVisibility.Always : BarItemVisibility.Never);

                var btton8 = GlobalVariables.AcsAuthorizeSDO.ControlInRoles.FirstOrDefault(o => o.CONTROL_CODE == BTN000008);
                MPS.ProcessorBase.PrintConfig.IsExportExcel = (btton8 != null);

                var btton9 = GlobalVariables.AcsAuthorizeSDO.ControlInRoles.FirstOrDefault(o => o.CONTROL_CODE == BTN000009);
                this.bbtnResetLisConfig.Visibility = (btton9 != null ? BarItemVisibility.Always : BarItemVisibility.Never);

                var bttonEmr = GlobalVariables.AcsAuthorizeSDO.ControlInRoles.FirstOrDefault(o => o.CONTROL_CODE == BTN000029);
                this.bbtnRefreshEmrConfig.Visibility = (bttonEmr != null ? BarItemVisibility.Always : BarItemVisibility.Never);

                //this.bbtnChanel.Visibility = ((GlobalVariables.AcsAuthorizeSDO.IsFull && !String.IsNullOrWhiteSpace(HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_HIS_PUBSUB_BASE)) ? BarItemVisibility.Always : BarItemVisibility.Never);
            }
            catch (Exception ex)
            {
                LogSystem.Fatal(ex);
            }
        }

        /// <summary>
        /// Set language text for control
        /// </summary>
        private void LoadKeysFromlanguage()
        {
            try
            {
                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt

                if (LanguageManager.GetLanguage() == LanguageManager.GetLanguageEn())
                {
                    this.Text = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HisConfigKeys.KEY__LANGUAGE__FRMMAIN__CAPTION__EN);
                }
                else if (LanguageManager.GetLanguage() == LanguageManager.GetLanguageMy())
                {
                    this.Text = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HisConfigKeys.KEY__LANGUAGE__FRMMAIN__CAPTION__MY);
                }
                else
                {
                    this.Text = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HisConfigKeys.KEY__LANGUAGE__FRMMAIN__CAPTION);
                }
                this.bbtnResetServerConfigCache.Caption = Inventec.Common.Resource.Get.Value("frmMain.bbtnResetServerConfigCache.Caption", Resources.ResourceLanguageManager.LanguageFrmMain, LanguageManager.GetCulture());
                this.bbtnRefreshCache.Caption = Inventec.Common.Resource.Get.Value("frmMain.bbtnRefreshCache.Caption", Resources.ResourceLanguageManager.LanguageFrmMain, LanguageManager.GetCulture());
                this.bbtnEventLog.Caption = Inventec.Common.Resource.Get.Value("frmMain.bbtnEventLog.Caption", Resources.ResourceLanguageManager.LanguageFrmMain, LanguageManager.GetCulture());
                this.bbtnConfigApplication.Caption = Inventec.Common.Resource.Get.Value("frmMain.bbtnConfigApplication.Caption", Resources.ResourceLanguageManager.LanguageFrmMain, LanguageManager.GetCulture());
                this.bbtnChangePassword.Caption = Inventec.Common.Resource.Get.Value("frmMain.bbtnChangePassword.Caption", Resources.ResourceLanguageManager.LanguageFrmMain, LanguageManager.GetCulture());
                this.bbtnPlugin.Caption = Inventec.Common.Resource.Get.Value("frmMain.bbtnPlugin.Caption", Resources.ResourceLanguageManager.LanguageFrmMain, LanguageManager.GetCulture());
                this.bbtnClose.Caption = Inventec.Common.Resource.Get.Value("frmMain.bbtnClose.Caption", Resources.ResourceLanguageManager.LanguageFrmMain, LanguageManager.GetCulture());
                this.lblLanguage.Caption = Inventec.Common.Resource.Get.Value("frmMain.lblLanguage.Caption", Resources.ResourceLanguageManager.LanguageFrmMain, LanguageManager.GetCulture());
                this.lblRoomName.Caption = Inventec.Common.Resource.Get.Value("frmMain.lblRoomName.Caption", Resources.ResourceLanguageManager.LanguageFrmMain, LanguageManager.GetCulture());
                this.skinRibbonGalleryBarItem1.Caption = Inventec.Common.Resource.Get.Value("frmMain.skinRibbonGalleryBarItem1.Caption", Resources.ResourceLanguageManager.LanguageFrmMain, LanguageManager.GetCulture());
                this.barHeaderItem1.Caption = Inventec.Common.Resource.Get.Value("frmMain.barHeaderItem1.Caption", Resources.ResourceLanguageManager.LanguageFrmMain, LanguageManager.GetCulture());
                this.barHeaderItem2.Caption = Inventec.Common.Resource.Get.Value("frmMain.barHeaderItem2.Caption", Resources.ResourceLanguageManager.LanguageFrmMain, LanguageManager.GetCulture());
                this.lblVersion.Caption = Inventec.Common.Resource.Get.Value("frmMain.lblVersion.Caption", Resources.ResourceLanguageManager.LanguageFrmMain, LanguageManager.GetCulture());
                this.bbtnReport.Caption = Inventec.Common.Resource.Get.Value("frmMain.bbtnReport.Caption", Resources.ResourceLanguageManager.LanguageFrmMain, LanguageManager.GetCulture());
                this.bbtnVersionInfo.Caption = Inventec.Common.Resource.Get.Value("frmMain.bbtnVersionInfo.Caption", Resources.ResourceLanguageManager.LanguageFrmMain, LanguageManager.GetCulture());
                this.bbtnCommonChooseRoom.Caption = Inventec.Common.Resource.Get.Value("frmMain.bbtnCommonChooseRoom.Caption", Resources.ResourceLanguageManager.LanguageFrmMain, LanguageManager.GetCulture());
                this.bbtnTutorial.Caption = Inventec.Common.Resource.Get.Value("frmMain.bbtnTutorial.Caption", Resources.ResourceLanguageManager.LanguageFrmMain, LanguageManager.GetCulture());
                this.blblHelp.Caption = Inventec.Common.Resource.Get.Value("frmMain.blblHelp.Caption", Resources.ResourceLanguageManager.LanguageFrmMain, LanguageManager.GetCulture());
                this.bbtnChanelRemoteSupport.Caption = Inventec.Common.Resource.Get.Value("frmMain.bbtnChanelRemoteSupport.Caption", Resources.ResourceLanguageManager.LanguageFrmMain, LanguageManager.GetCulture());
                this.bbtnClose.Caption = Inventec.Common.Resource.Get.Value("frmMain.bbtnClose.Caption", Resources.ResourceLanguageManager.LanguageFrmMain, LanguageManager.GetCulture());
                this.bbtnResetLisConfig.Caption = Inventec.Common.Resource.Get.Value("frmMain.bbtnResetLisConfig.Caption", Resources.ResourceLanguageManager.LanguageFrmMain, LanguageManager.GetCulture());
                this.bbtnRefreshEmrConfig.Caption = Inventec.Common.Resource.Get.Value("frmMain.bbtnRefreshEmrConfig.Caption", Resources.ResourceLanguageManager.LanguageFrmMain, LanguageManager.GetCulture());
                //this.toolTipItem4.Text = Inventec.Common.Resource.Get.Value("frmMain.bbtnChanelRemoteSupport.Caption", Resources.ResourceLanguageManager.LanguageFrmMain, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultTabpageFocus()
        {
            try
            {
                if (this.ribbonMain.Pages.Count > 1)
                {
                    bool selectFirstPage = false;
                    for (int i = 0; i < this.ribbonMain.Pages.Count; i++)
                    {
                        string tagCode = (this.ribbonMain.Pages[i].Tag ?? "").ToString();
                        if (!String.IsNullOrEmpty(tagCode)
                            && (tagCode.Contains(String.Format("__{0}__", IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__BUONG)) || tagCode.Contains(String.Format("__{0}__", IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__XL))))
                        {
                            this.ribbonMain.SelectedPage = this.ribbonMain.Pages[i];
                            selectFirstPage = true;
                            //tagCode:"ModuleTypeCode__InRole__B01__4__"___
                            //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => tagCode), tagCode));
                            //string[] seperator = { "__" };
                            //var arrButtonSplitNames = tagCode.Split(seperator, StringSplitOptions.RemoveEmptyEntries);
                            //if (arrButtonSplitNames != null && arrButtonSplitNames.Length > 2)
                            //{
                            //    long roomId = Inventec.Common.TypeConvert.Parse.ToInt64(arrButtonSplitNames[1]);
                            //    long roomTypeId = Inventec.Common.TypeConvert.Parse.ToInt64(arrButtonSplitNames[2]);
                            //    RichEditorConfig.WorkingRoom = BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.ID == roomId).FirstOrDefault();
                            //}
                            break;
                        }
                    }
                    if (!selectFirstPage)
                        this.ribbonMain.SelectedPage = this.ribbonMain.Pages[1];
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void LogoutAndResetToDefault()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("LogoutAndResetToDefault.1");
                if (!Inventec.Desktop.Common.Token.TokenManager.Logout())
                    Inventec.Common.Logging.LogSystem.Debug(TabWorker.formClosing);

                System.Diagnostics.Process.Start(Application.ExecutablePath);
                GlobalVariables.isLogouter = true;
                GlobalVariables.IsLostToken = true;
                //close this one
                Inventec.Common.Logging.LogSystem.Info("LogoutAndResetToDefault.2");
                System.Diagnostics.Process.GetCurrentProcess().Kill();



                //CloseHostWcfAssignPrescriptionByCFG();

                //this.CallRemoveTokenData();

                //try
                //{
                //    HIS.Desktop.Modules.Login.frmLogin frmLogin = new Login.frmLogin();
                //    frmLogin.Show();
                //}
                //catch (Exception exx)
                //{
                //    Inventec.Common.Logging.LogSystem.Error(exx);
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async void CallRemoveTokenData()
        {
            try
            {
                CommonParam paramCommon = new CommonParam();
                var result = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).PostAsync<bool>("api/HisAccountBook/Cancel", ApiConsumers.MosConsumer, null, paramCommon);
                if (result)
                {
                    Inventec.Common.Logging.LogSystem.Debug("LogoutAndResetToDefault ==> Goi api huy yeu cau su dung hoa don thanh cong");
                }
                else
                {
                    Inventec.Common.Logging.LogSystem.Warn("LogoutAndResetToDefault ==> Goi api huy yeu cau su dung hoa don that bai____Api uri:api/HisAccountBook/Cancel____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => result), result));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessAfterChangePasswordSuccess()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("ProcessAfterChangePasswordSuccess. 1");
                this.LogoutAndResetToDefault();
                Inventec.Common.Logging.LogSystem.Info("ProcessAfterChangePasswordSuccess. 2");
                //this.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ShowModuleSelectedRoom()
        {
            try
            {
                WaitingManager.Show();

                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ChooseRoom").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.ChooseRoom'");
                if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'HIS.Desktop.Plugins.ChooseRoom' is not plugins");

                List<object> listArgs = new List<object>();
                listArgs.Add((HIS.Desktop.Common.RefeshReference)RefeshReference);
                var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, moduleData.RoomId, moduleData.RoomTypeId), listArgs);
                if (extenceInstance == null) throw new ArgumentNullException("Khoi tao moduleData that bai. extenceInstance = null");

                WaitingManager.Hide();
                ((Form)extenceInstance).ShowDialog(this);
            }
            catch (NullReferenceException ex)
            {
                WaitingManager.Hide();
                MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBKhongTimThayPluginsCuaChucNangNay), HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBKhongTimThayPluginsCuaChucNangNay), HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void RefeshReference()
        {
            try
            {
                //Dong tat ca cac tab page dang mo, dong thoi mo 1 page default
                ResetAllTabpageToDefault();
                InitStatusBar();
                txtRoomSelected.Text = WorkPlace.GetRoomId() + "__" + WorkPlace.GetRoomTypeIds().FirstOrDefault();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void CloseRightTabProcess(XtraTabControl sender)
        {
            try
            {
                if (sender != null)
                {
                    XtraTabPage currentTab = null;
                    XtraTabPageCollection tabCollection = new XtraTabPageCollection(sender);
                    foreach (XtraTabPage item in sender.TabPages)
                    {
                        tabCollection.Add(item);
                        var index = sender.TabPages.IndexOf(item);
                        if (index == sender.SelectedTabPageIndex)
                        {
                            currentTab = item;
                        }
                    }

                    if (tabCollection != null && currentTab != null)
                    {
                        foreach (XtraTabPage item in tabCollection)
                        {
                            var currentTabIndex = sender.TabPages.IndexOf(currentTab);
                            var index = sender.TabPages.IndexOf(item);
                            if (index > currentTabIndex)
                            {
                                sender.SelectedTabPageIndex = index;
                                //CloseButtonHandlerClick(item, sender);
                                TabControlBaseProcess.CloseButtonHandlerClick(item, sender, LoadGridMenuTabPageExt);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CloseOtherTabProcess(XtraTabControl sender)
        {
            try
            {
                if (sender != null)
                {
                    XtraTabPage currentTab = null;
                    XtraTabPageCollection tabCollection = new XtraTabPageCollection(sender);
                    foreach (XtraTabPage item in sender.TabPages)
                    {
                        tabCollection.Add(item);
                        var index = sender.TabPages.IndexOf(item);
                        if (index == sender.SelectedTabPageIndex)
                        {
                            currentTab = item;
                        }
                    }

                    if (tabCollection != null && currentTab != null)
                    {
                        foreach (XtraTabPage item in tabCollection)
                        {
                            var currentTabIndex = sender.TabPages.IndexOf(currentTab);
                            var index = sender.TabPages.IndexOf(item);
                            if (index != currentTabIndex)
                            {
                                sender.SelectedTabPageIndex = index;
                                TabControlBaseProcess.CloseButtonHandlerClick(item, sender, LoadGridMenuTabPageExt);
                                //CloseButtonHandlerClick(item, sender);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        Inventec.Desktop.Common.Modules.Module CreateModuleData(Inventec.Desktop.Common.Modules.Module data, long roomId, long roomTypeId)
        {
            Inventec.Desktop.Common.Modules.Module result = new Inventec.Desktop.Common.Modules.Module();
            try
            {
                if (data == null) throw new ArgumentNullException("data");

                result.Children = data.Children;
                result.Description = data.Description;
                result.ExtensionInfo = data.ExtensionInfo;
                result.Icon = data.Icon;
                result.ImageIndex = data.ImageIndex;
                result.IsPlugin = data.IsPlugin;
                result.leaf = data.leaf;
                result.ModuleCode = data.ModuleCode;
                result.ModuleLink = data.ModuleLink;
                result.ModuleTypeId = data.ModuleTypeId;
                result.NumberOrder = data.NumberOrder;
                result.PageGroupCaption = data.PageGroupCaption;
                result.PageGroupName = data.PageGroupName;
                result.ParentCode = data.ParentCode;
                result.PluginInfo = data.PluginInfo;
                result.RoomId = roomId;
                result.RoomTypeId = roomTypeId;
                result.text = data.text;
                result.Visible = data.Visible;
                result.IsNotShowDialog = data.IsNotShowDialog;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }

            return result;
        }

        #endregion

        #region Public Method

        /// <summary>
        /// Gets the <see cref="TabControlMain"/> object that manages workspace tab groups.
        /// </summary>
        public DevExpress.XtraTab.XtraTabControl TabControlMain
        {
            get { return tabControlMain; }
        }

        /// <summary>
        /// Function init call after new formMain
        /// Set language, generate menu
        /// </summary>
        public void InitMultipleThread()
        {
            try
            {
                List<Action> methods = new List<Action>();
                methods.Add(this.LoadKeysFromlanguage);
                methods.Add(this.InitMenuFontSize);
                methods.Add(this.GenerateMenuInTopBar);
                methods.Add(this.ProcessAuthoriButton);
                methods.Add(this.InitStatusBar);
                methods.Add(this.InitMenuLanguage);
                methods.Add(this.MpsPrinterLoadFactory);
                ThreadCustomManager.MultipleThreadWithJoin(methods);

                //Set shortcut menu ship
                var currentModules = GlobalVariables.currentModules.Where(o => o.Visible).ToList();
                this.BuildToolStrip(GlobalVariables.currentModules);
            }
            catch (Exception ex)
            {
                LogSystem.Fatal(ex);
            }
        }

        /// <summary>
        /// Change room
        /// generate menu, close all tabpage opened and select tabpage working table
        /// </summary>
        public void ChooseRoomType()
        {
            try
            {
                this.LoadKeysFromlanguage();

                //new Base.Menu().Init();

                //Choose room type -> visible all tabpage, show tabpage default
                new Base.Menu().GenerateMenu(this.ribbonMain, this.imageCollection32, imageCollection16);

                //Close all tabpage has opened except tabpage working tab page
                this.CloseAllTabpage(this.TabControlMain);

                //Set shortcut menu ship
                var currentModules = GlobalVariables.currentModules.Where(o => o.Visible).ToList();
                this.BuildToolStrip(currentModules);

                //Set default tabcontrol
                this.SetDefaultTabpageFocus();
                this.InitStatusBar();
                //this.InitMenuLanguage();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        //Sau khi goi module chon phong, chon xong, ham nay se duoc goi
        //Gen lại thanh menu theo phong duoc chọn
        public void ResetAllTabpageToDefault()
        {
            try
            {
                Base.Menu menu = new Base.Menu();

                //Choose room type -> visible all tabpage not in room selected
                menu.ProcessPageInRoomRemoved(ribbonMain, tabControlMain);
                menu.GenerateMenu(this.ribbonMain, this.imageCollection32, imageCollection16);
                LoadGridMenuTabPageExt();
                CreateApplicationMenu();
                SetDefaultTabpageFocus();
                MPS.ProcessorBase.PrintConfig.MediOrgCode = HIS.Desktop.LocalStorage.HisConfig.HisMediOrgCFG.MEDI_ORG_VALUE__CURRENT;
                MPS.ProcessorBase.PrintConfig.OrganizationName = HIS.Desktop.LocalStorage.HisConfig.HisMediOrgCFG.ORGANIZATION_NAME;
                MPS.ProcessorBase.PrintConfig.ParentOrganizationName = HIS.Desktop.LocalStorage.HisConfig.HisMediOrgCFG.PARENT_ORGANIZATION_NAME;
                MPS.ProcessorBase.PrintConfig.OrganizationAddress = HIS.Desktop.LocalStorage.HisConfig.HisMediOrgCFG.ORGANIZATION_ADDRESS;
                if (HIS.Desktop.LocalStorage.HisConfig.HisMediOrgCFG.ORGANIZATION_LOGO != null)
                {
                    MPS.ProcessorBase.PrintConfig.OrganizationLogo = Inventec.Common.FlexCellExport.Common.ConverterImageToArray(HIS.Desktop.LocalStorage.HisConfig.HisMediOrgCFG.ORGANIZATION_LOGO);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        public void LoadGridMenuTabPageExt1()
        {
            try
            {
                gridViewMenuTabpage.GridControl.BeginUpdate();
                List<MenuTabPageExtADO> menuTabPageExts = new List<MenuTabPageExtADO>();
                if (TabControlMain.TabPages.Count > 0)
                {
                    foreach (XtraTabPage item in TabControlMain.TabPages)
                    {
                        MenuTabPageExtADO menuTabPage = new MenuTabPageExtADO();
                        menuTabPage.Text = item.Text;
                        menuTabPage.Name = item.Name;
                        if (item.Tag != null && item.Tag is Inventec.Desktop.Common.Modules.Module)
                        {
                            menuTabPage.ModuleData = item.Tag as Inventec.Desktop.Common.Modules.Module;
                        }

                        menuTabPageExts.Add(menuTabPage);
                    }
                }
                gridViewMenuTabpage.GridControl.DataSource = menuTabPageExts.OrderBy(o => o.Text).ToList();
                gridViewMenuTabpage.GridControl.EndUpdate();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// CLose all tabpage of tabcontrol main except workingTable tabpage
        /// </summary>
        /// <param name="tabControlName"></param>
        internal void CloseAllTabpage(XtraTabControl tabControlName)
        {
            try
            {
                while (tabControlName.TabPages.Count > 0)
                {
                    TabControlBaseProcess.CloseButtonHandlerClick(tabControlName.TabPages[tabControlName.TabPages.Count - 1], tabControlName, LoadGridMenuTabPageExt);
                    //CloseButtonHandlerClick(tabControlName.TabPages[tabControlName.TabPages.Count - 1], tabControlName);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void ReloadTextTabpageExecuteServiceByPatient(string headerPageText)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => headerPageText), headerPageText));
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("TabControlMain.SelectedTabPage.Text", TabControlMain.SelectedTabPage.Text));
                TabControlMain.SelectedTabPage.Text = headerPageText;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessAutoOpenModuleLink()
        {
            try
            {
                if (WorkPlace.WorkPlaceSDO != null && WorkPlace.WorkPlaceSDO.Count == 1)
                {
                    System.Threading.Thread.Sleep(1000);

                    string moduleLinks = ConfigApplicationWorker.Get<string>(AppConfigKeys.CONFIG_KEY__HIS_DESKTOP__AUTO_RUN_MODULE_LINKS_CFG);
                    if (!String.IsNullOrWhiteSpace(moduleLinks))
                    {
                        List<HIS_ROOM_TYPE_MODULE> roomTypeModules = HIS.Desktop.Base.DATA.RoomTypeModules.Where(o => o.ROOM_TYPE_ID == WorkPlace.WorkPlaceSDO.First().RoomTypeId).ToList();
                        if (roomTypeModules == null || roomTypeModules.Count <= 0)
                        {
                            return;
                        }

                        string[] moduleLink = moduleLinks.Split(';');
                        foreach (var item in moduleLink)
                        {
                            System.Threading.Thread.Sleep(500);
                            //kiểm tra thiết lập loại phòng chức năng
                            if (!roomTypeModules.Exists(o => o.MODULE_LINK == item))
                            {
                                continue;
                            }

                            Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.FirstOrDefault(o => o.ModuleLink == item);
                            if (moduleData != null)
                            {
                                Module addmodule = PluginInstance.GetModuleWithWorkingRoom(moduleData, WorkPlace.WorkPlaceSDO.First().RoomId, WorkPlace.WorkPlaceSDO.First().RoomTypeId);

                                List<object> listArgs = new List<object>();
                                listArgs.Add(addmodule);

                                var extenceInstance = PluginInstance.GetPluginInstance(addmodule, listArgs);
                                if (extenceInstance == null)
                                {
                                    Inventec.Common.Logging.LogSystem.Error("extenceInstance is null");
                                    continue;
                                }

                                // Luôn dùng Show với form vì có thể mở nhiều popup
                                switch (moduleData.ModuleTypeId)
                                {
                                    case Inventec.Desktop.Common.Modules.Module.MODULE_TYPE_ID__FORM:
                                        ((System.Windows.Forms.Form)extenceInstance).Show(this);
                                        break;
                                    case Inventec.Desktop.Common.Modules.Module.MODULE_TYPE_ID__UC:
                                        TabControlBaseProcess.TabCreating(SessionManager.GetTabControlMain(), moduleData.ExtensionInfo.Code, moduleData.text, (System.Windows.Forms.UserControl)extenceInstance, addmodule);
                                        break;
                                    case Inventec.Desktop.Common.Modules.Module.MODULE_TYPE_ID__COMBO:
                                        if (extenceInstance is System.Windows.Forms.UserControl)
                                        {
                                            TabControlBaseProcess.TabCreating(SessionManager.GetTabControlMain(), moduleData.ExtensionInfo.Code, moduleData.text, (System.Windows.Forms.UserControl)extenceInstance, addmodule);
                                        }
                                        else if (extenceInstance is System.Windows.Forms.Form)
                                        {
                                            ((System.Windows.Forms.Form)extenceInstance).Show(this);
                                        }
                                        break;
                                    default:
                                        if (extenceInstance is System.Windows.Forms.UserControl)
                                        {
                                            TabControlBaseProcess.TabCreating(SessionManager.GetTabControlMain(), moduleData.ExtensionInfo.Code, moduleData.text, (System.Windows.Forms.UserControl)extenceInstance, addmodule);
                                        }
                                        else if (extenceInstance is System.Windows.Forms.Form)
                                        {
                                            ((System.Windows.Forms.Form)extenceInstance).Show(this);
                                        }
                                        break;
                                }
                            }
                            else
                            {
                                Inventec.Common.Logging.LogSystem.Error("Không tìm thấy module ứng với module link: " + item);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Status bar
        public void InitStatusBar()
        {
            try
            {
                string username = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
                string loginname = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                lblLoginName.Caption = (!String.IsNullOrEmpty(username) ? String.Format("{0} ({1})", loginname, username) : loginname);
                lblIP.Caption = Inventec.Common.WebApiClient.IpAddressUtils.GetIpAddressLocal();
                lblBranchName.Caption = (String.IsNullOrEmpty(WorkPlace.GetBranchName()) ? "" : WorkPlace.GetBranchName().ToUpper());
                lblRoomName.Caption = WorkPlace.GetRoomNames();
                lblRoomName.Visibility = BarItemVisibility.Never;
                lblIP.Visibility = BarItemVisibility.Always;
                lblBranchName.Visibility = (String.IsNullOrEmpty(WorkPlace.GetBranchName()) ? BarItemVisibility.Never : BarItemVisibility.Always);
                string updateStatus = IsAutoUpgrateState() ? "" : " (nâng cấp lỗi)";
                string versionContent = File.Exists(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, "readme.txt")) ? System.IO.File.ReadAllText(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, "readme.txt")) : "";
                bbtnVersionInfo.Caption = String.Format("{0} {1}", versionContent, updateStatus);
                this.bbtnVersionInfo.ItemAppearance.Normal.ForeColor = IsAutoUpgrateState() ? System.Drawing.Color.Black : System.Drawing.Color.Red;

                lblTokenTimeout.Caption = String.Format(ResourceCommon.PhienLamViecConHieuLucDen, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetTokenData().ExpireTime.ToString("dd/MM/yyyy HH:mm"));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        bool IsAutoUpgrateState()
        {
            bool success = true;
            try
            {
                string aupVersion = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HisConfigKeys.CONFIG_KEY__RUN_AUP_VERSION);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => aupVersion), aupVersion));
                if (String.IsNullOrWhiteSpace(aupVersion) || aupVersion == "v1")
                {
                    if (File.Exists(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, "readme.txt")) && File.Exists(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, "readmebk.txt")))
                    {
                        string readmeContent = System.IO.File.ReadAllText(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, "readme.txt"));
                        string readmeBKContent = System.IO.File.ReadAllText(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, "readmebk.txt"));
                        success = readmeContent == readmeBKContent;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return success;
        }

        private string GetCurrentDatetime()
        {
            try
            {
                string ngay = string.Format("{0:00}", DateTime.Now.Day);
                string thang = string.Format("{0:00}", DateTime.Now.Month);
                string nam = DateTime.Now.Year.ToString();
                string gio = string.Format("{0:00}", DateTime.Now.Hour);
                string phut = string.Format("{0:00}", DateTime.Now.Minute);
                string giay = string.Format("{0:00}", DateTime.Now.Second);
                return "" + ngay + "/" + thang + "/" + nam + " " + gio + ":" + phut + ":" + giay + "";
            }
            catch { }

            return "";
        }

        private void timerApplicationRuntime_Tick(object sender, EventArgs e)
        {
            try
            {
                CreateThreadUpdateApplicationTime();
                CreateThreadUpdateStateVoiceCommeand();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateStateVoiceCommeandExecute()
        {
            try
            {
                if (VoiceCommandData.isUseVoiceCommand && bbtnSpeechVoice.Visibility != BarItemVisibility.Always)
                {
                    bbtnSpeechVoice.Visibility = BarItemVisibility.Always;
                }
                else if (!VoiceCommandData.isUseVoiceCommand && bbtnSpeechVoice.Visibility != BarItemVisibility.Never)
                {
                    bbtnSpeechVoice.Visibility = BarItemVisibility.Never;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CreateThreadUpdateStateVoiceCommeand()
        {
            try
            {
                Task.Factory.StartNew(UpdateStateVoiceCommeandNewThread);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateStateVoiceCommeandNewThread()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate { UpdateStateVoiceCommeandExecute(); }));
                }
                else
                {
                    UpdateStateVoiceCommeandExecute();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateApplicationTime()
        {
            try
            {
                lblApplicationRuntime.Caption = GetCurrentDatetime();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CreateThreadUpdateApplicationTime()
        {
            try
            {
                Task.Factory.StartNew(UpdateApplicationTimeNewThread);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateApplicationTimeNewThread()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate { UpdateApplicationTime(); }));
                }
                else
                {
                    UpdateApplicationTime();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Language
        private void InitMenuLanguage()
        {
            try
            {
                if (!String.IsNullOrEmpty(LanguageManager.GetLanguage()))
                {
                    lblLanguage.Caption = lblLanguage.Hint = (LanguageManager.GetLanguage() == LanguageManager.GetLanguageVi() ? ResourceCommon.Lang__Caption__Vi : (LanguageManager.GetLanguage() == LanguageManager.GetLanguageEn() ? ResourceCommon.Lang__Caption__En : ResourceCommon.Lang__Caption__My));
                }
                else
                {
                    LanguageManager.Change(LanguageManager.LanguageEnum.VI);
                    lblLanguage.Caption = lblLanguage.Hint = ResourceCommon.Lang__Caption__Vi;
                }

                InitLanguagesMenu();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        LanguageManager.LanguageEnum GetCommonLanguageEnum(string language)
        {
            LanguageManager.LanguageEnum languageEnum = LanguageManager.LanguageEnum.VI;
            if (language == LanguageManager.LanguageVi)
            {
                languageEnum = LanguageManager.LanguageEnum.VI;
            }
            else if (language == LanguageManager.LanguageEn)
            {
                languageEnum = LanguageManager.LanguageEnum.EN;
            }
            else if (language == LanguageManager.LanguageMy)
            {
                languageEnum = LanguageManager.LanguageEnum.MY;
            }
            else
            {
                languageEnum = LanguageManager.LanguageEnum.VI;
            }
            return languageEnum;
        }

        private void InitLanguagesMenu()
        {
            try
            {
                lblLanguage.ClearLinks();
                //ribbonStatusBar.ForceInitialize();
                BarButtonItem item1 = new BarButtonItem();
                item1.Caption = ResourceCommon.Lang__Caption__Vi;
                item1.ImageIndex = 1;
                item1.Tag = LanguageManager.LanguageEnum.VI;
                lblLanguage.AddItem(item1);
                item1.ItemClick += new ItemClickEventHandler(OnlanguageClick);

                BarButtonItem item2 = new BarButtonItem();
                item2.Caption = ResourceCommon.Lang__Caption__En;
                item2.ImageIndex = 0;
                item2.Tag = LanguageManager.LanguageEnum.EN;
                lblLanguage.AddItem(item2);
                item2.ItemClick += new ItemClickEventHandler(OnlanguageClick);

                BarButtonItem item3 = new BarButtonItem();
                item3.Caption = ResourceCommon.Lang__Caption__My;
                item3.ImageIndex = 0;
                item3.Tag = LanguageManager.LanguageEnum.MY;
                lblLanguage.AddItem(item3);
                item3.ItemClick += new ItemClickEventHandler(OnlanguageClick);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void OnlanguageClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                lblLanguage.Caption = lblLanguage.Hint = e.Item.Caption;
                lblLanguage.Hint = lblLanguage.Caption;
                lblLanguage.ImageIndex = 1;
                LanguageManager.LanguageEnum langEnum = LanguageManager.LanguageEnum.VI;
                if (e.Item.Tag != null)
                    langEnum = (LanguageManager.LanguageEnum)(e.Item.Tag);
                LanguageManager.Change(langEnum);

                ChooseRoomType();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Font size
        private void InitMenuFontSize()
        {
            try
            {
                if (HIS.Desktop.ApplicationFont.ApplicationFontWorker.GetFontSize() > 0)
                {
                    bsiChooseFont.Caption = bsiChooseFont.Hint = (HIS.Desktop.ApplicationFont.ApplicationFontWorker.GetFontSize() / 100) + "";
                    HIS.Desktop.ApplicationFont.ApplicationFontWorker.ChangeFontSize(HIS.Desktop.ApplicationFont.ApplicationFontWorker.GetFontSize());
                }
                else
                {
                    HIS.Desktop.ApplicationFont.ApplicationFontWorker.ChangeFontSize(HIS.Desktop.ApplicationFont.ApplicationFontConfig.FontSize825);
                    bsiChooseFont.Caption = bsiChooseFont.Hint = HIS.Desktop.ApplicationFont.ApplicationFontWorker.GetFontSize() + "";
                }

                InitFontSizeMenu();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitFontSizeMenu()
        {
            try
            {
                ribbonMain.Manager.ForceInitialize();
                var fSizes = HIS.Desktop.ApplicationFont.ApplicationFontWorker.GetAllFontSize();
                if (fSizes != null && fSizes.Count > 0)
                {
                    foreach (var fs in fSizes)
                    {
                        BarButtonItem item1 = new BarButtonItem(ribbonMain.Manager, (fs / 100) + "");
                        item1.Tag = fs;
                        bsiChooseFont.AddItem(item1);
                        item1.ItemClick += new ItemClickEventHandler(OnChangeFontSizeClick);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void OnChangeFontSizeClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                bsiChooseFont.Caption = bsiChooseFont.Hint = e.Item.Caption;
                float fontSize = HIS.Desktop.ApplicationFont.ApplicationFontConfig.FontSize825;
                if (e.Item.Tag != null)
                    fontSize = (float)(e.Item.Tag);
                HIS.Desktop.ApplicationFont.ApplicationFontWorker.ChangeFontSize(fontSize);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Button click
        private void bbtnClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("bbtnClose_ItemClick.1");
                if (MessageBox.Show(HIS.Desktop.Resources.ResourceCommon.ThongBaoBanCoMuonThoatPhanMem, HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    var watch = System.Diagnostics.Stopwatch.StartNew();
                    ResetClientCacheFile();

                    if (!Inventec.Desktop.Common.Token.TokenManager.Logout())
                        Inventec.Common.Logging.LogSystem.Warn(TabWorker.formClosing);

                    //this.LogoutAndResetToDefault();

                    // Thời gian kết thúc
                    watch.Stop();
                    Inventec.Common.Logging.LogAction.Info(String.Format("{0}____{1}____{2}____{3}____{4}____{5}____{6}____{7}", GlobalVariables.APPLICATION_CODE, GlobalString.VersionApp, (double)((double)watch.ElapsedMilliseconds / (double)1000), "HIS.Desktop", "LogoutApp", Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName(), StringUtil.GetIpLocal(), StringUtil.CustomerCode));


                    System.Diagnostics.Process.Start(Application.ExecutablePath);
                    GlobalVariables.isLogouter = true;
                    GlobalVariables.IsLostToken = true;
                    //close this one
                    Inventec.Common.Logging.LogSystem.Info("bbtnClose_ItemClick.2");
                    System.Diagnostics.Process.GetCurrentProcess().Kill();


                    //if (GlobalVariables.IsLoginAgain)
                    //{
                    //    Inventec.Common.Logging.LogSystem.Debug("bbtnClose_ItemClick.1.1");
                    //    GlobalVariables.IsLostToken = true;
                    //    GlobalVariables.isLogouter = true;
                    //    Inventec.Common.Logging.LogSystem.Debug("bbtnClose_ItemClick.1.2");
                    //}
                }
                Inventec.Common.Logging.LogSystem.Info("bbtnClose_ItemClick.3");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnReportTypeList_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ReportAll").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.ReportAll'");
                if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'HIS.Desktop.Plugins.ReportAll' is not plugins");

                List<object> listArgs = new List<object>();

                string listApiJson = "";
                var tokenData = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetTokenData();
                listApiJson += "{";
                listApiJson += String.Format("\"application_code\":\"{0}\",", GlobalVariables.APPLICATION_CODE);
                listApiJson += String.Format("\"token_code\":\"{0}\",", tokenData != null ? tokenData.TokenCode : "null");
                listApiJson += String.Format("\"uri_api_acs\":\"{0}\",", ConfigSystems.URI_API_ACS);
                listApiJson += String.Format("\"uri_api_mos\":\"{0}\",", ConfigSystems.URI_API_MOS);
                listApiJson += String.Format("\"uri_api_mrs\":\"{0}\",", ConfigSystems.URI_API_MRS);
                listApiJson += String.Format("\"uri_api_sar\":\"{0}\",", ConfigSystems.URI_API_SAR);
                listApiJson += String.Format("\"uri_api_sda\":\"{0}\"", ConfigSystems.URI_API_SDA);
                listApiJson += "}";

                listArgs.Add(listApiJson);
                var moduleWK = PluginInstance.GetModuleWithWorkingRoom(moduleData, moduleData.RoomId, moduleData.RoomTypeId);
                var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(moduleWK, listArgs);
                if (extenceInstance == null) throw new ArgumentNullException("Khoi tao moduleData that bai. extenceInstance = null");

                WaitingManager.Hide();
                TabControlBaseProcess.TabCreating(tabControlMain, "HIS.Desktop.Plugins.ReportAll", moduleData.text, (System.Windows.Forms.UserControl)extenceInstance, moduleWK);
            }
            catch (NullReferenceException ex)
            {
                WaitingManager.Hide();
                MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBKhongTimThayPluginsCuaChucNangNay), HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBKhongTimThayPluginsCuaChucNangNay), HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnChangePassword_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                List<object> results = new List<object>();
                results.Add(ApiConsumers.SdaConsumer);
                results.Add((Inventec.UC.ChangePassword.HasExceptionApi)new PluginInstanceBehavior().HasExceptionApi);
                results.Add("APP.ico");
                results.Add((Inventec.Desktop.Plugins.ChangePassword.ChangePasswordSuccessDelegate)ProcessAfterChangePasswordSuccess);
                Inventec.Desktop.Plugins.ChangePassword.ChangePasswordProcessor changePasswordProcessor = new Inventec.Desktop.Plugins.ChangePassword.ChangePasswordProcessor();
                var extenceInstance = changePasswordProcessor.Run(results.ToArray()) as Form;
                if (extenceInstance != null)
                {
                    extenceInstance.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnConfigApplication_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                List<object> results = new List<object>();
                results.Add((Inventec.UC.ConfigApplication.Refesh)new PluginInstanceBehavior().RefeshDataConfigApplication);

                Inventec.Desktop.Plugins.ConfigApplication.ConfigApplicationProcessor configApplicationProcessor = new Inventec.Desktop.Plugins.ConfigApplication.ConfigApplicationProcessor();
                var extenceInstance = configApplicationProcessor.Run(results.ToArray());
                Module module = new Module();
                module.ModuleCode = "Inventec.Desktop.Plugins.ConfigApplication";
                module.ModuleLink = "Inventec.Desktop.Plugins.ConfigApplication";
                module.text = Inventec.Common.Resource.Get.Value("frmMain.bbtnConfigApplication.Caption", Resources.ResourceLanguageManager.LanguageFrmMain, LanguageManager.GetCulture());
                TabControlBaseProcess.TabCreating(tabControlMain, "Inventec.Desktop.Plugins.ConfigApplication", module.text, (System.Windows.Forms.UserControl)extenceInstance, module);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnEventLog_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.EventLog").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.EventLog'");
                if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'HIS.Desktop.Plugins.EventLog' is not plugins");

                List<object> listArgs = new List<object>();
                Inventec.UC.EventLogControl.Data.DataInit3 dataInit3 = new Inventec.UC.EventLogControl.Data.DataInit3(ConfigSystems.URI_API_SDA, GlobalVariables.APPLICATION_CODE, ConfigApplications.NumPageSize, "");

                listArgs.Add(dataInit3);
                listArgs.Add(Inventec.Desktop.Common.Modules.Module.MODULE_TYPE_ID__UC);

                var moduleWK = PluginInstance.GetModuleWithWorkingRoom(moduleData, moduleData.RoomId, moduleData.RoomTypeId);
                var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(moduleWK, listArgs);
                if (extenceInstance == null) throw new ArgumentNullException("Khoi tao moduleData that bai. extenceInstance = null");

                WaitingManager.Hide();
                TabControlBaseProcess.TabCreating(tabControlMain, "HIS.Desktop.Plugins.EventLog", moduleData.text, (System.Windows.Forms.UserControl)extenceInstance, moduleWK);
            }
            catch (NullReferenceException ex)
            {
                WaitingManager.Hide();
                MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBKhongTimThayPluginsCuaChucNangNay), HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBKhongTimThayPluginsCuaChucNangNay), HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnPlugin_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                List<object> results = new List<object>();
                Inventec.Desktop.Plugins.Plugin.PluginProcessor pluginProcessor = new Inventec.Desktop.Plugins.Plugin.PluginProcessor();
                var extenceInstance = pluginProcessor.Run(results.ToArray()) as Form;
                if (extenceInstance != null)
                {
                    extenceInstance.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnVersionInfo_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                //if (!IsAutoUpgrateState())
                //{
                HIS.Desktop.Modules.WaitingForm.frmUpgradeOption frmUpgradeOption = new HIS.Desktop.Modules.WaitingForm.frmUpgradeOption();
                frmUpgradeOption.ShowDialog();
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBXuatHienExceptionChuaKiemDuocSoat), HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnCommonChooseRoom_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                ShowModuleSelectedRoom();

                //làm việc tại 1 phòng hoặc chọn 1 phòng
                ProcessAutoOpenModuleLink();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnRefreshCache_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                List<object> results = new List<object>();
                HIS.Desktop.Plugins.LocalCacheManager.LocalCacheManagerProcessor localCacheProcessor = new HIS.Desktop.Plugins.LocalCacheManager.LocalCacheManagerProcessor();
                var extenceInstance = localCacheProcessor.Run(results.ToArray()) as Form;
                if (extenceInstance != null)
                {
                    extenceInstance.ShowDialog();
                }
            }
            catch (NullReferenceException ex)
            {
                MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBKhongTimThayPluginsCuaChucNangNay), HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            catch (Exception ex)
            {
                MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBKhongTimThayPluginsCuaChucNangNay), HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnResetMosConfig_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                object data = null;
                bool success = true;

                string masterAddressUri = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HisConfigKeys.KEY__MASTER_ADDRESS);
                if (!String.IsNullOrEmpty(masterAddressUri))
                {
                    Inventec.Common.WebApiClient.ApiConsumer mosConsumer = new Inventec.Common.WebApiClient.ApiConsumer(masterAddressUri, GlobalVariables.APPLICATION_CODE);
                    mosConsumer.SetTokenCode(ApiConsumers.MosConsumer.GetTokenCode());
                    success = new BackendAdapter(param).Post<bool>(HisRequestUriStore.HIS_CONFIG_RESET, mosConsumer, data, param);
                }
                else
                {
                    success = new BackendAdapter(param).Post<bool>(HisRequestUriStore.HIS_CONFIG_RESET, ApiConsumers.MosConsumer, data, param);
                }

                CommonParam paramCache = new CommonParam();
                bool useRedisCacheServer = (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HisConfigKeys.CONFIG_KEY__HIS_IS_USE_REDIS_CACHE_SERVER) == "1");
                if (useRedisCacheServer)
                {
                    RdCacheSDO cacheSDO = new RdCacheSDO() { BRANCH_ID = BranchDataWorker.GetCurrentBranchId() };
                    success = new BackendAdapter(paramCache).Post<bool>(RDCACHE.URI.RdCache.POST_RELOAD_ALL, ApiConsumers.RdCacheConsumer, cacheSDO, paramCache) && success;
                }

                WaitingManager.Hide();
                if (paramCache != null && paramCache.Messages != null && paramCache.Messages.Count > 0)
                    param.Messages.AddRange(paramCache.Messages);
                if (paramCache != null && paramCache.BugCodes != null && paramCache.BugCodes.Count > 0)
                    param.BugCodes.AddRange(paramCache.BugCodes);
                MessageManager.Show(this, param, success);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBKQXLYCCuaFrontendThatBai), HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnResetLisConfig_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                object data = null;
                bool success = new BackendAdapter(param).Post<bool>(HisRequestUriStore.HIS_RESET_LIS_CONFIG, ApiConsumers.LisConsumer, data, param);
                WaitingManager.Hide();
                MessageManager.Show(this, param, success);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBKhongTimThayPluginsCuaChucNangNay), HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnTutorial_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                string link = "";
                if (!String.IsNullOrEmpty(GlobalVariables.CurrentTabPage))
                {
                    string[] seperator = { "_" };
                    var arrButtonSplitNames = GlobalVariables.CurrentTabPage.Split(seperator, StringSplitOptions.RemoveEmptyEntries);
                    if (arrButtonSplitNames != null && arrButtonSplitNames.Length > 0)
                    {
                        link = arrButtonSplitNames[0];
                    }
                }
                HIS.Desktop.Plugins.HisTutorial.Form1 frmTutorial = new Plugins.HisTutorial.Form1(link);
                frmTutorial.ShowDialog();
            }
            catch (NullReferenceException ex)
            {
                MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBKhongTimThayPluginsCuaChucNangNay), HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            catch (Exception ex)
            {
                MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBKhongTimThayPluginsCuaChucNangNay), HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                AcsUserUpdateLoginNameTDO userUpdateLoginNameTDO = new AcsUserUpdateLoginNameTDO();
                userUpdateLoginNameTDO.LoginName = "phuongdt";//TODO
                userUpdateLoginNameTDO.Password = "phuongdt";//TODO
                userUpdateLoginNameTDO.SubLoginName = "subphuongdt";//TODO
                CommonParam commonParam = new CommonParam();
                var rs = new BackendAdapter(commonParam).Post<bool>("/api/AcsUser/UpdateSubLoginname", ApiConsumer.ApiConsumers.AcsConsumer, userUpdateLoginNameTDO, commonParam);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void bbtnRefeshRamGC_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("GC.Collect. 1");
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                Inventec.Common.Logging.LogSystem.Info("GC.Collect. 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnSortcutRuntime_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("bbtnSortcutRuntime_ItemClick. 1");
                if ((!String.IsNullOrEmpty(textBox1.Text)))
                {
                    var base64EncodedBytes = System.Convert.FromBase64String(textBox1.Text);
                    string vlShortcut = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
                    //string vlShortcut = textBox1.Text;
                    string[] seperator = { "__" };
                    var arrButtonSplitNames = vlShortcut.Split(seperator, StringSplitOptions.RemoveEmptyEntries);
                    if (arrButtonSplitNames != null && arrButtonSplitNames.Count() > 1)
                    {
                        long roomId = long.Parse(arrButtonSplitNames[1]);
                        long roomTypeId = long.Parse(arrButtonSplitNames[2]);
                        Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == arrButtonSplitNames[0]).FirstOrDefault();
                        if (moduleData != null)
                        {
                            Inventec.Desktop.Common.Modules.Module module = CreateModuleData(moduleData, roomId, roomTypeId);
                            PluginInstanceBehavior.ShowModule(module, new PluginInstanceBehavior().AddItemIntoListArg(module.PluginInfo.AssemblyResolve));
                        }
                        else
                        {
                            Inventec.Common.Logging.LogSystem.Debug("Khong tim thay modulelink tuong ung");
                        }
                    }
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => arrButtonSplitNames), arrButtonSplitNames));
                }
                Inventec.Common.Logging.LogSystem.Debug("bbtnSortcutRuntime_ItemClick. 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem2_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                // là admin mới được mở chức năng này
                if (!GlobalVariables.AcsAuthorizeSDO.IsFull)
                {
                    return;
                }

                var page = (SessionManager.GetTabControlMain().TabPages[GlobalVariables.SelectedTabPageIndex]);
                if (page != null)
                {
                    UserControl container = null;
                    if (page.Controls.Count > 0)
                    {
                        var control = page.Controls[0];

                        if (control != null && control is UserControl)
                        {
                            container = page.Controls[0] as UserControl;
                        }
                        else if (control != null && control is XtraUserControl)
                        {
                            container = page.Controls[0] as XtraUserControl;
                        }
                    }
                    if (container != null)
                    {
                        ModuleControlProcess controlProcess = new ModuleControlProcess(true);
                        var ModuleControls = controlProcess.GetControls(container);
                        ModuleControls = ModuleControls != null ? ModuleControls.Where(o => !String.IsNullOrEmpty(o.ControlName)).Distinct().ToList() : null;
                        if (ModuleControls != null && ModuleControls.Count > 0)
                        {
                            WaitingManager.Show();
                            Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ModuleControlVisible").FirstOrDefault();
                            if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.ModuleControlVisible'");

                            List<object> listArgs = new List<object>();
                            listArgs.Add(ModuleControls);

                            if (GlobalVariables.CurrentModuleSelected != null)
                            {
                                SDA_HIDE_CONTROL hideControl = new SDA_HIDE_CONTROL();
                                hideControl.MODULE_LINK = GlobalVariables.CurrentModuleSelected.ModuleLink;
                                hideControl.APP_CODE = GlobalVariables.APPLICATION_CODE;
                                listArgs.Add(hideControl);

                                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => hideControl), hideControl));
                            }

                            //moduleData.ModuleTypeId = Inventec.Desktop.Common.Modules.Module.MODULE_TYPE_ID__FORM;
                            var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, 0, 0), listArgs);
                            if (extenceInstance == null) throw new NullReferenceException("extenceInstance is null");

                            WaitingManager.Hide();
                            ((System.Windows.Forms.Form)extenceInstance).ShowDialog();
                        }
                        else
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Khong lay duoc Modules theo link: HIS.Desktop.Plugins.ModuleControlVisible");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnCustomizeButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                // là admin mới được mở chức năng này
                if (!GlobalVariables.AcsAuthorizeSDO.IsFull)
                {
                    return;
                }

                var page = (SessionManager.GetTabControlMain().TabPages[GlobalVariables.SelectedTabPageIndex]);
                if (page != null)
                {
                    UserControl container = null;
                    if (page.Controls.Count > 0)
                    {
                        var control = page.Controls[0];

                        if (control != null && control is UserControl)
                        {
                            container = page.Controls[0] as UserControl;
                        }
                        else if (control != null && control is XtraUserControl)
                        {
                            container = page.Controls[0] as XtraUserControl;
                        }
                    }
                    if (container != null)
                    {
                        ModuleControlProcess controlProcess = new ModuleControlProcess();
                        var ModuleControls = controlProcess.GetControls(container);
                        ModuleControls = ModuleControls != null ? ModuleControls.Where(o => !String.IsNullOrEmpty(o.ControlName)).Distinct().ToList() : null;
                        if (ModuleControls != null && ModuleControls.Count > 0)
                        {
                            WaitingManager.Show();
                            Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ModuleButtonCustomize").FirstOrDefault();
                            if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.ModuleButtonCustomize'");

                            List<object> listArgs = new List<object>();
                            listArgs.Add(ModuleControls);

                            if (GlobalVariables.CurrentModuleSelected != null)
                            {
                                SDA_CUSTOMIZE_BUTTON customButton = new SDA_CUSTOMIZE_BUTTON();
                                customButton.MODULE_LINK = GlobalVariables.CurrentModuleSelected.ModuleLink;
                                customButton.APP_CODE = GlobalVariables.APPLICATION_CODE;
                                listArgs.Add(customButton);

                                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => customButton), customButton));
                            }

                            //moduleData.ModuleTypeId = Inventec.Desktop.Common.Modules.Module.MODULE_TYPE_ID__FORM;
                            var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, 0, 0), listArgs);
                            if (extenceInstance == null) throw new NullReferenceException("extenceInstance is null");

                            WaitingManager.Hide();
                            ((System.Windows.Forms.Form)extenceInstance).ShowDialog();
                        }
                        else
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Khong lay duoc Module theo link: HIS.Desktop.Plugins.ModuleButtonCustomize");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnHisConfig_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                //Open module ConfigAppUser
                Inventec.Desktop.Common.Modules.Module moduleAppConfigUser = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ConfigAppUser").FirstOrDefault();
                if (moduleAppConfigUser == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.ConfigAppUser'");
                moduleAppConfigUser.ModuleTypeId = Inventec.Desktop.Common.Modules.Module.MODULE_TYPE_ID__FORM;
                List<object> listArgsAppConfigUser = new List<object>();
                List<object> listArgs = new List<object>();
                if (GlobalVariables.CurrentModuleSelected != null)
                {
                    listArgs.Add(GlobalVariables.CurrentModuleSelected.ModuleLink);
                }
                long numpageSize = ConfigApplicationWorker.Get<long>("CONFIG_KEY__NUM_PAGESIZE") == 0 ? ConfigApplications.NumPageSize : ConfigApplicationWorker.Get<long>("CONFIG_KEY__NUM_PAGESIZE");
                listArgsAppConfigUser.Add(ApiConsumers.SdaConsumer);
                listArgsAppConfigUser.Add(numpageSize);
                listArgsAppConfigUser.Add((Action)new PluginInstanceBehavior().RefeshDataConfigApplication);
                listArgsAppConfigUser.Add(Inventec.Desktop.Common.Modules.Module.MODULE_TYPE_ID__FORM);

                List<string> listString = new List<string>();
                listString.Add(GlobalVariables.APPLICATION_CODE);
                if (GlobalVariables.CurrentModuleSelected != null)
                {
                    listString.Add(GlobalVariables.CurrentModuleSelected.ModuleLink);
                }

                listArgsAppConfigUser.Add(listString);

                //moduleAppConfigUser.ModuleTypeId = Inventec.Desktop.Common.Modules.Module.MODULE_TYPE_ID__FORM;
                var module = PluginInstance.GetModuleWithWorkingRoom(moduleAppConfigUser, 0, 0);

                var extenceInstanceAppConfigUser = PluginInstance.GetPluginInstance(module, listArgsAppConfigUser);
                if (extenceInstanceAppConfigUser == null) throw new NullReferenceException("extenceInstanceAppConfigUser is null");
                //TabControlBaseProcess.TabCreating(tabControlMain, "HIS.Desktop.Plugins.ConfigAppUser", moduleAppConfigUser.text, (System.Windows.Forms.UserControl)extenceInstanceAppConfigUser, module);
                ((System.Windows.Forms.Form)extenceInstanceAppConfigUser).Show();

                //Open module HisConfig
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisConfig").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.HisConfig'");


                var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, 0, 0), listArgs);
                if (extenceInstance == null) throw new NullReferenceException("extenceInstance is null");

                WaitingManager.Hide();

                ((System.Windows.Forms.Form)extenceInstance).Show();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void bbtnRemoteSupport_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                ProcessRemoteByRemoteDesktopSoftware();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        #endregion

        /// <summary>
        /// Change theme in gallery item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void skinRibbonGalleryBarItem1_GalleryItemClick(object sender, DevExpress.XtraBars.Ribbon.GalleryItemClickEventArgs e)
        {
            try
            {
                DevExpressTheme.THEME_NAME = e.Item.Caption;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }

        }

        private void gridViewMenuTabpage_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                var menuTabPage = (MenuTabPageExtADO)this.gridViewMenuTabpage.GetFocusedRow();
                if (menuTabPage != null)
                {
                    if (e.Column.FieldName == "Text")
                    {
                        this.tabControlMain.SelectedTabPage = this.tabControlMain.TabPages.FirstOrDefault(o => o.Name == menuTabPage.Name);
                        popupControlContainerOtherTabpage.HidePopup();
                    }
                    else if (e.Column.FieldName == "Del")
                    {
                        var page = this.tabControlMain.TabPages.FirstOrDefault(o => o.Name == menuTabPage.Name);
                        if (page != null)
                        {
                            TabControlBaseProcess.CloseButtonHandlerClick(page, TabControlMain, LoadGridMenuTabPageExt);
                            //popupControlContainerOtherTabpage.HidePopup();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void ribbonMain_SelectedPageChanged(object sender, EventArgs e)
        {
            try
            {
                if (tabControlMain.TabPages.Count > 0 && ribbonMain.SelectedPage.Tag != null)
                {
                    long roomTypeId = 0, roomId = 0;
                    string roomCode = "";
                    var arrTag = ribbonMain.SelectedPage.Tag.ToString().Split(new string[] { "__" }, StringSplitOptions.None);
                    roomTypeId = Inventec.Common.TypeConvert.Parse.ToInt64(arrTag.Length > 3 ? arrTag[3] : "");
                    roomCode = arrTag.Length > 3 ? arrTag[2] : "";
                    MOS.SDO.WorkPlaceSDO workPlace = String.IsNullOrEmpty(roomCode) ? null : HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.Where(o => o.RoomCode == roomCode && o.RoomTypeId == roomTypeId).FirstOrDefault();
                    roomId = workPlace != null ? workPlace.RoomId : 0;

                    for (int i = 0; i < tabControlMain.TabPages.Count; i++)
                    {
                        if (tabControlMain.TabPages[i].Tag != null && tabControlMain.TabPages[i].Tag is Inventec.Desktop.Common.Modules.Module)
                        {
                            Inventec.Desktop.Common.Modules.Module mdPage = tabControlMain.TabPages[i].Tag as Inventec.Desktop.Common.Modules.Module;
                            if (mdPage != null && mdPage.RoomTypeId == roomTypeId && mdPage.RoomId == roomId)
                            {
                                if (fontBoldGeneral == null || (fontBoldGeneral != null && tabControlMain.TabPages[i].Appearance.Header.Font.Name != fontBoldGeneral.Name && fontBoldGeneral.Size != tabControlMain.TabPages[i].Appearance.Header.Font.Size))
                                {
                                    fontBoldGeneral = new System.Drawing.Font(tabControlMain.TabPages[i].Appearance.Header.Font.Name, tabControlMain.TabPages[i].Appearance.Header.Font.Size, FontStyle.Bold);
                                }

                                tabControlMain.TabPages[i].Appearance.Header.ForeColor = Color.Green;
                                tabControlMain.TabPages[i].Appearance.Header.Font = fontBoldGeneral;
                            }
                            else
                            {
                                if (fontGeneral == null || (fontGeneral != null && tabControlMain.TabPages[i].Appearance.Header.Font.Name != fontGeneral.Name && fontGeneral.Size != tabControlMain.TabPages[i].Appearance.Header.Font.Size))
                                {
                                    fontGeneral = new System.Drawing.Font(tabControlMain.TabPages[i].Appearance.Header.Font.Name, tabControlMain.TabPages[i].Appearance.Header.Font.Size, FontStyle.Regular);
                                }

                                tabControlMain.TabPages[i].Appearance.Header.ForeColor = Color.Black;
                                tabControlMain.TabPages[i].Appearance.Header.Font = fontGeneral;
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

        /// <summary>
        /// Close one tabpage in tabcontrol
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabControlMain_CloseButtonClick(object sender, EventArgs e)
        {
            try
            {
                XtraTabPage page = (e as DevExpress.XtraTab.ViewInfo.ClosePageButtonEventArgs).Page as XtraTabPage;
                TabControlBaseProcess.CloseButtonHandlerClick(page, (XtraTabControl)sender, LoadGridMenuTabPageExt);
                //CloseButtonHandlerClick(page, (XtraTabControl)sender);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void tabControlMain_CustomHeaderButtonClick(object sender, DevExpress.XtraTab.ViewInfo.CustomHeaderButtonEventArgs e)
        {
            try
            {
                XtraTabControl tabControl = (XtraTabControl)sender;
                SkinTabControlViewInfo viewInfo = ((IXtraTab)tabControl).ViewInfo as SkinTabControlViewInfo;
                Rectangle buttonBounds = viewInfo.HeaderInfo.Buttons.GetButtonBounds(e.Button);
                popupControlContainerOtherTabpage.ShowPopup(tabControl.PointToScreen(new Point(buttonBounds.X, buttonBounds.Bottom)));
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Select tabpage
        /// Set tabpage name and tabpage index selected in ram
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabControlMain_SelectedPageChanged(object sender, TabPageChangedEventArgs e)
        {
            try
            {
                if (e.Page != null)
                {
                    GlobalVariables.CurrentTabPage = e.Page.Name;
                    XtraTabControl xtab = (XtraTabControl)sender;
                    if (xtab != null)
                    {
                        Inventec.Common.FlexCelPrint.Ado.SupportAdo.RemoteSupport = (() => barBtnSendReflection_ItemClick(null, null));
                        GlobalVariables.SelectedTabPageIndex = xtab.SelectedTabPageIndex;
                        var mod = (Inventec.Desktop.Common.Modules.Module)(xtab.TabPages[xtab.SelectedTabPageIndex].Tag);
                        if (mod != null)
                        {
                            GlobalVariables.CurrentModuleSelected = ButtonMenuProcessor.CreateModuleData(mod, mod.RoomId, mod.RoomTypeId);
                            RichEditorConfig.WorkingRoom = BackendDataWorker.Get<V_HIS_ROOM>().Where(o => o.ID == mod.RoomId).FirstOrDefault();
                            new PluginInstanceBehavior().FocusToControl();
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("WorkingRoom", RichEditorConfig.WorkingRoom));
                        }

                    }
                }
                else
                {
                    GlobalVariables.CurrentTabPage = "";
                    GlobalVariables.CurrentModuleSelected = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void tabControlMain_TabMiddleClick(object sender, PageEventArgs e)
        {
            try
            {
                XtraTabPage page = e.Page as XtraTabPage;
                TabControlBaseProcess.CloseButtonHandlerClick(page, (XtraTabControl)sender, LoadGridMenuTabPageExt);
                //CloseButtonHandlerClick(page, (XtraTabControl)sender);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void tabControlMain_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Right)
                {
                    XtraTabControl tabMain = (XtraTabControl)sender;
                    popupMenuProcessor = new PopupMenuProcessor(TabRightMouse_Click, barManager1);
                    popupMenuProcessor.InitMenu(tabMain);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void TabRightMouse_Click(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (e.Item is BarButtonItem)
                {
                    var bbtnItem = sender as BarButtonItem;
                    PopupMenuProcessor.TabControlType type = (PopupMenuProcessor.TabControlType)(e.Item.Tag);
                    if (type.Type == PopupMenuProcessor.ModuleType.Other)
                    {
                        CloseOtherTabProcess(type.Tab);
                    }
                    else if (type.Type == PopupMenuProcessor.ModuleType.Right)
                    {
                        CloseRightTabProcess(type.Tab);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnAlt9ForUnittest_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void bbtnSpeechVoice_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (!VoiceCommandData.isUseVoiceCommand)
                {
                    VoiceCommandProcess.InitVoiceCommand();
                }
                else
                {
                    VoiceCommandProcess.DisconnectVoiceCommand();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnCtrlShiftV_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                VoiceCommandProcess.InitVoiceCommand();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnCtrlShiftT_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                VoiceCommandProcess.DisconnectVoiceCommand();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barbtnResetCacheStateControl_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (GlobalVariables.CurrentModuleSelected != null)
                {
                    Inventec.Common.Logging.LogSystem.Debug("ProcessResetControlState.1");
                    HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                    controlStateWorker.ResetData(GlobalVariables.CurrentModuleSelected.ModuleLink);
                }
                Inventec.Common.Logging.LogSystem.Debug("ProcessResetControlState.2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barBtnSendReflection_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                ProcessRemoteByRemoteDesktopSoftware();
                //if (BranchDataWorker.Branch != null)
                //{
                //    UAS.WCF.Client.CRMRequestClientManager client = new UAS.WCF.Client.CRMRequestClientManager();
                //    UAS.WCF.DCO.WcfRequestDCO dco = new UAS.WCF.DCO.WcfRequestDCO();

                //    dco.BHYT_CODE = BranchDataWorker.Branch.HEIN_MEDI_ORG_CODE;
                //    dco.APPLICATION = "HIS PRO";
                //    dco.LOGINNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();

                //    string filename = "";
                //    log4net.Repository.Hierarchy.Hierarchy hierarchy = log4net.LogManager.GetRepository() as log4net.Repository.Hierarchy.Hierarchy;
                //    log4net.Repository.Hierarchy.Logger logger = hierarchy.Root;

                //    log4net.Appender.IAppender[] appenders = logger.Repository.GetAppenders();

                //    // Check each appender this logger has
                //    foreach (log4net.Appender.IAppender appender in appenders)
                //    {
                //        Type t = appender.GetType();
                //        // Get the file name from the first FileAppender found and return
                //        if (t.Equals(typeof(log4net.Appender.FileAppender)) || t.Equals(typeof(log4net.Appender.RollingFileAppender)))
                //        {
                //            filename = ((log4net.Appender.FileAppender)appender).File;
                //            break;
                //        }
                //    }

                //    if (!String.IsNullOrWhiteSpace(filename))
                //    {
                //        if (!File.Exists(filename))
                //        {
                //            string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
                //            filename = Path.Combine(path, filename);
                //        }

                //        if (File.Exists(filename))
                //        {
                //            string newPath = Path.Combine(Path.GetDirectoryName(filename), "clone");
                //            if (!Directory.Exists(newPath))
                //            {
                //                Directory.CreateDirectory(newPath);
                //            }

                //            string newFileName = Path.Combine(newPath, Path.GetFileName(filename));

                //            File.Copy(filename, newFileName, true);

                //            dco.LinkFile = newFileName;
                //        }
                //    }

                //    string data = Newtonsoft.Json.JsonConvert.SerializeObject(dco);
                //    try
                //    {
                //        var requestData = client.RequestData(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(data)));
                //    }
                //    catch (Exception ex)
                //    {
                //        Inventec.Common.Logging.LogSystem.Error(ex);
                //        XtraMessageBox.Show("Hiện tại không thể gửi yêu cầu do không kết nối đến ứng dụng UAS. Vui lòng kiểm tra lại ứng dụng UAS");
                //    }
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnFeedback_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                barBtnSendReflection_ItemClick(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bsiServerConnectStatus_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                frmCheckServerConnect frmCheckServerConnect = new frmCheckServerConnect();
                frmCheckServerConnect.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonEdit1_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                //frmCheckServerConnect frmCheckServerConnect = new frmCheckServerConnect();
                //frmCheckServerConnect.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnRefreshEmrConfig_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                object data = null;
                bool success = new BackendAdapter(param).Post<bool>("api/EmrConfig/ResetAll", ApiConsumers.EmrConsumer, data, param);
                WaitingManager.Hide();
                MessageManager.Show(this, param, success);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBKhongTimThayPluginsCuaChucNangNay), HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnRemoteSupportShortcut_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("bbtnRemoteSupportShortcut_ItemClick.1");
                ProcessRemoteByRemoteDesktopSoftware();
                Inventec.Common.Logging.LogSystem.Info("bbtnRemoteSupportShortcut_ItemClick.2");
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        public void LoadGridMenuTabPageExt()
        {
            try
            {
                List<MOS.SDO.WorkPlaceSDO> workPlaces = new List<MOS.SDO.WorkPlaceSDO>();

                workPlaces.AddRange(WorkPlace.WorkPlaceSDO);
                workPlaces = workPlaces.OrderBy(o => o.RoomName).ToList();

                gridControl1.DataSource = null;
                gridControl1.DataSource = workPlaces;

                gridControl2.DataSource = null;
                gridControl2.DataSource = workPlaces;

                LoadGridMenuTabPageExt1();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void gridViewApplicationMenu_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {

                GridView view = sender as GridView;
                GridHitInfo hi = view.CalcHitInfo(e.Location);
                if (hi.InRowCell)
                {
                    view.FocusedRowHandle = hi.RowHandle;
                    view.FocusedColumn = hi.Column;

                    int rowHandle = gridView1.GetVisibleRowHandle(hi.RowHandle);
                    var dataRow = (MOS.SDO.WorkPlaceSDO)gridView1.GetRow(rowHandle);
                    if (dataRow != null)
                    {
                        if (hi.Column.FieldName == "RoomName")
                        {
                            var rbSelectpage = GetRibbonTabPageByRoom(dataRow);
                            if (rbSelectpage != null)
                            {
                                ribbonMain.SelectedPage = rbSelectpage;
                            }

                            ribbonMain.HideApplicationButtonContentControl();
                            backstageViewControl2.Hide();
                        }
                        else if (hi.Column.FieldName == "Del" && hi.Column.RealColumnEdit.GetType() == typeof(DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit))
                        {
                            var rbSelectpage = GetRibbonTabPageByRoom(dataRow);
                            if (rbSelectpage != null)
                            {
                                ProcessRemoveRoomSelectedFromModuleChooseRoom(dataRow);
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

        private void gridView1_CustomDrawEmptyForeground(object sender, DevExpress.XtraGrid.Views.Base.CustomDrawEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;

                if (view.RowCount != 0) return;

                StringFormat drawFormat = new StringFormat();
                drawFormat.Alignment = drawFormat.LineAlignment = StringAlignment.Center;
                e.Graphics.DrawString("Chưa có phòng làm việc nào", e.Appearance.Font, SystemBrushes.ControlDark, new RectangleF(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height), drawFormat);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        DevExpress.XtraBars.Ribbon.RibbonPage GetRibbonTabPageByRoom(MOS.SDO.WorkPlaceSDO dataRow)
        {
            DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage = this.ribbonMain.Pages[0];
            try
            {
                for (int i = 0; i < this.ribbonMain.Pages.Count; i++)
                {
                    string tagCode = (this.ribbonMain.Pages[i].Tag ?? "").ToString();
                    if (!String.IsNullOrEmpty(tagCode))
                    {
                        long roomTypeId = 0, roomId = 0;
                        string roomCode = "";
                        var arrTag = tagCode.Split(new string[] { "__" }, StringSplitOptions.None);
                        roomTypeId = Inventec.Common.TypeConvert.Parse.ToInt64(arrTag.Length > 3 ? arrTag[3] : "");
                        roomCode = arrTag.Length > 3 ? arrTag[2] : "";
                        MOS.SDO.WorkPlaceSDO workPlace = String.IsNullOrEmpty(roomCode) ? null : HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO.Where(o => o.RoomCode == roomCode && o.RoomTypeId == roomTypeId).FirstOrDefault();
                        roomId = workPlace != null ? workPlace.RoomId : 0;
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => roomCode), roomCode) + "__" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => roomTypeId), roomTypeId) + "__" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => roomId), roomId));

                        if (dataRow.RoomId == roomId
                            && dataRow.RoomTypeId == roomTypeId
                            )
                        {
                            return this.ribbonMain.Pages[i];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return ribbonPage;
        }

        private void ProcessRemoveRoomSelectedFromModuleChooseRoom(MOS.SDO.WorkPlaceSDO workPlaceSDO)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();

                MOS.SDO.WorkInfoSDO workInfoUpdateSDO = new WorkInfoSDO();
                workInfoUpdateSDO.NurseLoginName = WorkPlace.WorkInfoSDO.NurseLoginName;
                workInfoUpdateSDO.NurseUserName = WorkPlace.WorkInfoSDO.NurseUserName;
                workInfoUpdateSDO.Rooms = WorkPlace.WorkInfoSDO.Rooms.Where(t => !(t.RoomId == workPlaceSDO.RoomId && t.DeskId == workPlaceSDO.DeskId)).ToList();

                workInfoUpdateSDO.WorkingShiftId = WorkPlace.WorkInfoSDO.WorkingShiftId;

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => workPlaceSDO), workPlaceSDO)
                    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => workInfoUpdateSDO), workInfoUpdateSDO));

                var rsWorkPlaceSDO = new BackendAdapter(param).PostRO<List<WorkPlaceSDO>>(HisRequestUriStore.TOKEN__UPDATE_WORK_PLACE_INFO, ApiConsumer.ApiConsumers.MosConsumer, workInfoUpdateSDO, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                if (rsWorkPlaceSDO != null && rsWorkPlaceSDO.Success)
                {
                    WorkPlace.WorkPlaceSDO = rsWorkPlaceSDO.Data;
                    if (WorkPlace.WorkPlaceSDO == null || WorkPlace.WorkPlaceSDO.Count == 0)
                    {
                        GlobalVariables.CurrentRoomTypeId = 0;
                        GlobalVariables.CurrentRoomTypeIds = null;
                        GlobalVariables.CurrentRoomTypeCodes = null;
                        GlobalVariables.CurrentRoomTypeCode = "";
                        WorkPlace.WorkInfoSDO = null;
                        WorkPlace.WorkPlaceSDO = null;
                    }
                    else
                    {
                        WorkPlace.WorkInfoSDO = workInfoUpdateSDO;
                        GlobalVariables.CurrentRoomTypeIds = WorkPlace.WorkPlaceSDO.Select(o => o.RoomTypeId).ToList();
                        GlobalVariables.CurrentRoomTypeId = WorkPlace.WorkPlaceSDO.FirstOrDefault().RoomTypeId;
                        var roomTypes = BackendDataWorker.Get<HIS_ROOM_TYPE>().Where(o => GlobalVariables.CurrentRoomTypeIds.Contains(o.ID)).ToList();
                        if (roomTypes != null && roomTypes.Count > 0)
                        {
                            GlobalVariables.CurrentRoomTypeCode = roomTypes[0].ROOM_TYPE_CODE;
                            GlobalVariables.CurrentRoomTypeCodes = roomTypes.Select(o => o.ROOM_TYPE_CODE).ToList();
                        }
                        if (BranchDataWorker.GetCurrentBranchId() == 0 || BranchDataWorker.GetCurrentBranchId() != WorkPlace.GetBranchId())
                        {
                            //Luu chi nhanh dang chon vao registry
                            HIS.Desktop.LocalStorage.BackendData.BranchDataWorker.ChangeBranch(WorkPlace.GetBranchId());
                        }
                    }
                    ResetAllTabpageToDefault();
                    //if (this.refeshReference != null)
                    //    this.refeshReference();
                    WaitingManager.Hide();
                }
                else
                {
                    if (param.Messages.Count == 0)
                        param.Messages.Add(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBXuatHienExceptionChuaKiemDuocSoat));

                    WaitingManager.Hide();
                    MessageManager.Show(param, false);

                    LogSystem.Warn("Goi api Token.UpdateWorkplace khong thanh cong" + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => workPlaceSDO), workPlaceSDO));
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView2_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                GridView view = sender as GridView;
                GridHitInfo hi = view.CalcHitInfo(e.Location);
                if (hi.InRowCell)
                {
                    view.FocusedRowHandle = hi.RowHandle;
                    view.FocusedColumn = hi.Column;

                    int rowHandle = gridView2.GetVisibleRowHandle(hi.RowHandle);
                    var dataRow = (MOS.SDO.WorkPlaceSDO)gridView2.GetRow(rowHandle);
                    if (dataRow != null)
                    {
                        if (hi.Column.FieldName == "RoomName")
                        {
                            var rbSelectpage = GetRibbonTabPageByRoom(dataRow);
                            if (rbSelectpage != null)
                            {
                                ribbonMain.SelectedPage = rbSelectpage;
                            }

                            ribbonMain.HideApplicationButtonContentControl();
                            backstageViewControl2.Hide();
                        }
                        else if (hi.Column.FieldName == "Del" && hi.Column.RealColumnEdit.GetType() == typeof(DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit))
                        {
                            var rbSelectpage = GetRibbonTabPageByRoom(dataRow);
                            if (rbSelectpage != null)
                            {
                                ProcessRemoveRoomSelectedFromModuleChooseRoom(dataRow);
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

        private void gridView2_CustomDrawEmptyForeground(object sender, DevExpress.XtraGrid.Views.Base.CustomDrawEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (view.RowCount != 0) return;

                StringFormat drawFormat = new StringFormat();
                drawFormat.Alignment = drawFormat.LineAlignment = StringAlignment.Center;
                e.Graphics.DrawString("Chưa có phòng làm việc nào", e.Appearance.Font, SystemBrushes.ControlDark, new RectangleF(e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height), drawFormat);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barBtnOpenBrower_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                string moduleLink = GlobalVariables.CurrentModuleSelected != null ? GlobalVariables.CurrentModuleSelected.ModuleLink : "";
                string domain = HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_CRM;
                string url = string.Format("{0}ords/f?p=104:5:::::P5_CODE:{1}", domain, moduleLink);
                Inventec.Common.Logging.LogSystem.Debug("Open brower - url__:" + url);
                if (!string.IsNullOrEmpty(moduleLink) && !string.IsNullOrEmpty(domain))
                {
                    try
                    {
                        WaitingManager.Show();
                        System.Diagnostics.Process.Start(url);
                        WaitingManager.Hide();
                    }
                    catch (Exception ex)
                    {
                        WaitingManager.Hide();
                        Inventec.Common.Logging.LogSystem.Error(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void timerEffectText_Tick(object sender, EventArgs e)
        {
            try
            {
                this.MessageLicense = MessageLicense.Insert(MessageLicense.Length, MessageLicense.Substring(0, 1));
                this.MessageLicense = MessageLicense.Substring(1, MessageLicense.Length - 1);
                lblLicense.Caption = this.MessageLicense;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

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
using ACS.EFMODEL.DataModels;
using ACS.Filter;
using DevExpress.Xpo.Logger;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.HisConfig;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Utility;
using HIS.Desktop.Utility.ValidateRule;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.Message;
using Inventec.Desktop.Common.Modules;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.Plugins.Library.DrugInterventionInfo;

namespace HIS.Desktop.Utilities.RemoteSupport
{
    public partial class frmRemoteSupportCreate : Form
    {
        const string CONFIG_KEY__VPLUS_CUSTOMER_INFO = "HIS.Desktop.VPLUS_CUSTOMER_INFO";
        string stt_sản_phẩm = "1";
        string stt_phần_mềm = "2";
        string yourAnydeskID = "";
        List<HIS_DEPARTMENT> lstDepartment = new List<HIS_DEPARTMENT>();
        public HIS_EMPLOYEE currentEmployee = new HIS_EMPLOYEE();
        internal Inventec.Desktop.Common.Modules.Module currentModule;
        int positionHandle;
        System.Windows.Forms.Timer timerGetAnydeskID;
        System.Windows.Forms.Timer timerHideWindowsAnydesk;
        byte[] imgCaptureMyScreen;
        List<FileAttachADO> lstAttachFileAdo = new List<FileAttachADO>();

        string ModuleLink = "HIS.Desktop.Utilities.RemoteSupport";
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        bool IsInitForm = false;
        public frmRemoteSupportCreate()
            : this(null)
        {
            
        }

        public frmRemoteSupportCreate(Module module)
        // : base(module)
        {
            InitializeComponent();
            try
            {
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);

                this.imgCaptureMyScreen = CaptureMyScreen();
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("module____:", module));
                currentModule = module;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        bool IsProcessOpen(string name)
        {
            foreach (Process clsProcess in Process.GetProcesses())
            {
                if (clsProcess.ProcessName.Contains(name))
                {
                    return true;
                }
            }

            return false;
        }

        bool IsProcessOpenExact(string name)
        {
            foreach (Process clsProcess in Process.GetProcesses())
            {
                if (clsProcess.ProcessName == name || clsProcess.ProcessName == String.Format("{0}.exe", name) || clsProcess.ProcessName == String.Format("{0} (32 bit)", name) || clsProcess.ProcessName == String.Format("{0}.exe (32 bit)", name))
                {
                    return true;
                }
            }

            return false;
        }
        System.Windows.Forms.Timer timerCheck = new Timer();
        private void frmRemoteSupportCreate_Load(object sender, EventArgs e)
        {
            try
            {
                timerCheck.Interval = 100;
                timerCheck.Tick += TimerCheck_Tick;
                GetEmployee();
                timerCheck.Start();
                InitControlState();
                this.txtContactInfo.Text = String.Format("IP: {0}, Tài khoản phần mềm: {1}-{2}, Điện thoại: {3}, Phiên bản: {4}", Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginAddress(), Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName(), Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName(), currentEmployee.TDL_MOBILE, GlobalString.VersionApp);


                this.GetAnydeskID();

                if (this.IsProcessOpen("AnyDesk"))
                {
                    //NOTHING
                }
                else
                {
                    Process.Start(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath + "\\Tool\\AnyDesk\\AnyDesk.exe");
                }

                timerGetAnydeskID = new System.Windows.Forms.Timer();
                timerGetAnydeskID.Interval = 1000;
                timerGetAnydeskID.Enabled = true;
                timerGetAnydeskID.Tick += timerRemoteSupportAnydesk_Tick;
                timerGetAnydeskID.Start();

                string defaultAnydeskPassword = (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.DefaultAnydeskPassword"));
                //string defaultAnydeskPassword = (ConfigurationManager.AppSettings["HIS.Desktop.DefaultAnydeskPassword"] ?? "123456a@");
                if (!String.IsNullOrEmpty(defaultAnydeskPassword))
                {
                    try
                    {
                        bool isChangePass = false;

                        if (File.Exists(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath + "\\Tool\\AnyDesk\\testbat.bat"))
                        {
                            string passBatContentOld = File.ReadAllText(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath + "\\Tool\\AnyDesk\\testbat.bat");
                            if (!String.IsNullOrEmpty(passBatContentOld))
                            {
                                if (passBatContentOld != String.Format("echo {0} | anydesk.exe --set-password", defaultAnydeskPassword))
                                {
                                    isChangePass = true;
                                }
                            }
                            else
                            {
                                isChangePass = true;
                            }
                        }
                        else
                        {
                            isChangePass = true;
                        }

                        if (isChangePass)
                        {
                            string passBatContent = String.Format("echo {0} | anydesk.exe --set-password", defaultAnydeskPassword);
                            File.WriteAllText(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath + "\\Tool\\AnyDesk\\testbat.bat", passBatContent);

                            //echo 123456 | anydesk.exe --set-password                       
                            Process.Start(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath + "\\Tool\\AnyDesk\\Inventec.AnydeskSetting.exe");
                        }
                    }
                    catch (Exception exx)
                    {
                        LogSystem.Warn(exx);
                    }
                }

                SetValidateRule();

                txtTitle.Focus();
                txtTitle.SelectAll();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void TimerCheck_Tick(object sender, EventArgs e)
        {
            try
            {
                timerCheck.Stop();
                string value = HisConfigs.Get<string>(CONFIG_KEY__VPLUS_CUSTOMER_INFO);
                if(string.IsNullOrEmpty(value))
                {
                    MessageManager.Show("Vui lòng khai báo cấu hình hệ thống \"HIS.Desktop.VPLUS_CUSTOMER_INFO\" để sử dụng tính năng này!");
                    this.Close();
                }
                if (string.IsNullOrEmpty(this.currentEmployee.VCONG_LOGINNAME))
                {
                    MessageManager.Show("Tài khoản của bạn chưa thế tạo yêu cầu do chưa có thông tin tài khoản trên hệ thống V+. Vui lòng bổ sung để sử dụng tính năng này");
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void GetEmployee()
        {
            try
            {
                CommonParam paramCo = new CommonParam();
                HisEmployeeFilter hisFilter = new HisEmployeeFilter();
                hisFilter.LOGINNAME__EXACT = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                var employees = new Inventec.Common.Adapter.BackendAdapter
                    (paramCo).Get<List<MOS.EFMODEL.DataModels.HIS_EMPLOYEE>>
                    ("api/HisEmployee/Get", ApiConsumer.ApiConsumers.MosConsumer, hisFilter, paramCo);
                if (employees != null && employees.Count > 0)
                {
                    currentEmployee = employees.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void GetAnydeskID()
        {
            try
            {
                if (!String.IsNullOrEmpty(this.yourAnydeskID))
                {
                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => yourAnydeskID), yourAnydeskID));
                    return;
                }

                string pathFile = "";
                string path1 = @"C:\Users\" + Environment.UserName + @"\AppData\Roaming\AnyDesk\system.conf";
                string path2 = @"C:\ProgramData\AnyDesk\system.conf";
                if (File.Exists(path1))
                {
                    pathFile = path1;
                }
                else if (File.Exists(path2))
                {
                    pathFile = path2;
                }

                //Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => pathFile), pathFile) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => path1), path1)
                //    + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => path2), path2));
                string txtsystemConf = File.ReadAllText(pathFile);
                //Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => txtsystemConf), txtsystemConf));
                if (!String.IsNullOrEmpty(txtsystemConf))
                {
                    var arrsystemConf = txtsystemConf.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => arrsystemConf), arrsystemConf));
                    if (arrsystemConf != null && arrsystemConf.Length > 0)
                    {
                        var systemConf = arrsystemConf.Where(t => t.Contains("ad.anynet.id=")).FirstOrDefault();
                        Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => systemConf), systemConf));
                        if (systemConf != null && !String.IsNullOrEmpty(systemConf))
                        {
                            var arrsystemConfValue = systemConf.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                            Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => arrsystemConfValue), arrsystemConfValue));
                            if (arrsystemConfValue != null && arrsystemConfValue.Count() > 0)
                            {
                                this.yourAnydeskID = arrsystemConfValue[1];
                                this.lblAnydeskID.Text = this.yourAnydeskID;
                            }
                        }
                        else
                        {
                            if (File.Exists(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath + "\\Tool\\AnyDesk\\storeAnydeskID.txt"))
                            {
                                string idBatContentOld = File.ReadAllText(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath + "\\Tool\\AnyDesk\\storeAnydeskID.txt");
                                if (!String.IsNullOrEmpty(idBatContentOld))
                                {
                                    this.yourAnydeskID = idBatContentOld;
                                    this.lblAnydeskID.Text = this.yourAnydeskID;
                                }
                            }

                            if (String.IsNullOrEmpty(this.yourAnydeskID))
                            {
                                ProcessStartInfo procInfo = new ProcessStartInfo();
                                procInfo.UseShellExecute = true;
                                procInfo.FileName = HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath + "\\Tool\\AnyDesk\\storeAnydeskID.bat";
                                Process.Start(procInfo);  //Start that process.
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void SetValidateRule()
        {
            CommonValidateMaxLength validateMaxLengthTitle = new CommonValidateMaxLength();
            validateMaxLengthTitle.textEdit = txtTitle;
            validateMaxLengthTitle.maxLength = 150;
            validateMaxLengthTitle.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
            validateMaxLengthTitle.isValidNull = true;
            dxValidationProviderEditorInfo.SetValidationRule(txtTitle, validateMaxLengthTitle);


            ValidationSingleControl(txtDescription, dxValidationProviderEditorInfo, "", null);


            CommonValidateMaxLength validateMaxLengthDes = new CommonValidateMaxLength();
            validateMaxLengthDes.textEdit = txtContactInfo;
            validateMaxLengthDes.maxLength = 400;
            validateMaxLengthDes.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
            validateMaxLengthDes.isValidNull = true;
            dxValidationProviderEditorInfo.SetValidationRule(txtContactInfo, validateMaxLengthDes);

        }

        protected void ValidationSingleControl(Control control, DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProviderEditor, string messageErr, IsValidControl isValidControl)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                if (isValidControl != null)
                {
                    validRule.isUseOnlyCustomValidControl = true;
                    validRule.isValidControl = isValidControl;
                }
                if (!String.IsNullOrEmpty(messageErr))
                    validRule.ErrorText = messageErr;
                else
                    validRule.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditor.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        protected void ValidationSingleControl(BaseEdit control, DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProviderEditor)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditor.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            bool IsThrowExFss = true;
            try
            {
                positionHandle = -1;
                bool valid = dxValidationProviderEditorInfo.Validate();
                Inventec.Common.Logging.LogSystem.Info("btnSave_Click:" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => valid), valid));
                if (!dxValidationProviderEditorInfo.Validate())
                    return;

                WaitingManager.Show();

                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => yourAnydeskID), yourAnydeskID));
                if (yourAnydeskID != "-")
                {
                    List<string> lstMessage = new List<string>();
                    foreach (var item in lstAttachFileAdo)
                    {
                        if (!File.Exists(item.PathFile))
                            continue;
                        FileInfo fi = new FileInfo(item.PathFile); //Đơn vị Byte
                        decimal fileSizeMb = fi.Length / (decimal)1024 / (decimal)1024;// Đổi sang MB
                        if (fileSizeMb > 20)
                            lstMessage.Add(item.FileAttachName);
                    }
                    if(lstMessage != null && lstMessage.Count > 0)
                    {
                        WaitingManager.Hide();
                        XtraMessageBox.Show(String.Format("Các tệp {0} có dung lượng lớn hơn 20Mb. Vui lòng kiểm tra lại", string.Join("; ", lstMessage)));
                        return;
                    }


                    dynamic remoteSupport = new System.Dynamic.ExpandoObject();

                    remoteSupport.tiêu_đề = txtTitle.Text;

                    List<FileHolder> files = new List<FileHolder>();

                    string fileLog = GetLogFileLocal();
                    long? fileLogSize = null;
                    long? imgCaptureSize = null;
                    long attFileSize = 0;

                    if (chkAttackWImage.Checked)
                    {
                        if (this.imgCaptureMyScreen == null || this.imgCaptureMyScreen.Length == 0)
                        {
                            this.imgCaptureMyScreen = CaptureMyScreen();
                        }
                        if (this.imgCaptureMyScreen != null)
                        {
                            FileHolder fileHolder = new FileHolder();
                            fileHolder.Content = new MemoryStream(this.imgCaptureMyScreen);
                            fileHolder.FileName = "imgCaptureMyScreen.png";
                            files.Add(fileHolder);

                            imgCaptureSize = fileHolder.Content.Length;
                        }
                    }

                    if (chkAttackLog.Checked && !String.IsNullOrEmpty(fileLog) && File.Exists(fileLog))
                    {
                        FileHolder fileHolder = new FileHolder();
                        fileHolder.Content = new MemoryStream(File.ReadAllBytes(fileLog));
                        fileHolder.FileName = "LogSystem.txt";
                        files.Add(fileHolder);

                        fileLogSize = fileHolder.Content.Length;
                    }

                    if(lstAttachFileAdo != null && lstAttachFileAdo.Count > 0)
                    {
                        foreach (var item in lstAttachFileAdo)
                        {
                            FileHolder fileHolder = new FileHolder();
                            fileHolder.Content = new MemoryStream(File.ReadAllBytes(item.PathFile));
                            fileHolder.FileName = item.FileAttachName;
                            files.Add(fileHolder);

                            attFileSize += fileHolder.Content.Length;
                        }
                    }
                    Inventec.Common.Logging.LogSystem.Debug(string.Format("dung lượng file log: {0}, dung lượng ảnh chụp màn hình: {1}, dung lượng file đính kèm: {2}, địa chỉ FSS: {3}", fileLogSize, imgCaptureSize, attFileSize, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_FSS_FOR_CRM));
                    string fileContent = "";
                    string formatImg = "<img src=\"{0}\">";
                    string formatFile = "<a href=\"{0}\">{1}</a>";
                    if (files.Count > 0)
                    {
                        var fileResults = Inventec.Fss.Client.FileUpload.UploadFile(GlobalVariables.APPLICATION_CODE, "", files, false, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_FSS_FOR_CRM);
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => fileResults), fileResults));
                        IsThrowExFss = false;
                        if (fileResults != null && fileResults.Count > 0)
                        {
                            fileContent += "\r\n\r\n";
                            List<string> imgList = new List<string>() { ".jpg",".jpeg",".png",".gif",".webp"};
                            foreach (var f in fileResults)
                            {
                                if(imgList.Exists(o=> f.Url.ToLower().EndsWith(o)))
                                {
                                    fileContent += string.Format(formatImg, String.Format("{0}{1}", HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_FSS_FOR_CRM, f.Url.Replace("\\", "/")));
                                }
                                else
                                {
                                    fileContent += string.Format(formatFile, String.Format("{0}{1}", HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_FSS_FOR_CRM, f.Url.Replace("\\", "/")), f.OriginalName);
                                }
                                fileContent += "\r\n";
                            }
                        }
                        else
                        {
                            CommonParam paramW = new CommonParam();
                            paramW.Messages.Add(string.Format("Không upload được các file đính kèm, dung lượng file log: {0}, dung lượng ảnh chụp màn hình: {1},dung lượng file đính kèm {2}. Tạo yêu cầu khách hàng thất bại", fileLogSize, imgCaptureSize, attFileSize));
                            WaitingManager.Hide();
                            MessageManager.Show(this, paramW, false);
                            return;
                        }
                    }
                    remoteSupport.nội_dung = String.Format("{0}\r\n{1}", txtDescription.Text, fileContent);
                    remoteSupport.tổ_chức_yêu_cầu_id = HisConfigs.Get<string>(CONFIG_KEY__VPLUS_CUSTOMER_INFO);
                    remoteSupport.người_yêu_cầu = currentEmployee.VCONG_LOGINNAME;
                    remoteSupport.anydesk = !string.IsNullOrEmpty(yourAnydeskID) ? yourAnydeskID : "…";
                    remoteSupport.thông_tin_liên_lạc = txtContactInfo.Text;
                    bool cReateSuccess = false;

                    CommonParam param = new CommonParam();
                    var resultRemoteSupport = new BackendAdapter(param).PostWithouApiParam<string>("/ords/vplus/viec/viec/", ApiConsumers.CrmConsumer, param, remoteSupport, 0, null);
                    cReateSuccess = true;

                    Inventec.Common.Logging.LogSystem.Info("Gọi api tạo yêu cầu hỗ trợ " + (cReateSuccess ? "thành công" : "thất bại") + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => resultRemoteSupport), resultRemoteSupport) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => remoteSupport), remoteSupport));
                    if (resultRemoteSupport != null && !String.IsNullOrEmpty(resultRemoteSupport.ToString()))
                    {
                        cReateSuccess = false;
                        param.Messages.Add(resultRemoteSupport);
                    }
                    WaitingManager.Hide();
                    MessageManager.Show(this, param, cReateSuccess);

                    this.Hide();
                }
                //else
                //{
                //    WaitingManager.Hide();
                //    MessageManager.Show("Không tìm thấy anydesk trên máy tính, vui lòng kiểm tra lại");
                //    GetAnydeskID();
                //}
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                if (IsThrowExFss)
                {
                    if(ex.InnerException is AggregateException && ex.InnerException.InnerException is HttpRequestException)
                        XtraMessageBox.Show(String.Format("Xảy ra lỗi tạo yêu cầu khách hàng địa chỉ {0}. Vui lòng kiểm tra lại", HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_FSS_FOR_CRM));
                    else if ( lstAttachFileAdo != null && lstAttachFileAdo.Count > 0)
                        XtraMessageBox.Show("Không thể tải lên tệp đình kèm. Vui lòng kiểm tra lại");
                    else
                        XtraMessageBox.Show("Xảy ra lỗi trong quá trình tải lên. Vui lòng kiểm tra lại");
                }
                LogSystem.Error(ex);
            }
        }

        private byte[] CaptureMyScreen()
        {
            try
            {
                Bitmap captureBitmap = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height, PixelFormat.Format32bppArgb);
                Rectangle captureRectangle = Screen.AllScreens[0].Bounds;

                Graphics captureGraphics = Graphics.FromImage(captureBitmap);
                captureGraphics.CopyFromScreen(captureRectangle.Left, captureRectangle.Top, 0, 0, captureRectangle.Size);

                ImageConverter converter = new ImageConverter();
                return (byte[])converter.ConvertTo(captureBitmap, typeof(byte[]));

            }

            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return null;
        }

        string GetLogFileLocal()
        {
            string filepathLog = "";
            try
            {
                string filename = "";
                log4net.Repository.Hierarchy.Hierarchy hierarchy = log4net.LogManager.GetRepository() as log4net.Repository.Hierarchy.Hierarchy;
                log4net.Repository.Hierarchy.Logger logger = hierarchy.Root;

                log4net.Appender.IAppender[] appenders = logger.Repository.GetAppenders();

                // Check each appender this logger has
                foreach (log4net.Appender.IAppender appender in appenders)
                {
                    Type t = appender.GetType();
                    // Get the file name from the first FileAppender found and return
                    if ((t.Equals(typeof(log4net.Appender.FileAppender)) || t.Equals(typeof(log4net.Appender.RollingFileAppender))) && ((log4net.Appender.FileAppender)appender).File.ToLower().Contains("logsystem.txt"))
                    {
                        filename = ((log4net.Appender.FileAppender)appender).File;
                        break;
                    }
                }

                if (!String.IsNullOrWhiteSpace(filename))
                {
                    if (!File.Exists(filename))
                    {
                        string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
                        filename = Path.Combine(path, filename);
                    }

                    if (File.Exists(filename))
                    {
                        string newPath = Path.Combine(Path.GetDirectoryName(filename), "clone");
                        if (!Directory.Exists(newPath))
                        {
                            Directory.CreateDirectory(newPath);
                        }

                        string newFileName = Path.Combine(newPath, Path.GetFileName(filename));

                        File.Copy(filename, newFileName, true);

                        filepathLog = newFileName;
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }

            return filepathLog;
        }

        int dem = 0;
        private void timerRemoteSupportAnydesk_Tick(object sender, EventArgs e)
        {
            try
            {
                dem++;

                var handles = Process.GetProcesses().Select(y => y.MainWindowTitle).ToList();
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => handles), handles));

                try
                {
                    var mainCaption = handles.FirstOrDefault(o => o.Contains("AnyDesk"));
                    if (!String.IsNullOrEmpty(mainCaption) && timerGetAnydeskID.Enabled)
                    {
                        IntPtr hwnd = HIS.Desktop.Utility.AutoHiddenWindows.FindWindowByCaption(IntPtr.Zero, mainCaption);
                        HIS.Desktop.Utility.AutoHiddenWindows.ShowWindow(hwnd, HIS.Desktop.Utility.AutoHiddenWindows.SW_MINIMIZE);

                        if (dem > 10)
                        {
                            timerGetAnydeskID.Stop();
                            timerGetAnydeskID.Enabled = false;
                            timerGetAnydeskID.Dispose();
                        }
                    }
                }
                catch (Exception exx)
                {
                    LogSystem.Warn(exx);
                }

                if (String.IsNullOrEmpty(this.yourAnydeskID))
                {
                    GetAnydeskID();
                }

                if (!String.IsNullOrEmpty(this.yourAnydeskID))
                {
                    //timerGetAnydeskID.Stop();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void bbtnSaveShortcut_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            btnSave_Click(null, null);
        }

        private void dxValidationProviderEditorInfo_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPhone_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void bbtnResetPassword_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                RegistrySettingWorker.ChangeValue("AnydeskPasswordSetting", "");
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void btnAttachFile_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFile = new OpenFileDialog();
                openFile.Multiselect = true;
                openFile.CheckFileExists = true;
                if (openFile.ShowDialog() == DialogResult.OK)
                {
                    string[] fullfileNameAttack = openFile.FileNames;
                    if (fullfileNameAttack != null)
                    {
                        foreach (var item in fullfileNameAttack)
                        {
                            FileAttachADO ado = new FileAttachADO();
                            ado.PathFile = item;
                            ado.FileAttachName = item.Substring(item.LastIndexOf("\\") + 1);
                            if (lstAttachFileAdo != null && lstAttachFileAdo.Count > 0 && lstAttachFileAdo.FirstOrDefault(o => o.PathFile.Equals(ado.PathFile)) != null)
                                continue;
                            lstAttachFileAdo.Add(ado);
                        }
                        gridControl1.DataSource = null;
                        gridControl1.DataSource = lstAttachFileAdo;
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void repDel_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var data = (FileAttachADO)gridView1.GetFocusedRow();
                if(lstAttachFileAdo.IndexOf(data) > -1)
                    lstAttachFileAdo.Remove(data);
                gridControl1.DataSource = null;
                gridControl1.DataSource = lstAttachFileAdo;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void gridView1_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (FileAttachADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            try
                            {
                                e.Value = e.ListSourceRowIndex + 1;
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void chkAttackLog_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (IsInitForm)
                    return;
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkAttackLog.Name && o.MODULE_LINK == this.ModuleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkAttackLog.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkAttackLog.Name;
                    csAddOrUpdate.VALUE = (chkAttackLog.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = this.ModuleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }


        }

        private void chkAttackWImage_CheckedChanged(object sender, EventArgs e)
        {

            try
            {
                if (IsInitForm)
                    return;
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkAttackWImage.Name && o.MODULE_LINK == this.ModuleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkAttackWImage.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkAttackWImage.Name;
                    csAddOrUpdate.VALUE = (chkAttackWImage.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = this.ModuleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void InitControlState()
        {
            try
            {
                chkAttackLog.Checked = true;
                chkAttackWImage.Checked = false;
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(this.ModuleLink);
                IsInitForm = true;
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item_ in this.currentControlStateRDO)
                    {
                        if (item_.KEY == chkAttackLog.Name)
                        {
                           chkAttackLog.Checked = item_.VALUE == "1";
                        }
                        if (item_.KEY == chkAttackWImage.Name)
                        {
                            chkAttackWImage.Checked = item_.VALUE == "1";
                        }
                    }
                }
                IsInitForm = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnInstruct_Click(object sender, EventArgs e)
        {
            try
            {
                string defaultBrowser = (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.VCong.TutorialLink"));
                System.Diagnostics.Process.Start(defaultBrowser);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }     
    }
    public class FileAttachADO
    {
        public string FileAttachName { get; set; }
        public string PathFile { get; set; }
    }
}

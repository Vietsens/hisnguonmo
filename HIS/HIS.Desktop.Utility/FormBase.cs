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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Utilities;
using HIS.Desktop.Utilities.RemoteSupport;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.Message;
using SDA.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace HIS.Desktop.Utility
{
    //public delegate string DelegateProcesseShortcutReplace(string key, ref string rtfRepValue, ref string htmlRepValue, int libType);
    public class FormBase : Form
    {
        private DevExpress.XtraBars.BarManager barManager1;
        internal DevExpress.XtraTab.XtraTabControl tabControlMain;
        private DevExpress.XtraBars.Bar bar1;
        private DevExpress.XtraBars.BarDockControl barDockControlTop;
        private DevExpress.XtraBars.BarDockControl barDockControlBottom;
        private DevExpress.XtraBars.BarDockControl barDockControlLeft;
        private DevExpress.XtraBars.BarDockControl barDockControlRight;
        private DevExpress.XtraBars.BarButtonItem bbtnTutorial;
        protected Inventec.Desktop.Common.Modules.Module currentModuleBase;
        protected bool isUseShortcutReplaceKeyBase = false;
        public string ModuleLink { get; set; }
        List<string> HideControls { get; set; }
        protected List<ModuleControlADO> ModuleControls { get; set; }
        List<string> ExcludePrintTypeCode { get; set; }
        Dictionary<string, string> dicOfNameOnClickPrintWithPrintTypeCfg { get; set; }
        bool isAutoInitPrintTypeCfg = true;
        List<DevExpress.XtraBars.BarManager> barManagerRunning;

        string timerSessionKey = "";
        protected bool? IsUseApplyFormClosingOption = false;

        Stopwatch watch;

        public virtual void ProcessDisposeModuleDataAfterClose()
        {
            //Will override in module....
        }

        public FormBase()
            : this(null)
        {

        }

        public string GetModuleLink()
        {
            return ModuleLink;
        }

        public FormBase(Inventec.Desktop.Common.Modules.Module module)
        {
            try
            {
                this.watch = System.Diagnostics.Stopwatch.StartNew();
                this.timerSessionKey = Guid.NewGuid().ToString();
                InitializeComponent();

                if (module != null)
                {
                    this.currentModuleBase = module;
                    this.ModuleLink = (this.currentModuleBase != null ? this.currentModuleBase.ModuleLink : "");

                    try
                    {
                        string iconPath = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                        this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Warn(ex);
                    }

                    if (this.currentModuleBase != null && !string.IsNullOrEmpty(currentModuleBase.text))
                    {
                        this.Text = this.currentModuleBase.text;
                        Inventec.Common.FlexCelPrint.Ado.SupportAdo.RemoteSupport = ProcessRemoteByRemoteDesktopSoftware;
                    }
                }

                this.isUseShortcutReplaceKeyBase = ShortcutReplace.IsUseShortcutReplaceKeyBase;
                this.isAutoInitPrintTypeCfg = true;

                MemoryProcessor.CalculateMemoryRam(this.ModuleLink + "____Dung lượng RAM phần mềm HIS trước khi mở popup module là:");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessRemoteByRemoteDesktopSoftware()
        {
            try
            {
                if (this.currentModuleBase != null)
                {
                    OpenPopupRemoteDesktopSoftware(currentModuleBase);
                }
                else
                {
                    OpenPopupRemoteDesktopSoftware(GlobalVariables.CurrentModuleSelected);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void OpenPopupRemoteDesktopSoftware(Inventec.Desktop.Common.Modules.Module moduleBase)
        {
            try
            {
                frmRemoteSupportCreate frmRemoteSupportCreate = new frmRemoteSupportCreate(moduleBase);
                frmRemoteSupportCreate.ShowDialog();
                //frmChatbox frmChatbox = new frmChatbox(moduleBase);
                //frmChatbox.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            try
            {
                base.OnLoad(e);
                LocalStorage.LocalData.GlobalVariables.HwndParent = this.Handle;
                if (!String.IsNullOrEmpty(this.ModuleLink))
                {
                    this.CreateThreadProcessHideControl();
                    this.CreateThreadProcessCustomizaButtonAndRestoreLayout();
                    if (this.isAutoInitPrintTypeCfg)
                        this.CreateThreadInitPrintTypeCfg();
                    this.CreateThreadProcessSetFontTextControl();
                }
                this.watch.Stop();
                Inventec.Common.Logging.LogAction.Info(String.Format("{0}____{1}____{2}____{3}____{4}____{5}____{6}____{7}", GlobalVariables.APPLICATION_CODE, GlobalString.VersionApp, (double)((double)this.watch.ElapsedMilliseconds / (double)1000), this.ModuleLink, "OpenModule", Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName(), StringUtil.GetIpLocal(), StringUtil.CustomerCode));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        public void ChangeIsUseApplyFormClosingOption(bool _isApplyConfigFormClosing)
        {
            this.IsUseApplyFormClosingOption = _isApplyConfigFormClosing;
            Inventec.Common.Logging.LogSystem.Info("IsUseApplyFormClosingOption:" + IsUseApplyFormClosingOption);
        }
        protected bool IsApplyFormClosingOption()
        {
            bool isApplyConfigFormClosing = false;
            try
            {
                var strModuleLinkApply = HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplicationWorker.Get<string>("HIS.Desktop.FormClosingOption.ModuleLinkApply");
                var strFormClosing = HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplicationWorker.Get<string>("HIS.Desktop.FormClosingOption");
                Inventec.Common.Logging.LogSystem.Info("strModuleLinkApply:" + strModuleLinkApply + "____strFormClosing:" + strFormClosing);
                if (strFormClosing == "1" && !String.IsNullOrEmpty(strModuleLinkApply) && strModuleLinkApply.Contains(this.ModuleLink))
                {
                    isApplyConfigFormClosing = true;
                }
                if (IsUseApplyFormClosingOption.HasValue)
                {
                    isApplyConfigFormClosing = isApplyConfigFormClosing && IsUseApplyFormClosingOption.Value;
                }
                //if (isApplyConfigFormClosing)
                {
                    Inventec.Common.Logging.LogSystem.Info("strModuleLinkApply:" + strModuleLinkApply + "____strFormClosing:" + strFormClosing + "____ModuleLink:" + ModuleLink + "____isApplyConfigFormClosing:" + isApplyConfigFormClosing + "____IsUseApplyFormClosingOption:" + IsUseApplyFormClosingOption);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return isApplyConfigFormClosing;
        }

        protected bool IsApplyFormClosingOption(string module_link)
        {
            bool isApplyConfigFormClosing = false;
            try
            {
                var strModuleLinkApply = HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplicationWorker.Get<string>("HIS.Desktop.FormClosingOption.ModuleLinkApply");
                var strFormClosing = HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplicationWorker.Get<string>("HIS.Desktop.FormClosingOption");
                Inventec.Common.Logging.LogSystem.Info("strModuleLinkApply:" + strModuleLinkApply + "____strFormClosing:" + strFormClosing);
                if (strFormClosing == "1" && !String.IsNullOrEmpty(strModuleLinkApply) && strModuleLinkApply.Contains(module_link))
                {
                    isApplyConfigFormClosing = true;
                }
                if (IsUseApplyFormClosingOption.HasValue)
                {
                    isApplyConfigFormClosing = isApplyConfigFormClosing && IsUseApplyFormClosingOption.Value;
                }
                //if (isApplyConfigFormClosing)
                {
                    Inventec.Common.Logging.LogSystem.Info("strModuleLinkApply:" + strModuleLinkApply + "____strFormClosing:" + strFormClosing + "____ModuleLink:" + ModuleLink + "____isApplyConfigFormClosing:" + isApplyConfigFormClosing + "____IsUseApplyFormClosingOption:" + IsUseApplyFormClosingOption);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return isApplyConfigFormClosing;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            Inventec.Common.Logging.LogSystem.Debug(this.ModuleLink + ": OnFormClosing.1");
            try
            {
                if (e.CloseReason == CloseReason.WindowsShutDown)
                {
                    Inventec.Common.Logging.LogSystem.Info(this.ModuleLink + ": OnFormClosing.1.0: e.CloseReason = WindowsShutDown");
                    return;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            var watch = System.Diagnostics.Stopwatch.StartNew();
            if (IsApplyFormClosingOption())
            {
                Inventec.Common.Logging.LogSystem.Info(this.ModuleLink + ": Có cấu hình tùy chọn xử lý khi tắt popup form HIS.Desktop.FormClosingOption =" + true);
                this.WindowState = FormWindowState.Minimized;
                this.ShowInTaskbar = false;

                base.OnFormClosing(e);
                watch.Stop();
                e.Cancel = true;
                return;
            }

            base.OnFormClosing(e);

            if (e.Cancel)
            {
                Inventec.Common.Logging.LogSystem.Info(this.ModuleLink + ": OnFormClosing.1.0: e.Cancel = true");
                return;
            }

            DisposeTimerByModuleLink(this.ModuleLink);
            this.ProcessActionDisposeRegisted();
            try
            {
                Inventec.Common.Logging.LogSystem.Debug(this.ModuleLink + ": OnFormClosing.1.1");

                this.Dispose(true);

                Inventec.Common.Logging.LogSystem.Debug(this.ModuleLink + ": OnFormClosing.1.2");
            }
            catch (Exception exx)
            {
                Inventec.Common.Logging.LogSystem.Error(exx);
            }

            Inventec.Common.Logging.LogSystem.Debug(this.ModuleLink + ": OnFormClosing.2");
            Inventec.Common.Logging.LogSystem.Debug(this.ModuleLink + ": OnFormClosing.3");

            MemoryProcessor.CalculateMemoryRam(this.ModuleLink + "____Dung lượng RAM phần mềm HIS sau khi tắt popup module là:");

            Inventec.Common.Logging.LogSystem.Debug(this.ModuleLink + ": OnFormClosing.4");
            watch.Stop();
            Inventec.Common.Logging.LogAction.Info(String.Format("{0}____{1}____{2}____{3}____{4}____{5}____{6}____{7}", GlobalVariables.APPLICATION_CODE, GlobalString.VersionApp, (double)((double)watch.ElapsedMilliseconds / (double)1000), this.ModuleLink, "CloseModule", Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName(), StringUtil.GetIpLocal(), StringUtil.CustomerCode));

            try
            {
                currentModuleBase = null;
                HideControls = null;
                ModuleLink = "";
                ModuleControls = null;
                ExcludePrintTypeCode = null;
                dicOfNameOnClickPrintWithPrintTypeCfg = null;
                timerSessionKey = "";
                watch = null;

                Inventec.Common.Logging.LogSystem.Debug(this.ModuleLink + ": OnFormClosing.5");
                Inventec.Common.Logging.LogSystem.Debug(this.ModuleLink + ": OnFormClosing.DisposeAllControl.1");
                DisposeAllControl(this);
                Inventec.Common.Logging.LogSystem.Debug(this.ModuleLink + ": OnFormClosing.DisposeAllControl.2");

                // Force garbage collection.
                GC.Collect();
                // Wait for all finalizers to complete
                GC.WaitForPendingFinalizers();
                // Force garbage collection again.
                GC.Collect();
                Inventec.Common.Logging.LogSystem.Debug(this.ModuleLink + ": OnFormClosing.6");
            }
            catch (Exception exx)
            {
                Inventec.Common.Logging.LogSystem.Error(exx);
            }
        }

        void ProcessActionDisposeRegisted()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info(ModuleLink + ": Begin call ProcessDisposeModuleDataAfterClose");
                this.ProcessDisposeModuleDataAfterClose();
                Inventec.Common.FlexCelPrint.Ado.SupportAdo.RemoteSupport = null;
                Inventec.Common.Logging.LogSystem.Info(ModuleLink + ": End call ProcessDisposeModuleDataAfterClose");
            }
            catch (Exception ex1)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex1);
            }
        }

        private void DisposeAllControl(Control parentControl)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info(ModuleLink + ": DisposeAllControl.1");
                ModuleControlDispose.DisposeAllControl(parentControl);
                Inventec.Common.Logging.LogSystem.Info(ModuleLink + ": DisposeAllControl.2");
            }
            catch (Exception exx)
            {
                Inventec.Common.Logging.LogSystem.Error(exx);
            }
        }

        /// <summary>
        /// Hàm xử lý việc ghi log action các xử lý theo định dạng để phục vụ việc trích xuất theo dõi hoạt động phần mềm
        /// </summary>
        /// <param name="actionProcess">Hàm xử lý của tính năng, vd: hàm để xử lý cho nút lưu tiếp đón</param>
        /// <param name="actionThreadName">Tên action của tính năng xử lý cần ghi log, vd: btnSave_Click, btnSaveAndPrint_Click</param>
        public void LogTheadInSessionInfo(Action actionProcess, string actionThreadName)
        {
            try
            {
                LogThreadSessionProcess logThreadSessionProcess = new LogThreadSessionProcess(actionProcess, actionThreadName, this.ModuleLink);
                logThreadSessionProcess.Run();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void RegisterTimer(string moduleLink, string timerKeyName, int timeInterval, Action timerProcess)
        {
            try
            {
                MemoryProcessor.RegisterTimer(String.Format("{0}_{1}", moduleLink, this.timerSessionKey), timerKeyName, timeInterval, timerProcess, true);
            }
            catch (Exception exx)
            {
                Inventec.Common.Logging.LogSystem.Warn(exx);
            }
        }

        public void StartTimer(string moduleLink, string timerKeyName)
        {
            try
            {
                MemoryProcessor.StartTimer(String.Format("{0}_{1}", moduleLink, this.timerSessionKey), timerKeyName);
            }
            catch (Exception exx)
            {
                Inventec.Common.Logging.LogSystem.Warn(exx);
            }
        }

        public void StopTimer(string moduleLink, string timerKeyName)
        {
            try
            {
                MemoryProcessor.StopTimer(String.Format("{0}_{1}", moduleLink, this.timerSessionKey), timerKeyName);
            }
            catch (Exception exx)
            {
                Inventec.Common.Logging.LogSystem.Warn(exx);
            }
        }

        public void DisposeTimer(string moduleLink, string timerKeyName)
        {
            try
            {
                MemoryProcessor.DisposeTimer(String.Format("{0}_{1}", moduleLink, this.timerSessionKey), timerKeyName);
            }
            catch (Exception exx)
            {
                Inventec.Common.Logging.LogSystem.Warn(exx);
            }
        }

        public void DisposeTimerByModuleLink(string moduleLink)
        {
            try
            {
                MemoryProcessor.DisposeTimerByModuleLink(String.Format("{0}_{1}", moduleLink, this.timerSessionKey));
            }
            catch (Exception exx)
            {
                Inventec.Common.Logging.LogSystem.Warn(exx);
            }
        }

        protected void CreateThreadProcessSetFontTextControl()
        {
            try
            {
                if (!String.IsNullOrEmpty(ChangeFontControl.ModuleLinksApplySpecialFont) && ChangeFontControl.ModuleLinksApplySpecialFont.Contains(this.ModuleLink))
                {
                    HIS.Desktop.Utility.ChangeFontControl.SetFontTextControl(this, ChangeFontControl.NameFontUsed);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(this.ModuleLink, ex);
            }
        }

        protected void AddBarManager(DevExpress.XtraBars.BarManager bmanager)
        {
            try
            {
                if (this.barManagerRunning == null)
                    this.barManagerRunning = new List<DevExpress.XtraBars.BarManager>();

                this.barManagerRunning.Add(bmanager);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Nếu không muốn chạy luôn tính năng tự động generate nút khi chạy qua hàm Load, mà muốn chủ động gọi khi muốn thì gọi hàm này và truyền vào false
        /// </summary>
        /// <param name="isAutoInitPrintTypeCfg"></param>
        protected void SetIsAutoInitPrintTypeCfg(bool isAutoInitPrintTypeCfg)
        {
            this.isAutoInitPrintTypeCfg = isAutoInitPrintTypeCfg;
        }

        /// <summary>
        /// Truyền vào các mã in sẽ bị xóa khỏi danh sách nút khi generate động nút, danh sách này theo nghiệp vụ của từng chức năng và từng nút
        /// </summary>
        /// <param name="excludePrintTypeCode"></param>
        protected void SetExcludePrintType(List<string> excludePrintTypeCode)
        {
            this.ExcludePrintTypeCode = excludePrintTypeCode;
        }

        protected void AddItemIntoDicOfNameOnClickPrintWithPrintTypeCfg(string controlName, string nameOnClickPrintWithPrintTypeCfg)
        {
            try
            {
                if (dicOfNameOnClickPrintWithPrintTypeCfg == null)
                    dicOfNameOnClickPrintWithPrintTypeCfg = new Dictionary<string, string>();
                dicOfNameOnClickPrintWithPrintTypeCfg.Add(controlName, nameOnClickPrintWithPrintTypeCfg);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        protected void CreateThreadProcessCustomizaButtonAndRestoreLayout()
        {
            Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(ProcessCustomizaButtonAndRestoreLayoutNewThread));
            //thread.Priority = ThreadPriority.Normal;
            try
            {
                thread.Start();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                thread.Abort();
            }
        }

        private void ProcessCustomizaButtonAndRestoreLayoutNewThread()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate { this.ProcessCustomizaButtonAndRestoreLayout(); }));
                }
                else
                {
                    this.ProcessCustomizaButtonAndRestoreLayout();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessCustomizaButtonAndRestoreLayout()
        {
            try
            {
                CustomizaButtonAndRestoreLayoutInControlProcess customizaButtonInControlProcess = new CustomizaButtonAndRestoreLayoutInControlProcess();
                customizaButtonInControlProcess.Run(this, this.ModuleLink, barManagerRunning);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Khi mở form, kiểm tra dữ liệu cấu hình "Tùy biến giao diện" (SDA_CUSTOMIZE_UI):
        ///  Lấy ra bản ghi được khai báo bảng này và thỏa mãn:
        ///  APP_CODE là "HIS"
        ///  MODULE_LINK tương ứng với module_link của màn hình được mở
        ///  IS_DEFAULT_FOCUS
        ///  Nếu tồn tại bản ghi thì:
        ///Duyệt control trong màn hình có đường dẫn tương ứng với giá trị được khai báo trong (CONTROL_PATH).
        /// Nếu IS_DEFAULT_FOCUS = 1 thì set focus mặc định vào control đó
        /// </summary>
        protected void ProcessCustomizeUI()
        {
            try
            {
                CustomizeUIProcess customizeUIProcess = new CustomizeUIProcess();
                customizeUIProcess.Run(this, this.ModuleLink);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        protected void CreateThreadInitPrintTypeCfg()
        {
            try
            {
                //PrintTypeCfgProcess printTypeCfgProcess = new PrintTypeCfgProcess();
                //printTypeCfgProcess.Run(this.ModuleLink, this, this.ExcludePrintTypeCode, dicOfNameOnClickPrintWithPrintTypeCfg);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        protected void CreateThreadInitModuleControl()
        {
            try
            {
                ModuleControlProcess controlProcess = new ModuleControlProcess(true);
                ModuleControls = controlProcess.GetControls(this);
                ModuleControls = ModuleControls != null ? ModuleControls.Where(o => !String.IsNullOrEmpty(o.ControlName)).Distinct().ToList() : null;
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        protected void CreateThreadProcessHideControl()
        {
            Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(ProcessHideControlNewThread));
            //thread.Priority = ThreadPriority.Normal;
            try
            {
                thread.Start();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                thread.Abort();
            }
        }

        private void ProcessHideControlNewThread()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate { this.ProcessHideControl(); }));
                }
                else
                {
                    this.ProcessHideControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessHideControl()
        {
            try
            {
                HideControlInControlProcess hideControlInControlProcess = new Utility.HideControlInControlProcess();
                hideControlInControlProcess.Run(this, this.ModuleLink);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        protected override bool ProcessCmdKey(ref System.Windows.Forms.Message msg, Keys keyData)
        {
            try
            {
                if (keyData == Keys.Escape)
                {
                    this.Close();
                    return true;
                }
                else if (keyData == (Keys.Shift | Keys.F11))
                {
                    //if (!String.IsNullOrEmpty(this.ModuleLink))
                    //{
                    HIS.Desktop.Plugins.HisTutorial.Form1 frmTutorial = new Plugins.HisTutorial.Form1(this.ModuleLink);
                    frmTutorial.ShowDialog();
                    //}
                }
                else if (keyData == (Keys.Control | Keys.Shift | Keys.H))
                {
                    // là admin mới được mở chức năng này
                    if (!GlobalVariables.AcsAuthorizeSDO.IsFull)
                    {
                        return true;
                    }

                    this.CreateThreadInitModuleControl();
                    if (ModuleControls != null && ModuleControls.Count > 0)
                    {
                        WaitingManager.Show();
                        Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ModuleControlVisible").FirstOrDefault();
                        if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.ModuleControlVisible'");

                        List<object> listArgs = new List<object>();
                        listArgs.Add(ModuleControls);
                        listArgs.Add((HIS.Desktop.Common.DelegateSelectData)ProcessWhileSelectControl);

                        SDA_HIDE_CONTROL hideControl = new SDA_HIDE_CONTROL();
                        hideControl.MODULE_LINK = this.ModuleLink;
                        hideControl.APP_CODE = GlobalVariables.APPLICATION_CODE;
                        listArgs.Add(hideControl);

                        //moduleData.ModuleTypeId = Inventec.Desktop.Common.Modules.Module.MODULE_TYPE_ID__FORM;
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, 0, 0), listArgs);
                        if (extenceInstance == null) throw new NullReferenceException("extenceInstance is null");

                        WaitingManager.Hide();
                        ((System.Windows.Forms.Form)extenceInstance).ShowDialog();
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Debug("Khong lay duoc danh sach control tai chuc nang dang mo____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ModuleControls), ModuleControls));
                    }
                    return true;
                }
                else if (keyData == (Keys.Control | Keys.Shift | Keys.S))
                {

                    // là admin mới được mở chức năng này

                    if (!GlobalVariables.AcsAuthorizeSDO.IsFull)
                    {
                        return true;
                    }

                    this.CreateThreadInitModuleControl();
                    if (ModuleControls != null && ModuleControls.Count > 0)
                    {
                        WaitingManager.Show();
                        Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ModuleButtonCustomize").FirstOrDefault();
                        if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.ModuleButtonCustomize'");

                        List<object> listArgs = new List<object>();
                        listArgs.Add(ModuleControls);

                        SDA_CUSTOMIZE_BUTTON CustomizeButton = new SDA_CUSTOMIZE_BUTTON();
                        CustomizeButton.MODULE_LINK = this.ModuleLink;
                        CustomizeButton.APP_CODE = GlobalVariables.APPLICATION_CODE;
                        listArgs.Add(CustomizeButton);

                        //moduleData.ModuleTypeId = Inventec.Desktop.Common.Modules.Module.MODULE_TYPE_ID__FORM;
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, 0, 0), listArgs);
                        if (extenceInstance == null) throw new NullReferenceException("extenceInstance is null");

                        WaitingManager.Hide();
                        ((System.Windows.Forms.Form)extenceInstance).ShowDialog();
                    }
                    else
                    {
                        Inventec.Common.Logging.LogSystem.Debug("Khong lay duoc danh sach control tai chuc nang dang mo____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => ModuleControls), ModuleControls));
                    }
                    return true;
                }
                else if (keyData == (Keys.Control | Keys.Shift | Keys.C))
                {
                    WaitingManager.Show();
                    Inventec.Desktop.Common.Modules.Module moduleAppConfigUser = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.ConfigAppUser").FirstOrDefault();
                    if (moduleAppConfigUser == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.ConfigAppUser'");
                    moduleAppConfigUser.ModuleTypeId = Inventec.Desktop.Common.Modules.Module.MODULE_TYPE_ID__FORM;

                    List<object> listArgsAppConfigUser = new List<object>();
                    List<object> listArgs = new List<object>();
                    listArgs.Add(this.ModuleLink);
                    long numpageSize = ConfigApplicationWorker.Get<long>("CONFIG_KEY__NUM_PAGESIZE") == 0 ? ConfigApplications.NumPageSize : ConfigApplicationWorker.Get<long>("CONFIG_KEY__NUM_PAGESIZE");
                    listArgsAppConfigUser.Add(ApiConsumers.SdaConsumer);
                    listArgsAppConfigUser.Add(numpageSize);
                    listArgsAppConfigUser.Add((Action)RefeshDataConfigApplication);
                    listArgsAppConfigUser.Add(Inventec.Desktop.Common.Modules.Module.MODULE_TYPE_ID__FORM);

                    List<string> listString = new List<string>();
                    listString.Add(GlobalVariables.APPLICATION_CODE);
                    listString.Add(this.ModuleLink);
                    listArgsAppConfigUser.Add(listString);


                    var module = PluginInstance.GetModuleWithWorkingRoom(moduleAppConfigUser, 0, 0);
                    var extenceInstanceAppConfigUser = PluginInstance.GetPluginInstance(module, listArgsAppConfigUser);
                    if (extenceInstanceAppConfigUser == null) throw new NullReferenceException("extenceInstanceAppConfigUser is null");
                    //TabControlBaseProcess.TabCreating(tabControlMain, "HIS.Desktop.Plugins.ConfigAppUser", moduleAppConfigUser.text, (System.Windows.Forms.UserControl)extenceInstanceAppConfigUser, module);
                    ((System.Windows.Forms.Form)extenceInstanceAppConfigUser).Show();

                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisConfig").FirstOrDefault();
                    if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.HisConfig'");



                    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, 0, 0), listArgs);
                    if (extenceInstance == null) throw new NullReferenceException("extenceInstance is null");

                    WaitingManager.Hide();
                    ((System.Windows.Forms.Form)extenceInstance).Show();
                    return true;
                }
                else if (keyData == (Keys.Control | Keys.Shift | Keys.G))
                {
                    try
                    {
                        string moduleLink = this.ModuleLink;
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
                    return true;
                }
                else if (keyData == (Keys.Shift | Keys.F2))
                {
                    VoiceCommandProcess.InitVoiceCommand();
                }
                else if (keyData == (Keys.Shift | Keys.F3))
                {
                    VoiceCommandProcess.DisconnectVoiceCommand();
                }
                else if (keyData == Keys.Space && this.isUseShortcutReplaceKeyBase)
                {
                    Control c = Control.FromHandle(msg.HWnd);

                    if (ShortcutReplace.ReplaceValue(c)) return true;
                }
                else if (keyData == (Keys.Control | Keys.Shift | Keys.R))
                {
                    ProcessResetControlState();
                    return true;
                }
                else if (keyData == (Keys.Control | Keys.F2))
                {
                    //barBtnSendReflection_ItemClick();
                    frmRemoteSupportCreate frmRemoteSupportCreate = new frmRemoteSupportCreate(currentModuleBase);
                    frmRemoteSupportCreate.ShowDialog();
                    return true;
                }
                else if (keyData == (Keys.Control | Keys.Shift | Keys.NumPad1) || keyData == (Keys.Control | Keys.Shift | Keys.D1))
                {
                    var module = GlobalVariables.CurrentModuleSelected;
                    frmRemoteSupportCreate frmRemoteSupportCreate = new frmRemoteSupportCreate(module);
                    frmRemoteSupportCreate.ShowDialog();

                    return true;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        void ProcessResetControlState()
        {
            try
            {
                if (!String.IsNullOrEmpty(ModuleLink))
                {
                    HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                    controlStateWorker.ResetData(ModuleLink);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// Sau khi sửa cấu hình phần mềm => đóng các tab đang mở, reload lại các dữ liệu liên quan
        /// </summary>
        void RefeshDataConfigApplication()
        {
            try
            {
                ConfigApplicationWorker.ReloadAll();
                HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplicationWorker.Init();
                var formMain = HIS.Desktop.Controls.Session.SessionManager.GetFormMain();
                if (formMain != null)
                {
                    Type type = formMain.GetType();
                    if (type != null)
                    {
                        MethodInfo methodInfo__ResetAllTabpageToDefault = type.GetMethod("ResetAllTabpageToDefault");
                        if (methodInfo__ResetAllTabpageToDefault != null)
                            methodInfo__ResetAllTabpageToDefault.Invoke(formMain, null);
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        protected void ProcessLostToken()
        {
            try
            {
                CommonParam param = new CommonParam();
                param.HasException = true;
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLostBase(param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        protected void EventLogSuccess(string message)
        {
            try
            {
                if (!String.IsNullOrEmpty(message))
                {
                    His.EventLog.Logger.Log(HIS.Desktop.LocalStorage.LocalData.GlobalVariables.APPLICATION_CODE, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName(), message, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginAddress());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        protected void InitRestoreLayoutGridViewFromXml(DevExpress.XtraGrid.Views.Grid.GridView gridViewList)
        {
            try
            {
                CustomizaButtonAndRestoreLayoutInControlProcess customizaButtonInControlProcess = new CustomizaButtonAndRestoreLayoutInControlProcess();
                Inventec.Common.Logging.LogSystem.Debug("IsAllowRestoreLayout_" + this.ModuleLink);
                if (customizaButtonInControlProcess.IsAllowRestoreLayout(this.ModuleLink))
                {
                    RestoreLayoutProcess restoreLayoutProcess = new Utility.RestoreLayoutProcess(this.ModuleLink);
                    restoreLayoutProcess.InitRestoreLayoutGridViewFromXml(gridViewList);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        protected void InitRestoreLayoutTreeListFromXml(DevExpress.XtraTreeList.TreeList treeList)
        {
            try
            {
                CustomizaButtonAndRestoreLayoutInControlProcess customizaButtonInControlProcess = new CustomizaButtonAndRestoreLayoutInControlProcess();
                if (customizaButtonInControlProcess.IsAllowRestoreLayout(this.ModuleLink))
                {
                    RestoreLayoutProcess restoreLayoutProcess = new Utility.RestoreLayoutProcess(this.ModuleLink);
                    restoreLayoutProcess.InitRestoreLayoutTreeListFromXml(treeList);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitializeComponent()
        {
            DevExpress.Utils.SerializableAppearanceObject serializableAppearanceObject1 = new DevExpress.Utils.SerializableAppearanceObject();
            this.tabControlMain = new DevExpress.XtraTab.XtraTabControl();
            this.barManager1 = new DevExpress.XtraBars.BarManager();
            this.bar1 = new DevExpress.XtraBars.Bar();
            this.bbtnTutorial = new DevExpress.XtraBars.BarButtonItem();
            this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
            this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tabControlMain)).BeginInit();
            this.SuspendLayout();
            // 
            // barManager1
            // 
            this.barManager1.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
            this.bar1});
            this.barManager1.DockControls.Add(this.barDockControlTop);
            this.barManager1.DockControls.Add(this.barDockControlBottom);
            this.barManager1.DockControls.Add(this.barDockControlLeft);
            this.barManager1.DockControls.Add(this.barDockControlRight);
            this.barManager1.Form = this;
            this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
            this.bbtnTutorial});
            this.barManager1.MaxItemId = 1;
            // 
            // bar1
            // 
            this.bar1.BarName = "Tools";
            this.bar1.DockCol = 0;
            this.bar1.DockRow = 0;
            this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
            this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
            new DevExpress.XtraBars.LinkPersistInfo(this.bbtnTutorial)});
            this.bar1.Text = "Tools";
            this.bar1.Visible = false;
            // 
            // tabControlMain
            // 
            this.tabControlMain.ClosePageButtonShowMode = DevExpress.XtraTab.ClosePageButtonShowMode.InAllTabPageHeaders;
            this.tabControlMain.CustomHeaderButtons.AddRange(new DevExpress.XtraTab.Buttons.CustomHeaderButton[] {
            new DevExpress.XtraTab.Buttons.CustomHeaderButton(DevExpress.XtraEditors.Controls.ButtonPredefines.DropDown, "", -1, true, false, DevExpress.XtraEditors.ImageLocation.MiddleCenter, null, serializableAppearanceObject1, "", null, null, true)});
            this.tabControlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlMain.Location = new System.Drawing.Point(0, 76);
            this.tabControlMain.Name = "tabControlMain";
            this.tabControlMain.Size = new System.Drawing.Size(1160, 523);
            this.tabControlMain.TabIndex = 2;
            this.tabControlMain.TabMiddleClickFiringMode = DevExpress.XtraTab.TabMiddleClickFiringMode.MouseUp;
            //this.tabControlMain.SelectedPageChanged += new DevExpress.XtraTab.TabPageChangedEventHandler(this.tabControlMain_SelectedPageChanged);
            //this.tabControlMain.CloseButtonClick += new System.EventHandler(this.tabControlMain_CloseButtonClick);
            //this.tabControlMain.TabMiddleClick += new DevExpress.XtraTab.ViewInfo.PageEventHandler(this.tabControlMain_TabMiddleClick);
            //this.tabControlMain.CustomHeaderButtonClick += new DevExpress.XtraTab.ViewInfo.CustomHeaderButtonEventHandler(this.tabControlMain_CustomHeaderButtonClick);
            //this.tabControlMain.MouseUp += new System.Windows.Forms.MouseEventHandler(this.tabControlMain_MouseUp);

            // 
            // bbtnTutorial
            // 
            this.bbtnTutorial.Caption = "Trợ giúp (F11)";
            this.bbtnTutorial.Id = 0;
            this.bbtnTutorial.ItemShortcut = new DevExpress.XtraBars.BarShortcut(System.Windows.Forms.Keys.F11);
            this.bbtnTutorial.Name = "bbtnTutorial";
            this.bbtnTutorial.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbtnTutorial_ItemClick);
            // 
            // barDockControlTop
            // 
            this.barDockControlTop.CausesValidation = false;
            this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
            this.barDockControlTop.Size = new System.Drawing.Size(284, 29);
            // 
            // barDockControlBottom
            // 
            this.barDockControlBottom.CausesValidation = false;
            this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.barDockControlBottom.Location = new System.Drawing.Point(0, 261);
            this.barDockControlBottom.Size = new System.Drawing.Size(284, 0);
            // 
            // barDockControlLeft
            // 
            this.barDockControlLeft.CausesValidation = false;
            this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.barDockControlLeft.Location = new System.Drawing.Point(0, 29);
            this.barDockControlLeft.Size = new System.Drawing.Size(0, 232);
            // 
            // barDockControlRight
            // 
            this.barDockControlRight.CausesValidation = false;
            this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.barDockControlRight.Location = new System.Drawing.Point(284, 29);
            this.barDockControlRight.Size = new System.Drawing.Size(0, 232);
            // 
            // FormBase
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.barDockControlLeft);
            this.Controls.Add(this.barDockControlRight);
            this.Controls.Add(this.barDockControlBottom);
            this.Controls.Add(this.barDockControlTop);
            this.Name = "FormBase";
            ((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void bbtnTutorial_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(this.ModuleLink))
                {
                    HIS.Desktop.Plugins.HisTutorial.Form1 frmTutorial = new Plugins.HisTutorial.Form1(this.ModuleLink);
                    frmTutorial.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void ProcessWhileSelectControl(object data)
        {
            try
            {
                if (data is ModuleControlADO)
                {
                    var moduleControl = data as ModuleControlADO;
                    if (moduleControl != null && ModuleControls != null)
                    {
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => moduleControl.ControlPath), moduleControl.ControlPath));
                        var controlEdit = ModuleControls.Where(o => o.ControlPath == moduleControl.ControlPath).FirstOrDefault();
                        if (controlEdit != null && controlEdit.mControl != null)
                        {
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => controlEdit.mControl.BackColor), controlEdit.mControl.BackColor.ToString()));
                            controlEdit.mControl.BackColor = Color.YellowGreen;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region Validate rule base
        protected void ValidateLookupWithTextEdit(LookUpEdit cbo, TextEdit textEdit, DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProviderEditor)
        {
            try
            {
                LookupEditWithTextEditValidationRule validRule = new LookupEditWithTextEditValidationRule();
                validRule.txtTextEdit = textEdit;
                validRule.cbo = cbo;
                validRule.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditor.SetValidationRule(textEdit, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        protected void ValidateGridLookupWithTextEdit(GridLookUpEdit cbo, TextEdit textEdit, DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProviderEditor)
        {
            try
            {
                GridLookupEditWithTextEditValidationRule validRule = new GridLookupEditWithTextEditValidationRule();
                validRule.txtTextEdit = textEdit;
                validRule.cbo = cbo;
                validRule.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditor.SetValidationRule(textEdit, validRule);
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

        protected void ValidationSingleControl(Control control, DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProviderEditor, GetMessageErrorValidControl getMessageErrorValidControl, IsValidControl isValidControl)
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
                if (getMessageErrorValidControl != null)
                    validRule.ErrorText = getMessageErrorValidControl();
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
        #endregion

        private void barBtnSendReflection_ItemClick()
        {
            try
            {
                if (BranchDataWorker.Branch != null)
                {
                    UAS.WCF.Client.CRMRequestClientManager client = new UAS.WCF.Client.CRMRequestClientManager();
                    UAS.WCF.DCO.WcfRequestDCO dco = new UAS.WCF.DCO.WcfRequestDCO();

                    dco.BHYT_CODE = BranchDataWorker.Branch.HEIN_MEDI_ORG_CODE;
                    dco.APPLICATION = "HIS PRO";
                    dco.LOGINNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();

                    string filename = "";
                    log4net.Repository.Hierarchy.Hierarchy hierarchy = log4net.LogManager.GetRepository() as log4net.Repository.Hierarchy.Hierarchy;
                    log4net.Repository.Hierarchy.Logger logger = hierarchy.Root;

                    log4net.Appender.IAppender[] appenders = logger.Repository.GetAppenders();

                    // Check each appender this logger has
                    foreach (log4net.Appender.IAppender appender in appenders)
                    {
                        Type t = appender.GetType();
                        // Get the file name from the first FileAppender found and return
                        if (t.Equals(typeof(log4net.Appender.FileAppender)) || t.Equals(typeof(log4net.Appender.RollingFileAppender)))
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

                            dco.LinkFile = newFileName;
                        }
                    }

                    string data = Newtonsoft.Json.JsonConvert.SerializeObject(dco);
                    try
                    {
                        var requestData = client.RequestData(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(data)));
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error(ex);
                        XtraMessageBox.Show("Hiện tại không thể gửi yêu cầu do không kết nối đến ứng dụng UAS. Vui lòng kiểm tra lại ứng dụng UAS");
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

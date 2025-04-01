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
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using HIS.Desktop.LocalStorage.ConfigHideControl;
using System.Threading;
using Inventec.Common.Logging;
using System.IO;
using DevExpress.XtraGrid.Views.Base;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Common;
using System.Diagnostics;
using System.Runtime.InteropServices;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Utilities;

namespace HIS.Desktop.Utility
{
    public partial class UserControlBase : UserControl
    {
        protected Inventec.Desktop.Common.Modules.Module currentModuleBase;
        List<string> HideControls { get; set; }
        string ModuleLink { get; set; }
        string SubModuleLink { get; set; }
        protected List<ModuleControlADO> ModuleControls { get; set; }
        bool isAutoInitPrintTypeCfg = true;
        List<string> ExcludePrintTypeCode { get; set; }
        Dictionary<string, string> dicOfNameOnClickPrintWithPrintTypeCfg { get; set; }             

        string timerSessionKey = "";

        public virtual void ProcessDisposeModuleDataAfterClose()
        {
            //Will override in module....
        }

        Stopwatch watch;

        public UserControlBase()
            : this(null)
        {

        }

        public UserControlBase(Inventec.Desktop.Common.Modules.Module module)
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
                }
                this.isAutoInitPrintTypeCfg = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public UserControlBase(string modulelink, string subModuleLink)
           : this(null)
        {
            this.ModuleLink = modulelink;
            this.SubModuleLink = subModuleLink;
        }

        public void DisposeExt()
        {
            try
            {
                string mlink = String.IsNullOrEmpty(SubModuleLink) ? ModuleLink : String.Format("{0}.{1}", ModuleLink, SubModuleLink);
                Inventec.Common.Logging.LogAction.Debug(mlink + ": Begin call DisposeExt.1");
                try
                {
                    currentModuleBase = null;
                    HideControls = null;
                    //ModuleLink = "";
                    ModuleControls = null;
                    ExcludePrintTypeCode = null;
                    dicOfNameOnClickPrintWithPrintTypeCfg = null;
                    timerSessionKey = "";
                    watch = null;

                    Inventec.Common.Logging.LogSystem.Debug(mlink + ": UserControlBase.DisposeAllControl.1");
                    DisposeAllControl(this);
                    Inventec.Common.Logging.LogSystem.Debug(mlink + ": UserControlBase.DisposeAllControl.2");
                    if (String.IsNullOrEmpty(SubModuleLink))
                    {
                        // Force garbage collection.
                        GC.Collect();
                        // Wait for all finalizers to complete
                        GC.WaitForPendingFinalizers();
                        // Force garbage collection again.
                        GC.Collect();
                    }
                }
                catch (Exception exx)
                {
                    Inventec.Common.Logging.LogSystem.Error(exx);
                }
                MemoryProcessor.CalculateMemoryRam(mlink + "____Dung lượng RAM phần mềm HIS sau khi tắt tab user control module là: ");
                Inventec.Common.Logging.LogAction.Debug(mlink + ": End call DisposeExt.2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            try
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                string mlink = String.IsNullOrEmpty(SubModuleLink) ? ModuleLink : String.Format("{0}.{1}", ModuleLink, SubModuleLink);

                Inventec.Common.Logging.LogSystem.Info(mlink + ": UserControlBase.Dispose: disposing=" + disposing + ".1");
                if (disposing && (components != null))
                {
                    components.Dispose();
                }
                this.ProcessActionDisposeRegisted();
                base.Dispose(disposing);
                this.DisposeTimerByModuleLink(ModuleLink);
                Inventec.Common.Logging.LogSystem.Info(mlink + ": UserControlBase.Dispose: disposing=" + disposing + ".2");
                watch.Stop();
                Inventec.Common.Logging.LogAction.Info(String.Format("{0}____{1}____{2}____{3}____{4}____{5}____{6}____{7}", GlobalVariables.APPLICATION_CODE, GlobalString.VersionApp, (double)((double)watch.ElapsedMilliseconds / (double)1000), mlink, "CloseModule", Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName(), StringUtil.GetIpLocal(), StringUtil.CustomerCode));
                this.DisposeExt();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        protected bool IsApplyFormClosingOption(string module_link)
        {
            bool isApplyConfigFormClosing = false;
            try
            {
                var strModuleLinkApply = HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplicationWorker.Get<string>("HIS.Desktop.FormClosingOption.ModuleLinkApply");
                var strFormClosing = HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplicationWorker.Get<string>("HIS.Desktop.FormClosingOption");
                if (strFormClosing == "1" && !String.IsNullOrEmpty(strModuleLinkApply) && strModuleLinkApply.Contains(module_link))
                {
                    isApplyConfigFormClosing = true;
                }
                if (isApplyConfigFormClosing)
                {
                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => strModuleLinkApply), strModuleLinkApply) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => strFormClosing), strFormClosing) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => module_link), module_link) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => isApplyConfigFormClosing), isApplyConfigFormClosing));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return isApplyConfigFormClosing;
        }

        protected override void OnLoad(EventArgs e)
        {
            try
            {
                base.OnLoad(e);
                string mlink = String.IsNullOrEmpty(SubModuleLink) ? ModuleLink : String.Format("{0}.{1}", ModuleLink, SubModuleLink);
                LocalStorage.LocalData.GlobalVariables.HwndParent = this.Handle;
                if (!String.IsNullOrEmpty(this.ModuleLink))
                {
                    this.CreateThreadProcessHideControl();
                    this.CreateThreadProcessCustomizaButtonAndRestoreLayout();
                    if (this.isAutoInitPrintTypeCfg)
                        this.CreateThreadInitPrintTypeCfg();
                    this.CreateThreadProcessSetFontTextControl();
                }

                MemoryProcessor.CalculateMemoryRam(mlink + "____Dung lượng RAM phần mềm HIS khi mở tab user control module là:");
                this.watch.Stop();
                Inventec.Common.Logging.LogAction.Info(String.Format("{0}____{1}____{2}____{3}____{4}____{5}____{6}____{7}", GlobalVariables.APPLICATION_CODE, GlobalString.VersionApp, (double)((double)this.watch.ElapsedMilliseconds / (double)1000), mlink, "OpenModule", Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName(), StringUtil.GetIpLocal(), StringUtil.CustomerCode));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                LogSystem.Warn(this.ModuleLink, ex);
            }
        }

        protected void SetIsAutoInitPrintTypeCfg(bool isAutoInitPrintTypeCfg)
        {
            this.isAutoInitPrintTypeCfg = isAutoInitPrintTypeCfg;
        }

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

        protected void CreateThreadInitPrintTypeCfg()
        {
            try
            {
                //TODO
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
                ModuleControlProcess controlProcess = new ModuleControlProcess();
                ModuleControls = controlProcess.GetControls(this);
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
                customizaButtonInControlProcess.Run(this, this.ModuleLink);
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
    }
}

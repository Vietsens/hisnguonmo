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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.EventLog;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Modules.Main;
using HIS.Desktop.Resources;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Theme;
using Microsoft.Win32;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Modules.Login
{
    public partial class frmLogin : DevExpress.XtraEditors.XtraForm
    {
        Inventec.UC.Login.MainLogin LoginMain = new Inventec.UC.Login.MainLogin();
        UserControl UCLogin;
        bool isUseRegisTryToken = true;
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        const string moduleLink = "HIS.Desktop.frmLogin";
        public frmLogin()
        {
            try
            {
                InitializeComponent();
                LoadLabelLanguage();
                try
                {
                    string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                }
                catch (Exception ex)
                {
                    LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        #region Form Events
        private void frmLogin_Load(object sender, EventArgs e)
        {
            try
            {
                this.InitTheme();
                this.InitToken();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        private void frmLogin_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("frmLogin_FormClosing.Begin");
                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => GlobalVariables.IsLostToken), GlobalVariables.IsLostToken) + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => GlobalVariables.isLogouter), GlobalVariables.isLogouter));
                if (GlobalVariables.IsLostToken && GlobalVariables.isLogouter)
                {
                    CloseAllApp.CloseAllApps();
                    System.Diagnostics.Process.GetCurrentProcess().Kill();
                    Application.Exit();
                }
                Inventec.Common.Logging.LogSystem.Debug("frmLogin_FormClosing.End");
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Load keys from language
        private void LoadLabelLanguage()
        {
            try
            {
                Inventec.UC.Login.Base.LanguageWorker.SetLanguage(LanguageManager.GetLanguage());
                this.Text = Inventec.Common.Resource.Get.Value("frmLogin.Text", HIS.Desktop.Resources.ResourceLanguageManager.LanguageFrmLogin, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
        #endregion

        #region UCLogin Handler

        private void InitTheme()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Error("THEME_NAME ---1-----------: " + DevExpressTheme.THEME_NAME);
                if (String.IsNullOrEmpty(DevExpressTheme.THEME_NAME))
                {
                    DevExpressTheme.THEME_NAME = DevExpressConstant.THEME_OFFICE_2013;
                }
                Inventec.Common.Logging.LogSystem.Error("THEME_NAME ---2-----------: " + DevExpressTheme.THEME_NAME);
                DevExpress.LookAndFeel.UserLookAndFeel.Default.SetSkinStyle(DevExpressTheme.THEME_NAME);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void InitConfigWorker()
        {
            try
            {
                HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplicationWorker.Init();
                HIS.Desktop.LocalStorage.ConfigHideControl.ConfigHideControlWorker.Init();
                HIS.Desktop.LocalStorage.ConfigCustomizaButton.ConfigCustomizaButtonWorker.Init();
                HIS.Desktop.LocalStorage.ConfigCustomizaButton.ConfigCustomizeUIWorker.Init();
                //HIS.Desktop.LocalStorage.ConfigPrintType.ConfigPrintTypeWorker.Init();//TODO
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        public void InitToken()
        {
            try
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                Inventec.UC.Login.Base.AppConfig.APPLICATION_CODE = GlobalVariables.APPLICATION_CODE;
                CommonParam param = new CommonParam();
                //Goi api kiem tra token server
                string registryToken = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HisConfigKeys.CONFIG_KEY__HIS_DESKTOP__IS_USE_REGISTRY_TOKEN);
                isUseRegisTryToken = (registryToken == "1");
                if (isUseRegisTryToken)
                    Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.UseRegistry(true);
                else
                    Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.UseRegistry(false);
              
                var tokenData = isUseRegisTryToken ? Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.Init(param) : null;
                if (tokenData != null && tokenData.User != null)
                {
                    GlobalVariables.IsLostToken = false;
                    GlobalVariables.isLogouter = false;
                    this.Close();
                    frmLoadConfigSystem frmLoad = new frmLoadConfigSystem(tokenData, moduleLink);
                    frmLoad.ShowDialog();
                    //Xóa session khác đã đăng nhập trước đó.
                    RemoveOtherSession();

                    //gán delegate để out his khi mất token
                    Inventec.Common.Adapter.AdapterBase.SetDelegate(HIS.Desktop.Controls.Session.SessionManager.ActionLostToken);

                    //Vao trang chu
                    frmMain fMain = new frmMain();
                    fMain.ShowDialog();

                    ////Show form login
                    //// to start new instance of application
                    //System.Diagnostics.Process.Start(Application.ExecutablePath);

                    ////close this one
                    //System.Diagnostics.Process.GetCurrentProcess().Kill();     
                }
                else
                {
                    LogSystem.Info("Khong co du lieu token cua phien lam viec truoc, nguoi dung vao trang dang nhap");
                    //Neu token khong ton tai hoac khong hop le thi vao dang nhap
                    InitUCLogin();
                    LoadOnFocus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitUCLogin()
        {
            try
            {
                Inventec.UC.Login.UCD.InitUCD DataLogin = SetDataInitUCLogin();
                UCLogin = LoginMain.Init(Inventec.UC.Login.MainLogin.EnumTemplate.TEMPLATE3, DataLogin);
                UCLogin.Dock = DockStyle.Fill;
                this.Controls.Add(UCLogin);
                SetDelegateForUCLogin();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string GetLoginnameInStore()
        {
            string loginnameStore = "";
            try
            {
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(moduleLink);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == "Loginname")
                        {
                            loginnameStore = item.VALUE;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return loginnameStore;
        }

        private void SetLoginnameInStore(string loginname, ref string preLoginname)
        {
            try
            {
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(moduleLink);
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == "Loginname" && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;

                if (csAddOrUpdate != null)
                {
                    preLoginname = csAddOrUpdate.VALUE;
                    csAddOrUpdate.VALUE = loginname;
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = "Loginname";
                    csAddOrUpdate.VALUE = loginname;
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }

                if (preLoginname != loginname)
                {
                    this.controlStateWorker.SetData(this.currentControlStateRDO);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void LoadOnFocus()
        {
            try
            {
                LoginMain.SetLoadFocus(UCLogin);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessOwnerForm(string lang)
        {
            try
            {
                if (!String.IsNullOrEmpty(lang))
                {
                    this.Text = Inventec.Common.Resource.Get.Value("frmLogin.Text", HIS.Desktop.Resources.ResourceLanguageManager.LanguageFrmLogin, new CultureInfo(lang));
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private Inventec.UC.Login.UCD.InitUCD SetDataInitUCLogin()
        {
            Inventec.UC.Login.UCD.InitUCD result = null;
            try
            {
                result = new Inventec.UC.Login.UCD.InitUCD(ApiConsumers.SdaConsumer, GlobalVariables.APPLICATION_CODE, RegistryConstant.COMPANY_FOLDER, RegistryConstant.APP_FOLDER, ProcessOwnerForm);
                result.reloadLoginnameAfterLeave = ReloadLoginnameAfterLeave;
                var branchs = GetListBranch();
                result.Branchs = branchs;
                HIS_BRANCH branchFromConfig = this.GetBranchFromConfig(branchs);
                result.FirstBranchId = branchFromConfig != null ? branchFromConfig.ID : GetFirstBranchId(branchs);
                Inventec.UC.Login.UCD.LabelString labelString = new Inventec.UC.Login.UCD.LabelString();
                labelString.LblBranch = Inventec.Common.Resource.Get.Value("frmLogin.lblBranch", HIS.Desktop.Resources.ResourceLanguageManager.LanguageFrmLogin, LanguageManager.GetCulture());
                result.LabelString = labelString;
                result.LoginnameDefault = GetLoginnameInStore();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        private void ReloadLoginnameAfterLeave(string loginname)
        {
            try
            {
                List<HIS_BRANCH> branchAllows = null;
                long? branchId = null;
                if (!String.IsNullOrEmpty(loginname))
                {
                    CommonParam param = new CommonParam();
                    HisUserRoomViewFilter filter = new HisUserRoomViewFilter();
                    filter.LOGINNAME = loginname;
                    List<V_HIS_USER_ROOM> userRooms = new BackendAdapter(param)
                    .Get<List<V_HIS_USER_ROOM>>(HisRequestUriStore.HIS_USER_ROOM_GETVIEW, ApiConsumers.MosConsumer, filter, param);
                    if (userRooms != null && userRooms.Count > 0)
                    {
                        List<long> branchIds = userRooms.Select(o => o.BRANCH_ID).Distinct().ToList();

                        List<HIS_BRANCH> branchs = GetListBranch();
                        if (branchs == null) return;
                        branchAllows = (branchs.Where(o => branchIds.Contains(o.ID)).ToList());
                        if (LoginMain != null && UCLogin != null)
                        {
                            HIS_BRANCH branchFromConfig = this.GetBranchFromConfig(branchs);
                            branchId = branchFromConfig != null ? branchFromConfig.ID
                                : GetFirstBranchId(branchAllows);
                        }
                    }
                    else
                    {
                        List<HIS_BRANCH> branchs = GetListBranch();
                        if (branchs == null) return;
                        branchAllows = branchs;
                        if (LoginMain != null && UCLogin != null)
                        {
                            HIS_BRANCH branchFromConfig = this.GetBranchFromConfig(branchs);
                            branchId = branchFromConfig != null ? branchFromConfig.ID
                                : GetFirstBranchId(branchAllows);
                        }
                    }
                }
                LoginMain.SetBranch(UCLogin, branchAllows, branchId);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private long? GetFirstBranchId(List<HIS_BRANCH> branchs)
        {
            long? branchId = null;
            try
            {
                if (branchs != null && branchs.Count > 0)
                {
                    branchId = branchs.FirstOrDefault().ID;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return branchId;
        }

        private HIS_BRANCH GetBranchFromConfig(List<HIS_BRANCH> branchs)
        {
            HIS_BRANCH branch = null;
            try
            {
                if (branchs != null && branchs.Count > 0)
                {
                    string branchCode = System.Configuration.ConfigurationSettings.AppSettings["HIS.Config.BranchCodeDefault"];
                    if (!String.IsNullOrEmpty(branchCode))
                        branch = branchs.FirstOrDefault(o => o.BRANCH_CODE.ToUpper() == branchCode.Trim().ToUpper());
                    if (branch == null && BranchDataWorker.GetCurrentBranchId() > 0)
                    {
                        branch = branchs.FirstOrDefault(o => o.ID == BranchDataWorker.GetCurrentBranchId());
                    }
                }
            }
            catch (Exception ex)
            {
                branch = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return branch;
        }

        private List<HIS_BRANCH> GetListBranch()
        {
            List<HIS_BRANCH> results = null;
            try
            {
                CommonParam param = new CommonParam();
                HisBranchFilter branchFilter = new MOS.Filter.HisBranchFilter();
                branchFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                results = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_BRANCH>>(HisRequestUriStore.HIS_BRANCH_GET, ApiConsumers.MosConsumer, branchFilter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                results = null;
            }
            return results;
        }

        private void SetDelegateForUCLogin()
        {
            try
            {
                if (LoginMain.SetDelegateInforLogin(UCLogin, LoginSuccess))
                {
                    //
                }
                if (LoginMain.SetDelegateButtonConfig(UCLogin, ButtonConfigClick))
                {
                    //
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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

        private void LoginSuccess(Inventec.UC.Login.UCD.LoginSuccessUCD SuccessData)
        {
            try
            {

                var watch = System.Diagnostics.Stopwatch.StartNew();
                SyncTimeFormServer();
                if (!LanguageManager.Change(GetCommonLanguageEnum(SuccessData.LANGUAGE)))
                    Inventec.Common.Logging.LogSystem.Warn("Set ngon ngu client cho nguoi dung " + SuccessData.LOGINNAME + " dang nhap vao he thong khong thanh cong. Time=" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss fff"));
                Inventec.Common.Adapter.AdapterConfig.LanguageCode = SuccessData.LANGUAGE.ToUpper();

                var tokenData = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetTokenData();
                if (tokenData == null || tokenData.User == null) throw new ArgumentNullException("tokenData");
                GlobalVariables.IsLostToken = false;
                GlobalVariables.isLogouter = false;

                ApiConsumers.SetConsunmer(tokenData.TokenCode);
                CommonParam param = new CommonParam();
                var EmployeeSchedule = new BackendAdapter(param).Post<bool>("api/HisEmployeeSchedule/CheckSchedule", ApiConsumers.MosConsumer, null, param);
                if (!EmployeeSchedule)
                {
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param));
                    DevExpress.XtraEditors.XtraMessageBox.Show(param.GetMessage(), HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao));
                    GlobalVariables.IsLostToken = true;
                    GlobalVariables.isLogouter = true;
                    Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.Logout(param);
                    return;
                }

                // Thời gian kết thúc
                watch.Stop();
                Inventec.Common.Logging.LogAction.Info(String.Format("{0}____{1}____{2}____{3}____{4}____{5}____{6}____{7}", GlobalVariables.APPLICATION_CODE, GlobalString.VersionApp, (double)((double)watch.ElapsedMilliseconds / (double)1000), "HIS.Desktop", "LoginApp:CallApi", Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName(), StringUtil.GetIpLocal(), StringUtil.CustomerCode));
                this.Hide();
                frmLoadConfigSystem frm = new frmLoadConfigSystem(tokenData, moduleLink,  SuccessData);
                frm.ShowDialog();
                //Xóa session khác đã đăng nhập trước đó.
                RemoveOtherSession();

                //gán delegate để out his khi mất token
                Inventec.Common.Adapter.AdapterBase.SetDelegate(HIS.Desktop.Controls.Session.SessionManager.ActionLostToken);

                //Vao trang chu
                frmMain fMain = new frmMain();
                fMain.ShowDialog();

                ////Show form login
                //// to start new instance of application
                //System.Diagnostics.Process.Start(Application.ExecutablePath);

                ////close this one
                //System.Diagnostics.Process.GetCurrentProcess().Kill();     
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void RemoveOtherSession()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("RemoveOtherSession");

                string IsUsingSingleSession = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HisConfigKeys.CONFIG_KEY__IsUsingSingleSession);
                Inventec.Common.Logging.LogSystem.Debug(IsUsingSingleSession);
                if (IsUsingSingleSession == "1")
                {
                    Task tsremoveOtherSession = Task.Factory.StartNew(() =>
                    {
                        CommonParam param = new CommonParam();
                        bool removeOtherSession = new BackendAdapter(param).Post<bool>("api/Token/RemoveOtherSession", ApiConsumers.MosConsumer, null, param);
                        if (!removeOtherSession)
                        {
                            Inventec.Common.Logging.LogSystem.Error(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param));
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SyncTimeFormServer()
        {
            try
            {
                CommonParam param = new CommonParam();
                TimerSDO timeSync = new BackendAdapter(param).Get<TimerSDO>(AcsRequestUriStore.ACS_TIMER__SYNC, ApiConsumers.AcsConsumer, 1, param);
                if (timeSync != null)
                {
                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => timeSync), timeSync));
                    HIS.Desktop.Base.SystemTimeManager.SetTime(timeSync);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Doc du lieu cau hinh sdaconfig va thuc hien truy van lay du lieu va luu cac bien cau hinh vao ram
        /// </summary>
        private void InitGlobalVariableInRam()
        {
            try
            {
                HIS.Desktop.LocalStorage.LocalData.GlobalVariables.LoadConfigPrintType();
                HIS.Desktop.LocalStorage.HisConfig.WitRecognizeCFG.LoadConfig();
                Inventec.Desktop.Common.Message.MessageManager.AutoFormDelay = Inventec.Common.TypeConvert.Parse.ToInt32((ConfigurationManager.AppSettings["Inventec.Desktop.Common.Message.AutoFormDelay"] ?? "500").ToString());
                //LoadDataWorkerLog(ref i, 0, 1, "InitGlobalVariableInRam");
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void InitEmrSystemCategory()
        {
            try
            {
                string isUseEmrSystemCFG = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HisConfigKeys.CONFIG_KEY__HIS_IS_USE_EMR_SYSTEM);
                if (isUseEmrSystemCFG == GlobalVariables.CommonStringTrue)
                {
                    MPS.ProcessorBase.PrintConfig.EmrBusiness = BackendDataWorker.Get<EMR.EFMODEL.DataModels.EMR_BUSINESS>();
                    MPS.ProcessorBase.PrintConfig.EmrSigners = BackendDataWorker.Get<EMR.EFMODEL.DataModels.EMR_SIGNER>();
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void InitAllowEditTemplateFile()
        {
            try
            {
                bool isAllowEditTemplateFile = true;
                string isAllowEditTemplateFileCFG = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HisConfigKeys.CONFIG_KEY__HIS_IS_ALLOW_OPEN_MPS_TEMPLATE);
                if (isAllowEditTemplateFileCFG == "1")
                {
                    isAllowEditTemplateFile = GlobalVariables.AcsAuthorizeSDO.IsFull;
                }
                MPS.ProcessorBase.PrintConfig.IsAllowEditTemplateFile = isAllowEditTemplateFile;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => isAllowEditTemplateFileCFG), isAllowEditTemplateFileCFG) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => isAllowEditTemplateFile), isAllowEditTemplateFile) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => GlobalVariables.AcsAuthorizeSDO.IsFull), GlobalVariables.AcsAuthorizeSDO.IsFull));
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void ShowTutorialModule()
        {
            try
            {
                HIS.Desktop.Plugins.HisTutorial.Form1 frmTutorial = new Plugins.HisTutorial.Form1("_PM_Hướng dẫn sử dụng và chỉnh sửa biểu mẫu");
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

        private void InitOtherReferenceConstant()
        {
            try
            {
                //this.InitEmrSystemCategory();

                MPS.ProcessorBase.PrintConfig.PrintTypes = BackendDataWorker.Get<SAR.EFMODEL.DataModels.SAR_PRINT_TYPE>();
                MPS.ProcessorBase.PrintConfig.MediOrgCode = HIS.Desktop.LocalStorage.HisConfig.HisMediOrgCFG.MEDI_ORG_VALUE__CURRENT;
                MPS.ProcessorBase.PrintConfig.OrganizationName = HIS.Desktop.LocalStorage.HisConfig.HisMediOrgCFG.ORGANIZATION_NAME;
                MPS.ProcessorBase.PrintConfig.ParentOrganizationName = HIS.Desktop.LocalStorage.HisConfig.HisMediOrgCFG.PARENT_ORGANIZATION_NAME;
                MPS.ProcessorBase.PrintConfig.URI_API_SAR = HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR;
                MPS.ProcessorBase.PrintConfig.Language = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage();
                MPS.ProcessorBase.PrintConfig.TemnplatePathFolder = HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath;
                MPS.ProcessorBase.PrintConfig.OrganizationLogoUri = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                MPS.ProcessorBase.PrintConfig.AppCode = GlobalVariables.APPLICATION_CODE;
                MPS.ProcessorBase.PrintConfig.VersionApp = StringUtil.VersionApp;
                MPS.ProcessorBase.PrintConfig.IpLocal = StringUtil.GetIpLocal();
                MPS.ProcessorBase.PrintConfig.CustomerCode = StringUtil.CustomerCode;
                MPS.ProcessorBase.PrintConfig.ShowTutorialModule = ShowTutorialModule;

                string strDisposeAfterProcess = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HisConfigKeys.CONFIG_KEY__MPS_PROCESSOR_BASE__DISPOSE_AFTER_PROCESS);
                if (!String.IsNullOrEmpty(strDisposeAfterProcess))
                {
                    MPS.ProcessorBase.PrintConfig.IsDisposeAfterProcess = (strDisposeAfterProcess == "1");
                }

                this.InitAllowEditTemplateFile();
                Inventec.Common.RichEditor.Base.FileLocalStore.Rootpath = HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath;

                Inventec.Common.RichEditor.DAL.ApiConsumerStore.SarConsumer = ApiConsumers.SarConsumer;
                Inventec.Common.RichEditor.RichEditorConfig.PrintTypes = MPS.ProcessorBase.PrintConfig.PrintTypes;

                Inventec.Common.WordContent.Base.WordContentStorage.SarConsumer = ApiConsumers.SarConsumer;
                Inventec.Common.WordContent.Base.WordContentStorage.SdaConsumer = ApiConsumers.SdaConsumer;
                string isShowDecimalOptionStr = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HisConfigKeys.CONFIG_KEY__HIS_IS_SHOW_DECIMAL_OPTION);
                Inventec.Common.Number.Convert.isShowDecimalOption = isShowDecimalOptionStr == "1";

                //Event log              
                His.EventLog.Logger.InitConsumer(ApiConsumers.SdaConsumer);

                //ProcessExtMpsServer();

                //LoadDataWorkerLog(ref i, 0, 1, "InitOtherReferenceConstant");
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }      

        private void ButtonConfigClick()
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = Application.StartupPath + @"\HIS.Desktop.DesktopConfig.exe";
                //string cmdLn = "|SdaBaseUri|" + ConfigSystems.URI_API_SDA + "|ApplicationCode|" + GlobalVariables.APPLICATION_CODE + "|TokenCode|" + (tc != null ? tc.TokenCode : "");
                //startInfo.Arguments = cmdLn;
                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

    }
}

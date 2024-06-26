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
using System.Configuration;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Microsoft.Win32;
using MOS.Filter;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.Location;
using ACS.SDO;
using Inventec.Common.Adapter;

namespace HIS.Desktop.Modules.Main
{
    public partial class frmMain : RibbonForm
    {
        static readonly bool IsRunUnitTest = ((ConfigurationManager.AppSettings["HIS.Desktop.IsRunUnitTest"] ?? "") == "1");
        internal BackendDataWorkerSet BackendDataWorkerSet { get { return (BackendDataWorkerSet)Worker.Get<BackendDataWorkerSet>(); } }
        System.Windows.Forms.Timer timerRunUnitTest;

        private void InitForUnitTest()
        {
            try
            {
                frmLoginLoad();
                //Khoi tao du lieu menu luu vao ram
                Base.Menu menu = new Base.Menu();
                menu.Init();
                menu.ProcessDataModule();
                menu.SetModuleToGlobalVariables();
                GetSdaLicense();
                LoadKeysFromlanguage();
                this.InitMenuFontSize();
                this.GenerateMenuInTopBar();
                this.ProcessAuthoriButton();
                this.InitStatusBar();
                this.InitMenuLanguage();
                this.MpsPrinterLoadFactory();
                this.FrdLoadFactory();

                //Set shortcut menu ship
                var currentModules = GlobalVariables.currentModules.Where(o => o.Visible).ToList();
                this.BuildToolStrip(GlobalVariables.currentModules);

                TheadProcessInitBackgroundData();
                LoadServiceRooms();
                LoadUseRooms();
                LoadServices();
                LoadServicePatys();
                LoadIcds();
                LoadHeinMediOrgs();
                InitOtherReferenceConstant();
                InitGlobalVariableInRam();

                RunPubSub();

                RunCheckTokenTimeout();
                RunSyncBackendDataToLocal();
                RunLogAction();
                RunLogService();
                RunNotify();
                InitDefaultSelectRoom();
                DevExpress.XtraEditors.XtraMessageBox.AllowHtmlText = true;
                SetDefaultTabpageFocus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void timerForUnitTest_Tick(object sender, EventArgs e)
        {
            timerRunUnitTest.Stop();
            InitForUnitTest();
        }

        private void frmLoginLoad()
        {
            try
            {
                Inventec.Common.LocalStorage.SdaConfig.ApiConsumerConfig.SdaConsumer = ApiConsumer.ApiConsumers.SdaConsumer;
                Inventec.Common.LocalStorage.SdaConfig.ConfigLoader.Refresh();

                this.InitTheme();
                this.InitToken();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void InitTheme()
        {
            try
            {
                if (String.IsNullOrEmpty(Inventec.Desktop.Common.Theme.DevExpressTheme.THEME_NAME))
                {
                    Inventec.Desktop.Common.Theme.DevExpressTheme.THEME_NAME = Inventec.Desktop.Common.Theme.DevExpressConstant.THEME_OFFICE_2013;
                }
                DevExpress.LookAndFeel.UserLookAndFeel.Default.SetSkinStyle(Inventec.Desktop.Common.Theme.DevExpressTheme.THEME_NAME);
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
                Inventec.UC.Login.Base.AppConfig.APPLICATION_CODE = GlobalVariables.APPLICATION_CODE;
                //HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplicationWorker.Init();
                CommonParam param = new CommonParam();
                //Goi api kiem tra token server
                string registryToken = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HisConfigKeys.CONFIG_KEY__HIS_DESKTOP__IS_USE_REGISTRY_TOKEN);
                bool isUseRegisTryToken = (Inventec.Common.TypeConvert.Parse.ToInt64(String.IsNullOrEmpty(registryToken) ? "1" : registryToken) == 1);
                if (isUseRegisTryToken)
                    Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.UseRegistry(true);

                var tokenData = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.Init(param);
                if (tokenData != null)
                {

                    //Get languageCode form registry set to library Inventec.Common.Adapter
                    RegistryKey appExeFolder = Registry.CurrentUser.CreateSubKey(Inventec.Desktop.Common.LanguageManager.RegistryConstant.SOFTWARE_FOLDER).CreateSubKey(Inventec.Desktop.Common.LanguageManager.RegistryConstant.COMPANY_FOLDER).CreateSubKey(Inventec.Desktop.Common.LanguageManager.RegistryConstant.APP_FOLDER);
                    string languageCode = (appExeFolder.GetValue(Inventec.Desktop.Common.LanguageManager.RegistryConstant.LANGUAGE_KEY) ?? "").ToString().ToUpper();
                    Inventec.Common.Adapter.AdapterConfig.LanguageCode = languageCode;

                    SyncTimeFormServer();
                    ApiConsumers.SetConsunmer(tokenData.TokenCode);
                    InitConfigWorker();

                    //LogSystem.Debug("Co du lieu token cua phien lam viec truoc do: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => tokenData), tokenData));
                    //Neu da ton tai token va kiem tra trong registry thong tin token hop le thi cap nhat token code cho api consumer
                    if (tokenData.User != null)
                    {
                        HIS.Desktop.EventLog.LoginLog.LoginSuccessLog(String.Format(HIS.Desktop.Resources.ResourceCommon.TruyCapVaoPhanMemThanhCong, GlobalVariables.APPLICATION_CODE));

                        InitDefaultBranch();
                        ChoiceRoomForSessionLogin();
                    }
                    else
                    {
                        LogSystem.Debug("Khong lay duoc thong tin token cua phien lam viec truoc do");
                        //Neu token khong ton tai hoac khong hop le thi vao dang nhap                       
                        InitUCLogin();
                    }
                }
                else
                {
                    LogSystem.Debug("Khong co du lieu token cua phien lam viec truoc, nguoi dung vao trang dang nhap");
                    //Neu token khong ton tai hoac khong hop le thi vao dang nhap
                    InitUCLogin();
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
                Inventec.Desktop.Common.Message.MessageManager.Show("Đã hết phiên làm việc, bạn cần đăng nhập sau đó khởi động lại");
                HIS.Desktop.Modules.Login.frmLogin frmLogin = new HIS.Desktop.Modules.Login.frmLogin();
                frmLogin.ShowDialog();
                Application.Exit();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitConfigWorker()
        {
            try
            {
                HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplicationWorker.Init();
                HIS.Desktop.LocalStorage.ConfigHideControl.ConfigHideControlWorker.Init();
                HIS.Desktop.LocalStorage.ConfigCustomizaButton.ConfigCustomizaButtonWorker.Init();
                //HIS.Desktop.LocalStorage.ConfigPrintType.ConfigPrintTypeWorker.Init();//TODO
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
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
                    HIS.Desktop.Base.SystemTimeManager.SetTime(timeSync);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ChoiceRoomForSessionLogin()
        {
            try
            {
                var tokenData = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetTokenData();
                Inventec.Common.Adapter.AdapterBase.UserName = tokenData.User.LoginName + " - " + tokenData.User.UserName;

                long useCacheLocal = ConfigApplicationWorker.Get<long>(AppConfigKeys.CONFIG_KEY__HIS_DESKTOP__IS_USE_CACHE_LOCAL);

                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => useCacheLocal), useCacheLocal));
                HIS.Desktop.Library.CacheClient.SerivceConfig.CacheType = useCacheLocal;

                HIS.Desktop.Library.CacheClient.SerivceConfig.AcsBaseUri = ConfigSystems.URI_API_ACS;
                HIS.Desktop.Library.CacheClient.SerivceConfig.ApplicationCode = GlobalVariables.APPLICATION_CODE;
                HIS.Desktop.Library.CacheClient.SerivceConfig.MosBaseUri = ConfigSystems.URI_API_MOS;
                HIS.Desktop.Library.CacheClient.SerivceConfig.SarBaseUri = ConfigSystems.URI_API_SAR;
                HIS.Desktop.Library.CacheClient.SerivceConfig.SdaBaseUri = ConfigSystems.URI_API_SDA;
                HIS.Desktop.Library.CacheClient.SerivceConfig.TokenCode = tokenData.TokenCode;

                if (useCacheLocal == 2)
                {
                    HIS.Desktop.Library.CacheClient.SerivceConfig.PreNamespaceFolder = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(ApplicationStoreLocation.ApplicationDirectory));

                    //Fix
                    HIS.Desktop.Library.CacheClient.SerivceConfig.RedisSaveType = Library.CacheClient.Redis.RedisSaveType.IRedisList;
                    new RedisProcess().AutoInstallAndStartService();
                }

                HIS.Desktop.LocalStorage.LisConfig.ConfigLoader.Refresh();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitDefaultBranch()
        {
            try
            {
                if (BranchDataWorker.GetCurrentBranchId() <= 0)
                {
                    string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                    CommonParam param = new CommonParam();
                    HisUserRoomViewFilter userRoomViewFilter = new MOS.Filter.HisUserRoomViewFilter();
                    userRoomViewFilter.LOGINNAME = loginName;
                    userRoomViewFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    List<MOS.EFMODEL.DataModels.V_HIS_USER_ROOM> currentUserRooms = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_USER_ROOM>>(HisRequestUriStore.HIS_USER_ROOM_GETVIEW, ApiConsumers.MosConsumer, userRoomViewFilter, param);
                    if (currentUserRooms != null && currentUserRooms.Count > 0)
                    {
                        var branchIds = currentUserRooms.Select(o => o.BRANCH_ID).Distinct().ToList();

                        HisBranchFilter branchFilter = new HisBranchFilter();
                        param = new CommonParam();
                        branchFilter.IDs = branchIds;
                        var currentBranchs = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_BRANCH>>(HisRequestUriStore.HIS_BRANCH_GET, ApiConsumers.MosConsumer, branchFilter, param);
                        if (currentBranchs != null && currentBranchs.Count == 1)
                        {
                            BranchDataWorker.ChangeBranch(currentBranchs[0].ID);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void TheadProcessInitBackgroundData()
        {
            try
            {
                string isRunSyncDataFromDBCacheToRamAfterLogin = HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplicationWorker.Get<string>(AppConfigKeys.CONFIG_KEY__HIS_DESKTOP_IS_AUTO_SYNC_DATA_IN_DB_CACHE_TO_RAM_AFTER_LOGIN);

                if (isRunSyncDataFromDBCacheToRamAfterLogin == "1")
                {
                    Thread thread = new System.Threading.Thread(new ThreadStart(InitBackgroundData2));
                    try
                    {
                        thread.Start();
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Error(ex);
                        thread.Abort();
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void InitBackgroundData2()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("InitBackgroundData2. 1");
                var moduleLinks = GlobalVariables.currentModuleRaws.Select(o => o.ModuleLink).ToArray();
                if (moduleLinks.Contains("HIS.Desktop.Plugins.RegisterV2")
                    || moduleLinks.Contains("HIS.Desktop.Plugins.Register")
                    || moduleLinks.Contains("HIS.Desktop.Plugins.RegisterV3"))
                {
                    var province = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>();
                    var district = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>();
                    var commune = BackendDataWorker.Get<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>();
                    var ethnic = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_ETHNIC>();
                    var national = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_NATIONAL>();
                }

                if (moduleLinks.Contains("HIS.Desktop.Plugins.AssignPrescriptionPK")
                    || moduleLinks.Contains("HIS.Desktop.Plugins.AssignPrescriptionYHCT")
                    || moduleLinks.Contains("HIS.Desktop.Plugins.AssignPrescriptionKidney"))
                {
                    var mety = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE>();
                    var maty = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MATERIAL_TYPE>();
                }

                Inventec.Common.Logging.LogSystem.Debug("InitBackgroundData2. 2");
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
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

        private void InitOtherReferenceConstant()
        {
            try
            {
                this.InitEmrSystemCategory();

                MPS.ProcessorBase.PrintConfig.PrintTypes = BackendDataWorker.Get<SAR.EFMODEL.DataModels.SAR_PRINT_TYPE>();
                MPS.ProcessorBase.PrintConfig.MediOrgCode = HIS.Desktop.LocalStorage.HisConfig.HisMediOrgCFG.MEDI_ORG_VALUE__CURRENT;
                MPS.ProcessorBase.PrintConfig.OrganizationName = HIS.Desktop.LocalStorage.HisConfig.HisMediOrgCFG.ORGANIZATION_NAME;
                MPS.ProcessorBase.PrintConfig.ParentOrganizationName = HIS.Desktop.LocalStorage.HisConfig.HisMediOrgCFG.PARENT_ORGANIZATION_NAME;
                MPS.ProcessorBase.PrintConfig.URI_API_SAR = HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR;
                MPS.ProcessorBase.PrintConfig.Language = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage();
                MPS.ProcessorBase.PrintConfig.TemnplatePathFolder = HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath;
                MPS.ProcessorBase.PrintConfig.OrganizationLogoUri = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.InitAllowEditTemplateFile();
                Inventec.Common.RichEditor.Base.FileLocalStore.Rootpath = HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath;

                Inventec.Common.RichEditor.DAL.ApiConsumerStore.SarConsumer = ApiConsumers.SarConsumer;
                Inventec.Common.RichEditor.RichEditorConfig.PrintTypes = MPS.ProcessorBase.PrintConfig.PrintTypes;


                //Event log              
                His.EventLog.Logger.InitConsumer(GlobalVariables.APPLICATION_CODE, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetTokenData().TokenCode);

                //LoadDataWorkerLog(ref i, 0, 1, "InitOtherReferenceConstant");
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void LoadHeinMediOrgs()
        {
            try
            {
                var HeinMediOrgs = BackendDataWorkerSet.Set<MOS.EFMODEL.DataModels.HIS_MEDI_ORG>();
                //LoadDataWorkerLog(ref i, 0, (HeinMediOrgs != null ? HeinMediOrgs.Count : 0), new MOS.EFMODEL.DataModels.HIS_MEDI_ORG().GetType().ToString());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadIcds()
        {
            try
            {
                var icds = BackendDataWorkerSet.Set<MOS.EFMODEL.DataModels.HIS_ICD>();
                //LoadDataWorkerLog(ref i, 0, (icds != null ? icds.Count : 0), new MOS.EFMODEL.DataModels.HIS_ICD().GetType().ToString());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadUseRooms()
        {
            try
            {
                var useRooms = BackendDataWorkerSet.Set<MOS.EFMODEL.DataModels.V_HIS_USER_ROOM>();
                //LoadDataWorkerLog(ref i, 0, (useRooms != null ? useRooms.Count : 0), new MOS.EFMODEL.DataModels.V_HIS_USER_ROOM().GetType().ToString());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadServicePatys()
        {
            try
            {
                var servicePatys = BackendDataWorkerSet.Set<MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY>();
                //LoadDataWorkerLog(ref i, 0, (servicePatys != null ? servicePatys.Count : 0), new MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY().GetType().ToString());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadServices()
        {
            try
            {
                var services = BackendDataWorkerSet.Set<MOS.EFMODEL.DataModels.V_HIS_SERVICE>();
                //LoadDataWorkerLog(ref i, 0, (services != null ? services.Count : 0), new MOS.EFMODEL.DataModels.V_HIS_SERVICE().GetType().ToString());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadServiceRooms()
        {
            try
            {
                var serviceRooms = BackendDataWorkerSet.Set<MOS.EFMODEL.DataModels.V_HIS_SERVICE_ROOM>();
                //LoadDataWorkerLog(ref i, 0, (serviceRooms != null ? serviceRooms.Count : 0), new MOS.EFMODEL.DataModels.V_HIS_SERVICE_ROOM().GetType().ToString());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


    }
}

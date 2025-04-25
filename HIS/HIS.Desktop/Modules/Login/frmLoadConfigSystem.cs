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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using Inventec.Common.Logging;
using Inventec.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Windows.Forms;
using System.Linq;
using System.Drawing;
using HIS.Desktop.LocalStorage.HisConfig;
using Inventec.Common.ThreadCustom;
using System.Threading;
using HIS.Desktop.LocalStorage.ConfigSystem;
using Inventec.Common.Repository;
using HIS.Desktop.ModuleExt;
using System.IO;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Utility;
using Microsoft.Win32;
using Inventec.Desktop.Common.LanguageManager;
using ACS.SDO;
using Inventec.Common.Adapter;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using HIS.Desktop.Library.CacheClient;
using ApiConsumers = HIS.Desktop.ApiConsumer.ApiConsumers;
using HIS.Desktop.Modules.Main;
using HIS.Desktop.LocalStorage.ConfigCustomizaButton;
using DevExpress.XtraEditors;
using System.Threading.Tasks;
using SDA.EFMODEL.DataModels;

namespace HIS.Desktop.Modules.Login
{
    public partial class frmLoadConfigSystem : DevExpress.XtraEditors.XtraForm
    {
        public Inventec.UC.Loading.MainLoginLoading LoadingMain;
        UserControl UCLoading;
        int i = 0;
        Inventec.Token.Core.TokenData tokenData;
        private ControlStateWorker controlStateWorker;
        private List<ControlStateRDO> currentControlStateRDO;
        string moduleLink;
        internal BackendDataWorkerSet BackendDataWorkerSet { get { return (BackendDataWorkerSet)Worker.Get<BackendDataWorkerSet>(); } }
        Base.Menu menu = new Base.Menu();
        Inventec.UC.Login.UCD.LoginSuccessUCD SuccessData;
        private bool isChangeLanguage;

        public frmLoadConfigSystem(Inventec.Token.Core.TokenData tokenData, string moduleLink)
        {
            try
            {
                InitializeComponent();
                this.tokenData = tokenData;
                this.moduleLink = moduleLink;
                LoadingMain = new Inventec.UC.Loading.MainLoginLoading();
                UCLoading = LoadingMain.Init(Inventec.UC.Loading.MainLoginLoading.TEMPLATE1);
                this.Controls.Add(UCLoading);
                this.SetDelegate();
                try
                {
                    string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);

                    i = 0;
                }
                catch (Exception ex)
                {
                    LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public frmLoadConfigSystem()
        {
            try
            {
                InitializeComponent();
                LoadingMain = new Inventec.UC.Loading.MainLoginLoading();
                UCLoading = LoadingMain.Init(Inventec.UC.Loading.MainLoginLoading.TEMPLATE1);
                this.Controls.Add(UCLoading);
                this.SetDelegateLogin();
                try
                {
                    string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);

                    i = 0;
                }
                catch (Exception ex)
                {
                    LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public frmLoadConfigSystem(Inventec.Token.Core.TokenData tokenData, string moduleLink, Inventec.UC.Login.UCD.LoginSuccessUCD SuccessData)
        {
            try
            {
                InitializeComponent();
                this.tokenData = tokenData;
                this.moduleLink = moduleLink;
                this.SuccessData = SuccessData;
                LoadingMain = new Inventec.UC.Loading.MainLoginLoading();
                UCLoading = LoadingMain.Init(Inventec.UC.Loading.MainLoginLoading.TEMPLATE1);
                this.Controls.Add(UCLoading);
                this.SetDelegateLoginSuccess();
                try
                {
                    string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);

                    i = 0;
                }
                catch (Exception ex)
                {
                    LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmLoadConfigSystem_Load(object sender, EventArgs e)
        {
            try
            {
                HIS.Desktop.LocalStorage.LocalData.GlobalVariables.CurrentNumberModule = 0;
                HIS.Desktop.LocalStorage.LocalData.GlobalVariables.CurrentNumberMps = 0;
                LoadingMain.StartRunWorker(UCLoading);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public void SetDelegateLoginSuccess()
        {
            try
            {
                LoadingMain.SetDelegateDoWorker(UCLoading, DoWorkLoginSuccess);
                LoadingMain.SetDelegateRunWorkerCompleted(UCLoading, RunWorkerCompleted);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public void SetDelegateLogin()
        {
            try
            {
                LoadingMain.SetDelegateDoWorker(UCLoading, DoWorkLogin);
                LoadingMain.SetDelegateRunWorkerCompleted(UCLoading, RunWorkerCompleted);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public void SetDelegate()
        {
            try
            {
                LoadingMain.SetDelegateDoWorker(UCLoading, DoWork);
                LoadingMain.SetDelegateRunWorkerCompleted(UCLoading, RunWorkerCompleted);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public void LoadDataWorkerLogForm(int curNum, int nextNum, int dataCount, string type)
        {
            i = curNum;
            LoadDataWorkerLog(ref i, nextNum, dataCount, type, true);
        }
        private void LoadDataWorkerLog(ref int curNum, int nextNum, int dataCount, string type)
        {
            LoadDataWorkerLog(ref curNum, nextNum, dataCount, type, true);
        }

        private void LoadDataWorkerLog(ref int curNum, int nextNum, int dataCount, string type, bool isLogger)
        {
            try
            {
                int length = (curNum + (nextNum > 0 ? nextNum : 7));
                //if (isLogger)
                //    LogSystem.Info(string.Format("Loaded " + type + ", count = " + (dataCount) + ". ({0}%)", curNum));
                length = length < 100 ? length : 100;
                for (; (curNum < length); curNum++)
                {
                    LoadingMain.SetReportProgress(UCLoading, curNum);
                }
                System.Threading.Thread.Sleep(10);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #region Close
        private void LoadUserMenu()
        {
            try
            {

                var users = BackendDataWorkerSet.Set<ACS.EFMODEL.DataModels.ACS_USER>();
                LoadDataWorkerLog(ref i, 0, (users != null ? users.Count : 0), new ACS.EFMODEL.DataModels.ACS_USER().GetType().ToString());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadProvince()
        {
            try
            {
                var provinces = BackendDataWorkerSet.Set<SDA.EFMODEL.DataModels.V_SDA_PROVINCE>();
                LoadDataWorkerLog(ref i, 0, (provinces != null ? provinces.Count : 0), new SDA.EFMODEL.DataModels.V_SDA_PROVINCE().GetType().ToString());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadNational()
        {
            try
            {
                var nationals = BackendDataWorkerSet.Set<SDA.EFMODEL.DataModels.SDA_NATIONAL>();
                LoadDataWorkerLog(ref i, 0, (nationals != null ? nationals.Count : 0), new SDA.EFMODEL.DataModels.SDA_NATIONAL().GetType().ToString());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadCareer()
        {
            try
            {
                var careers = BackendDataWorkerSet.Set<MOS.EFMODEL.DataModels.HIS_CAREER>();
                LoadDataWorkerLog(ref i, 0, (careers != null ? careers.Count : 0), new MOS.EFMODEL.DataModels.HIS_CAREER().GetType().ToString());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadEventLogs()
        {
            try
            {
                var eventLogs = BackendDataWorkerSet.Set<SDA.EFMODEL.DataModels.SDA_EVENT_LOG>();
                LoadDataWorkerLog(ref i, 0, (eventLogs != null ? eventLogs.Count : 0), new SDA.EFMODEL.DataModels.SDA_EVENT_LOG().GetType().ToString());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadEthrnics()
        {
            try
            {
                var ethrnics = BackendDataWorkerSet.Set<SDA.EFMODEL.DataModels.SDA_ETHNIC>();
                LoadDataWorkerLog(ref i, 0, (ethrnics != null ? ethrnics.Count : 0), new SDA.EFMODEL.DataModels.SDA_ETHNIC().GetType().ToString());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDistricts()
        {
            try
            {
                var districts = BackendDataWorkerSet.Set<SDA.EFMODEL.DataModels.V_SDA_DISTRICT>();
                LoadDataWorkerLog(ref i, 0, (districts != null ? districts.Count : 0), new SDA.EFMODEL.DataModels.V_SDA_DISTRICT().GetType().ToString());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadCommunes()
        {
            try
            {
                var communes = BackendDataWorkerSet.Set<SDA.EFMODEL.DataModels.V_SDA_COMMUNE>();
                LoadDataWorkerLog(ref i, 0, (communes != null ? communes.Count : 0), new SDA.EFMODEL.DataModels.V_SDA_COMMUNE().GetType().ToString());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadLanguages()
        {
            try
            {
                var languages = BackendDataWorkerSet.Set<SDA.EFMODEL.DataModels.SDA_LANGUAGE>(false, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadTranslates()
        {
            try
            {
                var translates = BackendDataWorkerSet.Set<SDA.EFMODEL.DataModels.SDA_TRANSLATE>(false, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadTreatmentTypes()
        {
            try
            {
                var treatmentTypes = BackendDataWorkerSet.Set<MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE>();
                LoadDataWorkerLog(ref i, 0, (treatmentTypes != null ? treatmentTypes.Count : 0), new MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE().GetType().ToString());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadPatientTypeTypes()
        {
            try
            {
                var patientTypes = BackendDataWorkerSet.Set<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>();
                LoadDataWorkerLog(ref i, 0, (patientTypes != null ? patientTypes.Count : 0), new MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE().GetType().ToString());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadTreatmentResults()
        {
            try
            {
                var treatmentResults = BackendDataWorkerSet.Set<MOS.EFMODEL.DataModels.HIS_TREATMENT_RESULT>();
                LoadDataWorkerLog(ref i, 0, (treatmentResults != null ? treatmentResults.Count : 0), new MOS.EFMODEL.DataModels.HIS_TREATMENT_RESULT().GetType().ToString());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadCashierRooms()
        {
            try
            {
                var datas = BackendDataWorkerSet.Set<MOS.EFMODEL.DataModels.V_HIS_CASHIER_ROOM>();
                LoadDataWorkerLog(ref i, 0, (datas != null ? datas.Count : 0), new MOS.EFMODEL.DataModels.V_HIS_CASHIER_ROOM().GetType().ToString());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadMedicineTypes()
        {
            try
            {
                var medicineTypes = BackendDataWorkerSet.Set<MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE>();
                LoadDataWorkerLog(ref i, 0, (medicineTypes != null ? medicineTypes.Count : 0), new MOS.EFMODEL.DataModels.V_HIS_MEDICINE_TYPE().GetType().ToString());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadMedicinePatys()
        {
            try
            {
                var medicinePatys = BackendDataWorkerSet.Set<MOS.EFMODEL.DataModels.HIS_MEDICINE_PATY>();
                LoadDataWorkerLog(ref i, 0, (medicinePatys != null ? medicinePatys.Count : 0), new MOS.EFMODEL.DataModels.HIS_MEDICINE_PATY().GetType().ToString());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion
        private void LoadServicePatys()
        {
            try
            {
                var servicePatys = BackendDataWorkerSet.Set<MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY>();
                LoadDataWorkerLog(ref i, 0, (servicePatys != null ? servicePatys.Count : 0), new MOS.EFMODEL.DataModels.V_HIS_SERVICE_PATY().GetType().ToString());
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
                var vservices = BackendDataWorkerSet.Set<MOS.EFMODEL.DataModels.V_HIS_SERVICE>();
                var services = BackendDataWorkerSet.Set<MOS.EFMODEL.DataModels.HIS_SERVICE>();
                LoadDataWorkerLog(ref i, 0, (services != null ? services.Count : 0), new MOS.EFMODEL.DataModels.V_HIS_SERVICE().GetType().ToString());
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
                var vserviceRooms = BackendDataWorkerSet.Set<MOS.EFMODEL.DataModels.V_HIS_SERVICE_ROOM>();
                var serviceRooms = BackendDataWorkerSet.Set<MOS.EFMODEL.DataModels.HIS_SERVICE_ROOM>();                
                LoadDataWorkerLog(ref i, 0, (serviceRooms != null ? serviceRooms.Count : 0), new MOS.EFMODEL.DataModels.V_HIS_SERVICE_ROOM().GetType().ToString());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadServiceComboADOs()
        {
            try
            {
                var serviceComboADOs = BackendDataWorkerSet.Set<HIS.Desktop.LocalStorage.BackendData.ADO.ServiceComboADO>(false, true);
                LoadDataWorkerLog(ref i, 0, (serviceComboADOs != null ? serviceComboADOs.Count : 0), new HIS.Desktop.LocalStorage.BackendData.ADO.ServiceComboADO().GetType().ToString());
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
                var vicds = BackendDataWorkerSet.Set<MOS.EFMODEL.DataModels.V_HIS_ICD>();
                LoadDataWorkerLog(ref i, 0, (icds != null ? icds.Count : 0), new MOS.EFMODEL.DataModels.HIS_ICD().GetType().ToString());
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
                LoadDataWorkerLog(ref i, 0, (useRooms != null ? useRooms.Count : 0), new MOS.EFMODEL.DataModels.V_HIS_USER_ROOM().GetType().ToString());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadExecuteRooms()
        {
            try
            {
                var executeRooms = BackendDataWorkerSet.Set<MOS.EFMODEL.DataModels.HIS_EXECUTE_ROOM>();
                var vexecuteRooms = BackendDataWorkerSet.Set<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM>();
                LoadDataWorkerLog(ref i, 0, (executeRooms != null ? executeRooms.Count : 0), new MOS.EFMODEL.DataModels.HIS_EXECUTE_ROOM().GetType().ToString());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadServiceFollows()
        {
            try
            {
                var executeRooms = BackendDataWorkerSet.Set<MOS.EFMODEL.DataModels.HIS_SERVICE_FOLLOW>();
                var vexecuteRooms = BackendDataWorkerSet.Set<MOS.EFMODEL.DataModels.V_HIS_SERVICE_FOLLOW>();
                LoadDataWorkerLog(ref i, 0, (executeRooms != null ? executeRooms.Count : 0), new MOS.EFMODEL.DataModels.HIS_SERVICE_FOLLOW().GetType().ToString());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadMaterialTypes()
        {
            try
            {
                var materialTypes = BackendDataWorkerSet.Set<MOS.EFMODEL.DataModels.V_HIS_MATERIAL_TYPE>();
                LoadDataWorkerLog(ref i, 0, (materialTypes != null ? materialTypes.Count : 0), new MOS.EFMODEL.DataModels.V_HIS_MATERIAL_TYPE().GetType().ToString());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadHeinMediOrgs()
        {
            try
            {
                var HeinMediOrgs = BackendDataWorkerSet.Set<MOS.EFMODEL.DataModels.HIS_MEDI_ORG>();
                LoadDataWorkerLog(ref i, 0, (HeinMediOrgs != null ? HeinMediOrgs.Count : 0), new MOS.EFMODEL.DataModels.HIS_MEDI_ORG().GetType().ToString());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadMedicineMaterialTypeComboADOs()
        {
            try
            {
                var MedicineMaterialTypeComboADOs = BackendDataWorkerSet.Set<HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO>(false, true);
                LoadDataWorkerLog(ref i, 0, (MedicineMaterialTypeComboADOs != null ? MedicineMaterialTypeComboADOs.Count : 0), new HIS.Desktop.LocalStorage.BackendData.ADO.MedicineMaterialTypeComboADO().GetType().ToString());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        #region ConnectSever
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
        private void InitConfigWorker()
        {
            try
            {
                LoadDataWorkerLog(ref i, 0, 1, "ConfigApplicationWorker");
                HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplicationWorker.Init();
                LoadDataWorkerLog(ref i, 0, 1, "ConfigHideControlWorker");
                HIS.Desktop.LocalStorage.ConfigHideControl.ConfigHideControlWorker.Init();
                LoadDataWorkerLog(ref i, 0, 1, "ConfigCustomizaButtonWorker");
                HIS.Desktop.LocalStorage.ConfigCustomizaButton.ConfigCustomizaButtonWorker.Init();
                LoadDataWorkerLog(ref i, 0, 1, "ConfigCustomizeUIWorker");
                HIS.Desktop.LocalStorage.ConfigCustomizaButton.ConfigCustomizeUIWorker.Init();
                //HIS.Desktop.LocalStorage.ConfigPrintType.ConfigPrintTypeWorker.Init();//TODO
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
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
        /// <summary>
        ///Kiem tra cau hinh phong lam viec cua tai khoan dang nhap
        ///Neu chua duoc cau hinh phong=> hien thi thong bao
        ///Neu co phong lam viec: 
        ///-- 1. Neu nguoi dung co 1 phong lam viec hoac da cau hinh phong lam viec trong cau hinh phan mem => chon phong lam viec do va vao form main luon
        ///-- 2. Nguoc lai, neu co tu 2 phong lam thi mo form chon phong lam viec
        /// </summary>
        private void ChoiceRoomForSessionLogin(bool isChangeBranchOrFirstRun = true, bool isChangeLoginname = false)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("ChoiceRoomForSessionLogin.1");
                var tokenData = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetTokenData();
                AdapterBase.UserName = tokenData.User.LoginName + " - " + tokenData.User.UserName;

                long useCacheLocal = ConfigApplicationWorker.Get<long>(AppConfigKeys.CONFIG_KEY__HIS_DESKTOP__IS_USE_CACHE_LOCAL);

                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => useCacheLocal), useCacheLocal));
                HIS.Desktop.Library.CacheClient.SerivceConfig.CacheType = useCacheLocal;

                HIS.Desktop.Library.CacheClient.SerivceConfig.AcsBaseUri = ConfigSystems.URI_API_ACS;
                HIS.Desktop.Library.CacheClient.SerivceConfig.ApplicationCode = GlobalVariables.APPLICATION_CODE;
                HIS.Desktop.Library.CacheClient.SerivceConfig.MosBaseUri = ConfigSystems.URI_API_MOS;
                HIS.Desktop.Library.CacheClient.SerivceConfig.SarBaseUri = ConfigSystems.URI_API_SAR;
                HIS.Desktop.Library.CacheClient.SerivceConfig.SdaBaseUri = ConfigSystems.URI_API_SDA;
                HIS.Desktop.Library.CacheClient.SerivceConfig.TokenCode = tokenData.TokenCode;

                isChangeLanguage = false;
                string oldLanguageCode = TranslateDataWorker.Language != null ? TranslateDataWorker.Language.LANGUAGE_CODE : "";
                TranslateDataWorker.Language = null;
                if (TranslateDataWorker.Language != null && TranslateDataWorker.Language.IS_BASE != 1)
                {
                    var watch1 = System.Diagnostics.Stopwatch.StartNew();
                    BackendDataWorker.Reset<SDA.EFMODEL.DataModels.SDA_TRANSLATE>();
                    var translatesAll = BackendDataWorker.Get<SDA.EFMODEL.DataModels.SDA_TRANSLATE>();
                    Inventec.Common.Logging.LogSystem.Info("____translatesAll.count=" + (translatesAll != null ? translatesAll.Count : 0));
                    watch1.Stop();
                    Inventec.Common.Logging.LogAction.Info(String.Format("{0}____{1}____{2}____{3}____{4}____{5}____{6}____{7}", GlobalVariables.APPLICATION_CODE, GlobalString.VersionApp, (double)((double)watch1.ElapsedMilliseconds / (double)1000), "HIS.Desktop", "LoginApp:InitTranslateData", Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName(), StringUtil.GetIpLocal(), StringUtil.CustomerCode));
                }
                isChangeLanguage = (TranslateDataWorker.Language != null && TranslateDataWorker.Language.LANGUAGE_CODE != oldLanguageCode);
                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => TranslateDataWorker.Language), TranslateDataWorker.Language));

                if (useCacheLocal == 2)
                {
                    var watch1 = System.Diagnostics.Stopwatch.StartNew();
                    HIS.Desktop.Library.CacheClient.SerivceConfig.PreNamespaceFolder = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(ApplicationStoreLocation.ApplicationDirectory));

                    //Fix
                    HIS.Desktop.Library.CacheClient.SerivceConfig.RedisSaveType = Library.CacheClient.Redis.RedisSaveType.IRedisList;
                    new RedisProcess().AutoInstallAndStartService();
                    watch1.Stop();
                    Inventec.Common.Logging.LogAction.Info(String.Format("{0}____{1}____{2}____{3}____{4}____{5}____{6}____{7}", GlobalVariables.APPLICATION_CODE, GlobalString.VersionApp, (double)((double)watch1.ElapsedMilliseconds / (double)1000), "HIS.Desktop", "LoginApp:AutoInstallAndStartService", Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName(), StringUtil.GetIpLocal(), StringUtil.CustomerCode));
                }
                Inventec.Common.Logging.LogSystem.Info("ChoiceRoomForSessionLogin.2");
                if (isChangeBranchOrFirstRun)
                {
                    var watch = System.Diagnostics.Stopwatch.StartNew();
                    Inventec.Common.Logging.LogSystem.Info("ChoiceRoomForSessionLogin.3.1");
                    HIS.Desktop.LocalStorage.HisConfig.ConfigLoader.Refresh();
                    HIS.Desktop.LocalStorage.LisConfig.ConfigLoader.Refresh();

                    GlobalVariables.RefreshUsingAccountBookModule = null;
                    GlobalVariables.AuthorityAccountBook = null;
                    WorkPlace.WorkPlaceSDO = null;
                    WorkPlace.WorkInfoSDO = null;
                    GlobalVariables.currentModules = null;
                    GlobalVariables.currentModuleRaws = null;
                    GlobalVariables.CurrentRoomTypeCodes = null;
                    HIS.Desktop.ModuleExt.TabControlBaseProcess.dicSaveData = new Dictionary<string, HIS.Desktop.ModuleExt.SaveDataBeforeClose>();
                    HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.ResetForLogout();
                    watch.Stop();
                    Inventec.Common.Logging.LogAction.Info(String.Format("{0}____{1}____{2}____{3}____{4}____{5}____{6}____{7}", GlobalVariables.APPLICATION_CODE, GlobalString.VersionApp, (double)((double)watch.ElapsedMilliseconds / (double)1000), "HIS.Desktop", "ConfigLoader:Refresh(HisConfig,LisConfig)", Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName(), StringUtil.GetIpLocal(), StringUtil.CustomerCode));
                    Inventec.Common.Logging.LogSystem.Info("ChoiceRoomForSessionLogin.3.2");
                }
                else if (isChangeLoginname)
                {
                    var watch2 = System.Diagnostics.Stopwatch.StartNew();
                    Inventec.Common.Logging.LogSystem.Info("ChoiceRoomForSessionLogin.4.1");
                    GlobalVariables.RefreshUsingAccountBookModule = null;
                    GlobalVariables.AuthorityAccountBook = null;
                    GlobalVariables.currentModules = null;
                    GlobalVariables.currentModuleRaws = null;
                    HIS.Desktop.ModuleExt.TabControlBaseProcess.dicSaveData = new Dictionary<string, HIS.Desktop.ModuleExt.SaveDataBeforeClose>();
                    HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.ResetForLogout();
                    GlobalVariables.CurrentRoomTypeId = 0;
                    GlobalVariables.CurrentRoomTypeIds = null;
                    GlobalVariables.CurrentRoomTypeCodes = null;
                    GlobalVariables.CurrentRoomTypeCode = "";
                    WorkPlace.WorkPlaceSDO = null;
                    WorkPlace.WorkInfoSDO = null;
                    Inventec.Common.Logging.LogSystem.Info("ChoiceRoomForSessionLogin.4.2");
                    watch2.Stop();
                    Inventec.Common.Logging.LogAction.Info(String.Format("{0}____{1}____{2}____{3}____{4}____{5}____{6}____{7}", GlobalVariables.APPLICATION_CODE, GlobalString.VersionApp, (double)((double)watch2.ElapsedMilliseconds / (double)1000), "HIS.Desktop", "LoginApp:InitMenu", Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName(), StringUtil.GetIpLocal(), StringUtil.CustomerCode));
                }
                else if (GlobalVariables.currentModuleRaws == null || GlobalVariables.currentModuleRaws.Count == 0)
                {
                    var watch1 = System.Diagnostics.Stopwatch.StartNew();
                    Inventec.Common.Logging.LogSystem.Info("ChoiceRoomForSessionLogin.5.1");
                    HIS.Desktop.LocalStorage.HisConfig.ConfigLoader.Refresh();
                    HIS.Desktop.LocalStorage.LisConfig.ConfigLoader.Refresh();
                    watch1.Stop();
                    Inventec.Common.Logging.LogAction.Info(String.Format("{0}____{1}____{2}____{3}____{4}____{5}____{6}____{7}", GlobalVariables.APPLICATION_CODE, GlobalString.VersionApp, (double)((double)watch1.ElapsedMilliseconds / (double)1000), "HIS.Desktop", "ConfigLoader:Refresh(HisConfig,LisConfig)", Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName(), StringUtil.GetIpLocal(), StringUtil.CustomerCode));
                    Inventec.Common.Logging.LogSystem.Info("ChoiceRoomForSessionLogin.5.1");
                }
                else
                {
                    GlobalVariables.CurrentRoomTypeId = 0;
                    GlobalVariables.CurrentRoomTypeIds = null;
                    GlobalVariables.CurrentRoomTypeCodes = null;
                    GlobalVariables.CurrentRoomTypeCode = "";
                    WorkPlace.WorkPlaceSDO = null;
                    WorkPlace.WorkInfoSDO = null;

                    if (isChangeLanguage)
                    {
                        var watch1 = System.Diagnostics.Stopwatch.StartNew();
                        GlobalVariables.currentModules = null;
                        GlobalVariables.currentModuleRaws = null;
                        watch1.Stop();
                        Inventec.Common.Logging.LogAction.Info(String.Format("{0}____{1}____{2}____{3}____{4}____{5}____{6}____{7}", GlobalVariables.APPLICATION_CODE, GlobalString.VersionApp, (double)((double)watch1.ElapsedMilliseconds / (double)1000), "HIS.Desktop", "InitMenu", Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName(), StringUtil.GetIpLocal(), StringUtil.CustomerCode));
                    }
                    var watch2 = System.Diagnostics.Stopwatch.StartNew();
                    List<Action> methods = new List<Action>();
                    methods.Add(InitOtherReferenceConstant);
                    methods.Add(InitGlobalVariableInRam);
                    ThreadCustomManager.MultipleThreadWithJoin(methods);
                    watch2.Stop();
                    Inventec.Common.Logging.LogAction.Info(String.Format("{0}____{1}____{2}____{3}____{4}____{5}____{6}____{7}", GlobalVariables.APPLICATION_CODE, GlobalString.VersionApp, (double)((double)watch2.ElapsedMilliseconds / (double)1000), "HIS.Desktop", "InitRamOther", Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName(), StringUtil.GetIpLocal(), StringUtil.CustomerCode));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ConnectToSever()
        {
            try
            {
                //Get languageCode form registry set to library Inventec.Common.Adapter
                RegistryKey appExeFolder = Registry.CurrentUser.CreateSubKey(RegistryConstant.SOFTWARE_FOLDER).CreateSubKey(RegistryConstant.COMPANY_FOLDER).CreateSubKey(RegistryConstant.APP_FOLDER);
                string languageCode = (appExeFolder.GetValue(RegistryConstant.LANGUAGE_KEY) ?? "").ToString().ToUpper();
                Inventec.Common.Adapter.AdapterConfig.LanguageCode = languageCode;

                SyncTimeFormServer();
                ApiConsumers.SetConsunmer(tokenData.TokenCode);
                string preLoginname = "";
                InitDefaultBranch();
                HIS.Desktop.EventLog.LoginLog.LoginSuccessLog(String.Format(HIS.Desktop.Resources.ResourceCommon.TruyCapVaoPhanMemThanhCong, GlobalVariables.APPLICATION_CODE));

                SetLoginnameInStore(tokenData.User.LoginName, ref preLoginname);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void DoWorkLogin()
        {
            try
            {
                i = 0;
                var watch1 = System.Diagnostics.Stopwatch.StartNew();
                GlobalVariables.LoadType = GlobalVariables.Function.KetNoiHeThongMayChu;
                LoadDataWorkerLog(ref i, 0, 1, "Kết nối hệ thống máy chủ");
                //Load cau hinh trong file SystemConfig client
                HIS.Desktop.LocalStorage.ConfigSystem.Load.Init();
                List<Action> methods = new List<Action>();
                methods.Add(ConfigSdaRefresh);
                methods.Add(ConfigHisRefresh);
                ThreadCustomManager.MultipleThreadWithJoin(methods);
                watch1.Stop();
                Inventec.Common.Logging.LogAction.Info(String.Format("{0}____{1}____{2}____{3}____{4}____{5}____{6}____{7}", "HIS", HIS.Desktop.Utility.GlobalString.VersionApp, (double)((double)watch1.ElapsedMilliseconds / (double)1000), "HIS.Desktop", "ConfigLoader:Refresh(HisConfig)", "userapp", HIS.Desktop.Utility.StringUtil.GetIpLocal(), HIS.Desktop.Utility.StringUtil.CustomerCode));
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void ConfigSdaRefresh()
        {
            try
            {
                Inventec.Common.LocalStorage.SdaConfig.ApiConsumerConfig.SdaConsumer = ApiConsumer.ApiConsumers.SdaConsumer;
                Inventec.Common.LocalStorage.SdaConfig.ConfigLoader.Refresh();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        private void ConfigHisRefresh()
        {
            try
            {
                HIS.Desktop.LocalStorage.HisConfig.ConfigLoader.Refresh();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion
        private void DoWorkLoginSuccess()
        {
            try
            {
                i = 0;
                var watch1 = System.Diagnostics.Stopwatch.StartNew();
                GlobalVariables.LoadType = GlobalVariables.Function.TaiDuLieuCauHinh;
                string preLoginname = "";
                SetLoginnameInStore(tokenData.User.LoginName, ref preLoginname);
                LoadDataWorkerLog(ref i, 0, 1, "TaiDuLieuCauHinh");
                bool isChangeBranch = false, isChangeLoginanme = false;
                if (BranchDataWorker.GetCurrentBranchId() == 0 || BranchDataWorker.GetCurrentBranchId() != (SuccessData.BranchId ?? 0))
                {
                    //Luu chi nhanh dang chon vao registry
                    BranchDataWorker.ChangeBranch(SuccessData.BranchId ?? 0);
                    isChangeBranch = true;
                }
                if (GlobalVariables.currentModuleRaws == null || GlobalVariables.currentModuleRaws.Count == 0)
                {
                    isChangeBranch = true;
                }

                isChangeLoginanme = (preLoginname != tokenData.User.LoginName);
                if (isChangeBranch || isChangeLoginanme || ConfigApplicationWorker.GetAllConfig() == null || ConfigApplicationWorker.GetAllConfig().Count == 0)
                {
                    InitConfigWorker();
                }
                LoadDataWorkerLog(ref i, 0, 1, "TaiDuLieuCauHinh");
                if (isChangeBranch || GlobalVariables.currentModuleRaws == null || GlobalVariables.currentModuleRaws.Count == 0)
                {
                    var watch2 = System.Diagnostics.Stopwatch.StartNew();
                    LoadDataWorkerLog(ref i, 0, 1, "TaiDuLieuCauHinh");
                    ChoiceRoomForSessionLogin(isChangeBranch, isChangeLoginanme);//Kiem tra va chon phong lam viec
                    LoadDataWorkerLog(ref i, 0, 1, "InitDataCacheLocal");
                    List<Action> methods = new List<Action>();
                    methods.Add(BackendDataWorker.InitDataCacheLocal);
                    methods.Add(LoadServiceRooms);
                    methods.Add(LoadUseRooms);
                    methods.Add(LoadServices);
                    methods.Add(LoadServicePatys);
                    methods.Add(LoadIcds);
                    methods.Add(LoadHeinMediOrgs);
                    methods.Add(menu.Init);
                    ThreadCustomManager.MultipleThreadWithJoin(methods);
                    for (; (i <= 100); i++)
                    {
                        LoadingMain.SetReportProgress(UCLoading, i);
                        System.Threading.Thread.Sleep(5);
                    }
                    watch2.Stop();
                    Inventec.Common.Logging.LogAction.Info(String.Format("{0}____{1}____{2}____{3}____{4}____{5}____{6}____{7}", GlobalVariables.APPLICATION_CODE, GlobalString.VersionApp, (double)((double)watch2.ElapsedMilliseconds / (double)1000), "HIS.Desktop", "LoginApp:LoadServiceRooms,...", Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName(), StringUtil.GetIpLocal(), StringUtil.CustomerCode));
                }
                ProcessDll();
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();

                for (; (i <= 100); i++)
                {
                    LoadingMain.SetReportProgress(UCLoading, i);
                    System.Threading.Thread.Sleep(5);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void ProcessDll()
        {
            try
            {
                i = 0;
                GlobalVariables.LoadType = GlobalVariables.Function.ThietLapMenu;
                LoadDataWorkerLog(ref i, 0, 1, "ThietLapMenu");
                var watch3 = System.Diagnostics.Stopwatch.StartNew();
                List<Action> methodsSystemArgs = new List<Action>();
                methodsSystemArgs.Add(InitGlobalVariableInRam);
                methodsSystemArgs.Add(InitOtherReferenceConstant);
                LoadDataWorkerLog(ref i, 0, 1, "ThietLapMenu");
                methodsSystemArgs.Add(PluginsLoad);
                LoadDataWorkerLog(ref i, 0, 1, "ThietLapMenu");
                ThreadCustomManager.MultipleThreadWithJoin(methodsSystemArgs);
                watch3.Stop();
                Inventec.Common.Logging.LogAction.Info(String.Format("{0}____{1}____{2}____{3}____{4}____{5}____{6}____{7}", GlobalVariables.APPLICATION_CODE, GlobalString.VersionApp, (double)((double)watch3.ElapsedMilliseconds / (double)1000), "HIS.Desktop", "LoginApp: Thiết lập tham số hệ thống, Nạp Module, Nạp Mps", Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName(), StringUtil.GetIpLocal(), StringUtil.CustomerCode));

                //GlobalVariables.LoadType = GlobalVariables.Function.NapModule;
                //LoadDataWorkerLog(ref i, 0, 1, "NapModule");
                //var watch4 = System.Diagnostics.Stopwatch.StartNew();
                //List<Action> methodsModuleFrd = new List<Action>();
                //methodsModuleFrd.Add(this.SetModuleToGlobalVariables);
                //methodsModuleFrd.Add(this.FrdLoadFactory);
                //ThreadCustomManager.MultipleThreadWithJoin(methodsModuleFrd);
                //watch4.Start();
                //Inventec.Common.Logging.LogAction.Info(String.Format("{0}____{1}____{2}____{3}____{4}____{5}____{6}____{7}", GlobalVariables.APPLICATION_CODE, GlobalString.VersionApp, (double)((double)watch4.ElapsedMilliseconds / (double)1000), "HIS.Desktop", "LoginApp: Nạp chức năng", Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName(), StringUtil.GetIpLocal(), StringUtil.CustomerCode));

                //GlobalVariables.LoadType = GlobalVariables.Function.NapMps;
                //var watch5 = System.Diagnostics.Stopwatch.StartNew();
                //LoadDataWorkerLog(ref i, 0, 1, "NapMps");
                //List<Action> methodsMps = new List<Action>();
                //methodsMps.Add(this.MpsPrinterLoadFactory);
                //ThreadCustomManager.MultipleThreadWithJoin(methodsMps);
                //watch5.Start();
                //Inventec.Common.Logging.LogAction.Info(String.Format("{0}____{1}____{2}____{3}____{4}____{5}____{6}____{7}", GlobalVariables.APPLICATION_CODE, GlobalString.VersionApp, (double)((double)watch5.ElapsedMilliseconds / (double)1000), "HIS.Desktop", "LoginApp: Nạp Mps", Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName(), StringUtil.GetIpLocal(), StringUtil.CustomerCode));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void DoWork()
        {
            try
            {
                i = 0;
                var watch2 = System.Diagnostics.Stopwatch.StartNew();
                GlobalVariables.LoadType = GlobalVariables.Function.TaiDuLieuCauHinh;
                LoadDataWorkerLog(ref i, 0, 1, "TaiDuLieuCauHinh");
                ConnectToSever();
                InitConfigWorker();
                LoadDataWorkerLog(ref i, 0, 1, "TaiDuLieuCauHinh");
                ChoiceRoomForSessionLogin();
                LoadDataWorkerLog(ref i, 0, 1, "InitDataCacheLocal");
                LoadDataWorkerLog(ref i, 0, 1, "methods");
                List<Action> methods = new List<Action>();
                methods.Add(BackendDataWorker.InitDataCacheLocal);
                methods.Add(LoadServiceRooms);
                methods.Add(LoadUseRooms);
                methods.Add(LoadServices);
                methods.Add(LoadServiceFollows);
                methods.Add(LoadServicePatys);
                methods.Add(LoadIcds);
                methods.Add(LoadHeinMediOrgs);
                methods.Add(menu.Init);
                ThreadCustomManager.MultipleThreadWithJoin(methods);
                for (; (i <= 100); i++)
                {
                    LoadingMain.SetReportProgress(UCLoading, i);
                    System.Threading.Thread.Sleep(5);
                }
                watch2.Stop();
                Inventec.Common.Logging.LogAction.Info(String.Format("{0}____{1}____{2}____{3}____{4}____{5}____{6}____{7}", GlobalVariables.APPLICATION_CODE, GlobalString.VersionApp, (double)((double)watch2.ElapsedMilliseconds / (double)1000), "HIS.Desktop", "LoginApp:LoadServiceRooms,...", Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName(), StringUtil.GetIpLocal(), StringUtil.CustomerCode));
                ProcessDll();
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void SetModuleToGlobalVariables()
        {
            try
            {
                GlobalVariables.LoadType = GlobalVariables.Function.NapModule;
                this.menu.SetModuleToGlobalVariables((currentNumber) =>
                {
                    GlobalVariables.CurrentNumberModule += currentNumber;
                    LoadingMain.SetReportProgress(UCLoading, 1);
                    if (GlobalVariables.CurrentNumberModule == GlobalVariables.NumTotalModule)
                        GlobalVariables.LoadType = GlobalVariables.Function.NapMps;
                    else if (GlobalVariables.CurrentNumberMps == GlobalVariables.NumTotalMps)
                    {
                        GlobalVariables.LoadType = GlobalVariables.Function.NapModule;
                    }
                }, (TotalModule) =>
                    {
                        GlobalVariables.NumTotalModule += TotalModule;
                    });
                GlobalVariables.LoadType = GlobalVariables.Function.NapMps;
                LoadDataWorkerLog(ref i, 0, 1, "SetModuleToGlobalVariables");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void PluginsLoad()
        {
            try
            {
                menu.ProcessDataModule();
                List<Action> methodsModuleFrd = new List<Action>();
                methodsModuleFrd.Add(this.SetModuleToGlobalVariables);
                methodsModuleFrd.Add(this.FrdLoadFactory);
                methodsModuleFrd.Add(this.MpsPrinterLoadFactory);
                ThreadCustomManager.MultipleThreadWithJoin(methodsModuleFrd);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FrdLoadFactory()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("FrdLoadFactory .1");
                FRD.FrdProcessor process = new FRD.FrdProcessor();//load dll FRD
                GlobalVariables.LoadType = GlobalVariables.Function.NapModule;
                process.Initialize((currentNumber) =>
                {
                    GlobalVariables.CurrentNumberModule += currentNumber;
                    LoadingMain.SetReportProgress(UCLoading, 1);
                    if (GlobalVariables.CurrentNumberModule == GlobalVariables.NumTotalModule)
                        GlobalVariables.LoadType = GlobalVariables.Function.NapMps;
                    else if (GlobalVariables.CurrentNumberMps == GlobalVariables.NumTotalMps)
                    {
                        GlobalVariables.LoadType = GlobalVariables.Function.NapModule;
                    }
                }, (TotalFrd) =>
                    {
                        GlobalVariables.NumTotalModule += TotalFrd;
                    });
                GlobalVariables.LoadType = GlobalVariables.Function.NapMps;
                LoadDataWorkerLog(ref i, 0, 1, "FrdLoadFactory");
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
                MPS.MpsPrinter.LoadFactory((currentNumber) =>
                {
                    GlobalVariables.CurrentNumberMps = currentNumber;
                    LoadingMain.SetReportProgress(UCLoading, 1);
                    if (GlobalVariables.CurrentNumberModule == GlobalVariables.NumTotalModule)
                        GlobalVariables.LoadType = GlobalVariables.Function.NapMps;
                    else if (GlobalVariables.CurrentNumberMps == GlobalVariables.NumTotalMps)
                    {
                        GlobalVariables.LoadType = GlobalVariables.Function.NapModule;
                    }
                }, (TotalMps) =>
                {
                    GlobalVariables.NumTotalMps = TotalMps;
                });//load dll MPS
                GlobalVariables.LoadType = GlobalVariables.Function.NapMps;
                MPS.ProcessorBase.Core.ApiConsumerStore.SarConsumer = ApiConsumer.ApiConsumers.SarConsumer;
                MPS.ProcessorBase.PrintLog.PrintLogProcess.SendSarPrintLog();
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }
        private void RunWorkerCompleted()
        {
            try
            {
                this.Close();
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
                GlobalVariables.LoadType = GlobalVariables.Function.NapModule;
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
                MPS.ProcessorBase.PrintConfig.ShowTutorialModule = ShowTutorialModule;
                MPS.ProcessorBase.PrintConfig.AppCode = GlobalVariables.APPLICATION_CODE;
                MPS.ProcessorBase.PrintConfig.VersionApp = StringUtil.VersionApp;
                MPS.ProcessorBase.PrintConfig.IpLocal = StringUtil.GetIpLocal();
                MPS.ProcessorBase.PrintConfig.CustomerCode = StringUtil.CustomerCode;

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

                ProcessExtMpsServer();
                GlobalVariables.LoadType = GlobalVariables.Function.NapModule;
                LoadDataWorkerLog(ref i, 0, 1, "InitOtherReferenceConstant");
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void ProcessExtMpsServer()
        {
            try
            {
                //MPS.ProcessorBase.PrintConfig.BranchId = BranchDataWorker.GetCurrentBranchId();
                //string strConfigIsUsingMpsServer = HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplicationWorker.Get<string>(AppConfigKeys.CONFIG_KEY__IS_USE_MPS_SERVER);
                //MPS.ProcessorBase.PrintConfig.IsUsingMpsServer = (strConfigIsUsingMpsServer == "1");
                //Inventec.Common.RichEditor.RichEditorConfig.IsUsingMpsServer = (strConfigIsUsingMpsServer == "1");
                //MPS.ProcessorBase.Core.ApiConsumerStore.MpsConsumer = ApiConsumers.MpsConsumer;
                //MPS.ProcessorBase.Core.ApiConsumerStore.MpsConsumer.SetTokenCode(ApiConsumers.SarConsumer.GetTokenCode());
                //Inventec.Common.RichEditor.DAL.UriBaseStore.URI_API_MPS = HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_MPS;
                //Inventec.Common.RichEditor.DAL.ApiConsumerStore.MpsConsumer = MPS.ProcessorBase.Core.ApiConsumerStore.MpsConsumer;
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
    }
}

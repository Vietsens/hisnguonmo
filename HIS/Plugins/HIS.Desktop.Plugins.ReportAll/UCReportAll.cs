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
using System.Windows.Forms;
using System.Linq;
using Inventec.UC.ListReportType;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Desktop.Common.Message;
using Inventec.Core;
using Inventec.Common.Adapter;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Desktop.Common.LanguageManager;
using SAR.Filter;
using SAR.EFMODEL.DataModels;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using System.Threading;
using Inventec.Common.Logging;

namespace HIS.Desktop.Plugins.ReportAll
{
    internal partial class UCReportAll : HIS.Desktop.Utility.UserControlBase
    {
        #region Declare
        UserControl UCListReport;
        UserControl ucReportTypeList;
        UserControl ucReportTypeGroupList;
        CreateReport_Click createReportClick;
        Inventec.UC.ListReports.MainListReports MainListReports;
        Inventec.UC.ListReportType.MainListReportType MainReportTypeList;
        Inventec.UC.ListReportTypeGroup.MainListReportTypeGroup MainReportTypeGroupList;
        List<SAR_REPORT_TYPE> ActiveReports;
        Inventec.Desktop.Common.Modules.Module moduleData;
        #endregion

        #region Construct
        public UCReportAll(CreateReport_Click _createReportClick, Inventec.Desktop.Common.Modules.Module _moduleData)
            : base(_moduleData)
        {
            InitializeComponent();
            try
            {
                createReportClick = _createReportClick;
                moduleData = _moduleData;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public UCReportAll()
        {
            InitializeComponent();
        }
        #endregion

        #region Method
        #region private
        private void UCReportAll_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                ResourceLanguageManagerLoad();

                var tokenData = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetTokenData();
                ApiConsumers.SetConsunmer(tokenData != null ? tokenData.TokenCode : "");
                LoadConfigReportTypeByUser();
                LoadCreateReportConfig();
                InitUcListReports();
                InitUcReportType();
                InitUcReportTypeGroup();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void ResourceLanguageManagerLoad()
        {
            try
            {
                Inventec.UC.ListReportType.Base.ResouceManager.ResourceLanguageManager();
                Inventec.UC.ListReports.Base.ResouceManager.ResourceLanguageManager();
                His.UC.CreateReport.Base.ResouceManager.ResourceLanguageManager();
                HIS.UC.FormType.Base.ResouceManager.ResourceLanguageManager();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadConfigReportTypeByUser()
        {
            try
            {
                WaitingManager.Show();
                CommonParam paramCommon = new CommonParam();
                List<SAR.EFMODEL.DataModels.V_SAR_USER_REPORT_TYPE> userReportTypeByUsers;
                SAR.Filter.SarUserReportTypeViewFilter Userfilter = new SAR.Filter.SarUserReportTypeViewFilter();
                Userfilter.LOGINNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                Userfilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                //Userfilter.IS_DELETE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE;
                userReportTypeByUsers = new BackendAdapter(paramCommon).Get<List<SAR.EFMODEL.DataModels.V_SAR_USER_REPORT_TYPE>>(
                    "api/SarUserReportType/GetView", ApiConsumer.ApiConsumers.SarConsumer, Userfilter, paramCommon)??new List<SAR.EFMODEL.DataModels.V_SAR_USER_REPORT_TYPE>();

                //lọc bỏ các loại báo cáo bị khóa
                SAR.Filter.SarReportTypeFilter typefilter = new SAR.Filter.SarReportTypeFilter();
                typefilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                ActiveReports = new BackendAdapter(paramCommon).Get<List<SAR_REPORT_TYPE>>(
                    "api/SarReportType/Get", ApiConsumer.ApiConsumers.SarConsumer, Userfilter, paramCommon);
                //if (ActiveReports != null && ActiveReports.Count > 0)
                //{
                //    userReportTypeByUsers = userReportTypeByUsers.Where(o => ActiveReports.Select(s => s.ID).Contains(o.REPORT_TYPE_ID)).ToList();
                //}

                var employees = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EMPLOYEE>();
                if (employees != null)
                {
                    var employee = employees.FirstOrDefault(o => o.LOGINNAME == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName());
                    if (employee != null && employee.IS_ADMIN == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                        return;
                }

                ActiveReports = ActiveReports.Where(o => userReportTypeByUsers.Exists(p => p.REPORT_TYPE_ID == o.ID)).ToList();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadCreateReportConfig()
        {
            try
            {
                WaitingManager.Show();
                His.UC.CreateReport.CreateReportConfig.Language = LanguageManager.GetLanguage();
                His.UC.CreateReport.CreateReportConfig.LoginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                His.UC.CreateReport.CreateReportConfig.UserName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
                His.UC.CreateReport.CreateReportConfig.BranchId = HIS.Desktop.LocalStorage.BackendData.BranchDataWorker.GetCurrentBranchId();
                HIS.UC.FormType.FormTypeConfig.BranchId = HIS.Desktop.LocalStorage.BackendData.BranchDataWorker.GetCurrentBranchId();

                HIS.UC.FormType.FormTypeConfig.FormFields = BackendDataWorker.Get<SAR.EFMODEL.DataModels.SAR_FORM_FIELD>().Where(o => o.IS_ACTIVE == 1).ToList();
                His.UC.CreateReport.CreateReportConfig.FormFields = BackendDataWorker.Get<SAR.EFMODEL.DataModels.SAR_FORM_FIELD>().Where(o => o.IS_ACTIVE == 1).ToList();
                var employees = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EMPLOYEE>();
                if (employees != null)
                {
                    HIS.UC.FormType.FormTypeConfig.MyInfo = employees.FirstOrDefault(o => o.LOGINNAME == His.UC.CreateReport.CreateReportConfig.LoginName);
                    //His.UC.CreateReport.CreateReportConfig.MyInfo = HIS.UC.FormType.FormTypeConfig.MyInfo;
                }
                //His.UC.CreateReport.CreateReportConfig.RetyFofis = BackendDataWorker.Get<SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI>().Where(o => o.IS_ACTIVE == 1).ToList();
                His.UC.CreateReport.CreateReportConfig.RetyFofis = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_SAR_RETY_FOFI>>("api/SarRetyFofi/GetView", ApiConsumers.SarConsumer, new SarRetyFofiViewFilter() { IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE }, null);
                HIS.UC.FormType.FormTypeConfig.Language = LanguageManager.GetLanguage();

                //var listUserRoom = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_USER_ROOM>().Where(o => o.LOGINNAME == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName() && o.IS_ACTIVE == 1).ToList();

                //if (listUserRoom != null)
                //{
                //    var obj = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO;
                //    if (obj != null)
                //    {
                //        var listRoomID = obj.Select(o => o.RoomId).ToList();

                //        if (listRoomID.Count > 0)
                //        {
                //            HIS.UC.FormType.Config.HisFormTypeConfig.VHisMediStock = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK>().Where(o => listRoomID.Contains(o.ROOM_ID)).ToList();
                //            HIS.UC.FormType.Config.HisFormTypeConfig.VHisExecuteRooms = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM>().Where(o => listRoomID.Contains(o.ROOM_ID)).ToList();
                //            HIS.UC.FormType.Config.HisFormTypeConfig.VHisBedRooms = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_BED_ROOM>().Where(o => listRoomID.Contains(o.ROOM_ID)).ToList();
                //        }
                //    }
                //}
                HIS.UC.FormType.Config.HisFormTypeConfig.VHisMediStock = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK>();
                HIS.UC.FormType.Config.HisFormTypeConfig.VHisExecuteRooms = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM>();
                HIS.UC.FormType.Config.HisFormTypeConfig.VHisBedRooms = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_BED_ROOM>();
                HIS.UC.FormType.Config.HisFormTypeConfig.HisDepartments = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_DEPARTMENT>();
                HIS.UC.FormType.Config.HisFormTypeConfig.HisKskContracts = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_KSK_CONTRACT>();
                HIS.UC.FormType.Config.HisFormTypeConfig.HisTreatmentTypes = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE>();
                HIS.UC.FormType.Config.HisFormTypeConfig.HisPatientTypes = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>();
                HIS.UC.FormType.Config.HisFormTypeConfig.HisExpMestTypes = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EXP_MEST_TYPE>();
                HIS.UC.FormType.Config.HisFormTypeConfig.HisServiceTypes = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SERVICE_TYPE>();
                HIS.UC.FormType.FormTypeConfig.UserName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
                this.LoadAcsUserForFormType(ApiConsumers.AcsConsumer);
                HIS.UC.FormType.ApiConsumerStore.MosConsumer = ApiConsumers.MosConsumer;
                HIS.UC.FormType.ApiConsumerStore.AcsConsumer = ApiConsumers.AcsConsumer;
                HIS.UC.FormType.ApiConsumerStore.SarConsumer = ApiConsumers.SarConsumer;
                HIS.UC.FormType.ApiConsumerStore.SdaConsumer = ApiConsumers.SdaConsumer;
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadAcsUserForFormType(object acsConsumer)
        {
            try
            {
                CommonParam param = new CommonParam();
                ACS.Filter.AcsUserFilter filter = new ACS.Filter.AcsUserFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                filter.ColumnParams = new List<string>() { "LOGINNAME", "ID", "USERNAME" };
                HIS.UC.FormType.Config.AcsFormTypeConfig.HisAcsUser = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<ACS.EFMODEL.DataModels.ACS_USER>>("api/AcsUser/GetDynamic", (Inventec.Common.WebApiClient.ApiConsumer)acsConsumer, filter, param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitUcListReports()
        {
            try
            {
                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                bool isAdmin = HIS.Desktop.IsAdmin.CheckLoginAdmin.IsAdmin(loginName);
                Inventec.UC.ListReports.Data.InitData dataInit = new Inventec.UC.ListReports.Data.InitData(
                    ApiConsumer.ApiConsumers.SarConsumer,
                    ApiConsumer.ApiConsumers.SdaConsumer,
                    ApiConsumer.ApiConsumers.AcsConsumer,
                    Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager,
                    ConfigApplications.NumPageSize,
                    "APP.ico",
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture(),
                    isAdmin);

                dataInit.ProcessCopy = ProcessReportCopy;
                MainListReports = new Inventec.UC.ListReports.MainListReports();
                UCListReport = new UserControl();
                UCListReport = MainListReports.Init(
                    Inventec.UC.ListReports.MainListReports.EnumTemplate.TEMPLATE3,
                    dataInit);
                UCListReport.Dock = DockStyle.Fill;
                UCReportList.Controls.Add(UCListReport);
                MainListReports.MeShowUC(UCListReport);
                if (!MainListReports.SetDelegateProcessHasException(UCListReport, ProcessHasException))
                    Inventec.Common.Logging.LogSystem.Error("Loi setDelegateProcessHasException cho UCReportList");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessHasException(CommonParam param)
        {
            try
            {
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitUcReportTypeGroup()
        {
            try
            {
                WaitingManager.Show();
                Inventec.UC.ListReportTypeGroup.Data.InitData dataInit = new Inventec.UC.ListReportTypeGroup.Data.InitData(
                    (int)ConfigApplications.NumPageSize, UpdateReportTypeGroup);
                MainReportTypeGroupList = new Inventec.UC.ListReportTypeGroup.MainListReportTypeGroup();
                ucReportTypeGroupList = new UserControl();
                ucReportTypeGroupList = MainReportTypeGroupList.Init(
                    Inventec.UC.ListReportTypeGroup.MainListReportTypeGroup.EnumTemplate.TEMPLATE1, dataInit);
                ucReportTypeGroupList.Dock = DockStyle.Fill;
                MainReportTypeGroupList.SetDelegateCreateReport(ucReportTypeGroupList, ReportTypeGroupRowCellClick);
                UCReportTypeGroup.Controls.Add(ucReportTypeGroupList);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitUcReportType()
        {
            try
            {
                WaitingManager.Show();
                Inventec.UC.ListReportType.Data.InitData dataInit = new Inventec.UC.ListReportType.Data.InitData(
                    (int)ConfigApplications.NumPageSize, UpdateDataForPaging);
                MainReportTypeList = new Inventec.UC.ListReportType.MainListReportType();
                ucReportTypeList = new UserControl();
                ucReportTypeList = MainReportTypeList.Init(
                    Inventec.UC.ListReportType.MainListReportType.EnumTemplate.TEMPLATE1, dataInit);
                ucReportTypeList.Dock = DockStyle.Fill;
                MainReportTypeList.SetDelegateCreateReport(ucReportTypeList, CreateReportClick);
                UCReportType.Controls.Add(ucReportTypeList);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<SAR.EFMODEL.DataModels.SAR_REPORT_TYPE> UpdateDataForPaging(
            Inventec.UC.ListReportType.Data.SearchData data,
            object param,
            ref CommonParam resultParam)
        {
            List<SAR.EFMODEL.DataModels.SAR_REPORT_TYPE> resultData = null;
            try
            {
                CommonParam paramCommon = new CommonParam();
                if (ActiveReports != null && ActiveReports.Count > 0)
                {
                    data.KeyWord = (data.KeyWord ?? "");

                    var userReportTypes = ActiveReports.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).GroupBy(p => p.ID).Select(q => q.First()).ToList();
                    var rtSearchs = (
                        from m in userReportTypes
                        where

                        (m.REPORT_TYPE_NAME ?? "").ToLower().Contains(data.KeyWord.ToLower())
                        || (m.REPORT_TYPE_CODE ?? "").ToLower().Contains(data.KeyWord.ToLower())
                        select new SAR_REPORT_TYPE() { ID = m.ID, REPORT_TYPE_CODE = m.REPORT_TYPE_CODE, REPORT_TYPE_NAME = m.REPORT_TYPE_NAME, REPORT_TYPE_GROUP_ID = m.REPORT_TYPE_GROUP_ID }
                            ).Distinct();
                    if (data.ReportTypeGroupId > 0)
                    {
                        rtSearchs = rtSearchs.Where(o => o.REPORT_TYPE_GROUP_ID == data.ReportTypeGroupId).ToList();
                    }
                    resultData = rtSearchs.Skip(((CommonParam)param).Start ?? 0).Take(((CommonParam)param).Limit ?? (int)ConfigApplications.NumPageSize).ToList();
                    resultParam.Limit = resultData.Count;
                    resultParam.Count = rtSearchs.Count();

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return resultData;
        }

        private List<SAR.EFMODEL.DataModels.SAR_REPORT_TYPE_GROUP> UpdateReportTypeGroup(
           Inventec.UC.ListReportTypeGroup.Data.SearchData data,
           object param,
           ref CommonParam resultParam)
        {
            List<SAR.EFMODEL.DataModels.SAR_REPORT_TYPE_GROUP> resultData = null;
            try
            {
                CommonParam paramCommon = new CommonParam();
                SAR.Filter.SarReportTypeGroupFilter filter = new SAR.Filter.SarReportTypeGroupFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                filter.KEY_WORD = data.KeyWord;
                var rs = new BackendAdapter((CommonParam)param).Get<List<SAR.EFMODEL.DataModels.SAR_REPORT_TYPE_GROUP>>("api/SarReportTypeGroup/Get", ApiConsumer.ApiConsumers.SarConsumer, filter, (CommonParam)param);
                if (rs != null)
                {
                    //rs.Add(new SAR.EFMODEL.DataModels.SAR_REPORT_TYPE_GROUP() { REPORT_TYPE_GROUP_NAME = "Tất cả", REPORT_TYPE_GROUP_CODE = "V" });
                    resultData = rs;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return resultData;
        }

        private void ReportTypeGroupRowCellClick(object reportTypeGroup)
        {
            try
            {
                if (reportTypeGroup is SAR.EFMODEL.DataModels.SAR_REPORT_TYPE_GROUP)
                {
                    var data = reportTypeGroup as SAR.EFMODEL.DataModels.SAR_REPORT_TYPE_GROUP;

                    if (MainReportTypeList != null)
                    {
                        MainReportTypeList.ReLoadGridView(ucReportTypeList, data.ID);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CreateReportClick(object reportType)
        {
            try
            {
                if (reportType is SAR.EFMODEL.DataModels.SAR_REPORT_TYPE)
                {
                    var data = reportType as SAR.EFMODEL.DataModels.SAR_REPORT_TYPE;
                    CommonParam paramCommonType = new CommonParam();
                    var listReportType = new Inventec.Common.Adapter.BackendAdapter(paramCommonType).Get<List<SAR_REPORT_TYPE>>("api/SarReportType/Get", ApiConsumers.SarConsumer, new SarReportTypeFilter() { IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE, ID = data.ID }, paramCommonType);
                    if (listReportType != null && listReportType.Count > 0)
                    {
                        data = listReportType.First();
                    }
                    CommonParam paramCommon = new CommonParam();
                    His.UC.CreateReport.CreateReportConfig.ReportTemplates = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<SAR_REPORT_TEMPLATE>>("api/SarReportTemplate/Get", ApiConsumers.SarConsumer, new SarReportTemplateFilter() { IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE, REPORT_TYPE_ID = data.ID }, paramCommon);


                    if (createReportClick != null)
                    {
                        createReportClick(data);
                    }
                    else
                    {
                        frmMainReport frmMainReport = new frmMainReport(data, moduleData);
                        frmMainReport.delegateSearch = Search;
                        //frmMainReport.FormClosing += frmMainReport_FormClosing;
                        frmMainReport.ShowDialog();
                    }
                    #region Process has exception
                    SessionManager.ProcessTokenLost(paramCommon);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmMainReport_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (MainListReports != null)
                    MainListReports.ButtonSearchClick(UCListReport);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region public
        public void Search()
        {
            try
            {
                if (MainListReports != null)
                    MainListReports.ButtonSearchClick(UCListReport);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void Refesh()
        {
            try
            {
                if (MainListReports != null)
                    MainListReports.ButtonRefreshClick(UCListReport);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void ProcessReportCopy(SAR.EFMODEL.DataModels.V_SAR_REPORT report)
        {
            try
            {
                SAR.Filter.SarReportTypeFilter filter = new SAR.Filter.SarReportTypeFilter();
                CommonParam param = new CommonParam();
                filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
                filter.ID = report.REPORT_TYPE_ID;
                var rs = new BackendAdapter(param).Get<List<SAR.EFMODEL.DataModels.SAR_REPORT_TYPE>>(ApiConsumer.SarRequestUriStore.SAR_REPORT_TYPE_GET, ApiConsumer.ApiConsumers.SarConsumer, filter, param);
                if (rs != null && rs.Count > 0)
                {
                    frmMainReport frmMainReport = new frmMainReport(rs[0], report);
                    frmMainReport.delegateSearch = Search;
                    frmMainReport.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion
        #endregion
    }
}

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
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.UC.ListReportType;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using Inventec.Desktop.Common.Message;

namespace HIS.Desktop.Plugins.ReportTypeList
{
  public partial class UCReportTypeList : UserControl
  {
    Inventec.UC.ListReportType.MainListReportType MainReportTypeList;
    UserControl ucReportTypeList;
    CreateReport_Click createReportClick;
    List<SAR.EFMODEL.DataModels.SAR_USER_REPORT_TYPE> userReportTypeByUsers;
    public UCReportTypeList(CreateReport_Click _createReportClick)
    {
      try
      {
        InitializeComponent();
        createReportClick = _createReportClick;
        LoadConfigReportTypeByUser();
        LoadCreateReportConfig();
        MeShow();
        ResourceLanguageManagerLoad();
      }
      catch (Exception ex)
      {
        Inventec.Common.Logging.LogSystem.Error(ex);
      }
    }

    public UCReportTypeList()
    {
      try
      {
        InitializeComponent();
        LoadCreateReportConfig();
        MeShow();
        ResourceLanguageManagerLoad();
      }
      catch (Exception ex)
      {
        Inventec.Common.Logging.LogSystem.Error(ex);
      }
    }

    void LoadConfigReportTypeByUser()
    {
      try
      {
        CommonParam paramCommon = new CommonParam();
        WaitingManager.Show();

        SAR.Filter.SarUserReportTypeFilter Userfilter = new SAR.Filter.SarUserReportTypeFilter();
        Userfilter.LOGINNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
        userReportTypeByUsers = new BackendAdapter(paramCommon)
            .Get<List<SAR.EFMODEL.DataModels.SAR_USER_REPORT_TYPE>>("api/SarUserReportType/Get", ApiConsumers.SarConsumer, Userfilter, paramCommon);
        WaitingManager.Hide();

      }
      catch (Exception ex)
      {
        WaitingManager.Hide();

        Inventec.Common.Logging.LogSystem.Error(ex);
      }
    }

    public void MeShow()
    {
      try
      {
        WaitingManager.Show();
        Inventec.UC.ListReportType.Data.InitData dataInit = new Inventec.UC.ListReportType.Data.InitData(ConfigApplications.NumPageSize, UpdateDataForPaging);
        MainReportTypeList = new Inventec.UC.ListReportType.MainListReportType();
        ucReportTypeList = new UserControl();
        ucReportTypeList = MainReportTypeList.Init(Inventec.UC.ListReportType.MainListReportType.EnumTemplate.TEMPLATE1, dataInit);
        ucReportTypeList.Dock = DockStyle.Fill;
        MainReportTypeList.SetDelegateCreateReport(ucReportTypeList, CreateReportClick);
        this.Controls.Add(ucReportTypeList);
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
        His.UC.CreateReport.Base.ResouceManager.ResourceLanguageManager();
        HIS.UC.FormType.Base.ResouceManager.ResourceLanguageManager();
      }
      catch (Exception ex)
      {
        Inventec.Common.Logging.LogSystem.Error(ex);
      }
    }

    private void LoadCreateReportConfig()
    {
      try
      {
        WaitingManager.Show();
        //Create report constant
        His.UC.CreateReport.CreateReportConfig.Language = LanguageManager.GetLanguage();
        His.UC.CreateReport.CreateReportConfig.LoginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
        His.UC.CreateReport.CreateReportConfig.UserName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
        His.UC.CreateReport.CreateReportConfig.ReportTypes = BackendDataWorker.Get<SAR.EFMODEL.DataModels.SAR_REPORT_TYPE>();
        His.UC.CreateReport.CreateReportConfig.ReportTemplates = BackendDataWorker.Get<SAR.EFMODEL.DataModels.SAR_REPORT_TEMPLATE>();
        His.UC.CreateReport.CreateReportConfig.FormFields = BackendDataWorker.Get<SAR.EFMODEL.DataModels.SAR_FORM_FIELD>();
        His.UC.CreateReport.CreateReportConfig.RetyFofis = BackendDataWorker.Get<SAR.EFMODEL.DataModels.V_SAR_RETY_FOFI>();

        HIS.UC.FormType.FormTypeConfig.HisAcsUser = BackendDataWorker.Get<ACS.EFMODEL.DataModels.ACS_USER>();
        HIS.UC.FormType.FormTypeConfig.FormFields = BackendDataWorker.Get<SAR.EFMODEL.DataModels.SAR_FORM_FIELD>();
        HIS.UC.FormType.FormTypeConfig.Language = LanguageManager.GetLanguage();

        //var listUserRoom = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_USER_ROOM>().Where(o => o.LOGINNAME == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName()).ToList();
        
        //if (listUserRoom != null && listUserRoom.Count > 0)
        //{
        //  var listRoomID = listUserRoom.Select(o => o.ROOM_ID).ToList();

        //  HIS.UC.FormType.FormTypeConfig.VHisMediStock = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK>().Where(o => listRoomID.Contains(o.ROOM_ID)).ToList();
        //  HIS.UC.FormType.FormTypeConfig.VHisExecuteRooms = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM>().Where(o => listRoomID.Contains(o.ROOM_ID)).ToList();
        //  HIS.UC.FormType.FormTypeConfig.VHisBedRooms = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_BED_ROOM>().Where(o => listRoomID.Contains(o.ROOM_ID)).ToList();
        //}

        var obj = HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkPlaceSDO;
        if (obj != null)
        {
          var listRoomID = obj.Select(o => o.RoomId).ToList();

          HIS.UC.FormType.FormTypeConfig.VHisMediStock = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK>().Where(o => listRoomID.Contains(o.ROOM_ID)).ToList();
          Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("V_Hismedistock", HIS.UC.FormType.FormTypeConfig.VHisMediStock));
          HIS.UC.FormType.FormTypeConfig.VHisExecuteRooms = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_EXECUTE_ROOM>().Where(o => listRoomID.Contains(o.ROOM_ID)).ToList();
          HIS.UC.FormType.FormTypeConfig.VHisBedRooms = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_BED_ROOM>().Where(o => listRoomID.Contains(o.ROOM_ID)).ToList();
        }

        HIS.UC.FormType.FormTypeConfig.HisDepartments = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_DEPARTMENT>();
        HIS.UC.FormType.FormTypeConfig.HisKskContracts = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_KSK_CONTRACT>();
        HIS.UC.FormType.FormTypeConfig.HisTreatmentTypes = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE>();        
        HIS.UC.FormType.FormTypeConfig.HisPatientTypes = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>();
        HIS.UC.FormType.FormTypeConfig.HisExpMestTypes = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EXP_MEST_TYPE>();
        HIS.UC.FormType.FormTypeConfig.HisServiceTypes = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_SERVICE_TYPE>();
        HIS.UC.FormType.FormTypeConfig.UserName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
        HIS.UC.FormType.FormTypeConfig.MosConsumer = ApiConsumers.MosConsumer;

        ////Event log
        //HIS.UC.FormType.FormTypeConfig.HisMediStockPeriods = LOGIC.LocalStore.HisDataLocalStore.VHisMediStockPeriod;
        //HIS.UC.FormType.FormTypeConfig.HisTreatments = LOGIC.LocalStore.HisDataLocalStore.HisTreatments;
        //HIS.UC.FormType.FormTypeConfig.VHisMedicineTypes = LOGIC.LocalStore.HisDataLocalStore.VHisMedicineTypes;
        //HIS.UC.FormType.FormTypeConfig.VHisMaterialTypes = LOGIC.LocalStore.HisDataLocalStore.VHisMaterialTypes;
        //HIS.UC.FormType.FormTypeConfig.VHisMedicineBeans = LOGIC.LocalStore.HisDataLocalStore.VHisMedicineBeans;
        //HIS.UC.FormType.FormTypeConfig.VHisMaterialBeans = LOGIC.LocalStore.HisDataLocalStore.VHisMaterialBeans;
        WaitingManager.Hide();
      }
      catch (Exception ex)
      {
        WaitingManager.Hide();
        Inventec.Common.Logging.LogSystem.Error(ex);
      }
    }

    void CreateReportClick(object reportType)
    {
      try
      {
        if (reportType is SAR.EFMODEL.DataModels.SAR_REPORT_TYPE)
        {

          var data = reportType as SAR.EFMODEL.DataModels.SAR_REPORT_TYPE;
          if (createReportClick != null)
          {
            createReportClick(data);
          }
          else
          {
            HIS.UC.FormType.FormTypeConfig.ReportHight = 120;

            frmMainReport frmMainReport = new frmMainReport(data);
            frmMainReport.ShowDialog();
          }
        }
      }
      catch (Exception ex)
      {
        Inventec.Common.Logging.LogSystem.Error(ex);
      }
    }

    List<SAR.EFMODEL.DataModels.SAR_REPORT_TYPE> UpdateDataForPaging(Inventec.UC.ListReportType.Data.SearchData data, object param, ref CommonParam resultParam)
    {
      List<SAR.EFMODEL.DataModels.SAR_REPORT_TYPE> resultData = null;
      try
      {
        CommonParam paramCommon = new CommonParam();
        if (userReportTypeByUsers != null && userReportTypeByUsers.Count > 0)
        {
          data.KeyWord = (data.KeyWord ?? "");
          var ids = userReportTypeByUsers.Select(o => o.REPORT_TYPE_ID).ToList();
          if (ids.Count > 100)
          {
            var reportTypes = BackendDataWorker.Get<SAR.EFMODEL.DataModels.SAR_REPORT_TYPE>();
            var rtSearchs = (
                from m in reportTypes
                from n in userReportTypeByUsers
                where m.ID == n.REPORT_TYPE_ID
                && (
                (m.REPORT_TYPE_NAME ?? "").ToLower().Contains(data.KeyWord.ToLower())
                || (m.REPORT_TYPE_CODE ?? "").ToLower().Contains(data.KeyWord.ToLower())
                )
                select m
                    ).Distinct();

            resultData = rtSearchs.Skip(((CommonParam)param).Start ?? 0).Take(((CommonParam)param).Limit ?? (int)ConfigApplications.NumPageSize).ToList();
            resultParam.Limit = resultData.Count;
            resultParam.Count = rtSearchs.Count();

            //SAR.Filter.SarReportTypeFilter filter = new SAR.Filter.SarReportTypeFilter();
            //filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
            //filter.KEY_WORD = data.KeyWord;

            //filter.IDs = userReportTypeByUsers.Select(o => o.REPORT_TYPE_ID).ToList();
            //var rs = new BackendAdapter((CommonParam)param).GetRO<List<SAR.EFMODEL.DataModels.SAR_REPORT_TYPE>>(SarRequestUriStore.SAR_REPORT_TYPE_GET, ApiConsumers.SarConsumer, filter, (CommonParam)param);
            //if (rs != null && rs.Data != null)
            //{
            //    resultData = rs.Data;
            //    resultParam.Limit = rs.Data.Count;
            //    resultParam.Count = rs.Param.Count;
            //}
          }
          else
          {
            SAR.Filter.SarReportTypeFilter filter = new SAR.Filter.SarReportTypeFilter();
            filter.IS_ACTIVE = IMSys.DbConfig.SAR_RS.COMMON.IS_ACTIVE__TRUE;
            filter.KEY_WORD = data.KeyWord;
            filter.IDs = userReportTypeByUsers.Select(o => o.REPORT_TYPE_ID).ToList();
            var rs = new BackendAdapter((CommonParam)param).GetRO<List<SAR.EFMODEL.DataModels.SAR_REPORT_TYPE>>(SarRequestUriStore.SAR_REPORT_TYPE_GET, ApiConsumers.SarConsumer, filter, (CommonParam)param);
            if (rs != null && rs.Data != null)
            {
              resultData = rs.Data.Distinct().ToList();
              resultParam.Limit = rs.Data.Count;
              resultParam.Count = rs.Param.Count;
            }
          }
        }
      }
      catch (Exception ex)
      {
        Inventec.Common.Logging.LogSystem.Error(ex);
      }

      return resultData;
    }

    public void Search()
    {
      try
      {
        //MainReportTypeList.Search(ucReportTypeList);
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
        //MainReportTypeList.Refesh(ucReportTypeList);
      }
      catch (Exception ex)
      {
        Inventec.Common.Logging.LogSystem.Error(ex);
      }
    }
  }
}

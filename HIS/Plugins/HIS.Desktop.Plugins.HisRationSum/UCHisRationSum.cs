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
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.ApiConsumer;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.LocalStorage.LocalData;
using MOS.SDO;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.WebApiClient;
using HIS.Desktop.Plugins.HisRationSum.ADO;
using Inventec.Common.Controls.EditorLoader;
using MOS.EFMODEL.DataModels;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.Plugins.HisRationSum.Base;
using DevExpress.Utils;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid.Columns;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.Utilities.Extensions;
using Inventec.Common.Adapter;
using HIS.Desktop.Utility;
using DevExpress.XtraGrid.Views.Grid;
using MOS.ServicePaty;
using MOS.Filter;

namespace HIS.Desktop.Plugins.HisRationSum
{
    public partial class UCHisRationSum : HIS.Desktop.Utility.UserControlBase
    {
        #region Declare
        int rowCountRationSum = 0, rowCountServiceReq = 0;
        int dataTotalRationSum = 0, dataTotalServiceReq = 0;
        int startPageRationSum = 0, startPageServiceReq = 0, startPageSereServ = 0;
        private string LoggingName = "";
        long roomId = 0;
        long roomTypeId = 0;
        int positionHandleControl = -1;
        List<HIS_RATION_TIME> ListRationTime = new List<HIS_RATION_TIME>();

        ToolTipControlInfo lastInfo = null, lastInfoRationSum = null;
        int lastRowHandle = -1, lastRowHandleRationSum = -1;
        GridColumn lastColumn = null, lastColumnRationSum = null;
        string logginName = "";
        bool isAddmin = false;
        bool isFistOpen = true;
        List<V_HIS_SERE_SERV_15> ListSereServs;
        List<HIS_DEPARTMENT> _DepartmentSelecteds;
        List<HIS_REFECTORY> _RefectorySelecteds;
        List<V_HIS_ROOM> _RoomSelecteds;
        V_HIS_ROOM currentRoom;
        HIS_DEPARTMENT currentDepartment;
        V_HIS_SERVICE_REQ_10 currentData;

        #endregion

        #region Construct
        public UCHisRationSum()
        {
            InitializeComponent();
            try
            {
                ResourceLangManager.InitResourceLanguageManager();
                LoggingName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public UCHisRationSum(Inventec.Desktop.Common.Modules.Module _moduleData)
            : base(_moduleData)
        {
            InitializeComponent();
            try
            {
                ResourceLangManager.InitResourceLanguageManager();
                LoggingName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                this.roomId = _moduleData.RoomId;
                this.roomTypeId = _moduleData.RoomTypeId;
                this.isFistOpen = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void InitComboRationTime()
        {
            try
            {
                CommonParam param = new CommonParam();

                HisRationTimeFilter filter = new HisRationTimeFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                lstRationTime = new BackendAdapter(param).Get<List<HIS_RATION_TIME>>("api/HisRationTime/Get", ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        private void UCHisRationSum_Load(object sender, EventArgs e)
        {
            try
            {
                InitComboRationTime();
                //Gan ngon ngu
                SetCaptionByLanguageKey();


                // init controls
                InitControl();

                //Gan gia tri mac dinh
                SetDefaultValueControl();

                //Load du lieu
                FillDataToGridRationSum();

                btnSearch_Click(null, null);

                //gridViewRationSum_RowClick(null, null);
                //this.isFistOpen = true;

                gridControlServiceReq.ToolTipController = toolTipControllerServiceReq;
                gridControlRationSum.ToolTipController = toolTipControllerRationSum;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Private method

        private void SelectionGrid__Department(object sender, EventArgs e)
        {
            try
            {
                _DepartmentSelecteds = new List<HIS_DEPARTMENT>();
                foreach (HIS_DEPARTMENT rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        _DepartmentSelecteds.Add(rv);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__Refectory(object sender, EventArgs e)
        {
            try
            {
                _RefectorySelecteds = new List<HIS_REFECTORY>();
                foreach (HIS_REFECTORY rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        _RefectorySelecteds.Add(rv);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__Room(object sender, EventArgs e)
        {
            try
            {
                _RoomSelecteds = new List<V_HIS_ROOM>();
                foreach (V_HIS_ROOM r in (sender as GridCheckMarksSelection).Selection)
                {
                    if (r != null)
                        _RoomSelecteds.Add(r);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitControl()
        {
            try
            {
                currentRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.roomId);
                List<HIS_REFECTORY> refectoryList = GetRefectory();

                if (currentRoom != null)
                {
                    currentDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == currentRoom.DEPARTMENT_ID);
                    if (currentDepartment.IS_CLINICAL == 1 && currentRoom.ROOM_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__NA)
                    {
                        _DepartmentSelecteds = new List<HIS_DEPARTMENT>();
                        _DepartmentSelecteds.Add(currentDepartment);
                        InitCombo(cboRequestDepartment, _DepartmentSelecteds, "DEPARTMENT_NAME", "ID");
                        cboRequestDepartment.Enabled = false;
                        cboRequestDepartment.EditValue = currentDepartment.ID;

                        InitCheck(cboRefectory, SelectionGrid__Refectory);

                        InitCombo(cboRefectory, refectoryList.Where(o => o.IS_ACTIVE == 1).ToList(), "REFECTORY_NAME", "ID");

                        cboRefectory.Enabled = false;
                        cboRefectory.Enabled = true;
                    }
                    else
                    {
                        InitCheck(cboRequestDepartment, SelectionGrid__Department);

                        InitCombo(cboRequestDepartment, BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.IS_CLINICAL == 1 && o.IS_ACTIVE == 1).ToList(), "DEPARTMENT_NAME", "ID");
                        cboRequestDepartment.Enabled = false;
                        cboRequestDepartment.Enabled = true;

                        var currentRefectory = refectoryList.FirstOrDefault(o => o.ROOM_ID == currentRoom.ID);

                        _RefectorySelecteds = new List<HIS_REFECTORY>();
                        _RefectorySelecteds.Add(currentRefectory);
                        InitCombo(cboRefectory, _RefectorySelecteds, "REFECTORY_NAME", "ID");
                        cboRefectory.Enabled = false;
                        cboRefectory.EditValue = currentRefectory.ID;
                    }

                }
                InitComboRoom();

                this.ListRationTime = LoadDataToComboStatus();
                InitComboRationTime(this.ListRationTime);
                //InitComboDepartment(BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList());
                this.logginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                this.isAddmin = CheckIsAddmin();

                //InitComboRefectory(refectoryList);

                //InitCheck(cboLoai, SelectionGrid__Loai);
                //InitCombo(cboLoai);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboRoom()
        {
            try
            {
                List<V_HIS_ROOM> roomList = GetRoomList();

                InitCheck(cboRoom, SelectionGrid__Room);
                InitCombo(cboRoom, roomList, "ROOM_NAME", "ID");
                cboRoom.Enabled = false;
                cboRoom.Enabled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool CheckIsAddmin()
        {
            bool result = false;
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisEmployeeFilter filter = new MOS.Filter.HisEmployeeFilter();
                filter.LOGINNAME__EXACT = this.logginName;
                var employee = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_EMPLOYEE>>("api/HisEmployee/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
                if (employee != null && employee.Count() > 0 && employee.FirstOrDefault().IS_ADMIN == 1)
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void LoadKeysFromlanguage()
        {
            try
            {
                var cultureLang = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();

                //gridView

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<MOS.EFMODEL.DataModels.HIS_RATION_TIME> LoadDataToComboStatus()
        {
            List<HIS_RATION_TIME> Result = new List<HIS_RATION_TIME>();
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisRationTimeFilter filter = new MOS.Filter.HisRationTimeFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "CREATE_TIME";
                Result = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_RATION_TIME>>("api/HisRationTime/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex)
            {
                Result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return Result;
        }

        private List<HIS_REFECTORY> GetRefectory()
        {
            List<HIS_REFECTORY> result = null;
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisRefectoryFilter filter = new MOS.Filter.HisRefectoryFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                result = new BackendAdapter(param).Get<List<HIS_REFECTORY>>("api/HisRefectory/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private List<V_HIS_ROOM> GetRoomList()
        {
            List<V_HIS_ROOM> result = null;
            try
            {
                List<long> listIdDepartmentSelecteds = new List<long>();
                if (_DepartmentSelecteds != null && _DepartmentSelecteds.Count > 0)
                {
                    listIdDepartmentSelecteds = this._DepartmentSelecteds.Select(o => o.ID).ToList();
                }
                var data = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>();
                data = data != null ? data.Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE &&
                                                o.ROOM_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__BUONG &&
                                                listIdDepartmentSelecteds.Contains(o.DEPARTMENT_ID)).ToList() : null;
                result = data;
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void InitComboRefectory(List<HIS_REFECTORY> datas)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("REFECTORY_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("REFECTORY_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("REFECTORY_NAME", "ROOM_ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboRefectory, datas, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboRationTime(List<HIS_RATION_TIME> datas)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("RATION_TIME_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("RATION_TIME_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("RATION_TIME_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboRationTime, datas, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void RefreshSearch()
        {
            try
            {
                gridControlServiceReqRationSumDetail.BeginUpdate();
                gridControlServiceReqRationSumDetail.DataSource = null;
                gridControlServiceReqRationSumDetail.EndUpdate();
                gridViewServiceReq.BeginDataUpdate();
                gridControlServiceReq.DataSource = null;
                gridViewServiceReq.EndDataUpdate();
                gridViewSereServ.BeginDataUpdate();
                gridControlSereServ.DataSource = null;
                gridViewSereServ.EndUpdate();

                rowCountRationSum = 0; rowCountServiceReq = 0;
                dataTotalRationSum = 0; dataTotalServiceReq = 0;
                startPageRationSum = 0; startPageServiceReq = 0; startPageSereServ = 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultValueControl()
        {
            try
            {
                DateTime now = DateTime.Now;
                var startDate = new DateTime(now.Year, now.Month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);


                dtIntructionDateServiceReqFrom.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.StartDay() ?? 0) ?? DateTime.Now;
                dtIntructionDateServiceReqTo.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.EndDay() ?? 0) ?? DateTime.Now;

                txtKeyword.Text = "";
                radioHasNotSum.CheckState = CheckState.Checked;
                RadioHasSum.CheckState = CheckState.Unchecked;
                cboRefectory.EditValue = null;
                cboRationTime.EditValue = null;
                ResetCombo(cboRequestDepartment);
                ResetComboRoom();

                RefreshSearch();
                if(roomTypeId == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__BUONG)
                {
                    lciApproveRation.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                }
                else
                {
                    lciApproveRation.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ResetComboRoom()
        {
            try
            {
                GridCheckMarksSelection gridCheckMark = cboRoom.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboRoom.Properties.View);
                }
                cboRoom.Enabled = false;
                cboRoom.Enabled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void FillDataToGridRationSum()
        {
            try
            {
                WaitingManager.Show();
                int pagingSize = ucPagingRationSum.pagingGrid != null ? ucPagingRationSum.pagingGrid.PageSize : (int)ConfigApplications.NumPageSize;
                GridPagingRationSum(new CommonParam(0, pagingSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCountRationSum;
                param.Count = dataTotalRationSum;
                ucPagingRationSum.Init(GridPagingRationSum, param, pagingSize, this.gridControlRationSum);
                WaitingManager.Hide();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void FillDataToGridServiceReq()
        {
            try
            {
                WaitingManager.Show();
                try
                {
                    CommonParam paramCommon = new CommonParam();
                    List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_10> apiResult = null;
                    MOS.Filter.HisServiceReqView10Filter filter = new MOS.Filter.HisServiceReqView10Filter();
                    SetFilterServiceReq(ref filter);
                    gridViewServiceReq.BeginUpdate();
                    apiResult = new Inventec.Common.Adapter.BackendAdapter
                        (paramCommon).Get<List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_10>>
                        ("api/HisServiceReq/GetView10", ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                    if (apiResult != null)
                    {
                        Inventec.Common.Logging.LogUtil.TraceData(" API RESULT: " + Inventec.Common.Logging.LogUtil.GetMemberName(() => apiResult), apiResult);
                        if (apiResult != null && apiResult.Count > 0)
                        {
                            apiResult = apiResult.OrderBy(o => o.INTRUCTION_DATE).ThenBy(o => o.RATION_TIME_ID).ThenBy(q => q.TDL_PATIENT_NAME).ThenBy(o => o.REQUEST_DEPARTMENT_ID).ToList();
                            gridControlServiceReq.DataSource = apiResult;
                        }
                        else
                        {
                            gridControlServiceReq.DataSource = null;
                        }
                    }
                    else
                    {
                        gridControlServiceReq.DataSource = null;
                    }
                    gridViewServiceReq.EndUpdate();

                    #region Process has exception
                    HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramCommon);
                    #endregion
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                    gridViewServiceReq.EndUpdate();
                }
                WaitingManager.Hide();

                gridViewServiceReq.SelectAll();
                FillDataToGridSereServ();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private V_HIS_SERVICE_PATY GetServicePaty(V_HIS_SERE_SERV_RATION ssRation, List<V_HIS_SERVICE_PATY> servicePaties)
        {
            V_HIS_SERVICE_PATY servicePaty = new V_HIS_SERVICE_PATY();
            try
            {
                if (ssRation != null)
                {
                    servicePaty = MOS.ServicePaty.ServicePatyUtil.GetApplied(
                        servicePaties, 
                        WorkPlace.GetBranchId(), 
                        (ssRation.EXECUTE_ROOM_ID > 0 ? (long?)ssRation.EXECUTE_ROOM_ID : null),
                        (ssRation.REQUEST_ROOM_ID > 0 ? (long?)ssRation.REQUEST_ROOM_ID : null),
                        (ssRation.REQUEST_DEPARTMENT_ID > 0 ? (long?)ssRation.REQUEST_DEPARTMENT_ID : null),
                        ssRation.INTRUCTION_TIME,
                        ssRation.IN_TIME,
                        ssRation.SERVICE_ID,
                        ssRation.PATIENT_TYPE_ID, 
                        null, 
                        null, 
                        null,
                        null,
                        ssRation.TDL_PATIENT_CLASSIFY_ID,
                        ssRation.RATION_TIME_ID);
                    return servicePaty;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return servicePaty;
        }

        private void FillDataToGridSereServ()
        {
            try
            {
                WaitingManager.Show();
                CommonParam paramCommon = new CommonParam();
                MOS.Filter.HisSereServRationViewFilter filter = new MOS.Filter.HisSereServRationViewFilter();
                SetFilterSereServRation(ref filter);
                if (filter != null && (filter.SERVICE_REQ_IDs == null || filter.SERVICE_REQ_IDs.Count() == 0))
                {
                    gridViewSereServ.BeginDataUpdate();
                    gridControlSereServ.DataSource = null;
                    gridViewSereServ.EndDataUpdate();

                    gridView3.BeginDataUpdate();
                    gridControl1.DataSource = null;
                    gridView3.EndDataUpdate();
                    WaitingManager.Hide();
                    return;
                }
                List<V_HIS_SERVICE_PATY> servicePaties = BackendDataWorker.Get<V_HIS_SERVICE_PATY>().ToList();
                gridViewSereServ.BeginUpdate();
                gridView3.BeginDataUpdate();
                var datas = new Inventec.Common.Adapter.BackendAdapter
                    (paramCommon).Get<List<V_HIS_SERE_SERV_RATION>>
                    ("api/HisSereServRation/GetView", ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                if (datas != null && datas.Count > 0)
                {
                    foreach (var item in datas)
                    {
                        var servicePaty = GetServicePaty(item, servicePaties);
                        if (servicePaty != null)
                            item.PRICE = servicePaty.PRICE;
                    }
                    List<SereServADO> dataSource = GroupSereServ(datas);
                    dataSource = (dataSource != null && dataSource.Count() > 0)
                        ? dataSource.OrderBy(q => q.SERVICE_NAME).ToList() : dataSource;

                    gridControlSereServ.DataSource = dataSource;
                    List<SereServADO> result = new List<SereServADO>();
                    foreach (var group in datas)
                    {
                        SereServADO sereServADO = new SereServADO(group);
                        sereServADO.AMOUNT_SUM = group.AMOUNT;
                        result.Add(sereServADO);
                    }
                    gridControl1.DataSource = result;
                }
                else
                {
                    gridControlSereServ.DataSource = null;
                    gridControl1.DataSource = null;
                }
                gridViewSereServ.EndUpdate();
                gridView3.EndDataUpdate();
                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramCommon);
                #endregion
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        List<long> rationSumIdList = null;
        private void GetServiceReqByRationSum()
        {
            try
            {
                WaitingManager.Show();
                int pagingSize = ucPagingServiceReqRationSumDetail.pagingGrid != null ? ucPagingServiceReqRationSumDetail.pagingGrid.PageSize : (int)ConfigApplications.NumPageSize;
                GridPagingServiceReqRationSumDetail(new CommonParam(0, pagingSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCountServiceReq;
                param.Count = dataTotalServiceReq;
                ucPagingServiceReqRationSumDetail.Init(GridPagingServiceReqRationSumDetail, param, pagingSize, this.gridControlServiceReqRationSumDetail);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GridPagingServiceReqRationSumDetail(object param)
        {
            try
            {
                startPageServiceReq = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPageServiceReq, limit);
                ApiResultObject<List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_10>> apiResult = null;
                MOS.Filter.HisServiceReqView1Filter filter = new MOS.Filter.HisServiceReqView1Filter();
                filter.RATION_SUM_IDs = rationSumIdList;
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                gridViewServiceReqRationSumDetail.BeginUpdate();
                apiResult = new Inventec.Common.Adapter.BackendAdapter
                    (paramCommon).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_10>>
                    ("api/HisServiceReq/GetView1", ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                if (apiResult != null)
                {
                    var data = apiResult.Data;
                    if (data != null && data.Count > 0)
                    {
                        gridControlServiceReqRationSumDetail.DataSource = data;
                        rowCountServiceReq = (data == null ? 0 : data.Count);
                        dataTotalServiceReq = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                    else
                    {
                        gridControlServiceReqRationSumDetail.DataSource = null;
                        rowCountServiceReq = (data == null ? 0 : data.Count);
                        dataTotalServiceReq = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                else
                {
                    rowCountServiceReq = 0;
                    dataTotalServiceReq = 0;
                    gridControlServiceReqRationSumDetail.DataSource = null;
                }
                gridViewServiceReqRationSumDetail.EndUpdate();

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                gridViewServiceReq.EndUpdate();
            }
        }

        private void FillDataToGridServiceReqByRationSum()
        {
            try
            {
                WaitingManager.Show();
                int pagingSize = ucPagingServiceReqRationSumDetail.pagingGrid != null ? ucPagingServiceReqRationSumDetail.pagingGrid.PageSize : (int)ConfigApplications.NumPageSize;
                GridPagingRationSum(new CommonParam(0, pagingSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCountRationSum;
                param.Count = dataTotalRationSum;
                ucPagingRationSum.Init(GridPagingRationSum, param, pagingSize, this.gridControlRationSum);
                WaitingManager.Hide();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void FillDataToTreeServiceReqGroup(List<V_HIS_SERVICE_REQ_10> ListSereServADO)
        {
            try
            {
                if (ListSereServADO == null || ListSereServADO.Count() == 0)
                {
                    this.gridControlServiceReqRationSumDetail.DataSource = null;
                    return;
                }
                //List<SereServ1ADO> dataSource = new List<SereServ1ADO>();

                //var groupSereServ = ListSereServADO.GroupBy(o => o.TDL_REQUEST_DEPARTMENT_ID).ToArray();

                //foreach (var group in groupSereServ)
                //{
                //    var fistGroup = group.FirstOrDefault();
                //    SereServ1ADO Parent = new SereServ1ADO(fistGroup);
                //    Parent.TITLE_NAME = fistGroup.REQUEST_DEPARTMENT_NAME;
                //    Parent.PARENT_ID_STR = ".";
                //    Parent.CHILD_ID = fistGroup.ID + ".";
                //    Parent.IS_PARENT = 1;
                //    dataSource.Add(Parent);

                //    var groupByService = group.GroupBy(o => new { o.TDL_SERVICE_CODE, o.PRICE });

                //    foreach (var item in groupByService)
                //    {
                //        SereServ1ADO ChildRent = new SereServ1ADO(item.FirstOrDefault());
                //        ChildRent.CHILD_ID = ChildRent.ID + "." + ChildRent.ID;
                //        ChildRent.PARENT_ID_STR = Parent.CHILD_ID;
                //        ChildRent.AMOUNT_SUM = item.Sum(o => o.AMOUNT);

                //        string AmountSumStr = String.Format("{0:0.####}", ChildRent.AMOUNT_SUM);
                //        string totalPrice = Inventec.Common.Number.Convert.NumberToString((ChildRent.AMOUNT_SUM * ChildRent.PRICE), ConfigApplications.NumberSeperator);

                //        ChildRent.TITLE_NAME = ChildRent.TDL_SERVICE_NAME + ": " + AmountSumStr + " suất - " + totalPrice;
                //        dataSource.Add(ChildRent);
                //    }
                //}

                //BindingList<SereServ1ADO> records = new BindingList<SereServ1ADO>(dataSource);
                this.gridControlServiceReqRationSumDetail.RefreshDataSource();
                this.gridControlServiceReqRationSumDetail.DataSource = null;
                this.gridControlServiceReqRationSumDetail.DataSource = ListSereServADO;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitCheck(GridLookUpEdit cbo, GridCheckMarksSelection.SelectionChangedEventHandler eventSelect)
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cbo.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(eventSelect);
                cbo.Properties.Tag = gridCheck;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cbo.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<SereServADO> GroupSereServ(List<V_HIS_SERE_SERV_RATION> input)
        {
            List<SereServADO> result = new List<SereServADO>();
            try
            {
                if (input != null && input.Count() > 0)
                {
                    var groupSS = input.GroupBy(o => new { o.SERVICE_CODE, o.PRICE, o.PATIENT_TYPE_NAME }).ToArray();
                    foreach (var group in groupSS)
                    {
                        var firstItem = group.FirstOrDefault();
                        SereServADO sereServADO = new SereServADO(firstItem);
                        sereServADO.AMOUNT_SUM = group.Sum(o => o.AMOUNT);
                        result.Add(sereServADO);
                    }
                    result = result.OrderBy(o => o.SERVICE_NAME).ToList();
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
        private List<SereServADO> GroupSereServDetail(List<V_HIS_SERE_SERV_RATION> input)
        {
            List<SereServADO> result = new List<SereServADO>();
            try
            {
                if (input != null && input.Count() > 0)
                {
                    var groupSS = input.GroupBy(o => new { o.SERVICE_REQ_CODE, o.TDL_PATIENT_NAME }).ToArray();
                    foreach (var group in groupSS)
                    {
                        var firstItem = group.FirstOrDefault();
                        SereServADO sereServADO = new SereServADO(firstItem);
                        sereServADO.AMOUNT_SUM = group.Sum(o => o.AMOUNT);
                        result.Add(sereServADO);
                    }
                    result = result.OrderBy(o => o.SERVICE_NAME).ToList();
                }
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void GridPagingSereServRation(object param)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                gridViewSereServ.EndUpdate();
            }
        }

        private void GridPagingRationSum(object param)
        {
            try
            {
                startPageRationSum = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPageRationSum, limit);
                ApiResultObject<List<MOS.EFMODEL.DataModels.V_HIS_RATION_SUM>> apiResult = null;
                MOS.Filter.HisRationSumViewFilter filter = new MOS.Filter.HisRationSumViewFilter();
                SetFilterRationSum(ref filter);
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                //filter.ROOM_ID = this.roomId;
                gridViewRationSum.BeginUpdate();
                apiResult = new Inventec.Common.Adapter.BackendAdapter
                    (paramCommon).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_RATION_SUM>>
                    ("api/HisRationSum/GetView", ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                if (apiResult != null)
                {
                    var data = apiResult.Data;
                    if (data != null && data.Count > 0)
                    {
                        gridControlRationSum.DataSource = data;
                        rowCountRationSum = (data == null ? 0 : data.Count);
                        dataTotalRationSum = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                    else
                    {
                        gridControlRationSum.DataSource = null;
                        rowCountRationSum = (data == null ? 0 : data.Count);
                        dataTotalRationSum = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                else
                {
                    rowCountRationSum = 0;
                    dataTotalRationSum = 0;
                    gridControlRationSum.DataSource = null;
                }
                gridViewRationSum.EndUpdate();

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                gridViewServiceReq.EndUpdate();
            }
        }

        private void SetFilterServiceReq(ref MOS.Filter.HisServiceReqView10Filter filter)
        {
            try
            {
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
                filter.KEY_WORD = txtKeyword.Text;

                if (dtIntructionDateServiceReqFrom.EditValue != null && dtIntructionDateServiceReqFrom.DateTime != DateTime.MinValue)
                {
                    filter.INTRUCTION_TIME_FROM = Convert.ToInt64(dtIntructionDateServiceReqFrom.DateTime.ToString("yyyyMMddHHmm") + "00");
                }
                if (dtIntructionDateServiceReqTo.EditValue != null && dtIntructionDateServiceReqTo.DateTime != DateTime.MinValue)
                {
                    filter.INTRUCTION_TIME_TO = Convert.ToInt64(dtIntructionDateServiceReqTo.DateTime.ToString("yyyyMMddHHmm") + "00");
                }

                if (radioHasNotSum.CheckState == CheckState.Checked)
                {
                    filter.HAS_RATION_SUM_ID = false;
                    //filter.EXECUTE_ROOM_ID = this.roomId;
                }
                else if (RadioHasSum.CheckState == CheckState.Checked)
                {
                    filter.HAS_RATION_SUM_ID = true;
                    //filter.EXECUTE_ROOM_ID = this.roomId;
                }
                //else
                //{
                //    filter.EXECUTE_ROOM_ID = null;
                //}

                if (_DepartmentSelecteds != null && _DepartmentSelecteds.Count > 0)
                {
                    filter.REQUEST_DEPARTMENT_IDs = _DepartmentSelecteds.Select(o => o.ID).ToList();
                }
                if (_RefectorySelecteds != null && _RefectorySelecteds.Count > 0)
                {
                    filter.EXECUTE_ROOM_IDs = _RefectorySelecteds.Select(o => o.ROOM_ID).Distinct().ToList();
                }
                if (cboRationTime.EditValue != null)
                {
                    filter.RATION_TIME_ID = Convert.ToInt32(cboRationTime.EditValue.ToString());
                }
                if (_RoomSelecteds != null && _RoomSelecteds.Count > 0)
                {
                    filter.REQUEST_ROOM_IDs = _RoomSelecteds.Select(o => o.ID).Distinct().ToList();
                }
                filter.SERVICE_REQ_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__AN;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetFilterSereServRation(ref MOS.Filter.HisSereServRationViewFilter filter)
        {
            try
            {
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";

                List<V_HIS_SERVICE_REQ_10> serviceReqs = new List<V_HIS_SERVICE_REQ_10>();
                int[] selectRows = gridViewServiceReq.GetSelectedRows();

                if (selectRows != null && selectRows.Count() > 0)//xuandv
                {
                    foreach (var item in selectRows)
                    {
                        var serviceReq = (V_HIS_SERVICE_REQ_10)gridViewServiceReq.GetRow(item);
                        serviceReqs.Add(serviceReq);
                    }
                }

                // filter.SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__AN;

                filter.SERVICE_REQ_IDs = serviceReqs.Select(o => o.ID).Distinct().ToList();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetFilterRationSum(ref MOS.Filter.HisRationSumViewFilter filter)
        {
            try
            {
                filter.KEY_WORD = txtKeyword.Text;
                //if (dtIntructionDateServiceReqFrom.EditValue != null && dtIntructionDateServiceReqFrom.DateTime != DateTime.MinValue)
                //{
                //    filter.INTRUCTION_TIME_FROM = Convert.ToInt64(dtIntructionDateServiceReqFrom.DateTime.ToString("yyyyMMddHHmm") + "00");
                //}
                //if (dtIntructionDateServiceReqTo.EditValue != null && dtIntructionDateServiceReqTo.DateTime != DateTime.MinValue)
                //{
                //    filter.INTRUCTION_TIME_TO = Convert.ToInt64(dtIntructionDateServiceReqTo.DateTime.ToString("yyyyMMddHHmm") + "00");
                //}

                //if (radioHasNotSum.CheckState == CheckState.Checked)
                //{
                //    filter.HAS_RATION_SUM_ID = false;
                //    filter.EXECUTE_ROOM_ID = this.roomId;
                //}
                //else if (RadioHasSum.CheckState == CheckState.Checked)
                //{
                //    filter.HAS_RATION_SUM_ID = true;
                //    filter.EXECUTE_ROOM_ID = this.roomId;
                //}
                //else
                //{
                //    filter.EXECUTE_ROOM_ID = null;
                //}

                if (_DepartmentSelecteds != null && _DepartmentSelecteds.Count > 0)
                {
                    filter.DEPARTMENT_IDs = _DepartmentSelecteds.Select(o => o.ID).ToList();
                }
                if (_RefectorySelecteds != null && _RefectorySelecteds.Count > 0)
                {
                    filter.ROOM_IDs = _RefectorySelecteds.Select(o => o.ROOM_ID).Distinct().ToList();
                }
                //if (cboRefectory.EditValue != null)
                //{
                //    filter.REQUEST_ROOM_ID = Convert.ToInt32(cboRefectory.EditValue.ToString());
                //}
                //if (cboRationTime.EditValue != null)
                //{
                //    filter.RATION_TIME_ID = Convert.ToInt32(cboRationTime.EditValue.ToString());
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public static int? ToNullableInt(string s)
        {
            int i;
            if (int.TryParse(s, out i)) return i;
            return null;
        }

        #endregion

        #region envent control
        private void txtDispenseCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    FillDataToGridServiceReq();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboStatus_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewRationSum_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            try
            {
                var selectRows = gridViewRationSum.GetSelectedRows();
                List<V_HIS_RATION_SUM> rationSumList = new List<V_HIS_RATION_SUM>();
                if (selectRows != null && selectRows.Count() > 0)
                {
                    foreach (var item in selectRows)
                    {
                        var rationSumItem = (V_HIS_RATION_SUM)gridViewRationSum.GetRow(item);
                        rationSumList.Add(rationSumItem);
                    }
                }
                if (rationSumList != null && rationSumList.Count > 0)
                {
                    //RefreshSearch();
                    rationSumIdList = rationSumList.Select(o => o.ID).Distinct().ToList();
                    GetServiceReqByRationSum();
                    //btnSearch_Click(null, null);
                    this.isFistOpen = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtKeyWord_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

        }

        private void btSearch_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToGridServiceReq();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                SetDefaultValueControl();
                FillDataToGridServiceReq();
                FillDataToGridRationSum();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ButtonEditViewDetail_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (EMR.EFMODEL.DataModels.V_EMR_DOCUMENT)gridViewServiceReq.GetFocusedRow();
                if (row == null) return;

                List<object> listArgs = new List<object>();
                listArgs.Add(row);
                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.DispenseDetail", roomId, roomTypeId, listArgs);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ButtonEditConfirm_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            try
            {
                bool success = false;
                var row = (EMR.EFMODEL.DataModels.V_EMR_DOCUMENT)gridViewServiceReq.GetFocusedRow();
                if (row != null)
                {
                    HisDispenseConfirmSDO hisDispenseConfirmSDO = new HisDispenseConfirmSDO();
                    hisDispenseConfirmSDO.Id = row.ID;
                    hisDispenseConfirmSDO.RequestRoomId = this.roomId;
                    WaitingManager.Show();
                    var apiresult = new Inventec.Common.Adapter.BackendAdapter
                       (param).Post<HisDispenseResultSDO>
                       ("api/HisDispense/Confirm", ApiConsumers.MosConsumer, hisDispenseConfirmSDO, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    if (apiresult != null)
                    {
                        success = true;
                        FillDataToGridServiceReq();
                    }
                    WaitingManager.Hide();
                    #region Show message
                    MessageManager.Show(this.ParentForm, param, success);
                    #endregion

                    #region Process has exception
                    HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ButtonEditDisConfirm_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            try
            {
                bool success = false;
                var row = (EMR.EFMODEL.DataModels.V_EMR_DOCUMENT)gridViewServiceReq.GetFocusedRow();
                if (row != null)
                {
                    HisDispenseConfirmSDO hisDispenseConfirmSDO = new MOS.SDO.HisDispenseConfirmSDO();
                    hisDispenseConfirmSDO.Id = row.ID;
                    hisDispenseConfirmSDO.RequestRoomId = this.roomId;
                    WaitingManager.Show();
                    var apiresult = new Inventec.Common.Adapter.BackendAdapter
                       (param).Post<HisDispenseResultSDO>
                       ("api/HisDispense/UnConfirm", ApiConsumers.MosConsumer, hisDispenseConfirmSDO, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    if (apiresult != null)
                    {
                        success = true;
                        FillDataToGridServiceReq();
                    }
                    WaitingManager.Hide();
                    #region Show message
                    MessageManager.Show(this.ParentForm, param, success);
                    #endregion

                    #region Process has exception
                    HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnEditBid_Click(object sender, EventArgs e)
        {
            try
            {
                var row = (EMR.EFMODEL.DataModels.V_EMR_DOCUMENT)gridViewServiceReq.GetFocusedRow();
                if (row == null) return;

                List<object> listArgs = new List<object>();
                listArgs.Add(row.ID);
                //listArgs.Add((HIS.Desktop.Common.RefeshReference)FillDataToGrid);
                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.DispenseMedicine", roomId, roomTypeId, listArgs);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_10 data = (MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_10)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;

                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "TDL_PATIENT_DOB_STR")
                        {
                            if (data.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == (short)1)
                            {
                                e.Value = data.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                            }
                            else
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.TDL_PATIENT_DOB);
                            }
                        }
                        else if (e.Column.FieldName == "TDL_INTRUCTION_DATE_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.INTRUCTION_DATE);
                        }
                        else if (e.Column.FieldName == "REQUEST_DEPARTMENT_NAME")
                        {
                            var requestDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>().FirstOrDefault(o => o.ID == data.REQUEST_DEPARTMENT_ID);
                            e.Value = requestDepartment != null ? requestDepartment.DEPARTMENT_NAME : "";
                        }
                        else if (e.Column.FieldName == "RATION_TIME_ID_STR")
                        {
                            var rationTime = this.ListRationTime.FirstOrDefault(o => o.ID == (data.RATION_TIME_ID ?? 0));
                            e.Value = rationTime != null ? rationTime.RATION_TIME_NAME : "";
                        }
                        else if (e.Column.FieldName == "TRANGTHAI_IMG")
                        {
                            //Chua xu ly: mau trang
                            //dang xu ly: mau vang
                            //Da ket thuc: mau den

                            long statusId = data.SERVICE_REQ_STT_ID;
                            if (statusId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL)
                            {
                                e.Value = imageListIcon.Images[0];
                            }
                            else if (statusId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL)
                            {
                                e.Value = imageListIcon.Images[1];
                            }
                            else if (statusId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT)
                            {
                                e.Value = imageListIcon.Images[4];
                            }
                            else
                            {
                                e.Value = imageListIcon.Images[0];
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

        private void gridView_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                V_HIS_SERVICE_REQ_10 data = (V_HIS_SERVICE_REQ_10)gridViewServiceReq.GetRow(e.RowHandle);
                string creator = (gridViewServiceReq.GetRowCellValue(e.RowHandle, "CREATOR") ?? "").ToString().Trim();
                int? isConfirm = ToNullableInt((gridViewServiceReq.GetRowCellValue(e.RowHandle, "IS_CONFIRM") ?? "").ToString());
                int? isActive = ToNullableInt((gridViewServiceReq.GetRowCellValue(e.RowHandle, "IS_ACTIVE") ?? "").ToString());

                if (e.Column.FieldName == "DELETE_DISPLAY") // xóa
                {
                    try
                    {
                        if (creator == LoggingName && isActive == 1 && isConfirm != 1)
                            e.RepositoryItem = ButtonEditDelete;
                        else
                            e.RepositoryItem = ButtonDeleteDisable;
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Warn(ex);
                    }
                }
                if (e.Column.FieldName == "UPDATE_DISPLAY") // sửa
                {
                    try
                    {
                        if (creator == LoggingName && isActive == 1 && isConfirm != 1)
                            e.RepositoryItem = btnEditBid;
                        else
                            e.RepositoryItem = btnEditBid_Disable;
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Warn(ex);
                    }
                }
                if (e.Column.FieldName == "IsConfirm" && isActive == 1)
                {
                    if (isConfirm == 1)
                    {
                        e.RepositoryItem = ButtonEditDisConfirm;
                    }
                    else
                    {
                        e.RepositoryItem = ButtonEditConfirm;
                    }
                }
                if (e.Column.FieldName == "CANCEL")
                {
                    e.RepositoryItem = (data.RATION_SUM_ID != null ? ButtonEditCancel : ButtonEditDisableCancel);

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSearchRationSum_Click(object sender, EventArgs e)
        {
            FillDataToGridRationSum();
            RefreshSearch();
        }

        private void btnSum_Click(object sender, EventArgs e)
        {
            try
            {
                this.positionHandleControl = -1;
                if (!btnSum.Enabled || !radioHasNotSum.Checked || !dxValidationProviderSave.Validate())
                    return;


                int[] selectRows = gridViewServiceReq.GetSelectedRows();
                if (selectRows.Count() == 0)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Bạn phải chọn yêu cầu suất ăn trước khi thực hiện duyệt chốt", "Thông báo");
                    return;
                }

                List<V_HIS_SERVICE_REQ_10> serviceReqs = new List<V_HIS_SERVICE_REQ_10>();
                if (selectRows != null && selectRows.Count() > 0)//xuandv
                {
                    foreach (var item in selectRows)
                    {
                        var serviceReq = (V_HIS_SERVICE_REQ_10)gridViewServiceReq.GetRow(item);
                        serviceReqs.Add(serviceReq);
                    }
                }

                bool success = false;
                CommonParam param = new CommonParam();
                List<HIS_RATION_SUM> result = null;

                HisRationSumSDO input = new HisRationSumSDO();
                input.RoomId = this.roomId;
                input.ServiceReqIds = serviceReqs.Select(p => p.ID).Distinct().ToList();
                //input.RationTimeId = Inventec.Common.TypeConvert.Parse.ToInt64(cboRationTime.EditValue.ToString());
                //input.IntructionDate = Int64.Parse(dtIntructionDate.DateTime.ToString("yyyyMMdd") + "000000");
                result = new Inventec.Common.Adapter.BackendAdapter
                        (param).Post<List<HIS_RATION_SUM>>
                        ("api/HisRationSum/Create", ApiConsumer.ApiConsumers.MosConsumer, input, param);

                if (result != null && result.Count > 0)
                {
                    success = true;
                    FillDataToGridRationSum();
                    //SetDefaultValueControl();
                    btnSearch_Click(null, null);
                }

                #region Show message
                Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                #endregion

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewServiceReq_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                FillDataToGridSereServ();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ButtonEdit_DeleteEnable_ButtonClick(V_HIS_RATION_SUM focus)
        {
            try
            {
                if (this.currentRoom.DEPARTMENT_ID == focus.DEPARTMENT_ID
                            && (focus.RATION_SUM_STT_ID == IMSys.DbConfig.HIS_RS.HIS_RATION_SUM_STT.ID__REQ || focus.RATION_SUM_STT_ID == IMSys.DbConfig.HIS_RS.HIS_RATION_SUM_STT.ID__REJECT)
                            && (this.isAddmin || focus.CREATOR == this.logginName))
                {
                    if (focus != null && DevExpress.XtraEditors.XtraMessageBox.Show(
                       ResourceMessageLang.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong,
                       ResourceMessageLang.ThongBao,
                       MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        bool success = false;
                        CommonParam param = new CommonParam();

                        HisRationSumSDO input = new HisRationSumSDO();
                        input.Id = focus.ID;

                        success = new Inventec.Common.Adapter.BackendAdapter
                                (param).Post<bool>
                                ("api/HisRationSum/Delete", ApiConsumer.ApiConsumers.MosConsumer, input, param);

                        if (success)
                        {
                            //SetDefaultValueControl();

                            gridControlServiceReqRationSumDetail.BeginUpdate();
                            gridControlServiceReqRationSumDetail.DataSource = null;
                            gridControlServiceReqRationSumDetail.EndUpdate();

                            FillDataToGridRationSum();
                            btnSearch_Click(null, null);
                        }

                        #region Show message
                        Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                        #endregion

                        #region Process has exception
                        HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            this.isFistOpen = true;
            FillDataToGridServiceReq();
            FillDataToGridSereServ();
            FillDataToGridRationSum();
        }

        private void gridViewRationSum_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    V_HIS_RATION_SUM pData = (V_HIS_RATION_SUM)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + startPageRationSum;
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_STR")
                    {

                        if (pData.CREATE_TIME != null)
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.CREATE_TIME ?? 0);
                        }
                        else
                        {
                            e.Value = null;
                        }
                    }
                    else if (e.Column.FieldName == "RATION_SUM_STT_ID_IMAGE")
                    {
                        //Chua xu ly: mau trang
                        //dang xu ly: mau vang
                        //Da ket thuc: mau den

                        long statusId = pData.RATION_SUM_STT_ID;
                        if (statusId == IMSys.DbConfig.HIS_RS.HIS_RATION_SUM_STT.ID__REQ)
                        {
                            e.Value = imageListIcon.Images[0];
                        }
                        else if (statusId == IMSys.DbConfig.HIS_RS.HIS_RATION_SUM_STT.ID__APPROVAL)
                        {
                            e.Value = imageListIcon.Images[3];
                        }
                        else if (statusId == IMSys.DbConfig.HIS_RS.HIS_RATION_SUM_STT.ID__REJECT)
                        {
                            e.Value = imageListIcon.Images[4];
                        }
                        else
                        {
                            e.Value = imageListIcon.Images[0];
                        }
                    }
                    else if (e.Column.FieldName == "MAX_INTRUCTION_DATE_str")
                    {

                        if (pData.MAX_INTRUCTION_DATE != null)
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(pData.MAX_INTRUCTION_DATE ?? 0);
                        }
                        else
                        {
                            e.Value = null;
                        }
                    }
                    else if (e.Column.FieldName == "RATION_TIME_NAMES")
                    {
                        if (!string.IsNullOrEmpty(pData.RATION_TIME_IDS) && lstRationTime != null && lstRationTime.Count > 0)
                        {
                            var arrRatioTime = pData.RATION_TIME_IDS.Split(',');
                            List<string> names = new List<string>();
                            foreach (var item in arrRatioTime)
                            {
                                var rationTime = lstRationTime.FirstOrDefault(o => o.ID == Int64.Parse(item));
                                if (rationTime != null)
                                    names.Add(rationTime.RATION_TIME_NAME);
                            }
                            if (names.Count > 0)
                                e.Value = string.Join(",", names);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewRationSum_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {

                    V_HIS_RATION_SUM data = (V_HIS_RATION_SUM)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (e.Column.FieldName == "DELETE")
                    {
                        if (this.currentRoom.DEPARTMENT_ID == data.DEPARTMENT_ID
                            && (data.RATION_SUM_STT_ID == IMSys.DbConfig.HIS_RS.HIS_RATION_SUM_STT.ID__REQ || data.RATION_SUM_STT_ID == IMSys.DbConfig.HIS_RS.HIS_RATION_SUM_STT.ID__REJECT)
                            && (this.isAddmin || data.CREATOR == this.logginName))
                        {
                            e.RepositoryItem = ButtonEdit_DeleteEnable;
                        }
                        else
                        {
                            e.RepositoryItem = ButtonEdit_DeleteDisable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void radioHasNotSum_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.radioHasNotSum.Checked && this.RadioHasSum.Checked)
                {
                    this.RadioHasSum.Checked = !this.radioHasNotSum.Checked;
                    btnSum.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void RadioHasSum_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.RadioHasSum.Checked && this.radioHasNotSum.Checked)
                {
                    this.radioHasNotSum.Checked = !this.RadioHasSum.Checked;
                    btnSum.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtRequestDepartmentCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboRequestDepartment_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboRequestDepartment.EditValue != null)
                    {
                        HIS_DEPARTMENT gt = BackendDataWorker.Get<HIS_DEPARTMENT>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboRequestDepartment.EditValue.ToString()));
                        if (gt != null)
                        {

                        }
                    }
                    else
                    {
                    }
                }
                FillDataToComboRoom();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToComboRoom()
        {
            try
            {
                var data = GetRoomList();
                cboRoom.Properties.DataSource = data;

                cboRoom.Enabled = false;
                cboRoom.Enabled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboRequestDepartment_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                cboRoom.Focus();
                e.Handled = true;
            }
        }

        private void gridViewSereServ_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    SereServADO data = (SereServADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;

                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1 + startPageRationSum;
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "PRICE_STR")
                        {
                            e.Value = Inventec.Common.Number.Convert.NumberToString(data.PRICE, ConfigApplications.NumberSeperator);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyword_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ButtonEditTreeServicePrint_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                Print();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboRequestDepartment_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboRequestDepartment.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        #endregion

        private void toolTipController1_GetActiveObjectInfo(object sender, DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                if (e.Info == null && e.SelectedControl == gridControlServiceReq)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = gridControlServiceReq.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                    GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
                    if (info.InRowCell)
                    {
                        if (lastRowHandle != info.RowHandle || lastColumn != info.Column)
                        {
                            lastColumn = info.Column;
                            lastRowHandle = info.RowHandle;
                            string text = "";
                            if (info.Column.FieldName == "TRANGTHAI_IMG")
                            {
                                long serviceReqSTTId = Inventec.Common.TypeConvert.Parse.ToInt64((view.GetRowCellValue(lastRowHandle, "SERVICE_REQ_STT_ID") ?? "").ToString());
                                if (serviceReqSTTId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL)
                                {
                                    text = Inventec.Common.Resource.Get.Value("UCHisRationSum.ToolTipControl.CXL", ResourceLangManager.LanguageUCHisRationSum, LanguageManager.GetCulture());
                                }
                                else if (serviceReqSTTId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL)
                                {
                                    text = Inventec.Common.Resource.Get.Value("UCHisRationSum.ToolTipControl.DXL", ResourceLangManager.LanguageUCHisRationSum, LanguageManager.GetCulture());
                                }
                                else if (serviceReqSTTId == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT)
                                {
                                    text = Inventec.Common.Resource.Get.Value("UCHisRationSum.ToolTipControl.KT", ResourceLangManager.LanguageUCHisRationSum, LanguageManager.GetCulture());
                                }
                            }

                            lastInfo = new ToolTipControlInfo(new DevExpress.XtraGrid.GridToolTipInfo(view, new DevExpress.XtraGrid.Views.Base.CellToolTipInfo(info.RowHandle, info.Column, "Text")), text);
                        }
                        e.Info = lastInfo;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceReq_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            try
            {
                int[] selectRows = gridViewServiceReq.GetSelectedRows();
                if (selectRows == null || selectRows.Count() == 0)
                {
                    var focus = (V_HIS_SERVICE_REQ_10)gridViewServiceReq.GetFocusedRow();
                    if (focus != null)
                    {
                        FillDataToGridSereServ();
                    }

                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnPrinMps_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                richEditorMain.RunPrintTemplate(MPS.Processor.Mps000329.PDO.Mps000329PDO.printTypeCode, DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        List<HIS_RATION_GROUP> _RatioGroupSelecteds;
        private List<HIS_RATION_TIME> lstRationTime;

        private void InitCombo(GridLookUpEdit cbo, object data, string DisplayValue, string ValueMember)
        {
            try
            {
                cbo.Properties.DataSource = data;
                cbo.Properties.DisplayMember = DisplayValue;
                cbo.Properties.ValueMember = ValueMember;

                DevExpress.XtraGrid.Columns.GridColumn col2 = cbo.Properties.View.Columns.AddField(DisplayValue);
                col2.VisibleIndex = 1;
                col2.Width = 200;
                col2.Caption = "Tất cả";
                cbo.Properties.PopupFormWidth = 350;
                cbo.Properties.View.OptionsView.ShowColumnHeaders = true;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;

                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.SelectAll(cbo.Properties.DataSource);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void InitCombo(GridLookUpEdit cbo)
        {
            try
            {
                MOS.Filter.HisRationGroupFilter filter = new MOS.Filter.HisRationGroupFilter();
                filter.IS_ACTIVE = 1;
                var datas = new BackendAdapter(new CommonParam()).Get<List<HIS_RATION_GROUP>>("api/HisRationGroup/Get", ApiConsumers.MosConsumer, filter, null);

                cbo.Properties.DataSource = datas;
                cbo.Properties.DisplayMember = "RATION_GROUP_NAME";
                cbo.Properties.ValueMember = "ID";
                DevExpress.XtraGrid.Columns.GridColumn col2 = cbo.Properties.View.Columns.AddField("RATION_GROUP_NAME");

                col2.VisibleIndex = 1;
                col2.Width = 400;
                col2.Caption = "Tất cả";
                cbo.Properties.PopupFormWidth = 400;
                cbo.Properties.View.OptionsView.ShowColumnHeaders = true;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;

                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.SelectAll(cbo.Properties.DataSource);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void SelectionGrid__Loai(object sender, EventArgs e)
        {
            try
            {
                _RatioGroupSelecteds = new List<HIS_RATION_GROUP>();
                foreach (HIS_RATION_GROUP rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        _RatioGroupSelecteds.Add(rv);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ResetCombo(GridLookUpEdit cbo)
        {
            try
            {
                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.SelectAll(cbo.Properties.DataSource);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void cboLoai_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string str = "";
                if (_RatioGroupSelecteds != null && _RatioGroupSelecteds.Count > 0)
                {
                    str = string.Join(",", _RatioGroupSelecteds.Select(p => p.RATION_GROUP_NAME).Distinct().ToList());
                }

                e.DisplayText = str;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButton__Print_ButtonClick(V_HIS_RATION_SUM row)
        {
            try
            {
                if (row != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.RationSumPrint").FirstOrDefault();
                    if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.RationSumPrint'");
                    moduleData.RoomId = this.roomId;
                    moduleData.RoomTypeId = this.roomTypeId;
                    List<V_HIS_RATION_SUM> rationSumList = new List<V_HIS_RATION_SUM>();
                    rationSumList.Add(row);
                    if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                    {
                        List<object> listArgs = new List<object>();
                        listArgs.Add(moduleData);
                        listArgs.Add(rationSumList);
                        var extenceInstance = PluginInstance.GetPluginInstance(moduleData, listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                        ((Form)extenceInstance).ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtIntructionDateServiceReqFrom_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    dtIntructionDateServiceReqTo.Focus();
                    dtIntructionDateServiceReqTo.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtIntructionDateServiceReqTo_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboRequestDepartment_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string statusName = "";
                if (_DepartmentSelecteds != null && _DepartmentSelecteds.Count > 0)
                {
                    foreach (var item in _DepartmentSelecteds)
                    {
                        statusName += item.DEPARTMENT_NAME + ", ";
                    }
                }

                e.DisplayText = statusName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewRationSum_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var selectRows = gridViewRationSum.GetSelectedRows();
                List<V_HIS_RATION_SUM> rationSumList = new List<V_HIS_RATION_SUM>();
                if (selectRows != null && selectRows.Count() > 0)
                {
                    foreach (var item in selectRows)
                    {
                        var rationSumItem = (V_HIS_RATION_SUM)gridViewRationSum.GetRow(item);
                        rationSumList.Add(rationSumItem);
                    }
                }
                if (rationSumList != null && rationSumList.Count > 0)
                {
                    //RefreshSearch();
                    rationSumIdList = rationSumList.Select(o => o.ID).Distinct().ToList();
                    GetServiceReqByRationSum();
                    //btnSearch_Click(null, null);
                    this.isFistOpen = false;
                }
                else
                {
                    FillDataToTreeServiceReqGroup(null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceReqRationSumDetail_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    V_HIS_SERVICE_REQ_10 data = (V_HIS_SERVICE_REQ_10)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;

                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1 + startPageServiceReq;
                        }
                        else if (e.Column.FieldName == "RATION_TIME_ID_STR")
                        {
                            var rationTime = this.ListRationTime.FirstOrDefault(o => o.ID == (data.RATION_TIME_ID ?? 0));
                            e.Value = rationTime != null ? rationTime.RATION_TIME_NAME : "";
                        }
                        else if (e.Column.FieldName == "TDL_INTRUCTION_DATE_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.INTRUCTION_TIME);
                        }
                        else if (e.Column.FieldName == "TDL_PATIENT_DOB_STR")
                        {
                            if (data.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == (short)1)
                                e.Value = data.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                            else
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.TDL_PATIENT_DOB);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboRefectory_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboRefectory.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboRationTime_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboRationTime.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void toolTipControllerRationSum_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                if (e.Info == null && e.SelectedControl == gridControlRationSum)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = gridControlRationSum.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                    GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
                    if (info.InRowCell)
                    {
                        if (lastRowHandleRationSum != info.RowHandle || lastColumnRationSum != info.Column)
                        {
                            lastColumnRationSum = info.Column;
                            lastRowHandleRationSum = info.RowHandle;
                            string text = "";
                            if (info.Column.FieldName == "RATION_SUM_STT_ID_IMAGE")
                            {
                                long rationSumSTTId = Inventec.Common.TypeConvert.Parse.ToInt64((view.GetRowCellValue(lastRowHandleRationSum, "RATION_SUM_STT_ID") ?? "").ToString());
                                if (rationSumSTTId == IMSys.DbConfig.HIS_RS.HIS_RATION_SUM_STT.ID__REQ)
                                {
                                    text = Inventec.Common.Resource.Get.Value("UCHisRationSum.ToolTipControl.HIS_RATION_SUM_STT.ID__REQ", ResourceLangManager.LanguageUCHisRationSum, LanguageManager.GetCulture());
                                }
                                else if (rationSumSTTId == IMSys.DbConfig.HIS_RS.HIS_RATION_SUM_STT.ID__APPROVAL)
                                {
                                    text = Inventec.Common.Resource.Get.Value("UCHisRationSum.ToolTipControl.HIS_RATION_SUM_STT.ID__APPROVAL", ResourceLangManager.LanguageUCHisRationSum, LanguageManager.GetCulture());
                                }
                                else if (rationSumSTTId == IMSys.DbConfig.HIS_RS.HIS_RATION_SUM_STT.ID__REJECT)
                                {
                                    text = Inventec.Common.Resource.Get.Value("UCHisRationSum.ToolTipControl.HIS_RATION_SUM_STT.ID__REJECT", ResourceLangManager.LanguageUCHisRationSum, LanguageManager.GetCulture());
                                }
                            }

                            lastInfoRationSum = new ToolTipControlInfo(new DevExpress.XtraGrid.GridToolTipInfo(view, new DevExpress.XtraGrid.Views.Base.CellToolTipInfo(info.RowHandle, info.Column, "Text")), text);
                        }
                        e.Info = lastInfoRationSum;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }



        private void btnPrintRationSum_Click(object sender, EventArgs e)
        {
            try
            {
                List<V_HIS_RATION_SUM> rationSumSelectList = new List<V_HIS_RATION_SUM>();
                var selectrationSum = gridViewRationSum.GetSelectedRows();
                if (selectrationSum != null && selectrationSum.Count() > 0)
                {
                    foreach (var item in selectrationSum)
                    {
                        var department = (V_HIS_RATION_SUM)gridViewRationSum.GetRow(item);
                        rationSumSelectList.Add(department);
                    }
                }

                if (rationSumSelectList == null || rationSumSelectList.Count == 0)
                {
                    MessageBox.Show("Chưa chọn phiếu tổng hợp");
                    return;
                }

                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.RationSumPrint").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.RationSumPrint'");
                moduleData.RoomId = this.roomId;
                moduleData.RoomTypeId = this.roomTypeId;
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(rationSumSelectList);
                    listArgs.Add(moduleData);
                    var extenceInstance = PluginInstance.GetPluginInstance(moduleData, listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewRationSum_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    GridView view = sender as GridView;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);
                    if (hi.InRowCell)
                    {
                        var rationSum = (V_HIS_RATION_SUM)gridViewRationSum.GetRow(hi.RowHandle);
                        if (hi.Column.FieldName == "DELETE")
                        {
                            ButtonEdit_DeleteEnable_ButtonClick(rationSum);
                        }
                        if (hi.Column.FieldName == "PRINT_RATION_SUM")
                        {
                            repositoryItemButton__Print_ButtonClick(rationSum);
                        }
                        if (hi.Column.FieldName == "VIEW_DETAIL")
                        {
                            ButtonEdit_ViewDetail_ButtonClick(rationSum);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ButtonEdit_ViewDetail_ButtonClick(V_HIS_RATION_SUM focus)
        {
            try
            {
                if (focus == null)
                    return;
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.MealRationDetail").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.RationSumPrint'");
                moduleData.RoomId = this.roomId;
                moduleData.RoomTypeId = this.roomTypeId;
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(moduleData);
                    listArgs.Add(focus);
                    var extenceInstance = PluginInstance.GetPluginInstance(moduleData, listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboRefectory_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtIntructionDateServiceReqFrom.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtIntructionDateServiceReqFrom_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtIntructionDateServiceReqTo.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtIntructionDateServiceReqTo_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboRationTime.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboRationTime_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboRequestDepartment.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnApproveRation_Click(object sender, EventArgs e)
        {
            try
            {
                Popup.frmRationSchedule frm = new Popup.frmRationSchedule(roomId, GetTime);
                frm.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void GetTime(long nFrom, long nTo)
        {
            try
            {
                dtIntructionDateServiceReqFrom.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(nFrom) ?? DateTime.Now;
                dtIntructionDateServiceReqTo.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(nTo) ?? DateTime.Now;
                FillDataToGridServiceReq();
                FillDataToGridSereServ();
                FillDataToGridRationSum();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridControl1_DataSourceChanged(object sender, EventArgs e)
        {
            try
            {
                List<SereServADO> dataSource = gridControl1.DataSource != null ? (gridControl1.DataSource as IEnumerable<SereServADO>).ToList() : null;
                if (dataSource != null)
                {
                    if (dataSource.All(o => o.IS_RATION == 1))
                    {
                        gridColumn27.Caption = "Mức ăn";
                        gridColumn27.ToolTip = "";
                    }
                    else if (dataSource.All(o => o.IS_RATION == null))
                    {
                        gridColumn27.Caption = "ĐTTT";
                        gridColumn27.ToolTip = "Đối tượng thanh toán";
                    }
                    else
                    {
                        gridColumn27.Caption = "Mức ăn/ĐTTT";
                        gridColumn27.ToolTip = "Mức ăn/Đối tượng thanh toán";
                    }
                }
                else
                {
                    gridColumn27.Caption = "Mức ăn/ĐTTT";
                    gridColumn27.ToolTip = "Mức ăn/Đối tượng thanh toán";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView3_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    SereServADO data = (SereServADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;

                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "PRICE_STR")
                        {
                            e.Value = Inventec.Common.Number.Convert.NumberToString(data.PRICE, ConfigApplications.NumberSeperator);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView3_CustomDrawGroupRow(object sender, RowObjectCustomDrawEventArgs e)
        {
            try
            {
                //var info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;
                //info.GroupText = " - " + Convert.ToString(this.gridView1.GetGroupRowValue(e.RowHandle, this.gridColumn37) ?? "");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboRefectory_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string statusName = "";
                if (_RefectorySelecteds != null && _RefectorySelecteds.Count > 0)
                {
                    foreach (var item in _RefectorySelecteds)
                    {
                        statusName += item.REFECTORY_NAME + ", ";
                    }
                }

                e.DisplayText = statusName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewServiceReq_RowCellClick(object sender, RowCellClickEventArgs e)
        {
            if (e.Column.FieldName == "CANCEL")
            {
                var treatmentData = (V_HIS_SERVICE_REQ_10)gridViewServiceReq.GetRow(e.RowHandle);
                if (treatmentData != null)
                {
                    if (treatmentData.RATION_SUM_ID != null)
                    {
                        if (DevExpress.XtraEditors.XtraMessageBox.Show(
                           ResourceMessageLang.HeThongTBCuaSoThongBaoBanCoMuonHuyTongHopKhong,
                           ResourceMessageLang.ThongBao,
                           MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            bool success = false;
                            CommonParam param = new CommonParam();
                            var rowData = (MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ_10)gridViewServiceReq.GetFocusedRow();
                            HisRationSumUpdateSDO input = new HisRationSumUpdateSDO();
                            input.RationSumId = rowData.RATION_SUM_ID.Value;
                            input.ServiceReqId = rowData.ID;
                            input.WorkingRoomId = currentRoom.ID;


                            success = new Inventec.Common.Adapter.BackendAdapter
                                    (param).Post<bool>
                                    ("api/HisRationSum/Remove", ApiConsumer.ApiConsumers.MosConsumer, input, param);

                            if (success)
                            {
                                //SetDefaultValueControl();

                                gridControlServiceReqRationSumDetail.BeginUpdate();
                                gridControlServiceReqRationSumDetail.DataSource = null;
                                gridControlServiceReqRationSumDetail.EndUpdate();

                                FillDataToGridRationSum();
                                btnSearch_Click(null, null);
                            }

                            #region Show message
                            Inventec.Desktop.Common.Message.MessageManager.Show(this.ParentForm, param, success);
                            #endregion

                            #region Process has exception
                            HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                            #endregion
                        }
                    }
                }

            }
        }

        //private void gridViewServiceReq_MouseDown(object sender, MouseEventArgs e)
        //{
        //    try
        //    {
        //        GridView view = sender as GridView;
        //        GridHitInfo hi = view.CalcHitInfo(e.Location);
        //        if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
        //        {
        //            if (hi.InRowCell)
        //            {
        //                var treatmentData = (V_HIS_SERVICE_REQ_10)gridViewServiceReq.GetRow(hi.RowHandle);
        //                if (treatmentData != null)
        //                {
        //                    if (hi.Column.FieldName == "CANCEL")
        //                    {
        //                        ButtonEditCancel_Click(sender, e);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        private void cboRoom_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboRoom.EditValue = null;

                    GridCheckMarksSelection gridCheckMark = cboRoom.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                    {
                        gridCheckMark.ClearSelection(cboRoom.Properties.View);
                    }
                    btnSearch.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboRoom_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                btnSearch.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboRoom_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string statusName = "";
                int index = 0;
                if (_RoomSelecteds != null && _RoomSelecteds.Count > 0)
                {
                    foreach (var item in _RoomSelecteds)
                    {
                        statusName += item.ROOM_NAME + ", ";
                    }
                    index = statusName.LastIndexOf(',');
                    e.DisplayText = statusName.Remove(index, 1);
                }
                else
                {
                    e.DisplayText = statusName;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboRoom_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch.Focus();
                    btnSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridControlSereServ_DataSourceChanged(object sender, EventArgs e)
        {
            try
            {
                List<SereServADO> dataSource = gridControlSereServ.DataSource != null ? (gridControlSereServ.DataSource as IEnumerable<SereServADO>).ToList() : null;
                if (dataSource != null)
                {
                    if (dataSource.All(o => o.IS_RATION == 1))
                    {
                        gridSereServColumnPatientTypeName.Caption = "Mức ăn";
                        gridSereServColumnPatientTypeName.ToolTip = "";
                    }
                    else if (dataSource.All(o => o.IS_RATION == null))
                    {
                        gridSereServColumnPatientTypeName.Caption = "ĐTTT";
                        gridSereServColumnPatientTypeName.ToolTip = "Đối tượng thanh toán";
                    }
                    else
                    {
                        gridSereServColumnPatientTypeName.Caption = "Mức ăn/ĐTTT";
                        gridSereServColumnPatientTypeName.ToolTip = "Mức ăn/Đối tượng thanh toán";
                    }
                }
                else
                {
                    gridSereServColumnPatientTypeName.Caption = "Mức ăn/ĐTTT";
                    gridSereServColumnPatientTypeName.ToolTip = "Mức ăn/Đối tượng thanh toán";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}

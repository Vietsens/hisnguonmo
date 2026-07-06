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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.HemodialysisDispensary.ADO;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;

namespace HIS.Desktop.Plugins.HemodialysisDispensary
{
    public partial class UCHemodialysisDispensary : UserControlBase
    {
        #region Declare

        private int patientStart = 0;
        private int patientLimit = 0;
        private int patientRowCount = 0;
        private int patientTotalData = 0;

        private int oldStart = 0;
        private int oldLimit = 0;
        private int oldRowCount = 0;
        private int oldTotalData = 0;

        ToolTipControlInfo lastInfo = null;
        GridColumn lastColumn = null;
        int lastRowHandle = -1;

        /// <summary>Chặn xử lý EditValueChanged khi đang khởi tạo combo phòng.</summary>
        private bool isInitializing = false;

        private long currentRoomId = 0;
        private List<V_HIS_ROOM> listRoom = new List<V_HIS_ROOM>();

        /// <summary>Danh sách BN đã xếp lịch chạy thận trong Phòng + Ngày + Ca (lưới trái).</summary>
        private List<V_HIS_SERVICE_REQ_8> currentListData = new List<V_HIS_SERVICE_REQ_8>();

        /// <summary>BN đang chọn (y lệnh chạy thận trong phòng — dùng làm ServiceReqParentId).</summary>
        private V_HIS_SERVICE_REQ_8 currentServiceReq = null;

        /// <summary>Y lệnh đơn chạy thận BS đang chọn (lưới phải trên).</summary>
        private V_HIS_SERVICE_REQ_7 currentPrescription = null;

        private const string ModuleLinkAssignPrescriptionKidney = "HIS.Desktop.Plugins.AssignPrescriptionKidney";

        #endregion

        #region Constructor + Load

        public UCHemodialysisDispensary(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
        }

        private void UCHemodialysisDispensary_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                this.SetCaptionByLanguageKey();
                this.InitComboRoom();
                this.SetDefaultControlValue();
                gridControlPatient.ToolTipController = this.toolTipController1;
                this.FillDataToGridPatient();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager(
                    "HIS.Desktop.Plugins.HemodialysisDispensary.Resources.Lang",
                    typeof(UCHemodialysisDispensary).Assembly);

                this.lciRoomCode.Text = GetLang("UCHemodialysisDispensary.lciRoomCode.Text");
                this.lciDate.Text = GetLang("UCHemodialysisDispensary.lciDate.Text");
                this.lciShift.Text = GetLang("UCHemodialysisDispensary.lciShift.Text");
                this.txtKeyword.Properties.NullValuePrompt = GetLang("UCHemodialysisDispensary.txtKeyword.NullValuePrompt");
                this.btnFind.Text = GetLang("UCHemodialysisDispensary.btnFind.Text");
                this.lcg_OldPrescription.Text = GetLang("UCHemodialysisDispensary.lcg_OldPrescription.Text");

                this.Gc_Patient_ScheduleDate.Caption = GetLang("UCHemodialysisDispensary.Gc_Patient_ScheduleDate.Caption");
                this.Gc_Patient_Shift.Caption = GetLang("UCHemodialysisDispensary.Gc_Patient_Shift.Caption");
                this.Gc_Patient_PatientName.Caption = GetLang("UCHemodialysisDispensary.Gc_Patient_PatientName.Caption");
                this.Gc_Patient_DobYear.Caption = GetLang("UCHemodialysisDispensary.Gc_Patient_DobYear.Caption");
                this.Gc_Patient_GenderName.Caption = GetLang("UCHemodialysisDispensary.Gc_Patient_GenderName.Caption");
                this.Gc_Patient_MedicineInfo.Caption = GetLang("UCHemodialysisDispensary.Gc_Patient_MedicineInfo.Caption");
                this.Gc_Patient_PatientCode.Caption = GetLang("UCHemodialysisDispensary.Gc_Patient_PatientCode.Caption");
                this.Gc_Patient_TreatmentCode.Caption = GetLang("UCHemodialysisDispensary.Gc_Patient_TreatmentCode.Caption");
                this.Gc_Patient_Note.Caption = GetLang("UCHemodialysisDispensary.Gc_Patient_Note.Caption");

                this.Gc_OldPres_IntructionTime.Caption = GetLang("UCHemodialysisDispensary.Gc_OldPres_IntructionTime.Caption");
                this.Gc_OldPres_RequestUser.Caption = GetLang("UCHemodialysisDispensary.Gc_OldPres_RequestUser.Caption");
                this.Gc_OldPres_KidneyTimes.Caption = GetLang("UCHemodialysisDispensary.Gc_OldPres_KidneyTimes.Caption");

                this.Gc_Detail_MedicineTypeName.Caption = GetLang("UCHemodialysisDispensary.Gc_Detail_MedicineTypeName.Caption");
                this.Gc_Detail_ServiceUnit.Caption = GetLang("UCHemodialysisDispensary.Gc_Detail_ServiceUnit.Caption");
                this.Gc_Detail_Amount.Caption = GetLang("UCHemodialysisDispensary.Gc_Detail_Amount.Caption");
                this.Gc_Detail_Kidney.Caption = GetLang("UCHemodialysisDispensary.Gc_Detail_Kidney.Caption");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private string GetLang(string key)
        {
            try
            {
                return Inventec.Common.Resource.Get.Value(
                    key,
                    Resources.ResourceLanguageManager.LanguageResource,
                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return "";
        }

        #endregion

        #region Init controls

        private void InitComboRoom()
        {
            try
            {
                isInitializing = true;

                this.listRoom = BackendDataWorker.Get<V_HIS_ROOM>()
                    .Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    .OrderBy(o => o.ROOM_CODE)
                    .ToList();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("ROOM_CODE", "Mã phòng", 100, 1));
                columnInfos.Add(new ColumnInfo("ROOM_NAME", "Tên phòng", 220, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("ROOM_NAME", "ID", columnInfos, false, 330);
                ControlEditorLoader.Load(this.cboRoom, this.listRoom, controlEditorADO);

                this.currentRoomId = this.currentModuleBase != null ? this.currentModuleBase.RoomId : 0;
                V_HIS_ROOM currentRoom = this.listRoom.FirstOrDefault(o => o.ID == this.currentRoomId);
                if (currentRoom != null)
                {
                    this.cboRoom.EditValue = currentRoom.ID;
                    this.txtRoomCode.Text = currentRoom.ROOM_CODE;
                }

                isInitializing = false;
            }
            catch (Exception ex)
            {
                isInitializing = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultControlValue()
        {
            try
            {
                this.txtKeyword.Text = "";
                this.dtScheduleDate.DateTime = DateTime.Now;
                this.cboShift.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Đọc Ca đang chọn (1..5); 0 nếu chưa chọn.</summary>
        private long GetSelectedShift()
        {
            long result = 0;
            try
            {
                if (this.cboShift.EditValue != null)
                {
                    long.TryParse(this.cboShift.EditValue.ToString(), out result);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        #endregion

        #region Load patient grid (Phòng + Ngày + Ca)

        private void FillDataToGridPatient()
        {
            int numPageSize;
            if (ucPagingPatient.pagingGrid != null)
            {
                numPageSize = ucPagingPatient.pagingGrid.PageSize;
            }
            else
            {
                numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
            }

            LoadPagingPatient(new CommonParam(0, numPageSize));

            CommonParam param = new CommonParam();
            param.Limit = this.patientRowCount;
            param.Count = this.patientTotalData;
            ucPagingPatient.Init(LoadPagingPatient, param, numPageSize);
        }

        private void LoadPagingPatient(object param)
        {
            try
            {
                this.currentServiceReq = null;
                this.patientStart = ((CommonParam)param).Start ?? 0;
                this.patientLimit = ((CommonParam)param).Limit ?? 0;
                this.currentListData = new List<V_HIS_SERVICE_REQ_8>();
                CommonParam paramCommon = new CommonParam(this.patientStart, this.patientLimit);

                HisServiceReqView8Filter filter = new HisServiceReqView8Filter();
                filter.EXECUTE_ROOM_ID = this.currentRoomId;
                filter.KEY_WORD = txtKeyword.Text;
                filter.IS_KIDNEY = true;
                filter.ORDER_DIRECTION = "ASC";
                filter.ORDER_DIRECTION1 = "ASC";
                filter.ORDER_FIELD = "KIDNEY_SHIFT";
                filter.ORDER_FIELD1 = "INTRUCTION_TIME";
                filter.SERVICE_REQ_TYPE_IDs = new List<long>()
                {
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__AN,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__CDHA,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__G,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__GPBL,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KHAC,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__NS,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__PHCN,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__PT,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__SA,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TDCN,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__TT,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN,
                };
                if (dtScheduleDate.EditValue != null && dtScheduleDate.DateTime != DateTime.MinValue)
                {
                    filter.INTRUCTION_DATE_FROM = Convert.ToInt64(dtScheduleDate.DateTime.ToString("yyyyMMdd") + "000000");
                    filter.INTRUCTION_DATE_TO = Convert.ToInt64(dtScheduleDate.DateTime.ToString("yyyyMMdd") + "235959");
                }

                var rs = new BackendAdapter(paramCommon).GetRO<List<V_HIS_SERVICE_REQ_8>>(
                    RequestUriStore.HIS_SERVICE_REQ_GETVIEW_8, ApiConsumers.MosConsumer, filter, paramCommon);
                if (rs != null && rs.Data != null)
                {
                    this.currentListData = rs.Data;
                }

                // Lọc Ca (KIDNEY_SHIFT) phía client — an toàn với mọi kiểu (long/long?) của view.
                long selectedShift = GetSelectedShift();
                if (selectedShift > 0)
                {
                    string shiftText = selectedShift.ToString();
                    this.currentListData = this.currentListData
                        .Where(o => Convert.ToString(o.KIDNEY_SHIFT) == shiftText)
                        .ToList();
                }

                this.patientRowCount = (this.currentListData == null ? 0 : this.currentListData.Count);
                this.patientTotalData = (rs != null && rs.Param != null ? (rs.Param.Count ?? 0) : this.patientRowCount);

                gridControlPatient.BeginUpdate();
                gridControlPatient.DataSource = this.currentListData;
                gridControlPatient.EndUpdate();

                FillDataToGriOldPres();

                #region Process has exception
                SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        #region Load doctor prescriptions (cross-treatment by patient)

        private void FillDataToGriOldPres()
        {
            try
            {
                int numPageSize;
                if (ucPagingOldPres.pagingGrid != null)
                {
                    numPageSize = ucPagingOldPres.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = 10;
                }

                LoadPagingOldPres(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = this.oldRowCount;
                param.Count = this.oldTotalData;
                ucPagingOldPres.Init(LoadPagingOldPres, param, numPageSize);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadPagingOldPres(object param)
        {
            try
            {
                this.currentPrescription = null;
                this.oldStart = ((CommonParam)param).Start ?? 0;
                this.oldLimit = ((CommonParam)param).Limit ?? 0;
                List<V_HIS_SERVICE_REQ_7> listData = new List<V_HIS_SERVICE_REQ_7>();
                CommonParam paramCommon = new CommonParam(this.oldStart, this.oldLimit);

                if (this.currentServiceReq != null)
                {
                    // V_HIS_SERVICE_REQ_7 load theo bệnh nhân (cross-treatment). Lưu ý:
                    // HisServiceReqView7Filter KHÔNG có field IS_KIDNEY — lọc theo loại đơn
                    // (Đơn điều trị + Đơn chạy thận), giống cách AssignPrescriptionKidney đang dùng.
                    HisServiceReqView7Filter filter = new HisServiceReqView7Filter();
                    filter.ORDER_DIRECTION = "DESC";
                    filter.ORDER_FIELD = "INTRUCTION_TIME";
                    filter.TDL_PATIENT_ID = this.currentServiceReq.TDL_PATIENT_ID;
                    filter.SERVICE_REQ_TYPE_IDs = new List<long>
                    {
                        IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT,
                        IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK
                    };

                    var rs = new BackendAdapter(paramCommon).GetRO<List<V_HIS_SERVICE_REQ_7>>(
                        RequestUriStore.HIS_SERVICE_REQ_GETVIEW_7, ApiConsumers.MosConsumer, filter, paramCommon);
                    if (rs != null && rs.Data != null)
                    {
                        listData = rs.Data;
                        this.oldRowCount = listData.Count;
                        this.oldTotalData = (rs.Param == null ? 0 : rs.Param.Count ?? 0);
                    }
                }
                else
                {
                    this.oldRowCount = 0;
                    this.oldTotalData = 0;
                }

                gridControlOldPres.BeginUpdate();
                gridControlOldPres.DataSource = listData;
                gridControlOldPres.EndUpdate();

                LoadDataToGridDetail();

                #region Process has exception
                SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        #region Load medicine detail (Còn lại)

        private void LoadDataToGridDetail()
        {
            try
            {
                List<MetyMatyADO> listData = new List<MetyMatyADO>();
                if (this.currentPrescription != null)
                {
                    HisServiceReqMetyFilter metyFilter = new HisServiceReqMetyFilter();
                    metyFilter.SERVICE_REQ_ID = this.currentPrescription.ID;
                    var metyReqs = new BackendAdapter(new CommonParam()).Get<List<V_HIS_SERVICE_REQ_METY>>(
                        RequestUriStore.HIS_SERVICE_REQ_METY_GETVIEW, ApiConsumers.MosConsumer, metyFilter, null);
                    if (metyReqs != null && metyReqs.Count > 0)
                    {
                        var medicineTypeDict = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().ToDictionary(o => o.ID, o => o);
                        foreach (var item in metyReqs)
                        {
                            if (!item.MEDICINE_TYPE_ID.HasValue) continue;
                            V_HIS_MEDICINE_TYPE medicineType = null;
                            medicineTypeDict.TryGetValue(item.MEDICINE_TYPE_ID.Value, out medicineType);
                            listData.Add(new MetyMatyADO(item, medicineType));
                        }
                    }
                }

                gridControlPresDetail.BeginUpdate();
                gridControlPresDetail.DataSource = listData;
                gridControlPresDetail.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        #region Grid patient events

        private void gridViewPatient_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    V_HIS_SERVICE_REQ_8 pData = (V_HIS_SERVICE_REQ_8)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + this.patientStart;
                    }
                    else if (e.Column.FieldName == "INTRUCTION_DATE_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(pData.INTRUCTION_DATE);
                    }
                    else if (e.Column.FieldName == "DOB_YEAR")
                    {
                        string dob = Convert.ToString(pData.TDL_PATIENT_DOB);
                        e.Value = (dob != null && dob.Length >= 4) ? dob.Substring(0, 4) : dob;
                    }
                    else if (e.Column.FieldName == "IMG_STATUS")
                    {
                        if (pData.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL)
                            e.Value = imageListIcon.Images[0];
                        else if (pData.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL)
                            e.Value = imageListIcon.Images[1];
                        else if (pData.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT)
                            e.Value = imageListIcon.Images[4];
                        else
                            e.Value = imageListIcon.Images[0];
                    }
                    else if (e.Column.FieldName == "HAS_DISPENSARY")
                    {
                        if (pData.EXECUTE_KIDNEY_SERVICE_REQ_ID.HasValue && pData.EXECUTE_KIDNEY_SERVICE_REQ_ID.Value > 0)
                            e.Value = imageCollection1.Images[0];
                        else
                            e.Value = imageCollection1.Images[2];
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridControlPatient_Click(object sender, EventArgs e)
        {
            try
            {
                if (gridViewPatient.FocusedRowHandle < 0)
                {
                    return;
                }
                WaitingManager.Show();
                this.currentServiceReq = (V_HIS_SERVICE_REQ_8)gridViewPatient.GetFocusedRow();
                FillDataToGriOldPres();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        #region Grid doctor-prescription events

        private void gridViewOldPres_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    V_HIS_SERVICE_REQ_7 pData = (V_HIS_SERVICE_REQ_7)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "INTRUCTION_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.INTRUCTION_TIME);
                    }
                    else if (e.Column.FieldName == "REQUEST_LOGINAME_STR")
                    {
                        e.Value = pData.REQUEST_LOGINNAME + " - " + pData.REQUEST_USERNAME;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewOldPres_RowCellClick(object sender, RowCellClickEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                this.currentPrescription = (V_HIS_SERVICE_REQ_7)gridViewOldPres.GetFocusedRow();
                this.LoadDataToGridDetail();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        #region Grid medicine-detail events

        private void gridViewPresDetail_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewPresDetail_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            try
            {
                MetyMatyADO data = (MetyMatyADO)gridViewPresDetail.GetRow(e.RowHandle);
                if (data != null && e.Column.FieldName == "IN_PRES_CREATE")
                {
                    // R11: (+) chỉ bật khi loại thuốc chạy thận VÀ Còn lại > 0.
                    if (this.currentServiceReq != null
                        && this.currentPrescription != null
                        && data.IsKidney
                        && data.KidneyAmountLeft > 0)
                    {
                        e.RepositoryItem = repositoryItemBtn_InPres;
                    }
                    else
                    {
                        e.RepositoryItem = repositoryItemBtn_InPres__Disable;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemBtn_InPres_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                MetyMatyADO row = (MetyMatyADO)gridViewPresDetail.GetFocusedRow();
                if (row == null || row.ReqMety == null || this.currentServiceReq == null || this.currentPrescription == null)
                {
                    return;
                }

                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws
                    .Where(o => o.ModuleLink == ModuleLinkAssignPrescriptionKidney).FirstOrDefault();
                if (moduleData == null)
                {
                    Inventec.Common.Logging.LogSystem.Error("khong tim thay moduleLink = " + ModuleLinkAssignPrescriptionKidney);
                    return;
                }
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    WaitingManager.Show();

                    // Lấy HIS_SERVICE_REQ gốc của y lệnh BS (đủ field, đặc biệt KIDNEY_TIMES để tính SL = SL/Số lần chạy — R12).
                    HisServiceReqFilter reqFilter = new HisServiceReqFilter();
                    reqFilter.ID = this.currentPrescription.ID;
                    List<HIS_SERVICE_REQ> requests = new BackendAdapter(new CommonParam()).Get<List<HIS_SERVICE_REQ>>(
                        RequestUriStore.HIS_SERVICE_REQ_GET, ApiConsumers.MosConsumer, reqFilter, null);
                    HIS_SERVICE_REQ serviceReqBS = requests != null ? requests.FirstOrDefault() : null;

                    HIS.Desktop.ADO.AssignPrescriptionKidneyADO assignServiceADO = new HIS.Desktop.ADO.AssignPrescriptionKidneyADO();
                    assignServiceADO.ServiceReq = serviceReqBS;
                    assignServiceADO.ServiceReqMety = new HIS_SERVICE_REQ_METY
                    {
                        ID = row.ReqMety.ID,
                        SERVICE_REQ_ID = row.ReqMety.SERVICE_REQ_ID,
                        MEDICINE_TYPE_ID = row.ReqMety.MEDICINE_TYPE_ID,
                        AMOUNT = row.ReqMety.AMOUNT
                    };
                    assignServiceADO.ServiceReqParentId = this.currentServiceReq.ID;

                    List<object> listArgs = new List<object>();
                    listArgs.Add(assignServiceADO);
                    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModuleBase.RoomId, this.currentModuleBase.RoomTypeId));
                    var extenceInstance = PluginInstance.GetPluginInstance(
                        PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModuleBase.RoomId, this.currentModuleBase.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");

                    WaitingManager.Hide();
                    ((Form)extenceInstance).ShowDialog();

                    // Sau khi kê xong → nạp lại chi tiết + trạng thái BN.
                    this.LoadDataToGridDetail();
                    this.FillDataToGridPatient();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        #region Filter events

        private void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnFind.Enabled) return;
                WaitingManager.Show();
                FillDataToGridPatient();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboRoom_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (isInitializing) return;
                if (this.cboRoom.EditValue == null) return;

                long roomId = 0;
                long.TryParse(this.cboRoom.EditValue.ToString(), out roomId);
                this.currentRoomId = roomId;

                V_HIS_ROOM room = this.listRoom.FirstOrDefault(o => o.ID == roomId);
                this.txtRoomCode.Text = room != null ? room.ROOM_CODE : "";

                WaitingManager.Show();
                FillDataToGridPatient();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnPrevDate_Click(object sender, EventArgs e)
        {
            try
            {
                if (dtScheduleDate.EditValue == null || dtScheduleDate.DateTime == DateTime.MinValue)
                    dtScheduleDate.DateTime = DateTime.Now;
                dtScheduleDate.DateTime = dtScheduleDate.DateTime.AddDays(-1);
                WaitingManager.Show();
                FillDataToGridPatient();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnNextDate_Click(object sender, EventArgs e)
        {
            try
            {
                if (dtScheduleDate.EditValue == null || dtScheduleDate.DateTime == DateTime.MinValue)
                    dtScheduleDate.DateTime = DateTime.Now;
                dtScheduleDate.DateTime = dtScheduleDate.DateTime.AddDays(1);
                WaitingManager.Show();
                FillDataToGridPatient();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnPrevShift_Click(object sender, EventArgs e)
        {
            try
            {
                if (cboShift.SelectedIndex > 0)
                {
                    cboShift.SelectedIndex = cboShift.SelectedIndex - 1;
                    WaitingManager.Show();
                    FillDataToGridPatient();
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnNextShift_Click(object sender, EventArgs e)
        {
            try
            {
                if (cboShift.SelectedIndex < cboShift.Properties.Items.Count - 1)
                {
                    cboShift.SelectedIndex = cboShift.SelectedIndex + 1;
                    WaitingManager.Show();
                    FillDataToGridPatient();
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtKeyword_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnFind_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtScheduleDate_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtKeyword.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Tooltip + shortcut

        private void toolTipController1_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                if (e.Info == null && e.SelectedControl == gridControlPatient)
                {
                    GridView view = gridControlPatient.FocusedView as GridView;
                    GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
                    if (info.InRowCell && (lastInfo == null || lastRowHandle != info.RowHandle || lastColumn != info.Column))
                    {
                        lastColumn = info.Column;
                        lastRowHandle = info.RowHandle;
                        string text = "";
                        var data = ((V_HIS_SERVICE_REQ_8)view.GetRow(info.RowHandle));
                        if (data != null && info.Column.FieldName == "IMG_STATUS")
                        {
                            if (data.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL)
                                text = "Chưa xử lý";
                            else if (data.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL)
                                text = "Đang xử lý";
                            else if (data.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT)
                                text = "Kết thúc";
                        }
                        else if (data != null && info.Column.FieldName == "HAS_DISPENSARY")
                        {
                            text = (data.EXECUTE_KIDNEY_SERVICE_REQ_ID.HasValue && data.EXECUTE_KIDNEY_SERVICE_REQ_ID.Value > 0)
                                ? "Đã dự trù thuốc chạy thận"
                                : "Chưa dự trù thuốc chạy thận";
                        }
                        lastInfo = new ToolTipControlInfo(new DevExpress.XtraGrid.GridToolTipInfo(view, new CellToolTipInfo(info.RowHandle, info.Column, "Text")), text);
                    }
                    e.Info = lastInfo;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>Phím tắt Ctrl+F (KeyboardWorker).</summary>
        public void BTN_FIND()
        {
            try
            {
                btnFind_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion
    }
}

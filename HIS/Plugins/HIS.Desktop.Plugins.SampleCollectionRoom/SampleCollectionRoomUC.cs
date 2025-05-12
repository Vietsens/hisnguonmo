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
using LIS.EFMODEL.DataModels;
using DevExpress.XtraGrid.Columns;
using DevExpress.Utils;
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using System.Collections;
//using HIS.Desktop.LocalStorage.SdaConfigKey.Config;
using DevExpress.XtraGrid.Views.Grid;
using Inventec.Desktop.Common.Message;
using Inventec.Core;
using Inventec.Common.Adapter;
using HIS.Desktop.Controls.Session;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using Inventec.Common.Logging;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.ConfigApplication;
using MOS.Filter;
using Inventec.Common.Controls.EditorLoader;
using System.Resources;
using Inventec.Desktop.Common.LanguageManager;
using LIS.Filter;
using HIS.Desktop.Plugins.SampleCollectionRoom.ADO;
using LIS.SDO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.SampleCollectionRoom.Config;
using DevExpress.XtraBars;
using HIS.Desktop.Common;
using System.Globalization;
using HIS.Desktop.LocalStorage.LocalData;
using AutoMapper;
using HIS.Desktop.Utility;
using Bartender.PrintClient;
using MOS.SDO;
using HIS.Desktop.Utilities.Extensions;

namespace HIS.Desktop.Plugins.SampleCollectionRoom
{
    public partial class SampleCollectionRoomUC : HIS.Desktop.Utility.UserControlBase
    {
        #region Declare
        V_HIS_ROOM room = null;
        V_HIS_EXECUTE_ROOM executeRoom = null;
        Inventec.Desktop.Common.Modules.Module currentModule;
        bool statecheckColumn = false;
        internal Inventec.Core.ApiResultObject<List<V_LIS_SAMPLE>> apiResult;
        internal List<SampleListViewADO> currentSample { get; set; }
        internal SampleListViewADO rowSample { get; set; }
        internal SampleListViewADO samplePrint { get; set; }
        internal List<LIS_SAMPLE_SERVICE> listSampleService { get; set; }
        int lastRowHandle = -1;
        internal V_LIS_RESULT currentLisResult { get; set; }
        GridColumn lastColumn = null;
        ToolTipControlInfo lastInfo = null;
        List<SampleListViewADO> lstAll { get; set; }
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        int limit = 0;

        string serviceCodeSelect;

        internal List<V_LIS_RESULT> _LisResults = new List<V_LIS_RESULT>();
        List<HIS_TREATMENT_TYPE> listTreatmentType;
        List<HIS_TREATMENT_TYPE> _DienDieuTriSelecteds;
        internal HIS_SERVICE_REQ currentServiceReq = new HIS_SERVICE_REQ();
        internal List<V_HIS_SERE_SERV_TEIN> listSereServTein = new List<V_HIS_SERE_SERV_TEIN>();
        List<V_HIS_TEST_INDEX_RANGE> testIndexRangeAll = null;
        internal List<SampleLisResultADO> lstCheckPrint = new List<SampleLisResultADO>();
        NumberStyles style = NumberStyles.Any;
        internal List<LIS_MACHINE> _Machines { get; set; }
        CPA.WCFClient.CallPatientClient.CallPatientClientManager clienttManager = null;
        BarManager baManager = null;
        PopupMenuProcessor popupMenuProcessor = null;
        #endregion

        #region Contructor

        public SampleCollectionRoomUC()
        {
            InitializeComponent();
            LisConfigCFG.LoadConfig();
        }

        public SampleCollectionRoomUC(Inventec.Desktop.Common.Modules.Module currentModule)
            : base(currentModule)
        {
            InitializeComponent();
            try
            {
                this.currentModule = currentModule;
                LisConfigCFG.LoadConfig();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UC_SampleCollectionRoomUC_Load(object sender, EventArgs e)
        {
            try
            {
                if (room == null)
                {
                    room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == currentModule.RoomId);
                }

                this.testIndexRangeAll = BackendDataWorker.Get<V_HIS_TEST_INDEX_RANGE>();
                LoadDataToCombo();
                LoadDefaultData();
                InitTreatmentArea();
                InitCheck(cboTreatmentArea, SelectionGrid__TreatmentArea);
                InitCombo(cboTreatmentArea, listTreatmentType, "TREATMENT_TYPE_NAME", "ID");
                this.gridControlSample.ToolTipController = this.toolTipControllerGrid;
                this.gridControlSampleCollectionResult.ToolTipController = this.toolTipController1;
                FillDataToGridControl();
                LoadCboMachine();
                txtGateNumber.Text = this.room.ROOM_CODE;
                setSizeInformationPatient();
                SetCheckAllColumn(this.statecheckColumn);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitTreatmentArea()
        {
            try
            {
                this.listTreatmentType = BackendDataWorker.Get<HIS_TREATMENT_TYPE>();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                ControlEditorADO controlEditorADO = new ControlEditorADO("TREATMENT_TYPE_NAME", "ID", columnInfos, false, 200);
                ControlEditorLoader.Load(cboTreatmentArea, this.listTreatmentType, controlEditorADO);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

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

                cbo.Properties.PopupFormWidth = 200;
                cbo.Properties.View.OptionsView.ShowColumnHeaders = true;
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

        private void SelectionGrid__TreatmentArea(object sender, EventArgs e)
        {
            try
            {
                _DienDieuTriSelecteds = new List<HIS_TREATMENT_TYPE>();
                foreach (HIS_TREATMENT_TYPE rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        _DienDieuTriSelecteds.Add(rv);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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

        #endregion

        #region Gridview Sample

        private void toolTipControllerGrid_GetActiveObjectInfo(object sender, DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                if (e.Info == null && e.SelectedControl == gridControlSample)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = gridControlSample.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                    GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
                    if (info.InRowCell)
                    {
                        if (lastRowHandle != info.RowHandle || lastColumn != info.Column)
                        {
                            lastColumn = info.Column;
                            lastRowHandle = info.RowHandle;
                            string text = "";
                            if (info.Column.FieldName == "STATUS")
                            {
                                //text = (view.GetRowCellValue(lastRowHandle, "SAMPLE_STT_NAME") ?? "").ToString();
                                var busyCount = ((SampleListViewADO)view.GetRow(lastRowHandle)).SAMPLE_STT_ID;
                                if (busyCount == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM)
                                {
                                    text = "Chưa lấy mẫu";
                                }
                                else if (busyCount == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DA_LM)
                                {
                                    text = "Đã lấy mẫu";
                                }

                                else if (busyCount == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CO_KQ)
                                {
                                    text = "Có kết quả";
                                }

                                else if (busyCount == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TRA_KQ)
                                {
                                    text = "Đã có kết quả";
                                }
                                else if (busyCount == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TU_CHOI)
                                {
                                    text = "Từ chối mẫu";
                                }
                                else if (busyCount == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHAP_NHAN)
                                {
                                    text = "Chấp nhận mẫu";
                                }
                                else if (busyCount == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DUYET_KQ)
                                {
                                    text = "Duyệt kết quả";
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

        private void gridViewSample_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                rowSample = null;
                rowSample = (SampleListViewADO)gridViewSample.GetFocusedRow();
                LoadLisResult(rowSample);
                LoadDataToGridTestResult2();
                LoadInformationPatient();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void setSizeInformationPatient()
        {
            try
            {
                string screenWidth = Screen.PrimaryScreen.Bounds.Width.ToString();
                string screenHeight = Screen.PrimaryScreen.Bounds.Height.ToString();
                if (Int64.Parse(screenHeight) >= 768)
                {
                    layoutControlItem5.Size = new System.Drawing.Size(812, 240);
                }
                if (Int64.Parse(screenHeight) >= 900)
                {
                    layoutControlItem5.Size = new System.Drawing.Size(812, 190);
                }
                if (Int64.Parse(screenHeight) >= 1000)
                {
                    layoutControlItem5.Size = new System.Drawing.Size(812, 160);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadInformationPatient()
        {
            try
            {
                CommonParam param = new CommonParam();
                lblPatientCode.Text = rowSample.PATIENT_CODE;
                lblPatientName.Text = rowSample.VIR_PATIENT_NAME;
                lblGender.Text = rowSample.GENDER_NAME;
                lblDOB.Text = rowSample.DOB != null ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(rowSample.DOB.ToString()) : "";
                lblAdress.Text = rowSample.VIR_ADDRESS;
                lblHeinCardNumber.Text = rowSample.HEIN_CARD_NUMBER;
                lblHeinMediOrgCode.Text = rowSample.HEIN_MEDI_ORG_CODE;
                lblHanTu.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(rowSample.HEIN_CARD_FROM_TIME ?? 0) + " - " + Inventec.Common.DateTime.Convert.TimeNumberToDateString(rowSample.HEIN_CARD_TO_TIME ?? 0);
                lblServiceReqCode.Text = rowSample.SERVICE_REQ_CODE;
                lblCancelReason.Text = rowSample.CANCEL_REASON;
                lblRejectReason.Text = rowSample.REJECT_REASON;
                lblDepartment.Text = rowSample.REQUEST_DEPARTMENT_NAME;
                lblInstructionTime.Text = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rowSample.INTRUCTION_TIME ?? 0).Substring(0, Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rowSample.INTRUCTION_TIME ?? 0).Length - 3);
                lblTreatmentType.Text = rowSample.PATIENT_TYPE_NAME;
                MOS.Filter.HisServiceReqFilter Filter = new HisServiceReqFilter();
                Filter.SERVICE_REQ_CODE__EXACT = rowSample.SERVICE_REQ_CODE;
                var ServiceReq = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, Filter, param);
                if (ServiceReq != null && ServiceReq.Count > 0)
                {
                    lblIcdName.Text = ServiceReq[0].ICD_CODE + " - " + ServiceReq[0].ICD_NAME;
                    lblIcdText.Text = ServiceReq[0].ICD_SUB_CODE + " - " + ServiceReq[0].ICD_TEXT;
                }

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadLisResult(SampleListViewADO data)
        {
            try
            {
                CommonParam param = new CommonParam();
                _LisResults = new List<V_LIS_RESULT>();
                if (data != null)
                {
                    LIS.Filter.LisResultViewFilter resultFilter = new LisResultViewFilter();
                    resultFilter.SAMPLE_ID = data.ID;
                    _LisResults = new BackendAdapter(param).Get<List<V_LIS_RESULT>>("api/LisResult/GetView", ApiConsumer.ApiConsumers.LisConsumer, resultFilter, param);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewSample_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                GridView View = sender as GridView;
                if (e.RowHandle >= 0)
                {
                    var sampleStt = Inventec.Common.TypeConvert.Parse.ToInt64((gridViewSample.GetRowCellValue(e.RowHandle, "SAMPLE_STT_ID")).ToString());
                    var data = (SampleListViewADO)gridViewSample.GetRow(e.RowHandle);
                    if (data == null) return;

                    if (e.Column.FieldName == "DUYET")
                    {
                        if (sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM
                            || sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TU_CHOI)
                        {
                            e.RepositoryItem = DuyetE;
                        }
                        else if (sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DA_LM)
                        {
                            e.RepositoryItem = HuyDuyetE;
                        }
                        else
                        {
                            e.RepositoryItem = HuyDuyetD;
                        }
                    }
                    else if (e.Column.FieldName == "BARCODE")
                    {
                        if ((data.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM ||
                            data.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TU_CHOI)
                            && LisConfigCFG.IS_AUTO_CREATE_BARCODE != "1")
                        {
                            e.RepositoryItem = repositoryItemTextBarcodeE;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemTextBarcodeD;
                        }
                    }
                    else if (e.Column.FieldName == "CALL_PATIENT")
                    {
                        e.RepositoryItem = ButtonEdit_CallPatientEnable;
                    }
                    else if (e.Column.FieldName == "UPDATE_BARCODE_TIME")
                    {
                        if (sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DA_LM
                            || sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM
                            || sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHAP_NHAN
                            || sampleStt == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TU_CHOI)
                        {
                            e.RepositoryItem = repositoryItemBtnUpdateBarcodeTime_Enable;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemBtnUpdateBarcodeTime_Disable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewSample_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    SampleListViewADO data = (SampleListViewADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1 + startPage;
                        }
                        else if (e.Column.FieldName == "STATUS")
                        {
                            //Chua lấy mẫu: mau trang
                            //Đã lấy mẫu: mau vang
                            //Đã có kết quả: mau cam
                            //Đã trả kết quả: mau do
                            long statusId = data.SAMPLE_STT_ID;
                            if (statusId == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM)
                            {
                                e.Value = imageListIcon.Images[0];
                            }
                            else if (statusId == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DA_LM)
                            {
                                e.Value = imageListIcon.Images[1];
                            }
                            else if (statusId == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CO_KQ)
                            {
                                e.Value = imageListIcon.Images[2];
                            }
                            else if (statusId == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TRA_KQ)
                            {
                                e.Value = imageListIcon.Images[3];
                            }
                            else if (statusId == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TU_CHOI)
                            {
                                e.Value = imageListIcon.Images[5];
                            }
                            else if (statusId == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHAP_NHAN)
                            {
                                e.Value = imageListIcon.Images[11];
                            }
                            else if (statusId == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DUYET_KQ)
                            {
                                e.Value = imageListIcon.Images[10];
                            }
                        }
                        else if (e.Column.FieldName == "PATIENT_NAME")
                        {
                            e.Value = data.LAST_NAME + " " + data.FIRST_NAME;
                        }
                        else if (e.Column.FieldName == "DOB_DISPLAY")
                        {
                            if (data.IS_HAS_NOT_DAY_DOB == 1)
                            {
                                if (!string.IsNullOrEmpty(data.DOB.ToString()))
                                {
                                    e.Value = data.DOB.ToString().Substring(0, 4);
                                }
                                else
                                {
                                    e.Value = "0";
                                }
                            }
                            else
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.DOB ?? 0);
                            }

                        }
                        else if (e.Column.FieldName == "CREATE_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "SAMPLE_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.SAMPLE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "APPOINTMENT_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.APPOINTMENT_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "INSTRUCTION_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.INTRUCTION_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "REQUEST_USER")
                        {
                            e.Value = (data.REQUEST_LOGINNAME ?? "") + " - " + (data.REQUEST_USERNAME ?? "");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region Method

        internal void FillDataToGridControl()
        {
            txtSearchKey.Focus();
            txtSearchKey.SelectAll();

            FillDataToGridSample(new CommonParam(0, (int)ConfigApplications.NumPageSize));

            CommonParam param = new CommonParam();
            param.Limit = rowCount;
            param.Count = dataTotal;
            ucPaging1.Init(FillDataToGridSample, param, (int)ConfigApplications.NumPageSize, this.gridControlSample);
        }

        internal void FillDataToGridSample(object param)
        {
            try
            {
                btnPrintBarcode.Enabled = false;
                WaitingManager.Show();
                startPage = ((CommonParam)param).Start ?? 0;
                limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);

                gridControlSample.DataSource = null;

                LisSampleViewFilter lisSampleFilter = new LisSampleViewFilter();
                if (room != null)
                {
                    if (this.room.ROOM_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__BP)
                    {
                        lisSampleFilter.SAMPLE_ROOM_CODE__EXACT = room.ROOM_CODE;
                    }
                    else if (this.room.ROOM_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__BUONG)
                    {
                        lisSampleFilter.REQUEST_DEPARTMENT_CODE__EXACT = room.DEPARTMENT_CODE;
                    }
                    else if (this.room.ROOM_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__XL)
                    {
                        if (executeRoom == null)
                            executeRoom = BackendDataWorker.Get<V_HIS_EXECUTE_ROOM>().FirstOrDefault(o => o.ROOM_ID == this.room.ID);
                        if (executeRoom.IS_EXAM == 1)
                            lisSampleFilter.REQUEST_DEPARTMENT_CODE__EXACT = room.DEPARTMENT_CODE;
                        else
                            lisSampleFilter.EXECUTE_ROOM_CODE__EXACT = room.ROOM_CODE;
                    }
                    else
                    {
                        lisSampleFilter.EXECUTE_ROOM_CODE__EXACT = room.ROOM_CODE;
                    }
                }
                else
                {
                    throw new Exception("Room is null");
                }
                if (!String.IsNullOrWhiteSpace(txtFindServiceReqCode.Text))
                {
                    string code = txtFindServiceReqCode.Text.Trim();
                    if (code.Length < 12 && checkDigit(code))
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        txtFindServiceReqCode.Text = code;
                    }
                    lisSampleFilter.SERVICE_REQ_CODE__EXACT = code;
                }
                else if (!String.IsNullOrWhiteSpace(txtFindServiceBarCode.Text))
                {
                    string code = txtFindServiceBarCode.Text.Trim();
                    //if (code.Length <= 5)
                    //{
                    //    txtFindTreamentCode.Text = code;
                    //}
                    lisSampleFilter.BARCODE__EXACT = code;
                }
                else if (!String.IsNullOrWhiteSpace(txtFindTreamentCode.Text))
                {
                    string code = txtFindTreamentCode.Text.Trim();
                    if (code.Length < 12)
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        txtFindTreamentCode.Text = code;
                    }
                    lisSampleFilter.TREATMENT_CODE__EXACT = code;
                }
                else if (!String.IsNullOrWhiteSpace(txtFindPatientCode.Text))
                {
                    string code = txtFindPatientCode.Text.Trim();
                    if (code.Length < 10)
                    {
                        code = string.Format("{0:0000000000}", Convert.ToInt64(code));
                        txtFindPatientCode.Text = code;
                    }
                    lisSampleFilter.PATIENT_CODE__EXACT = code;
                }
                else
                {
                    if (!String.IsNullOrEmpty(txtSearchKey.Text))
                        lisSampleFilter.KEY_WORD = txtSearchKey.Text.Trim();
                    if (dtCreatefrom != null && dtCreatefrom.DateTime != DateTime.MinValue)
                        lisSampleFilter.BARCODE_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtCreatefrom.EditValue).ToString("yyyyMMdd") + "000000");
                    if (dtCreateTo != null && dtCreateTo.DateTime != DateTime.MinValue)
                        lisSampleFilter.BARCODE_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(Convert.ToDateTime(dtCreateTo.EditValue).ToString("yyyyMMdd") + "235959");
                }


                lisSampleFilter.ORDER_FIELD = "CALL_SAMPLE_ORDER";
                lisSampleFilter.ORDER_DIRECTION = "ASC";

                List<long> vs = new List<long>();
                if (cboTreatmentArea.EditValue != null)
                {
                    if (this._DienDieuTriSelecteds != null && this._DienDieuTriSelecteds.Count > 0)
                    {
                        lisSampleFilter.TREATMENT_TYPE_IDs = this._DienDieuTriSelecteds.Select(o => o.ID).ToList();
                    }

                    if (cboTreatmentArea.EditValue is List<long>)
                    {
                        lisSampleFilter.TREATMENT_TYPE_IDs = (List<long>)cboTreatmentArea.EditValue;
                    }
                }

                List<long> lstTestServiceReqSTT = new List<long>();

                //Tất cả 0
                //Chưa lấy mẫu 1
                //Đã lấy mẫu 2
                //Đã có kết quả
                //Đã trả kết quả
                //Filter yeu cau chua lấy mẫu
                if (cboFind.EditValue != null)
                {
                    //Chưa lấy mẫu
                    if ((long)cboFind.EditValue == 1)
                    {
                        lisSampleFilter.SAMPLE_STT_IDs = new List<long>()
                            {
                                IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM,
                                IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TU_CHOI
                            };
                    }
                    //Đã lấy mẫu
                    else if ((long)cboFind.EditValue == 2)
                    {
                        lisSampleFilter.SAMPLE_STT_IDs = new List<long>()
                            {
                                IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DA_LM,
                                IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHAP_NHAN
                            };
                    }
                    //đã có kết quả
                    else if ((long)cboFind.EditValue == 3)
                    {
                        lisSampleFilter.SAMPLE_STT_IDs = new List<long>()
                            {
                                IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CO_KQ,
                                IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DUYET_KQ
                            };
                    }//đã trả kết quả
                    else if ((long)cboFind.EditValue == 4)
                    {
                        lisSampleFilter.SAMPLE_STT_ID = IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TRA_KQ;
                    }
                    else if ((long)cboFind.EditValue == 999) //Chưa có kq
                    {
                        List<long> sampleSttIds = new List<long>();
                        sampleSttIds.Add(IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM);//chưa lay mau
                        sampleSttIds.Add(IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DA_LM);
                        sampleSttIds.Add(IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CO_KQ);
                        sampleSttIds.Add(IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHAP_NHAN);
                        sampleSttIds.Add(IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TU_CHOI);
                        sampleSttIds.Add(IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DUYET_KQ);
                        lisSampleFilter.SAMPLE_STT_IDs = sampleSttIds;
                    }
                    //Tất cả
                    else
                    {
                        lisSampleFilter.SAMPLE_STT_ID = null;
                    }
                }
                apiResult = new ApiResultObject<List<V_LIS_SAMPLE>>();

                apiResult = new BackendAdapter(paramCommon).GetRO<List<V_LIS_SAMPLE>>("api/LisSample/GetView", ApiConsumer.ApiConsumers.LisConsumer, lisSampleFilter, paramCommon);

                if (apiResult != null)
                {
                    WaitingManager.Hide();
                    var data = (List<V_LIS_SAMPLE>)apiResult.Data;
                    if (data != null)
                    {
                        lstAll = new List<SampleListViewADO>();
                        foreach (var item in data)
                        {
                            lstAll.Add(new SampleListViewADO(item));
                        }
                        gridControlSample.DataSource = lstAll;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                        if (data.Count == 1)
                        {
                            gridViewSample.FocusedRowHandle = 0;
                            btnPrintBarcode.Enabled = true;
                        }
                    }
                    #region Process has exception
                    SessionManager.ProcessTokenLost((CommonParam)param);
                    #endregion
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void SetCheckAllColumn(bool state)
        {
            try
            {
                this.grcChecked.ImageAlignment = StringAlignment.Center;
                this.grcChecked.Image = (state ? this.imageCollection2.Images[1] : this.imageCollection2.Images[0]);
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void LoadDataToCombo()
        {
            try
            {
                List<HIS.Desktop.Plugins.SampleCollectionRoom.ComboADO> status = new List<HIS.Desktop.Plugins.SampleCollectionRoom.ComboADO>();
                //status.Add(new HIS.Desktop.Plugins.SampleCollectionRoom.ComboADO(999, "Chưa trả kết quả"));
                status.Add(new HIS.Desktop.Plugins.SampleCollectionRoom.ComboADO(0, "Tất cả"));
                status.Add(new HIS.Desktop.Plugins.SampleCollectionRoom.ComboADO(IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM, "Chưa lấy mẫu"));
                status.Add(new HIS.Desktop.Plugins.SampleCollectionRoom.ComboADO(IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DA_LM, "Đã lấy mẫu"));
                status.Add(new HIS.Desktop.Plugins.SampleCollectionRoom.ComboADO(IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CO_KQ, "Có kết quả"));
                //status.Add(new HIS.Desktop.Plugins.SampleCollectionRoom.ComboADO(IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TRA_KQ, "Trả kết quả"));
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("statusName", "Trạng thái", 50, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("statusName", "id", columnInfos, true, 50);
                ControlEditorLoader.Load(cboFind, status, controlEditorADO);

                cboFind.EditValue = status[1].id;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void LoadDefaultData()
        {
            try
            {
                txtSearchKey.Focus();
                txtSearchKey.SelectAll();
                dtCreatefrom.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.StartDay() ?? 0);
                dtCreateTo.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.EndDay() ?? 0);
                gridControlSample.DataSource = null;
                gridControlSampleCollectionResult.DataSource = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool checkDigit(string s)
        {
            bool result = false;
            try
            {
                for (int i = 0; i < s.Length; i++)
                {
                    if (char.IsDigit(s[i]) == true) result = true;
                    else result = false;
                }
                return result;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return result;
            }
        }

        #region UpdateStt
        private void UpdateStt(long sampleSTT)
        {
            try
            {
                WaitingManager.Show();
                bool result = false;
                CommonParam param = new CommonParam();
                LIS.SDO.UpdateSampleSttSDO updateStt = new LIS.SDO.UpdateSampleSttSDO();
                var row = (SampleListViewADO)gridViewSample.GetFocusedRow();
                if (row != null)
                {
                    updateStt.Id = row.ID;
                    updateStt.SampleSttId = sampleSTT;

                    var curentSTT = new BackendAdapter(param).Post<LIS_SAMPLE>("/api/LisSample/UpdateStt", ApiConsumer.ApiConsumers.LisConsumer, updateStt, param);
                    if (curentSTT != null)
                    {
                        result = true;
                        FillDataToGridControl();
                        WaitingManager.Hide();
                    }
                }
                WaitingManager.Hide();
                #region Show message
                MessageManager.Show(this.ParentForm, param, result);
                #endregion

                #region Process has exception
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #endregion

        #region Event Button Sample

        private void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                try
                {
                    gridControlSampleCollectionResult.BeginUpdate();
                    gridControlSampleCollectionResult.DataSource = null;
                    gridControlSampleCollectionResult.EndUpdate();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
                FillDataToGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InBarcode_Click(object sender, EventArgs e)
        {
            try
            {
                onClickBtnPrintBarCode();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void DuyetE_Click(object sender, EventArgs e)
        {
            try
            {
                bool result = false;
                CommonParam param = new CommonParam();
                var row = (SampleListViewADO)gridViewSample.GetFocusedRow();
                if (row != null && (row.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM
                    || row.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TU_CHOI))
                {
                    MOS.Filter.HisTreatmentFilter treatmentFilter = new HisTreatmentFilter();
                    treatmentFilter.TREATMENT_CODE__EXACT = row.TREATMENT_CODE;
                    var treatment = new BackendAdapter(param).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, treatmentFilter, param).FirstOrDefault();
                    if (treatment != null && treatment.IS_PAUSE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    {
                        XtraMessageBox.Show("Hồ sơ đã kết thúc điều trị. Không cho phép thực hiện lấy mẫu.", "Thông báo");
                        return;
                    }
                    WaitingManager.Show();
                    if (LisConfigCFG.SHOW_FORM_SAMPLE_INFO == "1")
                    {
                        Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "LIS.Desktop.Plugins.SampleInfo").FirstOrDefault();
                        if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'LIS.Desktop.Plugins.SampleInfo'");
                        if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'LIS.Desktop.Plugins.SampleInfo' is not plugins");
                        List<object> listArgs = new List<object>();
                        var dataSend = new V_LIS_SAMPLE();
                        Inventec.Common.Mapper.DataObjectMapper.Map<V_LIS_SAMPLE>(dataSend, row);
                        listArgs.Add(dataSend);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                        ((Form)extenceInstance).ShowDialog();

                        FillDataToGridControl();
                        gridViewSample.RefreshData();
                    }
                    else
                    {
                        WaitingManager.Show();
                        LisSampleSampleSDO sdo = new LisSampleSampleSDO();
                        sdo.SampleId = row.ID;
                        sdo.RequestRoomCode = room.ROOM_CODE;
                        var curentSTT = new BackendAdapter(param).Post<LIS_SAMPLE>("api/LisSample/Sample", ApiConsumer.ApiConsumers.LisConsumer, sdo, param);
                        if (curentSTT != null)
                        {
                            rowSample = row;
                            rowSample.SAMPLE_STT_ID = curentSTT.SAMPLE_STT_ID;
                            rowSample.SAMPLE_TYPE_ID = curentSTT.SAMPLE_TYPE_ID;
                            rowSample.SAMPLE_TIME = curentSTT.SAMPLE_TIME;
                            rowSample.SAMPLE_LOGINNAME = curentSTT.SAMPLE_LOGINNAME;
                            rowSample.SAMPLE_USERNAME = curentSTT.SAMPLE_USERNAME;
                            rowSample.SAMPLE_ORDER = curentSTT.SAMPLE_ORDER;

                            FillDataToGridControl();
                            result = true;
                            gridViewSample.RefreshData();
                        }
                        WaitingManager.Hide();
                        #region Show message
                        MessageManager.Show(this.ParentForm, param, result);
                        #endregion

                        #region Process has exception
                        SessionManager.ProcessTokenLost(param);
                        #endregion
                    }
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }
        private void HuyDuyetE_Click(object sender, EventArgs e)
        {
            try
            {
                var row = (SampleListViewADO)gridViewSample.GetFocusedRow();
                if (row != null)
                {
                    frmCancelReason frm = new frmCancelReason(this.currentModuleBase, row, room.ROOM_CODE, FillDataToGridControl);
                    frm.ShowDialog();
                    gridViewSample.RefreshData();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSearchKey_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    FillDataToGridControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        #region Print Barcode

        private void onClickBtnPrintBarCode()
        {
            try
            {
                if (LisConfigCFG.PRINT_BARCODE_BY_BARTENDER == "1")
                {
                    this.PrintBarcodeByBartender();
                }
                else
                {
                    PrintType type = new PrintType();
                    type = PrintType.IN_BARCODE;
                    PrintProcess(type);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        internal enum PrintType
        {
            IN_BARCODE,
            IN_PHIEU_HEN
        }

        private void PrintProcess(PrintType printType)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                switch (printType)
                {
                    case PrintType.IN_BARCODE:
                        richEditorMain.RunPrintTemplate(MPS.Processor.Mps000077.PDO.Mps000077PDO.PrintTypeCode.Mps000077, DelegateRunPrinter);
                        break;
                    case PrintType.IN_PHIEU_HEN:
                        richEditorMain.RunPrintTemplate(MPS.Processor.Mps000233.PDO.Mps000233PDO.PrintTypeCode.Mps000233, DelegateRunPrinter);
                        break;
                    default:
                        break;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case MPS.Processor.Mps000077.PDO.Mps000077PDO.PrintTypeCode.Mps000077:
                        LoadBieuMauPhieuYCInBarCode(printTypeCode, fileName, ref result);
                        break;
                    case MPS.Processor.Mps000233.PDO.Mps000233PDO.PrintTypeCode.Mps000233:
                        LoadBieuMauPhieuHen(printTypeCode, fileName, ref result);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        internal void LoadBieuMauPhieuYCInBarCode(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                CommonParam param = new CommonParam();
                rowSample = new SampleListViewADO();
                rowSample = (SampleListViewADO)gridViewSample.GetFocusedRow();
                if (rowSample == null)
                    return;
                WaitingManager.Show();
                if (rowSample.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM
                    || rowSample.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TU_CHOI)
                {
                    if (LisConfigCFG.SHOW_FORM_SAMPLE_INFO == "1")
                    {
                        Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "LIS.Desktop.Plugins.SampleInfo").FirstOrDefault();
                        if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'LIS.Desktop.Plugins.SampleInfo'");
                        if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'LIS.Desktop.Plugins.SampleInfo' is not plugins");
                        List<object> listArgs = new List<object>();
                        var dataSend = new V_LIS_SAMPLE();
                        Inventec.Common.Mapper.DataObjectMapper.Map<V_LIS_SAMPLE>(dataSend, rowSample);
                        listArgs.Add(dataSend);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                        ((Form)extenceInstance).ShowDialog();

                        FillDataToGridControl();
                        gridViewSample.RefreshData();
                    }
                    else
                    {
                        LisSampleSampleSDO sdo = new LisSampleSampleSDO();
                        sdo.SampleId = rowSample.ID;
                        sdo.RequestRoomCode = room.ROOM_CODE;
                        var curentSTT = new BackendAdapter(param).Post<LIS_SAMPLE>("api/LisSample/Sample", ApiConsumer.ApiConsumers.LisConsumer, sdo, param);
                        if (curentSTT != null)
                        {
                            rowSample.SAMPLE_STT_ID = curentSTT.SAMPLE_STT_ID;
                            rowSample.SAMPLE_TYPE_ID = curentSTT.SAMPLE_TYPE_ID;
                            rowSample.SAMPLE_TIME = curentSTT.SAMPLE_TIME;
                            rowSample.SAMPLE_LOGINNAME = curentSTT.SAMPLE_LOGINNAME;
                            rowSample.SAMPLE_USERNAME = curentSTT.SAMPLE_USERNAME;
                            rowSample.SAMPLE_ORDER = curentSTT.SAMPLE_ORDER;

                            gridViewSample.RefreshData();
                        }
                    }
                }

                MOS.Filter.HisServiceReqViewFilter ServiceReqViewFilter = new HisServiceReqViewFilter();
                ServiceReqViewFilter.SERVICE_REQ_CODE = rowSample.SERVICE_REQ_CODE;
                var rs = new BackendAdapter(param).Get<List<V_HIS_SERVICE_REQ>>("api/HisServiceReq/GetView", ApiConsumers.MosConsumer, ServiceReqViewFilter, param).FirstOrDefault();

                MPS.Processor.Mps000077.PDO.Mps000077PDO mps000077RDO = new MPS.Processor.Mps000077.PDO.Mps000077PDO(
                           rowSample,
                           rs
                           );
                string printerName = "";

                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }
                if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    MPS.ProcessorBase.Core.PrintData PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000077RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                    result = MPS.MpsPrinter.Run(PrintData);
                }
                else
                {
                    MPS.ProcessorBase.Core.PrintData PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000077RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName);
                    result = MPS.MpsPrinter.Run(PrintData);
                }
                FillDataToGridControl();
                gridViewSample.RefreshData();
                txtSearchKey.Focus();
                txtSearchKey.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        internal void LoadBieuMauPhieuHen(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                rowSample = new SampleListViewADO();
                rowSample = (SampleListViewADO)gridViewSample.GetFocusedRow();
                if (rowSample == null)
                    return;
                //bool refresh = false;
                if (rowSample.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM
                    || rowSample.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TU_CHOI)
                {
                    if (LisConfigCFG.SHOW_FORM_SAMPLE_INFO == "1")
                    {
                        Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "LIS.Desktop.Plugins.SampleInfo").FirstOrDefault();
                        if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'LIS.Desktop.Plugins.SampleInfo'");
                        if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'LIS.Desktop.Plugins.SampleInfo' is not plugins");
                        List<object> listArgs = new List<object>();
                        var dataSend = new V_LIS_SAMPLE();
                        Inventec.Common.Mapper.DataObjectMapper.Map<V_LIS_SAMPLE>(dataSend, rowSample);
                        listArgs.Add(dataSend);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                        ((Form)extenceInstance).ShowDialog();

                        FillDataToGridControl();
                        gridViewSample.RefreshData();
                    }
                    else
                    {
                        LisSampleSampleSDO sdo = new LisSampleSampleSDO();
                        sdo.SampleId = rowSample.ID;
                        sdo.RequestRoomCode = room.ROOM_CODE;
                        var curentSTT = new BackendAdapter(param).Post<LIS_SAMPLE>("api/LisSample/Sample", ApiConsumer.ApiConsumers.LisConsumer, sdo, param);
                        if (curentSTT != null)
                        {
                            rowSample.SAMPLE_STT_ID = curentSTT.SAMPLE_STT_ID;
                            rowSample.SAMPLE_TYPE_ID = curentSTT.SAMPLE_TYPE_ID;
                            rowSample.SAMPLE_TIME = curentSTT.SAMPLE_TIME;
                            rowSample.SAMPLE_LOGINNAME = curentSTT.SAMPLE_LOGINNAME;
                            rowSample.SAMPLE_USERNAME = curentSTT.SAMPLE_USERNAME;
                            rowSample.SAMPLE_ORDER = curentSTT.SAMPLE_ORDER;
                            gridViewSample.RefreshData();
                        }
                    }
                }

                HisServiceReqViewFilter ServiceReqViewFilter = new HisServiceReqViewFilter();
                ServiceReqViewFilter.SERVICE_REQ_CODE = rowSample.SERVICE_REQ_CODE;
                var rs = new BackendAdapter(param).Get<List<V_HIS_SERVICE_REQ>>("api/HisServiceReq/GetView", ApiConsumers.MosConsumer, ServiceReqViewFilter, param).FirstOrDefault();

                // get sereServs
                MOS.Filter.HisSereServView6Filter hisSereServView6Filter = new MOS.Filter.HisSereServView6Filter();
                hisSereServView6Filter.SERVICE_REQ_ID = rs.ID;
                hisSereServView6Filter.SERVICE_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN;
                var sereServs = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV_6>>("api/HisSereServ/GetView6", ApiConsumer.ApiConsumers.MosConsumer, hisSereServView6Filter, param);
                List<V_HIS_SERVICE> serviceParents = new List<V_HIS_SERVICE>();
                if (sereServs != null && sereServs.Count > 0)
                {
                    var services = BackendDataWorker.Get<V_HIS_SERVICE>().Where(p => sereServs.Select(o => o.SERVICE_ID).Distinct().Contains(p.ID)).ToList();
                    if (services != null && services.Count > 0)
                    {
                        serviceParents = BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => services.Select(p => p.PARENT_ID).Distinct().Contains(o.ID)).ToList();
                    }
                }

                MPS.Processor.Mps000233.PDO.Mps000233PDO mps000233RDO = new MPS.Processor.Mps000233.PDO.Mps000233PDO(
                           rowSample,
                           rs,
                           serviceParents,
                           sereServs
                           );
                string printerName = "";

                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }
                if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    MPS.ProcessorBase.Core.PrintData PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000233RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                    result = MPS.MpsPrinter.Run(PrintData);
                }
                else
                {
                    MPS.ProcessorBase.Core.PrintData PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000233RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName);
                    result = MPS.MpsPrinter.Run(PrintData);
                }
                FillDataToGridControl();
                gridViewSample.RefreshData();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        private void PrintBarcodeByBartender()
        {
            try
            {
                rowSample = null;
                rowSample = (SampleListViewADO)gridViewSample.GetFocusedRow();
                if (rowSample == null)
                    return;
                WaitingManager.Show();
                if (rowSample.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM
                    || rowSample.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TU_CHOI)
                {
                    if (LisConfigCFG.SHOW_FORM_SAMPLE_INFO == "1")
                    {
                        WaitingManager.Hide();
                        Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "LIS.Desktop.Plugins.SampleInfo").FirstOrDefault();
                        if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'LIS.Desktop.Plugins.SampleInfo'");
                        if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'LIS.Desktop.Plugins.SampleInfo' is not plugins");
                        List<object> listArgs = new List<object>();
                        var dataSend = new V_LIS_SAMPLE();
                        Inventec.Common.Mapper.DataObjectMapper.Map<V_LIS_SAMPLE>(dataSend, rowSample);
                        listArgs.Add(dataSend);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                        ((Form)extenceInstance).ShowDialog();
                        WaitingManager.Show();
                        FillDataToGridControl();
                        gridViewSample.RefreshData();
                    }
                    else
                    {
                        CommonParam param = new CommonParam();
                        LisSampleSampleSDO sdo = new LisSampleSampleSDO();
                        sdo.SampleId = rowSample.ID;
                        sdo.RequestRoomCode = room.ROOM_CODE;
                        var curentSTT = new BackendAdapter(param).Post<LIS_SAMPLE>("api/LisSample/Sample", ApiConsumer.ApiConsumers.LisConsumer, sdo, param);
                        if (curentSTT != null)
                        {
                            rowSample.SAMPLE_STT_ID = curentSTT.SAMPLE_STT_ID;
                            rowSample.SAMPLE_TYPE_ID = curentSTT.SAMPLE_TYPE_ID;
                            rowSample.SAMPLE_TIME = curentSTT.SAMPLE_TIME;
                            rowSample.SAMPLE_LOGINNAME = curentSTT.SAMPLE_LOGINNAME;
                            rowSample.SAMPLE_USERNAME = curentSTT.SAMPLE_USERNAME;
                            rowSample.SAMPLE_ORDER = curentSTT.SAMPLE_ORDER;

                            FillDataToGridControl();
                            gridViewSample.RefreshData();
                        }
                    }
                }
                if (StartAppPrintBartenderProcessor.OpenAppPrintBartender())
                {
                    ClientPrintADO ado = new ClientPrintADO();
                    ado.Barcode = rowSample.BARCODE;
                    if (rowSample.DOB.HasValue)
                    {
                        ado.DobYear = rowSample.DOB.Value.ToString().Substring(0, 4);
                        ado.DobAge = MPS.AgeUtil.CalculateFullAge(rowSample.DOB.Value);
                    }
                    ado.ExecuteRoomCode = rowSample.EXECUTE_ROOM_CODE;
                    ado.ExecuteRoomName = rowSample.EXECUTE_ROOM_NAME ?? "";
                    ado.ExecuteRoomName_Unsign = Inventec.Common.String.Convert.UnSignVNese(rowSample.EXECUTE_ROOM_NAME ?? "");
                    ado.GenderName = (!String.IsNullOrWhiteSpace(rowSample.GENDER_CODE)) ? (rowSample.GENDER_CODE == "01" ? "Nữ" : "Nam") : "";
                    ado.GenderName_Unsign = Inventec.Common.String.Convert.UnSignVNese(ado.GenderName);
                    ado.PatientCode = rowSample.PATIENT_CODE ?? "";
                    List<string> name = new List<string>();
                    if (!String.IsNullOrWhiteSpace(rowSample.LAST_NAME))
                    {
                        name.Add(rowSample.LAST_NAME);
                    }
                    if (!String.IsNullOrWhiteSpace(rowSample.FIRST_NAME))
                    {
                        name.Add(rowSample.FIRST_NAME);
                    }
                    ado.PatientName = String.Join(" ", name);
                    ado.PatientName_Unsign = Inventec.Common.String.Convert.UnSignVNese(ado.PatientName);
                    ado.RequestDepartmentCode = rowSample.REQUEST_DEPARTMENT_CODE ?? "";
                    ado.RequestDepartmentName = rowSample.REQUEST_DEPARTMENT_NAME ?? "";
                    ado.RequestDepartmentName_Unsign = Inventec.Common.String.Convert.UnSignVNese(ado.RequestDepartmentName);
                    ado.ServiceReqCode = rowSample.SERVICE_REQ_CODE ?? "";
                    ado.TreatmentCode = rowSample.TREATMENT_CODE;
                    ado.SampleTime = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rowSample.SAMPLE_TIME ?? 0);
                    ado.ResultTime = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rowSample.RESULT_TIME ?? 0);
                    ado.AppointmentTime = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(rowSample.APPOINTMENT_TIME ?? 0);
                    if (!String.IsNullOrWhiteSpace(rowSample.SAMPLE_TYPE_CODE))
                    {
                        ado.SampleTypeCode = rowSample.SAMPLE_TYPE_CODE;
                        ado.SampleTypeName = rowSample.SAMPLE_TYPE_NAME;
                    }
                    else
                    {
                        CommonParam cmp = new CommonParam();
                        LIS.Filter.LisSampleServiceFilter fLSS = new LisSampleServiceFilter();
                        fLSS.SAMPLE_ID = rowSample.ID;
                        var lisSampleService = new BackendAdapter(cmp).Get<List<LIS_SAMPLE_SERVICE>>("api/LisSampleService/get", ApiConsumers.LisConsumer, fLSS, cmp);
                        if(lisSampleService != null && lisSampleService.Count > 0)
                        {
                            var vHS = BackendDataWorker.Get<V_HIS_SERVICE>().Where(x => lisSampleService.Any(y => y.SERVICE_CODE == x.SERVICE_CODE)).ToList();

                            ado.SampleTypeCode = string.Join(",", vHS
                                                     .Select(x => x.SAMPLE_TYPE_CODE)
                                                     .Distinct());                           
                        }
                    }
                    BartenderPrintClientManager client = new BartenderPrintClientManager();
                    bool success = client.BartenderPrint(ado);
                    if (!success)
                    {
                        LogSystem.Error("In barcode Bartender that bai. Check log BartenderPrint");
                    }
                }
                else
                {
                    LogSystem.Warn("Khong mo duoc APP Print Bartender");
                }
                txtSearchKey.Focus();
                txtSearchKey.SelectAll();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        #region Print Result

        internal enum PrintTypeKXN
        {
            IN_KET_QUA_XET_NGHIEM,
        }

        void PrintProcess(PrintTypeKXN printType)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);

                switch (printType)
                {
                    case PrintTypeKXN.IN_KET_QUA_XET_NGHIEM:
                        richEditorMain.RunPrintTemplate(MPS.Processor.Mps000096.PDO.PrintTypeCode.Mps000096, DelegateRunPrinterKXN);
                        break;
                    default:
                        break;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        bool DelegateRunPrinterKXN(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case MPS.Processor.Mps000096.PDO.PrintTypeCode.Mps000096:
                        LoadBieuMauInKetQuaXetNghiemV2(printTypeCode, fileName, ref result);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }

        private void LoadBieuMauInKetQuaXetNghiemV2(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();

                MOS.Filter.HisTreatmentFilter treatmentFilter = new HisTreatmentFilter();
                treatmentFilter.ID = currentServiceReq.TREATMENT_ID;
                var curentTreatment = new BackendAdapter(param).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, treatmentFilter, param).FirstOrDefault();

                var currentPatientTypeAlter = new BackendAdapter(param).Get<HIS_PATIENT_TYPE_ALTER>("api/HisPatientTypeAlter/GetLastByTreatmentId", ApiConsumer.ApiConsumers.MosConsumer, currentServiceReq.TREATMENT_ID, param);
                List<V_HIS_TEST_INDEX> currentTestIndexs = new List<V_HIS_TEST_INDEX>();
                List<V_HIS_TEST_INDEX_RANGE> testIndexRanges = new List<V_HIS_TEST_INDEX_RANGE>();
                var testIndex = BackendDataWorker.Get<V_HIS_TEST_INDEX>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                if (_LisResults != null && _LisResults.Count > 0)
                {
                    _LisResults = _LisResults.OrderBy(o => o.SERVICE_NUM_ORDER).ToList();

                    var serviceCodes = _LisResults.Select(o => o.SERVICE_CODE).Distinct().ToList();
                    currentTestIndexs = testIndex.Where(o => serviceCodes.Contains(o.SERVICE_CODE)).ToList();
                    if (currentTestIndexs != null && currentTestIndexs.Count > 0 && testIndex != null && testIndex.Count > 0)
                    {
                        var testIndexCodes = currentTestIndexs.Select(o => o.TEST_INDEX_CODE).Distinct().ToList();
                        testIndexRanges = BackendDataWorker.Get<V_HIS_TEST_INDEX_RANGE>().Where(o => testIndexCodes.Contains(o.TEST_INDEX_CODE)).ToList();
                    }
                }

                MPS.Processor.Mps000096.PDO.Mps000096PDO mps000096RDO = new MPS.Processor.Mps000096.PDO.Mps000096PDO(
                           currentPatientTypeAlter,
                           curentTreatment,
                           rowSample,
                           currentServiceReq,
                           currentTestIndexs,
                           _LisResults,
                           testIndexRanges,
                           LoadGenderId(),
                           BackendDataWorker.Get<V_HIS_SERVICE>());
                string printerName = "";

                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }
                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000096RDO, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName);
                }
                PrintData.ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog;
                result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                WaitingManager.Hide();
            }
        }

        private long LoadGenderId()
        {
            long genderId = 0;
            try
            {
                CommonParam param = new CommonParam();
                if (rowSample != null && !String.IsNullOrWhiteSpace(rowSample.GENDER_CODE))
                {
                    genderId = rowSample.GENDER_CODE == "01" ? 1 : 2;
                }
                else if (rowSample != null && !String.IsNullOrWhiteSpace(rowSample.PATIENT_CODE))
                {
                    HisPatientFilter patientFilter = new HisPatientFilter();
                    patientFilter.PATIENT_CODE = rowSample.PATIENT_CODE;
                    var patients = new BackendAdapter(param).Get<List<HIS_PATIENT>>("api/HisPatient/Get", ApiConsumer.ApiConsumers.MosConsumer, patientFilter, param);
                    if (patients != null && patients.Count > 0)
                    {
                        genderId = patients.FirstOrDefault().GENDER_ID;
                    }
                }
            }
            catch (Exception ex)
            {
                genderId = 0;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return genderId;
        }

        private void CallModuleShowPrintLog(string printTypeCode, string uniqueCode)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(printTypeCode) && !String.IsNullOrWhiteSpace(uniqueCode))
                {
                    //goi modul
                    HIS.Desktop.ADO.PrintLogADO ado = new HIS.Desktop.ADO.PrintLogADO(printTypeCode, uniqueCode);

                    List<object> listArgs = new List<object>();
                    listArgs.Add(ado);

                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("Inventec.Desktop.Plugins.PrintLog", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        private void SetCaptionByLanguageKey()
        {
            try
            {
                if (this.currentModule != null && !String.IsNullOrEmpty(this.currentModule.text))
                {
                    this.Text = this.currentModule.text;
                }
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.SampleCollectionRoom.Resources.Lang", typeof(HIS.Desktop.Plugins.SampleCollectionRoom.SampleCollectionRoomUC).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.btnResetNumOrder.Text = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.btnResetNumOrder.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnInstructionTime.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.txtSERVICE_REQ_CODE__EXACT.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.btnSave.Text = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //this.btnPrint.Text = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.btnPrint.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdCollSTT.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.grdCollSTT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColIndexCode.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.grdColIndexCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColIndexName.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.grdColIndexName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdCollDonvitinh.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.grdCollDonvitinh.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColVallue.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.grdColVallue.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColValueNormal.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.grdColValueNormal.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColOldValue.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.grdColMinValue.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColMaxValue.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.grdColMaxValue.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColLevel.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.grdColLevel.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColIsParent.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.grdColIsParent.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColNote.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.grdColNote.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboFind.Properties.NullText = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.cboFind.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtSearchKey.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.txtSearchKey.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.gridColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.gridColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn15.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.gridColumn15.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.gridColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.gridColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn17.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.gridColumn17.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn11.Caption = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.gridColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem11.Text = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.layoutControlItem11.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem12.Text = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.layoutControlItem12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem34.Text = Inventec.Common.Resource.Get.Value("UC_ConnectionTest.layoutControlItem34.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewTestResult_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                SampleLisResultADO data = null;
                if (e.RowHandle > -1)
                {
                    data = (SampleLisResultADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                }
                if (e.RowHandle >= 0)
                {
                    long is_parent = Inventec.Common.TypeConvert.Parse.ToInt64((gridViewSampleCollectionResult.GetRowCellValue(e.RowHandle, "IS_PARENT") ?? "").ToString());
                    long has_one_child = Inventec.Common.TypeConvert.Parse.ToInt64((gridViewSampleCollectionResult.GetRowCellValue(e.RowHandle, "HAS_ONE_CHILD") ?? "").ToString());
                    if (e.Column.FieldName == "VALUE_RANGE")
                    {
                        if (is_parent == 1 && has_one_child == 0)
                        {
                            e.RepositoryItem = repositoryItemTextValue_Disable;
                            e.Column.OptionsColumn.AllowEdit = false;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemText__OldValue;
                            e.Column.OptionsColumn.AllowEdit = false;
                        }
                    }
                    if (e.Column.FieldName == "OLD_VALUE")
                    {
                        if (is_parent == 1 && has_one_child == 0)
                        {
                            e.RepositoryItem = repositoryItemTextValue_Disable;
                            e.Column.OptionsColumn.AllowEdit = false;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemText__OldValue;
                            e.Column.OptionsColumn.AllowEdit = false;
                        }
                    }
                    else if (e.Column.FieldName == "IMAGE")
                    {
                        if (is_parent == 1)
                        {
                            e.RepositoryItem = repositoryItemButton__TraKetQua;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemPictureEdit2;
                        }
                    }
                    else if (e.Column.FieldName == "MACHINE_ID")
                    {
                        if (is_parent == 1)
                        {
                            e.RepositoryItem = repositoryItemGridLookUp_Machine;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemTextValue_Disable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToGridTestResult2()
        {
            try
            {
                CommonParam param = new CommonParam();

                List<V_HIS_TEST_INDEX_RANGE> testIndexRanges = null;
                List<V_HIS_TEST_INDEX> currentTestIndexs = null;
                var testIndex = BackendDataWorker.Get<V_HIS_TEST_INDEX>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                if (_LisResults != null && _LisResults.Count > 0)
                {
                    _LisResults = _LisResults.OrderBy(o => o.SERVICE_NUM_ORDER).ToList();

                    currentTestIndexs = new List<V_HIS_TEST_INDEX>();
                    var serviceCodes = _LisResults.Select(o => o.SERVICE_CODE).Distinct().ToList();
                    currentTestIndexs = testIndex.Where(o => serviceCodes.Contains(o.SERVICE_CODE)).ToList();
                    if (currentTestIndexs != null && currentTestIndexs.Count > 0 && testIndex != null && testIndex.Count > 0)
                    {
                        var testIndexCodes = currentTestIndexs.Select(o => o.TEST_INDEX_CODE).Distinct().ToList();
                        testIndexRanges = BackendDataWorker.Get<V_HIS_TEST_INDEX_RANGE>().Where(o => testIndexCodes.Contains(o.TEST_INDEX_CODE)).ToList();
                    }
                }
                long genderId = LoadGenderId();
                List<SampleLisResultADO> lstHisSereServTeinSDO = new List<SampleLisResultADO>();
                if (currentTestIndexs != null && currentTestIndexs.Count > 0)
                {
                    var groupListResult = _LisResults.GroupBy(o => o.SERVICE_CODE).ToList();

                    foreach (var group in groupListResult)
                    {
                        SampleLisResultADO hisSereServTeinSDO = new SampleLisResultADO();
                        var fistGroup = group.FirstOrDefault();
                        hisSereServTeinSDO.IS_PARENT = 1;
                        hisSereServTeinSDO.TEST_INDEX_CODE = fistGroup != null ? fistGroup.SERVICE_CODE : "";
                        hisSereServTeinSDO.TEST_INDEX_NAME = fistGroup != null ? fistGroup.SERVICE_NAME : "";
                        hisSereServTeinSDO.SERVICE_CODE = fistGroup != null ? fistGroup.SERVICE_CODE : "";
                        hisSereServTeinSDO.SERVICE_NAME = fistGroup != null ? fistGroup.SERVICE_NAME : "";
                        hisSereServTeinSDO.ID = fistGroup.ID;
                        hisSereServTeinSDO.IS_NO_EXECUTE = fistGroup.IS_NO_EXECUTE;
                        hisSereServTeinSDO.PARENT_ID = ".";
                        hisSereServTeinSDO.MODIFIER = "";
                        hisSereServTeinSDO.CHILD_ID = fistGroup.ID + ".";
                        hisSereServTeinSDO.SERVICE_NUM_ORDER = fistGroup.SERVICE_NUM_ORDER;
                        hisSereServTeinSDO.NUM_ORDER = 999999;
                        //Lay machine_id
                        var lstResultItem = group.ToList();
                        lstResultItem = lstResultItem.OrderBy(o => o.ID).ThenBy(p => p.SERVICE_NAME).ToList();
                        if (this._LisResults != null
                            && this._LisResults.Count > 0
                            && lstResultItem != null
                            && lstResultItem.Count > 0)
                        {
                            var machineByLisResult = this._LisResults.FirstOrDefault(p => p.SERVICE_CODE == hisSereServTeinSDO.SERVICE_CODE);
                            if (machineByLisResult != null)
                            {
                                hisSereServTeinSDO.MACHINE_ID = machineByLisResult.MACHINE_ID;
                                hisSereServTeinSDO.MACHINE_ID_OLD = machineByLisResult.MACHINE_ID;
                            }
                        }
                        var testIndFist = currentTestIndexs.FirstOrDefault(o => o.TEST_INDEX_CODE == lstResultItem[0].TEST_INDEX_CODE);

                        if (lstResultItem != null
                            && lstResultItem.Count == 1
                            && testIndFist != null && testIndFist.IS_NOT_SHOW_SERVICE == 1)
                        {

                            hisSereServTeinSDO.HAS_ONE_CHILD = 1;
                            hisSereServTeinSDO.CHILD_ID = lstResultItem[0].ID + "." + lstResultItem[0].ID;
                            hisSereServTeinSDO.MODIFIER = lstResultItem[0].MODIFIER;
                            hisSereServTeinSDO.TEST_INDEX_CODE = "       " + lstResultItem[0].TEST_INDEX_CODE;
                            hisSereServTeinSDO.TEST_INDEX_NAME = lstResultItem[0].TEST_INDEX_NAME;
                            hisSereServTeinSDO.TEST_INDEX_UNIT_NAME = testIndFist.TEST_INDEX_UNIT_NAME;
                            hisSereServTeinSDO.IS_IMPORTANT = testIndFist.IS_IMPORTANT;
                            hisSereServTeinSDO.SAMPLE_SERVICE_ID = lstResultItem[0].SAMPLE_SERVICE_ID;
                            hisSereServTeinSDO.SAMPLE_SERVICE_STT_ID = lstResultItem[0].SAMPLE_SERVICE_STT_ID;
                            hisSereServTeinSDO.MODIFIER = lstResultItem[0].MODIFIER;
                            hisSereServTeinSDO.VALUE_RANGE = lstResultItem[0].VALUE;
                            hisSereServTeinSDO.LIS_RESULT_ID = lstResultItem[0].ID;
                            hisSereServTeinSDO.ID = lstResultItem[0].ID;
                            hisSereServTeinSDO.SAMPLE_ID = lstResultItem[0].SAMPLE_ID;
                            hisSereServTeinSDO.SAMPLE_SERVICE_ID = lstResultItem[0].SAMPLE_SERVICE_ID;
                            hisSereServTeinSDO.SAMPLE_SERVICE_STT_CODE = lstResultItem[0].SAMPLE_SERVICE_STT_CODE;
                            hisSereServTeinSDO.SAMPLE_SERVICE_STT_ID = lstResultItem[0].SAMPLE_SERVICE_STT_ID;
                            hisSereServTeinSDO.SAMPLE_SERVICE_STT_NAME = lstResultItem[0].SAMPLE_SERVICE_STT_NAME;
                            hisSereServTeinSDO.MACHINE_ID_OLD = lstResultItem[0].MACHINE_ID;
                            hisSereServTeinSDO.MACHINE_ID = lstResultItem[0].MACHINE_ID;
                            hisSereServTeinSDO.NOTE = lstResultItem[0].DESCRIPTION;
                            hisSereServTeinSDO.SERVICE_NUM_ORDER = lstResultItem[0].SERVICE_NUM_ORDER;
                            hisSereServTeinSDO.OLD_VALUE = lstResultItem[0].OLD_VALUE;
                        }
                        lstHisSereServTeinSDO.Add(hisSereServTeinSDO);

                        if (lstResultItem != null
                            && (lstResultItem.Count > 1
                            || (lstResultItem.Count == 1
                            && testIndFist != null && testIndFist.IS_NOT_SHOW_SERVICE != 1))
                            )
                        {
                            foreach (var ssTein in lstResultItem)
                            {
                                var testIndChild = currentTestIndexs.FirstOrDefault(o => o.TEST_INDEX_CODE == ssTein.TEST_INDEX_CODE);
                                SampleLisResultADO hisSereServTein = new SampleLisResultADO();
                                hisSereServTein.HAS_ONE_CHILD = 0;
                                Inventec.Common.Mapper.DataObjectMapper.Map<SampleLisResultADO>(hisSereServTein, ssTein);
                                hisSereServTein.IS_PARENT = 0;

                                if (testIndChild != null)
                                {
                                    hisSereServTein.IS_IMPORTANT = testIndChild.IS_IMPORTANT;
                                    hisSereServTein.TEST_INDEX_UNIT_NAME = testIndChild.TEST_INDEX_UNIT_NAME;
                                    hisSereServTein.NUM_ORDER = testIndChild.NUM_ORDER;
                                }
                                else
                                {
                                    hisSereServTein.NUM_ORDER = null;
                                }
                                hisSereServTein.CHILD_ID = ssTein.ID + "." + ssTein.ID;
                                hisSereServTein.ID = ssTein.ID;
                                hisSereServTein.PARENT_ID = hisSereServTeinSDO.CHILD_ID;
                                hisSereServTein.TEST_INDEX_CODE = "       " + ssTein.TEST_INDEX_CODE;
                                hisSereServTein.TEST_INDEX_NAME = ssTein.TEST_INDEX_NAME;
                                hisSereServTein.MODIFIER = "";
                                hisSereServTeinSDO.SAMPLE_SERVICE_ID = ssTein.SAMPLE_SERVICE_ID;
                                hisSereServTeinSDO.SAMPLE_SERVICE_STT_ID = ssTein.SAMPLE_SERVICE_STT_ID;
                                hisSereServTein.MODIFIER = ssTein.MODIFIER;
                                hisSereServTein.VALUE_RANGE = ssTein.VALUE;
                                hisSereServTein.LIS_RESULT_ID = ssTein.ID;
                                hisSereServTein.MACHINE_ID = ssTein.MACHINE_ID;
                                hisSereServTein.MACHINE_ID_OLD = ssTein.MACHINE_ID;
                                hisSereServTein.SAMPLE_ID = ssTein.SAMPLE_ID;
                                hisSereServTein.SAMPLE_SERVICE_ID = ssTein.SAMPLE_SERVICE_ID;
                                hisSereServTein.SAMPLE_SERVICE_STT_CODE = ssTein.SAMPLE_SERVICE_STT_CODE;
                                hisSereServTein.SAMPLE_SERVICE_STT_ID = ssTein.SAMPLE_SERVICE_STT_ID;
                                hisSereServTein.SAMPLE_SERVICE_STT_NAME = ssTein.SAMPLE_SERVICE_STT_NAME;
                                hisSereServTein.SERVICE_NUM_ORDER = ssTein.SERVICE_NUM_ORDER;
                                hisSereServTein.OLD_VALUE = ssTein.OLD_VALUE;
                                hisSereServTein.NOTE = ssTein.DESCRIPTION;
                                hisSereServTein.DESCRIPTION = "";

                                lstHisSereServTeinSDO.Add(hisSereServTein);
                            }
                        }
                    }
                }
                // gán test index range
                if (lstHisSereServTeinSDO != null && lstHisSereServTeinSDO.Count > 0)
                {

                    foreach (var hisSereServTeinSDO in lstHisSereServTeinSDO)
                    {
                        V_HIS_TEST_INDEX_RANGE testIndexRange = new V_HIS_TEST_INDEX_RANGE();
                        testIndexRange = GetTestIndexRange(this.rowSample.DOB ?? 0, genderId, hisSereServTeinSDO.TEST_INDEX_CODE.Trim(), ref this.testIndexRangeAll);
                        if (testIndexRange != null)
                        {
                            ProcessMaxMixValue(hisSereServTeinSDO, testIndexRange);
                        }
                    }
                }

                if (lstHisSereServTeinSDO != null && lstHisSereServTeinSDO.Count() > 0)
                {
                    lstHisSereServTeinSDO = lstHisSereServTeinSDO.OrderBy(o => o.SERVICE_NUM_ORDER)
                    .ThenByDescending(p => p.NUM_ORDER).ToList();
                }

                // treeList
                gridControlSampleCollectionResult.BeginUpdate();
                gridControlSampleCollectionResult.DataSource = lstHisSereServTeinSDO;
                gridControlSampleCollectionResult.EndUpdate();
                gridViewSampleCollectionResult.FocusedRowHandle = 1;
                gridViewSampleCollectionResult.FocusedColumn = gridViewSampleCollectionResult.VisibleColumns[3];
                gridViewSampleCollectionResult.ShowEditor();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        V_HIS_TEST_INDEX_RANGE GetTestIndexRange(long dob, long genderId, string testIndexId, ref List<V_HIS_TEST_INDEX_RANGE> testIndexRanges)
        {
            V_HIS_TEST_INDEX_RANGE testIndexRange = new V_HIS_TEST_INDEX_RANGE();
            try
            {
                if (testIndexRanges != null && testIndexRanges.Count > 0)
                {
                    long age = Inventec.Common.DateTime.Calculation.Age(dob);

                    var query = testIndexRanges.Where(o => o.TEST_INDEX_CODE == testIndexId
                            && ((o.AGE_FROM.HasValue && o.AGE_FROM.Value <= age) || !o.AGE_FROM.HasValue)
                            && ((o.AGE_TO.HasValue && o.AGE_TO.Value >= age) || !o.AGE_TO.HasValue));
                    HIS_GENDER gender = BackendDataWorker.Get<HIS_GENDER>().FirstOrDefault(p => p.ID == genderId);
                    if (gender != null && gender.ID == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__MALE)
                    {
                        query = query.Where(o => o.IS_MALE == 1);
                    }
                    else if (gender != null && gender.ID == IMSys.DbConfig.HIS_RS.HIS_GENDER.ID__FEMALE)
                    {
                        query = query.Where(o => o.IS_FEMALE == 1);
                    }
                    testIndexRange = query.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                testIndexRange = new V_HIS_TEST_INDEX_RANGE();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return testIndexRange;
        }

        private void ProcessMaxMixValue(SampleLisResultADO ti, V_HIS_TEST_INDEX_RANGE testIndexRange)
        {
            try
            {
                Decimal minValue = 0, maxValue = 0, value = 0;
                if (ti != null && testIndexRange != null)
                {
                    ti.DESCRIPTION = "";
                    if (!String.IsNullOrWhiteSpace(testIndexRange.MIN_VALUE))
                    {
                        if (Decimal.TryParse((testIndexRange.MIN_VALUE ?? "").Replace('.', ','), style, LanguageManager.GetCulture(), out minValue))
                        {
                            ti.MIN_VALUE = minValue;
                        }
                        else
                        {
                            ti.MIN_VALUE = null;
                        }
                    }
                    if (!String.IsNullOrWhiteSpace(testIndexRange.MAX_VALUE))
                    {
                        if (Decimal.TryParse((testIndexRange.MAX_VALUE ?? "").Replace('.', ','), style, LanguageManager.GetCulture(), out maxValue))
                        {
                            ti.MAX_VALUE = maxValue;
                        }
                        else
                        {
                            ti.MAX_VALUE = null;
                        }
                    }

                    if (!String.IsNullOrWhiteSpace(ti.VALUE_RANGE))
                    {
                        if (Decimal.TryParse((ti.VALUE_RANGE ?? "").Replace('.', ','), style, LanguageManager.GetCulture(), out value))
                        {
                            ti.VALUE = value;
                        }
                        else
                        {
                            ti.VALUE = null;
                        }
                    }

                    ti.IS_NORMAL = null;
                    ti.IS_HIGHER = null;
                    ti.IS_LOWER = null;

                    if (!String.IsNullOrEmpty(testIndexRange.NORMAL_VALUE))
                    {
                        ti.DESCRIPTION = testIndexRange.NORMAL_VALUE;
                        if (!String.IsNullOrWhiteSpace(ti.DESCRIPTION) && !String.IsNullOrWhiteSpace(ti.VALUE_RANGE) && ti.VALUE_RANGE.ToUpper() == ti.DESCRIPTION.ToUpper())
                        {
                            ti.IS_NORMAL = true;
                        }
                        else if (!String.IsNullOrWhiteSpace(ti.DESCRIPTION) && !String.IsNullOrWhiteSpace(ti.VALUE_RANGE) && ti.VALUE_RANGE.ToUpper() != ti.DESCRIPTION.ToUpper())
                        {
                            ti.IS_LOWER = true;
                            ti.IS_HIGHER = true;
                        }
                    }
                    else
                    {
                        if (testIndexRange.IS_ACCEPT_EQUAL_MIN == 1 && testIndexRange.IS_ACCEPT_EQUAL_MAX == null)
                        {
                            if (testIndexRange.MIN_VALUE != null)
                            {
                                ti.DESCRIPTION += testIndexRange.MIN_VALUE + "<= ";
                            }

                            ti.DESCRIPTION += "X";

                            if (testIndexRange.MAX_VALUE != null)
                            {
                                ti.DESCRIPTION += " < " + testIndexRange.MAX_VALUE;
                            }

                            if (ti.VALUE != null && ti.MIN_VALUE != null && ti.MIN_VALUE <= ti.VALUE && ti.MAX_VALUE != null && ti.VALUE < ti.MAX_VALUE)
                            {
                                ti.IS_NORMAL = true;
                            }
                            else if (ti.VALUE != null && ti.MIN_VALUE != null && ti.VALUE < ti.MIN_VALUE)
                            {
                                ti.IS_LOWER = true;
                            }
                            else if (ti.VALUE != null && ti.MAX_VALUE != null && ti.MAX_VALUE <= ti.VALUE)
                            {
                                ti.IS_HIGHER = true;
                            }
                        }
                        else if (testIndexRange.IS_ACCEPT_EQUAL_MIN == 1 && testIndexRange.IS_ACCEPT_EQUAL_MAX == 1)
                        {
                            if (testIndexRange.MIN_VALUE != null)
                            {
                                ti.DESCRIPTION += testIndexRange.MIN_VALUE + "<= ";
                            }

                            ti.DESCRIPTION += "X";

                            if (testIndexRange.MAX_VALUE != null)
                            {
                                ti.DESCRIPTION += " <= " + testIndexRange.MAX_VALUE;
                            }

                            if (ti.VALUE != null && ti.MIN_VALUE != null && ti.MIN_VALUE <= ti.VALUE && ti.MAX_VALUE != null && ti.VALUE <= ti.MAX_VALUE)
                            {
                                ti.IS_NORMAL = true;
                            }
                            else if (ti.VALUE != null && ti.MIN_VALUE != null && ti.VALUE < ti.MIN_VALUE)
                            {
                                ti.IS_LOWER = true;
                            }
                            else if (ti.VALUE != null && ti.MAX_VALUE != null && ti.MAX_VALUE < ti.VALUE)
                            {
                                ti.IS_HIGHER = true;
                            }
                        }
                        else if (testIndexRange.IS_ACCEPT_EQUAL_MIN == null && testIndexRange.IS_ACCEPT_EQUAL_MAX == 1)
                        {
                            if (testIndexRange.MIN_VALUE != null)
                            {
                                ti.DESCRIPTION += testIndexRange.MIN_VALUE + "< ";
                            }

                            ti.DESCRIPTION += "X";

                            if (testIndexRange.MAX_VALUE != null)
                            {
                                ti.DESCRIPTION += " <= " + testIndexRange.MAX_VALUE;
                            }

                            if (ti.VALUE != null && ti.MIN_VALUE != null && ti.MIN_VALUE < ti.VALUE && ti.MAX_VALUE != null && ti.VALUE <= ti.MAX_VALUE)
                            {
                                ti.IS_NORMAL = true;
                            }
                            else if (ti.VALUE != null && ti.MIN_VALUE != null && ti.VALUE < ti.MIN_VALUE)
                            {
                                ti.IS_LOWER = true;
                            }
                            else if (ti.VALUE != null && ti.MAX_VALUE != null && ti.MAX_VALUE < ti.VALUE)
                            {
                                ti.IS_HIGHER = true;
                            }
                        }
                        else if (testIndexRange.IS_ACCEPT_EQUAL_MIN == null && testIndexRange.IS_ACCEPT_EQUAL_MAX == null)
                        {
                            if (testIndexRange.MIN_VALUE != null)
                            {
                                ti.DESCRIPTION += testIndexRange.MIN_VALUE + "< ";
                            }

                            ti.DESCRIPTION += "X";

                            if (testIndexRange.MAX_VALUE != null)
                            {
                                ti.DESCRIPTION += " < " + testIndexRange.MAX_VALUE;
                            }
                            if (ti.VALUE != null && ti.MIN_VALUE != null && ti.MIN_VALUE < ti.VALUE && ti.MAX_VALUE != null && ti.VALUE < ti.MAX_VALUE)
                            {
                                ti.IS_NORMAL = true;
                            }
                            else if (ti.VALUE != null && ti.MIN_VALUE != null && ti.VALUE <= ti.MIN_VALUE)
                            {
                                ti.IS_LOWER = true;
                            }
                            else if (ti.VALUE != null && ti.MAX_VALUE != null && ti.MAX_VALUE <= ti.VALUE)
                            {
                                ti.IS_HIGHER = true;
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

        private void gridViewTestResult_RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            try
            {
                long is_parent = Inventec.Common.TypeConvert.Parse.ToInt64(gridViewSampleCollectionResult.GetRowCellValue(e.RowHandle, "IS_PARENT").ToString());
                long has_one_child = Inventec.Common.TypeConvert.Parse.ToInt64(gridViewSampleCollectionResult.GetRowCellValue(e.RowHandle, "HAS_ONE_CHILD").ToString());
                if (is_parent == 1 || has_one_child == 1)
                {
                    e.Appearance.FontStyleDelta = System.Drawing.FontStyle.Bold;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewTestResult_RowStyle(object sender, RowStyleEventArgs e)
        {
            try
            {
                Decimal valueRange;
                SampleLisResultADO data = (SampleLisResultADO)gridViewSampleCollectionResult.GetRow(e.RowHandle);
                if (data != null)
                {
                    if (data.IS_NO_EXECUTE != null)
                    {
                        e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Strikeout);
                    }

                    if (Decimal.TryParse(data.VALUE_RANGE, out valueRange))
                    {
                        if (data.MIN_VALUE != null && valueRange < data.MIN_VALUE)
                        {
                            e.Appearance.ForeColor = Color.Blue;
                            e.HighPriority = true;
                        }
                        else if (data.MAX_VALUE != null && valueRange > data.MAX_VALUE)
                        {
                            e.Appearance.ForeColor = Color.Red;
                            e.HighPriority = true;
                        }
                        else
                        {
                            e.Appearance.ForeColor = Color.Black;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemTextValue_Enable_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    GridView view = gridControlSampleCollectionResult.FocusedView as GridView;
                    if (view != null)
                    {
                        var data = (List<SampleLisResultADO>)view.GridControl.DataSource;
                        if (data.Count == 0)
                            return;
                        for (int i = view.FocusedRowHandle + 1; i < data.Count; i++)
                            if (data[i].IS_PARENT != 1)
                            {
                                view.FocusedColumn = view.Columns[grdColVallue.FieldName];
                                view.FocusedRowHandle = i;
                                view.ShowEditor();
                                break;
                            }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewTestResult_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    SampleLisResultADO data = (SampleLisResultADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null && data.IS_PARENT != 1)
                    {
                        if (e.Column.FieldName == "IMAGE")
                        {
                            long statusId = data.SAMPLE_SERVICE_STT_ID.Value;

                            if (statusId == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_SERVICE_STT.ID__CHUA_CO_KQ)
                            {
                                e.Value = imageList1.Images[0];
                            }
                            else if (statusId == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_SERVICE_STT.ID__DA_CO_KQ)
                            {

                                e.Value = imageList1.Images[1];
                            }
                            else if (statusId == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_SERVICE_STT.ID__DA_TRA_KQ)
                            {
                                e.Value = imageList1.Images[2];
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

        private void toolTipController1_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                if (e.Info == null && e.SelectedControl == gridControlSampleCollectionResult)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = gridControlSampleCollectionResult.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                    GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
                    if (info.InRowCell)
                    {
                        if (lastRowHandle != info.RowHandle || lastColumn != info.Column)
                        {
                            lastColumn = info.Column;
                            lastRowHandle = info.RowHandle;
                            string text = "";
                            if (info.Column.FieldName == "IMAGE")
                            {
                                var data = ((SampleLisResultADO)view.GetRow(lastRowHandle));
                                text = data.SAMPLE_SERVICE_STT_NAME;
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

        private void gridViewTestResult_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                if (e.Column.FieldName != "VALUE_RANGE")
                {
                    return;
                }
                var row = (SampleLisResultADO)gridViewSampleCollectionResult.GetFocusedRow();
                if (row != null && row.LIS_RESULT_ID > 0 && !string.IsNullOrEmpty(row.VALUE_RANGE))
                {
                    row.Item_Edit_Value = 1;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                PrintProcess(PrintTypeKXN.IN_KET_QUA_XET_NGHIEM);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewTestResult_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0 && e.Column.FieldName == "DX$CheckboxSelectorColumn")
                {
                    var data = (SampleLisResultADO)gridViewSampleCollectionResult.GetRow(e.RowHandle);
                    if (data != null && data.IS_PARENT != 1)
                    {
                        e.Handled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewTestResult_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                lstCheckPrint = new List<SampleLisResultADO>();
                if (gridViewSampleCollectionResult.RowCount > 0)
                {
                    for (int i = 0; i < gridViewSampleCollectionResult.SelectedRowsCount; i++)
                    {
                        if (gridViewSampleCollectionResult.GetSelectedRows()[i] >= 0)
                        {
                            lstCheckPrint.Add((SampleLisResultADO)gridViewSampleCollectionResult.GetRow(gridViewSampleCollectionResult.GetSelectedRows()[i]));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void SEARCH()
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

        public void FocusF1()
        {
            try
            {
                txtSearchKey.Focus();
                txtSearchKey.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void FocusF2()
        {
            try
            {
                txtFindServiceReqCode.Focus();
                txtFindServiceReqCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void FocusF3()
        {
            try
            {
                txtSearchKey.Focus();
                txtSearchKey.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void FocusF4()
        {
            try
            {
                txtFindServiceBarCode.Focus();
                txtFindServiceBarCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ShotcurtReCall()
        {
            try
            {
                btnRecallPatient_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ShotcurtCall()
        {
            try
            {
                btnCallPatient_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void PrintBarcode()
        {
            try
            {
                btnPrintBarcode_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewTestResult_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    GridView view = sender as GridView;
                    GridViewInfo viewInfo = view.GetViewInfo() as GridViewInfo;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);
                    if (hi.Column.FieldName == "IMAGE")
                    {
                        var data = (SampleLisResultADO)gridViewSampleCollectionResult.GetRow(hi.RowHandle);
                        if (data != null && data.IS_PARENT == 1 && data.SAMPLE_SERVICE_STT_ID != IMSys.DbConfig.LIS_RS.LIS_SAMPLE_SERVICE_STT.ID__DA_TRA_KQ)
                        {
                            if (hi.HitTest == GridHitTest.RowCell)
                            {
                                WaitingManager.Show();
                                bool success = false;
                                CommonParam param = new CommonParam();

                                List<long> sampleServiceIds = new List<long>();
                                sampleServiceIds.Add(data.SAMPLE_SERVICE_ID.Value);
                                var rs = new BackendAdapter(param).Post<List<LIS_SAMPLE_SERVICE>>("api/LisSampleService/ReturnResult", ApiConsumers
                                    .LisConsumer, sampleServiceIds, param);
                                if (rs != null)
                                {
                                    success = true;
                                    FillDataToGridControl();
                                    gridViewSample_RowCellClick(null, null);
                                }
                                WaitingManager.Hide();
                                MessageManager.Show(this.ParentForm, param, success);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtSearchKey_Click(object sender, EventArgs e)
        {
            try
            {
                txtSearchKey.Focus();
                txtSearchKey.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadCboMachine()
        {
            try
            {
                LIS.Filter.LisMachineFilter filter = new LisMachineFilter();
                this._Machines = new List<LIS_MACHINE>();
                this._Machines = new BackendAdapter(new CommonParam()).Get<List<LIS_MACHINE>>("api/LisMachine/Get", ApiConsumers.LisConsumer, filter, null
                    );
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MACHINE_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("MACHINE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("MACHINE_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(this.repositoryItemGridLookUp_Machine, this._Machines, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewTestResult_ShowingEditor(object sender, CancelEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                SampleLisResultADO data = view.GetFocusedRow() as SampleLisResultADO;
                if (view.FocusedColumn.FieldName == "MACHINE_ID" && view.ActiveEditor is GridLookUpEdit && data.IS_PARENT == 1)
                {
                    GridLookUpEdit editor = view.ActiveEditor as GridLookUpEdit;
                    FillDataMachineCombo(data, editor);
                    //editor.EditValue = data != null ? data.MACHINE_ID : 0;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataMachineCombo(SampleLisResultADO data, DevExpress.XtraEditors.GridLookUpEdit cbo)
        {
            try
            {
                List<LIS_MACHINE> dataMachines = new List<LIS_MACHINE>();
                if (this._Machines != null && this._Machines.Count > 0 && this.rowSample != null)
                {
                    dataMachines = this._Machines.Where(o => o.EXECUTE_ROOM_CODE == this.rowSample.EXECUTE_ROOM_CODE).ToList();
                }
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("MACHINE_CODE", "", 150, 1));
                columnInfos.Add(new ColumnInfo("MACHINE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("MACHINE_NAME", "ID", columnInfos, false, 250);
                ControlEditorLoader.Load(cbo, dataMachines, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnCallPatient_Click(object sender, EventArgs e)
        {
            try
            {
                CreateThreadCallPatient();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnRecallPatient_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnRecallPatient.Enabled)
                    return;
                CreateThreadReCallPatient();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LayMauNhanh()
        {
            try
            {
                frmGetSampleFaster frmGetSampleFaster = new frmGetSampleFaster(this.currentModule, this.room, this.delegateSelectData);
                frmGetSampleFaster.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void delegateSelectData(object result)
        {
            try
            {
                if (result != null)
                {
                    this.FillDataToGridControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GoiBNVaoLayKetQuaNhanh()
        {
            try
            {
                frmCallPatientFaster frmCallPatientFaster = new frmCallPatientFaster(this.currentModule, null);
                frmCallPatientFaster.ShowDialog();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region in phiếu hẹn
        private void onClickBtnPrintPHieuHen()
        {
            try
            {
                PrintType type = new PrintType();
                type = PrintType.IN_PHIEU_HEN;
                PrintProcess(type);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void ButtonEdit_InPhieuHen_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                rowSample = null;
                rowSample = (SampleListViewADO)gridViewSample.GetFocusedRow();

                if (rowSample != null)
                {
                    this.serviceCodeSelect = rowSample.SERVICE_REQ_CODE;
                    if (rowSample.SAMPLE_STT_ID != IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM &&
                        rowSample.SAMPLE_STT_ID != IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TU_CHOI &&
                        !string.IsNullOrEmpty(rowSample.SERVICE_REQ_CODE))
                    {
                        popupMenu1.ShowPopup(Control.MousePosition);
                    }
                    else if (rowSample.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM)
                    {
                        onClickBtnPrintPHieuHen();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void btnCallPatientFaster_Click(object sender, EventArgs e)
        {
            GoiBNVaoLayKetQuaNhanh();
        }

        private void btnGetSampleFaster_Click(object sender, EventArgs e)
        {
            try
            {
                LayMauNhanh();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewSample_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "BARCODE")
                {
                    var focus = (SampleListViewADO)gridViewSample.GetFocusedRow();
                    ProcessUpdateBarcode(focus);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessUpdateBarcode(SampleListViewADO data)
        {
            try
            {
                if (LisConfigCFG.IS_AUTO_CREATE_BARCODE == "0" && LisConfigCFG.IsAutoSampleAfterEnterBarcode)
                {
                    if (data.SAMPLE_STT_ID != IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM && data.SAMPLE_STT_ID != IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TU_CHOI)
                    {
                        UpdateBarcode(data);
                    }
                    else if (data.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM || data.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TU_CHOI)
                    {
                        LayMauBenhPham(data);
                    }

                }
                else if (LisConfigCFG.IS_AUTO_CREATE_BARCODE == "0" && data.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM)
                {
                    UpdateBarcode(data);
                }


                //if (data == null || LisConfigCFG.IS_AUTO_CREATE_BARCODE == "1" || data.SAMPLE_STT_ID != IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM)
                //{
                //    return;
                //}
                //LisSampleUpdateBarcodeSDO input = new LisSampleUpdateBarcodeSDO();
                //input.SampleId = data.ID;
                //input.Barcode = data.BARCODE;
                //CommonParam param = new CommonParam();
                //bool success = false;
                //WaitingManager.Show();
                //var rs = new BackendAdapter(param).Post<LIS_SAMPLE>("api/LisSample/UpdateBarcode", ApiConsumers.LisConsumer, input, param);
                //if (rs != null)
                //{
                //    success = true;
                //    FillDataToGridSample(new CommonParam(this.startPage, this.limit));
                //}
                //WaitingManager.Hide();
                //MessageManager.Show(this.ParentForm, param, success);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        private void UpdateBarcode(SampleListViewADO sample)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool result = false;
                LisSampleUpdateBarcodeSDO input = new LisSampleUpdateBarcodeSDO();
                input.SampleId = sample.ID;
                input.Barcode = sample.BARCODE;

                WaitingManager.Show();
                var rs = new BackendAdapter(param).Post<LIS_SAMPLE>("api/LisSample/UpdateBarcode", ApiConsumers.LisConsumer, input, param);
                if (rs != null)
                {
                    result = true;
                    //lblNewestBarcode.Text = sample.BARCODE;
                }
                else
                {
                    result = false;
                    FillDataToGridControl();
                }
                WaitingManager.Hide();
                MessageManager.Show(this.ParentForm, param, result);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LayMauBenhPham(SampleListViewADO sample)
        {
            try
            {
                if (sample != null && (sample.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM
                    || sample.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TU_CHOI))
                {
                    if (LisConfigCFG.SHOW_FORM_SAMPLE_INFO == "1")
                    {
                        Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "LIS.Desktop.Plugins.SampleInfo").FirstOrDefault();
                        if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'LIS.Desktop.Plugins.SampleInfo'");
                        if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'LIS.Desktop.Plugins.SampleInfo' is not plugins");
                        List<object> listArgs = new List<object>();
                        var dataSend = new V_LIS_SAMPLE();
                        Inventec.Common.Mapper.DataObjectMapper.Map<V_LIS_SAMPLE>(dataSend, sample);
                        listArgs.Add(dataSend);
                        listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                        var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                        if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                        ((Form)extenceInstance).ShowDialog();

                        rowSample = null;
                        FillDataToGridControl();
                        gridViewSample.RefreshData();
                    }
                    else
                    {
                        CommonParam param = new CommonParam();
                        bool result = false;
                        WaitingManager.Show();
                        LisSampleSampleSDO sdo = new LisSampleSampleSDO();
                        sdo.SampleId = sample.ID;
                        sdo.RequestRoomCode = room.ROOM_CODE;
                        sdo.Barcode = sample.BARCODE;
                        var curentSTT = new BackendAdapter(param).Post<LIS_SAMPLE>("api/LisSample/Sample", ApiConsumer.ApiConsumers.LisConsumer, sdo, param);
                        if (curentSTT != null)
                        {
                            rowSample = sample;
                            rowSample.SAMPLE_STT_ID = curentSTT.SAMPLE_STT_ID;
                            rowSample.SAMPLE_TYPE_ID = curentSTT.SAMPLE_TYPE_ID;
                            rowSample.SAMPLE_TIME = curentSTT.SAMPLE_TIME;
                            rowSample.SAMPLE_LOGINNAME = curentSTT.SAMPLE_LOGINNAME;
                            rowSample.SAMPLE_USERNAME = curentSTT.SAMPLE_USERNAME;
                            FillDataToGridControl();
                            result = true;
                            gridViewSample.RefreshData();
                        }

                        WaitingManager.Hide();
                        #region Show message
                        MessageManager.Show(this.ParentForm, param, result);
                        #endregion

                        #region Process has exception
                        SessionManager.ProcessTokenLost(param);
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        private void gridViewSample_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            try
            {
                GridHitInfo hi = e.HitInfo;
                if (hi.InRowCell)
                {
                    var row = (SampleListViewADO)gridViewSample.GetFocusedRow();
                    if (this.baManager == null)
                    {
                        this.baManager = new BarManager();
                        this.baManager.Form = this;
                    }
                    if (row != null)
                    {
                        if (row.IsChecked)
                        {
                            this.popupMenuProcessor = new PopupMenuProcessor(this.baManager, Sample_MouseRightClick);
                            this.popupMenuProcessor.InitMenuChecked();
                        }
                        else
                        {
                            this.popupMenuProcessor = new PopupMenuProcessor(this.baManager, Sample_MouseRightClick);
                            this.popupMenuProcessor.InitMenu();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Sample_MouseRightClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                var row = (SampleListViewADO)gridViewSample.GetFocusedRow();
                if ((e.Item is BarButtonItem) && row != null)
                {
                    var type = (PopupMenuProcessor.ItemType)e.Item.Tag;
                    switch (type)
                    {
                        case PopupMenuProcessor.ItemType.LichSuXetNghiem:
                            this.LichSuXetNghiemCuaBenhNha(row);
                            break;
                        case PopupMenuProcessor.ItemType.LayMau:
                            var ListRow = lstAll.Where(o => o.IsChecked).ToList();
                            this.LayMauChecked(ListRow);
                            break;
                        default:
                            break;
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LayMauChecked(List<SampleListViewADO> lst)
        {
            try
            {
                if (lst != null && lst.Count > 0)
                {
                    if (lst.Select(o => o.TREATMENT_CODE).Distinct().Count() > 1)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Các y lệnh không thuộc 1 hồ sơ điều trị", HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK);
                        return;
                    }
                    if (lst.Exists(o => o.SAMPLE_STT_ID != IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM && o.SAMPLE_STT_ID != IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TU_CHOI))
                    {
                        List<string> serviceReqCodes = lst.Where(o => o.SAMPLE_STT_ID != IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CHUA_LM && o.SAMPLE_STT_ID != IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TU_CHOI).Select(o => o.SERVICE_REQ_CODE).ToList();
                        DevExpress.XtraEditors.XtraMessageBox.Show(String.Format("Y lệnh {0} đã lấy mẫu", string.Join(", ", serviceReqCodes)), HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK);
                        return;
                    }
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "LIS.Desktop.Plugins.SampleInfo").FirstOrDefault();
                    if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'LIS.Desktop.Plugins.SampleInfo'");
                    if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'LIS.Desktop.Plugins.SampleInfo' is not plugins");
                    List<object> listArgs = new List<object>();
                    List<V_LIS_SAMPLE> dataSend = new List<V_LIS_SAMPLE>();
                    foreach (var item in lst)
                    {
                        V_LIS_SAMPLE dt = new V_LIS_SAMPLE();
                        Inventec.Common.Mapper.DataObjectMapper.Map<V_LIS_SAMPLE>(dt, item);
                        dataSend.Add(dt);
                    }
                    listArgs.Add(dataSend);
                    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).ShowDialog();

                    FillDataToGridControl();
                    gridViewSample.RefreshData();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LichSuXetNghiemCuaBenhNha(SampleListViewADO data)
        {
            try
            {
                if (data != null)
                {
                    Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "LIS.Desktop.Plugins.TestHistory").FirstOrDefault();
                    if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'LIS.Desktop.Plugins.TestHistory'");
                    if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'LIS.Desktop.Plugins.TestHistory' is not plugins");
                    List<object> listArgs = new List<object>();
                    listArgs.Add(data.PATIENT_CODE);
                    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId));
                    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("moduleData is null");
                    ((Form)extenceInstance).ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtFindServiceReqCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPrintBarcode_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnPrintBarcode.Enabled) return;
                this.onClickBtnPrintBarCode();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnInPhieuHen_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                onClickBtnPrintPHieuHen();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnInPhieuChiDinh_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                onClickBtnPrintPhieuChiDinh();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void onClickBtnPrintPhieuChiDinh()
        {
            try
            {
                if (!string.IsNullOrEmpty(this.serviceCodeSelect))
                {
                    WaitingManager.Show();
                    HisServiceReqViewFilter filter = new HisServiceReqViewFilter();
                    filter.SERVICE_REQ_CODE = rowSample.SERVICE_REQ_CODE;
                    var _LisServiceReq = new BackendAdapter(new CommonParam()).Get<List<V_HIS_SERVICE_REQ>>("api/HisServiceReq/GetView", ApiConsumer.ApiConsumers.MosConsumer, filter, null);
                    if (_LisServiceReq != null && _LisServiceReq.Count() > 0)
                    {
                        var serviceReq = _LisServiceReq.FirstOrDefault();

                        CommonParam param = new CommonParam();
                        HisServiceReqListResultSDO HisServiceReqSDO = new HisServiceReqListResultSDO();
                        HisServiceReqSDO.SereServs = GetSereServ(serviceReq.TREATMENT_ID, _LisServiceReq.Where(o => o.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN).ToList().Select(o => o.ID).ToList());
                        HisServiceReqSDO.ServiceReqs = _LisServiceReq.Where(o => o.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN).ToList();
                        HisServiceReqSDO.SereServBills = GetSereServBills(serviceReq.TREATMENT_ID, _LisServiceReq.Where(o => o.SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__XN).ToList().Select(o => o.ID).ToList());
                        HisServiceReqSDO.SereServDeposits = GetSereServDeposits(serviceReq.TREATMENT_ID, HisServiceReqSDO.SereServs.Select(o => o.ID).ToList());
                        List<V_HIS_BED_LOG> listBedLogs = GetBedLog(serviceReq.TREATMENT_ID);
                        V_HIS_PATIENT_TYPE_ALTER patientTypeAlter = new BackendAdapter(param).Get<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER>("api/HisPatientTypeAlter/GetViewLastByTreatmentId", ApiConsumers.MosConsumer, serviceReq.TREATMENT_ID, param);
                        HisTreatmentFilter ft = new HisTreatmentFilter();
                        ft.ID = serviceReq.TREATMENT_ID;
                        var currentTreatment = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, ft, param);

                        HisTreatmentWithPatientTypeInfoSDO HisTreatment = new HisTreatmentWithPatientTypeInfoSDO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HisTreatmentWithPatientTypeInfoSDO>(HisTreatment, currentTreatment);

                        if (patientTypeAlter != null)
                        {
                            HisTreatment.PATIENT_TYPE_CODE = patientTypeAlter.PATIENT_TYPE_CODE;
                            HisTreatment.HEIN_CARD_FROM_TIME = patientTypeAlter.HEIN_CARD_FROM_TIME ?? 0;
                            HisTreatment.HEIN_CARD_NUMBER = patientTypeAlter.HEIN_CARD_NUMBER;
                            HisTreatment.HEIN_CARD_TO_TIME = patientTypeAlter.HEIN_CARD_TO_TIME ?? 0;
                            HisTreatment.HEIN_MEDI_ORG_CODE = patientTypeAlter.HEIN_MEDI_ORG_CODE;
                            HisTreatment.LEVEL_CODE = patientTypeAlter.LEVEL_CODE;
                            HisTreatment.RIGHT_ROUTE_CODE = patientTypeAlter.RIGHT_ROUTE_CODE;
                            HisTreatment.RIGHT_ROUTE_TYPE_CODE = patientTypeAlter.RIGHT_ROUTE_TYPE_CODE;
                            HisTreatment.TREATMENT_TYPE_CODE = patientTypeAlter.TREATMENT_TYPE_CODE;
                            HisTreatment.HEIN_CARD_ADDRESS = patientTypeAlter.ADDRESS;
                        }

                        var PrintServiceReqProcessor = new Library.PrintServiceReq.PrintServiceReqProcessor(HisServiceReqSDO, HisTreatment, listBedLogs, currentModule != null ? currentModule.RoomId : 0);
                        WaitingManager.Hide();
                        PrintServiceReqProcessor.Print("Mps000026", false);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<V_HIS_SERE_SERV> GetSereServ(long treatmentId, List<long> srList)
        {
            List<V_HIS_SERE_SERV> rs = null;
            try
            {
                CommonParam param = new CommonParam();
                HisSereServViewFilter filter = new HisSereServViewFilter();
                filter.TREATMENT_ID = treatmentId;
                filter.HAS_EXECUTE = true;
                filter.SERVICE_REQ_IDs = srList;
                rs = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV>>("api/HisSereServ/GetView", ApiConsumers.MosConsumer, filter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }

            return rs;
        }

        private List<HIS_SERE_SERV_BILL> GetSereServBills(long treatmentId, List<long> srList)
        {
            List<HIS_SERE_SERV_BILL> rs = null;
            try
            {
                CommonParam param = new CommonParam();
                HisSereServBillFilter Filter = new HisSereServBillFilter();
                Filter.TDL_TREATMENT_ID = treatmentId;
                Filter.TDL_SERVICE_REQ_IDs = srList;
                rs = new BackendAdapter(param).Get<List<HIS_SERE_SERV_BILL>>("api/HisSereServBill/Get", ApiConsumers.MosConsumer, Filter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
            return rs;
        }

        private List<HIS_SERE_SERV_DEPOSIT> GetSereServDeposits(long treatmentId, List<long> ssList)
        {
            List<HIS_SERE_SERV_DEPOSIT> rs = null;
            try
            {
                CommonParam param = new CommonParam();
                HisSereServDepositFilter Filter = new HisSereServDepositFilter();
                Filter.TDL_TREATMENT_ID = treatmentId;
                Filter.SERE_SERV_IDs = ssList;
                rs = new BackendAdapter(param).Get<List<HIS_SERE_SERV_DEPOSIT>>("api/HisSereServDeposit/Get", ApiConsumers.MosConsumer, Filter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
            return rs;
        }

        private List<V_HIS_BED_LOG> GetBedLog(long treatmentId)
        {
            List<V_HIS_BED_LOG> rs = null;
            try
            {
                CommonParam param = new CommonParam();
                HisBedLogViewFilter bedLogFilter = new HisBedLogViewFilter();
                bedLogFilter.TREATMENT_ID = treatmentId;

                rs = new BackendAdapter(param).Get<List<V_HIS_BED_LOG>>("api/HisBedLog/GetView", ApiConsumers.MosConsumer, bedLogFilter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
            return rs;
        }

        private void txtFindPatientCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnFind_Click(null, null);
                    if (String.IsNullOrWhiteSpace(txtFindPatientCode.Text))
                    {
                        txtFindTreamentCode.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtFindTreamentCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnFind_Click(null, null);
                    if (String.IsNullOrWhiteSpace(txtFindTreamentCode.Text))
                    {
                        dtCreatefrom.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemBtnUpdateBarcodeTime_Enable_Click(object sender, EventArgs e)
        {
            try
            {
                var data = (SampleListViewADO)gridViewSample.GetFocusedRow();
                if (data != null)
                {
                    if (data.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__TRA_KQ
                        || data.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__CO_KQ
                        || data.SAMPLE_STT_ID == IMSys.DbConfig.LIS_RS.LIS_SAMPLE_STT.ID__DUYET_KQ)
                    {
                        return;
                    }
                    WaitingManager.Show();
                    CommonParam param = new CommonParam();
                    bool success = false;
                    var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<LIS_SAMPLE>("api/LisSample/UpdateBarcodeTime", ApiConsumers.LisConsumer, data.ID, param);
                    if (rs != null)
                    {
                        data.SAMPLE_ORDER = rs.SAMPLE_ORDER;
                        success = true;
                    }
                    WaitingManager.Hide();
                    if (success)
                    {
                        MessageManager.Show(this.ParentForm, param, success);
                    }
                    else
                    {
                        MessageManager.Show(param, success);
                    }

                    SessionManager.ProcessTokenLost(param);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewSample_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    GridView view = sender as GridView;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);
                    if (hi.Column.FieldName == "IsChecked")
                    {
                        if (hi.InColumnPanel)
                        {
                            statecheckColumn = !statecheckColumn;
                            this.SetCheckAllColumn(statecheckColumn);
                            this.GridCheckChange(statecheckColumn);
                        }
                        else if (hi.InRowCell)
                        {
                            if (lstAll.Where(o => o.IsChecked).Count() == lstAll.Count)
                            {
                                statecheckColumn = true;
                                this.SetCheckAllColumn(statecheckColumn);
                            }
                            else
                            {
                                statecheckColumn = false;
                                this.SetCheckAllColumn(statecheckColumn);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void GridCheckChange(bool checkedAll)
        {
            try
            {
                foreach (var item in this.lstAll)
                {
                    item.IsChecked = checkedAll;
                }
                this.gridViewSample.BeginUpdate();
                this.gridViewSample.GridControl.DataSource = this.lstAll;
                this.gridViewSample.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTreatmentArea_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string dienDieuTri = "";
                if (_DienDieuTriSelecteds != null && _DienDieuTriSelecteds.Count > 0)
                {
                    foreach (var item in _DienDieuTriSelecteds)
                    {
                        dienDieuTri += item.TREATMENT_TYPE_NAME + ", ";
                    }

                    dienDieuTri = dienDieuTri.TrimEnd(',', ' ');

                    dienDieuTri = dienDieuTri.TrimStart(',', ' ');
                }

                e.DisplayText = dienDieuTri;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtFindServiceBarCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

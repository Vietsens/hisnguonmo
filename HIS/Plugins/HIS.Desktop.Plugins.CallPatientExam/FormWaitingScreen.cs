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
using DevExpress.XtraGrid.Columns;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.CallPatientDepartment.ADO;
using DevExpress.XtraGrid.Views.Grid;
using HIS.Desktop.LocalStorage.LocalData;

namespace HIS.Desktop.Plugins.CallPatientExam
{
    public partial class FormWaitingScreen99 : FormBase
    {
        private List<ADO.RoomADO> LstRoom;
        Dictionary<long, long> dicRoomCol;
        Inventec.Desktop.Common.Modules.Module _Module;
        L_HIS_TREATMENT_BED_ROOM dataBedRoom;
        HIS_SERVICE_REQ dataRoom;
        List<L_HIS_TREATMENT_BED_ROOM> lstTreatmentBedRoom;
        List<HIS_EXECUTE_ROOM> lstHisExRoom;
        HIS_EXECUTE_ROOM HisExecuteRoom;
        int index = 0;
        List<CallPatientExamm> CallPatientExamm_ = new List<CallPatientExamm>();
        int countOldList = 0;
        int categoryChoose = 0;
        public FormWaitingScreen99()
        {
            InitializeComponent();
        }

        public FormWaitingScreen99(List<ADO.RoomADO> lstRoom, Inventec.Desktop.Common.Modules.Module module, int _Category)
            : base(module)
        {
            InitializeComponent();
            try
            {
                this.categoryChoose = _Category;
                this.LstRoom = lstRoom;
                this._Module = module;
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationDirectory, ConfigurationManager.AppSettings["Inventec.Desktop.Icon"]));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FormWaitingScreen_Load(object sender, EventArgs e)
        {
            try
            {
                //  InitColumn();
                //  SetFromConfigToControl();

                FillDataToGridControl();
                //timerReload.Interval = 30000;
                timerReload.Interval = WaitingScreenCFG.TIMER_FOR_AUTO_LOAD_WAITING_SCREENS * 1000;
                timerReload.Enabled = true;
                timerReload.Start();
                timer1.Interval = 5000;
                timer1.Enabled = true;
                timer1.Start();
                //   backgroundWorker1.RunWorkerAsync();
                this.WindowState = FormWindowState.Maximized;
                this.BringToFront();
                // this.TopMost = true;
                this.Focus();
                var BranchId = WorkPlace.GetBranchId();
                var currentHIS_BRANCH = BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == BranchId);
                if (currentHIS_BRANCH != null)
                {
                    lblBenhVien1.Text = currentHIS_BRANCH.BRANCH_NAME;
                    lblBenhVien2.Text = "Địa chỉ: " + currentHIS_BRANCH.ADDRESS + ", Điện thoại:" + currentHIS_BRANCH.PHONE;
                }


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetFromConfigToControl()
        {
            try
            {
                //mau background
                //List<int> parentBackColorCodes = WaitingScreenCFG.PARENT_BACK_COLOR_CODES;
                //if (parentBackColorCodes != null && parentBackColorCodes.Count == 3)
                //{
                //    layoutControlGroup1.AppearanceGroup.BackColor = System.Drawing.Color.FromArgb(parentBackColorCodes[0], parentBackColorCodes[1], parentBackColorCodes[2]);
                //}

                // màu nền grid patients
                List<int> gridpatientBackColorCodes = WaitingScreenCFG.GRID_NUM_ORDERS_BACK_COLOR_CODES;
                if (gridpatientBackColorCodes != null && gridpatientBackColorCodes.Count == 3)
                {
                    gridView1.Appearance.Empty.BackColor = System.Drawing.Color.FromArgb(gridpatientBackColorCodes[0], gridpatientBackColorCodes[1], gridpatientBackColorCodes[2]);
                }


                // màu nền của header danh sách bệnh nhân
                List<int> gridpatientHeaderBackColorCodes = WaitingScreenCFG.GRID_NUM_ORDERS_HEADER_BACK_COLOR_CODES;
                if (gridpatientHeaderBackColorCodes != null && gridpatientHeaderBackColorCodes.Count == 3)
                {
                    foreach (GridColumn Col in gridView1.Columns)
                    {
                        Col.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(gridpatientHeaderBackColorCodes[0], gridpatientHeaderBackColorCodes[1], gridpatientHeaderBackColorCodes[2]);
                    }
                }

                // màu chữ của header danh sách bệnh nhân
                List<int> gridpatientHeaderForceColorCodes = WaitingScreenCFG.GRID_NUM_ORDERS_HEADER_FORCE_COLOR_CODES;
                if (gridpatientHeaderForceColorCodes != null && gridpatientHeaderForceColorCodes.Count == 3)
                {
                    foreach (GridColumn Col in gridView1.Columns)
                    {
                        Col.AppearanceHeader.ForeColor = System.Drawing.Color.FromArgb(gridpatientHeaderForceColorCodes[0], gridpatientHeaderForceColorCodes[1], gridpatientHeaderForceColorCodes[2]);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitColumn()
        {
            try
            {
                if (LstRoom != null && LstRoom.Count > 0)
                {
                    //cỡ chữ mặc định 75
                    dicRoomCol = new Dictionary<long, long>();
                    for (int i = 1; i <= LstRoom.Count; i++)
                    {
                        GridColumn col = gridView1.Columns.AddField("COL_VALUE_" + i);
                        col.VisibleIndex = i - 1;
                        col.Caption = LstRoom[i - 1].EXECUTE_ROOM_CODE;

                        col.OptionsColumn.AllowEdit = false;
                        col.OptionsColumn.AllowFocus = false;
                        col.OptionsColumn.AllowShowHide = false;
                        col.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
                        col.OptionsFilter.AllowFilter = false;

                        col.AppearanceHeader.ForeColor = Color.Red;
                        col.AppearanceHeader.BackColor = Color.Black;
                        col.AppearanceHeader.Font = new Font(col.AppearanceHeader.Font.Name, 75);
                        col.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        col.AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;

                        col.AppearanceCell.BackColor = Color.Blue;
                        //col.AppearanceCell.BorderColor = Color.White;
                        col.AppearanceCell.Font = new Font(col.AppearanceCell.Font.Name, 75);
                        col.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                        col.AppearanceCell.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;

                        if (!dicRoomCol.ContainsKey(LstRoom[i - 1].ID))
                            dicRoomCol[LstRoom[i - 1].ID] = i;
                    }
                    gridView1.BestFitColumns();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView1_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    var rowColor = (Color)gridView1.GetRowCellValue(e.RowHandle, e.Column.FieldName + "_COLOR");
                    if (rowColor != null)
                    {
                        e.Appearance.ForeColor = rowColor;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void timerReload_Tick(object sender, EventArgs e)
        {
            try
            {
                LoadDataGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void LoadDataGrid()
        {
            try
            {
                Task.Factory.StartNew(ExecuteThreadLoadDataGrid);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void ExecuteThreadLoadDataGrid()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate { FillDataToGridControl(); }));
                }
                else
                {
                    FillDataToGridControl();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void CreateNextRow()
        {
            try
            {
                Task.Factory.StartNew(ExecuteThreadCreateNextRow);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void ExecuteThreadCreateNextRow()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate { NextRowGridView(); }));
                }
                else
                {
                    NextRowGridView();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void NextRowGridView()
        {
            try
            {
                if (gridView1.RowCount <= 0)
                    return;
                if (index == countOldList)
                    index = 0;
                gridView1.FocusedRowHandle = index;
                if (index == gridView1.RowCount)
                    index = 0;
                else
                    index++;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void FillDataToGridControl()
        {
            try
            {
                FillDataToGridControlRoom();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void FillDataToGridControlBedRoom()
        {
            try
            {
                if (LstRoom != null && LstRoom.Count > 0)
                {
                    LstRoom = LstRoom.Where(o => o.CategoryChoose == categoryChoose).ToList();

                    CommonParam param = new CommonParam();
                    MOS.Filter.HisBedRoomFilter filterHisBedRoom = new MOS.Filter.HisBedRoomFilter();
                    filterHisBedRoom.ROOM_IDs = LstRoom.Select(o => o.ID).ToList();
                    var apiHisBedRoom = new BackendAdapter(param).Get<List<HIS_BED_ROOM>>("api/HisBedRoom/Get", ApiConsumers.MosConsumer, filterHisBedRoom, param);

                    if (apiHisBedRoom != null && apiHisBedRoom.Count > 0)
                    {
                        MOS.Filter.HisTreatmentBedRoomLViewFilter filterTBedRoom = new MOS.Filter.HisTreatmentBedRoomLViewFilter();
                        filterTBedRoom.BED_ROOM_IDs = apiHisBedRoom.Select(o => o.ID).ToList();
                        filterTBedRoom.IS_IN_ROOM = true;

                        var rsTreatmentBedRoom = new BackendAdapter(param).Get<List<L_HIS_TREATMENT_BED_ROOM>>("api/HisTreatmentBedRoom/GetLView", ApiConsumers.MosConsumer, filterTBedRoom, param);
                        countOldList = rsTreatmentBedRoom.Count();
                        if (rsTreatmentBedRoom != null && rsTreatmentBedRoom.Count > 0)
                        {
                            if (this.InvokeRequired)
                            {
                                this.Invoke(new MethodInvoker(delegate
                                {
                                    gridControl1.BeginUpdate();
                                    gridControl1.DataSource = rsTreatmentBedRoom.OrderBy(o => o.TREATMENT_ID);
                                    gridControl1.EndUpdate();
                                    if (countOldList == index)
                                    {
                                        index = 0;
                                    }
                                    gridView1.FocusedRowHandle = index;
                                }));
                            }
                            else
                            {
                                gridControl1.BeginUpdate();
                                gridControl1.DataSource = rsTreatmentBedRoom.OrderBy(o => o.TREATMENT_ID);
                                gridControl1.EndUpdate();

                            }

                        }
                    }



                    #region Process has exception
                    SessionManager.ProcessTokenLost(param);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void FillDataToGridControlRoom()
        {
            try
            {
                if (LstRoom != null && LstRoom.Count > 0)
                {
                    this.lstHisExRoom = BackendDataWorker.Get<HIS_EXECUTE_ROOM>();

                    //LstRoom = LstRoom.Where(o => o.CategoryChoose == categoryChoose).ToList();
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisServiceReqFilter filter = new MOS.Filter.HisServiceReqFilter();
                    filter.EXECUTE_ROOM_IDs = LstRoom.Select(s => s.ID).ToList();
                    filter.HAS_EXECUTE = true;
                    filter.SERVICE_REQ_TYPE_IDs = new List<long> { IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__KH };
                    filter.INTRUCTION_DATE_FROM = Inventec.Common.DateTime.Get.StartDay();
                    filter.INTRUCTION_DATE_TO = Inventec.Common.DateTime.Get.EndDay();
                    filter.SERVICE_REQ_STT_IDs = new List<long>() {
                        IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL,
                        IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL,
                        IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__HT };
                    var result = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, filter, param);
                    #region
                    if (result != null && result.Count > 0)
                    {
                        foreach (var item in LstRoom.Select(s => s.ID).ToList())
                        {
                            CallPatientExamm CallPatientExamms = new CallPatientExamm();
                            var lstHisExRoom_ = lstHisExRoom.Where(o => o.ROOM_ID == item);
                            var checkSUM_WAIT_PATIENT = result.Where(o => o.SERVICE_REQ_STT_ID == 1 && o.EXECUTE_ROOM_ID == item);


                            if (checkSUM_WAIT_PATIENT != null && checkSUM_WAIT_PATIENT.Count() > 0)
                            {
                                CallPatientExamms.SUM_WAIT_PATIENT = checkSUM_WAIT_PATIENT.Count();
                            }
                            var checkSUM_DOINHG_PATIENT = result.Where(o => o.SERVICE_REQ_STT_ID == 2 && o.EXECUTE_ROOM_ID == item);

                            if (checkSUM_DOINHG_PATIENT.Count() > 0 && checkSUM_DOINHG_PATIENT != null)
                            {
                                CallPatientExamms.SUM_DOINHG_PATIENT = checkSUM_DOINHG_PATIENT.Count();
                            }

                            var checkSUM_DONE_PATIENT = result.Where(o => o.SERVICE_REQ_STT_ID == 3 && o.EXECUTE_ROOM_ID == item);

                            if (checkSUM_DONE_PATIENT != null && checkSUM_DONE_PATIENT.Count() > 0)
                            {
                                CallPatientExamms.SUM_DONE_PATIENT = checkSUM_DONE_PATIENT.Count();
                            }
                            // + STT đang tới: lấy ra stt thỏa mãn điều kiện ( SERVICE_REQ_STT_ID = 2, SERVICE_REQ_TYPE_ID = 1, START_TIME lớn nhất trong HIS_SERVICE_REQ)
                            var checkSTT_PATIENT = result.Where(o => (o.SERVICE_REQ_STT_ID == 2 || o.SERVICE_REQ_TYPE_ID == 1) && o.EXECUTE_ROOM_ID == item);
                            if (checkSTT_PATIENT != null && checkSTT_PATIENT.Count() > 0)
                            {
                                if (checkSTT_PATIENT.OrderByDescending(o => o.START_TIME).FirstOrDefault().START_TIME != null)
                                {
                                    CallPatientExamms.STT_PATIENT = checkSTT_PATIENT.OrderByDescending(o => o.START_TIME).FirstOrDefault().NUM_ORDER.ToString();
                                }
                                else
                                {
                                    CallPatientExamms.STT_PATIENT = "";
                                }
                            }

                            if (CallPatientExamms.SUM_WAIT_PATIENT != null && CallPatientExamms.SUM_DOINHG_PATIENT != null && CallPatientExamms.SUM_DONE_PATIENT != null)
                            {
                                CallPatientExamms.SUM_PATIENT = CallPatientExamms.SUM_WAIT_PATIENT + CallPatientExamms.SUM_DOINHG_PATIENT + CallPatientExamms.SUM_DONE_PATIENT;
                            }

                            var checkNameRoom = lstHisExRoom.Where(o => o.IS_EXAM == 1 && o.ROOM_ID == item).FirstOrDefault();

                            if (checkNameRoom != null)
                            {
                                CallPatientExamms.NameRoom = checkNameRoom.EXECUTE_ROOM_NAME;
                            }

                            CallPatientExamms.TYPE_ID = 1;
                            if (CallPatientExamms != null)
                            {
                                CallPatientExamm_.Add(CallPatientExamms);
                            }


                        }
                    }

                    CommonParam param_ = new CommonParam();
                    MOS.Filter.HisVaccinationExamFilter filter_ = new MOS.Filter.HisVaccinationExamFilter();
                    filter_.EXECUTE_ROOM_IDs = LstRoom.Select(s => s.ID).ToList();
                    filter_.REQUEST_TIME_FROM = Inventec.Common.DateTime.Get.StartDay();
                    filter_.REQUEST_TIME_TO = Inventec.Common.DateTime.Get.EndDay();
                    var result_ = new BackendAdapter(param_).Get<List<HIS_VACCINATION_EXAM>>("api/HisVaccinationExam/Get", ApiConsumers.MosConsumer, filter_, param_);
                    #endregion
                    #region
                    if (result_ != null && result_.Count > 0)
                    {
                        foreach (var item in LstRoom.Select(s => s.ID).ToList())
                        {

                            CallPatientExamm _CallPatientExamm = new CallPatientExamm();

                            var chkSUM_WAIT_PATIENT = result_.Where(o => o.EXECUTE_TIME == null && o.EXECUTE_ROOM_ID == item);

                            if (chkSUM_WAIT_PATIENT != null && chkSUM_WAIT_PATIENT.Count() > 0)
                            {
                                _CallPatientExamm.SUM_WAIT_PATIENT = chkSUM_WAIT_PATIENT.Count();
                            }

                            var chkSUM_DONE_PATIENT = result_.Where(o => o.EXECUTE_TIME != null && o.EXECUTE_ROOM_ID == item);

                            if (chkSUM_DONE_PATIENT != null && chkSUM_DONE_PATIENT.Count() > 0)
                            {
                                _CallPatientExamm.SUM_DONE_PATIENT = chkSUM_DONE_PATIENT.Count();
                            }


                            var chkSTT_PATIENT = result_.Where(o => (o.EXECUTE_TIME != null && o.EXECUTE_ROOM_ID == item)).ToList();

                            if (chkSTT_PATIENT != null && chkSTT_PATIENT.Count > 0)
                            {
                                _CallPatientExamm.STT_PATIENT = chkSTT_PATIENT.OrderByDescending(o => o.EXECUTE_TIME).FirstOrDefault().NUM_ORDER.ToString();

                            }
                            if (_CallPatientExamm.SUM_WAIT_PATIENT != null && _CallPatientExamm.SUM_DONE_PATIENT != null)
                            {
                                _CallPatientExamm.SUM_PATIENT = _CallPatientExamm.SUM_WAIT_PATIENT + _CallPatientExamm.SUM_DONE_PATIENT;
                            }

                            _CallPatientExamm.TYPE_ID = 2;
                            var chkNameRoom = lstHisExRoom.Where(o => o.IS_VACCINE == 1 && o.ROOM_ID == item).FirstOrDefault();

                            if (chkNameRoom != null)
                            {
                                _CallPatientExamm.NameRoom = chkNameRoom.EXECUTE_ROOM_NAME;
                            }
                            if (_CallPatientExamm != null)
                            {
                                CallPatientExamm_.Add(_CallPatientExamm);
                            }

                        }

                    }
                    #endregion
                    CallPatientExamm _CallPatientExammSUM = new CallPatientExamm();
                    _CallPatientExammSUM.NameRoom = "Tổng";
                    _CallPatientExammSUM.SUM_PATIENT = CallPatientExamm_.Select(o => o.SUM_PATIENT).Sum();
                    _CallPatientExammSUM.SUM_DOINHG_PATIENT = CallPatientExamm_.Select(o => o.SUM_DOINHG_PATIENT).Sum();
                    _CallPatientExammSUM.SUM_WAIT_PATIENT = CallPatientExamm_.Select(o => o.SUM_WAIT_PATIENT).Sum();
                    _CallPatientExammSUM.SUM_DONE_PATIENT = CallPatientExamm_.Select(o => o.SUM_DONE_PATIENT).Sum();

                    if (CallPatientExamm_.Where(o => o.TYPE_ID == 1).ToList().Count > 0 && CallPatientExamm_.Where(o => o.TYPE_ID == 2).ToList().Count > 0)
                    {
                        _CallPatientExammSUM.TYPE_ID = 2;
                    }
                    else if (CallPatientExamm_.Where(o => o.TYPE_ID == 1).ToList().Count > 0 && CallPatientExamm_.Where(o => o.TYPE_ID == 2).ToList().Count <= 0)
                    {
                        _CallPatientExammSUM.TYPE_ID = 1;
                    }
                    else if (CallPatientExamm_.Where(o => o.TYPE_ID == 2).ToList().Count > 0 && CallPatientExamm_.Where(o => o.TYPE_ID == 1).ToList().Count <= 0)
                    {
                        _CallPatientExammSUM.TYPE_ID = 2;
                    }
                    else
                    {
                        _CallPatientExammSUM.TYPE_ID = 3;
                    }

                    _CallPatientExammSUM.STT_PATIENT = "-";
                    CallPatientExamm_.Add(_CallPatientExammSUM);
                    if (CallPatientExamm_ != null && CallPatientExamm_.Count() > 0)
                    {
                        //List<CallPatientExamm> CallPatientEx = new List<CallPatientExamm>();
                        //foreach (var item in CallPatientExamm_)
                        //{
                        //    CallPatientEx.Add(item);
                        //}
                        if (this.InvokeRequired)
                        {
                            this.Invoke(new MethodInvoker(delegate
                            {
                                gridControl1.BeginUpdate();
                                gridControl1.DataSource = CallPatientExamm_;
                                gridControl1.EndUpdate();

                                if (countOldList == index)
                                {
                                    index = 0;
                                }
                                gridView1.FocusedRowHandle = index;

                            }));
                        }
                        else
                        {
                            gridControl1.BeginUpdate();
                            gridControl1.DataSource = CallPatientExamm_;
                            gridControl1.EndUpdate();
                        }
                        gridControl1.BeginUpdate();
                        gridControl1.DataSource = CallPatientExamm_;
                        gridControl1.EndUpdate();

                        gridView1.ExpandAllGroups();
                    }



                    #region Process has exception
                    SessionManager.ProcessTokenLost(param);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void gridView1_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                CallPatientExamm data = (CallPatientExamm)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {


                    if (e.Column.FieldName == "TYPE_ID_str")
                    {
                        if (data.TYPE_ID == 1)
                        {
                            e.Value = "KHÁM THƯỜNG";
                        }
                        else if (data.TYPE_ID == 2)
                        {
                            e.Value = "KHÁM TIÊM CHỦNG";
                        }
                        else
                        {
                            e.Value = "";
                        }
                    }



                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

            try
            {
                if (gridView1.RowCount <= 0)
                    e.Cancel = true;
                for (int i = 0; i < gridView1.RowCount; i++)
                {
                    if (backgroundWorker1.CancellationPending)
                    {
                        e.Cancel = true;

                    }
                    else
                    {
                        System.Threading.Thread.Sleep(1 * 1000);
                        backgroundWorker1.ReportProgress(i);
                    }

                }

            }
            catch (Exception ex)
            {
                e.Cancel = true;
                backgroundWorker1.CancelAsync();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Warn(e.ProgressPercentage.ToString());
                gridView1.FocusedRowHandle = e.ProgressPercentage;
                index = e.ProgressPercentage;
                gridView1.RefreshData();
                Application.DoEvents();

            }
            catch (Exception ex)
            {
                backgroundWorker1.CancelAsync();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {

                gridView1.FocusedRowHandle = 0;
                gridView1.RefreshData();
                backgroundWorker1.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                backgroundWorker1.CancelAsync();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FormWaitingScreen99_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                backgroundWorker1.CancelAsync();
                timerReload.Stop();
            }
            catch (Exception ex)
            {
                backgroundWorker1.CancelAsync();
                timerReload.Stop();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                CreateNextRow();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView1_CustomDrawGroupRow(object sender, RowObjectCustomDrawEventArgs e)
        {
            try
            {
                var info = e.Info as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridGroupRowInfo;
                info.GroupText = Convert.ToString(this.gridView1.GetGroupRowValue(e.RowHandle, this.gridColumnPK) ?? "");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }



    }
}

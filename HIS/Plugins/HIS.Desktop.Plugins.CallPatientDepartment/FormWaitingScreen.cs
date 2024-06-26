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

namespace HIS.Desktop.Plugins.CallPatientDepartment
{
    public partial class FormWaitingScreen99 : FormBase
    {
        private List<ADO.RoomADO> LstRoom;
        Dictionary<long, long> dicRoomCol;
        Inventec.Desktop.Common.Modules.Module _Module;
        L_HIS_TREATMENT_BED_ROOM dataBedRoom;
        HIS_SERVICE_REQ dataRoom;
        List<L_HIS_TREATMENT_BED_ROOM> lstTreatmentBedRoom;
        int index = 0;
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
                SetFromConfigToControl();

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
                List<int> gridBackColorCodes = WaitingScreenCFG.PARENT_BACK_COLOR_CODES;
                if (gridBackColorCodes != null && gridBackColorCodes.Count == 3)
                {
                    gridView1.Appearance.Empty.BackColor = System.Drawing.Color.FromArgb(gridBackColorCodes[0], gridBackColorCodes[1], gridBackColorCodes[2]);
                }

                // màu nền của row tt
                List<int> gridpatientBackColorCodes = WaitingScreenCFG.GRID_NUM_ORDERS_BACK_COLOR_CODES;
                if (gridpatientBackColorCodes != null && gridpatientBackColorCodes.Count == 3)
                {
                    gridColumn1.AppearanceCell.BackColor = System.Drawing.Color.FromArgb(gridpatientBackColorCodes[0], gridpatientBackColorCodes[1], gridpatientBackColorCodes[2]);
                }


                // màu nền của header danh sách bệnh nhân
                List<int> gridpatientHeaderBackColorCodes = WaitingScreenCFG.GRID_NUM_ORDERS_HEADER_BACK_COLOR_CODES;
                if (gridpatientHeaderBackColorCodes != null && gridpatientHeaderBackColorCodes.Count == 3)
                {
                    gridColumn1.AppearanceHeader.BackColor = System.Drawing.Color.FromArgb(gridpatientHeaderBackColorCodes[0], gridpatientHeaderBackColorCodes[1], gridpatientHeaderBackColorCodes[2]);
                }

                // màu chữ của header danh sách bệnh nhân
                List<int> gridpatientHeaderForceColorCodes = WaitingScreenCFG.GRID_NUM_ORDERS_HEADER_FORCE_COLOR_CODES;
                if (gridpatientHeaderForceColorCodes != null && gridpatientHeaderForceColorCodes.Count == 3)
                {
                    gridColumn1.AppearanceHeader.ForeColor = System.Drawing.Color.FromArgb(gridpatientHeaderForceColorCodes[0], gridpatientHeaderForceColorCodes[1], gridpatientHeaderForceColorCodes[2]);
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
                        col.Caption = LstRoom[i - 1].ROOM_CODE;

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

        private void LoadDataGrid()
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

        private void CreateNextRow()
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
                if (categoryChoose == 1)
                {
                    FillDataToGridControlBedRoom();
                }
                else
                {
                    FillDataToGridControlRoom();
                }
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
                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("____________" + Inventec.Common.Logging.LogUtil.GetMemberName(() => LstRoom), LstRoom));

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
                    LstRoom = LstRoom.Where(o => o.CategoryChoose == categoryChoose).ToList();
                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("____________" + Inventec.Common.Logging.LogUtil.GetMemberName(() => LstRoom), LstRoom));

                    CommonParam param = new CommonParam();
                    MOS.Filter.HisServiceReqFilter filter = new MOS.Filter.HisServiceReqFilter();

                    filter.EXECUTE_ROOM_IDs = LstRoom.Select(s => s.ID).ToList();
                    filter.HAS_EXECUTE = true;
                    filter.NOT_IN_SERVICE_REQ_TYPE_IDs = new List<long> { 
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK, 
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONM,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONTT,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__G};

                    filter.INTRUCTION_DATE_FROM = Inventec.Common.DateTime.Get.StartDay();
                    filter.INTRUCTION_DATE_TO = Inventec.Common.DateTime.Get.EndDay();

                    filter.SERVICE_REQ_STT_IDs = new List<long>() {
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL,
                    IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL};

                    var result = new BackendAdapter(param).Get<List<HIS_SERVICE_REQ>>("api/HisServiceReq/Get", ApiConsumers.MosConsumer, filter, param);
                    MOS.Filter.HisTreatmentBedRoomLViewFilter filterTBedRoom = new MOS.Filter.HisTreatmentBedRoomLViewFilter();
                    filterTBedRoom.TREATMENT_IDs = result.Select(o => o.TREATMENT_ID).ToList();
                    lstTreatmentBedRoom = new BackendAdapter(param).Get<List<L_HIS_TREATMENT_BED_ROOM>>("api/HisTreatmentBedRoom/GetLView", ApiConsumers.MosConsumer, filterTBedRoom, param);

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("____________" + Inventec.Common.Logging.LogUtil.GetMemberName(() => lstTreatmentBedRoom), lstTreatmentBedRoom));

                    if (result != null && result.Count > 0)
                    {

                        if (this.InvokeRequired)
                        {
                            this.Invoke(new MethodInvoker(delegate
                            {
                                gridControl1.BeginUpdate();
                                gridControl1.DataSource = result.OrderBy(o => o.TREATMENT_ID);
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
                            gridControl1.DataSource = result.OrderBy(o => o.TREATMENT_ID);
                            gridControl1.EndUpdate();

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
        private void gridView1_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;

                    if (categoryChoose == 1)
                    {
                        dataBedRoom = (L_HIS_TREATMENT_BED_ROOM)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    }
                    else
                    {
                        dataRoom = (HIS_SERVICE_REQ)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];

                    }
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    else if (e.Column.FieldName == "TDL_PATIENT_LAST_NAME_STR")
                    {
                        try
                        {
                            string lastName = (view.GetRowCellValue(e.ListSourceRowIndex, "TDL_PATIENT_LAST_NAME") ?? "").ToString();
                            if (!string.IsNullOrEmpty(lastName))
                            {
                                e.Value = lastName;
                            }
                            else
                            {
                                e.Value = "";
                            }

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao TDL_PATIENT_LAST_NAME", ex);
                        }
                    }
                    else if (e.Column.FieldName == "TDL_PATIENT_FIRST_NAME_STR")
                    {
                        try
                        {
                            string firstName = (view.GetRowCellValue(e.ListSourceRowIndex, "TDL_PATIENT_FIRST_NAME") ?? "").ToString();
                            if (!string.IsNullOrEmpty(firstName))
                            {
                                e.Value = firstName;
                            }
                            else
                            {
                                e.Value = "";
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao TDL_PATIENT_FIRST_NAME", ex);
                        }
                    }
                    else if (e.Column.FieldName == "TDL_PATIENT_GENDER_NAME_STR")
                    {
                        try
                        {
                            string genDer = (view.GetRowCellValue(e.ListSourceRowIndex, "TDL_PATIENT_GENDER_NAME") ?? "").ToString();
                            if (!string.IsNullOrEmpty(genDer))
                            {
                                e.Value = genDer;
                            }
                            else
                            {
                                e.Value = "";
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao TDL_PATIENT_GENDER_NAME", ex);
                        }
                    }
                    else if (e.Column.FieldName == "TDL_PATIENT_DOB_STR")
                    {
                        try
                        {
                            if (categoryChoose == 1)
                            {
                                if (dataBedRoom != null)
                                {
                                    e.Value = Inventec.Common.DateTime.Calculation.Age(dataBedRoom.TDL_PATIENT_DOB);
                                }
                                else
                                {
                                    e.Value = "";
                                }
                            }
                            else
                            {
                                if (dataRoom != null)
                                {
                                    e.Value = Inventec.Common.DateTime.Calculation.Age(dataRoom.TDL_PATIENT_DOB);
                                }
                                else
                                {
                                    e.Value = "";
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao TDL_PATIENT_DOB", ex);
                        }
                    }
                    else if (e.Column.FieldName == "BED_ROOM_NAME_STR")
                    {
                        try
                        {
                            string bedRoomName = "";
                            if (categoryChoose == 1)
                            {
                                bedRoomName = (view.GetRowCellValue(e.ListSourceRowIndex, "BED_ROOM_NAME") ?? "").ToString();
                            }
                            else
                            {
                                if (lstTreatmentBedRoom != null && lstTreatmentBedRoom.Count > 0)
                                {
                                    bedRoomName = lstTreatmentBedRoom.Where(o => o.TREATMENT_ID == dataRoom.TREATMENT_ID).FirstOrDefault().BED_ROOM_NAME;
                                }
                            }
                            if (!string.IsNullOrEmpty(bedRoomName))
                            {
                                e.Value = bedRoomName;
                            }
                            else
                            {
                                e.Value = "";
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao BED_ROOM_NAME", ex);
                        }
                    }
                    else if (e.Column.FieldName == "BED_NAME_STR")
                    {
                        try
                        {
                            string bedName = "";

                            if (categoryChoose == 1)
                            {
                                bedName = (view.GetRowCellValue(e.ListSourceRowIndex, "BED_NAME") ?? "").ToString();
                            }
                            else
                            {
                                if (lstTreatmentBedRoom != null && lstTreatmentBedRoom.Count > 0)
                                {
                                    bedName = lstTreatmentBedRoom.Where(o => o.TREATMENT_ID == dataRoom.TREATMENT_ID).FirstOrDefault().BED_NAME;
                                }
                            }
                            if (!string.IsNullOrEmpty(bedName))
                            {
                                e.Value = bedName;
                            }
                            else
                            {
                                e.Value = "";
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Loi set gia tri cho cot ngay tao BED_NAME", ex);
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




    }
}

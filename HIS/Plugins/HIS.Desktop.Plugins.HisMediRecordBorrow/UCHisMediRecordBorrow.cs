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
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.LocalStorage.BackendData;
using MOS.EFMODEL.DataModels;
using Inventec.Common.Controls.EditorLoader;
using DevExpress.XtraEditors.Controls;
using Inventec.Desktop.Common.LanguageManager;
using System.Resources;
using MOS.Filter;
using Inventec.Common.Adapter;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.Utils;
using DevExpress.XtraGrid.Columns;
using DevExpress.Utils.Menu;

namespace HIS.Desktop.Plugins.HisMediRecordBorrow
{
    public partial class UCHisMediRecordBorrow : HIS.Desktop.Utility.UserControlBase
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        long roomId = 0;
        long roomTypeId = 0;
        System.Globalization.CultureInfo cultureLang;
        List<MOS.EFMODEL.DataModels.V_HIS_MEDI_RECORD_BORROW> listTreatmentBorrow;
        Inventec.Desktop.Common.Modules.Module currentModule;

        ToolTipControlInfo lastInfo;
        GridColumn lastColumn = null;
        int lastRowHandle = -1;

        internal string typeCodeFind__KeyWork_InDate = "Trong ngày";//Set lại giá trị trong resource
        internal string typeCodeFind_InDate = "Trong ngày";//Set lại giá trị trong resource
        internal string typeCodeFind__InMonth = "Trong tháng";//Set lại giá trị trong resource

        internal string typeCodeFind__KeyWork_OutDate = "Trong ngày";//Set lại giá trị trong resource
        internal string typeCodeFind_OutDate = "Trong ngày";//Set lại giá trị trong resource
        internal string typeCodeFind__OutMonth = "Trong tháng";//Set lại giá trị trong resource
        #endregion

        #region Construct
        public UCHisMediRecordBorrow()
        {
            InitializeComponent();
            try
            {
                cultureLang = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public UCHisMediRecordBorrow(Inventec.Desktop.Common.Modules.Module _module):base(_module)
        {
            InitializeComponent();
            try
            {
                cultureLang = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                this.currentModule = _module;
                this.roomId = _module.RoomId;
                this.roomTypeId = _module.RoomTypeId;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCHisMediRecordBorrow_Load(object sender, EventArgs e)
        {
            try
            {
                gridControl.ToolTipController = toolTipController1;
                //Gan ngon ngu
                LoadKeysFromlanguage();

                //Gan gia tri mac dinh
                SetDefaultValueControl();

                LoadComboDepartment();

                LoadComboDataStore();

                LoadComboStatus();

                InitBorrowDate();

                InitGiveBackDate();
                //Load du lieu
                FillDataToGrid();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region Private method
        private void LoadKeysFromlanguage()
        {
            try
            {
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisMediRecordBorrow.Resources.Lang", typeof(HIS.Desktop.Plugins.HisMediRecordBorrow.UCHisMediRecordBorrow).Assembly);

                //filter
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCHisMediRecordBorrow.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
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
                txtKeyWord.Text = "";
                txtStoreCode.Text = "";
                cboDataStore.EditValue = null;
                cboDepartment.EditValue = null;
                txtPatientCode.Text = "";
                txtTreatmentCode.Text = "";
                dtGiveDateFrom.DateTime = DateTime.Now;
                dtReceiveDateFrom.EditValue = null;

                cboStatus.EditValue = "3";
                //Focus
                txtStoreCode.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadComboDataStore()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisDataStoreFilter filter = new HisDataStoreFilter();
                filter.IS_ACTIVE = 1;

                var data = new BackendAdapter(param).Get<List<HIS_DATA_STORE>>("api/HisDataStore/Get", ApiConsumer.ApiConsumers.MosConsumer, filter, param);

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("DATA_STORE_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("DATA_STORE_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("DATA_STORE_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(this.cboDataStore, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void LoadComboDepartment()
        {
            try
            {

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("DEPARTMENT_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("DEPARTMENT_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("DEPARTMENT_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(this.cboDepartment, BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.IS_ACTIVE == 1).ToList(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void LoadComboStatus()
        {
            try
            {
                List<StatusADO> ados = new List<StatusADO>();
                ados.Add(new StatusADO("1", "Tất cả"));
                ados.Add(new StatusADO("2", "Đã trả"));
                ados.Add(new StatusADO("3", "Chưa trả"));

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("NAME", "", 200, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("NAME", "ID", columnInfos, false, 200);
                ControlEditorLoader.Load(this.cboStatus, ados, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void FillDataToGrid()
        {
            try
            {
                WaitingManager.Show();
                int pagingSize = ucPaging.pagingGrid != null ? ucPaging.pagingGrid.PageSize : (int)ConfigApplications.NumPageSize;
                GridPaging(new CommonParam(0, pagingSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging.Init(GridPaging, param, pagingSize, this.gridControl);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void GridPaging(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                ApiResultObject<List<MOS.EFMODEL.DataModels.V_HIS_MEDI_RECORD_BORROW>> apiResult = null;
                MOS.Filter.HisMediRecordBorrowViewFilter filter = new MOS.Filter.HisMediRecordBorrowViewFilter();
                SetFilter(ref filter);
                gridView.BeginUpdate();
                apiResult = new Inventec.Common.Adapter.BackendAdapter
                    (paramCommon).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_MEDI_RECORD_BORROW>>
                    ("api/HisMediRecordBorrow/GetView", ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                if (apiResult != null)
                {
                    listTreatmentBorrow = apiResult.Data.OrderByDescending(o => o.MODIFY_TIME).ThenByDescending(o => o.ID).ToList();
                    if (listTreatmentBorrow != null && listTreatmentBorrow.Count > 0)
                    {
                        gridControl.DataSource = listTreatmentBorrow;
                        rowCount = (listTreatmentBorrow == null ? 0 : listTreatmentBorrow.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                    else
                    {
                        gridControl.DataSource = null;
                        rowCount = (listTreatmentBorrow == null ? 0 : listTreatmentBorrow.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                gridView.EndUpdate();

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                gridView.EndUpdate();
            }
        }

        private void SetFilter(ref MOS.Filter.HisMediRecordBorrowViewFilter filter)
        {
            try
            {

                if (!String.IsNullOrEmpty(txtStoreCode.Text))
                {
                    filter.STORE_CODE__EXACT = txtStoreCode.Text;
                }
                else if (!String.IsNullOrEmpty(txtTreatmentCode.Text.Trim()))
                {
                    string code = txtTreatmentCode.Text.Trim();
                    if (code.Length < 12)
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        txtTreatmentCode.Text = code;
                    }
                    filter.TREATMENT_CODE__EXACT = code;
                }
                else if (!String.IsNullOrEmpty(txtPatientCode.Text.Trim()))
                {
                    string code = txtPatientCode.Text.Trim();
                    if (code.Length < 10)
                    {
                        code = string.Format("{0:0000000000}", Convert.ToInt64(code));
                        txtPatientCode.Text = code;
                    }
                    filter.PATIENT_CODE__EXACT = code;
                }
                else
                {
                    filter.ORDER_FIELD = "MODIFY_TIME";
                    filter.ORDER_DIRECTION = "DESC";
                    filter.KEY_WORD = txtKeyWord.Text.Trim();
                    if (cboStatus.EditValue != null)
                    {
                        if (cboStatus.EditValue.ToString() == "2")
                        {
                            filter.IS_RECEIVE = true;
                        }
                        else if (cboStatus.EditValue.ToString() == "3")
                        {
                            filter.IS_RECEIVE = false;
                        }
                    }


                    if (cboDataStore.EditValue != null)
                    {
                        filter.DATA_STORE_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboDataStore.EditValue.ToString());
                    }

                    if (cboDepartment.EditValue != null)
                    {
                        filter.DEPARTMENT_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboDepartment.EditValue.ToString());
                    }
                }
                if (this.typeCodeFind__KeyWork_InDate == this.typeCodeFind_InDate
                        && dtGiveDateFrom.EditValue != null && dtGiveDateFrom.DateTime != DateTime.MinValue)
                {
                    filter.GIVE_DATE__EQUAL = Inventec.Common.TypeConvert.Parse.ToInt64(
                    Convert.ToDateTime(dtGiveDateFrom.EditValue).ToString("yyyyMMdd") + "000000");
                }
                else if (this.typeCodeFind__KeyWork_InDate == typeCodeFind__InMonth
                    && dtGiveDateFrom.EditValue != null && dtGiveDateFrom.DateTime != DateTime.MinValue)
                {
                    filter.GIVE_MONTH__EQUAL = Inventec.Common.TypeConvert.Parse.ToInt64(
                    Convert.ToDateTime(dtGiveDateFrom.EditValue).ToString("yyyyMM") + "00000000");
                }


                if (this.typeCodeFind__KeyWork_OutDate == this.typeCodeFind_OutDate
                    && dtReceiveDateFrom.EditValue != null && dtReceiveDateFrom.DateTime != DateTime.MinValue)
                {
                    filter.RECEIVE_DATE__EQUAL = Inventec.Common.TypeConvert.Parse.ToInt64(
                    Convert.ToDateTime(dtReceiveDateFrom.EditValue).ToString("yyyyMMdd") + "000000");
                }
                else if (this.typeCodeFind__KeyWork_OutDate == typeCodeFind__OutMonth
                    && dtReceiveDateFrom.EditValue != null && dtReceiveDateFrom.DateTime != DateTime.MinValue)
                {
                    filter.RECEIVE_MONTH__EQUAL = Inventec.Common.TypeConvert.Parse.ToInt64(
                    Convert.ToDateTime(dtReceiveDateFrom.EditValue).ToString("yyyyMM") + "00000000");
                }
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
                    MOS.EFMODEL.DataModels.V_HIS_MEDI_RECORD_BORROW data = (MOS.EFMODEL.DataModels.V_HIS_MEDI_RECORD_BORROW)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1 + startPage;
                        }
                        if (e.Column.FieldName == "BORROW_NAME")
                        {
                            if (!string.IsNullOrEmpty(data.BORROW_LOGINNAME))
                            {
                                e.Value = data.BORROW_LOGINNAME + " - " + data.BORROW_USERNAME;
                            }
                            else if (!string.IsNullOrEmpty(data.BORROW_USERNAME))
                            {
                                e.Value = data.BORROW_USERNAME;
                            }
                            
                        }
                        if (e.Column.FieldName == "GIVER_NAME")
                        {
                            if (!string.IsNullOrEmpty(data.GIVER_LOGINNAME))
                            {
                                e.Value = data.GIVER_LOGINNAME + " - " + data.GIVER_USERNAME;
                            }
                        }
                        if (e.Column.FieldName == "RECEIVER_NAME")
                        {
                            if (!string.IsNullOrEmpty(data.RECEIVER_LOGINNAME))
                            {
                                e.Value = data.RECEIVER_LOGINNAME + " - " + data.RECEIVER_USERNAME;
                            }
                        }
                        else if (e.Column.FieldName == "GIVE_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.GIVE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "RECEIVE_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.RECEIVE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "APPOINTMENT_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.APPOINTMENT_TIME ?? 0);
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
                if (e.RowHandle >= 0)
                {
                    var row = (V_HIS_MEDI_RECORD_BORROW)gridView.GetRow(e.RowHandle);
                    if (row != null)
                    {
                        long isActive = long.Parse((gridView.GetRowCellValue(e.RowHandle, "IS_ACTIVE") ?? "").ToString());
                        string creator = (gridView.GetRowCellValue(e.RowHandle, "CREATOR") ?? "").ToString();
                        if (e.Column.FieldName == "Receive")
                        {
                            if (isActive == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE && row.RECEIVE_TIME == null)
                                e.RepositoryItem = Btn_Receive_Enable;
                            else
                                e.RepositoryItem = Btn_Receive_Disable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region Event
        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToGrid();
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
                LoadDefaultData();
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTreatmentBorrowCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtStoreCode.Text))
                    {
                        btnSearch_Click(null, null);
                    }
                    else
                    {
                        txtKeyWord.Focus();
                        txtKeyWord.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyWord_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter) btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataApterClose(MOS.EFMODEL.DataModels.V_HIS_MEDI_RECORD_BORROW ExpMestData)
        {
            try
            {
                MOS.Filter.HisMediRecordBorrowViewFilter filter = new MOS.Filter.HisMediRecordBorrowViewFilter();
                filter.ID = ExpMestData.ID;
                var listTreat = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.V_HIS_MEDI_RECORD_BORROW>>("api/HisMediRecordBorrow/GetView", ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                if (listTreat != null && listTreat.Count == 1)
                {
                    listTreatmentBorrow[listTreatmentBorrow.IndexOf(ExpMestData)] = listTreat.First();
                    gridControl.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        #endregion
        #endregion

        #region Public method
        public void Search()
        {
            try
            {
                btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void Refreshs()
        {
            try
            {
                btnRefresh_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void FocusCode()
        {
            try
            {
                txtStoreCode.Focus();
                txtStoreCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void cboTreatmentBorrowType_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboDataStore.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void cboDepartment_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
        {

            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboDepartment.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void Btn_Receive_Enable_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;
                if (DevExpress.XtraEditors.XtraMessageBox.Show(
                    "Bạn có muốn trả hồ sơ không?", "Thông báo",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    var row = (MOS.EFMODEL.DataModels.V_HIS_MEDI_RECORD_BORROW)gridView.GetFocusedRow();
                    if (row != null)
                    {
                        var data = new MOS.EFMODEL.DataModels.HIS_MEDI_RECORD_BORROW();
                        Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_MEDI_RECORD_BORROW>(data, row);
                        var apiData = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_MEDI_RECORD_BORROW>("api/HisMediRecordBorrow/Receive", ApiConsumer.ApiConsumers.MosConsumer, data, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                        if (apiData != null)
                        {
                            success = true;
                            FillDataToGrid();
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
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTreatmentCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtTreatmentCode.Text))
                    {
                        btnSearch_Click(null, null);
                    }
                    else
                    {
                        txtKeyWord.Focus();
                        txtKeyWord.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPatientCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtPatientCode.Text))
                    {
                        btnSearch_Click(null, null);
                    }
                    else
                    {
                        txtKeyWord.Focus();
                        txtKeyWord.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridView_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    var row = (V_HIS_MEDI_RECORD_BORROW)gridView.GetRow(e.RowHandle);
                    if (row != null)
                    {
                        if (row.APPOINTMENT_TIME != null && row.APPOINTMENT_TIME < Inventec.Common.DateTime.Get.Now())
                        {
                            e.Appearance.ForeColor = Color.Red;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void toolTipController1_GetActiveObjectInfo(object sender, DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                if (e.Info == null && e.SelectedControl == gridControl)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = gridControl.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                    GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
                    if (info.InRowCell)
                    {
                        if (lastRowHandle != info.RowHandle || lastColumn != info.Column)
                        {
                            lastColumn = info.Column;
                            lastRowHandle = info.RowHandle;

                            string text = "";
                            var appointmentTime = (long?)view.GetRowCellValue(lastRowHandle, "APPOINTMENT_TIME");
                            if (appointmentTime != null && appointmentTime < Inventec.Common.DateTime.Get.Now())
                            {
                                text = "Hồ sơ bệnh án đã quá hạn hẹn trả";
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

        private void InitBorrowDate()
        {
            try
            {
                DXPopupMenu menu = new DXPopupMenu();
                DXMenuItem itemInDateCode = new DXMenuItem(typeCodeFind__KeyWork_InDate, new EventHandler(dropDownButtonBorrow_Click));
                itemInDateCode.Tag = "InDate";
                menu.Items.Add(itemInDateCode);

                DXMenuItem itemInMonth = new DXMenuItem(typeCodeFind__InMonth, new EventHandler(dropDownButtonBorrow_Click));
                itemInMonth.Tag = "InMonth";
                menu.Items.Add(itemInMonth);

                dropDownButtonBorrow.DropDownControl = menu;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitGiveBackDate()
        {
            try
            {
                DXPopupMenu menu = new DXPopupMenu();
                DXMenuItem itemOutDateCode = new DXMenuItem(typeCodeFind__KeyWork_OutDate, new EventHandler(dropDownButtonGiveBack_Click));
                itemOutDateCode.Tag = "OutDate";
                menu.Items.Add(itemOutDateCode);

                DXMenuItem itemOutMonth = new DXMenuItem(typeCodeFind__OutMonth, new EventHandler(dropDownButtonGiveBack_Click));
                itemOutMonth.Tag = "OutMonth";
                menu.Items.Add(itemOutMonth);

                dropDownButtonGiveBack.DropDownControl = menu;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        
        private void btnPreBorrowTime_Click(object sender, EventArgs e)
        {
            try
            {
                if (dtGiveDateFrom.EditValue != null && dtGiveDateFrom.DateTime != DateTime.MinValue && !String.IsNullOrWhiteSpace(dropDownButtonBorrow.Text))
                {
                    var currentdate = dtGiveDateFrom.DateTime;
                    if (this.typeCodeFind__KeyWork_InDate == this.typeCodeFind_InDate)
                        dtGiveDateFrom.EditValue = currentdate.AddDays(-1);
                    else
                        dtGiveDateFrom.EditValue = currentdate.AddMonths(-1);

                    btnSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnNextBorrowTime_Click(object sender, EventArgs e)
        {
            try
            {
                if (dtGiveDateFrom.EditValue != null && dtGiveDateFrom.DateTime != DateTime.MinValue && !String.IsNullOrWhiteSpace(dropDownButtonBorrow.Text))
                {
                    var currentdate = dtGiveDateFrom.DateTime;
                    if (this.typeCodeFind__KeyWork_InDate == this.typeCodeFind_InDate)
                        dtGiveDateFrom.EditValue = currentdate.AddDays(1);
                    else
                        dtGiveDateFrom.EditValue = currentdate.AddMonths(1);

                    btnSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPreGiveBackTime_Click(object sender, EventArgs e)
        {
            try
            {
                if (dtReceiveDateFrom.EditValue != null && dtReceiveDateFrom.DateTime != DateTime.MinValue && !String.IsNullOrWhiteSpace(dropDownButtonGiveBack.Text))
                {
                    var currentdate = dtReceiveDateFrom.DateTime;
                    if (this.typeCodeFind__KeyWork_OutDate == this.typeCodeFind_OutDate)
                        dtReceiveDateFrom.EditValue = currentdate.AddDays(-1);
                    else
                        dtReceiveDateFrom.EditValue = currentdate.AddMonths(-1);

                    btnSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnNextGiveBackTime_Click(object sender, EventArgs e)
        {
            try
            {
                if (dtReceiveDateFrom.EditValue != null && dtReceiveDateFrom.DateTime != DateTime.MinValue && !String.IsNullOrWhiteSpace(dropDownButtonGiveBack.Text))
                {
                    var currentdate = dtReceiveDateFrom.DateTime;
                    if (this.typeCodeFind__KeyWork_OutDate == this.typeCodeFind_OutDate)
                        dtReceiveDateFrom.EditValue = currentdate.AddDays(1);
                    else
                        dtReceiveDateFrom.EditValue = currentdate.AddMonths(1);

                    btnSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FormatDtBorrowDate()
        {
            try
            {
                if (this.typeCodeFind__KeyWork_InDate == this.typeCodeFind_InDate)
                {
                    dtGiveDateFrom.Properties.VistaCalendarViewStyle = DevExpress.XtraEditors.VistaCalendarViewStyle.Default;
                    dtGiveDateFrom.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dtGiveDateFrom.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
                    dtGiveDateFrom.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dtGiveDateFrom.Properties.EditFormat.FormatString = "dd/MM/yyyy";
                    dtGiveDateFrom.Properties.EditMask = "dd/MM/yyyy";
                    dtGiveDateFrom.Properties.Mask.EditMask = "dd/MM/yyyy";
                }
                else
                {
                    dtGiveDateFrom.Properties.VistaCalendarViewStyle = DevExpress.XtraEditors.VistaCalendarViewStyle.YearView;
                    dtGiveDateFrom.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dtGiveDateFrom.Properties.DisplayFormat.FormatString = "MM/yyyy";
                    dtGiveDateFrom.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dtGiveDateFrom.Properties.EditFormat.FormatString = "MM/yyyy";
                    dtGiveDateFrom.Properties.EditMask = "MM/yyyy";
                    dtGiveDateFrom.Properties.Mask.EditMask = "MM/yyyy";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FormatDtGiveBackDate()
        {
            try
            {
                if (this.typeCodeFind__KeyWork_OutDate == this.typeCodeFind_OutDate)
                {
                    dtReceiveDateFrom.Properties.VistaCalendarViewStyle = DevExpress.XtraEditors.VistaCalendarViewStyle.Default;
                    dtReceiveDateFrom.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dtReceiveDateFrom.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
                    dtReceiveDateFrom.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dtReceiveDateFrom.Properties.EditFormat.FormatString = "dd/MM/yyyy";
                    dtReceiveDateFrom.Properties.EditMask = "dd/MM/yyyy";
                    dtReceiveDateFrom.Properties.Mask.EditMask = "dd/MM/yyyy";
                }
                else
                {
                    dtReceiveDateFrom.Properties.VistaCalendarViewStyle = DevExpress.XtraEditors.VistaCalendarViewStyle.YearView;
                    dtReceiveDateFrom.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dtReceiveDateFrom.Properties.DisplayFormat.FormatString = "MM/yyyy";
                    dtReceiveDateFrom.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    dtReceiveDateFrom.Properties.EditFormat.FormatString = "MM/yyyy";
                    dtReceiveDateFrom.Properties.EditMask = "MM/yyyy";
                    dtReceiveDateFrom.Properties.Mask.EditMask = "MM/yyyy";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dropDownButtonBorrow_Click(object sender, EventArgs e)
        {
            try
            {
                var btnMenuCodeFind = sender as DXMenuItem;
                dropDownButtonBorrow.Text = btnMenuCodeFind.Caption;
                this.typeCodeFind__KeyWork_InDate = btnMenuCodeFind.Caption;

                FormatDtBorrowDate();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dropDownButtonGiveBack_Click(object sender, EventArgs e)
        {
            try
            {
                var btnMenuCodeFind = sender as DXMenuItem;
                dropDownButtonGiveBack.Text = btnMenuCodeFind.Caption;
                this.typeCodeFind__KeyWork_OutDate = btnMenuCodeFind.Caption;

                FormatDtGiveBackDate();

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
                dropDownButtonBorrow.Text = typeCodeFind__KeyWork_InDate;
                FormatDtBorrowDate();
                FormatDtGiveBackDate();
                dtGiveDateFrom.DateTime = DateTime.Now;
                //dtOutHospital.DateTime = null;
                //cboFind.SelectedIndex = 0;
                //cboTreatmentType.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

    }
}

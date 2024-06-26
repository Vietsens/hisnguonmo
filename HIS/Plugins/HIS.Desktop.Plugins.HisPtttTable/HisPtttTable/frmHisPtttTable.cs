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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.Utility;
using Inventec.UC.Paging;
using Inventec.Common.Logging;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.ConfigApplication;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using Inventec.Common.Logging;
using HIS.Desktop.LocalStorage.LocalData;
using DevExpress.XtraEditors;
using HIS.Desktop.LibraryMessage;
using DevExpress.XtraEditors.DXErrorProvider;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using Inventec.Common.Adapter;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.ApiConsumer;
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.Plugins.HisPtttTable.Validate;
using HIS.Desktop.ModuleExt;

namespace HIS.Desktop.Plugins.HisPtttTable.HisPtttTable
{
    public partial class frmHisPtttTable : FormBase
    {
        #region Reclare variable
        PagingGrid pagingGrid;
        Inventec.Desktop.Common.Modules.Module moduleData;
        int ActionType = -1;
        int startPage = 0;
        int rowCount = 0;
        int dataTotal = 0;
        int positionHandle = -1;
        long PtttTableID;
        private const short IS_ACTIVE_FALSE = 0;
        private const short IS_ACTIVE_TRUE = 1;
        V_HIS_PTTT_TABLE currentData;
        #endregion

        #region
        public frmHisPtttTable(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();

                pagingGrid = new PagingGrid();
                this.moduleData = moduleData;
                grdControlHisPtttTable.ToolTipController = toolTipControllerGrid;

                try
                {
                    string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                }
                catch (Exception ex)
                {
                    LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region ---Private method

        private void frmHisPtttTable_Load(object sender, EventArgs e)
        {
            try
            {
                Show();
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void Show()
        {
            SetDefaultValue();

            EnableControlChanged(this.ActionType);

            FillDataToControl();
            //load data len cbo excute room
            InitComboPatientType();
            // kiem tra du lieu nhap vao
            Validate();
            //set ngon ngu
            SetCaptionByLanguagekey();
        }

        private void SetDefaultValue()
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;
                txtSearch.Text = "";
                txtPtttTableCode.Text = "";
                txtPtttTableName.Text = "";

            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void EnableControlChanged(int action)
        {
            btnEdit.Enabled = (action == GlobalVariables.ActionEdit);
            btnAdd.Enabled = (action == GlobalVariables.ActionAdd);
            txtPtttTableCode.ReadOnly = !(action == GlobalVariables.ActionAdd);
        }

        private void FillDataToControl()
        {
            try
            {
                WaitingManager.Show();
                int pagingSize = 0;
                if (ucPaging2.pagingGrid != null)
                {
                    pagingSize = ucPaging2.pagingGrid.PageSize;
                }
                else
                {
                    pagingSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }
                LoadPaging(new CommonParam(0, pagingSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging2.Init(LoadPaging, param, pagingSize, this.grdControlHisPtttTable);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void LoadPaging(object param)
        {

            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                //limit=pageSize
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramcommon = new CommonParam(startPage, limit);
                ApiResultObject<List<V_HIS_PTTT_TABLE>> apiResult = null;
                MOS.Filter.HisPtttTableViewFilter filter = new MOS.Filter.HisPtttTableViewFilter();
                SetFilter(ref filter);
                grdControlHisPtttTable.DataSource = null;
                grdViewHisPtttTable.BeginUpdate();
                apiResult = new BackendAdapter(paramcommon).GetRO<List<V_HIS_PTTT_TABLE>>(HisRequestUriStore.HisPtttTable_Get, ApiConsumers.MosConsumer, filter, paramcommon);
                if (apiResult != null)
                {
                    var data = (List<V_HIS_PTTT_TABLE>)apiResult.Data;
                    if (data != null && data.Count > 0)
                    {
                        grdControlHisPtttTable.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }

                }

                grdViewHisPtttTable.EndUpdate();
                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramcommon);
                #endregion

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetFilter(ref MOS.Filter.HisPtttTableViewFilter filter)
        {
            try
            {
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.KEY_WORD = txtSearch.Text.Trim();
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void RestFormData()
        {
            try
            {
                if (!lcEditorInfo.IsInitialized)
                    return;
                lcEditorInfo.BeginUpdate();
                try
                {
                    foreach (DevExpress.XtraLayout.BaseLayoutItem item in lcEditorInfo.Items)
                    {
                        DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                        if (lci != null && lci.Control != null && lci.Control is BaseEdit)
                        {
                            DevExpress.XtraEditors.BaseEdit fomatFrm = lci.Control as DevExpress.XtraEditors.BaseEdit;
                            fomatFrm.ResetText();
                            fomatFrm.EditValue = null;
                            txtPtttTableCode.Focus();
                            txtPtttTableCode.SelectAll();
                        }
                    }

                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }
                finally
                {
                    lcEditorInfo.EndUpdate();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SaveProcess()
        {
            CommonParam param = new CommonParam();
            try
            {
                bool success = false;
                if (!dxValidationProvider1.Validate())
                    return;
                WaitingManager.Show();
                HIS_PTTT_TABLE updateDTO = new HIS_PTTT_TABLE();
                UpdataDTOFromDataFrom(ref updateDTO);
                if (ActionType == GlobalVariables.ActionAdd)
                {
                    var resultData = new BackendAdapter(param).Post<HIS_PTTT_TABLE>(HisRequestUriStore.HisPtttTable_Create, ApiConsumer.ApiConsumers.MosConsumer, updateDTO, param);
                    if (resultData != null)
                    {
                        BackendDataWorker.Reset<HIS_PTTT_TABLE>();
                        success = true;
                        FillDataToControl();
                        RestFormData();
                    }

                }
                else
                {
                    if (PtttTableID > 0)
                    {
                        updateDTO.ID = PtttTableID;
                        var ResultData = new BackendAdapter(param).Post<HIS_PTTT_TABLE>(HisRequestUriStore.HisPtttTable_Update, ApiConsumer.ApiConsumers.MosConsumer, updateDTO, param);
                        if (ResultData != null)
                        {
                            BackendDataWorker.Reset<HIS_PTTT_TABLE>();
                            success = true;
                            FillDataToControl();
                        }

                    }
                }
                WaitingManager.Hide();
                #region Hien thi message thong bao
                MessageManager.Show(this, param, success);
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {

                WaitingManager.Hide();
                LogSystem.Warn(ex);
            }
        }

        private void UpdataDTOFromDataFrom(ref HIS_PTTT_TABLE data)
        {
            try
            {
                data.PTTT_TABLE_CODE = txtPtttTableCode.Text.Trim();
                data.PTTT_TABLE_NAME = txtPtttTableName.Text.Trim();
                data.EXECUTE_ROOM_ID = (long)(cboExcuteRoomID.EditValue);
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void changeDataRow(V_HIS_PTTT_TABLE data)
        {
            try
            {
                if (data != null)
                {
                    fillDataEditorControl(data);
                    this.ActionType = GlobalVariables.ActionEdit;
                    EnableControlChanged(ActionType);
                    btnEdit.Enabled = (this.currentData.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);
                    positionHandle = -1;
                    Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);

                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void fillDataEditorControl(V_HIS_PTTT_TABLE data)
        {
            try
            {
                if (data != null)
                {
                    PtttTableID = data.ID;
                    txtPtttTableCode.Text = data.PTTT_TABLE_CODE;
                    txtPtttTableName.Text = data.PTTT_TABLE_NAME;
                    cboExcuteRoomID.EditValue = data.EXECUTE_ROOM_ID;
                    txtExcuteRoomID.Text = data.EXECUTE_ROOM_CODE;
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void SetFocusEditor()
        {
            try
            {
                //TODO

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

        private void InitComboPatientType()
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisExecuteRoomFilter filter = new MOS.Filter.HisExecuteRoomFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                var data = new BackendAdapter(param).Get<List<HIS_EXECUTE_ROOM>>("api/HisExecuteRoom/Get", ApiConsumers.MosConsumer, filter, null).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EXECUTE_ROOM_CODE", "Mã phòng khám", 100, 1));
                columnInfos.Add(new ColumnInfo("EXECUTE_ROOM_NAME", "Tên phòng khám", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("EXECUTE_ROOM_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboExcuteRoomID, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadtxtBidCode(string text)
        {
            try
            {
                List<HIS_EXECUTE_ROOM> listResult = new List<HIS_EXECUTE_ROOM>();
                listResult = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_EXECUTE_ROOM>().Where(o => (o.EXECUTE_ROOM_CODE != null && o.EXECUTE_ROOM_CODE.StartsWith(text))).ToList();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("EXECUTE_ROOM_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("EXECUTE_ROOM_NAME", "", 250, 2));

                if (listResult.Count == 1)
                {
                    cboExcuteRoomID.EditValue = listResult[0].ID;
                    txtExcuteRoomID.Text = listResult[0].EXECUTE_ROOM_CODE;
                    if (this.ActionType == GlobalVariables.ActionEdit)
                    {
                        btnEdit.Focus();
                    }
                    else
                    {
                        btnAdd.Focus();
                    }
                }
                else if (listResult.Count > 1)
                {
                    cboExcuteRoomID.EditValue = null;
                    cboExcuteRoomID.Focus();
                    cboExcuteRoomID.ShowPopup();
                    //PopupLoader.SelectFirstRowPopup(cboServiceUnit);
                }
                else
                {
                    cboExcuteRoomID.EditValue = null;
                    cboExcuteRoomID.Focus();
                    cboExcuteRoomID.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region ---button click
        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToControl();
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                SaveProcess();
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                SaveProcess();
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void btnRest_Click(object sender, EventArgs e)
        {
            this.ActionType = GlobalVariables.ActionAdd;
            EnableControlChanged(this.ActionType);
            positionHandle = -1;
            Inventec.Desktop.Controls.ControlWorker.ValidationProviderRemoveControlError(dxValidationProvider1, dxErrorProvider1);
            RestFormData();
            txtPtttTableCode.Focus();
        }

        private void bbtnEdit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (this.ActionType == GlobalVariables.ActionEdit && btnEdit.Enabled)
                {
                    btnEdit_Click(null, null);
                }

            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void bbtnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (this.ActionType == GlobalVariables.ActionAdd && btnAdd.Enabled)
                {
                    btnAdd_Click(null, null);
                }

            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void bbtnRest_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnRest_Click(null, null);
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void bbtnRestFocust_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void btnLock_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            V_HIS_PTTT_TABLE HisPtttTable = new V_HIS_PTTT_TABLE();
            bool notHandler = false;
            try
            {
                V_HIS_PTTT_TABLE rowdata = (V_HIS_PTTT_TABLE)grdViewHisPtttTable.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {

                    WaitingManager.Show();
                    HisPtttTable = new BackendAdapter(param).Post<V_HIS_PTTT_TABLE>(HisRequestUriStore.HisPtttTable_ChangeLock, ApiConsumer.ApiConsumers.MosConsumer, rowdata.ID, param);
                    WaitingManager.Hide();
                    if (HisPtttTable != null) FillDataToControl();
                }
                notHandler = true;
                BackendDataWorker.Reset<V_HIS_PTTT_TABLE>();
                MessageManager.Show(this.ParentForm, param, notHandler);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnUnLock_Click(object sender, EventArgs e)
        {
            CommonParam param = new CommonParam();
            V_HIS_PTTT_TABLE HisPtttTable = new V_HIS_PTTT_TABLE();
            bool notHandler = false;
            try
            {
                V_HIS_PTTT_TABLE rowdata = (V_HIS_PTTT_TABLE)grdViewHisPtttTable.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {

                    WaitingManager.Show();
                    HisPtttTable = new BackendAdapter(param).Post<V_HIS_PTTT_TABLE>(HisRequestUriStore.HisPtttTable_ChangeLock, ApiConsumer.ApiConsumers.MosConsumer, rowdata.ID, param);
                    WaitingManager.Hide();
                    if (HisPtttTable != null) FillDataToControl();
                }
                notHandler = true;
                BackendDataWorker.Reset<V_HIS_PTTT_TABLE>();
                MessageManager.Show(this.ParentForm, param, notHandler);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                var rowData = (V_HIS_PTTT_TABLE)grdViewHisPtttTable.GetFocusedRow();
                if (MessageBox.Show(LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (rowData != null)
                    {
                        bool success = false;
                        success = new BackendAdapter(param).Post<bool>(HisRequestUriStore.HisPtttTable_Delete, ApiConsumers.MosConsumer, rowData.ID, param);
                        if (success)
                        {
                            FillDataToControl();
                            currentData = ((List<V_HIS_PTTT_TABLE>)grdControlHisPtttTable.DataSource).FirstOrDefault();
                        }
                        MessageManager.Show(this, param, success);
                    }
                }
            }
            catch (Exception ex)
            {

                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region ---KeyUp
        private void txtSearch_KeyUp(object sender, KeyEventArgs e)
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

                LogSystem.Warn(ex);
            }
        }
        #endregion

        #region ---grdViewHisPtttTable
        private void grdViewHisPtttTable_Click(object sender, EventArgs e)
        {
            try
            {
                var rowData = (V_HIS_PTTT_TABLE)grdViewHisPtttTable.GetFocusedRow();
                if (rowData != null)
                {
                    currentData = rowData;
                    changeDataRow(rowData);
                    SetFocusEditor();
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void grdViewHisPtttTable_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    V_HIS_PTTT_TABLE data = (V_HIS_PTTT_TABLE)grdViewHisPtttTable.GetRow(e.RowHandle);
                    if (e.Column.FieldName == "IS_ACTIVE_STR")
                    {
                        if (data.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE)
                            e.Appearance.ForeColor = Color.Red;
                        else
                            e.Appearance.ForeColor = Color.Green;
                    }
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void grdViewHisPtttTable_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    V_HIS_PTTT_TABLE data = (V_HIS_PTTT_TABLE)((IList)((BaseView)sender).DataSource)[e.RowHandle];

                    if (e.Column.FieldName == "LOCK")
                    {
                        e.RepositoryItem = (data.IS_ACTIVE == IS_ACTIVE_FALSE ? btnUnLock : btnLock);

                    }
                    if (e.Column.FieldName == "DELETE")
                    {
                        if (data.IS_ACTIVE == IS_ACTIVE_TRUE)
                        {
                            e.RepositoryItem = btnDelete;
                        }
                        else
                            e.RepositoryItem = btnEnableDelete;
                    }
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void grdViewHisPtttTable_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    V_HIS_PTTT_TABLE data = (V_HIS_PTTT_TABLE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1 + startPage;
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "IS_ACTIVE_STR")
                        {
                            e.Value = data.IS_ACTIVE == 1 ? "Hoạt động" : "Tạm khóa";
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void grdViewHisPtttTable_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var rowdata = (MOS.EFMODEL.DataModels.V_HIS_PTTT_TABLE)grdViewHisPtttTable.GetFocusedRow();
                    if (rowdata != null)
                    {
                        changeDataRow(rowdata);
                        SetFocusEditor();
                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region ---KeyDown
        private void txtPtttTableCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtPtttTableName.Focus();
                    txtPtttTableName.SelectAll();
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void txtPtttTableName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtExcuteRoomID.Focus();
                    txtExcuteRoomID.SelectAll();
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void txtExcuteRoomID_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var text = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    LoadtxtBidCode(text);
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        #endregion

        private void cboExcuteRoomID_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboExcuteRoomID.EditValue != null && cboExcuteRoomID.EditValue != cboExcuteRoomID.OldEditValue)
                    {
                        var PTTTTableLine = BackendDataWorker.Get<HIS_EXECUTE_ROOM>().SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((cboExcuteRoomID.EditValue ?? "").ToString()));
                        if (PTTTTableLine != null)
                        {
                            txtExcuteRoomID.Text = PTTTTableLine.EXECUTE_ROOM_CODE;
                            if (this.ActionType == GlobalVariables.ActionEdit && btnEdit.Enabled)
                            {
                                btnEdit.Focus();
                            }
                            else
                            {
                                btnAdd.Focus();
                            }
                        }

                    }
                    else
                    {
                        if (this.ActionType == GlobalVariables.ActionEdit && btnEdit.Enabled)
                        {
                            btnEdit.Focus();
                        }
                        else
                        {
                            btnAdd.Focus();
                        }
                    }

                }

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnTimKiem_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void cboExcuteRoomID_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboExcuteRoomID.EditValue != null)
                    {
                        if (this.ActionType == GlobalVariables.ActionEdit)
                        {
                            btnEdit.Focus();
                        }
                        else
                        {
                            btnAdd.Focus();
                        }
                        cboExcuteRoomID.ShowPopup();
                    }
                    else
                    {
                        cboExcuteRoomID.ShowPopup();
                    }
                }
                else
                {
                    cboExcuteRoomID.ShowPopup();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();

                Inventec.Desktop.Common.Modules.Module module = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisImportPtttTable").FirstOrDefault();
                if (module == null) LogSystem.Error("Khong tim thay modulelink=HIS.Desktop.Plugins.HisImportPtttTable");
                if (module.IsPlugin && module.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(PluginInstance.GetModuleWithWorkingRoom(module, this.moduleData.RoomId, this.moduleData.RoomTypeId));
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule(PluginInstance.GetModuleWithWorkingRoom(module, this.moduleData.RoomId, this.moduleData.RoomTypeId), listArgs);
                }

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnImport_Click(null, null);
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);

            }
        }
    }
}

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
using ACS.EFMODEL.DataModels;
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.ConfigSystem;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.Common.Controls.EditorLoader;
using LIS.EFMODEL.DataModels;
using LIS.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MOS.Filter;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.BackendData;
using ACS.Filter;
using Inventec.Common.Controls.PopupLoader;
using System.IO;
using HIS.Desktop.Plugins.LisDeliveryNoteCreateUpdate.ADO;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using Inventec.Desktop.Common.Controls.ValidationRule;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.Common;
using LIS.SDO;
using SDA.EFMODEL.DataModels;
using HIS.Desktop.ModuleExt;
using HIS.Desktop.Controls.Session;

namespace HIS.Desktop.Plugins.LisDeliveryNoteCreateUpdate.LisDeliveryNoteCreateUpdate
{
    public partial class UCLisDeliveryNoteCreateUpdate : HIS.Desktop.Utility.UserControlBase
    {
        #region Declare
        LIS_DELIVERY_NOTE lisDeliveryNote;
        int ActionType = -1;
        string roomCode = "";
        List<V_LIS_SAMPLE> lstLisSample = new List<V_LIS_SAMPLE>();
        List<ImportLisDeliveryNoteADO> lstGridInfomation = new List<ImportLisDeliveryNoteADO>();
        List<ACS_USER> acsUsers = new List<ACS_USER>();
        List<HIS_USER_ROOM> userRooms = new List<HIS_USER_ROOM>();
        List<HIS_EXECUTE_ROOM> executeRooms = new List<HIS_EXECUTE_ROOM>();
        RefeshReference refeshReference;
        List<LIS_SAMPLE> dataToPrint = new List<LIS_SAMPLE>();
        private Inventec.Desktop.Common.Modules.Module moduleData;
        Inventec.Common.ExcelImport.Import import;
        DevExpress.XtraGrid.Columns.GridColumn lastColumn = null;
        DevExpress.Utils.ToolTipControlInfo lastInfo = null;
        int lastRowHandle = -1;
        int positionHandle = -1;
        LIS_DELIVERY_NOTE currentLisDeliveryNote = new LIS_DELIVERY_NOTE();

        #endregion

        public UCLisDeliveryNoteCreateUpdate(Inventec.Desktop.Common.Modules.Module moduleData)
        {
            InitializeComponent();
        }

        public UCLisDeliveryNoteCreateUpdate(Inventec.Desktop.Common.Modules.Module _moduleData, LIS_DELIVERY_NOTE _lisDeliveryNote, RefeshReference _refeshReference)
            : base(_moduleData)
        {
            InitializeComponent();
            try
            {
                this.moduleData = _moduleData;
                this.lisDeliveryNote = _lisDeliveryNote;
                this.refeshReference = _refeshReference;
                currentLisDeliveryNote = _lisDeliveryNote;
                //Resources.ResourceLanguageManager.InitResourceLanguageManager();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void MeShow()
        {
            try
            {
                var room = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == moduleData.RoomId);
                Inventec.Common.Logging.LogSystem.Error(Inventec.Common.Logging.LogUtil.TraceData("room____", room));
                if (room != null)
                    this.roomCode = room.ROOM_CODE;

                SetDefaultValue();
                EnableControlChanged(this.ActionType);
                LoadData();
                FillDataToGridSampling();
                ValidateControl();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidateControl()
        {
            try
            {
                ValidationComboControl(cboReceptRoom, dxValidationProvider1);
                ValidControlDeliverName();
                ValidControlDescription();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboReceptRoom()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisExecuteRoomFilter hisExecuteRoomFilter = new HisExecuteRoomFilter();
                hisExecuteRoomFilter.IS_ACTIVE = 1;
                var lstExecuteRoom = new BackendAdapter(param).Get<List<HIS_EXECUTE_ROOM>>("api/HisExecuteRoom/Get", ApiConsumers.MosConsumer, hisExecuteRoomFilter, param);
                if (lstExecuteRoom != null && lstExecuteRoom.Count() > 0)
                {
                    executeRooms.AddRange(lstExecuteRoom);
                    List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                    columnInfos.Add(new ColumnInfo("EXECUTE_ROOM_CODE", "", 100, 1));
                    columnInfos.Add(new ColumnInfo("EXECUTE_ROOM_NAME", "", 250, 2));
                    ControlEditorADO controlEditorADO = new ControlEditorADO("EXECUTE_ROOM_NAME", "ID", columnInfos, false, 350);
                    ControlEditorLoader.Load(cboReceptRoom, executeRooms, controlEditorADO);
                    cboReceptRoom.Properties.ImmediatePopup = true;
                    if (executeRooms.Count() == 1)
                        cboReceptRoom.EditValue = executeRooms.FirstOrDefault().ID;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UCLisDeliveryNoteCreateUpdate_Load(object sender, EventArgs e)
        {
            try
            {
                MeShow();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboSampler()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("LOGINNAME", "", 100, 1));
                columnInfos.Add(new ColumnInfo("USERNAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("USERNAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboSampler, this.acsUsers, controlEditorADO);
                cboSampler.Properties.ImmediatePopup = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadData()
        {
            try
            {
                InitComboReceptRoom();
                GetDataUserRoom();
                GetDataSample();
                InitComboSampler();
                if (currentLisDeliveryNote != null && currentLisDeliveryNote.ID > 0)
                    SetDataFromLisDeliveryNote(currentLisDeliveryNote);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void SetDataFromLisDeliveryNote(LIS_DELIVERY_NOTE currentLisDeliveryNote)
        {
            try
            {
                lstGridInfomation = new List<ImportLisDeliveryNoteADO>();
                txtDelivery.Text = currentLisDeliveryNote.DELIVERY_NOTE_CODE;
                txtDeliverName.Text = currentLisDeliveryNote.DELIVER_NAME;
                txtNote.Text = currentLisDeliveryNote.NOTE;
                cboReceptRoom.EditValue = executeRooms.Where(o => o.EXECUTE_ROOM_CODE == currentLisDeliveryNote.RECEIVE_ROOM_CODE).FirstOrDefault().ID;
                CommonParam param = new CommonParam();
                LisSampleViewFilter lisSampleViewFilter = new LisSampleViewFilter();
                lisSampleViewFilter.SAMPLE_ROOM_CODE__EXACT = this.roomCode;
                lisSampleViewFilter.DELIVERY_NOTE_ID = currentLisDeliveryNote.ID;
                var result = new BackendAdapter(param).Get<List<V_LIS_SAMPLE>>("api/LisSample/GetView", ApiConsumers.LisConsumer, lisSampleViewFilter, param);
                if (result != null && result.Count() > 0)
                    foreach (var item in result)
                    {
                        ImportLisDeliveryNoteADO ado = new ImportLisDeliveryNoteADO(item);
                        lstGridInfomation.Add(ado);
                    }
                FillDataToGridInfomation();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetDataUserRoom()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisUserRoomFilter userRoomfilter = new HisUserRoomFilter();
                userRoomfilter.ROOM_ID = moduleData.RoomId;
                userRoomfilter.IS_ACTIVE = 1;
                var result = new BackendAdapter(param).Get<List<HIS_USER_ROOM>>("api/HisUserRoom/Get", ApiConsumers.MosConsumer, userRoomfilter, param);
                if (result != null && result.Count() > 0)
                {
                    this.userRooms.AddRange(result);
                    GetDataAcsUser();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetDataAcsUser()
        {
            try
            {
                var lstacsUser = BackendDataWorker.Get<ACS_USER>().ToList();
                if (lstacsUser != null && lstacsUser.Count() > 0)
                {
                    foreach (var item in userRooms)
                    {
                        var user = lstacsUser.Where(o => o.LOGINNAME == item.LOGINNAME && o.IS_ACTIVE == 1);
                        if (user != null && user.Count() > 0)
                            this.acsUsers.Add(user.FirstOrDefault());
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetDataSample()
        {
            try
            {
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                LisSampleViewFilter lisSampleViewFilter = new LisSampleViewFilter();
                lisSampleViewFilter.SAMPLE_ROOM_CODE__EXACT = this.roomCode;
                lisSampleViewFilter.HAS_DELIVERY_NOTE_ID = false;
                SetFilterNavBar(ref lisSampleViewFilter);
                var result = new BackendAdapter(param).Get<List<V_LIS_SAMPLE>>("api/LisSample/GetView", ApiConsumers.LisConsumer, lisSampleViewFilter, param);
                if (result != null && result.Count() > 0)
                {
                    this.lstLisSample = result;
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void SetDefaultValue()
        {
            try
            {
                this.ActionType = GlobalVariables.ActionAdd;

                txtKeyword.Text = "";
                cboSampler.EditValue = null;
                cboReceptRoom.EditValue = null;
                txtDelivery.Text = "";
                txtDeliverName.Text = "";
                txtNote.Text = "";
                lblSumSample.Text = "";

                btnPrint.Enabled = false;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void EnableControlChanged(int action)
        {
            try
            {
                // btnEdit.Enabled = (action == GlobalVariables.ActionEdit);
                //btnAdd.Enabled = (action == GlobalVariables.ActionAdd);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        // filldata
        private void FillDataToGridSampling()
        {
            try
            {
                this.gridControlSampling.DataSource = null;
                //long branchId = Inventec.Common.TypeConvert.Parse.ToInt64((this.cboSampler.EditValue ?? "0").ToString());

                string keyword = Inventec.Common.String.Convert.UnSignVNese(this.txtKeyword.Text.ToLower().Trim());
                //var query = this.lstLisSample.AsQueryable();
                var query = lstLisSample.Where(o => (!string.IsNullOrEmpty(o.BARCODE) && o.BARCODE.ToLower().Contains(keyword))
                        || (!string.IsNullOrEmpty(o.VIR_PATIENT_NAME) && Inventec.Common.String.Convert.UnSignVNese(o.VIR_PATIENT_NAME.ToLower()).Contains(keyword))
                        || (!string.IsNullOrEmpty(o.PHONE_NUMBER) && o.PHONE_NUMBER.ToLower().Contains(keyword))
                        || (!string.IsNullOrEmpty(o.SAMPLE_LOGINNAME) && o.SAMPLE_LOGINNAME.ToLower().Contains(keyword))
                        || (!string.IsNullOrEmpty(o.COMMUNE_NAME) && Inventec.Common.String.Convert.UnSignVNese(o.COMMUNE_NAME.ToLower()).Contains(keyword))
                        || (!string.IsNullOrEmpty(o.DISTRICT_NAME) && Inventec.Common.String.Convert.UnSignVNese(o.DISTRICT_NAME.ToLower()).Contains(keyword))
                        || (!string.IsNullOrEmpty(o.PROVINCE_NAME) && Inventec.Common.String.Convert.UnSignVNese(o.PROVINCE_NAME.ToLower()).Contains(keyword))
                        || (!string.IsNullOrEmpty(o.SPECIMEN_ORDER.ToString()) && o.SPECIMEN_ORDER.ToString().Contains(keyword))).ToList();
                //Inventec.Common.Logging.LogSystem.Error(Inventec.Common.Logging.LogUtil.TraceData("query____1", query));
                if (cboSampler.EditValue != null)
                    query = query.Where(o => o.SAMPLE_LOGINNAME == acsUsers.Where(p => p.ID == Convert.ToInt32(cboSampler.EditValue)).ToList().FirstOrDefault().LOGINNAME).ToList();
                //Inventec.Common.Logging.LogSystem.Error(Inventec.Common.Logging.LogUtil.TraceData("query____2", query));

                if (query != null && query.Count() > 0)
                {
                    gridViewSampling.FocusedRowHandle = 0;
                    gridViewSampling.FocusedColumn = gridColumn15;
                }

                gridControlSampling.DataSource = null;
                gridControlSampling.DataSource = query;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetFilterNavBar(ref LisSampleViewFilter filter)
        {
            try
            {
                filter.KEY_WORD = txtKeyword.Text.Trim();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //action grid

        private void gridViewSampling_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    V_LIS_SAMPLE pData = (V_LIS_SAMPLE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "DOB_STR")
                    {
                        if (pData.IS_HAS_NOT_DAY_DOB == 1)
                        {
                            e.Value = pData.DOB.ToString().Substring(0, 4);
                        }
                        else
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(pData.DOB ?? 0);
                        }

                    }
                    else if (e.Column.FieldName == "SAMPLE_TIME_STR")
                    {
                        if (pData.SAMPLE_TIME != null)
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.SAMPLE_TIME ?? 0);
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewSampling_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    long i = gridViewSampling.FocusedRowHandle;
                    if (e.RowHandle == i)
                        e.Appearance.BackColor = Color.LightGreen;

                    //V_LIS_SAMPLE data = (V_LIS_SAMPLE)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    //if (data != null)
                    //{
                    //    e.Appearance.BackColor = Color.LightGreen;
                    //}
                    //else
                    //{
                    //    e.Appearance.BackColor = Color.White;
                    //}
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewSampling_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var rowData = (V_LIS_SAMPLE)gridViewSampling.GetFocusedRow();
                    AddAndRemove(null, rowData);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void AddAndRemove(ImportLisDeliveryNoteADO info, V_LIS_SAMPLE sample)
        {
            try
            {
                if (info == null)
                {
                    ImportLisDeliveryNoteADO dataADO = new ImportLisDeliveryNoteADO(sample);
                    this.lstGridInfomation.Add(dataADO);
                }
                else
                    this.lstGridInfomation.Add(info);

                this.lstLisSample.Remove(sample);
                FillDataToGridSampling();
                FillDataToGridInfomation();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonAdd_Click(object sender, EventArgs e)
        {
            try
            {
                var rowData = (V_LIS_SAMPLE)gridViewSampling.GetFocusedRow();
                AddAndRemove(null, rowData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToGridInfomation()
        {
            try
            {
                lblSumSample.Text = this.lstGridInfomation.Count().ToString();
                gridControlInfomation.DataSource = null;
                gridControlInfomation.DataSource = this.lstGridInfomation;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //action

        private void txtKeyword_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    FillDataToGridSampling();
                }
                else if (e.KeyCode == Keys.Down)
                {
                    this.gridViewSampling.FocusedRowHandle = 0;
                    this.gridViewSampling.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtKeyword_TextChanged(object sender, EventArgs e)
        {
            try
            {
                FillDataToGridSampling();
                var dataGrid = this.gridControlSampling.DataSource as List<V_LIS_SAMPLE>;
                if (dataGrid.Count() == 1)
                {
                    this.gridViewSampling.FocusedRowHandle = 0;
                    this.gridViewSampling.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboSampler_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty((this.cboSampler.EditValue ?? "").ToString()))
                    {
                        this.FillDataToGridSampling();
                        this.txtKeyword.Focus();
                        this.txtKeyword.SelectAll();
                    }
                }
                else
                {
                    this.cboSampler.ShowPopup();
                    PopupLoader.SelectFirstRowPopup(this.cboSampler);
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void gridViewInfomation_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    ImportLisDeliveryNoteADO pData = (ImportLisDeliveryNoteADO)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "DOB_STR")
                    {
                        if (pData.IS_HAS_NOT_DAY_DOB == 1)
                        {
                            e.Value = pData.DOB.ToString().Substring(0, 4);
                        }
                        else
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(pData.DOB ?? 0);
                        }
                    }
                    else if (e.Column.FieldName == "SAMPLE_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.SAMPLE_TIME ?? 0);
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewInfomation_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    ImportLisDeliveryNoteADO data = (ImportLisDeliveryNoteADO)((IList)((BaseView)sender).DataSource)[e.RowHandle];

                    if (data.ID > 0)
                    {
                        e.Appearance.ForeColor = Color.Black;
                    }
                    else
                    {
                        e.Appearance.ForeColor = Color.Green;
                    }
                    if (!string.IsNullOrEmpty(data.MESSAGE_WARN))
                    {
                        e.Appearance.ForeColor = Color.Goldenrod;
                    }
                    else if (!string.IsNullOrEmpty(data.MESSAGE_ERR))
                    {
                        e.Appearance.ForeColor = Color.Maroon;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonDelete_Click(object sender, EventArgs e)
        {
            try
            {
                var rowData = (ImportLisDeliveryNoteADO)gridViewInfomation.GetFocusedRow();
                if (rowData.ID > 0)
                    RemoveRowGridInfomationAndAddGridSample(rowData);
                else

                    RemoveRowGridInfomation(rowData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void RemoveRowGridInfomationAndAddGridSample(ImportLisDeliveryNoteADO rowData)
        {
            try
            {
                this.lstLisSample.Add(rowData);
                this.lstGridInfomation.Remove(rowData);
                FillDataToGridSampling();
                FillDataToGridInfomation();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void RemoveRowGridInfomation(ImportLisDeliveryNoteADO rowData)
        {
            try
            {
                this.lstGridInfomation.Remove(rowData);
                FillDataToGridInfomation();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboSampler_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                cboSampler.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboReceptRoom_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                cboReceptRoom.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                //SetDefaulGrid();
                SetDefaultValue();
                ClearDataGrid();
                this.currentLisDeliveryNote = new LIS_DELIVERY_NOTE();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ClearDataGrid()
        {
            try
            {
                this.lstGridInfomation = new List<ImportLisDeliveryNoteADO>();
                this.lstLisSample = new List<V_LIS_SAMPLE>();
                GetDataSample();
                FillDataToGridSampling();
                FillDataToGridInfomation();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaulGrid()
        {
            try
            {
                if (currentLisDeliveryNote != null && currentLisDeliveryNote.ID > 0)
                {
                    GetDeliveryNote(currentLisDeliveryNote.ID);
                    SetDataFromLisDeliveryNote(currentLisDeliveryNote);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetDeliveryNote(long id)
        {
            try
            {
                CommonParam param = new CommonParam();
                LisDeliveryNoteFilter filter = new LisDeliveryNoteFilter();
                filter.ID = id;
                this.currentLisDeliveryNote = new BackendAdapter(param).Get<List<LIS_DELIVERY_NOTE>>("api/LisDeliveryNote/Get", ApiConsumers.LisConsumer, filter, param).FirstOrDefault();
                //Inventec.Common.Logging.LogSystem.Error(Inventec.Common.Logging.LogUtil.TraceData("currentLisDeliveryNote____", currentLisDeliveryNote));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnDowloadTemplate_Click(object sender, EventArgs e)
        {
            try
            {
                var source = System.IO.Path.Combine(Application.StartupPath + "/Tmp/Imp", "IMPORT_DELIVERY_NOTE_CREATE_UPDATE.xlsx");

                if (File.Exists(source))
                {
                    SaveFileDialog saveFileDialog1 = new SaveFileDialog();

                    saveFileDialog1.Title = "Save File";
                    saveFileDialog1.FileName = "IMPORT_DELIVERY_NOTE_CREATE_UPDATE";
                    saveFileDialog1.DefaultExt = "xlsx";
                    saveFileDialog1.Filter = "Excel files (*.xlsx)|All files (*.*)";
                    saveFileDialog1.FilterIndex = 2;
                    saveFileDialog1.RestoreDirectory = true;

                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        File.Copy(source, saveFileDialog1.FileName, true);
                        DevExpress.XtraEditors.XtraMessageBox.Show("Tải file thành công");
                    }
                }
                else
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Không tìm thấy file import");
                }
            }
            catch (Exception ex)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show("Tải file thất bại");
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                //string messageError = "";
                //if (!CheckAllowAdd(ref messageError))
                //{
                //    MessageManager.Show(messageError);
                //    return;
                //}

                CommonParam param = new CommonParam();
                bool success = false;
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Multiselect = false;
                if (ofd.ShowDialog() != DialogResult.OK || !(ofd.FileName.EndsWith(".xlsx")))
                    return;
                WaitingManager.Show();
                import = new Inventec.Common.ExcelImport.Import();
                if (import.ReadFileExcel(ofd.FileName))
                {
                    var dataImport = import.Get<ImportLisDeliveryNoteADO>(0);
                    if (dataImport == null || dataImport.Count == 0)
                        param.Messages.Add("Dữ liệu Đọc từ file Excel rỗng!");
                    else
                    {
                        List<ImportLisDeliveryNoteADO> lstImport = new List<ImportLisDeliveryNoteADO>();
                        foreach (var item in dataImport)
                        {
                            if (string.IsNullOrEmpty(item.VIR_PATIENT_NAME_TMP) &&
                                string.IsNullOrEmpty(item.GENDER_CODE_TMP) &&
                                string.IsNullOrEmpty(item.COMMUNE_NAME_TMP) &&
                                string.IsNullOrEmpty(item.DISTRICT_NAME_TMP) &&
                                string.IsNullOrEmpty(item.PROVINCE_NAME_TMP) &&
                                string.IsNullOrEmpty(item.DAY_SAMPLE_TIME_TMP) &&
                                string.IsNullOrEmpty(item.HOURS_SAMPLE_TIME_TMP) &&
                                string.IsNullOrEmpty(item.SAMPLE_TYPE_CODE_TMP) &&
                                string.IsNullOrEmpty(item.DOB_TMP.ToString()))
                                continue;
                            lstImport.Add(item);
                        }
                        success = true;
                        ProcessListImportADO(ref success, ref param, lstImport);
                        // SetEnableControlCommon();
                    }
                }
                WaitingManager.Hide();
                MessageManager.Show(param, success);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessListImportADO(ref bool success, ref CommonParam param, List<ImportLisDeliveryNoteADO> listImport)
        {
            try
            {
                bool isCheck = false;
                var hisGender = BackendDataWorker.Get<HIS_GENDER>().ToList();
                var lisSampleType = BackendDataWorker.Get<LIS_SAMPLE_TYPE>().ToList();
                var sdaProvince = BackendDataWorker.Get<SDA_PROVINCE>().ToList();
                foreach (var item in listImport)
                {
                    SDA_PROVINCE province = new SDA_PROVINCE();
                    SDA_DISTRICT district = new SDA_DISTRICT();

                    ImportLisDeliveryNoteADO info = new ImportLisDeliveryNoteADO();
                    string DaySample = "00000000";
                    string HoursSample = "0000";
                    string lackMsg = "";
                    if (!string.IsNullOrEmpty(item.BARCODE_TMP))
                        info.BARCODE = item.BARCODE_TMP;
                    if (!string.IsNullOrEmpty(item.VIR_PATIENT_NAME_TMP))
                    {
                        string firstName = "";
                        string lastName = "";
                        var name = item.VIR_PATIENT_NAME_TMP.Split(' ');

                        if (name != null && name.Count() == 1)
                        {
                            firstName = name[0];
                        }
                        else if (name != null && name.Count() > 1)
                        {
                            firstName = name[name.Count() - 1];
                            name[name.Count() - 1] = "";
                            List<string> lst = name.ToList();
                            lastName = string.Join(" ", lst).Trim();
                        }
                        else
                            lackMsg += " Họ tên,";

                        info.FIRST_NAME = firstName;
                        info.LAST_NAME = lastName;
                        info.VIR_PATIENT_NAME = lastName + " " + firstName;
                    }
                    else
                        lackMsg += " Họ tên,";

                    if (!string.IsNullOrEmpty(item.DOB_TMP.ToString()))
                    {
                        if (item.DOB_TMP.ToString().Length == 4)
                        {
                            info.DOB = Convert.ToInt64(item.DOB_TMP.ToString() + "0101000000");
                            info.IS_HAS_NOT_DAY_DOB = 1;
                        }
                        else if (item.DOB_TMP.ToString().Length == 8)
                        {
                            info.DOB = Convert.ToInt64(item.DOB_TMP.ToString() + "0000000");
                        }
                        else if (item.DOB_TMP.ToString().Length == 14)
                            info.DOB = item.DOB_TMP;
                        else
                            lackMsg += " Ngày sinh,";
                    }
                    else
                        lackMsg += " Ngày sinh,";
                    if (!string.IsNullOrEmpty(item.GENDER_CODE_TMP))
                    {
                        try
                        {
                            var gender = hisGender.Where(o => o.GENDER_CODE == item.GENDER_CODE_TMP);
                            Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("gender__________", gender));

                            if (gender != null && gender.Count() > 0)
                            {
                                info.GENDER_CODE = gender.FirstOrDefault().GENDER_CODE;
                                info.GENDER_NAME = gender.FirstOrDefault().GENDER_NAME;
                            }
                            else
                                lackMsg += " Giới tính,";
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else
                        lackMsg += " Giới tính,";

                    if (!string.IsNullOrEmpty(item.PHONE_NUMBER_TMP))
                        info.PHONE_NUMBER = item.PHONE_NUMBER_TMP;
                    else
                        lackMsg += " Số điện thoại,";
                    if (!string.IsNullOrEmpty(item.SAMPLE_TYPE_CODE_TMP))
                    {
                        try
                        {
                            var sample = lisSampleType.Where(o => o.SAMPLE_TYPE_CODE.ToString().ToLower() == item.SAMPLE_TYPE_CODE_TMP.ToString().ToLower());
                            Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("sample__________", sample));

                            if (sample != null && sample.Count() > 0)
                            {
                                info.SAMPLE_TYPE_ID = sample.SingleOrDefault().ID;
                                info.SAMPLE_TYPE_CODE = sample.SingleOrDefault().SAMPLE_TYPE_CODE;
                                info.SAMPLE_TYPE_NAME = sample.SingleOrDefault().SAMPLE_TYPE_NAME;
                            }
                            else
                                lackMsg += " Mã loại bệnh phẩm,";
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else
                        lackMsg += " Mã loại bệnh phẩm,";
                    if (!string.IsNullOrEmpty(item.SAMPLE_LOGINNAME_TMP))
                        info.SAMPLE_LOGINNAME = item.SAMPLE_LOGINNAME_TMP;

                    if (!string.IsNullOrEmpty(item.PROVINCE_NAME_TMP))
                    {
                        var provinceCheck = sdaProvince.Where(o => o.PROVINCE_NAME.ToLower().Contains(item.PROVINCE_NAME_TMP.ToLower()));
                        if (provinceCheck != null && provinceCheck.Count() > 0)
                        {
                            province = provinceCheck.FirstOrDefault();
                            info.PROVINCE_CODE = province.PROVINCE_CODE;
                            info.PROVINCE_NAME = province.PROVINCE_NAME;
                        }
                        else
                            lackMsg += " Tỉnh/thành phố,";
                    }
                    else
                        lackMsg += " Tỉnh/thành phố,";

                    if (!string.IsNullOrEmpty(item.DISTRICT_NAME_TMP) && province != null && province.ID > 0)
                    {
                        var districtCheck = BackendDataWorker.Get<SDA_DISTRICT>().ToList().Where(o => o.PROVINCE_ID == province.ID && o.DISTRICT_NAME.ToLower().Contains(item.DISTRICT_NAME_TMP.ToLower()));
                        if (districtCheck != null && districtCheck.Count() > 0)
                        {
                            district = districtCheck.FirstOrDefault();
                            info.DISTRICT_CODE = district.DISTRICT_CODE;
                            info.DISTRICT_NAME = district.DISTRICT_NAME;
                        }
                        else
                            lackMsg += " Quận/huyện,";
                    }
                    else
                        lackMsg += " Quận/huyện,";

                    if (!string.IsNullOrEmpty(item.COMMUNE_NAME_TMP) && district != null && district.ID > 0)
                    {
                        var communeCheck = BackendDataWorker.Get<SDA_COMMUNE>().ToList().Where(o => o.DISTRICT_ID == district.ID && o.COMMUNE_NAME.ToLower().Contains(item.COMMUNE_NAME_TMP.ToLower()));
                        if (communeCheck != null && communeCheck.Count() > 0)
                        {
                            info.COMMUNE_CODE = communeCheck.FirstOrDefault().COMMUNE_CODE;
                            info.COMMUNE_NAME = communeCheck.FirstOrDefault().COMMUNE_NAME;
                        }
                        else
                            lackMsg += " Xã/phường,";
                    }
                    else
                        lackMsg += " Xã/phường,";

                    if (!string.IsNullOrEmpty(item.SPECIMEN_ORDER_TMP.ToString()))
                        info.SPECIMEN_ORDER = item.SPECIMEN_ORDER_TMP;
                    if (!string.IsNullOrEmpty(item.CMND_NUMBER_TMP.ToString()))
                        info.CMND_NUMBER = item.CMND_NUMBER_TMP;
                    if (!string.IsNullOrEmpty(item.HEIN_CARD_NUMBER_TMP.ToString()))
                        info.HEIN_CARD_NUMBER = item.HEIN_CARD_NUMBER_TMP;
                    if (!string.IsNullOrEmpty(item.NATIONAL_NAME_TMP.ToString()))
                        info.NATIONAL_NAME = item.NATIONAL_NAME_TMP;
                    if (!string.IsNullOrEmpty(item.PASSPORT_NUMBER_TMP.ToString()))
                        info.PASSPORT_NUMBER = item.PASSPORT_NUMBER_TMP;
                    if (!string.IsNullOrEmpty(item.VIR_ADDRESS_TMP.ToString()))
                        info.VIR_ADDRESS = item.VIR_ADDRESS_TMP;
                    if (!string.IsNullOrEmpty(item.SICK_TIME_TMP.ToString()))
                        info.SICK_TIME = item.SICK_TIME_TMP;
                    if (!string.IsNullOrEmpty(item.DAY_SAMPLE_TIME_TMP) && item.DAY_SAMPLE_TIME_TMP.ToString().Length == 8)
                        DaySample = item.DAY_SAMPLE_TIME_TMP;
                    else
                        lackMsg += " Ngày lấy mẫu,";
                    if (!string.IsNullOrEmpty(item.HOURS_SAMPLE_TIME_TMP) && item.HOURS_SAMPLE_TIME_TMP.ToString().Length == 6)
                        HoursSample = item.HOURS_SAMPLE_TIME_TMP;
                    else
                        lackMsg += " Giờ lấy mẫu,";
                    info.SAMPLE_TIME = Convert.ToInt64(DaySample + HoursSample);

                    var sampleCheck = this.lstLisSample.Where(o => o.BARCODE == info.BARCODE);
                    // nếu trùng barcode thì add sang grid bên phải
                    if (sampleCheck != null && sampleCheck.Count() > 0)
                    {
                        string msg = "";
                        if (info.VIR_PATIENT_NAME.Trim() != item.VIR_PATIENT_NAME_TMP.Trim())
                            msg += " Họ tên,";
                        else if (info.DOB != item.DOB_TMP)
                            msg += " Ngày sinh,";
                        else if (info.GENDER_CODE != item.GENDER_CODE_TMP)
                            msg += " Giới tính,";
                        info = new ImportLisDeliveryNoteADO(sampleCheck.FirstOrDefault());
                        if (!string.IsNullOrEmpty(msg))
                            msg = "Dữ liệu" + msg + " trong file excel khác với dữ liệu đã có trên hệ thống.";
                        info.MESSAGE_WARN = msg;
                        AddAndRemove(info, sampleCheck.FirstOrDefault());
                    }
                    // Nếu không tồn tại dữ liệu tương ứng ở grid bên trái thì tự động xử lý bổ sung dòng đó vào grid bên phải
                    else
                    {
                        if (!string.IsNullOrEmpty(lackMsg))
                            info.MESSAGE_ERR = "Mẫu chưa được nhập thông tin" + lackMsg;
                        AddToGridSample(info);
                    }

                    Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("info_________", info));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void AddToGridSample(ImportLisDeliveryNoteADO info)
        {
            try
            {
                this.lstGridInfomation.Add(info);
                FillDataToGridInfomation();
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
                if (e.Info == null && e.SelectedControl == gridControlInfomation)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = gridControlInfomation.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                    GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
                    if (info.InRowCell)
                    {
                        if (lastRowHandle != info.RowHandle || lastColumn != info.Column)
                        {
                            lastColumn = info.Column;
                            lastRowHandle = info.RowHandle;
                            string msg = "";
                            long? isConfirm = null;
                            if (!String.IsNullOrWhiteSpace((view.GetRowCellValue(lastRowHandle, "MESSAGE_WARN") ?? "").ToString()))
                            {
                                msg = (view.GetRowCellValue(lastRowHandle, "MESSAGE_WARN") ?? "").ToString();
                            }
                            else if (!String.IsNullOrWhiteSpace((view.GetRowCellValue(lastRowHandle, "MESSAGE_ERR") ?? "").ToString()))
                            {
                                msg = (view.GetRowCellValue(lastRowHandle, "MESSAGE_ERR") ?? "").ToString();
                            }
                            lastInfo = new ToolTipControlInfo(new DevExpress.XtraGrid.GridToolTipInfo(view, new DevExpress.XtraGrid.Views.Base.CellToolTipInfo(info.RowHandle, info.Column, "Text")), msg);
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

        // valid

        protected void ValidationComboControl(BaseEdit control, DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider dxValidationProviderEditor)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = Inventec.Desktop.Common.LibraryMessage.MessageUtil.GetMessage(Inventec.Desktop.Common.LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProviderEditor.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidControlDescription()
        {
            try
            {
                Inventec.Desktop.Common.Controls.ValidationRule.ControlMaxLengthValidationRule validate = new Inventec.Desktop.Common.Controls.ValidationRule.ControlMaxLengthValidationRule();
                validate.editor = this.txtNote;
                validate.maxLength = 1000;
                validate.IsRequired = false;
                validate.ErrorText = string.Format("Nhập quá ký tự cho phép {0}", 1000);
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(this.txtNote, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlDeliverName()
        {
            try
            {
                Inventec.Desktop.Common.Controls.ValidationRule.ControlMaxLengthValidationRule validate = new Inventec.Desktop.Common.Controls.ValidationRule.ControlMaxLengthValidationRule();
                validate.editor = this.txtDeliverName;
                validate.maxLength = 200;
                validate.IsRequired = false;
                validate.ErrorText = string.Format("Nhập quá ký tự cho phép {0}", 200);
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(this.txtDeliverName, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewSampling_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {

        }

        private void cboSampler_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                FillDataToGridSampling();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                positionHandle = -1;
                if (!dxValidationProvider1.Validate())
                    return;

                bool success = false;
                string msg = "";
                var checkDataWarn = lstGridInfomation.Where(o => !string.IsNullOrEmpty(o.MESSAGE_WARN)).ToList();
                var checkDataErr = lstGridInfomation.Where(o => !string.IsNullOrEmpty(o.MESSAGE_ERR)).ToList();
                var checkDataGreen = lstGridInfomation.Where(o => o.ID <= 0 && string.IsNullOrEmpty(o.MESSAGE_ERR) && string.IsNullOrEmpty(o.MESSAGE_WARN)).ToList();
                var checkDataDark = lstGridInfomation.Where(o => o.ID > 0 && string.IsNullOrEmpty(o.MESSAGE_WARN) && string.IsNullOrEmpty(o.MESSAGE_ERR)).ToList();

                if (checkDataErr != null && checkDataErr.Count() > 0)
                {
                    MessageBox.Show("Các mẫu bệnh phẩm màu đỏ nhập thiếu thông tin bắt buộc", "Thông báo");
                    return;
                }

                if (checkDataWarn != null && checkDataWarn.Count() > 0)
                    msg += "Các mẫu bệnh phẩm màu vàng có dữ liệu trên hệ thống khác với dữ liệu trong file excel.";
                if (checkDataGreen != null && checkDataGreen.Count() > 0)
                    msg += "Các mẫu bệnh phẩm màu xanh chưa có dữ liệu trên hệ thống, phần mềm sẽ tự động tạo thông tin mẫu bệnh phẩm và tạo phiếu tương ứng.";

                string api = "";
                if (this.currentLisDeliveryNote != null && this.currentLisDeliveryNote.ID > 0)
                    api = "api/LisDeliveryNote/UpdateSDO";
                else
                    api = "api/LisDeliveryNote/CreateSDO";

                WaitingManager.Show();
                LisDeliveryNoteSDO sdo = new LisDeliveryNoteSDO();
                sdo.DeliveryNoteInfo = new LisDeliveryNoteInfoSDO();
                sdo.Samples = new List<LIS_SAMPLE>();
                List<long> lstsampleIds = new List<long>();
                if (checkDataWarn != null && checkDataWarn.Count() > 0)
                    lstsampleIds.AddRange(checkDataWarn.Select(o => o.ID));
                if (checkDataGreen != null && checkDataGreen.Count() > 0)
                {
                    foreach (var item in checkDataGreen)
                    {
                        LisSampleADO sample = new LisSampleADO(item);
                        sdo.Samples.Add(sample);
                    }
                }
                if (checkDataDark != null && checkDataDark.Count() > 0)
                    lstsampleIds.AddRange(checkDataDark.Select(o => o.ID));

                if (this.currentLisDeliveryNote != null && this.currentLisDeliveryNote.ID > 0)
                    sdo.DeliveryNoteInfo.DeliveryNoteId = this.currentLisDeliveryNote.ID;
                sdo.DeliveryNoteInfo.DeliverName = txtDeliverName.Text;
                sdo.DeliveryNoteInfo.Note = txtNote.Text;
                sdo.DeliveryNoteInfo.ReceiveRoomCode = executeRooms.Where(o => o.ID == Convert.ToInt32(cboReceptRoom.EditValue)).FirstOrDefault().EXECUTE_ROOM_CODE;
                sdo.DeliveryNoteInfo.ReceiveRoomName = executeRooms.Where(o => o.ID == Convert.ToInt32(cboReceptRoom.EditValue)).FirstOrDefault().EXECUTE_ROOM_NAME;
                sdo.DeliveryNoteInfo.WorkingRoomId = moduleData.RoomId;
                sdo.SampleIds = lstsampleIds;

                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("input__________", sdo));
                var result = new BackendAdapter(param).Post<LisDeliveryNoteSDO>(api, ApiConsumers.LisConsumer, sdo, param);

                if (result != null)
                {
                    this.currentLisDeliveryNote = new LIS_DELIVERY_NOTE();
                    this.currentLisDeliveryNote.ID = result.DeliveryNoteInfo.DeliveryNoteId ?? 0;
                    success = true;
                    btnPrint.Enabled = true;
                    if (result.Samples != null && result.Samples.Count() > 0)
                    {
                        dataToPrint = new List<LIS_SAMPLE>();
                        dataToPrint = result.Samples;
                    }
                    if (refeshReference != null)
                        this.refeshReference();

                    SetDefaulGrid();
                }

                WaitingManager.Hide();

                #region Hien thi message thong bao
                MessageManager.Show(this.ParentForm, param, success);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                try
                {
                    Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                    store.RunPrintTemplate("Mps000460", DelegateRunPrinter);
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool DelegateRunPrinter(string printCode, string fileName)
        {
            bool result = false;
            try
            {
                List<V_LIS_SAMPLE> dataPrint = new List<V_LIS_SAMPLE>();
                if (dataToPrint != null && dataToPrint.Count() > 0)
                    foreach (var item in dataToPrint)
                    {
                        V_LIS_SAMPLE sample = new V_LIS_SAMPLE();
                        Inventec.Common.Mapper.DataObjectMapper.Map<V_LIS_SAMPLE>(sample, item);
                        dataPrint.Add(sample);
                    }

                MPS.Processor.Mps000460.PDO.Mps000460PDO mps000460RDO = new MPS.Processor.Mps000460.PDO.Mps000460PDO(
                dataPrint,
                this.currentLisDeliveryNote
                );

                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("mps000460RDO__________", mps000460RDO));
                MPS.ProcessorBase.Core.PrintData PrintData = null;
                if (GlobalVariables.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printCode, fileName, mps000460RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                }
                else
                {
                    PrintData = new MPS.ProcessorBase.Core.PrintData(printCode, fileName, mps000460RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, "");
                }
                result = MPS.MpsPrinter.Run(PrintData);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void bbtnImport_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnImport_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnPrint_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnNew_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnFind_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                txtKeyword.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonAdd_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    var rowData = (V_LIS_SAMPLE)gridViewSampling.GetFocusedRow();
                    AddAndRemove(null, rowData);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonError_Click(object sender, EventArgs e)
        {
            try
            {
                var row = (ADO.ImportLisDeliveryNoteADO)gridViewInfomation.GetFocusedRow();
                if (row != null)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(row.MESSAGE_ERR);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewInfomation_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            if (e.RowHandle >= 0)
            {
                string ERROR = (gridViewInfomation.GetRowCellValue(e.RowHandle, "MESSAGE_ERR") ?? "").ToString().Trim();
                if (e.Column.FieldName == "ErrorLine")
                {
                    if (!string.IsNullOrEmpty(ERROR))
                    {
                        e.RepositoryItem = repositoryItemButtonError;
                    }
                    else
                    {
                        e.RepositoryItem = null;
                    }
                }
            }
        }

        private void repositoryItemButtonDeleteSample_Click(object sender, EventArgs e)
        {
            try
            {
                var rowData = (V_LIS_SAMPLE)gridViewSampling.GetFocusedRow();
                if (rowData.ID > 0)
                {
                    bool success = false;
                    CommonParam param = new CommonParam();
                    success = new BackendAdapter(param).Post<bool>("api/LisSample/Delete", ApiConsumers.LisConsumer, rowData.ID, param);
                    if (success)
                    {
                        GetDataSample();
                        FillDataToGridSampling();
                    }
                    MessageManager.Show(this.ParentForm, param, success);
                }


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

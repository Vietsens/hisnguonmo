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
using AutoMapper;
using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.Plugins.TreatmentBedRoomList.Validation;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
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

namespace HIS.Desktop.Plugins.TreatmentBedRoomList
{
    public partial class frmTreatmentBedRoomList : FormBase
    {
        private string treatmentCode;
        private int rowCount = 0;
        private int dataTotal = 0;
        private int start = 0;
        private int limit = 0;

        private int positionHandleControl = -1;

        private V_HIS_TREATMENT_BED_ROOM_1 editData = null;

        V_HIS_ROOM currentRoom = null;

        public frmTreatmentBedRoomList(Inventec.Desktop.Common.Modules.Module moduleData, string code)
            : base(moduleData)
        {
            InitializeComponent();
            this.treatmentCode = code;
        }

        private void frmTreatmentBedRoomList_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                this.InitComboDepartment();
                this.InitComboBedRoom(null);
                this.SetDefaulValue();
                this.GetRoom();
                this.ValidControl();
                this.ProcessSetEditForm();
                txtTreatmentCode.Text = this.treatmentCode ?? "";
                //this.treatmentCode = null;
                btnCancel.Enabled = false;
                btnSave.Enabled = false;
                this.FillDataToGrid();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetRoom()
        {
            try
            {
                this.currentRoom = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.currentModuleBase.RoomId);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboDepartment()
        {
            try
            {
                var data = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.BRANCH_ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetBranchId()).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("DEPARTMENT_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("DEPARTMENT_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("DEPARTMENT_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboDepartment, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboBedRoom(List<V_HIS_BED_ROOM> listBedRoom)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("BED_ROOM_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("BED_ROOM_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("BED_ROOM_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboBedRoom, listBedRoom, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidControl()
        {
            try
            {
                TimeValidationRule rule = new TimeValidationRule();
                rule.dtAddTime = dtAddTime;
                rule.dtRemoveTime = dtRemoveTime;
                dxValidationProvider1.SetValidationRule(dtAddTime, rule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void RemoveErrorValidControl()
        {
            try
            {
                dxValidationProvider1.RemoveControlError(dtAddTime);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGrid()
        {
            try
            {
                int numPageSize = 0;
                if (ucPaging1.pagingGrid != null)
                {
                    numPageSize = ucPaging1.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = (int)ConfigApplications.NumPageSize;
                }
                LoadPaging(new CommonParam(0, numPageSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(LoadPaging, param, numPageSize, this.gridControlTreatmentBedRoom);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadPaging(object param)
        {
            try
            {
                this.editData = null;
                this.ProcessSetEditForm();
                List<V_HIS_TREATMENT_BED_ROOM_1> listData = new List<V_HIS_TREATMENT_BED_ROOM_1>();
                this.start = ((CommonParam)param).Start ?? 0;
                this.limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(start, limit);
                Inventec.Core.ApiResultObject<List<V_HIS_TREATMENT_BED_ROOM_1>> apiResult = null;
                HisTreatmentBedRoomView1Filter filter = new HisTreatmentBedRoomView1Filter();
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "ADD_TIME";

                if (!string.IsNullOrEmpty(this.treatmentCode))
                {
                    if (this.treatmentCode.Length < 12 && checkDigit(this.treatmentCode))
                    {
                        this.treatmentCode = string.Format("{0:000000000000}", Convert.ToInt64(this.treatmentCode));
                        txtTreatmentCode.Text = this.treatmentCode;
                    }
                    filter.TREATMENT_CODE__EXACT = this.treatmentCode;
                }
                else
                {
                    if (!String.IsNullOrEmpty(txtTreatmentCode.Text))
                    {
                        string code = txtTreatmentCode.Text.Trim();
                        if (code.Length < 12 && checkDigit(code))
                        {
                            code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                            txtTreatmentCode.Text = code;
                        }
                        filter.TREATMENT_CODE__EXACT = code;
                    }
                }
                if (!String.IsNullOrEmpty(txtPatientCode.Text))
                {
                    string code = txtPatientCode.Text.Trim();
                    if (code.Length < 10 && checkDigit(code))
                    {
                        code = string.Format("{0:0000000000}", Convert.ToInt64(code));
                        txtPatientCode.Text = code;
                    }
                    filter.TDL_PATIENT_CODE__EXACT = code;
                }
                else
                {
                    filter.KEY_WORD = txtKeyword.Text.Trim();
                    if (dtFilterAddTimeFrom != null && dtFilterAddTimeFrom.DateTime != DateTime.MinValue)
                        filter.ADD_TIME_FROM = Convert.ToInt64(dtFilterAddTimeFrom.DateTime.ToString("yyyyMMdd") + "000000");
                    if (dtFilterAddTimeTo != null && dtFilterAddTimeTo.DateTime != DateTime.MinValue)
                        filter.ADD_TIME_TO = Convert.ToInt64(dtFilterAddTimeTo.DateTime.ToString("yyyyMMdd") + "235959");
                    if (cboBedRoom.EditValue != null)
                    {
                        filter.BED_ROOM_ID = Convert.ToInt64(cboBedRoom.EditValue);
                    }
                    if (cboDepartment.EditValue != null)
                    {
                        filter.DEPARTMENT_ID = Convert.ToInt64(cboDepartment.EditValue);
                    }
                }
                gridViewTreatmentBedRoom.BeginUpdate();
                apiResult = new BackendAdapter(paramCommon).GetRO<List<V_HIS_TREATMENT_BED_ROOM_1>>("api/HisTreatmentBedRoom/GetView1", ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    listData = (List<V_HIS_TREATMENT_BED_ROOM_1>)apiResult.Data;
                    if (listData != null)
                    {
                        rowCount = (listData == null ? 0 : listData.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                gridViewTreatmentBedRoom.GridControl.DataSource = listData;
                gridViewTreatmentBedRoom.EndUpdate();

                #region Process has exception
                SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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

        private void SetDefaulValue()
        {
            try
            {
                txtKeyword.Text = "";
                txtPatientCode.Text = "";
                txtTreatmentCode.Text = "";
                cboDepartment.EditValue = null;
                cboBedRoom.EditValue = null;
                if (!string.IsNullOrEmpty(this.treatmentCode))
                {
                    dtFilterAddTimeFrom.Reset();
                    dtFilterAddTimeTo.Reset();
                }
                else
                {
                    dtFilterAddTimeFrom.DateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    dtFilterAddTimeTo.DateTime = DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtKeyword_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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

        private void txtTreatmentCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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

        private void txtPatientCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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

        private void cboDepartment_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                cboBedRoom.EditValue = null;
                List<V_HIS_BED_ROOM> listBedRoom = new List<V_HIS_BED_ROOM>();
                if (cboDepartment.EditValue != null)
                {
                    cboDepartment.Properties.Buttons[1].Visible = true;
                    listBedRoom = BackendDataWorker.Get<V_HIS_BED_ROOM>().Where(o => o.DEPARTMENT_ID == Convert.ToInt64(cboDepartment.EditValue)).ToList();
                }
                else
                {
                    cboDepartment.Properties.Buttons[1].Visible = false;
                }
                this.InitComboBedRoom(listBedRoom);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDepartment_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboDepartment.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboBedRoom_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (cboBedRoom.EditValue != null)
                {
                    cboBedRoom.Properties.Buttons[1].Visible = true;
                }
                else
                {
                    cboBedRoom.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboBedRoom_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboBedRoom.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDepartment_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                cboBedRoom.Focus();
                cboBedRoom.ShowPopup();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                FillDataToGrid();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                this.SetDefaulValue();
                FillDataToGrid();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewTreatmentBedRoom_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    V_HIS_TREATMENT_BED_ROOM_1 pData = (V_HIS_TREATMENT_BED_ROOM_1)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1 + start;
                    }
                    else if (e.Column.FieldName == "ADD_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.ADD_TIME);
                    }
                    else if (e.Column.FieldName == "REMOVE_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(pData.REMOVE_TIME ?? 0);
                    }
                    else if (e.Column.FieldName == "DOB_STR")
                    {
                        if (pData.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == 1)
                        {
                            e.Value = pData.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                        }
                        else
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(pData.TDL_PATIENT_DOB);
                        }
                    }
                    else if (e.Column.FieldName == "ADD_NAME")
                    {
                        e.Value = pData.ADD_LOGINNAME + " - " + pData.ADD_USERNAME;
                    }
                    else if (e.Column.FieldName == "REMOVE_NAME")
                    {
                        if (!String.IsNullOrWhiteSpace(pData.REMOVE_LOGINNAME))
                        {
                            e.Value = pData.REMOVE_LOGINNAME + " - " + pData.REMOVE_USERNAME;
                        }
                        else
                        {
                            e.Value = null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewTreatmentBedRoom_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    V_HIS_TREATMENT_BED_ROOM_1 data = (V_HIS_TREATMENT_BED_ROOM_1)gridViewTreatmentBedRoom.GetRow(e.RowHandle);
                    if (data != null && e.Column.FieldName == "EDIT")
                    {
                        if (this.currentRoom != null && this.currentRoom.DEPARTMENT_ID == data.DEPARTMENT_ID)
                            e.RepositoryItem = repositoryItemButtonEdit;
                        else
                            e.RepositoryItem = repositoryItemButtonEditDisable;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonEdit_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                this.editData = (V_HIS_TREATMENT_BED_ROOM_1)gridViewTreatmentBedRoom.GetFocusedRow();
                this.ProcessSetEditForm();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessSetEditForm()
        {
            try
            {
                this.RemoveErrorValidControl();
                if (this.editData != null)
                {
                    dtAddTime.Enabled = true;
                    lblTreatmentCode.Text = this.editData.TREATMENT_CODE;
                    lblBedRoomName.Text = this.editData.BED_ROOM_NAME;
                    lblPatientName.Text = this.editData.TDL_PATIENT_NAME;
                    dtAddTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.editData.ADD_TIME).Value;
                    if (this.editData.REMOVE_TIME.HasValue)
                    {
                        dtRemoveTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.editData.REMOVE_TIME ?? 0).Value;
                        dtRemoveTime.Enabled = true;
                    }
                    else
                    {
                        dtRemoveTime.EditValue = null;
                        dtRemoveTime.Enabled = false;
                    }
                    btnSave.Enabled = true;
                    btnCancel.Enabled = true;
                }
                else
                {
                    btnSave.Enabled = false;
                    btnCancel.Enabled = false;
                    lblTreatmentCode.Text = "";
                    lblBedRoomName.Text = "";
                    lblPatientName.Text = "";
                    dtAddTime.EditValue = null;
                    dtAddTime.Enabled = false;
                    dtRemoveTime.EditValue = null;
                    dtRemoveTime.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtAddTime_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (dtRemoveTime.Enabled)
                    {
                        dtRemoveTime.Focus();
                    }
                    else
                    {
                        btnSave.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtRemoveTime_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    btnSave.Focus();
                }
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
                if (!btnSave.Enabled || this.editData == null) return;
                if (!dxValidationProvider1.Validate()) return;
                if (dtAddTime.EditValue == null || dtAddTime.DateTime == DateTime.MinValue)
                {
                    XtraMessageBox.Show("Bạn chưa nhập thời gian vào", "Thông báo", DevExpress.Utils.DefaultBoolean.True);
                    return;
                }
                if (this.editData.REMOVE_TIME.HasValue && (dtRemoveTime.EditValue == null || dtRemoveTime.DateTime == DateTime.MinValue))
                {
                    XtraMessageBox.Show("Bạn chưa nhập thời gian ra", "Thông báo", DevExpress.Utils.DefaultBoolean.True);
                    return;
                }
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                Mapper.CreateMap<V_HIS_TREATMENT_BED_ROOM_1, HIS_TREATMENT_BED_ROOM>();
                HIS_TREATMENT_BED_ROOM data = Mapper.Map<HIS_TREATMENT_BED_ROOM>(this.editData);
                data.ADD_TIME = Convert.ToInt64(dtAddTime.DateTime.ToString("yyyyMMddHHmmss"));
                if (data.REMOVE_TIME.HasValue)
                {
                    data.REMOVE_TIME = Convert.ToInt64(dtRemoveTime.DateTime.ToString("yyyyMMddHHmmss"));
                }
                var rs = new BackendAdapter(param).Post<HIS_TREATMENT_BED_ROOM>("api/HisTreatmentBedRoom/UpdateTime", ApiConsumers.MosConsumer, data, param);
                if (rs != null)
                {
                    success = true;
                    this.LoadPaging(new CommonParam(this.start, this.limit));
                }
                WaitingManager.Hide();
                MessageManager.Show(this, param, success);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnCancel.Enabled) return;
                this.editData = null;
                this.ProcessSetEditForm();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;
                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;
                if (positionHandleControl == -1)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControl > edit.TabIndex)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.Focus();
                        edit.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barBtnSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barBtnRefresh_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnRefresh_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barBtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void barBtnCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnCancel_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewTreatmentBedRoom_Click(object sender, EventArgs e)
        {
            try
            {
                var rowData = (MOS.EFMODEL.DataModels.V_HIS_TREATMENT_BED_ROOM_1)gridViewTreatmentBedRoom.GetFocusedRow();
                if (rowData != null)
                {
                    ChangedDataRow(rowData);

                    //Set focus vào control editor đầu tiên
                    SetFocusEditor();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangedDataRow(V_HIS_TREATMENT_BED_ROOM_1 data)
        {
            try
            {
                if (data != null)
                {
                    FillDataToEditorControl(data);
                    lblTreatmentCode.Text = data.TREATMENT_CODE;
                    lblPatientName.Text = data.TDL_PATIENT_NAME;
                    lblBedRoomName.Text = data.BED_ROOM_NAME;
                    dtAddTime.Text = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.ADD_TIME);
                    if (data.REMOVE_TIME != null)
                    {
                        dtRemoveTime.Text = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.REMOVE_TIME ?? 0);
                    }
                    else
                    {
                        dtRemoveTime.Text = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToEditorControl(V_HIS_TREATMENT_BED_ROOM_1 data)
        {
            try
            {
                if (data != null)
                {
                    lblTreatmentCode.Text= data.TREATMENT_CODE;
                    lblPatientName.Text= data.TDL_PATIENT_NAME;
                    lblBedRoomName.Text=data.BED_ROOM_NAME;
                    dtAddTime.Text= Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.ADD_TIME);
                    if (data.REMOVE_TIME != null)
                    {
                        dtRemoveTime.Text = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.REMOVE_TIME ?? 0);
                    }
                    else
                    {
                        dtRemoveTime.Text = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtTreatmentCode_EditValueChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtTreatmentCode.Text))
            {
                this.treatmentCode = null;
            }
            
        }
    }
}

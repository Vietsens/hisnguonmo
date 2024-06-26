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
using HIS.Desktop.Utility;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.Utilities.Extensions;
using MOS.Filter;
using MOS.EFMODEL.DataModels;
using Inventec.Core;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.LocalStorage.ConfigApplication;
using MOS.SDO;
using DevExpress.XtraEditors;
using HIS.Desktop.Controls.Session;
using Inventec.Common.Controls.EditorLoader;
using Newtonsoft.Json;

namespace HIS.Desktop.Plugins.HoldReturnList
{
    public partial class UCHoldReturnList : UserControlBase
    {
        private int start = 0;
        private int limit = 0;
        private int rowCount = 0;
        private int totalData = 0;

        private List<HIS_DOC_HOLD_TYPE> listDocHoldType;
        private List<long> selecedDocHoldTypeIds = new List<long>();
        private List<HoldReturnListADO> lstStatus;

        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;

        public UCHoldReturnList(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
        }

        private void UcHoldReturnList_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                this.InitDocHoldTypeCheck();
                this.InitDocHoldType();
                InitComboPatientStatus();
                this.SetDefaultValueControl();
                this.InitControlState();
                this.FillDataToGrid();
                txtPatientCode.Focus();
                txtPatientCode.SelectAll();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitControlState()
        {
            try
            {
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData("HIS.Desktop.Plugins.HoldReturnList");
                HoldReturnListADO ado = new HoldReturnListADO();
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == "HoldReturnListADO")
                        {
                            if (!string.IsNullOrEmpty(item.VALUE))
                            {
                                ado = JsonConvert.DeserializeObject<HoldReturnListADO>(item.VALUE);
                            }
                        }
                    }
                    if (ado != null)
                    {
                        if (ado.RANGE == 1)
                            chkAll.Checked = true;
                        else if (ado.RANGE == 2)
                            chkWorkingRoom.Checked = true;
                        if (ado.PATIENT_STATUS_ID != 0)
                            cboPatientStatus.EditValue = ado.PATIENT_STATUS_ID;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboPatientStatus()
        {
            try
            {
                lstStatus = new List<HoldReturnListADO>();
                for (int i = 1; i < 6; i++)
                {
                    HoldReturnListADO ado = new HoldReturnListADO();
                    ado.PATIENT_STATUS_ID = i;
                    if (i == 1)
                        ado.PATIENT_STATUS_NAME = "Chưa kết thúc điều trị";
                    else if (i == 2)
                        ado.PATIENT_STATUS_NAME = "Chưa khóa viện phí";
                    else if (i == 3)
                        ado.PATIENT_STATUS_NAME = "Đã khóa viện phí";
                    else if (i == 4)
                        ado.PATIENT_STATUS_NAME = "Đã kết thúc điều trị";
                    else if (i == 5)
                        ado.PATIENT_STATUS_NAME = "Tất cả";
                    lstStatus.Add(ado);
                }

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PATIENT_STATUS_NAME", "", 100, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PATIENT_STATUS_NAME", "PATIENT_STATUS_ID", columnInfos, false, 100);
                ControlEditorLoader.Load(cboPatientStatus, lstStatus, controlEditorADO);
                cboPatientStatus.EditValue = lstStatus[4].PATIENT_STATUS_ID;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitDocHoldTypeCheck()
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cboDocHoldType.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(SelectionGrid__DispenseType);
                cboDocHoldType.Properties.Tag = gridCheck;
                cboDocHoldType.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cboDocHoldType.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboDocHoldType.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitDocHoldType()
        {
            try
            {
                HisDocHoldTypeFilter filter = new HisDocHoldTypeFilter();
                filter.IS_ACTIVE = 1;
                this.listDocHoldType = new BackendAdapter(new CommonParam()).Get<List<HIS_DOC_HOLD_TYPE>>("api/HisDocHoldType/Get", ApiConsumers.MosConsumer, filter, null);
                if (this.listDocHoldType != null)
                {
                    cboDocHoldType.Properties.DataSource = this.listDocHoldType;
                    cboDocHoldType.Properties.DisplayMember = "DOC_HOLD_TYPE_NAME";
                    cboDocHoldType.Properties.ValueMember = "ID";
                    DevExpress.XtraGrid.Columns.GridColumn col2 = cboDocHoldType.Properties.View.Columns.AddField("DOC_HOLD_TYPE_NAME");
                    col2.VisibleIndex = 1;
                    col2.Width = 200;
                    col2.Caption = "Tất cả";
                    cboDocHoldType.Properties.PopupFormWidth = 200;
                    //cboDocHoldType.Properties.View.OptionsView.ShowColumnHeaders = false;
                    cboDocHoldType.Properties.View.OptionsSelection.MultiSelect = true;
                    GridCheckMarksSelection gridCheckMark = cboDocHoldType.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                    {
                        gridCheckMark.ClearSelection(cboDocHoldType.Properties.View);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SelectionGrid__DispenseType(object sender, EventArgs e)
        {
            try
            {
                selecedDocHoldTypeIds = new List<long>();
                List<string> name = new List<string>();
                foreach (HIS_DOC_HOLD_TYPE rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                    {
                        selecedDocHoldTypeIds.Add(rv.ID);
                        name.Add(rv.DOC_HOLD_TYPE_NAME);
                    }
                }
                string text = "";
                if (name.Count > 0)
                {
                    text = String.Join(",", name);
                }
                cboDocHoldType.Text = text;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultValueControl()
        {
            try
            {
                txtPatientCode.Text = "";
                txtKeyword.Text = "";
                cboStatus.SelectedIndex = 1;
                GridCheckMarksSelection gridCheckMark = cboDocHoldType.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.SelectAll(this.listDocHoldType);
                }
                cboDocHoldType.Text = "Tất cả";
                cboPatientStatus.EditValue = lstStatus[4].PATIENT_STATUS_ID;
                txtPatientCode.Focus();
                txtPatientCode.SelectAll();
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
                int pagingSize = ucPaging1.pagingGrid != null ? ucPaging1.pagingGrid.PageSize : (int)ConfigApplications.NumPageSize;
                GridPaging(new CommonParam(0, pagingSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = totalData;
                ucPaging1.Init(GridPaging, param, pagingSize);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GridPaging(object param)
        {
            try
            {
                this.start = ((CommonParam)param).Start ?? 0;
                this.limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(this.start, this.limit);
                List<V_HIS_HOLD_RETURN> listData = new List<V_HIS_HOLD_RETURN>();
                HisHoldReturnViewFilter filter = new HisHoldReturnViewFilter();

                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "HOLD_TIME";
                //filter.RESPONSIBLE_ROOM_ID = this.currentModuleBase.RoomId;
                if (!String.IsNullOrWhiteSpace(txtTreatmentCode.Text))
                {
                    string code = txtTreatmentCode.Text.Trim();
                    if (code.Length < 12)
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                    }
                    txtTreatmentCode.Text = code;
                    filter.TREATMENT_CODE__EXACT = code;
                }
                else if (!String.IsNullOrWhiteSpace(txtPatientCode.Text))
                {
                    string code = txtPatientCode.Text.Trim();
                    if (code.Length < 10)
                    {
                        code = string.Format("{0:0000000000}", Convert.ToInt64(code));
                    }
                    txtPatientCode.Text = code;
                    filter.PATIENT_CODE__EXACT = code;
                }
                else
                {
                    filter.KEY_WORD = txtKeyword.Text;
                    if (cboStatus.SelectedIndex == 1)
                    {
                        filter.HAS_RETURN_TIME = false;
                    }
                    else if (cboStatus.SelectedIndex == 2)
                    {
                        filter.HAS_RETURN_TIME = true;
                    }
                    if (selecedDocHoldTypeIds != null && selecedDocHoldTypeIds.Count > 0)
                    {
                        filter.DOC_HOLD_TYPE_IDs = selecedDocHoldTypeIds;
                    }
                    if (cboPatientStatus.EditValue != null)
                    {
                        var sttPatient = Convert.ToInt16(cboPatientStatus.EditValue);
                        switch (sttPatient)
                        {
                            case 1:
                                filter.TREATMENT_IS_PAUSE = false;
                                break;
                            case 2:
                                filter.TREATMENT_IS_ACTIVE = 1;
                                break;
                            case 3:
                                filter.TREATMENT_IS_ACTIVE = 0;
                                break;
                            case 4:
                                filter.TREATMENT_IS_PAUSE = true;
                                break;
                        }
                    }
                    if (chkWorkingRoom.Checked)
                    {
                        filter.HOLD_ROOM_ID = this.currentModuleBase.RoomId;
                    }
                }

                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData("filter:___", filter));

                ApiResultObject<List<V_HIS_HOLD_RETURN>> rs = new BackendAdapter(paramCommon).GetRO<List<V_HIS_HOLD_RETURN>>("api/HisHoldReturn/GetView", ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);

                if (rs != null)
                {
                    if (rs.Data != null)
                    {
                        listData = rs.Data;
                    }
                    this.rowCount = (listData == null ? 0 : listData.Count);
                    this.totalData = (rs.Param == null ? 0 : rs.Param.Count ?? 0);
                }

                gridControlHoldReturn.BeginUpdate();
                gridControlHoldReturn.DataSource = listData;
                gridControlHoldReturn.EndUpdate();

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramCommon);
                #endregion
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
                    btnFind_Click(null, null);
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
                    btnFind_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDocHoldType_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    GridCheckMarksSelection gridCheckMark = cboDocHoldType.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                    {
                        gridCheckMark.ClearSelection(cboDocHoldType.Properties.View);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDocHoldType_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                GridCheckMarksSelection gridCheckMark = cboDocHoldType.Properties.Tag as GridCheckMarksSelection;
                List<string> name = new List<string>();
                if (gridCheckMark == null) return;
                foreach (HIS_DOC_HOLD_TYPE rv in gridCheckMark.Selection)
                {
                    name.Add(rv.DOC_HOLD_TYPE_NAME);
                }
                string text = "";
                if (name.Count > 0)
                {
                    text = String.Join(",", name);
                }
                if (name.Count() == this.listDocHoldType.Count())
                {
                    text = "Tất cả";
                }
                e.DisplayText = text;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDocHoldType_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboStatus_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnFind1.Enabled) return;
                WaitingManager.Show();
                this.FillDataToGrid();
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
                if (!btnRefresh1.Enabled) return;
                WaitingManager.Show();
                this.SetDefaultValueControl();
                this.FillDataToGrid();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewHoldReturn_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    V_HIS_HOLD_RETURN data = (V_HIS_HOLD_RETURN)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;

                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1 + this.start;
                        }
                        else if (e.Column.FieldName == "TDL_PATIENT_DOB_STR")
                        {
                            if (data.TDL_PATIENT_IS_HAS_NOT_DAY_DOB.HasValue && data.TDL_PATIENT_IS_HAS_NOT_DAY_DOB.Value == 1)
                            {
                                e.Value = data.TDL_PATIENT_DOB.ToString().Substring(0, 4);
                            }
                            else
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.TDL_PATIENT_DOB);
                            }
                        }
                        else if (e.Column.FieldName == "HOLD_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.HOLD_TIME);
                        }
                        else if (e.Column.FieldName == "HOLD_USER")
                        {
                            e.Value = (data.HOLD_LOGINNAME ?? "") + " - " + (data.HOLD_USERNAME ?? "");
                        }
                        else if (e.Column.FieldName == "RETURN_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.RETURN_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "RETURN_USER")
                        {
                            if (!String.IsNullOrWhiteSpace(data.RETURN_LOGINNAME))
                            {
                                e.Value = (data.RETURN_LOGINNAME ?? "") + " - " + (data.RETURN_USERNAME ?? "");
                            }
                            else
                            {
                                e.Value = "";
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
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewHoldReturn_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle < 0 || e.Column.FieldName != "RETURN") return;
                V_HIS_HOLD_RETURN data = (V_HIS_HOLD_RETURN)gridViewHoldReturn.GetRow(e.RowHandle);
                if (data != null)
                {
                    if (e.Column.FieldName == "RETURN")
                    {
                        if (data.RETURN_TIME.HasValue)
                        {
                            e.RepositoryItem = repositoryItemButtonUnreturn__Enable;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemButtonReturn__Enable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonReturn__Enable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                V_HIS_HOLD_RETURN row = (V_HIS_HOLD_RETURN)gridViewHoldReturn.GetFocusedRow();
                if (row != null && !row.RETURN_TIME.HasValue)
                {
                    var yes = XtraMessageBox.Show("Bạn có muốn trả giấy tờ cho bệnh nhân?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question, DevExpress.Utils.DefaultBoolean.True);
                    if (yes != DialogResult.Yes)
                    {
                        return;
                    }
                    WaitingManager.Show();
                    CommonParam param = new CommonParam();
                    bool success = false;
                    HisHoldReturnSDO sdo = new HisHoldReturnSDO();
                    sdo.HoldReturnId = row.ID;
                    sdo.WorkingRoomId = this.currentModuleBase.RoomId;

                    HIS_HOLD_RETURN rs = new BackendAdapter(param).Post<HIS_HOLD_RETURN>("api/HisHoldReturn/Return", ApiConsumers.MosConsumer, sdo, param);
                    if (rs != null)
                    {
                        success = true;
                        this.FillDataToGrid();
                    }
                    WaitingManager.Hide();

                    MessageManager.Show(this.ParentForm, param, success);

                    SessionManager.ProcessTokenLost(param);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonUnreturn__Enable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                V_HIS_HOLD_RETURN row = (V_HIS_HOLD_RETURN)gridViewHoldReturn.GetFocusedRow();
                if (row != null && row.RETURN_TIME.HasValue)
                {
                    var yes = XtraMessageBox.Show("Bạn có muốn Hủy trả giấy tờ của bệnh nhân?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question, DevExpress.Utils.DefaultBoolean.True);
                    if (yes != DialogResult.Yes)
                    {
                        return;
                    }
                    WaitingManager.Show();
                    CommonParam param = new CommonParam();
                    bool success = false;
                    HisHoldReturnSDO sdo = new HisHoldReturnSDO();
                    sdo.HoldReturnId = row.ID;
                    sdo.WorkingRoomId = this.currentModuleBase.RoomId;

                    HIS_HOLD_RETURN rs = new BackendAdapter(param).Post<HIS_HOLD_RETURN>("api/HisHoldReturn/CancelReturn", ApiConsumers.MosConsumer, sdo, param);
                    if (rs != null)
                    {
                        success = true;
                        this.FillDataToGrid();
                    }
                    WaitingManager.Hide();

                    MessageManager.Show(this.ParentForm, param, success);

                    SessionManager.ProcessTokenLost(param);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void BtnFind()
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

        public void BtnRefresh()
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

        private void txtTreatmentCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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

        private void cboStatus_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboStatus.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCHoldReturnList_Leave(object sender, EventArgs e)
        {
            try
            {
                if (cboPatientStatus.EditValue != null || chkAll.Checked || chkWorkingRoom.Checked)
                {
                    SaveData();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SaveData()
        {
            try
            {
                HoldReturnListADO ado = new HoldReturnListADO();
                if (cboPatientStatus.EditValue != null)
                {
                    ado.PATIENT_STATUS_ID = Convert.ToInt16(cboPatientStatus.EditValue);
                    ado.PATIENT_STATUS_NAME = lstStatus.Where(o => o.PATIENT_STATUS_ID == Convert.ToInt64(cboPatientStatus.EditValue)).First().PATIENT_STATUS_NAME;
                }
                if (chkAll.Checked)
                    ado.RANGE = 1;
                else if (chkWorkingRoom.Checked)
                    ado.RANGE = 2;
                string textJson = JsonConvert.SerializeObject(ado);

                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdateValue = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == "HoldReturnListADO" && o.MODULE_LINK == "HIS.Desktop.Plugins.HoldReturnList").FirstOrDefault() : null;
                if (csAddOrUpdateValue != null)
                {
                    csAddOrUpdateValue.VALUE = textJson;
                }
                else
                {
                    csAddOrUpdateValue = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdateValue.KEY = "HoldReturnListADO";
                    csAddOrUpdateValue.VALUE = textJson;
                    csAddOrUpdateValue.MODULE_LINK = "HIS.Desktop.Plugins.HoldReturnList";
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdateValue);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                chkWorkingRoom.Checked = !chkAll.Checked;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkWorkingRoom_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                chkAll.Checked = !chkWorkingRoom.Checked;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnFind_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnFind_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void cboPatientStatus_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                cboPatientStatus.EditValue = null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

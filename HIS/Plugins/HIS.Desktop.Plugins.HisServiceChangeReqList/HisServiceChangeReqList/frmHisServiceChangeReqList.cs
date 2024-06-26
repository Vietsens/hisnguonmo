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
using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.HisServiceChangeReqList.ADO;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using Inventec.UC.Paging;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.HisServiceChangeReqList.HisServiceChangeReqList
{
    public partial class frmHisServiceChangeReqList : FormBase
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        HIS_EXECUTE_ROOM currentExecuteRoom;
        private string CurrentLoginName;

        Inventec.Desktop.Common.Modules.Module moduleData;
        V_HIS_SERVICE_CHANGE_REQ currentDataPrint;
        #endregion

        public frmHisServiceChangeReqList(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
            this.moduleData = module;
            this.Text = module.text;
        }

        private void frmHisServiceChangeReqList_Load(object sender, EventArgs e)
        {
            try
            {
                SetCaptionByLanguageKey();
                currentExecuteRoom = BackendDataWorker.Get<HIS_EXECUTE_ROOM>().FirstOrDefault(o => o.ROOM_ID == moduleData.RoomId);
                CurrentLoginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                LoadDataCombo();
                SetDefaultValue();
                FillDataToGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultValue()
        {
            try
            {
                txtKeyword.Text = "";
                txtServiceReqCode.Text = "";
                txtTreatmentCode.Text = "";
                cboExecuteRoom.EditValue = null;
                cboReqRoom.EditValue = null;
                cboStatus.EditValue = null;
                dtTimeFrom.EditValue = DateTime.Now;
                dtTimeTo.EditValue = DateTime.Now;

                if (currentExecuteRoom != null)
                {
                    cboStatus.EditValue = FilterTypeADO.stt.moi;
                    if (currentExecuteRoom.IS_EXAM == 1)
                    {
                        cboReqRoom.EditValue = currentExecuteRoom.ROOM_ID;
                    }
                    else
                    {
                        cboExecuteRoom.EditValue = currentExecuteRoom.ROOM_ID;
                    }
                }
                else if (moduleData.RoomTypeId == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__BUONG)
                {
                    cboStatus.EditValue = FilterTypeADO.stt.moi;
                    cboReqRoom.EditValue = moduleData.RoomId;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataCombo()
        {
            try
            {
                var executeRoom = BackendDataWorker.Get<HIS_EXECUTE_ROOM>();
                var bedRoom = BackendDataWorker.Get<HIS_BED_ROOM>();

                List<RoomADO> roomData = new List<RoomADO>();
                if (executeRoom != null && executeRoom.Count > 0)
                {
                    foreach (var item in executeRoom)
                    {
                        roomData.Add(new RoomADO(item));
                    }
                }

                if (bedRoom != null && bedRoom.Count > 0)
                {
                    foreach (var item in bedRoom)
                    {
                        roomData.Add(new RoomADO(item));
                    }
                }

                LoadDataToCbo(cboExecuteRoom, roomData, "CODE", "NAME", "ID");
                LoadDataToCbo(cboReqRoom, roomData, "CODE", "NAME", "ID");
                InitComboStatus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void InitComboStatus()
        {
            try
            {
                List<FilterTypeADO> ListStatusAll = new List<FilterTypeADO>();

                FilterTypeADO all = new FilterTypeADO(FilterTypeADO.stt.moi, "Mới");
                ListStatusAll.Add(all);

                FilterTypeADO duyetBhyt = new FilterTypeADO(FilterTypeADO.stt.duyetChiDinh, "Đã duyệt sửa chỉ định");
                ListStatusAll.Add(duyetBhyt);

                FilterTypeADO ketthuc = new FilterTypeADO(FilterTypeADO.stt.duyetPhieuThu, "Đã duyệt sửa phiếu thu");
                ListStatusAll.Add(ketthuc);

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("Name", "", 250, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("Name", "id", columnInfos, false, 250);
                ControlEditorLoader.Load(cboStatus, ListStatusAll, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToCbo(GridLookUpEdit cbo, object data, string code, string name, string value)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo(code, "", 150, 1));
                columnInfos.Add(new ColumnInfo(name, "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO(name, value, columnInfos, false, 250);
                ControlEditorLoader.Load(cbo, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmHisServiceChangeReqList.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmHisServiceChangeReqList.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnReset.Text = Inventec.Common.Resource.Get.Value("frmHisServiceChangeReqList.btnReset.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnFind.Text = Inventec.Common.Resource.Get.Value("frmHisServiceChangeReqList.btnFind.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboStatus.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisServiceChangeReqList.cboStatus.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboExecuteRoom.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisServiceChangeReqList.cboExecuteRoom.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboReqRoom.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisServiceChangeReqList.cboReqRoom.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmHisServiceChangeReqList.txtKeyword.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtServiceReqCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmHisServiceChangeReqList.txtServiceReqCode.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("frmHisServiceChangeReqList.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gc_Stt.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceChangeReqList.gc_Stt.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gc_ServiceReqCode.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceChangeReqList.gc_ServiceReqCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gc_TreatmentCode.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceChangeReqList.gc_TreatmentCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gc_PatientName.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceChangeReqList.gc_PatientName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gc_ServiceName.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceChangeReqList.gc_ServiceName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gc_AlterServiceName.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceChangeReqList.gc_AlterServiceName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gc_Amount.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceChangeReqList.gc_Amount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gc_RequestName.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceChangeReqList.gc_RequestName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gc_RequestRoom.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceChangeReqList.gc_RequestRoom.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gc_ApprovalName.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceChangeReqList.gc_ApprovalName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gc_ApprovalCashier.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceChangeReqList.gc_ApprovalCashier.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gc_Creator.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceChangeReqList.gc_Creator.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gc_CreateTime.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceChangeReqList.gc_CreateTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gc_Modifier.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceChangeReqList.gc_Modifier.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gc_ModifyTime.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceChangeReqList.gc_ModifyTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtTreatmentCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmHisServiceChangeReqList.txtTreatmentCode.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciReqRoom.Text = Inventec.Common.Resource.Get.Value("frmHisServiceChangeReqList.lciReqRoom.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciExecuteRoom.Text = Inventec.Common.Resource.Get.Value("frmHisServiceChangeReqList.lciExecuteRoom.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciStatus.Text = Inventec.Common.Resource.Get.Value("frmHisServiceChangeReqList.lciStatus.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTimeFrom.Text = Inventec.Common.Resource.Get.Value("frmHisServiceChangeReqList.lciTimeFrom.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTimeTo.Text = Inventec.Common.Resource.Get.Value("frmHisServiceChangeReqList.lciTimeTo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gc_ExecuteRoom.Caption = Inventec.Common.Resource.Get.Value("frmHisServiceChangeReqList.gc_ExecuteRoom.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void FillDataToGridControl()
        {
            try
            {
                WaitingManager.Show();
                int numPageSize = 0;
                if (ucPaging.pagingGrid != null)
                {
                    numPageSize = ucPaging.pagingGrid.PageSize;
                }
                else
                {
                    numPageSize = ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                }

                LoadPage(new CommonParam(0, numPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging.Init(LoadPage, param, numPageSize, this.gridControl);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        public void LoadPage(object param)
        {
            try
            {
                WaitingManager.Show();
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                Inventec.Core.ApiResultObject<List<V_HIS_SERVICE_CHANGE_REQ>> apiResult = null;
                HisServiceChangeReqViewFilter filter = new HisServiceChangeReqViewFilter();
                SetFilterNavBar(ref filter);
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                gridControl.DataSource = null;
                gridView.BeginUpdate();
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => filter), filter));
                apiResult = new BackendAdapter(paramCommon).GetRO<List<V_HIS_SERVICE_CHANGE_REQ>>(HisRequestUriStore.HIS_SERVICE_CHANGE_REQ_GETVIEW, ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    var data = (List<V_HIS_SERVICE_CHANGE_REQ>)apiResult.Data;
                    if (data != null)
                    {
                        gridControl.DataSource = data;
                        gridView.GridControl.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                gridView.EndUpdate();

                #region Process has exception
                SessionManager.ProcessTokenLost(paramCommon);
                #endregion
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
                WaitingManager.Hide();
            }
        }

        private void SetFilterNavBar(ref HisServiceChangeReqViewFilter filter)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(txtTreatmentCode.Text.Trim()))
                {
                    string code = txtTreatmentCode.Text.Trim();
                    if (code.Length < 12 && checkDigit(code))
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        txtTreatmentCode.Text = code;
                    }
                    filter.TDL_TREATMENT_CODE__EXACT = code;
                }
                else if (!String.IsNullOrWhiteSpace(txtServiceReqCode.Text.Trim()))
                {
                    string code = txtServiceReqCode.Text.Trim();
                    if (code.Length < 12 && checkDigit(code))
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        txtServiceReqCode.Text = code;
                    }
                    filter.SERVICE_REQ_CODE__EXACT = code;
                }
                else
                {
                    filter.KEY_WORD = txtKeyword.Text.Trim();
                    if (cboStatus.EditValue != null)
                    {
                        FilterTypeADO.stt status = (FilterTypeADO.stt)cboStatus.EditValue;
                        switch (status)
                        {
                            case FilterTypeADO.stt.moi:
                                filter.HAS_APPROVAL_LOGINNAME = false;
                                filter.HAS_APPROVAL_CASHIER_LOGINNAME = false;
                                break;
                            case FilterTypeADO.stt.duyetChiDinh:
                                filter.HAS_APPROVAL_LOGINNAME = true;
                                filter.HAS_APPROVAL_CASHIER_LOGINNAME = false;
                                break;
                            case FilterTypeADO.stt.duyetPhieuThu:
                                filter.HAS_APPROVAL_LOGINNAME = true;
                                filter.HAS_APPROVAL_CASHIER_LOGINNAME = true;
                                break;
                            default:
                                break;
                        }
                    }

                    if (dtTimeFrom.EditValue != null && dtTimeFrom.DateTime != DateTime.MinValue)
                        filter.MODIFY_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(dtTimeFrom.DateTime.ToString("yyyyMMdd") + "000000");

                    if (dtTimeTo.EditValue != null && dtTimeTo.DateTime != DateTime.MinValue)
                        filter.MODIFY_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(dtTimeTo.DateTime.ToString("yyyyMMdd") + "235959");

                    if (cboReqRoom.EditValue != null)
                    {
                        filter.REQUEST_ROOM_ID = Inventec.Common.TypeConvert.Parse.ToInt32(cboReqRoom.EditValue.ToString());
                    }

                    if (cboExecuteRoom.EditValue != null)
                    {
                        filter.EXECUTE_ROOM_ID = Inventec.Common.TypeConvert.Parse.ToInt32(cboExecuteRoom.EditValue.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
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

        private void gridView_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    string approvalLoginname = (gridView.GetRowCellValue(e.RowHandle, "APPROVAL_LOGINNAME") ?? "").ToString();
                    long branchId = Inventec.Common.TypeConvert.Parse.ToInt64((gridView.GetRowCellValue(e.RowHandle, "BRANCH_ID") ?? "").ToString());
                    long requestRoomId = Inventec.Common.TypeConvert.Parse.ToInt64((gridView.GetRowCellValue(e.RowHandle, "REQUEST_ROOM_ID") ?? "").ToString());
                    long executeRoomId = Inventec.Common.TypeConvert.Parse.ToInt64((gridView.GetRowCellValue(e.RowHandle, "EXECUTE_ROOM_ID") ?? "").ToString());
                    string approvalCashierLoginname = (gridView.GetRowCellValue(e.RowHandle, "APPROVAL_CASHIER_LOGINNAME") ?? "").ToString();
                    string creator = (gridView.GetRowCellValue(e.RowHandle, "CREATOR") ?? "").ToString();
                    if (e.Column.FieldName == gc_ApproveChange.FieldName)
                    {
                        if (moduleData.RoomTypeId == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__TN && branchId == WorkPlace.GetBranchId() && String.IsNullOrWhiteSpace(approvalCashierLoginname))
                        {
                            e.RepositoryItem = repositoryItemBtnApprovalCashier;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemBtnApprovalCashier_Disable;
                        }
                    }
                    else if (e.Column.FieldName == gc_ApproveEdit.FieldName)
                    {
                        if (((currentExecuteRoom != null && (currentExecuteRoom.ROOM_ID == requestRoomId || currentExecuteRoom.ROOM_ID == executeRoomId)) || moduleData.RoomTypeId == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__BUONG) && String.IsNullOrWhiteSpace(approvalLoginname))
                        {
                            e.RepositoryItem = repositoryItemBtnApproval;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemBtnApproval_Disable;
                        }
                    }
                    else if (e.Column.FieldName == gc_Delete.FieldName)
                    {
                        if (String.IsNullOrWhiteSpace(approvalLoginname) && String.IsNullOrWhiteSpace(approvalCashierLoginname)
                            && (HIS.Desktop.IsAdmin.CheckLoginAdmin.IsAdmin(CurrentLoginName) || creator == CurrentLoginName))
                        {
                            e.RepositoryItem = repositoryItemEnableDelete;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemDisableDelete;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    V_HIS_SERVICE_CHANGE_REQ data = (V_HIS_SERVICE_CHANGE_REQ)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1 + startPage;
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "REQUEST_NAME")
                        {
                            e.Value = string.Format("{0} - {1}", data.REQUEST_LOGINNAME, data.REQUEST_USERNAME);
                        }
                        else if (e.Column.FieldName == "APPROVAL_NAME")
                        {
                            e.Value = string.Format("{0} - {1}", data.APPROVAL_LOGINNAME, data.APPROVAL_USERNAME);
                        }
                        else if (e.Column.FieldName == "APPROVAL_CASHIER_NAME")
                        {
                            e.Value = string.Format("{0} - {1}", data.APPROVAL_CASHIER_LOGINNAME, data.APPROVAL_CASHIER_USERNAME);
                        }
                    }
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
                FillDataToGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            try
            {
                SetDefaultValue();
                FillDataToGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemEnableDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (V_HIS_SERVICE_CHANGE_REQ)gridView.GetFocusedRow();
                if (row != null)
                {
                    CommonParam param = new CommonParam();
                    bool success = false;

                    bool reqChange = new BackendAdapter(param).Post<bool>("api/HisServiceChangeReq/Delete", ApiConsumers.MosConsumer, row.ID, param);
                    if (reqChange)
                    {
                        success = true;
                        FillDataToGridControl();
                    }

                    MessageManager.Show(this, param, success);
                    SessionManager.ProcessTokenLost(param);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemBtnApproval_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (V_HIS_SERVICE_CHANGE_REQ)gridView.GetFocusedRow();
                if (row != null)
                {
                    CommonParam param = new CommonParam();
                    bool success = false;

                    HisServiceChangeReqApproveSDO sdo = new HisServiceChangeReqApproveSDO();
                    sdo.ServiceChangeReqId = row.ID;
                    sdo.WorkingRoomId = this.moduleData.RoomId;

                    HIS_SERVICE_CHANGE_REQ reqChange = new BackendAdapter(param).Post<HIS_SERVICE_CHANGE_REQ>("api/HisServiceChangeReq/Approve", ApiConsumers.MosConsumer, sdo, param);
                    if (reqChange != null)
                    {
                        success = true;
                        FillDataToGridControl();
                    }

                    MessageManager.Show(this, param, success);
                    SessionManager.ProcessTokenLost(param);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemBtnApprovalCashier_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (V_HIS_SERVICE_CHANGE_REQ)gridView.GetFocusedRow();
                if (row != null)
                {
                    ApprovalCashier.FormApprovalCashier form = new ApprovalCashier.FormApprovalCashier(this.moduleData, row, FillDataToGridControl);
                    if (form != null)
                    {
                        form.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboReqRoom_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboReqRoom.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboExecuteRoom_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboExecuteRoom.EditValue = null;
                }
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
                btnReset_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barBtnSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void txtTreatmentCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrWhiteSpace(txtTreatmentCode.Text))
                    {
                        FillDataToGridControl();
                    }
                    else
                    {
                        txtServiceReqCode.Focus();
                        txtServiceReqCode.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtServiceReqCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrWhiteSpace(txtServiceReqCode.Text))
                    {
                        FillDataToGridControl();
                    }
                    else
                    {
                        txtKeyword.Focus();
                        txtKeyword.SelectAll();
                    }
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
                    FillDataToGridControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemBtnPrint_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (V_HIS_SERVICE_CHANGE_REQ)gridView.GetFocusedRow();
                currentDataPrint = row;
                if (row != null)
                {
                    Inventec.Common.RichEditor.RichEditorStore richStore = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                    richStore.RunPrintTemplate("Mps000433", this.DelegateRunPrinter);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                if (this.currentDataPrint == null)
                    return result;

                WaitingManager.Show();
                V_HIS_SERVICE_REQ serviceReq = new V_HIS_SERVICE_REQ();
                V_HIS_TREATMENT treatment = new V_HIS_TREATMENT();
                V_HIS_PATIENT_TYPE_ALTER patienTypeAlter = new V_HIS_PATIENT_TYPE_ALTER();
                List<HIS_SERE_SERV> sereServ = new List<HIS_SERE_SERV>();

                CommonParam param = new CommonParam();
                HisServiceReqViewFilter reqFilter = new HisServiceReqViewFilter();
                reqFilter.ID = currentDataPrint.TDL_SERVICE_REQ_ID;
                var reqApiResult = new BackendAdapter(param).Get<List<V_HIS_SERVICE_REQ>>("api/HisServiceReq/GetView", ApiConsumers.MosConsumer, reqFilter, ProcessLostToken, param);
                if (reqApiResult != null && reqApiResult.Count > 0)
                {
                    serviceReq = reqApiResult.FirstOrDefault();
                }

                HisSereServFilter ssFilter = new HisSereServFilter();
                ssFilter.IDs = new List<long> { (currentDataPrint.ALTER_SERE_SERV_ID ?? 0), currentDataPrint.SERE_SERV_ID };
                var ssApiResult = new BackendAdapter(param).Get<List<HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumers.MosConsumer, ssFilter, ProcessLostToken, param);
                if (ssApiResult != null && ssApiResult.Count > 0)
                {
                    sereServ.AddRange(ssApiResult);
                }

                long treatmentId = 0;
                if (serviceReq != null)
                {
                    treatmentId = serviceReq.TREATMENT_ID;
                }
                else if (sereServ != null && sereServ.Count > 0)
                {
                    treatmentId = sereServ.First().TDL_TREATMENT_ID ?? 0;
                }

                HisTreatmentViewFilter treaFilter = new HisTreatmentViewFilter();
                treaFilter.ID = treatmentId;
                var treatApiResult = new BackendAdapter(param).Get<List<V_HIS_TREATMENT>>("api/HisTreatment/GetView", ApiConsumers.MosConsumer, treaFilter, ProcessLostToken, param);
                if (treatApiResult != null && treatApiResult.Count > 0)
                {
                    treatment = treatApiResult.FirstOrDefault();
                }

                patienTypeAlter = new BackendAdapter(param).Get<V_HIS_PATIENT_TYPE_ALTER>("/api/HisPatientTypeAlter/GetViewLastByTreatmentId", ApiConsumers.MosConsumer, treatmentId, param);

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode(currentDataPrint.TDL_TREATMENT_CODE, printTypeCode, moduleData != null ? moduleData.RoomId : 0);

                MPS.Processor.Mps000433.PDO.Mps000433PDO pdo = new MPS.Processor.Mps000433.PDO.Mps000433PDO(treatment, patienTypeAlter, serviceReq, currentDataPrint, sereServ);

                WaitingManager.Hide();
                if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                }
                else
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO });
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
    }
}

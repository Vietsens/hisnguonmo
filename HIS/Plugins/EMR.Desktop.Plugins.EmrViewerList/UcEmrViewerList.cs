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
using Inventec.Desktop.Common.Message;
using Inventec.Core;
using HIS.Desktop.LocalStorage.ConfigApplication;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.Utils;
using DevExpress.XtraGrid.Columns;
using Inventec.Common.Controls.EditorLoader;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.BackendData;
using DevExpress.XtraRichEdit.Mouse;

namespace EMR.Desktop.Plugins.EmrViewerList
{
    public partial class UcEmrViewerList : HIS.Desktop.Utility.UserControlBase
    {
        private HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        private List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        private Inventec.Desktop.Common.Modules.Module moduleData;
        private System.Globalization.CultureInfo cultureLang;
        private int rowCount = 0;
        private int dataTotal = 0;
        private int startPage = 0;
        private ToolTipControlInfo lastInfo;
        private GridColumn lastColumn = null;
        private int lastRowHandle = -1;
        private bool IsRoomLt;
        private bool isNotLoadWhileChangeControlStateInFirst;

        enum TypeCancel
        {
            CancelApproval,
            CancelDisApproval
        }

        private string LogginName = "";
        private string RoomCode = "";

        public UcEmrViewerList()
        {
            InitializeComponent();
        }

        public UcEmrViewerList(Inventec.Desktop.Common.Modules.Module moduleData)
            : base(moduleData)
        {
            try
            {
                InitializeComponent();
                gridControlViewer.ToolTipController = this.toolTipController1;
                this.moduleData = moduleData;
                this.cultureLang = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                LogginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                var room = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.moduleData.RoomId);
                if (room != null)
                {
                    this.RoomCode = room.ROOM_CODE;
                }
                IsRoomLt = this.moduleData.RoomTypeId == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__LT;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UcEmrViewerList_Load(object sender, EventArgs e)
        {
            try
            {
                InitControlState();

                //Gan ngon ngu
                LoadKeysFromlanguage();

                //Gan gia tri mac dinh
                SetDefaultValueControl();

                //Load du lieu
                FillDataToGrid();

                // load combobox
                FillDataToGridLookupedit(cboRoomArchive, "ROOM_NAME", "ROOM_CODE", BackendDataWorker.Get<V_HIS_ROOM>()
                    .Where(o => o.ROOM_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__LT && o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList());

                TxtKeyword.Focus();
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
                WaitingManager.Show();
                int pagingSize = ucPaging.pagingGrid != null ? ucPaging.pagingGrid.PageSize : (int)ConfigApplications.NumPageSize;
                GridPaging(new CommonParam(0, pagingSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging.Init(GridPaging, param, pagingSize, this.gridControlViewer);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GridPaging(object param)
        {
            try   
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                ApiResultObject<List<EMR.EFMODEL.DataModels.V_EMR_VIEWER>> apiResult = null;
                EMR.Filter.EmrViewerViewFilter filter = new EMR.Filter.EmrViewerViewFilter();
                SetFilter(ref filter);
                gridViewViewer.BeginUpdate();
                apiResult = new Inventec.Common.Adapter.BackendAdapter
                    (paramCommon).GetRO<List<EMR.EFMODEL.DataModels.V_EMR_VIEWER>>
                    (EMR.URI.EmrViewer.GET_VIEW, HIS.Desktop.ApiConsumer.ApiConsumers.EmrConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                if (apiResult != null)
                {
                    var data = apiResult.Data;
                    if (data != null && data.Count > 0)
                    {
                        gridControlViewer.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                    else
                    {
                        gridControlViewer.DataSource = null;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                }
                else
                {
                    rowCount = 0;
                    dataTotal = 0;
                    gridControlViewer.DataSource = null;
                }
                gridViewViewer.EndUpdate();

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetFilter(ref Filter.EmrViewerViewFilter filter)
        {
            try
            {
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";

                if (DtTimeFrom.EditValue != null && DtTimeFrom.DateTime != DateTime.MinValue)
                    filter.CREATE_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(DtTimeFrom.DateTime.ToString("yyyyMMdd") + "000000");

                if (DtTimeTo.EditValue != null && DtTimeTo.DateTime != DateTime.MinValue)
                    filter.CREATE_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(DtTimeTo.DateTime.ToString("yyyyMMdd") + "235959");

                if (!String.IsNullOrEmpty(TxtKeyword.Text)) filter.KEY_WORD = TxtKeyword.Text.Trim();

                if (ChkYeuCau.Checked) filter.REQUEST_LOGINNAME__EXACT = LogginName;
                else filter.DATA_STORE_CODE__EXACT = RoomCode;

                if (cboRoomArchive.EditValue != null)
                {
                    filter.DATA_STORE_CODE__EXACT = cboRoomArchive.EditValue.ToString();
                }
                //if (!IsRoomLt) filter.REQUEST_LOGINNAME__EXACT = LogginName;
                //else filter.DATA_STORE_CODE__EXACT = RoomCode;

                if (ChkStt_YeuCau.Checked) filter.IS_APPROVAL = true;

                if (ChkStt_Duyet.Checked) filter.IS_DIS_APPROVAL = true;

                if (ChkStt_TuChoi.Checked) filter.IS_REQUEST = true;

                if (ChkStt_YeuCau.Checked == true)
                {
                    ChkStt_Duyet.Checked = false;
                    ChkStt_TuChoi.Checked = false;
                }
                else if (ChkStt_Duyet.Checked == true)
                {
                    ChkStt_TuChoi.Checked = false;
                    ChkStt_YeuCau.Checked = false;
                }
                else if (ChkStt_TuChoi.Checked == true)
                {
                    ChkStt_YeuCau.Checked = false;
                    ChkStt_Duyet.Checked = false;
                }
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
                //EMR.Filter.EmrViewerViewFilter filter = new EMR.Filter.EmrViewerViewFilter();
                ChkStt_YeuCau.Checked = false;
                ChkStt_Duyet.Checked = false;
                ChkStt_TuChoi.Checked = false;

                //Nếu phòng làm việc mở ra module không phải là "Tủ bệnh án" thì checkbox trên sẽ ở trạng thái tích chọn
                //ChkYeuCau.Enabled = this.moduleData.RoomTypeId == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__LT;
               //ChkYeuCau.Checked = this.moduleData.RoomTypeId != IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__LT;

                DtTimeFrom.DateTime = DateTime.Now;
                DtTimeTo.DateTime = DateTime.Now;
                TxtKeyword.Text = "";
                TxtKeyword.Focus();
                TxtKeyword.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadKeysFromlanguage()
        {
            try
            {
                this.BarCreateTime.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_VIEWER_LIST__BAR_CREATE_TIME");
                this.BarStt.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_VIEWER_LIST__BAR_STT");
                this.BarYeuCau.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_VIEWER_LIST__BAR_YEU_CAU");
                this.BtnRefresh.Text = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_VIEWER_LIST__BTN_REFRESH");
                this.BtnSearch.Text = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_VIEWER_LIST__BTN_SEARCH");
                this.ChkStt_TuChoi.Text = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_VIEWER_LIST__CHK_STT_TU_CHOI");
                this.ChkStt_Duyet.Text = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_VIEWER_LIST__CHK_STT_DUYET");
                this.ChkStt_YeuCau.Text = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_VIEWER_LIST__CHK_STT_YEU_CAU");
                this.ChkYeuCau.Text = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_VIEWER_LIST__CHK_YEU_CAU");
                this.Gc_Approval.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_VIEWER_LIST__GC_APPROVAL");
                this.Gc_ApprovalTime.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_VIEWER_LIST__GC_APPROVAL_TIME");
                this.Gc_ApprovalUsername.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_VIEWER_LIST__GC_APPROVAL_USERNAME");
                this.Gc_ClinicalInTime.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_VIEWER_LIST__GC_CLINICAL_IN_TIME");
                this.Gc_DepartmentName.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_VIEWER_LIST__GC_DEPARTMENT_NAME");
                this.Gc_DisApproval.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_VIEWER_LIST__GC_DIS_APPROVAL");
                this.Gc_Dob.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_VIEWER_LIST__GC_DOB");
                this.Gc_FinishTime.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_VIEWER_LIST__GC_FINISH_TIME");
                this.Gc_Gender.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_VIEWER_LIST__GC_GENDER");
                this.Gc_HeinCardNumber.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_VIEWER_LIST__GC_HEIN_CAR_NUMBER");
                this.Gc_IcdCode.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_VIEWER_LIST__GC_ICD_CODE");
                this.Gc_IcdName.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_VIEWER_LIST__GC_ICD_NAME");
                this.Gc_IcdText.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_VIEWER_LIST__GC_ICD_TEXT");
                this.Gc_InTime.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_VIEWER_LIST__GC_IN_TIME");
                this.Gc_OutTime.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_VIEWER_LIST__GC_OUT_TIME");
                this.Gc_PatientCode.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_VIEWER_LIST__GC_PATIENT_CODE");
                this.Gc_PatientName.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_VIEWER_LIST__GC_PATIENT_NAME");
                this.Gc_RequestFinishTime.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_VIEWER_LIST__GC_REQUEST_FINISH_TIME");
                this.Gc_RequestName.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_VIEWER_LIST__GC_REQUEST_NAME");
                this.Gc_Stt.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_VIEWER_LIST__GC_STT");
                this.Gc_TrangThai.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_VIEWER_LIST__GC_TRANG_THAI");
                this.Gc_TreatmentCode.Caption = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_VIEWER_LIST__GC_TREATMENT_CODE");
                this.LciTimeFrom.Text = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_VIEWER_LIST__LCI_TIME_FROM");
                this.LciTimeTo.Text = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_VIEWER_LIST__LCI_TIME_TO");
                this.repositoryItemBtnApproval.Buttons[0].ToolTip = this.repositoryItemBtnApprovalDisable.Buttons[0].ToolTip = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_VIEWER_LIST__RP_BTN_APPROVAL");
                this.repositoryItemBtnDisApproval.Buttons[0].ToolTip = this.repositoryItemBtnDisApprovalDisable.Buttons[0].ToolTip = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_VIEWER_LIST__RP_BTN_DIS_APPROVAL");
                this.TxtKeyword.Properties.NullValuePrompt = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_VIEWER_LIST__TXT_KEYWORD_NULL_VALUE");

                this.repositoryItemBtnCancelApproval.Buttons[0].ToolTip = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_VIEWER_LIST__RP_BTN_CANCEL_APPROVAL");
                this.repositoryItemBtnCancelDisApproval.Buttons[0].ToolTip = GetLanguageControl("IVT_LANGUAGE_KEY__UC_EMR_VIEWER_LIST__RP_BTN_CANCEL_DIS_APPROVAL");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string GetLanguageControl(string key)
        {
            return Inventec.Common.Resource.Get.Value(key, Resources.ResourceLanguageManager.LanguageResource, cultureLang);
        }

        private void repositoryItemBtnApproval_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (EMR.EFMODEL.DataModels.V_EMR_VIEWER)gridViewViewer.GetFocusedRow();
                if (row != null)
                {
                    List<object> _listObj = new List<object>();
                    _listObj.Add(row.ID);//truyền vào V_EMR_VIEWER id
                    _listObj.Add((HIS.Desktop.Common.DelegateRefreshData)Refreshs);

                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("EMR.Desktop.Plugins.EmrApproveViewer", moduleData.RoomId, moduleData.RoomTypeId, _listObj);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemBtnDisApproval_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (EMR.EFMODEL.DataModels.V_EMR_VIEWER)gridViewViewer.GetFocusedRow();
                if (row != null)
                {
                    if (DevExpress.XtraEditors.XtraMessageBox.Show(
                        Resources.ResourceLanguageManager.HeThongTBCuaSoThongBaoBanCoMuonTuChoiKhong,
                        Resources.ResourceLanguageManager.ThongBao,
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        CommonParam param = new CommonParam();
                        bool success = false;
                        var apiResult = new Inventec.Common.Adapter.BackendAdapter(param).Post<EMR.EFMODEL.DataModels.V_EMR_VIEWER>(EMR.URI.EmrViewer.REJECT, HIS.Desktop.ApiConsumer.ApiConsumers.EmrConsumer, row, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                        if (apiResult != null)
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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewViewer_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    EMR.EFMODEL.DataModels.V_EMR_VIEWER data = (EMR.EFMODEL.DataModels.V_EMR_VIEWER)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;

                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1 + startPage;
                        }
                        else if (e.Column.FieldName == "DOB_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.DOB);
                            if (data.IS_HAS_NOT_DAY_DOB == 1)
                            {
                                e.Value = data.DOB.ToString().Substring(0, 4);
                            }
                        }
                        else if (e.Column.FieldName == "IN_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.IN_TIME);
                        }
                        else if (e.Column.FieldName == "CLINICAL_IN_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CLINICAL_IN_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "OUT_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.OUT_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "APPROVAL_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.APPROVAL_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "REQUEST_FINISH_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.REQUEST_FINISH_TIME);
                        }
                        else if (e.Column.FieldName == "FINISH_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.FINISH_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "STATUS")// trạng thái
                        {
                            if (!data.APPROVAL_TIME.HasValue && !data.REJECT_TIME.HasValue) //yeu cau
                            {
                                e.Value = imageListStt.Images[1];
                            }
                            else if (data.REJECT_TIME.HasValue) // tu choi duyet
                            {
                                e.Value = imageListStt.Images[2];
                            }
                            else if (data.APPROVAL_TIME.HasValue) // duyet
                            {
                                e.Value = imageListStt.Images[3];
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

        private void gridViewViewer_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView View = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    string approval = (View.GetRowCellValue(e.RowHandle, "APPROVAL_TIME") ?? "").ToString();
                    string reject = (View.GetRowCellValue(e.RowHandle, "REJECT_TIME") ?? "").ToString();
                    string DATA_STORE_CODE = (View.GetRowCellValue(e.RowHandle, "DATA_STORE_CODE") ?? "").ToString();

                    if (e.Column.FieldName == "APPROVAL")
                    {
                        if (this.moduleData.RoomTypeId == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__LT
                            && RoomCode == DATA_STORE_CODE)
                        {
                            if (String.IsNullOrWhiteSpace(reject) && String.IsNullOrWhiteSpace(approval))
                            {
                                e.RepositoryItem = repositoryItemBtnApproval;
                            }
                            else if (!String.IsNullOrWhiteSpace(approval))
                            {
                                e.RepositoryItem = repositoryItemBtnCancelApproval;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemBtnApprovalDisable;
                            }
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemBtnApprovalDisable;
                        }
                    }
                    else if (e.Column.FieldName == "DIS_APPROVAL")
                    {
                        if (this.moduleData.RoomTypeId == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__LT
                            && RoomCode == DATA_STORE_CODE)
                        {
                            if (String.IsNullOrWhiteSpace(reject) && String.IsNullOrWhiteSpace(approval))
                            {
                                e.RepositoryItem = repositoryItemBtnDisApproval;
                            }
                            else if (!String.IsNullOrWhiteSpace(reject))
                            {
                                e.RepositoryItem = repositoryItemBtnCancelDisApproval;
                            }
                            else
                            {
                                e.RepositoryItem = repositoryItemBtnDisApprovalDisable;
                            }
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemBtnDisApprovalDisable;
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
                if (e.Info == null && e.SelectedControl == gridControlViewer)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = gridControlViewer.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
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
                                string approval = (view.GetRowCellValue(lastRowHandle, "APPROVAL_TIME") ?? "").ToString();
                                string reject = (view.GetRowCellValue(lastRowHandle, "REJECT_TIME") ?? "").ToString();

                                if (!String.IsNullOrWhiteSpace(approval))
                                {
                                    text = Resources.ResourceLanguageManager.Duyet;
                                }
                                else if (!String.IsNullOrWhiteSpace(reject))
                                {
                                    text = Resources.ResourceLanguageManager.TuChoiDuyet;
                                }
                                else if (String.IsNullOrWhiteSpace(approval) && String.IsNullOrWhiteSpace(reject))
                                {
                                    text = Resources.ResourceLanguageManager.YeuCau;
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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                BtnSearch.Focus();
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                BtnRefresh.Focus();
                SetDefaultValueControl();
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #region Public method
        public void Search()
        {
            try
            {
                BtnSearch_Click(null, null);
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
                BtnRefresh_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void repositoryItemBtnCancelApproval_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (EMR.EFMODEL.DataModels.V_EMR_VIEWER)gridViewViewer.GetFocusedRow();
                if (row != null)
                {
                    UpdateDataViewer(row, TypeCancel.CancelApproval);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemBtnCancelDisApproval_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (EMR.EFMODEL.DataModels.V_EMR_VIEWER)gridViewViewer.GetFocusedRow();
                if (row != null)
                {
                    UpdateDataViewer(row, TypeCancel.CancelDisApproval);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UpdateDataViewer(EMR.EFMODEL.DataModels.V_EMR_VIEWER row, TypeCancel type)
        {
            try
            {
                if (row != null)
                {
                    var data = new EMR.EFMODEL.DataModels.EMR_VIEWER();
                    Inventec.Common.Mapper.DataObjectMapper.Map<EMR.EFMODEL.DataModels.EMR_VIEWER>(data, row);
                    if (type == TypeCancel.CancelApproval)
                    {
                        data.APPROVAL_TIME = null;
                        data.APPROVAL_LOGINNAME = null;
                        data.APPROVAL_USERNAME = null;
                        data.FINISH_TIME = null;
                    }
                    else if (type == TypeCancel.CancelDisApproval)
                    {
                        data.REJECT_TIME = null;
                    }

                    CommonParam param = new CommonParam();
                    bool success = false;
                    var apiResult = new Inventec.Common.Adapter.BackendAdapter(param).Post<EMR.EFMODEL.DataModels.EMR_VIEWER>(EMR.URI.EmrViewer.UPDATE, HIS.Desktop.ApiConsumer.ApiConsumers.EmrConsumer, data, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    if (apiResult != null)
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
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void FillDataToGridLookupedit(DevExpress.XtraEditors.GridLookUpEdit cboEditor, string displayMember, string valueMember, object datasource)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo(displayMember, "", 200, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO(displayMember, valueMember, columnInfos, false, 100);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(cboEditor, datasource, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }

        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void ChkYeuCau_CheckedChanged(object sender, EventArgs e)
        {
            HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) 
                ? this.currentControlStateRDO.Where(o => o.KEY == ChkYeuCau.Name && o.MODULE_LINK == this.moduleData.ModuleLink).FirstOrDefault() : null;
            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
            if (csAddOrUpdate != null)
            {
                csAddOrUpdate.VALUE = (ChkYeuCau.Checked ? "1" : "");
            }
            else
            {
                csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                csAddOrUpdate.KEY = ChkYeuCau.Name;
                csAddOrUpdate.VALUE = (ChkYeuCau.Checked ? "1" : "");
                csAddOrUpdate.MODULE_LINK = this.moduleData.ModuleLink;
                if (this.currentControlStateRDO == null)
                    this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                this.currentControlStateRDO.Add(csAddOrUpdate);
            }
            this.controlStateWorker.SetData(this.currentControlStateRDO);
        }

        private void InitControlState()
        {
            isNotLoadWhileChangeControlStateInFirst = true;
            try
            {
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(this.moduleData.ModuleLink);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == ChkYeuCau.Name)
                        {
                            ChkYeuCau.Checked = item.VALUE == "1";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            isNotLoadWhileChangeControlStateInFirst = false;
        }

        private void cboRoomArchive_Properties_Click(object sender, EventArgs e)
        {
            
        }

        private void cboRoomArchive_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
            {
                cboRoomArchive.EditValue = null;
            }
            
        }

        private void ChkStt_YeuCau_CheckedChanged(object sender, EventArgs e)
        {
            if (ChkStt_YeuCau.Checked)
            {
                ChkStt_Duyet.Checked = false;
                ChkStt_TuChoi.Checked = false;
            }
        }

        private void ChkStt_Duyet_CheckedChanged(object sender, EventArgs e)
        {
            if (ChkStt_Duyet.Checked)
            {
                ChkStt_YeuCau.Checked = false;
                ChkStt_TuChoi.Checked = false;
            }
        }

        private void ChkStt_TuChoi_CheckedChanged(object sender, EventArgs e)
        {
            if (ChkStt_TuChoi.Checked)
            {
                ChkStt_YeuCau.Checked = false;
                ChkStt_Duyet.Checked = false;
            }
        }
    }
}

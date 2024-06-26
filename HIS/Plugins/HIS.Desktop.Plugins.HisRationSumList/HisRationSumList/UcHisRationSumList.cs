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
using Inventec.Desktop.Common.Modules;
using Inventec.Common.Logging;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.LocalStorage.LocalData;
using MOS.SDO;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.Controls.EditorLoader;

namespace HIS.Desktop.Plugins.HisRationSumList.HisRationSumList
{
    public partial class UcHisRationSumList : HIS.Desktop.Utility.UserControlBase
    {
        #region ---Declare variable---
        Module ModuleCurrent;
        int Rowcount = 0;
        int DataTotal = 0;
        int StartPage = 0;
        List<V_HIS_RATION_SUM> listRationSumSelect;
        List<HIS_RATION_TIME> lstRationTime;
        #endregion

        #region ---Constructor---
        public UcHisRationSumList()
        {
            InitializeComponent();
        }

        public UcHisRationSumList(Module module)
        {
            try
            {
                InitializeComponent();
                this.ModuleCurrent = module;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region ---Methdod--
        #region ---Public method---
        public void Search()
        {
            try
            {
                if (btnRefresh.Enabled)
                    btnSearch_Click_1(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void Refreshs()
        {
            try
            {
                if (btnSearch.Enabled)
                    btnRefresh_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void Print()
        {
            try
            {
                if (btnPrint.Enabled)
                    btnPrint_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region ---Private method---
        private void UcHisRationSumList_Load(object sender, EventArgs e)
        {
            try
            {
                InitComboRationTime();
                //Set du lieu mac dinh
                SetDataDefault();
                //Set key ngon ngu
                SetCationByLanguageKey();
                //Do du lieu vao gridcontrol
                FillDataToGridcontrol();

            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void FillDataToGridcontrol()
        {
            try
            {
                WaitingManager.Show();
                int NumPage = 0;
                if (ucPaging.pagingGrid != null)
                {
                    NumPage = ucPaging.pagingGrid.PageSize;
                }
                else
                    NumPage = HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplicationWorker.Get<int>("CONFIG_KEY__NUM_PAGESIZE");
                LoadData(new CommonParam(0, NumPage));
                CommonParam param = new CommonParam();
                param.Limit = Rowcount;
                param.Count = DataTotal;
                ucPaging.Init(LoadData, param, NumPage, gridControlRationSumList);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }

        private void LoadData(object data)
        {
            try
            {
                int limit = ((CommonParam)data).Limit ?? 0;
                StartPage = ((CommonParam)data).Start ?? 0;
                CommonParam param = new CommonParam(StartPage, limit);
                ApiResultObject<List<V_HIS_RATION_SUM>> result = null;
                HisRationSumViewFilter filter = new HisRationSumViewFilter();
                setFilter(ref filter);
                gridViewRationSumList.BeginUpdate();
                result = new BackendAdapter(param).GetRO<List<V_HIS_RATION_SUM>>(HisRequestUriStore.Ration_Sum_GetView, ApiConsumers.MosConsumer, filter, param);
                if (result != null)
                {
                    var DataResult = ((List<V_HIS_RATION_SUM>)result.Data);
                    if (DataResult != null && DataResult.Count > 0)
                    {
                        gridControlRationSumList.DataSource = DataResult;
                        this.Rowcount = (DataResult == null ? 0 : DataResult.Count);
                        this.DataTotal = (result.Param == null ? 0 : result.Param.Count ?? 0);
                    }
                    else
                    {
                        gridControlRationSumList.DataSource = null;
                    }
                }
                gridViewRationSumList.EndUpdate();

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
            }
        }

        private void setFilter(ref HisRationSumViewFilter filter)
        {
            try
            {
                filter.ORDER_DIRECTION = "DESC";
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.KEY_WORD = txtKeyWork.Text.Trim();
                if (dtApprovalDateFrom.EditValue != null)
                {
                    filter.APPROVAL_DATE_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                            Convert.ToDateTime(dtApprovalDateFrom.EditValue).ToString("yyyyMMdd") + "000000");

                }
                if (dtApprovalDateTo.EditValue != null)
                {
                    filter.APPROVAL_DATE_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                            Convert.ToDateTime(dtApprovalDateTo.EditValue).ToString("yyyyMMdd") + "235959");
                }
                if (dtRequsetTimeFrom.EditValue != null)
                {
                    filter.REQ_DATE_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                            Convert.ToDateTime(dtRequsetTimeFrom.EditValue).ToString("yyyyMMdd") + "000000");
                }
                if (dtRequestTimeTo.EditValue != null)
                {
                    filter.REQ_DATE_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                            Convert.ToDateTime(dtRequestTimeTo.EditValue).ToString("yyyyMMdd") + "235959");
                }
                List<long> StatusID = new List<long>();
                if (chkApprove.Checked)
                {
                    StatusID.Add(IMSys.DbConfig.HIS_RS.HIS_RATION_SUM_STT.ID__APPROVAL);
                }
                if (chkReject.Checked)
                {
                    StatusID.Add(IMSys.DbConfig.HIS_RS.HIS_RATION_SUM_STT.ID__REJECT);
                }
                if (chkRequest.Checked)
                {
                    StatusID.Add(IMSys.DbConfig.HIS_RS.HIS_RATION_SUM_STT.ID__REQ);
                }
                if (StatusID != null && StatusID.Count > 0)
                {
                    filter.RATION_SUM_STT_IDs = StatusID;
                }
                if (cboRation.EditValue != null)
                    filter.RATION_TIME_IDs = new List<long>() { (long)cboRation.EditValue };
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        private void InitComboRationTime()
        {
            try
            {
                CommonParam param = new CommonParam();

                HisRationTimeFilter filter = new HisRationTimeFilter();
                filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                lstRationTime = new BackendAdapter(param).Get<List<HIS_RATION_TIME>>("api/HisRationTime/Get", ApiConsumers.MosConsumer, filter, param);

                List<ColumnInfo> colum = new List<ColumnInfo>();
                colum.Add(new ColumnInfo("RATION_TIME_CODE", "Mã", 100, 1));
                colum.Add(new ColumnInfo("RATION_TIME_NAME", "Tên", 250, 2));
                ControlEditorADO controlEditADO = new ControlEditorADO("RATION_TIME_NAME", "ID", colum, true, 350);
                ControlEditorLoader.Load(cboRation, lstRationTime, controlEditADO);
                cboRation.Properties.ImmediatePopup = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        private void SetCationByLanguageKey()
        {
            try
            {
                Resources.ResourceLanguageManager.LanguageResource = new System.Resources.ResourceManager("HIS.Desktop.Plugins.HisRationSumList.Resources.Lang", typeof(HIS.Desktop.Plugins.HisRationSumList.HisRationSumList.UcHisRationSumList).Assembly);
                this.txtKeyWork.Properties.NullValuePrompt = SetKey("UcHisRationSumList.txtKeyWork.Properties.NullValuePrompt");
                this.lcApprovalDateFrom.Text = SetKey("UcHisRationSumList.lcApprovalDateFrom.Text");
                this.lcApprovalDateTo.Text = SetKey("UcHisRationSumList.lcApprovalDateTo.Text");
                this.lcRequestTimeFrom.Text = SetKey("UcHisRationSumList.lcRequestTimeFrom.Text");
                this.lcRequestTimeTo.Text = SetKey("UcHisRationSumList.lcRequestTimeTo.Text");
                this.navBarGroupRequester.Caption = SetKey("UcHisRationSumList.navBarGroupRequester.Caption");
                this.navBarGroupStatus.Caption = SetKey("UcHisRationSumList.navBarGroupStatus.Caption");
                this.navBarGroupApprove.Caption = SetKey("UcHisRationSumList.navBarGroupApprove.Caption");
                this.chkApprove.Text = SetKey("UcHisRationSumList.chkApprove.Text");
                this.chkReject.Text = SetKey("UcHisRationSumList.chkReject.Text");
                this.chkRequest.Text = SetKey("UcHisRationSumList.chkRequest.Text");
                this.btnPrint.Text = SetKey("UcHisRationSumList.btnPrint.Text");
                this.btnRefresh.Text = SetKey("UcHisRationSumList.btnRefresh.Text");
                this.btnSearch.Text = SetKey("UcHisRationSumList.btnSearch.Text");
                this.grdColRationSumCode.Caption = SetKey("UcHisRationSumList.grdColRationSumCode.Caption");
                this.grdColRoomName.Caption = SetKey("UcHisRationSumList.grdColRoomName.Caption");
                this.grdColDepartmentName.Caption = SetKey("UcHisRationSumList.grdColDepartmentName.Caption");
                this.grdColPertitioner.Caption = SetKey("UcHisRationSumList.grdColPertitioner.Caption");
                this.grdcolApprover.Caption = SetKey("UcHisRationSumList.grdcolApprover.Caption");
                this.grdColApprovalTime.Caption = SetKey("UcHisRationSumList.grdColApprovalTime.Caption");
                this.grdColCreateTime.Caption = SetKey("UcHisRationSumList.grdColCreateTime.Caption");
                this.grdColCreator.Caption = SetKey("UcHisRationSumList.grdColCreator.Caption");
                this.grdColModifytime.Caption = SetKey("UcHisRationSumList.grdColModifytime.Caption");
                this.grdColModifier.Caption = SetKey("UcHisRationSumList.grdColModifier.Caption");

            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private string SetKey(string key)
        {
            string Result = "";
            try
            {
                Result = Inventec.Common.Resource.Get.Value(key, Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Result = "";
                LogSystem.Warn(ex);
            }
            return Result;
        }

        private void SetDataDefault()
        {
            try
            {
                txtKeyWork.Text = "";
                dtRequsetTimeFrom.DateTime = DateTime.Now;
                dtRequestTimeTo.DateTime = DateTime.Now;
                dtApprovalDateFrom.EditValue = null;
                dtApprovalDateTo.EditValue = null;
                cboRation.EditValue = null;
                chkApprove.Checked = false;
                chkReject.Checked = false;
                chkRequest.Checked = false;
                btnPrint.Enabled = false;
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private object StringDisplay(string loginame, string username)
        {
            string Result = "";
            try
            {
                if (string.IsNullOrEmpty(loginame) && string.IsNullOrEmpty(username))
                    Result = "";
                else
                {
                    if (!string.IsNullOrEmpty(loginame) && !string.IsNullOrEmpty(username))
                    {
                        Result = string.Format("{0} - {1}", loginame, username);
                    }
                    else if (!string.IsNullOrEmpty(loginame) && string.IsNullOrEmpty(username))
                        Result = loginame;
                    else
                        Result = username;
                }
            }
            catch (Exception ex)
            {
                Result = "";
                LogSystem.Warn(ex);
            }
            return Result;
        }
        #endregion
        #endregion

        #region ---Even---
        #region ---Click---
        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.listRationSumSelect != null && this.listRationSumSelect.Count > 0)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(listRationSumSelect);
                    if (this.ModuleCurrent != null)
                    {
                        CallModule callModule = new CallModule(CallModule.HisRationSumPrint, this.ModuleCurrent.RoomId, this.ModuleCurrent.RoomTypeId, listArgs);
                    }
                    else
                    {
                        CallModule callModule = new CallModule(CallModule.HisRationSumPrint, 0, 0, listArgs);
                    }
                }
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                gridViewRationSumList.ClearSelection();
                SetDataDefault();
                FillDataToGridcontrol();
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void btnSearch_Click_1(object sender, EventArgs e)
        {
            try
            {
                this.listRationSumSelect = new List<V_HIS_RATION_SUM>();
                btnPrint.Enabled = false;
                FillDataToGridcontrol();
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if ((MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question)) == DialogResult.Yes)
                {
                    bool result = false;
                    WaitingManager.Show();
                    CommonParam param = new CommonParam();
                    var datarow = (V_HIS_RATION_SUM)gridViewRationSumList.GetFocusedRow();

                    if (datarow != null)
                    {
                        HisRationSumSDO dataUpdate = new HisRationSumSDO();
                        dataUpdate.Id = datarow.ID;
                        dataUpdate.RoomId = this.ModuleCurrent.RoomId;
                        result = new BackendAdapter(param).Post<bool>(HisRequestUriStore.Ration_Sum_Delete, ApiConsumers.MosConsumer, dataUpdate, param);
                        if (result)
                        {
                            FillDataToGridcontrol();
                        }
                    }
                    MessageManager.Show(this.ParentForm, param, result);
                    WaitingManager.Hide();
                }

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }
        #endregion

        #region ---Even Gridview---
        private void gridViewRationSumList_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    V_HIS_RATION_SUM datarow = (V_HIS_RATION_SUM)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (datarow != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + StartPage + 1;
                        }
                        else if (e.Column.FieldName == "REQ_STR")
                        {
                            string loginame = datarow.REQ_LOGINNAME;
                            string username = datarow.REQ_USERNAME;
                            e.Value = StringDisplay(loginame, username);
                        }
                        else if (e.Column.FieldName == "APPROVAL_STR")
                        {
                            string loginame = datarow.APPROVAL_LOGINNAME;
                            string username = datarow.APPROVAL_USERNAME;
                            e.Value = StringDisplay(loginame, username);
                        }
                        else if (e.Column.FieldName == "APPROVAL_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(datarow.APPROVAL_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(datarow.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(datarow.MODIFY_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "STATUS")
                        {
                            if (datarow.RATION_SUM_STT_ID == IMSys.DbConfig.HIS_RS.HIS_RATION_SUM_STT.ID__APPROVAL)
                                e.Value = imageListStatus.Images[0];
                            else if (datarow.RATION_SUM_STT_ID == IMSys.DbConfig.HIS_RS.HIS_RATION_SUM_STT.ID__REQ)
                                e.Value = imageListStatus.Images[1];
                            else
                                e.Value = imageListStatus.Images[2];
                        }
                        else if (e.Column.FieldName == "MAX_INTRUCTION_DATE_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(datarow.MAX_INTRUCTION_DATE ?? 0);
                        }
                        else if (e.Column.FieldName == "RATION_TIME_NAMES")
                        {
                            if (!string.IsNullOrEmpty(datarow.RATION_TIME_IDS) && lstRationTime != null && lstRationTime.Count > 0)
                            {
                                var arrRatioTime = datarow.RATION_TIME_IDS.Split(',');
                                List<string> names = new List<string>();
                                foreach (var item in arrRatioTime)
                                {
                                    var rationTime = lstRationTime.FirstOrDefault(o => o.ID == Int64.Parse(item));
                                    if (rationTime != null)
                                        names.Add(rationTime.RATION_TIME_NAME);
                                }
                                if(names.Count > 0)
                                    e.Value = string.Join(",", names);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void gridViewRationSumList_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;

                if (e.RowHandle >= 0)
                {
                    V_HIS_RATION_SUM datarow = (V_HIS_RATION_SUM)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (datarow != null)
                    {
                        if (e.Column.FieldName == "DELETE")
                        {
                            var loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                            if ((loginName.ToUpper().Trim() == datarow.CREATOR.ToUpper().Trim() || HIS.Desktop.IsAdmin.CheckLoginAdmin.IsAdmin(loginName)) && (datarow.RATION_SUM_STT_ID == IMSys.DbConfig.HIS_RS.HIS_RATION_SUM_STT.ID__REQ || datarow.RATION_SUM_STT_ID == IMSys.DbConfig.HIS_RS.HIS_RATION_SUM_STT.ID__REJECT))
                                e.RepositoryItem = btnDelete;
                            else
                                e.RepositoryItem = btnDisibleDelete;
                        }
                        if (e.Column.FieldName == "APPROVAL")
                        {

                            if (datarow.RATION_SUM_STT_ID == IMSys.DbConfig.HIS_RS.HIS_RATION_SUM_STT.ID__APPROVAL && this.ModuleCurrent.RoomId == datarow.ROOM_ID)
                            {
                                e.RepositoryItem = btnCancalApproval;
                            }
                            else if (datarow.RATION_SUM_STT_ID == IMSys.DbConfig.HIS_RS.HIS_RATION_SUM_STT.ID__REQ && this.ModuleCurrent.RoomId == datarow.ROOM_ID)
                            {
                                e.RepositoryItem = btnApproval;
                            }
                            else
                                e.RepositoryItem = btnDisbaleApprove;

                        }
                        if (e.Column.FieldName == "APPROVAL_REJECT")
                        {
                            if (datarow.RATION_SUM_STT_ID == IMSys.DbConfig.HIS_RS.HIS_RATION_SUM_STT.ID__REQ && this.ModuleCurrent.RoomId == datarow.ROOM_ID)
                            {
                                e.RepositoryItem = btnApprovalReject;
                            }
                            else if (datarow.RATION_SUM_STT_ID == IMSys.DbConfig.HIS_RS.HIS_RATION_SUM_STT.ID__REJECT && this.ModuleCurrent.RoomId == datarow.ROOM_ID)
                            {
                                e.RepositoryItem = btnCancalApproveReject;
                            }
                            else
                                e.RepositoryItem = btnDisbaleCancalApproval;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                LogSystem.Error(ex);
            }
        }

        private void gridViewRationSumList_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            try
            {
                this.listRationSumSelect = new List<V_HIS_RATION_SUM>();
                int[] rowHandles = gridViewRationSumList.GetSelectedRows();
                if (rowHandles != null && rowHandles.Count() > 0)
                {
                    foreach (var i in rowHandles)
                    {
                        var rationsum = (V_HIS_RATION_SUM)gridViewRationSumList.GetRow(i);
                        if (rationsum != null)
                            this.listRationSumSelect.Add(rationsum);
                    }
                }
                if (this.listRationSumSelect != null && this.listRationSumSelect.Count > 0)
                {
                    btnPrint.Enabled = true;
                }
                else
                    btnPrint.Enabled = false;
            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }

        private void gridViewRationSumList_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {

                if ((Control.ModifierKeys & Keys.Control) != Keys.Control)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    GridHitInfo hi = view.CalcHitInfo(e.Location);
                    if (hi.InRowCell)
                    {
                        var RationSum = (V_HIS_RATION_SUM)gridViewRationSumList.GetRow(hi.RowHandle);
                        if (hi.Column.FieldName == "SEE_DETAIL")
                        {
                            btnSeeDetail_ButtonClick(RationSum);
                        }
                        if (hi.Column.FieldName == "APPROVAL")
                        {
                            if (RationSum.RATION_SUM_STT_ID == IMSys.DbConfig.HIS_RS.HIS_RATION_SUM_STT.ID__APPROVAL && this.ModuleCurrent.RoomId == RationSum.ROOM_ID)
                            {
                                btnCancalApproval_ButtonClick(RationSum);
                            }
                            else if (RationSum.RATION_SUM_STT_ID == IMSys.DbConfig.HIS_RS.HIS_RATION_SUM_STT.ID__REQ && this.ModuleCurrent.RoomId == RationSum.ROOM_ID)
                            {
                                btnApproval_ButtonClick(RationSum);
                            }
                        }
                        if (hi.Column.FieldName == "APPROVAL_REJECT")
                        {
                            if (RationSum.RATION_SUM_STT_ID == IMSys.DbConfig.HIS_RS.HIS_RATION_SUM_STT.ID__REQ && this.ModuleCurrent.RoomId == RationSum.ROOM_ID)
                            {
                                btnApprovalReject_ButtonClick(RationSum);
                            }
                            else if (RationSum.RATION_SUM_STT_ID == IMSys.DbConfig.HIS_RS.HIS_RATION_SUM_STT.ID__REJECT && this.ModuleCurrent.RoomId == RationSum.ROOM_ID)
                            {
                                btnCancalApproveReject_ButtonClick(RationSum);
                            }
                        }
                        if (hi.Column.FieldName == "DELETE")
                        {
                            var loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                            if ((loginName.ToUpper().Trim() == RationSum.CREATOR.ToUpper().Trim() || HIS.Desktop.IsAdmin.CheckLoginAdmin.IsAdmin(loginName)) && (RationSum.RATION_SUM_STT_ID == IMSys.DbConfig.HIS_RS.HIS_RATION_SUM_STT.ID__REQ || RationSum.RATION_SUM_STT_ID == IMSys.DbConfig.HIS_RS.HIS_RATION_SUM_STT.ID__REJECT))
                                btnDelete_Click(RationSum);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region ---ButtonClick---
        private void btnSeeDetail_ButtonClick(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                var dataRow = (V_HIS_RATION_SUM)gridViewRationSumList.GetFocusedRow();
                if (dataRow != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(dataRow);
                    if (this.ModuleCurrent != null)
                    {
                        CallModule callModule = new CallModule(CallModule.HisMealRationDetail, this.ModuleCurrent.RoomId, this.ModuleCurrent.RoomTypeId, listArgs);
                    }
                    else
                    {
                        CallModule callModule = new CallModule(CallModule.HisMealRationDetail, 0, 0, listArgs);
                    }
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Warn(ex);
            }
        }

        private void btnApproval_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                bool success = false;
                CommonParam param = new CommonParam();
                var dataRow = (V_HIS_RATION_SUM)gridViewRationSumList.GetFocusedRow();
                if (dataRow != null)
                {
                    HisRationSumUpdateSDO dataUpdate = new HisRationSumUpdateSDO();
                    dataUpdate.RationSumId = dataRow.ID;
                    dataUpdate.WorkingRoomId = this.ModuleCurrent.RoomId;
                    var Result = new BackendAdapter(param).Post<HIS_RATION_SUM>(HisRequestUriStore.Ration_Sum_Approve, ApiConsumers.MosConsumer, dataUpdate, param);
                    if (Result != null)
                    {
                        success = true;
                        FillDataToGridcontrol();
                        this.listRationSumSelect = new List<V_HIS_RATION_SUM>();
                        btnPrint.Enabled = false;
                    }
                }
                WaitingManager.Hide();
                MessageManager.Show(this.ParentForm, param, success);

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }

        private void btnCancalApproval_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                bool success = false;
                CommonParam param = new CommonParam();
                var dataRow = (V_HIS_RATION_SUM)gridViewRationSumList.GetFocusedRow();
                if (dataRow != null)
                {
                    HisRationSumUpdateSDO dataUpdate = new HisRationSumUpdateSDO();
                    dataUpdate.RationSumId = dataRow.ID;
                    dataUpdate.WorkingRoomId = this.ModuleCurrent.RoomId;
                    var Result = new BackendAdapter(param).Post<HIS_RATION_SUM>(HisRequestUriStore.Ration_Sum_Unapprove, ApiConsumers.MosConsumer, dataUpdate, param);
                    if (Result != null)
                    {
                        success = true;
                        FillDataToGridcontrol();
                        this.listRationSumSelect = new List<V_HIS_RATION_SUM>();
                        btnPrint.Enabled = false;
                    }
                }
                WaitingManager.Hide();
                MessageManager.Show(this.ParentForm, param, success);

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }

        private void btnApprovalReject_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                bool success = false;
                CommonParam param = new CommonParam();
                var dataRow = (V_HIS_RATION_SUM)gridViewRationSumList.GetFocusedRow();
                if (dataRow != null)
                {
                    HisRationSumUpdateSDO dataUpdate = new HisRationSumUpdateSDO();
                    dataUpdate.RationSumId = dataRow.ID;
                    dataUpdate.WorkingRoomId = this.ModuleCurrent.RoomId;
                    var Result = new BackendAdapter(param).Post<HIS_RATION_SUM>(HisRequestUriStore.Ration_Sum_Reject, ApiConsumers.MosConsumer, dataUpdate, param);
                    if (Result != null)
                    {
                        success = true;
                        FillDataToGridcontrol();
                        this.listRationSumSelect = new List<V_HIS_RATION_SUM>();
                        btnPrint.Enabled = false;
                    }
                }
                WaitingManager.Hide();
                MessageManager.Show(this.ParentForm, param, success);

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }

        private void btnCancalApproveReject_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                bool success = false;
                CommonParam param = new CommonParam();
                var dataRow = (V_HIS_RATION_SUM)gridViewRationSumList.GetFocusedRow();
                if (dataRow != null)
                {
                    HisRationSumUpdateSDO dataUpdate = new HisRationSumUpdateSDO();
                    dataUpdate.RationSumId = dataRow.ID;
                    dataUpdate.WorkingRoomId = this.ModuleCurrent.RoomId;
                    var Result = new BackendAdapter(param).Post<HIS_RATION_SUM>(HisRequestUriStore.Ration_Sum_Unreject, ApiConsumers.MosConsumer, dataUpdate, param);
                    if (Result != null)
                    {
                        success = true;
                        FillDataToGridcontrol();
                        this.listRationSumSelect = new List<V_HIS_RATION_SUM>();
                        btnPrint.Enabled = false;
                    }
                }
                WaitingManager.Hide();
                MessageManager.Show(this.ParentForm, param, success);

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }
        #region
        private void btnSeeDetail_ButtonClick(V_HIS_RATION_SUM dataRow)
        {
            try
            {
                WaitingManager.Show();
                if (dataRow != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(dataRow);
                    if (this.ModuleCurrent != null)
                    {
                        CallModule callModule = new CallModule(CallModule.HisMealRationDetail, this.ModuleCurrent.RoomId, this.ModuleCurrent.RoomTypeId, listArgs);
                    }
                    else
                    {
                        CallModule callModule = new CallModule(CallModule.HisMealRationDetail, 0, 0, listArgs);
                    }
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Warn(ex);
            }
        }

        private void btnApproval_ButtonClick(V_HIS_RATION_SUM dataRow)
        {
            try
            {
                WaitingManager.Show();
                bool success = false;
                CommonParam param = new CommonParam();
                if (dataRow != null)
                {
                    HisRationSumUpdateSDO dataUpdate = new HisRationSumUpdateSDO();
                    dataUpdate.RationSumId = dataRow.ID;
                    dataUpdate.WorkingRoomId = this.ModuleCurrent.RoomId;
                    var Result = new BackendAdapter(param).Post<HIS_RATION_SUM>(HisRequestUriStore.Ration_Sum_Approve, ApiConsumers.MosConsumer, dataUpdate, param);
                    if (Result != null)
                    {
                        success = true;
                        FillDataToGridcontrol();
                        this.listRationSumSelect = new List<V_HIS_RATION_SUM>();
                        btnPrint.Enabled = false;
                    }
                }
                WaitingManager.Hide();
                MessageManager.Show(this.ParentForm, param, success);

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }

        private void btnCancalApproval_ButtonClick(V_HIS_RATION_SUM dataRow)
        {
            try
            {
                WaitingManager.Show();
                bool success = false;
                CommonParam param = new CommonParam();
                if (dataRow != null)
                {
                    HisRationSumUpdateSDO dataUpdate = new HisRationSumUpdateSDO();
                    dataUpdate.RationSumId = dataRow.ID;
                    dataUpdate.WorkingRoomId = this.ModuleCurrent.RoomId;
                    var Result = new BackendAdapter(param).Post<HIS_RATION_SUM>(HisRequestUriStore.Ration_Sum_Unapprove, ApiConsumers.MosConsumer, dataUpdate, param);
                    if (Result != null)
                    {
                        success = true;
                        FillDataToGridcontrol();
                        this.listRationSumSelect = new List<V_HIS_RATION_SUM>();
                        btnPrint.Enabled = false;
                    }
                }
                WaitingManager.Hide();
                MessageManager.Show(this.ParentForm, param, success);

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }

        private void btnApprovalReject_ButtonClick(V_HIS_RATION_SUM dataRow)
        {
            try
            {
                WaitingManager.Show();
                bool success = false;
                CommonParam param = new CommonParam();
                if (dataRow != null)
                {
                    HisRationSumUpdateSDO dataUpdate = new HisRationSumUpdateSDO();
                    dataUpdate.RationSumId = dataRow.ID;
                    dataUpdate.WorkingRoomId = this.ModuleCurrent.RoomId;
                    var Result = new BackendAdapter(param).Post<HIS_RATION_SUM>(HisRequestUriStore.Ration_Sum_Reject, ApiConsumers.MosConsumer, dataUpdate, param);
                    if (Result != null)
                    {
                        success = true;
                        FillDataToGridcontrol();
                        this.listRationSumSelect = new List<V_HIS_RATION_SUM>();
                        btnPrint.Enabled = false;
                    }
                }
                WaitingManager.Hide();
                MessageManager.Show(this.ParentForm, param, success);

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }

        private void btnCancalApproveReject_ButtonClick(V_HIS_RATION_SUM dataRow)
        {
            try
            {
                WaitingManager.Show();
                bool success = false;
                CommonParam param = new CommonParam();
                if (dataRow != null)
                {
                    HisRationSumUpdateSDO dataUpdate = new HisRationSumUpdateSDO();
                    dataUpdate.RationSumId = dataRow.ID;
                    dataUpdate.WorkingRoomId = this.ModuleCurrent.RoomId;
                    var Result = new BackendAdapter(param).Post<HIS_RATION_SUM>(HisRequestUriStore.Ration_Sum_Unreject, ApiConsumers.MosConsumer, dataUpdate, param);
                    if (Result != null)
                    {
                        success = true;
                        FillDataToGridcontrol();
                        this.listRationSumSelect = new List<V_HIS_RATION_SUM>();
                        btnPrint.Enabled = false;
                    }
                }
                WaitingManager.Hide();
                MessageManager.Show(this.ParentForm, param, success);

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }
        private void btnDelete_Click(V_HIS_RATION_SUM datarow)
        {
            try
            {
                if ((MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), "", MessageBoxButtons.YesNo, MessageBoxIcon.Question)) == DialogResult.Yes)
                {
                    bool result = false;
                    WaitingManager.Show();
                    CommonParam param = new CommonParam();

                    if (datarow != null)
                    {
                        HisRationSumSDO dataUpdate = new HisRationSumSDO();
                        dataUpdate.Id = datarow.ID;
                        dataUpdate.RoomId = this.ModuleCurrent.RoomId;
                        result = new BackendAdapter(param).Post<bool>(HisRequestUriStore.Ration_Sum_Delete, ApiConsumers.MosConsumer, dataUpdate, param);
                        if (result)
                        {
                            FillDataToGridcontrol();
                        }
                    }
                    MessageManager.Show(this.ParentForm, param, result);
                    WaitingManager.Hide();
                }

            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                LogSystem.Error(ex);
            }
        }
        #endregion

        private void txtKeyWork_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    FillDataToGridcontrol();
                }

            }
            catch (Exception ex)
            {

                LogSystem.Warn(ex);
            }
        }
        #endregion

        #endregion

        private void cboRation_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {

            try
            {
                if(e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete) {
                    cboRation.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
    }
}

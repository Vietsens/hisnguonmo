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
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.ApiConsumer;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.LocalStorage;
using MOS.SDO;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.IsAdmin;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraBars;
using HIS.Desktop.Utility;
using HIS.Desktop.ADO;

namespace HIS.Desktop.Plugins.HisBidList
{
    public partial class UCHisBidList : HIS.Desktop.Utility.UserControlBase
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        private string LoggingName = "";
        Inventec.Desktop.Common.Modules.Module currentModule = null;
        long roomId = 0;
        long roomTypeId = 0;
        List<ACS.EFMODEL.DataModels.ACS_CONTROL> controlAcs;
        V_HIS_BID currentBID = null;
        PopupMenuProcessor popupMenuProcessor = null;
        #endregion

        #region Construct
        public UCHisBidList()
        {
            InitializeComponent();
            try
            {
                Resources.ResourceLanguageManager.InitResourceLanguageManager();
                LoggingName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public UCHisBidList(Inventec.Desktop.Common.Modules.Module _moduleData)
            : base(_moduleData)
        {
            try
            {
                InitializeComponent();
                Resources.ResourceLanguageManager.InitResourceLanguageManager();
                LoggingName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                this.currentModule = _moduleData;
                this.roomId = _moduleData.RoomId;
                this.roomTypeId = _moduleData.RoomTypeId;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCHisBidList_Load(object sender, EventArgs e)
        {
            try
            {
                if (GlobalVariables.AcsAuthorizeSDO != null)
                {
                    controlAcs = GlobalVariables.AcsAuthorizeSDO.ControlInRoles;
                }

                //Gan ngon ngu
                LoadKeysFromlanguage();

                //Gan gia tri mac dinh
                SetDefaultValueControl();

                //Load du lieu
                FillDataToGrid();

                txtKeyWord.Focus();

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
                var cultureLang = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                //filter
                this.txtKeyWord.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_LIST__TXT_KEYWORD",
                    Resources.ResourceLanguageManager.LanguageUCHisBidList,
                    cultureLang);
                this.navBarGroupCreateTime.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_LIST__NAV_BAR_CREATE_TIME",
                    Resources.ResourceLanguageManager.LanguageUCHisBidList,
                    cultureLang);
                this.lciCreateTimeFrom.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_LIST__LCI_CREATE_TIME_FROM",
                    Resources.ResourceLanguageManager.LanguageUCHisBidList,
                    cultureLang);
                this.lciCreateTimeTo.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_LIST__LCI_CREATE_TIME_TO",
                    Resources.ResourceLanguageManager.LanguageUCHisBidList,
                    cultureLang);
                this.btnRefresh.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_LIST__BTN_REFRESH",
                    Resources.ResourceLanguageManager.LanguageUCHisBidList,
                    cultureLang);
                this.btSearch.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_LIST__BTN_SEARCH",
                    Resources.ResourceLanguageManager.LanguageUCHisBidList,
                    cultureLang);

                //gridView
                this.STT.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_LIST__GRID_COLUMN__STT",
                    Resources.ResourceLanguageManager.LanguageUCHisBidList,
                    cultureLang);
                this.GcBidName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_LIST__GRID_COLUMN__BID_NAME",
                    Resources.ResourceLanguageManager.LanguageUCHisBidList,
                    cultureLang);
                this.GcBidNumber.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_LIST__GRID_COLUMN__BID_NUMBER",
                    Resources.ResourceLanguageManager.LanguageUCHisBidList,
                    cultureLang);
                this.GcBloodTypeCount.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_LIST__GRID_COLUMN__BLOOD_TYPE_COUNT",
                    Resources.ResourceLanguageManager.LanguageUCHisBidList,
                    cultureLang);
                this.GcCreateTime.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_LIST__GRID_COLUMN__CREATE_TIME",
                    Resources.ResourceLanguageManager.LanguageUCHisBidList,
                    cultureLang);
                this.GcCreator.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_LIST__GRID_COLUMN__CREATOR",
                    Resources.ResourceLanguageManager.LanguageUCHisBidList,
                    cultureLang);
                this.GcMaterialTypeCount.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_LIST__GRID_COLUMN__MATERIAL_TYPE_COUNT",
                    Resources.ResourceLanguageManager.LanguageUCHisBidList,
                    cultureLang);
                this.GcMedicineTypeCount.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_LIST__GRID_COLUMN__MEDICINE_TYPE_COUNT",
                    Resources.ResourceLanguageManager.LanguageUCHisBidList,
                    cultureLang);
                this.GcModifier.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_LIST__GRID_COLUMN__MODIFIER",
                    Resources.ResourceLanguageManager.LanguageUCHisBidList,
                    cultureLang);
                this.GcModifyTime.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_LIST__GRID_COLUMN__MODIFY_TIME",
                    Resources.ResourceLanguageManager.LanguageUCHisBidList,
                    cultureLang);
                this.Gc_BidTypeNanme.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_LIST__GRID_COLUMN__BID_TYPE_NAME",
                    Resources.ResourceLanguageManager.LanguageUCHisBidList,
                    cultureLang);

                this.ButtonDeleteDisable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_LIST__BUTTON_DELETE",
                    Resources.ResourceLanguageManager.LanguageUCHisBidList,
                    cultureLang);
                this.ButtonEditDelete.Buttons[0].ToolTip = this.ButtonDeleteDisable.Buttons[0].ToolTip;
                this.ButtonEditViewDetail.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_LIST__BUTTON_VIEW",
                    Resources.ResourceLanguageManager.LanguageUCHisBidList,
                    cultureLang);
                this.btnEditBid.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_BID_LIST__BUTTON_EDIT",
                    Resources.ResourceLanguageManager.LanguageUCHisBidList,
                    cultureLang);
                this.btnEditBid_Disable.Buttons[0].ToolTip = this.btnEditBid.Buttons[0].ToolTip;

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
                dtCreateTimeFrom.DateTime = DateTime.Now;
                dtCreateTimeTo.DateTime = DateTime.Now;
                txtKeyWord.Focus();
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
                ApiResultObject<List<MOS.EFMODEL.DataModels.V_HIS_BID>> apiResult = null;
                MOS.Filter.HisBidViewFilter filter = new MOS.Filter.HisBidViewFilter();
                SetFilter(ref filter);
                gridView.BeginUpdate();
                apiResult = new Inventec.Common.Adapter.BackendAdapter
                    (paramCommon).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_BID>>
                    (ApiConsumer.HisRequestUriStore.HIS_BID_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);

                if (apiResult != null)
                {
                    var data = apiResult.Data;
                    if (data != null && data.Count > 0)
                    {
                        gridControl.DataSource = data;
                        rowCount = (data == null ? 0 : data.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                    else
                    {
                        gridControl.DataSource = null;
                        rowCount = (data == null ? 0 : data.Count);
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

        private void SetFilter(ref MOS.Filter.HisBidViewFilter filter)
        {
            try
            {
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
                filter.KEY_WORD = txtKeyWord.Text.Trim();

                if (dtCreateTimeFrom.EditValue != null && dtCreateTimeFrom.DateTime != DateTime.MinValue)
                    filter.CREATE_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtCreateTimeFrom.EditValue).ToString("yyyyMMdd") + "000000");

                if (dtCreateTimeTo.EditValue != null && dtCreateTimeTo.DateTime != DateTime.MinValue)
                    filter.CREATE_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtCreateTimeTo.EditValue).ToString("yyyyMMdd") + "235959");
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
                if (e.KeyCode == Keys.Enter)
                {
                    btSearch_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtCreateTimeFrom_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (dtCreateTimeFrom.EditValue != null)
                    {
                        dtCreateTimeTo.Focus();
                        dtCreateTimeTo.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtCreateTimeTo_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
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

        private void btSearch_Click(object sender, EventArgs e)
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
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ButtonEditViewDetail_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (MOS.EFMODEL.DataModels.V_HIS_BID)gridView.GetFocusedRow();
                MOS.EFMODEL.DataModels.HIS_BID data = new MOS.EFMODEL.DataModels.HIS_BID();
                Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_BID>(data, row);
                if (data == null) return;

                List<object> listArgs = new List<object>();
                listArgs.Add(data);
                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.BidDetail", roomId, roomTypeId, listArgs);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ButtonEditDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            try
            {
                bool success = false;
                var row = (MOS.EFMODEL.DataModels.V_HIS_BID)gridView.GetFocusedRow();
                if (row != null)
                {
                    WaitingManager.Show();
                    if (DevExpress.XtraEditors.XtraMessageBox.Show(
                    Resources.ResourceMessage.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong,
                    Resources.ResourceMessage.ThongBao,
                    MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        MOS.EFMODEL.DataModels.HIS_BID data = new MOS.EFMODEL.DataModels.HIS_BID();
                        Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_BID>(data, row);
                        var apiresult = new Inventec.Common.Adapter.BackendAdapter
                           (param).Post<bool>
                           (HisRequestUriStore.HIS_BID_DELETE, ApiConsumers.MosConsumer, data, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                        if (apiresult)
                        {
                            success = true;
                            FillDataToGrid();
                        }
                        WaitingManager.Hide();
                        #region Show message
                        MessageManager.Show(this.ParentForm, param, success);
                        #endregion

                        #region Process has exception
                        HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnEditBid_Click(object sender, EventArgs e)
        {
            try
            {
                var row = (MOS.EFMODEL.DataModels.V_HIS_BID)gridView.GetFocusedRow();
                MOS.EFMODEL.DataModels.HIS_BID data = new MOS.EFMODEL.DataModels.HIS_BID();
                Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_BID>(data, row);
                if (data == null) return;

                List<object> listArgs = new List<object>();
                listArgs.Add(data.ID);
                listArgs.Add((HIS.Desktop.Common.RefeshReference)FillDataToGrid);
                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.BidUpdate", roomId, roomTypeId, listArgs);
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
                    MOS.EFMODEL.DataModels.V_HIS_BID data = (MOS.EFMODEL.DataModels.V_HIS_BID)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;

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
                        else if (e.Column.FieldName == "IS_ACCEPTING_NO_EXECUTE_STR")
                        {

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

                string creator = (gridView.GetRowCellValue(e.RowHandle, "CREATOR") ?? "").ToString().Trim();
                string alllowUpdateLoginnames = (gridView.GetRowCellValue(e.RowHandle, "ALLOW_UPDATE_LOGINNAMES") ?? "").ToString().Trim();
                string alllowU = (gridView.GetRowCellValue(e.RowHandle, "CREATE_TIME_DISPLAY") ?? "").ToString().Trim();
                short isActive = Inventec.Common.TypeConvert.Parse.ToInt16((gridView.GetRowCellValue(e.RowHandle, "IS_ACTIVE") ?? "").ToString());
                MOS.EFMODEL.DataModels.V_HIS_BID data = (MOS.EFMODEL.DataModels.V_HIS_BID)((IList)((BaseView)sender).DataSource)[e.RowHandle];

                if (e.Column.FieldName == "DELETE_DISPLAY") // xóa
                {
                    try
                    {
                        if (creator == LoggingName)
                            e.RepositoryItem = ButtonEditDelete;
                        else
                            e.RepositoryItem = ButtonDeleteDisable;
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Warn(ex);
                    }
                }
                else if (e.Column.FieldName == "UPDATE_DISPLAY") // sửa
                {
                    try
                    { //  
                        if (creator == LoggingName || (!String.IsNullOrWhiteSpace(alllowUpdateLoginnames) && alllowUpdateLoginnames.Contains(LoggingName)))
                            e.RepositoryItem = btnEditBid;
                        else
                            e.RepositoryItem = btnEditBid_Disable;
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Warn(ex);
                    }
                }
                else if (e.Column.FieldName == "ACTIVE_DISPLAY") // Khóa
                {
                    try
                    {
                        if (creator == LoggingName || CheckLoginAdmin.IsAdmin(LoggingName))
                        {
                            if (isActive == 1)
                                e.RepositoryItem = btnUnLock;
                            else
                                e.RepositoryItem = btnLock;
                        }
                        else
                        {
                            if (isActive == 1)
                                e.RepositoryItem = btnUnLockDis;
                            else
                                e.RepositoryItem = btnLockDis;
                        }
                    }
                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Warn(ex);
                    }
                }
                else if (e.Column.FieldName == "USER_EDITALBE_DATA") // sửa
                {
                    try
                    {
                        if (creator == LoggingName)
                            e.RepositoryItem = repositoryItemButtonEdit_UserEditableData_Enable;
                        else
                            e.RepositoryItem = repositoryItemButtonEdit_UserEditableData_Disable;

                        if (data.APPROVAL_TIME == null)
                        {
                            e.RepositoryItem = repositoryItemButtonEdit_UserEditableData_Enable;
                        }
                    }

                    catch (Exception ex)
                    {
                        Inventec.Common.Logging.LogSystem.Warn(ex);
                    }
                }
                // Đoạn này

                //Nếu chưa duyệt (APPROVAl_TIME = null trong V_HIS_BID) thì hiển thị nút "duyệt". Nếu đã duyệt (APPROVAl_TIME <> null trong V_HIS_BID) thì hiển thị nút "hủy duyệt".

                else if (e.Column.FieldName == "IS_ACCEPTING_NO_EXECUTE_STR")
                {
                    if (data.APPROVAL_TIME == null)
                    {
                        if (controlAcs.FirstOrDefault(o => o.CONTROL_CODE == "HIS000031") != null) // && (creator == LoggingName || CheckLoginAdmin.IsAdmin(LoggingName))
                        {
                            e.RepositoryItem = repositoryItemButtonEdit_Enduyet;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemButtonEdit_DisDuyet;
                        }

                    }
                    else
                    {
                        if (controlAcs.FirstOrDefault(o => o.CONTROL_CODE == "HIS000032") != null) // && (creator == LoggingName || CheckLoginAdmin.IsAdmin(LoggingName))
                        {
                            e.RepositoryItem = repositoryItemButtonEditAllowNotExecute_Enable;
                        }
                        else
                        {
                            e.RepositoryItem = repositoryItemButtonEditAllowNotExecute_Disable;
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

        #region Public method
        public void Search()
        {
            try
            {
                btSearch_Click(null, null);
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
        #endregion

        private void repositoryItemButtonEdit_UserEditableData_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (MOS.EFMODEL.DataModels.V_HIS_BID)gridView.GetFocusedRow();
                if (row == null) return;

                List<object> listArgs = new List<object>();
                listArgs.Add(row.ID);
                listArgs.Add(Grantable.HIS_BID);
                listArgs.Add(row.ALLOW_UPDATE_LOGINNAMES);
                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.HisUserEditableData", roomId, roomTypeId, listArgs);
                FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonEdit_Enduyet_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {

            try
            {
                bool success = false;
                CommonParam param = new CommonParam();
                var row = (MOS.EFMODEL.DataModels.V_HIS_BID)gridView.GetFocusedRow();
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => row), row));
                if (row != null)
                {
                    WaitingManager.Show();

                    MOS.EFMODEL.DataModels.HIS_BID data = new MOS.EFMODEL.DataModels.HIS_BID();
                    Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_BID>(data, row);
                    var rs = new Inventec.Common.Adapter.BackendAdapter
                 (param).Post<MOS.EFMODEL.DataModels.V_HIS_BID>
                 ("api/HisBid/Approve", ApiConsumer.ApiConsumers.MosConsumer, data.ID, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    if (rs != null)
                    {
                        success = true;
                        FillDataToGrid();
                    }
                    WaitingManager.Hide();
                    #region Show message
                    MessageManager.Show(this.ParentForm, param, success);
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

        private void repositoryItemButtonEditAllowNotExecute_Enable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                bool success = false;
                CommonParam param = new CommonParam();
                var row = (MOS.EFMODEL.DataModels.V_HIS_BID)gridView.GetFocusedRow();
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => row), row));
                if (row != null)
                {
                    WaitingManager.Show();

                    MOS.EFMODEL.DataModels.HIS_BID data = new MOS.EFMODEL.DataModels.HIS_BID();
                    Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_BID>(data, row);
                    var rs = new Inventec.Common.Adapter.BackendAdapter
                 (param).Post<MOS.EFMODEL.DataModels.V_HIS_BID>
                 ("api/HisBid/Unapprove", ApiConsumer.ApiConsumers.MosConsumer, data.ID, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => rs), rs));
                    if (rs != null)
                    {
                        success = true;
                        FillDataToGrid();
                    }
                    WaitingManager.Hide();
                    #region Show message
                    MessageManager.Show(this.ParentForm, param, success);
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

        private void gridView_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("gridView_PopupMenuShowing.1");
                GridHitInfo hi = e.HitInfo;
                if (hi.InRowCell)
                {
                    Inventec.Common.Logging.LogSystem.Debug("gridView_PopupMenuShowing.2");
                    int rowHandle = gridView.GetVisibleRowHandle(hi.RowHandle);

                    this.currentBID = (V_HIS_BID)gridView.GetRow(rowHandle);

                    gridView.OptionsSelection.EnableAppearanceFocusedCell = true;
                    gridView.OptionsSelection.EnableAppearanceFocusedRow = true;
                    if (barManager1 == null)
                    {
                        barManager1 = new BarManager();
                        barManager1.Form = this;
                    }
                    Inventec.Common.Logging.LogSystem.Debug("gridView_PopupMenuShowing.3");
                    popupMenuProcessor = new PopupMenuProcessor(this.currentBID, barManager1, MouseRight_Click);
                    popupMenuProcessor.InitMenu();
                    Inventec.Common.Logging.LogSystem.Debug("gridView_PopupMenuShowing.4");
                    Inventec.Common.Logging.LogSystem.Debug("e.Allow:" + e.Allow);
                }
                Inventec.Common.Logging.LogSystem.Debug("gridView_PopupMenuShowing.5");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        void MouseRight_Click(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (e.Item is BarButtonItem && this.currentBID != null)
                {
                    var bbtnItem = sender as BarButtonItem;
                    PopupMenuProcessor.ItemType type = (PopupMenuProcessor.ItemType)(e.Item.Tag);
                    switch (type)
                    {
                        case PopupMenuProcessor.ItemType.EventLog:
                            btnEvenLogClick();
                            break;

                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnEvenLogClick()
        {
            try
            {
                if (this.currentBID != null)
                {
                    WaitingManager.Show();
                    List<object> listArgs = new List<object>();
                    Inventec.UC.EventLogControl.Data.DataInit dataInit = new Inventec.UC.EventLogControl.Data.DataInit(ConfigApplications.NumPageSize, "", "", "", "", "", "", this.currentBID.BID_NUMBER);
                    KeyCodeADO ado = new KeyCodeADO();
                    ado.bidNumber = this.currentBID.BID_NUMBER;
                    listArgs.Add(ado);
                    listArgs.Add(Inventec.Desktop.Common.Modules.Module.MODULE_TYPE_ID__FORM);

                    var moduleWK = PluginInstance.GetModuleWithWorkingRoom(this.currentModule, this.currentModule.RoomId, this.currentModule.RoomTypeId);
                    var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(moduleWK, listArgs);
                    if (extenceInstance == null) throw new ArgumentNullException("Khoi tao moduleData that bai. extenceInstance = null");

                    WaitingManager.Hide();
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.EventLog", moduleWK.RoomId, moduleWK.RoomTypeId, listArgs);
                }
            }
            catch (NullReferenceException ex)
            {
                WaitingManager.Hide();
                MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBKhongTimThayPluginsCuaChucNangNay), HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                MessageBox.Show(HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.HeThongTBKhongTimThayPluginsCuaChucNangNay), HIS.Desktop.LibraryMessage.MessageUtil.GetMessage(LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;
                V_HIS_BID data = (V_HIS_BID)gridView.GetFocusedRow();
                if (data != null)
                {
                    WaitingManager.Show();
                    MOS.EFMODEL.DataModels.HIS_BID bid = new MOS.EFMODEL.DataModels.HIS_BID();
                    Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_BID>(bid, data);
                    var apiresult = new Inventec.Common.Adapter.BackendAdapter
                       (param).Post<HIS_BID>
                       ("api/HisBid/ChangeLock", ApiConsumers.MosConsumer, bid, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    if (apiresult != null)
                    {
                        success = true;
                        FillDataToGrid();
                    }
                    WaitingManager.Hide();
                    #region Show message
                    MessageManager.Show(this.ParentForm, param, success);
                    #endregion

                    #region Process has exception
                    HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnUnLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;
                V_HIS_BID data = (V_HIS_BID)gridView.GetFocusedRow();
                if (data != null)
                {
                    WaitingManager.Show();
                    MOS.EFMODEL.DataModels.HIS_BID bid = new MOS.EFMODEL.DataModels.HIS_BID();
                    Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_BID>(bid, data);
                    var apiresult = new Inventec.Common.Adapter.BackendAdapter
                       (param).Post<HIS_BID>
                       ("api/HisBid/ChangeLock", ApiConsumers.MosConsumer, bid, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                    if (apiresult != null)
                    {
                        success = true;
                        FillDataToGrid();
                    }
                    WaitingManager.Hide();
                    #region Show message
                    MessageManager.Show(this.ParentForm, param, success);
                    #endregion

                    #region Process has exception
                    HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(param);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

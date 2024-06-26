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
using HIS.Desktop.LocalStorage.LocalData;
using MOS.SDO;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Common.WebApiClient;
using EMR.Desktop.Plugins.EmrDocumentList.ADO;
using Inventec.Common.Controls.EditorLoader;
using DevExpress.XtraEditors;
using HIS.Desktop.Utilities.Extensions;
using System.IO;
using Inventec.Common.SignLibrary.ADO;
using Inventec.Common.SignLibrary;
using HIS.Desktop.LocalStorage.ConfigSystem;
using EMR.EFMODEL.DataModels;
using HIS.Desktop.Plugins.Library.EmrGenerate;
using EMR.SDO;
using MOS.EFMODEL.DataModels;
using Inventec.Common.Adapter;

namespace EMR.Desktop.Plugins.EmrDocumentList
{
    public partial class UCEmrDocumentList : HIS.Desktop.Utility.UserControlBase
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        private string LoggingName = "";
        long roomId = 0;
        long roomTypeId = 0;
        List<EMR.EFMODEL.DataModels.EMR_DOCUMENT_TYPE> documentTypeSelecteds;
        Inventec.Desktop.Common.Modules.Module moduleData;
        #endregion

        #region Construct
        public UCEmrDocumentList()
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

        public UCEmrDocumentList(Inventec.Desktop.Common.Modules.Module _moduleData)
            : base(_moduleData)
        {
            try
            {
                InitializeComponent();
                this.moduleData = _moduleData;
                Resources.ResourceLanguageManager.InitResourceLanguageManager();
                LoggingName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                this.roomId = _moduleData.RoomId;
                this.roomTypeId = _moduleData.RoomTypeId;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCEmrDocumentList_Load(object sender, EventArgs e)
        {
            try
            {
                //Gan ngon ngu
                LoadKeysFromlanguage();

                InitComboSTT(LoadDataToComboStatus());

                //Gan gia tri mac dinh
                SetDefaultValueControl();

                InitCheck(cboDocumentType, SelectionGrid__Status);

                InitCombo(cboDocumentType, GetDocumentType(), "DOCUMENT_TYPE_NAME", "ID");

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

        private List<EMR.EFMODEL.DataModels.EMR_DOCUMENT_TYPE> GetDocumentType()
        {
            List<EMR.EFMODEL.DataModels.EMR_DOCUMENT_TYPE> result = new List<EFMODEL.DataModels.EMR_DOCUMENT_TYPE>();
            try
            {
                CommonParam param = new CommonParam();
                EMR.Filter.EmrDocumentTypeFilter filter = new Filter.EmrDocumentTypeFilter();
                filter.IS_ACTIVE = 1;
                result = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<EMR.EFMODEL.DataModels.EMR_DOCUMENT_TYPE>>("api/EmrDocumentType/Get", ApiConsumers.EmrConsumer, filter, param);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }
        private void LoadKeysFromlanguage()
        {
            try
            {
                var cultureLang = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                //filter
                this.txtKeyWord.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_EMR_DOCUMENT_LIST__TXT_KEYWORD",
                    Resources.ResourceLanguageManager.LanguageUCHisBidList,
                    cultureLang);
                this.navBarGroupCreateTime.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_EMR_DOCUMENT_LIST__NAV_BAR_CREATE_TIME",
                    Resources.ResourceLanguageManager.LanguageUCHisBidList,
                    cultureLang);
                this.btnRefresh.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_EMR_DOCUMENT_LIST__BTN_REFRESH",
                    Resources.ResourceLanguageManager.LanguageUCHisBidList,
                    cultureLang);
                this.btSearch.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_EMR_DOCUMENT_LIST__BTN_SEARCH",
                    Resources.ResourceLanguageManager.LanguageUCHisBidList,
                    cultureLang);

                //gridView
                this.STT.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_EMR_DOCUMENT_LIST__GRID_COLUMN__STT",
                    Resources.ResourceLanguageManager.LanguageUCHisBidList,
                    cultureLang);
                this.GcDispenseTime.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_EMR_DOCUMENT_LIST__GRID_COLUMN__DISPENSE_TIME",
                    Resources.ResourceLanguageManager.LanguageUCHisBidList,
                    cultureLang);
                this.GcDispenseCode.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_EMR_DOCUMENT_LIST__GRID_COLUMN__DISPENSE_CODE",
                    Resources.ResourceLanguageManager.LanguageUCHisBidList,
                    cultureLang);
                this.GcCreateTime.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_EMR_DOCUMENT_LIST__GRID_COLUMN__CREATE_TIME",
                    Resources.ResourceLanguageManager.LanguageUCHisBidList,
                    cultureLang);
                this.GcCreator.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_EMR_DOCUMENT_LIST__GRID_COLUMN__CREATOR",
                    Resources.ResourceLanguageManager.LanguageUCHisBidList,
                    cultureLang);
                this.GcModifier.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_EMR_DOCUMENT_LIST__GRID_COLUMN__MODIFIER",
                    Resources.ResourceLanguageManager.LanguageUCHisBidList,
                    cultureLang);
                this.GcModifyTime.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_EMR_DOCUMENT_LIST__GRID_COLUMN__MODIFY_TIME",
                    Resources.ResourceLanguageManager.LanguageUCHisBidList,
                    cultureLang);
                this.ButtonDeleteDisable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_EMR_DOCUMENT_LIST__BUTTON_DELETE",
                    Resources.ResourceLanguageManager.LanguageUCHisBidList,
                    cultureLang);
                this.ButtonEditDelete.Buttons[0].ToolTip = this.ButtonDeleteDisable.Buttons[0].ToolTip;
                this.ButtonEditViewDetail.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_EMR_DOCUMENT_LIST__BUTTON_VIEW",
                    Resources.ResourceLanguageManager.LanguageUCHisBidList,
                    cultureLang);
                this.btnEditBid.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_EMR_DOCUMENT_LIST__BUTTON_EDIT",
                    Resources.ResourceLanguageManager.LanguageUCHisBidList,
                    cultureLang);
                this.btnEditBid_Disable.Buttons[0].ToolTip = this.btnEditBid.Buttons[0].ToolTip;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboSTT(List<StatusADO> data)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("STT_NAME", "", 150, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("STT_NAME", "ID", columnInfos, false, 150);
                ControlEditorLoader.Load(cboStatus, data, controlEditorADO);

                //mặc định: chờ ký
                cboStatus.EditValue = 2;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private List<StatusADO> LoadDataToComboStatus()
        {
            List<StatusADO> Result = new List<StatusADO>();
            try
            {
                StatusADO StatusADO_TatCa = new StatusADO();
                StatusADO_TatCa.ID = 0;
                StatusADO_TatCa.STT_NAME = "Tất cả";
                Result.Add(StatusADO_TatCa);

                StatusADO StatusADO_DaKy = new StatusADO();
                StatusADO_DaKy.ID = 1;
                StatusADO_DaKy.STT_NAME = "Đã ký xong";
                Result.Add(StatusADO_DaKy);

                StatusADO StatusADO_DangKy = new StatusADO();
                StatusADO_DangKy.ID = 2;
                StatusADO_DangKy.STT_NAME = "Đang ký";
                Result.Add(StatusADO_DangKy);

                StatusADO StatusADO_BiTuChoi = new StatusADO();
                StatusADO_BiTuChoi.ID = 3;
                StatusADO_BiTuChoi.STT_NAME = "Bị từ chối";
                Result.Add(StatusADO_BiTuChoi);

                StatusADO StatusADO_DaXoa = new StatusADO();
                StatusADO_DaXoa.ID = 4;
                StatusADO_DaXoa.STT_NAME = "Đã xóa";
                Result.Add(StatusADO_DaXoa);
            }
            catch (Exception ex)
            {
                Result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return Result;
        }

        private void SetDefaultValueControl()
        {
            try
            {
                txtKeyWord.Text = "";

                cboStatus.EditValue = 0;

                DateTime now = DateTime.Now;
                var startDate = new DateTime(now.Year, now.Month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);

                dtCreateTimeFrom.DateTime = startDate;
                dtCreateTimeTo.DateTime = endDate;

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
                ApiResultObject<List<EMR.EFMODEL.DataModels.V_EMR_DOCUMENT>> apiResult = null;
                EMR.Filter.EmrDocumentViewFilter filter = new EMR.Filter.EmrDocumentViewFilter();
                SetFilter(ref filter);
                gridView.BeginUpdate();
                apiResult = new Inventec.Common.Adapter.BackendAdapter
                    (paramCommon).GetRO<List<EMR.EFMODEL.DataModels.V_EMR_DOCUMENT>>
                    ("api/EmrDocument/GetView", ApiConsumers.EmrConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
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
                else
                {
                    rowCount = 0;
                    dataTotal = 0;
                    gridControl.DataSource = null;
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

        private void SetFilter(ref EMR.Filter.EmrDocumentViewFilter filter)
        {
            try
            {
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";

                if (!String.IsNullOrEmpty(txtKeyWord.Text))
                {
                    filter.KEY_WORD = txtKeyWord.Text.Trim();
                }
                filter.CREATOR = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();

                filter.IS_STORE_TIME_NOT_NULL = false;
                if (cboStatus.EditValue != null)
                {
                    long Stt = Int64.Parse(cboStatus.EditValue.ToString());
                    if (Stt == 1)// da ky xong
                    {
                        filter.IS_NEXT_SIGNER_NOT_NULL = false;
                        filter.IS_REJECTER_NOT_NULL = false;
                        filter.IS_DELETE = false;
                    }
                    else if (Stt == 2) // dang ky
                    {
                        filter.IS_REJECTER_NOT_NULL = false;
                        filter.IS_NEXT_SIGNER_NOT_NULL = true;
                        filter.IS_DELETE = false;
                    }
                    else if (Stt == 3) // bi tu choi
                    {
                        filter.IS_REJECTER_NOT_NULL = true;
                        filter.IS_DELETE = false;
                    }
                    else if (Stt == 4) // da xoa
                    {
                        filter.IS_DELETE = true;
                    }
                }

                if (dtCreateTimeFrom.EditValue != null && dtCreateTimeFrom.DateTime != DateTime.MinValue)
                {
                    filter.CREATE_TIME_FROM = Convert.ToInt64(dtCreateTimeFrom.DateTime.ToString("yyyyMMdd") + "000000");
                }

                if (dtCreateTimeTo.EditValue != null && dtCreateTimeTo.DateTime != DateTime.MinValue)
                {
                    filter.CREATE_TIME_TO = Convert.ToInt64(dtCreateTimeTo.DateTime.ToString("yyyyMMdd") + "235959");
                }

                if (this.documentTypeSelecteds != null && this.documentTypeSelecteds.Count() > 0)
                {
                    filter.DOCUMENT_TYPE_IDs = this.documentTypeSelecteds.Select(o => o.ID).Distinct().ToList();
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
                var row = (EMR.EFMODEL.DataModels.V_EMR_DOCUMENT)gridView.GetFocusedRow();
                if (row == null) return;

                List<object> listArgs = new List<object>();
                listArgs.Add(row);
                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.DispenseDetail", roomId, roomTypeId, listArgs);
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
                var row = (EMR.EFMODEL.DataModels.V_EMR_DOCUMENT)gridView.GetFocusedRow();
                if (row != null)
                {


                    if (DevExpress.XtraEditors.XtraMessageBox.Show(
                    Resources.ResourceMessage.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong,
                    Resources.ResourceMessage.ThongBao,
                    MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        HisDispenseDeleteSDO hisDispenseDeleteSDO = new HisDispenseDeleteSDO();
                        hisDispenseDeleteSDO.Id = row.ID;
                        hisDispenseDeleteSDO.RequestRoomId = this.roomId;
                        WaitingManager.Show();
                        var apiresult = new Inventec.Common.Adapter.BackendAdapter
                           (param).Post<bool>
                           ("api/HisDispense/Delete", ApiConsumers.MosConsumer, hisDispenseDeleteSDO, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                        WaitingManager.Hide();
                        if (apiresult)
                        {
                            success = true;
                            FillDataToGrid();
                        }

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

        private void ButtonEditConfirm_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            try
            {
                bool success = false;
                var row = (EMR.EFMODEL.DataModels.V_EMR_DOCUMENT)gridView.GetFocusedRow();
                if (row != null)
                {
                    HisDispenseConfirmSDO hisDispenseConfirmSDO = new HisDispenseConfirmSDO();
                    hisDispenseConfirmSDO.Id = row.ID;
                    hisDispenseConfirmSDO.RequestRoomId = this.roomId;
                    WaitingManager.Show();
                    var apiresult = new Inventec.Common.Adapter.BackendAdapter
                       (param).Post<HisDispenseResultSDO>
                       ("api/HisDispense/Confirm", ApiConsumers.MosConsumer, hisDispenseConfirmSDO, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
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

        private void ButtonEditDisConfirm_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            try
            {
                bool success = false;
                var row = (EMR.EFMODEL.DataModels.V_EMR_DOCUMENT)gridView.GetFocusedRow();
                if (row != null)
                {
                    HisDispenseConfirmSDO hisDispenseConfirmSDO = new MOS.SDO.HisDispenseConfirmSDO();
                    hisDispenseConfirmSDO.Id = row.ID;
                    hisDispenseConfirmSDO.RequestRoomId = this.roomId;
                    WaitingManager.Show();
                    var apiresult = new Inventec.Common.Adapter.BackendAdapter
                       (param).Post<HisDispenseResultSDO>
                       ("api/HisDispense/UnConfirm", ApiConsumers.MosConsumer, hisDispenseConfirmSDO, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
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

        private void btnEditBid_Click(object sender, EventArgs e)
        {
            try
            {
                var row = (EMR.EFMODEL.DataModels.V_EMR_DOCUMENT)gridView.GetFocusedRow();
                if (row == null) return;

                List<object> listArgs = new List<object>();
                listArgs.Add(row.ID);
                //listArgs.Add((HIS.Desktop.Common.RefeshReference)FillDataToGrid);
                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.DispenseMedicine", roomId, roomTypeId, listArgs);
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
                    EMR.EFMODEL.DataModels.V_EMR_DOCUMENT data = (EMR.EFMODEL.DataModels.V_EMR_DOCUMENT)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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
                        else if (e.Column.FieldName == "STORE_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.STORE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "DOB_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.DOB);
                        }
                        else if (e.Column.FieldName == "NEXT_SIGNER_STR")
                        {
                            e.Value = String.IsNullOrWhiteSpace(data.REJECTER) ? data.NEXT_SIGNER : "";
                        }
                        else if (e.Column.FieldName == "REJECT_REASON")
                        {
                            e.Value = data.REJECT_REASON;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public static int? ToNullableInt(string s)
        {
            int i;
            if (int.TryParse(s, out i)) return i;
            return null;
        }

        public bool HasSpecialChars(string stString)
        {
            if (stString.Trim().StartsWith("#@!@#"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void gridView_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView View = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    V_EMR_DOCUMENT data = (V_EMR_DOCUMENT)gridView.GetRow(e.RowHandle);
                    string creator = (gridView.GetRowCellValue(e.RowHandle, "CREATOR") ?? "").ToString().Trim();
                    int? isConfirm = ToNullableInt((gridView.GetRowCellValue(e.RowHandle, "IS_CONFIRM") ?? "").ToString());
                    int? isActive = ToNullableInt((gridView.GetRowCellValue(e.RowHandle, "IS_ACTIVE") ?? "").ToString());
                    string nextSigner = (gridView.GetRowCellValue(e.RowHandle, "NEXT_SIGNER") ?? "").ToString();

                    if (e.Column.FieldName == "DELETE_DISPLAY") // xóa
                    {
                        try
                        {
                            if (creator == LoggingName && isActive == 1 && isConfirm != 1)
                                e.RepositoryItem = ButtonEditDelete;
                            else
                                e.RepositoryItem = ButtonDeleteDisable;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn(ex);
                        }
                    }
                    if (e.Column.FieldName == "UPDATE_DISPLAY") // sửa
                    {
                        try
                        {
                            if (creator == LoggingName && isActive == 1 && isConfirm != 1)
                                e.RepositoryItem = btnEditBid;
                            else
                                e.RepositoryItem = btnEditBid_Disable;
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Warn(ex);
                        }
                    }
                    if (e.Column.FieldName == "IsConfirm" && isActive == 1)
                    {
                        if (isConfirm == 1)
                        {
                            e.RepositoryItem = ButtonEditDisConfirm;
                        }
                        else
                        {
                            e.RepositoryItem = ButtonEditConfirm;
                        }
                    }
                    if (e.Column.FieldName == "PATIENT_SIGH")
                    {
                        if (!String.IsNullOrWhiteSpace(nextSigner) && HasSpecialChars(nextSigner) && (data.IS_DELETE != 1 && data.COUNT_RESIGN_FAILED == null || data.COUNT_RESIGN_FAILED <= 0))
                        {
                            e.RepositoryItem = ButtonEdit_BNKy;
                        }
                        else
                        {
                            e.RepositoryItem = ButtonEdit_BNKyDisable;
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
        public void FocusDispenseCode()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void txtDispenseCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    FillDataToGrid();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void navBarControl1_Click(object sender, EventArgs e)
        {

        }

        private void InitCombo(GridLookUpEdit cbo, object data, string DisplayValue, string ValueMember)
        {
            try
            {
                cbo.Properties.DataSource = data;
                cbo.Properties.DisplayMember = DisplayValue;
                cbo.Properties.ValueMember = ValueMember;

                DevExpress.XtraGrid.Columns.GridColumn col2 = cbo.Properties.View.Columns.AddField(DisplayValue);
                col2.VisibleIndex = 1;
                col2.Width = 200;
                col2.Caption = "Tất cả";
                cbo.Properties.PopupFormWidth = 200;
                cbo.Properties.View.OptionsView.ShowColumnHeaders = true;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;

                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    //gridCheckMark.SelectAll(cbo.Properties.DataSource);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitCheck(GridLookUpEdit cbo, GridCheckMarksSelection.SelectionChangedEventHandler eventSelect)
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cbo.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(eventSelect);
                cbo.Properties.Tag = gridCheck;
                cbo.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cbo.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cbo.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__Status(object sender, EventArgs e)
        {
            try
            {
                documentTypeSelecteds = new List<EMR.EFMODEL.DataModels.EMR_DOCUMENT_TYPE>();
                foreach (EMR.EFMODEL.DataModels.EMR_DOCUMENT_TYPE rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                        documentTypeSelecteds.Add(rv);
                }
                //var checkKham = treatmentTypeSelecteds.FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM);
                //var checkDieuTri = treatmentTypeSelecteds.FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU || o.ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU);

                //if (checkKham == null && checkDieuTri != null)
                //{
                //    SwitchSetDefaultDate(false);
                //}
                //else
                //{
                //    SwitchSetDefaultDate(true);
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboDocumentType_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string dayName = "";
                if (this.documentTypeSelecteds != null && this.documentTypeSelecteds.Count > 0)
                {
                    foreach (var item in this.documentTypeSelecteds)
                    {
                        dayName += item.DOCUMENT_TYPE_NAME + ", ";
                    }
                }

                e.DisplayText = dayName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ButtonEdit_BNKy_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                EMR.EFMODEL.DataModels.V_EMR_DOCUMENT row = (EMR.EFMODEL.DataModels.V_EMR_DOCUMENT)gridView.GetFocusedRow();
                if (row != null)
                {
                    CommonParam param = new CommonParam();
                    var apiResult = GetEmrDocumentFile(row.ID, false, null, false, ref param);
                    if (apiResult != null && apiResult.Count > 0)
                    {
                        //goi tool view
                        String temFile = Path.GetTempFileName();
                        temFile = temFile.Replace(".tmp", ".pdf");
                        List<string> joinStreams = new List<string>();

                        List<MemoryStream> documentData = new List<MemoryStream>();
                        foreach (var item in apiResult)
                        {
                            if (item.Extension.ToLower().Equals("pdf"))
                            {
                                string pdfAddFile = Utils.GenerateTempFileWithin();
                                Utils.ByteToFile(Utils.StreamToByte(new MemoryStream(Convert.FromBase64String(item.Base64Data))), pdfAddFile);
                                joinStreams.Add(pdfAddFile);
                            }
                        }

                        Stream currentStream = File.Open(temFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);

                        var pdfConcat = new iTextSharp.text.pdf.PdfConcatenate(currentStream);

                        var pages = new List<int>();
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("Đây là dữ liệu joinStreams: " + Inventec.Common.Logging.LogUtil.GetMemberName(() => joinStreams), joinStreams));

                        foreach (var file in joinStreams)
                        {
                            iTextSharp.text.pdf.PdfReader pdfReader = null;
                            pdfReader = new iTextSharp.text.pdf.PdfReader(file);
                            pages = new List<int>();
                            for (int i = 0; i <= pdfReader.NumberOfPages; i++)
                            {
                                pages.Add(i);
                            }
                            pdfReader.SelectPages(pages);
                            pdfConcat.AddPages(pdfReader);
                            pdfReader.Close();
                        }
                        try
                        {
                            pdfConcat.Close();
                        }
                        catch { }

                        foreach (var file in joinStreams)
                        {
                            try
                            {
                                File.Delete(file);
                            }
                            catch { }
                        }
                        SignLibraryGUIProcessor libraryProcessor = new SignLibraryGUIProcessor();

                        #region Thêm in mới
                        Inventec.Common.SignLibrary.ADO.InputADO inputADO = new EmrGenerateProcessor().GenerateInputADO(row.TREATMENT_CODE, row.DOCUMENT_CODE, row.DOCUMENT_NAME, moduleData.RoomId);
                        if (row.WIDTH != null && row.HEIGHT != null && row.RAW_KIND != null)
                        {
                            inputADO.PaperSizeDefault = new System.Drawing.Printing.PaperSize(row.PAPER_NAME, (int)row.WIDTH, (int)row.HEIGHT);
                            if (row.RAW_KIND != null)
                            {
                                inputADO.PaperSizeDefault.RawKind = (int)row.RAW_KIND;
                            }
                        }
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => inputADO.PaperSizeDefault), inputADO.PaperSizeDefault));
                        #endregion


                        //InputADO inputADO = new InputADO();
                        inputADO.DTI = ConfigSystems.URI_API_ACS + "|" + ConfigSystems.URI_API_EMR + "|" + ConfigSystems.URI_API_FSS;
                        inputADO.DTI = String.Format("{0}|{1}|{2}|{3}|{4}|{5}", ConfigSystems.URI_API_ACS, ConfigSystems.URI_API_EMR, ConfigSystems.URI_API_FSS, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetTokenData().TokenCode, Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName(), Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName());
                        inputADO.IsSave = false;
                        inputADO.IsSign = true;//set true nếu cần gọi ký
                        inputADO.IsPatientSign = true;
                        inputADO.IsReject = false;
                        inputADO.IsPrint = false;
                        inputADO.IsExport = false;

                        //Mở popup 
                        inputADO.Treatment = new Inventec.Common.SignLibrary.DTO.TreatmentDTO();
                        inputADO.Treatment.TREATMENT_CODE = row.TREATMENT_CODE;//mã hồ sơ điều trị

                        inputADO.DocumentCode = row.DOCUMENT_CODE;
                        inputADO.DocumentName = row.DOCUMENT_NAME;//Tên văn bản cần tạo

                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => inputADO), inputADO));
                        if (!String.IsNullOrWhiteSpace(temFile) && File.Exists(temFile))
                            libraryProcessor.ShowPopup(temFile, inputADO);
                        else
                        {
                            XtraMessageBox.Show("Không tìm được văn bản ký");
                        }

                        if (File.Exists(temFile)) File.Delete(temFile);
                    }
                    else
                    {
                        MessageManager.Show(this.ParentForm, param, false);
                    }
                }
                else
                {
                    XtraMessageBox.Show("Không tìm được văn bản ký");
                }
                
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<EmrDocumentFileSDO> GetEmrDocumentFile(long documentId, bool? IsMerge, bool? IsShowPatientSign, bool? IsShowWatermark, ref CommonParam paramCommon)
        {
            EmrDocumentDownloadFileSDO sdo = new EmrDocumentDownloadFileSDO();
            var emrFilter = new EMR.Filter.EmrDocumentViewFilter();
            emrFilter.ID = documentId;
            sdo.EmrDocumentViewFilter = emrFilter;
            sdo.IsMerge = IsMerge;
            sdo.IsShowPatientSign = IsShowPatientSign;
            sdo.IsShowWatermark = IsShowWatermark;
            sdo.IsView = null;
            var room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.moduleData.RoomId);
            sdo.RoomCode = room != null ? room.ROOM_CODE : null;
            sdo.DepartmentCode = room != null ? room.DEPARTMENT_CODE : null;
            return new BackendAdapter(paramCommon).Post<List<EmrDocumentFileSDO>>("api/EmrDocument/DownloadFile", ApiConsumers.EmrConsumer, sdo, paramCommon);
        }

        private void gridView_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    var data = (V_EMR_DOCUMENT)((IList)((BaseView)sender).DataSource)[e.RowHandle];
                    if (data != null)
                    {
                        if (data.IS_DELETE == 1)// đã xóa
                        {
                            e.Appearance.ForeColor = Color.Red;
                            e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Strikeout);
                        }
                        else if (data.COUNT_RESIGN_FAILED != null && data.COUNT_RESIGN_FAILED > 0)
                        {
                            e.Appearance.ForeColor = Color.Maroon;
                        }
                        else if (!String.IsNullOrEmpty(data.REJECTER))// bị từ chối
                        {
                            e.Appearance.ForeColor = Color.Red;
                        }

                        else if (data.COUNT_RESIGN_WAIT != null && data.COUNT_RESIGN_WAIT > 0)
                        {
                            e.Appearance.ForeColor = Color.Green;
                        }
                        else if (!String.IsNullOrWhiteSpace(data.NEXT_SIGNER))
                        {
                            e.Appearance.ForeColor = Color.Blue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}

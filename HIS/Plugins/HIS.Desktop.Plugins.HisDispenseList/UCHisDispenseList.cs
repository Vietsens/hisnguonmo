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
using MOS.Filter;
using AutoMapper;
using MOS.EFMODEL.DataModels;
using Inventec.Common.Adapter;
using HIS.Desktop.Utilities.Extensions;

namespace HIS.Desktop.Plugins.HisDispenseList
{
    public partial class UCHisDispenseList : HIS.Desktop.Utility.UserControlBase
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        private string LoggingName = "";
        long roomId = 0;
        long roomTypeId = 0;
        #endregion

        private List<HIS_DISPENSE_TYPE> listDispenseType = null;
        private List<long> selectedDispensTypeIds = new List<long>();

        #region Construct
        public UCHisDispenseList()
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

        public UCHisDispenseList(Inventec.Desktop.Common.Modules.Module _moduleData)
            : base(_moduleData)
        {
            InitializeComponent();
            try
            {
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

        private void UCHisBidList_Load(object sender, EventArgs e)
        {
            try
            {
                //Gan ngon ngu
                LoadKeysFromlanguage();

                //Gan gia tri mac dinh
                SetDefaultValueControl();

                InitDispenseTypeCheck();
                InitComboDispenseType();

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
                    "IVT_LANGUAGE_KEY__UC_HIS_DISPENSE_LIST__TXT_KEYWORD",
                    Resources.ResourceLanguageManager.LanguageUCHisBidList,
                    cultureLang);
                this.txtDispenseCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_DISPENSE_LIST__TXT_DISPENSE_CODE",
                    Resources.ResourceLanguageManager.LanguageUCHisBidList,
                    cultureLang);
                this.navBarGroupCreateTime.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_DISPENSE_LIST__NAV_BAR_CREATE_TIME",
                    Resources.ResourceLanguageManager.LanguageUCHisBidList,
                    cultureLang);
                this.navBarGroupDispenseTime.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_DISPENSE_LIST__NAV_BAR_DISPENSE_TIME",
                    Resources.ResourceLanguageManager.LanguageUCHisBidList,
                    cultureLang);
                this.lciCreateTimeFrom.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_DISPENSE_LIST__LCI_CREATE_TIME_FROM",
                    Resources.ResourceLanguageManager.LanguageUCHisBidList,
                    cultureLang);
                this.lciCreateTimeTo.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_DISPENSE_LIST__LCI_CREATE_TIME_TO",
                    Resources.ResourceLanguageManager.LanguageUCHisBidList,
                    cultureLang);
                this.lciDispenseTimeFrom.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_DISPENSE_LIST__LCI_DISPENSE_TIME_FROM",
                    Resources.ResourceLanguageManager.LanguageUCHisBidList,
                    cultureLang);
                this.lciDispenseTimeTo.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_DISPENSE_LIST__LCI_DISPENSE_TIME_TO",
                    Resources.ResourceLanguageManager.LanguageUCHisBidList,
                    cultureLang);

                this.btnRefresh.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_DISPENSE_LIST__BTN_REFRESH",
                    Resources.ResourceLanguageManager.LanguageUCHisBidList,
                    cultureLang);
                this.btSearch.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_DISPENSE_LIST__BTN_SEARCH",
                    Resources.ResourceLanguageManager.LanguageUCHisBidList,
                    cultureLang);

                //gridView
                this.STT.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_DISPENSE_LIST__GRID_COLUMN__STT",
                    Resources.ResourceLanguageManager.LanguageUCHisBidList,
                    cultureLang);
                this.GcDispenseTime.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_DISPENSE_LIST__GRID_COLUMN__DISPENSE_TIME",
                    Resources.ResourceLanguageManager.LanguageUCHisBidList,
                    cultureLang);
                this.GcDispenseCode.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_DISPENSE_LIST__GRID_COLUMN__DISPENSE_CODE",
                    Resources.ResourceLanguageManager.LanguageUCHisBidList,
                    cultureLang);
                this.GcCreateTime.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_DISPENSE_LIST__GRID_COLUMN__CREATE_TIME",
                    Resources.ResourceLanguageManager.LanguageUCHisBidList,
                    cultureLang);
                this.GcCreator.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_DISPENSE_LIST__GRID_COLUMN__CREATOR",
                    Resources.ResourceLanguageManager.LanguageUCHisBidList,
                    cultureLang);
                this.GcModifier.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_DISPENSE_LIST__GRID_COLUMN__MODIFIER",
                    Resources.ResourceLanguageManager.LanguageUCHisBidList,
                    cultureLang);
                this.GcModifyTime.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_DISPENSE_LIST__GRID_COLUMN__MODIFY_TIME",
                    Resources.ResourceLanguageManager.LanguageUCHisBidList,
                    cultureLang);
                this.ButtonDeleteDisable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_DISPENSE_LIST__BUTTON_DELETE",
                    Resources.ResourceLanguageManager.LanguageUCHisBidList,
                    cultureLang);
                this.ButtonEditDelete.Buttons[0].ToolTip = this.ButtonDeleteDisable.Buttons[0].ToolTip;
                this.ButtonEditViewDetail.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_DISPENSE_LIST__BUTTON_VIEW",
                    Resources.ResourceLanguageManager.LanguageUCHisBidList,
                    cultureLang);
                this.btnEditBid.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_HIS_DISPENSE_LIST__BUTTON_EDIT",
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
                txtDispenseCode.Text = "";
                dtCreateTimeFrom.DateTime = DateTime.Now;
                dtCreateTimeTo.DateTime = DateTime.Now;
                dtDispenseTimeFrom.EditValue = null;
                dtDispenseTimeTo.EditValue = null;
                GridCheckMarksSelection gridCheckMark = cboDispenseType.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboDispenseType.Properties.View);
                }
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
                ApiResultObject<List<V_HIS_DISPENSE>> apiResult = null;
                HisDispenseViewFilter filter = new HisDispenseViewFilter();
                SetFilter(ref filter);
                gridView.BeginUpdate();
                apiResult = new Inventec.Common.Adapter.BackendAdapter
                    (paramCommon).GetRO<List<V_HIS_DISPENSE>>
                    ("api/HisDispense/GetView", ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
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

        private void SetFilter(ref HisDispenseViewFilter filter)
        {
            try
            {
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";

                if (!String.IsNullOrEmpty(txtKeyWord.Text))
                {
                    filter.KEY_WORD = txtKeyWord.Text.Trim();
                }

                if (!String.IsNullOrEmpty(txtDispenseCode.Text))
                {
                    string code = txtDispenseCode.Text.Trim();
                    if (code.Length < 10)
                    {
                        code = string.Format("{0:00000000}", Convert.ToInt64(code));
                        txtDispenseCode.Text = code;
                    }
                    filter.DISPENSE_CODE__EXACT = code;
                }

                if (this.roomId > 0)
                {
                    var currentMedistock = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_MEDI_STOCK>().FirstOrDefault(o => o.ROOM_ID == this.roomId);
                    filter.MEDI_STOCK_ID = currentMedistock.ID;
                }

                if (selectedDispensTypeIds != null && selectedDispensTypeIds.Count > 0)
                {
                    filter.DISPENSE_TYPE_IDs = selectedDispensTypeIds;
                }

                if (dtCreateTimeFrom.EditValue != null && dtCreateTimeFrom.DateTime != DateTime.MinValue)
                    filter.CREATE_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtCreateTimeFrom.EditValue).ToString("yyyyMMdd") + "000000");

                if (dtCreateTimeTo.EditValue != null && dtCreateTimeTo.DateTime != DateTime.MinValue)
                    filter.CREATE_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtCreateTimeTo.EditValue).ToString("yyyyMMdd") + "235959");

                if (dtDispenseTimeFrom.EditValue != null && dtDispenseTimeFrom.DateTime != DateTime.MinValue)
                    filter.DISPENSE_DATE_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtDispenseTimeFrom.EditValue).ToString("yyyyMMdd") + "000000");

                if (dtDispenseTimeTo.EditValue != null && dtDispenseTimeTo.DateTime != DateTime.MinValue)
                    filter.DISPENSE_DATE_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtDispenseTimeTo.EditValue).ToString("yyyyMMdd") + "235959");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboDispenseType()
        {
            try
            {
                HisDispenseTypeFilter filter = new HisDispenseTypeFilter();
                listDispenseType = new BackendAdapter(new CommonParam()).Get<List<HIS_DISPENSE_TYPE>>("api/HisDispenseType/Get", ApiConsumers.MosConsumer, filter, null);
                if (listDispenseType != null)
                {
                    cboDispenseType.Properties.DataSource = listDispenseType;
                    cboDispenseType.Properties.DisplayMember = "DISPENSE_TYPE_NAME";
                    cboDispenseType.Properties.ValueMember = "ID";
                    DevExpress.XtraGrid.Columns.GridColumn col2 = cboDispenseType.Properties.View.Columns.AddField("DISPENSE_TYPE_NAME");
                    col2.VisibleIndex = 1;
                    col2.Width = 200;
                    col2.Caption = "";
                    cboDispenseType.Properties.PopupFormWidth = 200;
                    cboDispenseType.Properties.View.OptionsView.ShowColumnHeaders = false;
                    cboDispenseType.Properties.View.OptionsSelection.MultiSelect = true;
                    GridCheckMarksSelection gridCheckMark = cboDispenseType.Properties.Tag as GridCheckMarksSelection;
                    if (gridCheckMark != null)
                    {
                        gridCheckMark.ClearSelection(cboDispenseType.Properties.View);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitDispenseTypeCheck()
        {
            try
            {
                GridCheckMarksSelection gridCheck = new GridCheckMarksSelection(cboDispenseType.Properties);
                gridCheck.SelectionChanged += new GridCheckMarksSelection.SelectionChangedEventHandler(SelectionGrid__DispenseType);
                cboDispenseType.Properties.Tag = gridCheck;
                cboDispenseType.Properties.View.OptionsSelection.MultiSelect = true;
                GridCheckMarksSelection gridCheckMark = cboDispenseType.Properties.Tag as GridCheckMarksSelection;
                if (gridCheckMark != null)
                {
                    gridCheckMark.ClearSelection(cboDispenseType.Properties.View);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__DispenseType(object sender, EventArgs e)
        {
            try
            {
                selectedDispensTypeIds = new List<long>();
                List<string> name = new List<string>();
                foreach (HIS_DISPENSE_TYPE rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                    {
                        selectedDispensTypeIds.Add(rv.ID);
                        name.Add(rv.DISPENSE_TYPE_NAME);
                    }
                }
                string text = "";
                if (name.Count > 0)
                {
                    text = String.Join(",", name);
                }
                cboDispenseType.Text = text;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                    dtDispenseTimeFrom.Focus();
                    dtDispenseTimeFrom.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtDispenseTimeFrom_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    dtDispenseTimeTo.Focus();
                    dtDispenseTimeTo.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtDispenseTimeTo_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                var row = (V_HIS_DISPENSE)gridView.GetFocusedRow();
                if (row == null) return;

                List<object> listArgs = new List<object>();
                Mapper.CreateMap<V_HIS_DISPENSE, HIS_DISPENSE>();
                listArgs.Add(Mapper.Map<HIS_DISPENSE>(row));
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
                var row = (V_HIS_DISPENSE)gridView.GetFocusedRow();
                if (row != null)
                {
                    if (DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceMessage.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong, Resources.ResourceMessage.ThongBao,
                    MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        if (row.DISPENSE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_DISPENSE_TYPE.ID__DISPENSE_MEDICINE)
                        {
                            HisDispenseDeleteSDO hisDispenseDeleteSDO = new HisDispenseDeleteSDO();
                            hisDispenseDeleteSDO.Id = row.ID;
                            hisDispenseDeleteSDO.RequestRoomId = this.roomId;
                            WaitingManager.Show();
                            var apiresult = new BackendAdapter(param).Post<bool>
                               ("api/HisDispense/Delete", ApiConsumers.MosConsumer, hisDispenseDeleteSDO, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                            WaitingManager.Hide();
                            if (apiresult)
                            {
                                success = true;
                                FillDataToGrid();
                            }
                        }
                        else
                        {
                            HisPackingSDO deleteSDO = new HisPackingSDO();
                            deleteSDO.Id = row.ID;
                            deleteSDO.RequestRoomId = this.roomId;
                            WaitingManager.Show();
                            var apiresult = new BackendAdapter(param).Post<bool>
                               ("api/HisDispense/PackingDelete", ApiConsumers.MosConsumer, deleteSDO, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                            WaitingManager.Hide();
                            if (apiresult)
                            {
                                success = true;
                                FillDataToGrid();
                            }
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
                var row = (V_HIS_DISPENSE)gridView.GetFocusedRow();
                if (row != null)
                {
                    if (row.DISPENSE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_DISPENSE_TYPE.ID__DISPENSE_MEDICINE)
                    {
                        HisDispenseConfirmSDO hisDispenseConfirmSDO = new HisDispenseConfirmSDO();
                        hisDispenseConfirmSDO.Id = row.ID;
                        hisDispenseConfirmSDO.RequestRoomId = this.roomId;
                        WaitingManager.Show();
                        var apiresult = new BackendAdapter(param).Post<HisDispenseResultSDO>
                           ("api/HisDispense/Confirm", ApiConsumers.MosConsumer, hisDispenseConfirmSDO, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                        if (apiresult != null)
                        {
                            success = true;
                            FillDataToGrid();
                        }
                    }
                    else
                    {
                        HisPackingSDO confirmSDO = new HisPackingSDO();
                        confirmSDO.Id = row.ID;
                        confirmSDO.RequestRoomId = this.roomId;
                        WaitingManager.Show();
                        var apiresult = new BackendAdapter(param).Post<HisDispenseResultSDO>
                           ("api/HisDispense/PackingConfirm", ApiConsumers.MosConsumer, confirmSDO, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                        if (apiresult != null)
                        {
                            success = true;
                            FillDataToGrid();
                        }
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
                var row = (V_HIS_DISPENSE)gridView.GetFocusedRow();
                if (row != null)
                {
                    if (row.DISPENSE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_DISPENSE_TYPE.ID__DISPENSE_MEDICINE)
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
                    }
                    else
                    {
                        HisPackingSDO unConfirmSDO = new HisPackingSDO();
                        unConfirmSDO.Id = row.ID;
                        unConfirmSDO.RequestRoomId = this.roomId;
                        WaitingManager.Show();
                        var apiresult = new Inventec.Common.Adapter.BackendAdapter(param).Post<HisDispenseResultSDO>
                           ("api/HisDispense/PackingUnconfirm", ApiConsumers.MosConsumer, unConfirmSDO, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                        if (apiresult != null)
                        {
                            success = true;
                            FillDataToGrid();
                        }
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
                var row = (V_HIS_DISPENSE)gridView.GetFocusedRow();
                if (row == null) return;

                List<object> listArgs = new List<object>();

                if (row.DISPENSE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_DISPENSE_TYPE.ID__DISPENSE_MEDICINE)
                {
                    listArgs.Add(row.ID);
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.DispenseMedicine", roomId, roomTypeId, listArgs);
                }
                else
                {
                    Mapper.CreateMap<V_HIS_DISPENSE, HIS_DISPENSE>();
                    listArgs.Add(Mapper.Map<HIS_DISPENSE>(row));
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.PackingMaterial", roomId, roomTypeId, listArgs);
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
                    V_HIS_DISPENSE data = (V_HIS_DISPENSE)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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
                        else if (e.Column.FieldName == "DISPENSE_TIME_STR")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.DISPENSE_TIME);
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

        private void gridView_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                string creator = (gridView.GetRowCellValue(e.RowHandle, "CREATOR") ?? "").ToString().Trim();
                int? isConfirm = ToNullableInt((gridView.GetRowCellValue(e.RowHandle, "IS_CONFIRM") ?? "").ToString());
                int? isActive = ToNullableInt((gridView.GetRowCellValue(e.RowHandle, "IS_ACTIVE") ?? "").ToString());

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
                txtDispenseCode.Focus();
                txtDispenseCode.SelectAll();
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

        private void cboDispenseType_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)
                {
                    cboDispenseType.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDispenseType_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                GridCheckMarksSelection gridCheckMark = cboDispenseType.Properties.Tag as GridCheckMarksSelection;
                List<string> name = new List<string>();
                if (gridCheckMark == null) return;
                foreach (HIS_DISPENSE_TYPE rv in gridCheckMark.Selection)
                {
                    name.Add(rv.DISPENSE_TYPE_NAME);
                }
                string text = "";
                if (name.Count > 0)
                {
                    text = String.Join(",", name);
                }
                e.DisplayText = text;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

    }
}

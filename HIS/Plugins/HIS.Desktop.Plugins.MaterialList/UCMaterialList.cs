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
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using HIS.Desktop.LocalStorage.ConfigApplication;
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.Plugins.MaterialList.Reason;

namespace HIS.Desktop.Plugins.MaterialList
{
    public partial class UCMaterialList : HIS.Desktop.Utility.UserControlBase
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        long roomId = 0;
        long roomTypeId = 0;
        System.Globalization.CultureInfo cultureLang;
        List<MOS.EFMODEL.DataModels.V_HIS_MATERIAL_1> listMaterial;
        List<MOS.EFMODEL.DataModels.HIS_BID> listBid = new List<MOS.EFMODEL.DataModels.HIS_BID>();
        V_HIS_MEDI_STOCK mediStock = null;
        Inventec.Desktop.Common.Modules.Module currentModule;
        #endregion

        #region Construct
        public UCMaterialList()
        {
            InitializeComponent();
            try
            {
                Resources.ResourceLanguageManager.InitResourceLanguageManager();
                cultureLang = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public UCMaterialList(Inventec.Desktop.Common.Modules.Module _moduleData)
            : base(_moduleData)
        {
            InitializeComponent();
            try
            {
                Resources.ResourceLanguageManager.InitResourceLanguageManager();
                cultureLang = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                this.roomId = _moduleData.RoomId;
                this.roomTypeId = _moduleData.RoomTypeId;
                this.currentModule = _moduleData;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCMaterialList_Load(object sender, EventArgs e)
        {
            try
            {
                //Gan ngon ngu
                LoadKeysFromlanguage();
                this.mediStock = BackendDataWorker.Get<V_HIS_MEDI_STOCK>().FirstOrDefault(o => o.ROOM_ID == this.currentModuleBase.RoomId);
                //Gan gia tri mac dinh
                SetDefaultValueControl();
                chkAll.Checked = true;
                chkLock.Checked = false;
                chkUnlock.Checked = false;
                //Load du lieu
                FillDataToGrid();
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
                //filter
                this.txtKeyWord.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MATERIAL_LIST__TXT_KEYWORD",
                    Resources.ResourceLanguageManager.LanguageUCMaterialList,
                    cultureLang);
                this.txtMaterialTypeCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MATERIAL_LIST__TXT_MATERIAL_TYPE_CODE",
                    Resources.ResourceLanguageManager.LanguageUCMaterialList,
                    cultureLang);
                this.navBarGroupImpTime.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MATERIAL_LIST__NAV_BAR_IMP_TIME",
                    Resources.ResourceLanguageManager.LanguageUCMaterialList,
                    cultureLang);
                this.lciImpTimeFrom.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MATERIAL_LIST__LCI_IMP_TIME_FROM",
                    Resources.ResourceLanguageManager.LanguageUCMaterialList,
                    cultureLang);
                this.lciImpTimeTo.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MATERIAL_LIST__LCI_IMP_TIME_TO",
                    Resources.ResourceLanguageManager.LanguageUCMaterialList,
                    cultureLang);
                this.chkAll.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MATERIAL_LIST__CHK_ALL",
                    Resources.ResourceLanguageManager.LanguageUCMaterialList,
                    cultureLang);
                this.chkLock.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MATERIAL_LIST__CHK_LOCK",
                    Resources.ResourceLanguageManager.LanguageUCMaterialList,
                    cultureLang);
                this.chkUnlock.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MATERIAL_LIST__CHK_UNLOCK",
                    Resources.ResourceLanguageManager.LanguageUCMaterialList,
                    cultureLang);
                this.btnRefresh.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MATERIAL_LIST__BTN_REFRESH",
                    Resources.ResourceLanguageManager.LanguageUCMaterialList,
                    cultureLang);
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MATERIAL_LIST__BTN_SEARCH",
                    Resources.ResourceLanguageManager.LanguageUCMaterialList,
                    cultureLang);
                //gridView
                this.Gc_Amount.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MATERIAL_LIST__GRID_COLUMN__AMOUNT",
                    Resources.ResourceLanguageManager.LanguageUCMaterialList,
                    cultureLang);
                this.Gc_BidName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MATERIAL_LIST__GRID_COLUMN__BID_NAME",
                    Resources.ResourceLanguageManager.LanguageUCMaterialList,
                    cultureLang);
                this.Gc_BidNumber.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MATERIAL_LIST__GRID_COLUMN__BID_NUMBER",
                    Resources.ResourceLanguageManager.LanguageUCMaterialList,
                    cultureLang);
                this.Gc_BidNumOrder.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MATERIAL_LIST__GRID_COLUMN__BID_NUM_ORDER",
                    Resources.ResourceLanguageManager.LanguageUCMaterialList,
                    cultureLang);
                this.Gc_CreateTime.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MATERIAL_LIST__GRID_COLUMN__CREATE_TIME",
                    Resources.ResourceLanguageManager.LanguageUCMaterialList,
                    cultureLang);
                this.Gc_Creator.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MATERIAL_LIST__GRID_COLUMN__CREATOR",
                    Resources.ResourceLanguageManager.LanguageUCMaterialList,
                    cultureLang);
                this.Gc_ExpiredDate.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MATERIAL_LIST__GRID_COLUMN__EXPRIED_DATE",
                    Resources.ResourceLanguageManager.LanguageUCMaterialList,
                    cultureLang);
                this.Gc_ImpPrice.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MATERIAL_LIST__GRID_COLUMN__IMP_PRICE",
                    Resources.ResourceLanguageManager.LanguageUCMaterialList,
                    cultureLang);
                this.Gc_ImpTime.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MATERIAL_LIST__GRID_COLUMN__IMP_TIME",
                    Resources.ResourceLanguageManager.LanguageUCMaterialList,
                    cultureLang);
                this.Gc_ImpVatRatio.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MATERIAL_LIST__GRID_COLUMN__IMP_VAT_RATIO",
                    Resources.ResourceLanguageManager.LanguageUCMaterialList,
                    cultureLang);
                this.Gc_ManufacturerName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MATERIAL_LIST__GRID_COLUMN__MANUFACTURER_NAME",
                    Resources.ResourceLanguageManager.LanguageUCMaterialList,
                    cultureLang);
                this.Gc_MaterialTypeCode.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MATERIAL_LIST__GRID_COLUMN__MATERIAL_TYPE_CODE",
                    Resources.ResourceLanguageManager.LanguageUCMaterialList,
                    cultureLang);
                this.Gc_MaterialTypeName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MATERIAL_LIST__GRID_COLUMN__MATERIAL_TYPE_NAME",
                    Resources.ResourceLanguageManager.LanguageUCMaterialList,
                    cultureLang);
                this.Gc_Modifier.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MATERIAL_LIST__GRID_COLUMN__MODIFIER",
                    Resources.ResourceLanguageManager.LanguageUCMaterialList,
                    cultureLang);
                this.Gc_ModifyTime.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MATERIAL_LIST__GRID_COLUMN__MODIFY_TIME",
                    Resources.ResourceLanguageManager.LanguageUCMaterialList,
                    cultureLang);
                this.Gc_NationalName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MATERIAL_LIST__GRID_COLUMN__NAMTIONAL_NAME",
                    Resources.ResourceLanguageManager.LanguageUCMaterialList,
                    cultureLang);
                this.Gc_PackageNumber.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MATERIAL_LIST__GRID_COLUMN__PACKGE_NUMBER",
                    Resources.ResourceLanguageManager.LanguageUCMaterialList,
                    cultureLang);
                this.Gc_PackingTypeName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MATERIAL_LIST__GRID_COLUMN__PACKING_TYPE_NAME",
                    Resources.ResourceLanguageManager.LanguageUCMaterialList,
                    cultureLang);
                this.Gc_STT.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MATERIAL_LIST__GRID_COLUMN__STT",
                    Resources.ResourceLanguageManager.LanguageUCMaterialList,
                    cultureLang);
                this.Gc_SupplierName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MATERIAL_LIST__GRID_COLUMN__SUPPLIER_NAME",
                    Resources.ResourceLanguageManager.LanguageUCMaterialList,
                    cultureLang);
                this.Gc_BidGroupCode.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MATERIAL_LIST__GRID_COLUMN__BID_GROUP_CODE",
                    Resources.ResourceLanguageManager.LanguageUCMaterialList,
                    cultureLang);
                this.Gc_BidPackageCode.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MATERIAL_LIST__GRID_COLUMN__BID_PACKAGE_CODE",
                    Resources.ResourceLanguageManager.LanguageUCMaterialList,
                    cultureLang);

                //grid button
                this.ButtonEdit.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MATERIAL_LIST__GRID_COLUMN__BUTTON_EDIT",
                    Resources.ResourceLanguageManager.LanguageUCMaterialList,
                    cultureLang);
                this.ButtonLock.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MATERIAL_LIST__GRID_COLUMN__BUTTON_LOCK",
                    Resources.ResourceLanguageManager.LanguageUCMaterialList,
                    cultureLang);
                this.ButtonUnlock.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MATERIAL_LIST__GRID_COLUMN__BUTTON_UN_LOCK",
                    Resources.ResourceLanguageManager.LanguageUCMaterialList,
                    cultureLang);
                this.ButtonEditMaterialPatyDisable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MATERIAL_LIST__GRID_COLUMN__BUTTON_MATERIAL_PATY",
                    Resources.ResourceLanguageManager.LanguageUCMaterialList,
                    cultureLang);
                this.ButtonEditMaterialPatyEnable.Buttons[0].ToolTip = this.ButtonEditMaterialPatyDisable.Buttons[0].ToolTip;
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
                txtMaterialTypeCode.Text = "";
                dtImpTimeTo.DateTime = DateTime.Now;
                dtImpTimeFrom.DateTime = DateTime.Now;
                chkAll.Checked = true;
                chkLock.Checked = false;
                chkUnlock.Checked = false;
                //Focus
                txtMaterialTypeCode.Focus();

                listBid = new Inventec.Common.Adapter.BackendAdapter
                    (new CommonParam()).Get<List<MOS.EFMODEL.DataModels.HIS_BID>>
                    (ApiConsumer.HisRequestUriStore.HIS_BID_GET, ApiConsumer.ApiConsumers.MosConsumer,
                    new MOS.Filter.HisBidFilter(), HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
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
                ucPaging.Init(GridPaging, param, pagingSize, gridControl);
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
                ApiResultObject<List<MOS.EFMODEL.DataModels.V_HIS_MATERIAL_1>> apiResult = null;
                MOS.Filter.HisMaterialView1Filter filter = new MOS.Filter.HisMaterialView1Filter();
                SetFilter(ref filter);
                gridView.BeginUpdate();
                apiResult = new Inventec.Common.Adapter.BackendAdapter
                    (paramCommon).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_MATERIAL_1>>
                    (ApiConsumer.HisRequestUriStore.HIS_MATERIAL_1_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                if (apiResult != null)
                {
                    listMaterial = apiResult.Data.OrderByDescending(o => o.MODIFY_TIME).ThenByDescending(o => o.ID).ToList();
                    if (listMaterial != null && listMaterial.Count > 0)
                    {
                        gridControl.DataSource = listMaterial;
                        rowCount = (listMaterial == null ? 0 : listMaterial.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                    else
                    {
                        gridControl.DataSource = null;
                        rowCount = (listMaterial == null ? 0 : listMaterial.Count);
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

        private void SetFilter(ref MOS.Filter.HisMaterialView1Filter filter)
        {
            try
            {
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
                if (mediStock != null)
                {
                    filter.IS_BUSINESS = (this.mediStock.IS_BUSINESS == 1);
                }

                if (!String.IsNullOrWhiteSpace(txtImpMestCode.Text))
                {
                    string code = txtImpMestCode.Text.Trim();
                    if (code.Length < 12)
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        txtImpMestCode.Text = code;
                        txtImpMestCode.Focus();
                        txtImpMestCode.Select();
                    }
                    filter.TDL_IMP_MEST_CODE = code;
                }
                else
                {
                    filter.TDL_IMP_MEST_SUB_CODE = txtImpMestSubCode.Text.Trim();
                    filter.KEY_WORD = txtKeyWord.Text.Trim();
                    if (!String.IsNullOrEmpty(txtMaterialTypeCode.Text))
                    {
                        filter.MATERIAL_TYPE_CODE__EXACT = txtMaterialTypeCode.Text;
                        filter.KEY_WORD = "";
                    }
                    if (chkLock.Checked)
                    {
                        filter.IS_ACTIVE = 0;
                    }
                    if (chkUnlock.Checked)
                    {
                        filter.IS_ACTIVE = 1;
                    }

                    if (dtImpTimeFrom.EditValue != null && dtImpTimeFrom.DateTime != DateTime.MinValue)
                        filter.IMP_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                            Convert.ToDateTime(dtImpTimeFrom.EditValue).ToString("yyyyMMdd") + "000000");

                    if (dtImpTimeTo.EditValue != null && dtImpTimeTo.DateTime != DateTime.MinValue)
                        filter.IMP_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                            Convert.ToDateTime(dtImpTimeTo.EditValue).ToString("yyyyMMdd") + "235959");
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
                    var data = (MOS.EFMODEL.DataModels.V_HIS_MATERIAL_1)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    var bids = listBid.FirstOrDefault(o => o.ID == data.BID_ID);
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
                        else if (e.Column.FieldName == "EXPIRED_DATE_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.EXPIRED_DATE ?? 0);
                        }
                        else if (e.Column.FieldName == "IMP_VAT_RATIO_DISPLAY")
                        {
                            e.Value = (data.IMP_VAT_RATIO * 100).ToString() + "%";
                        }
                        else if (e.Column.FieldName == "IMP_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.IMP_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "BID_NAME")
                        {
                            e.Value = bids == null ? "" : bids.BID_NAME;
                        }
                        else if (e.Column.FieldName == "PRICE_AFTER_VAT")
                        {
                            e.Value = data.IMP_PRICE * (1 + data.IMP_VAT_RATIO);
                        }
                        else if (e.Column.FieldName == "BID_FORM_DISPLAY")
                        {
                            if (data.BID_FORM_ID == 1)
                            {
                                e.Value = "Đấu thầu rộng rãi";
                            }
                            else if (data.BID_FORM_ID == 2)
                            {
                                e.Value = "Đấu thầu hạn chế";
                            }
                            else if (data.BID_FORM_ID == 3)
                            {
                                e.Value = "Chỉ định thầu";
                            }
                            else if (data.BID_FORM_ID == 4)
                            {
                                e.Value = "Chào hàng cạnh tranh";
                            }
                            else if (data.BID_FORM_ID == 5)
                            {
                                e.Value = "Mua sắm trực tiếp";
                            }
                            else if (data.BID_FORM_ID == 6)
                            {
                                e.Value = "Khác";
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

        private void gridView_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    long isActive = long.Parse((gridView.GetRowCellValue(e.RowHandle, "IS_ACTIVE") ?? "").ToString());
                    if (e.Column.FieldName == "LOCK_DISPLAY")
                    {
                        if (isActive == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                            e.RepositoryItem = ButtonLock;
                        else
                            e.RepositoryItem = ButtonUnlock;
                    }
                    else if (e.Column.FieldName == "MATERIAL_PATY_BTN")
                    {
                        if (isActive == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                        {
                            e.RepositoryItem = ButtonEditMaterialPatyEnable;
                        }
                        else
                        {
                            e.RepositoryItem = ButtonEditMaterialPatyDisable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void Check()
        {

        }

        #region Event
        private void txtMaterialTypeCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtMaterialTypeCode.Text))
                    {
                        FillDataToGrid();
                    }
                    else
                    {
                        txtKeyWord.Focus();
                        txtKeyWord.SelectAll();
                    }
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
                if (e.KeyCode == Keys.Enter) FillDataToGrid();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtImpTimeFrom_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (dtImpTimeFrom.EditValue != null)
                    {
                        dtImpTimeTo.Focus();
                        dtImpTimeTo.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtImpTimeTo_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
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

        private void btnSearch_Click(object sender, EventArgs e)
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

        private void ButtonEdit_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var row = (MOS.EFMODEL.DataModels.V_HIS_MATERIAL_1)gridView.GetFocusedRow();

                if (row != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(row);
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.MaterialUpdate", roomId, roomTypeId, listArgs);
                    FillDataApterClose(row);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataApterClose(MOS.EFMODEL.DataModels.V_HIS_MATERIAL_1 row)
        {
            try
            {
                MOS.Filter.HisMaterialView1Filter filter = new MOS.Filter.HisMaterialView1Filter();
                filter.ID = row.ID;
                var listTreat = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.V_HIS_MATERIAL_1>>(ApiConsumer.HisRequestUriStore.HIS_MATERIAL_1_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, null);
                if (listTreat != null && listTreat.Count == 1)
                {
                    listMaterial[listMaterial.IndexOf(row)] = listTreat.First();
                    gridControl.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ButtonLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {

                var row = (MOS.EFMODEL.DataModels.V_HIS_MATERIAL_1)gridView.GetFocusedRow();
                if (row != null)
                {
                    frmReasonLock frm = new frmReasonLock(this.currentModule, row, FillDataToGrid);
                    frm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void resultDelegate(bool success)
        {
            try
            {
                if (success)
                {
                    FillDataToGrid();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ButtonUnlock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool success = false;
                if (DevExpress.XtraEditors.XtraMessageBox.Show(
                    Resources.ResourceMessage.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong,
                    Resources.ResourceMessage.ThongBao,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    var row = (MOS.EFMODEL.DataModels.V_HIS_MATERIAL_1)gridView.GetFocusedRow();
                    if (row != null)
                    {
                        var data = new MOS.SDO.HisMaterialChangeLockSDO();
                        data.MaterialId = row.ID;
                        data.MediStockId = null;
                        data.WorkingRoomId = this.roomId;
                        var result = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>(ApiConsumer.HisRequestUriStore.HIS_MATERIAL_UNLOCK, ApiConsumer.ApiConsumers.MosConsumer, data, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                        if (result)
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

        private void ButtonEditMaterialPatyEnable_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                var focus = (MOS.EFMODEL.DataModels.V_HIS_MATERIAL_1)gridView.GetFocusedRow();
                if (focus != null)
                {
                    MOS.EFMODEL.DataModels.V_HIS_MATERIAL materialFocus = new MOS.EFMODEL.DataModels.V_HIS_MATERIAL();
                    AutoMapper.Mapper.CreateMap<MOS.EFMODEL.DataModels.V_HIS_MATERIAL_1, MOS.EFMODEL.DataModels.V_HIS_MATERIAL>();
                    materialFocus = AutoMapper.Mapper.Map<MOS.EFMODEL.DataModels.V_HIS_MATERIAL>(focus);
                    List<object> listArgs = new List<object>();
                    listArgs.Add(materialFocus);
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.HisMaterialPaty", roomId, roomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion
        #endregion

        #region Public method
        public void Search()
        {
            try
            {
                btnSearch_Click(null, null);
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

        public void FocusCode()
        {
            try
            {
                txtMaterialTypeCode.Focus();
                txtMaterialTypeCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        private void chkAll_CheckedChanged(object sender, EventArgs e)
        {
            if (chkAll.Checked)
            {
                chkLock.Checked = false;
                chkUnlock.Checked = false;

            }
        }

        private void chkLock_CheckedChanged(object sender, EventArgs e)
        {

            if (chkLock.Checked)
            {
                chkAll.Checked = false;
                chkUnlock.Checked = false;
            }

        }

        private void chkUnlock_CheckedChanged(object sender, EventArgs e)
        {

            if (chkUnlock.Checked)
            {
                chkAll.Checked = false;
                chkLock.Checked = false;
            }
        }

        private void txtImpMestCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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

        private void txtImpMestSubCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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
    }
}

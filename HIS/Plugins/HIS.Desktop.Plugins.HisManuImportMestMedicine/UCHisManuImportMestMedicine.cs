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
using HIS.Desktop.LocalStorage.BackendData;
using DevExpress.XtraEditors.Controls;
using MOS.EFMODEL.DataModels;
using Inventec.Desktop.Common.Message;
using Inventec.Core;
using HIS.Desktop.LocalStorage.ConfigApplication;
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Base;
using System.Collections;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.Utils;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.ApiConsumer;

namespace HIS.Desktop.Plugins.HisManuImportMestMedicine
{
    public partial class UCHisManuImportMestMedicine : UserControl
    {
        #region Declare
        int rowCount = 0;
        int dataTotal = 0;
        int startPage = 0;
        long roomId = 0;
        long roomTypeId = 0;
        DevExpress.XtraGrid.Columns.GridColumn lastColumn = null;
        DevExpress.Utils.ToolTipControlInfo lastInfo = null;
        int lastRowHandle = -1;
        private string LoggingName = "";
        MOS.EFMODEL.DataModels.V_HIS_MEDI_STOCK medistock;
        List<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_MANU> dataGrid;
        #endregion

        #region Construct
        public UCHisManuImportMestMedicine()
        {
            InitializeComponent();
            try
            {
                Resources.ResourceLanguageManager.InitResourceLanguageManager();
                FillDataStatus();
                gridControl.ToolTipController = this.toolTipController;
                LoggingName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public UCHisManuImportMestMedicine(long roomId, long roomTypeId)
            : this()
        {
            try
            {
                medistock = Base.GlobalStore.HisListMediStocks.FirstOrDefault(o => o.ROOM_ID == roomId && o.ROOM_TYPE_ID == roomTypeId);
                this.roomId = roomId;
                this.roomTypeId = roomTypeId;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCHisManuImportMestMedicine_Load(object sender, EventArgs e)
        {
            try
            {
                //Gan ngon ngu
                LoadKeysFromlanguage();

                //Gan gia tri mac dinh
                SetDefaultValueControl();

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
        private void FillDataStatus()
        {
            try
            {
                navBarControlFilter.BeginUpdate();
                int d = 0;
                foreach (var item in Base.GlobalStore.HisImpMestStts)
                {
                    navBarGroupStatus.GroupClientHeight += 25;
                    DevExpress.XtraEditors.CheckEdit checkEdit = new DevExpress.XtraEditors.CheckEdit();
                    layoutControlStatus.Controls.Add(checkEdit);
                    checkEdit.Location = new System.Drawing.Point(50, 2 + (d * 23));
                    checkEdit.Name = item.ID.ToString();
                    checkEdit.Properties.Caption = item.IMP_MEST_STT_NAME;
                    checkEdit.Size = new System.Drawing.Size(150, 19);
                    checkEdit.StyleController = this.layoutControlStatus;
                    checkEdit.TabIndex = 4 + d;
                    checkEdit.EnterMoveNextControl = false;
                    d++;
                }
                navBarControlFilter.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadKeysFromlanguage()
        {
            try
            {
                var cultureLang = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                //filter
                this.txtImpCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MANU_IMPORT_MEST_MEDICINE__TXT_IMP_CODE",
                    Resources.ResourceLanguageManager.LanguageUCHisImportMestMedicine,
                    cultureLang);
                this.txtKeyWord.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MANU_IMPORT_MEST_MEDICINE__TXT_KEYWORD",
                    Resources.ResourceLanguageManager.LanguageUCHisImportMestMedicine,
                    cultureLang);
                this.lciCreateTimeFrom.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MANU_IMPORT_MEST_MEDICINE__LCI_CREATE_TIME_FROM",
                    Resources.ResourceLanguageManager.LanguageUCHisImportMestMedicine,
                    cultureLang);
                this.lciCreateTimeTo.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MANU_IMPORT_MEST_MEDICINE__LCI_CREATE_TIME_TO",
                    Resources.ResourceLanguageManager.LanguageUCHisImportMestMedicine,
                    cultureLang);
                this.lciDocumentDateFrom.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MANU_IMPORT_MEST_MEDICINE__LCI_DOCUMENT_DATE_FROM",
                    Resources.ResourceLanguageManager.LanguageUCHisImportMestMedicine,
                    cultureLang);
                this.lciDocumentDateTo.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MANU_IMPORT_MEST_MEDICINE__LCI_DOCUMENT_DATE_TO",
                    Resources.ResourceLanguageManager.LanguageUCHisImportMestMedicine,
                    cultureLang);
                this.btnRefresh.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MANU_IMPORT_MEST_MEDICINE__BTN_REFRESH",
                    Resources.ResourceLanguageManager.LanguageUCHisImportMestMedicine,
                    cultureLang);
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MANU_IMPORT_MEST_MEDICINE__BTN_SEARCH",
                    Resources.ResourceLanguageManager.LanguageUCHisImportMestMedicine,
                    cultureLang);
                this.navBarCreateTime.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MANU_IMPORT_MEST_MEDICINE__NAV_BAR_CREATE_TIME",
                    Resources.ResourceLanguageManager.LanguageUCHisImportMestMedicine,
                    cultureLang);
                this.navBarDocumentDate.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MANU_IMPORT_MEST_MEDICINE__NAV_BAR_DOCUMENT_DATE",
                    Resources.ResourceLanguageManager.LanguageUCHisImportMestMedicine,
                    cultureLang);
                this.navBarGroupStatus.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MANU_IMPORT_MEST_MEDICINE__NAV_BAR_STATUS",
                    Resources.ResourceLanguageManager.LanguageUCHisImportMestMedicine,
                    cultureLang);
                this.navBarGroupDocumentNumber.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MANU_IMPORT_MEST_MEDICINE__NAV_BAR_DOCUMENT_NUMBER",
                    Resources.ResourceLanguageManager.LanguageUCHisImportMestMedicine,
                    cultureLang);
                this.checkEditHasDocumentNumber.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MANU_IMPORT_MEST_MEDICINE__CHK_HAS_DOCUMENT_NUMBER",
                    Resources.ResourceLanguageManager.LanguageUCHisImportMestMedicine,
                    cultureLang);
                this.checkEditNoDocumentNumber.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MANU_IMPORT_MEST_MEDICINE__CHK_NO_DOCUMENT_NUMBER",
                    Resources.ResourceLanguageManager.LanguageUCHisImportMestMedicine,
                    cultureLang);
                //grid
                this.STT.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MANU_IMPORT_MEST_MEDICINE__GRID_COLUMN__STT",
                    Resources.ResourceLanguageManager.LanguageUCHisImportMestMedicine,
                    cultureLang);
                this.GcApprovalLoginName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MANU_IMPORT_MEST_MEDICINE__GRID_COLUMN__APPROVAL_LOGINNAME",
                    Resources.ResourceLanguageManager.LanguageUCHisImportMestMedicine,
                    cultureLang);
                this.GcApprovalTime.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MANU_IMPORT_MEST_MEDICINE__GRID_COLUMN__APPROVAL_TIME",
                    Resources.ResourceLanguageManager.LanguageUCHisImportMestMedicine,
                    cultureLang);
                this.GcBidNumber.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MANU_IMPORT_MEST_MEDICINE__GRID_COLUMN__BID_NUMBER",
                    Resources.ResourceLanguageManager.LanguageUCHisImportMestMedicine,
                    cultureLang);
                this.GcCreateTime.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MANU_IMPORT_MEST_MEDICINE__GRID_COLUMN__CREATE_TIME",
                    Resources.ResourceLanguageManager.LanguageUCHisImportMestMedicine,
                    cultureLang);
                this.GcCreator.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MANU_IMPORT_MEST_MEDICINE__GRID_COLUMN__CREATOR",
                    Resources.ResourceLanguageManager.LanguageUCHisImportMestMedicine,
                    cultureLang);
                this.GcDeliverer.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MANU_IMPORT_MEST_MEDICINE__GRID_COLUMN__DELIVERER",
                    Resources.ResourceLanguageManager.LanguageUCHisImportMestMedicine,
                    cultureLang);
                this.GcDiscount.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MANU_IMPORT_MEST_MEDICINE__GRID_COLUMN__DISCOUNT",
                    Resources.ResourceLanguageManager.LanguageUCHisImportMestMedicine,
                    cultureLang);
                this.GcDiscountRaito.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MANU_IMPORT_MEST_MEDICINE__GRID_COLUMN__DISCOUNT_RATIO",
                    Resources.ResourceLanguageManager.LanguageUCHisImportMestMedicine,
                    cultureLang);
                this.GcDocumentDate.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MANU_IMPORT_MEST_MEDICINE__GRID_COLUMN__DOCUMENT_DATE",
                    Resources.ResourceLanguageManager.LanguageUCHisImportMestMedicine,
                    cultureLang);
                this.GcDocumentNumber.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MANU_IMPORT_MEST_MEDICINE__GRID_COLUMN__DOCUMENT_NUMBER",
                    Resources.ResourceLanguageManager.LanguageUCHisImportMestMedicine,
                    cultureLang);
                this.GcImpLoginName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MANU_IMPORT_MEST_MEDICINE__GRID_COLUMN__IMP_LOGINNAME",
                    Resources.ResourceLanguageManager.LanguageUCHisImportMestMedicine,
                    cultureLang);
                this.GcImpMestCode.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MANU_IMPORT_MEST_MEDICINE__GRID_COLUMN__IMP_MEST_CODE",
                    Resources.ResourceLanguageManager.LanguageUCHisImportMestMedicine,
                    cultureLang);
                this.GcImpTime.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MANU_IMPORT_MEST_MEDICINE__GRID_COLUMN__IMP_TIME",
                    Resources.ResourceLanguageManager.LanguageUCHisImportMestMedicine,
                    cultureLang);
                this.GcMediStockCode.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MANU_IMPORT_MEST_MEDICINE__GRID_COLUMN__MEDI_STOCK_CODE",
                    Resources.ResourceLanguageManager.LanguageUCHisImportMestMedicine,
                    cultureLang);
                this.GcMediStockName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MANU_IMPORT_MEST_MEDICINE__GRID_COLUMN__MEDI_STOCK_NAME",
                    Resources.ResourceLanguageManager.LanguageUCHisImportMestMedicine,
                    cultureLang);
                this.GcModifier.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MANU_IMPORT_MEST_MEDICINE__GRID_COLUMN__MODIFIER",
                    Resources.ResourceLanguageManager.LanguageUCHisImportMestMedicine,
                    cultureLang);
                this.GcModifyTime.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MANU_IMPORT_MEST_MEDICINE__GRID_COLUMN__MODIFY_TIME",
                    Resources.ResourceLanguageManager.LanguageUCHisImportMestMedicine,
                    cultureLang);
                this.GcReqLoginName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MANU_IMPORT_MEST_MEDICINE__GRID_COLUMN__REQ_LOGINNAME",
                    Resources.ResourceLanguageManager.LanguageUCHisImportMestMedicine,
                    cultureLang);
                this.GcSupplierCode.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MANU_IMPORT_MEST_MEDICINE__GRID_COLUMN__SUPPLIER_CODE",
                    Resources.ResourceLanguageManager.LanguageUCHisImportMestMedicine,
                    cultureLang);
                this.GcSupplierName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MANU_IMPORT_MEST_MEDICINE__GRID_COLUMN__SUPPLIER_NAME",
                    Resources.ResourceLanguageManager.LanguageUCHisImportMestMedicine,
                    cultureLang);
                this.GcDocumentPrice.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MANU_IMPORT_MEST_MEDICINE__GRID_COLUMN__DOCUMENT_PRICE",
                    Resources.ResourceLanguageManager.LanguageUCHisImportMestMedicine,
                    cultureLang);

                //grid button
                this.ButtonEditActualImportDisable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MANU_IMPORT_MEST_MEDICINE__GR_BUTTON_IMPORT",
                    Resources.ResourceLanguageManager.LanguageUCHisImportMestMedicine,
                    cultureLang);
                this.ButtonEditActualImportEnable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MANU_IMPORT_MEST_MEDICINE__GR_BUTTON_IMPORT",
                    Resources.ResourceLanguageManager.LanguageUCHisImportMestMedicine,
                    cultureLang);
                this.ButtonEditApprovalDisable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MANU_IMPORT_MEST_MEDICINE__GR_BUTTON_APPROVAL",
                    Resources.ResourceLanguageManager.LanguageUCHisImportMestMedicine,
                    cultureLang);
                this.ButtonEditApprovalEnable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MANU_IMPORT_MEST_MEDICINE__GR_BUTTON_APPROVAL",
                    Resources.ResourceLanguageManager.LanguageUCHisImportMestMedicine,
                    cultureLang);
                this.ButtonEditDisApprovalDisable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MANU_IMPORT_MEST_MEDICINE__GR_BUTTON_DIS_APPROVAL",
                    Resources.ResourceLanguageManager.LanguageUCHisImportMestMedicine,
                    cultureLang);
                this.ButtonEditDisApprovalEnable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MANU_IMPORT_MEST_MEDICINE__GR_BUTTON_DIS_APPROVAL",
                    Resources.ResourceLanguageManager.LanguageUCHisImportMestMedicine,
                    cultureLang);
                this.ButtonEditDiscardDisable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MANU_IMPORT_MEST_MEDICINE__GR_BUTTON_DISCARD",
                    Resources.ResourceLanguageManager.LanguageUCHisImportMestMedicine,
                    cultureLang);
                this.ButtonEditDiscardEnable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MANU_IMPORT_MEST_MEDICINE__GR_BUTTON_DISCARD",
                    Resources.ResourceLanguageManager.LanguageUCHisImportMestMedicine,
                    cultureLang);
                this.ButtonEditEditDisable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MANU_IMPORT_MEST_MEDICINE__GR_BUTTON_EDIT",
                    Resources.ResourceLanguageManager.LanguageUCHisImportMestMedicine,
                    cultureLang);
                this.ButtonEditEditEnable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MANU_IMPORT_MEST_MEDICINE__GR_BUTTON_EDIT",
                    Resources.ResourceLanguageManager.LanguageUCHisImportMestMedicine,
                    cultureLang);
                this.ButtonEditExpSupplierDisable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MANU_IMPORT_MEST_MEDICINE__GR_BUTTON_EXPORT_SUPPLIER",
                    Resources.ResourceLanguageManager.LanguageUCHisImportMestMedicine,
                    cultureLang);
                this.ButtonEditExpSupplierEnable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MANU_IMPORT_MEST_MEDICINE__GR_BUTTON_EXPORT_SUPPLIER",
                    Resources.ResourceLanguageManager.LanguageUCHisImportMestMedicine,
                    cultureLang);
                this.ButtonEditReApproval.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MANU_IMPORT_MEST_MEDICINE__GR_BUTTON_RE_APPROVAL",
                    Resources.ResourceLanguageManager.LanguageUCHisImportMestMedicine,
                    cultureLang);
                this.ButtonEditReApprovalDisable.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MANU_IMPORT_MEST_MEDICINE__GR_BUTTON_RE_APPROVAL",
                    Resources.ResourceLanguageManager.LanguageUCHisImportMestMedicine,
                    cultureLang);
                this.ButtonEditViewDetail.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY__UC_MANU_IMPORT_MEST_MEDICINE__GR_BUTTON_VIEW",
                    Resources.ResourceLanguageManager.LanguageUCHisImportMestMedicine,
                    cultureLang);
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
                txtImpCode.Text = "";
                DateTime? TimeNow = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.Now() ?? 0);
                dtCreateTimeFrom.EditValue = TimeNow;
                dtCreateTimeTo.EditValue = TimeNow;
                dtDocumentDateFrom.EditValue = null;
                dtDocumentDateTo.EditValue = null;
                checkEditHasDocumentNumber.Checked = false;
                checkEditNoDocumentNumber.Checked = false;
                txtImpCode.Focus();
                SetDefaultStatus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultStatus()
        {
            try
            {
                if (layoutControlStatus.Controls.Count > 0)
                {
                    for (int i = 0; i < layoutControlStatus.Controls.Count; i++)
                    {
                        if (layoutControlStatus.Controls[i] is DevExpress.XtraEditors.CheckEdit)
                        {
                            var checkEdit = layoutControlStatus.Controls[i] as DevExpress.XtraEditors.CheckEdit;
                            checkEdit.Checked = false;
                        }
                    }
                }
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
                ImportMestPaging(new CommonParam(0, pagingSize));
                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging.Init(ImportMestPaging, param, pagingSize);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ImportMestPaging(object param)
        {
            try
            {
                startPage = ((CommonParam)param).Start ?? 0;
                int limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(startPage, limit);
                ApiResultObject<List<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_MANU>> apiResult = null;
                MOS.Filter.HisManuImpMestViewFilter filter = new MOS.Filter.HisManuImpMestViewFilter();
                SetFilter(ref filter);
                gridView.BeginUpdate();
                apiResult = new Inventec.Common.Adapter.BackendAdapter
                    (paramCommon).GetRO<List<MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_MANU>>
                    (ApiConsumer.HisRequestUriStore.HIS_MANU_IMP_MEST_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, filter, paramCommon);
                if (apiResult != null)
                {
                    dataGrid = apiResult.Data;
                    if (dataGrid != null && dataGrid.Count > 0)
                    {
                        gridControl.DataSource = dataGrid;
                        rowCount = (dataGrid == null ? 0 : dataGrid.Count);
                        dataTotal = (apiResult.Param == null ? 0 : apiResult.Param.Count ?? 0);
                    }
                    else
                    {
                        gridControl.DataSource = null;
                        rowCount = (dataGrid == null ? 0 : dataGrid.Count);
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
            }
        }

        private void SetFilter(ref MOS.Filter.HisManuImpMestViewFilter filter)
        {
            try
            {

                filter.ORDER_FIELD = "IMP_MEST_MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
                filter.KEY_WORD = txtKeyWord.Text.Trim();
                if (roomId == 0)
                {
                    filter.CREATOR = LoggingName;
                }
                else
                {
                    filter.DATA_DOMAIN_FILTER = true;
                    filter.WORKING_ROOM_ID = roomId;
                }
                if (!String.IsNullOrEmpty(txtImpCode.Text))
                {
                    string code = txtImpCode.Text.Trim();
                    if (code.Length < 12 && checkDigit(code))
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        txtImpCode.Text = code;
                    }
                    filter.IMP_MEST_CODE__EXACT = code;
                }
                if (checkEditHasDocumentNumber.Checked)
                    filter.HAS_DOCUMENT_NUMBER = true;
                if (checkEditNoDocumentNumber.Checked)
                    filter.HAS_DOCUMENT_NUMBER = false;
                if (dtCreateTimeFrom.EditValue != null && dtCreateTimeFrom.DateTime != DateTime.MinValue)
                    filter.CREATE_TIME_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtCreateTimeFrom.EditValue).ToString("yyyyMMdd") + "000000");

                if (dtCreateTimeTo.EditValue != null && dtCreateTimeTo.DateTime != DateTime.MinValue)
                    filter.CREATE_TIME_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtCreateTimeTo.EditValue).ToString("yyyyMMdd") + "235959");

                if (dtDocumentDateFrom.EditValue != null && dtDocumentDateFrom.DateTime != DateTime.MinValue)
                    filter.DOCUMENT_DATE_FROM = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtDocumentDateFrom.EditValue).ToString("yyyyMMdd") + "000000");

                if (dtDocumentDateTo.EditValue != null && dtCreateTimeTo.DateTime != DateTime.MinValue)
                    filter.DOCUMENT_DATE_TO = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtDocumentDateTo.EditValue).ToString("yyyyMMdd") + "235959");
                SetFilterStatus(ref filter);
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

        private void SetFilterStatus(ref MOS.Filter.HisManuImpMestViewFilter filter)
        {
            try
            {
                if (layoutControlStatus.Controls.Count > 0)
                {
                    for (int i = 0; i < layoutControlStatus.Controls.Count; i++)
                    {
                        if (layoutControlStatus.Controls[i] is DevExpress.XtraEditors.CheckEdit)
                        {
                            var checkEdit = layoutControlStatus.Controls[i] as DevExpress.XtraEditors.CheckEdit;
                            if (checkEdit.Checked)
                            {
                                if (filter.IMP_MEST_STT_IDs == null)
                                    filter.IMP_MEST_STT_IDs = new List<long>();
                                filter.IMP_MEST_STT_IDs.Add(Inventec.Common.TypeConvert.Parse.ToInt64(checkEdit.Name));
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

        private string DisplayName(string loginname, string username)
        {
            string value = "";
            try
            {
                if (String.IsNullOrEmpty(loginname) && String.IsNullOrEmpty(username))
                {
                    value = "";
                }
                else if (loginname != "" && username == "")
                {
                    value = loginname;
                }
                else if (loginname == "" && username != "")
                {
                    value = username;
                }
                else if (loginname != "" && username != "")
                {
                    value = string.Format("{0} - {1}", loginname, username);
                }
                return value;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                return value;
            }
        }

        private void gridView_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData && e.Column.UnboundType != UnboundColumnType.Bound)
                {
                    MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_MANU data = (MOS.EFMODEL.DataModels.V_HIS_IMP_MEST_MANU)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1 + startPage;
                        }
                        else if (e.Column.FieldName == "IMP_MEST_STT_ICON")// trạng thái
                        {
                            if (data.IMP_MEST_STT_ID == Base.HisImpMestSttCFG.IMP_MEST_STT_ID__DRAFT) //nhap
                            {
                                e.Value = imageListStatus.Images[0];
                            }
                            else if (data.IMP_MEST_STT_ID == Base.HisImpMestSttCFG.IMP_MEST_STT_ID__UNAPPROVED)//yeu cau
                            {
                                e.Value = imageListStatus.Images[1];
                            }
                            else if (data.IMP_MEST_STT_ID == Base.HisImpMestSttCFG.IMP_MEST_STT_ID__REJECTED)//// tu choi
                            {
                                e.Value = imageListStatus.Images[2];
                            }
                            else if (data.IMP_MEST_STT_ID == Base.HisImpMestSttCFG.IMP_MEST_STT_ID__APPROVED) // 
                            {
                                e.Value = imageListStatus.Images[3];
                            }
                            else if (data.IMP_MEST_STT_ID == Base.HisImpMestSttCFG.IMP_MEST_STT_ID__IMPORTED) // da nhap
                            {
                                e.Value = imageListStatus.Images[4];
                            }
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "IMP_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.IMP_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "APPROVAL_TIME_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.APPROVAL_TIME ?? 0);
                        }
                        else if (e.Column.FieldName == "DOCUMENT_DATE_DISPLAY")
                        {
                            e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.DOCUMENT_DATE ?? 0);
                        }
                        else if (e.Column.FieldName == "APPROVAL_LOGINNAME_DISPLAY")
                        {
                            string APPROVAL_LOGINNAME = data.APPROVAL_LOGINNAME;
                            string APPROVAL_USERNAME = data.APPROVAL_USERNAME;
                            e.Value = DisplayName(APPROVAL_LOGINNAME, APPROVAL_USERNAME);
                        }
                        else if (e.Column.FieldName == "IMP_LOGINNAME_DISPLAY")
                        {
                            string IMP_LOGINNAME = data.IMP_LOGINNAME;
                            string IMP_USERNAME = data.IMP_USERNAME;
                            e.Value = DisplayName(IMP_LOGINNAME, IMP_USERNAME);
                        }
                        else if (e.Column.FieldName == "REQ_LOGINNAME_DISPLAY")
                        {
                            string Req_loginName = data.REQ_LOGINNAME;
                            string Req_UserName = data.REQ_USERNAME;
                            e.Value = DisplayName(Req_loginName, Req_UserName);
                        }
                        else if (e.Column.FieldName == "DISCOUNT_RATIO_DISPLAY")
                        {
                            if (data.DISCOUNT_RATIO != 0)
                                e.Value = String.Format("{0:0.#%}", data.DISCOUNT_RATIO);
                        }
                        else if (e.Column.FieldName == "DISCOUNT_DISPLAY")
                        {
                            if (data.DISCOUNT != 0)
                                e.Value = Inventec.Common.Number.Convert.NumberToStringMoneyAfterRound(data.DISCOUNT ?? 0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    long statusIdCheckForButtonEdit = long.Parse((gridView.GetRowCellValue(e.RowHandle, "IMP_MEST_STT_ID") ?? "").ToString());
                    long typeIdCheckForButtonEdit = long.Parse((gridView.GetRowCellValue(e.RowHandle, "IMP_MEST_TYPE_ID") ?? "").ToString());
                    long mediStockId = Inventec.Common.TypeConvert.Parse.ToInt64((gridView.GetRowCellValue(e.RowHandle, "MEDI_STOCK_ID") ?? "").ToString());
                    string creator = (gridView.GetRowCellValue(e.RowHandle, "CREATOR") ?? "").ToString();
                    if (e.Column.FieldName == "APPROVAL_DISPLAY")
                    {
                        if (medistock != null && medistock.ID == mediStockId &&
                            (statusIdCheckForButtonEdit == Base.HisImpMestSttCFG.IMP_MEST_STT_ID__REJECTED ||
                            statusIdCheckForButtonEdit == Base.HisImpMestSttCFG.IMP_MEST_STT_ID__UNAPPROVED))
                        {
                            e.RepositoryItem = ButtonEditApprovalEnable;
                        }
                        else
                        {
                            e.RepositoryItem = ButtonEditApprovalDisable;
                        }
                    }
                    else if (e.Column.FieldName == "EDIT")//sua
                    {
                        if (creator == LoggingName && medistock != null && medistock.ID == mediStockId)
                        {
                            e.RepositoryItem = ButtonEditEditEnable;
                        }
                        else
                            e.RepositoryItem = ButtonEditEditDisable;
                    }
                    else if (e.Column.FieldName == "DISCARD_DISPLAY")//hủy
                    {
                        if ((creator == LoggingName) &&
                            (statusIdCheckForButtonEdit == Base.HisImpMestSttCFG.IMP_MEST_STT_ID__DRAFT ||
                            statusIdCheckForButtonEdit == Base.HisImpMestSttCFG.IMP_MEST_STT_ID__REJECTED ||
                            statusIdCheckForButtonEdit == Base.HisImpMestSttCFG.IMP_MEST_STT_ID__UNAPPROVED))
                        {
                            e.RepositoryItem = ButtonEditDiscardEnable;
                        }
                        else
                            e.RepositoryItem = ButtonEditDiscardDisable;
                    }
                    else if (e.Column.FieldName == "IMPORT_DISPLAY")// thực nhập
                    {
                        if (medistock != null && (statusIdCheckForButtonEdit == Base.HisImpMestSttCFG.IMP_MEST_STT_ID__APPROVED))
                        {
                            if (medistock.ID == mediStockId)
                                e.RepositoryItem = ButtonEditActualImportEnable;
                            else
                                e.RepositoryItem = ButtonEditActualImportDisable;
                        }
                        else
                        {
                            e.RepositoryItem = ButtonEditActualImportDisable;
                        }
                    }
                    else if (e.Column.FieldName == "DIS_APPROVAL")// Không duyệt
                    {
                        if (medistock != null && medistock.ID == mediStockId &&
                            (statusIdCheckForButtonEdit != Base.HisImpMestSttCFG.IMP_MEST_STT_ID__DRAFT &&
                            statusIdCheckForButtonEdit != Base.HisImpMestSttCFG.IMP_MEST_STT_ID__REJECTED &&
                            statusIdCheckForButtonEdit != Base.HisImpMestSttCFG.IMP_MEST_STT_ID__IMPORTED))
                        {
                            e.RepositoryItem = ButtonEditDisApprovalEnable;
                        }
                        else
                        {
                            e.RepositoryItem = ButtonEditDisApprovalDisable;
                        }
                    }
                    else if (e.Column.FieldName == "EXP_SUPPLIER")// Xuất trả NCC
                    {
                        if (medistock != null && medistock.ID == mediStockId &&
                            statusIdCheckForButtonEdit == Base.HisImpMestSttCFG.IMP_MEST_STT_ID__IMPORTED)
                        {
                            e.RepositoryItem = ButtonEditExpSupplierEnable;
                        }
                        else
                        {
                            e.RepositoryItem = ButtonEditExpSupplierDisable;
                        }
                    }
                    else if (e.Column.FieldName == "RE_APPROVAL")// Hủy duyệt
                    {
                        if (medistock != null && medistock.ID == mediStockId &&
                            (statusIdCheckForButtonEdit == Base.HisImpMestSttCFG.IMP_MEST_STT_ID__APPROVED ||
                            statusIdCheckForButtonEdit == Base.HisImpMestSttCFG.IMP_MEST_STT_ID__REJECTED))
                        {
                            e.RepositoryItem = ButtonEditReApproval;
                        }
                        else
                        {
                            e.RepositoryItem = ButtonEditReApprovalDisable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void toolTipController_GetActiveObjectInfo(object sender, DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                if (e.Info == null && e.SelectedControl == gridControl)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = gridControl.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                    GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
                    if (info.InRowCell)
                    {
                        if (lastRowHandle != info.RowHandle || lastColumn != info.Column)
                        {
                            lastColumn = info.Column;
                            lastRowHandle = info.RowHandle;

                            string text = "";
                            if (info.Column.FieldName == "IMP_MEST_STT_NAME")
                            {
                                text = (view.GetRowCellValue(lastRowHandle, "IMP_MEST_STT_NAME") ?? "").ToString();
                            }
                            else if (info.Column.FieldName == "IMP_MEST_STT_ICON")
                            {
                                text = (view.GetRowCellValue(lastRowHandle, "IMP_MEST_STT_NAME") ?? "").ToString();
                            }
                            lastInfo = new ToolTipControlInfo(new DevExpress.XtraGrid.GridToolTipInfo(view, new DevExpress.XtraGrid.Views.Base.CellToolTipInfo(info.RowHandle, info.Column, "Text")), text);
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

        public void Focus()
        {
            try
            {
                txtImpCode.Focus();
                txtImpCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion
    }
}

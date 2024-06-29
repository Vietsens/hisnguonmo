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
using MOS.EFMODEL.DataModels;
using HIS.Desktop.LocalStorage.LocalData;
using MOS.Filter;
using Inventec.Core;
using Inventec.Common.Adapter;
using HIS.Desktop.ApiConsumer;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.LocalStorage.ConfigApplication;
using DevExpress.Data;
using System.Collections;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraEditors.ViewInfo;
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.Plugins.HisAccountBookList.Base;
using HIS.Desktop.LocalStorage.BackendData;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.Plugins.HisAccountBookList.Validation;
using DevExpress.XtraEditors;
using HIS.Desktop.IsAdmin;

namespace HIS.Desktop.Plugins.HisAccountBookList
{
    public partial class UCHisAccountBookList : HIS.Desktop.Utility.UserControlBase
    {
        #region Declare
        internal V_HIS_ACCOUNT_BOOK DataAccountBook { get; set; }
        int positionHandle = -1;
        int lastRowHandle = -1;
        List<MOS.EFMODEL.DataModels.V_HIS_ACCOUNT_BOOK> ListAccountBook;
        List<MOS.EFMODEL.DataModels.V_HIS_TRANSACTION> ListTransaction;
        DevExpress.XtraGrid.Columns.GridColumn lastColumn = null;
        DevExpress.Utils.ToolTipControlInfo lastInfo = null;
        internal int ActionType = 0;
        internal static long branchId;
        Inventec.Desktop.Common.Modules.Module currentModule;
        List<HIS_WORKING_SHIFT> listWorkingShift;
        List<ACS.EFMODEL.DataModels.ACS_CONTROL> controlAcs;
        string loginName = null;

        #endregion

        #region Construct
        public UCHisAccountBookList()
        {
            InitializeComponent();
            try
            {
                Base.ResourceLanguageManager.InitResourceLanguageManager();
                gridControlAccountBook.ToolTipController = this.toolTipController;
                currentModule = new Inventec.Desktop.Common.Modules.Module();
                this.loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                Base.GlobalStore.ListOfReportType();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public UCHisAccountBookList(Inventec.Desktop.Common.Modules.Module _moduleData)
            : base(_moduleData)
        {
            InitializeComponent();
            try
            {
                Base.ResourceLanguageManager.InitResourceLanguageManager();
                gridControlAccountBook.ToolTipController = this.toolTipController;
                currentModule = new Inventec.Desktop.Common.Modules.Module();
                currentModule.RoomId = _moduleData.RoomId;
                currentModule.RoomTypeId = _moduleData.RoomTypeId;
                branchId = WorkPlace.WorkPlaceSDO.FirstOrDefault(o => o.RoomTypeId == _moduleData.RoomTypeId && o.RoomId == _moduleData.RoomId).BranchId;
                this.loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                Base.GlobalStore.ListOfReportType();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UCHisAccountBookList_Load(object sender, EventArgs e)
        {
            try
            {
                //Gan ngon ngu
                LoadKeysFromlanguage();

                //trang thai nut
                EnableButton(this.ActionType);

                new Base.GlobalStore().CreateBillType();
                FillDataToCbo();
                FillDataToCboBillType();
                FillDataToCboWorkingShift();
                FillDataToCboEInvoiceSys();
                //Gan gia tri mac dinh
                SetDefaultValueControl();

                //Load du lieu
                FillDataToGridAccountBookList();

                //Load Validation
                ValidControls();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                    "IVT_LANGUAGE_KEY_UC_ACCOUNT_BOOK__KEY_WORD_NULL_VALUE",
                    Base.ResourceLanguageManager.LanguageUCAccountBook,
                    cultureLang);
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY_UC_ACCOUNT_BOOK__BTN_SEARCH",
                    Base.ResourceLanguageManager.LanguageUCAccountBook,
                    cultureLang);
                this.btnRefesh.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY_UC_ACCOUNT_BOOK__BTN_REFRESH",
                    Base.ResourceLanguageManager.LanguageUCAccountBook,
                    cultureLang);

                this.lciAccountBookCode.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY_UC_ACCOUNT_BOOK__LAYOUT_ACCOUNT_BOOK_CODE",
                    Base.ResourceLanguageManager.LanguageUCAccountBook,
                    cultureLang);

                this.lciAccountBookName.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY_UC_ACCOUNT_BOOK__LAYOUT_ACCOUNT_BOOK_NAME",
                    Base.ResourceLanguageManager.LanguageUCAccountBook,
                    cultureLang);
                this.lciCount.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY_UC_ACCOUNT_BOOK__LAYOUT_COUNT",
                    Base.ResourceLanguageManager.LanguageUCAccountBook,
                    cultureLang);
                this.lciDescription.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY_UC_ACCOUNT_BOOK__LAYOUT_DESCRIPTION",
                    Base.ResourceLanguageManager.LanguageUCAccountBook,
                    cultureLang);
                this.lciFromNumberOrder.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY_UC_ACCOUNT_BOOK__FROM_ORDER_NUMBER",
                    Base.ResourceLanguageManager.LanguageUCAccountBook,
                    cultureLang);
                this.lciForAccountBook.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY_UC_ACCOUNT_BOOK__LAYOUT_FOR_ACCOUNT_BOOK",
                    Base.ResourceLanguageManager.LanguageUCAccountBook,
                    cultureLang);
                this.chkShowAll.Text = Inventec.Common.Resource.Get.Value(
                   "IVT_LANGUAGE_KEY_UC_ACCOUNT_BOOK__CHK_SHOW_ALL",
                   Base.ResourceLanguageManager.LanguageUCAccountBook,
                   cultureLang);
                this.chkForBill.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY_UC_ACCOUNT_BOOK__LAYOUT_CHK_FOR_BILL",
                    Base.ResourceLanguageManager.LanguageUCAccountBook,
                    cultureLang);
                this.chkForOtherSale.Text = Inventec.Common.Resource.Get.Value(
                   "IVT_LANGUAGE_KEY_UC_ACCOUNT_BOOK__LAYOUT_CHK_FOR_OTHER_SALE",
                   Base.ResourceLanguageManager.LanguageUCAccountBook,
                   cultureLang);
                this.chkForDeposit.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY_UC_ACCOUNT_BOOK__LAYOUT_CHK_FOR_DEPOSIT",
                    Base.ResourceLanguageManager.LanguageUCAccountBook,
                    cultureLang);
                this.chkForRepay.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY_UC_ACCOUNT_BOOK__LAYOUT_CHK_FOR_REPAY",
                    Base.ResourceLanguageManager.LanguageUCAccountBook,
                    cultureLang);
                this.chkForDebt.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY_UC_ACCOUNT_BOOK__LAYOUT_CHK_FOR_DEBT",
                    Base.ResourceLanguageManager.LanguageUCAccountBook,
                    cultureLang);

                this.btnAdd.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY_UC_ACCOUNT_BOOK__BTN_ADD",
                    Base.ResourceLanguageManager.LanguageUCAccountBook,
                    cultureLang);
                this.btnCancel.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY_UC_ACCOUNT_BOOK__BTN_CANCEL",
                    Base.ResourceLanguageManager.LanguageUCAccountBook,
                    cultureLang);
                //this.btnEdit.Text = Inventec.Common.Resource.Get.Value(
                //    "IVT_LANGUAGE_KEY_UC_ACCOUNT_BOOK__BTN_EDIT",
                //    Base.ResourceLanguageManager.LanguageUCAccountBook,
                //    cultureLang);
                this.btnSave.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY_UC_ACCOUNT_BOOK__BTN_SAVE",
                    Base.ResourceLanguageManager.LanguageUCAccountBook,
                    cultureLang);
                this.lciNumOrder.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY_UC_ACCOUNT_BOOK__LAYOUT_NUM_ORDER",
                    Base.ResourceLanguageManager.LanguageUCAccountBook,
                    cultureLang);
                this.lciPatientTypeId.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY_UC_ACCOUNT_BOOK__LAYOUT_PATIENT_TYPE_ID",
                    Base.ResourceLanguageManager.LanguageUCAccountBook,
                    cultureLang);
                this.lciReleaseTime.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY_UC_ACCOUNT_BOOK__LAYOUT_RELEASE_TIME",
                    Base.ResourceLanguageManager.LanguageUCAccountBook,
                    cultureLang);
                this.lciSymbolCode.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY_UC_ACCOUNT_BOOK__LAYOUT_SYMBOL_CODE",
                    Base.ResourceLanguageManager.LanguageUCAccountBook,
                    cultureLang);
                this.lciTemplateCode.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY_UC_ACCOUNT_BOOK__LAYOUT_TEMPLATE_CODE",
                    Base.ResourceLanguageManager.LanguageUCAccountBook,
                    cultureLang);
                this.lciBillType.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY_UC_ACCOUNT_BOOK__LAYOUT_BILL_TYPE",
                    Base.ResourceLanguageManager.LanguageUCAccountBook,
                    cultureLang);
                this.lciNotGenOrder.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY_UC_ACCOUNT_BOOK__LAYOUT_NOT_GEN_ORDER",
                    Base.ResourceLanguageManager.LanguageUCAccountBook,
                    cultureLang);
                this.chkNotGenOrder.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY_UC_ACCOUNT_BOOK__CHK_NOT_GEN_ORDER",
                    Base.ResourceLanguageManager.LanguageUCAccountBook,
                    cultureLang);
                this.chkShowAll.Text = Inventec.Common.Resource.Get.Value(
                   "IVT_LANGUAGE_KEY_UC_ACCOUNT_BOOK__CHK_SHOW_ALL",
                   Base.ResourceLanguageManager.LanguageUCAccountBook,
                   cultureLang);

                //gridColumn Account Book
                this.gridColumnNumOrder.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY_UC_ACCOUNT_BOOK__GRID_COLUMN__NUM_ORDER",
                    Base.ResourceLanguageManager.LanguageUCAccountBook,
                    cultureLang);
                this.gridColumnWorkingShift.Caption = Inventec.Common.Resource.Get.Value(
                   "IVT_LANGUAGE_KEY_UC_ACCOUNT_BOOK__GRID_WORKING_SHIFT",
                   Base.ResourceLanguageManager.LanguageUCAccountBook,
                   cultureLang);
                this.gridColumnPatientType.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY_UC_ACCOUNT_BOOK__GRID_COLUMN__PATIENT_TYPE_ID",
                    Base.ResourceLanguageManager.LanguageUCAccountBook,
                    cultureLang);
                this.gridColumnReleaseTime.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY_UC_ACCOUNT_BOOK__GRID_COLUMN__RELEASE_TIME",
                    Base.ResourceLanguageManager.LanguageUCAccountBook,
                    cultureLang);
                this.gridColumnSymbolCode.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY_UC_ACCOUNT_BOOK__GRID_COLUMN__SYMBOL_CODE",
                    Base.ResourceLanguageManager.LanguageUCAccountBook,
                    cultureLang);
                this.gridColumnTemplateCode.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY_UC_ACCOUNT_BOOK__GRID_COLUMN__TEMPLATE_CODE",
                    Base.ResourceLanguageManager.LanguageUCAccountBook,
                    cultureLang);
                this.gridColumnActive.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY_UC_ACCOUNT_BOOK__GRID_COLUMN_IS_ACTIVE",
                    Base.ResourceLanguageManager.LanguageUCAccountBook,
                    cultureLang);
                this.gridColumnCode.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY_UC_ACCOUNT_BOOK__GRID_COLUMN_ACCOUNT_BOOK_CODE",
                    Base.ResourceLanguageManager.LanguageUCAccountBook,
                    cultureLang);
                this.gridColumnCreate.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY_UC_ACCOUNT_BOOK__GRID_COLUMN_CREATE_TIME",
                    Base.ResourceLanguageManager.LanguageUCAccountBook,
                    cultureLang);
                this.gridColumnCreator.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY_UC_ACCOUNT_BOOK__GRID_COLUMN_CREATOR",
                    Base.ResourceLanguageManager.LanguageUCAccountBook,
                    cultureLang);
                this.gridColumnCurrent.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY_UC_ACCOUNT_BOOK__GRID_COLUMN_CURRENT_NUM_ORDER",
                    Base.ResourceLanguageManager.LanguageUCAccountBook,
                    cultureLang);
                this.gridColumnFrom.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY_UC_ACCOUNT_BOOK__GRID_COLUMN_FROM_NUM_ORDER",
                    Base.ResourceLanguageManager.LanguageUCAccountBook,
                    cultureLang);
                this.gridColumnHoler.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY_UC_ACCOUNT_BOOK__GRID_COLUMN_ACCOUNT_HOLDER",
                    Base.ResourceLanguageManager.LanguageUCAccountBook,
                    cultureLang);
                this.gridColumnModifier.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY_UC_ACCOUNT_BOOK__GRID_COLUMN_MODIFIER",
                    Base.ResourceLanguageManager.LanguageUCAccountBook,
                    cultureLang);
                this.gridColumnModify.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY_UC_ACCOUNT_BOOK__GRID_COLUMN_MODIFY_TIME",
                    Base.ResourceLanguageManager.LanguageUCAccountBook,
                    cultureLang);
                this.gridColumnName.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY_UC_ACCOUNT_BOOK__GRID_COLUMN_ACCOUNT_BOOK_NAME",
                    Base.ResourceLanguageManager.LanguageUCAccountBook,
                    cultureLang);
                this.gridColumnTo.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY_UC_ACCOUNT_BOOK__GRID_COLUMN_TO_NUM_ORDER",
                    Base.ResourceLanguageManager.LanguageUCAccountBook,
                    cultureLang);
                this.gridColumnWorkingShift.Caption = Inventec.Common.Resource.Get.Value(
                     "IVT_LANGUAGE_KEY_UC_ACCOUNT_BOOK__GRID_COLUMN_WORKING_SHIFT",
                     Base.ResourceLanguageManager.LanguageUCAccountBook,
                     cultureLang);
                this.gridColumnTotal.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY_UC_ACCOUNT_BOOK__GRID_COLUMN_TOTAL",
                    Base.ResourceLanguageManager.LanguageUCAccountBook,
                    cultureLang);
                this.gridColumnMaxItemNumber.Caption = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY_UC_ACCOUNT_BOOK__GRID_COLUMN_MAX_ITEM_NUMBER",
                    Base.ResourceLanguageManager.LanguageUCAccountBook,
                    cultureLang);
                this.lciMaxItemNumber.Text = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY_UC_ACCOUNT_BOOK__LAYOUT_MAX_ITEM_NUMBER",
                    Base.ResourceLanguageManager.LanguageUCAccountBook,
                    cultureLang);
                this.spinEditMaxItemNumber.ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY_UC_ACCOUNT_BOOK__TEXTBOX_MAX_ITEM_NUMBER",
                    Base.ResourceLanguageManager.LanguageUCAccountBook,
                    cultureLang);
                this.lciMaxItemNumber.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value(
                    "IVT_LANGUAGE_KEY_UC_ACCOUNT_BOOK__TEXTBOX_MAX_ITEM_NUMBER",
                    Base.ResourceLanguageManager.LanguageUCAccountBook,
                    cultureLang);
                this.lciWorkingShift.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value(
                  "IVT_LANGUAGE_KEY_UC_ACCOUNT_BOOK__COMBOBOX_WORKING_SHIFT",
                  Base.ResourceLanguageManager.LanguageUCAccountBook,
                  cultureLang);
                // minhnq
                this.lciEInvoiceSys.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCHisAccountBookList.lciEInvoiceSys.OptionsToolTip.ToolTip", Base.ResourceLanguageManager.LanguageUCAccountBook, LanguageManager.GetCulture());
                this.lciEInvoiceSys.Text = Inventec.Common.Resource.Get.Value("UCHisAccountBookList.lciEInvoiceSys.Text", Base.ResourceLanguageManager.LanguageUCAccountBook, LanguageManager.GetCulture());
                this.lciWorkingShift.Text = Inventec.Common.Resource.Get.Value("UCHisAccountBookList.lciWorkingShift.Text", Base.ResourceLanguageManager.LanguageUCAccountBook, LanguageManager.GetCulture());
                this.chkShowAll.Properties.Caption = Inventec.Common.Resource.Get.Value("UCHisAccountBookList.chkShowAll.Properties.Caption", Base.ResourceLanguageManager.LanguageUCAccountBook, LanguageManager.GetCulture());
                this.lciCreatTimeFrom.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCHisAccountBookList.lciCreatTimeFrom.OptionsToolTip.ToolTip", Base.ResourceLanguageManager.LanguageUCAccountBook, LanguageManager.GetCulture());
                this.lciCreatTimeFrom.Text = Inventec.Common.Resource.Get.Value("UCHisAccountBookList.lciCreatTimeFrom.Text", Base.ResourceLanguageManager.LanguageUCAccountBook, LanguageManager.GetCulture());
                this.lciCreateTimeTo.Text = Inventec.Common.Resource.Get.Value("UCHisAccountBookList.lciCreateTimeTo.Text", Base.ResourceLanguageManager.LanguageUCAccountBook, LanguageManager.GetCulture());
                this.btnSearchTransaction.Text = Inventec.Common.Resource.Get.Value("UCHisAccountBookList.btnSearchTransaction.Text", Base.ResourceLanguageManager.LanguageUCAccountBook, LanguageManager.GetCulture());
                this.btnExportTransactionToExcel.Text = Inventec.Common.Resource.Get.Value("UCHisAccountBookList.btnExportTransactionToExcel.Text", Base.ResourceLanguageManager.LanguageUCAccountBook, LanguageManager.GetCulture());
                this.lciTotalIncome.Text = Inventec.Common.Resource.Get.Value("UCHisAccountBookList.lciTotalIncome.Text", Base.ResourceLanguageManager.LanguageUCAccountBook, LanguageManager.GetCulture());
                this.lciTotalRefund.Text = Inventec.Common.Resource.Get.Value("UCHisAccountBookList.lciTotalRefund.Text", Base.ResourceLanguageManager.LanguageUCAccountBook, LanguageManager.GetCulture());
                this.lciTotalDebt.Text = Inventec.Common.Resource.Get.Value("UCHisAccountBookList.lciTotalDebt.Text", Base.ResourceLanguageManager.LanguageUCAccountBook, LanguageManager.GetCulture());
                this.lciAccountingSubmission.Text = Inventec.Common.Resource.Get.Value("UCHisAccountBookList.lciAccountingSubmission.Text", Base.ResourceLanguageManager.LanguageUCAccountBook, LanguageManager.GetCulture());
                this.gridColumn_Transaction_Stt.Caption = Inventec.Common.Resource.Get.Value("UCHisAccountBookList.gridColumn_Transaction_Stt.Caption", Base.ResourceLanguageManager.LanguageUCAccountBook, LanguageManager.GetCulture());
                this.gridColumn_Transaction_TransactionCode.Caption = Inventec.Common.Resource.Get.Value("UCHisAccountBookList.gridColumn_Transaction_TransactionCode.Caption", Base.ResourceLanguageManager.LanguageUCAccountBook, LanguageManager.GetCulture());
                this.gridColumn_Transaction_Amount.Caption = Inventec.Common.Resource.Get.Value("UCHisAccountBookList.gridColumn_Transaction_Amount.Caption", Base.ResourceLanguageManager.LanguageUCAccountBook, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("UCHisAccountBookList.gridColumn8.Caption", Base.ResourceLanguageManager.LanguageUCAccountBook, LanguageManager.GetCulture());
                this.gridColumn8.ToolTip = Inventec.Common.Resource.Get.Value("UCHisAccountBookList.gridColumn8.ToolTip", Base.ResourceLanguageManager.LanguageUCAccountBook, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("UCHisAccountBookList.gridColumn6.Caption", Base.ResourceLanguageManager.LanguageUCAccountBook, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("UCHisAccountBookList.gridColumn5.Caption", Base.ResourceLanguageManager.LanguageUCAccountBook, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("UCHisAccountBookList.gridColumn4.Caption", Base.ResourceLanguageManager.LanguageUCAccountBook, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("UCHisAccountBookList.gridColumn3.Caption", Base.ResourceLanguageManager.LanguageUCAccountBook, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("UCHisAccountBookList.gridColumn2.Caption", Base.ResourceLanguageManager.LanguageUCAccountBook, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("UCHisAccountBookList.gridColumn9.Caption", Base.ResourceLanguageManager.LanguageUCAccountBook, LanguageManager.GetCulture());
                this.gc_SwipeAmount.Caption = Inventec.Common.Resource.Get.Value("UCHisAccountBookList.gc_SwipeAmount.Caption", Base.ResourceLanguageManager.LanguageUCAccountBook, LanguageManager.GetCulture());
                this.gc_SwipeAmount.ToolTip = Inventec.Common.Resource.Get.Value("UCHisAccountBookList.gc_SwipeAmount.ToolTip", Base.ResourceLanguageManager.LanguageUCAccountBook, LanguageManager.GetCulture());
                this.gc_TransferAmuont.Caption = Inventec.Common.Resource.Get.Value("UCHisAccountBookList.gc_TransferAmuont.Caption", Base.ResourceLanguageManager.LanguageUCAccountBook, LanguageManager.GetCulture());
                this.gc_TransferAmuont.ToolTip = Inventec.Common.Resource.Get.Value("UCHisAccountBookList.gc_TransferAmuont.ToolTip", Base.ResourceLanguageManager.LanguageUCAccountBook, LanguageManager.GetCulture());
                this.gridColumn_Transaction_PayFormName.Caption = Inventec.Common.Resource.Get.Value("UCHisAccountBookList.gridColumn_Transaction_PayFormName.Caption", Base.ResourceLanguageManager.LanguageUCAccountBook, LanguageManager.GetCulture());
                this.gridColumn_TRANSACTION_BANK_NAME.Caption = Inventec.Common.Resource.Get.Value("UCHisAccountBookList.gridColumn_TRANSACTION_BANK_NAME.Caption", Base.ResourceLanguageManager.LanguageUCAccountBook, LanguageManager.GetCulture());
                this.gridColumn_Transaction_Tig_TransactionCode.Caption = Inventec.Common.Resource.Get.Value("UCHisAccountBookList.gridColumn_Transaction_Tig_TransactionCode.Caption", Base.ResourceLanguageManager.LanguageUCAccountBook, LanguageManager.GetCulture());
                this.gridColumn_Transaction_TransactionTypeName.Caption = Inventec.Common.Resource.Get.Value("UCHisAccountBookList.gridColumn_Transaction_TransactionTypeName.Caption", Base.ResourceLanguageManager.LanguageUCAccountBook, LanguageManager.GetCulture());
                this.gridColumn_Transaction_DerectlyBilling.Caption = Inventec.Common.Resource.Get.Value("UCHisAccountBookList.gridColumn_Transaction_DerectlyBilling.Caption", Base.ResourceLanguageManager.LanguageUCAccountBook, LanguageManager.GetCulture());
                this.gridColumn_Transaction_Cashier.Caption = Inventec.Common.Resource.Get.Value("UCHisAccountBookList.gridColumn_Transaction_Cashier.Caption", Base.ResourceLanguageManager.LanguageUCAccountBook, LanguageManager.GetCulture());
                this.gridColumn_Transaction_TransactionTime.Caption = Inventec.Common.Resource.Get.Value("UCHisAccountBookList.gridColumn_Transaction_TransactionTime.Caption", Base.ResourceLanguageManager.LanguageUCAccountBook, LanguageManager.GetCulture());
                this.gridColumn_Transaction_TransactionTime.ToolTip = Inventec.Common.Resource.Get.Value("UCHisAccountBookList.gridColumn_Transaction_TransactionTime.ToolTip", Base.ResourceLanguageManager.LanguageUCAccountBook, LanguageManager.GetCulture());
                this.gridColumn_Transaction_CashierRoomName.Caption = Inventec.Common.Resource.Get.Value("UCHisAccountBookList.gridColumn_Transaction_CashierRoomName.Caption", Base.ResourceLanguageManager.LanguageUCAccountBook, LanguageManager.GetCulture());
                this.gridColumn_Transaction_NumOrder.Caption = Inventec.Common.Resource.Get.Value("UCHisAccountBookList.gridColumn_Transaction_NumOrder.Caption", Base.ResourceLanguageManager.LanguageUCAccountBook, LanguageManager.GetCulture());
                this.gridColumn_Transaction_AccountBookCode.Caption = Inventec.Common.Resource.Get.Value("UCHisAccountBookList.gridColumn_Transaction_AccountBookCode.Caption", Base.ResourceLanguageManager.LanguageUCAccountBook, LanguageManager.GetCulture());
                this.gridColumn_Transaction_AccountBookName.Caption = Inventec.Common.Resource.Get.Value("UCHisAccountBookList.gridColumn_Transaction_AccountBookName.Caption", Base.ResourceLanguageManager.LanguageUCAccountBook, LanguageManager.GetCulture());
                this.gridColumn_Transaction_TreatmentCode.Caption = Inventec.Common.Resource.Get.Value("UCHisAccountBookList.gridColumn_Transaction_TreatmentCode.Caption", Base.ResourceLanguageManager.LanguageUCAccountBook, LanguageManager.GetCulture());
                this.gridColumn_Transaction_VirPatientName.Caption = Inventec.Common.Resource.Get.Value("UCHisAccountBookList.gridColumn_Transaction_VirPatientName.Caption", Base.ResourceLanguageManager.LanguageUCAccountBook, LanguageManager.GetCulture());
                this.gridColumn_Transaction_Dob.Caption = Inventec.Common.Resource.Get.Value("UCHisAccountBookList.gridColumn_Transaction_Dob.Caption", Base.ResourceLanguageManager.LanguageUCAccountBook, LanguageManager.GetCulture());
                this.gridColumn_Transaction_GenderName.Caption = Inventec.Common.Resource.Get.Value("UCHisAccountBookList.gridColumn_Transaction_GenderName.Caption", Base.ResourceLanguageManager.LanguageUCAccountBook, LanguageManager.GetCulture());
                this.gridColumn_Transaction_PatientCode.Caption = Inventec.Common.Resource.Get.Value("UCHisAccountBookList.gridColumn_Transaction_PatientCode.Caption", Base.ResourceLanguageManager.LanguageUCAccountBook, LanguageManager.GetCulture());
                this.gridColumn_Transaction_NationalTransactionCode.Caption = Inventec.Common.Resource.Get.Value("UCHisAccountBookList.gridColumn_Transaction_NationalTransactionCode.Caption", Base.ResourceLanguageManager.LanguageUCAccountBook, LanguageManager.GetCulture());
                this.gridColumn_Transaction_CreateTime.Caption = Inventec.Common.Resource.Get.Value("UCHisAccountBookList.gridColumn_Transaction_CreateTime.Caption", Base.ResourceLanguageManager.LanguageUCAccountBook, LanguageManager.GetCulture());
                this.gridColumn_Transaction_Creator.Caption = Inventec.Common.Resource.Get.Value("UCHisAccountBookList.gridColumn_Transaction_Creator.Caption", Base.ResourceLanguageManager.LanguageUCAccountBook, LanguageManager.GetCulture());
                this.gridColumn_Transaction_ModifyTime.Caption = Inventec.Common.Resource.Get.Value("UCHisAccountBookList.gridColumn_Transaction_ModifyTime.Caption", Base.ResourceLanguageManager.LanguageUCAccountBook, LanguageManager.GetCulture());
                this.gridColumn_Transaction_Modifier.Caption = Inventec.Common.Resource.Get.Value("UCHisAccountBookList.gridColumn_Transaction_Modifier.Caption", Base.ResourceLanguageManager.LanguageUCAccountBook, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("UCHisAccountBookList.gridColumn1.Caption", Base.ResourceLanguageManager.LanguageUCAccountBook, LanguageManager.GetCulture());
                this.gridColumn1.ToolTip = Inventec.Common.Resource.Get.Value("UCHisAccountBookList.gridColumn1.ToolTip", Base.ResourceLanguageManager.LanguageUCAccountBook, LanguageManager.GetCulture());
                this.chkLuonTachTheoTungSo.Properties.Caption = Inventec.Common.Resource.Get.Value("UCHisAccountBookList.chkLuonTachTheoTungSo.Properties.Caption", Base.ResourceLanguageManager.LanguageUCAccountBook, LanguageManager.GetCulture());
                this.chkLuonTachTheoTungSo.ToolTip = Inventec.Common.Resource.Get.Value("UCHisAccountBookList.chkLuonTachTheoTungSo.ToolTip", Base.ResourceLanguageManager.LanguageUCAccountBook, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("UCHisAccountBookList.gridColumn7.Caption", Base.ResourceLanguageManager.LanguageUCAccountBook, LanguageManager.GetCulture());
                this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("UCHisAccountBookList.gridColumn10.Caption", Base.ResourceLanguageManager.LanguageUCAccountBook, LanguageManager.GetCulture());

                this.layoutControlItem12.Text = Inventec.Common.Resource.Get.Value("UCHisAccountBookList.layoutControlItem12.Text", Base.ResourceLanguageManager.LanguageUCAccountBook, LanguageManager.GetCulture());
                this.layoutControlItem19.Text = Inventec.Common.Resource.Get.Value("UCHisAccountBookList.layoutControlItem19.Text", Base.ResourceLanguageManager.LanguageUCAccountBook, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void EnableButton(int action)
        {
            try
            {
                UpdateItemsReadOnly();
                btnCancel.Enabled = (action == GlobalVariables.ActionEdit);
                btnSave.Enabled = (action == GlobalVariables.ActionAdd || action == GlobalVariables.ActionEdit);
                btnAdd.Enabled = true;
                chkNotGenOrder.Enabled = true;

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateItemsReadOnly()
        {
            bool IsEdit = !(ActionType == GlobalVariables.ActionView);
            if (!layoutControlCRUD.IsInitialized) return;
            layoutControlCRUD.BeginUpdate();
            try
            {
                foreach (DevExpress.XtraLayout.BaseLayoutItem item in layoutControlCRUD.Items)
                {
                    DevExpress.XtraLayout.LayoutControlItem lci = item as DevExpress.XtraLayout.LayoutControlItem;
                    if (lci != null && lci.Control != null)
                    {
                        DevExpress.XtraEditors.BaseEdit be = lci.Control as DevExpress.XtraEditors.BaseEdit;
                        if (be != null)
                        {
                            be.Properties.ReadOnly = !IsEdit;
                        }
                    }
                }
            }
            finally
            {
                layoutControlCRUD.EndUpdate();
            }
        }

        private void FillDataToCbo()
        {
            try
            {
                cboPatientTypeId.Properties.DataSource = Base.GlobalStore.ListPatientType;
                cboPatientTypeId.Properties.DisplayMember = "PATIENT_TYPE_NAME";
                cboPatientTypeId.Properties.ValueMember = "ID";
                cboPatientTypeId.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboPatientTypeId.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Default;
                cboPatientTypeId.Properties.ImmediatePopup = true;
                cboPatientTypeId.ForceInitialize();
                cboPatientTypeId.Properties.View.Columns.Clear();
                cboPatientTypeId.Properties.PopupFormSize = new Size(200, 250);

                DevExpress.XtraGrid.Columns.GridColumn aColumnCode = cboPatientTypeId.Properties.View.Columns.AddField("PATIENT_TYPE_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 50;

                DevExpress.XtraGrid.Columns.GridColumn aColumnName = cboPatientTypeId.Properties.View.Columns.AddField("PATIENT_TYPE_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 150;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToCboWorkingShift()
        {
            try
            {
                var data = BackendDataWorker.Get<HIS_WORKING_SHIFT>().FirstOrDefault(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE);
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("WORKING_SHIFT_CODE", "", 2, 1));
                columnInfos.Add(new ColumnInfo("WORKING_SHIFT_NAME", "", 100, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("WORKING_SHIFT_NAME", "ID", columnInfos, false, 102);
                ControlEditorLoader.Load(cboWorkingShift, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToCboBillType()
        {
            try
            {
                cboBillType.Properties.DataSource = Base.GlobalStore.listBillType;
                cboBillType.Properties.DisplayMember = "BillTypeName";
                cboBillType.Properties.ValueMember = "ID";
                cboBillType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboBillType.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Default;
                cboBillType.Properties.ImmediatePopup = true;
                cboBillType.ForceInitialize();
                cboBillType.Properties.View.Columns.Clear();
                cboBillType.Properties.PopupFormSize = new Size(200, 250);

                DevExpress.XtraGrid.Columns.GridColumn aColumnName = cboBillType.Properties.View.Columns.AddField("BillTypeName");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 150;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToCboEInvoiceSys()
        {
            try
            {
                cboEInvoiceSys.Properties.DataSource = BackendDataWorker.Get<HIS_EINVOICE_TYPE>();
                cboEInvoiceSys.Properties.DisplayMember = "EINVOICE_TYPE_NAME";
                cboEInvoiceSys.Properties.ValueMember = "ID";
                cboEInvoiceSys.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboEInvoiceSys.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Default;
                cboEInvoiceSys.Properties.ImmediatePopup = true;
                cboEInvoiceSys.ForceInitialize();
                cboEInvoiceSys.Properties.View.Columns.Clear();
                cboEInvoiceSys.Properties.PopupFormSize = new Size(200, 250);

                DevExpress.XtraGrid.Columns.GridColumn aColumnCode = cboEInvoiceSys.Properties.View.Columns.AddField("EINVOICE_TYPE_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 50;

                DevExpress.XtraGrid.Columns.GridColumn aColumnName = cboEInvoiceSys.Properties.View.Columns.AddField("EINVOICE_TYPE_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 150;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadAcsControls()
        {
            try
            {
                this.controlAcs = new List<ACS.EFMODEL.DataModels.ACS_CONTROL>();

                if (GlobalVariables.AcsAuthorizeSDO != null)
                {
                    this.controlAcs = GlobalVariables.AcsAuthorizeSDO.ControlInRoles;
                    var acsControl = controlAcs.Exists(o => o.CONTROL_CODE == ControlCode.BtnShowAll);
                    if (controlAcs != null && controlAcs.Exists(o => o.CONTROL_CODE == "HIS000022"))
                    {
                        chkShowAll.Enabled = true;
                    }
                    else
                    {
                        chkShowAll.Enabled = false;
                        chkShowAll.Checked = false;
                    }
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
                txtKeyWord.Text = "";
                SetDefaultControlDateTime();
                gridControlAccountBook.DataSource = null;
                txtAccountBookCode.Focus();
                LoadAcsControls();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultControlDateTime()
        {
            try
            {
                dtCreateTimeFrom.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.StartDay() ?? 0);
                dtCreateTimeTo.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.EndDay() ?? 0);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridAccountBookList()
        {
            try
            {
                CommonParam paramCommon = new CommonParam();
                
                HisAccountBookViewFilter filter = new HisAccountBookViewFilter();
                filter.KEY_WORD = txtKeyWord.Text.Trim();
                gridViewAccountBook.BeginUpdate();

                ListAccountBook = new BackendAdapter(paramCommon).Get<List<V_HIS_ACCOUNT_BOOK>>(HisRequestUriStore.HIS_ACCOUNT_BOOK_GETVIEW, ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);

                List<V_HIS_ACCOUNT_BOOK> accountBooks = new List<V_HIS_ACCOUNT_BOOK>();
                if (chkShowAll.Checked == false)
                {
                    if (ListAccountBook != null && ListAccountBook.Count > 0)
                    {
                        string loginname = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                        accountBooks = ListAccountBook.Where(o => o.CREATOR == loginname).ToList();

                        HisUserAccountBookFilter userfilter = new HisUserAccountBookFilter();
                        userfilter.LOGINNAME__EXACT = loginname;
                        List<HIS_USER_ACCOUNT_BOOK> ListUserAccountBook = new BackendAdapter(paramCommon).Get<List<HIS_USER_ACCOUNT_BOOK>>(HisRequestUriStore.HIS_USER_ACCOUNT_BOOK_GET, ApiConsumers.MosConsumer, userfilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);

                        if (ListUserAccountBook != null && ListUserAccountBook.Count > 0)
                        {
                            List<long> accountBookIds = ListUserAccountBook.Select(o => o.ACCOUNT_BOOK_ID).ToList();
                            List<V_HIS_ACCOUNT_BOOK> hisAccountBooks = ListAccountBook.Where(o => accountBookIds.Exists(p => p == o.ID)).ToList();
                            if (hisAccountBooks != null && hisAccountBooks.Count > 0)
                            {
                                accountBooks.AddRange(hisAccountBooks);
                            }
                        }
                        if (this.currentModule != null)
                        {
                            var cashierRoom = BackendDataWorker.Get<V_HIS_CASHIER_ROOM>().FirstOrDefault(o => o.ROOM_ID == currentModule.RoomId);
                            HisCaroAccountBookFilter carofilter = new HisCaroAccountBookFilter();
                            carofilter.CASHIER_ROOM_ID = cashierRoom != null ? (long?)cashierRoom.ID : null;
                            List<HIS_CARO_ACCOUNT_BOOK> ListCaroAccountBook = new BackendAdapter(paramCommon).Get<List<HIS_CARO_ACCOUNT_BOOK>>(HisRequestUriStore.HIS_CARO_ACCOUNT_BOOK_GET, ApiConsumers.MosConsumer, carofilter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                            if (ListCaroAccountBook != null && ListCaroAccountBook.Count > 0)
                            {
                                List<long> accountBookIds = ListCaroAccountBook.Select(o => o.ACCOUNT_BOOK_ID).ToList();
                                List<V_HIS_ACCOUNT_BOOK> hisAccountBookByCaros = ListAccountBook.Where(o => accountBookIds.Exists(p => p == o.ID)).ToList();
                                if (hisAccountBookByCaros != null && hisAccountBookByCaros.Count > 0)
                                {
                                    accountBooks.AddRange(hisAccountBookByCaros);
                                }
                            }
                        }
                    }
                }
                if (chkShowAll.Checked == true)
                {
                    if (ListAccountBook != null && ListAccountBook.Count > 0)
                    {
                        ListAccountBook = ListAccountBook.OrderByDescending(o => o.MODIFY_TIME).ThenByDescending(o => o.ID).ToList();
                        gridViewAccountBook.GridControl.DataSource = ListAccountBook;
                        DataAccountBook = ListAccountBook.FirstOrDefault();
                        FillDataToControl(DataAccountBook);
                        FillDataToGridTransaction();

                    }
                    else
                    {
                        EnableButton(GlobalVariables.ActionView);
                        DataAccountBook = null;
                        gridViewAccountBook.GridControl.DataSource = null;
                        FillDataToControl(DataAccountBook);
                        FillDataToGridTransaction();
                    }
                }
                else if (chkShowAll.Checked == false)
                {
                    if (accountBooks != null && accountBooks.Count > 0)
                    {
                        accountBooks = accountBooks.OrderByDescending(o => o.MODIFY_TIME).ThenByDescending(o => o.ID).Distinct().ToList();
                        gridViewAccountBook.GridControl.DataSource = accountBooks;
                        DataAccountBook = accountBooks.FirstOrDefault();
                        FillDataToControl(DataAccountBook);
                        FillDataToGridTransaction();
                    }
                    else
                    {
                        EnableButton(GlobalVariables.ActionView);
                        DataAccountBook = null;
                        gridViewAccountBook.GridControl.DataSource = null;
                        FillDataToControl(DataAccountBook);
                        FillDataToGridTransaction();
                    }
                }
                gridViewAccountBook.EndUpdate();

                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                gridViewAccountBook.EndUpdate();
            }
        }
        #region show and update EINVOICE_PAGE_SIZE
        private string getEINVOICE(string code)
        {
            try
            {
                CommonParam param = new CommonParam();
                string rs = "";
                HisAccountBookFilter filter = new HisAccountBookFilter();
                filter.KEY_WORD = code;
                List<HIS_ACCOUNT_BOOK> result = new BackendAdapter(param).Get<List<HIS_ACCOUNT_BOOK>>(HisRequestUriStore.HIS_ACCOUNT_BOOK_GET, ApiConsumers.MosConsumer, filter, param);
                if (result != null)
                {
                    var data = result.FirstOrDefault(o => o.ACCOUNT_BOOK_CODE == code);
                    rs = data.EINVOICE_PAGE_SIZE;
                }
                return rs;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool UpdateEINVOICE(V_HIS_ACCOUNT_BOOK data)
        {
            try
            {
                CommonParam param = new CommonParam();
                bool sucess = false;
                HisAccountBookFilter filter = new HisAccountBookFilter();
                filter.KEY_WORD = data.ACCOUNT_BOOK_NAME;
                var result = new BackendAdapter(param).Get<List<HIS_ACCOUNT_BOOK>>(HisRequestUriStore.HIS_ACCOUNT_BOOK_GET, ApiConsumers.MosConsumer, filter, param);
                if (result != null)
                {
                    var dataUpdate = result.FirstOrDefault(s => s.ACCOUNT_BOOK_CODE == data.ACCOUNT_BOOK_CODE);
                    if (!string.IsNullOrEmpty(txtKhoGiay.Text.Trim())) dataUpdate.EINVOICE_PAGE_SIZE = txtKhoGiay.Text.Trim();
                    var resultAPI = new BackendAdapter(param).Post<MOS.EFMODEL.DataModels.HIS_ACCOUNT_BOOK>(HisRequestUriStore.HIS_ACCOUNT_BOOK_UPDATE, ApiConsumers.MosConsumer, dataUpdate, param);
                    if (resultAPI != null)
                    {
                        sucess = true;
                    }
                }
                return sucess;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
        private void FillDataToControl(V_HIS_ACCOUNT_BOOK data)
        {
            try
            {
                if (data != null)
                {
                    txtAccountBookCode.Text = data.ACCOUNT_BOOK_CODE;
                    txtAccountBookName.Text = data.ACCOUNT_BOOK_NAME;
                    spinCount.Value = data.TOTAL;
                    spinFromNumberOrder.Value = data.FROM_NUM_ORDER;
                    txtTemplateCode.Text = data.TEMPLATE_CODE;
                    txtSymbolCode.Text = data.SYMBOL_CODE;
                    spinNumOrder.EditValue = data.NUM_ORDER;
                    dtReleaseTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.RELEASE_TIME ?? 0);
                    cboWorkingShift.EditValue = data.WORKING_SHIFT_ID;
                    if (data.ACCOUNT_BOOK_CODE != null)
                    {
                        txtKhoGiay.Text = getEINVOICE(data.ACCOUNT_BOOK_CODE);
                    }
                    else txtKhoGiay.Text = "";
                    if (data.WORKING_SHIFT_ID != null)
                    {
                        cboWorkingShift.EditValue = data.WORKING_SHIFT_ID;
                        cboWorkingShift.Properties.Buttons[1].Visible = true;
                    }
                    else
                    {
                        cboWorkingShift.EditValue = null;
                        cboWorkingShift.Properties.Buttons[1].Visible = false;
                    }

                    if (data.BILL_TYPE_ID != null)
                    {
                        var type = Base.GlobalStore.listBillType.FirstOrDefault(o => o.ID == data.BILL_TYPE_ID);
                        if (type != null)
                        {
                            cboBillType.EditValue = type.ID;
                            cboBillType.Properties.Buttons[1].Visible = true;
                        }
                        else
                        {
                            cboBillType.EditValue = null;
                            cboBillType.Properties.Buttons[1].Visible = false;
                        }
                    }
                    else
                    {
                        cboBillType.EditValue = null;
                        cboBillType.Properties.Buttons[1].Visible = false;
                    }

                    txtDescription.Text = data.DESCRIPTION;
                    spinEditMaxItemNumber.EditValue = data.MAX_ITEM_NUM_PER_TRANS;
                    if (data.CURRENT_NUM_ORDER > 0) spinFromNumberOrder.Properties.ReadOnly = true;
                    chkForDeposit.Checked = ((data.IS_FOR_DEPOSIT ?? 0) == Base.GlobalStore.IS_TRUE ? true : false);
                    chkForBill.Checked = ((data.IS_FOR_BILL ?? 0) == Base.GlobalStore.IS_TRUE ? true : false);
                    chkForOtherSale.Checked = ((data.IS_FOR_OTHER_SALE ?? 0) == Base.GlobalStore.IS_TRUE ? true : false);
                    chkForRepay.Checked = ((data.IS_FOR_REPAY ?? 0) == Base.GlobalStore.IS_TRUE ? true : false);
                    chkForDebt.Checked = ((data.IS_FOR_DEBT ?? 0) == Base.GlobalStore.IS_TRUE ? true : false);
                    chkNotGenOrder.Checked = ((data.IS_NOT_GEN_TRANSACTION_ORDER ?? 0) == Base.GlobalStore.IS_TRUE ? true : false);
                    chkLuonTachTheoTungSo.Checked = ((data.NUM_ORDER_SPLIT_BY_BOOK ?? 0) == Base.GlobalStore.IS_TRUE ? true : false);
                    cboEInvoiceSys.EditValue = data.EINVOICE_TYPE_ID;
                }
                else
                {
                    txtAccountBookCode.Text = null;
                    txtAccountBookName.Text = null;
                    spinCount.Value = 0;
                    spinFromNumberOrder.Value = 0;
                    cboBillType.EditValue = null;
                    cboBillType.Properties.Buttons[1].Visible = false;
                    txtTemplateCode.Text = null;
                    txtSymbolCode.Text = null;
                    spinNumOrder.EditValue = 0;
                    dtReleaseTime.EditValue = null;
                    cboPatientTypeId.EditValue = null;
                    cboPatientTypeId.Properties.Buttons[1].Visible = false;
                    txtPatientTypeId.Text = "";
                    cboWorkingShift.EditValue = null;
                    cboWorkingShift.Properties.Buttons[1].Visible = false;
                    cboEInvoiceSys.EditValue = null;

                    txtDescription.Text = "";
                    spinEditMaxItemNumber.Value = 0;
                    chkForDeposit.Checked = false;
                    chkForBill.Checked = false;
                    chkForOtherSale.Checked = false;
                    chkForRepay.Checked = false;
                    chkForDebt.Checked = false;
                    chkNotGenOrder.Checked = false;
                    chkLuonTachTheoTungSo.Checked = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateDataFromAccountBook(V_HIS_ACCOUNT_BOOK data)
        {
            try
            {
                data.ACCOUNT_BOOK_CODE = txtAccountBookCode.Text.Trim();
                data.ACCOUNT_BOOK_NAME = txtAccountBookName.Text.Trim();
                data.TOTAL = Inventec.Common.TypeConvert.Parse.ToInt64(spinCount.Value.ToString());
                data.FROM_NUM_ORDER = Inventec.Common.TypeConvert.Parse.ToInt64(spinFromNumberOrder.Value.ToString());
                data.TEMPLATE_CODE = txtTemplateCode.Text.Trim();
                data.SYMBOL_CODE = txtSymbolCode.Text.Trim();
                data.NUM_ORDER = Inventec.Common.TypeConvert.Parse.ToInt64(spinNumOrder.Value.ToString());
                if (chkLuonTachTheoTungSo.Checked == true)
                {
                    data.NUM_ORDER_SPLIT_BY_BOOK = 1;
                }
                else
                {
                    data.NUM_ORDER_SPLIT_BY_BOOK = null;
                }
                if (dtReleaseTime.DateTime != DateTime.MinValue)
                {
                    data.RELEASE_TIME = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtReleaseTime.DateTime);
                }
                else
                    data.RELEASE_TIME = null;
                //if (cboPatientTypeId.EditValue != null)
                //{
                //    data.PATIENT_TYPE_ID__DELETE = Inventec.Common.TypeConvert.Parse.ToInt64(cboPatientTypeId.EditValue.ToString());
                //}

                if (cboBillType.EditValue != null)
                {
                    data.BILL_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboBillType.EditValue.ToString());
                }

                if (cboWorkingShift.EditValue != null)
                {
                    data.WORKING_SHIFT_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboWorkingShift.EditValue.ToString());
                }
                else
                    data.WORKING_SHIFT_ID = null;
                data.DESCRIPTION = txtDescription.Text.Trim();
                data.MAX_ITEM_NUM_PER_TRANS = Inventec.Common.TypeConvert.Parse.ToInt64(spinEditMaxItemNumber.Value.ToString());
                if (chkForBill.Checked) data.IS_FOR_BILL = Base.GlobalStore.IS_TRUE;
                else data.IS_FOR_BILL = null;
                if (chkForOtherSale.Checked) data.IS_FOR_OTHER_SALE = Base.GlobalStore.IS_TRUE;
                else data.IS_FOR_OTHER_SALE = null;
                if (chkForDeposit.Checked) data.IS_FOR_DEPOSIT = Base.GlobalStore.IS_TRUE;
                else data.IS_FOR_DEPOSIT = null;
                if (chkForRepay.Checked) data.IS_FOR_REPAY = Base.GlobalStore.IS_TRUE;
                else data.IS_FOR_REPAY = null;
                if (chkForDebt.Checked) data.IS_FOR_DEBT = Base.GlobalStore.IS_TRUE;
                else data.IS_FOR_DEBT = null;
                if (chkNotGenOrder.Checked) data.IS_NOT_GEN_TRANSACTION_ORDER = Base.GlobalStore.IS_TRUE;
                else data.IS_NOT_GEN_TRANSACTION_ORDER = null;

                if (cboEInvoiceSys.EditValue != null)
                {
                    data.EINVOICE_TYPE_ID = Inventec.Common.TypeConvert.Parse.ToInt64(cboEInvoiceSys.EditValue.ToString());
                }
                else
                {
                    data.EINVOICE_TYPE_ID = null;
                }
            }

            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void EditGridClick(V_HIS_ACCOUNT_BOOK data)
        {
            if (data != null)
            {
                txtAccountBookName.SelectAll();
                txtAccountBookName.Focus();
                this.ActionType = GlobalVariables.ActionEdit;
                EnableButton(this.ActionType);
                FillDataToControl(data);
                RemoveError();
                positionHandle = -1;
                if (data.TEMPLATE_CODE != null)
                {
                    txtTemplateCode.ReadOnly = true;
                }
                if (data.SYMBOL_CODE != null)
                {
                    txtSymbolCode.ReadOnly = true;
                }
                if (data.EINVOICE_TYPE_ID.HasValue)
                {
                    cboEInvoiceSys.ReadOnly = true;
                }
                if (data.CURRENT_NUM_ORDER != null && data.CURRENT_NUM_ORDER > 0)
                {
                    spinFromNumberOrder.ReadOnly = true;
                    txtTemplateCode.ReadOnly = true;
                    txtSymbolCode.ReadOnly = true;
                    //cboEInvoiceSys.ReadOnly = true;
                }
                //CheckReadOnly();
            }
        }

        private void FillDataToGridTransaction()
        {
            try
            {
                if (this.DataAccountBook == null)
                {
                    this.ListTransaction = null;
                    gridViewTransaction.GridControl.DataSource = null;
                    SetDataToLabels_TransactionCaculated();
                    return;
                }
                CommonParam paramCommon = new CommonParam();

                HisTransactionViewFilter filter = new HisTransactionViewFilter();
                filter.ORDER_FIELD = "MODIFY_TIME";
                filter.ORDER_DIRECTION = "DESC";
                SetFilterTransaction(ref filter);
                gridViewTransaction.BeginUpdate();
                this.ListTransaction = new BackendAdapter(paramCommon).Get<List<V_HIS_TRANSACTION>>(HisRequestUriStore.HIS_TRANSACTION_GETVIEW, ApiConsumers.MosConsumer, filter, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);
                if (ListTransaction != null && ListTransaction.Count > 0)
                {
                    gridViewTransaction.GridControl.DataSource = ListTransaction;
                }
                else
                {
                    gridViewTransaction.GridControl.DataSource = null;
                }
                gridViewTransaction.EndUpdate();
                SetDataToLabels_TransactionCaculated();
                #region Process has exception
                HIS.Desktop.Controls.Session.SessionManager.ProcessTokenLost(paramCommon);
                #endregion
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                gridViewTransaction.EndUpdate();
            }
        }

        private void SetFilterTransaction(ref HisTransactionViewFilter filter)
        {
            try
            {
                if (this.DataAccountBook != null)
                {
                    filter.ACCOUNT_BOOK_ID = this.DataAccountBook.ID;
                }


                if (chkTransactionTime.Checked)
                {
                    if (dtCreateTimeFrom.EditValue != null && dtCreateTimeFrom.DateTime != DateTime.MinValue)
                    {
                        filter.CREATE_TIME_FROM = Convert.ToInt64(dtCreateTimeFrom.DateTime.ToString("yyyyMMddHHmm") + "00");

                    }
                    if (dtCreateTimeTo.EditValue != null && dtCreateTimeTo.DateTime != DateTime.MinValue)
                    {
                        filter.CREATE_TIME_TO = Convert.ToInt64(dtCreateTimeTo.DateTime.ToString("yyyyMMddHHmm") + "59");

                    }

                }
                if (chkNumOrder.Checked)
                {
                    if (!string.IsNullOrEmpty(txtNumOrderForm.Text))
                    {
                        filter.NUM_ORDER_FROM = Convert.ToInt64(txtNumOrderForm.Text.Trim());
                    }
                    if (!string.IsNullOrEmpty(txtNumOrderTo.Text))
                    {
                        filter.NUM_ORDER_TO = Convert.ToInt64(txtNumOrderTo.Text.Trim());
                    }
                }


            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDataToLabels_TransactionCaculated()
        {
            try
            {
                if (this.ListTransaction != null)
                {
                    var totalIncome = ListTransaction.Where(o => o.IS_CANCEL != 1
                                                            && (o.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT
                                                            || o.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TU)
                                                            ).Sum(o => o.AMOUNT - (o.KC_AMOUNT ?? 0) - (o.TDL_BILL_FUND_AMOUNT ?? 0) - (o.EXEMPTION ?? 0));
                    var totalRefund = ListTransaction.Where(o => o.IS_CANCEL != 1
                                                            && o.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__HU
                                                            ).Sum(o => o.AMOUNT);
                    var totalDebt = ListTransaction.Where(o => o.IS_CANCEL != 1
                                                            && o.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__NO
                                                            ).Sum(o => o.AMOUNT);
                    var accountingSubmission = totalIncome - totalRefund;

                    if (totalIncome != 0)
                        lblTotalIncome.Text = Inventec.Common.Number.Convert.NumberToString(totalIncome, ConfigApplications.NumberSeperator);
                    else
                        lblTotalIncome.Text = "0";

                    if (totalRefund != 0)
                        lblTotalRefund.Text = Inventec.Common.Number.Convert.NumberToString(totalRefund, ConfigApplications.NumberSeperator);
                    else
                        lblTotalRefund.Text = "0";

                    if (totalDebt != 0)
                        lblTotalDebt.Text = Inventec.Common.Number.Convert.NumberToString(totalDebt, ConfigApplications.NumberSeperator);
                    else
                        lblTotalDebt.Text = "0";

                    if (accountingSubmission != 0)
                        lblAccountingSubmission.Text = Inventec.Common.Number.Convert.NumberToString(accountingSubmission, ConfigApplications.NumberSeperator);
                    else
                        lblAccountingSubmission.Text = "0";
                }
                else
                {
                    lblTotalIncome.Text = "0";
                    lblTotalRefund.Text = "0";
                    lblTotalDebt.Text = "0";
                    lblAccountingSubmission.Text = "0";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewAccountBook_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.IsGetData)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                    if (e.Column.FieldName == "STT")
                    {
                        e.Value = e.ListSourceRowIndex + 1;
                    }
                    if (e.Column.FieldName == "FOR_DEPOSIT_IMG")
                    {
                        DevExpress.Utils.ImageCollection images = new DevExpress.Utils.ImageCollection();
                        short type = Inventec.Common.TypeConvert.Parse.ToInt16((view.GetRowCellValue(e.ListSourceRowIndex, "IS_FOR_DEPOSIT") ?? "0").ToString());
                        if (type == Base.GlobalStore.IS_TRUE)
                        {
                            e.Value = imageListIcon.Images[0];
                        }
                        else
                        {
                            e.Value = null;
                        }
                    }
                    else if (e.Column.FieldName == "FOR_BILL_IMG")
                    {
                        DevExpress.Utils.ImageCollection images = new DevExpress.Utils.ImageCollection();
                        short type = Inventec.Common.TypeConvert.Parse.ToInt16((view.GetRowCellValue(e.ListSourceRowIndex, "IS_FOR_BILL") ?? "0").ToString());
                        if (type == Base.GlobalStore.IS_TRUE)
                        {
                            e.Value = imageListIcon.Images[1];
                        }
                        else
                        {
                            e.Value = null;
                        }
                    }
                    else if (e.Column.FieldName == "FOR_OTHER_SALE_IMG")
                    {
                        DevExpress.Utils.ImageCollection images = new DevExpress.Utils.ImageCollection();
                        short type = Inventec.Common.TypeConvert.Parse.ToInt16((view.GetRowCellValue(e.ListSourceRowIndex, "IS_FOR_OTHER_SALE") ?? "0").ToString());
                        if (type == Base.GlobalStore.IS_TRUE)
                        {
                            e.Value = imageListIcon.Images[5];
                        }
                        else
                        {
                            e.Value = null;
                        }
                    }
                    else if (e.Column.FieldName == "FOR_REPAY_IMG")
                    {
                        DevExpress.Utils.ImageCollection images = new DevExpress.Utils.ImageCollection();
                        short type = Inventec.Common.TypeConvert.Parse.ToInt16((view.GetRowCellValue(e.ListSourceRowIndex, "IS_FOR_REPAY") ?? "0").ToString());
                        if (type == Base.GlobalStore.IS_TRUE)
                        {
                            e.Value = imageListIcon.Images[2];
                        }
                        else
                        {
                            e.Value = null;
                        }
                    }
                    else if (e.Column.FieldName == "FOR_DEBT_IMG")
                    {
                        DevExpress.Utils.ImageCollection images = new DevExpress.Utils.ImageCollection();
                        short type = Inventec.Common.TypeConvert.Parse.ToInt16((view.GetRowCellValue(e.ListSourceRowIndex, "IS_FOR_DEBT") ?? "0").ToString());
                        if (type == Base.GlobalStore.IS_TRUE)
                        {
                            e.Value = imageListIcon.Images[3];
                        }
                        else
                        {
                            e.Value = null;
                        }
                    }
                    else if (e.Column.FieldName == "IS_ACTIVE_DISPLAY")
                    {
                        short type = Inventec.Common.TypeConvert.Parse.ToInt16((view.GetRowCellValue(e.ListSourceRowIndex, "IS_ACTIVE") ?? "").ToString());
                        if (type == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                        {
                            e.Value = "Hoạt động";
                        }
                        else
                        {
                            e.Value = "Tạm khóa";
                        }
                    }
                    else if (e.Column.FieldName == "CREATE_TIME_DISPLAY")
                    {
                        long createTime = Inventec.Common.TypeConvert.Parse.ToInt64((view.GetRowCellValue(e.ListSourceRowIndex, "CREATE_TIME") ?? "").ToString());
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(createTime);
                    }
                    else if (e.Column.FieldName == "MODIFY_TIME_DISPLAY")
                    {
                        long modifyTime = Inventec.Common.TypeConvert.Parse.ToInt64((view.GetRowCellValue(e.ListSourceRowIndex, "MODIFY_TIME") ?? "").ToString());
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(modifyTime);
                    }
                    else if (e.Column.FieldName == "TO_NUM_ORDER")
                    {
                        decimal fromNumOrder = Inventec.Common.TypeConvert.Parse.ToDecimal((view.GetRowCellValue(e.ListSourceRowIndex, "FROM_NUM_ORDER") ?? "").ToString());
                        decimal total = Inventec.Common.TypeConvert.Parse.ToDecimal((view.GetRowCellValue(e.ListSourceRowIndex, "TOTAL") ?? "").ToString());
                        e.Value = (int)(fromNumOrder + total - 1);
                    }
                    else if (e.Column.FieldName == "RELEASE_TIME_DISPLAY")
                    {
                        long releaseTime = Inventec.Common.TypeConvert.Parse.ToInt64((view.GetRowCellValue(e.ListSourceRowIndex, "RELEASE_TIME") ?? "").ToString());
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(releaseTime);
                    }
                    else if (e.Column.FieldName == "BILL_TYPE_DISPLAY")
                    {
                        long patientTypeId = Inventec.Common.TypeConvert.Parse.ToInt64((view.GetRowCellValue(e.ListSourceRowIndex, "BILL_TYPE_ID") ?? "").ToString());
                        var patientType = Base.GlobalStore.listBillType.FirstOrDefault(o => o.ID == patientTypeId);
                        if (patientType != null)
                        {
                            e.Value = patientType.BillTypeName;
                        }
                        else
                            e.Value = null;
                    }
                    else if (e.Column.FieldName == "WORKING_SHIFT_NAME")
                    {
                        try
                        {
                            long workingShiftId = Inventec.Common.TypeConvert.Parse.ToInt64((view.GetRowCellValue(e.ListSourceRowIndex, "WORKING_SHIFT_ID") ?? "").ToString());
                            var workingShift = BackendDataWorker.Get<HIS_WORKING_SHIFT>().FirstOrDefault(o => o.ID == workingShiftId);
                            if (workingShift != null)
                            {
                                e.Value = workingShift.WORKING_SHIFT_NAME;
                            }

                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    else if (e.Column.FieldName == "EINVOICE_TYPE_NAME")
                    {
                        try
                        {
                            long invoiceType = Inventec.Common.TypeConvert.Parse.ToInt64((view.GetRowCellValue(e.ListSourceRowIndex, "EINVOICE_TYPE_ID") ?? "").ToString());
                            var type = BackendDataWorker.Get<HIS_EINVOICE_TYPE>().FirstOrDefault(o => o.ID == invoiceType);
                            if (type != null)
                            {
                                e.Value = type.EINVOICE_TYPE_NAME;
                            }
                        }
                        catch (Exception ex)
                        {
                            Inventec.Common.Logging.LogSystem.Error(ex);
                        }
                    }
                    gridViewAccountBook.RefreshData();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewAccountBook_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                DataAccountBook = (MOS.EFMODEL.DataModels.V_HIS_ACCOUNT_BOOK)gridViewAccountBook.GetFocusedRow();
                EditGridClick(DataAccountBook);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewAccountBook_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    DataAccountBook = (MOS.EFMODEL.DataModels.V_HIS_ACCOUNT_BOOK)gridViewAccountBook.GetFocusedRow();
                    EditGridClick(DataAccountBook);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewAccountBook_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            try
            {
                btnCancel_Click(null, null);
                DataAccountBook = (MOS.EFMODEL.DataModels.V_HIS_ACCOUNT_BOOK)gridViewAccountBook.GetFocusedRow();
                FillDataToControl(DataAccountBook);
                FillDataToGridTransaction();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dxValidationProvider_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                DevExpress.XtraEditors.BaseEdit edit = e.InvalidControl as DevExpress.XtraEditors.BaseEdit;
                if (edit == null)
                    return;

                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;

                if (positionHandle == -1)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
                if (positionHandle > edit.TabIndex)
                {
                    positionHandle = edit.TabIndex;
                    edit.SelectAll();
                    edit.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void toolTipController_GetActiveObjectInfo(object sender, DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                if (e.Info == null && e.SelectedControl == gridControlAccountBook)
                {
                    DevExpress.XtraGrid.Views.Grid.GridView view = gridControlAccountBook.FocusedView as DevExpress.XtraGrid.Views.Grid.GridView;
                    DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
                    if (info.InRowCell)
                    {
                        if (lastRowHandle != info.RowHandle || lastColumn != info.Column)
                        {
                            lastColumn = info.Column;
                            lastRowHandle = info.RowHandle;
                            V_HIS_ACCOUNT_BOOK dataRow = (V_HIS_ACCOUNT_BOOK)((IList)((BaseView)gridViewAccountBook).DataSource)[info.RowHandle];
                            string text = "";
                            if (info.Column.FieldName == "ACT_LOCK")
                                text = Inventec.Common.Resource.Get.Value(
                                    "IVT_LANGUAGE_KEY_UC_ACCOUNT_BOOK__TOOLTIP_ACT_LOCK",
                                    Base.ResourceLanguageManager.LanguageUCAccountBook,
                                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                            if (info.Column.FieldName == "FOR_DEPOSIT_IMG" && dataRow.IS_FOR_DEPOSIT == Base.GlobalStore.IS_TRUE)
                                text = Inventec.Common.Resource.Get.Value(
                                    "IVT_LANGUAGE_KEY_UC_ACCOUNT_BOOK__LAYOUT_CHK_FOR_DEPOSIT",
                                    Base.ResourceLanguageManager.LanguageUCAccountBook,
                                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                            if (info.Column.FieldName == "FOR_OTHER_SALE_IMG" && dataRow.IS_FOR_OTHER_SALE == Base.GlobalStore.IS_TRUE)
                                text = Inventec.Common.Resource.Get.Value(
                                    "IVT_LANGUAGE_KEY_UC_ACCOUNT_BOOK__LAYOUT_CHK_FOR_OTHER_SALE",
                                    Base.ResourceLanguageManager.LanguageUCAccountBook,
                                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                            if (info.Column.FieldName == "FOR_REPAY_IMG" && dataRow.IS_FOR_REPAY == Base.GlobalStore.IS_TRUE)
                                text = Inventec.Common.Resource.Get.Value(
                                    "IVT_LANGUAGE_KEY_UC_ACCOUNT_BOOK__LAYOUT_CHK_FOR_REPAY",
                                    Base.ResourceLanguageManager.LanguageUCAccountBook,
                                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                            if (info.Column.FieldName == "FOR_BILL_IMG" && dataRow.IS_FOR_BILL == Base.GlobalStore.IS_TRUE)
                                text = Inventec.Common.Resource.Get.Value(
                                    "IVT_LANGUAGE_KEY_UC_ACCOUNT_BOOK__LAYOUT_CHK_FOR_BILL",
                                    Base.ResourceLanguageManager.LanguageUCAccountBook,
                                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                            if (info.Column.FieldName == "FOR_DEBT_IMG" && dataRow.IS_FOR_DEBT == Base.GlobalStore.IS_TRUE)
                                text = Inventec.Common.Resource.Get.Value(
                                    "IVT_LANGUAGE_KEY_UC_ACCOUNT_BOOK__LAYOUT_CHK_FOR_DEBT",
                                    Base.ResourceLanguageManager.LanguageUCAccountBook,
                                    Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());


                            lastInfo = new DevExpress.Utils.ToolTipControlInfo(new DevExpress.XtraGrid.GridToolTipInfo(view, new CellToolTipInfo(info.RowHandle, info.Column, "Text")), text);
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

        public void Refesh()
        {
            try
            {
                btnRefesh_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void Save()
        {
            try
            {
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void Add()
        {
            try
            {
                btnAdd_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void Edit()
        {
            try
            {
                //btnEdit_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public void Cancel()
        {
            try
            {
                btnCancel_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        private void txtAccountBookCode_TextChanged(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void repositoryItemButtonEditAddUser_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {

        }

        private void repositoryItemButtonEditAddUser_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                var row = (MOS.EFMODEL.DataModels.V_HIS_ACCOUNT_BOOK)gridViewAccountBook.GetFocusedRow();
                if (row != null)
                {
                    WaitingManager.Show();

                    List<object> listArgs = new List<object>();
                    listArgs.Add(row);
                    CallModule callModule = new CallModule(CallModule.HisUserAccountBook, this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                    this.txtKeyWord.Focus();
                    this.txtKeyWord.SelectAll();
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonEditAddCashierRoom_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                var row = (MOS.EFMODEL.DataModels.V_HIS_ACCOUNT_BOOK)gridViewAccountBook.GetFocusedRow();
                if (row != null)
                {
                    WaitingManager.Show();

                    List<object> listArgs = new List<object>();
                    listArgs.Add(row);
                    CallModule callModule = new CallModule(CallModule.HisCaroAccountBook, this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
                    this.txtKeyWord.Focus();
                    this.txtKeyWord.SelectAll();
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboEInvoiceSys_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Delete && !cboEInvoiceSys.ReadOnly && cboEInvoiceSys.Enabled)
                {
                    cboEInvoiceSys.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                List<object> listArgs = new List<object>();
                //listArgs.Add(row);
                CallModule callModule = new CallModule(CallModule.HisAccountBookListImport, this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSearchTransaction_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToGridTransaction();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewTransaction_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.ListSourceRowIndex >= 0 && e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (V_HIS_TRANSACTION)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            try
                            {
                                e.Value = e.ListSourceRowIndex + 1;
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "CASHIER")
                        {
                            try
                            {
                                e.Value = data.CASHIER_LOGINNAME + (String.IsNullOrEmpty(data.CASHIER_USERNAME) ? "" : " - " + data.CASHIER_USERNAME);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "TRANSACTION_TIME_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.TRANSACTION_TIME);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "DOB_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.TDL_PATIENT_DOB ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "CREATE_TIME_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.CREATE_TIME ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "MODIFY_TIME_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.MODIFY_TIME ?? 0);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "THUC_THU_STR")
                        {
                            try
                            {
                                if (data.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__HU)
                                {
                                    decimal? ado = -1 * data.AMOUNT;
                                    e.Value = Inventec.Common.Number.Convert.NumberToString(ado ?? 0, ConfigApplications.NumberSeperator);
                                }
                                else if (data.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TU
                                    || data.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT)
                                {
                                    decimal? ado = data.AMOUNT - (data.KC_AMOUNT ?? 0) - (data.TDL_BILL_FUND_AMOUNT ?? 0) - (data.EXEMPTION ?? 0);
                                    e.Value = Inventec.Common.Number.Convert.NumberToString(ado ?? 0, ConfigApplications.NumberSeperator);
                                }
                                else if (data.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__NO)
                                {
                                    e.Value = "0";
                                }
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "ROUNDED_TOTAL_PRICE_STR")
                        {
                            try
                            {
                                if (data.ROUNDED_TOTAL_PRICE != null)
                                {
                                    e.Value = Inventec.Common.Number.Convert.NumberToString(data.ROUNDED_TOTAL_PRICE ?? 0, ConfigApplications.NumberSeperator);
                                }
                                else
                                {
                                    e.Value = "";
                                }
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "AMOUNT_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.Number.Convert.NumberToString(data.AMOUNT, ConfigApplications.NumberSeperator);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "KC_AMOUNT_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.Number.Convert.NumberToString(data.KC_AMOUNT ?? 0, ConfigApplications.NumberSeperator);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "TDL_BILL_FUND_AMOUNT_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.Number.Convert.NumberToString(data.TDL_BILL_FUND_AMOUNT ?? 0, ConfigApplications.NumberSeperator);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }

                        else if (e.Column.FieldName == "StatusStr")
                        {
                            try
                            {
                                e.Value = data.IS_CANCEL == 1 ? "Đã hủy" : "";
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "EXEMPTION_STR")
                        {
                            try
                            {
                                e.Value = Inventec.Common.Number.Convert.NumberToString(data.EXEMPTION ?? 0, ConfigApplications.NumberSeperator);
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == "DIRECTLY_BILLING_STR")
                        {
                            if (data.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT)
                            {
                                if (data.IS_DIRECTLY_BILLING == 1)
                                {
                                    e.Value = "Thu trực tiếp";
                                }
                                else
                                {
                                    e.Value = "Ra viện";
                                }
                            }
                            //else
                            //{
                            //    e.Value = data.TRANSACTION_TYPE_NAME;
                            //}
                        }
                        else if (e.Column.FieldName == gc_SwipeAmount.FieldName)
                        {
                            try
                            {
                                if (data.PAY_FORM_ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TMQT)
                                {
                                    e.Value = Inventec.Common.Number.Convert.NumberToString(data.SWIPE_AMOUNT ?? 0, ConfigApplications.NumberSeperator);
                                }
                                else
                                {
                                    e.Value = null;
                                }
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
                            }
                        }
                        else if (e.Column.FieldName == gc_TransferAmuont.FieldName)
                        {
                            try
                            {
                                if (data.PAY_FORM_ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TMCK)
                                {
                                    e.Value = Inventec.Common.Number.Convert.NumberToString(data.TRANSFER_AMOUNT ?? 0, ConfigApplications.NumberSeperator);
                                }
                                else
                                {
                                    e.Value = null;
                                }
                            }
                            catch (Exception ex)
                            {
                                Inventec.Common.Logging.LogSystem.Error(ex);
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

        private void gridViewTransaction_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        {
            try
            {
                if (e.RowHandle >= 0)
                {
                    var data = (V_HIS_TRANSACTION)gridViewTransaction.GetRow(e.RowHandle);
                    if (data != null)
                    {
                        if (data.IS_CANCEL == 1)
                        {
                            if (e.Column.FieldName == "STT" || e.Column.FieldName == "CancelTransaction" || e.Column.FieldName == "ChangeLock")
                                return;
                            e.Appearance.ForeColor = Color.Gray; //Giao dịch đã bị hủy => Màu nâu
                            e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font.FontFamily, e.Appearance.Font.Size, FontStyle.Italic);
                            e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Strikeout);
                            if (!String.IsNullOrWhiteSpace(data.INVOICE_CODE))//Với các giao dịch đã xuất hóa đơn điện tử (HIS_TRANSACTION có INVOICE_CODE), thì hiển thị bôi đậm
                            {
                                e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, FontStyle.Bold | FontStyle.Strikeout);
                            }
                        }
                        else if (data.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT)
                        {
                            e.Appearance.ForeColor = Color.Blue; //Giao dịch thanh toán => Màu xanh nước biển
                            if (!String.IsNullOrWhiteSpace(data.INVOICE_CODE))//Với các giao dịch đã xuất hóa đơn điện tử (HIS_TRANSACTION có INVOICE_CODE), thì hiển thị bôi đậm
                            {
                                e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Bold);
                            }
                        }
                        else if (data.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TU)
                        {
                            e.Appearance.ForeColor = Color.Green; //Giao dịch tạm ứng => Màu xanh lá cây
                        }
                        else if (data.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__HU)
                        {
                            e.Appearance.ForeColor = Color.Red; //Giao dịch hoàn ứng => Màu đỏ
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnExportTransactionToExcel_Click(object sender, EventArgs e)
        {
            try
            {
                ExportToExcel(this.gridControlTransaction);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ExportToExcel(DevExpress.XtraGrid.GridControl gridControl)
        {
            try
            {
                if (gridControl != null)
                {
                    SaveFileDialog saveFile = new SaveFileDialog();
                    saveFile.Filter = "Excel file|*.xlsx|All file|*.*";
                    if (saveFile.ShowDialog() == DialogResult.OK)
                    {
                        gridControl.ExportToXlsx(saveFile.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewAccountBook_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            try
            {
                btnCancel_Click(null, null);
                DataAccountBook = (MOS.EFMODEL.DataModels.V_HIS_ACCOUNT_BOOK)gridViewAccountBook.GetFocusedRow();
                FillDataToControl(DataAccountBook);
                FillDataToGridTransaction();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtCreateTimeFrom_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    dtCreateTimeTo.Focus();
                    dtCreateTimeTo.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtCreateTimeTo_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearchTransaction.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkNotGenOrder_CheckedChanged(object sender, EventArgs e)
        {

            try
            {
                if (chkNotGenOrder.Checked == true)
                {
                    chkLuonTachTheoTungSo.Enabled = false;
                    chkLuonTachTheoTungSo.Checked = false;
                }
                else
                {
                    chkLuonTachTheoTungSo.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void chkNotGenOrder_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (chkLuonTachTheoTungSo.Enabled == true)
                    {
                        chkLuonTachTheoTungSo.Focus();
                    }
                    else
                    {
                        chkForDeposit.Focus();
                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewAccountBook_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            try
            {
                DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (e.RowHandle >= 0)
                {
                    long active = Int64.Parse((gridViewAccountBook.GetRowCellValue(e.RowHandle, "IS_ACTIVE") ?? "").ToString());
                    string creator = (gridViewAccountBook.GetRowCellValue(e.RowHandle, "CREATOR") ?? "").ToString();
                    if (e.Column.FieldName == "ACT_LOCK")
                    {
                        if (active == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                        {

                            e.RepositoryItem = (creator == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName() ? repositoryItemButtonUnLock : repositoryItemButtonUnLockDis);

                        }
                        else
                        {
                            e.RepositoryItem = (creator == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName() ? repositoryItemButtonLock : repositoryItemButtonLockDis);
                        }
                    }
                    else if (e.Column.FieldName == "ACT_EDIT")
                    {
                        e.RepositoryItem = (creator == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName() ? ButtonEdit : ButtonEditDis);

                    }
                    else if (e.Column.FieldName == "ACT_DESTROY")
                    {
                        e.RepositoryItem = (creator == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName() ? repositoryItemButtonDestroy : repositoryItemButtonDestroyDis);

                    }
                    else if (e.Column.FieldName == "ADD_USER")
                    {
                        if (active == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                            && (CheckLoginAdmin.IsAdmin(loginName) || creator == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName()))
                        {
                            e.RepositoryItem = repositoryItemButtonEditAddUser;
                        }
                        else
                            e.RepositoryItem = repositoryItemButtonEditAddUserDis;
                    }
                    else if (e.Column.FieldName == "ADD_CASHIER_ROOM")
                    {
                        if (active == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE
                            && (CheckLoginAdmin.IsAdmin(loginName) || creator == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName()))
                        {
                            e.RepositoryItem = repositoryItemButtonEditAddCashierRoom;
                        }
                        else
                            e.RepositoryItem = repositoryItemButtonEditAddCashierRoomDis;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkTransactionTime_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkTransactionTime.Checked)
                {
                    lciNumOrderFrom.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    lciNumOrderTo.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    lciCreatTimeFrom.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    lciCreateTimeTo.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkNumOrder_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkNumOrder.Checked)
                {
                    lciNumOrderFrom.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    lciNumOrderTo.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    lciCreatTimeFrom.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    lciCreateTimeTo.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtNumOrderForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '/'))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtNumOrderTo_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '/'))
                {
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemButtonLock_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            CommonParam param = new CommonParam();
            try
            {
                bool success = false;
                string messageConfirm = "";
                MOS.EFMODEL.DataModels.V_HIS_ACCOUNT_BOOK row = (MOS.EFMODEL.DataModels.V_HIS_ACCOUNT_BOOK)gridViewAccountBook.GetFocusedRow();
                if (row != null && row.CREATOR == Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName())
                {
                    if (row.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                    {
                        messageConfirm = ResourceMessage.HeThongTBCuaSoThongBaoBanCoMuonKhoaDuLieuKhong;
                    }
                    else
                    {
                        messageConfirm = ResourceMessage.HeThongTBCuaSoThongBaoBanCoMuonBoKhoaDuLieuKhong;
                    }
                    if (DevExpress.XtraEditors.XtraMessageBox.Show(messageConfirm, ResourceMessage.ThongBao, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        MOS.EFMODEL.DataModels.HIS_ACCOUNT_BOOK data = new MOS.EFMODEL.DataModels.HIS_ACCOUNT_BOOK();
                        Inventec.Common.Mapper.DataObjectMapper.Map<MOS.EFMODEL.DataModels.HIS_ACCOUNT_BOOK>(data, row);
                        var apiresult = new BackendAdapter(param).Post<ApiResultObject<MOS.EFMODEL.DataModels.HIS_ACCOUNT_BOOK>>(HisRequestUriStore.HIS_ACCOUNT_BOOK_CHANGE_LOG, ApiConsumers.MosConsumer, data, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, param);
                        if (apiresult != null)
                        {
                            success = true;
                            FillDataToGridAccountBookList();
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
                Inventec.Common.Logging.LogSystem.Fatal(ex);
            }
        }
    }
}

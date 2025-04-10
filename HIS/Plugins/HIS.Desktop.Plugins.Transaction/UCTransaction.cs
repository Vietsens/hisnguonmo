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
using DevExpress.Utils;
using DevExpress.XtraBars;  
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.Transaction.ADO;
using HIS.Desktop.Plugins.Transaction.Base;
using HIS.Desktop.Plugins.Transaction.Config;
using HIS.Desktop.Utilities.Extensions;
using HIS.Desktop.Utility;
using HIS.UC.SereServTree;
using HIS.UC.TotalPriceInfo;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.ThreadCustom;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.Transaction
{
    public partial class UCTransaction : UserControlBase
    {
        #region Declare
        BarManager barManager = null;
        PopupMenuProcessor popupMenuProcessor = null;

        SereServTreeProcessor ssTreeProcessor;
        TotalPriceInfoProcessor totalPriceProcessor;
        UserControl ucSereServTree;
        UserControl ucTotalPriceInfo;
        V_HIS_TREATMENT_FEE currentTreatment;
        List<V_HIS_SERE_SERV_5> listSereServ = new List<V_HIS_SERE_SERV_5>();
        List<V_HIS_TREATMENT_FEE> listTreatment = new List<V_HIS_TREATMENT_FEE>();
        Dictionary<long, List<HIS_SERE_SERV_BILL>> dicSereServBill;
        List<MOS.EFMODEL.DataModels.HIS_TREATMENT_TYPE> treatmentTypeSelecteds;
        List<MOS.EFMODEL.DataModels.HIS_DEPARTMENT> departmentSelecteds;
        List<ACS.EFMODEL.DataModels.ACS_CONTROL> controlAcs;
        List<HIS_PATIENT_TYPE> PatientTypeSelecteds;
        V_HIS_PATIENT_TYPE_ALTER lastPatientType;

        Inventec.Desktop.Common.Modules.Module currentModule;
        Dictionary<long, HIS_TREATMENT_RESULT> treatmentResult;
        int rowCount = 0;
        int dataTotal = 0;
        int start = 0;
        int limit = 0;

        V_HIS_TRANSACTION transaction;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        string moduleLink = "HIS.Desktop.Plugins.Transaction";
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        bool isNotLoadWhileChkIsInDebtStateInFirst;
        long RoomId;
        long RoomTypeId;
        V_HIS_CASHIER_ROOM cashierRoom = null;
        PopupItemStatusAdo PopupItemStatusAdo = new PopupItemStatusAdo();

        CPA.WCFClient.CallPatientClient.CallPatientClientManager clienttManager = null;
            
        int positionHandleControl = -1;
        private bool allowUnlock = false;
        private string loginname = null;      
        string configKeyCallPatientCPA;
        string configKeyCallPatientByCPA;   
        #endregion
        public UCTransaction(long roomId, long roomTypeId)
        {
            Inventec.Common.Logging.LogSystem.Debug("UCTransaction.InitializeComponent => 1");
            Task.Run(() => InitializeComponent()).Wait();
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("UCTransaction.InitializeComponent => 2");
                Base.ResourceLangManager.InitResourceLanguageManager();
                Inventec.Common.Logging.LogSystem.Debug("UCTransaction.InitializeComponent => 3");
                Task.Run(() => InitSereServTree()).Wait();
                Inventec.Common.Logging.LogSystem.Debug("UCTransaction.InitializeComponent => 4");
                InitTotalPriceInfo();
                Inventec.Common.Logging.LogSystem.Debug("UCTransaction.InitializeComponent => 5");
                this.RoomId = roomId;
                this.RoomTypeId = roomTypeId;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public UCTransaction(long roomId, long roomTypeId, Inventec.Desktop.Common.Modules.Module _currentModule)
            : base(_currentModule)
        {
            InitializeComponent();
            try
            {
                this.currentModule = _currentModule;
                Inventec.Common.Logging.LogSystem.Debug("UCTransaction.InitializeComponent => 2");
                Base.ResourceLangManager.InitResourceLanguageManager();
                Inventec.Common.Logging.LogSystem.Debug("UCTransaction.InitializeComponent => 3");
                Task.Run(() => InitSereServTree()).Wait();
                Inventec.Common.Logging.LogSystem.Debug("UCTransaction.InitializeComponent => 4");
                InitTotalPriceInfo();
                Inventec.Common.Logging.LogSystem.Debug("UCTransaction.InitializeComponent => 5");
                this.RoomId = roomId;
                this.RoomTypeId = roomTypeId;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitSereServTree()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("UCTransaction.InitSereServTree => 1");
                this.ssTreeProcessor = new UC.SereServTree.SereServTreeProcessor();
                SereServTreeADO ado = new SereServTreeADO();
                ado.IsShowSearchPanel = true;
                ado.SereServTreeColumns = new List<SereServTreeColumn>();
                ado.SelectImageCollection = this.imageCollection1;
                ado.StateImageCollection = this.imageCollection1;
                ado.SereServTree_GetStateImage = treeSereServ_GetStateImage;
                ado.SereServTree_GetSelectImage = treeSereServ_GetSelectImage;
                ado.SereServTree_StateImageClick = treeSereServ_StateImageClick;
                ado.SereServTree_CustomDrawNodeCell = treeSereServ_CustomDrawNodeCell;
                ado.SereServTree_CustomUnboundColumnData = sereServTree_CustomUnboundColumnData;
                ado.LayoutSereServExpend = "Hao phí";

                //Column tên dịch vụ
                SereServTreeColumn serviceNameCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__TREE_SERE_SERV__COLUMN_SERVICE_NAME", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "TDL_SERVICE_NAME", 200, false);
                serviceNameCol.VisibleIndex = 0;
                ado.SereServTreeColumns.Add(serviceNameCol);

                //Column Số lượng
                SereServTreeColumn amountCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__TREE_SERE_SERV__COLUMN_AMOUNT", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "AMOUNT_PLUS", 40, false);
                amountCol.VisibleIndex = 1;
                amountCol.Format = new DevExpress.Utils.FormatInfo();
                amountCol.Format.FormatString = "#,##0.00";
                amountCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.SereServTreeColumns.Add(amountCol);

                //Column đơn giá
                SereServTreeColumn virPriceCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__TREE_SERE_SERV__COLUMN_VIR_PRICE", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "VIR_PRICE_DISPLAY", 70, false);
                virPriceCol.VisibleIndex = 2;
                virPriceCol.Format = new DevExpress.Utils.FormatInfo();
                virPriceCol.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;
                ado.SereServTreeColumns.Add(virPriceCol);

                //Column thành tiền
                SereServTreeColumn virTotalPriceCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__TREE_SERE_SERV__COLUMN_VIR_TOTAL_PRICE", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "VIR_TOTAL_PRICE_DISPLAY", 75, false);
                virTotalPriceCol.VisibleIndex = 3;
                virTotalPriceCol.Format = new DevExpress.Utils.FormatInfo();
                virTotalPriceCol.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;
                ado.SereServTreeColumns.Add(virTotalPriceCol);

                //Column Bảo hiểm chi trả
                SereServTreeColumn virTotalHeinPriceCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__TREE_SERE_SERV__COLUMN_VIR_TOTAL_HEIN_PRICE", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "VIR_TOTAL_HEIN_PRICE_DISPLAY", 75, false);
                virTotalHeinPriceCol.VisibleIndex = 4;
                virTotalHeinPriceCol.Format = new DevExpress.Utils.FormatInfo();
                virTotalHeinPriceCol.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;
                ado.SereServTreeColumns.Add(virTotalHeinPriceCol);

                //Column bệnh nhân trả
                SereServTreeColumn virTotalPatientPriceCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__TREE_SERE_SERV__COLUMN_VIR_TOTAL_PATIENT_PRICE", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "VIR_TOTAL_PATIENT_PRICE_DISPLAY", 80, false);
                virTotalPatientPriceCol.VisibleIndex = 5;
                virTotalPatientPriceCol.Format = new DevExpress.Utils.FormatInfo();
                virTotalPatientPriceCol.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;
                ado.SereServTreeColumns.Add(virTotalPatientPriceCol);

                //Column vat (%)
                SereServTreeColumn virVatRatioCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__TREE_SERE_SERV__COLUMN_VAT_RATIO", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "VAT", 50, false);
                virVatRatioCol.VisibleIndex = 6;
                virVatRatioCol.Format = new DevExpress.Utils.FormatInfo();
                virVatRatioCol.Format.FormatString = "#,##0.00";
                virVatRatioCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.SereServTreeColumns.Add(virVatRatioCol);

                //Column mã dịch vụ
                SereServTreeColumn serviceCodeCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__TREE_SERE_SERV__COLUMN_SERVICE_CODE", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "TDL_SERVICE_CODE", 60, false);
                serviceCodeCol.VisibleIndex = 7;
                ado.SereServTreeColumns.Add(serviceCodeCol);

                //Column chiết khấu
                SereServTreeColumn virDiscountCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__TREE_SERE_SERV__COLUMN_DISCOUNT", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "DISCOUNT_DISPLAY", 70, false);
                virDiscountCol.VisibleIndex = 8;
                virDiscountCol.Format = new DevExpress.Utils.FormatInfo();
                virDiscountCol.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;
                ado.SereServTreeColumns.Add(virDiscountCol);

                //Column hao phí
                SereServTreeColumn virIsExpendCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__TREE_SERE_SERV__COLUMN_IS_EXPEND", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "IsExpend", 55, false);
                virIsExpendCol.VisibleIndex = 9;
                ado.SereServTreeColumns.Add(virIsExpendCol);

                //Column Mã yêu cầu
                SereServTreeColumn serviceReqCodeCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__TREE_SERE_SERV__COLUMN_SERVICE_REQ_CODE", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "TDL_SERVICE_REQ_CODE", 100, false);
                serviceReqCodeCol.VisibleIndex = 10;
                ado.SereServTreeColumns.Add(serviceReqCodeCol);

                //Column mã giao dịch
                //SereServTreeColumn TRANSACTIONCodeCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__TREE_SERE_SERV__COLUMN_TRANSACTION_CODE", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "TRANSACTION_CODE", 100, false);
                //TRANSACTIONCodeCol.VisibleIndex = 11;
                //ado.SereServTreeColumns.Add(TRANSACTIONCodeCol);

                this.ucSereServTree = (UserControl)this.ssTreeProcessor.Run(ado);
                if (this.ucSereServTree != null)
                {
                    this.panelControlTreeSereServ.Controls.Add(this.ucSereServTree);
                    this.ucSereServTree.Dock = DockStyle.Fill;
                }
                Inventec.Common.Logging.LogSystem.Debug("UCTransaction.InitSereServTree => 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void treeSereServ_GetStateImage(SereServADO data, DevExpress.XtraTreeList.GetStateImageEventArgs e)
        {
            try
            {
                if (data != null)
                {
                    if (!e.Node.HasChildren)
                    {
                        if (dicSereServBill.ContainsKey(data.ID))
                        {
                            e.NodeImageIndex = 1;
                        }
                        else
                        {
                            e.NodeImageIndex = 0;
                        }
                    }
                    else
                    {
                        e.NodeImageIndex = -1;
                    }
                }
                else
                {
                    e.NodeImageIndex = -1;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task InitTotalPriceInfo()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("UCTransaction.InitTotalPriceInfo => 1");
                this.totalPriceProcessor = new TotalPriceInfoProcessor();
                UC.TotalPriceInfo.ADO.InitADO data = new UC.TotalPriceInfo.ADO.InitADO();
                data.LayoutDiscount = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__TOTAL_PRICE_INFO__LAYOUT_DISCOUNT", ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                data.LayoutTotalDiscount = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__TOTAL_PRICE_INFO__LAYOUT_TOTAL_DISCOUNT", ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                data.LayoutVirTotalBillFundPrice = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__TOTAL_PRICE_INFO__LAYOUT_VIR_TOTAL_BILL_FUND_PRICE", ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                data.LayoutVirTotalBillPrice = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__TOTAL_PRICE_INFO__LAYOUT_VIR_TOTAL_BILL_PRICE", ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                data.LayoutVirTotalBillTransferPrice = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__TOTAL_PRICE_INFO__LAYOUT_VIR_TOTAL_BILL_TRANSFER_PRICE", ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                data.LayoutVirTotalDepositPrice = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__TOTAL_PRICE_INFO__LAYOUT_VIR_TOTAL_DEPOSIT_PRICE", ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                data.LayoutVirTotalServiceDepositPrice = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__TOTAL_PRICE_INFO__LAYOUT_VIR_TOTAL_SERVICE_DEPOSIT_PRICE", ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                data.LayoutVirTotalHeinPrice = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__TOTAL_PRICE_INFO__LAYOUT_VIR_TOTAL_HEIN_PRICE", ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                data.LayoutVirTotalHeinPriceTotip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__TOTAL_PRICE_INFO__LAYOUT_VIR_TOTAL_HEIN_PRICE_TOTIP", ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                data.LayoutVirTotalPatientPrice = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__TOTAL_PRICE_INFO__LAYOUT_VIR_TOTAL_PATIENT_PRICE", ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                data.LayoutVirTotalPatientPriceToTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__TOTAL_PRICE_INFO__LAYOUT_VIR_TOTAL_PATIENT_PRICE_TOTIP", ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                data.LayoutVirTotalPrice = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__TOTAL_PRICE_INFO__LAYOUT_VIR_TOTAL_PRICE", ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                data.LayoutVirTotalPriceTotip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__TOTAL_PRICE_INFO__LAYOUT_VIR_TOTAL_PRICE_TOTIP", ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                data.LayoutVirTotalReceiveMorePrice = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__TOTAL_PRICE_INFO__LAYOUT_VIR_TOTAL_RECEIVE_MORE_PRICE", ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                data.LayoutVirTotalReceiveMorePriceTotip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__TOTAL_PRICE_INFO__LAYOUT_VIR_TOTAL_RECEIVE_MORE_PRICE_TOTIP", ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                data.LayoutVirTotalReceivePrice = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__TOTAL_PRICE_INFO__LAYOUT_VIR_TOTAL_RECEIVE_PRICE", ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                data.LayoutVirTotalReceivePriceToTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__TOTAL_PRICE_INFO__LAYOUT_VIR_TOTAL_RECEIVE_PRICE_TOTIP", ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                data.LayoutVirTotalRepayPrice = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__TOTAL_PRICE_INFO__LAYOUT_VIR_TOTAL_REPAY_PRICE", ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                data.LayoutVirTotalOtherCopaidPrice = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__TOTAL_PRICE_INFO__LAYOUT_VIR_TOTAL_OTHER_COPAID_PRICE", ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                data.LayoutVirTotalOtherCopaidPriceTotip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__TOTAL_PRICE_INFO__LAYOUT_VIR_TOTAL_OTHER_COPAID_PRICE_TOTIP", ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                data.IsShowRepayPriceCFG = HisConfigCFG.IsSplitTotalReceivePrice;
                data.LayoutTotalRepayPrice = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__TOTAL_PRICE_INFO__LAYOUT_TOTAL_REPAY_PRICE", ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                data.LayoutTotalOtherBillAmount = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__TOTAL_PRICE_INFO__LAYOUT_TOTAL_OTHER_BILL_AMOUNT", ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                data.LayoutTotalOtherBillAmountTotip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__TOTAL_PRICE_INFO__LAYOUT_TOTAL_OTHER_BILL_AMOUNT_TOTIP", ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                data.LayoutVirTotalPriceNoExpend = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__TOTAL_PRICE_INFO__LAYOUT_VIR_TOTAL_PRICE_NO_EXPEND_TEXT", ResourceLangManager.LanguageUCTransaction,
Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                data.layoutTotalDebtAmount = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__TOTAL_DEBT_AMOUNT__LAYOUT_TOTAL_DEBT_AMOUNT_TEXT", ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                data.layoutTotalDebtAmountTotip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__TOTAL_DEBT_AMOUNT__LAYOUT_TOTAL_DEBT_AMOUNT_TOTIP", ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                this.txtStepNumber.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCTransaction.txtStepNumber.Properties.NullValuePrompt", ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.txtGateNumber.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCTransaction.txtGateNumber.Properties.NullValuePrompt", ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                //minhnq
                data.layoutOtherSourcePrice = Inventec.Common.Resource.Get.Value("UCTotalPriceInfo.lciOtherSourcePrice.Text", ResourceLangManager.LanguageUCTransaction, LanguageManager.GetCulture());
                data.LayoutTotalOtherBillAmount = Inventec.Common.Resource.Get.Value("UCTotalPriceInfo.lciTotalOtherBillAmount.Text", ResourceLangManager.LanguageUCTransaction, LanguageManager.GetCulture());
                this.btnTranList.Text = Inventec.Common.Resource.Get.Value("UCTransaction.btnTranList.Text", ResourceLangManager.LanguageUCTransaction, LanguageManager.GetCulture());
                this.btnTranList.ToolTip = Inventec.Common.Resource.Get.Value("UCTransaction.btnTranList.ToolTip", ResourceLangManager.LanguageUCTransaction, LanguageManager.GetCulture());

                data.LayoutLockingAmount = Inventec.Common.Resource.Get.Value("UCTotalPriceInfo.LayoutLockingAmount.Text", ResourceLangManager.LanguageUCTransaction, LanguageManager.GetCulture());
                data.LayoutLockingAmountTotip = Inventec.Common.Resource.Get.Value("UCTotalPriceInfo.LayoutLockingAmountTotip.Text", ResourceLangManager.LanguageUCTransaction, LanguageManager.GetCulture());

                this.ucTotalPriceInfo = (UserControl)this.totalPriceProcessor.Run(data);
                if (this.ucTotalPriceInfo != null)
                {
                    this.panelPriceInfo.Controls.Add(this.ucTotalPriceInfo);
                    this.ucTotalPriceInfo.Dock = DockStyle.Fill;
                }
                Inventec.Common.Logging.LogSystem.Debug("UCTransaction.InitTotalPriceInfo => 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void UCTransaction_Load(object sender, EventArgs e)
        {
            try
            {
                configKeyCallPatientCPA = ConfigApplicationWorker.Get<string>(AppConfigKey.CONFIG_KEY__DANG_KY_TIEP_DON__GOI_BENH_NHAN_BANG_PHAN_MEM_CPA);
                configKeyCallPatientByCPA = ConfigApplicationWorker.Get<string>(AppConfigKey.CONFIG_KEY__GOI_BENH_NHAN_THANH_TOAN_VP_QUA_CPA);
                if (configKeyCallPatientCPA != "1")
                {
                    timerInitForm.Enabled = true;
                    timerInitForm.Interval = 100;
                    RegisterTimer(currentModule.ModuleLink, "timerInitForm", timerInitForm.Interval, timerInitForm_Tick);
                    StartTimer(currentModule.ModuleLink, "timerInitForm");
                    txtFromNumber.Enabled = true;
                    txtToNumber.Enabled = true;
                    GATE();
                }
                else
                {
                    this.clienttManager = new CPA.WCFClient.CallPatientClient.CallPatientClientManager();
                }
                Inventec.Common.Logging.LogSystem.Debug("UCTransaction.Load => 1");
                WaitingManager.Show();
                if (GlobalVariables.AcsAuthorizeSDO != null)
                {
                    controlAcs = GlobalVariables.AcsAuthorizeSDO.ControlInRoles;
                }
                LoadKeyUCLanguage();
                LoadCashierRoom();
                this.allowUnlock = (GlobalVariables.AcsAuthorizeSDO.ControlInRoles != null && GlobalVariables.AcsAuthorizeSDO.ControlInRoles.Any(a => a.CONTROL_CODE == "HIS000026"));
                this.loginname = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                Inventec.Common.Logging.LogSystem.Debug("UCTransaction.Load => 2");
                ValidControl();
                Inventec.Common.Logging.LogSystem.Debug("UCTransaction.Load => 3");
                SetDefaultValueControl();
                chkBranch.CheckState = CheckState.Checked;
                Inventec.Common.Logging.LogSystem.Debug("UCTransaction.Load => 4");
                SetBranch();
                Inventec.Common.Logging.LogSystem.Debug("UCTransaction.Load => 5");
                SetEnableButton(false);
                Inventec.Common.Logging.LogSystem.Debug("UCTransaction.Load => 6");
                HisConfigCFG.LoadConfig();
                Inventec.Common.Logging.LogSystem.Debug("UCTransaction.Load => 8");
                InitCheck(cboTreatmentType, SelectionGrid__Status);
                InitCheck(cboEndDepartment, SelectionGrid__EndDepartment);
                InitCheck(CboPatientType, SelectionGrid__PatientType);
                InitCombo(cboTreatmentType, BackendDataWorker.Get<HIS_TREATMENT_TYPE>(), "TREATMENT_TYPE_NAME", "ID");
                Inventec.Common.Logging.LogSystem.Debug("UCTransaction.Load => 9");
                InitCombo(cboEndDepartment, BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => o.IS_CLINICAL == 1).ToList(), "DEPARTMENT_NAME", "ID");
                Inventec.Common.Logging.LogSystem.Debug("UCTransaction.Load => 10");
                this.gridControlPatientTypeInfo.ToolTipController = toolTipController1;
                InitCombo(CboPatientType, BackendDataWorker.Get<HIS_PATIENT_TYPE>(), "PATIENT_TYPE_NAME", "ID");
                //InitCboPatientType();

                this.InItComboActive(this.SetDataIsActive());
                this.toolTipController1.ShowBeak = true;
                this.toolTipController1.ToolTipType = ToolTipType.SuperTip;
                this.txtFindTreatmentCode.Focus();
                this.txtFindTreatmentCode.SelectAll();
                Inventec.Common.Logging.LogSystem.Debug("UCTransaction.Load => 11");

                // khởi tạo trạng thái của check Is In Debt
                //if (!String.IsNullOrEmpty(transaction.INVOICE_CODE) && !String.IsNullOrEmpty(transaction.INVOICE_SYS)) //? is Necessary?
                //{
                //    lciChkIsInDebt.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                InitControlState();
                FillDataToGridTreatment();
                //}
                chkIsInDebt.ShowToolTips = true;

                this.ProcessCustomizeUI();

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void timerInitForm_Tick()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("timer1_Tick .1");
                StopTimer(currentModule.ModuleLink, "timerInitForm");
                this.GATE();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void InitCboPatientType()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PATIENT_TYPE_NAME", "Đối tượng", 200, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PATIENT_TYPE_NAME", "ID", columnInfos, true, 200);
                ControlEditorLoader.Load(CboPatientType, BackendDataWorker.Get<HIS_PATIENT_TYPE>(), controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetBranch()
        {
            try
            {
                string branch = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.Treatment.Is_Show_All_Branch");
                lciBranch.Enabled = (branch == "1");
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
                txtKeyword.Text = "";
                dtInTimeFrom.DateTime = DateTime.Now;
                dtInTimeTo.DateTime = DateTime.Now;

                this.treatmentResult = BackendDataWorker.Get<HIS_TREATMENT_RESULT>().ToDictionary(o => o.ID, o => o);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task LoadCashierRoom()
        {
            try
            {
                List<V_HIS_CASHIER_ROOM> cashierRooms = null;
                if (BackendDataWorker.IsExistsKey<V_HIS_CASHIER_ROOM>())
                {
                    cashierRooms = BackendDataWorker.Get<V_HIS_CASHIER_ROOM>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    MOS.Filter.HisCashierRoomFilter filter = new MOS.Filter.HisCashierRoomFilter();
                    cashierRooms = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.V_HIS_CASHIER_ROOM>>("api/HisCashierRoom/GetView", ApiConsumers.MosConsumer, filter, paramCommon);

                    if (cashierRooms != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.V_HIS_CASHIER_ROOM), cashierRooms, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                this.cashierRoom = cashierRooms.FirstOrDefault(o => o.ROOM_ID == this.RoomId && o.ROOM_TYPE_ID == this.RoomTypeId);

                if (this.cashierRoom == null)
                    Inventec.Common.Logging.LogSystem.Error("Khong lay duoc phong thu ngan: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => RoomId), RoomId));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error("Khong lay duoc phong thu ngan: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => RoomId), RoomId));
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToGridTreatment()
        {
            try
            {
                FillDataToGridTreatment(new CommonParam(0, (int)ConfigApplications.NumPageSize));

                CommonParam param = new CommonParam();
                param.Limit = rowCount;
                param.Count = dataTotal;
                ucPaging1.Init(FillDataToGridTreatment, param, (int)ConfigApplications.NumPageSize, this.gridControlTreatment);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SwitchSetDefaultDate(bool isInDate)
        {
            try
            {
                if (isInDate)
                {
                    dtInTimeFrom.DateTime = DateTime.Now;
                    dtInTimeTo.DateTime = DateTime.Now;
                    dtOutTimeFrom.EditValue = null;
                    dtOutTimeTo.EditValue = null;
                }
                else
                {
                    dtOutTimeFrom.DateTime = DateTime.Now;
                    dtOutTimeTo.DateTime = DateTime.Now;
                    dtInTimeFrom.EditValue = null;
                    dtInTimeTo.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SelectionGrid__EndDepartment(object sender, EventArgs e)
        {
            try
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                departmentSelecteds = new List<HIS_DEPARTMENT>();
                foreach (HIS_DEPARTMENT rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                    {
                        if (sb.ToString().Length > 0) { sb.Append(", "); }
                        sb.Append(rv.DEPARTMENT_NAME);
                        departmentSelecteds.Add(rv);
                    }
                }
                cboEndDepartment.Text = sb.ToString();
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
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                treatmentTypeSelecteds = new List<HIS_TREATMENT_TYPE>();
                foreach (HIS_TREATMENT_TYPE rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                    {
                        if (sb.ToString().Length > 0) { sb.Append(", "); }
                        sb.Append(rv.TREATMENT_TYPE_NAME);
                        treatmentTypeSelecteds.Add(rv);
                    }
                }
                cboTreatmentType.Text = sb.ToString();

                var checkKham = treatmentTypeSelecteds.FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM);
                var checkDieuTri = treatmentTypeSelecteds.FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU || o.ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU);

                if (checkKham == null && checkDieuTri != null)
                {
                    SwitchSetDefaultDate(false);
                }
                else
                {
                    SwitchSetDefaultDate(true);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SelectionGrid__PatientType(object sender, EventArgs e)
        {
            try
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                PatientTypeSelecteds = new List<HIS_PATIENT_TYPE>();
                foreach (HIS_PATIENT_TYPE rv in (sender as GridCheckMarksSelection).Selection)
                {
                    if (rv != null)
                    {
                        if (sb.ToString().Length > 0) { sb.Append(", "); }
                        sb.Append(rv.PATIENT_TYPE_NAME);
                        PatientTypeSelecteds.Add(rv);
                    }
                }
                CboPatientType.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task InitCombo(GridLookUpEdit cbo, object data, string DisplayValue, string ValueMember)
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

        private List<IsActiveADO> SetDataIsActive()
        {
            List<IsActiveADO> result = new List<IsActiveADO>();
            try
            {
                IsActiveADO all = new IsActiveADO();
                all.ID = 0;
                all.Name = "Tất cả";
                result.Add(all);

                IsActiveADO active = new IsActiveADO();
                active.ID = 1;
                active.Name = "Chưa khóa viện phí";
                result.Add(active);

                IsActiveADO deactive = new IsActiveADO();
                deactive.ID = 2;
                deactive.Name = "Đã khóa viện phí";
                result.Add(deactive);

                IsActiveADO ktChuaDuyet = new IsActiveADO();
                ktChuaDuyet.ID = 3;
                ktChuaDuyet.Name = "Đã kết thúc điều trị nhưng chưa duyệt khóa viện phí";
                result.Add(ktChuaDuyet);

                IsActiveADO chuaKT = new IsActiveADO();
                chuaKT.ID = 4;
                chuaKT.Name = "Chưa kết thúc điều trị";
                result.Add(chuaKT);

                IsActiveADO bhyt = new IsActiveADO();
                bhyt.ID = 5;
                bhyt.Name = "Bệnh nhân BHYT";
                result.Add(bhyt);

                IsActiveADO khoaVpChuaDuyetBhyt = new IsActiveADO();
                khoaVpChuaDuyetBhyt.ID = 6;
                khoaVpChuaDuyetBhyt.Name = "Đã khóa viện phí nhưng chưa duyệt bhyt";
                result.Add(khoaVpChuaDuyetBhyt);

                IsActiveADO duyetBhytChuaKhoaBhyt = new IsActiveADO();
                duyetBhytChuaKhoaBhyt.ID = 7;
                duyetBhytChuaKhoaBhyt.Name = "Đã duyệt bhyt nhưng chưa khóa bhyt";
                result.Add(duyetBhytChuaKhoaBhyt);

                IsActiveADO KhoaBhytChuaThanhToan = new IsActiveADO();
                KhoaBhytChuaThanhToan.ID = 8;
                KhoaBhytChuaThanhToan.Name = "Đã khóa BHYT nhưng chưa thanh toán";
                result.Add(KhoaBhytChuaThanhToan);

            }
            catch (Exception ex)
            {
                result = new List<IsActiveADO>();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private async Task InItComboActive(object dataSoucre)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("Name", "", 300, 0));
                ControlEditorADO controlEditorADO = new ControlEditorADO("Name", "ID", columnInfos, false, 300);
                ControlEditorLoader.Load(this.cboIsActive, dataSoucre, controlEditorADO);
                // set default 
                this.cboIsActive.EditValue = 0;
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

        private long IS_REQUIRED = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<long>("HIS.Desktop.Plugins.Transaction.IsRequiredLatchApproveStore");
        private void FillDataToGridTreatment(object param)
        {
            try
            {
                this.listTreatment = new List<V_HIS_TREATMENT_FEE>();
                this.currentTreatment = null;
                this.listSereServ = new List<V_HIS_SERE_SERV_5>();

                //reset SereServTree
                this.gridControlTreatment.DataSource = null;
                start = ((CommonParam)param).Start ?? 0;
                limit = ((CommonParam)param).Limit ?? 0;
                CommonParam paramCommon = new CommonParam(start, limit);
                HisTreatmentFeeViewFilter treatFilter = new HisTreatmentFeeViewFilter();
                this.SetTreatmentFilterByValueControl(ref treatFilter);
                if (IS_REQUIRED == 1)
                {
                    treatFilter.IS_APPROVE_STORE = true;
                }
                var result = new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetRO<List<V_HIS_TREATMENT_FEE>>("api/HisTreatment/GetFeeView", ApiConsumers.MosConsumer, treatFilter, paramCommon);
                if (result != null)
                {
                    this.listTreatment = (List<V_HIS_TREATMENT_FEE>)result.Data;
                    rowCount = (this.listTreatment == null ? 0 : this.listTreatment.Count);
                    dataTotal = (result.Param == null ? 0 : result.Param.Count ?? 0);
                }
                this.gridControlTreatment.BeginUpdate();
                this.gridControlTreatment.DataSource = this.listTreatment;
                this.gridControlTreatment.EndUpdate();
                this.FillDataToControlBySelectTreatment(false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetTreatmentFilterByValueControl(ref HisTreatmentFeeViewFilter treatFilter)
        {
            treatFilter.ORDER_DIRECTION = "MODIFY_TIME";
            treatFilter.ORDER_FIELD = "DESC";
            if (chkIsInDebt.CheckState == CheckState.Checked)
            {
                treatFilter.IS_IN_DEBT = true;
            }
            else
            {
                treatFilter.IS_IN_DEBT = null;
            }
            if (chkBranch.CheckState == CheckState.Checked)
            {
                treatFilter.BRANCH_ID = HIS.Desktop.LocalStorage.BackendData.BranchDataWorker.Branch.ID;
            }
            if (!String.IsNullOrEmpty(txtFindTreatmentCode.Text))
            {
                string code = txtFindTreatmentCode.Text.Trim();
                if (code.Length < 12)
                {
                    code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                    txtFindTreatmentCode.Text = code;
                }
                treatFilter.TREATMENT_CODE__EXACT = code;
            }
            else if (!string.IsNullOrEmpty(txtFindPatientCode.Text))
            {
                string code = txtFindPatientCode.Text.Trim();
                if (code.Length < 10)
                {
                    code = String.Format("{0:0000000000}", Convert.ToInt64(code));
                    txtFindPatientCode.Text = code;
                }
                treatFilter.PATIENT_CODE__EXACT = code;
            }
            else if (!string.IsNullOrEmpty(txtInCode.Text))
            {
                string code = txtInCode.Text.Trim();
                treatFilter.IN_CODE__EXACT = code;
            }
            else
            {
                if (dtInTimeFrom.EditValue != null && dtInTimeFrom.DateTime != DateTime.MinValue)
                {
                    treatFilter.IN_DATE_FROM = Convert.ToInt64(dtInTimeFrom.DateTime.ToString("yyyyMMdd") + "000000");
                }
                if (dtInTimeTo.EditValue != null && dtInTimeTo.DateTime != DateTime.MinValue)
                {
                    treatFilter.IN_DATE_TO = Convert.ToInt64(dtInTimeTo.DateTime.ToString("yyyyMMdd") + "235959");
                }
                if (dtOutTimeFrom.EditValue != null && dtOutTimeFrom.DateTime != DateTime.MinValue)
                {
                    treatFilter.OUT_TIME_FROM = Convert.ToInt64(dtOutTimeFrom.DateTime.ToString("yyyyMMdd") + "000000");
                }
                if (dtOutTimeTo.EditValue != null && dtOutTimeTo.DateTime != DateTime.MinValue)
                {
                    treatFilter.OUT_TIME_TO = Convert.ToInt64(dtOutTimeTo.DateTime.ToString("yyyyMMdd") + "235959");
                }
                if (!String.IsNullOrEmpty(txtKeyword.Text))
                {
                    treatFilter.KEY_WORD__VIR_PATIENT_NAME__PATIENT_CODE__TREATMENT_CODE__GENDER_NAME = txtKeyword.Text;
                }
                if (this.treatmentTypeSelecteds != null && this.treatmentTypeSelecteds.Count > 0)
                {
                    treatFilter.TDL_TREATMENT_TYPE_IDs = this.treatmentTypeSelecteds.Select(o => o.ID).ToList();
                }
                if (this.departmentSelecteds != null && this.departmentSelecteds.Count > 0)
                {
                    treatFilter.END_DEPARTMENT_IDs = this.departmentSelecteds.Select(o => o.ID).ToList();
                }

                if (this.PatientTypeSelecteds != null && this.PatientTypeSelecteds.Count > 0)
                {
                    treatFilter.TDL_PATIENT_TYPE_IDs = this.PatientTypeSelecteds.Select(o => o.ID).ToList();
                }

                if (cboIsActive.EditValue != null)
                {
                    var activeId = Convert.ToInt16(cboIsActive.EditValue.ToString());
                    if (activeId == 0)//tất cả
                    {
                        treatFilter.IS_ACTIVE = null;
                    }
                    else if (activeId == 1)// chua khoa
                    {
                        treatFilter.IS_ACTIVE = 1;
                    }
                    else if (activeId == 2)//Đã khóa
                    {
                        treatFilter.IS_ACTIVE = 0;
                    }
                    else if (activeId == 3)// đã kết thúc điều trị nhưng chưa duyệt khóa viện phí
                    {
                        treatFilter.IS_PAUSE = true;
                        treatFilter.IS_ACTIVE = 1;
                    }
                    else if (activeId == 4)// chưa kết thúc điều trị
                    {
                        treatFilter.IS_PAUSE = false;
                    }
                    else if (activeId == 5)//Bn Bhyt
                    {
                        treatFilter.TDL_PATIENT_TYPE_ID = HisConfigCFG.PatientTypeId__BHYT;
                    }
                    else if (activeId == 6)//Đã khóa viện phí nhưng chưa duyệt bhyt
                    {
                        treatFilter.IS_ACTIVE = 0;
                        treatFilter.HAS_HEIN_APPROVAL = false;
                    }
                    else if (activeId == 7)//Đã duyệt bhyt nhưng chưa khóa bhyt
                    {
                        treatFilter.HAS_HEIN_APPROVAL = true;
                        treatFilter.HAS_LOCK_HEIN = false;
                    }
                    else if (activeId == 8)//Đã khóa BHYT nhưng chưa thanh toán
                    {
                        treatFilter.HAS_LOCK_HEIN = true;
                        treatFilter.HAS_PAY = true;
                    }
                }
            }
        }

        private void SetFocusTreatmentCodeFind()
        {
            try
            {
                txtFindTreatmentCode.Focus();
                txtFindTreatmentCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControl()
        {
            try
            {
                ValidControlFindTreatmentCode();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlFindTreatmentCode()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;
                BaseEditViewInfo viewInfo = edit.GetViewInfo() as BaseEditViewInfo;
                if (viewInfo == null)
                    return;
                if (positionHandleControl == -1)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.SelectAll();
                        edit.Focus();
                    }
                }
                if (positionHandleControl > edit.TabIndex)
                {
                    positionHandleControl = edit.TabIndex;
                    if (edit.Visible)
                    {
                        edit.Focus();
                        edit.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadKeyUCLanguage()
        {
            try
            {
                //Button
                this.btnBill.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__BTN_BILL", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnBillNotKC.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__BTN_BILL_NOT_KC", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnDeposit.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__BTN_DEPOSIT", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnMienGiam.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__POPUP_MENU__ITEM_MIEN_GIAM", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnDepositService.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__BTN_DEPOSIT_SERVICE", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                //this.btnFind.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__BTN_FIND", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnInvoice.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__BTN_INVOICE", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnLock.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__BTN_LOCK", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnLockHistory.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__BTN_LOCK_HISTORY", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnRepayService.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__BTN_REPAY_SERVICE", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnRepay.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__BTN_REPAY", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnTranfer.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__BTN_TRANSFER", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnBordereau.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__BTN_BORDEREAU", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnTranList.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__BTN_TRANSACTION_LIST", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnTranList.ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__BTN_TRANSACTION_LIST_TOOLTIP", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                //Layout
                this.layoutFromTime.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__LAYOUT_FROM_TIME", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutToTime.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__LAYOUT_TO_TIME", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciOutTimeSearch.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__LAYOUT_OUT_TIME_SEARCH", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciBranch.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__LAYOUT_CUNG_CO_SO", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciBranch.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__LAYOUT_CUNG_CO_SO_TOOLTIP", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciTreatmentType.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__LAYOUT_TREATMENT_TYPE", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciTreatmentType.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__LAYOUT_TREATMENT_TYPE_TOOLTIP", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciEndDeparment.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__LAYOUT_END_DEPARTMENT", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciEndDeparment.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__LAYOUT_END_DEPARTMENT_TOOLTIP", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciInDate.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__LAYOUT_IN_DATE", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciTreatmentEndType.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__LAYOUT_TREATMENT_END_TYPE_NAME", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciTreatmentResult.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__LAYOUT_TREATMENT_RESULT_NAME", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciOutDate.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__LAYOUT_OUT_DATE_LBL", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciInHospital.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__LAYOUT_IN_HOSPITAL", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciInHospital.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__LAYOUT_IN_HOSPITAL_TOOLTIP", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciTotalTreatDay.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__LAYOUT_TOTAL_TREAT_DAY", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciIcd.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__LAYOUT_ICD", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciSubIcd.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__LAYOUT_SUB_ICD", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                //GridControl Blood
                this.gridColumn_Treatment_CreateTime.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__GRID_TREATMENT__COLUMN_CREATE_TIME", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Treatment_OutTime.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__GRID_TREATMENT__COLUMN_OUT_TIME", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Treatment_Creator.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__GRID_TREATMENT__COLUMN_CREATOR", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Treatment_Dob.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__GRID_TREATMENT__COLUMN_DOB", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Treatment_GenderName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__GRID_TREATMENT__COLUMN_GENDER_NAME", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Treatment_GenderName.ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__GRID_TREATMENT__COLUMN_GENDER_NAME_TOOLTIP", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Treatment_LockTime.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__GRID_TREATMENT__COLUMN_LOCK_TIME", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Treatment_PatientCode.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__GRID_TREATMENT__COLUMN_PATIENT_CODE", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Treatment_PatientCode.ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__GRID_TREATMENT__COLUMN_PATIENT_CODE_TOOLTIP", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Treatment_Stt.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__GRID_TREATMENT__COLUMN_STT", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Treatment_TreatmentCode.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__GRID_TREATMENT__COLUMN_TREATMENT_CODE", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Treatment_TreatmentResultName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__GRID_TREATMENT__COLUMN_TREATMENT_RESULT_NAME", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Treatment_VirAddress.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__GRID_TREATMENT__COLUMN_VIR_ADDRESS", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Treatment_VirPatientName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__GRID_TREATMENT__COLUMN_VIR_PATIENT_NAME", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                //Repository Button
                this.repositoryItemImgIsLockFee.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__REPOSITORY__BTN_LOCK_FEE", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.repositoryItemImgIsLockHein.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__REPOSITORY__BTN_LOCK_HEIN", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.repositoryItemImgIsPause.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__REPOSITORY__BTN_PAUSE", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.repositoryItemImgOpen.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__REPOSITORY__BTN_OPEN", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                this.txtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__TXT_KEYWORD_NULL_VALUE", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.txtFindTreatmentCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__TXT_FIND_TREATMENT_CODE_NULL_VALUE", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.txtFindPatientCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__TXT_FIND_PATIENT_CODE_NULL_VALUE", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.LciPatientType.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__LCI_PATIENT_TYPE", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.LciDepartment.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__LCI_LAST_DEPARTMENT", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnOtherPayment.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__OTHER_PAYMENT", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciNextDepartment.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__LCI_NEXT_DEPARTMENT", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                this.lciChkIsInDebt.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__LCI_CHK_IS_IN_DEBT", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciChkIsInDebt.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__LCI_CHK_IS_IN_DEBT_TOOLTIP", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                //minhnq
                this.txtInCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCTransaction.txtInCode.Properties.NullValuePrompt", Base.ResourceLangManager.LanguageUCTransaction, LanguageManager.GetCulture());
                this.LciActive.Text = Inventec.Common.Resource.Get.Value("UCTransaction.LciActive.Text", Base.ResourceLangManager.LanguageUCTransaction, LanguageManager.GetCulture());
                this.chkIsInDebt.Properties.Caption = Inventec.Common.Resource.Get.Value("UCTransaction.chkIsInDebt.Properties.Caption", Base.ResourceLangManager.LanguageUCTransaction, LanguageManager.GetCulture());
                this.chkIsInDebt.ToolTip = Inventec.Common.Resource.Get.Value("UCTransaction.chkIsInDebt.ToolTip", Base.ResourceLangManager.LanguageUCTransaction, LanguageManager.GetCulture());
                this.btnCallPatient.Text = Inventec.Common.Resource.Get.Value("UCTransaction.btnCallPatient.Text", Base.ResourceLangManager.LanguageUCTransaction, LanguageManager.GetCulture());
                this.btnRecallPatient.Text = Inventec.Common.Resource.Get.Value("UCTransaction.btnRecallPatient.Text", Base.ResourceLangManager.LanguageUCTransaction, LanguageManager.GetCulture());
                this.btnFind.Text = Inventec.Common.Resource.Get.Value("UCTransaction.btnFind.Text", Base.ResourceLangManager.LanguageUCTransaction, LanguageManager.GetCulture());
                this.layoutControlItem35.Text = Inventec.Common.Resource.Get.Value("UCTransaction.layoutControlItem35.Text", Base.ResourceLangManager.LanguageUCTransaction, LanguageManager.GetCulture());
                this.layoutControlItem36.Text = Inventec.Common.Resource.Get.Value("UCTransaction.layoutControlItem36.Text", Base.ResourceLangManager.LanguageUCTransaction, LanguageManager.GetCulture());
                this.layoutControlItem37.Text = Inventec.Common.Resource.Get.Value("UCTransaction.layoutControlItem37.Text", Base.ResourceLangManager.LanguageUCTransaction, LanguageManager.GetCulture());
                this.layoutControlItem38.Text = Inventec.Common.Resource.Get.Value("UCTransaction.layoutControlItem38.Text", Base.ResourceLangManager.LanguageUCTransaction, LanguageManager.GetCulture());
                this.lciNextDepartment.Text = Inventec.Common.Resource.Get.Value("UCTransaction.lciNextDepartment.Text", Base.ResourceLangManager.LanguageUCTransaction, LanguageManager.GetCulture());
                this.lciInDate.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("UCTransaction.lciInDate.OptionsToolTip.ToolTip", Base.ResourceLangManager.LanguageUCTransaction, LanguageManager.GetCulture());
                this.gridColumn_Treatment_PatientPhone.Caption = Inventec.Common.Resource.Get.Value("UCTransaction.gridColumn_Treatment_PatientPhone.Caption", Base.ResourceLangManager.LanguageUCTransaction, LanguageManager.GetCulture());
                this.gridColumn_Treatment_MilitaryRank.Caption = Inventec.Common.Resource.Get.Value("UCTransaction.gridColumn_Treatment_MilitaryRank.Caption", Base.ResourceLangManager.LanguageUCTransaction, LanguageManager.GetCulture());
                this.gridColumn_Treatment_WorkPlaceName.Caption = Inventec.Common.Resource.Get.Value("UCTransaction.gridColumn_Treatment_WorkPlaceName.Caption", Base.ResourceLangManager.LanguageUCTransaction, LanguageManager.GetCulture());
                this.gridColumn_Treatment_PostionName.Caption = Inventec.Common.Resource.Get.Value("UCTransaction.gridColumn_Treatment_PostionName.Caption", Base.ResourceLangManager.LanguageUCTransaction, LanguageManager.GetCulture());



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

        public void BtnInvoice()
        {
            btnInvoice_Click(null, null);
        }

        public void BtnDeposit()
        {
            btnDeposit_Click(null, null);
        }

        public void BtnMienGiam()
        {
            btnMienGiam_Click(null, null);
        }

        public void BtnDepositService()
        {
            try
            {
                if (btnDepositService.Enabled)
                    btnDepositService_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void BtnRepay()
        {
            btnRepay_Click(null, null);
        }

        public void BtnRepayService()
        {
            try
            {
                if (btnRepayService.Enabled)
                    btnRapayService_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void BtnBill()
        {
            btnBill_Click(null, null);
        }

        public void BtnBillNotKc()
        {
            btnBillNotKC_Click(null, null);
        }

        public void BtnLock()
        {
            btnLock_Click(null, null);
        }

        public void BtnLockHistory()
        {
            btnLockHistory_Click(null, null);
        }

        public void BtnBordereau()
        {
            try
            {
                btnBordereau_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void TemporaryLock()
        {
            try
            {
                btnTemporaryLock_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void BtnTranList()
        {
            try
            {
                btnTranList_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void FocusKeyword()
        {
            try
            {
                txtFindTreatmentCode.Focus();
                txtFindTreatmentCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void FocusPatientCode()
        {
            try
            {
                txtFindPatientCode.Focus();
                txtFindPatientCode.SelectAll();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void BtnCallPatient()
        {
            try
            {
                btnCallPatient_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public void BtnRecallPatient()
        {
            try
            {
                btnRecallPatient_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnTemporaryLock_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.currentTreatment == null)
                {
                    return;
                }
                bool success = false;
                CommonParam param = new CommonParam();
                HIS_TREATMENT result = null;
                WaitingManager.Show();

                HisTreatmentTemporaryLockSDO hisTreatmentTemporaryLockSDO = new HisTreatmentTemporaryLockSDO();
                hisTreatmentTemporaryLockSDO.TreatmentId = this.currentTreatment.ID;
                hisTreatmentTemporaryLockSDO.RequestRoomId = this.RoomId;
                if (this.currentTreatment.IS_TEMPORARY_LOCK == 1)
                {

                    result = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_TREATMENT>("api/HisTreatment/CancelTemporaryLock", ApiConsumer.ApiConsumers.MosConsumer, hisTreatmentTemporaryLockSDO, param);
                }
                else
                {
                    result = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_TREATMENT>("api/HisTreatment/TemporaryLock", ApiConsumer.ApiConsumers.MosConsumer, hisTreatmentTemporaryLockSDO, param);
                }

                WaitingManager.Hide();
                if (result != null)
                {
                    this.currentTreatment.IS_TEMPORARY_LOCK = result.IS_TEMPORARY_LOCK;
                    if (result.IS_TEMPORARY_LOCK == 1)
                    {
                        btnTemporaryLock.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__BTN_TEMPORARY_UN_LOCK", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    }
                    else
                    {
                        btnTemporaryLock.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__BTN_TEMPORARY_LOCK", Base.ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    }
                    success = true;
                }
                MessageManager.Show(param, success);
                SessionManager.ProcessTokenLost(param);
                if (success)
                {
                    FillDataToControlBySelectTreatment(true);
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboTreatmentType_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string dayName = "";
                if (this.treatmentTypeSelecteds != null && this.treatmentTypeSelecteds.Count > 0)
                {
                    foreach (var item in this.treatmentTypeSelecteds)
                    {
                        dayName += item.TREATMENT_TYPE_NAME;
                        if (!(item == treatmentTypeSelecteds.Last()))
                        {
                            dayName += ", ";
                        }
                    }
                }

                e.DisplayText = dayName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboEndDepartment_CustomDisplayText(object sender, DevExpress.XtraEditors.Controls.CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string dayName = "";
                if (this.departmentSelecteds != null && this.departmentSelecteds.Count > 0)
                {
                    foreach (var item in this.departmentSelecteds)
                    {
                        dayName += item.DEPARTMENT_NAME;
                        if (!(item == departmentSelecteds.Last()))
                        {
                            dayName += ", ";
                        }
                    }
                }

                e.DisplayText = dayName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CboPatientType_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
        {
            try
            {
                e.DisplayText = "";
                string dayName = "";
                if (this.PatientTypeSelecteds != null && this.PatientTypeSelecteds.Count > 0)
                {
                    foreach (var item in this.PatientTypeSelecteds)
                    {
                        dayName += item.PATIENT_TYPE_NAME;
                        if (!(item == PatientTypeSelecteds.Last()))
                        {
                            dayName += ", ";
                        }
                    }
                }

                e.DisplayText = dayName;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtInTimeFrom_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    dtInTimeFrom.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtInTimeTo_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    dtInTimeTo.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtOutTimeFrom_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    dtOutTimeFrom.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dtOutTimeTo_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    dtOutTimeTo.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboTreatmentType_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                //cboTreatmentType.Enabled = true;
                //cboTreatmentType.Focus();

                if (isNotLoadWhileChkIsInDebtStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == cboTreatmentType.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                List<string> listIDTreatmentType = this.treatmentTypeSelecteds.Select(o => o.ID.ToString()).ToList();
                string strIDS = string.Join(",", listIDTreatmentType);
                Inventec.Common.Logging.LogSystem.Debug("11111     " + strIDS);
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = strIDS;
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = cboTreatmentType.Name;
                    csAddOrUpdate.VALUE = strIDS;
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboEndDepartment_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    cboEndDepartment.EditValue = cboEndDepartment.Properties.GetKeyValue(gridView1.FocusedRowHandle);
                }

                if (isNotLoadWhileChkIsInDebtStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == cboEndDepartment.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                List<string> listIDEndDepartment = this.departmentSelecteds.Select(o => o.ID.ToString()).ToList();
                string strIDS = string.Join(",", listIDEndDepartment);
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = strIDS;
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = cboEndDepartment.Name;
                    csAddOrUpdate.VALUE = strIDS;
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboEndDepartment_MouseClick(object sender, MouseEventArgs e)
        {
            //GridView view = sender as GridView;
            //if (view == null)
            //    return;
            //view.OptionsSelection.MultiSelect = false;
            //view.ClearSelection();
            //view.OptionsSelection.MultiSelect = true;
            //cboEndDepartment.Properties.View.SelectRow(view.FocusedRowHandle);

        }

        private void cboEndDepartment_QueryPopUp(object sender, CancelEventArgs e)
        {
            //cboEndDepartment.Properties.View.OptionsSelection.MultiSelect = false;
            //cboEndDepartment.Properties.View.ClearSelection();
            //cboEndDepartment.Properties.View.OptionsSelection.MultiSelect = true;
            //cboEndDepartment.Properties.View.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CheckBoxRowSelect;
            //cboEndDepartment.Properties.View.SelectRow(cboEndDepartment.Properties.View.FocusedRowHandle);
        }

        private void cboEndDepartment_Properties_MouseDown(object sender, MouseEventArgs e)
        {
            //GridView view = sender as GridView;
            //var hi = view.CalcHitInfo(e.Location);
            //if (hi.InColumnPanel)
            //{
            //    DevExpress.Utils.DXMouseEventArgs.GetMouseArgs(e).Handled = true;
            //    int dataRowCount = view.DataRowCount;
            //    gridView1.BeginSelection();
            //    for (int i = 0; i < dataRowCount; i++)
            //    {
            //        if (i == gridView1.FocusedRowHandle)
            //            view.SelectRow(i);
            //    }
            //    gridView1.EndSelection();
            //}
        }

        private void toolTipController1_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                if (e.SelectedControl == gridControlPatientTypeInfo)
                {
                    GridView view = gridControlPatientTypeInfo.FocusedView as GridView;
                    GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
                    if (info.InRowCell)
                    {
                        if (info.Column.FieldName == "TITLE")
                        {
                            string tile = view.GetRowCellValue(info.RowHandle, info.Column).ToString();
                            if (tile == Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__TREATMENT_INFO__LAYOUT_KSK_PAYMENT_RATIO", ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()))
                            {
                                string tlThanhToan = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__TREATMENT_INFO__LAYOUT_KSK_PAYMENT_RATIO_TOOLTIP", ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                                string cellKey = info.RowHandle.ToString() + " - " + info.Column.ToString();
                                e.Info = new DevExpress.Utils.ToolTipControlInfo(cellKey, tlThanhToan);
                            }
                            if (tile == "Nơi ĐKKCBBD:")
                            {
                                string DKKCBBD = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__TREATMENT_INFO__LAYOUT_NOI_DKKCBBD_TOTIP", ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                                string cellKey = info.RowHandle.ToString() + " - " + info.Column.ToString();
                                e.Info = new DevExpress.Utils.ToolTipControlInfo(cellKey, DKKCBBD);

                            }
                            string hieuLuc = view.GetRowCellValue(info.RowHandle, info.Column).ToString();
                            if (hieuLuc == Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__TREATMENT_INFO__LAYOUT_KSK_EFFECT_EXPIRY_DATE", ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()))
                            {
                                string ngayHieuLucHetHan = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_TRANSACTION__TREATMENT_INFO__LAYOUT_KSK_EFFECT_EXPIRY_DATE_TOOLTIP", ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                                string cellKey = info.RowHandle.ToString() + " - " + info.Column.ToString();
                                e.Info = new DevExpress.Utils.ToolTipControlInfo(cellKey, ngayHieuLucHetHan);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboEndDepartment_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                //if(e.KeyCode == Keys.Down)
                //cboEndDepartment.EditValue = cboEndDepartment.Properties.GetKeyValue(gridView1.FocusedRowHandle);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void btnMienGiam_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.currentTreatment == null)
                    return;
                Inventec.Desktop.Common.Modules.Module moduleData = HIS.Desktop.LocalStorage.LocalData.GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.Exemptions").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.Exemptions'");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    moduleData.RoomId = this.RoomId;
                    moduleData.RoomTypeId = this.RoomTypeId;
                    List<object> listArgs = new List<object>();
                    listArgs.Add(this.currentTreatment.ID);
                    listArgs.Add(moduleData);
                    var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(HIS.Desktop.Utility.PluginInstance.GetModuleWithWorkingRoom(moduleData, this.RoomId, this.RoomTypeId), listArgs);
                    if (extenceInstance == null)
                    {
                        throw new ArgumentNullException("extenceInstance is null");
                    }

                    ((Form)extenceInstance).ShowDialog();
                    FillDataToControlBySelectTreatment(true);
                    txtFindTreatmentCode.Focus();
                    txtFindTreatmentCode.SelectAll();
                }
                else
                {
                    MessageManager.Show(Base.ResourceMessageLang.ChucNangNayChuaDuocHoTroTrongPhienBanNay);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void BtnThanhToanKhac()
        {
            try
            {
                if (this.currentTreatment == null)
                    return;
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TransactionBillOther").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.TransactionBillOther'");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    List<object> listArgs = new List<object>();
                    listArgs.Add(this.currentTreatment.ID);
                    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.RoomId, this.RoomTypeId), listArgs);
                    if (extenceInstance == null)
                    {
                        throw new ArgumentNullException("extenceInstance is null");
                    }

                    ((Form)extenceInstance).ShowDialog();
                    FillDataToControlBySelectTreatment(true);
                    txtFindTreatmentCode.Focus();
                    txtFindTreatmentCode.SelectAll();
                }
                else
                {
                    MessageManager.Show(Base.ResourceMessageLang.ChucNangNayChuaDuocHoTroTrongPhienBanNay);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CboPatientType_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    CboPatientType.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CboPatientType_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void CboPatientType_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboEndDepartment.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CboPatientType_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    CboPatientType.EditValue = CboPatientType.Properties.GetKeyValue(gridView1.FocusedRowHandle);
                }

                if (isNotLoadWhileChkIsInDebtStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == CboPatientType.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                List<string> listIDPatientType = this.PatientTypeSelecteds.Select(o => o.ID.ToString()).ToList();
                string strIDS = string.Join(",", listIDPatientType);
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = strIDS;
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = CboPatientType.Name;
                    csAddOrUpdate.VALUE = strIDS;
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnTransactionDebt_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TransactionDebt").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.TransactionDebt'");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    moduleData.RoomId = this.RoomId;
                    moduleData.RoomTypeId = this.RoomTypeId;
                    List<object> listArgs = new List<object>();

                    listArgs.Add(this.currentTreatment);
                    listArgs.Add(this.lastPatientType);
                    listArgs.Add(moduleData);
                    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.RoomId, this.RoomTypeId), listArgs);
                    if (extenceInstance == null)
                    {
                        throw new ArgumentNullException("extenceInstance is null");
                    }

                    ((Form)extenceInstance).ShowDialog();
                    FillDataToControlBySelectTreatment(true);
                    txtFindTreatmentCode.Focus();
                    txtFindTreatmentCode.SelectAll();
                }
                else
                {
                    MessageManager.Show(Base.ResourceMessageLang.ChucNangNayChuaDuocHoTroTrongPhienBanNay);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnTransactionDebtCollect_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TransactionDebtCollect").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.TransactionDebtCollect'");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    moduleData.RoomId = this.RoomId;
                    moduleData.RoomTypeId = this.RoomTypeId;
                    List<object> listArgs = new List<object>();

                    listArgs.Add(this.currentTreatment);
                    listArgs.Add(this.lastPatientType);
                    listArgs.Add(moduleData);
                    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.RoomId, this.RoomTypeId), listArgs);
                    if (extenceInstance == null)
                    {
                        throw new ArgumentNullException("extenceInstance is null");
                    }

                    ((Form)extenceInstance).ShowDialog();
                    FillDataToControlBySelectTreatment(true);
                    txtFindTreatmentCode.Focus();
                    txtFindTreatmentCode.SelectAll();
                }
                else
                {
                    MessageManager.Show(Base.ResourceMessageLang.ChucNangNayChuaDuocHoTroTrongPhienBanNay);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void sereServTree_CustomUnboundColumnData(SereServADO data, DevExpress.XtraTreeList.TreeListCustomColumnDataEventArgs e)
        {
            try
            {
                if (data != null && e.Node.ParentNode == null)
                {
                }
                else if (data != null && e.Node.HasChildren)
                {
                    if (e.Column.FieldName == "VIR_TOTAL_PRICE_DISPLAY")
                    {
                        e.Value = Inventec.Common.Number.Convert.NumberToString((data.VIR_TOTAL_PRICE ?? 0), ConfigApplications.NumberSeperator);
                    }
                    else if (e.Column.FieldName == "VIR_TOTAL_HEIN_PRICE_DISPLAY")
                    {
                        e.Value = Inventec.Common.Number.Convert.NumberToString((data.VIR_TOTAL_HEIN_PRICE ?? 0), ConfigApplications.NumberSeperator);
                    }
                    else if (e.Column.FieldName == "VIR_TOTAL_PATIENT_PRICE_DISPLAY")
                    {
                        e.Value = Inventec.Common.Number.Convert.NumberToString((data.VIR_TOTAL_PATIENT_PRICE ?? 0), ConfigApplications.NumberSeperator);
                    }
                }
                else if (data != null && !e.Node.HasChildren)
                {
                    if (e.Column.FieldName == "VIR_TOTAL_PRICE_DISPLAY")
                    {
                        e.Value = Inventec.Common.Number.Convert.NumberToString((data.VIR_TOTAL_PRICE ?? 0), ConfigApplications.NumberSeperator);
                    }
                    else if (e.Column.FieldName == "VIR_TOTAL_HEIN_PRICE_DISPLAY")
                    {
                        e.Value = Inventec.Common.Number.Convert.NumberToString((data.VIR_TOTAL_HEIN_PRICE ?? 0), ConfigApplications.NumberSeperator);
                    }
                    else if (e.Column.FieldName == "VIR_TOTAL_PATIENT_PRICE_DISPLAY")
                    {
                        e.Value = Inventec.Common.Number.Convert.NumberToString((data.VIR_TOTAL_PATIENT_PRICE ?? 0), ConfigApplications.NumberSeperator);
                    }
                    else if (e.Column.FieldName == "VIR_PRICE_DISPLAY")
                    {
                        e.Value = Inventec.Common.Number.Convert.NumberToString((data.VIR_PRICE ?? 0), ConfigApplications.NumberSeperator);
                    }
                    else if (e.Column.FieldName == "DISCOUNT_DISPLAY")
                    {
                        e.Value = Inventec.Common.Number.Convert.NumberToString((data.DISCOUNT ?? 0), ConfigApplications.NumberSeperator);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            try
            {
                BtnThanhToanKhac();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void btnBillNotKC_Click(object sender, EventArgs e)
        {
            try
            {
                if ((!btnBillNotKC.Enabled || this.currentTreatment == null) && !HisConfigCFG.IsketChuyenCFG.Equals("4"))
                    return;

                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TransactionBillSelect").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.TransactionBillSelect'");
                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                {
                    moduleData.RoomId = this.RoomId;
                    moduleData.RoomTypeId = this.RoomTypeId;
                    List<object> listArgs = new List<object>();

                    listArgs.Add(this.currentTreatment);
                    listArgs.Add(this.listSereServ);
                    listArgs.Add(this.lastPatientType);
                    listArgs.Add(moduleData);
                    listArgs.Add(true);
                    var extenceInstance = PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.RoomId, this.RoomTypeId), listArgs);
                    if (extenceInstance == null)
                    {
                        throw new ArgumentNullException("extenceInstance is null");
                    }

                    ((Form)extenceInstance).ShowDialog();
                    FillDataToControlBySelectTreatment(true);
                    txtFindTreatmentCode.Focus();
                    txtFindTreatmentCode.SelectAll();
                }
                else
                {
                    MessageManager.Show(Base.ResourceMessageLang.ChucNangNayChuaDuocHoTroTrongPhienBanNay);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void InitControlState()
        {
            isNotLoadWhileChkIsInDebtStateInFirst = true;
            try
            {
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(moduleLink);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == chkIsInDebt.Name)
                        {
                            chkIsInDebt.Checked = item.VALUE == "1";
                        }
                        if (item.KEY == cboIsActive.Name)
                        {
                            cboIsActive.EditValue = item.VALUE;
                        }
                        if (item.KEY == txtGateNumber.Name)
                        {
                            txtGateNumber.Text = item.VALUE;
                        }
                        if (item.KEY == cboTreatmentType.Name)
                        {
                            if (!String.IsNullOrEmpty(item.VALUE))
                            {
                                List<int> listIDS = item.VALUE.Split(',').Select(s => int.Parse(s)).ToList();
                                List<HIS_TREATMENT_TYPE> listTreatmentType = BackendDataWorker.Get<HIS_TREATMENT_TYPE>().Where(o => listIDS.Any(p => p == o.ID)).ToList();

                                GridCheckMarksSelection gridCheckMark = cboTreatmentType.Properties.Tag as GridCheckMarksSelection;
                                gridCheckMark.ClearSelection(cboTreatmentType.Properties.View);
                                cboTreatmentType.EditValue = null;
                                cboTreatmentType.Focus();
                                this.treatmentTypeSelecteds.AddRange(listTreatmentType);
                                gridCheckMark.SelectAll(this.treatmentTypeSelecteds);
                            }
                        }
                        if (item.KEY == CboPatientType.Name)
                        {
                            if (!String.IsNullOrEmpty(item.VALUE))
                            {
                                List<int> listIDS = item.VALUE.Split(',').Select(s => int.Parse(s)).ToList();
                                List<HIS_PATIENT_TYPE> listPatientType = BackendDataWorker.Get<HIS_PATIENT_TYPE>().Where(o => listIDS.Any(p => p == o.ID)).ToList();

                                GridCheckMarksSelection gridCheckMark = CboPatientType.Properties.Tag as GridCheckMarksSelection;
                                gridCheckMark.ClearSelection(CboPatientType.Properties.View);
                                CboPatientType.EditValue = null;
                                CboPatientType.Focus();
                                this.PatientTypeSelecteds.AddRange(listPatientType);
                                gridCheckMark.SelectAll(this.PatientTypeSelecteds);
                            }
                        }
                        if (item.KEY == cboEndDepartment.Name)
                        {
                            if (!String.IsNullOrEmpty(item.VALUE))
                            {
                                List<int> listIDS = item.VALUE.Split(',').Select(s => int.Parse(s)).ToList();
                                List<HIS_DEPARTMENT> listEndDepartment = BackendDataWorker.Get<HIS_DEPARTMENT>().Where(o => listIDS.Any(p => p == o.ID)).ToList();

                                GridCheckMarksSelection gridCheckMark = cboEndDepartment.Properties.Tag as GridCheckMarksSelection;
                                gridCheckMark.ClearSelection(cboEndDepartment.Properties.View);
                                cboEndDepartment.EditValue = null;
                                cboEndDepartment.Focus();
                                this.departmentSelecteds.AddRange(listEndDepartment);
                                gridCheckMark.SelectAll(this.departmentSelecteds);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            isNotLoadWhileChkIsInDebtStateInFirst = false;
        }

        private void chkIsInDebt_CheckedChanged(object sender, EventArgs e)
        {

            try
            {
                if (isNotLoadWhileChkIsInDebtStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkIsInDebt.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkIsInDebt.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkIsInDebt.Name;
                    csAddOrUpdate.VALUE = (chkIsInDebt.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtStepNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar))
                    e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtStepNumber_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnCallPatient_Click(object sender, EventArgs e)
        {
            try
            {
                this.CreateThreadCallPatient();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnRecallPatient_Click(object sender, EventArgs e)
        {
            try
            {
                this.CreateThreadRecallCallPatient();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboIsActive_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (isNotLoadWhileChkIsInDebtStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == cboIsActive.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (cboIsActive.EditValue.ToString() ?? "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = cboIsActive.Name;
                    csAddOrUpdate.VALUE = (cboIsActive.EditValue.ToString() ?? "");
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtInCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrEmpty(txtInCode.Text))
                    {
                        positionHandleControl = -1;
                        if (!dxValidationProvider1.Validate())
                            return;
                        WaitingManager.Show();
                        FillDataToGridTreatment();
                        if (listTreatment != null && listTreatment.Count == 1)
                        {
                            this.currentTreatment = listTreatment.First();
                            FillDataToControlBySelectTreatment(false, true);
                            SetEnableButton(null);
                        }
                        txtFindTreatmentCode.SelectAll();
                        WaitingManager.Hide();
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

        private void toolTipController2_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            try
            {
                if (e.SelectedControl == gridControlTreatment)
                {
                    GridView view = gridControlTreatment.FocusedView as GridView;
                    GridHitInfo info = view.CalcHitInfo(e.ControlMousePosition);
                    if (info.InRowCell)
                    {
                        V_HIS_TREATMENT_FEE dataRow = (V_HIS_TREATMENT_FEE)((IList)((BaseView)gridViewTreatment).DataSource)[info.RowHandle];

                        if (info.Column.FieldName == "DISPLAY_COLOR_STR" && dataRow.TDL_PATIENT_CLASSIFY_ID != null)
                        {
                            string patientName = BackendDataWorker.Get<HIS_PATIENT_CLASSIFY>().Where(o => o.ID == dataRow.TDL_PATIENT_CLASSIFY_ID).FirstOrDefault().PATIENT_CLASSIFY_NAME;
                            string cellKey = info.RowHandle.ToString() + " - " + info.Column.ToString();
                            e.Info = new DevExpress.Utils.ToolTipControlInfo(cellKey, patientName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtGateNumber_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    this.txtStepNumber.Focus();
                    this.txtStepNumber.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtGateNumber_Leave(object sender, EventArgs e)
        {
            try
            {
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == txtGateNumber.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = txtGateNumber.Text;
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = txtGateNumber.Name;
                    csAddOrUpdate.VALUE = txtGateNumber.Text;
                    csAddOrUpdate.MODULE_LINK = moduleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

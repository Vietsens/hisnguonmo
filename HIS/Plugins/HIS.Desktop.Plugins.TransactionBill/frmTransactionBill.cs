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
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraExport;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.TransactionBill.ADO;
using HIS.Desktop.Plugins.TransactionBill.Base;
using HIS.Desktop.Plugins.TransactionBill.Config;
using HIS.Desktop.Plugins.TransactionBill.Validtion;
using HIS.Desktop.Print;
using HIS.Desktop.Utility;
using HIS.UC.SereServTree;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using Inventec.Fss.Client;
using MOS.EFMODEL.DataModels;      
using MOS.Filter;
using MOS.LibraryHein.HcmPoorFund;
using MOS.SDO;
using Newtonsoft.Json;
using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TransactionBill
{
    public partial class frmTransactionBill : HIS.Desktop.Utility.FormBase
    {
        private static List<long> clsPtServiceTypeIds = new List<long>()
        {
            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__CDHA,
            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__GPBL,
            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__NS,
            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PHCN,
            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__PT,
            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__SA,
            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TDCN,
            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__TT,
            IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__XN          
        };

        bool isNotLoadWhilechkAutoCloseStateInFirst = true;
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorkerAutoClose;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDOAutoClose;
        string moduleLink = "HIS.Desktop.Plugins.TransactionBill";


        private const string SIGNED_EXTENSION = ".pdf";
        const string invoiceTypeCreate__CreateInvoiceVnpt = "1";
        const string invoiceTypeCreate__CreateInvoiceHIS = "2";
        public static decimal? RepayAmount;

        SereServTreeProcessor ssTreeProcessor = null;
        UserControl ucSereServTree = null;
        Inventec.Desktop.Common.Modules.Module currentModule = null;
        List<V_HIS_TRANSACTION> listTransaction = new List<V_HIS_TRANSACTION>();
        V_HIS_TRANSACTION resultTranBill = null;
        List<VHisBillFundADO> ListBillFund = new List<VHisBillFundADO>();
        List<V_HIS_SERE_SERV_5> ListSereServ = new List<V_HIS_SERE_SERV_5>();
        List<V_HIS_SERE_SERV_5> ListSereServNoExecute = new List<V_HIS_SERE_SERV_5>();
        List<V_HIS_SERE_SERV_5> currentSereServs = null;
        List<V_HIS_SERE_SERV_5> ListSereServTranfer;// list này từ module khác truyền sang, nếu không truyền thì gọi api để lấy về sereServ
        Dictionary<long, List<HIS_SERE_SERV_BILL>> dicSereServBill = null;
        List<V_HIS_ACCOUNT_BOOK> ListAccountBook = new List<V_HIS_ACCOUNT_BOOK>();
        List<V_HIS_ACCOUNT_BOOK> ListAccountBookDeposit = new List<V_HIS_ACCOUNT_BOOK>();
        List<V_HIS_ACCOUNT_BOOK> ListAccountBookRepay = new List<V_HIS_ACCOUNT_BOOK>();
        V_HIS_CASHIER_ROOM cashierRoom;
        long? treatmentId = null;
        V_HIS_TREATMENT_FEE currentTreatment = null;
        private int positionHandleControl = -1;
        bool isInit = true;
        string departmentName = "";
        internal string statusTreatmentOut { get; set; }
        decimal totalPatientPrice = 0;
        decimal totalPatientPriceFund = 0;
        decimal totalDiscount = 0;
        decimal totalFund = 0;
        decimal totalHienDu = 0;
        decimal totalCanThu = 0;
        decimal totalCanThuThem = 0;
        bool notHandleCheckedChanged = false;
        V_HIS_PATIENT_TYPE_ALTER resultPatientType;
        List<V_HIS_BILL_FUND> ListBillFundPay;
        HIS_BRANCH branch = null;
        string userName = "";        
        bool? IsDirectlyBilling = null;

        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentBySessionControlStateRDO;
        bool isNotLoadWhileChangeControlStateInFirst;
        List<PayFormADO> payFormList = new List<PayFormADO>();
        List<HIS_BANK> hisBankList = null;

        List<HIS_CARD> hisCard = null;
        V_HIS_PATIENT hispatient = null;
        string creator = "";
        bool hienHoaDonNhap = true;
        bool IsNeedAccountBook = true;
        bool IsPin = false;
        public bool printNowMps000113 = false;
        List<HIS_WORK_PLACE> dtWorkPlace = new List<HIS_WORK_PLACE>();
        V_HIS_TRANSACTION currentTransaction;
        List<long> lstSereServId = new List<long>();
        Timer timerClose = new Timer();
        bool PrintMps279 { get; set; }

        private List<HIS_BILL_FUND> listBillFundPrint { get; set; }
        private List<HIS_SERE_SERV_BILL> hisSSBillsPrint { get; set; }
        private List<HIS_SERE_SERV> listSereServPrint { get; set; }
        private V_HIS_PATIENT_TYPE_ALTER patientTypeAlterPrint { get; set; }
        private V_HIS_DEPARTMENT_TRAN departmentTranPrint { get; set; }
        private V_HIS_PATIENT patientsPrint { get; set; }
        private List<V_HIS_TRANSACTION> lstTranPrint { get; set; }
        private List<HIS_SESE_DEPO_REPAY> lstSeseRepayPrint { get; set; }
        private List<HIS_SERE_SERV_DEPOSIT> listSereDepoPrint { get; set; }
        public frmTransactionBill(Inventec.Desktop.Common.Modules.Module module, V_HIS_TREATMENT_FEE data, List<V_HIS_SERE_SERV_5> _ListSereServ, V_HIS_PATIENT_TYPE_ALTER patientTypeAlter, bool? isDirectlyBilling, V_HIS_TRANSACTION tran)
            : base(module)
        {
            Inventec.Common.Logging.LogSystem.Debug("frmTransactionBill.1. 1");
            InitializeComponent();
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("frmTransactionBill.1. 2");
                Base.ResourceLangManager.InitResourceLanguageManager();
                SetIcon();
                this.currentModule = module;
                this.IsDirectlyBilling = isDirectlyBilling;
                if (data != null)
                {
                    this.treatmentId = data.ID;
                    this.currentTreatment = data;
                }
                this.currentTransaction = tran;
                if (this.currentModule != null && this.IsDirectlyBilling.HasValue && this.IsDirectlyBilling.Value)
                {

                    this.Text =
                        Inventec.Common.Resource.Get.Value("frmTransactionBill.Text", Base.ResourceLangManager.LanguageFrmTransactionBill, LanguageManager.GetCulture()) + Inventec.Common.Resource.Get.Value("frmTransactionBill.DirectCollection", Base.ResourceLangManager.LanguageFrmTransactionBill, LanguageManager.GetCulture());
                    _ListSereServ = _ListSereServ.Where(o => o.PATIENT_TYPE_ID != HisConfigCFG.PatientTypeId__BHYT && o.VIR_TOTAL_HEIN_PRICE == 0 && o.VIR_TOTAL_PRICE != 0).ToList();
                }
                else
                {
                    this.Text = Inventec.Common.Resource.Get.Value("frmTransactionBill.Text", Base.ResourceLangManager.LanguageFrmTransactionBill, LanguageManager.GetCulture());

                }

                this.currentTransaction = tran;

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("_ListSereServ____", _ListSereServ));
                this.ListSereServTranfer = _ListSereServ;

                this.resultPatientType = patientTypeAlter;
                Inventec.Common.Logging.LogSystem.Debug("frmTransactionBill.1. 3");
                InitSereServTree();
                this.bindingSource1.DataSource = ListBillFund;
                userName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
                Inventec.Common.Logging.LogSystem.Debug("frmTransactionBill.1. 4");
                creator = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public frmTransactionBill(Inventec.Desktop.Common.Modules.Module module, V_HIS_TREATMENT_FEE data, V_HIS_PATIENT_TYPE_ALTER patientTypeAlter, bool? isDirectlyBilling, V_HIS_TRANSACTION tran)
            : base(module)
        {
            Inventec.Common.Logging.LogSystem.Debug("frmTransactionBill.2. 1");
            InitializeComponent();
            try
            {
                Base.ResourceLangManager.InitResourceLanguageManager();
                SetIcon();
                this.currentModule = module;
                this.IsDirectlyBilling = isDirectlyBilling;
                if (data != null)
                {
                    this.treatmentId = data.ID;
                    this.currentTreatment = data;
                }

                if (this.currentModule != null && this.IsDirectlyBilling == true)
                {
                    this.Text = Inventec.Common.Resource.Get.Value("frmTransactionBill.Text", Base.ResourceLangManager.LanguageFrmTransactionBill, LanguageManager.GetCulture()) + Inventec.Common.Resource.Get.Value("frmTransactionBill.DirectCollection", Base.ResourceLangManager.LanguageFrmTransactionBill, LanguageManager.GetCulture());
                }
                else
                {
                    this.Text = Inventec.Common.Resource.Get.Value("frmTransactionBill.Text", Base.ResourceLangManager.LanguageFrmTransactionBill, LanguageManager.GetCulture());
                }

                this.currentTransaction = tran;
                Inventec.Common.Logging.LogSystem.Debug("frmTransactionBill.2. 2");
                InitSereServTree();
                this.resultPatientType = patientTypeAlter;
                this.bindingSource1.DataSource = ListBillFund;
                userName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
                Inventec.Common.Logging.LogSystem.Debug("frmTransactionBill.2. 3");
                creator = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public frmTransactionBill(Inventec.Desktop.Common.Modules.Module module, bool? isDirectlyBilling, V_HIS_TRANSACTION tran)
            : base(module)
        {
            Inventec.Common.Logging.LogSystem.Debug("frmTransactionBill.3. 1");
            InitializeComponent();
            try
            {
                Base.ResourceLangManager.InitResourceLanguageManager();
                SetIcon();
                this.currentModule = module;
                this.IsDirectlyBilling = isDirectlyBilling;
                if (this.currentModule != null && this.IsDirectlyBilling == true)
                {
                    this.Text = Inventec.Common.Resource.Get.Value("frmTransactionBill.Text", Base.ResourceLangManager.LanguageFrmTransactionBill, LanguageManager.GetCulture()) + Inventec.Common.Resource.Get.Value("frmTransactionBill.DirectCollection", Base.ResourceLangManager.LanguageFrmTransactionBill, LanguageManager.GetCulture());
                }
                else
                {
                    this.Text = Inventec.Common.Resource.Get.Value("frmTransactionBill.Text", Base.ResourceLangManager.LanguageFrmTransactionBill, LanguageManager.GetCulture());
                }
                this.currentTransaction = tran;
                Inventec.Common.Logging.LogSystem.Debug("frmTransactionBill.3. 2");
                InitSereServTree();
                this.bindingSource1.DataSource = ListBillFund;
                userName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
                Inventec.Common.Logging.LogSystem.Debug("frmTransactionBill.3. 3");
                creator = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultValueTransaction()
        {
            try
            {
                if (currentTransaction != null && currentTransaction.IS_CANCEL == 1)
                {

                    cboAccountBook.EditValue = currentTransaction.ACCOUNT_BOOK_ID;
                    cboAccountBook.Enabled = false;
                    txtReplaceReason.Text = currentTransaction.REPLACE_REASON;
                    txtBuyerName.Text = currentTransaction.BUYER_NAME;
                    txtBuyerTaxCode.Text = currentTransaction.BUYER_TAX_CODE;
                    txtBuyerAddress.Text = currentTransaction.BUYER_ADDRESS;
                    //txtBuyEmail.Text = currentTransaction.BUYER_EMAIL;
                    txtBuyerAccountNumber.Text = currentTransaction.BUYER_ACCOUNT_NUMBER;

                    cboBuyerOrganization.EditValue = null;
                    txtBuyerOrganization.Text = currentTransaction.BUYER_ORGANIZATION;
                    chkOther.Checked = true;

                    btnStateForInformationUser.Properties.Buttons[0].Visible = false;
                    btnStateForInformationUser.Properties.Buttons[1].Visible = true;
                    IsPin = false;

                    lciReplaceReason.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    Inventec.Desktop.Common.Controls.ValidationRule.ControlMaxLengthValidationRule validate = new Inventec.Desktop.Common.Controls.ValidationRule.ControlMaxLengthValidationRule();
                    validate.editor = this.txtReplaceReason;
                    validate.maxLength = 1000;
                    validate.IsRequired = true;
                    validate.ErrorText = string.Format("Nhập quá ký tự cho phép {0}", 1000);
                    validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                    dxValidationProvider1.SetValidationRule(this.txtReplaceReason, validate);


                    HisSereServBillViewFilter ssBillFilter = new HisSereServBillViewFilter();
                    ssBillFilter.BILL_ID = currentTransaction.ID;
                    var hisSSBills = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV_BILL>>("api/HisSereServBill/GetView", ApiConsumers.MosConsumer, ssBillFilter, null);
                    if (hisSSBills != null && hisSSBills.Count > 0)
                        lstSereServId = hisSSBills.Select(o => o.SERE_SERV_ID).ToList();

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void SetIcon()
        {
            try
            {
                this.Icon = Icon.ExtractAssociatedIcon(System.IO.Path.Combine(ApplicationStoreLocation.ApplicationStartupPath, ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]));
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
                ssTreeProcessor = new UC.SereServTree.SereServTreeProcessor();
                SereServTreeADO ado = new SereServTreeADO();
                ado.IsShowCheckNode = true;
                ado.IsShowSearchPanel = false;
                ado.SereServTreeForBill_BeforeCheck = treeSereServ_BeforeCheckNode;
                ado.SereServTree_AfterCheck = treeSereServ_AfterCheckNode;
                ado.SereServTree_CheckAllNode = treeSereServ_CheckAllNode;
                ado.sereServTree_ShowingEditor = sereServTree_ShowingEditorDG;
                ado.SereServTree_CustomDrawNodeCell = treeSereServ_CustomDrawNodeCell;
                ado.SereServTree_CustomDrawNodeCheckBox = treeSereServ_CustomDrawNodeCheckBox;
                ado.SereServTree_CustomUnboundColumnData = treeSereServ_CustomUnboundColumnData;
                ado.SereServTreeColumns = new List<SereServTreeColumn>();
                ado.LayoutSereServExpend = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__LAYOUT_SERE_SERV_EXPEND", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                //Column tên dịch vụ
                SereServTreeColumn serviceNameCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__TREE_SERE_SERV__COLUMN_SERVICE_NAME", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "TDL_SERVICE_NAME", 200, false);
                serviceNameCol.VisibleIndex = 0;
                ado.SereServTreeColumns.Add(serviceNameCol);

                //Column Số lượng
                SereServTreeColumn amountCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__TREE_SERE_SERV__COLUMN_AMOUNT", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "AMOUNT_PLUS", 40, false);//AMOUNT_PLUS
                amountCol.VisibleIndex = 1;
                amountCol.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;
                amountCol.Format = new DevExpress.Utils.FormatInfo();
                amountCol.Format.FormatString = "#,##0.00";
                amountCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.SereServTreeColumns.Add(amountCol);

                //Column đơn giá
                SereServTreeColumn virPriceCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__TREE_SERE_SERV__COLUMN_VIR_PRICE", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "VIR_PRICE_DISPLAY", 110, false);//VIR_PRICE
                virPriceCol.VisibleIndex = 2;
                virPriceCol.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;
                //virPriceCol.Format = new DevExpress.Utils.FormatInfo();
                //virPriceCol.Format.FormatString = "#,##0.0000";
                //virPriceCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.SereServTreeColumns.Add(virPriceCol);

                //Column thành tiền
                SereServTreeColumn virTotalPriceCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__TREE_SERE_SERV__COLUMN_VIR_TOTAL_PRICE", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "VIR_TOTAL_PRICE_DISPLAY", 110, false);//VIR_TOTAL_PRICE
                virTotalPriceCol.VisibleIndex = 3;
                virTotalPriceCol.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;
                //virTotalPriceCol.Format = new DevExpress.Utils.FormatInfo();
                //virTotalPriceCol.Format.FormatString = "#,##0.0000";
                //virTotalPriceCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.SereServTreeColumns.Add(virTotalPriceCol);

                //Column đồng chi trả
                SereServTreeColumn virTotalHeinPriceCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__TREE_SERE_SERV__COLUMN_VIR_TOTAL_HEIN_PRICE", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "VIR_TOTAL_HEIN_PRICE_DISPLAY", 110, false);//VIR_TOTAL_HEIN_PRICE
                virTotalHeinPriceCol.VisibleIndex = 4;
                virTotalHeinPriceCol.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;

                //virTotalHeinPriceCol.Format = new DevExpress.Utils.FormatInfo();
                //virTotalHeinPriceCol.Format.FormatString = "#,##0.0000";
                //virTotalHeinPriceCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.SereServTreeColumns.Add(virTotalHeinPriceCol);

                //Column bệnh nhân trả
                SereServTreeColumn virTotalPatientPriceCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__TREE_SERE_SERV__COLUMN_VIR_TOTAL_PATIENT_PRICE", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "VIR_TOTAL_PATIENT_PRICE_DISPLAY", 110, false);//VIR_TOTAL_PATIENT_PRICE
                virTotalPatientPriceCol.VisibleIndex = 5;
                virTotalPatientPriceCol.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;

                //virTotalPatientPriceCol.Format = new DevExpress.Utils.FormatInfo();
                //virTotalPatientPriceCol.Format.FormatString = "#,##0.0000";
                //virTotalPatientPriceCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.SereServTreeColumns.Add(virTotalPatientPriceCol);

                //Column chiết khấu
                SereServTreeColumn virDiscountCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__TREE_SERE_SERV__COLUMN_DISCOUNT", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "DISCOUNT_DISPLAY", 110, false);
                virDiscountCol.VisibleIndex = 6;
                virDiscountCol.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;

                //virDiscountCol.Format = new DevExpress.Utils.FormatInfo();
                //virDiscountCol.Format.FormatString = "#,##0.0000";
                //virDiscountCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.SereServTreeColumns.Add(virDiscountCol);

                //Column hao phí
                SereServTreeColumn virIsExpendCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__TREE_SERE_SERV__COLUMN_IS_EXPEND", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "IsExpend", 60, false);
                virIsExpendCol.VisibleIndex = 7;
                ado.SereServTreeColumns.Add(virIsExpendCol);

                //Column vat (%)
                SereServTreeColumn virVatRatioCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__TREE_SERE_SERV__COLUMN_VAT_RATIO", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "VAT_DISPLAY", 100, false);
                virVatRatioCol.VisibleIndex = 8;
                virVatRatioCol.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;

                //virVatRatioCol.Format = new DevExpress.Utils.FormatInfo();
                //virVatRatioCol.Format.FormatString = "#,##0.00";
                //virVatRatioCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.SereServTreeColumns.Add(virVatRatioCol);

                //Column mã dịch vụ
                SereServTreeColumn serviceCodeCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__TREE_SERE_SERV__COLUMN_SERVICE_CODE", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "TDL_SERVICE_CODE", 100, false);
                serviceCodeCol.VisibleIndex = 9;
                ado.SereServTreeColumns.Add(serviceCodeCol);

                //Column Mã yêu cầu
                SereServTreeColumn serviceReqCodeCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__TREE_SERE_SERV__COLUMN_SERVICE_REQ_CODE", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "TDL_SERVICE_REQ_CODE", 100, false);
                serviceReqCodeCol.VisibleIndex = 10;
                ado.SereServTreeColumns.Add(serviceReqCodeCol);

                //Column mã giao dịch
                //SereServTreeColumn INSURANCE_EXPERTISECodeCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__TREE_SERE_SERV__COLUMN_TRANSACTION_CODE", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "TRANSACTION_CODE", 100, false);
                //INSURANCE_EXPERTISECodeCol.VisibleIndex = 11;
                //ado.SereServTreeColumns.Add(INSURANCE_EXPERTISECodeCol);

                this.ucSereServTree = (UserControl)ssTreeProcessor.Run(ado);
                if (this.ucSereServTree != null)
                {
                    this.panelControlTreeSereServ.Controls.Add(this.ucSereServTree);
                    this.ucSereServTree.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void sereServTree_ShowingEditorDG(TreeListNode node, object sender)
        {
            try
            {
                var nodeData = node.TreeList.GetDataRecordByNode(node);
                if (nodeData != null && Config.HisConfigCFG.MustFinishTreatmentForBill == "1" && (nodeData as SereServADO).PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT)
                {
                    ((TreeList)sender).ActiveEditor.Properties.ReadOnly = true;
                }
                else if (nodeData != null && Config.HisConfigCFG.MustFinishTreatmentForBill == "2")
                {
                    ((TreeList)sender).ActiveEditor.Properties.ReadOnly = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private bool SetStateEnableControl()
        {
            bool enable = false;
            try
            {
                if (HisConfigCFG.AutoCreateDepositTransaction && currentTreatment.IS_PAUSE == 1)
                {
                    cboDepositBook.Enabled = true;
                    spnNumOrder.Enabled = true;
                    enable = true;
                }
                else
                {
                    cboDepositBook.Enabled = false;
                    spnNumOrder.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return enable;
        }
        private void timerInitForm_Tick(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("timerInitForm_Tick. 1");
                this.timerInitForm.Stop();
                SetStateEnableControl();
                SetDefaultValueTransaction();
                this.LoadDataToTreeSereServ(false);//TODO
                Inventec.Common.Logging.LogSystem.Debug("timerInitForm_Tick. 1");
                this.EnableCheckNotTakenPress();
                Inventec.Common.Logging.LogSystem.Debug("timerInitForm_Tick. 2");
                this.FillInfoPatient(this.currentTreatment);
                Inventec.Common.Logging.LogSystem.Debug("timerInitForm_Tick. 3");
                this.CalcuTotalPrice();
                Inventec.Common.Logging.LogSystem.Debug("timerInitForm_Tick. 4");
                this.ProcessBillFund();
                this.ProcessFundForHCM();
                Inventec.Common.Logging.LogSystem.Debug("timerInitForm_Tick. 5");
                this.CalcuHienDu();
                this.CalcuCanThu();
                Inventec.Common.Logging.LogSystem.Debug("timerInitForm_Tick. 6");
                this.FillDataToButtonPrint();
                this.InitMenuToButtonPrint();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmTransactionBill_Load(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("frmTransactionBill_Load. 1");
                WaitingManager.Show();
                timerClose.Tick += new System.EventHandler(this.timerClose_Tick);
                timerClose.Interval = 100;
                CheckEnableBtnQR();
                this.InitComboBuyerOrganization();
                HisConfigCFG.LoadConfig();
                InitControlState();
                this.LoadKeyFrmLanguage();
                UpdateFormatSpin();
                this.InitElectrictBillConfig();
                this.AutoCheckRepaySetDefault();
                Inventec.Common.Logging.LogSystem.Debug("frmTransactionBill_Load. 2");
                this.LoadCashierRoomAndBranch();
                Inventec.Common.Logging.LogSystem.Debug("frmTransactionBill_Load. 3");
                this.ValidControl();
                Inventec.Common.Logging.LogSystem.Debug("frmTransactionBill_Load. 4");
                this.LoadAccountBookToLocal();
                this.LoadAccountBookRepayToLocal();
                this.LoadDataToComboFund();
                this.LoadDataToComboPayForm();
                this.FillDataToGirdTransaction();
                this.LoadDataToComboAccountBookDeposit();
                this.LoadComboAccountBook();
                Inventec.Common.Logging.LogSystem.Debug("frmTransactionBill_Load. 5");
                this.GeneratePopupMenu();
                this.ResetControlValue();
                Inventec.Common.Logging.LogSystem.Debug("frmTransactionBill_Load. 6");
                this.cboAccountBook.Focus();
                if (this.currentTreatment != null && !String.IsNullOrWhiteSpace(this.currentTreatment.TREATMENT_CODE))
                {
                    this.txtFindTreatmentCode.Text = this.currentTreatment.TREATMENT_CODE;
                    this.txtFindTreatmentCode.SelectionStart = this.txtFindTreatmentCode.Text.Length;
                    this.txtFindTreatmentCode.DeselectAll();
                }

                if (this.IsDirectlyBilling.HasValue && Config.HisConfigCFG.IsketChuyenCFG != null && Config.HisConfigCFG.IsketChuyenCFG.Equals("4"))
                    chkCoKetChuyen.Checked = !IsDirectlyBilling.Value;

                GetList();

                this.AddBarManager(this.barManager1);

                this.ProcessCustomizeUI();

                Inventec.Common.Logging.LogSystem.Debug("frmTransactionBill_Load. 7");
                isInit = false;
                timerInitForm.Interval = 100;
                timerInitForm.Enabled = true;
                timerInitForm.Start();
                WaitingManager.Hide();

                InitControlStateAutoClose();
                EnableSave();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void timerClose_Tick(object sender, EventArgs e)
        {
            try
            {
                if (PrintMps279)
                {
                    PrintMps279 = false;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void EnableSave()
        {
            try
            {
                ValidControlAccountBook();
                layoutAccountBook.AppearanceItemCaption.ForeColor = Color.Maroon;
                IsNeedAccountBook = true;
                btnSaveAndSign.Enabled = false;
                btnSavePrint.Enabled = false;
                btnSave.Enabled = false;
                var dtaPrice = ssTreeProcessor.GetListCheck(ucSereServTree);
                decimal totalPrice = 0;
                if (dtaPrice != null && dtaPrice.Count > 0)
                {
                    totalPrice = dtaPrice.Where(o => o.VIR_TOTAL_PATIENT_PRICE > 0).Sum(o => o.VIR_TOTAL_PATIENT_PRICE ?? 0);
                    if (HisConfigCFG.EnableSaveOption == "1")
                    {
                        if (totalPrice > 0 || currentTreatment.IS_PAUSE == (short?)1)
                        {
                            btnSave.Enabled = true;
                            btnSaveAndSign.Enabled = true;
                            btnSavePrint.Enabled = true;
                        }
                    }
                    else
                    {
                        if (totalPrice > 0)
                        {
                            btnSave.Enabled = true;
                            btnSaveAndSign.Enabled = true;
                            btnSavePrint.Enabled = true;
                        }

                    }
                }
                if (HisConfigCFG.EnableSaveOption == "1" && currentTreatment.IS_PAUSE == (short?)1)
                {
                    btnSave.Enabled = true;
                    btnSaveAndSign.Enabled = true;
                    btnSavePrint.Enabled = true;
                    if (HisConfigCFG.AllowToCreateNoPriceTransaction != "1" && totalPrice == 0)
                    {
                        dxValidationProvider1.SetValidationRule(cboAccountBook, null);
                        layoutAccountBook.AppearanceItemCaption.ForeColor = Color.Black;
                        IsNeedAccountBook = false;
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitControlStateAutoClose()
        {
            isNotLoadWhilechkAutoCloseStateInFirst = true;
            try
            {
                this.controlStateWorkerAutoClose = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDOAutoClose = controlStateWorkerAutoClose.GetData(moduleLink);
                if (this.currentControlStateRDOAutoClose != null && this.currentControlStateRDOAutoClose.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDOAutoClose)
                    {
                        if (item.KEY == chkAutoClose.Name)
                        {
                            switch (item.VALUE)
                            {
                                case "Unchecked":
                                    {
                                        chkAutoClose.CheckState = CheckState.Unchecked;
                                        break;
                                    }
                                case "Checked":
                                    {
                                        chkAutoClose.CheckState = CheckState.Checked;
                                        break;
                                    }
                                case "Indeterminate":
                                    {
                                        chkAutoClose.CheckState = CheckState.Indeterminate;
                                        break;
                                    }
                            }
                        }
                    }
                }
                isNotLoadWhilechkAutoCloseStateInFirst = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void AutoCheckRepaySetDefault()
        {
            try
            {
                if (this.currentTreatment.IS_PAUSE == 1)
                {
                    chkAutoRepay.Enabled = true;
                    chkAutoRepay.ReadOnly = false;
                    chkAutoRepay.Checked = HisConfigCFG.IsCheckAutoRepayAsDefault;
                }
                else
                {
                    chkAutoRepay.Checked = false;
                    chkAutoRepay.Enabled = false;
                    chkAutoRepay.ReadOnly = true;
                }

                cboAccountBookRepay.Enabled = chkAutoRepay.Checked;
                spinNumOrderRepay.Enabled = chkAutoRepay.Checked;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UpdateFormatSpin()
        {
            try
            {
                FormatControl(ConfigApplications.NumberSeperator, txtTotalAmount);
                FormatControl(ConfigApplications.NumberSeperator, spinTransferAmount);
                FormatControl(ConfigApplications.NumberSeperator, txtDiscount);
                FormatControl(ConfigApplications.NumberSeperator, spinAmountBNDua);
                FormatControl(ConfigApplications.NumberSeperator, repositoryItemTxtAmount);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string FormatControl(int numberDigit, DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit spinControl)
        {
            string format = "#,##0";
            try
            {
                switch (numberDigit)
                {
                    case 0:
                        format = "#,##0";
                        break;
                    case 1:
                        format = "#,##0.0";
                        break;
                    case 2:
                        format = "#,##0.00";
                        break;
                    case 3:
                        format = "#,##0.000";
                        break;
                    case 4:
                        format = "#,##0.0000";
                        break;
                    default:
                        break;
                }

                spinControl.Properties.EditFormat.FormatString = format;
                spinControl.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                spinControl.Properties.DisplayFormat.FormatString = format;
                spinControl.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return format;
        }

        private void FormatControl(int numberDigit, DevExpress.XtraEditors.SpinEdit spinControl)
        {
            string format = "#,##0";
            string formatDefault = "#,##0";
            try
            {
                switch (numberDigit)
                {
                    case 0:
                        format = "#,##0";
                        break;
                    case 1:
                        format = "#,##0.0";
                        break;
                    case 2:
                        format = "#,##0.00";
                        break;
                    case 3:
                        format = "#,##0.000";
                        break;
                    case 4:
                        format = "#,##0.0000";
                        break;
                    default:
                        break;
                }

                if (Math.Abs(spinControl.Value) % 1 == 0)
                {
                    spinControl.Properties.EditFormat.FormatString = formatDefault;
                    spinControl.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    spinControl.Properties.DisplayFormat.FormatString = formatDefault;
                    spinControl.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                }
                else
                {
                    spinControl.Properties.EditFormat.FormatString = format;
                    spinControl.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                    spinControl.Properties.DisplayFormat.FormatString = format;
                    spinControl.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UpdateDictionaryNumOrderAccountBook(V_HIS_ACCOUNT_BOOK accountBook, decimal numOrder)
        {
            try
            {
                if (accountBook != null && HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook != null && HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.Count > 0 && HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.ContainsKey(accountBook.ID))
                {
                    HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook[accountBook.ID] = numOrder;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultPayFormForUser()
        {
            try
            {
                if (this.payFormList != null && this.payFormList.Count > 0)
                {
                    var PayFormMinByCode = this.payFormList.OrderBy(o => o.PAY_FORM_CODE);
                    var payFormDefault = PayFormMinByCode.FirstOrDefault();
                    if (payFormDefault != null)
                    {
                        var data = this.payFormList.FirstOrDefault(o => o.PAY_FORM_CODE == payFormDefault.PAY_FORM_CODE);
                        if (data != null)
                        {
                            cboPayForm.EditValue = data.PayFormId;
                            //txtPayFormCode.Text = data.PAY_FORM_CODE;
                            CheckPayFormTienMatChuyenKhoan(data);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private decimal setDataToDicNumOrderInAccountBook(V_HIS_ACCOUNT_BOOK accountBook)
        {
            decimal result = (accountBook.CURRENT_NUM_ORDER ?? 0) + 1;
            try
            {
                if (accountBook != null)
                {
                    if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook == null || HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.Count == 0 || (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook != null && HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.Count > 0 && !HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.ContainsKey(accountBook.ID)))
                    {
                        if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook == null)
                        {
                            HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook = new Dictionary<long, decimal>();
                        }

                        CommonParam param = new CommonParam();
                        MOS.Filter.HisAccountBookViewFilter hisAccountBookViewFilter = new MOS.Filter.HisAccountBookViewFilter();
                        hisAccountBookViewFilter.ID = accountBook.ID;
                        var accountBooks = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_ACCOUNT_BOOK>>(ApiConsumer.HisRequestUriStore.HIS_ACCOUNT_BOOK_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, hisAccountBookViewFilter, param);
                        if (accountBooks != null && accountBooks.Count > 0)
                        {
                            var accountBookNew = accountBooks.FirstOrDefault();
                            decimal num = 0;
                            if ((accountBookNew.CURRENT_NUM_ORDER ?? 0) > 0)
                            {
                                num = (accountBookNew.CURRENT_NUM_ORDER ?? 0);
                            }
                            else
                            {
                                num = (decimal)accountBookNew.FROM_NUM_ORDER - 1;
                            }

                            HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.Add(accountBookNew.ID, num);
                            result = (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook[accountBook.ID]) + 1;
                        }
                    }
                    else
                    {
                        result = (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook[accountBook.ID]) + 1;
                    }
                }
                else
                {
                    result = (accountBook.CURRENT_NUM_ORDER ?? 0) + 1;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void InitElectrictBillConfig()
        {
            try
            {
                if (String.IsNullOrEmpty(TransactionBillConfig.InvoiceTypeCreate)
                    || (TransactionBillConfig.InvoiceTypeCreate != invoiceTypeCreate__CreateInvoiceVnpt && TransactionBillConfig.InvoiceTypeCreate != invoiceTypeCreate__CreateInvoiceHIS))
                {
                    lcibtnSaveAndSign.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    lciHideHddt.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadCashierRoomAndBranch()
        {
            try
            {
                if (this.currentModule != null)
                {
                    this.cashierRoom = BackendDataWorker.Get<V_HIS_CASHIER_ROOM>().FirstOrDefault(o => o.ROOM_ID == currentModule.RoomId && o.ROOM_TYPE_ID == currentModule.RoomTypeId);
                    if (cashierRoom != null)
                    {
                        departmentName = cashierRoom.DEPARTMENT_NAME;
                    }

                    branch = BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == WorkPlace.GetBranchId());
                }

                if (this.currentTreatment == null || this.currentTreatment.ID == 0)
                {
                    if (this.treatmentId.HasValue)
                    {
                        HisTreatmentFeeViewFilter feeFilter = new HisTreatmentFeeViewFilter();
                        feeFilter.ID = this.treatmentId.Value;
                        var treatmentFees = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_TREATMENT_FEE>>("api/HisTreatment/GetView2", ApiConsumers.MosConsumer, feeFilter, null);
                        if (treatmentFees == null || treatmentFees.Count == 0)
                        {
                            return;
                        }
                        this.currentTreatment = treatmentFees.First();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task LoadAccountBookToLocal()
        {
            try
            {
                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                this.ListAccountBook = new List<V_HIS_ACCOUNT_BOOK>();

                //Sửa lại đoạn code này
                //Api bổ sung filter chứ không get nhiều api
                //TODO               
                HisAccountBookViewFilter acFilter = new HisAccountBookViewFilter();
                acFilter.CASHIER_ROOM_ID = this.cashierRoom.ID;//Kiểm tra sổ còn hay k
                acFilter.LOGINNAME = loginName;//Kiểm tra sổ còn hay k
                acFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                acFilter.FOR_BILL = true;
                acFilter.IS_OUT_OF_BILL = false;
                acFilter.ORDER_DIRECTION = "DESC";
                acFilter.ORDER_FIELD = "ID";
                ListAccountBook = await new BackendAdapter(new CommonParam()).GetAsync<List<V_HIS_ACCOUNT_BOOK>>("api/HisAccountBook/GetView", ApiConsumers.MosConsumer, acFilter, null);
                if (ListAccountBook != null && ListAccountBook.Count > 0)
                {
                    if (WorkPlace.WorkInfoSDO != null && WorkPlace.WorkInfoSDO.WorkingShiftId.HasValue)
                    {
                        ListAccountBook = ListAccountBook.Where(o => !o.WORKING_SHIFT_ID.HasValue || o.WORKING_SHIFT_ID == WorkPlace.WorkInfoSDO.WorkingShiftId.Value).ToList();
                    }
                    else
                    {
                        ListAccountBook = ListAccountBook.Where(o => !o.WORKING_SHIFT_ID.HasValue).ToList();
                    }
                }

                LoadDataToComboAccountBook();
                SetDefaultAccountBook();//TODO
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToComboAccountBook()
        {
            try
            {
                cboAccountBook.Properties.DataSource = ListAccountBook;
                cboAccountBook.Properties.DisplayMember = "ACCOUNT_BOOK_NAME";
                cboAccountBook.Properties.ValueMember = "ID";
                cboAccountBook.Properties.ForceInitialize();
                cboAccountBook.Properties.Columns.Clear();
                cboAccountBook.Properties.Columns.Add(new LookUpColumnInfo("ACCOUNT_BOOK_CODE", "", 50));
                cboAccountBook.Properties.Columns.Add(new LookUpColumnInfo("ACCOUNT_BOOK_NAME", "", 200));
                cboAccountBook.Properties.ShowHeader = false;
                cboAccountBook.Properties.ImmediatePopup = true;
                cboAccountBook.Properties.DropDownRows = 10;
                cboAccountBook.Properties.PopupWidth = 250;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboAccountBookRepay(List<V_HIS_ACCOUNT_BOOK> db)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("ACCOUNT_BOOK_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("ACCOUNT_BOOK_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("ACCOUNT_BOOK_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(this.cboAccountBookRepay, db, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboBuyerOrganization()
        {
            try
            {
                dtWorkPlace = BackendDataWorker.Get<HIS_WORK_PLACE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                this.InitComboCommon(this.cboBuyerOrganization, dtWorkPlace, "ID", "WORK_PLACE_NAME", "TAX_CODE");

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboCommon(Control cboEditor, object data, string valueMember, string displayMember, string displayMemberCode)
        {
            try
            {
                InitComboCommon(cboEditor, data, valueMember, displayMember, 0, displayMemberCode, 0);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboCommon(Control cboEditor, object data, string valueMember, string displayMember, int displayMemberWidth, string displayMemberCode, int displayMemberCodeWidth)
        {
            try
            {
                int popupWidth = 0;
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                if (!String.IsNullOrEmpty(displayMember))
                {
                    columnInfos.Add(new ColumnInfo(displayMember, "Tên", (displayMemberWidth > 0 ? displayMemberWidth : 250), 1));
                    popupWidth += (displayMemberWidth > 0 ? displayMemberWidth : 350);
                }
                if (!String.IsNullOrEmpty(displayMemberCode))
                {
                    columnInfos.Add(new ColumnInfo(displayMemberCode, "Mã số thuế", (displayMemberCodeWidth > 0 ? displayMemberCodeWidth : 100), 2));
                    popupWidth += (displayMemberCodeWidth > 0 ? displayMemberCodeWidth : 100);
                }
                ControlEditorADO controlEditorADO = new ControlEditorADO(displayMember, valueMember, columnInfos, true, popupWidth);
                ControlEditorLoader.Load(cboEditor, data, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadDataToComboAccountBookDeposit()
        {
            try
            {
                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                this.ListAccountBookDeposit = new List<V_HIS_ACCOUNT_BOOK>();
                List<long> ids = new List<long>();
                HisUserAccountBookFilter useAccountBookFilter = new HisUserAccountBookFilter();
                useAccountBookFilter.LOGINNAME__EXACT = loginName;
                var userAccountBooks = new BackendAdapter(new CommonParam()).Get<List<HIS_USER_ACCOUNT_BOOK>>("api/HisUserAccountBook/Get", ApiConsumers.MosConsumer, useAccountBookFilter, null);

                List<HIS_CARO_ACCOUNT_BOOK> caroAccountBooks = null;
                HisCaroAccountBookFilter caroAccountBookFilter = new HisCaroAccountBookFilter();
                caroAccountBookFilter.CASHIER_ROOM_ID = Convert.ToInt64(currentModule.RoomId);
                caroAccountBooks = new BackendAdapter(new CommonParam()).Get<List<HIS_CARO_ACCOUNT_BOOK>>("api/HisCaroAccountBook/Get", ApiConsumers.MosConsumer, caroAccountBookFilter, null);
                // Kiểm tra sổ còn hay k
                if (userAccountBooks != null && userAccountBooks.Count > 0)
                {
                    ids.AddRange(userAccountBooks.Select(s => s.ACCOUNT_BOOK_ID).ToList());
                }
                if (caroAccountBooks != null && caroAccountBooks.Count > 0)
                {
                    ids.AddRange(caroAccountBooks.Select(s => s.ACCOUNT_BOOK_ID).ToList());
                }
                ids = ids.Distinct().ToList();
                if (ids != null && ids.Count > 0)
                {
                    int count = ids.Count;
                    int step = 0;
                    while (count > 0)
                    {
                        var lstId = ids.Skip(step).Take(100).ToList();
                        HisAccountBookViewFilter acFilter = new HisAccountBookViewFilter();
                        acFilter.IDs = lstId;
                        acFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                        acFilter.FOR_DEPOSIT = true;
                        acFilter.IS_OUT_OF_BILL = false;
                        acFilter.ORDER_DIRECTION = "DESC";
                        acFilter.ORDER_FIELD = "ID";
                        var dt = new BackendAdapter(new CommonParam()).Get<List<V_HIS_ACCOUNT_BOOK>>("api/HisAccountBook/GetView", ApiConsumers.MosConsumer, acFilter, null);
                        if (dt != null && dt.Count > 0)
                            ListAccountBookDeposit.AddRange(dt);
                        step += 100;
                        count -= 100;
                    }
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void LoadComboAccountBook()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("ACCOUNT_BOOK_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("ACCOUNT_BOOK_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("ACCOUNT_BOOK_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboDepositBook, ListAccountBookDeposit, controlEditorADO);

                SetDefaultAccountBookDeposit();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task LoadAccountBookRepayToLocal()
        {
            try
            {
                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                this.ListAccountBookRepay = new List<V_HIS_ACCOUNT_BOOK>();

                //Sửa lại đoạn code này
                //Api bổ sung filter chứ không get nhiều api
                HisAccountBookViewFilter acFilter = new HisAccountBookViewFilter();
                acFilter.CASHIER_ROOM_ID = this.cashierRoom.ID;//Kiểm tra sổ còn hay k
                acFilter.LOGINNAME = loginName;//Kiểm tra sổ còn hay k
                acFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                acFilter.FOR_REPAY = true;
                acFilter.IS_OUT_OF_BILL = false;
                acFilter.ORDER_DIRECTION = "DESC";
                acFilter.ORDER_FIELD = "ID";
                this.ListAccountBookRepay = await new BackendAdapter(new CommonParam()).GetAsync<List<V_HIS_ACCOUNT_BOOK>>("api/HisAccountBook/GetView", ApiConsumers.MosConsumer, acFilter, null);
                if (this.ListAccountBookRepay != null && this.ListAccountBookRepay.Count > 0)
                {
                    if (WorkPlace.WorkInfoSDO != null && WorkPlace.WorkInfoSDO.WorkingShiftId.HasValue)
                    {
                        this.ListAccountBookRepay = this.ListAccountBookRepay.Where(o => !o.WORKING_SHIFT_ID.HasValue || o.WORKING_SHIFT_ID == WorkPlace.WorkInfoSDO.WorkingShiftId.Value).ToList();
                    }
                    else
                    {
                        this.ListAccountBookRepay = this.ListAccountBookRepay.Where(o => !o.WORKING_SHIFT_ID.HasValue).ToList();
                    }
                }

                InitComboAccountBookRepay(this.ListAccountBookRepay);
                SetDefaultAccountBookRepay();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task LoadDataToComboPayForm()
        {
            try
            {
                this.payFormList = new List<PayFormADO>();
                List<HIS_PAY_FORM> lData = null;
                if (BackendDataWorker.IsExistsKey<HIS_PAY_FORM>())
                {
                    lData = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PAY_FORM>().Where(o => o.IS_ACTIVE == 1).ToList();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();

                    lData = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.HIS_PAY_FORM>>("api/HisPayForm/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                    if (lData != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_PAY_FORM), lData, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                if (BackendDataWorker.IsExistsKey<HIS_BANK>())
                {
                    hisBankList = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BANK>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    hisBankList = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.HIS_BANK>>("api/HisBank/Get", ApiConsumers.MosConsumer, filter, paramCommon);
                    if (hisBankList != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_BANK), hisBankList, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                if (hisBankList != null && hisBankList.Count > 0)
                {
                    hisBankList = hisBankList.Where(o => o.IS_CARD_PAYMENT_ACCEPTED == (short)1 && o.IS_ACTIVE == (short)1).ToList();
                }

                if (lData != null && lData.Count > 0)
                {
                    foreach (var item in lData)
                    {
                        PayFormADO payForm = new PayFormADO();
                        payForm.ID = item.ID;
                        payForm.PayFormId = item.ID.ToString();
                        payForm.PAY_FORM_CODE = item.PAY_FORM_CODE;
                        payForm.PAY_FORM_NAME = item.PAY_FORM_NAME;
                        payForm.BANK_ID = null;
                        this.payFormList.Add(payForm);
                    }
                }

                if (hisBankList != null && hisBankList.Count > 0
                    && lData != null && lData.Count > 0
                    && lData.Exists(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__QUET_THE))
                {
                    var payForm__QuetThe = this.payFormList.FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__QUET_THE);
                    this.payFormList.RemoveAll(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__QUET_THE);

                    foreach (var item in hisBankList)
                    {
                        PayFormADO payForm = new PayFormADO();
                        payForm.PayFormId = String.Format("{0}{1}", IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__QUET_THE, item.ID);
                        payForm.ID = IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__QUET_THE;
                        payForm.PAY_FORM_CODE = payForm__QuetThe.PAY_FORM_CODE + item.BANK_CODE;
                        payForm.PAY_FORM_NAME = payForm__QuetThe.PAY_FORM_NAME + " " + item.BANK_NAME;
                        payForm.BANK_ID = item.ID;
                        this.payFormList.Add(payForm);
                    }
                }

                cboPayForm.Properties.DataSource = this.payFormList;
                cboPayForm.Properties.DisplayMember = "PAY_FORM_NAME";
                cboPayForm.Properties.ValueMember = "PayFormId";
                cboPayForm.Properties.ForceInitialize();
                cboPayForm.Properties.Columns.Clear();
                cboPayForm.Properties.Columns.Add(new LookUpColumnInfo("PAY_FORM_CODE", "", 50));
                cboPayForm.Properties.Columns.Add(new LookUpColumnInfo("PAY_FORM_NAME", "", 250));
                cboPayForm.Properties.ShowHeader = false;
                cboPayForm.Properties.ImmediatePopup = true;
                cboPayForm.Properties.DropDownRows = 10;
                cboPayForm.Properties.PopupWidth = 300;

                var PayFormMinByCode = this.payFormList.OrderBy(o => o.PAY_FORM_CODE);
                var payFormDefault = PayFormMinByCode.FirstOrDefault();
                if (payFormDefault != null)
                {
                    cboPayForm.EditValue = payFormDefault.PayFormId;
                    //txtPayFormCode.Text = payFormDefault.PAY_FORM_CODE;
                    CheckPayFormTienMatChuyenKhoan(payFormDefault);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task LoadDataToComboFund()
        {
            try
            {
                List<HIS_FUND> lData = null;
                if (BackendDataWorker.IsExistsKey<HIS_FUND>())
                {
                    lData = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_FUND>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    lData = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.HIS_FUND>>("api/HisFund/Get", ApiConsumers.MosConsumer, filter, paramCommon);
                    if (lData != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_FUND), lData, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                repositoryItemCboFund.DataSource = lData;
                repositoryItemCboFund.DisplayMember = "FUND_NAME";
                repositoryItemCboFund.ValueMember = "ID";
                repositoryItemCboFund.ForceInitialize();
                repositoryItemCboFund.Columns.Clear();
                repositoryItemCboFund.Columns.Add(new LookUpColumnInfo("FUND_CODE", "", 100));
                repositoryItemCboFund.Columns.Add(new LookUpColumnInfo("FUND_NAME", "", 250));
                repositoryItemCboFund.ShowHeader = false;
                repositoryItemCboFund.ImmediatePopup = true;
                repositoryItemCboFund.DropDownRows = 10;
                repositoryItemCboFund.PopupWidth = 350;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToTreeSereServ(bool hasIsNoExecute)
        {
            try
            {
                ListSereServ = new List<V_HIS_SERE_SERV_5>();
                currentSereServs = new List<V_HIS_SERE_SERV_5>();
                dicSereServBill = new Dictionary<long, List<HIS_SERE_SERV_BILL>>();
                if (this.treatmentId.HasValue)
                {
                    HisSereServBillFilter ssBillFilter = new HisSereServBillFilter();
                    ssBillFilter.TDL_TREATMENT_ID = this.treatmentId.Value;
                    var listSSBill = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV_BILL>>("api/HisSereServBill/Get", ApiConsumers.MosConsumer, ssBillFilter, null);
                    if (listSSBill != null && listSSBill.Count > 0)
                    {
                        foreach (var item in listSSBill)
                        {
                            if (item.IS_CANCEL == (short)1)
                                continue;
                            if (!dicSereServBill.ContainsKey(item.SERE_SERV_ID))
                                dicSereServBill[item.SERE_SERV_ID] = new List<HIS_SERE_SERV_BILL>();
                            dicSereServBill[item.SERE_SERV_ID].Add(item);
                        }
                    }

                    if (!hasIsNoExecute && ListSereServTranfer != null && ListSereServTranfer.Count > 0)
                    {
                        currentSereServs = ListSereServTranfer;
                        foreach (var item in ListSereServTranfer)
                        {
                            if (dicSereServBill.ContainsKey(item.ID))
                                continue;
                            if (item.IS_NO_PAY == 1 || item.IS_NO_EXECUTE == 1)
                                continue;
                            ListSereServ.Add(item);
                        }
                    }
                    else
                    {
                        HisSereServView5Filter ssFilter = new HisSereServView5Filter();
                        ssFilter.TDL_TREATMENT_ID = this.treatmentId;
                        var hisSereServs = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV_5>>("api/HisSereServ/GetView5", ApiConsumers.MosConsumer, ssFilter, null);
                        if (hisSereServs != null && hisSereServs.Count > 0)
                        {
                            currentSereServs = hisSereServs;
                            foreach (var item in hisSereServs)
                            {
                                if (dicSereServBill.ContainsKey(item.ID))
                                    continue;
                                if (hasIsNoExecute && item.IS_NO_EXECUTE == 1 && item.TDL_SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK)
                                {
                                    this.ListSereServNoExecute.Add(item);
                                    continue;
                                }

                                if (item.IS_NO_PAY == 1 || item.IS_NO_EXECUTE == 1)
                                    continue;

                                ListSereServ.Add(item);
                            }
                        }
                    }
                }

                if (this.IsDirectlyBilling.HasValue && this.IsDirectlyBilling.Value == true
                    && ListSereServ != null && ListSereServ.Count > 0)
                {
                    ListSereServ = ListSereServ.Where(o => o.PATIENT_TYPE_ID != HisConfigCFG.PatientTypeId__BHYT && o.VIR_TOTAL_HEIN_PRICE == 0 && o.VIR_TOTAL_PRICE != 0).ToList();
                }

                // bỏ những dịch vụ đã chốt nợ
                if (this.treatmentId.HasValue && ListSereServ != null && ListSereServ.Count > 0)
                {
                    MOS.Filter.HisSereServDebtFilter sereServDebtFilter = new HisSereServDebtFilter();
                    sereServDebtFilter.TDL_TREATMENT_ID = this.treatmentId.Value;
                    var sereServDebtList = new BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV_DEBT>>("api/HisSereServDebt/Get", ApiConsumer.ApiConsumers.MosConsumer, sereServDebtFilter, null);
                    if (sereServDebtList != null && sereServDebtList.Count > 0)
                    {
                        sereServDebtList = sereServDebtList.Where(o => o.IS_CANCEL != 1).ToList();

                        this.ListSereServ = sereServDebtList != null && sereServDebtList.Count > 0
                            ? this.ListSereServ.Where(o => !sereServDebtList.Select(p => p.SERE_SERV_ID).Contains(o.ID)).ToList()
                            : this.ListSereServ;
                    }
                }
                if (!chkShowServiceNotPay.Checked)
                {
                    this.ListSereServ = ListSereServ.Where(o => o.VIR_TOTAL_PATIENT_PRICE > 0).ToList();
                }

                ssTreeProcessor.Reload(ucSereServTree, ListSereServ);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void EnableCheckNotTakenPress()
        {
            try
            {
                checkNotTakePres.Enabled = (this.ListSereServ != null && this.ListSereServ.Any(a => a.TDL_SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task FillDataToGirdTransaction()
        {
            try
            {
                if (this.treatmentId > 0)
                {
                    CommonParam param = new CommonParam();
                    HisTransactionViewFilter tranFilter = new HisTransactionViewFilter();
                    tranFilter.TREATMENT_ID = this.treatmentId;
                    tranFilter.ORDER_DIRECTION = "DESC";
                    tranFilter.ORDER_FIELD = "MODIFY_TIME";
                    tranFilter.TRANSACTION_TYPE_IDs = new List<long>() { IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TU };
                    this.listTransaction = await new Inventec.Common.Adapter.BackendAdapter(param).GetAsync<List<V_HIS_TRANSACTION>>("api/HisTransaction/GetView", ApiConsumers.MosConsumer, tranFilter, param);

                    if (HisConfigCFG.ShowServerTimeByDefault == "1")
                    {
                        dtTransactionTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(param.Now) ?? DateTime.MinValue;
                    }
                    gridControlTransaction.BeginUpdate();
                    gridControlTransaction.DataSource = this.listTransaction;
                    gridControlTransaction.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                gridControlTransaction.EndUpdate();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CalcuTotalPrice()
        {
            try
            {
                totalPatientPrice = 0;
                totalPatientPriceFund = 0;
                var listData = ssTreeProcessor.GetListCheck(this.ucSereServTree);
                if (listData != null)
                {
                    totalPatientPrice = listData.Sum(o => o.VIR_TOTAL_PATIENT_PRICE ?? 0);
                    totalPatientPriceFund = listData.Where(o => o.IS_FUND_ACCEPTED.HasValue && o.IS_FUND_ACCEPTED == 1).Sum(s => s.VIR_TOTAL_PATIENT_PRICE ?? 0);
                }

                Inventec.Common.Logging.LogSystem.Debug("totalPatientPrice" + totalPatientPrice);
                txtTotalAmount.Value = totalPatientPrice;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ResetControlValue()
        {
            try
            {
                ListBillFund = new List<VHisBillFundADO>();
                resultTranBill = null;
                totalPatientPrice = 0;
                totalDiscount = 0;
                totalPatientPriceFund = 0;
                totalFund = 0;
                totalHienDu = 0;
                dxValidationProvider1.RemoveControlError(txtDiscountRatio);
                dxValidationProvider1.RemoveControlError(dtTransactionTime);
                dxValidationProvider1.RemoveControlError(cboPayForm);
                totalCanThu = 0;
                spinTongTuDen.Value = 0;
                //SetDefaultAccountBook();//TODO
                //SetDefaultPayFormForUser();//TODO
                txtDescription.Text = "";
                txtDiscount.EditValue = null;
                txtDiscountRatio.EditValue = null;
                spinAmountBNDua.EditValue = null;
                lblAmountTraBN.Text = "";
                txtReason.Text = "";
                //
                if (!IsPin)
                {
                    txtBuyerName.Text = "";
                    txtBuyerTaxCode.Text = "";
                    txtBuyerAccountNumber.Text = "";
                    txtBuyerOrganization.Text = "";
                    txtBuyerAddress.Text = "";
                    chkOther.Checked = false;
                    cboBuyerOrganization.EditValue = null;
                    cboBuyerOrganization.Properties.Buttons[1].Visible = false;
                }
                //
                txtTotalAmount.Value = 0;
                btnNew.Enabled = true;
                btnSave.Enabled = true;
                lciBtnSave.Enabled = true;
                btnSavePrint.Enabled = true;
                btnSaveAndSign.Enabled = true;
                btnAddSignEmr.Enabled = true;
                if (TransactionBillConfig.InvoiceTypeCreate == invoiceTypeCreate__CreateInvoiceVnpt)
                {
                    ddBtnPrint.Enabled = true;
                }
                else
                {
                    ddBtnPrint.Enabled = false;
                }
                panelMenuPrintBill.Enabled = false;
                spinTransferAmount.EditValue = null;
                dtTransactionTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.Now() ?? 0);
                lciCoKetChuyen.Enabled = true;
                if (Config.HisConfigCFG.IsketChuyenCFG != null && Config.HisConfigCFG.IsketChuyenCFG.Equals("1")
                    || (!Config.HisConfigCFG.IsketChuyenCFG.Equals("2") && !Config.HisConfigCFG.IsketChuyenCFG.Equals("3") && !Config.HisConfigCFG.IsketChuyenCFG.Equals("4")))
                {
                    chkCoKetChuyen.CheckState = CheckState.Unchecked;
                }
                else if (Config.HisConfigCFG.IsketChuyenCFG != null && Config.HisConfigCFG.IsketChuyenCFG.Equals("2"))
                {
                    chkCoKetChuyen.CheckState = CheckState.Checked;
                }
                else if (Config.HisConfigCFG.IsketChuyenCFG != null && Config.HisConfigCFG.IsketChuyenCFG.Equals("3") && this.currentTreatment.IS_PAUSE == 1)
                {
                    chkCoKetChuyen.CheckState = CheckState.Checked;
                }
                else if (Config.HisConfigCFG.IsketChuyenCFG != null && Config.HisConfigCFG.IsketChuyenCFG.Equals("4"))
                {
                    lciCoKetChuyen.Enabled = false;
                }
                else
                {
                    chkCoKetChuyen.CheckState = CheckState.Unchecked;
                }

                if (Config.HisConfigCFG.IsEditTransactionBillCFG != null && Config.HisConfigCFG.IsEditTransactionBillCFG.Equals("1"))
                {
                    layoutTransactionTime.Enabled = true;
                }
                else
                {
                    layoutTransactionTime.Enabled = false;
                }

                Inventec.Common.Logging.LogSystem.Debug("lciCoKetChuyen.Enabled: " + lciCoKetChuyen.Enabled);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ResetFillPatientDefault()
        {
            try
            {
                txtPatient.Text = "";
                txtPatientName.Text = "";
                txtDOB.Text = "";
                txtGender.Text = "";
                txtAddress.Text = "";
                txtPatienType.Text = "";
                txtHeinCard.Text = "";
                txtHeinFrom.Text = "";
                txtHeinTo.Text = "";
                txtMediOrg.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultAccountBook()
        {
            try
            {
                cboAccountBook.EditValue = null;
                V_HIS_ACCOUNT_BOOK accountBook = null;
                //chọn mặc định sổ nếu có sổ tương ứng
                if (GlobalVariables.DefaultAccountBookTransactionBill != null && GlobalVariables.DefaultAccountBookTransactionBill.Count > 0)
                {
                    var lstBook = GlobalVariables.DefaultAccountBookTransactionBill.Where(o => ListAccountBook.Select(s => s.ID).Contains(o.ID)).ToList();
                    if (lstBook != null && lstBook.Count > 0)
                    {
                        accountBook = lstBook.First();
                    }
                }

                if (HisConfigCFG.IsAutoSelectAccountBookIfHasOne && accountBook == null && ListAccountBook.Count == 1)
                {
                    accountBook = ListAccountBook.First();
                }

                if (accountBook != null)
                {
                    cboAccountBook.EditValue = accountBook.ID;
                    //SetDataToDicNumOrderInAccountBook(accountBook);
                }
                else
                {
                    spinTongTuDen.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultAccountBookRepay()
        {
            try
            {
                if (!chkAutoRepay.Checked)
                {
                    cboAccountBookRepay.EditValue = null;
                    spinNumOrderRepay.EditValue = null;
                    return;
                }

                cboAccountBookRepay.EditValue = null;
                V_HIS_ACCOUNT_BOOK accountBook = null;
                //chọn mặc định sổ nếu có sổ tương ứng
                if (GlobalVariables.DefaultAccountBookTransactionBill__Repay != null && GlobalVariables.DefaultAccountBookTransactionBill__Repay.Count > 0)
                {
                    var lstBook = this.ListAccountBookRepay.Where(o => GlobalVariables.DefaultAccountBookTransactionBill__Repay.Select(s => s.ID).Contains(o.ID)).ToList();
                    if (lstBook != null && lstBook.Count > 0)
                    {
                        accountBook = lstBook.OrderByDescending(o => o.ID).First();
                    }
                }
                if (accountBook != null)
                {
                    cboAccountBookRepay.EditValue = accountBook.ID;
                    //SetDataToDicNumOrderInAccountBook(accountBook);
                }
                else
                {
                    spinNumOrderRepay.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void SetDefaultAccountBookDeposit()
        {
            try
            {
                if (!SetStateEnableControl())
                    return;
                cboDepositBook.EditValue = null;
                V_HIS_ACCOUNT_BOOK accountBook = null;
                //chọn mặc định sổ nếu có sổ tương ứng
                if (GlobalVariables.DefaultAccountBookTransactionBill__Deposit != null && GlobalVariables.DefaultAccountBookTransactionBill__Deposit.Count > 0)
                {
                    var lstBook = this.ListAccountBookDeposit.Where(o => GlobalVariables.DefaultAccountBookTransactionBill__Deposit.Select(s => s.ID).Contains(o.ID)).ToList();
                    if (lstBook != null && lstBook.Count > 0)
                    {
                        accountBook = lstBook.OrderByDescending(o => o.ID).First();
                    }
                }
                if (accountBook != null)
                {
                    cboDepositBook.EditValue = accountBook.ID;
                    //SetDataToDicNumOrderInAccountBook(accountBook);
                }
                else
                {
                    spnNumOrder.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ProcessFundForHCM()
        {
            try
            {
                decimal totalHcmPrice = 0;
                this.LciBillFund.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                this.ListBillFund = new List<VHisBillFundADO>();
                this.ProcessFundTreatment(totalHcmPrice);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessFundTreatment(decimal totalHcmPrice)
        {
            try
            {
                HIS_FUND fund = (this.currentTreatment != null && (this.currentTreatment.FUND_ID ?? 0) > 0) ? BackendDataWorker.Get<HIS_FUND>().FirstOrDefault(o => o.ID == this.currentTreatment.FUND_ID) : null;
                if (fund != null)
                {
                    VHisBillFundADO ado = new VHisBillFundADO();
                    ado.IsNotEdit = true;
                    ado.FUND_CODE = fund.FUND_CODE;
                    ado.FUND_NAME = fund.FUND_NAME;
                    ado.FUND_ID = fund.ID;
                    ado.FUND_BUDGET = this.currentTreatment.FUND_BUDGET;

                    //nếu có quỹ hn thì tổng tiền sẽ trừ đi tiền quỹ
                    decimal fundAmount = 0;
                    if (totalHcmPrice > 0)
                    {
                        fundAmount = totalPatientPriceFund - totalHcmPrice;
                    }
                    else
                        fundAmount = totalPatientPriceFund;

                    if (this.currentTreatment.FUND_BUDGET.HasValue)
                    {
                        //nếu quỹ đã thanh toán thì  tính số tiền quỹ đã thanh toán
                        //nếu nhỏ hơn hạn mức thì lấy số tiền hạn mức còn lại so với số tiền phải thanh toán.
                        decimal FUND_BUDGET = this.currentTreatment.FUND_BUDGET.Value;
                        if (ListBillFundPay != null && ListBillFundPay.Count > 0)
                        {
                            FUND_BUDGET = this.currentTreatment.FUND_BUDGET.Value - ListBillFundPay.Sum(s => s.AMOUNT);
                        }

                        //nếu hạn mức còn lại nhỏ hơn hoặc bằng 0 thì không hiển thị quỹ
                        if (FUND_BUDGET < 0) return;

                        //+ Nếu tổng tiền < "hạn mức" --> lấy theo "số tiền cần thu" 
                        //+ Nếu tổng tiền > "hạn mức" --> lấy theo hạn mức
                        if (fundAmount <= FUND_BUDGET)
                        {
                            ado.AMOUNT = fundAmount;
                        }
                        else
                        {
                            ado.AMOUNT = FUND_BUDGET;
                        }
                        this.ListBillFund.Add(ado);
                    }
                }

                this.bindingSource1.DataSource = this.ListBillFund;
                this.gridControlBillFund.DataSource = this.bindingSource1;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task CalcuHienDu()
        {
            try
            {
                if (this.currentTreatment == null)
                    return;

                totalHienDu = (this.currentTreatment.TOTAL_DEPOSIT_AMOUNT ?? 0) - ((this.currentTreatment.TOTAL_REPAY_AMOUNT ?? 0) + (this.currentTreatment.TOTAL_BILL_TRANSFER_AMOUNT ?? 0));
                lblHienDu.Text = Inventec.Common.Number.Convert.NumberToString(totalHienDu, ConfigApplications.NumberSeperator);
                totalCanThuThem = (this.currentTreatment.TOTAL_PATIENT_PRICE ?? 0) - (((this.currentTreatment.TOTAL_DEPOSIT_AMOUNT ?? 0) + (this.currentTreatment.TOTAL_BILL_AMOUNT ?? 0) - (this.currentTreatment.TOTAL_BILL_TRANSFER_AMOUNT ?? 0) - (this.currentTreatment.TOTAL_BILL_FUND ?? 0) - (this.currentTreatment.TOTAL_REPAY_AMOUNT ?? 0)) - (this.currentTreatment.TOTAL_BILL_EXEMPTION ?? 0)) - (this.currentTreatment.TOTAL_BILL_FUND ?? 0) - (this.currentTreatment.TOTAL_BILL_EXEMPTION ?? 0);

                if (resultTranBill != null)
                {
                    totalCanThuThem = totalCanThuThem - (resultTranBill.AMOUNT - (resultTranBill.KC_AMOUNT ?? 0));
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CalcuCanThu()
        {
            try
            {
                var listFund = bindingSource1.DataSource as List<VHisBillFundADO>;
                decimal totalFund = 0;
                decimal canthuAmount = 0;
                if (listFund != null && listFund.Count > 0)
                {
                    totalFund = listFund.Sum(o => o.AMOUNT);
                }

                //var discount = txtDiscount.Value;
                // nếu checkbox "có kết chuyển" bỏ check thì không tính số tiền hiện dư vào 
                decimal SoTienChuyenKhoan = 0;
                if (spinTransferAmount.EditValue != null)
                {
                    SoTienChuyenKhoan = spinTransferAmount.Value;
                }

                lblRepayAmount.Text = "0";
                RepayAmount = 0;
                if (chkCoKetChuyen.CheckState == CheckState.Unchecked)
                {
                    canthuAmount = (totalPatientPrice - totalFund - this.totalDiscount) - SoTienChuyenKhoan;
                    lblReceiveAmount.Text = Inventec.Common.Number.Convert.NumberToString(((totalPatientPrice - totalFund - this.totalDiscount - SoTienChuyenKhoan)), ConfigApplications.NumberSeperator);
                    RepayAmount = totalHienDu;
                    lblRepayAmount.Text = Inventec.Common.Number.Convert.NumberToString(totalHienDu, ConfigApplications.NumberSeperator);
                }
                else
                {
                    if (totalHienDu >= (totalPatientPrice - totalFund - this.totalDiscount))
                    {
                        lblReceiveAmount.Text = Inventec.Common.Number.Convert.NumberToString(0, ConfigApplications.NumberSeperator);
                    }
                    else
                    {
                        canthuAmount = (totalPatientPrice - totalFund - this.totalDiscount) - totalHienDu - SoTienChuyenKhoan;
                        lblReceiveAmount.Text = Inventec.Common.Number.Convert.NumberToString(((totalPatientPrice - totalFund - this.totalDiscount) - totalHienDu - SoTienChuyenKhoan), ConfigApplications.NumberSeperator);
                    }

                    if (totalHienDu > totalPatientPrice)
                    {
                        RepayAmount = totalHienDu - totalPatientPrice;
                        lblRepayAmount.Text = Inventec.Common.Number.Convert.NumberToString(totalHienDu - totalPatientPrice, ConfigApplications.NumberSeperator);
                    }
                }

                lblAmountTraBN.Text = "";
                if (spinAmountBNDua.EditValue != null)
                {
                    lblAmountTraBN.Text = Inventec.Common.Number.Convert.NumberToStringRoundAuto(spinAmountBNDua.Value - canthuAmount, ConfigApplications.NumberSeperator);
                }
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
                ValidControlAccountBook();
                ValidControlPayForm();
                ValidControlTransactionTime();
                ValidControlSpinVAT();
                ValidControlDescription();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlDescription()
        {
            try
            {
                Inventec.Desktop.Common.Controls.ValidationRule.ControlMaxLengthValidationRule validate = new Inventec.Desktop.Common.Controls.ValidationRule.ControlMaxLengthValidationRule();
                validate.editor = this.txtDescription;
                validate.maxLength = 2000;
                validate.IsRequired = false;
                validate.ErrorText = string.Format("Nhập quá ký tự cho phép {0}", 2000);
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(this.txtDescription, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlAccountBook()
        {
            try
            {
                AccountBookValidationRule accBookRule = new AccountBookValidationRule();
                accBookRule.cboAccountBook = cboAccountBook;
                dxValidationProvider1.SetValidationRule(cboAccountBook, accBookRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlTransactionTime()
        {
            try
            {
                TransactionTimeValidationRule transactionTimeRule = new TransactionTimeValidationRule();
                transactionTimeRule.dtTransactionTime = dtTransactionTime;
                dxValidationProvider1.SetValidationRule(dtTransactionTime, transactionTimeRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlNumorderRepay(bool isRequired)
        {
            try
            {
                spinNumOrderRepayValidationRule numorderRule = new spinNumOrderRepayValidationRule();
                numorderRule.spinNumorder = spinNumOrderRepay;
                numorderRule.isRequired = isRequired;
                dxValidationProvider1.SetValidationRule(spinNumOrderRepay, numorderRule);
                if (isRequired)
                {
                    lciNumOrderRepay.AppearanceItemCaption.ForeColor = Color.Maroon;
                }
                else
                {
                    lciNumOrderRepay.AppearanceItemCaption.ForeColor = Color.Black;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlPayForm()
        {
            try
            {
                PayFormValidationRule payFormRule = new PayFormValidationRule();
                payFormRule.cboPayForm = cboPayForm;
                dxValidationProvider1.SetValidationRule(cboPayForm, payFormRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlSpinVAT()
        {
            try
            {
                SpinVATValidationRule payFormRule = new SpinVATValidationRule();
                payFormRule.spinVAT = txtDiscountRatio;
                dxValidationProvider1.SetValidationRule(txtDiscountRatio, payFormRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlPIN(bool IsRequiredField)
        {
            try
            {
                //txtPINValidationRule PINRule = new txtPINValidationRule();
                //PINRule.txtPinCode = txtPin;
                //PINRule.isRequiredPin = IsRequiredField;
                //dxValidationProvider1.SetValidationRule(txtPin, PINRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlTransferAmount(bool IsRequiredField)
        {
            try
            {
                SpinTranferAmountValidationRule PINRule = new SpinTranferAmountValidationRule();
                PINRule.spinTranferAmount = spinTransferAmount;
                PINRule.isRequiredPin = IsRequiredField;
                dxValidationProvider1.SetValidationRule(spinTransferAmount, PINRule);
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

        private void LoadKeyFrmLanguage()
        {
            try
            {
                //Button
                this.btnNew.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__BTN_NEW", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__BTN_SAVE", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnSaveAndSign.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__BTN_SAVE_INVOICE_SIGN", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.ddBtnPrint.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__BTN_PRINT", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__BTN_SEARCH", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                //Layout
                this.layoutAccountBook.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__LAYOUT_ACCOUNT_BOOK", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutHienDu.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__LAYOUT_HIEN_DU", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutDescription.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__LAYOUT_DESCRIPTION", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutDiscount.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__LAYOUT_DISCOUNT", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutDiscountRatio.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__LAYOUT_DISCOUNT_RATIO", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutDiscountRatio.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__LAYOUT_DISCOUNT_RATIO_TOOLTIP", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciTranferAmount.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__LAYOUT_TRANSFER_AMOUNT_TEXT", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciTranferAmount.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__LAYOUT_TRANSFER_AMOUNT_TOOLTIP", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                //this.layoutNumOrder.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__LAYOUT_NUM_ORDER", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutPayForm.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__LAYOUT_PAY_FORM", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutReason.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__LAYOUT_DISCOUNT_REASON", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutReceiveAmount.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__LAYOUT_TOTAL_RECEIVE", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutTongTuDen.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__LAYOUT_TONG_TU_DEN", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutTotalAmount.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__LAYOUT_TOTAL_AMOUNT", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                //GridControl Blood
                this.gridColumn_Fund_Amount.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__GRID_FUND__COLUMN_AMOUNT", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Fund_FundCode.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__GRID_FUND__COLUMN_FUND_CODE", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Fund_FundName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__GRID_FUND__COLUMN_FUND_NAME", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumn_Fund_Stt.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__GRID_CONTROL__COLUMN_STT", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                //Repository Button
                this.repositoryItemBtnDeleteFund.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__REPOSITORY__BTN_DELETE_BILL_FUND", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutTransactionTime.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__TRANSACTION_TIME", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutTransactionTime.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__TRANSACTION_TIME__TOOLTIP", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciRightRoute.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__LAYOUT_RIGHT_ROUTE", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciHeinRatio.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__LAYOUT_HEIN_RATIO", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                //InfoPatient
                this.layoutControlItem13.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__layoutControlItem13", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutControlItem15.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__layoutControlItem15", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutControlItem21.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__layoutControlItem21", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutControlItem20.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__layoutControlItem20", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutControlItem16.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__layoutControlItem16", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutControlItem14.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__layoutControlItem14", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutControlItem23.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__layoutControlItem23", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutControlItem25.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__layoutControlItem25", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutControlItem26.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__layoutControlItem26", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutControlItem22.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__layoutControlItem22", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__btnSearch", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.chkAutoRepay.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__chkAutoRepay", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.chkAutoRepay.ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__chkAutoRepay_ToolTip", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                //this.lciCoKetChuyen.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__CHK_CO_KET_CHUYEN", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                //minhnq
                this.layoutControlItem11.Text = Inventec.Common.Resource.Get.Value("frmTransactionBill.layoutControlItem11.Text", Base.ResourceLangManager.LanguageFrmTransactionBill, LanguageManager.GetCulture());
                this.lciServiceGroup.Text = Inventec.Common.Resource.Get.Value("frmTransactionBill.lciServiceGroup.Text", Base.ResourceLangManager.LanguageFrmTransactionBill, LanguageManager.GetCulture());
                this.radioSGAll.Properties.Caption = Inventec.Common.Resource.Get.Value("frmTransactionBill.radioSGAll.Properties.Caption", Base.ResourceLangManager.LanguageFrmTransactionBill, LanguageManager.GetCulture());
                this.radioSGMedicine.Properties.Caption = Inventec.Common.Resource.Get.Value("frmTransactionBill.radioSGMedicine.Properties.Caption", Base.ResourceLangManager.LanguageFrmTransactionBill, LanguageManager.GetCulture());
                this.radioSGCLS.Properties.Caption = Inventec.Common.Resource.Get.Value("frmTransactionBill.radioSGCLS.Properties.Caption", Base.ResourceLangManager.LanguageFrmTransactionBill, LanguageManager.GetCulture());
                this.radioSGExam.Properties.Caption = Inventec.Common.Resource.Get.Value("frmTransactionBill.radioSGExam.Properties.Caption", Base.ResourceLangManager.LanguageFrmTransactionBill, LanguageManager.GetCulture());
                this.layoutControlItem42.Text = Inventec.Common.Resource.Get.Value("frmTransactionBill.layoutControlItem42.Text", Base.ResourceLangManager.LanguageFrmTransactionBill, LanguageManager.GetCulture());
                this.LciBillFund.Text = Inventec.Common.Resource.Get.Value("frmTransactionBill.LciBillFund.Text", Base.ResourceLangManager.LanguageFrmTransactionBill, LanguageManager.GetCulture());
                this.LciBillFund.Text = Inventec.Common.Resource.Get.Value("frmTransactionBill.LciBillFund.Text", Base.ResourceLangManager.LanguageFrmTransactionBill, LanguageManager.GetCulture());
                this.lciAccountBookRepay.Text = Inventec.Common.Resource.Get.Value("frmTransactionBill.lciAccountBookRepay.Text", Base.ResourceLangManager.LanguageFrmTransactionBill, LanguageManager.GetCulture());
                this.lciNumOrderRepay.Text = Inventec.Common.Resource.Get.Value("frmTransactionBill.lciNumOrderRepay.Text", Base.ResourceLangManager.LanguageFrmTransactionBill, LanguageManager.GetCulture());
                this.groupControl1.Text = Inventec.Common.Resource.Get.Value("frmTransactionBill.groupControl1.Text", Base.ResourceLangManager.LanguageFrmTransactionBill, LanguageManager.GetCulture());
                this.layoutControlItem32.Text = Inventec.Common.Resource.Get.Value("frmTransactionBill.layoutControlItem32.Text", Base.ResourceLangManager.LanguageFrmTransactionBill, LanguageManager.GetCulture());
                this.layoutControlItem33.Text = Inventec.Common.Resource.Get.Value("frmTransactionBill.layoutControlItem33.Text", Base.ResourceLangManager.LanguageFrmTransactionBill, LanguageManager.GetCulture());
                this.layoutControlItem34.Text = Inventec.Common.Resource.Get.Value("frmTransactionBill.layoutControlItem34.Text", Base.ResourceLangManager.LanguageFrmTransactionBill, LanguageManager.GetCulture());
                this.layoutControlItem44.Text = Inventec.Common.Resource.Get.Value("frmTransactionBill.layoutControlItem44.Text", Base.ResourceLangManager.LanguageFrmTransactionBill, LanguageManager.GetCulture());
                this.layoutControlItem36.Text = Inventec.Common.Resource.Get.Value("frmTransactionBill.layoutControlItem36.Text", Base.ResourceLangManager.LanguageFrmTransactionBill, LanguageManager.GetCulture());
                this.chkOther.Properties.Caption = Inventec.Common.Resource.Get.Value("frmTransactionBill.chkOther.Properties.Caption", Base.ResourceLangManager.LanguageFrmTransactionBill, LanguageManager.GetCulture());
                this.gridColumn_Transaction_Lock.Caption = Inventec.Common.Resource.Get.Value("frmTransactionBill.gridColumn_Transaction_Lock.Caption", Base.ResourceLangManager.LanguageFrmTransactionBill, LanguageManager.GetCulture());
                this.gridColumn_Transaction_TransactionCode.Caption = Inventec.Common.Resource.Get.Value("frmTransactionBill.gridColumn_Transaction_TransactionCode.Caption", Base.ResourceLangManager.LanguageFrmTransactionBill, LanguageManager.GetCulture());
                this.gridColumn_Transaction_Amount.Caption = Inventec.Common.Resource.Get.Value("frmTransactionBill.gridColumn_Transaction_Amount.Caption", Base.ResourceLangManager.LanguageFrmTransactionBill, LanguageManager.GetCulture());
                this.gridColumn_Transaction_PayForm.Caption = Inventec.Common.Resource.Get.Value("frmTransactionBill.gridColumn_Transaction_PayForm.Caption", Base.ResourceLangManager.LanguageFrmTransactionBill, LanguageManager.GetCulture());
                this.gridColumn_Transaction_CashierUseName.Caption = Inventec.Common.Resource.Get.Value("frmTransactionBill.gridColumn_Transaction_CashierUseName.Caption", Base.ResourceLangManager.LanguageFrmTransactionBill, LanguageManager.GetCulture());
                this.gridColumn_Transaction_CashierRoom.Caption = Inventec.Common.Resource.Get.Value("frmTransactionBill.gridColumn_Transaction_CashierRoom.Caption", Base.ResourceLangManager.LanguageFrmTransactionBill, LanguageManager.GetCulture());
                this.gridColumn_Transaction_NumOrder.Caption = Inventec.Common.Resource.Get.Value("frmTransactionBill.gridColumn_Transaction_NumOrder.Caption", Base.ResourceLangManager.LanguageFrmTransactionBill, LanguageManager.GetCulture());
                this.gridColumn_Transaction_AccountBookCode.Caption = Inventec.Common.Resource.Get.Value("frmTransactionBill.gridColumn_Transaction_AccountBookCode.Caption", Base.ResourceLangManager.LanguageFrmTransactionBill, LanguageManager.GetCulture());
                this.gridColumn_Transaction_AccountBookName.Caption = Inventec.Common.Resource.Get.Value("frmTransactionBill.gridColumn_Transaction_AccountBookName.Caption", Base.ResourceLangManager.LanguageFrmTransactionBill, LanguageManager.GetCulture());
                this.gridColumn_Transaction_CreateTime.Caption = Inventec.Common.Resource.Get.Value("frmTransactionBill.gridColumn_Transaction_CreateTime.Caption", Base.ResourceLangManager.LanguageFrmTransactionBill, LanguageManager.GetCulture());
                this.gridColumn_Transaction_Creator.Caption = Inventec.Common.Resource.Get.Value("frmTransactionBill.gridColumn_Transaction_Creator.Caption", Base.ResourceLangManager.LanguageFrmTransactionBill, LanguageManager.GetCulture());
                this.gridColumn_Transaction_ModifyTime.Caption = Inventec.Common.Resource.Get.Value("frmTransactionBill.gridColumn_Transaction_ModifyTime.Caption", Base.ResourceLangManager.LanguageFrmTransactionBill, LanguageManager.GetCulture());
                this.gridColumn_Transaction_Modifier.Caption = Inventec.Common.Resource.Get.Value("frmTransactionBill.gridColumn_Transaction_Modifier.Caption", Base.ResourceLangManager.LanguageFrmTransactionBill, LanguageManager.GetCulture());
                this.layoutControlItem29.Text = Inventec.Common.Resource.Get.Value("frmTransactionBill.layoutControlItem29.Text", Base.ResourceLangManager.LanguageFrmTransactionBill, LanguageManager.GetCulture());
                this.layoutControlItem31.Text = Inventec.Common.Resource.Get.Value("frmTransactionBill.layoutControlItem31.Text", Base.ResourceLangManager.LanguageFrmTransactionBill, LanguageManager.GetCulture());
                this.lciRepayAmount.Text = Inventec.Common.Resource.Get.Value("frmTransactionBill.lciRepayAmount.Text", Base.ResourceLangManager.LanguageFrmTransactionBill, LanguageManager.GetCulture());
                this.chkAutoClose.Properties.Caption = Inventec.Common.Resource.Get.Value("frmTransactionBill.chkAutoClose.Properties.Caption", Base.ResourceLangManager.LanguageFrmTransactionBill, LanguageManager.GetCulture());
                this.chkAutoClose.ToolTip = Inventec.Common.Resource.Get.Value("frmTransactionBill.chkAutoClose.ToolTip", Base.ResourceLangManager.LanguageFrmTransactionBill, LanguageManager.GetCulture());
                this.chkHideHddt.Properties.Caption = Inventec.Common.Resource.Get.Value("frmTransactionBill.chkHideHddt.Properties.Caption", Base.ResourceLangManager.LanguageFrmTransactionBill, LanguageManager.GetCulture());
                this.chkPrintHddt.Properties.Caption = Inventec.Common.Resource.Get.Value("frmTransactionBill.chkPrintHddt.Properties.Caption", Base.ResourceLangManager.LanguageFrmTransactionBill, LanguageManager.GetCulture());
                this.chkPrintPrescription.Properties.Caption = Inventec.Common.Resource.Get.Value("frmTransactionBill.chkPrintPrescription.Properties.Caption", Base.ResourceLangManager.LanguageFrmTransactionBill, LanguageManager.GetCulture());
                this.chkPrintPrescription.ToolTip = Inventec.Common.Resource.Get.Value("frmTransactionBill.chkPrintPrescription.ToolTip", Base.ResourceLangManager.LanguageFrmTransactionBill, LanguageManager.GetCulture());
                this.chkPrintBKBHNT.Properties.Caption = Inventec.Common.Resource.Get.Value("frmTransactionBill.chkPrintBKBHNT.Properties.Caption", Base.ResourceLangManager.LanguageFrmTransactionBill, LanguageManager.GetCulture());
                this.chkPrintBKBHNT.ToolTip = Inventec.Common.Resource.Get.Value("frmTransactionBill.chkPrintBKBHNT.ToolTip", Base.ResourceLangManager.LanguageFrmTransactionBill, LanguageManager.GetCulture());
                this.layoutControlItem39.Text = Inventec.Common.Resource.Get.Value("frmTransactionBill.layoutControlItem39.Text", Base.ResourceLangManager.LanguageFrmTransactionBill, LanguageManager.GetCulture());
                this.chkCoKetChuyen.Properties.Caption = Inventec.Common.Resource.Get.Value("frmTransactionBill.chkCoKetChuyen.Properties.Caption", Base.ResourceLangManager.LanguageFrmTransactionBill, LanguageManager.GetCulture());
                this.chkConnectPOS.Properties.Caption = Inventec.Common.Resource.Get.Value("frmTransactionBill.chkConnectPOS.Properties.Caption", Base.ResourceLangManager.LanguageFrmTransactionBill, LanguageManager.GetCulture());
                this.chkConnectPOS.ToolTip = Inventec.Common.Resource.Get.Value("frmTransactionBill.chkConnectPOS.ToolTip", Base.ResourceLangManager.LanguageFrmTransactionBill, LanguageManager.GetCulture());
                this.btnSavePrint.Text = Inventec.Common.Resource.Get.Value("frmTransactionBill.btnSavePrint.Text", Base.ResourceLangManager.LanguageFrmTransactionBill, LanguageManager.GetCulture());



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
                isInit = true;
                treatmentId = null;
                currentTreatment = null;
                //resultPatientType = null;
                ResetFillPatientDefault();
                LoadSearch();
                FillInfoPatient(currentTreatment);
                LoadAccountBookToLocal();
                LoadAccountBookRepayToLocal();
                LoadDataToComboAccountBook();
                FillDataToGirdTransaction();
                ResetControlValue();
                SetDefaultAccountBook();//TODO
                SetDefaultAccountBookRepay();
                SetDefaultAccountBookDeposit();
                SetDefaultPayFormForUser();//TODO
                LoadDataToTreeSereServ(false);
                //LoadDataForBordereau();
                EnableCheckNotTakenPress();
                FillDataToButtonPrint();
                InitMenuToButtonPrint();
                CalcuTotalPrice();
                ProcessBillFund();
                ProcessFundForHCM();
                CalcuHienDu();
                CalcuCanThu();
                txtTotalAmount.Value = this.totalPatientPrice;
                cboAccountBook.Focus();
                isInit = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool CheckPayformCard(PayFormADO payForm)
        {
            bool result = false;
            try
            {
                if (payForm != null && payForm.ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__THE)
                    result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }

        private void UpdatePINControl(bool isCardPayform)
        {
            try
            {
                //dxValidationProvider1.RemoveControlError(txtPin);
                //ValidControlPIN(isCardPayform);
                //if (isCardPayform)
                //{
                //    lciPin.AppearanceItemCaption.ForeColor = Color.Maroon;
                //    lciPin.Enabled = true;
                //}
                //else
                //{
                //    lciPin.AppearanceItemCaption.ForeColor = Color.Black;
                //    lciPin.Enabled = false;
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadSearch()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisTreatmentFeeViewFilter filter = new HisTreatmentFeeViewFilter();
                if (!String.IsNullOrEmpty(txtFindTreatmentCode.Text))
                {
                    string code = txtFindTreatmentCode.Text.Trim();
                    if (code.Length < 12)
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        txtFindTreatmentCode.Text = code;
                    }

                    filter.TREATMENT_CODE__EXACT = code;

                    var listTreatment = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_FEE>>("api/HisTreatment/GetFeeView", ApiConsumers.MosConsumer, filter, param);
                    if (listTreatment != null && listTreatment.Count > 0)
                    {
                        currentTreatment = listTreatment.FirstOrDefault();
                        treatmentId = currentTreatment.ID;
                    }
                    else
                    {
                        param.Messages.Add(Base.ResourceMessageLang.KhongTimThayMaDieuTri);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtFindTreatmentCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                    btnSearch.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void spinTongTuDen_ValueChanged(object sender, EventArgs e)
        {

        }

        private void spinTongTuDen_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboAccountBook.EditValue != null)
                    {
                        var accountBook = this.ListAccountBook.FirstOrDefault(o => o.ID == Convert.ToInt64(cboAccountBook.EditValue.ToString()));
                        UpdateDictionaryNumOrderAccountBook(accountBook, spinTongTuDen.Value);
                    }

                    dtTransactionTime.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinTongTuDen_Spin(object sender, SpinEventArgs e)
        {
            try
            {
                if (cboAccountBook.EditValue != null)
                {
                    var accountBook = this.ListAccountBook.FirstOrDefault(o => o.ID == Convert.ToInt64(cboAccountBook.EditValue.ToString()));
                    UpdateDictionaryNumOrderAccountBook(accountBook, spinTongTuDen.Value);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmTransactionBill_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                SavePin();
                string repay = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.TransactionBill.Repay");
                if (repay == "1")
                {
                    if (btnSave.Enabled == false || lciBtnSave.Enabled == false)
                    {
                        LoadSearch();
                        resultTranBill = null;
                        CalcuHienDu();
                        Inventec.Common.Logging.LogSystem.Debug("totalCanThuThem: " + totalCanThuThem);
                        if (totalCanThuThem < 0)//totalHienDu >= (totalPatientPrice - (totalDiscount + totalFund))
                        {
                            if (MessageBox.Show(String.Format(ResourceMessageLang.BanCoMuonHoanUngKhong), ResourceMessageLang.ThongBao, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TransactionRepay").FirstOrDefault();
                                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.TransactionRepay'");
                                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                                {
                                    moduleData.RoomId = this.currentModule.RoomId;
                                    moduleData.RoomTypeId = this.currentModule.RoomTypeId;
                                    List<object> listArgs = new List<object>();
                                    TransactionRepayADO ado = new TransactionRepayADO(this.currentTreatment.ID, this.cashierRoom.ID);
                                    listArgs.Add(ado);
                                    var extenceInstance = PluginInstance.GetPluginInstance(moduleData, listArgs);
                                    if (extenceInstance == null)
                                    {
                                        throw new ArgumentNullException("moduleData is null");
                                    }
                                    ((Form)extenceInstance).ShowDialog();
                                }
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

        private void SavePin()
        {
            try
            {
                InformationBuyerADO ado = new InformationBuyerADO();
                ado.FullName = txtBuyerName.Text;
                ado.TaxCode = txtBuyerTaxCode.Text;
                ado.UnitID = cboBuyerOrganization.EditValue != null ? Int64.Parse(cboBuyerOrganization.EditValue.ToString()) : 0;
                ado.Address = txtBuyerAddress.Text;
                ado.AccountNumber = txtBuyerAccountNumber.Text;
                ado.UnitText = txtBuyerOrganization.Text;
                ado.checkBox = chkOther.Checked ? "1" : "0";
                string textJson = JsonConvert.SerializeObject(ado);

                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdateValue = (this.currentBySessionControlStateRDO != null && this.currentBySessionControlStateRDO.Count > 0) ? this.currentBySessionControlStateRDO.Where(o => o.KEY == "InformationBuyerADO" && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdateValue != null)
                {
                    csAddOrUpdateValue.VALUE = IsPin ? textJson : "";
                }
                else
                {
                    csAddOrUpdateValue = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdateValue.KEY = "InformationBuyerADO";
                    csAddOrUpdateValue.VALUE = IsPin ? textJson : "";
                    csAddOrUpdateValue.MODULE_LINK = moduleLink;
                    if (this.currentBySessionControlStateRDO == null)
                        this.currentBySessionControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentBySessionControlStateRDO.Add(csAddOrUpdateValue);
                }
                this.controlStateWorker.SetDataBySession(this.currentBySessionControlStateRDO);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkCoKetChuyen_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                CalcuCanThu();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridViewTransaction_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
        {

        }

        private void txtDiscount_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                FormatControl(ConfigApplications.NumberSeperator, txtDiscount);
                if (txtDiscount.EditValue != null)
                {
                    this.totalDiscount = txtDiscount.Value;
                    if (this.totalPatientPrice > 0)
                    {
                        //txtDiscountRatio.EditValue = (this.totalDiscount / this.totalPatientPrice) * 100;
                    }
                }
                else
                {
                    this.totalDiscount = 0;
                    //txtDiscountRatio.EditValue = null;
                }

                CalcuCanThu();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDiscountRatio_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtDiscountRatio.EditValue != null)
                {
                    var ratio = txtDiscountRatio.Value / 100;
                    var dis = this.totalPatientPrice * ratio;
                    if (Math.Abs((dis - this.totalDiscount)) > 0.0001m)
                    {
                        this.totalDiscount = dis;
                    }
                }
                else
                {
                    this.totalDiscount = 0;
                }

                CalcuCanThu();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ddBtnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                ddBtnPrint.ShowDropDown();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessBillFund()
        {
            try
            {
                if (currentTreatment != null && currentTreatment.FUND_ID.HasValue)
                {
                    ListBillFundPay = new List<V_HIS_BILL_FUND>();
                    CommonParam param = new CommonParam();
                    HisBillFundViewFilter filter = new HisBillFundViewFilter();
                    filter.FUND_ID = currentTreatment.FUND_ID;
                    filter.TREATMENT_ID = currentTreatment.ID;
                    ListBillFundPay = new BackendAdapter(param).Get<List<V_HIS_BILL_FUND>>("api/HisBillFund/GetView", ApiConsumers.MosConsumer, filter, param);
                }
            }
            catch (Exception ex)
            {
                ListBillFundPay = new List<V_HIS_BILL_FUND>();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void radioSGAll_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (!isInit && radioSGAll.Checked)
                {
                    this.ssTreeProcessor.CheckAllNode(this.ucSereServTree);
                    this.CalcuTotalPrice();
                    this.CalcuCanThu();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void radioSGExam_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (radioSGExam.Checked)
                {
                    this.ssTreeProcessor.CheckAllNode(this.ucSereServTree);
                    this.CalcuTotalPrice();
                    this.ProcessFundForHCM();
                    this.CalcuCanThu();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void radioSGCLS_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (radioSGCLS.Checked)
                {
                    this.ssTreeProcessor.CheckAllNode(this.ucSereServTree);
                    this.CalcuTotalPrice();
                    this.ProcessFundForHCM();
                    this.CalcuCanThu();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void radioSGMedicine_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (radioSGMedicine.Checked)
                {
                    this.ssTreeProcessor.CheckAllNode(this.ucSereServTree);
                    this.CalcuTotalPrice();
                    this.ProcessFundForHCM();
                    this.CalcuCanThu();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkNotTakePres_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (this.notHandleCheckedChanged)
                {
                    this.notHandleCheckedChanged = false;
                    return;
                }

                bool check = checkNotTakePres.Checked;
                if (checkNotTakePres.Checked)
                {
                    this.ProcessNotTakenPrescription(ref check);
                }
                else
                {
                    this.ProcessTakenPrescription(ref check);
                }

                if (check != checkNotTakePres.Checked)
                {
                    this.notHandleCheckedChanged = true;
                    checkNotTakePres.Checked = check;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinTransferAmount_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                FormatControl(ConfigApplications.NumberSeperator, spinTransferAmount);
                CalcuCanThu();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboAccountBook_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                spinTongTuDen.EditValue = null;
                spinTongTuDen.Enabled = false;
                //cboAccountBook.Properties.Buttons[1].Visible = false;
                if (cboAccountBook.EditValue != null)
                {
                    //cboAccountBook.Properties.Buttons[1].Visible = true;
                    var account = this.ListAccountBook.FirstOrDefault(o => o.ID == Convert.ToInt64(cboAccountBook.EditValue));
                    if (account != null)
                    {
                        spinTongTuDen.EditValue = setDataToDicNumOrderInAccountBook(account);
                        if (account.IS_NOT_GEN_TRANSACTION_ORDER == 1)
                        {
                            spinTongTuDen.Enabled = true;
                        }

                        // thu ngân mở 2 phòng.
                        // sổ ở phòng nào tự động chọn theo phòng đó.
                        if (GlobalVariables.DefaultAccountBookTransactionBill == null)
                        {
                            GlobalVariables.DefaultAccountBookTransactionBill = new List<V_HIS_ACCOUNT_BOOK>();
                        }

                        if (GlobalVariables.DefaultAccountBookTransactionBill.Count > 0)
                        {
                            List<V_HIS_ACCOUNT_BOOK> acc = new List<V_HIS_ACCOUNT_BOOK>();
                            acc.AddRange(GlobalVariables.DefaultAccountBookTransactionBill);
                            //add lại sổ để luôn đưa sổ vừa chọn lên đầu.
                            GlobalVariables.DefaultAccountBookTransactionBill = new List<V_HIS_ACCOUNT_BOOK>();
                            GlobalVariables.DefaultAccountBookTransactionBill.Add(account);
                            foreach (var item in acc)
                            {
                                if (item.ID != account.ID)
                                {
                                    GlobalVariables.DefaultAccountBookTransactionBill.Add(item);
                                }
                            }
                        }
                        else
                        {
                            GlobalVariables.DefaultAccountBookTransactionBill.Add(account);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtTotalAmount_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                FormatControl(ConfigApplications.NumberSeperator, txtTotalAmount);
                EnableSave();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkAutoRepay_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                cboAccountBookRepay.Enabled = chkAutoRepay.Checked;
                spinNumOrderRepay.Enabled = chkAutoRepay.Checked;
                SetDefaultAccountBookRepay();
                this.lciRepayAmount.Visibility = chkAutoRepay.Checked ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;

                if (chkAutoRepay.Checked)
                {
                    var payForm = this.payFormList.FirstOrDefault(o => o.PayFormId == this.cboPayForm.EditValue);
                    if (payForm != null && payForm.ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__THE)
                    {
                        chkCoKetChuyen.Checked = true;
                        chkCoKetChuyen.ReadOnly = true;
                        chkCoKetChuyen.Enabled = false;
                    }
                }
                else
                {
                    chkCoKetChuyen.ReadOnly = false;
                    chkCoKetChuyen.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboAccountBookRepay_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                spinNumOrderRepay.EditValue = null;
                spinNumOrderRepay.Enabled = false;
                if (cboAccountBookRepay.EditValue != null)
                {
                    var account = this.ListAccountBookRepay.FirstOrDefault(o => o.ID == Convert.ToInt64(cboAccountBookRepay.EditValue));
                    if (account != null)
                    {
                        spinNumOrderRepay.EditValue = setDataToDicNumOrderInAccountBook(account);
                        if (account.IS_NOT_GEN_TRANSACTION_ORDER == 1)
                        {
                            spinNumOrderRepay.Enabled = true;
                            ValidControlNumorderRepay(true);
                        }
                        else
                        {
                            spinNumOrderRepay.Enabled = false;
                            ValidControlNumorderRepay(false);
                        }

                        GlobalVariables.DefaultAccountBookTransactionBill__Repay = new List<V_HIS_ACCOUNT_BOOK>();
                        GlobalVariables.DefaultAccountBookTransactionBill__Repay.Add(account);
                    }
                }
                else
                {
                    ValidControlNumorderRepay(false);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboAccountBookRepay_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (cboAccountBookRepay.EditValue != null)
                    {
                        var account = this.ListAccountBookRepay.FirstOrDefault(o => o.ID == Convert.ToInt64(cboAccountBookRepay.EditValue));
                        if (account != null)
                        {
                            //txtAccountBookCode.Text = account.ACCOUNT_BOOK_CODE;
                            //SetDataToDicNumOrderInAccountBook(account);
                            //GlobalVariables.DefaultAccountBookTransactionBill = new List<V_HIS_ACCOUNT_BOOK>();
                            //GlobalVariables.DefaultAccountBookTransactionBill.Add(account);
                        }
                    }
                    else
                    {
                        spinNumOrderRepay.Text = "";
                        spinNumOrderRepay.EditValue = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboAccountBook_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinTongTuDen.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void dtTransactionTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboPayForm.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkAutoRepay_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboAccountBookRepay.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboAccountBookRepay_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinNumOrderRepay.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinNumOrderRepay_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboAccountBookRepay.EditValue != null)
                    {
                        var accountBook = this.ListAccountBookRepay.FirstOrDefault(o => o.ID == Convert.ToInt64(cboAccountBookRepay.EditValue.ToString()));
                        UpdateDictionaryNumOrderAccountBook(accountBook, spinNumOrderRepay.Value);
                    }
                    chkCoKetChuyen.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinNumOrderRepay_Spin(object sender, SpinEventArgs e)
        {
            if (cboAccountBookRepay.EditValue != null)
            {
                var accountBook = this.ListAccountBookRepay.FirstOrDefault(o => o.ID == Convert.ToInt64(cboAccountBookRepay.EditValue.ToString()));
                UpdateDictionaryNumOrderAccountBook(accountBook, spinNumOrderRepay.Value);
            }
        }

        private void txtBuyerName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtBuyerTaxCode.Focus();
                    txtBuyerTaxCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtBuyerTaxCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtBuyerAccountNumber.Focus();
                    txtBuyerAccountNumber.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtBuyerAccountNumber_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtBuyerOrganization.Focus();
                    txtBuyerOrganization.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtBuyerOrganization_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtBuyerAddress.Focus();
                    txtBuyerAddress.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitControlState()
        {
            try
            {
                isNotLoadWhileChangeControlStateInFirst = true;
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(currentModule.ModuleLink);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == chkHideHddt.Name)
                        {
                            chkHideHddt.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == chkInHoanUng.Name)
                        {
                            chkInHoanUng.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == chkPrintHddt.Name)
                        {
                            chkPrintHddt.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == chkPrintPrescription.Name)
                        {
                            chkPrintPrescription.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == chkConnectPOS.Name)
                        {
                            chkConnectPOS.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == chkPrintBKBHNT.Name)
                        {
                            chkPrintBKBHNT.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == chkShowServiceNotPay.Name)
                        {
                            chkShowServiceNotPay.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == btnStateForInformationUser.Name)
                        {
                            if (item.VALUE == "1")
                            {
                                btnStateForInformationUser.Properties.Buttons[0].Visible = true;
                                btnStateForInformationUser.Properties.Buttons[1].Visible = false;
                                IsPin = true;
                            }
                            else
                            {
                                btnStateForInformationUser.Properties.Buttons[0].Visible = false;
                                btnStateForInformationUser.Properties.Buttons[1].Visible = true;
                                IsPin = false;
                            }
                        }
                        else if (item.KEY == chkAddressBhyt.Name)
                        {
                            chkAddressBhyt.Checked = item.VALUE == "1";
                        }
                    }
                }
                InformationBuyerADO ado = new InformationBuyerADO();
                this.currentBySessionControlStateRDO = controlStateWorker.GetDataBySession(moduleLink);
                if (this.currentBySessionControlStateRDO != null && this.currentBySessionControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentBySessionControlStateRDO)
                    {
                        if (item.KEY == "InformationBuyerADO")
                        {
                            if (!string.IsNullOrEmpty(item.VALUE))
                            {
                                ado = JsonConvert.DeserializeObject<InformationBuyerADO>(item.VALUE);
                            }
                            else
                            {

                                btnStateForInformationUser.Properties.Buttons[0].Visible = false;
                                btnStateForInformationUser.Properties.Buttons[1].Visible = true;
                                IsPin = false;
                            }
                        }
                    }
                    if (ado != null)
                    {
                        txtBuyerName.Text = ado.FullName;
                        txtBuyerTaxCode.Text = ado.TaxCode;
                        cboBuyerOrganization.EditValue = ado.UnitID;
                        txtBuyerAddress.Text = ado.Address;
                        txtBuyerAccountNumber.Text = ado.AccountNumber;
                        txtBuyerOrganization.Text = ado.UnitText;
                        chkOther.Checked = ado.checkBox == "1" ? true : false;
                        cboBuyerOrganization.Focus();
                    }
                }
                else
                {
                    btnStateForInformationUser.Properties.Buttons[0].Visible = false;
                    btnStateForInformationUser.Properties.Buttons[1].Visible = true;
                    IsPin = false;
                }


                bool isBHYT = false;
                long patientTypeIdBHYT = HisConfigCFG.PatientTypeId__BHYT;
                var sereServ_BHYT = this.ListSereServTranfer.Where(o =>
                    o.PATIENT_TYPE_ID == patientTypeIdBHYT).ToList();

                if (sereServ_BHYT != null && sereServ_BHYT.Count > 0)
                {
                    isBHYT = true;
                }
                chkPrintBKBHNT.Enabled = isBHYT;
                if (!isBHYT)
                {
                    chkPrintBKBHNT.Checked = false;
                }

                isNotLoadWhileChangeControlStateInFirst = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkHideHddt_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }

                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkHideHddt.Name && o.MODULE_LINK == currentModule.ModuleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkHideHddt.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkHideHddt.Name;
                    csAddOrUpdate.VALUE = (chkHideHddt.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = currentModule.ModuleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }

                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkPrintHddt_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }

                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkPrintHddt.Name && o.MODULE_LINK == currentModule.ModuleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkPrintHddt.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkPrintHddt.Name;
                    csAddOrUpdate.VALUE = (chkPrintHddt.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = currentModule.ModuleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }

                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkAutoClose_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhilechkAutoCloseStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkAutoClose.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkAutoClose.CheckState.ToString());
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkAutoClose.Name;
                    csAddOrUpdate.VALUE = (chkAutoClose.CheckState.ToString());
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

        private void chkPrintPrescription_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkPrintPrescription.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkPrintPrescription.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkPrintPrescription.Name;
                    csAddOrUpdate.VALUE = (chkPrintPrescription.Checked ? "1" : "");
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

        private void chkConnectPOS_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkConnectPOS.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkConnectPOS.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkConnectPOS.Name;
                    csAddOrUpdate.VALUE = (chkConnectPOS.Checked ? "1" : "");
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

        private void chkPrintBKBHNT_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkPrintBKBHNT.Name && o.MODULE_LINK == currentModule.ModuleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkPrintBKBHNT.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkPrintBKBHNT.Name;
                    csAddOrUpdate.VALUE = (chkPrintBKBHNT.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = currentModule.ModuleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }

                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ckShowServiceNotPay_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkShowServiceNotPay.Name && o.MODULE_LINK == currentModule.ModuleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkShowServiceNotPay.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkShowServiceNotPay.Name;
                    csAddOrUpdate.VALUE = (chkShowServiceNotPay.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = currentModule.ModuleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                LoadDataToTreeSereServ(false);
                EnableSave();
                this.controlStateWorker.SetData(this.currentControlStateRDO);

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnStateForInformationUser_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Glyph)
                {
                    if (e.Button.Index == 0)
                    {
                        btnStateForInformationUser.Properties.Buttons[0].Visible = false;
                        btnStateForInformationUser.Properties.Buttons[1].Visible = true;
                        IsPin = false;
                    }
                    else if (e.Button.Index == 1)
                    {
                        btnStateForInformationUser.Properties.Buttons[0].Visible = true;
                        btnStateForInformationUser.Properties.Buttons[1].Visible = false;
                        IsPin = true;
                    }
                    if (IsPin)
                    {
                        chkAddressBhyt.Checked = false;
                        chkAddressBhyt.Enabled = false;
                    }
                    else
                    {
                        chkAddressBhyt.Checked = false;
                        chkAddressBhyt.Enabled = true;
                    }
                    btnStateForInformationUser.Update();
                    HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == btnStateForInformationUser.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                    if (csAddOrUpdate != null)
                    {
                        csAddOrUpdate.VALUE = IsPin ? "1" : "";
                    }
                    else
                    {
                        csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                        csAddOrUpdate.KEY = btnStateForInformationUser.Name;
                        csAddOrUpdate.VALUE = IsPin ? "1" : "";
                        csAddOrUpdate.MODULE_LINK = moduleLink;
                        if (this.currentControlStateRDO == null)
                            this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                        this.currentControlStateRDO.Add(csAddOrUpdate);
                    }
                    this.controlStateWorker.SetData(this.currentControlStateRDO);


                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //private void txtBuyerName_EditValueChanged(object sender, EventArgs e)
        //{
        //    try
        //    {

        //        HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdateValue = (this.currentBySessionControlStateRDO != null && this.currentBySessionControlStateRDO.Count > 0) ? this.currentBySessionControlStateRDO.Where(o => o.KEY == txtBuyerName.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
        //        if (csAddOrUpdateValue != null)
        //        {
        //            csAddOrUpdateValue.VALUE = IsPin ? txtBuyerName.Text.Trim() : "";
        //        }
        //        else
        //        {
        //            csAddOrUpdateValue = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
        //            csAddOrUpdateValue.KEY = txtBuyerName.Name;
        //            csAddOrUpdateValue.VALUE = IsPin ? txtBuyerName.Text.Trim() : "";
        //            csAddOrUpdateValue.MODULE_LINK = moduleLink;
        //            if (this.currentBySessionControlStateRDO == null)
        //                this.currentBySessionControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
        //            this.currentBySessionControlStateRDO.Add(csAddOrUpdateValue);
        //        }
        //        this.controlStateWorker.SetDataBySession(this.currentBySessionControlStateRDO);

        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        //private void txtBuyerTaxCode_EditValueChanged(object sender, EventArgs e)
        //{
        //    try
        //    {

        //        HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdateValue = (this.currentBySessionControlStateRDO != null && this.currentBySessionControlStateRDO.Count > 0) ? this.currentBySessionControlStateRDO.Where(o => o.KEY == txtBuyerTaxCode.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
        //        if (csAddOrUpdateValue != null)
        //        {
        //            csAddOrUpdateValue.VALUE = IsPin ? txtBuyerTaxCode.Text.Trim() : "";
        //        }
        //        else
        //        {
        //            csAddOrUpdateValue = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
        //            csAddOrUpdateValue.KEY = txtBuyerTaxCode.Name;
        //            csAddOrUpdateValue.VALUE = IsPin ? txtBuyerTaxCode.Text.Trim() : "";
        //            csAddOrUpdateValue.MODULE_LINK = moduleLink;
        //            if (this.currentBySessionControlStateRDO == null)
        //                this.currentBySessionControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
        //            this.currentBySessionControlStateRDO.Add(csAddOrUpdateValue);
        //        }
        //        this.controlStateWorker.SetDataBySession(this.currentBySessionControlStateRDO);

        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        //private void txtBuyerAccountNumber_EditValueChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdateValue = (this.currentBySessionControlStateRDO != null && this.currentBySessionControlStateRDO.Count > 0) ? this.currentBySessionControlStateRDO.Where(o => o.KEY == txtBuyerAccountNumber.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
        //        if (csAddOrUpdateValue != null)
        //        {
        //            csAddOrUpdateValue.VALUE = IsPin ? txtBuyerAccountNumber.Text.Trim() : "";
        //        }
        //        else
        //        {
        //            csAddOrUpdateValue = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
        //            csAddOrUpdateValue.KEY = txtBuyerAccountNumber.Name;
        //            csAddOrUpdateValue.VALUE = IsPin ? txtBuyerAccountNumber.Text.Trim() : "";
        //            csAddOrUpdateValue.MODULE_LINK = moduleLink;
        //            if (this.currentBySessionControlStateRDO == null)
        //                this.currentBySessionControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
        //            this.currentBySessionControlStateRDO.Add(csAddOrUpdateValue);
        //        }
        //        this.controlStateWorker.SetDataBySession(this.currentBySessionControlStateRDO);

        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        //private void txtBuyerAddress_EditValueChanged(object sender, EventArgs e)
        //{
        //    try
        //    {

        //        HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdateValue = (this.currentBySessionControlStateRDO != null && this.currentBySessionControlStateRDO.Count > 0) ? this.currentBySessionControlStateRDO.Where(o => o.KEY == txtBuyerAddress.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
        //        if (csAddOrUpdateValue != null)
        //        {
        //            csAddOrUpdateValue.VALUE = IsPin ? txtBuyerAddress.Text.Trim() : "";
        //        }
        //        else
        //        {
        //            csAddOrUpdateValue = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
        //            csAddOrUpdateValue.KEY = txtBuyerAddress.Name;
        //            csAddOrUpdateValue.VALUE = IsPin ? txtBuyerAddress.Text.Trim() : "";
        //            csAddOrUpdateValue.MODULE_LINK = moduleLink;
        //            if (this.currentBySessionControlStateRDO == null)
        //                this.currentBySessionControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
        //            this.currentBySessionControlStateRDO.Add(csAddOrUpdateValue);
        //        }
        //        this.controlStateWorker.SetDataBySession(this.currentBySessionControlStateRDO);

        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        private void checkEdit1_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkOther.Checked)
                {
                    cboBuyerOrganization.Visible = false;
                    txtBuyerOrganization.Visible = true;

                    this.dxValidationProvider1.SetValidationRule(txtBuyerOrganization, null);
                    ValidControlBuyerOrganization();
                }
                else
                {
                    cboBuyerOrganization.Visible = true;
                    txtBuyerOrganization.Visible = false;
                }

                //HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdateValue = (this.currentBySessionControlStateRDO != null && this.currentBySessionControlStateRDO.Count > 0) ? this.currentBySessionControlStateRDO.Where(o => o.KEY == chkOther.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                //if (csAddOrUpdateValue != null)
                //{
                //    csAddOrUpdateValue.VALUE = IsPin ? (chkOther.Checked ? "1" : "") : "";
                //}
                //else
                //{
                //    csAddOrUpdateValue = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                //    csAddOrUpdateValue.KEY = chkOther.Name;
                //    csAddOrUpdateValue.VALUE = IsPin ? (chkOther.Checked ? "1" : "") : "";
                //    csAddOrUpdateValue.MODULE_LINK = moduleLink;
                //    if (this.currentBySessionControlStateRDO == null)
                //        this.currentBySessionControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                //    this.currentBySessionControlStateRDO.Add(csAddOrUpdateValue);
                //}
                //this.controlStateWorker.SetDataBySession(this.currentBySessionControlStateRDO);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidControlBuyerOrganization()
        {
            try
            {
                txtBuyerOrganizationValidationRule BuyerOrganizationRule = new txtBuyerOrganizationValidationRule();
                BuyerOrganizationRule.txtBuyerOrganization = txtBuyerOrganization;
                dxValidationProvider1.SetValidationRule(txtBuyerOrganization, BuyerOrganizationRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboBuyerOrganization_EditValueChanged(object sender, EventArgs e)
        {
            try
            {

                if (cboBuyerOrganization.EditValue != null)
                {
                    var dt = dtWorkPlace.Where(o => o.ID == Int64.Parse(cboBuyerOrganization.EditValue.ToString())).First();
                    if (!chkAddressBhyt.Checked)
                    {
                        txtBuyerAddress.Text = dt.ADDRESS;
                        txtBuyerTaxCode.Text = dt.TAX_CODE;
                    }
                    cboBuyerOrganization.Properties.Buttons[1].Visible = true;
                    //HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdateValue = (this.currentBySessionControlStateRDO != null && this.currentBySessionControlStateRDO.Count > 0) ? this.currentBySessionControlStateRDO.Where(o => o.KEY == cboBuyerOrganization.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                    //if (csAddOrUpdateValue != null)
                    //{
                    //    csAddOrUpdateValue.VALUE = IsPin ? cboBuyerOrganization.EditValue.ToString() : null;
                    //}
                    //else
                    //{
                    //    csAddOrUpdateValue = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    //    csAddOrUpdateValue.KEY = cboBuyerOrganization.Name;
                    //    csAddOrUpdateValue.VALUE = IsPin ? cboBuyerOrganization.EditValue.ToString() : null;
                    //    csAddOrUpdateValue.MODULE_LINK = moduleLink;
                    //    if (this.currentBySessionControlStateRDO == null)
                    //        this.currentBySessionControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    //    this.currentBySessionControlStateRDO.Add(csAddOrUpdateValue);
                    //}
                    //this.controlStateWorker.SetDataBySession(this.currentBySessionControlStateRDO);

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboBuyerOrganization_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboBuyerOrganization.EditValue = null;
                    cboBuyerOrganization.Properties.Buttons[1].Visible = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkInHoanUng_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }

                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkInHoanUng.Name && o.MODULE_LINK == currentModule.ModuleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkInHoanUng.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkInHoanUng.Name;
                    csAddOrUpdate.VALUE = (chkInHoanUng.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = currentModule.ModuleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }

                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void radioSuatAn_CheckStateChanged(object sender, EventArgs e)
        {
            try
            {
                if (radioSuatAn.Checked)
                {
                    this.ssTreeProcessor.CheckAllNode(this.ucSereServTree);
                    this.CalcuTotalPrice();
                    this.ProcessFundForHCM();
                    this.CalcuCanThu();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboDepositBook_ButtonClick(object sender, ButtonPressedEventArgs e)
        {

            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboDepositBook.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void cboDepositBook_EditValueChanged(object sender, EventArgs e)
        {

            try
            {
                var data = cboDepositBook.Properties.DataSource as List<V_HIS_ACCOUNT_BOOK>;
                if (data != null && data.Count > 0)
                {
                    if (cboDepositBook.EditValue != null)
                    {
                        var dbook = data.FirstOrDefault(o => o.ID == (long)cboDepositBook.EditValue);
                        if (dbook != null)
                        {
                            spnNumOrder.EditValue = setDataToDicNumOrderInAccountBook(dbook);
                            if (dbook.IS_NOT_GEN_TRANSACTION_ORDER == 1)
                            {
                                spnNumOrder.Enabled = true;
                                layoutControlItem47.AppearanceItemCaption.ForeColor = Color.Maroon;
                                SetValildSpnNumOrder(true);
                            }
                            else
                            {
                                spnNumOrder.Enabled = false;
                                layoutControlItem47.AppearanceItemCaption.ForeColor = Color.Black;
                                SetValildSpnNumOrder(false);
                            }

                            GlobalVariables.DefaultAccountBookTransactionBill__Deposit = new List<V_HIS_ACCOUNT_BOOK>();
                            GlobalVariables.DefaultAccountBookTransactionBill__Deposit.Add(dbook);
                        }
                    }
                    else
                    {
                        spnNumOrder.EditValue = null;
                    }
                }
                else
                {
                    spnNumOrder.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        private void SetValildSpnNumOrder(bool IsRequied)
        {

            try
            {
                SpinNumOrderRule validate = new SpinNumOrderRule();
                validate.spn = this.spnNumOrder;
                validate.IsRequied = IsRequied;
                dxValidationProvider1.SetValidationRule(this.spnNumOrder, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void spnNumOrder_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboDepositBook.EditValue != null)
                    {
                        var accountBook = this.ListAccountBookDeposit.FirstOrDefault(o => o.ID == Convert.ToInt64(cboDepositBook.EditValue.ToString()));
                        UpdateDictionaryNumOrderAccountBook(accountBook, spnNumOrder.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spnNumOrder_Spin(object sender, SpinEventArgs e)
        {
            if (cboDepositBook.EditValue != null)
            {
                var accountBook = this.ListAccountBookDeposit.FirstOrDefault(o => o.ID == Convert.ToInt64(cboDepositBook.EditValue.ToString()));
                UpdateDictionaryNumOrderAccountBook(accountBook, spnNumOrder.Value);
            }
        }
        V_HIS_TRANSACTION TransactionQr = null;
        List<HIS_CONFIG> listConfig = new List<HIS_CONFIG>();
        private void CheckEnableBtnQR()
        {
            try
            {
                listConfig = BackendDataWorker.Get<HIS_CONFIG>().Where(o => o.KEY.StartsWith("HIS.Desktop.Plugins.PaymentQrCode") && !string.IsNullOrEmpty(o.VALUE)).ToList();

                lciQr.Visibility = listConfig != null && listConfig.Count > 0 ? DevExpress.XtraLayout.Utils.LayoutVisibility.Always : DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                btnQr.Enabled = false;
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        private HIS_CONFIG selectedConfig = new HIS_CONFIG();
        private void btnQr_Click(object sender, EventArgs e)
        {

            try
            {
                if (!btnQr.Enabled)
                    return;
                btnQr.Enabled = true;
                if (listConfig != null && listConfig.Count > 0)
                {
                    if (listConfig.Count > 1)
                    {
                        popupMenu1.ClearLinks();
                        foreach (var item in listConfig)
                        {
                            string key = "";
                            string value = item.KEY;
                            int index = value.IndexOf("Info");
                            if (index > 0)
                            {
                                var shotkey = value.Substring(0, index);
                                string[] parts = shotkey.Split('.');
                                if (parts.Length > 0)
                                {
                                    key = parts[parts.Length - 1];
                                }
                            }
                            else
                            {
                                key = item.KEY;
                            }


                            BarButtonItem btnOption = new BarButtonItem(null, key);
                            btnOption.ItemClick += (s, args) =>
                            {

                                selectedConfig = item;
                                List<object> listArgs = new List<object>();
                                TransReqQRADO adoqr = new TransReqQRADO();
                                adoqr.TreatmentId = this.treatmentId ?? 0;
                                adoqr.TransReqId = CreateReqType.Transaction;
                                adoqr.ConfigValue = selectedConfig;
                                HIS_TRANSACTION tran = new HIS_TRANSACTION();
                                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TRANSACTION>(tran, TransactionQr);
                                adoqr.Transaction = tran;
                                listArgs.Add(adoqr);
                                LogSystem.Debug("_____Load module : HIS.Desktop.Plugins.CreateTransReqQR ; KEY: " + selectedConfig.KEY);

                                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.CreateTransReqQR", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);

                            };
                            popupMenu1.AddItem(btnOption);
                        }

                        popupMenu1.ShowPopup(Cursor.Position);
                    }
                    else
                    {
                        selectedConfig = listConfig[0];
                        List<object> listArgs = new List<object>();
                        TransReqQRADO adoqr = new TransReqQRADO();
                        adoqr.TreatmentId = this.treatmentId ?? 0;
                        adoqr.TransReqId = CreateReqType.Transaction;
                        adoqr.ConfigValue = selectedConfig;
                        HIS_TRANSACTION tran = new HIS_TRANSACTION();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TRANSACTION>(tran, TransactionQr);
                        adoqr.Transaction = tran;
                        listArgs.Add(adoqr);
                        LogSystem.Debug("_____Load module : HIS.Desktop.Plugins.CreateTransReqQR " + selectedConfig.KEY);
                        HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.CreateTransReqQR", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);


                    }

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void chkAddressBhyt_CheckedChanged(object sender, EventArgs e)
        {

            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                if (chkAddressBhyt.Checked && resultPatientType != null && !string.IsNullOrEmpty(resultPatientType.ADDRESS))
                {
                    txtBuyerAddress.Text = resultPatientType.ADDRESS;
                }
                else if (currentTransaction != null && currentTransaction.IS_CANCEL == 1)
                {
                    txtBuyerAddress.Text = currentTransaction.BUYER_ADDRESS;
                }
                else if (this.currentTreatment != null)
                {
                    txtBuyerAddress.Text = currentTreatment.TDL_PATIENT_ADDRESS;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkAddressBhyt.Name && o.MODULE_LINK == currentModule.ModuleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkAddressBhyt.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkAddressBhyt.Name;
                    csAddOrUpdate.VALUE = (chkAddressBhyt.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = currentModule.ModuleLink;
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
    }
}

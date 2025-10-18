using DevExpress.XtraWaitForm;
using Inventec.Common.WebApiClient;
using HIS.Desktop.Utility;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HIS.Desktop.ApiConsumer;
using Inventec.Core;
using HIS.Desktop.LocalStorage.BackendData;
using DevExpress.XtraExport;
using MOS.Filter;
using Inventec.Common.Adapter;
using Inventec.Common.Logging;
using static DevExpress.XtraPrinting.Native.ExportOptionsPropertiesNames;
using HIS.Desktop.Plugins.AdjustmentTransaction.config;
using Inventec.Desktop.Common.LanguageManager;
using HIS.UC.SereServTree;
using HIS.Desktop.Plugins.TransactionBill.ADO;
using DevExpress.XtraEditors.Controls;
using HIS.Desktop.Plugins.AdjustmentTransaction.Base;
using Inventec.Desktop.Common.Message;
using HIS.Desktop.Plugins.HIS.Desktop.Plugins.AdjustmentTransaction.Base;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraTreeList;
using HIS.Desktop.LocalStorage.LocalData;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TreeView;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using HIS.Desktop.Common;

namespace HIS.Desktop.Plugins.AdjustmentTransaction.AdjustmentTransaction
{
    public partial class frmAdjustmentTransaction : FormBase
    {
        V_HIS_TREATMENT_FEE treatmentFee = new V_HIS_TREATMENT_FEE();
        V_HIS_PATIENT_TYPE_ALTER resultPatientType;
        V_HIS_TRANSACTION currentTransaction = new V_HIS_TRANSACTION();
        Inventec.Desktop.Common.Modules.Module currentModule = null;
        SereServTreeProcessor ssTreeProcessor = null;
        UserControl ucSereServTree = null;
        decimal totalPatientPrice = 0;
        decimal totalPatientPriceFund = 0;
        List<PayFormADO> payFormList = new List<PayFormADO>();
        List<V_HIS_ACCOUNT_BOOK> ListAccountBook = new List<V_HIS_ACCOUNT_BOOK>();
        List<V_HIS_ACCOUNT_BOOK> ListAccountBookDeposit = new List<V_HIS_ACCOUNT_BOOK>();
        List<V_HIS_TRANSACTION> listTransaction = new List<V_HIS_TRANSACTION>();
        List<HIS_BANK> hisBankList = null;
        V_HIS_TRANSACTION resultTranBill = null;
        bool hienHoaDonNhap = true;
        bool PrintMps279 { get; set; }
        private int positionHandleControl = -1;
        private List<HIS_BILL_FUND> listBillFundPrint { get; set; }
        private List<HIS_SERE_SERV_BILL> hisSSBillsPrint { get; set; }
        private List<HIS_SERE_SERV> listSereServPrint { get; set; }
        private V_HIS_PATIENT_TYPE_ALTER patientTypeAlterPrint { get; set; }
        private V_HIS_DEPARTMENT_TRAN departmentTranPrint { get; set; }
        private V_HIS_PATIENT patientsPrint { get; set; }
        private List<V_HIS_TRANSACTION> lstTranPrint { get; set; }
        private List<HIS_SESE_DEPO_REPAY> lstSeseRepayPrint { get; set; }
        private List<HIS_SERE_SERV_DEPOSIT> listSereDepoPrint { get; set; }
        decimal totalCanThu = 0;
        const string invoiceTypeCreate__CreateInvoiceHIS = "2";
        const string invoiceTypeCreate__CreateInvoiceVnpt = "1";
        private const string SIGNED_EXTENSION = ".pdf";
        List<long> lstSereServId = new List<long>();
        List<V_HIS_SERE_SERV_5> ListSereServ = new List<V_HIS_SERE_SERV_5>();
        List<V_HIS_SERE_SERV_5> currentSereServs = null;
        Dictionary<long, List<V_HIS_SERE_SERV_BILL_1>> dicSereServBill = null;
        List<V_HIS_SERE_SERV_5> ListSereServTranfer;// list này từ module khác truyền sang, nếu không truyền thì gọi api để lấy về sereServ
        List<V_HIS_SERE_SERV_5> ListSereServNoExecute = new List<V_HIS_SERE_SERV_5>();
        bool? IsDirectlyBilling = null;
        V_HIS_CASHIER_ROOM cashierRoom;
        HIS_BRANCH branch = null;
        string departmentName = "";
        private Dictionary<object, decimal> adjustmentValues = new Dictionary<object, decimal>();
        DelegateRefreshData delegateRefreshData = null;
        public frmAdjustmentTransaction(Inventec.Desktop.Common.Modules.Module module, V_HIS_TRANSACTION tran, DelegateRefreshData delegateRefreshData)
            : base(module)
        {
            InitializeComponent();
            Base.ResourceLangManager.InitResourceLanguageManager();
            this.currentTransaction = tran;
            this.currentModule = module;
            this.delegateRefreshData = delegateRefreshData;
            InitSereServTree();
        }

        private void FillPatient()
        {
            try
            {
                if (treatmentFee != null)
                {
                    txtPatientCode.Text = treatmentFee.TDL_PATIENT_CODE;
                    txtPatientName.Text = treatmentFee.TDL_PATIENT_NAME;
                    txtPatientDob.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(treatmentFee.TDL_PATIENT_DOB);
                    txtPatientGender.Text = treatmentFee.TDL_PATIENT_GENDER_NAME;
                    txtPatientAddr.Text = treatmentFee.TDL_PATIENT_ADDRESS;

                    if (this.resultPatientType == null || this.resultPatientType.ID == 0)
                    {
                        this.resultPatientType = new BackendAdapter(new CommonParam())
                        .Get<MOS.EFMODEL.DataModels.V_HIS_PATIENT_TYPE_ALTER>("api/HisPatientTypeAlter/GetViewLastByTreatmentId",
                        ApiConsumers.MosConsumer, treatmentFee.ID, null);
                    }

                    if (this.resultPatientType != null)
                    {
                        txtPatientBHYT.Text = TrimHeinCardNumber(resultPatientType.HEIN_CARD_NUMBER);
                        txtPatientBHYTFrom.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(resultPatientType.HEIN_CARD_FROM_TIME ?? 0);
                        txtPatientBHYTTo.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(resultPatientType.HEIN_CARD_TO_TIME ?? 0);
                        txtNDKKCBBD.Text = resultPatientType.HEIN_MEDI_ORG_NAME;
                        txtPatientType.Text = resultPatientType.PATIENT_TYPE_NAME ?? "";

                        string rightRoute = "";
                        if (resultPatientType.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE)
                        {
                            rightRoute = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__RIGHT_ROUTE_TRUE", Base.ResourceLangManager.LanguageFrmAdjustmentTransaction, LanguageManager.GetCulture());
                        }
                        else
                        {
                            rightRoute = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__RIGHT_ROUTE_FALSE", Base.ResourceLangManager.LanguageFrmAdjustmentTransaction, LanguageManager.GetCulture());
                        }
                        txtRightRoute.Text = rightRoute ?? "";
                        string ratio = "";
                        if (resultPatientType.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT)
                        {
                            decimal? heinRatio = new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(resultPatientType.HEIN_TREATMENT_TYPE_CODE, resultPatientType.HEIN_CARD_NUMBER, resultPatientType.LEVEL_CODE, resultPatientType.RIGHT_ROUTE_CODE, (treatmentFee.TOTAL_HEIN_PRICE ?? 0 + treatmentFee.TOTAL_PATIENT_PRICE_BHYT ?? 0));
                            if (heinRatio.HasValue)
                            {
                                ratio = ((long)(heinRatio.Value * 100)).ToString() + "%";
                            }
                        }
                        txtRatio.Text = ratio ?? "";
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        internal static string TrimHeinCardNumber(string chucodau)
        {
            string result = "";
            try
            {
                result = System.Text.RegularExpressions.Regex.Replace(chucodau, @"[-,_ ]|[_]{2}|[_]{3}|[_]{4}|[_]{5}", "").ToUpper();
            }
            catch (Exception ex)
            {
                LogSystem.Error("Không thể tách thẻ BHYT");
            }
            return result;
        }

        private void FillHisTreatmentFee()
        {
            try
            {
                if (currentTransaction != null)
                {
                    CommonParam common = new CommonParam();
                    MOS.Filter.HisTreatmentFeeViewFilter filter = new HisTreatmentFeeViewFilter();
                    filter.ID = this.currentTransaction.TDL_PATIENT_ID;
                    var treatmentFeeList = new BackendAdapter(new CommonParam()).Get<List<V_HIS_TREATMENT_FEE>>("api/HisTreatment/GetFeeView", ApiConsumers.MosConsumer, filter, null);

                    if (treatmentFeeList != null && treatmentFeeList.Count > 0)
                    {
                        this.treatmentFee = treatmentFeeList.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkEdit1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void frmAdjustmentTransaction_Load(object sender, EventArgs e)
        {
            try
            {
                this.CheckTransaction();
                this.FillHisTreatmentFee();
                this.FillPatient();
                this.GetListSereServBill();
                this.LoadDataToComboPayForm();
                this.FillDataToGirdTransaction();
                this.LoadDataToComboAccountBookDeposit();
                this.LoadDataToComboAccountBook();
                this.GeneratePopupMenu();
                this.LoadDataToTreeSereServ(false);//TODO
                this.LoadCashierRoomAndBranch();
                this.LoadAccountBookToLocal();
                this.ResetControlValue();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CheckTransaction()
        {
            try
            {
                if (currentTransaction != null &&
                    (currentTransaction.TRANSACTION_TYPE_ID != 3 || currentTransaction.IS_ADJUSTMENT == 1 || currentTransaction.IS_CANCEL == 1))
                {
                    this.btnSaveAndSign.Enabled = false;
                    this.btnSavePrint.Enabled = false;
                    this.btnSave.Enabled = false;
                    this.ddBtnPrint.Enabled = false;
                    this.txtReason.Enabled = false;
                    this.cboPayForm.Enabled = false;
                    this.cboAccountBook.Enabled = false;
                    this.dtTransactionTime.Enabled = false;
                }
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
                    lData = LocalStorage.BackendData.BackendDataWorker.Get<HIS_PAY_FORM>().Where(o => o.IS_ACTIVE == 1).ToList();
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
                    hisBankList = LocalStorage.BackendData.BackendDataWorker.Get<HIS_BANK>();
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
                        payForm.IS_REQUIRED_BANK = item.IS_REQUIRED_BANK;
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
                        payForm.IS_REQUIRED_BANK = payForm__QuetThe.IS_REQUIRED_BANK;
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
                }
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

        private async Task FillDataToGirdTransaction()
        {
            try
            {
                //if (this.treatmentFee != null)
                {
                    CommonParam param = new CommonParam();
                    //HisTransactionViewFilter tranFilter = new HisTransactionViewFilter();
                    //tranFilter.TREATMENT_ID = this.treatmentFee.ID;
                    //tranFilter.ORDER_DIRECTION = "DESC";
                    //tranFilter.ORDER_FIELD = "MODIFY_TIME";
                    //tranFilter.TRANSACTION_TYPE_IDs = new List<long>() { IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TU };
                    //this.listTransaction = await new Inventec.Common.Adapter.BackendAdapter(param).GetAsync<List<V_HIS_TRANSACTION>>("api/HisTransaction/GetView", ApiConsumers.MosConsumer, tranFilter, param);

                    if (HisConfigCFG.ShowServerTimeByDefault == "1")
                    {
                        dtTransactionTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(param.Now) ?? DateTime.MinValue;
                    }
                }
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
        private void InitSereServTree()
        {
            try
            {
                ssTreeProcessor = new UC.SereServTree.SereServTreeProcessor();
                SereServTreeADO ado = new SereServTreeADO();
                ado.IsShowCheckNode = true;
                ado.IsShowSearchPanel = false;
                ado.HideCheckColumn = true;     // ẩn cột check
                ado.SereServTreeForBill_BeforeCheck = treeSereServ_BeforeCheckNode;
                //ado.SereServTree_AfterCheck = treeSereServ_AfterCheckNode;
                ado.SereServTree_CheckAllNode = treeSereServ_CheckAllNode;
                ado.sereServTree_ShowingEditor = sereServTree_ShowingEditorDG;
                ado.SereServTree_CustomDrawNodeCell = treeSereServ_CustomDrawNodeCell;
                ado.SereServTree_CustomDrawNodeCheckBox = treeSereServ_CustomDrawNodeCheckBox;
                ado.SereServTree_CustomUnboundColumnData = treeSereServ_CustomUnboundColumnData;
                ado.SereServTree_MouseDown = treeSereServ_MouseDown;
                ado.sereServTree_ShowingEditorArgs = sereServTree_ShowingEditorArgs;
                ado.treeSereServ_CellValueChanged = treeSereServ_CellValueChanged;

                ado.SereServTreeColumns = new List<SereServTreeColumn>();
                ado.LayoutSereServExpend = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_ADJUSTMENT_TRANSACTION__LAYOUT_SERE_SERV_EXPEND", Base.ResourceLangManager.LanguageFrmAdjustmentTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                //Column tên dịch vụ
                SereServTreeColumn serviceNameCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_ADJUSTMENT_TRANSACTION__TREE_SERE_SERV__COLUMN_SERVICE_NAME", Base.ResourceLangManager.LanguageFrmAdjustmentTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "TDL_SERVICE_NAME", 180, false);
                serviceNameCol.VisibleIndex = 0;
                ado.SereServTreeColumns.Add(serviceNameCol);

                //Column mã dịch vụ
                SereServTreeColumn serviceCodeCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_ADJUSTMENT_TRANSACTION__TREE_SERE_SERV__COLUMN_SERVICE_CODE", Base.ResourceLangManager.LanguageFrmAdjustmentTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "TDL_SERVICE_CODE", 80, false);
                serviceCodeCol.VisibleIndex = 1;
                ado.SereServTreeColumns.Add(serviceCodeCol);

                //Column Số lượng
                SereServTreeColumn amountCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_ADJUSTMENT_TRANSACTION__TREE_SERE_SERV__COLUMN_AMOUNT", Base.ResourceLangManager.LanguageFrmAdjustmentTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "AMOUNT_PLUS", 40, false);//AMOUNT_PLUS
                amountCol.VisibleIndex = 2;
                amountCol.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;
                amountCol.Format = new DevExpress.Utils.FormatInfo();
                amountCol.Format.FormatString = "#,##0.00";
                amountCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.SereServTreeColumns.Add(amountCol);

                //Column đơn giá
                SereServTreeColumn virPriceCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_ADJUSTMENT_TRANSACTION__TREE_SERE_SERV__COLUMN_VIR_PRICE", Base.ResourceLangManager.LanguageFrmAdjustmentTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "VIR_PRICE_DISPLAY", 80, false);//VIR_PRICE
                virPriceCol.VisibleIndex = 3;
                virPriceCol.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;
                ado.SereServTreeColumns.Add(virPriceCol);

                //Column vat (%)
                SereServTreeColumn virVatRatioCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_ADJUSTMENT_TRANSACTION__TREE_SERE_SERV__COLUMN_VAT_RATIO", Base.ResourceLangManager.LanguageFrmAdjustmentTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "VAT_DISPLAY", 80, false);
                virVatRatioCol.VisibleIndex = 4;
                virVatRatioCol.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;
                ado.SereServTreeColumns.Add(virVatRatioCol);

                //Column thành tiền
                SereServTreeColumn virTotalPriceCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_ADJUSTMENT_TRANSACTION__TREE_SERE_SERV__COLUMN_VIR_TOTAL_PRICE", Base.ResourceLangManager.LanguageFrmAdjustmentTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "VIR_TOTAL_PRICE_DISPLAY", 80, false);//VIR_TOTAL_PRICE
                virTotalPriceCol.VisibleIndex = 5;
                virTotalPriceCol.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;
                ado.SereServTreeColumns.Add(virTotalPriceCol);

                //Column bệnh nhân trả
                SereServTreeColumn virTotalPatientPriceCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_ADJUSTMENT_TRANSACTION__TREE_SERE_SERV__COLUMN_VIR_TOTAL_PATIENT_PRICE", Base.ResourceLangManager.LanguageFrmAdjustmentTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "VIR_TOTAL_PATIENT_PRICE_DISPLAY", 80, false);//VIR_TOTAL_PATIENT_PRICE
                virTotalPatientPriceCol.VisibleIndex = 6;
                virTotalPatientPriceCol.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;
                ado.SereServTreeColumns.Add(virTotalPatientPriceCol);

                //Column tổng hóa đơn
                SereServTreeColumn virTotalBillAmountCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_ADJUSTMENT_TRANSACTION__TREE_SERE_SERV__COLUMN_TOTAL_BILL_AMOUNT", Base.ResourceLangManager.LanguageFrmAdjustmentTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "TOTAL_BILL_AMOUNT", 80, false);//TOTAL_BILL_AMOUNT
                virTotalBillAmountCol.VisibleIndex = 7;
                virTotalBillAmountCol.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;
                ado.SereServTreeColumns.Add(virTotalBillAmountCol);

                //Column điều chỉnh
                SereServTreeColumn virEditAmountCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_ADJUSTMENT_TRANSACTION__TREE_SERE_SERV__COLUMN_EDIT_AMOUNT", Base.ResourceLangManager.LanguageFrmAdjustmentTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "EDIT_AMOUNT", 80, true);
                virEditAmountCol.VisibleIndex = 8;
                virEditAmountCol.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Decimal;
                virEditAmountCol.Format = new DevExpress.Utils.FormatInfo();
                virEditAmountCol.Format.FormatString = "#,##0";
                virEditAmountCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.SereServTreeColumns.Add(virEditAmountCol);


                // Column nút ↑     
                SereServTreeColumn increaseCol = new SereServTreeColumn(" ", "INCREASE_BTN", 40, false);
                increaseCol.VisibleIndex = 9;
                increaseCol.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.String;
                ado.SereServTreeColumns.Add(increaseCol);

                // Column nút ↓
                SereServTreeColumn decreaseCol = new SereServTreeColumn(" ", "DECREASE_BTN", 40, false);
                decreaseCol.VisibleIndex = 10;
                decreaseCol.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.String;
                ado.SereServTreeColumns.Add(decreaseCol);

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
        private void sereServTree_ShowingEditorArgs(SereServADO data, DevExpress.XtraTreeList.GetCustomNodeCellEditEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "EDIT_AMOUNT")
                {
                    // Tạo editor chỉ cho phép nhập số
                    RepositoryItemTextEdit textEdit = new RepositoryItemTextEdit();
                    textEdit.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
                    textEdit.Mask.EditMask = "n0"; // Số nguyên
                    textEdit.Mask.UseMaskAsDisplayFormat = true;

                    e.RepositoryItem = textEdit;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void treeSereServ_CellValueChanged(SereServADO data, DevExpress.XtraTreeList.CellValueChangedEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == "EDIT_AMOUNT" && data != null)
                {
                    if (data.IsFather == true || e.Node.Level == 0)
                        return;
                    // Lấy giá trị mới
                    decimal newValue = 0;
                    if (e.Value != null && decimal.TryParse(e.Value.ToString(), out decimal parsedValue))
                    {
                        newValue = parsedValue;
                    }

                    // Lưu vào dictionary
                    object nodeKey = data.ID;
                    adjustmentValues[nodeKey] = newValue;

                    // ✅ QUAN TRỌNG: Refresh node hiện tại trước
                    TreeList tree = FindTreeListInControl(this.ucSereServTree);
                    if (tree != null && e.Node != null)
                    {
                        tree.RefreshNode(e.Node); // ← Thêm dòng này

                        // Refresh node cha (nếu có)
                        if (e.Node.ParentNode != null)
                        {
                            tree.RefreshNode(e.Node.ParentNode);

                            // Refresh cả node cha của cha (nếu có)
                            TreeListNode grandParent = e.Node.ParentNode.ParentNode;
                            while (grandParent != null)
                            {
                                tree.RefreshNode(grandParent);
                                grandParent = grandParent.ParentNode;
                            }
                        }
                    }

                    // Cập nhật tổng
                    this.UpdateTotalAdjustment();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private TreeList FindTreeListInControl(Control parent)
        {
            foreach (Control ctrl in parent.Controls)
            {
                if (ctrl is TreeList tree)
                    return tree;

                var found = FindTreeListInControl(ctrl);
                if (found != null)
                    return found;
            }
            return null;
        }
        private void treeSereServ_MouseDown(SereServADO data, MouseEventArgs e)
        {
            try
            {
                // Nếu data null thì dừng
                if (data == null)
                    return;

                // Lấy TreeList hiện tại từ control đang hiển thị
                TreeList tree = FindTreeListInControl(this.ucSereServTree);
                if (tree == null)
                {
                    MessageBox.Show("Không tìm thấy TreeList trong ucSereServTree");
                    return;
                }

                // Lấy key duy nhất cho node
                object nodeKey = data.ID;
                // Lấy giá trị bệnh nhân trả
                // Lấy giá trị bệnh nhân trả
                decimal patientPrice = data.TOTAL_BILL_AMOUNT ?? 0;

                // Nếu click chuột phải hoặc giữa thì bỏ qua
                if (e.Button != MouseButtons.Left)
                    return;

                // Xác định vị trí click trên TreeList
                var hitInfo = tree.CalcHitInfo(e.Location);
                if (hitInfo == null || hitInfo.Column == null)
                    return;

                if (hitInfo.Column.FieldName == "INCREASE_BTN")
                {
                    // Lấy giá trị hiện tại
                    decimal currentValue = adjustmentValues.ContainsKey(nodeKey) ? adjustmentValues[nodeKey] : 0m;

                    if (currentValue == 0)
                    {
                        // Từ 0 → nhảy lên patientPrice
                        adjustmentValues[nodeKey] = patientPrice;
                    }
                    else if (currentValue < 0)
                    {
                        // Từ âm → về 0
                        adjustmentValues[nodeKey] = 0;
                    }
                    else if (currentValue > 0 && currentValue < patientPrice)
                    {
                        // Từ số dương nhỏ hơn patientPrice → nhảy lên patientPrice
                        adjustmentValues[nodeKey] = patientPrice;
                    }
                    else if (currentValue == patientPrice)
                    {
                        // Đã ở max → không làm gì hoặc giữ nguyên
                        return;
                    }
                    else
                    {
                        // Trường hợp khác (nếu nhập tay > patientPrice) → nhảy về patientPrice
                        adjustmentValues[nodeKey] = patientPrice;
                    }

                    tree.RefreshDataSource();
                    this.UpdateTotalAdjustment();
                }
                else if (hitInfo.Column.FieldName == "DECREASE_BTN")
                {
                    decimal currentValue = adjustmentValues.ContainsKey(nodeKey) ? adjustmentValues[nodeKey] : 0m;

                    if (currentValue == 0)
                    {
                        // Từ 0 → nhảy xuống -patientPrice
                        adjustmentValues[nodeKey] = -patientPrice;
                    }
                    else if (currentValue > 0)
                    {
                        // Từ dương → về 0
                        adjustmentValues[nodeKey] = 0;
                    }
                    else if (currentValue < 0 && currentValue > -patientPrice)
                    {
                        // Từ số âm lớn hơn -patientPrice → nhảy xuống -patientPrice
                        adjustmentValues[nodeKey] = -patientPrice;
                    }
                    else if (currentValue == -patientPrice)
                    {
                        // Đã ở min → không làm gì hoặc giữ nguyên
                        return;
                    }
                    else
                    {
                        // Trường hợp khác (nếu nhập tay < -patientPrice) → nhảy về -patientPrice
                        adjustmentValues[nodeKey] = -patientPrice;
                    }

                    tree.RefreshDataSource();
                    var currentNode = tree.FocusedNode;
                    if (currentNode != null && currentNode.ParentNode != null)
                    {
                        tree.RefreshNode(currentNode.ParentNode);
                    }
                    this.UpdateTotalAdjustment();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi click: " + ex.Message);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void UpdateTotalAdjustment()
        {
            try
            {
                // Nếu chưa có dictionary thì thoát
                if (adjustmentValues == null || adjustmentValues.Count == 0)
                {
                    this.txtTotalAdjustment.Text = "0";
                    return;
                }

                // Tính tổng tất cả giá trị trong dictionary
                decimal total = adjustmentValues.Values.Sum();

                if(total != 0 && HisAdjustmentBillResult == null)
                {
                    btnSave.Enabled = true;
                    btnSavePrint.Enabled = true;
                    btnSaveAndSign.Enabled = true;
                    bbtnRCSave.Enabled = true;
                    bbtnRCSavePrint.Enabled = true;
                    bbtnRCSaveSign.Enabled = true;
                }
                else
                {
                    btnSave.Enabled = false;
                    btnSavePrint.Enabled = false;
                    btnSaveAndSign.Enabled = false;
                    bbtnRCSave.Enabled = false;
                    bbtnRCSavePrint.Enabled = false;
                    bbtnRCSaveSign.Enabled = false;
                }
                // Gán vào textbox, định dạng đẹp hơn
                this.txtTotalAdjustment.Text = total.ToString("#,##0.00");

            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tính tổng điều chỉnh: " + ex.Message);
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetListSereServBill()
        {

            try
            {
                HisSereServBillViewFilter ssBillFilter = new HisSereServBillViewFilter();
                ssBillFilter.BILL_ID = currentTransaction.ID;
                var hisSSBills = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV_BILL>>("api/HisSereServBill/GetView", ApiConsumers.MosConsumer, ssBillFilter, null);
                if (hisSSBills != null && hisSSBills.Count > 0)
                    lstSereServId = hisSSBills.Select(o => o.SERE_SERV_ID).ToList();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private void sereServTree_ShowingEditorDG(TreeListNode node, object sender)
        {
            try
            {
                var nodeData = node.TreeList.GetDataRecordByNode(node);
                if (nodeData != null && config.HisConfigCFG.MustFinishTreatmentForBill == "1" && (nodeData as SereServADO).PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT)
                {
                    ((TreeList)sender).ActiveEditor.Properties.ReadOnly = true;
                }
                else if (nodeData != null && config.HisConfigCFG.MustFinishTreatmentForBill == "2")
                {
                    ((TreeList)sender).ActiveEditor.Properties.ReadOnly = true;
                }
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
                dicSereServBill = new Dictionary<long, List<V_HIS_SERE_SERV_BILL_1>>();
                List<V_HIS_SERE_SERV_BILL_1> listSSBill = new List<V_HIS_SERE_SERV_BILL_1>();
                if (this.currentTransaction != null && this.currentTransaction.TREATMENT_ID != null)
                {
                    HisSereServBillFilter ssBillFilter = new HisSereServBillFilter();
                    ssBillFilter.TDL_TREATMENT_ID = this.currentTransaction.TREATMENT_ID ?? 0;
                    ssBillFilter.BILL_ID = this.currentTransaction.ID;
                    listSSBill = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV_BILL_1>>("api/HisSereServBill/GetView1", ApiConsumers.MosConsumer, ssBillFilter, null);
                    if (listSSBill != null && listSSBill.Count > 0)
                    {
                        foreach (var item in listSSBill)
                        {
                            if (item.IS_CANCEL == (short)1)
                                continue;
                            if (!dicSereServBill.ContainsKey(item.SERE_SERV_ID))
                                dicSereServBill[item.SERE_SERV_ID] = new List<V_HIS_SERE_SERV_BILL_1>();
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
                        ssFilter.TDL_TREATMENT_ID = this.currentTransaction.TREATMENT_ID;
                        var hisSereServs = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV_5>>("api/HisSereServ/GetView5", ApiConsumers.MosConsumer, ssFilter, null);
                        if (hisSereServs != null && hisSereServs.Count > 0)
                        {
                            currentSereServs = hisSereServs;
                            foreach (var item in hisSereServs)
                            {
                                if (dicSereServBill.ContainsKey(item.ID))
                                {
                                    ListSereServ.Add(item);
                                }
                                if (hasIsNoExecute && item.IS_NO_EXECUTE == 1 && item.TDL_SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONK)
                                {
                                    this.ListSereServNoExecute.Add(item);
                                    continue;
                                }

                                if (item.IS_NO_PAY == 1 || item.IS_NO_EXECUTE == 1)
                                    continue;
                            }
                        }
                    }
                }

                // bỏ những dịch vụ đã chốt nợ
                if (this.currentTransaction != null && this.currentTransaction.TREATMENT_ID != null && ListSereServ != null && ListSereServ.Count > 0)
                {
                    MOS.Filter.HisSereServDebtFilter sereServDebtFilter = new HisSereServDebtFilter();
                    sereServDebtFilter.TDL_TREATMENT_ID = this.currentTransaction.TREATMENT_ID;
                    var sereServDebtList = new BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV_DEBT>>("api/HisSereServDebt/Get", ApiConsumer.ApiConsumers.MosConsumer, sereServDebtFilter, null);
                    if (sereServDebtList != null && sereServDebtList.Count > 0)
                    {
                        sereServDebtList = sereServDebtList.Where(o => o.IS_CANCEL != 1).ToList();

                        this.ListSereServ = sereServDebtList != null && sereServDebtList.Count > 0
                            ? this.ListSereServ.Where(o => !sereServDebtList.Select(p => p.SERE_SERV_ID).Contains(o.ID)).ToList()
                            : this.ListSereServ;
                    }
                }

                ssTreeProcessor.Reload(ucSereServTree, ListSereServ, listSSBill);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboAccountBook_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (cboAccountBook.EditValue != null)
                    {
                        var account = ListAccountBook.FirstOrDefault(o => o.ID == Convert.ToInt64(cboAccountBook.EditValue));
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
                        spinTongTuDen.Text = "";
                    }
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

                if (this.treatmentFee == null || this.treatmentFee.ID == 0)
                {
                    if (this.currentTransaction.TREATMENT_ID.HasValue)
                    {
                        HisTreatmentFeeViewFilter feeFilter = new HisTreatmentFeeViewFilter();
                        feeFilter.ID = this.currentTransaction.TREATMENT_ID ?? 0;
                        var treatmentFees = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_TREATMENT_FEE>>("api/HisTreatment/GetView2", ApiConsumers.MosConsumer, feeFilter, null);
                        if (treatmentFees == null || treatmentFees.Count == 0)
                        {
                            return;
                        }
                        this.treatmentFee = treatmentFees.First();
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

                HisAccountBookViewFilter acFilter = new HisAccountBookViewFilter();
                acFilter.CASHIER_ROOM_ID = this.currentModule.RoomId;
                acFilter.LOGINNAME = loginName;
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
                SetDefaultAccountBook();
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

        private decimal setDataToDicNumOrderInAccountBook(V_HIS_ACCOUNT_BOOK accountBook)
        {
            decimal result = (accountBook.CURRENT_NUM_ORDER ?? 0) + 1;
            try
            {
                if (accountBook != null)
                {
                    if (LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook == null || LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.Count == 0 || (LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook != null && LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.Count > 0 && !LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.ContainsKey(accountBook.ID)))
                    {
                        if (LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook == null)
                        {
                            LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook = new Dictionary<long, decimal>();
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

                            LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.Add(accountBookNew.ID, num);
                            result = (LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook[accountBook.ID]) + 1;
                        }
                    }
                    else
                    {
                        result = (LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook[accountBook.ID]) + 1;
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
        private void UpdateDictionaryNumOrderAccountBook(V_HIS_ACCOUNT_BOOK accountBook, decimal numOrder)
        {
            try
            {
                if (accountBook != null && LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook != null && LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.Count > 0 && LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.ContainsKey(accountBook.ID))
                {
                    LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook[accountBook.ID] = numOrder;
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

        private void dtTransactionTime_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (layoutTongTuDen.Enabled)
                    {
                        spinTongTuDen.Focus();
                        spinTongTuDen.SelectAll();
                    }
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

        private void ResetControlValue()
        {
            try
            {
                resultTranBill = null;
                totalPatientPrice = 0;
                totalPatientPriceFund = 0;
                dxValidationProvider1.RemoveControlError(dtTransactionTime);
                dxValidationProvider1.RemoveControlError(cboPayForm);
                totalCanThu = 0;
                spinTongTuDen.Value = 0;
                txtReason.Text = "";
                //
                if (AdjustmentTransactionConfig.InvoiceTypeCreate == invoiceTypeCreate__CreateInvoiceVnpt)
                {
                    ddBtnPrint.Enabled = true;
                }
                else
                {
                    ddBtnPrint.Enabled = false;
                }

                // ✅ Cấu hình hiển thị thời gian có cả giờ, phút, giây
                dtTransactionTime.Properties.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm:ss";
                dtTransactionTime.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
                dtTransactionTime.Properties.EditFormat.FormatString = "dd/MM/yyyy HH:mm:ss";
                dtTransactionTime.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
                dtTransactionTime.Properties.Mask.EditMask = "dd/MM/yyyy HH:mm:ss";
                dtTransactionTime.Properties.Mask.UseMaskAsDisplayFormat = true;

                // Nếu dùng CalendarTimeProperties (tránh trường hợp control DateEdit chưa có time mask)
                dtTransactionTime.Properties.CalendarTimeProperties.DisplayFormat.FormatString = "dd/MM/yyyy HH:mm:ss";
                dtTransactionTime.Properties.CalendarTimeProperties.EditFormat.FormatString = "dd/MM/yyyy HH:mm:ss";
                dtTransactionTime.Properties.CalendarTimeProperties.Mask.EditMask = "dd/MM/yyyy HH:mm:ss";

                // ✅ Gán thời gian hiện tại (có cả giờ/phút/giây)
                DateTime now = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.Now() ?? 0) ?? DateTime.Now;
                dtTransactionTime.EditValue = now;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void bbtnRCSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSave_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnRCSavePrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSavePrint_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnRCSaveSign_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSaveAndSign_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSavePrint_EnabledChanged(object sender, EventArgs e)
        {
            try
            {

            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}

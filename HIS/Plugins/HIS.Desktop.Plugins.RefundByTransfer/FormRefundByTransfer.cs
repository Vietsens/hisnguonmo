using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.Library.BankHub;
using HIS.Desktop.Plugins.RefundByTransfer.Base;
using HIS.Desktop.Plugins.RefundByTransfer.Resources;
using HIS.Desktop.Utility;
using HIS.UC.TotalPriceInfo;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.Controls.ValidationRule;
using Inventec.Desktop.Common.Message;
using Inventec.Desktop.Common.Modules;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.RefundByTransfer
{
    public partial class FormRefundByTransfer : FormBase
    {
        private Inventec.Desktop.Common.Modules.Module currentModule;
        private HIS_TREATMENT treatment;
        private HIS_TRANSACTION transaction;
        private string bankCode;

        private V_HIS_TREATMENT_FEE currentTreatment;
        private V_HIS_TRANSACTION currentTransaction;
        private TotalPriceInfoProcessor totalPriceProcessor;
        private UserControl ucTotalPriceInfo;
        HIS.Desktop.Common.RefeshReference refresh;
        int ActionType = -1;
        int positionHandle = -1;

        public FormRefundByTransfer()
        {
            InitializeComponent();
        }

        public FormRefundByTransfer(Module moduleData, string bankCode, HIS_TREATMENT treatment, HIS_TRANSACTION transaction, HIS.Desktop.Common.RefeshReference refresh)
            : base(moduleData)
        {
            try
            {
                // TODO: Complete member initialization
                this.currentModule = moduleData;
                this.treatment = treatment;
                this.transaction = transaction;
                this.bankCode = !string.IsNullOrWhiteSpace(bankCode) ? bankCode : "MBB";
                this.refresh = refresh;
                if ((transaction != null && transaction.ID <= 0))
                {
                    MessageBox.Show("Thông tin giao dịch không hợp lệ. Vui lòng kiểm tra lại");
                    return;
                }

                if (transaction != null && transaction.TRANSACTION_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__HU)
                {
                    MessageBox.Show("Không phải giao dịch hoàn ứng");
                    return;
                }

                InitializeComponent();
                ResourceLangManager.InitResourceLanguageManager();
                SetIcon();
                InitTotalPriceInfo();
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
                if (this.currentModule != null)
                {
                    this.Text = currentModule.text;
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

                //minhnq
                data.layoutOtherSourcePrice = Inventec.Common.Resource.Get.Value("UCTotalPriceInfo.lciOtherSourcePrice.Text", ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                data.LayoutTotalOtherBillAmount = Inventec.Common.Resource.Get.Value("UCTotalPriceInfo.lciTotalOtherBillAmount.Text", ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                data.LayoutLockingAmount = Inventec.Common.Resource.Get.Value("UCTotalPriceInfo.LayoutLockingAmount.Text", ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                data.LayoutLockingAmountTotip = Inventec.Common.Resource.Get.Value("UCTotalPriceInfo.LayoutLockingAmountTotip.Text", ResourceLangManager.LanguageUCTransaction, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                this.ucTotalPriceInfo = (UserControl)this.totalPriceProcessor.Run(data);
                if (this.ucTotalPriceInfo != null)
                {
                    this.panelControl1.Controls.Add(this.ucTotalPriceInfo);
                    this.ucTotalPriceInfo.Dock = DockStyle.Fill;
                }
                Inventec.Common.Logging.LogSystem.Debug("UCTransaction.InitTotalPriceInfo => 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FormRefundByTransfer_Load(object sender, EventArgs e)
        {
            try
            {
                string denyReason = null;
                if (!CheckAllowRefundByTransfer(ref denyReason))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(denyReason, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.Close();
                    return;
                }

                InitComboBankPayer();

                CommonParam param = new CommonParam();
                HisTreatmentFeeViewFilter treatFilter = new HisTreatmentFeeViewFilter();
                treatFilter.ID = this.treatment.ID;
                var result = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_TREATMENT_FEE>>("api/HisTreatment/GetFeeView", ApiConsumers.MosConsumer, treatFilter, param);
                if (result != null && result.Count > 0)
                {
                    currentTreatment = result.FirstOrDefault();
                }

                HisPatientBankAccountFilter patientBankAccountFilter = new HisPatientBankAccountFilter();
                patientBankAccountFilter.PATIENT_ID = currentTreatment.PATIENT_ID;
                List<HIS_PATIENT_BANK_ACCOUNT> paBankAccs = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_PATIENT_BANK_ACCOUNT>>("api/HisPatientBankAccount/Get", ApiConsumers.MosConsumer, patientBankAccountFilter, param);
                if (paBankAccs != null && paBankAccs.Count > 0)
                {
                    HIS_PATIENT_BANK_ACCOUNT bankAcc = paBankAccs.OrderByDescending(o => o.ID).FirstOrDefault();
                    if (bankAcc != null)
                    {
                        cboBank.EditValue = bankAcc.PAYEE_BANK_ID;
                        txtAccNum.Text = bankAcc.PAYEE_ACCOUNT_NUMBER;
                        txtAccName.Text = bankAcc.PAYEE_NAME;
                    }
                }

                txtDescription.Text = string.Format("HOAN VIEN PHI {0}({1})", currentTreatment.TDL_PATIENT_UNSIGNED_NAME, currentTreatment.TREATMENT_CODE);
                FillInfoPatient(currentTreatment);

                if (transaction != null)
                {
                    HisTransactionViewFilter tranFilter = new HisTransactionViewFilter();
                    tranFilter.ID = this.transaction.ID;
                    var resultTran = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<V_HIS_TRANSACTION>>("api/HisTransaction/GetView", ApiConsumers.MosConsumer, tranFilter, param);
                    if (resultTran != null && resultTran.Count > 0)
                    {
                        //cố định số tiền theo giao dịch
                        currentTransaction = resultTran.FirstOrDefault();
                        spAmount.Value = currentTransaction.AMOUNT;
                        spAmount.ReadOnly = true;
                    }
                }

                if (!String.IsNullOrEmpty(bankCode) && bankCode == "MBB")
                {
                    string serviceConfig = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HisConfigCFG.REFUND_BY_TRANSFER_MBB__CONFIG);
                    string[] strings = serviceConfig.Split('|');
                    txtSourceNum.Text = strings[0];
                }

                ValidateForm();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Chốt chặn hoàn tiền ngân hàng trùng cho 1 giao dịch hoàn ứng (vCong 60555, 62084 - Bạch Mai).
        /// Backend khi ngân hàng báo thành công sẽ mở khóa giao dịch và xóa TRANS_REQ_ID,
        /// nên không thể chỉ dựa vào TRANS_REQ_ID. Thứ tự kiểm tra:
        /// 1. Giao dịch còn gắn yêu cầu hoàn tiền: chỉ cho khi yêu cầu đó FAILED/CANCEL.
        /// 2. Giao dịch đã mở khóa sang hình thức CK và có BANK_TRANSACTION_TIME: đã hoàn thành công, chặn.
        /// 3. Trong đợt điều trị còn yêu cầu hoàn tiền đã gửi ngân hàng (có BANK_REF_NO), chưa FAILED/CANCEL,
        ///    không gắn với giao dịch hoàn ứng nào khác, cùng số tiền: coi là của giao dịch này, chặn.
        /// Không gọi được API thì chặn (fail-closed) để không lọt lệnh chi trùng.
        /// </summary>
        private bool CheckAllowRefundByTransfer(ref string reason)
        {
            bool result = false;
            reason = "Đã có yêu cầu hoàn tiền chưa hoàn thành hoặc yêu cầu hoàn tiền đã được phê duyệt xin vui lòng kiểm tra lại";
            try
            {
                if (this.transaction == null) return true;

                // Lấy lại giao dịch từ server, tránh dữ liệu cũ do màn hình gọi truyền vào
                CommonParam param = new CommonParam();
                HisTransactionFilter tranFilter = new HisTransactionFilter();
                tranFilter.ID = this.transaction.ID;
                var transactions = new BackendAdapter(param).Get<List<HIS_TRANSACTION>>("api/HisTransaction/Get", ApiConsumers.MosConsumer, tranFilter, param);
                HIS_TRANSACTION tran = (transactions != null && transactions.Count > 0) ? transactions.First() : null;
                if (tran == null)
                {
                    Inventec.Common.Logging.LogSystem.Warn("CheckAllowRefundByTransfer: khong lay duoc giao dich tu server, chan hoan tien. TRANSACTION_ID=" + this.transaction.ID);
                    reason = "Không lấy được thông tin giao dịch từ máy chủ, không thể hoàn tiền ngân hàng. Vui lòng thử lại.";
                    return false;
                }

                // 1. Giao dịch còn gắn yêu cầu hoàn tiền
                if (tran.TRANS_REQ_ID.HasValue)
                {
                    HisTransReqFilter transReqFilter = new HisTransReqFilter();
                    transReqFilter.ID = tran.TRANS_REQ_ID.Value;
                    var transReqs = new BackendAdapter(param).Get<List<HIS_TRANS_REQ>>("api/HisTransReq/Get", ApiConsumers.MosConsumer, transReqFilter, param);
                    var transReq = transReqs != null ? transReqs.FirstOrDefault() : null;
                    result = transReq != null && IsFailedOrCancel(transReq);
                    Inventec.Common.Logging.LogSystem.Debug("CheckAllowRefundByTransfer[1]: TRANSACTION_ID=" + tran.ID + ", TRANS_REQ_ID=" + tran.TRANS_REQ_ID + ", TRANS_REQ_STT_ID=" + (transReq != null ? transReq.TRANS_REQ_STT_ID + "" : "NULL") + ", result=" + result);
                    return result;
                }

                // 2. Đã mở khóa sau khi ngân hàng báo thành công (backend chuyển CK + ghi thời gian ngân hàng)
                if (tran.PAY_FORM_ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__CK && tran.BANK_TRANSACTION_TIME.HasValue && tran.BANK_TRANSACTION_TIME.Value > 0)
                {
                    Inventec.Common.Logging.LogSystem.Debug("CheckAllowRefundByTransfer[2]: TRANSACTION_ID=" + tran.ID + " da hoan tien ngan hang thanh cong, BANK_TRANSACTION_TIME=" + tran.BANK_TRANSACTION_TIME);
                    reason = "Giao dịch này đã được hoàn tiền qua ngân hàng thành công, không được tạo lệnh chi lần nữa.";
                    return false;
                }

                // 3. Dò yêu cầu hoàn tiền của đợt điều trị chưa gắn với giao dịch nào khác
                long? treatmentId = tran.TREATMENT_ID.HasValue ? tran.TREATMENT_ID : (this.treatment != null ? (long?)this.treatment.ID : null);
                if (!treatmentId.HasValue)
                {
                    Inventec.Common.Logging.LogSystem.Warn("CheckAllowRefundByTransfer: giao dich khong co TREATMENT_ID, chan hoan tien. TRANSACTION_ID=" + tran.ID);
                    reason = "Giao dịch không có thông tin đợt điều trị, không thể kiểm tra hoàn tiền ngân hàng.";
                    return false;
                }

                HisTransReqFilter reqByTreatFilter = new HisTransReqFilter();
                reqByTreatFilter.TREATMENT_ID = treatmentId.Value;
                var reqsOfTreatment = new BackendAdapter(param).Get<List<HIS_TRANS_REQ>>("api/HisTransReq/Get", ApiConsumers.MosConsumer, reqByTreatFilter, param);

                HisTransactionFilter tranByTreatFilter = new HisTransactionFilter();
                tranByTreatFilter.TREATMENT_ID = treatmentId.Value;
                var transOfTreatment = new BackendAdapter(param).Get<List<HIS_TRANSACTION>>("api/HisTransaction/Get", ApiConsumers.MosConsumer, tranByTreatFilter, param);

                if (reqsOfTreatment == null || transOfTreatment == null)
                {
                    Inventec.Common.Logging.LogSystem.Warn("CheckAllowRefundByTransfer: khong lay duoc HIS_TRANS_REQ/HIS_TRANSACTION theo TREATMENT_ID=" + treatmentId + ", chan hoan tien.");
                    reason = "Không kiểm tra được lịch sử hoàn tiền ngân hàng của đợt điều trị, không thể hoàn tiền. Vui lòng thử lại.";
                    return false;
                }

                List<long> linkedReqIds = transOfTreatment
                    .Where(o => o.ID != tran.ID && o.TRANS_REQ_ID.HasValue)
                    .Select(o => o.TRANS_REQ_ID.Value).Distinct().ToList();

                var pendingReq = reqsOfTreatment.FirstOrDefault(r =>
                    r.IS_DELETE != 1
                    && (r.TRANS_REQ_TYPE == IMSys.DbConfig.HIS_RS.HIS_TRANS_REQ_TYPE.ID__BY_REPAY_BY_TRANSACTION
                        || r.TRANS_REQ_TYPE == IMSys.DbConfig.HIS_RS.HIS_TRANS_REQ_TYPE.ID__BY_REPAY_REQ)
                    && !IsFailedOrCancel(r)
                    && !String.IsNullOrWhiteSpace(r.BANK_REF_NO)
                    && !linkedReqIds.Contains(r.ID)
                    && Math.Abs(r.AMOUNT - tran.AMOUNT) < 1);

                if (pendingReq != null)
                {
                    Inventec.Common.Logging.LogSystem.Debug("CheckAllowRefundByTransfer[3]: TRANSACTION_ID=" + tran.ID + " bi chan boi HIS_TRANS_REQ.ID=" + pendingReq.ID + ", TRANS_REQ_STT_ID=" + pendingReq.TRANS_REQ_STT_ID + ", BANK_REF_NO=" + pendingReq.BANK_REF_NO + ", AMOUNT=" + pendingReq.AMOUNT);
                    if (pendingReq.TRANS_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_TRANS_REQ_STT.ID__FINISHED)
                        reason = "Giao dịch này đã được hoàn tiền qua ngân hàng thành công (mã tham chiếu " + pendingReq.BANK_REF_NO + "), không được tạo lệnh chi lần nữa.";
                    else
                        reason = "Đã có lệnh chi hoàn tiền gửi sang ngân hàng (mã tham chiếu " + pendingReq.BANK_REF_NO + ") đang chờ xử lý. Vui lòng kiểm tra trạng thái hoàn tiền ngân hàng trước.";
                    return false;
                }

                result = true;
                Inventec.Common.Logging.LogSystem.Debug("CheckAllowRefundByTransfer[3]: TRANSACTION_ID=" + tran.ID + ", TREATMENT_ID=" + treatmentId + ", reqs=" + reqsOfTreatment.Count + ", linked=" + linkedReqIds.Count + ", result=true");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                reason = "Lỗi khi kiểm tra lịch sử hoàn tiền ngân hàng, không thể hoàn tiền. Vui lòng thử lại.";
                result = false;
            }
            return result;
        }

        private static bool IsFailedOrCancel(HIS_TRANS_REQ transReq)
        {
            return transReq.TRANS_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_TRANS_REQ_STT.ID__FAILED
                || transReq.TRANS_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_TRANS_REQ_STT.ID__CANCEL;
        }

        private async Task FillInfoPatient(V_HIS_TREATMENT_FEE data)
        {
            try
            {
                FillDataToControlBySelectTreatment();
                if (data != null)
                {
                    lblName.Text = data.TDL_PATIENT_NAME;
                    lblDob.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.TDL_PATIENT_DOB);
                    lblGender.Text = data.TDL_PATIENT_GENDER_NAME;
                    lblAddress.Text = data.TDL_PATIENT_ADDRESS;
                }
                else
                {
                    lblName.Text = "";
                    lblDob.Text = "";
                    lblGender.Text = "";
                    lblAddress.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task FillDataToControlBySelectTreatment()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("step 1");
                UC.TotalPriceInfo.ADO.TotalPriceADO adoPrice = new UC.TotalPriceInfo.ADO.TotalPriceADO();
                if (this.currentTreatment != null)
                {
                    adoPrice.Discount = Inventec.Common.Number.Convert.NumberToString(this.currentTreatment.TOTAL_BILL_EXEMPTION ?? 0, ConfigApplications.NumberSeperator);
                    adoPrice.TotalDiscount = Inventec.Common.Number.Convert.NumberToString(this.currentTreatment.TOTAL_DISCOUNT ?? 0, ConfigApplications.NumberSeperator);
                    if (this.currentTreatment.TOTAL_PATIENT_PRICE.HasValue && this.currentTreatment.TOTAL_PATIENT_PRICE.Value > 0)
                    {
                        decimal discountRatio = 0;
                        if (this.currentTreatment.TOTAL_DISCOUNT.HasValue)
                        {
                            discountRatio = (this.currentTreatment.TOTAL_DISCOUNT.Value) / this.currentTreatment.TOTAL_PATIENT_PRICE.Value;
                        }
                    }
                    adoPrice.TotalBillFundPrice = Inventec.Common.Number.Convert.NumberToString(this.currentTreatment.TOTAL_BILL_FUND ?? 0, ConfigApplications.NumberSeperator);
                    adoPrice.TotalBillPrice = Inventec.Common.Number.Convert.NumberToString(this.currentTreatment.TOTAL_BILL_AMOUNT ?? 0, ConfigApplications.NumberSeperator);
                    adoPrice.TotalBillTransferPrice = Inventec.Common.Number.Convert.NumberToString(this.currentTreatment.TOTAL_BILL_TRANSFER_AMOUNT ?? 0, ConfigApplications.NumberSeperator);
                    //adoPrice.TotalDepositPrice = Inventec.Common.Number.Convert.NumberToString(this.currentTreatment.TOTAL_DEPOSIT_AMOUNT ?? 0, ConfigApplications.NumberSeperator);
                    adoPrice.TotalHeinPrice = Inventec.Common.Number.Convert.NumberToString(this.currentTreatment.TOTAL_HEIN_PRICE ?? 0, ConfigApplications.NumberSeperator);
                    adoPrice.TotalPatientPrice = Inventec.Common.Number.Convert.NumberToString(this.currentTreatment.TOTAL_PATIENT_PRICE ?? 0, ConfigApplications.NumberSeperator);
                    adoPrice.TotalPrice = Inventec.Common.Number.Convert.NumberToString(this.currentTreatment.TOTAL_PRICE ?? 0, ConfigApplications.NumberSeperator);
                    adoPrice.TotalRepayPrice = Inventec.Common.Number.Convert.NumberToString(this.currentTreatment.TOTAL_REPAY_AMOUNT ?? 0, ConfigApplications.NumberSeperator);
                    adoPrice.VirTotalPriceNoExpend = Inventec.Common.Number.Convert.NumberToString(this.currentTreatment.TOTAL_PRICE_EXPEND ?? 0, ConfigApplications.NumberSeperator);
                    adoPrice.TotalDebtAmount = Inventec.Common.Number.Convert.NumberToString(this.currentTreatment.TOTAL_DEBT_AMOUNT ?? 0, ConfigApplications.NumberSeperator);
                    adoPrice.TotalOtherCopaidPrice = Inventec.Common.Number.Convert.NumberToString(this.currentTreatment.TOTAL_OTHER_COPAID_PRICE ?? 0, ConfigApplications.NumberSeperator);

                    decimal totalReceive = ((this.currentTreatment.TOTAL_DEPOSIT_AMOUNT ?? 0) + (this.currentTreatment.TOTAL_BILL_AMOUNT ?? 0) - (this.currentTreatment.TOTAL_BILL_TRANSFER_AMOUNT ?? 0) - (this.currentTreatment.TOTAL_BILL_FUND ?? 0) - (this.currentTreatment.TOTAL_REPAY_AMOUNT ?? 0)) - (this.currentTreatment.TOTAL_BILL_EXEMPTION ?? 0) + (this.currentTreatment.LOCKING_AMOUNT ?? 0);

                    decimal totalReceiveMore = (this.currentTreatment.TOTAL_PATIENT_PRICE ?? 0) - totalReceive - (this.currentTreatment.TOTAL_BILL_FUND ?? 0);
                    adoPrice.TotalReceiveMorePrice = Inventec.Common.Number.Convert.NumberToString(totalReceiveMore - (this.currentTreatment.TOTAL_BILL_EXEMPTION ?? 0), ConfigApplications.NumberSeperator);
                    adoPrice.TotalReceivePrice = Inventec.Common.Number.Convert.NumberToString(totalReceive, ConfigApplications.NumberSeperator);
                    adoPrice.TotalOtherBillAmount = Inventec.Common.Number.Convert.NumberToString(this.currentTreatment.TOTAL_BILL_OTHER_AMOUNT ?? 0, ConfigApplications.NumberSeperator);
                    adoPrice.TotalOtherSourcePrice = Inventec.Common.Number.Convert.NumberToString(this.currentTreatment.TOTAL_OTHER_SOURCE_PRICE ?? 0, ConfigApplications.NumberSeperator);

                    adoPrice.TotalServiceDepositPrice = Inventec.Common.Number.Convert.NumberToString(this.currentTreatment.TOTAL_SERVICE_DEPOSIT_AMOUNT ?? 0, ConfigApplications.NumberSeperator);
                    adoPrice.TotalDepositPrice = Inventec.Common.Number.Convert.NumberToString((this.currentTreatment.TOTAL_DEPOSIT_AMOUNT ?? 0) - (this.currentTreatment.TOTAL_SERVICE_DEPOSIT_AMOUNT ?? 0), ConfigApplications.NumberSeperator);
                    adoPrice.LockingAmount = Inventec.Common.Number.Convert.NumberToString(this.currentTreatment.LOCKING_AMOUNT ?? 0, ConfigApplications.NumberSeperator);

                    if (transaction == null)
                    {
                        //số tiền bệnh nhân còn phải trả.
                        spAmount.Value = totalReceiveMore;
                    }
                    Inventec.Common.Logging.LogSystem.Debug("step 2");
                }
                else
                {
                    adoPrice.TotalBillFundPrice = Inventec.Common.Number.Convert.NumberToString(0);
                    adoPrice.TotalBillPrice = Inventec.Common.Number.Convert.NumberToString(0);
                    adoPrice.TotalBillTransferPrice = Inventec.Common.Number.Convert.NumberToString(0);
                    adoPrice.TotalDepositPrice = Inventec.Common.Number.Convert.NumberToString(0);
                    adoPrice.TotalServiceDepositPrice = Inventec.Common.Number.Convert.NumberToString(0);
                    adoPrice.TotalHeinPrice = Inventec.Common.Number.Convert.NumberToString(0);
                    adoPrice.TotalPatientPrice = Inventec.Common.Number.Convert.NumberToString(0);
                    adoPrice.TotalPrice = Inventec.Common.Number.Convert.NumberToString(0);
                    adoPrice.TotalRepayPrice = Inventec.Common.Number.Convert.NumberToString(0);
                    adoPrice.TotalReceiveMorePrice = Inventec.Common.Number.Convert.NumberToString(0);
                    adoPrice.TotalReceivePrice = Inventec.Common.Number.Convert.NumberToString(0);
                    adoPrice.TotalOtherBillAmount = Inventec.Common.Number.Convert.NumberToString(0);
                    adoPrice.TotalDiscount = Inventec.Common.Number.Convert.NumberToString(0);
                    adoPrice.VirTotalPriceNoExpend = Inventec.Common.Number.Convert.NumberToString(0);
                    adoPrice.TotalDebtAmount = Inventec.Common.Number.Convert.NumberToString(0);
                    adoPrice.TotalOtherSourcePrice = Inventec.Common.Number.Convert.NumberToString(0);
                    adoPrice.TotalOtherCopaidPrice = Inventec.Common.Number.Convert.NumberToString(0);
                    adoPrice.LockingAmount = Inventec.Common.Number.Convert.NumberToString(0);
                }
                Inventec.Common.Logging.LogSystem.Debug("step 12");
                totalPriceProcessor.SetValue(ucTotalPriceInfo, adoPrice);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitComboBankPayer()
        {
            try
            {
                cboBank.EditValue = null;
                List<HIS_BANK> data = BackendDataWorker.Get<HIS_BANK>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("BANK_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("BANK_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("BANK_NAME", "ID", columnInfos, false, 350);

                ControlEditorLoader.Load(cboBank, data, controlEditorADO);
                cboBank.Properties.ImmediatePopup = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidateForm()
        {
            try
            {
                ValidationSingleControl(spAmount);
                ValidationSingleControl(txtAccNum);
                ValidationSingleControl(cboBank);
                ValidationSingleControl(txtAccName);

                // PVCB: tài khoản nguồn do hệ thống ngân hàng tự xác định nên không bắt buộc nhập
                bool isSourceNumRequired = !string.Equals(bankCode, "PVCB", StringComparison.OrdinalIgnoreCase);
                if (isSourceNumRequired)
                {
                    ValidationSingleControl(txtSourceNum);
                }
                ValidationMaxLength(txtSourceNum, 2000, isSourceNumRequired);

                // Bỏ tô màu "bắt buộc" (Maroon) trên nhãn khi không bắt buộc nhập
                lciSourceNum.AppearanceItemCaption.ForeColor = isSourceNumRequired ? Color.Maroon : Color.Black;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationSingleControl(BaseEdit control)
        {
            try
            {
                ControlEditValidationRule validRule = new ControlEditValidationRule();
                validRule.editor = control;
                validRule.ErrorText = MessageUtil.GetMessage(LibraryMessage.Message.Enum.TruongDuLieuBatBuoc);
                validRule.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, validRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidationMaxLength(Control control, int? maxLength, bool required = false)
        {
            try
            {
                ControlMaxLengthValidationRule valid = new ControlMaxLengthValidationRule();
                valid.editor = control;
                valid.maxLength = maxLength;
                valid.IsRequired = required;
                valid.ErrorType = ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(control, valid);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                positionHandle = -1;
                if (!dxValidationProvider1.Validate())
                    return;

                // Kiểm tra lại ngay trước khi gửi lệnh chi: form có thể mở lâu, trạng thái đã đổi ở máy khác
                string denyReason = null;
                if (!CheckAllowRefundByTransfer(ref denyReason))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show(denyReason, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.Close();
                    return;
                }

                string data = BankHubProcess.GetAccessToken(this.bankCode);
                if (string.IsNullOrEmpty(data))
                {
                    MessageBox.Show("Không thể kết nối đến hệ thống ngân hàng, vui lòng thử lại sau.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                WaitingManager.Show();
                bool success = false;
                CommonParam param = new CommonParam();
                TransReqCreateSDO sdo = new TransReqCreateSDO();
                sdo.TreatmentId = this.currentTreatment.ID;
                sdo.TransReqType = currentTransaction != null ? IMSys.DbConfig.HIS_RS.HIS_TRANS_REQ_TYPE.ID__BY_REPAY_BY_TRANSACTION : IMSys.DbConfig.HIS_RS.HIS_TRANS_REQ_TYPE.ID__BY_REPAY_REQ;
                sdo.RequestRoomId = this.currentModule.RoomId;
                sdo.Amount = spAmount.Value;
                sdo.TransactionId = currentTransaction != null ? (long?)currentTransaction.ID : null;

                sdo.SourceAccount = txtSourceNum.Text.Trim();
                sdo.BenAccount = txtAccNum.Text.Trim();
                sdo.BenAccountName = txtAccName.Text.Trim();
                sdo.BenDescription = txtDescription.Text.Trim();
                sdo.BankCode = this.bankCode;

                long bankId = 0;
                if (cboBank.EditValue != null)
                    bankId = Convert.ToInt64(cboBank.EditValue);
                var currentBank = BackendDataWorker.Get<HIS_BANK>().FirstOrDefault(o => o.ID == bankId);
                sdo.BenBankCode = currentBank.BANK_CODE;

                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sdo), sdo));
                HIS_TRANS_REQ currentTransReq = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_TRANS_REQ>("api/HisTransReq/CreateRepay", ApiConsumers.MosConsumer, sdo, param);
                if (currentTransReq != null)
                {
                    success = true;
                    if (refresh != null)
                        refresh();
                }
                WaitingManager.Hide();

                #region Hien thi message thong bao
                MessageManager.Show(this, param, success);
                #endregion

                #region Neu phien lam viec bi mat, phan mem tu dong logout va tro ve trang login
                SessionManager.ProcessTokenLost(param);
                #endregion
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
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

        private void txtAccNum_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Search && !string.IsNullOrWhiteSpace(txtAccNum.Text))
                {
                    string data = BankHubProcess.GetAccessToken(this.bankCode);
                    if (string.IsNullOrEmpty(data))
                    {
                        MessageBox.Show("Không thể kết nối đến hệ thống ngân hàng, vui lòng thử lại sau.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    CommonParam param = new CommonParam();
                    BankAccountSDO bankAccountSDO = new BankAccountSDO();
                    bankAccountSDO.BankCode = this.bankCode;
                    bankAccountSDO.BenAccountNumber = txtAccNum.Text.Trim();
                    long bankId = 0;
                    if (cboBank.EditValue != null)
                        bankId = Convert.ToInt64(cboBank.EditValue);
                    var currentBank = BackendDataWorker.Get<HIS_BANK>().FirstOrDefault(o => o.ID == bankId);
                    bankAccountSDO.BenBankCode = currentBank.BANK_CODE;
                    BankAccountResultSDO bankAccountResultSDO = new BackendAdapter(param).Post<BankAccountResultSDO>("api/HisPatientBankAccount/CheckBankAccount", ApiConsumers.MosConsumer, bankAccountSDO, param);

                    bool CheckStatusTrue = bankAccountResultSDO != null && bankAccountResultSDO.Status;
                    if (CheckStatusTrue)
                    {
                        txtAccName.Text = bankAccountResultSDO.AccountName;

                        if (refresh != null)
                            refresh();
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

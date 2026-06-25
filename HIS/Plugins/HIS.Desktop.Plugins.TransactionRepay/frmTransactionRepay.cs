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
using DevExpress.DocumentView.Controls;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.TransactionRepay.Config;
using HIS.Desktop.Plugins.TransactionRepay.Validation;
using HIS.Desktop.Print;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TransactionRepay
{
    public partial class frmTransactionRepay : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module currentModule = null;

        long? treatmentId = null;
        long cashierRoomId;
        int positionHandleControl = -1;
        V_HIS_TREATMENT_FEE Treatment = null;
        V_HIS_TRANSACTION resultTransaction = null;
        List<V_HIS_ACCOUNT_BOOK> ListAccountBook = new List<V_HIS_ACCOUNT_BOOK>();
        V_HIS_PATIENT_TYPE_ALTER currentHisPatientTypeAlter;
        V_HIS_PATIENT_BANK_ACCOUNT currentPatientBankAccount = null;
        bool isNotLoadWhilechkAutoCloseStateInFirst = true;

        // 42727 - Ngữ cảnh "Nhập lại xuất bán" (mở từ Danh sách nhập)
        long? impMestId = null;
        decimal? autoAmount = null;
        string preferredRepayReasonCode = null;
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        string moduleLink = "HIS.Desktop.Plugins.TransactionRepay";
        DateTime dteCommonParam { get; set; }
        private static bool _isXemNgay = false;

        // 45677 - Hoan ung theo goi benh nhan
        HIS_PATIENT patient = null;
        HIS_PATIENT_PACKAGE patientPackage = null;
        // Chieu cao hang thong tin goi (1 row layout) - dung de thu nho form khi khong co goi
        const int PATIENT_PACKAGE_ROW_HEIGHT = 24;

        public frmTransactionRepay(Inventec.Desktop.Common.Modules.Module module, TransactionRepayADO data)
            : base(module)
        {
            // 42727 - Designer.cs để components = null nên BarManager(this.components) throw NRE khi form được tạo từ plugin ngoài (vd HisImportMestMedicine).
            // Khởi tạo container trước khi InitializeComponent() chạy.
            if (this.components == null)
                this.components = new System.ComponentModel.Container();

            InitializeComponent();

            // 42727 - Designer.cs CHỈ DECLARE các field sau nhưng KHÔNG INSTANTIATE.
            // Tự khởi tạo ở đây để tránh NRE khi runtime sử dụng:
            //   - dxValidationProvider1 (cho ValidControlXxx → SetValidationRule)
            //   - timerInitForm (cho Load → timerInitForm.Start)
            if (this.dxValidationProvider1 == null)
            {
                this.dxValidationProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider(this.components);
            }
            if (this.timerInitForm == null)
            {
                this.timerInitForm = new System.Windows.Forms.Timer(this.components);
                this.timerInitForm.Tick += new System.EventHandler(this.timerInitForm_Tick);
            }
            try
            {
                Base.ResourceLangManager.InitResourceLanguageManager();
                if (data != null)
                {
                    this.treatmentId = data.TreatmentId;
                    this.cashierRoomId = data.CashierRoomId;
                    this.Treatment = data.Treatment;
                    this.currentHisPatientTypeAlter = data.PatientTypeAlter;

                    // 42727 - Ngữ cảnh "Nhập lại xuất bán"
                    this.impMestId = data.ImpMestId;
                    this.autoAmount = data.AutoAmount;
                    this.preferredRepayReasonCode = data.RepayReasonCode;

                    // 45677 - Hoan ung theo goi benh nhan
                    this.patient = data.Patient;
                    this.patientPackage = data.PatientPackage;
                }
                this.currentModule = module;
                this.Size = new Size(this.ClientSize.Width, this.ClientSize.Height - barDockControlTop.Height+15);
                try
                {
                    string iconPath = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Warn(ex);
                }

                if (this.currentModule != null && !string.IsNullOrEmpty(currentModule.text))
                {
                    this.Text = Inventec.Common.Resource.Get.Value("frmTransactionRepay.Text", Base.ResourceLangManager.LanguageFrmTransactionRepay, LanguageManager.GetCulture());

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmTransactionRepay_Load(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("frmTransactionRepay_Load. 1");
                WaitingManager.Show();
                this.LoadKeyFrmLanguage();
                chkXemTruoc.Checked = _isXemNgay;
                // 45677 - Hien thi/an thong tin goi benh nhan (chay bat ke co treatmentId hay khong)
                // 42727 - Wrap try/catch riêng vì LoadPatientPackageInfo có thể throw MissingMethodException
                // tại JIT phase nếu MOS.EFMODEL.dll runtime không có property REGISTER_DATE (version mismatch).
                // Try/catch nội bộ trong LoadPatientPackageInfo KHÔNG bắt được lỗi JIT-phase này.
                try { this.LoadPatientPackageInfo(); }
                catch (Exception exP) { Inventec.Common.Logging.LogSystem.Warn(exP); }

                // 42727 - treatmentId > 0 thì load đầy đủ; nếu = 0 (luồng "Nhập lại xuất bán" type KHAC/BTL không có treatment) thì skip
                bool hasValidTreatment = this.treatmentId.HasValue && this.treatmentId.Value > 0;
                if (this.treatmentId.HasValue)
                {
                    try { this.ValidControl(); }
                    catch (Exception exV) { Inventec.Common.Logging.LogSystem.Warn(exV); }

                    if (hasValidTreatment)
                    {
                        try { this.LoadTreatmentAmount(); }
                        catch (Exception exL) { Inventec.Common.Logging.LogSystem.Warn(exL); }

                        try { this.GetPatientTypeAlter(this.treatmentId.Value); }
                        catch (Exception exP) { Inventec.Common.Logging.LogSystem.Warn(exP); }
                    }
                    else
                    {
                        // 42727 - Không có treatment thì vẫn cho phép user nhập số tiền tay
                        // Pre-fill từ autoAmount nếu có (từ phiếu nhập)
                        try
                        {
                            if (this.autoAmount.HasValue && this.autoAmount.Value > 0)
                                txtTotalAmount.Value = this.autoAmount.Value;
                        }
                        catch (Exception exA) { Inventec.Common.Logging.LogSystem.Warn(exA); }
                    }

                    try
                    {
                        this.txtTotalAmount.Focus();
                        this.txtTotalAmount.SelectAll();
                    }
                    catch (Exception exF) { Inventec.Common.Logging.LogSystem.Warn(exF); }

                    try
                    {
                        timerInitForm.Interval = 100;
                        timerInitForm.Enabled = true;
                        timerInitForm.Start();
                    }
                    catch (Exception exT) { Inventec.Common.Logging.LogSystem.Warn(exT); }

                    try { InitControlState(); }
                    catch (Exception exI) { Inventec.Common.Logging.LogSystem.Warn(exI); }
                }
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Debug("frmTransactionRepay_Load. 2");
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitControlState()
        {
            isNotLoadWhilechkAutoCloseStateInFirst = true;
            try
            {
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(moduleLink);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentControlStateRDO), currentControlStateRDO));
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
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

        private void timerInitForm_Tick(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("timerInitForm_Tick. 1");
                this.timerInitForm.Stop();

                this.LoadAccountBookToLocal(true);
                this.LoadDataToComboRepayReason();
                this.LoadDataToComboPayForm();
                HisConfigCFG.LoadConfig();
                this.ResetDefaultValueControl();
                this.InitCompensationControl();

                Inventec.Common.Logging.LogSystem.Debug("timerInitForm_Tick. 2");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// 2669 - Hien thi checkbox "Den bu" khi:
        ///   - Cau hinh MOS.HIS_TREATMENT.COMPENSATION_REFUND_ENABLE = 1
        ///   - VA ho so dieu tri duoc danh dau IS_COMPENSATION = 1
        /// Co IS_COMPENSATION nam tren HIS_TREATMENT (theo PTTK 2669 muc 1.1), KHONG nam tren V_HIS_TREATMENT_FEE,
        /// nen phai load HIS_TREATMENT theo treatmentId de doc co.
        /// Khi hien thi: mac dinh tich san. Nguoc lai: an checkbox.
        /// </summary>
        private void InitCompensationControl()
        {
            try
            {
                bool isEnableCompensation = false;

                if (HisConfigCFG.CompensationRefundEnable == "1"
                    && this.treatmentId.HasValue && this.treatmentId.Value > 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisTreatmentFilter treatmentFilter = new MOS.Filter.HisTreatmentFilter();
                    treatmentFilter.ID = this.treatmentId.Value;
                    var treatment = new Inventec.Common.Adapter.BackendAdapter(param)
                        .Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, treatmentFilter, param)
                        .FirstOrDefault();

                    isEnableCompensation = treatment != null && (treatment.IS_COMPENSATION ?? 0) == 1;
                }

                if (isEnableCompensation)
                {
                    this.lciCompensation.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    this.chkCompensation.Checked = true;
                }
                else
                {
                    this.chkCompensation.Checked = false;
                    this.lciCompensation.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ValidControl()
        {
            try
            {
                ValidControlAccountBook();
                ValidControlPayForm();
                ValidControlAmount();
                ValidControlTransactionTime();
                ValidControlRepayReason();
                ValidControlDescription();
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
                var ListPayForm = BackendDataWorker.Get<HIS_PAY_FORM>();
                if (ListPayForm != null && ListPayForm.Count > 0)
                {
                    var payFormDefault = ListPayForm.OrderBy(o => o.PAY_FORM_CODE).FirstOrDefault();
                    if (payFormDefault != null)
                    {
                        cboPayForm.EditValue = payFormDefault.ID;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        // nếu hồ sơ điều trị đã kết thúc thì mặc định chọn lý do hoàn ứng là "ra viện", cho phép người dùng chọn
        private void SetDefaultRepayReason()
        {
            try
            {
                HIS_REPAY_REASON repayReason = null;

                // 42727 - Ưu tiên lý do được truyền từ ngữ cảnh mở form (vd: "Nhập lại xuất bán" code = "07")
                if (!string.IsNullOrEmpty(this.preferredRepayReasonCode))
                {
                    repayReason = BackendDataWorker.Get<HIS_REPAY_REASON>()
                        .FirstOrDefault(o => o.REPAY_REASON_CODE == this.preferredRepayReasonCode);
                }

                if (repayReason == null)
                {
                    if (this.currentHisPatientTypeAlter != null && (this.currentHisPatientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNGOAITRU || this.currentHisPatientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__KHAM))
                    {
                        repayReason = BackendDataWorker.Get<HIS_REPAY_REASON>().FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_REPAY_REASON.ID__HOAN_NGT_CNT);
                    }
                    else if (this.Treatment != null && this.Treatment.IS_PAUSE == 1 && this.currentHisPatientTypeAlter != null && this.currentHisPatientTypeAlter.TREATMENT_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU)
                    {
                        repayReason = BackendDataWorker.Get<HIS_REPAY_REASON>().FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_REPAY_REASON.ID__HOAN_NT_RV);
                    }
                    else
                    {
                        repayReason = BackendDataWorker.Get<HIS_REPAY_REASON>().FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_REPAY_REASON.ID__HOAN_TUNT);
                    }
                }

                if (repayReason != null)
                {
                    cboRepayReason.EditValue = repayReason.ID;
                    txtRepayReason.Text = repayReason.REPAY_REASON_CODE;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void UpdateDictionaryNumOrderAccountBook(V_HIS_ACCOUNT_BOOK accountBook)
        {
            try
            {
                if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook != null && HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.Count > 0 && HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.ContainsKey(accountBook.ID))
                {
                    HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook[accountBook.ID] = spinTongTuDen.Value;
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

        //private void SetDataToDicNumOrderInAccountBook(V_HIS_ACCOUNT_BOOK accountBook, bool isFirstLoad = false)
        //{
        //    try
        //    {
        //        if (accountBook != null && accountBook.IS_NOT_GEN_TRANSACTION_ORDER == 1)//IMSys.DbConfig.HIS_RS.HIS_ACCOUNT_BOOK.IS_NOT_GEN_TRANSACTION_ORDER__TRUE
        //        {
        //            if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook == null || HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.Count == 0 || (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook != null && HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.Count > 0 && !HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.ContainsKey(accountBook.ID)))
        //            {
        //                if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook == null)
        //                {
        //                    HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook = new Dictionary<long, decimal>();
        //                }

        //                layoutTongTuDen.Enabled = true;

        //                if (!isFirstLoad)
        //                {
        //                    CommonParam param = new CommonParam();
        //                    MOS.Filter.HisAccountBookViewFilter hisAccountBookViewFilter = new HisAccountBookViewFilter();
        //                    hisAccountBookViewFilter.ID = accountBook.ID;
        //                    var accountBooks = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_ACCOUNT_BOOK>>(HisRequestUriStore.HIS_ACCOUNT_BOOK_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, hisAccountBookViewFilter, param);
        //                    if (accountBooks != null && accountBooks.Count > 0)
        //                    {
        //                        accountBook = accountBooks.FirstOrDefault();
        //                    }
        //                }

        //                if (accountBook != null)
        //                {
        //                    decimal num = 0;
        //                    if ((accountBook.CURRENT_NUM_ORDER ?? 0) > 0)
        //                    {
        //                        num = (accountBook.CURRENT_NUM_ORDER ?? 0);
        //                    }
        //                    else
        //                    {
        //                        num = (decimal)accountBook.FROM_NUM_ORDER - 1;
        //                    }
        //                    HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.Add(accountBook.ID, num);
        //                    spinTongTuDen.Value = (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook[accountBook.ID]) + 1;
        //                }
        //            }
        //            else
        //            {
        //                layoutTongTuDen.Enabled = true;
        //                spinTongTuDen.Value = (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook[accountBook.ID]) + 1;
        //            }
        //        }
        //        else
        //        {
        //            spinTongTuDen.Value = (accountBook.CURRENT_NUM_ORDER ?? 0) + 1;
        //            layoutTongTuDen.Enabled = false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        private void ValidControlAccountBook()
        {
            try
            {
                AccountBookValidationRule accountBookRule = new AccountBookValidationRule();
                accountBookRule.txtAccountBookCode = txtAccountBookCode;
                accountBookRule.cboAccountBook = cboAccountBook;
                dxValidationProvider1.SetValidationRule(txtAccountBookCode, accountBookRule);
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

        private void ValidControlRepayReason()
        {
            try
            {
                RepayReasonValidationRule repayReasonRule = new RepayReasonValidationRule();
                repayReasonRule.txtRepayReasonCode = txtRepayReason;
                repayReasonRule.cboRepayReason = cboRepayReason;
                repayReasonRule.isRequred = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<long>("HIS.Desktop.Plugins.Repay.Is_Required_Repay_Reason");
                dxValidationProvider1.SetValidationRule(txtRepayReason, repayReasonRule);
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

        private void ValidControlAmount()
        {
            try
            {
                AmountValidationRule amountRule = new AmountValidationRule();
                amountRule.txtTotalAmount = txtTotalAmount;
                dxValidationProvider1.SetValidationRule(txtTotalAmount, amountRule);
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
                TransactionTimeValidationRule amountRule = new TransactionTimeValidationRule();
                amountRule.dtTransactionTime = dtTransactionTime;
                dxValidationProvider1.SetValidationRule(dtTransactionTime, amountRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void GetPatientTypeAlter(long treatmentId)
        {
            CommonParam param = new CommonParam();
            try
            {
                this.currentHisPatientTypeAlter = new Inventec.Common.Adapter.BackendAdapter(param).Get<V_HIS_PATIENT_TYPE_ALTER>("api/HisPatientTypeAlter/GetLastByTreatmentId", ApiConsumer.ApiConsumers.MosConsumer, treatmentId, param);
                dteCommonParam = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(param.Now) ?? DateTime.Now;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadTreatmentAmount()
        {
            try
            {
                // 45677 - Khi hoan ung theo goi: so tien hoan mac dinh = TOTAL_PAID - TOTAL_REFUNDED - TOTAL_USED
                if (this.patientPackage != null)
                {
                    txtTotalAmount.Value = this.patientPackage.TOTAL_PAID - this.patientPackage.TOTAL_REFUNDED - this.patientPackage.TOTAL_USED;
                    return;
                }

                // 42727 - Khi mở từ luồng "Nhập lại xuất bán" số tiền đã được tính sẵn từ phiếu xuất bán gốc
                if (this.impMestId.HasValue && this.autoAmount.HasValue)
                {
                    txtTotalAmount.Value = this.autoAmount.Value;
                    return;
                }

                if (this.Treatment == null || this.Treatment.ID == 0)
                {
                    if (this.treatmentId.HasValue)
                    {
                        HisTreatmentFeeViewFilter feeFilter = new HisTreatmentFeeViewFilter();
                        feeFilter.ID = this.treatmentId.Value;
                        var treatmentFees = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_TREATMENT_FEE>>("api/HisTreatment/GetFeeView", ApiConsumers.MosConsumer, feeFilter, null);
                        if (treatmentFees == null || treatmentFees.Count == 0)
                        {
                            Inventec.Common.Logging.LogSystem.Warn("Khong lay duoc treatmentFee theo TreatmentId: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => treatmentId), treatmentId));
                            return;
                        }
                        this.Treatment = treatmentFees.First();
                    }
                }

                if (this.Treatment != null)
                {
                    // 2669 - Loai gia tri hoan den bu (compensation) khoi tong hoan ung de khong lam sai so tien hoan ung mac dinh
                    decimal totalReceive = ((this.Treatment.TOTAL_DEPOSIT_AMOUNT ?? 0) + (this.Treatment.TOTAL_BILL_AMOUNT ?? 0) - (this.Treatment.TOTAL_BILL_TRANSFER_AMOUNT ?? 0) - (this.Treatment.TOTAL_BILL_FUND ?? 0) - ((this.Treatment.TOTAL_REPAY_AMOUNT ?? 0) - (this.Treatment.TOTAL_COMPENSATION_REPAY ?? 0))) - (this.Treatment.TOTAL_BILL_EXEMPTION ?? 0);

                    decimal totalReceiveMore = (this.Treatment.TOTAL_PATIENT_PRICE ?? 0) - totalReceive - (this.Treatment.TOTAL_BILL_FUND ?? 0) - (this.Treatment.TOTAL_BILL_EXEMPTION ?? 0);
                    txtTotalAmount.Value = -totalReceiveMore;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        // 45677 - Hien thi thong tin goi benh nhan (ten goi, ngay dang ky, tien da dong, tien da dung)
        // Khong co goi -> an dong va thu nho form de giu nguyen giao dien hoan ung cu.
        private void LoadPatientPackageInfo()
        {
            try
            {
                if (this.patientPackage == null)
                {
                    this.lciPackageName.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    this.lciRegisterDate.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    this.lciTotalPaid.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    this.lciTotalUsed.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    this.Height -= PATIENT_PACKAGE_ROW_HEIGHT;
                    return;
                }

                // 42727 - REGISTER_DATE là kiểu long (yyyyMMddHHmmss), không phải DateTime
                string registerDate = this.patientPackage.REGISTER_DATE > 0
                    ? Inventec.Common.DateTime.Convert.TimeNumberToDateString(this.patientPackage.REGISTER_DATE)
                    : "";

                this.lblPackageName.Text = this.patientPackage.PACKAGE_NAME;
                this.lblRegisterDate.Text = registerDate;
                this.lblTotalPaid.Text = Inventec.Common.Number.Convert.NumberToStringRoundAuto(this.patientPackage.TOTAL_PAID, ConfigApplications.NumberSeperator);
                this.lblTotalUsed.Text = Inventec.Common.Number.Convert.NumberToStringRoundAuto(this.patientPackage.TOTAL_USED, ConfigApplications.NumberSeperator);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task LoadAccountBookToLocal(bool isFirstLoad = false)
        {
            try
            {
                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                this.ListAccountBook = new List<V_HIS_ACCOUNT_BOOK>();

                HisAccountBookViewFilter acFilter = new HisAccountBookViewFilter();
                acFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                // 42727 - Chỉ filter CASHIER_ROOM_ID khi > 0 (luồng "Nhập lại xuất bán" có thể không có cashier room)
                if (this.cashierRoomId > 0)
                    acFilter.CASHIER_ROOM_ID = this.cashierRoomId;
                acFilter.LOGINNAME = loginName;//Kiểm tra sổ còn hay k
                acFilter.FOR_REPAY = true;
                acFilter.IS_OUT_OF_BILL = false;
                acFilter.ORDER_DIRECTION = "DESC";
                acFilter.ORDER_FIELD = "ID";

                this.ListAccountBook = await new BackendAdapter(new CommonParam()).GetAsync<List<V_HIS_ACCOUNT_BOOK>>("api/HisAccountBook/GetView", ApiConsumer.ApiConsumers.MosConsumer, acFilter, null);
                this.ListAccountBook = this.ListAccountBook.Where(o => o.WORKING_SHIFT_ID == null || o.WORKING_SHIFT_ID == (HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkInfoSDO.WorkingShiftId ?? 0)).ToList();
                this.LoadDataToComboAccountBook();
                this.SetDefaultAccountBook(isFirstLoad);
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

        private async Task LoadDataToComboRepayReason()
        {
            try
            {
                List<HIS_REPAY_REASON> repayReasons = null;
                if (BackendDataWorker.IsExistsKey<HIS_REPAY_REASON>())
                {
                    repayReasons = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_REPAY_REASON>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    repayReasons = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.HIS_REPAY_REASON>>("api/HisRepayReason/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                    if (repayReasons != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_REPAY_REASON), repayReasons, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }
                cboRepayReason.Properties.DataSource = repayReasons;
                if (cboRepayReason.Properties.Columns.Count == 0)
                {
                    cboRepayReason.Properties.DisplayMember = "REPAY_REASON_NAME";
                    cboRepayReason.Properties.ValueMember = "ID";
                    cboRepayReason.Properties.ForceInitialize();
                    cboRepayReason.Properties.Columns.Clear();
                    cboRepayReason.Properties.Columns.Add(new LookUpColumnInfo("REPAY_REASON_CODE", "", 50));
                    cboRepayReason.Properties.Columns.Add(new LookUpColumnInfo("REPAY_REASON_NAME", "", 150));
                    cboRepayReason.Properties.ShowHeader = false;
                    cboRepayReason.Properties.ImmediatePopup = true;
                    cboRepayReason.Properties.DropDownRows = 10;
                    cboRepayReason.Properties.PopupWidth = 200;
                }

                SetDefaultRepayReason();
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
                List<MOS.EFMODEL.DataModels.HIS_PAY_FORM> listPayForm = null;
                if (BackendDataWorker.IsExistsKey<HIS_PAY_FORM>())
                {
                    listPayForm = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PAY_FORM>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    listPayForm = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.HIS_PAY_FORM>>("api/HisPayForm/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                    if (listPayForm != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_PAY_FORM), listPayForm, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                cboPayForm.Properties.DataSource = listPayForm.Where(o => o.IS_ACTIVE == 1).ToList();
                cboPayForm.Properties.DisplayMember = "PAY_FORM_NAME";
                cboPayForm.Properties.ValueMember = "ID";
                cboPayForm.Properties.ForceInitialize();
                cboPayForm.Properties.Columns.Clear();
                cboPayForm.Properties.Columns.Add(new LookUpColumnInfo("PAY_FORM_CODE", "", 50));
                cboPayForm.Properties.Columns.Add(new LookUpColumnInfo("PAY_FORM_NAME", "", 150));
                cboPayForm.Properties.ShowHeader = false;
                cboPayForm.Properties.ImmediatePopup = true;
                cboPayForm.Properties.DropDownRows = 10;
                cboPayForm.Properties.PopupWidth = 200;

                var payFormDefault = listPayForm != null ? listPayForm.OrderBy(o => o.PAY_FORM_CODE).FirstOrDefault() : null;
                if (payFormDefault != null)
                {
                    cboPayForm.EditValue = payFormDefault.ID;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ResetDefaultValueControl()
        {
            try
            {
                this.resultTransaction = null;
                txtDescription.Text = "";
                txtCashierUsername.Text = "";
                spinTongTuDen.Value = 0;
                spinTongTuDen.Text = "";
                //txtTransactionCode.Text = "";
                dtCreateTime.EditValue = null;
                cboRepayReason.EditValue = null;
                txtRepayReason.Text = "";
                btnPrint.Enabled = false;
                btnSave.Enabled = true;
                txtNguoiThuHuong.Text = "";
                currentPatientBankAccount = null;
                //CommonParam paramCommon = new CommonParam();
                //dteCommonParam = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(paramCommon.Now) ?? DateTime.Now;
                if (HisConfigCFG.ShowServerTimeByDefault == "1")
                {
                    dtTransactionTime.DateTime = dteCommonParam;
                }
                else
                {
                    dtTransactionTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.Now() ?? 0);
                }
                ValidControlRepayReason();
                HisConfigCFG.LoadConfig();
                // set mau cho text ly do hoan ung
                if (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<long>("HIS.Desktop.Plugins.Repay.Is_Required_Repay_Reason") == 1)
                {
                    lciRepayReasonCode.AppearanceItemCaption.ForeColor = Color.Maroon;
                }
                else
                {
                    lciRepayReasonCode.AppearanceItemCaption.ForeColor = Color.Black;
                }

                if (HisConfigCFG.IsEditTransactionTimeCFG != null && HisConfigCFG.IsEditTransactionTimeCFG.Equals("1"))
                {
                    lciTransactionTime.Enabled = true;
                }
                else
                {
                    lciTransactionTime.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultAccountBook(bool isFirstLoad = false)
        {
            try
            {
                V_HIS_ACCOUNT_BOOK data = null;
                //chọn mặc định sổ nếu có sổ tương ứng
                if (GlobalVariables.LastAccountBook != null && GlobalVariables.LastAccountBook.Count > 0)
                {
                    var lstBook = ListAccountBook.Where(o => GlobalVariables.LastAccountBook.Select(s => s.ID).Contains(o.ID)).ToList();
                    if (lstBook != null && lstBook.Count > 0)
                    {
                        data = lstBook.OrderByDescending(o => o.ID).First();
                    }
                }

                if (data == null) data = ListAccountBook.FirstOrDefault();
                if (data != null)
                {
                    txtAccountBookCode.Text = data.ACCOUNT_BOOK_CODE;
                    cboAccountBook.EditValue = data.ID;
                    //SetDataToDicNumOrderInAccountBook(data, isFirstLoad);
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

        private void txtTotalAmount_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (dtTransactionTime.Enabled)
                    {
                        dtTransactionTime.Focus();
                        dtTransactionTime.ShowPopup();
                    }
                    else
                    {
                        txtDescription.Focus();
                        txtDescription.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtDescription_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtAccountBookCode.Focus();
                    txtAccountBookCode.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void txtAccountBookCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool valid = false;
                    if (!String.IsNullOrEmpty(txtAccountBookCode.Text))
                    {
                        var listData = ListAccountBook.Where(o => o.ACCOUNT_BOOK_CODE.Contains(txtAccountBookCode.Text)).ToList();
                        if (listData != null && listData.Count == 1)
                        {
                            valid = true;
                            txtAccountBookCode.Text = listData.First().ACCOUNT_BOOK_CODE;
                            cboAccountBook.EditValue = listData.First().ID;
                            cboPayForm.Focus();
                        }
                        else
                        {
                            spinTongTuDen.Text = "";
                        }
                    }
                    if (!valid)
                    {
                        cboAccountBook.Focus();
                        cboAccountBook.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboAccountBook_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
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
                            txtAccountBookCode.Text = account.ACCOUNT_BOOK_CODE;
                        }
                    }
                    else
                    {
                        spinTongTuDen.Text = "";
                    }
                    cboPayForm.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPayForm_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
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

        private void cboPayForm_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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

        private void btnSavePrint_Click(object sender, EventArgs e)
        {
            try
            {
                positionHandleControl = -1;
                if (!btnSave.Enabled || !dxValidationProvider1.Validate() || this.treatmentId == null)
                    return;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                this.SaveRepay(param, ref success);
                if (success)
                {
                    btnSave.Enabled = false;
                    btnPrint.Enabled = true;
                    btnSavePrint.Enabled = false;
                }
                WaitingManager.Hide();
                if (!success)
                {
                    MessageManager.Show(param, success);
                }
                else
                {
                    if (chkXemTruoc.Checked)
                    {
                        this.InPhieuHoanUng(false);
                    }
                    else
                    {
                        this.InPhieuHoanUng(true);
                    }
                }
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                positionHandleControl = -1;
                if (!btnSave.Enabled || !dxValidationProvider1.Validate() || this.treatmentId == null)
                    return;
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                bool success = false;
                this.SaveRepay(param, ref success);
                if (success)
                {
                    btnSave.Enabled = false;
                    btnPrint.Enabled = true;
                    btnSavePrint.Enabled = false;
                }
                WaitingManager.Hide();
                if (success)
                {
                    MessageManager.Show(this, param, success);
                    if (chkAutoClose.Checked)
                    {
                        this.Close();
                    }
                }
                else
                {
                    MessageManager.Show(param, success);
                }
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnPrint.Enabled || this.resultTransaction == null)
                    return;
                if (chkXemTruoc.Checked)
                {
                    this.InPhieuHoanUng(false);
                }    
                else
                {
                    this.InPhieuHoanUng(true);
                }    
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnNew.Enabled)
                    return;
                WaitingManager.Show();
                LoadAccountBookToLocal();
                ResetDefaultValueControl();
                SetDefaultPayFormForUser();
                SetDefaultRepayReason();
                txtTotalAmount.Value = 0;
                spinTransferAmount.Value = 0;
                txtAccountBookCode.Focus();
                txtAccountBookCode.SelectAll();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
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

        private void bbtnRCPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnPrint_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnNew_Click(null, null);
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

        bool CheckPayformCard(MOS.EFMODEL.DataModels.HIS_PAY_FORM payForm)
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

        // gọi sang WCF hoàn ứng qua thẻ
        CARD.WCF.DCO.WcfRefundDCO RepayCard(ref CARD.WCF.DCO.WcfRefundDCO RepayDCO)
        {
            CARD.WCF.DCO.WcfRefundDCO result = null;
            CommonParam param = new CommonParam();
            try
            {
                // gọi api HisCard/Get để lấy về serviceCodes

                MOS.Filter.HisCardFilter cardFilter = new HisCardFilter();
                cardFilter.PATIENT_ID = this.Treatment.PATIENT_ID;
                var HisCards = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_CARD>>("api/HisCard/Get", ApiConsumer.ApiConsumers.MosConsumer, cardFilter, param);
                if (HisCards != null && HisCards.Count > 0)
                {
                    RepayDCO.ServiceCodes = HisCards.Select(o => o.SERVICE_CODE).ToArray();

                }

                CARD.WCF.Client.TransactionClient.TransactionClientManager transactionClientManager = new CARD.WCF.Client.TransactionClient.TransactionClientManager();

                result = transactionClientManager.Refund(RepayDCO);
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        // gọi WCF (phần mềm thẻ) để hủy giao dịch 
        CARD.WCF.DCO.WcfVoidDCO VoidCard(ref CARD.WCF.DCO.WcfVoidDCO VoidDCO)
        {
            CARD.WCF.DCO.WcfVoidDCO result = null;
            CommonParam param = new CommonParam();
            try
            {
                CARD.WCF.Client.TransactionClient.TransactionClientManager transactionClientManager = new CARD.WCF.Client.TransactionClient.TransactionClientManager();
                result = transactionClientManager.Void(VoidDCO);
            }
            catch (Exception ex)
            {
                result = null;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void FormatSpint(DevExpress.XtraEditors.TextEdit textEdit)
        {
            try
            {
                int munberSeperator = HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator;
                int munbershowDecimal = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<int>("HIS.Desktop.Plugins.ShowDecimalOption");
                string Format = "";
                if (munbershowDecimal == 0)
                {
                    if (munberSeperator == 0)
                    {
                        Format = "#,##0";
                    }
                    else
                    {
                        Format = "#,##0.";
                        for (int i = 0; i < munberSeperator; i++)
                        {
                            Format += "0";
                        }
                    }

                }
                else if (munbershowDecimal == 1)
                {
                    if (textEdit.EditValue != null)
                    {
                        if (Inventec.Common.TypeConvert.Parse.ToDecimal(textEdit.EditValue.ToString() ?? "") % 1 > 0)
                        {
                            if (munberSeperator == 0)
                            {
                                Format = "#,##0";
                            }
                            else
                            {
                                Format = "#,##0.";
                                for (int i = 0; i < munberSeperator; i++)
                                {
                                    Format += "0";
                                }
                            }
                        }
                        else
                        {
                            Format = "#,##0";
                        }
                    }
                }
                textEdit.Properties.DisplayFormat.FormatString = Format;
                textEdit.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
                textEdit.Properties.EditFormat.FormatString = Format;
                textEdit.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SaveRepay(CommonParam param, ref bool success)
        {
            try
            {
                if (cboAccountBook.EditValue == null || cboPayForm.EditValue == null || txtTotalAmount.Value <= 0)
                {
                    param.Messages.Add(Base.ResourceMessageLang.ThieuTruongDuLieuBatBuoc);
                    return;
                }

                CARD.WCF.DCO.WcfRefundDCO repayDCO = null;

                var payForm = BackendDataWorker.Get<HIS_PAY_FORM>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboPayForm.EditValue));

                if (payForm == null)
                    return;

                HisTransactionRepaySDO data = new HisTransactionRepaySDO();
                //HisRepaySDO data = new HisRepaySDO();
                //data.Repay = new HIS_REPAY();
                //Review List<long> DereDetailIds
                if (this.currentModule != null)
                {
                    data.RequestRoomId = this.currentModule.RoomId;
                }
                data.Transaction = new HIS_TRANSACTION();

                var accountBook = ListAccountBook.FirstOrDefault(o => o.ID == Convert.ToInt64(cboAccountBook.EditValue));
                if (accountBook != null)
                {
                    data.Transaction.ACCOUNT_BOOK_ID = accountBook.ID;
                }
                if (accountBook != null && accountBook.IS_NOT_GEN_TRANSACTION_ORDER == 1)
                {
                    data.Transaction.NUM_ORDER = (long)(spinTongTuDen.Value);
                }

                if (cboRepayReason.EditValue != null)
                {
                    var repayReason = BackendDataWorker.Get<HIS_REPAY_REASON>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboRepayReason.EditValue));
                    data.Transaction.REPAY_REASON_ID = repayReason.ID;
                }

                data.Transaction.CASHIER_ROOM_ID = this.cashierRoomId;
                data.Transaction.AMOUNT = txtTotalAmount.Value;

                if(currentPatientBankAccount!=null && currentPatientBankAccount.ID > 0)
                {
                    data.Transaction.PATIENT_BANK_ACCOUNT_ID = currentPatientBankAccount.ID;
                }

                if (payForm != null)
                {
                    data.Transaction.PAY_FORM_ID = payForm.ID;
                    if (payForm.ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TMCK && spinTransferAmount.EditValue != null)
                    {

                        if (spinTransferAmount.Value > data.Transaction.AMOUNT)
                        {
                            param.Messages.Add(String.Format(Base.ResourceMessageLang.SoTienChuyenKhoanLonHonSoTienHoanUng, Inventec.Common.Number.Convert.NumberToStringRoundAuto(spinTransferAmount.Value, ConfigApplications.NumberSeperator), Inventec.Common.Number.Convert.NumberToStringRoundAuto(data.Transaction.AMOUNT, ConfigApplications.NumberSeperator)));
                            return;
                        }
                        else
                        {
                            data.Transaction.TRANSFER_AMOUNT = Inventec.Common.TypeConvert.Parse.ToDecimal(spinTransferAmount.Text);
                        }
                    }
                    else if (payForm.ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TMQT && spinTransferAmount.EditValue != null)
                    {

                        if (spinTransferAmount.Value > data.Transaction.AMOUNT)
                        {
                            param.Messages.Add(String.Format(Base.ResourceMessageLang.SoTienQuetTheLonHonSoTienHoanUng, Inventec.Common.Number.Convert.NumberToStringRoundAuto(spinTransferAmount.Value, ConfigApplications.NumberSeperator), Inventec.Common.Number.Convert.NumberToStringRoundAuto(data.Transaction.AMOUNT, ConfigApplications.NumberSeperator)));
                            return;
                        }
                        else
                        {
                            data.Transaction.SWIPE_AMOUNT = Inventec.Common.TypeConvert.Parse.ToDecimal(spinTransferAmount.Text);
                        }
                    }
                }

                if (dtTransactionTime.EditValue != null && dtTransactionTime.DateTime != DateTime.MinValue)
                    data.Transaction.TRANSACTION_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtTransactionTime.EditValue).ToString("yyyyMMddHHmm") + "00");

                // 42727 - Chỉ gán TREATMENT_ID khi > 0. Nếu = 0 (luồng "Nhập lại xuất bán" loại BTL/KHAC không có treatment)
                // → để null để BE biết bỏ qua treatment lookup (tránh NRE tại HisTreatmentGet.GetUnpaid)

                if (this.treatmentId.HasValue && this.treatmentId.Value > 0)
                    data.Transaction.TREATMENT_ID = this.treatmentId.Value;
                data.Transaction.DESCRIPTION = txtDescription.Text;

                // 45677 - Luu thong tin goi benh nhan vao giao dich hoan ung
                if (this.patientPackage != null && this.patientPackage.ID > 0)
                {
                    data.Transaction.PATIENT_PACKAGE_ID = this.patientPackage.ID;
                }

                // 42727 - Truyền mã phiếu nhập để backend tự ghi REPAY_ID ngược lại HIS_IMP_MEST
                if (this.impMestId.HasValue && this.impMestId.Value > 0)
                {
                    data.IMP_MEST_ID = this.impMestId.Value;
                }

                // 2669 - Neu checkbox "Den bu" dang hien thi va duoc tich -> danh dau giao dich hoan ung la hoan den bu
                if (this.lciCompensation.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always
                    && this.chkCompensation.Checked)
                {
                    data.Transaction.IS_COMPENSATION = 1;
                }

                Inventec.Common.Logging.LogSystem.Warn("Du lieu dau vao khi goi api HisTransaction/CreateRepay " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));

                // nếu hình thức thanh toán qua thẻ thì gọi WCF tab thẻ (POS)
                if (payForm.ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__THE)
                {
                    //kiểm tra trước khi thanh toán nếu là giao dịch qua thẻ
                    var check = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>(UriStores.HIS_TRANSACTION_CHECK_REPAY, ApiConsumers.MosConsumer, data, param);

                    if (!check)
                        return;

                    CARD.WCF.DCO.WcfRefundDCO RepayDCO = new CARD.WCF.DCO.WcfRefundDCO();
                    RepayDCO.Amount = Inventec.Common.TypeConvert.Parse.ToDecimal(txtTotalAmount.Text.Trim());
                    //DepositDCO.PinCode = this.txtPin.Text.Trim();
                    repayDCO = RepayCard(ref RepayDCO);
                    // nếu gọi sang POS trả về false thì kết thúc
                    if (repayDCO == null || repayDCO.ResultCode == null || !repayDCO.ResultCode.Equals("00"))
                    {
                        success = false;
                        Inventec.Common.Logging.LogSystem.Info("repayDCO: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => repayDCO), repayDCO));
                        MappingErrorTHE mappingErrorTHE = new Config.MappingErrorTHE();
                        //param.Messages.Add(ResourceMessageLang.HoanUngQuaTheThatBai);
                        if (repayDCO != null
                            && !String.IsNullOrWhiteSpace(repayDCO.ResultCode)
                            && mappingErrorTHE.dicMapping != null
                            && mappingErrorTHE.dicMapping.Count > 0
                            && mappingErrorTHE.dicMapping.ContainsKey(repayDCO.ResultCode))
                        {
                            param.Messages.Add(mappingErrorTHE.dicMapping[repayDCO.ResultCode]);
                        }
                        else if (repayDCO != null && String.IsNullOrWhiteSpace(repayDCO.ResultCode))
                        {
                            param.Messages.Add("Kiểm tra lại phần mềm kết nối thiết bị");
                        }
                        else if (repayDCO != null
                            && !String.IsNullOrWhiteSpace(repayDCO.ResultCode)
                            && mappingErrorTHE.dicMapping != null
                            && mappingErrorTHE.dicMapping.Count > 0
                            && !mappingErrorTHE.dicMapping.ContainsKey(repayDCO.ResultCode)
                            )
                        {
                            param.Messages.Add("Kiểm tra lại phần mềm kết nối thiết bị");
                        }
                        return;
                    }
                    else
                    {
                        //nếu giao dịch thanh toán qua thẻ thì gửi lên TIG_TRANSACTION_CODE
                        data.Transaction.TIG_TRANSACTION_CODE = repayDCO.TransactionCode;
                        data.Transaction.TIG_TRANSACTION_TIME = repayDCO.TransactionTime;
                    }
                }

                var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<V_HIS_TRANSACTION>(UriStores.HIS_TRANSACTION_CREATE_REPAY, ApiConsumers.MosConsumer, data, param);
                if (rs != null)
                {
                    success = true;
                    AddLastAccountToLocal();
                    this.resultTransaction = rs;
                    SetValueContronlDepositSuccess();
                    UpdateDictionaryNumOrderAccountBook(accountBook);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                success = false;
            }
        }

        private void ProcessAddLastAccount()
        {
            System.Threading.Thread add = new System.Threading.Thread(AddLastAccountToLocal);
            try
            {
                add.Start();
            }
            catch (Exception ex)
            {
                add.Abort();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void AddLastAccountToLocal()
        {
            try
            {
                if (GlobalVariables.LastAccountBook == null) GlobalVariables.LastAccountBook = new List<V_HIS_ACCOUNT_BOOK>();

                var accountBook = ListAccountBook.FirstOrDefault(o => o.ID == Convert.ToInt64(cboAccountBook.EditValue));
                if (accountBook != null)
                {
                    //sổ chưa có sẽ add thêm vào
                    //xóa bỏ sổ cũ cùng loại
                    if (!GlobalVariables.LastAccountBook.Exists(o => o.ID == accountBook.ID))
                    {
                        var lstSameType = GlobalVariables.LastAccountBook.Where(o => o.IS_FOR_REPAY == accountBook.IS_FOR_REPAY && o.ID != accountBook.ID).ToList();
                        if (lstSameType != null && lstSameType.Count > 0)
                        {
                            foreach (var item in lstSameType)
                            {
                                GlobalVariables.LastAccountBook.Remove(item);
                            }
                        }
                        GlobalVariables.LastAccountBook.Add(accountBook);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetValueContronlDepositSuccess()
        {
            try
            {
                if (this.resultTransaction != null)
                {
                    txtTotalAmount.Value = this.resultTransaction.AMOUNT;
                    txtTransactionCode.Text = this.resultTransaction.TRANSACTION_CODE;
                    spinTongTuDen.Value = Inventec.Common.TypeConvert.Parse.ToDecimal(this.resultTransaction.NUM_ORDER.ToString());//Review
                    txtCashierUsername.Text = this.resultTransaction.CASHIER_USERNAME;
                    var dt = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(this.resultTransaction.CREATE_TIME ?? 0);
                    if (dt.HasValue && dt.Value != DateTime.MinValue)
                    {
                        dtCreateTime.DateTime = dt.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InPhieuHoanUng(bool isPrintNow)
        {
            try
            {
                if (isPrintNow)
                {
                    Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                    store.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuThuHoanUng_MPS000113, ProcessPrintRepayPrintNow);
                }
                else
                {
                    Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                    store.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuThuHoanUng_MPS000113, ProcessPrintRepay);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool ProcessPrintRepay(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                if (this.resultTransaction == null)
                    throw new NullReferenceException("this.resultRepay = null");

                decimal ratio = 0;

                //MPS.Processor.Mps000113.PDO.PatyAlterBhytADO mpsPatyAlterBhyt = new PatyAlterBhytADO();
                var PatyAlterBhyt = new V_HIS_PATIENT_TYPE_ALTER();
                PrintGlobalStore.LoadCurrentPatientTypeAlter(this.resultTransaction.TREATMENT_ID ?? 0, 0, ref PatyAlterBhyt);
                if (PatyAlterBhyt != null && !String.IsNullOrEmpty(PatyAlterBhyt.HEIN_CARD_NUMBER))
                {
                    ratio = new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(PatyAlterBhyt.HEIN_TREATMENT_TYPE_CODE, PatyAlterBhyt.HEIN_CARD_NUMBER, PatyAlterBhyt.LEVEL_CODE, PatyAlterBhyt.RIGHT_ROUTE_CODE) ?? 0;
                    //Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000113.PDO.PatyAlterBhytADO>(mpsPatyAlterBhyt,PatyAlterBhyt);
                }
                HisDepartmentTranLastFilter departLastFilter = new HisDepartmentTranLastFilter();
                departLastFilter.TREATMENT_ID = this.resultTransaction.TREATMENT_ID ?? 0;
                departLastFilter.BEFORE_LOG_TIME = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
                var departmentTran = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<V_HIS_DEPARTMENT_TRAN>("api/HisDepartmentTran/GetLastByTreatmentId", ApiConsumers.MosConsumer, departLastFilter, null);

                CommonParam paramtreatment = new CommonParam();
                HisTreatmentFeeViewFilter filterTreat = new HisTreatmentFeeViewFilter();
                filterTreat.ID = resultTransaction.TREATMENT_ID;
                var TreatmentFee = new Inventec.Common.Adapter.BackendAdapter(paramtreatment).Get<List<V_HIS_TREATMENT_FEE>>("api/HisTreatment/GetFeeView", ApiConsumers.MosConsumer, filterTreat, paramtreatment);

                HisTransactionViewFilter filterTran = new HisTransactionViewFilter();
                filterTran.TREATMENT_ID = resultTransaction.TREATMENT_ID;
                //filterTran.TRANSACTION_TYPE_IDs = new List<long>() { IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT };
                filterTran.IS_CANCEL = false;
                List<V_HIS_TRANSACTION> transa = new BackendAdapter(paramtreatment).Get<List<V_HIS_TRANSACTION>>("api/HisTransaction/GetView", ApiConsumers.MosConsumer, filterTran, paramtreatment);
                if (transa == null) transa = new List<V_HIS_TRANSACTION>();

                MPS.Processor.Mps000113.PDO.Mps000113PDO rdo = new MPS.Processor.Mps000113.PDO.Mps000113PDO(this.resultTransaction, null, ratio, PatyAlterBhyt, departmentTran, TreatmentFee.First(), transa);
                MPS.ProcessorBase.Core.PrintData printData = null;
                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.resultTransaction != null ? this.resultTransaction.TREATMENT_CODE : ""), printTypeCode, currentModule != null ? currentModule.RoomId : 0);

                if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                {
                    //printData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, null);
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO, ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog });
                }
                else
                {
                    //printData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, null);
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO, ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog });
                }
                //result = MPS.MpsPrinter.Run(printData);

                //if (result && chkAutoClose.CheckState == CheckState.Checked)
                //{
                //    this.Close();
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private bool ProcessPrintRepayPrintNow(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                if (this.resultTransaction == null)
                    throw new NullReferenceException("this.resultRepay = null");
                //MPS.Processor.Mps000113.PDO.PatyAlterBhytADO mpsPatyAlterBhyt = new PatyAlterBhytADO();
                decimal ratio = 0;
                var PatyAlterBhyt = new V_HIS_PATIENT_TYPE_ALTER();
                PrintGlobalStore.LoadCurrentPatientTypeAlter(this.resultTransaction.TREATMENT_ID ?? 0, 0, ref PatyAlterBhyt);
                if (PatyAlterBhyt != null && !String.IsNullOrEmpty(PatyAlterBhyt.HEIN_CARD_NUMBER))
                {
                    ratio = new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(PatyAlterBhyt.HEIN_TREATMENT_TYPE_CODE, PatyAlterBhyt.HEIN_CARD_NUMBER, PatyAlterBhyt.LEVEL_CODE, PatyAlterBhyt.RIGHT_ROUTE_CODE) ?? 0;
                    //Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000113.PDO.PatyAlterBhytADO>(mpsPatyAlterBhyt, PatyAlterBhyt);
                }
                HisDepartmentTranLastFilter departLastFilter = new HisDepartmentTranLastFilter();
                departLastFilter.TREATMENT_ID = this.resultTransaction.TREATMENT_ID ?? 0;
                departLastFilter.BEFORE_LOG_TIME = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
                var departmentTran = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<V_HIS_DEPARTMENT_TRAN>("api/HisDepartmentTran/GetLastByTreatmentId", ApiConsumers.MosConsumer, departLastFilter, null);

                CommonParam paramtreatment = new CommonParam();
                HisTreatmentFeeViewFilter filterTreat = new HisTreatmentFeeViewFilter();
                filterTreat.ID = resultTransaction.TREATMENT_ID;
                var TreatmentFee = new Inventec.Common.Adapter.BackendAdapter(paramtreatment).Get<List<V_HIS_TREATMENT_FEE>>("api/HisTreatment/GetFeeView", ApiConsumers.MosConsumer, filterTreat, paramtreatment);

                HisTransactionViewFilter filterTran = new HisTransactionViewFilter();
                filterTran.TREATMENT_ID = resultTransaction.TREATMENT_ID;
                filterTran.TRANSACTION_TYPE_IDs = new List<long>() { IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT };
                filterTran.IS_CANCEL = false;
                List<V_HIS_TRANSACTION> transa = new BackendAdapter(paramtreatment).Get<List<V_HIS_TRANSACTION>>("api/HisTransaction/GetView", ApiConsumers.MosConsumer, filterTran, paramtreatment);
                if (transa == null) transa = new List<V_HIS_TRANSACTION>();

                MPS.Processor.Mps000113.PDO.Mps000113PDO rdo = new MPS.Processor.Mps000113.PDO.Mps000113PDO(this.resultTransaction, null, ratio, PatyAlterBhyt, departmentTran, TreatmentFee.First(), transa);
                //MPS.ProcessorBase.Core.PrintData printData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, null);
                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((this.resultTransaction != null ? this.resultTransaction.TREATMENT_CODE : ""), printTypeCode, currentModule != null ? currentModule.RoomId : 0);

                result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO, ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog });

                if (result && chkAutoClose.CheckState == CheckState.Checked)
                {
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private void CallModuleShowPrintLog(string printTypeCode, string uniqueCode)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(printTypeCode) && !String.IsNullOrWhiteSpace(uniqueCode))
                {
                    //goi modul
                    HIS.Desktop.ADO.PrintLogADO ado = new HIS.Desktop.ADO.PrintLogADO(printTypeCode, uniqueCode);

                    List<object> listArgs = new List<object>();
                    listArgs.Add(ado);

                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("Inventec.Desktop.Plugins.PrintLog", currentModule.RoomId, currentModule.RoomTypeId, listArgs);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadKeyFrmLanguage()
        {
            try
            {
                this.Text = Inventec.Common.Resource.Get.Value("frmTransactionRepay.Text", Base.ResourceLangManager.LanguageFrmTransactionRepay, LanguageManager.GetCulture());


                //Button
                this.btnNew.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_REPAY__BTN_NEW", Base.ResourceLangManager.LanguageFrmTransactionRepay, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_REPAY__BTN_SAVE", Base.ResourceLangManager.LanguageFrmTransactionRepay, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnSavePrint.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_REPAY__BTN_SAVE_PRINT", Base.ResourceLangManager.LanguageFrmTransactionRepay, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.btnPrint.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_REPAY__BTN_PRINT", Base.ResourceLangManager.LanguageFrmTransactionRepay, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                //Layout
                this.layoutAccountBook.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_REPAY__LAYOUT_ACCOUNT_BOOK", Base.ResourceLangManager.LanguageFrmTransactionRepay, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutCreateTime.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_REPAY__LAYOUT_CREATE_TIME", Base.ResourceLangManager.LanguageFrmTransactionRepay, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutCashierUsername.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_REPAY__LAYOUT_CASHIER_USERNAME", Base.ResourceLangManager.LanguageFrmTransactionRepay, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutDescription.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_REPAY__LAYOUT_DESCRIPTION", Base.ResourceLangManager.LanguageFrmTransactionRepay, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutTongTuDen.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_REPAY__LAYOUT_NUM_ORDER", Base.ResourceLangManager.LanguageFrmTransactionRepay, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciPayForm.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_REPAY__LAYOUT_PAY_FORM", Base.ResourceLangManager.LanguageFrmTransactionRepay, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutTongTuDen.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_REPAY__LAYOUT_TONG_TU_DEN", Base.ResourceLangManager.LanguageFrmTransactionRepay, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutTotalAmount.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_REPAY__LAYOUT_TOTAL_AMOUNT", Base.ResourceLangManager.LanguageFrmTransactionRepay, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.layoutTransactionCode.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_REPAY__LAYOUT_TRANSACTION_CODE", Base.ResourceLangManager.LanguageFrmTransactionRepay, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciTransactionTime.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_REPAY__LAYOUT_TRANSACTION_TIME", Base.ResourceLangManager.LanguageFrmTransactionRepay, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.lciRepayReasonCode.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_REPAY__LAYOUT_REPAY_REASON_CODE", Base.ResourceLangManager.LanguageFrmTransactionRepay, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                // 45677 - Caption thong tin goi benh nhan
                this.lciPackageName.Text = Inventec.Common.Resource.Get.Value("frmTransactionRepay.lciPackageName.Text", Base.ResourceLangManager.LanguageFrmTransactionRepay, LanguageManager.GetCulture());
                this.lciRegisterDate.Text = Inventec.Common.Resource.Get.Value("frmTransactionRepay.lciRegisterDate.Text", Base.ResourceLangManager.LanguageFrmTransactionRepay, LanguageManager.GetCulture());
                this.lciTotalPaid.Text = Inventec.Common.Resource.Get.Value("frmTransactionRepay.lciTotalPaid.Text", Base.ResourceLangManager.LanguageFrmTransactionRepay, LanguageManager.GetCulture());
                this.lciTotalUsed.Text = Inventec.Common.Resource.Get.Value("frmTransactionRepay.lciTotalUsed.Text", Base.ResourceLangManager.LanguageFrmTransactionRepay, LanguageManager.GetCulture());

                this.layoutTongTuDen.Text = Inventec.Common.Resource.Get.Value("frmTransactionRepay.layoutTongTuDen.Text", Base.ResourceLangManager.LanguageFrmTransactionRepay, LanguageManager.GetCulture());
                //check box
                this.chkAutoClose.Properties.Caption = Inventec.Common.Resource.Get.Value("frmTransactionRepay.chkAutoClose.Properties.Caption", Base.ResourceLangManager.LanguageFrmTransactionRepay, LanguageManager.GetCulture());
                this.chkAutoClose.ToolTip = Inventec.Common.Resource.Get.Value("frmTransactionRepay.chkAutoClose.ToolTip", Base.ResourceLangManager.LanguageFrmTransactionRepay, LanguageManager.GetCulture());
                this.chkXemTruoc.Properties.Caption = Inventec.Common.Resource.Get.Value("frmTransactionRepay.chkXemTruoc.Properties.Caption", Base.ResourceLangManager.LanguageFrmTransactionRepay, LanguageManager.GetCulture());
                this.chkCompensation.Properties.Caption = Inventec.Common.Resource.Get.Value("frmTransactionRepay.chkCompensation.Properties.Caption", Base.ResourceLangManager.LanguageFrmTransactionRepay, LanguageManager.GetCulture());
                this.chkCompensation.ToolTip = Inventec.Common.Resource.Get.Value("frmTransactionRepay.chkCompensation.ToolTip", Base.ResourceLangManager.LanguageFrmTransactionRepay, LanguageManager.GetCulture());


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
                    txtDescription.Focus();
                    txtDescription.SelectAll();
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
                        UpdateDictionaryNumOrderAccountBook(accountBook);
                    }
                    cboPayForm.Focus();
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
                    UpdateDictionaryNumOrderAccountBook(accountBook);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtRepayReason_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    bool valid = false;
                    if (!String.IsNullOrEmpty(txtRepayReason.Text))
                    {
                        var listData = BackendDataWorker.Get<HIS_REPAY_REASON>().Where(o => o.REPAY_REASON_CODE.Contains(txtRepayReason.Text)).ToList();
                        if (listData != null && listData.Count == 1)
                        {
                            valid = true;
                            txtRepayReason.Text = listData.First().REPAY_REASON_CODE;
                            cboRepayReason.EditValue = listData.First().ID;
                            cboRepayReason.Focus();
                            SendKeys.Send("{TAB}");
                        }
                    }
                    if (!valid)
                    {
                        cboPayForm.Focus();
                        cboPayForm.ShowPopup();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboRepayReason_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (cboRepayReason.EditValue != null)
                    {
                        var repayReason = BackendDataWorker.Get<HIS_REPAY_REASON>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboRepayReason.EditValue));
                        if (repayReason != null)
                        {
                            txtRepayReason.Text = repayReason.REPAY_REASON_CODE;
                            cboRepayReason.Properties.Buttons[1].Visible = true;
                        }
                    }
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboRepayReason_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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

        private void cboRepayReason_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboRepayReason.Properties.Buttons[1].Visible = false;
                    cboRepayReason.EditValue = null;
                    txtRepayReason.Text = "";
                    txtRepayReason.Focus();
                    txtRepayReason.SelectAll();
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
                txtAccountBookCode.Text = "";
                spinTongTuDen.EditValue = null;
                spinTongTuDen.Enabled = false;
                if (cboAccountBook.EditValue != null)
                {
                    var account = this.ListAccountBook.FirstOrDefault(o => o.ID == Convert.ToInt64(cboAccountBook.EditValue));
                    if (account != null)
                    {
                        txtAccountBookCode.Text = account.ACCOUNT_BOOK_CODE;
                        spinTongTuDen.EditValue = setDataToDicNumOrderInAccountBook(account);

                        if (account.IS_NOT_GEN_TRANSACTION_ORDER == 1)
                        {
                            spinTongTuDen.Enabled = true;
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
                FormatSpint(txtTotalAmount);
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
                FormatSpint(spinTransferAmount);
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
                SpinTranferAmountValidationRule rule = new SpinTranferAmountValidationRule();
                rule.spinTranferAmount = spinTransferAmount;
                rule.isRequiredPin = IsRequiredField;
                dxValidationProvider1.SetValidationRule(spinTransferAmount, rule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPayForm_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                CheckPayFormTienMatChuyenKhoan();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        void CheckPayFormTienMatChuyenKhoan()
        {
            try
            {
                if (cboPayForm.EditValue != null && Convert.ToInt64(cboPayForm.EditValue) == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TMCK)
                {
                    dxValidationProvider1.RemoveControlError(spinTransferAmount);
                    ValidControlTransferAmount(true);

                    this.lciTransferAmount.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_REPAY__LAYOUT_TRANSFER_AMOUNT_TEXT", Base.ResourceLangManager.LanguageFrmTransactionRepay, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    this.lciTransferAmount.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_REPAY__LAYOUT_TRANSFER_AMOUNT_TOOLTIP", Base.ResourceLangManager.LanguageFrmTransactionRepay, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    lciTransferAmount.AppearanceItemCaption.ForeColor = Color.Maroon;
                    lciTransferAmount.Enabled = true;

                }
                else if (cboPayForm.EditValue != null && Convert.ToInt64(cboPayForm.EditValue) == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TMQT)
                {
                    dxValidationProvider1.RemoveControlError(spinTransferAmount);
                    ValidControlTransferAmount(true);

                    this.lciTransferAmount.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_REPAY__LAYOUT_SWIPE_AMOUNT_TEXT", Base.ResourceLangManager.LanguageFrmTransactionRepay, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    this.lciTransferAmount.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_REPAY__LAYOUT_SWIPE_AMOUNT_TOOLTIP", Base.ResourceLangManager.LanguageFrmTransactionRepay, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    lciTransferAmount.AppearanceItemCaption.ForeColor = Color.Maroon;
                    lciTransferAmount.Enabled = true;

                }
                else
                {
                    dxValidationProvider1.RemoveControlError(spinTransferAmount);
                    ValidControlTransferAmount(false);
                    this.lciTransferAmount.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_REPAY__LAYOUT_TRANSFER_AMOUNT_TEXT", Base.ResourceLangManager.LanguageFrmTransactionRepay, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    this.lciTransferAmount.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_REPAY__LAYOUT_TRANSFER_AMOUNT_TOOLTIP", Base.ResourceLangManager.LanguageFrmTransactionRepay, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    lciTransferAmount.AppearanceItemCaption.ForeColor = Color.Black;
                    lciTransferAmount.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
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
        private void onSendDelegate(object data)
        {
            currentPatientBankAccount = null;
            if(data != null)
            {
                currentPatientBankAccount = ((MOS.EFMODEL.DataModels.V_HIS_PATIENT_BANK_ACCOUNT)data);

                var parts = new List<string>();

                if (!string.IsNullOrWhiteSpace(currentPatientBankAccount.PAYEE_BANK_NAME))
                    parts.Add(currentPatientBankAccount.PAYEE_BANK_NAME);

                if (!string.IsNullOrWhiteSpace(currentPatientBankAccount.PAYEE_ACCOUNT_NUMBER))
                    parts.Add(currentPatientBankAccount.PAYEE_ACCOUNT_NUMBER);

                if (!string.IsNullOrWhiteSpace(currentPatientBankAccount.PAYEE_NAME))
                    parts.Add(currentPatientBankAccount.PAYEE_NAME);

                string beneficiary = string.Join(" - ", parts);

                if (!string.IsNullOrWhiteSpace(currentPatientBankAccount.RELATION_NAME))
                {
                    beneficiary += " (" + currentPatientBankAccount.RELATION_NAME + ")";
                }

                txtNguoiThuHuong.Text = beneficiary;
            }
            else {                 
                txtNguoiThuHuong.Text = null;
            }
        }
        private void btnThuHuong_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.HisPatientBankAccount").FirstOrDefault();
                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.HisPatientBankAccount'");
                if (!moduleData.IsPlugin || moduleData.ExtensionInfo == null) throw new NullReferenceException("Module 'HIS.Desktop.Plugins.HisPatientBankAccount' is not plugins");
                List<object> listArgs = new List<object>();
                var Histreatment = new HIS_TREATMENT();
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TREATMENT>(Histreatment, this.Treatment);
                listArgs.Add(Histreatment);
                listArgs.Add(this.currentPatientBankAccount);
                listArgs.Add((DelegateSelectData)onSendDelegate);
                var extenceInstance = HIS.Desktop.Utility.PluginInstance.GetPluginInstance(PluginInstance.GetModuleWithWorkingRoom(moduleData, this.currentModule.RoomId, this.currentModule.RoomTypeId), listArgs);
                if (extenceInstance == null) throw new ArgumentNullException("Khoi tao moduleData that bai. extenceInstance = null");

                WaitingManager.Hide();
                ((Form)extenceInstance).ShowDialog();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkXemTruoc_CheckedChanged(object sender, EventArgs e)
        {
            _isXemNgay = chkXemTruoc.Checked;
        }
    }
}

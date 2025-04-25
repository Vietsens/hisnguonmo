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
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.DepositService.Config;
using HIS.Desktop.Plugins.DepositService.DepositService.Validtion;
using HIS.Desktop.Print;
using HIS.UC.SereServTree;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.WCF.JsonConvert;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using WCF;
using WCF.Client;
using HIS.Desktop.Utility;
using Inventec.Desktop.Common.LanguageManager;
using HIS.Desktop.ADO;

namespace HIS.Desktop.Plugins.DepositService.DepositService
{
    public partial class frmDepositService :HIS.Desktop.Utility.FormBase
    {
        #region Declaration
        MOS.EFMODEL.DataModels.V_HIS_TREATMENT_FEE hisTreatment { get; set; }
        long treatmentId;
        MOS.EFMODEL.DataModels.V_HIS_TRANSACTION hisDeposit { get; set; }
        int positionHandle = -1;
        decimal totalAmountDeposit = 0;

        bool isNotLoadWhilechkAutoCloseStateInFirst = true;
        bool isNotLoadWhileChangeControlStateInFirst;
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        string moduleLink = "HIS.Desktop.Plugins.DepositService";

        List<MOS.EFMODEL.DataModels.HIS_PAY_FORM> ListPayForm;
        List<MOS.EFMODEL.DataModels.V_HIS_CASHIER_ROOM> ListCashierRoom;
        List<MOS.EFMODEL.DataModels.V_HIS_ACCOUNT_BOOK> ListAccountBook;
        List<V_HIS_SERE_SERV_5> sereServByTreatment;
        List<V_HIS_SERE_SERV_5> sSByTreatment;
        List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> ListPatientType;
        long? branchId;
        long cashierRoomId;
        MOS.SDO.HisTransactionDepositSDO hisDepositSDO;
        SendResultToOtherForm sendResultToOtherForm;
        int SetDefaultDepositPrice;
        SereServTreeProcessor ssTreeProcessor;
        UserControl ucSereServTree;
        bool isPrintNow = false;
        bool isSign = false;
        Inventec.Desktop.Common.Modules.Module moduleData;
        HIS.Desktop.Common.DelegateReturnSuccess returnData = null;
        bool? IsDepositAll = null;

        WcfClient cll;
        string creator = "";
        private enum CashierRoomPaymentOption
        {
            Option1 = 1,
            Option2 = 2
        }
        bool IsChangeControl = false;
        #endregion

        #region Construct
        public frmDepositService(long hisTreatmentId, MOS.SDO.HisTransactionDepositSDO _hisDepositSDO, SendResultToOtherForm _sendResultToOtherForm, long? _branchId, long cashierRoomId, List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_5> sereServs, Inventec.Desktop.Common.Modules.Module _module, HIS.Desktop.Common.DelegateReturnSuccess returnSuccess, bool? isDepositAll)
            : base(_module)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("frmDepositService.InitializeComponent. 1");
                InitializeComponent();
                this.treatmentId = hisTreatmentId;
                this.cashierRoomId = cashierRoomId;
                this.hisDepositSDO = _hisDepositSDO;
                this.branchId = _branchId;
                this.sendResultToOtherForm = _sendResultToOtherForm;
                this.sSByTreatment = sereServs;
                this.sereServByTreatment = sereServs;
                this.SetDefaultDepositPrice = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<int>("HIS_RS.HIS_DEPOSIT.DEFAULT_PRICE_FOR_BHYT_OUT_PATIENT");
                Inventec.Common.Logging.LogSystem.Debug("Gia tri cua SetDefaultDepositPrice. " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.SetDefaultDepositPrice), this.SetDefaultDepositPrice));
                this.moduleData = _module;
                this.SetCaptionByLanguageKey();
                this.returnData = returnSuccess;
                this.IsDepositAll = isDepositAll;
                Inventec.Common.Logging.LogSystem.Debug("frmDepositService.InitializeComponent. 2");
                creator = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                try
                {
                    string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                }
                catch (Exception ex)
                {
                    LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public frmDepositService(V_HIS_TREATMENT_FEE hisTreatment, MOS.SDO.HisTransactionDepositSDO _hisDepositSDO, SendResultToOtherForm _sendResultToOtherForm, long? _branchId, long cashierRoomId, List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_5> sereServs, Inventec.Desktop.Common.Modules.Module _module, HIS.Desktop.Common.DelegateReturnSuccess returnSuccess, bool? isDepositAll)
            : base(_module)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("frmDepositService.InitializeComponent. 1");
                InitializeComponent();
                this.hisTreatment = hisTreatment;
                this.cashierRoomId = cashierRoomId;
                this.hisDepositSDO = _hisDepositSDO;
                this.branchId = _branchId;
                this.sendResultToOtherForm = _sendResultToOtherForm;
                this.sSByTreatment = sereServs;
                this.sereServByTreatment = sereServs;
                this.SetDefaultDepositPrice = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<int>("HIS_RS.HIS_DEPOSIT.DEFAULT_PRICE_FOR_BHYT_OUT_PATIENT");
                Inventec.Common.Logging.LogSystem.Debug("Gia tri cua SetDefaultDepositPrice. " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.SetDefaultDepositPrice), this.SetDefaultDepositPrice));
                this.moduleData = _module;
                this.returnData = returnSuccess;
                this.SetCaptionByLanguageKey();
                this.IsDepositAll = isDepositAll;
                Inventec.Common.Logging.LogSystem.Debug("frmDepositService.InitializeComponent. 2");
                creator = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                try
                {
                    string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                    this.Icon = Icon.ExtractAssociatedIcon(iconPath);
                }
                catch (Exception ex)
                {
                    LogSystem.Warn(ex);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Load
        private void frmDepositService_Load(object sender, EventArgs e)
        {
            try
            {
                HisConfigCFG.LoadConfig();
                timerInitForm.Interval = 100;
                RegisterTimer(GetModuleLink(), "timerInitForm", timerInitForm.Interval, timerInitForm_Tick);
                Inventec.Common.Logging.LogSystem.Debug("frmDepositService_Load. 1");
                WaitingManager.Show();
                CommonParam param = new CommonParam();
                positionHandle = -1;
                btnQR.Enabled = false;
                if (!CheckInputData())
                {
                    Inventec.Common.Logging.LogSystem.Debug("Du lieu truyen vao khong hop le____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => hisDepositSDO), hisDepositSDO));
                    return;
                }

                if ((this.hisTreatment == null || this.hisTreatment.ID == 0) && this.treatmentId > 0)
                {
                    LoadTreatment(this.treatmentId);
                }
                else
                {
                    this.treatmentId = this.hisTreatment.ID;
                }

                //if (this.IsDepositAll.HasValue && this.IsDepositAll.Value)
                //    panelControlTreeSereServ.Enabled = false;
                //else
                //    panelControlTreeSereServ.Enabled = true;

                FillDataToControl(this.hisDeposit);
                Inventec.Common.Logging.LogSystem.Debug("frmDepositService_Load. 2");
                LoadDataToControl();
                Inventec.Common.Logging.LogSystem.Debug("frmDepositService_Load. 3");
                WaitingManager.Hide();


                timerInitForm.Enabled = true;
                StartTimer(GetModuleLink(), "timerInitForm");
                Inventec.Common.Logging.LogSystem.Debug("frmDepositService_Load. 4");
                btnQR.Click += new EventHandler(btnQR_Click);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }




        private void timerInitForm_Tick()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("timerInitForm_Tick. 1");
                StopTimer(GetModuleLink(), "timerInitForm");

                LoadSereServByTreatment();
                Inventec.Common.Logging.LogSystem.Debug("timerInitForm_Tick. 2");
                InitControlState();
                SetDefaultControl();
                SetDefaultEnableCheckEdit();
                Inventec.Common.Logging.LogSystem.Debug("timerInitForm_Tick. 3");
                ValidControls();
                InitMenuToButtonPrint();
                Inventec.Common.Logging.LogSystem.Debug("timerInitForm_Tick. 4");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region private function

        /// <summary>
        /// kiểm tra đầu vào 
        /// </summary>
        /// <returns>boolean</returns>
        bool CheckInputData()
        {
            bool result = false;
            try
            {
                if ((this.treatmentId > 0 || this.hisTreatment != null) && this.cashierRoomId > 0)
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        void CheckPayFormTienMatChuyenKhoan(HIS_PAY_FORM payForm)
        {
            try
            {
                if (payForm != null && payForm.ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TMCK)
                {
                    dxValidationProvider1.RemoveControlError(spinTransferAmount);
                    ValidControlTransferAmount(true);

                    this.lciTranferAmount.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEPOSIT__LAYOUT_TRANSFER_AMOUNT_TEXT", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    this.lciTranferAmount.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEPOSIT__LAYOUT_TRANSFER_AMOUNT_TOOLTIP", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    lciTranferAmount.AppearanceItemCaption.ForeColor = Color.Maroon;
                    lciTranferAmount.Enabled = true;

                }
                else if (payForm != null && payForm.ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TMQT)
                {
                    dxValidationProvider1.RemoveControlError(spinTransferAmount);
                    ValidControlTransferAmount(true);

                    this.lciTranferAmount.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEPOSIT__LAYOUT_SWIPE_AMOUNT_TEXT", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    this.lciTranferAmount.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEPOSIT__LAYOUT_SWIPE_AMOUNT_TOOLTIP", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    lciTranferAmount.AppearanceItemCaption.ForeColor = Color.Maroon;
                    lciTranferAmount.Enabled = true;

                }
                else if (payForm != null && payForm.ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__QUET_THE)
                {
                    dxValidationProvider1.RemoveControlError(spinTransferAmount);
                    ValidControlTransferAmount(false);
                    this.lciTranferAmount.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEPOSIT__LAYOUT_TRANSFER_AMOUNT_TEXT", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    this.lciTranferAmount.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEPOSIT__LAYOUT_TRANSFER_AMOUNT_TOOLTIP", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    lciTranferAmount.AppearanceItemCaption.ForeColor = Color.Black;
                    lciTranferAmount.Enabled = false;
                }
                else
                {
                    dxValidationProvider1.RemoveControlError(spinTransferAmount);
                    ValidControlTransferAmount(false);
                    this.lciTranferAmount.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEPOSIT__LAYOUT_TRANSFER_AMOUNT_TEXT", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    this.lciTranferAmount.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEPOSIT__LAYOUT_TRANSFER_AMOUNT_TOOLTIP", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    lciTranferAmount.AppearanceItemCaption.ForeColor = Color.Black;
                    lciTranferAmount.Enabled = false;
                }
                spinTransferAmount.EditValue = 0;
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

        /// <summary>
        /// lọc các sereServ đã được tạm ứng và hoàn ứng
        /// </summary>
        void FilterSereServDepositAndRepay(ref List<V_HIS_SERE_SERV_5> sereServByTreatmentSDOProcess, List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_DEPOSIT> sereServDepositByTreatments)
        {
            List<V_HIS_SERE_SERV_5> ListSereServByTreatmentSDOResult = new List<V_HIS_SERE_SERV_5>();
            try
            {

                // lấy List sereServDeposit có IS_CANCEL !=1
                //var sereServDepositByTreatments = GetSereServDepositByTreatment(this.treatmentId);
                if (sereServDepositByTreatments != null && sereServDepositByTreatments.Count > 0)
                {
                    var sereServDepositByTreatmentNotCancels = sereServDepositByTreatments.Where(o => o.IS_CANCEL != 1).ToList();
                    if (sereServDepositByTreatmentNotCancels != null && sereServDepositByTreatmentNotCancels.Count > 0)
                    {
                        // lấy list SereServDepositRepays có IS_CANCEL !=1
                        var seseDepoRepays = GetSeSeDepoRePay(this.treatmentId);
                        if (seseDepoRepays != null && seseDepoRepays.Count > 0)
                        {
                            var seseDepoRepayNotCancels = seseDepoRepays.Where(o => o.IS_CANCEL != 1).ToList();
                            if (seseDepoRepayNotCancels != null && seseDepoRepayNotCancels.Count > 0)
                            {
                                List<long> seseDepoIds = seseDepoRepayNotCancels.Select(o => o.SERE_SERV_DEPOSIT_ID).ToList();
                                // lấy List sereServDeposit không có trong list SereServDepositRepays
                                var sereServDepositNotContainRepays = sereServDepositByTreatmentNotCancels.Where(o => !seseDepoIds.Contains(o.ID)).ToList();
                                ListSereServByTreatmentSDOResult = sereServByTreatmentSDOProcess.Where(o => !sereServDepositNotContainRepays.Select(p => p.SERE_SERV_ID).Contains(o.ID)).ToList();
                            }
                            else
                            {
                                var ListSereServByTreatmentSDOResult1 = sereServByTreatmentSDOProcess.Where(o => !sereServDepositByTreatmentNotCancels.Select(p => p.SERE_SERV_ID).Contains(o.ID)).ToList();
                                if (ListSereServByTreatmentSDOResult1 != null && ListSereServByTreatmentSDOResult1.Count > 0)
                                    ListSereServByTreatmentSDOResult.AddRange(ListSereServByTreatmentSDOResult1);
                            }
                        }
                        else
                        {
                            var ListSereServByTreatmentSDOResult1 = sereServByTreatmentSDOProcess.Where(o => !sereServDepositByTreatmentNotCancels.Select(p => p.SERE_SERV_ID).Contains(o.ID)).ToList();
                            if (ListSereServByTreatmentSDOResult1 != null && ListSereServByTreatmentSDOResult1.Count > 0)
                                ListSereServByTreatmentSDOResult.AddRange(ListSereServByTreatmentSDOResult1);
                        }
                    }
                    else
                    {
                        ListSereServByTreatmentSDOResult = sereServByTreatmentSDOProcess;
                    }
                }
                else if (sereServByTreatmentSDOProcess != null && sereServByTreatmentSDOProcess.Count > 0)
                {
                    ListSereServByTreatmentSDOResult = sereServByTreatmentSDOProcess;
                }

                // bỏ những dữ liệu trùng

                sereServByTreatmentSDOProcess = (ListSereServByTreatmentSDOResult != null && ListSereServByTreatmentSDOResult.Count > 0) ? ListSereServByTreatmentSDOResult.GroupBy(o => o.ID).Select(g => g.FirstOrDefault()).ToList() : ListSereServByTreatmentSDOResult;

                //Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("sereServByTreatmentSDOProcess 1__________", sereServByTreatmentSDOProcess.Select(o => o.VIR_TOTAL_HEIN_PRICE).ToList()));
                if (chkHideServiceBHYT.Checked == true)
                    sereServByTreatmentSDOProcess = sereServByTreatmentSDOProcess.Where(o => o.VIR_TOTAL_HEIN_PRICE <= 0).ToList();
                if (chkHideServiceNoCost.Checked == true)
                    sereServByTreatmentSDOProcess = sereServByTreatmentSDOProcess.Where(o => o.VIR_TOTAL_PRICE != 0).ToList();

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// lấy các HIS_SERE_SERV_DEPOSIT theo treatment dùng để lọc các ServSere đã được tạm ứng
        /// </summary>
        /// <param name="treatmentId"></param>
        /// <returns></returns>
        List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_DEPOSIT> GetSereServDepositByTreatment(long treatmentId)
        {
            List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_DEPOSIT> sereServDeposits = null;
            CommonParam param = new CommonParam();
            try
            {
                MOS.Filter.HisSereServDepositFilter sereServDepositFilter = new HisSereServDepositFilter();
                sereServDepositFilter.TDL_TREATMENT_ID = treatmentId;
                sereServDeposits = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_DEPOSIT>>("api/HisSereServDeposit/Get", ApiConsumer.ApiConsumers.MosConsumer, sereServDepositFilter, param);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return sereServDeposits;
        }

        //Lấy List<HIS_SESE_DEPO_REPAY> đã hủy hoàn ứng
        List<MOS.EFMODEL.DataModels.HIS_SESE_DEPO_REPAY> GetSeSeDepoRePay(long treatmentId)
        {
            List<MOS.EFMODEL.DataModels.HIS_SESE_DEPO_REPAY> seseDepoRepays = null;
            CommonParam param = new CommonParam();
            try
            {
                MOS.Filter.HisSeseDepoRepayFilter seseDepositRepayFilter = new HisSeseDepoRepayFilter();
                seseDepositRepayFilter.TDL_TREATMENT_ID = treatmentId;
                seseDepoRepays = new BackendAdapter(param).Get<List<HIS_SESE_DEPO_REPAY>>("api/HisSeseDepoRepay/Get", ApiConsumer.ApiConsumers.MosConsumer, seseDepositRepayFilter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return seseDepoRepays;
        }

        private void LoadTreatment(long _treatmentId)
        {
            try
            {
                if (_treatmentId > 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisTreatmentFeeViewFilter treatment1ViewFilter = new HisTreatmentFeeViewFilter();
                    treatment1ViewFilter.ID = _treatmentId;
                    var treatment1s = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_FEE>>("api/HisTreatment/GetFeeView", ApiConsumer.ApiConsumers.MosConsumer, treatment1ViewFilter, param);
                    if (treatment1s != null && treatment1s.Count > 0)
                    {
                        this.hisTreatment = treatment1s.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// set giá trị mặc định cho các control
        /// </summary>
        private void SetDefaultControl()
        {
            try
            {

                V_HIS_ACCOUNT_BOOK accountBookDefault = null;
                //chọn mặc định sổ nếu có sổ tương ứng
                if (GlobalVariables.LastAccountBook != null && GlobalVariables.LastAccountBook.Count > 0)
                {
                    var lstBook = ListAccountBook.Where(o => GlobalVariables.LastAccountBook.Select(s => s.ID).Contains(o.ID)).ToList();
                    if (lstBook != null && lstBook.Count > 0)
                    {
                        accountBookDefault = lstBook.OrderByDescending(o => o.ID).First();
                    }
                }

                if (accountBookDefault == null) accountBookDefault = ListAccountBook.FirstOrDefault();

                SetDefaultAccountBookForUser(accountBookDefault);
                SetDataToDicNumOrderInAccountBook(accountBookDefault, true);
                SetDefaultPayFormForUser();
                EnableButton(true);
                layoutTongTuDen.Enabled = true;
                lciTransactionTime.Enabled = (HisConfigCFG.IsEditTransactionTimeCFG != null && HisConfigCFG.IsEditTransactionTimeCFG.Equals("1"));

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// load dl vào combo hình thức thanh toán
        /// </summary>
        /// <param name="_payFormCode"></param>
        private void LoadPayFormCombo(string _payFormCode)
        {
            try
            {
                List<MOS.EFMODEL.DataModels.HIS_PAY_FORM> listResult = new List<MOS.EFMODEL.DataModels.HIS_PAY_FORM>();
                listResult = ListPayForm.Where(o => (o.PAY_FORM_CODE != null && o.PAY_FORM_CODE.StartsWith(_payFormCode))).ToList();

                //List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                //columnInfos.Add(new ColumnInfo("PAY_FORM_CODE", "", 100, 1));
                //columnInfos.Add(new ColumnInfo("PAY_FORM_NAME", "", 250, 2));
                //ControlEditorADO controlEditorADO = new ControlEditorADO("PAY_FORM_NAME", "ID", columnInfos, false, 350);
                //ControlEditorLoader.Load(cboPayForm, listResult, controlEditorADO);
                if (listResult.Count == 1)
                {
                    cboPayForm.EditValue = listResult[0].ID;
                    //txtPayFormCode.Text = listResult[0].PAY_FORM_CODE;
                    CheckPayFormTienMatChuyenKhoan(listResult[0]);
                    dtTransactionTime.Focus();
                    dtTransactionTime.ShowPopup();


                }
                else if (listResult.Count > 1)
                {
                    cboPayForm.EditValue = null;
                    cboPayForm.Focus();
                    dtTransactionTime.ShowPopup();

                }
                else
                {
                    cboPayForm.EditValue = null;
                    cboPayForm.Focus();
                    dtTransactionTime.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadAccountBookCombo(string _accountBookCode)
        {
            try
            {
                List<V_HIS_ACCOUNT_BOOK> listResult = new List<V_HIS_ACCOUNT_BOOK>();
                listResult = ListAccountBook.Where(o => (o.ACCOUNT_BOOK_CODE != null && o.ACCOUNT_BOOK_CODE.StartsWith(_accountBookCode))).ToList();

                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("ACCOUNT_BOOK_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("ACCOUNT_BOOK_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("ACCOUNT_BOOK_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboAccountBook, listResult, controlEditorADO);
                if (listResult.Count == 1)
                {
                    cboAccountBook.EditValue = listResult[0].ID;
                    txtAccountBookCode.Text = listResult[0].ACCOUNT_BOOK_CODE;
                    SetDataToDicNumOrderInAccountBook(listResult[0]);
                    cboPayForm.Focus();
                    cboPayForm.SelectAll();
                }
                else if (listResult.Count > 1)
                {
                    cboAccountBook.EditValue = null;
                    cboAccountBook.Focus();
                    cboAccountBook.ShowPopup();
                }
                else
                {
                    cboAccountBook.EditValue = null;
                    cboAccountBook.Focus();
                    cboAccountBook.ShowPopup();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// update số chứng từ của sổ
        /// </summary>
        /// <param name="accountBook"></param>
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

        /// <summary>
        /// gán giá trị vào dictionary số chứng từ
        /// </summary>
        /// <param name="accountBook"></param>
        //private void SetDataToDicNumOrderInAccountBook(V_HIS_ACCOUNT_BOOK accountBook, bool isFirstLoad = false)
        //{
        //    try
        //    {
        //        if (accountBook != null && accountBook.IS_NOT_GEN_TRANSACTION_ORDER == 1)
        //        {
        //            if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook == null
        //                || HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.Count == 0
        //                || (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook != null
        //                && HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.Count > 0
        //                && !HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.ContainsKey(accountBook.ID)))
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

        //                HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.Add(accountBook.ID, accountBook.CURRENT_NUM_ORDER ?? 0);
        //                spinTongTuDen.Value = (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook[accountBook.ID]) + 1;
        //            }
        //            else
        //            {
        //                layoutTongTuDen.Enabled = true;
        //                spinTongTuDen.Value = (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook[accountBook.ID]) + 1;
        //            }
        //        }
        //        else
        //        {
        //            spinTongTuDen.Value = accountBook.CURRENT_NUM_ORDER ?? 0 + 1;
        //            layoutTongTuDen.Enabled = false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Error(ex);
        //    }
        //}

        private void SetDataToDicNumOrderInAccountBook(V_HIS_ACCOUNT_BOOK accountBook, bool isFirstLoad = false)
        {
            try
            {
                if (accountBook != null && accountBook.IS_NOT_GEN_TRANSACTION_ORDER == 1)
                {
                    if (GlobalVariables.dicNumOrderInAccountBook == null || GlobalVariables.dicNumOrderInAccountBook.Count == 0 || (GlobalVariables.dicNumOrderInAccountBook != null && GlobalVariables.dicNumOrderInAccountBook.Count > 0 && !GlobalVariables.dicNumOrderInAccountBook.ContainsKey(accountBook.ID)))
                    {
                        if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook == null)
                        {
                            HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook = new Dictionary<long, decimal>();
                        }

                        layoutTongTuDen.Enabled = true;
                        if (!isFirstLoad)
                        {
                            CommonParam param = new CommonParam();
                            MOS.Filter.HisAccountBookViewFilter hisAccountBookViewFilter = new HisAccountBookViewFilter();
                            hisAccountBookViewFilter.ID = accountBook.ID;
                            var accountBooks = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_ACCOUNT_BOOK>>(HisRequestUriStore.HIS_ACCOUNT_BOOK_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, hisAccountBookViewFilter, param);
                            if (accountBooks != null && accountBooks.Count > 0)
                            {
                                accountBook = accountBooks.FirstOrDefault();
                            }
                        }
                        if (accountBook != null)
                        {
                            decimal num = 0;
                            if ((accountBook.CURRENT_NUM_ORDER ?? 0) > 0)
                            {
                                num = (accountBook.CURRENT_NUM_ORDER ?? 0);
                            }
                            else
                            {
                                num = (decimal)accountBook.FROM_NUM_ORDER - 1;
                            }
                            HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.Add(accountBook.ID, num);
                            spinTongTuDen.Value = (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook[accountBook.ID]) + 1;
                        }
                    }
                    else
                    {
                        layoutTongTuDen.Enabled = true;
                        spinTongTuDen.Value = (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook[accountBook.ID]) + 1;
                    }
                }
                else
                {
                    spinTongTuDen.Value = (accountBook.CURRENT_NUM_ORDER ?? 0) + 1;
                    layoutTongTuDen.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetSereServToDataTransfer(List<SereServADO> sereServADOs, HisTransactionDepositSDO transactionData)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Warn("Giá trị của key MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT is:. " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<long>("MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT")), HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<long>("MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT")));

                Inventec.Common.Logging.LogSystem.Warn("Giá trị của sereServADOs is:. " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sereServADOs), sereServADOs));

                foreach (var sereServAdo in sereServADOs)
                {
                    if (sereServAdo.IsLeaf ?? false)
                    {
                        HIS_SERE_SERV_DEPOSIT sereServDeposit = new HIS_SERE_SERV_DEPOSIT();
                        sereServDeposit.SERE_SERV_ID = sereServAdo.ID;
                        if (sereServAdo.PATIENT_TYPE_ID == HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<long>("MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT"))
                        {
                            V_HIS_SERE_SERV sereServ = new V_HIS_SERE_SERV();
                            sereServ = AutoMapper.Mapper.Map<SereServADO, V_HIS_SERE_SERV>(sereServAdo);
                            if (SetDefaultDepositPrice == 1 || (SetDefaultDepositPrice == 3 && hisTreatment.TDL_TREATMENT_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU))
                            {
                                sereServDeposit.AMOUNT = MOS.LibraryHein.Bhyt.BhytPriceCalculator.DefaultPatientPriceForOutBhyt(sereServ) ?? 0;
                            }
                            else if (SetDefaultDepositPrice == 2)
                            {
                                sereServDeposit.AMOUNT = sereServAdo.VIR_TOTAL_PRICE ?? 0;
                            }
                            else
                            {
                                sereServDeposit.AMOUNT = sereServAdo.VIR_TOTAL_PATIENT_PRICE ?? 0;
                            }
                            Inventec.Common.Logging.LogSystem.Warn("PATIENT_TYPE_ID!=BHYT Giá trị của sereServDeposit.AMOUNT is:. " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sereServDeposit.AMOUNT), sereServDeposit.AMOUNT));
                        }
                        else
                        {
                            sereServDeposit.AMOUNT = sereServAdo.VIR_TOTAL_PATIENT_PRICE ?? 0;
                            Inventec.Common.Logging.LogSystem.Warn("PATIENT_TYPE_ID==BHYT Giá trị của sereServDeposit.AMOUNT is:. " + LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sereServDeposit.AMOUNT), sereServDeposit.AMOUNT));
                        }

                        transactionData.SereServDeposits.Add(sereServDeposit);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// gán dữ liệu vào delegate refresh dữ liệu
        /// </summary>
        private void RefreshDataAfterSave()
        {
            if (this.sendResultToOtherForm != null)
            {
                this.sendResultToOtherForm(this.hisDeposit);
            }
        }

        private void LoadDataToControl()
        {
            //LoadPayForm();
            LoadPatientType();
            LoadAccountBook();
            LoadCashierRoom();
            InitComboAccountBook();
        }

        /// <summary>
        /// lấy về các sereServBill chưa bị hủy dùng để lọc các dịch vụ đã thanh toán
        /// </summary>
        /// <param name="treatmentId">treatmentId</param>
        /// <returns>"List<HIS_SERE_SERV_BILL>"</returns>
        List<HIS_SERE_SERV_BILL> GetSereServBillsByTreatment(long treatmentId)
        {
            List<HIS_SERE_SERV_BILL> sereServBills = null;
            CommonParam param = new CommonParam();
            try
            {
                MOS.Filter.HisSereServBillFilter hisSereServBillFilter = new HisSereServBillFilter();
                hisSereServBillFilter.TDL_TREATMENT_ID = treatmentId;
                hisSereServBillFilter.IS_NOT_CANCEL = true;
                sereServBills = new BackendAdapter(param).Get<List<HIS_SERE_SERV_BILL>>("api/HisSereServBill/Get", ApiConsumer.ApiConsumers.MosConsumer, hisSereServBillFilter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return sereServBills;
        }

        // lọc các sereServ đã thanh toán
        void FilterSereServBill(ref List<V_HIS_SERE_SERV_5> sereServ5s)
        {
            try
            {
                var sereServBills = GetSereServBillsByTreatment(this.treatmentId);

                CommonParam param = new CommonParam();
                MOS.Filter.HisSereServBillFilter hisSereServBillFilter = new HisSereServBillFilter();
                hisSereServBillFilter.TDL_TREATMENT_ID = this.treatmentId;
                hisSereServBillFilter.IS_NOT_CANCEL = true;
                sereServBills = new BackendAdapter(param).Get<List<HIS_SERE_SERV_BILL>>("api/HisSereServBill/Get", ApiConsumer.ApiConsumers.MosConsumer, hisSereServBillFilter, param);

                if (sereServBills == null || sereServBills.Count == 0)
                    return;
                List<long> SereServBillIds = sereServBills.Select(o => o.SERE_SERV_ID).ToList();
                // lọc các sereServ đã thanh toán
                sereServ5s = sereServ5s.Where(o => !SereServBillIds.Contains(o.ID)).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// lấy các sereServ theo treatment
        /// </summary>
        private async Task LoadSereServByTreatment()
        {
            try
            {
                CommonParam param = new CommonParam();
                this.sereServByTreatment = GetSereByTreatmentId();


                this.sereServByTreatment = this.sereServByTreatment != null ? this.sereServByTreatment.Where(o => o.IS_EXPEND == null || o.IS_EXPEND != 1).ToList() : null;
                if (this.sereServByTreatment != null && this.sereServByTreatment.Count > 0)
                {
                    // bỏ những dịch vụ không thực hiện (IS_NO_EXECUTE), không cho phép thanh toán hoặc tạm ứng (IS_NO_PAY)
                    this.sereServByTreatment = this.sereServByTreatment.Where(o => o.IS_NO_EXECUTE != 1 && o.IS_NO_PAY != 1).ToList();
                    if (this.sereServByTreatment == null || this.sereServByTreatment.Count == 0)
                        return;
                    if (!HisConfigCFG.IsShowInPatientPrescriptionOption)
                    {
                        // bỏ thuốc/ vật tư thuộc đơn nội trú
                        var ListSereServByTreatmentDNT = this.sereServByTreatment.Where(o => o.TDL_SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT && (o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT)).ToList();
                        if (ListSereServByTreatmentDNT != null && ListSereServByTreatmentDNT.Count > 0 && this.sereServByTreatment != null && this.sereServByTreatment.Count > 0)
                        {
                            List<long> ListSereServByTreatmentDNTIds = ListSereServByTreatmentDNT.Select(o => o.ID).ToList();
                            this.sereServByTreatment = this.sereServByTreatment.Where(o => !ListSereServByTreatmentDNTIds.Contains(o.ID)).ToList();
                        }
                        if (this.sereServByTreatment == null || this.sereServByTreatment.Count == 0)
                            return;
                    }

                    param = new CommonParam();
                    MOS.Filter.HisSereServBillFilter hisSereServBillFilter = new HisSereServBillFilter();
                    hisSereServBillFilter.TDL_TREATMENT_ID = this.treatmentId;
                    hisSereServBillFilter.IS_NOT_CANCEL = true;
                    var sereServBills = await new BackendAdapter(param).GetAsync<List<HIS_SERE_SERV_BILL>>("api/HisSereServBill/Get", ApiConsumer.ApiConsumers.MosConsumer, hisSereServBillFilter, param);

                    if (sereServBills != null && sereServBills.Count > 0)
                    {
                        List<long> SereServBillIds = sereServBills.Select(o => o.SERE_SERV_ID).ToList();
                        // lọc các sereServ đã thanh toán
                        this.sereServByTreatment = this.sereServByTreatment.Where(o => !SereServBillIds.Contains(o.ID)).ToList();
                    }

                    // kiểm tra có trong his_sere_serv_debt chua neu co roi thi bo qua
                    if (this.sereServByTreatment == null || this.sereServByTreatment.Count == 0)
                        return;

                    MOS.Filter.HisSereServDebtFilter sereServDebtFilter = new HisSereServDebtFilter();
                    sereServDebtFilter.TDL_TREATMENT_ID = this.treatmentId;
                    var sereServDebtList = new BackendAdapter(param).Get<List<HIS_SERE_SERV_DEBT>>("api/HisSereServDebt/Get", ApiConsumer.ApiConsumers.MosConsumer, sereServDebtFilter, null);
                    if (sereServDebtList != null && sereServDebtList.Count > 0)
                    {
                        sereServDebtList = sereServDebtList.Where(o => o.IS_CANCEL != 1).ToList();

                        this.sereServByTreatment = sereServDebtList != null && sereServDebtList.Count > 0
                            ? this.sereServByTreatment.Where(o => !sereServDebtList.Select(p => p.SERE_SERV_ID).Contains(o.ID)).ToList()
                            : this.sereServByTreatment;
                    }

                    //FilterSereServBill(ref this.sereServByTreatment);
                    //if (this.sereServByTreatment == null || this.sereServByTreatment.Count == 0)
                    //    return;               

                    param = new CommonParam();
                    MOS.Filter.HisSereServDepositFilter sereServDepositFilter = new HisSereServDepositFilter();
                    sereServDepositFilter.TDL_TREATMENT_ID = treatmentId;
                    var sereServDeposits = await new BackendAdapter(param).GetAsync<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_DEPOSIT>>("api/HisSereServDeposit/Get", ApiConsumer.ApiConsumers.MosConsumer, sereServDepositFilter, param);

                    List<V_HIS_SERE_SERV_5> sereServByTreatmentProcess = new List<V_HIS_SERE_SERV_5>();

                    this.FilterSereServDepositAndRepay(ref this.sereServByTreatment, sereServDeposits);

                    if (this.sereServByTreatment != null && this.sereServByTreatment.Count > 0 && chkHidePatientPrice.Checked)
                        sereServByTreatment = sereServByTreatment.Where(o => o.VIR_TOTAL_PATIENT_PRICE > 0).ToList();

                    if (this.ssTreeProcessor == null)
                    {
                        this.InitSereServTree();
                    }
                    else
                    {
                        this.ssTreeProcessor.Reload(this.ucSereServTree, this.sereServByTreatment);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CalulateVirTotalPatientPrice(List<V_HIS_SERE_SERV_5> listCheckeds)
        {
            try
            {
                foreach (var item in listCheckeds)
                {
                    if (item != null && (item.PARENT_ID.HasValue))
                    {
                        if (item.PATIENT_TYPE_ID == HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<long>("MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT"))
                        {
                            MOS.EFMODEL.DataModels.V_HIS_SERE_SERV sereServ = new V_HIS_SERE_SERV();
                            AutoMapper.Mapper.CreateMap<V_HIS_SERE_SERV_5, V_HIS_SERE_SERV>();
                            sereServ = AutoMapper.Mapper.Map<V_HIS_SERE_SERV>(item);
                            if (SetDefaultDepositPrice == 1 || (SetDefaultDepositPrice == 3 && hisTreatment.TDL_TREATMENT_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU))
                            {
                                item.VIR_TOTAL_PATIENT_PRICE = MOS.LibraryHein.Bhyt.BhytPriceCalculator.DefaultPatientPriceForOutBhyt(sereServ) ?? 0;
                            }
                            else if (SetDefaultDepositPrice == 2)
                            {
                                item.VIR_TOTAL_PATIENT_PRICE = item.VIR_TOTAL_PRICE ?? 0;
                            }
                            else
                            {
                                item.VIR_TOTAL_PATIENT_PRICE = ((item.VIR_TOTAL_PATIENT_PRICE != null && !String.IsNullOrEmpty(item.VIR_TOTAL_PATIENT_PRICE.ToString())) ? Convert.ToDecimal(item.VIR_TOTAL_PATIENT_PRICE) : 0);
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

        private bool CheckSSB(List<HIS_SERE_SERV_BILL> sereServBills, V_HIS_SERE_SERV_5 sereserv5)
        {
            bool valid = true;
            var checkSereServBill = sereServBills.FirstOrDefault(o => o.SERE_SERV_ID == sereserv5.ID);
            if (checkSereServBill != null && checkSereServBill.IS_CANCEL != 1)
            {
                valid = false;
            }
            return valid;
        }

        private List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_5> GetSereByTreatmentId()
        {
            List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_5> rs = null;
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisSereServView5Filter sereServFilter = new HisSereServView5Filter();
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("GetSereByTreatmentId() this.hisTreatment ", this.hisTreatment));
                sereServFilter.TDL_TREATMENT_ID = this.hisTreatment.ID;
                sereServFilter.IS_EXPEND = false;
                var apiData = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_5>>("/api/HisSereServ/GetView5", ApiConsumers.MosConsumer, sereServFilter, null);
                if (apiData != null && apiData.Count > 0)
                {
                    rs = apiData.Where(o => o.AMOUNT > 0).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
            return rs;
        }

        private List<HIS_SERE_SERV_BILL> GetSereServBillBySSID(List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_5> ListSereServByTreatment)
        {
            int start = 0;
            CommonParam param = new CommonParam();
            int count = ListSereServByTreatment.Count;
            List<HIS_SERE_SERV_BILL> sereServBills = new List<HIS_SERE_SERV_BILL>();
            while (count > 0)
            {
                int limit = (count <= ManagerConstant.MAX_REQUEST_LENGTH_PARAM) ? count : ManagerConstant.MAX_REQUEST_LENGTH_PARAM;
                List<long> sereServLimitIds = ListSereServByTreatment.Select(o => o.ID).Skip(start).Take(limit).ToList();
                MOS.Filter.HisSereServBillFilter sereServBillFilter = new HisSereServBillFilter();
                sereServBillFilter.SERE_SERV_IDs = sereServLimitIds;
                var sereServBillLimits = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_SERE_SERV_BILL>>(RequestUri.HIS_SERE_SERV_BILL_GET, ApiConsumer.ApiConsumers.MosConsumer, sereServBillFilter, param);
                sereServBills.AddRange(sereServBillLimits);
                start += ManagerConstant.MAX_REQUEST_LENGTH_PARAM;
                count -= ManagerConstant.MAX_REQUEST_LENGTH_PARAM;
            }

            return sereServBills;
        }

        private void LoadPayForm()
        {
            try
            {
                this.ListPayForm = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PAY_FORM>();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        /// <summary>
        /// lấy các phòng tài chính mà user đang đăng nhập được gán quyền 
        /// </summary>
        private void LoadCashierRoom()
        {
            try
            {
                CommonParam param = new CommonParam();
                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                var userRooms = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_USER_ROOM>()
                    .Where(o =>
                        o.LOGINNAME == loginName
                        && o.BRANCH_ID == this.branchId
                        && (o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)).ToList();

                if (userRooms != null && userRooms.Count > 0)
                {
                    List<long> userRoomIds = new List<long>();
                    userRoomIds = userRooms.Select(o => o.ROOM_ID).Distinct().ToList();

                    this.ListCashierRoom = BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_CASHIER_ROOM>()
                        .Where(o =>
                            o.ROOM_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_ROOM_TYPE.ID__TN
                            && (o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE)
                            && userRoomIds.Contains(o.ROOM_ID)).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateDataFormTransactionDepositToDTO(ref HisTransactionDepositSDO transactionData, MOS.EFMODEL.DataModels.V_HIS_TREATMENT_FEE treatment)
        {
            try
            {
                if (transactionData == null)
                {
                    transactionData = new HisTransactionDepositSDO();
                    transactionData.Transaction = new HIS_TRANSACTION();
                }

                transactionData.SereServDeposits = new List<HIS_SERE_SERV_DEPOSIT>();
                if (this.cashierRoomId > 0)
                {
                    var cashierRoom = ListCashierRoom.FirstOrDefault(o => o.ID == this.cashierRoomId);
                    transactionData.RequestRoomId = cashierRoom.ROOM_ID;
                }
                transactionData.Transaction.AMOUNT = (txtAmount.Tag != null && !string.IsNullOrEmpty(txtAmount.Tag.ToString())) ? Convert.ToDecimal(txtAmount.Tag.ToString()) : 0;
                transactionData.Transaction.TRANSACTION_TYPE_ID = IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TU;
                if (cboAccountBook.EditValue != null)
                {
                    var accountBook = this.ListAccountBook.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboAccountBook.EditValue.ToString()));//Sửa lại
                    transactionData.Transaction.ACCOUNT_BOOK_ID = accountBook.ID;
                    if (accountBook.IS_NOT_GEN_TRANSACTION_ORDER == 1)
                    {
                        transactionData.Transaction.NUM_ORDER = (long)(spinTongTuDen.Value);
                    }
                }
                if (cboPayForm.EditValue != null)
                {
                    transactionData.Transaction.PAY_FORM_ID = (Inventec.Common.TypeConvert.Parse.ToInt64((cboPayForm.EditValue ?? "").ToString()));

                }
                transactionData.Transaction.CASHIER_ROOM_ID = this.cashierRoomId;
                if (dtTransactionTime.EditValue != null && dtTransactionTime.DateTime != DateTime.MinValue)
                    transactionData.Transaction.TRANSACTION_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(
                        Convert.ToDateTime(dtTransactionTime.EditValue).ToString("yyyyMMddHHmm") + "00");
                if (treatment != null)
                {
                    transactionData.Transaction.TREATMENT_ID = treatment.ID;
                    transactionData.Transaction.TDL_TREATMENT_CODE = treatment.TREATMENT_CODE;
                }
                transactionData.Transaction.DESCRIPTION = txtDescription.Text;
                List<SereServADO> listCheckeds = ssTreeProcessor.GetListCheck(ucSereServTree);
                SetSereServToDataTransfer(listCheckeds, transactionData);
                if (transactionData.Transaction != null && transactionData.Transaction.PAY_FORM_ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TMCK && spinTransferAmount.EditValue != null)
                {
                    transactionData.Transaction.TRANSFER_AMOUNT = spinTransferAmount.Value;
                }
                else if (transactionData.Transaction != null && transactionData.Transaction.PAY_FORM_ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TMQT && spinTransferAmount.EditValue != null)
                {
                    transactionData.Transaction.SWIPE_AMOUNT = spinTransferAmount.Value;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadPatientType()
        {
            try
            {
                ListPatientType = BackendDataWorker.Get<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE>();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadAccountBook()
        {
            try
            {
                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                HisAccountBookViewFilter acFilter = new HisAccountBookViewFilter();
                acFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                acFilter.CASHIER_ROOM_ID = this.cashierRoomId;//Kiểm tra sổ còn hay k
                acFilter.LOGINNAME = loginName;//Kiểm tra sổ còn hay k
                acFilter.FOR_DEPOSIT = true;
                acFilter.IS_OUT_OF_BILL = false;
                acFilter.ORDER_DIRECTION = "DESC";
                acFilter.ORDER_FIELD = "ID";
                CommonParam paramCommon = new CommonParam();
                this.ListAccountBook = new BackendAdapter(paramCommon).Get<List<V_HIS_ACCOUNT_BOOK>>("api/HisAccountBook/GetView", ApiConsumer.ApiConsumers.MosConsumer, acFilter, paramCommon);
                this.ListAccountBook = this.ListAccountBook.Where(o => o.WORKING_SHIFT_ID == null || o.WORKING_SHIFT_ID == (HIS.Desktop.LocalStorage.LocalData.WorkPlace.WorkInfoSDO.WorkingShiftId ?? 0)).ToList();
                if (HisConfigCFG.ShowServerTimeByDefault != null && HisConfigCFG.ShowServerTimeByDefault.Equals("1"))
                {
                    dtTransactionTime.DateTime = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(paramCommon.Now) ?? DateTime.MinValue;
                }
                else
                {
                    dtTransactionTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.Now() ?? 0);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboPayForm()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("PAY_FORM_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("PAY_FORM_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("PAY_FORM_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(this.cboPayForm, this.ListPayForm, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitComboAccountBook()
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("ACCOUNT_BOOK_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("ACCOUNT_BOOK_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("ACCOUNT_BOOK_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(cboAccountBook, this.ListAccountBook, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        /// <summary>
        /// neu = true thi btnSave duoc enable
        /// neu = false thi btnAdd duoc enable
        /// </summary>
        /// <param name="isEnable"></param>
        private void EnableButton(bool isEnable)
        {
            try
            {
                btnAdd.Enabled = !isEnable;
                btnSave.Enabled = isEnable;
                btnSaveAndPrint.Enabled = isEnable;
                btnSavePrintAndTrans.Enabled = isEnable;
                btnAddSign.Enabled = isEnable;
                ddbPrint.Enabled = !isEnable;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void InitSereServTree()
        {
            try
            {
                this.ssTreeProcessor = new UC.SereServTree.SereServTreeProcessor();
                SereServTreeADO ado = new SereServTreeADO();
                ado.IsShowSearchPanel = false;
                ado.IsShowCheckNode = true;
                ado.isAdvance = true;
                ado.SereServs = this.sereServByTreatment;
                ado.SereServTree_CustomUnboundColumnData = sereServTree_CustomUnboundColumnData;

                ado.SereServTreeColumns = new List<SereServTreeColumn>();
                ado.SelectImageCollection = this.imageCollection1;
                ado.SereServTree_CustomDrawNodeCell = treeSereServ_CustomDrawNodeCell;
                ado.SereServTree_AfterCheck = sereServTree_AfterCheck;
                ado.SereServTreeForBill_BeforeCheck = sereServTree_BeforeCheck;
                ado.sereServTree_ShowingEditor = sereServTree_ShowingEditorDG;
                ado.SereServTree_CheckAllNode = sereServTree_CheckAllNode;


                ado.LayoutSereServExpend = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_DEPOSIT_SERVICE__TREE_SERE_SERV__LAYOUT_SERE_SERV_EXPEND", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                //Cột Tên dịch vụ
                SereServTreeColumn serviceNameCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_DEPOSIT_SERVICE__TREE_SERE_SERV__COLUMN_SERVICE_NAME", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "TDL_SERVICE_NAME", 200, false);
                serviceNameCol.VisibleIndex = 0;
                ado.SereServTreeColumns.Add(serviceNameCol);
                //Cột Số lượng
                SereServTreeColumn amountCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_DEPOSIT_SERVICE__TREE_SERE_SERV__COLUMN_AMOUNT", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "AMOUNT_PLUS_STR", 60, false);
                amountCol.VisibleIndex = 1;
                amountCol.Format = new DevExpress.Utils.FormatInfo();
                amountCol.Format.FormatString = "#,##0.00";
                amountCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                amountCol.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;
                ado.SereServTreeColumns.Add(amountCol);
                //Cột Đơn giá
                SereServTreeColumn virPriceCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_DEPOSIT_SERVICE__TREE_SERE_SERV__COLUMN_VIR_PRICE", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "VIR_PRICE_DISPLAY", 110, false);
                virPriceCol.VisibleIndex = 2;
                virPriceCol.Format = new DevExpress.Utils.FormatInfo();
                virPriceCol.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;
                ado.SereServTreeColumns.Add(virPriceCol);
                //Cột thành tiền
                SereServTreeColumn virTotalPriceCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_DEPOSIT_SERVICE__TREE_SERE_SERV__COLUMN_VIR_TOTAL_PRICE", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "VIR_TOTAL_PRICE_DISPLAY", 110, false);
                virTotalPriceCol.VisibleIndex = 3;
                virTotalPriceCol.Format = new DevExpress.Utils.FormatInfo();
                virTotalPriceCol.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;
                //virTotalPriceCol.Format.FormatString = "#,##0.0000";
                //virTotalPriceCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.SereServTreeColumns.Add(virTotalPriceCol);
                //Cột Đồng chi trả
                SereServTreeColumn virTotalHeinPriceCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_DEPOSIT_SERVICE__TREE_SERE_SERV__COLUMN_VIR_TOTAL_HEIN_PRICE", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "VIR_TOTAL_HEIN_PRICE_DISPLAY", 110, false);
                virTotalHeinPriceCol.VisibleIndex = 4;
                virTotalHeinPriceCol.Format = new DevExpress.Utils.FormatInfo();
                virTotalHeinPriceCol.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;
                //virTotalHeinPriceCol.Format.FormatString = "#,##0.0000";
                //virTotalHeinPriceCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.SereServTreeColumns.Add(virTotalHeinPriceCol);

                SereServTreeColumn virTotalPatientPriceCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_DEPOSIT_SERVICE__TREE_SERE_SERV__COLUMN_VIR_TOTAL_PATIENT_PRICE", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "VIR_TOTAL_PATIENT_PRICE_DISPLAY", 110, false);
                virTotalPatientPriceCol.VisibleIndex = 5;
                virTotalPatientPriceCol.Format = new DevExpress.Utils.FormatInfo();
                virTotalPatientPriceCol.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;
                //virTotalPatientPriceCol.Format.FormatString = "#,##0.0000";
                //virTotalPatientPriceCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.SereServTreeColumns.Add(virTotalPatientPriceCol);
                //Chiếu khấu
                SereServTreeColumn virDiscountCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_DEPOSIT_SERVICE__TREE_SERE_SERV__COLUMN_DISCOUNT", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "DISCOUNT_DISPLAY", 110, false);
                virDiscountCol.VisibleIndex = 6;
                virDiscountCol.Format = new DevExpress.Utils.FormatInfo();
                virDiscountCol.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;
                ado.SereServTreeColumns.Add(virDiscountCol);
                //Hao phí
                SereServTreeColumn virIsExpendCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_DEPOSIT_SERVICE__TREE_SERE_SERV__COLUMN_IS_EXPEND", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "IsExpend", 60, false);
                virIsExpendCol.VisibleIndex = 7;
                ado.SereServTreeColumns.Add(virIsExpendCol);
                //
                SereServTreeColumn virVatRatioCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_DEPOSIT_SERVICE__TREE_SERE_SERV__COLUMN_VAT_RATIO", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "VAT", 100, false);
                virVatRatioCol.VisibleIndex = 8;
                virVatRatioCol.Format = new DevExpress.Utils.FormatInfo();
                virVatRatioCol.Format.FormatString = "#,##0.00";
                virVatRatioCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.SereServTreeColumns.Add(virVatRatioCol);

                SereServTreeColumn serviceCodeCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_DEPOSIT_SERVICE__TREE_SERE_SERV__COLUMN_SERVICE_CODE", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "TDL_SERVICE_CODE", 100, false);
                serviceCodeCol.VisibleIndex = 9;
                ado.SereServTreeColumns.Add(serviceCodeCol);

                SereServTreeColumn serviceReqCodeCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_DEPOSIT_SERVICE__TREE_SERE_SERV__COLUMN_SERVICE_REQ_CODE", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "TDL_SERVICE_REQ_CODE", 100, false);
                serviceReqCodeCol.VisibleIndex = 10;
                ado.SereServTreeColumns.Add(serviceReqCodeCol);
                //SereServTreeColumn TRANSACTIONCodeCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_DEPOSIT_SERVICE__TREE_SERE_SERV__COLUMN_TRANSACTION_CODE", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "TRANSACTION_CODE", 100, false);
                //TRANSACTIONCodeCol.VisibleIndex = 11;
                //ado.SereServTreeColumns.Add(TRANSACTIONCodeCol);
                SereServTreeColumn intructionTime = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_DEPOSIT_SERVICE__TREE_SERE_SERV__COLUMN_INTRUCTION_TIME", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "TDL_INTRUCTION_TIME_STR", 130, false);
                intructionTime.VisibleIndex = 11;
                intructionTime.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;
                ado.SereServTreeColumns.Add(intructionTime);

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
                Inventec.Common.Logging.LogSystem.Debug("sereServTree_ShowingEditorDG this.IsDepositAll " + this.IsDepositAll);
                var nodeData = node.TreeList.GetDataRecordByNode(node);
                if (nodeData != null && this.IsDepositAll.HasValue && this.IsDepositAll.Value == true)
                {
                    Inventec.Common.Logging.LogSystem.Debug("((TreeList)sender).ActiveEditor.Properties.ReadOnly = " + this.IsDepositAll);
                    ((TreeList)sender).ActiveEditor.Properties.ReadOnly = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private string AddStringByConfig(int num)
        {
            string str = "";
            try
            {
                if (num > 0)
                {
                    for (int i = 1; i <= num; i++)
                    {
                        str += "0";
                    }
                }
                else
                {
                    return str = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return str = "";
            }
            return str;
        }

        private void sereServTree_CustomUnboundColumnData(SereServADO data, DevExpress.XtraTreeList.TreeListCustomColumnDataEventArgs e)
        {
            try
            {

                if (data != null && !e.Node.HasChildren)
                {
                    if (e.Column.FieldName == "VIR_TOTAL_PRICE_DISPLAY")
                    {
                        e.Value = ConvertNumberToString(data.VIR_TOTAL_PRICE ?? 0);
                    }
                    else if (e.Column.FieldName == "VIR_TOTAL_HEIN_PRICE_DISPLAY")
                    {
                        e.Value = ConvertNumberToString(data.VIR_TOTAL_HEIN_PRICE ?? 0);
                    }
                    else if (e.Column.FieldName == "VIR_TOTAL_PATIENT_PRICE_DISPLAY")
                    {
                        e.Value = ConvertNumberToString(data.VIR_TOTAL_PATIENT_PRICE ?? 0);
                    }
                    else if (e.Column.FieldName == "VIR_PRICE_DISPLAY")
                    {
                        e.Value = ConvertNumberToString(data.VIR_PRICE ?? 0);
                    }
                    else if (e.Column.FieldName == "DISCOUNT_DISPLAY")
                    {
                        e.Value = ConvertNumberToString(data.DISCOUNT ?? 0);
                    }
                    if (e.Column.FieldName == "AMOUNT_PLUS_STR")
                    {
                        e.Value = ConvertNumberToString(data.AMOUNT);
                    }
                    if (e.Column.FieldName == "TDL_INTRUCTION_TIME_STR")
                    {
                        e.Value = Inventec.Common.DateTime.Convert.TimeNumberToTimeStringWithoutSecond(data.TDL_INTRUCTION_TIME);
                    }
                }
                if (data != null && e.Node.HasChildren && data.VIR_TOTAL_PRICE > 0)
                {
                    if (e.Column.FieldName == "VIR_TOTAL_PRICE_DISPLAY")
                    {
                        e.Value = ConvertNumberToString(data.VIR_TOTAL_PRICE ?? 0);
                    }
                }
                if (data != null && e.Node.HasChildren && data.VIR_TOTAL_PATIENT_PRICE > 0)
                {
                    if (e.Column.FieldName == "VIR_TOTAL_PATIENT_PRICE_DISPLAY")
                    {
                        e.Value = ConvertNumberToString(data.VIR_TOTAL_PATIENT_PRICE ?? 0);
                    }
                }
                if (data != null && e.Node.HasChildren && data.AMOUNT > 0)
                {
                    if (e.Column.FieldName == "AMOUNT_PLUS_STR")
                    {
                        e.Value = ConvertNumberToString(data.AMOUNT);
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void sereServTree_BeforeCheck(TreeListNode node, DevExpress.XtraTreeList.CheckNodeEventArgs e)
        {
            try
            {
                if (node != null)
                {
                    var nodeData = (SereServADO)node.TreeList.GetDataRecordByNode(node);
                    if (nodeData != null && this.IsDepositAll.HasValue && this.IsDepositAll.Value)
                    {
                        e.CanCheck = false;
                        node.CheckAll();
                        return;
                    }
                    //while (node.ParentNode != null)
                    //{
                    //    node = node.ParentNode;
                    //    bool valid = false;
                    //    foreach (DevExpress.XtraTreeList.Nodes.TreeListNode item in node.Nodes)
                    //    {
                    //        if (item.CheckState == CheckState.Checked || item.CheckState == CheckState.Indeterminate)
                    //        {
                    //            valid = true;
                    //            break;
                    //        }
                    //    }
                    //    if (valid)
                    //    {
                    //        node.CheckState = CheckState.Checked;
                    //    }
                    //    else
                    //    {
                    //        node.CheckState = CheckState.Unchecked;
                    //    }
                    //}
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CalCulateTotalAmountDeposit()
        {
            try
            {
                List<SereServADO> listCheckeds = ssTreeProcessor.GetListCheck(ucSereServTree);

                ChangeCheckedNodes(listCheckeds);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void sereServTree_AfterCheck(TreeListNode node, SereServADO data)
        {
            try
            {
                CalCulateTotalAmountDeposit();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void sereServTree_CheckAllNode(TreeListNodes treeListNodes)
        {
            try
            {
                if (treeListNodes != null)
                {
                    foreach (TreeListNode node in treeListNodes)
                    {
                        node.CheckAll();
                        CheckNode(node);
                    }
                }
                CalCulateTotalAmountDeposit();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CheckNode(TreeListNode node)
        {
            try
            {
                if (node != null)
                {
                    foreach (TreeListNode childNode in node.Nodes)
                    {
                        childNode.CheckAll();
                        CheckNode(childNode);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hisDeposit"></param>
        /// 
        private void FillDataToControl(MOS.EFMODEL.DataModels.V_HIS_TRANSACTION hisDeposit)
        {
            try
            {
                if (hisDeposit != null)
                {
                    cboAccountBook.EditValue = hisDeposit.ACCOUNT_BOOK_ID;
                    txtAccountBookCode.Text = hisDeposit.TRANSACTION_CODE;
                    txtAmount.Tag = hisDeposit.AMOUNT;
                    txtAmount.Text = Inventec.Common.Number.Convert.NumberToString(hisDeposit.AMOUNT, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                    if (ListPayForm == null || ListPayForm.Count == 0)
                    {
                        ListPayForm = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PAY_FORM>();
                    }

                    var pf = ListPayForm.FirstOrDefault(o => o.ID == hisDeposit.PAY_FORM_ID);
                    if (pf != null)
                    {
                        cboPayForm.EditValue = pf.ID;
                        //txtPayFormCode.Text = pf.PAY_FORM_CODE;
                    }
                    txtDescription.Text = hisDeposit.DESCRIPTION;
                    txtTransactionCode.Text = hisDeposit.TRANSACTION_CODE;
                }
                else
                {
                    cboAccountBook.EditValue = null;
                    txtAccountBookCode.Text = "";
                    txtAmount.Tag = 0;
                    txtAmount.Text = "0";
                    txtDescription.Text = "";
                    txtTransactionCode.Text = "";
                    spinTransferAmount.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangeCheckedNodes(List<SereServADO> listCheckeds)
        {
            try
            {
                decimal amountSs = 0;
                txtAmount.Tag = 0;
                txtAmount.Text = "0";
                totalAmountDeposit = 0;
                foreach (var item in listCheckeds)
                {
                    if (item != null && (item.IsLeaf ?? false))
                    {
                        if (item.PATIENT_TYPE_ID == HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<long>("MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT"))
                        {
                            MOS.EFMODEL.DataModels.V_HIS_SERE_SERV sereServ = new V_HIS_SERE_SERV();
                            AutoMapper.Mapper.CreateMap<SereServADO, V_HIS_SERE_SERV>();
                            sereServ = AutoMapper.Mapper.Map<V_HIS_SERE_SERV>(item);
                            decimal itemSsAmount = 0;
                            if (SetDefaultDepositPrice == 1 || (SetDefaultDepositPrice == 3 && hisTreatment.TDL_TREATMENT_TYPE_ID != IMSys.DbConfig.HIS_RS.HIS_TREATMENT_TYPE.ID__DTNOITRU))
                            {
                                itemSsAmount = MOS.LibraryHein.Bhyt.BhytPriceCalculator.DefaultPatientPriceForOutBhyt(sereServ) ?? 0;
                            }
                            else if (SetDefaultDepositPrice == 2)
                            {
                                itemSsAmount = item.VIR_TOTAL_PRICE ?? 0;
                            }
                            else
                            {
                                itemSsAmount = ((item.VIR_TOTAL_PATIENT_PRICE != null && !String.IsNullOrEmpty(item.VIR_TOTAL_PATIENT_PRICE.ToString())) ? Convert.ToDecimal(item.VIR_TOTAL_PATIENT_PRICE) : 0);
                            }

                            amountSs += itemSsAmount;
                            txtAmount.Tag = amountSs;
                            txtAmount.Text = Inventec.Common.Number.Convert.NumberToString(amountSs, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                            totalAmountDeposit += itemSsAmount;
                        }
                        else
                        {
                            decimal totalPatientPrice = ((item.VIR_TOTAL_PATIENT_PRICE != null && !String.IsNullOrEmpty(item.VIR_TOTAL_PATIENT_PRICE.ToString())) ? Convert.ToDecimal(item.VIR_TOTAL_PATIENT_PRICE) : 0);
                            amountSs += totalPatientPrice;
                            txtAmount.Tag = amountSs;
                            txtAmount.Text = Inventec.Common.Number.Convert.NumberToString(amountSs, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                            totalAmountDeposit += totalPatientPrice;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //mặc định sổ mới nhất
        private void SetDefaultAccountBookForUser(V_HIS_ACCOUNT_BOOK accountBookDefault)
        {
            try
            {
                cboAccountBook.EditValue = accountBookDefault.ID;
                txtAccountBookCode.Text = accountBookDefault.ACCOUNT_BOOK_CODE;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private async Task SetDefaultPayFormForUser()
        {
            try
            {
                if (BackendDataWorker.IsExistsKey<HIS_PAY_FORM>())
                {
                    ListPayForm = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PAY_FORM>();
                }
                else
                {
                    CommonParam paramCommon = new CommonParam();
                    dynamic filter = new System.Dynamic.ExpandoObject();
                    ListPayForm = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.HIS_PAY_FORM>>("api/HisPayForm/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                    if (ListPayForm != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_PAY_FORM), ListPayForm, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
                }

                if (ListPayForm != null && ListPayForm.Count > 0)
                {
                    ListPayForm = ListPayForm.Where(o => o.IS_ACTIVE == 1).ToList();
                }
                if (string.IsNullOrEmpty(HisConfigCFG.CashierRoomPaymentOption) || (HisConfigCFG.CashierRoomPaymentOption != null && !CashierRoomPaymentOption.IsDefined(typeof(CashierRoomPaymentOption), Int32.Parse(HisConfigCFG.CashierRoomPaymentOption))))
                {
                    ListPayForm = ListPayForm.Where(o => o.ID != IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__THE).ToList();
                }

                InitComboPayForm();

                if (ListPayForm != null && ListPayForm.Count > 0)
                {
                    HIS_PAY_FORM payFormDefault = null;
                    if (HisConfigCFG.PayForm__DefaultOption)
                    {
                        payFormDefault = ListPayForm.FirstOrDefault(o => o.ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__THE);
                        cboPayForm.Enabled = false;
                        //txtPayFormCode.Enabled = false;
                    }
                    else
                    {
                        var PayFormMinByCode = ListPayForm.OrderBy(o => o.PAY_FORM_CODE);
                        payFormDefault = PayFormMinByCode.FirstOrDefault();
                    }

                    if (payFormDefault != null)
                    {
                        cboPayForm.EditValue = payFormDefault.ID;
                        ChangeTitle();
                        //txtPayFormCode.Text = payFormDefault.PAY_FORM_CODE;
                        CheckPayFormTienMatChuyenKhoan(payFormDefault);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void RemoveControlError()
        {
            try
            {
                positionHandle = -1;
                dxValidationProvider1.RemoveControlError(cboAccountBook);
                dxValidationProvider1.RemoveControlError(cboPayForm);
                dxValidationProvider1.RemoveControlError(txtAmount);
                dxValidationProvider1.RemoveControlError(spinTransferAmount);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private bool OneOfChildsIsChecked(TreeListNode node)
        {
            bool result = false;
            foreach (TreeListNode item in node.Nodes)
            {
                if (item.CheckState == CheckState.Checked || item.CheckState == CheckState.Indeterminate)
                {
                    result = true;
                }
            }
            return result;
        }


        // định dạng số tiền theo key
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
        #endregion

        #region public function
        public void Save()
        {
            btnSave_Click(null, null);
        }

        public void Add()
        {
            btnAdd_Click(null, null);
        }

        #endregion

        #region Event handler

        #region click
        private void btnCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {

                if (DevExpress.XtraEditors.XtraMessageBox.Show(MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao), MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    if (hisDepositSDO != null)
                    {
                        this.hisTreatment.CREATOR = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                        this.hisTreatment.GROUP_CODE = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetGroupCode();
                        success = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>(HisRequestUriStore.HIS_DEPOSIT_DELETE, ApiConsumers.MosConsumer, this.hisDeposit, param);
                        if (success)
                        {
                            FillDataToControl(this.hisDeposit);
                        }
                    }
                    MessageManager.Show(param, success);
                    SessionManager.ProcessTokenLost(param);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void reloadTreeSereServ()
        {
            try
            {
                WaitingManager.Show();
                LoadSereServByTreatment();
                //ssTreeProcessor.Reload(this.ucSereServTree, this.sereServByTreatment);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                FillDataToControl(null);
                reloadTreeSereServ();
                EnableButton(true);
                RemoveControlError();
                LoadAccountBook();
                V_HIS_ACCOUNT_BOOK accountBookDefault = null;
                //chọn mặc định sổ nếu có sổ tương ứng
                if (GlobalVariables.LastAccountBook != null && GlobalVariables.LastAccountBook.Count > 0)
                {
                    var lstBook = ListAccountBook.Where(o => GlobalVariables.LastAccountBook.Select(s => s.ID).Contains(o.ID)).ToList();
                    if (lstBook != null && lstBook.Count > 0)
                    {
                        accountBookDefault = lstBook.OrderByDescending(o => o.ID).First();
                    }
                }

                if (accountBookDefault == null) accountBookDefault = ListAccountBook.FirstOrDefault();
                SetDefaultAccountBookForUser(accountBookDefault);
                SetDataToDicNumOrderInAccountBook(accountBookDefault);
                txtAccountBookCode.SelectAll();
                txtAccountBookCode.Focus();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        // gọi sang WCF tạm ứng qua thẻ
        CARD.WCF.DCO.WcfSaleDCO DepositCard(ref CARD.WCF.DCO.WcfSaleDCO DepositDCO)
        {
            CARD.WCF.DCO.WcfSaleDCO result = null;
            CommonParam param = new CommonParam();
            try
            {
                // gọi api HisCard/Get để lấy về serviceCodes
                if (this.hisTreatment != null)
                {
                    MOS.Filter.HisCardFilter cardFilter = new HisCardFilter();
                    cardFilter.PATIENT_ID = this.hisTreatment.PATIENT_ID;
                    var HisCards = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_CARD>>("api/HisCard/Get", ApiConsumer.ApiConsumers.MosConsumer, cardFilter, param);
                    if (HisCards != null && HisCards.Count > 0)
                    {
                        DepositDCO.ServiceCodes = HisCards.Select(o => o.SERVICE_CODE).ToArray();

                    }
                    CARD.WCF.Client.TransactionClient.TransactionClientManager transactionClientManager = new CARD.WCF.Client.TransactionClient.TransactionClientManager();

                    result = transactionClientManager.Sale(DepositDCO);
                }
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

        private void SaveProcess(bool isSaveAndPrint, bool isShowTransation, bool isSaveAndSign)
        {
            try
            {
                //SetEnableButtonSave(false);
                CommonParam param = new CommonParam();
                bool success = false;
                positionHandle = -1;
                if (!dxValidationProvider1.Validate())
                {
                    SetEnableButtonSave(true);
                    return;
                }


                UpdateDataFormTransactionDepositToDTO(ref this.hisDepositSDO, this.hisTreatment);

                WaitingManager.Show();

                if (CheckValidForSave(param))
                {
                    bool IsOption2 = false;
                    //hinh thuc thanh toán
                    long payFormId = Convert.ToInt64(cboPayForm.EditValue);
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.hisDepositSDO.Transaction), this.hisDepositSDO.Transaction));
                    if (HisConfigCFG.CashierRoomPaymentOption == ((int)CashierRoomPaymentOption.Option1).ToString())
                    {
                        CARD.WCF.DCO.WcfSaleDCO saleDCO = null;

                        if (payFormId == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__THE)
                        {
                            // nếu hình thức thanh toán qua thẻ thì gọi WCF tab thẻ (POS)
                            // check depositSdo

                            var check = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>("api/HisTransaction/CheckDeposit", ApiConsumers.MosConsumer, this.hisDepositSDO, param);

                            if (!check)
                            {
                                WaitingManager.Hide();
                                MessageManager.Show(this, param, check);
                                return;
                            }

                            CARD.WCF.DCO.WcfSaleDCO wcfSaleDCO = new CARD.WCF.DCO.WcfSaleDCO();
                            wcfSaleDCO.Amount = (txtAmount.Tag != null && !string.IsNullOrEmpty(txtAmount.Tag.ToString())) ? Convert.ToDecimal(txtAmount.Tag.ToString()) : 0;
                            //DepositDCO.PinCode = this.txtPin.Text.Trim();
                            saleDCO = DepositCard(ref wcfSaleDCO);
                            // nếu gọi sang POS trả về false thì kết thúc
                            if (saleDCO == null || saleDCO.ResultCode == null || !saleDCO.ResultCode.Equals("00"))
                            {
                                success = false;
                                MappingErrorTHE mappingErrorTHE = new Config.MappingErrorTHE();
                                Inventec.Common.Logging.LogSystem.Info("depositDCO: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => saleDCO), saleDCO));

                                //param.Messages.Add(ResourceMessageLang.TamUngQuaTheThatBai);
                                if (saleDCO != null
                               && !String.IsNullOrWhiteSpace(saleDCO.ResultCode)
                               && mappingErrorTHE.dicMapping != null
                               && mappingErrorTHE.dicMapping.Count > 0
                               && mappingErrorTHE.dicMapping.ContainsKey(saleDCO.ResultCode))
                                {
                                    param.Messages.Add(mappingErrorTHE.dicMapping[saleDCO.ResultCode]);
                                }
                                else if (saleDCO != null && String.IsNullOrWhiteSpace(saleDCO.ResultCode))
                                {
                                    param.Messages.Add("Kiểm tra lại phần mềm kết nối thiết bị");
                                }
                                else if (saleDCO != null
                                    && !String.IsNullOrWhiteSpace(saleDCO.ResultCode)
                                    && mappingErrorTHE.dicMapping != null
                                    && mappingErrorTHE.dicMapping.Count > 0
                                    && !mappingErrorTHE.dicMapping.ContainsKey(saleDCO.ResultCode)
                                    )
                                {
                                    param.Messages.Add("Kiểm tra lại phần mềm kết nối thiết bị");
                                }
                                WaitingManager.Hide();
                                MessageManager.Show(this, param, false);
                                return;
                            }

                            //nếu giao dịch thanh toán qua thẻ co gui tra ve ket qua thì gửi thong tin len server
                            if (saleDCO != null)
                            {
                                this.hisDepositSDO.CardCode = saleDCO.TransServiceCode;
                                this.hisDepositSDO.Transaction.TIG_TRANSACTION_CODE = saleDCO.TransactionCode;
                                this.hisDepositSDO.Transaction.TIG_TRANSACTION_TIME = saleDCO.TransactionTime;
                            }
                        }

                        long money = 0;
                        Inventec.Common.Logging.LogSystem.Info("lấy tiền");
                        if (spinTransferAmount.EditValue != null && (payFormId == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TMQT))
                        {
                            money = (long)spinTransferAmount.Value;
                        }
                        else if (this.hisDepositSDO.Transaction.AMOUNT != 0 && payFormId == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__QUET_THE)
                        {
                            money = (long)this.hisDepositSDO.Transaction.AMOUNT;
                        }
                        Inventec.Common.Logging.LogSystem.Info(" money " + money);
                        if ((payFormId == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__QUET_THE || payFormId == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TMQT)
                        && chkConnectionPOS.Checked == true && money > 0)
                        {
                            //CommonParam checkParam = new CommonParam();
                            //var check = new Inventec.Common.Adapter.BackendAdapter(checkParam).Post<bool>("api/HisTransaction/CheckDeposit", ApiConsumers.MosConsumer, this.hisDepositSDO, checkParam);

                            //if (!check)
                            //{
                            //    WaitingManager.Hide();
                            //    MessageManager.Show(this, checkParam, check);
                            //    return;
                            //}

                            OpenAppPOS();
                            WcfRequest wc = new WcfRequest(); // Khởi tạo data

                            wc.AMOUNT = money; // Số tiền
                            wc.billId = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 20);
                            wc.creator = creator;
                            var json = JsonConvert.Serialize<WcfRequest>(wc);
                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => json), json));
                            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(json);
                            try
                            {
                                try
                                {
                                    if (cll == null)
                                    {
                                        cll = new WcfClient();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    success = false;
                                    chkConnectionPOS.Checked = false;
                                    XtraMessageBox.Show("Kiểm tra lại cấu hình NetTcpBinding_IService1", "Thông báo");
                                    Inventec.Common.Logging.LogSystem.Error(ex);
                                    return;
                                }
                                var result = cll.Sale(System.Convert.ToBase64String(plainTextBytes));
                                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => result), result));
                                if (result != null && result.RESPONSE_CODE == "00")
                                {
                                    this.hisDepositSDO.Transaction.POS_PAN = result.PAN;
                                    this.hisDepositSDO.Transaction.POS_CARD_HOLDER = result.NAME;
                                    this.hisDepositSDO.Transaction.POS_INVOICE = result.INVOICE.ToString();
                                    this.hisDepositSDO.Transaction.POS_RESULT_JSON = JsonConvert.Serialize<WcfRequest>(result);
                                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.hisDepositSDO.Transaction), this.hisDepositSDO.Transaction));
                                }
                                else
                                {
                                    if (result != null)
                                    {
                                        WaitingManager.Hide();
                                        if (DevExpress.XtraEditors.XtraMessageBox.
                                       Show("Tạm thu qua POS thất bại" + "(Mã lỗi: " + result.ERROR + ")", "Thông báo", System.Windows.Forms.MessageBoxButtons.OK) == System.Windows.Forms.DialogResult.OK)
                                            return;
                                    }
                                    WaitingManager.Hide();
                                    MessageManager.Show(this, param, false);
                                    return;
                                }
                            }
                            catch (Exception ex)
                            {
                                WaitingManager.Hide();
                                MessageManager.Show(this, param, false);
                                throw;
                            }
                        }
                    }
                    else if (HisConfigCFG.CashierRoomPaymentOption == ((int)CashierRoomPaymentOption.Option2).ToString())
                    {
                        if (Int64.Parse(cboPayForm.EditValue.ToString()) == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__THE)
                        {
                            if ((String.IsNullOrEmpty(txtLastDigitsOfBankCardCode.Text) || txtLastDigitsOfBankCardCode.Text.Length < 4))
                            {
                                WaitingManager.Hide();
                                if (DevExpress.XtraEditors.XtraMessageBox.Show(Base.ResourceMessageLang.ThieuThongTinTheNganHang, MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK) == DialogResult.OK)
                                {
                                    txtLastDigitsOfBankCardCode.Focus();
                                    txtLastDigitsOfBankCardCode.SelectAll();
                                    return;
                                }
                            }
                            else
                            {
                                hisDepositSDO.LastDigitsOfBankCardCode = txtLastDigitsOfBankCardCode.Text;
                                IsOption2 = true;
                            }
                        }
                        else if (Int64.Parse(cboPayForm.EditValue.ToString()) != IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__THE && hisTreatment.HAS_CARD == 1)
                        {
                            WaitingManager.Hide();
                            if (DevExpress.XtraEditors.XtraMessageBox.Show(Base.ResourceMessageLang.BnCoSuDungTheKhamChuaBenh, MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                cboPayForm.EditValue = IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__THE;
                                txtLastDigitsOfBankCardCode.Text = null;
                                lblAmountReturnToPatient.Text = null;
                                ChangeTitle();
                                txtLastDigitsOfBankCardCode.Focus();
                                txtLastDigitsOfBankCardCode.SelectAll();
                                return;
                            }
                        }
                    }

                    this.hisDepositSDO.IsCollected = this.IsDepositAll;

                    Inventec.Common.Logging.LogSystem.Debug("HisTransaction/CreateDeposit input: " + Inventec.Common.Logging.LogUtil.TraceData("", this.hisDepositSDO));
                    this.hisDeposit = new Inventec.Common.Adapter.BackendAdapter(param).Post<MOS.EFMODEL.DataModels.V_HIS_TRANSACTION>(HisRequestUriStore.HIS_DEPOSIT_CREATE, ApiConsumers.MosConsumer, this.hisDepositSDO, param);
                    if (this.hisDeposit != null)
                    {
                        if (IsOption2)
                        {
                            CheckCashierRoomPaymentOption2();
                        }
                        btnSaveAndPrint.Enabled = false;
                        btnSave.Enabled = false;
                        btnSavePrintAndTrans.Enabled = false;
                        success = true;
                        AddLastAccountToLocal();
                        InitComboAccountBook();
                        FillDataToControl(this.hisDeposit);
                        RefreshDataAfterSave();
                        EnableButton(false);
                        if(hisDeposit.PAY_FORM_ID == 8)btnQR.Enabled = true;
                        loadConfig();
                        if (hisDeposit.PAY_FORM_ID == 8 && hisDeposit.IS_ACTIVE == 0) CreateQR(hisDeposit, false);
                        V_HIS_ACCOUNT_BOOK accountBookDefault = null;
                        //chọn mặc định sổ nếu có sổ tương ứng
                        if (GlobalVariables.LastAccountBook != null && GlobalVariables.LastAccountBook.Count > 0)
                        {
                            var lstBook = ListAccountBook.Where(o => GlobalVariables.LastAccountBook.Select(s => s.ID).Contains(o.ID)).ToList();
                            if (lstBook != null && lstBook.Count > 0)
                            {
                                accountBookDefault = lstBook.OrderByDescending(o => o.ID).First();
                            }
                        }

                        if (accountBookDefault == null) accountBookDefault = ListAccountBook.FirstOrDefault();
                        //var accountBookDefault = ListUserAccountBook.FirstOrDefault();
                        SetDefaultAccountBookForUser(accountBookDefault);
                        UpdateDictionaryNumOrderAccountBook(accountBookDefault);
                        if (this.returnData != null)
                        {
                            this.returnData(true);
                        }
                    }
                    else
                    {
                        SetEnableButtonSave(true);
                    }

                    if (success && isSaveAndPrint)
                    {
                        this.isPrintNow = true;
                        Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__MPS000102, DelegateRunPrinter);
                        if (isShowTransation)
                        {
                            this.Close();
                            WaitingManager.Show();
                            List<object> listArgs = new List<object>();
                            LoadTreatment(hisTreatment.ID);
                            listArgs.Add(this.hisTreatment);
                            listArgs.Add(this.sSByTreatment);
                            V_HIS_PATIENT_TYPE_ALTER lastPatientType = new V_HIS_PATIENT_TYPE_ALTER();
                            MOS.Filter.HisPatientTypeAlterViewFilter patientTypeFilter = new HisPatientTypeAlterViewFilter();
                            patientTypeFilter.TREATMENT_ID = hisTreatment.ID;
                            var patientTypeAlters = new BackendAdapter(new CommonParam()).Get<List<V_HIS_PATIENT_TYPE_ALTER>>("api/HisPatientTypeALter/GetView", ApiConsumer.ApiConsumers.MosConsumer, patientTypeFilter, null);
                            if (patientTypeAlters != null && patientTypeAlters.Count > 0)
                            {
                                var patientTypeBhytCFG = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<long>("MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT");
                                lastPatientType = patientTypeAlters.OrderByDescending(o => o.LOG_TIME).FirstOrDefault();
                            }
                            listArgs.Add(lastPatientType);
                            listArgs.Add(moduleData);
                            listArgs.Add(false);
                            WaitingManager.Hide();
                            HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.TransactionBillSelect", this.moduleData.RoomId, this.moduleData.RoomTypeId, listArgs);
                        }
                    }
                    if (success && isSaveAndSign)
                    {
                        this.isSign = true;
                        Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);
                        richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__MPS000102, DelegateRunPrinter);
                        EnableButton(false);
                        //if (isShowTransation)
                        //{
                        //    this.Close();
                        //    WaitingManager.Show();
                        //    List<object> listArgs = new List<object>();
                        //    LoadTreatment(hisTreatment.ID);
                        //    listArgs.Add(this.hisTreatment);
                        //    listArgs.Add(this.sSByTreatment);
                        //    V_HIS_PATIENT_TYPE_ALTER lastPatientType = new V_HIS_PATIENT_TYPE_ALTER();
                        //    MOS.Filter.HisPatientTypeAlterViewFilter patientTypeFilter = new HisPatientTypeAlterViewFilter();
                        //    patientTypeFilter.TREATMENT_ID = hisTreatment.ID;
                        //    var patientTypeAlters = new BackendAdapter(new CommonParam()).Get<List<V_HIS_PATIENT_TYPE_ALTER>>("api/HisPatientTypeALter/GetView", ApiConsumer.ApiConsumers.MosConsumer, patientTypeFilter, null);
                        //    if (patientTypeAlters != null && patientTypeAlters.Count > 0)
                        //    {
                        //        var patientTypeBhytCFG = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<long>("MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT");
                        //        lastPatientType = patientTypeAlters.OrderByDescending(o => o.LOG_TIME).FirstOrDefault();
                        //    }
                        //    listArgs.Add(lastPatientType);
                        //    listArgs.Add(moduleData);
                        //    listArgs.Add(false);
                        //    WaitingManager.Hide();
                        //    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.TransactionBillSelect", this.moduleData.RoomId, this.moduleData.RoomTypeId, listArgs);
                        //}
                    }
                    WaitingManager.Hide();
                    MessageManager.Show(this, param, success);
                    if (success && chkAutoClose.CheckState == CheckState.Checked)
                        this.Close();
                    SessionManager.ProcessTokenLost(param);
                }
                else
                {
                    SetEnableButtonSave(true);
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Fatal(ex);
            }
        }
        List<HIS_CONFIG> listConfig = new List<HIS_CONFIG>();
        HIS_CONFIG selectedConfig = new HIS_CONFIG();
        private void loadConfig()
        {
            try
            {
                listConfig = BackendDataWorker.Get<HIS_CONFIG>().Where(o => o.KEY.StartsWith("HIS.Desktop.Plugins.PaymentQrCode") && !string.IsNullOrEmpty(o.VALUE)).ToList();
                if (listConfig == null || listConfig.Count == 0) btnQR.Enabled = false;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void CreateQR(V_HIS_TRANSACTION data, bool click)
        {
            try
            {
                if (listConfig != null)
                {
                    if (listConfig.Count > 1)
                    {
                        if (!click)
                        {
                            MessageBox.Show(this, "Vui lòng sử dụng nút tạo QR để thực hiện thanh toán", "Thông báo", MessageBoxButtons.OK);
                            return;
                        }
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
                                    key = parts[parts.Length - 1]; // Lấy phần cuối cùng sau khi tách
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
                                adoqr.TreatmentId = this.hisTreatment.ID;
                                adoqr.ConfigValue = selectedConfig;
                                adoqr.TransReqId = CreateReqType.Transaction;
                                HIS_TRANSACTION tran = new HIS_TRANSACTION();
                                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TRANSACTION>(tran, data);
                                adoqr.Transaction = tran;
                                listArgs.Add(adoqr);
                                Inventec.Common.Logging.LogSystem.Debug("Goi den module CreateTransReqQR. TreatmentID:" + adoqr.TreatmentId + " config: " + adoqr.ConfigValue.KEY + " tran_id: " + adoqr.Transaction.ID);
                                HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.CreateTransReqQR", this.moduleData.RoomId, this.moduleData.RoomTypeId, listArgs);

                            };
                            popupMenu1.AddItem(btnOption);
                        }
                        popupMenu1.Manager = barManager1;
                        popupMenu1.ShowPopup(Control.MousePosition);
                    }
                    else
                    {
                        selectedConfig = listConfig[0];
                        List<object> listArgs = new List<object>();
                        TransReqQRADO adoqr = new TransReqQRADO();
                        adoqr.TreatmentId = this.hisTreatment.ID;
                        adoqr.ConfigValue = selectedConfig;
                        adoqr.TransReqId = CreateReqType.Transaction;
                        HIS_TRANSACTION tran = new HIS_TRANSACTION();
                        Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TRANSACTION>(tran, data);
                        adoqr.Transaction = tran;
                        listArgs.Add(adoqr);
                        Inventec.Common.Logging.LogSystem.Debug("Goi den module CreateTransReqQR. TreatmentID:" + adoqr.TreatmentId + " config: " + adoqr.ConfigValue.KEY + " tran_id: " + adoqr.Transaction.ID);
                        HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.CreateTransReqQR", this.moduleData.RoomId, this.moduleData.RoomTypeId, listArgs);


                    }

                }

            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public bool OpenAppPOS()
        {
            try
            {
                if (IsProcessOpen("WCF"))
                {
                    return true;
                }
                else
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo();

                    startInfo.FileName = Application.StartupPath + @"\Integrate\POS.WCFService\WCF.exe";
                    Inventec.Common.Logging.LogSystem.Info("FileName " + startInfo.FileName);
                    Process.Start(startInfo);
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => startInfo), startInfo));
                    return true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return false;
        }

        private bool IsProcessOpen(string name)
        {
            foreach (Process clsProcess in Process.GetProcesses())
            {
                if (clsProcess.ProcessName.Contains(name))
                {
                    return true;
                }
            }
            return false;
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
                        var lstSameType = GlobalVariables.LastAccountBook.Where(o => o.IS_FOR_DEPOSIT == accountBook.IS_FOR_DEPOSIT && o.ID != accountBook.ID).ToList();
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnSave.Enabled) return;
                SaveProcess(false, false, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Fatal(ex);
            }
        }

        private void bbtnSave_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (btnSave.Enabled)
            {
                btnSave_Click(null, null);
            }
        }

        private void bbtnNew_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (btnAdd.Enabled)
            {
                btnAdd_Click(null, null);
            }
        }

        private void btnSaveAndPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnSaveAndPrint.Enabled) return;
                SaveProcess(true, false, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnSaveAndPrint_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (btnSaveAndPrint.Enabled)
            {
                btnSaveAndPrint_Click(null, null);
            }
        }
        #endregion

        #region previewKeyDown
        private void txtAccountBookCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    LoadAccountBookCombo(strValue);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //private void txtPayFormCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        //{
        //    try
        //    {
        //        if (e.KeyCode == Keys.Enter)
        //        {
        //            string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
        //            LoadPayFormCombo(strValue);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Inventec.Common.Logging.LogSystem.Warn(ex);
        //    }
        //}

        private void chkAddMedicineMaterial_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                reloadTreeSereServ();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkAddMedicineMaterial_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinNumberOrder_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtAmount.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinDiscount_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDescription_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void spinAmount_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtDescription.Focus();
                    txtDescription.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSave_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSave_Click(null, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtDescription_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSave.Focus();
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                    spinTransferAmount.Focus();
                    spinTransferAmount.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        #endregion

        #region closed

        private void cboAccountBook_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboAccountBook.EditValue != null && cboAccountBook.EditValue != cboAccountBook.OldEditValue)
                    {
                        var accountBook = ListAccountBook.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboAccountBook.EditValue.ToString()));
                        if (accountBook != null)
                        {
                            txtAccountBookCode.Text = accountBook.ACCOUNT_BOOK_CODE;
                            SetDataToDicNumOrderInAccountBook(accountBook);
                            cboPayForm.Focus();
                            cboPayForm.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboAccountBook_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (cboAccountBook.EditValue != null)
                    {
                        var accountBook = ListAccountBook.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboAccountBook.EditValue.ToString()));
                        if (accountBook != null)
                        {
                            txtAccountBookCode.Text = accountBook.ACCOUNT_BOOK_CODE;
                            SetDataToDicNumOrderInAccountBook(accountBook);
                            cboPayForm.Focus();
                            cboPayForm.SelectAll();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPayForm_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    HIS_PAY_FORM payForm = null;
                    if (cboPayForm.EditValue != null && cboPayForm.EditValue != cboPayForm.OldEditValue)
                    {
                        payForm = ListPayForm.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboPayForm.EditValue.ToString()));
                        if (payForm != null)
                        {
                            //txtPayFormCode.Text = payForm.PAY_FORM_CODE;
                            //if (lciTransactionTime.Enabled == true)
                            //{
                            //    dtTransactionTime.Focus();
                            //    dtTransactionTime.ShowPopup();
                            //}

                        }
                    }
                    ChangeTitle();
                    CheckPayFormTienMatChuyenKhoan(payForm);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboPayForm_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    ChangeTitle();
                    if (cboPayForm.EditValue != null)
                    {
                        MOS.EFMODEL.DataModels.HIS_PAY_FORM commune = ListPayForm.SingleOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64(cboPayForm.EditValue.ToString()));
                        if (commune != null)
                        {
                            CheckPayFormTienMatChuyenKhoan(commune);
                            //txtPayFormCode.Text = commune.PAY_FORM_CODE;
                            //dtTransactionTime.SelectAll();
                            //dtTransactionTime.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void treeSereServ_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtAccountBookCode.SelectAll();
                    txtAccountBookCode.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        #region other event
        private void treeSereServ_BeforeCheckNode(object sender, DevExpress.XtraTreeList.CheckNodeEventArgs e)
        {
            try
            {
                e.State = (e.PrevState == CheckState.Checked ? CheckState.Unchecked : CheckState.Checked);

                TreeListNode node = e.Node;
                if (node.Checked)
                {
                    node.UncheckAll();
                }
                else
                {
                    node.CheckAll();
                }
                while (node.ParentNode != null)
                {
                    node = node.ParentNode;
                    bool oneOfChildIsChecked = OneOfChildsIsChecked(node);
                    if (oneOfChildIsChecked)
                    {
                        node.CheckState = CheckState.Checked;
                    }
                    else
                    {
                        node.CheckState = CheckState.Unchecked;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void treeSereServ_AfterCheckNode(object sender, DevExpress.XtraTreeList.NodeEventArgs e)
        {
            try
            {
                //ChangeCheckedNodes(e.Node);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void treeSereServ_CustomDrawNodeCell(object sender, DevExpress.XtraTreeList.CustomDrawNodeCellEventArgs e)
        {
            try
            {
                if (sender != null && sender is SereServADO && e.Node.HasChildren && e.Column.FieldName == "TDL_SERVICE_NAME")
                {
                    var data = (SereServADO)sender;
                    if (data.IsFather == true)
                        e.Appearance.ForeColor = Color.Red;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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

        #endregion
        #region EditValuechanged
        private void txtAmount_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                FormatSpint(txtAmount);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
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

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        #endregion

        private void ddbPrint_Click(object sender, EventArgs e)
        {
            try
            {
                ddbPrint.ShowDropDown();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


        #endregion

        private void SetEnableButtonSave(bool enable)
        {
            try
            {
                btnSave.Enabled = enable;
                btnSaveAndPrint.Enabled = enable;
                btnSavePrintAndTrans.Enabled = enable;
                btnAddSign.Enabled = enable;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        string ConvertNumberToString(decimal number)
        {
            string result = "";
            try
            {
                result = Inventec.Common.Number.Convert.NumberToString(number, ConfigApplications.NumberSeperator);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = "";
            }
            return result;
        }

        private void dtTransactionTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (true)
                    {
                        spinTongTuDen.Focus();
                    }
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

        private void SetDefaultEnableCheckEdit()
        {
            try
            {
                if (HisConfigCFG.AutoCheckAndDisableOption)
                {
                    chkHideServiceBHYT.Checked = true;
                    chkHideServiceNoCost.Checked = true;
                    chkHidePatientPrice.Checked = true;
                    layoutControlItem15.Enabled = false;
                    layoutControlItem14.Enabled = false;
                    layoutControlItem13.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void InitControlState()
        {
            isNotLoadWhilechkAutoCloseStateInFirst = true;
            isNotLoadWhileChangeControlStateInFirst = true;
            bool IsHidePatientPrice = false;
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
                        else if (item.KEY == chkConnectionPOS.Name)
                        {
                            chkConnectionPOS.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == chkHideServiceBHYT.Name)
                        {
                            chkHideServiceBHYT.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == chkHideServiceNoCost.Name)
                        {
                            chkHideServiceNoCost.Checked = item.VALUE == "1";
                        }
                        else if (item.KEY == chkHidePatientPrice.Name)
                        {
                            IsHidePatientPrice = item.VALUE == "1";
                        }
                    }

                }
                if (HisConfigCFG.AutoCheckAndDisableOption)
                {
                    chkHidePatientPrice.Checked = true;
                    layoutControlItem15.Enabled = false;
                }
                else
                {
                    chkHidePatientPrice.Checked = IsHidePatientPrice;
                    layoutControlItem15.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            isNotLoadWhilechkAutoCloseStateInFirst = false;
            isNotLoadWhileChangeControlStateInFirst = false;
        }

        private void chkConnectionPOS_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkConnectionPOS.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkConnectionPOS.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkConnectionPOS.Name;
                    csAddOrUpdate.VALUE = (chkConnectionPOS.Checked ? "1" : "");
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

        private void btnPosConfig_Click(object sender, EventArgs e)
        {
            try
            {
                OpenAppPOS();
                try
                {
                    cll = new WcfClient();
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                    XtraMessageBox.Show("Kiểm tra lại cấu hình NetTcpBinding_IService1", "Thông báo");
                    return;
                }
                cll.cauhinh();
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show("Cấu hình thất bại", "Thông báo");
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spnAmountGiveByPatient_EditValueChanged(object sender, EventArgs e)
        {
            long amountGiveByPatient = (long)spnAmountGiveByPatient.Value;
            lblAmountReturnToPatient.Tag = amountGiveByPatient - Convert.ToInt64(totalAmountDeposit);
            lblAmountReturnToPatient.Text = (amountGiveByPatient - Convert.ToInt64(totalAmountDeposit)).ToString();
            this.lblAmountReturnToPatient.Text = string.Format("{0:#,##0}", double.Parse(this.lblAmountReturnToPatient.Text));

        }

        private void spinTransferAmount_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtDescription.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkHideServiceBHYT_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                LoadSereServByTreatment();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkHideServiceNoCost_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                LoadSereServByTreatment();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkHideServiceBHYT_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkHideServiceBHYT.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkHideServiceBHYT.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkHideServiceBHYT.Name;
                    csAddOrUpdate.VALUE = (chkHideServiceBHYT.Checked ? "1" : "");
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

        private void chkHideServiceNoCost_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkHideServiceNoCost.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkHideServiceNoCost.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkHideServiceNoCost.Name;
                    csAddOrUpdate.VALUE = (chkHideServiceNoCost.Checked ? "1" : "");
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

        private void chkHidePatientPrice_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                LoadSereServByTreatment();
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkHidePatientPrice.Name && o.MODULE_LINK == moduleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkHidePatientPrice.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkHidePatientPrice.Name;
                    csAddOrUpdate.VALUE = (chkHidePatientPrice.Checked ? "1" : "");
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

        private void ChangeTitle()
        {
            try
            {
                spnAmountGiveByPatient.EditValue = null;
                lblAmountReturnToPatient.Text = null;
                txtLastDigitsOfBankCardCode.Text = null;
                if (HisConfigCFG.CashierRoomPaymentOption == ((int)CashierRoomPaymentOption.Option2).ToString() && cboPayForm.EditValue != null && Int64.Parse(cboPayForm.EditValue.ToString()) == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__THE)
                {
                    this.layoutControlItem11.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    this.layoutControlItem17.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    this.layoutControlItem12.OptionsToolTip.ToolTip = null;
                    this.layoutControlItem12.Text = Inventec.Common.Resource.Get.Value("frmDepositService.layoutControlItem12Option.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                    this.layoutControlItem12.TextSize = new System.Drawing.Size(130, 20);
                    this.layoutTongTuDen.TextSize = new System.Drawing.Size(130, 20);
                    //this.layoutControlItem17.TextSize = new System.Drawing.Size(100, 20);
                    this.lblAmountReturnToPatient.Appearance.ForeColor = System.Drawing.Color.Black;
                    this.layoutControlItem16.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    this.layoutControlItem17.Size = new System.Drawing.Size(259, 20);
                    this.lciTransactionTime.TextSize = new System.Drawing.Size(110, 20);
                    this.lblTransactionCode.TextSize = new System.Drawing.Size(110, 20);
                    this.layoutControlItem17.TextSize = new System.Drawing.Size(110, 20);
                }
                else
                {
                    this.layoutControlItem11.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
                    this.layoutControlItem17.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    this.layoutControlItem12.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmDepositService.layoutControlItem12.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                    this.layoutControlItem12.Text = Inventec.Common.Resource.Get.Value("frmDepositService.layoutControlItem12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                    this.layoutControlItem12.TextSize = new System.Drawing.Size(90, 20);
                    this.layoutTongTuDen.TextSize = new System.Drawing.Size(90, 20);
                    this.lblAmountReturnToPatient.Appearance.ForeColor = System.Drawing.Color.Blue;
                    this.layoutControlItem16.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    this.layoutControlItem11.Size = new System.Drawing.Size(259, 20);
                    this.lciTransactionTime.TextSize = new System.Drawing.Size(90, 20);
                    this.lblTransactionCode.TextSize = new System.Drawing.Size(90, 20);
                    this.layoutControlItem17.TextSize = new System.Drawing.Size(90, 20);

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CheckCashierRoomPaymentOption2()
        {
            try
            {
                if (String.IsNullOrEmpty(txtLastDigitsOfBankCardCode.Text) || txtLastDigitsOfBankCardCode.Text.Length < 4)
                    return;
                CommonParam param = new CommonParam();
                decimal? value = null;
                CardBalanceFilter filter = new CardBalanceFilter();
                filter.PATIENT_ID = hisTreatment.PATIENT_ID;
                filter.LAST_DIGITS_OF_BANK_CARD_CODE = txtLastDigitsOfBankCardCode.Text;
                value = new Inventec.Common.Adapter.BackendAdapter(param).Get<decimal?>("api/HisPatient/GetCardBalanceBySpecified", ApiConsumers.MosConsumer, filter, param);
                this.lblAmountReturnToPatient.Text = string.Format("{0:#,##0}" + (value == null ? null : " đ"), value);
                SessionManager.ProcessTokenLost(param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void RefreshClickOption2()
        {
            try
            {
                if ((String.IsNullOrEmpty(txtLastDigitsOfBankCardCode.Text) || txtLastDigitsOfBankCardCode.Text.Length < 4) && DevExpress.XtraEditors.XtraMessageBox.Show(Base.ResourceMessageLang.ChuaNhapDuTheNganHang, MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaThongBao), MessageBoxButtons.OK) == DialogResult.OK)
                {
                    txtLastDigitsOfBankCardCode.Focus();
                    txtLastDigitsOfBankCardCode.SelectAll();
                    return;
                }
                CheckCashierRoomPaymentOption2();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnRefreshCardCode_Click(object sender, EventArgs e)
        {
            RefreshClickOption2();
        }

        private void txtLastDigitsOfBankCardCode_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                CheckCashierRoomPaymentOption2();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void txtLastDigitsOfBankCardCode_KeyPress(object sender, KeyPressEventArgs e)
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

        private void btnSavePrintAndTrans_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnSavePrintAndTrans.Enabled) return;
                SaveProcess(true, true, false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void bbtnSavePrintAndTrans_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                if (btnSavePrintAndTrans.Enabled)
                    btnSavePrintAndTrans_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        private void btnQR_Click (object sender, EventArgs e)
        {
            try
            {
                if (hisDeposit != null && hisDeposit.PAY_FORM_ID == 8 && hisDeposit.IS_ACTIVE == 0) CreateQR(hisDeposit, true);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnAddSign_Click(object sender, EventArgs e)
        {
            if (!btnAddSign.Enabled) return;
            SaveProcess(false, true, true);
        }
    }
}

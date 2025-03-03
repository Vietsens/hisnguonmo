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
using DevExpress.XtraTreeList.Nodes;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.RepayService.Config;
using HIS.Desktop.Plugins.RepayService.RepayService.Validtion;
using HIS.Desktop.Print;
using HIS.Desktop.Utilities.Extentions;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.RepayService.RepayService
{
    public partial class frmRepayService : HIS.Desktop.Utility.FormBase
    {
        #region Declaration
        MOS.EFMODEL.DataModels.V_HIS_TREATMENT_1 HisTreatment { get; set; }
        V_HIS_TREATMENT_FEE treatment;
        private decimal totalDiscount { get; set; }
        HisTransactionRepaySDO hisTransactionRepaySDO { get; set; }
        internal MOS.EFMODEL.DataModels.V_HIS_TRANSACTION hisTransaction { get; set; }
        internal HideCheckBoxHelper hideCheckBoxHelper;
        int positionHandle = -1;
        decimal totalAmountDeposit = 0;
        string keyDoExecute = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HIS_SERE_SERV.MUST_BE_ACCEPTED_BEFORE_SETING_NO_EXECUTE");


        bool isNotLoadWhilechkAutoCloseStateInFirst = true;
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        string moduleLink = "HIS.Desktop.Plugins.RepayService";
        List<long> listSereServIDs = new List<long>();
        internal List<MOS.EFMODEL.DataModels.HIS_PAY_FORM> ListPayForm;
        internal List<V_HIS_ACCOUNT_BOOK> ListAccountBook;
        internal List<V_HIS_SERE_SERV_DEPOSIT> ListSereServDeposit;
        internal List<MOS.EFMODEL.DataModels.HIS_PATIENT_TYPE> ListHisPatientType;
        internal List<MOS.EFMODEL.DataModels.V_HIS_CASHIER_ROOM> ListCashierRoom;
        internal List<V_HIS_SERE_SERV_5> ListSereServ;
        //internal string HIS_PAY_FORM_CODE_DEFAULT;//Not use
        const string REPAY_REASON_CODE_DEFAULT = "03";
        long? branchId;
        bool isPrintNow;
        internal long cashierRoomId;
        internal Inventec.Desktop.Common.Modules.Module moduleData;

        SendResultToOtherForm sendResultToOtherForm;
        #endregion

        #region Construct
        public frmRepayService()
        {
            try
            {
                InitializeComponent();
                CustomFormatTreeSereServ();
                EnableButton(true);
                ValidControls();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public frmRepayService(V_HIS_TREATMENT_FEE hisTreatment, HisTransactionRepaySDO transactionData, SendResultToOtherForm _sendResultToOtherForm, long? _branchId, long _cashierRoomId, Inventec.Desktop.Common.Modules.Module _module, List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_5> listSereServ)
            : base(_module)
        {
            try
            {
                InitializeComponent();
                this.CustomFormatTreeSereServ();
                this.treatment = hisTreatment;
                this.hisTransactionRepaySDO = transactionData;
                this.EnableButton(true);
                this.ValidControls();
                this.branchId = _branchId;
                this.sendResultToOtherForm = _sendResultToOtherForm;
                this.cashierRoomId = _cashierRoomId;
                this.SetCaptionByLanguageKey();
                this.moduleData = _module;

                if (keyDoExecute == "1")
                {
                    List<V_HIS_SERE_SERV_5> lstSereServProcess = new List<V_HIS_SERE_SERV_5>();

                    var lstSereServIsMedimate = listSereServ.Where(o => o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT).ToList();
                    var lstSereServIsNotMediMate = listSereServ.Where(o => !lstSereServIsMedimate.Exists(p => p.ID == o.ID)).ToList();

                    #region Loại dịch vụ là thuốc/ vật tư
                    if (lstSereServIsMedimate != null && lstSereServIsMedimate.Count() > 0)
                    {
                        lstSereServProcess.AddRange(lstSereServIsMedimate.Where(o => o.IS_ACCEPTING_NO_EXECUTE == 1).ToList());
                    }
                    #endregion

                    #region loại dịch vụ khác
                    if (lstSereServIsNotMediMate != null && lstSereServIsNotMediMate.Count() > 0)
                    {
                        var lstSereServIsNotMediMateGroup = lstSereServIsNotMediMate.GroupBy(o => o.REQUEST_ROOM_CODE);
                        foreach (var item in lstSereServIsNotMediMateGroup)
                        {
                            if (item.FirstOrDefault().REQUEST_ROOM_IS_EXAM == 1)
                            {
                                lstSereServProcess.AddRange(item.Where(o => o.IS_ACCEPTING_NO_EXECUTE == 1).ToList());
                            }
                            else
                            {
                                lstSereServProcess.AddRange(item.Where(o => o.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL).ToList());
                            }
                        }
                    }
                    #endregion

                    this.ListSereServ = lstSereServProcess;
                }
                else
                {
                    this.ListSereServ = listSereServ;
                }
                listSereServIDs = this.ListSereServ.Select(o => o.ID).ToList();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        #endregion

        #region Load
        private void frmRepayService_Load(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("frmRepayService_Load. 1");
                WaitingManager.Show();
                HisConfigCFG.LoadConfig();
                CommonParam param = new CommonParam();
                if (this.treatment != null)
                {
                    AutoMapper.Mapper.CreateMap<V_HIS_TREATMENT_FEE, V_HIS_TREATMENT_1>();
                    this.HisTreatment = AutoMapper.Mapper.Map<V_HIS_TREATMENT_1>(this.treatment);
                }
                positionHandle = -1;
                LoadDataToControl();
                Inventec.Common.Logging.LogSystem.Debug("frmRepayService_Load. 2");
                if (hisTransactionRepaySDO == null)
                {
                    hisTransactionRepaySDO = new MOS.SDO.HisTransactionRepaySDO();
                    hisTransactionRepaySDO.Transaction = new HIS_TRANSACTION();
                    hisTransactionRepaySDO.SereServDepositIds = new List<long>();
                }

                V_HIS_ACCOUNT_BOOK data = GetDefaultAccountBook();
                if (data != null)
                {
                    SetDefaultAccountBookForUser(data);
                    //SetDataToDicNumOrderInAccountBook(data, true);
                }
                Inventec.Common.Logging.LogSystem.Debug("frmRepayService_Load. 3");


                WaitingManager.Hide();
                cboAccountBook.Focus();

                timerInitForm.Interval = 100;
                timerInitForm.Enabled = true;
                timerInitForm.Start();
                Inventec.Common.Logging.LogSystem.Debug("frmRepayService_Load. 4");



                InitControlState();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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

                this.LoadSereServDepositByTreatment();
                this.SetDefaultPayFormForUser();
                this.SetDefaultRepayReason();
                Inventec.Common.Logging.LogSystem.Debug("timerInitForm_Tick. 2");
                this.treeSereServ.UncheckAll();
                this.SetDefaultDataAndConfig();
                this.ValidControls();
                this.InitMenuToButtonPrint();
                Inventec.Common.Logging.LogSystem.Debug("timerInitForm_Tick. 3");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #region private function

        private V_HIS_ACCOUNT_BOOK GetDefaultAccountBook()
        {
            V_HIS_ACCOUNT_BOOK data = null;
            try
            {
                if (GlobalVariables.LastAccountBook != null && GlobalVariables.LastAccountBook.Count > 0)
                {
                    var lstBook = ListAccountBook.Where(o => GlobalVariables.LastAccountBook.Select(s => s.ID).Contains(o.ID)).ToList();
                    if (lstBook != null && lstBook.Count > 0)
                    {
                        data = lstBook.OrderByDescending(o => o.ID).First();
                    }
                }
                if (data == null) data = ListAccountBook.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return data;
        }

        private void SetDefaultDataAndConfig()
        {
            try
            {
                if (HIS.Desktop.Plugins.RepayService.Config.HisConfigCFG.IsEditTransactionTimeCFG != null && HIS.Desktop.Plugins.RepayService.Config.HisConfigCFG.IsEditTransactionTimeCFG.Equals("1"))
                {
                    lciTransactionTime.Enabled = true;
                }
                else
                {
                    lciTransactionTime.Enabled = false;
                }
                dtCreateTime.EditValue = DateTime.Now;
                txtDescription.Text = "";
                txtTransactionCode.Text = "";
                spinAmount.Tag = 0;
                spinAmount.Text = "0";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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

        void CustomFormatTreeSereServ()
        {
            try
            {
                treeListColumnAmount.Format.FormatString = "#,##0." + AddStringByConfig(ConfigApplications.NumberSeperator);
                treeListColumnAmount.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                treeListColumnVirTotalPrice.Format.FormatString = "#,##0." + AddStringByConfig(ConfigApplications.NumberSeperator);
                treeListColumnVirTotalPrice.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                treeListColumnVirTotalHeinPrice.Format.FormatString = "#,##0." + AddStringByConfig(ConfigApplications.NumberSeperator);
                treeListColumnVirTotalHeinPrice.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                treeListColumnVirTotalPatientPrice.Format.FormatString = "#,##0." + AddStringByConfig(ConfigApplications.NumberSeperator);
                treeListColumnVirTotalPatientPrice.Format.FormatType = DevExpress.Utils.FormatType.Custom;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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

        void LoadTreatment(long _treatmentId)
        {
            try
            {
                if (_treatmentId > 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisTreatmentView1Filter treatment1ViewFilter = new HisTreatmentView1Filter();
                    treatment1ViewFilter.ID = _treatmentId;
                    var treatment1s = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_1>>("api/HisTreatment/GetView1", ApiConsumer.ApiConsumers.MosConsumer, treatment1ViewFilter, param);
                    if (treatment1s != null && treatment1s.Count > 0)
                    {
                        this.HisTreatment = treatment1s.FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void EnableButton(Boolean isEnable)
        {
            try
            {
                btnSave.Enabled = isEnable;
                btnSaveAndPrint.Enabled = isEnable;
                ddbPrint.Enabled = !isEnable;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        void RefreshDataAfterSave()
        {
            if (this.sendResultToOtherForm != null)
            {
                this.sendResultToOtherForm(this.hisTransaction);
            }
        }

        void LoadDataToControl()
        {
            //LoadSereServDepositByTreatment();
            //LoadPayForm();
            //InitComboPayForm();
            //LoadDataToComboRepayReason();
            SetDefaultPayFormForUser();
            LoadPatientType();
            LoadCashierRoom();
            LoadAccountBook();
            InitComboAccountBook();
        }

        private async Task SetDefaultRepayReason()
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

                LoadDataToComboRepayReason(repayReasons);

                var repayReason = repayReasons.FirstOrDefault(o => o.REPAY_REASON_CODE == REPAY_REASON_CODE_DEFAULT);
                if (repayReason != null)
                {
                    cboRepayReason.EditValue = repayReason.ID;
                    txtRepayReason.Text = repayReason.REPAY_REASON_CODE;
                }

                if (HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<long>("HIS.Desktop.Plugins.Repay.Is_Required_Repay_Reason") == 1)
                {
                    lciRepayReasonCode.AppearanceItemCaption.ForeColor = Color.Maroon;
                }
                else
                {
                    lciRepayReasonCode.AppearanceItemCaption.ForeColor = Color.Black;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToComboRepayReason(List<HIS_REPAY_REASON> data)
        {
            try
            {
                cboRepayReason.Properties.DataSource = data;
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
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        // dùng để check dịch vụ đã hoàn ứng chưa
        List<MOS.EFMODEL.DataModels.HIS_SESE_DEPO_REPAY> LoadHisSeseDepoRepay()
        {
            List<MOS.EFMODEL.DataModels.HIS_SESE_DEPO_REPAY> seseDepoRepays = null;
            try
            {

                if (this.ListSereServDeposit == null || this.ListSereServDeposit.Count == 0)
                {
                    return null;
                }
                MOS.Filter.HisSeseDepoRepayFilter hisSeseDepoRepayFilter = new HisSeseDepoRepayFilter();
                hisSeseDepoRepayFilter.SERE_SERV_DEPOSIT_IDs = this.ListSereServDeposit.Select(o => o.ID).ToList();
                //hisSeseDepoRepayFilter.IS_CANCEL = true;
                seseDepoRepays = new BackendAdapter(new CommonParam()).Get<List<MOS.EFMODEL.DataModels.HIS_SESE_DEPO_REPAY>>("api/HisSeseDepoRepay/Get", ApiConsumer.ApiConsumers.MosConsumer, hisSeseDepoRepayFilter, new CommonParam());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return seseDepoRepays;
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

        private async Task LoadSereServDepositByTreatment()
        {
            try
            {
                List<V_HIS_SERE_SERV_DEPOSIT> lstSereServDep = new List<V_HIS_SERE_SERV_DEPOSIT>();
                CommonParam param = new CommonParam();
                if (this.HisTreatment != null)
                {
                    MOS.Filter.HisSereServDepositViewFilter hisSereServDepositFilter = new HisSereServDepositViewFilter();
                    hisSereServDepositFilter.TDL_TREATMENT_ID = this.HisTreatment.ID;
                    hisSereServDepositFilter.IS_CANCEL = false;
                    this.ListSereServDeposit = await new BackendAdapter(param).GetAsync<List<V_HIS_SERE_SERV_DEPOSIT>>("api/HisSereServDeposit/GetView", ApiConsumers.MosConsumer, hisSereServDepositFilter, param);
                    if (this.ListSereServDeposit != null && this.ListSereServDeposit.Count > 0)
                    {
                        // bỏ những dịch vụ "không thực hiện"
                        //var ListSereServNoExecute = (ListSereServ != null && ListSereServ.Count > 0) ? ListSereServ.Where(o => o.IS_NO_EXECUTE == 1).ToList() : null;
                        //List<long> ListSereServNoExecuteId = (ListSereServNoExecute != null && ListSereServNoExecute.Count > 0) ? ListSereServNoExecute.Select(o => o.ID).ToList() : null;
                        //this.ListSereServDeposit = (ListSereServNoExecuteId != null && ListSereServNoExecuteId.Count > 0) ? this.ListSereServDeposit.Where(o => !ListSereServNoExecuteId.Contains(o.SERE_SERV_ID)).ToList() : this.ListSereServDeposit;
                        if (this.ListSereServDeposit == null || this.ListSereServDeposit.Count == 0)
                            return;
                        if (listSereServIDs != null && listSereServIDs.Count > 0)
                        {
                            foreach (var item in listSereServIDs)
                            {
                                var rs = ListSereServDeposit.FirstOrDefault(o => o.SERE_SERV_ID == item);
                                if (rs != null)
                                    lstSereServDep.Add(rs);
                            }
                        }
                        // lấy những dịch vụ có trạng thái là NEW trừ thuốc, vật tư, máu không cần check trạng thái
                        if (keyDoExecute == "1")
                        {

                            ListSereServDeposit = lstSereServDep;
                        }
                        else
                            this.ListSereServDeposit = this.ListSereServDeposit.Where(
                            o =>
                            {
                                var ss = this.ListSereServ.FirstOrDefault(p => p.ID == o.SERE_SERV_ID);
                                return (o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC
                                || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT
                                || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__MAU)
                                || o.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__CXL
                                || (keyDoExecute == "0" && o.SERVICE_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_STT.ID__DXL && ss != null && ss.IS_CONFIRM_NO_EXCUTE == 1);
                            }).ToList();
                        if (this.ListSereServDeposit == null || this.ListSereServDeposit.Count == 0)
                            return;

                        // lấy về sereSereBill để lọc ra các dịch vũ đã thanh toán chưa hay đã hủy thanh toán chưa
                        // nếu thanh toán rồi thì không cho phép hoàn ứng
                        // nếu chưa thanh toán thì cho phép hoàn ứng (IS_CANCEL)
                        param = new CommonParam();
                        MOS.Filter.HisSereServBillFilter sereServBillFilter = new HisSereServBillFilter();
                        sereServBillFilter.TDL_TREATMENT_ID = this.HisTreatment.ID;
                        sereServBillFilter.IS_NOT_CANCEL = true;
                        List<HIS_SERE_SERV_BILL> sereServBillByTreatments = await new BackendAdapter(param).GetAsync<List<HIS_SERE_SERV_BILL>>("api/HisSereServBill/Get", ApiConsumer.ApiConsumers.MosConsumer, sereServBillFilter, param);

                        //List<HIS_SERE_SERV_BILL> sereServBillByTreatments = GetSereServBillByTreatment(this.HisTreatment.ID);
                        List<long> sereServIdBySereServDeposit = this.ListSereServDeposit.Select(p => p.SERE_SERV_ID).Distinct().ToList();
                        List<HIS_SERE_SERV_BILL> sereServBillByTreatmentFilters = (sereServBillByTreatments != null && sereServBillByTreatments.Count > 0) ? sereServBillByTreatments.Where(o => sereServIdBySereServDeposit.Contains(o.SERE_SERV_ID)).ToList() : null;

                        if (sereServBillByTreatmentFilters != null && sereServBillByTreatmentFilters.Count > 0)
                        {
                            List<long> sereServBillByTreatmentIdFilters = sereServBillByTreatmentFilters.Select(o => o.SERE_SERV_ID).ToList();
                            this.ListSereServDeposit = this.ListSereServDeposit.Where(o => !sereServBillByTreatmentIdFilters.Contains(o.SERE_SERV_ID)).ToList();
                        }

                        this.ListSereServDeposit = FilterSereServDepositAndRepay(this.ListSereServDeposit);
                    }

                    RepayServiceProcess.FillDataToSereServTree(this.HisTreatment, this);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        List<V_HIS_SERE_SERV_DEPOSIT> FilterSereServDepositAndRepay(List<V_HIS_SERE_SERV_DEPOSIT> ListSereServByTreatmentSDOProcess)
        {
            List<V_HIS_SERE_SERV_DEPOSIT> ListSereServByTreatmentSDOResult = new List<V_HIS_SERE_SERV_DEPOSIT>();
            try
            {
                if (ListSereServByTreatmentSDOProcess != null && ListSereServByTreatmentSDOProcess.Count > 0)
                {
                    var hisSeseDepoRepays = LoadHisSeseDepoRepay();
                    //đã tạm ứng, chưa hủy tạm ứng, chưa hoàn ứng
                    var seseDepoRepayNotCancels = hisSeseDepoRepays.Where(o => o.IS_CANCEL != 1).ToList();

                    var sereServDepositByTreatmentNotCancels = ListSereServByTreatmentSDOProcess.Where(o => o.IS_CANCEL != 1).ToList();
                    if (seseDepoRepayNotCancels != null && seseDepoRepayNotCancels.Count > 0 && sereServDepositByTreatmentNotCancels != null && sereServDepositByTreatmentNotCancels.Count > 0)
                    {
                        sereServDepositByTreatmentNotCancels = sereServDepositByTreatmentNotCancels.Where(o => !seseDepoRepayNotCancels.Select(p => p.SERE_SERV_DEPOSIT_ID).Contains(o.ID)).ToList();
                    }
                    if (sereServDepositByTreatmentNotCancels != null && sereServDepositByTreatmentNotCancels.Count > 0)
                        ListSereServByTreatmentSDOResult.AddRange(sereServDepositByTreatmentNotCancels);

                    //// lấy những dịch vụ đã hoàn ứng (đã hủy)
                    //var seseDepoRepayCancels = hisSeseDepoRepays.Where(o => o.IS_CANCEL == 1).ToList();
                    //if (seseDepoRepayCancels != null && seseDepoRepayCancels.Count > 0)
                    //{
                    //    List<long> seseDepoNotCancelIds = seseDepoRepayCancels.Select(o => o.SERE_SERV_DEPOSIT_ID).ToList();
                    //    var ListSereServByTreatmentSDOResult3 = (seseDepoNotCancelIds != null && seseDepoNotCancelIds.Count > 0) ? ListSereServByTreatmentSDOProcess.Where(o => seseDepoNotCancelIds.Contains(o.ID)).ToList() : null;
                    //    if (ListSereServByTreatmentSDOResult3 != null && ListSereServByTreatmentSDOResult3.Count > 0)
                    //        ListSereServByTreatmentSDOResult.AddRange(ListSereServByTreatmentSDOResult3);
                    //}
                    ListSereServByTreatmentSDOResult = ListSereServByTreatmentSDOResult.GroupBy(p => p.SERE_SERV_ID).Select(g => g.FirstOrDefault()).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return ListSereServByTreatmentSDOResult;
        }

        List<HIS_SERE_SERV_BILL> GetSereServBillByTreatment(long treatmentId)
        {
            List<HIS_SERE_SERV_BILL> sereServBill = null;
            CommonParam param = new CommonParam();
            try
            {
                MOS.Filter.HisSereServBillFilter sereServBillFilter = new HisSereServBillFilter();
                sereServBillFilter.TDL_TREATMENT_ID = treatmentId;
                sereServBillFilter.IS_NOT_CANCEL = true;
                sereServBill = new BackendAdapter(param).Get<List<HIS_SERE_SERV_BILL>>("api/HisSereServBill/Get", ApiConsumer.ApiConsumers.MosConsumer, sereServBillFilter, param);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return sereServBill;
        }

        private void LoadPayForm()
        {
            try
            {
                MOS.Filter.HisPayFormFilter Filter = new HisPayFormFilter();
                Filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                ListPayForm = new Inventec.Common.Adapter.BackendAdapter(new CommonParam())
                    .Get<List<HIS_PAY_FORM>>("api/HisPayForm/Get", ApiConsumer.ApiConsumers.MosConsumer, Filter, new CommonParam());
                cboPayForm.EditValue = ListPayForm;
                foreach (var item in ListPayForm)
                {
                    LogSystem.Info($"ListPayForm : {item.ID}");
                }
                //ListPayForm = ListPayForm.Where(p => p.IS_ACTIVE == 1).ToList();
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

        private void LoadPatientType()
        {
            try
            {
                ListHisPatientType = BackendDataWorker.Get<HIS_PATIENT_TYPE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
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
                acFilter.FOR_REPAY = true;
                acFilter.IS_OUT_OF_BILL = false;
                acFilter.ORDER_DIRECTION = "DESC";
                acFilter.ORDER_FIELD = "ID";
                CommonParam paramCommon = new CommonParam();
                ListAccountBook = new BackendAdapter(paramCommon).Get<List<V_HIS_ACCOUNT_BOOK>>("api/HisAccountBook/GetView", ApiConsumer.ApiConsumers.MosConsumer, acFilter, paramCommon);
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

        internal void InitComboPayForm()
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

        private void ChangeCheckedNodes(TreeListNode node, CheckState check)
        {
            try
            {
                //var noteData = (MOS.EFMODEL.DataModels.V_HIS_SERE_SERV)node.Tag;
                //if (noteData != null)
                //{
                spinAmount.Tag = 0;
                spinAmount.Text = "0";
                totalAmountDeposit = 0;
                foreach (TreeListNode node1 in treeSereServ.Nodes)
                {
                    UpdateTotalPrice(node1);
                }
                //}
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

        //internal void SetDataToDicNumOrderInAccountBook(V_HIS_ACCOUNT_BOOK accountBook, bool isFirstLoad = false)
        //{
        //    try
        //    {
        //        if (accountBook != null && accountBook.IS_NOT_GEN_TRANSACTION_ORDER == 1)
        //        {
        //            if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook == null || HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.Count == 0 || (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook != null && HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.Count > 0 && !HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.ContainsKey(accountBook.ID)))
        //            {
        //                if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook == null)
        //                {
        //                    HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook = new Dictionary<long, decimal>();
        //                }

        //                layoutTongTuDen.Enabled = true;
        //                CommonParam param = new CommonParam();
        //                MOS.Filter.HisAccountBookViewFilter hisAccountBookViewFilter = new HisAccountBookViewFilter();
        //                hisAccountBookViewFilter.ID = accountBook.ID;
        //                var accountBooks = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_ACCOUNT_BOOK>>(HisRequestUriStore.HIS_ACCOUNT_BOOK_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, hisAccountBookViewFilter, param);
        //                if (accountBooks != null && accountBooks.Count > 0)
        //                {
        //                    var accountBookNew = accountBooks.FirstOrDefault();
        //                    decimal num = 0;
        //                    if ((accountBookNew.CURRENT_NUM_ORDER ?? 0) > 0)
        //                    {
        //                        num = (accountBookNew.CURRENT_NUM_ORDER ?? 0);
        //                    }
        //                    else
        //                    {
        //                        num = (decimal)accountBookNew.FROM_NUM_ORDER - 1;
        //                    }
        //                    HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.Add(accountBookNew.ID, num);
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

        internal void UpdateDictionaryNumOrderAccountBook(V_HIS_ACCOUNT_BOOK accountBook)
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

        private void SetDefaultAccountBookForUser(V_HIS_ACCOUNT_BOOK accountBookDefault)
        {
            try
            {
                if (ListAccountBook != null && ListAccountBook.Count > 0)
                {
                    cboAccountBook.EditValue = accountBookDefault.ID;
                }
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
                cboPayForm.EditValue = null;

                BackendDataWorker.Reset<HIS_PAY_FORM>();
                //if (cboPayForm.EditValue != null)
                //    return;

                if (BackendDataWorker.IsExistsKey<HIS_PAY_FORM>())
                {
                    ListPayForm = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PAY_FORM>();
                }
                else      
                {
                    CommonParam paramCommon = new CommonParam();
                    //dynamic filter = new System.Dynamic.ExpandoObject();
                    MOS.Filter.HisPayFormFilter Filter = new HisPayFormFilter();
                    Filter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                    ListPayForm = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.HIS_PAY_FORM>>("api/HisPayForm/Get", ApiConsumers.MosConsumer, Filter, paramCommon);

                    if (ListPayForm != null)
                        ListPayForm = ListPayForm.Where(p => p.IS_ACTIVE == 1).ToList();
                    BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_PAY_FORM), ListPayForm, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));

                    foreach (var item in ListPayForm)
                    {
                        LogSystem.Info($"------------------------------------- {item.ID}");
                    }
                }

                InitComboPayForm();

                if (ListPayForm != null && ListPayForm.Count > 0)
                {
                    var filteredPayForms = ListPayForm.Where(p => p.IS_ACTIVE == 1).ToList();
                    var PayFormMinByCode = filteredPayForms.OrderBy(o => o.PAY_FORM_CODE);
                    var payFormDefault = PayFormMinByCode.FirstOrDefault();
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

        private void RemoveControlError()
        {
            try
            {
                positionHandle = -1;
                dxValidationProvider1.RemoveControlError(cboAccountBook);
                dxValidationProvider1.RemoveControlError(cboPayForm);
                dxValidationProvider1.RemoveControlError(spinAmount);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ChangeCheckChildNodes(bool bCheck)
        {
            try
            {
                spinAmount.Tag = 0;
                spinAmount.Text = "0";
                totalAmountDeposit = 0;
                foreach (TreeListNode node in treeSereServ.Nodes)
                {
                    CheckAllNodeByFlag(node, bCheck);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void CheckAllNodeByFlag(TreeListNode Nodes, bool bCheck)
        {
            try
            {
                decimal TotalForTxtAmount = 0;
                Nodes.Checked = bCheck;
                foreach (TreeListNode node in Nodes.Nodes)
                {
                    node.Checked = bCheck;
                    if (bCheck)
                    {
                        node.CheckAll();
                    }
                    else
                    {
                        node.UncheckAll();
                    }

                    var item = (MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_DEPOSIT)node.Tag;
                    if (item != null && node.Checked)
                    {
                        TotalForTxtAmount += item.AMOUNT;
                        spinAmount.Tag = TotalForTxtAmount;
                        spinAmount.Text = Inventec.Common.Number.Convert.NumberToString(TotalForTxtAmount, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                        totalAmountDeposit += item.AMOUNT;
                    }
                    CheckAllNodeByFlag(node, bCheck);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateTotalPriceToControl()
        {
            try
            {
                spinAmount.Tag = 0;
                spinAmount.Text = "0";
                foreach (TreeListNode node in treeSereServ.Nodes)
                {
                    UpdateTotalPrice(node);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void UpdateTotalPrice(TreeListNode Nodes)
        {
            try
            {
                decimal TotalPriceForTxtAmount = 0;
                foreach (TreeListNode node in Nodes.Nodes)
                {
                    if (node.Level == 2)
                    {
                        var item = (MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_DEPOSIT)node.Tag;
                        if (item != null && node.Checked)
                        {
                            decimal totalPatientPrice = item.AMOUNT;
                            TotalPriceForTxtAmount += (totalPatientPrice);
                            totalAmountDeposit += totalPatientPrice;
                            spinAmount.Tag = totalAmountDeposit;
                            spinAmount.Text = Inventec.Common.Number.Convert.NumberToString(totalAmountDeposit, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                        }
                    }
                    UpdateTotalPrice(node);
                }
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
                MOS.Filter.HisTreatmentFilter treatmentFilter = new HisTreatmentFilter();
                treatmentFilter.ID = this.treatment.ID;
                var treatments = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumer.ApiConsumers.MosConsumer, treatmentFilter, param);
                if (treatments == null || treatments.Count == 0)
                {
                    return result;
                }
                MOS.Filter.HisCardFilter cardFilter = new HisCardFilter();
                cardFilter.PATIENT_ID = treatments.First().PATIENT_ID;
                var HisCards = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<HIS_CARD>>("api/HisCard/Get", ApiConsumer.ApiConsumers.MosConsumer, cardFilter, param);
                if (HisCards != null && HisCards.Count > 0)
                {
                    RepayDCO.ServiceCodes = HisCards.Select(o => o.SERVICE_CODE).ToArray();
                }
                CARD.WCF.Client.TransactionClient.TransactionClientManager transactionClientManager = new CARD.WCF.Client.TransactionClient.TransactionClientManager();

                result = transactionClientManager.Refund(RepayDCO);
                WaitingManager.Hide();
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

        private void SaveProcess(bool isSaveAndPrint)
        {
            try
            {
                SetEnableButtonSave(false);
                CommonParam param = new CommonParam();
                bool success = false;
                bool justSave = true;

                positionHandle = -1;
                if (!dxValidationProvider1.Validate())
                {
                    SetEnableButtonSave(true);
                    return;
                }

                RepayServiceProcess.UpdateDataFormTransactionDepositToDTO(hisTransactionRepaySDO, HisTreatment, this);

                WaitingManager.Show();
                if (CheckValidForSave(param))
                {

                    CARD.WCF.DCO.WcfRefundDCO repayDCO = null;
                    // thanh toán qua thẻ 
                    var payForm = BackendDataWorker.Get<HIS_PAY_FORM>().FirstOrDefault(o => o.ID == Convert.ToInt64(cboPayForm.EditValue));
                    if (payForm == null)
                    {
                        WaitingManager.Hide();
                        return;
                    }

                    // nếu hình thức thanh toán qua thẻ thì gọi WCF tab thẻ (POS)
                    if (payForm.ID == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__THE)
                    {
                        //kiểm tra trước khi thanh toán nếu là giao dịch qua thẻ
                        var check = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>("api/HisTransaction/CheckRepay", ApiConsumers.MosConsumer, this.hisTransactionRepaySDO, param);

                        if (!check)
                        {
                            WaitingManager.Hide();
                            MessageManager.Show(this, param, success);
                            SessionManager.ProcessTokenLost(param);
                            return;
                        }

                        CARD.WCF.DCO.WcfRefundDCO RepayDCO = new CARD.WCF.DCO.WcfRefundDCO();
                        RepayDCO.Amount = Convert.ToInt64(this.spinAmount.Tag);
                        //DepositDCO.PinCode = this.txtPin.Text.Trim();
                        repayDCO = RepayCard(ref RepayDCO);
                        // nếu gọi sang POS trả về false thì kết thúc
                        if (repayDCO == null || (repayDCO.ResultCode == null || !repayDCO.ResultCode.Equals("00")))
                        {
                            success = false;
                            MappingErrorTHE mappingErrorTHE = new Config.MappingErrorTHE();
                            Inventec.Common.Logging.LogSystem.Info("Output repayDCO: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => repayDCO), repayDCO));

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
                            WaitingManager.Hide();
                            SetEnableButtonSave(true);
                            MessageManager.Show(this, param, success);
                            return;
                        }

                        this.hisTransactionRepaySDO.Transaction.TIG_TRANSACTION_CODE = repayDCO.TransactionCode;
                        this.hisTransactionRepaySDO.Transaction.TIG_TRANSACTION_TIME = repayDCO.TransactionTime;
                    }

                    this.hisTransaction = new Inventec.Common.Adapter.BackendAdapter(param).Post<MOS.EFMODEL.DataModels.V_HIS_TRANSACTION>(HisRequestUriStore.HIS_REPAY_CREATE, ApiConsumers.MosConsumer, this.hisTransactionRepaySDO, param);
                    if (this.hisTransaction != null)
                    {

                        btnSave.Enabled = false;
                        btnSaveAndPrint.Enabled = false;
                        success = true;
                        AddLastAccountToLocal();
                        InitComboAccountBook();
                        RepayServiceProcess.FillDataToControl(this.HisTreatment, this.hisTransaction, this);
                        RefreshDataAfterSave();
                        EnableButton(false);

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
                        //var data = ListUserAccountBook.FirstOrDefault();
                        if (data != null)
                        {
                            SetDefaultAccountBookForUser(data);
                        }
                        var accountBook = this.ListAccountBook.FirstOrDefault(o => o.ID == Convert.ToInt64(cboAccountBook.EditValue.ToString()));
                        UpdateDictionaryNumOrderAccountBook(accountBook);
                        //ChangeCheckChildNodes(true);
                        ddbPrint.Enabled = true;
                        if (isSaveAndPrint)
                        {
                            justSave = false;
                            this.isPrintNow = true;
                            Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);
                            richEditorMain.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__MPS000110, DelegateRunPrinter);


                        }
                    }
                    else
                    {
                        SetEnableButtonSave(true);
                    }

                    MessageManager.Show(this, param, success);
                    if (success && justSave && chkAutoClose.CheckState == CheckState.Checked)
                        this.Close();
                    SessionManager.ProcessTokenLost(param);
                }
                else
                {
                    SetEnableButtonSave(true);
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Fatal(ex);
            }
        }

        private void ProcessAddLastAccount()
        {
            System.Threading.Thread add = new System.Threading.Thread(AddLastAccountToLocal);
            try
            {
                add.Start();
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("", add));
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

        private void bbtnNew_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (btnAdd.Enabled)
            {
                btnAdd_Click(null, null);
            }
        }

        private void btnCancel_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            CommonParam param = new CommonParam();
            bool success = false;
            try
            {

                if (DevExpress.XtraEditors.XtraMessageBox.Show(MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.HeThongTBCuaSoThongBaoBanCoMuonXoaDuLieuKhong), MessageUtil.GetMessage(HIS.Desktop.LibraryMessage.Message.Enum.TieuDeCuaSoThongBaoLaCanhBao), MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    if (hisTransactionRepaySDO != null)
                    {
                        //xemlai...
                        HisTreatment.CREATOR = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                        HisTreatment.GROUP_CODE = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetGroupCode();
                        success = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>(HisRequestUriStore.HIS_DEPOSIT_DELETE, ApiConsumers.MosConsumer, this.hisTransaction, param);
                        if (success)
                        {
                            EnableButton(false);
                            RepayServiceProcess.FillDataToControl(this.HisTreatment, this.hisTransaction, this);
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

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                RepayServiceProcess.FillDataToControl(null, null, this);
                LoadSereServDepositByTreatment();
                SetEnableButtonSave(true);
                LoadAccountBook();
                RepayServiceProcess.FillDataToSereServTree(HisTreatment, this);
                ChangeCheckChildNodes(true);
                treeSereServ.UncheckAll();
                RemoveControlError();
                EnableButton(true);
                dtTransactionTime.EditValue = Inventec.Common.DateTime.Convert.TimeNumberToSystemDateTime(Inventec.Common.DateTime.Get.Now() ?? 0);
                SetDefaultRepayReason();
                ValidControlRepayReason();

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
                    SetDefaultAccountBookForUser(data);

                }
                cboAccountBook.Focus();
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
                if (!btnSave.Enabled) return;
                SaveProcess(false);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void bbtnSave_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (btnSave.Enabled)
            {
                btnSave_Click(null, null);
            }
        }

        private void bbtnSaveAndPrint_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                btnSaveAndPrint_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSaveAndPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnSaveAndPrint.Enabled)
                {
                    SaveProcess(true);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetEnableButtonSave(bool enable)
        {
            try
            {
                btnSave.Enabled = enable;
                btnSaveAndPrint.Enabled = enable;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        #endregion

        #endregion

        #region previewKeyDown
        private void txtAccountBookCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    RepayServiceProcess.LoadAccountBookCombo(strValue, false, this);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPayFormCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    string strValue = (sender as DevExpress.XtraEditors.TextEdit).Text;
                    RepayServiceProcess.LoadPayFormCombo(strValue, false, this);
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
                    spinAmount.Focus();
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
                    txtRepayReason.Focus();
                    txtRepayReason.SelectAll();
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
                    SendKeys.Send("{TAB}");
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
                    cboPayForm.Focus();
                }
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                    dtTransactionTime.Focus();
                    dtTransactionTime.SelectAll();
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
                    dtTransactionTime.Focus();
                    dtTransactionTime.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboAccountBook_GetNotInListValue(object sender, DevExpress.XtraEditors.Controls.GetNotInListValueEventArgs e)
        {
            try
            {
                if (e.FieldName == "CURRENT_NUM_ORDER_DISPLAY")
                {
                    var item = ((List<MOS.EFMODEL.DataModels.V_HIS_ACCOUNT_BOOK>)cboAccountBook.Properties.DataSource)[e.RecordIndex];
                    if (item != null)
                        e.Value = string.Format("{0}/{1}/{2}", (item.TOTAL), (item.FROM_NUM_ORDER), ((int)(item.CURRENT_NUM_ORDER ?? 0)));
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
                    txtRepayReason.Focus();
                    txtRepayReason.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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

                    txtDescription.Focus();
                    txtDescription.SelectAll();
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
                    txtDescription.Focus();
                    txtDescription.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtRepayReason_KeyDown(object sender, KeyEventArgs e)
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
                            txtDescription.Focus();
                            txtDescription.SelectAll();
                        }
                    }
                    if (!valid)
                    {
                        cboRepayReason.Focus();
                        cboRepayReason.ShowPopup();
                    }
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

        private void cboPayForm_Closed(object sender, DevExpress.XtraEditors.Controls.ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    SendKeys.Send("{TAB}");
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
                    SendKeys.Send("{TAB}");
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

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
                ChangeCheckedNodes(e.Node, e.Node.CheckState);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void treeSereServ_CellValueChanged(object sender, DevExpress.XtraTreeList.CellValueChangedEventArgs e)
        {
            try
            {
                var noteData = (MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_DEPOSIT)e.Node.Tag;
                if (noteData != null)
                {
                    UpdateTotalPriceToControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void treeSereServ_CustomDrawNodeCell(object sender, DevExpress.XtraTreeList.CustomDrawNodeCellEventArgs e)
        {

        }

        private void treeSereServ_CustomNodeCellEdit(object sender, DevExpress.XtraTreeList.GetCustomNodeCellEditEventArgs e)
        {
            try
            {
                e.RepositoryItem = txtReadOnly;

                if (e.Column.FieldName == "IS_NO_EXECUTE_STR")
                {
                    e.RepositoryItem = chkIsNoExecute;
                }

                //if (e.Column.FieldName == "AMOUNT")
                //{
                //    e.RepositoryItem = txtReadOnly;

                //    var noteData = (MOS.EFMODEL.DataModels.V_HIS_SERE_SERV)e.Node.Tag;
                //    if (noteData != null)
                //    {
                //        if (noteData.BILL_ID > 0 && noteData.ACCOUNT_BOOK_ID > 0)
                //            e.RepositoryItem = txtReadOnly;
                //        else
                //            e.RepositoryItem = spinSoLuong;
                //        //{
                //        //    case "true":
                //        //        e.RepositoryItem = spinSoLuong;
                //        //        break;
                //        //    case "false":
                //        //        e.RepositoryItem = spinSoLuongReadOnly;
                //        //        break;
                //        //    default:
                //        //        e.RepositoryItem = txtReadOnly;
                //        //        break;
                //        //}
                //    }
                //    else
                //        e.RepositoryItem = txtReadOnly;
                //}
                //else
                //{
                //    e.RepositoryItem = txtReadOnly;
                //}
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
                    cboAccountBook.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void dxValidationProvider1_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
        {
            try
            {
                BaseEdit edit = e.InvalidControl as BaseEdit;
                if (edit == null)
                    return;

                DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo viewInfo = edit.GetViewInfo() as DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo;
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
                    }
                }
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
                this.CheckPayFormTienMatChuyenKhoan();
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

                    this.lciTransferAmount.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_REPAY__LAYOUT_TRANSFER_AMOUNT_TEXT", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    this.lciTransferAmount.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_REPAY__LAYOUT_TRANSFER_AMOUNT_TOOLTIP", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    lciTransferAmount.AppearanceItemCaption.ForeColor = Color.Maroon;
                    lciTransferAmount.Enabled = true;

                }
                else if (cboPayForm.EditValue != null && Convert.ToInt64(cboPayForm.EditValue) == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__TMQT)
                {
                    dxValidationProvider1.RemoveControlError(spinTransferAmount);
                    ValidControlTransferAmount(true);

                    this.lciTransferAmount.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_REPAY__LAYOUT_SWIPE_AMOUNT_TEXT", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    this.lciTransferAmount.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_REPAY__LAYOUT_SWIPE_AMOUNT_TOOLTIP", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    lciTransferAmount.AppearanceItemCaption.ForeColor = Color.Maroon;
                    lciTransferAmount.Enabled = true;

                }
                else
                {
                    dxValidationProvider1.RemoveControlError(spinTransferAmount);
                    ValidControlTransferAmount(false);
                    this.lciTransferAmount.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_REPAY__LAYOUT_TRANSFER_AMOUNT_TEXT", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    this.lciTransferAmount.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_REPAY__LAYOUT_TRANSFER_AMOUNT_TOOLTIP", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                    lciTransferAmount.AppearanceItemCaption.ForeColor = Color.Black;
                    lciTransferAmount.Enabled = false;
                }
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

    }
}

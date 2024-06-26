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
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.DepositServiceKiosk.ADO;
using HIS.Desktop.Plugins.DepositServiceKiosk.Base;
using HIS.Desktop.Plugins.DepositServiceKiosk.Config;
using HIS.Desktop.Plugins.DepositServiceKiosk.Validtion;
using HIS.Desktop.Print;
using HIS.Desktop.Utility;
using HIS.UC.SereServTree;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.Fss.Client;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.LibraryHein.HcmPoorFund;
using MOS.SDO;
using SAR.EFMODEL.DataModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace HIS.Desktop.Plugins.DepositServiceKiosk
{
    public partial class frmDepositServiceKiosk : HIS.Desktop.Utility.FormBase
    {
        private const string SIGNED_EXTENSION = ".pdf";

        Inventec.Desktop.Common.Modules.Module currentModule = null;

        V_HIS_TRANSACTION resultTranBill = null;
        List<V_HIS_ACCOUNT_BOOK> ListAccountBook = new List<V_HIS_ACCOUNT_BOOK>();
        HIS_CASHIER_ROOM cashierRoom;
        long treatmentId = 0;
        V_HIS_TREATMENT_FEE currentTreatment = null;
        private int positionHandleControl = -1;
        bool isInit = true;
        string departmentName = "";
        internal string statusTreatmentOut { get; set; }
        decimal totalPatientPrice = 0;
        V_HIS_PATIENT_TYPE_ALTER resultPatientType;
        List<V_HIS_SERE_SERV_5> sereServByTreatment;
        HIS_BRANCH branch = null;
        string userName = "";
        RefeshReference refeshReference = null;
        //List<V_HIS_SERE_SERV_5> ListSereServ;
        V_HIS_ACCOUNT_BOOK accountBook = null;
        int SetDefaultDepositPrice;
        MOS.EFMODEL.DataModels.V_HIS_TRANSACTION hisDeposit { get; set; }

        DelegateCloseForm_Uc DelegateClose;
        System.Threading.Thread CloseThread;
        private int loopCount = HisConfigCFG.timeWaitingMilisecond / 50;
        private bool stopThread;

        public frmDepositServiceKiosk(Inventec.Desktop.Common.Modules.Module module, long _treatmentId, HIS_CASHIER_ROOM cashierRoomData, DelegateCloseForm_Uc delegateClose)
            : base(module)
        {
            Inventec.Common.Logging.LogSystem.Debug("frmTransactionDebt.1. 1");
            InitializeComponent();
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("frmTransactionDebt.1. 2");
                Base.ResourceLangManager.InitResourceLanguageManager();
                SetIcon();
                this.currentModule = module;
                this.treatmentId = _treatmentId;
                this.cashierRoom = cashierRoomData;
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }
                this.DelegateClose = delegateClose;
                Inventec.Common.Logging.LogSystem.Debug("frmTransactionDebt.1. 3");
                userName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetUserName();
                Inventec.Common.Logging.LogSystem.Debug("frmTransactionDebt.1. 4");

                CloseThread = new System.Threading.Thread(ClosingForm);
                CloseThread.Start();
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

        private void LoadSearch()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisTreatmentFeeViewFilter filter = new HisTreatmentFeeViewFilter();
                filter.ID = this.treatmentId;

                stopThread = true;
                var listTreatment = new BackendAdapter(param)
                           .Get<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_FEE>>("api/HisTreatment/GetFeeView", ApiConsumers.MosConsumer, filter, param);
                stopThread = false;
                ResetLoopCount();
                if (listTreatment != null && listTreatment.Count > 0)
                {
                    currentTreatment = listTreatment.FirstOrDefault();
                    Inventec.Common.Logging.LogSystem.Debug("LoadSearch: " + Inventec.Common.Logging.LogUtil.TraceData("", currentTreatment.TREATMENT_CODE));
                }
                else
                {
                    param.Messages.Add(Base.ResourceMessageLang.KhongTimThayMaDieuTri);
                    return;
                }
            }
            catch (Exception ex)
            {
                stopThread = false;
                ResetLoopCount();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void timerInitForm_Tick()
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("timerInitForm_Tick. 1");
                StopTimer(GetModuleLink(), "timerInitForm");

                LoadSearch();

                LoadSereServByTreatment();
                ChangeCheckedNodes(this.sereServByTreatment);

                //this.FillInfoPatient(this.currentTreatment);
                //this.CalcuTotalPrice();
                Inventec.Common.Logging.LogSystem.Debug("timerInitForm_Tick. 4");
                this.CalcuCanThu();
                Inventec.Common.Logging.LogSystem.Debug("timerInitForm_Tick. 6");
                this.FillDataToButtonPrint();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmTransactionDebt_Load(object sender, EventArgs e)
        {
            try
            {
                RegisterTimer(GetModuleLink(), "timerInitForm", timerInitForm.Interval, timerInitForm_Tick);
                this.SetDefaultDepositPrice = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<int>("HIS_RS.HIS_DEPOSIT.DEFAULT_PRICE_FOR_BHYT_OUT_PATIENT");
                Inventec.Common.Logging.LogSystem.Debug("frmTransactionDebt_Load. 1");
                WaitingManager.Show();
                this.LoadKeyFrmLanguage();
                HisConfigCFG.LoadConfig();
                Inventec.Common.Logging.LogSystem.Debug("frmTransactionDebt_Load. 2");
                Inventec.Common.Logging.LogSystem.Debug("frmTransactionDebt_Load. 3");
                this.ValidControl();
                Inventec.Common.Logging.LogSystem.Debug("frmTransactionDebt_Load. 4");
                this.LoadAccountBookToLocal();
                Inventec.Common.Logging.LogSystem.Debug("frmTransactionDebt_Load. 5");
                this.GeneratePopupMenu();
                this.ResetControlValue();
                Inventec.Common.Logging.LogSystem.Debug("frmTransactionDebt_Load. 6");
                Inventec.Common.Logging.LogSystem.Debug("frmTransactionDebt_Load. 7");
                isInit = false;
                timerInitForm.Interval = 100;
                timerInitForm.Enabled = true;
                StartTimer(GetModuleLink(), "timerInitForm"); 
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ChangeCheckedNodes(List<V_HIS_SERE_SERV_5> listCheckeds)
        {
            try
            {
                decimal amountSs = 0;
                lblTotalPrice.Text = "0";
                totalPatientPrice = 0;
                if (listCheckeds == null || listCheckeds.Count == 0)
                {
                    return;
                }
                foreach (var item in listCheckeds)
                {
                    if (item != null)
                    {
                        if (item.PATIENT_TYPE_ID == HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<long>("MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT"))
                        {
                            MOS.EFMODEL.DataModels.V_HIS_SERE_SERV sereServ = new V_HIS_SERE_SERV();
                            AutoMapper.Mapper.CreateMap<V_HIS_SERE_SERV_5, V_HIS_SERE_SERV>();
                            sereServ = AutoMapper.Mapper.Map<V_HIS_SERE_SERV>(item);
                            decimal itemSsAmount = 0;
                            if (SetDefaultDepositPrice == 1)
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
                            totalPatientPrice = amountSs;
                            lblTotalPrice.Text = Inventec.Common.Number.Convert.NumberToString(amountSs, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                        }
                        else
                        {
                            decimal virTotalPrice = ((item.VIR_TOTAL_PATIENT_PRICE != null && !String.IsNullOrEmpty(item.VIR_TOTAL_PATIENT_PRICE.ToString())) ? Convert.ToDecimal(item.VIR_TOTAL_PATIENT_PRICE) : 0);
                            amountSs += virTotalPrice;
                            totalPatientPrice = amountSs;
                            lblTotalPrice.Text = Inventec.Common.Number.Convert.NumberToString(amountSs, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
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

        private void UpdateDictionaryNumOrderAccountBook(V_HIS_ACCOUNT_BOOK accountBook)
        {
            try
            {
                if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook != null && HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.Count > 0 && HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.ContainsKey(accountBook.ID))
                {
                    //HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook[accountBook.ID] = spinTongTuDen.Value;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDataToDicNumOrderInAccountBook(V_HIS_ACCOUNT_BOOK accountBook)
        {
            try
            {
                if (accountBook != null && accountBook.IS_NOT_GEN_TRANSACTION_ORDER == 1)
                {
                    if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook == null || HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.Count == 0 || (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook != null && HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.Count > 0 && !HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.ContainsKey(accountBook.ID)))
                    {
                        if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook == null)
                        {
                            HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook = new Dictionary<long, decimal>();
                        }
                        stopThread = true;
                        CommonParam param = new CommonParam();
                        MOS.Filter.HisAccountBookViewFilter hisAccountBookViewFilter = new HisAccountBookViewFilter();
                        hisAccountBookViewFilter.ID = accountBook.ID;
                        var accountBooks = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_ACCOUNT_BOOK>>(HisRequestUriStore.HIS_ACCOUNT_BOOK_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, hisAccountBookViewFilter, param);
                        stopThread = false;
                        ResetLoopCount();
                        if (accountBooks != null && accountBooks.Count > 0)
                        {
                            var accountBookNew = accountBooks.FirstOrDefault();
                            decimal num = 0;
                            if ((accountBookNew.CURRENT_NUM_ORDER ?? 0) > 0)
                            {
                                num = accountBookNew.CURRENT_NUM_ORDER ?? 0;
                            }
                            else
                            {
                                num = (decimal)accountBookNew.FROM_NUM_ORDER - 1;
                            }
                            HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.Add(accountBookNew.ID, num);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                stopThread = false;
                ResetLoopCount();
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
                acFilter.FOR_DEPOSIT = true;
                acFilter.IS_OUT_OF_BILL = false;
                acFilter.IS_NOT_GEN_TRANSACTION_ORDER = false;
                acFilter.ORDER_DIRECTION = "DESC";
                acFilter.ORDER_FIELD = "ID";
                stopThread = true;
                ListAccountBook = await new BackendAdapter(new CommonParam()).GetAsync<List<V_HIS_ACCOUNT_BOOK>>("api/HisAccountBook/GetView", ApiConsumers.MosConsumer, acFilter, null);
                stopThread = false;
                ResetLoopCount();
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
                stopThread = false;
                ResetLoopCount();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToComboAccountBook()
        {
            try
            {
                V_HIS_ACCOUNT_BOOK accountBook = null;
                //chọn mặc định sổ nếu có sổ tương ứng
                if (GlobalVariables.LastAccountBook != null && GlobalVariables.LastAccountBook.Count > 0)
                {
                    var lstBook = ListAccountBook.Where(o => GlobalVariables.LastAccountBook.Select(s => s.ID).Contains(o.ID)).ToList();
                    if (lstBook != null && lstBook.Count > 0)
                    {
                        accountBook = lstBook.Last();
                    }
                }

                if (accountBook != null)
                {
                    SetDataToDicNumOrderInAccountBook(accountBook);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_5> GetSereByTreatmentId()
        {
            List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_5> rs = null;
            try
            {
                stopThread = true;
                CommonParam param = new CommonParam();
                MOS.Filter.HisSereServView5Filter sereServFilter = new HisSereServView5Filter();
                sereServFilter.TDL_TREATMENT_ID = this.treatmentId;
                sereServFilter.IS_EXPEND = false;
                var apiData = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_5>>("/api/HisSereServ/GetView5", ApiConsumers.MosConsumer, sereServFilter, null);
                stopThread = false;
                ResetLoopCount();
                if (apiData != null && apiData.Count > 0)
                {
                    rs = apiData.Where(o => o.AMOUNT > 0).ToList();
                }
            }
            catch (Exception ex)
            {
                stopThread = false;
                ResetLoopCount();
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
            return rs;
        }

        private void LoadSereServByTreatment()
        {
            try
            {
                CommonParam param = new CommonParam();
                if (this.sereServByTreatment == null || this.sereServByTreatment.Count == 0)
                {
                    this.sereServByTreatment = GetSereByTreatmentId();
                }
                this.sereServByTreatment = this.sereServByTreatment != null ? this.sereServByTreatment.Where(o => o.IS_EXPEND == null || o.IS_EXPEND != 1).ToList() : null;
                if (this.sereServByTreatment != null && this.sereServByTreatment.Count > 0)
                {
                    // bỏ những dịch vụ không thực hiện (IS_NO_EXECUTE), không cho phép thanh toán hoặc tạm ứng (IS_NO_PAY)
                    this.sereServByTreatment = this.sereServByTreatment.Where(o => o.IS_NO_EXECUTE != 1 && o.IS_NO_PAY != 1).ToList();
                    if (this.sereServByTreatment == null || this.sereServByTreatment.Count == 0)
                        return;
                    // bỏ thuốc/ vật tư thuộc đơn nội trú
                    var ListSereServByTreatmentDNT = this.sereServByTreatment.Where(o => o.TDL_SERVICE_REQ_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_REQ_TYPE.ID__DONDT && (o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__THUOC || o.TDL_SERVICE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SERVICE_TYPE.ID__VT)).ToList();
                    if (ListSereServByTreatmentDNT != null && ListSereServByTreatmentDNT.Count > 0 && this.sereServByTreatment != null && this.sereServByTreatment.Count > 0)
                    {
                        List<long> ListSereServByTreatmentDNTIds = ListSereServByTreatmentDNT.Select(o => o.ID).ToList();
                        this.sereServByTreatment = this.sereServByTreatment.Where(o => !ListSereServByTreatmentDNTIds.Contains(o.ID)).ToList();
                    }
                    if (this.sereServByTreatment == null || this.sereServByTreatment.Count == 0)
                        return;

                    stopThread = true;
                    param = new CommonParam();
                    MOS.Filter.HisSereServBillFilter hisSereServBillFilter = new HisSereServBillFilter();
                    hisSereServBillFilter.TDL_TREATMENT_ID = this.treatmentId;
                    hisSereServBillFilter.IS_NOT_CANCEL = true;
                    var sereServBills = new BackendAdapter(param).Get<List<HIS_SERE_SERV_BILL>>("api/HisSereServBill/Get", ApiConsumer.ApiConsumers.MosConsumer, hisSereServBillFilter, param);
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
                    var sereServDeposits = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_DEPOSIT>>("api/HisSereServDeposit/Get", ApiConsumer.ApiConsumers.MosConsumer, sereServDepositFilter, param);

                    stopThread = false;
                    ResetLoopCount();
                    this.FilterSereServDepositAndRepay(ref this.sereServByTreatment, sereServDeposits);

                    gridControlSereServ.BeginUpdate();
                    gridControlSereServ.DataSource = null;
                    gridControlSereServ.DataSource = this.sereServByTreatment;
                    gridControlSereServ.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                stopThread = false;
                ResetLoopCount();
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        //Lấy List<HIS_SESE_DEPO_REPAY> đã hủy hoàn ứng
        List<MOS.EFMODEL.DataModels.HIS_SESE_DEPO_REPAY> GetSeSeDepoRePay(long treatmentId)
        {
            List<MOS.EFMODEL.DataModels.HIS_SESE_DEPO_REPAY> seseDepoRepays = null;
            CommonParam param = new CommonParam();
            try
            {
                stopThread = true;
                MOS.Filter.HisSeseDepoRepayFilter seseDepositRepayFilter = new HisSeseDepoRepayFilter();
                seseDepositRepayFilter.TDL_TREATMENT_ID = treatmentId;
                seseDepoRepays = new BackendAdapter(param).Get<List<HIS_SESE_DEPO_REPAY>>("api/HisSeseDepoRepay/Get", ApiConsumer.ApiConsumers.MosConsumer, seseDepositRepayFilter, param);
                stopThread = false;
                ResetLoopCount();
            }
            catch (Exception ex)
            {
                stopThread = false;
                ResetLoopCount();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return seseDepoRepays;
        }

        private void CalcuTotalPrice()
        {
            try
            {
                totalPatientPrice = 0;
                List<V_HIS_SERE_SERV_5> ListAll = (List<V_HIS_SERE_SERV_5>)gridControlSereServ.DataSource;
                if (ListAll == null || ListAll.Count == 0)
                {
                    totalPatientPrice = 0;
                    lblTotalPrice.Text = "0";
                }

                if (sereServByTreatment != null)
                {
                    totalPatientPrice = ListAll.Sum(o => o.VIR_TOTAL_PATIENT_PRICE ?? 0);
                    lblTotalPrice.Text = ConvertNumberToString(ListAll.Sum(o => o.VIR_TOTAL_PATIENT_PRICE ?? 0));
                }
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

        private void ResetControlValue()
        {
            try
            {
                resultTranBill = null;
                totalPatientPrice = 0;

                //SetDefaultAccountBook();//TODO
                //SetDefaultPayFormForUser();//TODO
                btnSave.Enabled = true;
                lciBtnSave.Enabled = true;
                ddBtnPrint.Enabled = false;
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
                //chọn mặc định sổ nếu có sổ tương ứng
                if (GlobalVariables.LastAccountBook != null && GlobalVariables.LastAccountBook.Count > 0)
                {
                    var lstBook = ListAccountBook.Where(o => GlobalVariables.LastAccountBook.Select(s => s.ID).Contains(o.ID)).ToList();
                    if (lstBook != null && lstBook.Count > 0)
                    {
                        accountBook = lstBook.OrderByDescending(o => o.ID).First();
                    }
                }
                if (accountBook == null) accountBook = ListAccountBook.FirstOrDefault();

                if (accountBook != null)
                {
                    SetDataToDicNumOrderInAccountBook(accountBook);
                }
                if (accountBook == null || accountBook.ID == 0)
                {
                    MessageBox.Show("Không chọn được số thu chi. Vui lòng liên hệ với nhân viên bệnh viện");
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
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEBT_COLLECT__BTN_SAVE", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.ddBtnPrint.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEBT_COLLECT__BTN_PRINT", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                this.lciTotalPrice.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEBT_COLLECT__LAYOUT_TOTAL_PRICE", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                //Repository Button
                //InfoPatient
                this.gridColumnSereServServiceCode.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEBT_COLLECT__GRID_COLUMN_SERE_SERV_DEBT_TRANSACTION_CODE", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumnSereServServiceName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEBT_COLLECT__GRID_COLUMN_SERE_SERV_DEBT_SERVICE_NAME", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                this.gridColumnSereServTotalPatientPrice.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_DEBT_COLLECT__GRID_COLUMN_SERE_SERV_DEBT_AMOUNT", Base.ResourceLangManager.LanguageFrmTransactionBill, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
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
                //resultPatientType = null;
                //LoadSearch();
                //FillInfoPatient(currentTreatment);
                LoadAccountBookToLocal();
                LoadDataToComboAccountBook();
                ResetControlValue();
                //SetDefaultAccountBook();//TODO
                LoadSereServByTreatment();
                //LoadDataForBordereau();
                //CalcuTotalPrice();
                CalcuCanThu();
                isInit = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool CheckPayformCard(MOS.EFMODEL.DataModels.HIS_PAY_FORM payForm)
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

        private void FormatGridcol(DevExpress.XtraGrid.Columns.GridColumn grdColName, decimal number)
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
                    if (number != null)
                    {
                        if (Inventec.Common.TypeConvert.Parse.ToDecimal(number.ToString() ?? "") % 1 > 0)
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
                grdColName.DisplayFormat.FormatString = Format;
                grdColName.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
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

        private void frmTransactionBill_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                CloseThread.Abort();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ddBtnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                onClickPhieuThuThanhToanKiosk(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewSereServDebt_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.ListSourceRowIndex >= 0 && e.IsGetData && e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    var data = (V_HIS_SERE_SERV_5)((IList)((BaseView)sender).DataSource)[e.ListSourceRowIndex];
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
                        else if (e.Column.FieldName == "VIR_TOTAL_PATIENT_PRICE_STR")
                        {
                            e.Value = ConvertNumberToString(data.VIR_TOTAL_PATIENT_PRICE ?? 0);
                        }
                    }
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
                decimal canthuAmount = 0;
                canthuAmount = totalPatientPrice;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewSereServDebt_SelectionChanged(object sender, DevExpress.Data.SelectionChangedEventArgs e)
        {
            try
            {
                //CalcuTotalPrice();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ClosingForm()
        {
            try
            {
                if (HisConfigCFG.timeWaitingMilisecond > 0)
                {
                    bool time_out = false;
                    ResetLoopCount();
                    while (!time_out)
                    {
                        if (stopThread)
                        {
                            ResetLoopCount();
                        }

                        if (this.loopCount <= 0)
                        {
                            time_out = true;
                        }

                        System.Threading.Thread.Sleep(50);
                        this.loopCount--;
                    }

                    this.Invoke(new MethodInvoker(delegate() { this.Close(); }));
                    if (DelegateClose != null)
                    {
                        DelegateClose(null);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ResetLoopCount()
        {
            try
            {
                this.loopCount = HisConfigCFG.timeWaitingMilisecond / 50;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

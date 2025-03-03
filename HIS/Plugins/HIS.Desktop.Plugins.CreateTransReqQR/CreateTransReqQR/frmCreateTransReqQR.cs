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
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using HIS.Desktop.ADO;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Common.BankQrCode;
using HIS.Desktop.Controls.Session;
using HIS.Desktop.IsAdmin;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.CreateTransReqQR.ADO;
using HIS.Desktop.Print;
using HIS.UC.SereServTree;
using Inventec.Common.Adapter;
using Inventec.Common.Controls.EditorLoader;
using Inventec.Common.Logging;
using Inventec.Common.QRCoder;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using MOS.TDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static DevExpress.Data.Helpers.ExpressiveSortInfo;

namespace HIS.Desktop.Plugins.CreateTransReqQR.CreateTransReqQR
{
    public delegate void SubScreenDelegate(DataSubScreen data);
    public partial class frmCreateTransReqQR : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module currentModule;
        SereServTreeProcessor ssTreeProcessor;
        UserControl ucSereServTree;
        TransReqQRADO inputTransReq { get; set; }
        List<V_HIS_SERE_SERV_5> sereServByTreatment;
        List<HIS_SERE_SERV_DEPOSIT> sereServDeposits { get; set; }
        V_HIS_TREATMENT hisTreatmentView;
        int SetDefaultDepositPrice;
        HIS_TRANS_REQ currentTransReq { get; set; }
        SubScreenDelegate dlg { get; set; }
        bool IsCheckNode { get; set; }
        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        bool IsLoadFirst { get; set; }
        public frmCreateTransReqQR(Inventec.Desktop.Common.Modules.Module currentModule, TransReqQRADO ado) : base(currentModule)
        {
            InitializeComponent();

            try
            {
                this.inputTransReq = ado;
                this.currentModule = currentModule;
                this.SetDefaultDepositPrice = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<int>("HIS_RS.HIS_DEPOSIT.DEFAULT_PRICE_FOR_BHYT_OUT_PATIENT");
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void frmCreateTransReqQR_Load(object sender, EventArgs e)
        {

            try
            {
                HisConfigCFG.LoadConfig();
                btnCreate.Enabled = false;
                loadCauHinhIn();
                this.InitSereServTree();
                LoadTreatment();
                RegisterTimer(GetModuleLink(), "timerInitForm", timerInitForm.Interval, timerInitForm_Tick);
                StartTimer(GetModuleLink(), "timerInitForm");
                LoadPayForm();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        private void timerInitForm_Tick()
        {
            try
            {
                StopTimer(GetModuleLink(), "timerInitForm");
                LoadCom();
                LoadSereServByTreatment();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        List<ComQR> comQRs = new List<ComQR>();
        private void LoadCom()
        {
            try
            {
                var dataCom = SerialPort.GetPortNames().ToList();
                comQRs.Add(new ComQR() { comName = "SDK Model" });
                foreach (var data in dataCom)
                {
                    comQRs.Add(new ComQR() { comName = data });
                }
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("comName", "Cổng", 40, 1));
                ControlEditorADO controlEditorADO = new ControlEditorADO("comName", "comName", columnInfos, true, 40);
                controlEditorADO.ImmediatePopup = true;
                ControlEditorLoader.Load(cboCom, comQRs, controlEditorADO);
                cboCom.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
                InitControlState();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void LoadPayForm()
        {
            try
            {
                IsLoadFirst = true;
                if (inputTransReq.Transaction != null || (inputTransReq.Transactions != null && inputTransReq.Transactions.Count > 0))
                {
                    var datas = BackendDataWorker.Get<HIS_PAY_FORM>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                    List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                    columnInfos.Add(new ColumnInfo("PAY_FORM_CODE", "Mã", 60, 1));
                    columnInfos.Add(new ColumnInfo("PAY_FORM_NAME", "Tên", 120, 2));
                    ControlEditorADO controlEditorADO = new ControlEditorADO("PAY_FORM_NAME", "ID", columnInfos, true, 180);
                    controlEditorADO.ImmediatePopup = true;
                    ControlEditorLoader.Load(cboPayForm, datas, controlEditorADO);
                    cboPayForm.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
                    cboPayForm.Properties.Buttons[1].Visible = false;
                    if (inputTransReq.Transaction != null)
                        cboPayForm.EditValue = inputTransReq.Transaction.PAY_FORM_ID;
                    else
                        cboPayForm.EditValue = inputTransReq.Transactions[0].PAY_FORM_ID;
                    cboPayForm.Enabled = (cboPayForm.EditValue != null && Int64.Parse(cboPayForm.EditValue.ToString()) == IMSys.DbConfig.HIS_RS.HIS_PAY_FORM.ID__QR) || (currentTransReq == null || (currentTransReq.TRANS_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_TRANS_REQ_STT.ID__REQUEST)) ? true : false;

                }
                else
                {
                    layoutControlItem16.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                    emptySpaceItem2.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                }
                IsLoadFirst = false;
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
                this.controlStateWorker = new HIS.Desktop.Library.CacheClient.ControlStateWorker();
                this.currentControlStateRDO = controlStateWorker.GetData(this.currentModule.ModuleLink);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == cboCom.Name && !string.IsNullOrEmpty(item.VALUE) && comQRs != null && comQRs.Exists(o => o.comName == item.VALUE))
                        {
                            cboCom.EditValue = item.VALUE;
                            IsConnectOld = true;
                            btnConnect_Click(null, null);
                        }
                        else if (item.KEY == chkOtherScreen.Name)
                        {
                            chkOtherScreen.Checked = item.VALUE == "1";
                        }
                        foreach (var phieu in lstLoaiPhieu)
                        {
                            if (item.KEY == phieu.ID)
                            {
                                phieu.Check = item.VALUE == "1";
                            }
                        }
                    }
                    gridView1.GridControl.RefreshDataSource();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        private void LoadTreatment()
        {
            try
            {
                if (this.inputTransReq.TreatmentId > 0)
                {
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisTreatmentViewFilter filter = new HisTreatmentViewFilter();
                    filter.ID = this.inputTransReq.TreatmentId;
                    hisTreatmentView = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT>>("/api/HisTreatment/GetView", ApiConsumers.MosConsumer, filter, null).FirstOrDefault();
                    if (hisTreatmentView != null)
                    {
                        lblPatientName.Text = hisTreatmentView.TDL_PATIENT_NAME;
                        lblPatientCode.Text = hisTreatmentView.TDL_PATIENT_CODE;
                        lblGenderName.Text = hisTreatmentView.TDL_PATIENT_GENDER_NAME;
                        lblAddress.Text = hisTreatmentView.TDL_PATIENT_ADDRESS;
                        lblDob.Text = hisTreatmentView.TDL_PATIENT_IS_HAS_NOT_DAY_DOB == 1 ? hisTreatmentView.TDL_PATIENT_DOB.ToString().Substring(0, 4) : Inventec.Common.DateTime.Convert.TimeNumberToDateString(hisTreatmentView.TDL_PATIENT_DOB);
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        decimal Amount = 0;
        private async Task LoadSereServByTreatment()
        {
            try
            {
                bool IsLoad = false;
                if (inputTransReq.Transaction != null || (inputTransReq.Transactions != null && inputTransReq.Transactions.Count > 0))
                {
                    long typeId = 0;
                    List<long> TransactionIds = new List<long>();
                    bool IsSale = false;
                    if (inputTransReq.Transaction != null)
                    {
                        typeId = inputTransReq.Transaction.TRANSACTION_TYPE_ID;
                        TransactionIds.Add(inputTransReq.Transaction.ID);
                        IsSale = inputTransReq.Transaction.SALE_TYPE_ID == 1;
                    }
                    else
                    {
                        typeId = inputTransReq.Transactions[0].TRANSACTION_TYPE_ID;
                        TransactionIds.AddRange(inputTransReq.Transactions.Select(o => o.ID).ToList());
                        IsSale = inputTransReq.Transactions[0].SALE_TYPE_ID == 1;
                    }
                    if (typeId == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TU)
                    {
                        LoadSsDeposit(TransactionIds);
                        IsLoad = true;
                    }
                    else if (typeId == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT && IsSale)
                    {
                        LoadSsBillGoods(TransactionIds);
                        IsLoad = true;
                    }
                    else if (typeId == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT && !IsSale)
                    {
                        LoadSsBill(TransactionIds);
                        IsLoad = true;
                    }
                    this.ssTreeProcessor.Reload(this.ucSereServTree, this.sereServByTreatment);
                }
                if (inputTransReq.TransReqId == CreateReqType.Deposit || inputTransReq.TransReqId == CreateReqType.Transaction)
                {
                    btnCreate_Click(null, null);
                    return;
                }
                this.sereServByTreatment = GetSereByTreatmentId();
                if (this.sereServByTreatment == null || this.sereServByTreatment.Count == 0)
                {
                    XtraMessageBox.Show("Hồ sơ không có dịch vụ cần tạm ứng");
                    this.Close();
                    return;
                }
                // bỏ những dịch vụ không thực hiện (IS_NO_EXECUTE), không cho phép thanh toán hoặc tạm ứng (IS_NO_PAY)
                this.sereServByTreatment = this.sereServByTreatment.Where(o => o.IS_NO_EXECUTE != 1 && o.IS_NO_PAY != 1).ToList();
                if (this.sereServByTreatment == null || this.sereServByTreatment.Count == 0)
                    return;


                CommonParam param = new CommonParam();
                MOS.Filter.HisSereServBillFilter hisSereServBillFilter = new HisSereServBillFilter();
                hisSereServBillFilter.TDL_TREATMENT_ID = this.inputTransReq.TreatmentId;
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
                sereServDebtFilter.TDL_TREATMENT_ID = this.inputTransReq.TreatmentId;
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
                sereServDepositFilter.TDL_TREATMENT_ID = this.inputTransReq.TreatmentId;
                sereServDeposits = await new BackendAdapter(param).GetAsync<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_DEPOSIT>>("api/HisSereServDeposit/Get", ApiConsumer.ApiConsumers.MosConsumer, sereServDepositFilter, param);

                List<V_HIS_SERE_SERV_5> sereServByTreatmentProcess = new List<V_HIS_SERE_SERV_5>();

                this.FilterSereServDepositAndRepay(ref this.sereServByTreatment, sereServDeposits);
                this.ssTreeProcessor.Reload(this.ucSereServTree, this.sereServByTreatment);
                btnCreate_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void LoadSsBill(List<long> transactionIds)
        {
            try
            {
                CommonParam param = new CommonParam();
                HisSereServBillFilter ssBillFilter = new HisSereServBillFilter();
                ssBillFilter.BILL_IDs = transactionIds;
                var hisSSBills = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV_BILL>>("api/HisSereServBill/Get", ApiConsumers.MosConsumer, ssBillFilter, null);
                if (hisSSBills == null || hisSSBills.Count <= 0)
                {
                    return;
                }
                HisSereServFilter ssfilter = new HisSereServFilter();
                ssfilter.IDs = hisSSBills.Select(o => o.SERE_SERV_ID).ToList();
                sereServByTreatment = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_5>>("/api/HisSereServ/GetView5", ApiConsumers.MosConsumer, ssfilter, null);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void LoadSsBillGoods(List<long> transactionIds)
        {
            try
            {
                HisBillGoodsFilter billGoodsFilter = new HisBillGoodsFilter();
                billGoodsFilter.BILL_IDs = transactionIds;
                var billGoods = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_BILL_GOODS>>("api/HisBillGoods/Get", ApiConsumers.MosConsumer, billGoodsFilter, null);
                if (billGoods != null && billGoods.Count > 0)
                {
                    this.sereServByTreatment = new List<V_HIS_SERE_SERV_5>();
                    foreach (var item in billGoods)
                    {
                        V_HIS_SERE_SERV_5 sereServBill = new V_HIS_SERE_SERV_5();
                        sereServBill.TDL_SERVICE_NAME = item.GOODS_NAME;
                        sereServBill.AMOUNT = item.AMOUNT;
                        sereServBill.VIR_PATIENT_PRICE = sereServBill.VIR_TOTAL_PATIENT_PRICE = item.AMOUNT * item.PRICE;
                        sereServBill.DISCOUNT = item.DISCOUNT;
                        sereServBill.VAT_RATIO = item.VAT_RATIO ?? 0;
                        sereServBill.SERVICE_UNIT_NAME = item.GOODS_UNIT_NAME;
                        sereServBill.VIR_PRICE = item.PRICE;
                        if (item.MEDICINE_TYPE_ID.HasValue)
                        {
                            var medicine = BackendDataWorker.Get<V_HIS_MEDICINE_TYPE>().FirstOrDefault(o => o.ID == item.MEDICINE_TYPE_ID.Value);
                            if (medicine != null)
                            {
                                sereServBill.TDL_SERVICE_CODE = medicine.MEDICINE_TYPE_CODE;
                                sereServBill.TDL_SERVICE_NAME = medicine.MEDICINE_TYPE_NAME;
                                sereServBill.SERVICE_UNIT_NAME = medicine.SERVICE_UNIT_NAME;
                            }
                        }
                        else if (item.MATERIAL_TYPE_ID.HasValue)
                        {
                            var material = BackendDataWorker.Get<V_HIS_MATERIAL_TYPE>().FirstOrDefault(o => o.ID == item.MATERIAL_TYPE_ID.Value);
                            if (material != null)
                            {
                                sereServBill.TDL_SERVICE_CODE = material.MATERIAL_TYPE_CODE;
                                sereServBill.TDL_SERVICE_NAME = material.MATERIAL_TYPE_NAME;
                                sereServBill.SERVICE_UNIT_NAME = material.SERVICE_UNIT_NAME;
                            }
                        }
                        sereServByTreatment.Add(sereServBill);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void LoadSsDeposit(List<long> transactionIds)
        {
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisSereServDepositFilter dereDetailFiter = new MOS.Filter.HisSereServDepositFilter();
                dereDetailFiter.DEPOSIT_IDs = transactionIds;
                var dereDetails = new BackendAdapter(param).Get<List<HIS_SERE_SERV_DEPOSIT>>("api/HisSereServDeposit/Get", ApiConsumers.MosConsumer, dereDetailFiter, param);
                if (dereDetails != null && dereDetails.Count > 0)
                {
                    var sereServIds = dereDetails.Select(o => o.SERE_SERV_ID).ToList();

                    MOS.Filter.HisSereServView5Filter sereServFilter = new MOS.Filter.HisSereServView5Filter();
                    sereServFilter.IDs = sereServIds;
                    this.sereServByTreatment = new BackendAdapter(param).Get<List<V_HIS_SERE_SERV_5>>("api/HisSereServ/GetView5", ApiConsumers.MosConsumer, sereServFilter, param);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

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
                        var seseDepoRepays = GetSeSeDepoRePay(this.inputTransReq.TreatmentId);
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

        private List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_5> GetSereByTreatmentId()
        {
            List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_5> rs = null;
            try
            {
                CommonParam param = new CommonParam();
                MOS.Filter.HisSereServView5Filter sereServFilter = new HisSereServView5Filter();
                sereServFilter.TDL_TREATMENT_ID = this.inputTransReq.TreatmentId;
                sereServFilter.IS_EXPEND = false;
                var apiData = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_5>>("/api/HisSereServ/GetView5", ApiConsumers.MosConsumer, sereServFilter, null);
                if (apiData != null && apiData.Count > 0)
                {
                    rs = apiData.Where(o => (o.PATIENT_TYPE_ID != HisConfigCFG.PatientTypeId__BHYT || (HisConfigCFG.ShowServiceBhyt && o.PATIENT_TYPE_ID == HisConfigCFG.PatientTypeId__BHYT)) && (o.VIR_TOTAL_PATIENT_PRICE ?? 0) > 0).ToList();
                    if (rs != null && rs.Count > 0 && HisConfigCFG.ShowServiceByRoomOption == "1")
                        rs = rs.Where(o => o.TDL_EXECUTE_ROOM_ID == currentModule.RoomId || o.TDL_REQUEST_ROOM_ID == currentModule.RoomId).ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return null;
            }
            return rs;
        }

        #region InitUC
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
        List<long> SereServIds = new List<long>();
        private void ChangeCheckedNodes(List<SereServADO> listCheckeds)
        {
            try
            {
                SereServIds = new List<long>();
                Amount = 0;
                lblAmount.Text = "0";
                foreach (var item in listCheckeds)
                {
                    if (item != null && (item.IsLeaf ?? false))
                    {
                        decimal totalPatientPrice = ((item.VIR_TOTAL_PATIENT_PRICE != null && !String.IsNullOrEmpty(item.VIR_TOTAL_PATIENT_PRICE.ToString())) ? Convert.ToDecimal(item.VIR_TOTAL_PATIENT_PRICE) : 0);
                        Amount += totalPatientPrice;
                        SereServIds.Add(item.ID);
                    }
                }
                lblAmount.Text = Inventec.Common.Number.Convert.NumberToString(Amount, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                SendData();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void sereServTree_BeforeCheck(TreeListNode node, DevExpress.XtraTreeList.CheckNodeEventArgs e)
        {
            try
            {
                if (node != null)
                {
                    var nodeData = (SereServADO)node.TreeList.GetDataRecordByNode(node);
                    if (nodeData != null && !IsCheckNode)
                    {
                        e.CanCheck = false;
                        node.CheckAll();
                        return;
                    }
                }
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
        private void sereServTree_ShowingEditorDG(TreeListNode node, object sender)
        {
            try
            {
                var nodeData = node.TreeList.GetDataRecordByNode(node);
                if (nodeData != null)
                {
                    ((TreeList)sender).ActiveEditor.Properties.ReadOnly = true;
                }
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
                this.ssTreeProcessor = new UC.SereServTree.SereServTreeProcessor();
                SereServTreeADO ado = new SereServTreeADO();
                ado.IsShowSearchPanel = false;
                ado.IsShowCheckNode = true;
                ado.isAdvance = true;
                ado.SereServs = this.sereServByTreatment;
                ado.SereServTree_CustomUnboundColumnData = sereServTree_CustomUnboundColumnData;

                ado.SereServTreeColumns = new List<SereServTreeColumn>();
                //ado.SelectImageCollection = this.imageCollection1;
                ado.SereServTree_CustomDrawNodeCell = treeSereServ_CustomDrawNodeCell;
                ado.SereServTree_AfterCheck = sereServTree_AfterCheck;
                ado.SereServTreeForBill_BeforeCheck = sereServTree_BeforeCheck;
                ado.sereServTree_ShowingEditor = sereServTree_ShowingEditorDG;
                ado.SereServTree_CheckAllNode = sereServTree_CheckAllNode;


                ado.LayoutSereServExpend = "Hao phí";
                //Cột Tên dịch vụ
                SereServTreeColumn serviceNameCol = new SereServTreeColumn("Tên dịch vụ", "TDL_SERVICE_NAME", 200, false);
                serviceNameCol.VisibleIndex = 0;
                ado.SereServTreeColumns.Add(serviceNameCol);
                //Cột Số lượng
                SereServTreeColumn amountCol = new SereServTreeColumn("Số lượng", "AMOUNT_PLUS_STR", 60, false);
                amountCol.VisibleIndex = 1;
                amountCol.Format = new DevExpress.Utils.FormatInfo();
                amountCol.Format.FormatString = "#,##0.00";
                amountCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                amountCol.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;
                ado.SereServTreeColumns.Add(amountCol);
                //Cột Đơn giá
                SereServTreeColumn virPriceCol = new SereServTreeColumn("Đơn giá", "VIR_PRICE_DISPLAY", 110, false);
                virPriceCol.VisibleIndex = 2;
                virPriceCol.Format = new DevExpress.Utils.FormatInfo();
                virPriceCol.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;
                ado.SereServTreeColumns.Add(virPriceCol);
                //Cột thành tiền
                SereServTreeColumn virTotalPriceCol = new SereServTreeColumn("Thành tiền", "VIR_TOTAL_PRICE_DISPLAY", 110, false);
                virTotalPriceCol.VisibleIndex = 3;
                virTotalPriceCol.Format = new DevExpress.Utils.FormatInfo();
                virTotalPriceCol.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;
                //virTotalPriceCol.Format.FormatString = "#,##0.0000";
                //virTotalPriceCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.SereServTreeColumns.Add(virTotalPriceCol);
                //Cột Đồng chi trả
                SereServTreeColumn virTotalHeinPriceCol = new SereServTreeColumn("Đồng chi trả", "VIR_TOTAL_HEIN_PRICE_DISPLAY", 110, false);
                virTotalHeinPriceCol.VisibleIndex = 4;
                virTotalHeinPriceCol.Format = new DevExpress.Utils.FormatInfo();
                virTotalHeinPriceCol.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;
                //virTotalHeinPriceCol.Format.FormatString = "#,##0.0000";
                //virTotalHeinPriceCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.SereServTreeColumns.Add(virTotalHeinPriceCol);

                SereServTreeColumn virTotalPatientPriceCol = new SereServTreeColumn("Bệnh nhân trả", "VIR_TOTAL_PATIENT_PRICE_DISPLAY", 110, false);
                virTotalPatientPriceCol.VisibleIndex = 5;
                virTotalPatientPriceCol.Format = new DevExpress.Utils.FormatInfo();
                virTotalPatientPriceCol.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;
                //virTotalPatientPriceCol.Format.FormatString = "#,##0.0000";
                //virTotalPatientPriceCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.SereServTreeColumns.Add(virTotalPatientPriceCol);
                //Chiếu khấu
                SereServTreeColumn virDiscountCol = new SereServTreeColumn("Chiết khấu", "DISCOUNT_DISPLAY", 110, false);
                virDiscountCol.VisibleIndex = 6;
                virDiscountCol.Format = new DevExpress.Utils.FormatInfo();
                virDiscountCol.UnboundType = DevExpress.XtraTreeList.Data.UnboundColumnType.Object;
                ado.SereServTreeColumns.Add(virDiscountCol);
                //Hao phí
                SereServTreeColumn virIsExpendCol = new SereServTreeColumn("Hao phí", "IsExpend", 60, false);
                virIsExpendCol.VisibleIndex = 7;
                ado.SereServTreeColumns.Add(virIsExpendCol);
                //
                SereServTreeColumn virVatRatioCol = new SereServTreeColumn("VAT %", "VAT", 100, false);
                virVatRatioCol.VisibleIndex = 8;
                virVatRatioCol.Format = new DevExpress.Utils.FormatInfo();
                virVatRatioCol.Format.FormatString = "#,##0.00";
                virVatRatioCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.SereServTreeColumns.Add(virVatRatioCol);

                SereServTreeColumn serviceCodeCol = new SereServTreeColumn("Mã dịch vụ", "TDL_SERVICE_CODE", 100, false);
                serviceCodeCol.VisibleIndex = 9;
                ado.SereServTreeColumns.Add(serviceCodeCol);

                SereServTreeColumn serviceReqCodeCol = new SereServTreeColumn("Mã yêu cầu", "TDL_SERVICE_REQ_CODE", 100, false);
                serviceReqCodeCol.VisibleIndex = 10;
                ado.SereServTreeColumns.Add(serviceReqCodeCol);
                //SereServTreeColumn TRANSACTIONCodeCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_DEPOSIT_SERVICE__TREE_SERE_SERV__COLUMN_TRANSACTION_CODE", Resources.ResourceLanguageManager.LanguageResource, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "TRANSACTION_CODE", 100, false);
                //TRANSACTIONCodeCol.VisibleIndex = 11;
                //ado.SereServTreeColumns.Add(TRANSACTIONCodeCol);
                SereServTreeColumn intructionTime = new SereServTreeColumn("Thời gian chỉ định", "TDL_INTRUCTION_TIME_STR", 130, false);
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
        #endregion

        private void timerReloadTransReq_Tick(object sender, EventArgs e)
        {

            try
            {
                if (currentTransReq != null)
                {
                    lblStt.Text = "Đang chờ";
                    CommonParam param = new CommonParam();
                    MOS.Filter.HisTransReqFilter sereServFilter = new HisTransReqFilter();
                    sereServFilter.ID = currentTransReq.ID;
                    var apiData = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_TRANS_REQ>>("/api/HisTransReq/Get", ApiConsumers.MosConsumer, sereServFilter, null);

                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => apiData), apiData));
                    if (apiData != null && apiData.Count > 0)
                    {
                        currentTransReq = apiData[0];
                        InitPopupMenuOther();
                        lblStt.Text = currentTransReq.TRANS_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_TRANS_REQ_STT.ID__FINISHED ? "Thành công" : currentTransReq.TRANS_REQ_STT_ID != IMSys.DbConfig.HIS_RS.HIS_TRANS_REQ_STT.ID__REQUEST ? "Thất bại" : "Đang chờ";
                        if (currentTransReq.TRANS_REQ_STT_ID != IMSys.DbConfig.HIS_RS.HIS_TRANS_REQ_STT.ID__REQUEST)
                        {
                            cboPayForm.Enabled = false;

                            timerReloadTransReq.Stop();
                            if (currentTransReq.TRANS_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_TRANS_REQ_STT.ID__FINISHED)
                            {
                                if (PosStatic.IsOpenPos())
                                    PosStatic.SendData(PosStatic.PAYMENT_SUCCESSS);
                                pbQr.EditValue = global::HIS.Desktop.Plugins.CreateTransReqQR.Properties.Resources.check;

                                btnNew.Enabled = btnCreate.Enabled = false;

                                if (transactionPrint != null)
                                {
                                    switch (transactionPrint.TRANSACTION_TYPE_ID)
                                    {
                                        case IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT:
                                            if (transactionPrint.SALE_TYPE_ID == null)
                                            {
                                                if (HisConfigCFG.TransactionBillSelect != "2")
                                                {
                                                    onClickThanhToanDv(null, null);
                                                }
                                                else
                                                {
                                                    onClickHoaDonThanhToan(null, null);
                                                }
                                            }
                                            else if (transactionPrint.SALE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SALE_TYPE.ID__SALE_EXP)
                                            {
                                                onClickHoaDonXb(null, null);
                                                onClickHoaDonThanhToan(null, null);
                                            }
                                            break;
                                        case IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TU:
                                            if (transactionPrint.TDL_SERE_SERV_DEPOSIT_COUNT == null)
                                            {
                                                onClickTamUng(null, null);
                                            }
                                            else
                                            {
                                                if (lstLoaiPhieu.FirstOrDefault(o => o.ID == "Mps000102").Check)
                                                    onClickTamUngDv(null, null);
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                if (lstLoaiPhieu.FirstOrDefault(o => o.ID == "Mps000276").Check)
                                {
                                    List<HIS_SERE_SERV_BILL> hisSSBills = new List<HIS_SERE_SERV_BILL>();
                                    List<HIS_SERE_SERV> sereServs = new List<HIS_SERE_SERV>();
                                    if (inputTransReq.TransReqId == CreateReqType.Deposit || inputTransReq.TransReqId == CreateReqType.Transaction)
                                    {
                                        HisSereServBillFilter ssBillFilter = new HisSereServBillFilter();
                                        ssBillFilter.BILL_IDs = transactionPrintList.Select(o => o.ID).ToList();
                                        hisSSBills = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV_BILL>>("api/HisSereServBill/Get", ApiConsumers.MosConsumer, ssBillFilter, null);
                                        if (hisSSBills == null || hisSSBills.Count <= 0)
                                        {
                                            return;
                                        }
                                        HisSereServFilter ssfilter = new HisSereServFilter();
                                        ssfilter.IDs = hisSSBills.Select(o => o.SERE_SERV_ID).ToList();
                                        sereServs = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV>>("/api/HisSereServ/Get", ApiConsumers.MosConsumer, ssfilter, null);
                                        if (sereServs != null && sereServs.Count > 0 && sereServs.Exists(o => o.SERVICE_REQ_ID.HasValue))
                                        {
                                            HisServiceReqViewFilter filter = new HisServiceReqViewFilter();
                                            filter.IDs = sereServs.Select(o => o.SERVICE_REQ_ID ?? 0).Distinct().ToList();
                                            var serviceReq = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ>>("/api/HisServiceReq/GetView", ApiConsumers.MosConsumer, filter, null);
                                            if (serviceReq != null && serviceReq.Count > 0)
                                            {
                                                HIS.Desktop.Plugins.Library.PrintServiceReqTreatment.PrintServiceReqTreatmentProcessor proc = new Library.PrintServiceReqTreatment.PrintServiceReqTreatmentProcessor(serviceReq, currentModule.RoomId);
                                                proc.Print("Mps000276", true);
                                            }
                                        }
                                    }
                                    else
                                    {

                                        HisSereServFilter ssfilter = new HisSereServFilter();
                                        ssfilter.IDs = SereServIds;
                                        sereServs = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV>>("/api/HisSereServ/Get", ApiConsumers.MosConsumer, ssfilter, null);
                                        if (sereServs != null && sereServs.Count > 0 && sereServs.Exists(o => o.SERVICE_REQ_ID.HasValue))
                                        {
                                            HisServiceReqViewFilter filter = new HisServiceReqViewFilter();
                                            filter.IDs = sereServs.Select(o => o.SERVICE_REQ_ID ?? 0).Distinct().ToList();
                                            var serviceReq = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_SERVICE_REQ>>("/api/HisServiceReq/GetView", ApiConsumers.MosConsumer, filter, null);
                                            if (serviceReq != null && serviceReq.Count > 0)
                                            {
                                                HIS.Desktop.Plugins.Library.PrintServiceReqTreatment.PrintServiceReqTreatmentProcessor proc = new Library.PrintServiceReqTreatment.PrintServiceReqTreatmentProcessor(serviceReq, currentModule.RoomId);
                                                proc.Print("Mps000276", true);
                                            }
                                        }
                                    }

                                }
                            }
                            else
                            {
                                pbQr.EditValue = global::HIS.Desktop.Plugins.CreateTransReqQR.Properties.Resources.delete;
                            }
                            if (PosStatic.IsOpenPos())
                                PosStatic.SendData(null);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            try
            {
                btnNew.Enabled = false;
                CommonParam param = new CommonParam();
                TransReqCreateSDO sdo = new TransReqCreateSDO();
                if (this.inputTransReq.TreatmentId > 0)
                    sdo.TreatmentId = this.inputTransReq.TreatmentId;
                sdo.TransReqType = inputTransReq.TransReqId == CreateReqType.Deposit ? IMSys.DbConfig.HIS_RS.HIS_TRANS_REQ_TYPE.ID__BY_DEPOSIT : inputTransReq.TransReqId == CreateReqType.Transaction ? IMSys.DbConfig.HIS_RS.HIS_TRANS_REQ_TYPE.ID__BY_TRANSACTION : IMSys.DbConfig.HIS_RS.HIS_TRANS_REQ_TYPE.ID__BY_SERVICE;
                sdo.SereServIds = SereServIds.Distinct().ToList();
                sdo.RequestRoomId = this.currentModule.RoomId;
                sdo.Amount = this.Amount;
                sdo.DepositReqId = inputTransReq.DepositReq != null ? (long?)inputTransReq.DepositReq.ID : null;
                sdo.TransactionId = inputTransReq.Transaction != null ? (long?)inputTransReq.Transaction.ID : null;
                sdo.TransactionIds = inputTransReq.Transactions != null && inputTransReq.Transactions.Count > 0 ? inputTransReq.Transactions.Select(o => o.ID).Distinct().ToList() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sdo), sdo));
                currentTransReq = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_TRANS_REQ>("api/HisTransReq/CreateSDO", ApiConsumers.MosConsumer, sdo, param);
                if (inputTransReq.TreatmentId <= 0)
                {
                    HisTransactionViewFilter tvf = new HisTransactionViewFilter();
                    tvf.TRANS_REQ_CODE__EXACT = currentTransReq.TRANS_REQ_CODE;
                    transactionPrint = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_TRANSACTION>>("api/HisTransaction/GetView", ApiConsumers.MosConsumer, tvf, null).FirstOrDefault();

                    lblPatientName.Text = transactionPrint.BUYER_NAME ?? transactionPrint.TDL_PATIENT_NAME;
                    lblAddress.Text = transactionPrint.BUYER_ADDRESS ?? transactionPrint.TDL_PATIENT_ADDRESS;
                }
                InitPopupMenuOther();
                if (currentTransReq == null)
                {
                    XtraMessageBox.Show(string.Format("Tạo QR thất bại. {0}", param.GetMessage()));
                }
                else
                {
                    Amount = currentTransReq.AMOUNT;
                    lblAmount.Text = Inventec.Common.Number.Convert.NumberToString(Amount, HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator);
                    btnNew.Enabled = true;
                    btnCreate.Enabled = false;
                    ShowQR();
                    SendData();
                    timerReloadTransReq.Start();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void CallApiCancelTransReq()
        {
            try
            {
                CommonParam param = new CommonParam();
                currentTransReq.TRANS_REQ_STT_ID = IMSys.DbConfig.HIS_RS.HIS_TRANS_REQ_STT.ID__CANCEL;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentTransReq), currentTransReq));
                currentTransReq = new Inventec.Common.Adapter.BackendAdapter(param).Post<HIS_TRANS_REQ>("api/HisTransReq/Update", ApiConsumers.MosConsumer, currentTransReq, param);
                if (currentTransReq != null)
                {
                    lblStt.Text = currentTransReq.TRANS_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_TRANS_REQ_STT.ID__FINISHED ? "Thành công" : currentTransReq.TRANS_REQ_STT_ID != IMSys.DbConfig.HIS_RS.HIS_TRANS_REQ_STT.ID__REQUEST ? "Thất bại" : "Đang chờ";
                    pbQr.EditValue = global::HIS.Desktop.Plugins.CreateTransReqQR.Properties.Resources.delete;
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
                timerReloadTransReq.Stop();
                var lstCheckBank = new List<string>() { "VCB", "MBB", "CTG" };
                if (string.IsNullOrEmpty(inputTransReq.BankName) || !lstCheckBank.Contains(inputTransReq.BankName))
                {
                    if (currentTransReq != null)
                    {
                        {
                            IsCheckNode = false;
                            CallApiCancelTransReq();
                        }

                    }
                }
                else if (lstCheckBank.Contains(inputTransReq.BankName) && currentTransReq != null && lstCheckBank.Exists(o => inputTransReq.ConfigValue.KEY.Contains(o)))
                {
                    {
                        IsCheckNode = false;
                        CommonParam param = new CommonParam();
                        MOS.SDO.QrPaymentCancelSDO tdo = new MOS.SDO.QrPaymentCancelSDO();
                        tdo.Bank = inputTransReq.BankName;
                        tdo.TransReqId = currentTransReq.ID;
                        tdo.BankConfig = inputTransReq.ConfigValue.VALUE;
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => tdo), tdo));
                        var IsCancel = new Inventec.Common.Adapter.BackendAdapter(param).Post<bool>("api/HisTransReq/QrPaymentCancel", ApiConsumers.MosConsumer, tdo, param);
                    }
                }


                QrCodeProcessor.DicContentBank = new Dictionary<string, string>();
                pbQr.Image = null;
                if (inputTransReq.Transaction == null || inputTransReq.Transactions == null || inputTransReq.Transactions.Count == 0)
                    IsCheckNode = true;
                btnCreate.Enabled = true;
                if (PosStatic.IsOpenPos())
                    PosStatic.SendData(null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void ShowQR()
        {
            try
            {
                if (currentTransReq == null)
                    return;
                var data = HIS.Desktop.Common.BankQrCode.QrCodeProcessor.CreateQrImage(currentTransReq, new List<HIS_CONFIG>() { inputTransReq.ConfigValue }).FirstOrDefault();
                using (var ms = new MemoryStream((byte[])data.Value))
                {
                    pbQr.Image = Image.FromStream(ms);
                }
                if (PosStatic.IsOpenPos())
                {
                    if (QrCodeProcessor.DicContentBank.ContainsKey(currentTransReq.TRANS_REQ_CODE))
                        PosStatic.SendData(QrCodeProcessor.DicContentBank[currentTransReq.TRANS_REQ_CODE] + (cboCom.EditValue.ToString() == "SDK Model" ? ("|" + lblAmount.Text + " VNĐ|" + (lblPatientName.Text.Split(' ').ToList().Count > 2 ? string.Join(" ", lblPatientName.Text.Split(' ').ToList().Take(3)) + "\r\n" + string.Join(" ", lblPatientName.Text.Split(' ').ToList().Skip(3).Take(10)) : lblPatientName.Text)) : ""));
                    else
                        PosStatic.SendData(null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Info("IN: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentTransReq), currentTransReq));
                if (currentTransReq == null) return;
                this.btnPrint.ShowDropDown();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        V_HIS_TRANSACTION transactionPrint = null;
        List<V_HIS_TRANSACTION> transactionPrintList = null;
        private void InitPopupMenuOther()
        {
            try
            {
                transactionPrint = null;
                transactionPrintList = null;
                DXPopupMenu menu = new DXPopupMenu();
                if (currentTransReq != null)
                {
                    if (currentTransReq.TRANS_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_TRANS_REQ_STT.ID__FINISHED)
                    {
                        HisTransactionViewFilter tvf = new HisTransactionViewFilter();
                        tvf.TRANS_REQ_CODE__EXACT = currentTransReq.TRANS_REQ_CODE;
                        transactionPrintList = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_TRANSACTION>>("api/HisTransaction/GetView", ApiConsumers.MosConsumer, tvf, null);
                        transactionPrint = transactionPrintList[0];
                    }
                    else if (currentTransReq.TRANS_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_TRANS_REQ_STT.ID__CANCEL && (inputTransReq.TransReqId == CreateReqType.Deposit || inputTransReq.TransReqId == CreateReqType.Transaction) && (inputTransReq.Transaction != null || (inputTransReq.Transactions != null && inputTransReq.Transactions.Count > 0)))
                    {
                        HisTransactionViewFilter tvf = new HisTransactionViewFilter();
                        tvf.IDs = inputTransReq.Transaction != null ? new List<long>() { inputTransReq.Transaction.ID } : (inputTransReq.Transactions != null && inputTransReq.Transactions.Count > 0 ? inputTransReq.Transactions.Select(o => o.ID).ToList() : null);
                        transactionPrintList = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_TRANSACTION>>("api/HisTransaction/GetView", ApiConsumers.MosConsumer, tvf, null);
                        transactionPrint = transactionPrintList[0];
                    }
                    if (currentTransReq.TRANS_REQ_STT_ID != IMSys.DbConfig.HIS_RS.HIS_TRANS_REQ_STT.ID__CANCEL)
                        menu.Items.Add(new DXMenuItem("QR", new EventHandler(onClickQR)));

                    if (transactionPrint != null)
                    {
                        switch (transactionPrint.TRANSACTION_TYPE_ID)
                        {
                            case IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT:
                                if (transactionPrint.SALE_TYPE_ID == null)
                                {
                                    menu.Items.Add(new DXMenuItem("Phiếu thu thanh toán", new EventHandler(onClickThanhToanDv)));
                                }
                                else if (transactionPrint.SALE_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_SALE_TYPE.ID__SALE_EXP)
                                {
                                    menu.Items.Add(new DXMenuItem("Hóa đơn xuất bán", new EventHandler(onClickHoaDonXb)));
                                }
                                break;
                            case IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TU:
                                if (transactionPrint.TDL_SERE_SERV_DEPOSIT_COUNT == null)
                                {
                                    menu.Items.Add(new DXMenuItem("Phiếu thu tạm ứng", new EventHandler(onClickTamUng)));
                                }
                                else
                                {
                                    menu.Items.Add(new DXMenuItem("Phiếu thu phí dịch vụ", new EventHandler(onClickTamUngDv)));
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }
                this.btnPrint.DropDownControl = menu;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void onClickQR(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(HIS.Desktop.ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), HIS.Desktop.LocalStorage.Location.PrintStoreLocation.PrintTemplatePath);
                richEditorMain.RunPrintTemplate("Mps000498", DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void onClickTamUngDv(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, LanguageManager.GetLanguage(), LocalStorage.LocalData.GlobalVariables.TemnplatePathFolder);
                richEditorMain.RunPrintTemplate("Mps000102", DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void onClickThanhToanDv(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, LanguageManager.GetLanguage(), LocalStorage.LocalData.GlobalVariables.TemnplatePathFolder);
                richEditorMain.RunPrintTemplate("Mps000111", DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void onClickHoaDonThanhToan(object sender, EventArgs e)
        {
            try
            {
                if (HisConfigCFG.TransactionBillSelect != "2")
                    return;
                if (HisConfigCFG.BillTwoOption == "2"
                    || HisConfigCFG.BillTwoOption == "3")
                {
                    Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                    foreach (var item in transactionPrintList)
                    {
                        transactionPrint = item;
                        if (transactionPrint.BILL_TYPE_ID == 2)
                        {
                            store.RunPrintTemplate("Mps000318", DelegateRunPrinter);
                        }
                        else
                        {
                            store.RunPrintTemplate("Mps000317", DelegateRunPrinter);
                        }
                    }
                }
                else
                {
                    Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                    foreach (var item in transactionPrintList)
                    {
                        transactionPrint = item;
                        if (transactionPrint.BILL_TYPE_ID == 2)
                        {
                            store.RunPrintTemplate("MPS000147", DelegateRunPrinter);
                        }
                        else
                        {
                            store.RunPrintTemplate("MPS000148", DelegateRunPrinter);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void onClickHoaDonXb(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, LanguageManager.GetLanguage(), LocalStorage.LocalData.GlobalVariables.TemnplatePathFolder);
                richEditorMain.RunPrintTemplate("Mps000339", DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void onClickTamUng(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.RichEditor.RichEditorStore richEditorMain = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumer.ApiConsumers.SarConsumer, HIS.Desktop.LocalStorage.ConfigSystem.ConfigSystems.URI_API_SAR, LanguageManager.GetLanguage(), LocalStorage.LocalData.GlobalVariables.TemnplatePathFolder);
                richEditorMain.RunPrintTemplate("Mps000112", DelegateRunPrinter);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        bool DelegateRunPrinter(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                switch (printTypeCode)
                {
                    case "Mps000498":
                        LoadBieuMau(printTypeCode, fileName, ref result);
                        break;
                    case "Mps000102":
                        LoadBieuMauDepositService(printTypeCode, fileName, ref result);
                        break;
                    case "Mps000112":
                        InPhieuThuTamUng(printTypeCode, fileName, ref result);
                        break;
                    case "Mps000339":
                        InHoaDonXuatBan(printTypeCode, fileName, ref result);
                        break;
                    case "Mps000111":
                        InPhieuThuThanhToan(printTypeCode, fileName, ref result);
                        break;
                    case "Mps000318":
                        InPhieuHoaDonThanhToanHcm115(printTypeCode, fileName, ref result);
                        break;
                    case "MPS000147":
                        InPhieuHoaDonThanhToan(printTypeCode, fileName, ref result);
                        break;
                    case "Mps000317":
                        InPhieuBienLaiThanhToanHcm115(printTypeCode, fileName, ref result);
                        break;
                    case "MPS000148":
                        InPhieuBienLaiThanhToan(printTypeCode, fileName, ref result);
                        break;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            return result;
        }
        private void InPhieuHoaDonThanhToan(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();
                MPS.Processor.Mps000147.PDO.Mps000147PDO rdo = new MPS.Processor.Mps000147.PDO.Mps000147PDO(transactionPrint);
                WaitingManager.Hide();
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode) && !String.IsNullOrEmpty(GlobalVariables.dicPrinter[printTypeCode]))
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, null) { ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog });
                }
                else
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, null) { ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog });
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InPhieuBienLaiThanhToan(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                HisSereServBillFilter ssBillFilter = new HisSereServBillFilter();
                ssBillFilter.BILL_ID = transactionPrint.ID;
                var listSSBill = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV_BILL>>("api/HisSereServBill/Get", ApiConsumers.MosConsumer, ssBillFilter, null);
                if (listSSBill == null || listSSBill.Count <= 0)
                {
                    throw new Exception("Khong lay duoc SSBill theo billId: " + transactionPrint.ID);
                }
                HisSereServFilter filter = new HisSereServFilter();
                filter.IDs = listSSBill.Select(s => s.SERE_SERV_ID).ToList();
                var listSereServ = new BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumers.MosConsumer, filter, null);

                if (listSereServ == null || listSereServ.Count == 0)
                {
                    throw new NullReferenceException("Khong lay duoc SereServ theo resultRecieptBill.ID" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => transactionPrint), transactionPrint));
                }
                MPS.Processor.Mps000148.PDO.Mps000148PDO rdo = new MPS.Processor.Mps000148.PDO.Mps000148PDO(transactionPrint, listSSBill, listSereServ, HisConfigCFG.PatientTypeId__BHYT);
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode) && !String.IsNullOrEmpty(GlobalVariables.dicPrinter[printTypeCode]))
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, GlobalVariables.dicPrinter[printTypeCode]) { ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog });
                }
                else
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, null) { ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog });
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void InPhieuBienLaiThanhToanHcm115(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                MPS.Processor.Mps000317.PDO.Mps000317PDO rdo = new MPS.Processor.Mps000317.PDO.Mps000317PDO(transactionPrint);
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode) && !String.IsNullOrEmpty(GlobalVariables.dicPrinter[printTypeCode]))
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, GlobalVariables.dicPrinter[printTypeCode]) { ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog });
                }
                else
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, null) { ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog });
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void InPhieuHoaDonThanhToanHcm115(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                WaitingManager.Show();
                MPS.Processor.Mps000318.PDO.Mps000318PDO rdo = new MPS.Processor.Mps000318.PDO.Mps000318PDO(transactionPrint);
                WaitingManager.Hide();
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode) && !String.IsNullOrEmpty(GlobalVariables.dicPrinter[printTypeCode]))
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, GlobalVariables.dicPrinter[printTypeCode]) { ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog });
                }
                else
                {
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, null) { ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog });
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void InPhieuThuThanhToan(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                foreach (var item in transactionPrintList)
                {
                    if (!item.TREATMENT_ID.HasValue)
                    {
                        MessageManager.Show("Hóa đơn thanh toán xuất bán thuốc/ vật tư");
                        continue;
                    }
                    if (item.IS_CANCEL == 1)
                    {
                        MessageManager.Show("Giao dịch đã bị hủy.");
                        continue;
                    }
                    WaitingManager.Show();

                    HisBillFundFilter billFundFilter = new HisBillFundFilter();
                    billFundFilter.BILL_ID = item.ID;
                    var listBillFund = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_BILL_FUND>>("api/HisBillFund/Get", ApiConsumers.MosConsumer, billFundFilter, null);

                    HisSereServBillFilter ssBillFilter = new HisSereServBillFilter();
                    ssBillFilter.BILL_ID = item.ID;
                    var hisSSBills = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV_BILL>>("api/HisSereServBill/Get", ApiConsumers.MosConsumer, ssBillFilter, null);
                    if (hisSSBills == null || hisSSBills.Count <= 0)
                    {
                        throw new Exception("Khong lay duoc SSBill theo billId: " + item.BILL_TYPE_ID);
                    }

                    List<HIS_SERE_SERV> listSereServ = new List<HIS_SERE_SERV>();
                    HisSereServFilter ssFilter = new HisSereServFilter();
                    ssFilter.TREATMENT_ID = item.TREATMENT_ID.Value;
                    List<HIS_SERE_SERV> listSereServApi = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV>>("api/HisSereServ/Get", ApiConsumers.MosConsumer, ssFilter, null);

                    if (listSereServApi != null && listSereServApi.Count > 0 && hisSSBills != null && hisSSBills.Count > 0)
                    {
                        listSereServ = listSereServApi.Where(o => hisSSBills.Select(p => p.SERE_SERV_ID).Contains(o.ID)).ToList();
                    }

                    HisPatientTypeAlterViewAppliedFilter patyAlterAppliedFilter = new HisPatientTypeAlterViewAppliedFilter();
                    patyAlterAppliedFilter.InstructionTime = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
                    patyAlterAppliedFilter.TreatmentId = item.TREATMENT_ID.Value;
                    var currentPatientTypeAlter = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, patyAlterAppliedFilter, null);


                    HisDepartmentTranLastFilter departLastFilter = new HisDepartmentTranLastFilter();
                    departLastFilter.TREATMENT_ID = item.TREATMENT_ID.Value;
                    departLastFilter.BEFORE_LOG_TIME = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
                    var departmentTran = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<V_HIS_DEPARTMENT_TRAN>("api/HisDepartmentTran/GetLastByTreatmentId", ApiConsumers.MosConsumer, departLastFilter, null);

                    V_HIS_PATIENT patient = new V_HIS_PATIENT();
                    if (item.TDL_PATIENT_ID != null)
                    {
                        HisPatientViewFilter patientFilter = new HisPatientViewFilter();
                        patientFilter.ID = item.TDL_PATIENT_ID;
                        var patients = new BackendAdapter(new CommonParam()).Get<List<V_HIS_PATIENT>>("api/HisPatient/GetView", ApiConsumer.ApiConsumers.MosConsumer, patientFilter, null);

                        if (patients != null && patients.Count > 0)
                        {
                            patient = patients.FirstOrDefault();
                        }
                    }

                    WaitingManager.Hide();
                    string printerName = "";
                    if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                    {
                        printerName = GlobalVariables.dicPrinter[printTypeCode];
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((item != null ? item.TDL_TREATMENT_CODE : ""), printTypeCode, currentModule != null ? currentModule.RoomId : 0);

                    // Lay thong tin cac dich vu da tam ung khong bi huy
                    List<HIS_SERE_SERV_DEPOSIT> listSereServDeposit = new List<HIS_SERE_SERV_DEPOSIT>();
                    CommonParam paramCommon = new CommonParam();
                    MOS.Filter.HisSereServDepositFilter sereServDepositFilter = new HisSereServDepositFilter();
                    sereServDepositFilter.TDL_TREATMENT_ID = item.TREATMENT_ID;
                    sereServDepositFilter.IS_CANCEL = false;
                    listSereServDeposit = new BackendAdapter(paramCommon).Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_DEPOSIT>>("api/HisSereServDeposit/Get", ApiConsumer.ApiConsumers.MosConsumer, sereServDepositFilter, paramCommon);

                    List<HIS_SESE_DEPO_REPAY> listSeseDepoRepay = new List<HIS_SESE_DEPO_REPAY>();
                    MOS.Filter.HisSeseDepoRepayFilter filterSeseDepoRepay = new MOS.Filter.HisSeseDepoRepayFilter();
                    filterSeseDepoRepay.TDL_TREATMENT_ID = item.TREATMENT_ID;
                    filterSeseDepoRepay.IS_CANCEL = false;
                    listSeseDepoRepay = new Inventec.Common.Adapter.BackendAdapter(paramCommon).Get<List<HIS_SESE_DEPO_REPAY>>("api/HisSeseDepoRepay/Get", ApiConsumer.ApiConsumers.MosConsumer, filterSeseDepoRepay, HIS.Desktop.Controls.Session.SessionManager.ActionLostToken, paramCommon);

                    HisTransactionViewFilter depositFilter = new HisTransactionViewFilter();
                    depositFilter.TREATMENT_ID = item.TREATMENT_ID.Value;
                    var lstTran = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_TRANSACTION>>("api/HisTransaction/GetView", ApiConsumers.MosConsumer, depositFilter, null);

                    LogSystem.Debug("dich vu da hoan ung bang " + listSeseDepoRepay.Count.ToString());
                    List<HIS_SERE_SERV_DEPOSIT> finalListSereServDeposit = new List<HIS_SERE_SERV_DEPOSIT>();

                    if (listSeseDepoRepay != null && listSeseDepoRepay.Count > 0)
                    {
                        finalListSereServDeposit = listSereServDeposit.Where(o => listSeseDepoRepay.All(k => k.SERE_SERV_DEPOSIT_ID != o.ID)).ToList();
                    }
                    else
                    {
                        finalListSereServDeposit = listSereServDeposit;
                    }

                    MPS.Processor.Mps000111.PDO.Mps000111PDO pdo = new MPS.Processor.Mps000111.PDO.Mps000111PDO(item,
                        patient,
                        listBillFund,
                        listSereServ,
                        departmentTran,
                        currentPatientTypeAlter,
                        GetId(HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("MOS.HIS_PATIENT_TYPE.PATIENT_TYPE_CODE.BHYT")),
                        null,
                        finalListSereServDeposit,
                        lstTran,
                        listSeseDepoRepay
                        );
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                    //if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                    //{
                    //    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                    //}
                    //else
                    //{
                    //    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO });
                    //}
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            finally
            {
                WaitingManager.Hide();
            }
        }

        private long GetId(string code)
        {
            long result = 0;
            try
            {
                var data = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.PATIENT_TYPE_CODE == code);
                result = data.ID;
            }
            catch (Exception ex)
            {
                LogSystem.Debug(ex);
                result = 0;
            }
            return result;
        }

        private void InHoaDonXuatBan(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                foreach (var item in transactionPrintList)
                {
                    WaitingManager.Show();

                    CommonParam param = new CommonParam();
                    HisBillGoodsFilter goodsFilter = new HisBillGoodsFilter();
                    goodsFilter.BILL_ID = item.ID;
                    List<HIS_BILL_GOODS> billGoods = new BackendAdapter(param).Get<List<HIS_BILL_GOODS>>("api/HisBillGoods/Get", ApiConsumers.MosConsumer, goodsFilter, param);

                    HisExpMestViewFilter expMestFilter = new HisExpMestViewFilter();
                    expMestFilter.BILL_ID = item.ID;
                    List<V_HIS_EXP_MEST> expMests = new BackendAdapter(param).Get<List<V_HIS_EXP_MEST>>("api/HisExpMest/GetView", ApiConsumers.MosConsumer, expMestFilter, param);

                    HisExpMestMedicineViewFilter expMestMedicineFilter = new HisExpMestMedicineViewFilter();
                    expMestMedicineFilter.EXP_MEST_IDs = expMests.Select(s => s.ID).ToList();
                    List<V_HIS_EXP_MEST_MEDICINE> expMestMedicines = new BackendAdapter(param)
                        .Get<List<V_HIS_EXP_MEST_MEDICINE>>("api/HisExpMestMedicine/GetVIew", ApiConsumers.MosConsumer, expMestMedicineFilter, param);

                    HisExpMestMaterialViewFilter expMestMaterialFilter = new HisExpMestMaterialViewFilter();
                    expMestMaterialFilter.EXP_MEST_IDs = expMests.Select(s => s.ID).ToList();
                    List<V_HIS_EXP_MEST_MATERIAL> expMestMaterials = new BackendAdapter(param)
                        .Get<List<V_HIS_EXP_MEST_MATERIAL>>("api/HisExpMestMaterial/GetVIew", ApiConsumers.MosConsumer, expMestMaterialFilter, param);

                    HisExpMestFilter hisexpmestFilter = new HisExpMestFilter();
                    hisexpmestFilter.BILL_ID = item.ID;
                    List<V_HIS_EXP_MEST> hisexpmest = new BackendAdapter(param)
                        .Get<List<V_HIS_EXP_MEST>>("api/HisExpMest/GetVIew", ApiConsumers.MosConsumer, hisexpmestFilter, param);

                    HisImpMestFilter hisimpmestFilter = new HisImpMestFilter();
                    hisimpmestFilter.MOBA_EXP_MEST_IDs = hisexpmest.Select(o => o.ID).ToList();
                    List<V_HIS_IMP_MEST> hisimpmest = new BackendAdapter(param)
                        .Get<List<V_HIS_IMP_MEST>>("api/HisImpMest/GetVIew", ApiConsumers.MosConsumer, hisimpmestFilter, param);

                    MPS.Processor.Mps000339.PDO.Mps000339PDO rdo = new MPS.Processor.Mps000339.PDO.Mps000339PDO(transactionPrint, billGoods, expMestMedicines, expMestMaterials, hisexpmest, hisimpmest);

                    string printerName = "";
                    if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                    {
                        printerName = GlobalVariables.dicPrinter[printTypeCode];
                    }

                    MPS.ProcessorBase.Core.PrintData printdata = null;
                    printdata = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                    //if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                    //{
                    //    printdata = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName);
                    //}
                    //else
                    //{
                    //    printdata = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName);
                    //} 

                    WaitingManager.Hide();
                    result = MPS.MpsPrinter.Run(printdata);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InPhieuThuTamUng(string printTypeCode, string fileName, ref bool result)
        {
            try
            {
                foreach (var item in transactionPrintList)
                {
                    if (item.IS_CANCEL == 1)
                    {
                        if (item.CREATOR != Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName() && item.CANCEL_LOGINNAME != Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName() && !CheckLoginAdmin.IsAdmin(Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName()))
                        {
                            MessageManager.Show("Bạn không có quyền in giao dịch đã hủy");
                            continue; ;
                        }
                    }
                    WaitingManager.Show();
                    V_HIS_PATIENT patient = null;

                    HisTransactionViewFilter depositFilter = new HisTransactionViewFilter();
                    depositFilter.ID = item.ID;
                    var listDeposit = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_TRANSACTION>>("api/HisTransaction/GetView", ApiConsumers.MosConsumer, depositFilter, null);
                    if (listDeposit == null || listDeposit.Count != 1)
                    {
                        throw new Exception("Khong lay duoc V_HIS_DEPOSIT theo transactionId, TransactionCode");
                    }

                    var deposit = listDeposit.First();

                    if (item.TDL_PATIENT_ID.HasValue)
                    {
                        HisPatientViewFilter patientFilter = new HisPatientViewFilter();
                        patientFilter.ID = item.TDL_PATIENT_ID;
                        var listPatient = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_PATIENT>>(HisRequestUriStore.HIS_PATIENT_GETVIEW, ApiConsumers.MosConsumer, patientFilter, null);
                        patient = listPatient.First();
                    }

                    decimal ratio = 0;
                    var PatyAlterBhyt = new V_HIS_PATIENT_TYPE_ALTER();
                    PrintGlobalStore.LoadCurrentPatientTypeAlter(item.TREATMENT_ID.Value, 0, ref PatyAlterBhyt);
                    if (PatyAlterBhyt != null && !String.IsNullOrEmpty(PatyAlterBhyt.HEIN_CARD_NUMBER))
                    {
                        ratio = new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(PatyAlterBhyt.HEIN_TREATMENT_TYPE_CODE, PatyAlterBhyt.HEIN_CARD_NUMBER, PatyAlterBhyt.LEVEL_CODE, PatyAlterBhyt.RIGHT_ROUTE_CODE) ?? 0;
                    }

                    HisDepartmentTranViewFilter departLastFilter = new HisDepartmentTranViewFilter();
                    departLastFilter.TREATMENT_ID = item.TREATMENT_ID.Value;
                    var departmentTrans = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_DEPARTMENT_TRAN>>("api/HisDepartmentTran/GetView", ApiConsumers.MosConsumer, departLastFilter, null);

                    string printerName = "";
                    if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                    {
                        printerName = GlobalVariables.dicPrinter[printTypeCode];
                    }

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((deposit != null ? deposit.TREATMENT_CODE : ""), printTypeCode, currentModule != null ? currentModule.RoomId : 0);

                    MPS.Processor.Mps000112.PDO.Mps000112ADO ado = new MPS.Processor.Mps000112.PDO.Mps000112ADO();

                    HisTransactionFilter depositCountFilter = new HisTransactionFilter();
                    depositCountFilter.TREATMENT_ID = item.TREATMENT_ID;
                    depositCountFilter.TRANSACTION_TIME_TO = item.TRANSACTION_TIME;
                    var deposits = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_TRANSACTION>>("api/HisTransaction/Get", ApiConsumers.MosConsumer, depositCountFilter, null);
                    if (deposits != null && deposits.Count > 0)
                    {
                        ado.DEPOSIT_NUM_ORDER = deposits.Where(o => o.IS_CANCEL != 1 && o.IS_DELETE == 0 && o.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TU).Count();
                        ado.DEPOSIT_SERVICE_NUM_ORDER = deposits.Where(o => o.TDL_SERE_SERV_DEPOSIT_COUNT != null && o.IS_CANCEL != 1 && o.IS_DELETE == 0).Count().ToString();
                    }

                    V_HIS_TREATMENT treatment = null;
                    HisTreatmentViewFilter filter = new HisTreatmentViewFilter();
                    filter.ID = item.TREATMENT_ID;
                    var treatmentList = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_TREATMENT>>("api/HisTreatment/GetView", ApiConsumers.MosConsumer, filter, null);
                    if (treatmentList != null && treatmentList.Count > 0)
                        treatment = treatmentList.First();
                    MPS.Processor.Mps000112.PDO.Mps000112PDO rdo =
                        new MPS.Processor.Mps000112.PDO.Mps000112PDO(deposit, null, ratio, PatyAlterBhyt, departmentTrans, ado, treatment, BackendDataWorker.Get<HIS_TREATMENT_TYPE>());
                    MPS.ProcessorBase.Core.PrintData printData = null;
                    WaitingManager.Hide();
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO, ShowPrintLog = (MPS.ProcessorBase.PrintConfig.DelegateShowPrintLog)CallModuleShowPrintLog });
                }
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            finally
            {
                WaitingManager.Hide();
            }
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

        private void LoadBieuMauDepositService(string printTypeCode, string fileName, ref bool result)
        {

            try
            {
                foreach (var item in transactionPrintList)
                {

                    Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((hisTreatmentView != null ? hisTreatmentView.TREATMENT_CODE : ""), printTypeCode, this.currentModule.RoomId);

                    //chỉ định chưa có thời gian ra viện nên chưa cso số ngày điều trị
                    long? totalDay = null;
                    string departmentName = "";

                    CommonParam param = new CommonParam();
                    HisPatientViewFilter df = new HisPatientViewFilter();
                    df.ID = hisTreatmentView.PATIENT_ID;
                    var patientPrint = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_PATIENT>>("/api/HisPatient/GetView", ApiConsumers.MosConsumer, df, null).FirstOrDefault();

                    HisPatientTypeAlterViewFilter ft = new HisPatientTypeAlterViewFilter();
                    ft.TDL_PATIENT_ID = hisTreatmentView.PATIENT_ID;
                    var currentHisPatientTypeAlter = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_PATIENT_TYPE_ALTER>>("api/HisPatientTypeAlter/GetView", ApiConsumers.MosConsumer, ft, null).FirstOrDefault();

                    MOS.Filter.HisTreatmentFeeViewFilter filterTreatmentFee = new MOS.Filter.HisTreatmentFeeViewFilter();
                    filterTreatmentFee.ID = this.hisTreatmentView.ID;
                    var treatmentPrint = new BackendAdapter(null)
                      .Get<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_FEE>>("api/HisTreatment/GetFeeView", ApiConsumer.ApiConsumers.MosConsumer, filterTreatmentFee, null).FirstOrDefault();


                    V_HIS_SERVICE_REQ firsExamRoom = new V_HIS_SERVICE_REQ();
                    if (this.hisTreatmentView.TDL_FIRST_EXAM_ROOM_ID.HasValue)
                    {
                        var room = BackendDataWorker.Get<V_HIS_ROOM>().FirstOrDefault(o => o.ID == this.hisTreatmentView.TDL_FIRST_EXAM_ROOM_ID);
                        if (room != null)
                        {
                            firsExamRoom.EXECUTE_ROOM_NAME = room.ROOM_NAME;
                        }
                    }

                    string ratio_text = ((new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(currentHisPatientTypeAlter.HEIN_TREATMENT_TYPE_CODE, currentHisPatientTypeAlter.HEIN_CARD_NUMBER, currentHisPatientTypeAlter.LEVEL_CODE, currentHisPatientTypeAlter.RIGHT_ROUTE_CODE) ?? 0) * 100) + "";

                    //sử dụng DepositedSereServs để hiển thị thêm dịch vụ thanh toán cha
                    List<V_HIS_SERE_SERV_5> sereServs5 = new List<V_HIS_SERE_SERV_5>();
                    List<V_HIS_SERE_SERV> sereServs = new List<V_HIS_SERE_SERV>();

                    if (SereServIds != null && SereServIds.Count > 0)
                        sereServs5 = sereServByTreatment.Where(o => SereServIds.Exists(p => p == o.ID)).ToList();
                    else if (item != null && item.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TU && item.TDL_SERE_SERV_DEPOSIT_COUNT != null)
                    {
                        MOS.Filter.HisSereServDepositFilter sereServDepositFilter = new HisSereServDepositFilter();
                        sereServDepositFilter.DEPOSIT_ID = item.ID;
                        var sereServDeposits = new BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_DEPOSIT>>("api/HisSereServDeposit/Get", ApiConsumer.ApiConsumers.MosConsumer, sereServDepositFilter, param);
                        if (sereServDeposits != null && sereServDeposits.Count > 0)
                        {
                            HisSereServView5Filter ss5 = new HisSereServView5Filter();
                            ss5.IDs = sereServDeposits.Select(o => o.SERE_SERV_ID).ToList();
                            sereServs5 = new BackendAdapter(null)
          .Get<List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV_5>>("api/HisSereServ/GetView5", ApiConsumer.ApiConsumers.MosConsumer, ss5, null);

                        }
                    }

                    List<MPS.Processor.Mps000102.PDO.SereServGroupPlusADO> sereServNotHitechADOs = new List<MPS.Processor.Mps000102.PDO.SereServGroupPlusADO>();
                    List<MPS.Processor.Mps000102.PDO.SereServGroupPlusADO> sereServHitechADOs = new List<MPS.Processor.Mps000102.PDO.SereServGroupPlusADO>();
                    List<MPS.Processor.Mps000102.PDO.SereServGroupPlusADO> sereServVTTTADOs = new List<MPS.Processor.Mps000102.PDO.SereServGroupPlusADO>();
                    Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sereServs5), sereServs5));
                    if (sereServs5 != null && sereServs5.Count > 0)
                    {
                        HisSereServViewFilter ssf = new HisSereServViewFilter();
                        ssf.IDs = sereServs5.Select(o => o.ID).ToList();
                        sereServs = new BackendAdapter(null)
                .Get<List<MOS.EFMODEL.DataModels.V_HIS_SERE_SERV>>("api/HisSereServ/GetView", ApiConsumer.ApiConsumers.MosConsumer, ssf, null);


                        var SERVICE_REPORT_ID__HIGHTECH = IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__DVKTC;
                        var sereServHitechs = sereServs.Where(o => o.TDL_HEIN_SERVICE_TYPE_ID == SERVICE_REPORT_ID__HIGHTECH).ToList();
                        sereServHitechADOs = PriceBHYTSereServAdoProcess(sereServHitechs);
                        //các sereServ trong nhóm vật tư
                        var SERVICE_REPORT__MATERIAL_VTTT_ID = IMSys.DbConfig.HIS_RS.HIS_HEIN_SERVICE_TYPE.ID__VT_TT;
                        var sereServVTTTs = sereServs.Where(o => o.TDL_HEIN_SERVICE_TYPE_ID == SERVICE_REPORT__MATERIAL_VTTT_ID && o.IS_OUT_PARENT_FEE != null).ToList();
                        sereServVTTTADOs = PriceBHYTSereServAdoProcess(sereServVTTTs);

                        var sereServNotHitechs = sereServs.Where(o => o.TDL_HEIN_SERVICE_TYPE_ID != SERVICE_REPORT_ID__HIGHTECH).ToList();
                        var servicePatyPrpos = BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => o.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE).ToList();
                        //Cộng các sereServ trong gói vào dv ktc
                        foreach (var sereServHitech in sereServHitechADOs)
                        {
                            List<MPS.Processor.Mps000102.PDO.SereServGroupPlusADO> sereServVTTTInKtcADOs = new List<MPS.Processor.Mps000102.PDO.SereServGroupPlusADO>();
                            var sereServVTTTInKtcs = sereServs.Where(o => o.PARENT_ID == sereServHitech.ID && o.IS_OUT_PARENT_FEE == null).ToList();
                            sereServVTTTInKtcADOs = PriceBHYTSereServAdoProcess(sereServVTTTInKtcs);
                            if (sereServHitech.PRICE_POLICY != null)
                            {
                                var servicePatyPrpo = servicePatyPrpos.Where(o => o.ID == sereServHitech.SERVICE_ID && o.BILL_PATIENT_TYPE_ID == sereServHitech.PATIENT_TYPE_ID && o.PACKAGE_PRICE == sereServHitech.PRICE_POLICY).ToList();
                                if (servicePatyPrpo != null && servicePatyPrpo.Count > 0)
                                {
                                    sereServHitech.VIR_PRICE = sereServHitech.PRICE;
                                }
                            }
                            else
                                sereServHitech.VIR_PRICE += sereServVTTTInKtcADOs.Sum(o => o.VIR_TOTAL_PRICE);

                            sereServHitech.VIR_HEIN_PRICE += sereServVTTTInKtcADOs.Sum(o => o.VIR_HEIN_PRICE);
                            sereServHitech.VIR_PATIENT_PRICE += sereServVTTTInKtcADOs.Sum(o => o.VIR_HEIN_PRICE);

                            decimal totalHeinPrice = 0;
                            foreach (var sereServVTTTInKtcADO in sereServVTTTInKtcADOs)
                            {
                                totalHeinPrice += sereServVTTTInKtcADO.AMOUNT * sereServVTTTInKtcADO.PRICE_BHYT;
                            }
                            sereServHitech.PRICE_BHYT += totalHeinPrice;
                            sereServHitech.HEIN_LIMIT_PRICE += sereServVTTTInKtcADOs.Sum(o => o.HEIN_LIMIT_PRICE);

                            sereServHitech.VIR_TOTAL_PRICE += sereServVTTTInKtcADOs.Sum(o => o.VIR_TOTAL_PRICE);
                            sereServHitech.VIR_TOTAL_HEIN_PRICE += sereServVTTTInKtcADOs.Sum(o => o.VIR_TOTAL_HEIN_PRICE);
                            sereServHitech.VIR_TOTAL_PATIENT_PRICE = sereServHitech.VIR_TOTAL_PRICE - sereServHitech.VIR_TOTAL_HEIN_PRICE;
                            sereServHitech.SERVICE_UNIT_NAME = BackendDataWorker.Get<HIS_SERVICE_UNIT>().FirstOrDefault(o => o.ID == sereServHitech.TDL_SERVICE_UNIT_ID).SERVICE_UNIT_NAME;
                        }

                        //Lọc các sereServ nằm không nằm trong dịch vụ ktc và vật tư thay thế
                        //
                        var sereServDeleteADOs = new List<MPS.Processor.Mps000102.PDO.SereServGroupPlusADO>();
                        foreach (var sereServVTTTADO in sereServVTTTADOs)
                        {
                            var sereServADODelete = sereServHitechADOs.Where(o => o.ID == sereServVTTTADO.PARENT_ID).ToList();
                            if (sereServADODelete.Count == 0)
                            {
                                sereServDeleteADOs.Add(sereServVTTTADO);
                            }
                        }

                        foreach (var sereServDelete in sereServDeleteADOs)
                        {
                            sereServVTTTADOs.Remove(sereServDelete);
                        }
                        var sereServVTTTIds = sereServVTTTADOs.Select(o => o.ID);
                        sereServNotHitechs = sereServNotHitechs.Where(o => !sereServVTTTIds.Contains(o.ID)).ToList();
                        sereServNotHitechADOs = PriceBHYTSereServAdoProcess(sereServNotHitechs);

                        if (sereServNotHitechADOs != null && sereServNotHitechADOs.Count > 0)
                        {
                            sereServNotHitechADOs = sereServNotHitechADOs.OrderBy(o => o.TDL_SERVICE_NAME).ToList();
                        }

                        if (sereServHitechADOs != null && sereServHitechADOs.Count > 0)
                        {
                            sereServHitechADOs = sereServHitechADOs.OrderBy(o => o.TDL_SERVICE_NAME).ToList();
                        }

                        if (sereServVTTTADOs != null && sereServVTTTADOs.Count > 0)
                        {
                            sereServVTTTADOs = sereServVTTTADOs.OrderBy(o => o.TDL_SERVICE_NAME).ToList();
                        }
                    }
                    MPS.Processor.Mps000102.PDO.PatientADO patientAdo = new MPS.Processor.Mps000102.PDO.PatientADO(patientPrint);

                    MPS.Processor.Mps000102.PDO.Mps000102PDO mps000102RDO = new MPS.Processor.Mps000102.PDO.Mps000102PDO(
                            patientAdo,
                            currentHisPatientTypeAlter,
                            departmentName,

                            sereServNotHitechADOs,
                            sereServHitechADOs,
                            sereServVTTTADOs,

                            null,//bản tin chuyển khoa, mps lấy ramdom thời gian vào khoa khi chỉ định tạm thời chưa cần
                            treatmentPrint,

                            BackendDataWorker.Get<HIS_HEIN_SERVICE_TYPE>(),
                            item,
                            sereServDeposits,
                            totalDay,
                            ratio_text,
                            firsExamRoom
                            );

                    string printerName = "";
                    if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                    {
                        printerName = GlobalVariables.dicPrinter[printTypeCode];
                    }
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000102RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                    //if (ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                    //{
                    //    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000102RDO, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                    //}
                    //else
                    //{
                    //    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, mps000102RDO, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO });
                    //}
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        public List<MPS.Processor.Mps000102.PDO.SereServGroupPlusADO> PriceBHYTSereServAdoProcess(List<V_HIS_SERE_SERV> sereServs)
        {
            List<MPS.Processor.Mps000102.PDO.SereServGroupPlusADO> sereServADOs = new List<MPS.Processor.Mps000102.PDO.SereServGroupPlusADO>();
            try
            {
                foreach (var item in sereServs)
                {
                    MPS.Processor.Mps000102.PDO.SereServGroupPlusADO sereServADO = new MPS.Processor.Mps000102.PDO.SereServGroupPlusADO();
                    Inventec.Common.Mapper.DataObjectMapper.Map<MPS.Processor.Mps000102.PDO.SereServGroupPlusADO>(sereServADO, item);

                    if (sereServADO.PATIENT_TYPE_ID != HisConfigCFG.PatientTypeId__BHYT)
                    {
                        sereServADO.PRICE_BHYT = 0;
                    }
                    else
                    {
                        if (sereServADO.HEIN_LIMIT_PRICE != null && sereServADO.HEIN_LIMIT_PRICE > 0)
                            sereServADO.PRICE_BHYT = (item.HEIN_LIMIT_PRICE ?? 0);
                        else
                            sereServADO.PRICE_BHYT = item.VIR_PRICE_NO_ADD_PRICE ?? 0;
                    }

                    sereServADOs.Add(sereServADO);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return sereServADOs;
        }

        private void LoadBieuMau(string printTypeCode, string fileName, ref bool result)
        {

            try
            {
                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((hisTreatmentView != null ? hisTreatmentView.TREATMENT_CODE : ""), printTypeCode, this.currentModule.RoomId);

                MPS.Processor.Mps000498.PDO.Mps000498PDO rdo = new MPS.Processor.Mps000498.PDO.Mps000498PDO(
                        hisTreatmentView,
                        currentTransReq,
                        new List<HIS_CONFIG>() { inputTransReq.ConfigValue }
                        );
                result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                //if (HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.CheDoInChoCacChucNangTrongPhanMem == 2)
                //{
                //    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                //}
                //else
                //{
                //    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, rdo, MPS.ProcessorBase.PrintConfig.PreviewType.Show, printerName) { EmrInputADO = inputADO });
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void frmCreateTransReqQR_FormClosing(object sender, FormClosingEventArgs e)
        {

            try
            {
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == cboCom.Name && o.MODULE_LINK == currentModule.ModuleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = cboCom.EditValue != null ? cboCom.EditValue.ToString() : null;
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = cboCom.Name;
                    csAddOrUpdate.VALUE = cboCom.EditValue != null ? cboCom.EditValue.ToString() : null;
                    csAddOrUpdate.MODULE_LINK = currentModule.ModuleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                if (currentTransReq != null && currentTransReq.TRANS_REQ_STT_ID == IMSys.DbConfig.HIS_RS.HIS_TRANS_REQ_STT.ID__REQUEST)
                {
                    if (XtraMessageBox.Show("Bạn có muốn tắt chức năng và hủy yêu cầu thanh toán hay không?", "Thông báo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        btnNew_Click(null, null);
                    }
                    else
                    {
                        e.Cancel = true;
                        return;
                    }
                }
                else
                {
                    if (PosStatic.IsOpenPos())
                    {
                        PosStatic.SendData(null);
                    }
                }
                if (frmSubSc != null)
                    frmSubSc.Close();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void PRINT_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (btnPrint.Enabled && btnPrint.Visible)
                btnPrint_Click(null, null);
        }

        private void NEW_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (btnNew.Enabled && btnNew.Visible)
                btnNew_Click(null, null);
        }

        private void CREATE_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (btnCreate.Enabled && btnCreate.Visible)
                btnCreate_Click(null, null);
        }

        private void cboCom_ButtonClick(object sender, ButtonPressedEventArgs e)
        {

            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                {
                    cboCom.EditValue = null;
                    if (PosStatic.IsOpenPos())
                        PosStatic.DisposePort();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        bool IsConnectOld = false;
        bool IsConnectCom = false;
        private List<LoaiPhieuInADO> lstLoaiPhieu;

        private void btnConnect_Click(object sender, EventArgs e)
        {

            try
            {
                if (btnConnect.Text == "Ngắt kết nối")
                {
                    btnConnect.Text = "Kết nối";
                    cboCom.Enabled = true;
                    if (PosStatic.IsOpenPos())
                    {
                        PosStatic.DisposePort();
                    }
                }
                else if (cboCom.EditValue != null)
                {
                    IsConnectCom = true;
                    string messError = null;
                    if (!PosStatic.IsOpenPos())
                    {
                        if (PosStatic.OpenPos(cboCom.EditValue.ToString() == "SDK Model" ? OptionPos.Library : OptionPos.Port, !IsConnectOld, cboCom.EditValue.ToString(), GetRecivedMessageDevice, ref messError))
                        {
                            cboCom.Enabled = false;
                            btnConnect.Text = "Ngắt kết nối";

                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => QrCodeProcessor.DicContentBank), QrCodeProcessor.DicContentBank));

                            Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => currentTransReq), currentTransReq));
                            if (currentTransReq != null && QrCodeProcessor.DicContentBank.ContainsKey(currentTransReq.TRANS_REQ_CODE))
                                PosStatic.SendData(QrCodeProcessor.DicContentBank[currentTransReq.TRANS_REQ_CODE] + (cboCom.EditValue.ToString() == "SDK Model" ? ("|" + lblAmount.Text + " VNĐ|" + (lblPatientName.Text.Split(' ').ToList().Count > 2 ? string.Join(" ", lblPatientName.Text.Split(' ').ToList().Take(3)) + "\r\n" + string.Join(" ", lblPatientName.Text.Split(' ').ToList().Skip(3).Take(10)) : lblPatientName.Text)) : ""));
                            else
                                PosStatic.SendData(null);
                        }
                        else if (!string.IsNullOrEmpty(messError))
                        {
                            PosStatic.Pos = null;
                            XtraMessageBox.Show(messError);
                        }
                    }
                    else if (PosStatic.IsOpenPos())
                    {
                        cboCom.Enabled = false;
                        btnConnect.Text = "Ngắt kết nối";
                        Inventec.Common.Logging.LogSystem.Info("Mở lại kết nối tới thiết bị IPOS");
                    }
                }
                else if (PosStatic.IsOpenPos())
                {
                    PosStatic.DisposePort();
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void GetRecivedMessageDevice(bool IsSuccess, string Message)
        {

            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate { UpdateSttButton(IsSuccess, Message); }));
                }
                else
                {
                    UpdateSttButton(IsSuccess, Message);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        private void UpdateSttButton(bool IsSuccess, string Message)
        {
            try
            {
                if (IsSuccess)
                {
                    cboCom.Enabled = false;
                    btnConnect.Text = "Ngắt kết nối";
                }
                else
                {
                    cboCom.Enabled = true;
                    btnConnect.Text = "Kết nối";
                }
                if (IsConnectCom)
                {
                    if ((IsConnectOld && !IsSuccess) || !IsConnectOld)
                    {
                        if (!string.IsNullOrEmpty(Message))
                            XtraMessageBox.Show(Message);
                        IsConnectOld = false;
                    }
                    IsConnectCom = false;
                }
                else
                {
                    if (!IsSuccess && !string.IsNullOrEmpty(Message))
                    {
                        XtraMessageBox.Show(Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        private void loadCauHinhIn()
        {
            try
            {
                lstLoaiPhieu = new List<LoaiPhieuInADO>()
                {
                    new LoaiPhieuInADO("Mps000102", "Phiếu thu phí dịch vụ",true),
                    new LoaiPhieuInADO("Mps000276", "Hướng dẫn bệnh nhân",true),
                };

                gridView1.BeginUpdate();
                gridView1.GridControl.DataSource = lstLoaiPhieu;
                gridView1.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void gridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                WaitingManager.Show();
                foreach (var item in lstLoaiPhieu)
                {
                    HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == item.ID && o.MODULE_LINK == ModuleLink).FirstOrDefault() : null;
                    if (csAddOrUpdate != null)
                    {
                        csAddOrUpdate.VALUE = (item.Check ? "1" : "");
                    }
                    else
                    {
                        csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                        csAddOrUpdate.KEY = item.ID;
                        csAddOrUpdate.VALUE = (item.Check ? "1" : "");
                        csAddOrUpdate.MODULE_LINK = ModuleLink;
                        if (this.currentControlStateRDO == null)
                            this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                        this.currentControlStateRDO.Add(csAddOrUpdate);
                    }
                }
                this.controlStateWorker.SetData(this.currentControlStateRDO);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void repositoryItemCheckEdit1_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                CheckEdit edit = sender as CheckEdit;
                if (edit != null)
                {
                    gridView1.SetRowCellValue(gridView1.FocusedRowHandle, gridColumn2, edit.Checked);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSetting_Click(object sender, EventArgs e)
        {
            try
            {
                popupControlContainer1.ShowPopup(new Point(LocationX + 300, LocationY + 160));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        int LocationX = 0;
        int LocationY = 0;
        private void frmCreateTransReqQR_LocationChanged(object sender, EventArgs e)
        {

            try
            {
                LocationX = btnSetting.Bounds.X;
                LocationY = btnSetting.Bounds.Y;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        frmSubScreen frmSubSc = null;
        private void chkOtherScreen_CheckedChanged(object sender, EventArgs e)
        {

            try
            {
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkOtherScreen.Name && o.MODULE_LINK == currentModule.ModuleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = chkOtherScreen.Checked ? "1" : "0";
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkOtherScreen.Name;
                    csAddOrUpdate.VALUE = chkOtherScreen.Checked ? "1" : "0";
                    csAddOrUpdate.MODULE_LINK = currentModule.ModuleLink;
                    if (this.currentControlStateRDO == null)
                        this.currentControlStateRDO = new List<HIS.Desktop.Library.CacheClient.ControlStateRDO>();
                    this.currentControlStateRDO.Add(csAddOrUpdate);
                }
                frmSubSc = new frmSubScreen(hisTreatmentView);
                dlg = new SubScreenDelegate(frmSubSc.dataGet);
                if (chkOtherScreen.Checked && frmSubSc != null)
                {
                    ShowFormInExtendMonitor(frmSubSc);
                }
                else
                {
                    TurnOffExtendMonitor(frmSubSc);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void ShowFormInExtendMonitor(Form control)
        {
            try
            {
                Screen[] sc;
                sc = Screen.AllScreens;
                if (sc.Length <= 1)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Không tìm thấy màn hình mở rộng");
                    control.Show();
                }
                else
                {
                    Screen secondScreen = sc.FirstOrDefault(o => o != Screen.PrimaryScreen);
                    //control.FormBorderStyle = FormBorderStyle.None;
                    control.Left = secondScreen.Bounds.Width;
                    control.Top = secondScreen.Bounds.Height;
                    control.StartPosition = FormStartPosition.Manual;
                    control.Location = secondScreen.Bounds.Location;
                    Point p = new Point(secondScreen.Bounds.Location.X, secondScreen.Bounds.Location.Y);
                    control.Location = p;
                    control.WindowState = FormWindowState.Maximized;
                    control.Show();
                }

                SendData();
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }
        private void TurnOffExtendMonitor(Form control)
        {
            try
            {
                if (control != null)
                {
                    if (Application.OpenForms != null && Application.OpenForms.Count > 0)
                    {
                        for (int i = 0; i < Application.OpenForms.Count; i++)
                        {
                            Form f = Application.OpenForms[i];
                            if (f.Name == control.Name)
                            {
                                f.Close();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error(ex);
            }
        }

        private void pbQr_EditValueChanged(object sender, EventArgs e)
        {

            try
            {
                SendData();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
        private void SendData()
        {

            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate
                    {
                        if (dlg != null)
                            dlg(new DataSubScreen() { image = pbQr.Image, amount = lblAmount.Text, status = lblStt.Text });
                    }));
                }
                else
                {
                    if (dlg != null)
                        dlg(new DataSubScreen() { image = pbQr.Image, amount = lblAmount.Text, status = lblStt.Text });
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void cboPayForm_ButtonClick(object sender, ButtonPressedEventArgs e)
        {

            try
            {
                if (e.Button.Kind == ButtonPredefines.Delete)
                    cboPayForm.EditValue = null;
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
                if (IsLoadFirst)
                    return;
                if (cboPayForm.EditValue != null && cboPayForm.EditValue != cboPayForm.OldEditValue)
                {
                    if (XtraMessageBox.Show("Bạn muốn đổi hình thực của giao dịch? Khi bạn thay đổi thì giao dịch sẽ tự động mở khóa và Mã QR sẽ bị hủy.", "Thông báo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        CommonParam param = new CommonParam();
                        MOS.SDO.TransactionChangePayFormSDO sdo = new MOS.SDO.TransactionChangePayFormSDO();
                        if (inputTransReq.Transaction != null)
                            sdo.TransactionIds = new List<long>() { inputTransReq.Transaction.ID };
                        else if (inputTransReq.Transactions != null && inputTransReq.Transactions.Count > 0)
                            sdo.TransactionIds = inputTransReq.Transactions.Select(o => o.ID).ToList();
                        sdo.PayFormId = Int64.Parse(cboPayForm.EditValue.ToString());
                        sdo.RequestRoomId = currentModule.RoomId;
                        sdo.IsNeedUnlock = true;
                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => sdo), sdo));
                        var Transactions = new Inventec.Common.Adapter.BackendAdapter(param).Post<List<HIS_TRANSACTION>>("api/HisTransaction/ChangePayForm", ApiConsumers.MosConsumer, sdo, param);

                        Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => param), param));
                        if (Transactions != null && Transactions.Count > 0)
                        {
                            if (PosStatic.IsOpenPos())
                                PosStatic.SendData(null);
                            btnNew.Enabled = btnCreate.Enabled = false;
                            btnPrint.Enabled = true;
                            cboPayForm.Enabled = false;
                            CallApiCancelTransReq();
                            InitPopupMenuOther();
                        }
                        else
                        {
                            MessageManager.Show(this, param, false);
                            cboPayForm.EditValue = cboPayForm.OldEditValue;
                        }
                    }
                    else
                    {
                        cboPayForm.EditValue = cboPayForm.OldEditValue;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }
    }
    public class ComQR
    {
        public string comName { get; set; }
    }
    public class DataSubScreen
    {
        public Image image { get; set; }
        public string amount { get; set; }
        public string status { get; set; }
    }
}

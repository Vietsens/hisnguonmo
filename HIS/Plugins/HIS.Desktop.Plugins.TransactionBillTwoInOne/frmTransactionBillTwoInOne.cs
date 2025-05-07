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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigApplication;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.LocalStorage.Location;
using HIS.Desktop.Plugins.TransactionBillTwoInOne.ADO;
using HIS.Desktop.Plugins.TransactionBillTwoInOne.Config;
using HIS.Desktop.Plugins.TransactionBillTwoInOne.Validation;
using HIS.Desktop.Plugins.TransactionBillTwoInOne.Validtion;
using HIS.Desktop.Utility;
using HIS.UC.SereServTree;
using Inventec.Common.Adapter;
using Inventec.Common.Integrate.EditorLoader;
using Inventec.Core;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.LibraryBillTwoBook;
using MOS.LibraryHein.HcmPoorFund;
using SAR.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WCF.Client;
using WCF;
using System.Diagnostics;
using HIS.Desktop.ADO;
using DevExpress.XtraBars;
using Inventec.Common.Logging;

namespace HIS.Desktop.Plugins.TransactionBillTwoInOne
{
    public partial class frmTransactionBillTwoInOne : HIS.Desktop.Utility.FormBase
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
        private const string HFS_KEY__PAY_FORM_CODE = "HFS_KEY__PAY_FORM_CODE";
        private const string HIS_CONFIG__PRINT_TYPE__PRINTER = "His.Config.PrintType.Printer";

        bool isSavePrint = false;
        bool isInit = true;

        List<V_HIS_TRANSACTION> listTransaction = new List<V_HIS_TRANSACTION>();
        Dictionary<long, List<HIS_SERE_SERV_BILL>> dicSereServBill = new Dictionary<long, List<HIS_SERE_SERV_BILL>>();

        V_HIS_TRANSACTION resultRecieptBill = null;
        V_HIS_TRANSACTION resultInvoiceBill = null;

        List<VHisBillFundADO> ListBillFund = new List<VHisBillFundADO>();
        List<V_HIS_SERE_SERV_5> ListSereServ = new List<V_HIS_SERE_SERV_5>();
        List<V_HIS_SERE_SERV_5> inputSereServs = null;
        List<VHisSereServADO> listSereServADO = new List<VHisSereServADO>();
        BindingList<VHisSereServADO> records;

        List<VHisSereServADO> listInvoiceData = new List<VHisSereServADO>();
        List<VHisSereServADO> listRecieptData = new List<VHisSereServADO>();

        List<VHisBillFundADO> listBillFundReciept = new List<VHisBillFundADO>();

        List<V_HIS_ACCOUNT_BOOK> listRecieptAccountBook = new List<V_HIS_ACCOUNT_BOOK>();
        List<V_HIS_ACCOUNT_BOOK> listInvoiceAccountBook = new List<V_HIS_ACCOUNT_BOOK>();
        List<V_HIS_ACCOUNT_BOOK> ListAccountBookRepay = new List<V_HIS_ACCOUNT_BOOK>();

        V_HIS_CASHIER_ROOM cashierRoom;
        long? treatmentId = null;
        V_HIS_TREATMENT_FEE treatment = null;

        decimal totalPatientPrice = 0;
        decimal totalHienDu = 0;
        decimal totalCanThuThem = 0;
        HIS_BRANCH branch = null;

        Dictionary<string, string> dicPrinter = new Dictionary<string, string>();

        private int positionHandleControl = -1;

        decimal totalReciept = 0;
        decimal totalInvoice = 0;
        bool? isDirectlyBilling = null;

        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        bool isNotLoadWhileChangeControlStateInFirst;
        Inventec.Desktop.Common.Modules.Module currentModule;

        List<HIS_CARD> hisCard = null;
        V_HIS_PATIENT hispatient = null;

        WcfClient cll;
        string nameFile = "";
        string creator = "";

        //IS_DIRECTLY_BILLING
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.TransactionBillTwoInOne.Resources.Lang", typeof(HIS.Desktop.Plugins.TransactionBillTwoInOne.frmTransactionBillTwoInOne).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmTransactionBillTwoInOne.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkNotReciept.Properties.Caption = Inventec.Common.Resource.Get.Value("frmTransactionBillTwoInOne.checkNotReciept.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmTransactionBillTwoInOne.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnRCSave.Caption = Inventec.Common.Resource.Get.Value("frmTransactionBillTwoInOne.bbtnRCSave.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnRCNew.Caption = Inventec.Common.Resource.Get.Value("frmTransactionBillTwoInOne.bbtnRCNew.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnRCConfigPrinter.Caption = Inventec.Common.Resource.Get.Value("frmTransactionBillTwoInOne.bbtnRCConfigPrinter.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnRCSavePrint.Caption = Inventec.Common.Resource.Get.Value("frmTransactionBillTwoInOne.bbtnRCSavePrint.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem1.Caption = Inventec.Common.Resource.Get.Value("frmTransactionBillTwoInOne.barButtonItem1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

                //     this.cboRecieptPayForm.Properties.NullText = Inventec.Common.Resource.Get.Value("frmTransactionBillTwoInOne.cboRecieptPayForm.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboRecieptAccountBook.Properties.NullText = Inventec.Common.Resource.Get.Value("frmTransactionBillTwoInOne.cboRecieptAccountBook.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcgReceiptGroup.Text = Inventec.Common.Resource.Get.Value("frmTransactionBillTwoInOne.layoutControlGroupReciept.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciRecieptAccountBook.Text = Inventec.Common.Resource.Get.Value("frmTransactionBillTwoInOne.layoutRecieptAccountBook.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //      this.lciRecieptPayForm.Text = Inventec.Common.Resource.Get.Value("frmTransactionBillTwoInOne.layoutRecieptPayForm.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciRecieptDescription.Text = Inventec.Common.Resource.Get.Value("frmTransactionBillTwoInOne.layoutRecieptDescription.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciRecieptNumOrder.Text = Inventec.Common.Resource.Get.Value("frmTransactionBillTwoInOne.lciRecieptNumOrder.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciRecieptAmount.Text = Inventec.Common.Resource.Get.Value("frmTransactionBillTwoInOne.layoutRecieptAmount.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciRecieptDiscountPrice.Text = Inventec.Common.Resource.Get.Value("frmTransactionBillTwoInOne.layoutRecieptDiscountPrice.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciRecieptDiscountRatio.Text = Inventec.Common.Resource.Get.Value("frmTransactionBillTwoInOne.layoutRecieptDiscountRatio.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciRecieptReason.Text = Inventec.Common.Resource.Get.Value("frmTransactionBillTwoInOne.layoutRecieptReason.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciNotReciept.Text = Inventec.Common.Resource.Get.Value("frmTransactionBillTwoInOne.lciNotReciept.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkNotInvoice.Properties.Caption = Inventec.Common.Resource.Get.Value("frmTransactionBillTwoInOne.checkNotInvoice.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboPayForm.Properties.NullText = Inventec.Common.Resource.Get.Value("frmTransactionBillTwoInOne.cboInvoicePayForm.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboInvoiceAccountBook.Properties.NullText = Inventec.Common.Resource.Get.Value("frmTransactionBillTwoInOne.cboInvoiceAccountBook.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcgInvoiceGroup.Text = Inventec.Common.Resource.Get.Value("frmTransactionBillTwoInOne.layoutControlGroupInvoice.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciInvoiceAccountBook.Text = Inventec.Common.Resource.Get.Value("frmTransactionBillTwoInOne.layoutInvoiceAccountBook.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                //        this.lciInvoicePayForm.Text = Inventec.Common.Resource.Get.Value("frmTransactionBillTwoInOne.layoutInvoicePayForm.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciInvoiceDescription.Text = Inventec.Common.Resource.Get.Value("frmTransactionBillTwoInOne.layoutInvoiceDescription.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciInvoiceNumOrder.Text = Inventec.Common.Resource.Get.Value("frmTransactionBillTwoInOne.lciInvoiceNumOrder.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciInvoiceDiscountPrice.Text = Inventec.Common.Resource.Get.Value("frmTransactionBillTwoInOne.layoutInvoiceDiscountPrice.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciInvoiceAmount1.Text = Inventec.Common.Resource.Get.Value("frmTransactionBillTwoInOne.layoutInvoiceAmount.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciInvoiceDiscountRatio.Text = Inventec.Common.Resource.Get.Value("frmTransactionBillTwoInOne.layoutInvoiceDiscountRatio.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciInvoiceReason.Text = Inventec.Common.Resource.Get.Value("frmTransactionBillTwoInOne.layoutInvoiceReason.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciNotInvoice.Text = Inventec.Common.Resource.Get.Value("frmTransactionBillTwoInOne.lciNotInvoice.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmTransactionBillTwoInOne.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmTransactionBillTwoInOne.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmTransactionBillTwoInOne.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtSearch.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmTransactionBillTwoInOne.txtFindTreatmentCode.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn_SereServ_ServiceName.Caption = Inventec.Common.Resource.Get.Value("frmTransactionBillTwoInOne.treeListColumn_SereServ_ServiceName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn_SereServ_Amount.Caption = Inventec.Common.Resource.Get.Value("frmTransactionBillTwoInOne.treeListColumn_SereServ_Amount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn_SereServ_Price.Caption = Inventec.Common.Resource.Get.Value("frmTransactionBillTwoInOne.treeListColumn_SereServ_Price.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn_SereServ_VirTotalPrice.Caption = Inventec.Common.Resource.Get.Value("frmTransactionBillTwoInOne.treeListColumn_SereServ_VirTotalPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn_SereServ_VirTotalHeinPrice.Caption = Inventec.Common.Resource.Get.Value("frmTransactionBillTwoInOne.treeListColumn_SereServ_VirTotalHeinPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn_SereServ_RecieptPrice.Caption = Inventec.Common.Resource.Get.Value("frmTransactionBillTwoInOne.treeListColumn_SereServ_RecieptPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn_SereServ_DifferentPrice.Caption = Inventec.Common.Resource.Get.Value("frmTransactionBillTwoInOne.treeListColumn_SereServ_DifferentPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn_SereServ_Discount.Caption = Inventec.Common.Resource.Get.Value("frmTransactionBillTwoInOne.treeListColumn_SereServ_Discount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn_SereServ_Expend.Caption = Inventec.Common.Resource.Get.Value("frmTransactionBillTwoInOne.treeListColumn_SereServ_Expend.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn_SereServ_ServiceCode.Caption = Inventec.Common.Resource.Get.Value("frmTransactionBillTwoInOne.treeListColumn_SereServ_ServiceCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn_SereServ_InvoicePrice.Caption = Inventec.Common.Resource.Get.Value("frmTransactionBillTwoInOne.treeListColumn_SereServ_InvoicePrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSavePrint.Text = Inventec.Common.Resource.Get.Value("frmTransactionBillTwoInOne.btnSavePrint.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_Transaction_Lock.Caption = Inventec.Common.Resource.Get.Value("frmTransactionBillTwoInOne.gridColumn_Transaction_Lock.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_Transaction_TransactionCode.Caption = Inventec.Common.Resource.Get.Value("frmTransactionBillTwoInOne.gridColumn_Transaction_TransactionCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_Transaction_Amount.Caption = Inventec.Common.Resource.Get.Value("frmTransactionBillTwoInOne.gridColumn_Transaction_Amount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_Transaction_PayForm.Caption = Inventec.Common.Resource.Get.Value("frmTransactionBillTwoInOne.gridColumn_Transaction_PayForm.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_Transaction_CashierUsername.Caption = Inventec.Common.Resource.Get.Value("frmTransactionBillTwoInOne.gridColumn_Transaction_CashierUsername.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_Transaction_CashierRoomName.Caption = Inventec.Common.Resource.Get.Value("frmTransactionBillTwoInOne.gridColumn_Transaction_CashierRoomName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_Transaction_NumOrder.Caption = Inventec.Common.Resource.Get.Value("frmTransactionBillTwoInOne.gridColumn_Transaction_NumOrder.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_Transaction_AccountBookCode.Caption = Inventec.Common.Resource.Get.Value("frmTransactionBillTwoInOne.gridColumn_Transaction_AccountBookCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_Transaction_AccountBookName.Caption = Inventec.Common.Resource.Get.Value("frmTransactionBillTwoInOne.gridColumn_Transaction_AccountBookName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_Transaction_CreateTime.Caption = Inventec.Common.Resource.Get.Value("frmTransactionBillTwoInOne.gridColumn_Transaction_CreateTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_Transaction_Creator.Caption = Inventec.Common.Resource.Get.Value("frmTransactionBillTwoInOne.gridColumn_Transaction_Creator.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_Transaction_ModifyTime.Caption = Inventec.Common.Resource.Get.Value("frmTransactionBillTwoInOne.gridColumn_Transaction_ModifyTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_Transaction_Modifier.Caption = Inventec.Common.Resource.Get.Value("frmTransactionBillTwoInOne.gridColumn_Transaction_Modifier.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnConfigPrinter.Text = Inventec.Common.Resource.Get.Value("frmTransactionBillTwoInOne.btnConfigPrinter.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnNew.Text = Inventec.Common.Resource.Get.Value("frmTransactionBillTwoInOne.btnNew.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.ddBtnPrint.Text = Inventec.Common.Resource.Get.Value("frmTransactionBillTwoInOne.ddBtnPrint.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutHienDu.Text = Inventec.Common.Resource.Get.Value("frmTransactionBillTwoInOne.layoutHienDu.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutCanThu.Text = Inventec.Common.Resource.Get.Value("frmTransactionBillTwoInOne.layoutCanThu.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTransactionTime.Text = Inventec.Common.Resource.Get.Value("frmTransactionBillTwoInOne.lciTransactionTime.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        public frmTransactionBillTwoInOne(Inventec.Desktop.Common.Modules.Module module, V_HIS_TREATMENT_FEE data, List<V_HIS_SERE_SERV_5> sereServs, bool? isDirectly)
            : base(module)
        {
            InitializeComponent();
            try
            {
                Base.ResourceLangManager.InitResourceLanguageManager();
                this.currentModule = module;
                if (data != null)
                {
                    this.treatmentId = data.ID;
                    this.treatment = data;
                }
                this.isDirectlyBilling = isDirectly;
                this.inputSereServs = sereServs;
                this.bindingSource1.DataSource = ListBillFund;
                InItSpinFormat();
                creator = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public frmTransactionBillTwoInOne(Inventec.Desktop.Common.Modules.Module module, V_HIS_TREATMENT_FEE data, bool? isDirectly)
            : base(module)
        {
            InitializeComponent();
            try
            {
                Base.ResourceLangManager.InitResourceLanguageManager();
                this.currentModule = module;
                if (data != null)
                {
                    this.treatmentId = data.ID;
                    this.treatment = data;
                }
                this.isDirectlyBilling = isDirectly;
                this.bindingSource1.DataSource = ListBillFund;
                InItSpinFormat();
                creator = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public frmTransactionBillTwoInOne(Inventec.Desktop.Common.Modules.Module module, bool? isDirectly)
            : base(module)
        {
            InitializeComponent();
            try
            {
                this.currentModule = module;
                Base.ResourceLangManager.InitResourceLanguageManager();
                this.isDirectlyBilling = isDirectly;
                this.bindingSource1.DataSource = ListBillFund;
                InItSpinFormat();
                creator = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("timerInitForm_Tick. 1");
                this.timerInitForm.Stop();

                Inventec.Common.Logging.LogSystem.Debug("timerInitForm_Tick. 2");
                this.LoadListSereServ();
                this.LoadAccountBookToLocal();
                Inventec.Common.Logging.LogSystem.Debug("timerInitForm_Tick. 3");
                this.ProcessDataByCheckNot();
                this.ResetControlValue();
                Inventec.Common.Logging.LogSystem.Debug("timerInitForm_Tick. 4");
                this.FillInfoPatient(treatment);
                Inventec.Common.Logging.LogSystem.Debug("timerInitForm_Tick. 5");
                this.CalcuTotalPrice();
                this.ProcessFundForHCM();
                Inventec.Common.Logging.LogSystem.Debug("timerInitForm_Tick. 6");
                this.CalcuHienDu();
                this.CalcuCanThu(true);
                this.LoadConfigPrinter();
                FillDataToTienHoaDon();
                FillDataToTongChiPhi();
                Inventec.Common.Logging.LogSystem.Debug("timerInitForm_Tick. 7");
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmTransactionBillTwoInOne_Load(object sender, EventArgs e)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug("frmTransactionBillTwoInOne_Load. 1");
                WaitingManager.Show();
                this.SetCaptionByLanguageKey();
                HisConfig.LoadConfig();
                UpdateFormatSpin();
                InitControlProperties();
                InitControlState();
                this.InitElectrictBillConfig();
                this.AutoCheckRepaySetDefault();
                this.LoadCashierRoomAndBranch();
                this.SetPrintTypeToMps();
                Inventec.Common.Logging.LogSystem.Debug("frmTransactionBillTwoInOne_Load. 2");
                this.LoadAccountBookRepayToLocal();
                this.LoadDataToComboPayForm();
                this.LoadDataToComboFund();
                this.FillDataToGirdTransaction();
                if (this.isDirectlyBilling.HasValue && HisConfig.IsketChuyenCFG != null && HisConfig.IsketChuyenCFG.Equals("4"))
                    checkIsKC.Checked = !this.isDirectlyBilling.Value;

                Inventec.Common.Logging.LogSystem.Debug("frmTransactionBillTwoInOne_Load. 3");
                this.GeneratePopupMenu();
                if (this.treatment.TREATMENT_CODE != null)
                {
                    this.txtSearch.Text = this.treatment.TREATMENT_CODE;
                    this.txtSearch.SelectionStart = this.txtSearch.Text.Length;
                    this.txtSearch.DeselectAll();

                }

                GetList();

                WaitingManager.Hide();
                this.isInit = false;
                timerInitForm.Interval = 100;
                timerInitForm.Enabled = true;
                timerInitForm.Start();
                loadConfig();
                Inventec.Common.Logging.LogSystem.Debug("frmTransactionBillTwoInOne_Load. 4");
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InitControlProperties()
        {
            try
            {
                navigationFrameBuyerInfo.AllowTransitionAnimation = DevExpress.Utils.DefaultBoolean.False;
                chkBuyerInfo.Checked = true;
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
                checkIsAutoRepay.Checked = HisConfig.IsCheckAutoRepayAsDefault;
                cboRepayAccountBook.Enabled = checkIsAutoRepay.Checked;
                spinRepayNumOrder.Enabled = checkIsAutoRepay.Checked;
                if (HisConfig.IsEditTransactionBillCFG.Equals("1"))
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

        private void UpdateFormatSpin()
        {
            try
            {
                FormatControl(ConfigApplications.NumberSeperator, spinRecieptDiscountPrice);
                FormatControl(ConfigApplications.NumberSeperator, spinInvoiceDiscountPrice);
                FormatControl(ConfigApplications.NumberSeperator, repositoryItemSpinFundAmount);
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

                //spinControl.valu

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

        private void SetDefaultKC()
        {
            try
            {
                lciIsKC.Enabled = true;
                if (HisConfig.IsketChuyenCFG != null && HisConfig.IsketChuyenCFG.Equals("1")
                   || (!HisConfig.IsketChuyenCFG.Equals("2") && !HisConfig.IsketChuyenCFG.Equals("3") && !HisConfig.IsketChuyenCFG.Equals("4")))
                {
                    checkIsKC.CheckState = CheckState.Unchecked;
                }
                else if (HisConfig.IsketChuyenCFG != null && HisConfig.IsketChuyenCFG.Equals("2"))
                {
                    checkIsKC.CheckState = CheckState.Checked;
                }
                else if (HisConfig.IsketChuyenCFG != null && HisConfig.IsketChuyenCFG.Equals("4"))
                {
                    lciIsKC.Enabled = false;
                }
                else if (HisConfig.IsketChuyenCFG != null && HisConfig.IsketChuyenCFG.Equals("3") && this.treatment.IS_PAUSE == 1)
                {
                    checkIsKC.CheckState = CheckState.Checked;
                }
                else
                {
                    checkIsKC.CheckState = CheckState.Unchecked;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToTienHoaDon()
        {
            try
            {
                lblPhaiThuVienPhi.Text = Inventec.Common.Number.Convert.NumberToString(totalReciept, ConfigApplications.NumberSeperator);
                lblPhaiThuDichVu.Text = Inventec.Common.Number.Convert.NumberToString(totalInvoice, ConfigApplications.NumberSeperator);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToTongChiPhi()
        {
            try
            {
                if (this.treatment != null)
                {
                    lblTongTienTamUng.Text = Inventec.Common.Number.Convert.NumberToString(this.treatment.TOTAL_DEPOSIT_AMOUNT ?? 0, ConfigApplications.NumberSeperator);
                    lblTongChiPhiNguoiBenh.Text = Inventec.Common.Number.Convert.NumberToString(this.treatment.TOTAL_PATIENT_PRICE ?? 0, ConfigApplications.NumberSeperator);
                    lblTongTienMienGiam.Text = Inventec.Common.Number.Convert.NumberToString(this.treatment.TOTAL_DISCOUNT ?? 0, ConfigApplications.NumberSeperator);
                    decimal totalReceive = ((this.treatment.TOTAL_DEPOSIT_AMOUNT ?? 0) + (this.treatment.TOTAL_BILL_AMOUNT ?? 0) - (this.treatment.TOTAL_BILL_TRANSFER_AMOUNT ?? 0) - (this.treatment.TOTAL_BILL_FUND ?? 0) - (this.treatment.TOTAL_REPAY_AMOUNT ?? 0)) - (this.treatment.TOTAL_BILL_EXEMPTION ?? 0);

                    decimal totalReceiveMore = (this.treatment.TOTAL_PATIENT_PRICE ?? 0) - totalReceive - (this.treatment.TOTAL_BILL_FUND ?? 0) - (this.treatment.TOTAL_BILL_EXEMPTION ?? 0);
                    if (totalReceiveMore <= 0)
                    {
                        lciTongTinHoanThu.Text = "Tổng tiền phải hoàn";
                        lblTongTienPhaiHoan.Text = Inventec.Common.Number.Convert.NumberToString(-totalReceiveMore, ConfigApplications.NumberSeperator); ;
                    }
                    else
                    {
                        lciTongTinHoanThu.Text = "Tổng tiền thu thêm";
                        lblTongTienPhaiHoan.Text = Inventec.Common.Number.Convert.NumberToString(totalReceiveMore, ConfigApplications.NumberSeperator); ;
                    }
                }

                MOS.Filter.HisTransactionViewFilter filter = new HisTransactionViewFilter();
                filter.TREATMENT_ID = this.treatment.ID;
                var transaction = new BackendAdapter(new CommonParam()).Get<List<V_HIS_TRANSACTION>>("api/HisTransaction/GetView", ApiConsumer.ApiConsumers.MosConsumer, filter, null);
                decimal recieptSum = 0, invoiceSum = 0;
                Inventec.Common.Logging.LogSystem.Debug("recieptAmountAll: " + recieptAmountAll);
                Inventec.Common.Logging.LogSystem.Debug("invoiceAmountAll: " + invoiceAmountAll);
                if (transaction != null && transaction.Count() > 0)
                {
                    Inventec.Common.Logging.LogSystem.Debug("transaction: " + Inventec.Common.Logging.LogUtil.TraceData("", transaction));
                    recieptSum = transaction.Where(o =>
                        (!o.IS_CANCEL.HasValue || o.IS_CANCEL != 1)
                        && o.BILL_TYPE_ID.HasValue && o.BILL_TYPE_ID == 1)
                        .Sum(o => o.AMOUNT) + recieptAmountAll;

                    invoiceSum = transaction.Where(o =>
                        (!o.IS_CANCEL.HasValue || o.IS_CANCEL != 1)
                        && o.BILL_TYPE_ID.HasValue && o.BILL_TYPE_ID == 2)
                        .Sum(o => o.AMOUNT) + invoiceAmountAll;
                }
                else
                {
                    recieptSum = recieptAmountAll;
                    invoiceSum = invoiceAmountAll;
                }
                lblTongTienVienPhi.Text = Inventec.Common.Number.Convert.NumberToString(recieptSum, ConfigApplications.NumberSeperator);
                lblTongTienDichVu.Text = Inventec.Common.Number.Convert.NumberToString(invoiceSum, ConfigApplications.NumberSeperator);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => this.treatment), this.treatment));
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void InItSpinFormat()
        {
            try
            {
                int separate = HIS.Desktop.LocalStorage.ConfigApplication.ConfigApplications.NumberSeperator;
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
                if (this.currentModuleBase != null)
                {
                    this.cashierRoom = BackendDataWorker.Get<V_HIS_CASHIER_ROOM>().FirstOrDefault(o => o.ROOM_ID == currentModuleBase.RoomId && o.ROOM_TYPE_ID == currentModuleBase.RoomTypeId);
                    branch = BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == WorkPlace.GetBranchId());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetPrintTypeToMps()
        {
            try
            {
                if (MPS.PrintConfig.PrintTypes == null || MPS.PrintConfig.PrintTypes.Count == 0)
                {
                    MPS.PrintConfig.PrintTypes = BackendDataWorker.Get<SAR_PRINT_TYPE>();
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
                this.listInvoiceAccountBook = new List<V_HIS_ACCOUNT_BOOK>();
                this.listRecieptAccountBook = new List<V_HIS_ACCOUNT_BOOK>();

                HisAccountBookViewFilter acFilter = new HisAccountBookViewFilter();
                acFilter.CASHIER_ROOM_ID = this.cashierRoom.ID;//Kiểm tra sổ còn hay k
                acFilter.LOGINNAME = loginName;//Kiểm tra sổ còn hay k
                acFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                acFilter.FOR_BILL = true;
                acFilter.IS_OUT_OF_BILL = false;
                acFilter.ORDER_DIRECTION = "DESC";
                acFilter.ORDER_FIELD = "ID";
                List<V_HIS_ACCOUNT_BOOK> listUserAcountBoook = await new BackendAdapter(new CommonParam()).GetAsync<List<V_HIS_ACCOUNT_BOOK>>("api/HisAccountBook/GetView", ApiConsumers.MosConsumer, acFilter, null);
                if (listUserAcountBoook != null && listUserAcountBoook.Count > 0)
                {
                    foreach (var item in listUserAcountBoook)
                    {
                        if ((item.FROM_NUM_ORDER + item.TOTAL - 1) <= item.CURRENT_NUM_ORDER)
                        {
                            continue;
                        }
                        if (item.BILL_TYPE_ID == 2)
                        {
                            listInvoiceAccountBook.Add(item);
                        }
                        else
                        {
                            listRecieptAccountBook.Add(item);
                        }
                    }
                }

                if (listInvoiceAccountBook != null && listInvoiceAccountBook.Count > 0)
                {
                    if (WorkPlace.WorkInfoSDO != null && WorkPlace.WorkInfoSDO.WorkingShiftId.HasValue)
                    {
                        listInvoiceAccountBook = listInvoiceAccountBook.Where(o => !o.WORKING_SHIFT_ID.HasValue || o.WORKING_SHIFT_ID == WorkPlace.WorkInfoSDO.WorkingShiftId.Value).ToList();
                    }
                    else
                    {
                        listInvoiceAccountBook = listInvoiceAccountBook.Where(o => !o.WORKING_SHIFT_ID.HasValue).ToList();
                    }
                }

                if (listRecieptAccountBook != null && listRecieptAccountBook.Count > 0)
                {
                    if (WorkPlace.WorkInfoSDO != null && WorkPlace.WorkInfoSDO.WorkingShiftId.HasValue)
                    {
                        listRecieptAccountBook = listRecieptAccountBook.Where(o => !o.WORKING_SHIFT_ID.HasValue || o.WORKING_SHIFT_ID == WorkPlace.WorkInfoSDO.WorkingShiftId.Value).ToList();
                    }
                    else
                    {
                        listRecieptAccountBook = listRecieptAccountBook.Where(o => !o.WORKING_SHIFT_ID.HasValue).ToList();
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

        private void LoadDataToComboAccountBook()
        {
            try
            {
                cboRecieptAccountBook.Properties.DataSource = listRecieptAccountBook;
                cboRecieptAccountBook.Properties.DisplayMember = "ACCOUNT_BOOK_NAME";
                cboRecieptAccountBook.Properties.ValueMember = "ID";
                cboRecieptAccountBook.Properties.ForceInitialize();
                cboRecieptAccountBook.Properties.Columns.Clear();
                cboRecieptAccountBook.Properties.Columns.Add(new LookUpColumnInfo("ACCOUNT_BOOK_CODE", "", 50));
                cboRecieptAccountBook.Properties.Columns.Add(new LookUpColumnInfo("ACCOUNT_BOOK_NAME", "", 200));
                cboRecieptAccountBook.Properties.ShowHeader = false;
                cboRecieptAccountBook.Properties.ImmediatePopup = true;
                cboRecieptAccountBook.Properties.DropDownRows = 10;
                cboRecieptAccountBook.Properties.PopupWidth = 250;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

            try
            {
                cboInvoiceAccountBook.Properties.DataSource = listInvoiceAccountBook;
                cboInvoiceAccountBook.Properties.DisplayMember = "ACCOUNT_BOOK_NAME";
                cboInvoiceAccountBook.Properties.ValueMember = "ID";
                cboInvoiceAccountBook.Properties.ForceInitialize();
                cboInvoiceAccountBook.Properties.Columns.Clear();
                cboInvoiceAccountBook.Properties.Columns.Add(new LookUpColumnInfo("ACCOUNT_BOOK_CODE", "", 50));
                cboInvoiceAccountBook.Properties.Columns.Add(new LookUpColumnInfo("ACCOUNT_BOOK_NAME", "", 200));
                cboInvoiceAccountBook.Properties.ShowHeader = false;
                cboInvoiceAccountBook.Properties.ImmediatePopup = true;
                cboInvoiceAccountBook.Properties.DropDownRows = 10;
                cboInvoiceAccountBook.Properties.PopupWidth = 250;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task LoadDataToComboPayForm()
        {
            List<HIS_PAY_FORM> lData = null;
            if (BackendDataWorker.IsExistsKey<HIS_PAY_FORM>())
            {
                lData = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_PAY_FORM>();
            }
            else
            {
                CommonParam paramCommon = new CommonParam();
                dynamic filter = new System.Dynamic.ExpandoObject();
                lData = await new Inventec.Common.Adapter.BackendAdapter(paramCommon).GetAsync<List<MOS.EFMODEL.DataModels.HIS_PAY_FORM>>("api/HisPayForm/Get", ApiConsumers.MosConsumer, filter, paramCommon);

                if (lData != null) BackendDataWorker.UpdateToRam(typeof(MOS.EFMODEL.DataModels.HIS_PAY_FORM), lData, long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss")));
            }
            InitComboPayForm(cboPayForm, lData.Where(o => o.IS_ACTIVE == 1));
            //            InitComboPayForm(cboInvoicePayForm, lData);
            SetDefaultPayForm();
            //try
            //{               
            //    //cboRecieptPayForm.Properties.DataSource = lData;
            //    //cboRecieptPayForm.Properties.DisplayMember = "PAY_FORM_NAME";
            //    //cboRecieptPayForm.Properties.ValueMember = "ID";
            //    //cboRecieptPayForm.Properties.ForceInitialize();
            //    //cboRecieptPayForm.Properties.Columns.Clear();
            //    //cboRecieptPayForm.Properties.Columns.Add(new LookUpColumnInfo("PAY_FORM_CODE", "", 50));
            //    //cboRecieptPayForm.Properties.Columns.Add(new LookUpColumnInfo("PAY_FORM_NAME", "", 150));
            //    //cboRecieptPayForm.Properties.ShowHeader = false;
            //    //cboRecieptPayForm.Properties.ImmediatePopup = true;
            //    //cboRecieptPayForm.Properties.DropDownRows = 10;
            //    //cboRecieptPayForm.Properties.PopupWidth = 200;
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Error(ex);
            //}

            //try
            //{                
            //    //cboInvoicePayForm.Properties.DataSource = lData;
            //    //cboInvoicePayForm.Properties.DisplayMember = "PAY_FORM_NAME";
            //    //cboInvoicePayForm.Properties.ValueMember = "ID";
            //    //cboInvoicePayForm.Properties.ForceInitialize();
            //    //cboInvoicePayForm.Properties.Columns.Clear();
            //    //cboInvoicePayForm.Properties.Columns.Add(new LookUpColumnInfo("PAY_FORM_CODE", "", 50));
            //    //cboInvoicePayForm.Properties.Columns.Add(new LookUpColumnInfo("PAY_FORM_NAME", "", 150));
            //    //cboInvoicePayForm.Properties.ShowHeader = false;
            //    //cboInvoicePayForm.Properties.ImmediatePopup = true;
            //    //cboInvoicePayForm.Properties.DropDownRows = 10;
            //    //cboInvoicePayForm.Properties.PopupWidth = 200;
            //}
            //catch (Exception ex)
            //{
            //    Inventec.Common.Logging.LogSystem.Error(ex);
            //}
        }

        private async Task LoadAccountBookRepayToLocal()
        {
            try
            {
                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                this.ListAccountBookRepay = new List<V_HIS_ACCOUNT_BOOK>();

                //Sửa lại đoạn code này
                //Api bổ sung filter chứ không get nhiều api
                //TODO               
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

        private void InitComboAccountBookRepay(List<V_HIS_ACCOUNT_BOOK> db)
        {
            try
            {
                List<ColumnInfo> columnInfos = new List<ColumnInfo>();
                columnInfos.Add(new ColumnInfo("ACCOUNT_BOOK_CODE", "", 100, 1));
                columnInfos.Add(new ColumnInfo("ACCOUNT_BOOK_NAME", "", 250, 2));
                ControlEditorADO controlEditorADO = new ControlEditorADO("ACCOUNT_BOOK_NAME", "ID", columnInfos, false, 350);
                ControlEditorLoader.Load(this.cboRepayAccountBook, db, controlEditorADO);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void SetDefaultAccountBookRepay()
        {
            try
            {
                if (!checkIsAutoRepay.Checked)
                {
                    cboRepayAccountBook.EditValue = null;
                    spinRepayNumOrder.EditValue = null;
                    return;
                }
                cboRepayAccountBook.EditValue = null;
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
                    cboRepayAccountBook.EditValue = accountBook.ID;
                    //SetDataToDicNumOrderInAccountBook(accountBook);
                }
                else
                {
                    spinRepayNumOrder.EditValue = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        decimal recieptAmountAll = 0;
        decimal invoiceAmountAll = 0;

        private void LoadListSereServ()
        {
            try
            {
                dicSereServBill = new Dictionary<long, List<HIS_SERE_SERV_BILL>>();
                listSereServADO = new List<VHisSereServADO>();
                ListSereServ = new List<V_HIS_SERE_SERV_5>();
                listRecieptData = new List<VHisSereServADO>();
                listInvoiceData = new List<VHisSereServADO>();
                if (this.treatmentId.HasValue)
                {
                    HisSereServBillFilter ssBillFilter = new HisSereServBillFilter();
                    ssBillFilter.TDL_TREATMENT_ID = this.treatmentId.Value;
                    ssBillFilter.IS_NOT_CANCEL = true;
                    var listSSBill = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV_BILL>>("api/HisSereServBill/Get", ApiConsumers.MosConsumer, ssBillFilter, null);
                    if (listSSBill != null && listSSBill.Count > 0)
                    {
                        foreach (var item in listSSBill)
                        {
                            if (item.IS_CANCEL == HisConfig.IS_TRUE)
                                continue;
                            if (!dicSereServBill.ContainsKey(item.SERE_SERV_ID))
                                dicSereServBill[item.SERE_SERV_ID] = new List<HIS_SERE_SERV_BILL>();
                            dicSereServBill[item.SERE_SERV_ID].Add(item);
                        }
                    }

                    if (inputSereServs == null || inputSereServs.Count <= 0)
                    {
                        HisSereServView5Filter ssFilter = new HisSereServView5Filter();
                        ssFilter.TDL_TREATMENT_ID = this.treatmentId;
                        inputSereServs = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV_5>>("api/HisSereServ/GetView5", ApiConsumers.MosConsumer, ssFilter, null);
                    }
                    if (inputSereServs != null && inputSereServs.Count > 0)
                    {
                        // bỏ những dịch vụ đã chốt nợ
                        MOS.Filter.HisSereServDebtFilter sereServDebtFilter = new HisSereServDebtFilter();
                        sereServDebtFilter.TDL_TREATMENT_ID = this.treatmentId.Value;
                        var sereServDebtList = new BackendAdapter(new CommonParam()).Get<List<HIS_SERE_SERV_DEBT>>("api/HisSereServDebt/Get", ApiConsumer.ApiConsumers.MosConsumer, sereServDebtFilter, null);

                        if (sereServDebtList != null && sereServDebtList.Count > 0)
                        {
                            sereServDebtList = sereServDebtList.Where(o => o.IS_CANCEL != 1).ToList();

                            inputSereServs = sereServDebtList != null && sereServDebtList.Count > 0
                                ? inputSereServs.Where(o => !sereServDebtList.Select(p => p.SERE_SERV_ID).Contains(o.ID)).ToList()
                                : inputSereServs;
                        }

                        var lstPaty = BackendDataWorker.Get<HIS_PATIENT_TYPE>();
                        lstPaty = lstPaty != null ? lstPaty.ToList() : null;

                        BillTwoBookPriceProcessor priceProcessor = new BillTwoBookPriceProcessor(HisConfig.PatientTypeId__BHYT, HisConfig.PATIENT_TYPE_ID__IS_FEE, HisConfig.PATIENT_TYPE_ID__SERVICE, lstPaty);

                        foreach (var item in inputSereServs)
                        {
                            if (item.IS_NO_PAY == HisConfig.IS_TRUE || item.VIR_TOTAL_PATIENT_PRICE <= 0 || item.IS_NO_EXECUTE == HisConfig.IS_TRUE)
                                continue;
                            VHisSereServADO ado = new VHisSereServADO(item);

                            if (HisConfig.BILL_TWO_BOOK__OPTION == (int)HisConfig.BILL_OPTION.HCM_115)
                            {
                                // Nếu không có đối tượng phụ thu (ĐTPT) và đối tượng thanh toán(ĐTTT) là BHYT và VP -> Cho vào hóa đơn thường.
                                // Nếu không có ĐTPT và ĐTTT được tích chọn Không vào hóa đơn dịch vụ (IS_NOT_SERVICE_BILL = 1) -> Cho vào hóa đơn thường.
                                // Nếu ĐTTT khác BHYT và VP và ĐTTT không được tích chọn Không vào hóa đơn dịch vụ (IS_NOT_SERVICE_BILL <> 1)-> Cho vào hóa đơn dịch vụ.
                                // Nếu ĐTTT là VP và loại dịch vụ là Giường -> Cho vào hóa đơn dịch vụ.
                                // Nếu ĐTTT là VP và có ĐTPT -> Tiền viện phí cho vào hóa đơn thường, Tiền chênh lệch cho vào hóa đơn dịch vụ.
                                // Nếu ĐTTT là BHYT và (có ĐTPT hoặc có trần) -> Tiền BHYT vào hóa đơn thường, Tiền chênh lệch cho vào hóa đơn dịch vụ.
                                // Nếu ĐTTT được tích chọn Không vào hóa đơn dịch vụ (IS_NOT_SERVICE_BILL = 1) và có ĐTPT -> Tiền ĐTTT cho vào hóa đơn thường, Tiền chênh lệch cho vào hóa đơn dịch vụ.

                                decimal recieptAmount = 0;
                                decimal invoiceAmount = 0;

                                priceProcessor.Hcm115Calculator(item, ref recieptAmount, ref invoiceAmount);

                                if (recieptAmount > 0) ado.RecieptPrice = recieptAmount;
                                if (invoiceAmount > 0) ado.InvoicePrice = invoiceAmount;

                                if (dicSereServBill.ContainsKey(item.ID))
                                {
                                    var hisSSBills = dicSereServBill[item.ID];
                                    if (hisSSBills.Exists(e => e.TDL_BILL_TYPE_ID == 2))
                                    {
                                        ado.InvoicePrice = null;
                                        ado.IsInvoiced = true;
                                    }
                                    if (hisSSBills.Exists(e => e.TDL_BILL_TYPE_ID == null || e.TDL_BILL_TYPE_ID == 1))
                                    {
                                        ado.RecieptPrice = null;
                                        ado.IsReciepted = true;
                                    }
                                }
                            }
                            else if (HisConfig.BILL_TWO_BOOK__OPTION == (int)HisConfig.BILL_OPTION.QBH_CUBA)
                            {
                                //1. Dịch vụ có ĐTTT khác BHYT và Viện Phí => vào hóa đơn dịch vụ
                                //2. Dịch vụ có ĐTTT Viện phí và không có ĐT Phụ thu => vào hóa đơn viện phí
                                //3. Dịch vu có ĐTTT viện phí và có ĐT phụ thu => giá viện phí vào hóa đơn viện phí. Giá chênh lệch phụ thu - viện phí vào hóa đơn dịch vụ
                                //4. Dịch vụ có ĐTTT BHYT và có ĐT Phụ thu => giá BN cùng chi trả vào hóa đơn viện phí. giá Chênh lêch BN tự trả vào hóa đơn dịch vụ
                                //5. Dịch vụ có ĐTTT BHYT và không có ĐTT phụ thu:
                                //    + Trường hợp khám, giường có trần => giá BN cùng chi trả vào hóa đơn viện phí. giá Chênh lêch BN tự trả vào hóa đơn dịch vụ
                                //    + Còn lại vào hóa đơn viện phí

                                decimal recieptAmount = 0;
                                decimal invoiceAmount = 0;

                                priceProcessor.QbhCubaCalcualator(item, ref recieptAmount, ref invoiceAmount);

                                if (recieptAmount > 0)
                                    ado.RecieptPrice = recieptAmount;
                                if (invoiceAmount > 0)
                                {
                                    ado.InvoicePrice = invoiceAmount;
                                }


                                if (dicSereServBill.ContainsKey(item.ID))
                                {
                                    var hisSSBills = dicSereServBill[item.ID];
                                    if (hisSSBills.Exists(e => e.TDL_BILL_TYPE_ID == 2))
                                    {
                                        ado.InvoicePrice = null;
                                        ado.IsInvoiced = true;
                                    }
                                    if (hisSSBills.Exists(e => e.TDL_BILL_TYPE_ID == null || e.TDL_BILL_TYPE_ID == 1))
                                    {
                                        ado.RecieptPrice = null;
                                        ado.IsReciepted = true;
                                    }
                                }
                            }
                            else
                            {
                                //Nghiep vu thanh toan hai so cua BV Trung Uong Can Tho
                                //1. PATIENT_TYPE_ID or PRIMARY_PATIENT_TYPE_ID là dịch vụ => vào hóa đơn dv
                                //2. Còn lại => vào hóa đơn vp

                                decimal recieptAmount = 0;
                                decimal invoiceAmount = 0;

                                priceProcessor.CtoTWCalcualator(item, ref recieptAmount, ref invoiceAmount);

                                if (recieptAmount > 0) ado.RecieptPrice = recieptAmount;
                                if (invoiceAmount > 0) ado.InvoicePrice = invoiceAmount;

                                if (dicSereServBill.ContainsKey(item.ID))
                                {
                                    if (item.PRIMARY_PATIENT_TYPE_ID.HasValue
                                && item.PRIMARY_PATIENT_TYPE_ID.Value == HisConfig.PATIENT_TYPE_ID__SERVICE)
                                    {
                                        ado.InvoicePrice = null;
                                        ado.IsInvoiced = true;
                                    }
                                    else if (item.PATIENT_TYPE_ID == HisConfig.PATIENT_TYPE_ID__SERVICE)
                                    {
                                        ado.InvoicePrice = null;
                                        ado.IsInvoiced = true;
                                    }
                                    else
                                    {
                                        var hisSSBills = dicSereServBill[item.ID];
                                        if (hisSSBills.Exists(e => e.TDL_BILL_TYPE_ID == 2))
                                        {
                                            ado.InvoicePrice = null;
                                            ado.IsInvoiced = true;
                                        }
                                        if (hisSSBills.Exists(e => e.TDL_BILL_TYPE_ID == null || e.TDL_BILL_TYPE_ID == 1))
                                        {
                                            ado.RecieptPrice = null;
                                            ado.IsReciepted = true;
                                        }
                                    }
                                }
                            }
                            listSereServADO.Add(ado);
                            if (ado.RecieptPrice > 0 && (!ado.IsReciepted))
                            {
                                if (!(((HisConfig.MustFinishTreatmentForBill == "1" && item.PATIENT_TYPE_ID == HisConfig.PatientTypeId__BHYT)
                                    || HisConfig.MustFinishTreatmentForBill == "2")
                                    && this.treatment.IS_PAUSE != 1))
                                {
                                    listRecieptData.Add(ado);
                                }
                            }
                            if (ado.InvoicePrice > 0 && (!ado.IsInvoiced))
                            {
                                if (!(((HisConfig.MustFinishTreatmentForBill == "1" && item.PATIENT_TYPE_ID == HisConfig.PatientTypeId__BHYT)
                                    || HisConfig.MustFinishTreatmentForBill == "2")
                                    && this.treatment.IS_PAUSE != 1))
                                {
                                    listInvoiceData.Add(ado);
                                }
                            }
                            ListSereServ.Add(item);
                        }
                    }
                }
                recieptAmountAll = listSereServADO.Sum(o => o.RecieptPrice ?? 0);
                invoiceAmountAll = listSereServADO.Sum(o => o.InvoicePrice ?? 0); ;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void FillDataToTreeSereServ(List<VHisSereServADO> listData)
        {
            try
            {
                List<VHisSereServADO> listDataSource = new List<VHisSereServADO>();
                if (listData != null && listData.Count > 0)
                {
                    var listRoot = listData.GroupBy(o => o.PATIENT_TYPE_ID).ToList();
                    foreach (var rootPaty in listRoot)
                    {
                        var listByPaty = rootPaty.ToList<VHisSereServADO>();
                        VHisSereServADO ssRootPaty = new VHisSereServADO();
                        ssRootPaty.CONCRETE_ID__IN_SETY = listByPaty.First().PATIENT_TYPE_ID + "";
                        ssRootPaty.TDL_SERVICE_NAME = listByPaty.First().PATIENT_TYPE_NAME;
                        ssRootPaty.PATIENT_TYPE_ID = listByPaty.First().PATIENT_TYPE_ID;
                        listDataSource.Add(ssRootPaty);
                        var listRootSety = listByPaty.GroupBy(g => g.TDL_SERVICE_TYPE_ID).ToList();
                        foreach (var rootSety in listRootSety)
                        {
                            var listBySety = rootSety.ToList<VHisSereServADO>();
                            VHisSereServADO ssRootSety = new VHisSereServADO();
                            ssRootSety.CONCRETE_ID__IN_SETY = ssRootPaty.CONCRETE_ID__IN_SETY + "_" + listBySety.First().TDL_SERVICE_TYPE_ID;
                            ssRootSety.PARENT_ID__IN_SETY = ssRootPaty.CONCRETE_ID__IN_SETY;
                            ssRootSety.PATIENT_TYPE_ID = ssRootPaty.PATIENT_TYPE_ID;
                            ssRootSety.TDL_SERVICE_NAME = listBySety.First().SERVICE_TYPE_NAME;
                            listDataSource.Add(ssRootSety);
                            foreach (var item in listBySety)
                            {
                                item.CONCRETE_ID__IN_SETY = ssRootSety.CONCRETE_ID__IN_SETY + "_" + item.ID;
                                item.PARENT_ID__IN_SETY = ssRootSety.CONCRETE_ID__IN_SETY;
                                item.IsLeaf = true;
                                listDataSource.Add(item);
                            }
                        }
                    }
                }
                listDataSource = listDataSource.OrderBy(o => o.PARENT_ID__IN_SETY).ThenByDescending(o => o.TDL_SERVICE_CODE).ToList();
                records = new BindingList<VHisSereServADO>(listDataSource);
                treeListSereServ.DataSource = records;
                treeListSereServ.ExpandAll();
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
                    HisTransactionViewFilter tranFilter = new HisTransactionViewFilter();
                    tranFilter.TREATMENT_ID = this.treatmentId;
                    tranFilter.TRANSACTION_TYPE_IDs = new List<long>() { IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TU };
                    listTransaction = await new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).GetAsync<List<V_HIS_TRANSACTION>>("api/HisTransaction/GetView", ApiConsumers.MosConsumer, tranFilter, null);
                    gridControlTransaction.BeginUpdate();
                    gridControlTransaction.DataSource = listTransaction;
                    gridControlTransaction.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessDataByCheckNot()
        {
            try
            {
                Dictionary<long, VHisSereServADO> dicAdo = new Dictionary<long, VHisSereServADO>();
                listRecieptData = new List<VHisSereServADO>();
                listInvoiceData = new List<VHisSereServADO>();
                foreach (var item in listSereServADO)
                {
                    dicAdo[item.ID] = item;
                    if ((!checkNotReciept.Checked) && item.RecieptPrice > 0 && !item.IsReciepted)
                    {
                        if (!(((HisConfig.MustFinishTreatmentForBill == "1" && item.PATIENT_TYPE_ID == HisConfig.PatientTypeId__BHYT)
                            || HisConfig.MustFinishTreatmentForBill == "2") && this.treatment.IS_PAUSE != 1))
                        {
                            listRecieptData.Add(item);
                        }
                    }
                    if ((!checkNotInvoice.Checked) && item.InvoicePrice > 0 && !item.IsInvoiced)
                    {
                        if (!(((HisConfig.MustFinishTreatmentForBill == "1" && item.PATIENT_TYPE_ID == HisConfig.PatientTypeId__BHYT)
                            || HisConfig.MustFinishTreatmentForBill == "2") && this.treatment.IS_PAUSE != 1))
                        {
                            listInvoiceData.Add(item);
                        }
                    }
                }
                FillDataToTreeSereServ(dicAdo.Select(s => s.Value).ToList());
                CheckAllNode();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CalcuTotalPrice()
        {
            try
            {
                totalPatientPrice = 0;
                decimal totalInvoice = 0;
                decimal totalReciept = 0;
                if (!checkNotReciept.Checked && listRecieptData != null && listRecieptData.Count > 0)
                {
                    totalReciept = listRecieptData.Sum(o => (o.RecieptPrice ?? 0));
                }
                if (!checkNotInvoice.Checked && listInvoiceData != null && listInvoiceData.Count > 0)
                {
                    totalInvoice = listInvoiceData.Sum(o => o.InvoicePrice ?? 0);
                }
                totalPatientPrice = totalInvoice + totalReciept;
                //spinRecieptAmount.Value = totalReciept;
                lblRecieptAmount.Text = Inventec.Common.Number.Convert.NumberToString(totalReciept, ConfigApplications.NumberSeperator);
                //spinInvoiceAmount.Value = totalInvoice;
                lblInvoiceAmount.Text = Inventec.Common.Number.Convert.NumberToString(totalInvoice, ConfigApplications.NumberSeperator);
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
                listBillFundReciept = new List<VHisBillFundADO>();
                resultInvoiceBill = null;
                resultRecieptBill = null;
                totalPatientPrice = 0;
                totalHienDu = 0;
                txtRecieptDescription.Text = "";
                txtInvoiceDescription.Text = "";
                spinRecieptDiscountPrice.Value = 0;
                spinInvoiceDiscountPrice.Value = 0;
                spinRecieptDiscountRatio.Value = 0;
                spinInvoiceDiscountRatio.Value = 0;

                dtTransactionTime.DateTime = DateTime.Now;

                txtRecieptReason.Text = "";
                txtInvoiceReason.Text = "";
                spinRecieptNumOrder.EditValue = null;
                lblSoBlvp.Text = "";
                lblSoBlvp.Text = "";
                spinInvoiceNumOrder.EditValue = null;
                lblSoBlvp.Text = "";
                lblRecieptAmount.Text = "";
                lblInvoiceAmount.Text = "";
                checkNotInvoice.Checked = false;
                checkNotReciept.Checked = false;
                //SetDefaultAccountBook();
                //SetDefaultPayForm();
                SetDefaultKC();
                btnNew.Enabled = true;
                btnSave.Enabled = true;
                btnSavePrint.Enabled = true;
                ddBtnPrint.Enabled = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefaultPayForm()
        {
            try
            {
                string code = String.IsNullOrEmpty(ConfigApplicationWorker.Get<string>(HFS_KEY__PAY_FORM_CODE)) ? GlobalVariables.HIS_PAY_FORM_CODE__CONSTANT : ConfigApplicationWorker.Get<string>(HFS_KEY__PAY_FORM_CODE);
                var data = BackendDataWorker.Get<HIS_PAY_FORM>().FirstOrDefault(o => o.PAY_FORM_CODE == code);
                if (data != null)
                {
                    cboPayForm.EditValue = data.ID;
                }
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
                cboRecieptAccountBook.EditValue = null;
                cboInvoiceAccountBook.EditValue = null;
                if (listRecieptData == null || listRecieptData.Count <= 0)
                {
                    checkNotReciept.Checked = true;
                }
                if (listRecieptAccountBook != null && listRecieptAccountBook.Count > 0)
                {
                    V_HIS_ACCOUNT_BOOK data = null;
                    //chọn mặc định sổ nếu có sổ tương ứng
                    if (GlobalVariables.DefaultAccountBookBillTwoInOne_VP != null && GlobalVariables.DefaultAccountBookBillTwoInOne_VP.Count > 0)
                    {
                        var lstBook = listRecieptAccountBook.Where(o => GlobalVariables.DefaultAccountBookBillTwoInOne_VP.Select(s => s.ID).Contains(o.ID)).ToList();
                        if (lstBook != null && lstBook.Count > 0)
                        {
                            data = lstBook.Last();
                        }
                    }

                    if (data != null)
                        cboRecieptAccountBook.EditValue = data.ID;
                }

                if (listInvoiceData == null || listInvoiceData.Count <= 0)
                {
                    checkNotInvoice.Checked = true;
                }
                if (listInvoiceAccountBook != null && listInvoiceAccountBook.Count > 0)
                {
                    V_HIS_ACCOUNT_BOOK data = null;
                    //chọn mặc định sổ nếu có sổ tương ứng
                    if (GlobalVariables.DefaultAccountBookBillTwoInOne_DV != null && GlobalVariables.DefaultAccountBookBillTwoInOne_DV.Count > 0)
                    {
                        var lstBook = listInvoiceAccountBook.Where(o => GlobalVariables.DefaultAccountBookBillTwoInOne_DV.Select(s => s.ID).Contains(o.ID)).ToList();
                        if (lstBook != null && lstBook.Count > 0)
                        {
                            data = lstBook.Last();
                        }
                    }

                    if (data != null)
                        cboInvoiceAccountBook.EditValue = data.ID;
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

        private void ProcessFundForHCM()
        {
            try
            {
                listBillFundReciept = new List<VHisBillFundADO>();
                if (this.treatment == null) return;
                if (listRecieptData == null || listRecieptData.Count <= 0)
                {
                    return;
                }
                //HIS_FUND fundHCM = BackendDataWorker.Get<HIS_FUND>().FirstOrDefault(o => o.ID == HisConfig.HisFundId__Hcm);
                //if (fundHCM == null)
                //    return;

                //List<HIS_PATIENT_TYPE_ALTER> districtPatientTypeAlters = new BackendAdapter(new CommonParam()).Get<List<HIS_PATIENT_TYPE_ALTER>>("api/HisPatientTypeAlter/GetDistinct", ApiConsumers.MosConsumer, this.treatment.ID, null);
                //districtPatientTypeAlters = districtPatientTypeAlters != null ? districtPatientTypeAlters.Where(o => o.PATIENT_TYPE_ID == HisConfig.PatientTypeId__BHYT).ToList() : null;
                //if (districtPatientTypeAlters == null || districtPatientTypeAlters.Count <= 0)
                //{
                //    return;
                //}

                //List<long> vcnAcceptServiceIds = new List<long>();
                //PoorFundPriceCalculator calculator = new PoorFundPriceCalculator(branch.HEIN_PROVINCE_CODE, HisConfig.VCN_ACCEPT_SERVICE_IDS, HisConfig.PatientTypeId__BHYT);
                //foreach (string t in HisConfig.HcmPoorFund__Vcn)
                //{
                //    string[] tmp = t.Split(':');
                //    if (tmp != null && tmp.Length >= 2)
                //    {
                //        V_HIS_SERVICE service = BackendDataWorker.Get<V_HIS_SERVICE>().Where(o => o.SERVICE_TYPE_CODE == tmp[0] && o.SERVICE_CODE == tmp[1]).FirstOrDefault();
                //        if (service != null)
                //        {
                //            vcnAcceptServiceIds.Add(service.ID);
                //        }
                //    }
                //}

                //List<HIS_SERE_SERV> listRecieptSereServ = new List<HIS_SERE_SERV>();
                //if (listRecieptData != null && listRecieptData.Count > 0)
                //{
                //    AutoMapper.Mapper.CreateMap<VHisSereServADO, HIS_SERE_SERV>();
                //    listRecieptSereServ = AutoMapper.Mapper.Map<List<HIS_SERE_SERV>>(listRecieptData);
                //}

                //List<string> heinCards = new List<string>();
                //decimal totalHcmPrice = 0;
                //foreach (var patyAlter in districtPatientTypeAlters)
                //{
                //    if (heinCards.Contains(patyAlter.HEIN_CARD_NUMBER))
                //        continue;
                //    if (!PoorFundPriceCalculator.IsPoorMan(patyAlter.HEIN_CARD_NUMBER, patyAlter.HNCODE))
                //        continue;
                //    var lstSS = listRecieptSereServ.Where(o => o.HEIN_CARD_NUMBER == patyAlter.HEIN_CARD_NUMBER).ToList();
                //    if (lstSS != null && lstSS.Count > 0)
                //    {
                //        decimal? amount = calculator.GetPaidAmount(lstSS, patyAlter.HNCODE, patyAlter.HEIN_CARD_NUMBER);
                //        totalHcmPrice += (amount ?? 0);
                //    }
                //    heinCards.Add(patyAlter.HEIN_CARD_NUMBER);
                //}

                //if (totalHcmPrice > 0 && fundHCM != null)
                //{
                //    VHisBillFundADO ado = new VHisBillFundADO();
                //    ado.AMOUNT = totalHcmPrice;
                //    ado.IsNotEdit = true;
                //    ado.FUND_CODE = fundHCM.FUND_CODE;
                //    ado.FUND_NAME = fundHCM.FUND_NAME;
                //    ado.FUND_ID = fundHCM.ID;
                //    listBillFundReciept.Add(ado);
                //}

                bindingSource1.DataSource = listBillFundReciept;
                gridControlFund.BeginUpdate();
                gridControlFund.DataSource = bindingSource1;
                gridControlFund.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CalcuHienDu()
        {
            try
            {
                if (this.treatment != null)
                {
                    totalHienDu = (treatment.TOTAL_DEPOSIT_AMOUNT ?? 0) - ((treatment.TOTAL_REPAY_AMOUNT ?? 0) + (treatment.TOTAL_BILL_TRANSFER_AMOUNT ?? 0));
                    lblHienDu.Text = Inventec.Common.Number.Convert.NumberToString(totalHienDu, ConfigApplications.NumberSeperator);
                }
                else if (this.treatmentId.HasValue)
                {
                    HisTreatmentFeeViewFilter feeFilter = new HisTreatmentFeeViewFilter();
                    feeFilter.ID = this.treatmentId.Value;
                    var treatmentFees = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_TREATMENT_FEE>>("api/HisTreatment/GetFeeView", ApiConsumers.MosConsumer, feeFilter, null);
                    if (treatmentFees == null || treatmentFees.Count == 0)
                    {
                        Inventec.Common.Logging.LogSystem.Info("Khong lay duoc treatmentFee theo TreatmentId: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => treatmentId), treatmentId));
                        return;
                    }
                    var treatmentFee = treatmentFees.First();
                    totalHienDu = (treatmentFee.TOTAL_DEPOSIT_AMOUNT ?? 0) - ((treatmentFee.TOTAL_REPAY_AMOUNT ?? 0) + (treatmentFee.TOTAL_BILL_TRANSFER_AMOUNT ?? 0));
                    lblHienDu.Text = Inventec.Common.Number.Convert.NumberToString(totalHienDu, ConfigApplications.NumberSeperator);
                    totalCanThuThem = (treatmentFee.TOTAL_PATIENT_PRICE ?? 0) - (((treatmentFee.TOTAL_DEPOSIT_AMOUNT ?? 0) + (treatmentFee.TOTAL_BILL_AMOUNT ?? 0) - (treatmentFee.TOTAL_BILL_TRANSFER_AMOUNT ?? 0) - (treatmentFee.TOTAL_BILL_FUND ?? 0) - (treatmentFee.TOTAL_REPAY_AMOUNT ?? 0)) - (treatmentFee.TOTAL_BILL_EXEMPTION ?? 0)) - (treatmentFee.TOTAL_BILL_FUND ?? 0) - (treatmentFee.TOTAL_BILL_EXEMPTION ?? 0);

                }
                //if (resultInvoiceBill != null)
                //{
                //    totalCanThuThem = totalCanThuThem - (resultInvoiceBill.AMOUNT - (resultInvoiceBill.KC_AMOUNT ?? 0));
                //}
                //if (resultRecieptBill != null)
                //{
                //    totalCanThuThem = totalCanThuThem - (resultRecieptBill.AMOUNT - (resultRecieptBill.KC_AMOUNT ?? 0));
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CalcuCanThu(bool isUpdateLbl)
        {
            try
            {
                var listRecieptFund = bindingSource1.DataSource as List<VHisBillFundADO>;
                decimal totalFund = 0;
                decimal discount = 0;
                if (!checkNotReciept.Checked)
                {
                    if (listRecieptFund != null && listRecieptFund.Count > 0)
                    {
                        totalFund += listRecieptFund.Sum(o => o.AMOUNT);
                    }
                    discount += spinRecieptDiscountPrice.Value;
                }
                if (!checkNotInvoice.Checked)
                {
                    discount += spinInvoiceDiscountPrice.Value;
                }

                if (isUpdateLbl)
                {
                    if (checkIsKC.CheckState == CheckState.Checked)
                    {
                        if (totalHienDu >= (totalPatientPrice - totalFund - discount))
                        {
                            lblCanThu.Text = Inventec.Common.Number.Convert.NumberToString(0);
                        }
                        else
                        {
                            lblCanThu.Text = Inventec.Common.Number.Convert.NumberToString((totalPatientPrice - totalFund - discount) - totalHienDu, ConfigApplications.NumberSeperator);
                        }
                    }
                    else
                    {
                        lblCanThu.Text = Inventec.Common.Number.Convert.NumberToString((totalPatientPrice - totalFund - discount), ConfigApplications.NumberSeperator);
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadConfigPrinter()
        {
            try
            {
                dicPrinter = new Dictionary<string, string>();
                string value = (System.Configuration.ConfigurationSettings.AppSettings[HIS_CONFIG__PRINT_TYPE__PRINTER] ?? "");
                if (!String.IsNullOrEmpty(value))
                {
                    string[] configs = value.Split(';');
                    if (configs == null || configs.Length <= 0)
                    {
                        throw new NullReferenceException("Khong cat duoc du lieu cau hinh: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => value), value));
                    }

                    foreach (var item in configs)
                    {
                        if (String.IsNullOrEmpty(item))
                            continue;
                        var data = item.Split(':');
                        if (data == null || data.Length != 2)
                        {
                            Inventec.Common.Logging.LogSystem.Info("Du lieu cau hinh khong chinh xac: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => item), item));
                            continue;
                        }
                        if (String.IsNullOrEmpty(data[0]) || String.IsNullOrEmpty(data[0].Trim()) || String.IsNullOrEmpty(data[1]) || String.IsNullOrEmpty(data[1].Trim()))
                        {
                            Inventec.Common.Logging.LogSystem.Info("Ma loai in hoac ten may in trong: " + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => data), data));
                            continue;
                        }
                        dicPrinter[data[0].Trim()] = data[1].Trim();
                    }
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
                dxValidationProvider1 = new DevExpress.XtraEditors.DXErrorProvider.DXValidationProvider();
                ValidControlRecieptAccountBook();
                ValidControlInvoiceAccountBook();
                ValidControlPayForm();
                ValidControlTransactionTime();
                ValidControlBuyerAccountCode();
                ValidControlBuyerAddress();
                ValidControlBuyerName();
                ValidControlBuyerOrganization();
                ValidControlBuyerTaxCode();
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
                validate.editor = this.txtRecieptDescription;
                validate.maxLength = 2000;
                validate.IsRequired = false;
                validate.ErrorText = string.Format("Nhập quá ký tự cho phép {0}", 2000);
                validate.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                dxValidationProvider1.SetValidationRule(this.txtRecieptDescription, validate);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlRecieptAccountBook()
        {
            try
            {
                RecieptAccountBookValidationRule recieptAccBookRule = new RecieptAccountBookValidationRule();
                recieptAccBookRule.listData = listRecieptData;
                recieptAccBookRule.txtRecieptAccountBookCode = txtRecieptAccountBookCode;
                recieptAccBookRule.cboRecieptAccountBook = cboRecieptAccountBook;
                recieptAccBookRule.checNotkReciept = checkNotReciept;
                dxValidationProvider1.SetValidationRule(txtRecieptAccountBookCode, recieptAccBookRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }



        private void ValidControlInvoiceAccountBook()
        {
            try
            {
                InvoiceAccountBookValidationRule invoiceAccBookRule = new InvoiceAccountBookValidationRule();
                invoiceAccBookRule.listData = listInvoiceData;
                invoiceAccBookRule.txtInvoiceAccountBookCode = txtInvoiceAccountBookCode;
                invoiceAccBookRule.cboInvoiceAccountBook = cboInvoiceAccountBook;
                invoiceAccBookRule.checkNotInvoice = checkNotInvoice;
                dxValidationProvider1.SetValidationRule(txtInvoiceAccountBookCode, invoiceAccBookRule);
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
                InvoicePayFormValidationRule invoicePayFormRule = new InvoicePayFormValidationRule();
                invoicePayFormRule.listData = listInvoiceData;
                invoicePayFormRule.txtInvoicePayFormCode = txtPayForm;
                invoicePayFormRule.cboInvoicePayForm = cboPayForm;
                invoicePayFormRule.checkNotInvoice = checkNotInvoice;
                dxValidationProvider1.SetValidationRule(txtPayForm, invoicePayFormRule);
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
                TransactionTimeValidationRule tranTimeRule = new TransactionTimeValidationRule();
                tranTimeRule.dtTransactionTime = dtTransactionTime;
                dxValidationProvider1.SetValidationRule(dtTransactionTime, tranTimeRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlBuyerName()
        {
            try
            {
                BuyerNameValidationRule buyerNameRule = new BuyerNameValidationRule();
                buyerNameRule.txtBuyerName = txtBuyerName;
                dxValidationProvider1.SetValidationRule(txtBuyerName, buyerNameRule);

                BuyerNameValidationRule buyerNameRule2 = new BuyerNameValidationRule();
                buyerNameRule2.txtBuyerName = txtBuyerName2;
                dxValidationProvider1.SetValidationRule(txtBuyerName2, buyerNameRule2);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlBuyerAddress()
        {
            try
            {
                BuyerAddressValidationRule buyerAddressRule = new BuyerAddressValidationRule();
                buyerAddressRule.txtBuyerAddress = txtBuyerAddress;
                dxValidationProvider1.SetValidationRule(txtBuyerAddress, buyerAddressRule);

                BuyerAddressValidationRule buyerAddressRule2 = new BuyerAddressValidationRule();
                buyerAddressRule2.txtBuyerAddress = txtBuyerAddress2;
                dxValidationProvider1.SetValidationRule(txtBuyerAddress2, buyerAddressRule2);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlBuyerAccountCode()
        {
            try
            {
                BuyerAccountCodeValidationRule buyerAccountCodeRule = new BuyerAccountCodeValidationRule();
                buyerAccountCodeRule.txtBuyerAccountCode = txtBuyerAccountCode;
                dxValidationProvider1.SetValidationRule(txtBuyerAccountCode, buyerAccountCodeRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlBuyerTaxCode()
        {
            try
            {
                BuyerTaxCodeValidationRule buyerTaxCodeRule = new BuyerTaxCodeValidationRule();
                buyerTaxCodeRule.txtBuyerTaxCode = txtBuyerTaxCode;
                dxValidationProvider1.SetValidationRule(txtBuyerTaxCode, buyerTaxCodeRule);

                BuyerTaxCodeValidationRule buyerTaxCodeRule2 = new BuyerTaxCodeValidationRule();
                buyerTaxCodeRule2.txtBuyerTaxCode = txtBuyerTaxCode2;
                dxValidationProvider1.SetValidationRule(txtBuyerTaxCode2, buyerTaxCodeRule2);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlBuyerOrganization()
        {
            try
            {
                BuyerOrganizationValidationRule buyerOrganizationRule = new BuyerOrganizationValidationRule();
                buyerOrganizationRule.txtBuyerOrganization = txtBuyerOrganization;
                dxValidationProvider1.SetValidationRule(txtBuyerOrganization, buyerOrganizationRule);

                BuyerOrganizationValidationRule buyerOrganizationRule2 = new BuyerOrganizationValidationRule();
                buyerOrganizationRule2.txtBuyerOrganization = txtBuyerOrganization2;
                dxValidationProvider1.SetValidationRule(txtBuyerOrganization2, buyerOrganizationRule2);
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

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                this.isInit = true;
                this.ResetFillPatientDefault();
                this.ResetData();
                this.ClearValidate();
                this.LoadSearch();
                this.FillInfoPatient(treatment);
                this.LoadAccountBookToLocal();
                this.FillDataToGirdTransaction();
                this.GeneratePopupMenu();
                this.LoadListSereServ();
                this.ProcessDataByCheckNot();
                this.ResetControlValue();
                this.SetDefaultPayForm();
                this.CalcuTotalPrice();
                this.ProcessFundForHCM();
                this.CalcuHienDu();
                this.CalcuCanThu(true);
                this.FillDataToTienHoaDon();
                this.FillDataToTongChiPhi();
                this.LoadConfigPrinter();
                WaitingManager.Hide();
                if (this.treatment != null)
                {
                    this.txtSearch.Text = this.treatment.TREATMENT_CODE;
                    this.btnSavePrint.Focus();

                }
                this.isInit = false;
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ResetFillPatientDefault()
        {
            try
            {
                txtPatientCode.Text = "";
                txtPatientName.Text = "";
                txtDOB.Text = "";
                txtGender.Text = "";
                txtAddress.Text = "";
                txtPatientType.Text = "";
                txtHeinCard.Text = "";
                txtHeinFrom.Text = "";
                txtHeinTo.Text = "";
                txtMediOrg.Text = "";
                txtBuyerAccountCode.Text = "";
                txtBuyerAddress.Text = "";
                txtBuyerName.Text = "";
                txtBuyerOrganization.Text = "";
                txtBuyerTaxCode.Text = "";
                lblHeinRatio.Text = "";
                lblRightRoute.Text = "";
                //qtcode
                txtBuyerEmail.Text = ""; 
                //qtcode
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ResetData()
        {
            try
            {
                listTransaction = new List<V_HIS_TRANSACTION>();
                dicSereServBill = new Dictionary<long, List<HIS_SERE_SERV_BILL>>();

                resultRecieptBill = null;
                resultInvoiceBill = null;

                ListBillFund = new List<VHisBillFundADO>();
                ListSereServ = new List<V_HIS_SERE_SERV_5>();

                listSereServADO = new List<VHisSereServADO>();
                records = null;

                listInvoiceData = new List<VHisSereServADO>();
                listRecieptData = new List<VHisSereServADO>();

                listBillFundReciept = new List<VHisBillFundADO>();

                this.inputSereServs = null;

                treatmentId = null;
                treatment = null;

                totalPatientPrice = 0;
                totalHienDu = 0;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private async Task FillInfoPatient(V_HIS_TREATMENT_FEE data)
        {
            if (treatment != null)
            {
                txtPatientCode.Text = data.TDL_PATIENT_CODE;
                txtPatientName.Text = data.TDL_PATIENT_NAME;
                txtDOB.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(data.TDL_PATIENT_DOB);
                txtGender.Text = data.TDL_PATIENT_GENDER_NAME;
                txtAddress.Text = data.TDL_PATIENT_ADDRESS;

                HisPatientFilter ft = new HisPatientFilter();
                ft.ID = data.PATIENT_ID; 
                var listPatient = new BackendAdapter(new CommonParam()).Get<List<HIS_PATIENT>>("api/HisPatient/Get", ApiConsumers.MosConsumer, ft, new CommonParam());
                if (listPatient != null && listPatient.Count > 0)
                {
                    HIS_PATIENT a = listPatient.FirstOrDefault();
                    txtBuyerEmail.Text = a.EMAIL; 
                }

                if (data.TDL_PATIENT_TYPE_ID != null)
                {
                    txtPatientType.Text = BackendDataWorker.Get<HIS_PATIENT_TYPE>().FirstOrDefault(o => o.ID == data.TDL_PATIENT_TYPE_ID).PATIENT_TYPE_NAME;
                }
                else
                {
                    txtPatientType.Text = "";
                }

                txtBuyerAccountCode.Text = data.TDL_PATIENT_ACCOUNT_NUMBER ?? "";
                txtBuyerAddress.Text = data.TDL_PATIENT_ADDRESS ?? "";
                

                txtBuyerName.Text = data.TDL_PATIENT_NAME ?? "";
                txtBuyerOrganization.Text = data.TDL_PATIENT_WORK_PLACE_NAME ?? data.TDL_PATIENT_WORK_PLACE ?? "";
                txtBuyerTaxCode.Text = data.TDL_PATIENT_TAX_CODE ?? "";

                txtBuyerName2.Text = data.TDL_PATIENT_NAME ?? "";
                txtBuyerAddress2.Text = data.WORK_PLACE_ADDRESS ?? "";
                txtBuyerOrganization2.Text = data.TDL_PATIENT_WORK_PLACE_NAME ?? data.TDL_PATIENT_WORK_PLACE ?? "";
                txtBuyerTaxCode2.Text = data.WORK_PLACE_TAX_CODE ?? "";

                HisPatientTypeAlterViewAppliedFilter filter = new HisPatientTypeAlterViewAppliedFilter();
                filter.TreatmentId = treatment.ID;
                filter.InstructionTime = Inventec.Common.DateTime.Get.Now() ?? 0;
                var currentPatientTypeAlter = await new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).GetAsync<V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, filter, null);
                if (currentPatientTypeAlter != null)
                {
                    txtHeinCard.Text = HeinCardHelper.TrimHeinCardNumber(currentPatientTypeAlter.HEIN_CARD_NUMBER);
                    txtHeinFrom.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(currentPatientTypeAlter.HEIN_CARD_FROM_TIME ?? 0);
                    txtHeinTo.Text = Inventec.Common.DateTime.Convert.TimeNumberToDateString(currentPatientTypeAlter.HEIN_CARD_TO_TIME ?? 0);
                    txtMediOrg.Text = currentPatientTypeAlter.HEIN_MEDI_ORG_NAME;
                    string rightRoute = "";
                    if (currentPatientTypeAlter.RIGHT_ROUTE_CODE == MOS.LibraryHein.Bhyt.HeinRightRoute.HeinRightRouteCode.TRUE)
                    {
                        rightRoute = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__RIGHT_ROUTE_TRUE", Base.ResourceLangManager.LanguageFrmTransactionBillTwoInOne, LanguageManager.GetCulture());
                    }
                    else
                    {
                        rightRoute = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_TRANSACTION_BILL__RIGHT_ROUTE_FALSE", Base.ResourceLangManager.LanguageFrmTransactionBillTwoInOne, LanguageManager.GetCulture());
                    }
                    lblRightRoute.Text = rightRoute ?? "";
                    string ratio = "";
                    if (currentPatientTypeAlter.PATIENT_TYPE_ID == HisConfig.PatientTypeId__BHYT)
                    {
                        decimal? heinRatio = new MOS.LibraryHein.Bhyt.BhytHeinProcessor().GetDefaultHeinRatio(currentPatientTypeAlter.HEIN_TREATMENT_TYPE_CODE, currentPatientTypeAlter.HEIN_CARD_NUMBER, currentPatientTypeAlter.LEVEL_CODE, currentPatientTypeAlter.RIGHT_ROUTE_CODE, this.GetTotalPriceOfTreatment());
                        if (heinRatio.HasValue)
                        {
                            ratio = ((long)(heinRatio.Value * 100)).ToString() + "%";
                        }
                    }
                    lblHeinRatio.Text = ratio ?? "";
                }
            }
        }

        private decimal GetTotalPriceOfTreatment()
        {
            decimal result = 0;
            try
            {
                if (this.inputSereServs != null)
                {
                    foreach (var item in this.inputSereServs)
                    {
                        if (item.IS_DELETE == 1 || !item.SERVICE_REQ_ID.HasValue || item.IS_EXPEND == 1 || item.IS_NO_EXECUTE == 1 || item.PATIENT_TYPE_ID != HisConfig.PatientTypeId__BHYT)
                            continue;
                        decimal totalPrice = (item.VIR_TOTAL_HEIN_PRICE ?? 0) + (item.VIR_TOTAL_PATIENT_PRICE_BHYT ?? 0);
                        result += totalPrice;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = 0;
            }
            return result;
        }

        private void LoadSearch()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisTreatmentFeeViewFilter filter = new HisTreatmentFeeViewFilter();
                if (!String.IsNullOrEmpty(txtSearch.Text))
                {
                    string code = txtSearch.Text.Trim();
                    if (code.Length < 12)
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        txtSearch.Text = code;
                    }
                    filter.TREATMENT_CODE__EXACT = code;

                    var listTreatment = new BackendAdapter(param)
                        .Get<List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_FEE>>(HisRequestUriStore.HIS_TREATMENT_GETFEEVIEW, ApiConsumers.MosConsumer, filter, param);
                    if (listTreatment != null && listTreatment.Count == 1)
                    {
                        this.treatment = listTreatment.FirstOrDefault();
                        this.treatmentId = treatment.ID;
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

        private void txtFindTreatmentCode_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                    btnSearch_Click(null, null);
                e.Handled = true;
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

        private void ClearValidate()
        {
            try
            {
                dxValidationProvider1.RemoveControlError(txtRecieptAccountBookCode);
                dxValidationProvider1.RemoveControlError(txtPayForm);
                dxValidationProvider1.RemoveControlError(txtInvoiceAccountBookCode);
                dxValidationProvider1.RemoveControlError(dtTransactionTime);
                dxValidationProvider1.RemoveControlError(txtBuyerName);
                dxValidationProvider1.RemoveControlError(txtBuyerAddress);
                dxValidationProvider1.RemoveControlError(txtBuyerAccountCode);
                dxValidationProvider1.RemoveControlError(txtBuyerTaxCode);
                dxValidationProvider1.RemoveControlError(txtBuyerOrganization);
                dxValidationProvider1.RemoveControlError(txtBuyerName2);
                dxValidationProvider1.RemoveControlError(txtBuyerAddress2);
                dxValidationProvider1.RemoveControlError(txtBuyerTaxCode2);
                dxValidationProvider1.RemoveControlError(txtBuyerOrganization2);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void radioSGAll_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (!isInit && radioSGAll.Checked)
                {
                    this.CheckAllNode();
                    this.ProcessAfterCheckNode();
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
                    this.CheckAllNode();
                    this.ProcessAfterCheckNode();
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
                    this.CheckAllNode();
                    this.ProcessAfterCheckNode();
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
                    this.CheckAllNode();
                    this.ProcessAfterCheckNode();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
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

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch_Click(null, null);
                }
                e.Handled = true;
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
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
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
                        else if (item.KEY == chkConnectPos.Name)
                        {
                            chkConnectPos.Checked = item.VALUE == "1";
                        }
                    }
                }
                isNotLoadWhileChangeControlStateInFirst = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
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
                        if (data.TRANSACTION_TYPE_ID == IMSys.DbConfig.HIS_RS.HIS_TRANSACTION_TYPE.ID__TT && !String.IsNullOrWhiteSpace(data.INVOICE_CODE))
                        {
                            e.Appearance.Font = new System.Drawing.Font(e.Appearance.Font, System.Drawing.FontStyle.Bold);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkIsAutoRepay_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                cboRepayAccountBook.Enabled = checkIsAutoRepay.Checked;
                spinRepayNumOrder.Enabled = checkIsAutoRepay.Checked;
                SetDefaultAccountBookRepay();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void checkIsAutoRepay_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    cboRepayAccountBook.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboRepayAccountBook_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == DevExpress.XtraEditors.PopupCloseMode.Normal)
                {
                    if (cboRepayAccountBook.EditValue != null)
                    {
                        var account = this.ListAccountBookRepay.FirstOrDefault(o => o.ID == Convert.ToInt64(cboRepayAccountBook.EditValue));
                        if (account != null)
                        {
                        }
                    }
                    else
                    {
                        spinRepayNumOrder.Text = "";
                        spinRepayNumOrder.EditValue = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboRepayAccountBook_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                spinRepayNumOrder.EditValue = null;
                spinRepayNumOrder.Enabled = false;
                if (cboRepayAccountBook.EditValue != null)
                {
                    var account = this.ListAccountBookRepay.FirstOrDefault(o => o.ID == Convert.ToInt64(cboRepayAccountBook.EditValue));
                    if (account != null)
                    {
                        spinRepayNumOrder.EditValue = setDataToDicNumOrderInAccountBook(account);

                        if (account.IS_NOT_GEN_TRANSACTION_ORDER == 1)
                        {
                            spinRepayNumOrder.Enabled = true;
                            ValidControlNumorderRepay(true);
                        }
                        else
                        {
                            spinRepayNumOrder.Enabled = false;
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

        private void cboRepayAccountBook_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinRepayNumOrder.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinRepayNumOrder_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

        }

        private void spinRepayNumOrder_Spin(object sender, SpinEventArgs e)
        {

        }

        private void ValidControlNumorderRepay(bool isRequired)
        {
            try
            {
                SpinNumOrderRepayValidationRule numorderRule = new SpinNumOrderRepayValidationRule();
                numorderRule.spinNumorder = spinRepayNumOrder;
                numorderRule.isRequired = isRequired;
                dxValidationProvider1.SetValidationRule(spinRepayNumOrder, numorderRule);
                if (isRequired)
                {
                    lciRepayNumOrder.AppearanceItemCaption.ForeColor = Color.Maroon;
                }
                else
                {
                    lciRepayNumOrder.AppearanceItemCaption.ForeColor = Color.Black;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }



        private void chkConnectPos_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (isNotLoadWhileChangeControlStateInFirst)
                {
                    return;
                }
                WaitingManager.Show();
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkConnectPos.Name && o.MODULE_LINK == currentModule.ModuleLink).FirstOrDefault() : null;
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => csAddOrUpdate), csAddOrUpdate));
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkConnectPos.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkConnectPos.Name;
                    csAddOrUpdate.VALUE = (chkConnectPos.Checked ? "1" : "");
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

        private void btnConfigPos_Click(object sender, EventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private bool IsProcessOpen(string name)
        {
            try
            {
                var processByNames = System.Diagnostics.Process.GetProcesses().Where(o => o.ProcessName.Contains(name)).ToList();
                if (processByNames != null && processByNames.Count >= 2)
                {
                    return true;
                }
                return false;
                //foreach (Process clsProcess in Process.GetProcesses())
                //{
                //    if (clsProcess.ProcessName.Contains(name))
                //    {
                //        return true;
                //    }
                //}               
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
                return false;
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
                    nameFile = startInfo.FileName;
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

        private void frmTransactionBillTwoInOne_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                string repay = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>("HIS.Desktop.Plugins.TransactionBill.Repay");
                if (repay == "1")
                {
                    if (btnSave.Enabled == false || layoutControlItem10.Enabled == false)
                    {
                        LoadSearch();
                        resultRecieptBill = null;
                        resultInvoiceBill = null;
                        CalcuHienDu();
                        Inventec.Common.Logging.LogSystem.Debug("totalHienDu: " + totalHienDu);
                        if (totalHienDu > 0)
                        {
                            if (MessageBox.Show("Bạn có muốn hoàn ứng không?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                Inventec.Desktop.Common.Modules.Module moduleData = GlobalVariables.currentModuleRaws.Where(o => o.ModuleLink == "HIS.Desktop.Plugins.TransactionRepay").FirstOrDefault();
                                if (moduleData == null) throw new NullReferenceException("Not found module by ModuleLink = 'HIS.Desktop.Plugins.TransactionRepay'");
                                if (moduleData.IsPlugin && moduleData.ExtensionInfo != null)
                                {
                                    moduleData.RoomId = this.currentModule.RoomId;
                                    moduleData.RoomTypeId = this.currentModule.RoomTypeId;
                                    List<object> listArgs = new List<object>();
                                    HIS.Desktop.ADO.TransactionRepayADO ado = new HIS.Desktop.ADO.TransactionRepayADO(this.treatment.ID, this.cashierRoom.ID);
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
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void chkBuyerInfo_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkBuyerInfo.Checked)
                {
                    navigationFrameBuyerInfo.SelectedPage = navigationPage1;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkOrganizationInfo_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkOrganizationInfo.Checked)
                {
                    navigationFrameBuyerInfo.SelectedPage = navigationPage2;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void navigationFrameBuyerInfo_SelectedPageChanged(object sender, DevExpress.XtraBars.Navigation.SelectedPageChangedEventArgs e)
        {
            try
            {
                if (chkBuyerInfo.Checked)
                {
                    navigationFrameBuyerInfo.SelectedPage = navigationPage1;
                }
                else if (chkOrganizationInfo.Checked)
                {
                    navigationFrameBuyerInfo.SelectedPage = navigationPage2;
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnQR_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.listTranToQR != null) CreateQR(this.listTranToQR, true);
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
        List<HIS_CONFIG> listConfig = new List<HIS_CONFIG>();
        HIS_CONFIG selectedConfig = new HIS_CONFIG();
        List<V_HIS_TRANSACTION> listTranToQR;
        class ConfigInfo
        {
            public string BANK { get; set; }
            public string VALUE { get; set; }
        }
        private void loadConfig()
        {
            try
            {
                listConfig = BackendDataWorker.Get<HIS_CONFIG>().Where(o => o.KEY.StartsWith("HIS.Desktop.Plugins.PaymentQrCode") && !string.IsNullOrEmpty(o.VALUE)).ToList();
                if (listConfig == null || listConfig.Count == 0) lciQR.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
                else lciQR.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Always;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private void CreateQR(List<V_HIS_TRANSACTION> data, bool click)
        {
            try
            {
                var currentRoom = BackendDataWorker.Get<V_HIS_ROOM>().Where(s => s.ID == this.currentModule.RoomId && !string.IsNullOrEmpty(s.QR_CONFIG_JSON));
                if (currentRoom != null && currentRoom.Count() > 0)
                {
                    ConfigInfo _config = Newtonsoft.Json.JsonConvert.DeserializeObject<ConfigInfo>(currentRoom.FirstOrDefault().QR_CONFIG_JSON);
                    HIS_CONFIG _cf = new HIS_CONFIG();
                    if (string.IsNullOrWhiteSpace(_config.BANK)) MessageBox.Show(this, "Cấu hình thiếu thông tin ngân hàng.", "Thông báo", MessageBoxButtons.OK);
                    _cf.KEY = string.Format("HIS.Desktop.Plugins.PaymentQrCode.{0}Info", _config.BANK.Trim());
                    _cf.VALUE = _config.VALUE;
                    //co cau hinh QR o buong benh
                    List<object> listArgs = new List<object>();
                    TransReqQRADO adoqr = new TransReqQRADO();
                    adoqr.TreatmentId = this.treatment.ID;
                    adoqr.ConfigValue = _cf;
                    adoqr.TransReqId = CreateReqType.Transaction;
                    AutoMapper.Mapper.CreateMap<V_HIS_TRANSACTION, HIS_TRANSACTION>();
                    List<HIS_TRANSACTION> lstTran = AutoMapper.Mapper.Map<List<V_HIS_TRANSACTION>, List<HIS_TRANSACTION>>(data);
                    adoqr.Transactions = lstTran;
                    listArgs.Add(adoqr);
                    LogSystem.Debug("_____Load module : HIS.Desktop.Plugins.CreateTransReqQR " + LogUtil.TraceData("listArgs", listArgs));
                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.CreateTransReqQR", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);

                }
                else
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
                                    adoqr.TreatmentId = this.treatment.ID;
                                    adoqr.ConfigValue = selectedConfig;
                                    adoqr.TransReqId = CreateReqType.Transaction;
                                    AutoMapper.Mapper.CreateMap<V_HIS_TRANSACTION, HIS_TRANSACTION>();
                                    List<HIS_TRANSACTION> lstTran = AutoMapper.Mapper.Map<List<V_HIS_TRANSACTION>, List<HIS_TRANSACTION>>(data);
                                    adoqr.Transactions = lstTran;
                                    listArgs.Add(adoqr);
                                    LogSystem.Debug("_____Load module : HIS.Desktop.Plugins.CreateTransReqQR " + LogUtil.TraceData("listArgs", listArgs));
                                    HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.CreateTransReqQR", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);

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
                            adoqr.TreatmentId = this.treatment.ID;
                            adoqr.ConfigValue = selectedConfig;
                            adoqr.TransReqId = CreateReqType.Transaction;
                            AutoMapper.Mapper.CreateMap<V_HIS_TRANSACTION, HIS_TRANSACTION>();
                            List<HIS_TRANSACTION> lstTran = AutoMapper.Mapper.Map<List<V_HIS_TRANSACTION>, List<HIS_TRANSACTION>>(data);
                            adoqr.Transactions = lstTran;
                            listArgs.Add(adoqr);
                            LogSystem.Debug("_____Load module : HIS.Desktop.Plugins.CreateTransReqQR " + LogUtil.TraceData("listArgs", listArgs));
                            HIS.Desktop.ModuleExt.PluginInstanceBehavior.ShowModule("HIS.Desktop.Plugins.CreateTransReqQR", this.currentModule.RoomId, this.currentModule.RoomTypeId, listArgs);


                        }

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

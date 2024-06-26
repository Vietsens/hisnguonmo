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
using HIS.Desktop.Plugins.InvoiceCreateForTreatment.Validation;
using HIS.Desktop.Utility;
using HIS.UC.SereServTree;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.LibraryHein.Bhyt;
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

namespace HIS.Desktop.Plugins.InvoiceCreateForTreatment
{
    public partial class frmInvoiceCreateForTreatment : HIS.Desktop.Utility.FormBase
    {
        SereServTreeProcessor ssTreeProcessor = new SereServTreeProcessor();
        UserControl ucSereServTree = null;
        List<V_HIS_SERE_SERV_5> currentSereServs;
        Dictionary<long, List<V_HIS_SERE_SERV_BILL>> dicSereServBill;
        HIS_BRANCH branch = null;

        HIS_INVOICE resultInvoice = null;
        List<V_HIS_USER_INVOICE_BOOK> listInvoiceBook = new List<V_HIS_USER_INVOICE_BOOK>();
        List<V_HIS_SERE_SERV_5> listSereServ = new List<V_HIS_SERE_SERV_5>();
        long treatmentId;
        V_HIS_TREATMENT_2 treatment = null;

        decimal totalPatientPrice = 0;
        int positionHandleControl = -1;

        Inventec.Desktop.Common.Modules.Module currentModule;

        const string EXE_KEY__PAY_FORM_CODE = "EXE_KEY__PAY_FORM_CODE";
        const string invoiceTypeCreate__CreateInvoiceVnpt = "1";

        string account = ConfigurationManager.AppSettings["Inventec.Common.ElectronicBill.Account"].ToString();// "vietsens";//Tài khoản được cấp phát cho nhân viên gọi lệnh phát hành hóa đơn
        string aCPass = ConfigurationManager.AppSettings["Inventec.Common.ElectronicBill.ACPass"].ToString();//"123456";
        string username = ConfigurationManager.AppSettings["Inventec.Common.ElectronicBill.Username"].ToString();//"bvkxservice";//Tài khoản được cấp phát cho khách hàng để gọi service.
        string pass = ConfigurationManager.AppSettings["Inventec.Common.ElectronicBill.Pass"].ToString();//"123456aA@";
        int convert = 1;//Mặc định là 0, 0 – Không cần convert từ TCVN3 sang Unicode. 1- Cần convert từ TCVN3 sang Unicode 
        string pattern = ConfigurationManager.AppSettings["Inventec.Common.ElectronicBill.Pattern"].ToString();//"02GTTT0/001";
        string serial = ConfigurationManager.AppSettings["Inventec.Common.ElectronicBill.Serial"].ToString();//"AA/17E";

        public frmInvoiceCreateForTreatment(Inventec.Desktop.Common.Modules.Module module, long data)
		:base(module)
        {
            InitializeComponent();
            try
            {
                Base.ResourceLangManager.InitResourceLanguageManager();
                this.SetIcon();
                this.treatmentId = data;
                this.currentModule = module;
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }
                IniSereServTree();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public frmInvoiceCreateForTreatment(V_HIS_TREATMENT_2 data)
        {
            InitializeComponent();
            try
            {
                Base.ResourceLangManager.InitResourceLanguageManager();
                this.SetIcon();
                this.treatmentId = data.ID;
                this.treatment = data;
                IniSereServTree();
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
                string iconPath = System.IO.Path.Combine(HIS.Desktop.LocalStorage.Location.ApplicationStoreLocation.ApplicationStartupPath, System.Configuration.ConfigurationSettings.AppSettings["Inventec.Desktop.Icon"]);
                this.Icon = Icon.ExtractAssociatedIcon(iconPath);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void IniSereServTree()
        {
            try
            {
                ssTreeProcessor = new UC.SereServTree.SereServTreeProcessor();
                SereServTreeADO ado = new SereServTreeADO();
                ado.IsShowSearchPanel = false;
                ado.SereServTreeColumns = new List<SereServTreeColumn>();
                ado.IsShowCheckNode = true;
                //ado.SereServTree_GetSelectImage = treeSereServ_GetSelectImage;
                //ado.SereServTree_SelectImageClick = treeSereServ_SelectImageClick;
                ado.SereServTree_CustomDrawNodeCell = treeSereServ_CustomDrawNodeCell;
                ado.SereServTree_BeforeCheck = treeSereServ_BeforeCheck;
                ado.SereServTree_AfterCheck = treeSereServ_AfterCheck;
                ado.SereServTree_CheckAllNode = treeSereServ_CheckAllNode;

                ado.LayoutSereServExpend = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE_FOR_TREATMENT__LAYOUT_SERE_SERV_EXPEND", Base.ResourceLangManager.LanguageFrmInvoiceCreateForTreatment, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());
                //Column tên dịch vụ
                SereServTreeColumn serviceNameCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE_FOR_TREATMENT__TREE_SERE_SERV__COLUMN_SERVICE_NAME", Base.ResourceLangManager.LanguageFrmInvoiceCreateForTreatment, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "TDL_SERVICE_NAME", 200, false);
                serviceNameCol.VisibleIndex = 0;
                ado.SereServTreeColumns.Add(serviceNameCol);

                //Column Số lượng
                SereServTreeColumn amountCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE_FOR_TREATMENT__TREE_SERE_SERV__COLUMN_AMOUNT", Base.ResourceLangManager.LanguageFrmInvoiceCreateForTreatment, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "AMOUNT_PLUS", 40, false);
                amountCol.VisibleIndex = 1;
                amountCol.Format = new DevExpress.Utils.FormatInfo();
                amountCol.Format.FormatString = "#,##0.00";
                amountCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.SereServTreeColumns.Add(amountCol);

                //Column đơn giá
                SereServTreeColumn virPriceCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE_FOR_TREATMENT__TREE_SERE_SERV__COLUMN_VIR_PRICE", Base.ResourceLangManager.LanguageFrmInvoiceCreateForTreatment, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "VIR_PRICE", 110, false);
                virPriceCol.VisibleIndex = 2;
                virPriceCol.Format = new DevExpress.Utils.FormatInfo();
                virPriceCol.Format.FormatString = "#,##0.0000";
                virPriceCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.SereServTreeColumns.Add(virPriceCol);

                //Column thành tiền
                SereServTreeColumn virTotalPriceCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE_FOR_TREATMENT__TREE_SERE_SERV__COLUMN_VIR_TOTAL_PRICE", Base.ResourceLangManager.LanguageFrmInvoiceCreateForTreatment, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "VIR_TOTAL_PRICE", 110, false);
                virTotalPriceCol.VisibleIndex = 3;
                virTotalPriceCol.Format = new DevExpress.Utils.FormatInfo();
                virTotalPriceCol.Format.FormatString = "#,##0.0000";
                virTotalPriceCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.SereServTreeColumns.Add(virTotalPriceCol);

                //Column đồng chi trả
                SereServTreeColumn virTotalHeinPriceCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE_FOR_TREATMENT__TREE_SERE_SERV__COLUMN_VIR_TOTAL_HEIN_PRICE", Base.ResourceLangManager.LanguageFrmInvoiceCreateForTreatment, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "VIR_TOTAL_HEIN_PRICE", 110, false);
                virTotalHeinPriceCol.VisibleIndex = 4;
                virTotalHeinPriceCol.Format = new DevExpress.Utils.FormatInfo();
                virTotalHeinPriceCol.Format.FormatString = "#,##0.0000";
                virTotalHeinPriceCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.SereServTreeColumns.Add(virTotalHeinPriceCol);

                //Column bệnh nhân trả
                SereServTreeColumn virTotalPatientPriceCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE_FOR_TREATMENT__TREE_SERE_SERV__COLUMN_VIR_TOTAL_PATIENT_PRICE", Base.ResourceLangManager.LanguageFrmInvoiceCreateForTreatment, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "VIR_TOTAL_PATIENT_PRICE", 110, false);
                virTotalPatientPriceCol.VisibleIndex = 5;
                virTotalPatientPriceCol.Format = new DevExpress.Utils.FormatInfo();
                virTotalPatientPriceCol.Format.FormatString = "#,##0.0000";
                virTotalPatientPriceCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.SereServTreeColumns.Add(virTotalPatientPriceCol);

                //Column chiết khấu
                SereServTreeColumn virDiscountCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE_FOR_TREATMENT__TREE_SERE_SERV__COLUMN_DISCOUNT", Base.ResourceLangManager.LanguageFrmInvoiceCreateForTreatment, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "DISCOUNT", 110, false);
                virDiscountCol.VisibleIndex = 6;
                virDiscountCol.Format = new DevExpress.Utils.FormatInfo();
                virDiscountCol.Format.FormatString = "#,##0.0000";
                virDiscountCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.SereServTreeColumns.Add(virDiscountCol);

                //Column hao phí
                SereServTreeColumn virIsExpendCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE_FOR_TREATMENT__TREE_SERE_SERV__COLUMN_IS_EXPEND", Base.ResourceLangManager.LanguageFrmInvoiceCreateForTreatment, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "IsExpend", 60, false);
                virIsExpendCol.VisibleIndex = 7;
                ado.SereServTreeColumns.Add(virIsExpendCol);

                //Column vat (%)
                SereServTreeColumn virVatRatioCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE_FOR_TREATMENT__TREE_SERE_SERV__COLUMN_VAT_RATIO", Base.ResourceLangManager.LanguageFrmInvoiceCreateForTreatment, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "VAT_RATIO", 100, false);
                virVatRatioCol.VisibleIndex = 8;
                virVatRatioCol.Format = new DevExpress.Utils.FormatInfo();
                virVatRatioCol.Format.FormatString = "#,##0.00";
                virVatRatioCol.Format.FormatType = DevExpress.Utils.FormatType.Custom;
                ado.SereServTreeColumns.Add(virVatRatioCol);

                //Column mã dịch vụ
                SereServTreeColumn serviceCodeCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE_FOR_TREATMENT__TREE_SERE_SERV__COLUMN_SERVICE_CODE", Base.ResourceLangManager.LanguageFrmInvoiceCreateForTreatment, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "TDL_SERVICE_CODE", 100, false);
                serviceCodeCol.VisibleIndex = 9;
                ado.SereServTreeColumns.Add(serviceCodeCol);

                //Column Mã yêu cầu
                SereServTreeColumn serviceReqCodeCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE_FOR_TREATMENT__TREE_SERE_SERV__COLUMN_SERVICE_REQ_CODE", Base.ResourceLangManager.LanguageFrmInvoiceCreateForTreatment, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "TDL_SERVICE_REQ_CODE", 100, false);
                serviceReqCodeCol.VisibleIndex = 10;
                ado.SereServTreeColumns.Add(serviceReqCodeCol);

                //Column mã giao dịch
                //SereServTreeColumn TRANSACTIONCodeCol = new SereServTreeColumn(Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE_FOR_TREATMENT__TREE_SERE_SERV__COLUMN_TRANSACTION_CODE", Base.ResourceLangManager.LanguageFrmInvoiceCreateForTreatment, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()), "TRANSACTION_CODE", 100, false);
                //TRANSACTIONCodeCol.VisibleIndex = 11;
                //ado.SereServTreeColumns.Add(TRANSACTIONCodeCol);

                this.ucSereServTree = (UserControl)ssTreeProcessor.Run(ado);
                if (this.ucSereServTree != null)
                {
                    this.panelControlSereServTree.Controls.Add(this.ucSereServTree);
                    this.ucSereServTree.Dock = DockStyle.Fill;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmInvoiceCreateForTreatment_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                LoadKeyFrmLanguage();
                ValidControl();
                LoadInvoiceBook();
                LoadDataToComboInvoiceBook();               
                LoadDataToComboPayForm();
                LoadDataToTreeSereServ();
                ResetControlValue();
                CalcuTotalPrice();
                LoadSellerInfo();
                LoadBuyerInfo();
                txtTemplateCode.Focus();
                txtTemplateCode.SelectAll();
                if (listInvoiceBook.Count == 1)
                {
                    txtTemplateCode.Text = listInvoiceBook.First().TEMPLATE_CODE;
                    cboInvoiceBook.EditValue = listInvoiceBook.First().INVOICE_BOOK_ID;
                }
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadInvoiceBook()
        {
            try
            {
                listInvoiceBook = new List<V_HIS_USER_INVOICE_BOOK>();
                HisUserInvoiceBookViewFilter filter = new HisUserInvoiceBookViewFilter();
                filter.LOGINNAME = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                var listData = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_USER_INVOICE_BOOK>>(HisRequestUriStore.HIS_USER_INVOICE_BOOK_GET__VIEW, ApiConsumers.MosConsumer, filter, null);
                if (listData != null && listData.Count > 0)
                {
                    foreach (var item in listData)
                    {
                        if (item.CURRENT_NUM_ORDER >= (item.TOTAL + item.FROM_NUM_ORDER - 1))
                            continue;
                        if (item.IS_ACTIVE == IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__FALSE)
                            continue;
                        listInvoiceBook.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToComboInvoiceBook()
        {
            try
            {
                cboInvoiceBook.Properties.DataSource = listInvoiceBook;
                cboInvoiceBook.Properties.DisplayMember = "SYMBOL_CODE";
                cboInvoiceBook.Properties.ValueMember = "INVOICE_BOOK_ID";
                cboInvoiceBook.Properties.ForceInitialize();
                cboInvoiceBook.Properties.Columns.Clear();
                cboInvoiceBook.Properties.Columns.Add(new LookUpColumnInfo("TEMPLATE_CODE", "", 70));
                cboInvoiceBook.Properties.Columns.Add(new LookUpColumnInfo("SYMBOL_CODE", "", 100));
                cboInvoiceBook.Properties.ShowHeader = false;
                cboInvoiceBook.Properties.ImmediatePopup = true;
                cboInvoiceBook.Properties.DropDownRows = 10;
                cboInvoiceBook.Properties.PopupWidth = 170;               
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToComboPayForm()
        {
            try
            {
                cboPayForm.Properties.DataSource = BackendDataWorker.Get<HIS_PAY_FORM>();
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
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToTreeSereServ()
        {
            try
            {
                listSereServ = new List<V_HIS_SERE_SERV_5>();
                currentSereServs = new List<V_HIS_SERE_SERV_5>();
                dicSereServBill = new Dictionary<long, List<V_HIS_SERE_SERV_BILL>>();
                if (this.treatmentId > 0)
                {
                    HisSereServBillViewFilter ssBillFilter = new HisSereServBillViewFilter();
                    ssBillFilter.TDL_TREATMENT_ID = this.treatmentId;
                    var listSSBill = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV_BILL>>("api/HisSereServBill/GetView", ApiConsumers.MosConsumer, ssBillFilter, null);
                    if (listSSBill != null && listSSBill.Count > 0)
                    {
                        foreach (var item in listSSBill)
                        {
                            if (item.IS_CANCEL == 1)
                                continue;
                            if (!dicSereServBill.ContainsKey(item.SERE_SERV_ID))
                                dicSereServBill[item.SERE_SERV_ID] = new List<V_HIS_SERE_SERV_BILL>();
                            dicSereServBill[item.SERE_SERV_ID].Add(item);
                        }
                    }
                    HisSereServView5Filter ssFilter = new HisSereServView5Filter();
                    ssFilter.TDL_TREATMENT_ID = this.treatmentId;
                    var hisSereServs = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_SERE_SERV_5>>("api/HisSereServ/GetView5", ApiConsumers.MosConsumer, ssFilter, null);
                    if (hisSereServs != null && hisSereServs.Count > 0)
                    {
                        hisSereServs = hisSereServs.Where(o => !o.INVOICE_ID.HasValue).ToList();
                        currentSereServs = hisSereServs;
                        foreach (var item in hisSereServs)
                        {
                            if (dicSereServBill.ContainsKey(item.ID))
                            {
                                //continue;
                                //if (item.IS_NO_PAY == IMSys.DbConfig.HIS_RS.HIS_SERE_SERV.IS_NO_PAY__TRUE || item.VIR_TOTAL_PATIENT_PRICE <= 0 || item.IS_NO_EXECUTE == IMSys.DbConfig.HIS_RS.HIS_SERE_SERV.IS_NO_EXECUTE__TRUE)
                                //    continue;
                                listSereServ.Add(item);
                            }
                        }
                    }
                }
                ssTreeProcessor.Reload(ucSereServTree, listSereServ);
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
                resultInvoice = null;
                SetDefaultPayForm();
                txtAmount.Value = 0;
                txtTemplateCode.Text = "";
                cboInvoiceBook.EditValue = null;
                txtTongTuDen.Text = "";
                dtInvoiceTime.DateTime = DateTime.Now;
                txtDiscount.Value = 0;
                txtVatRatio.Text = "";
                btnSave.Enabled = true;
                btnPrint.Enabled = false;
                txtNumOrder.Text = "";
                txtNumOrder.EditValue = null;
                txtNumOrder.Enabled = true;
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
                string code = String.IsNullOrEmpty(ConfigApplicationWorker.Get<string>(EXE_KEY__PAY_FORM_CODE)) ? GlobalVariables.HIS_PAY_FORM_CODE__CONSTANT : ConfigApplicationWorker.Get<string>(EXE_KEY__PAY_FORM_CODE);
                
                var data = BackendDataWorker.Get<HIS_PAY_FORM>().FirstOrDefault(o => o.PAY_FORM_CODE == code);
                if (data != null)
                {
                    txtPayFormCode.Text = data.PAY_FORM_CODE;
                    cboPayForm.EditValue = data.ID;
                }
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
                if (listSereServ != null)
                {
                    totalPatientPrice = listSereServ.Sum(o => o.VIR_TOTAL_PATIENT_PRICE ?? 0);
                }
                txtAmount.Value = this.totalPatientPrice;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadBuyerInfo()
        {
            try
            {
                if (this.treatmentId <= 0)
                    return;
                if (this.treatment == null)
                {
                    HisTreatmentView2Filter treatFilter = new HisTreatmentView2Filter();
                    treatFilter.ID = this.treatmentId;
                    var listData = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_TREATMENT_2>>(HisRequestUriStore.HIS_TREATMENT_GETVIEW_2, ApiConsumers.MosConsumer, treatFilter, null);
                    if (listData != null && listData.Count == 1)
                    {
                        this.treatment = listData.First();
                    }
                }
                if (this.treatment != null)
                {
                    txtBuyerName.Text = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase((this.treatment.TDL_PATIENT_NAME ?? "").ToLower());
                    txtBuyerAddress.Text = this.treatment.TDL_PATIENT_ADDRESS;
                    string description = "";
                    description = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE_FOR_TREATMENT_FOR_TREATMENT__BUYER_INFO__PATIENT_TYPE", Base.ResourceLangManager.LanguageFrmInvoiceCreateForTreatment, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()) + this.treatment.PATIENT_TYPE_INFO;
                    if (!String.IsNullOrEmpty(this.treatment.TDL_HEIN_CARD_NUMBER))
                    {
                        HisPatientTypeAlterViewAppliedFilter appFilter = new HisPatientTypeAlterViewAppliedFilter();
                        appFilter.TreatmentId = this.treatmentId;
                        appFilter.InstructionTime = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmss"));
                        var patientTypeAlter = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<V_HIS_PATIENT_TYPE_ALTER>(HisRequestUriStore.HIS_PATIENT_TYPE_ALTER_GET_APPLIED, ApiConsumers.MosConsumer, appFilter, null);
                        if (patientTypeAlter != null)
                        {
                            //HisPatyAlterBhytViewFilter patyFilter = new HisPatyAlterBhytViewFilter();
                            //patyFilter.PATIENT_TYPE_ALTER_ID = patientTypeAlter.ID;
                            //var listPaty = new Inventec.Common.Adapter.BackendAdapter(new CommonParam()).Get<List<V_HIS_PATY_ALTER_BHYT>>(HisRequestUriStore.HIS_PATY_ALTER_BHYT_GETVIEW, ApiConsumers.MosConsumer, patyFilter, null);
                            //if (listPaty != null && listPaty.Count == 1)
                            //{
                            var patyAlter = patientTypeAlter;// listPaty.First();
                            description += Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE_FOR_TREATMENT_FOR_TREATMENT__BUYER_INFO__HEIN_CARD_NUMBER", Base.ResourceLangManager.LanguageFrmInvoiceCreateForTreatment, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()) + HeinCardHelper.SetHeinCardNumberDisplayByNumber(patyAlter.HEIN_CARD_NUMBER);
                            description += Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE_FOR_TREATMENT_FOR_TREATMENT__BUYER_INFO__HEIN_CARD_FROM_TIME", Base.ResourceLangManager.LanguageFrmInvoiceCreateForTreatment, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()) + Inventec.Common.DateTime.Convert.TimeNumberToDateString(patyAlter.HEIN_CARD_FROM_TIME ?? 0);
                            description += Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE_FOR_TREATMENT_FOR_TREATMENT__BUYER_INFO__HEIN_CARD_TO_TIME", Base.ResourceLangManager.LanguageFrmInvoiceCreateForTreatment, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()) + Inventec.Common.DateTime.Convert.TimeNumberToDateString(patyAlter.HEIN_CARD_TO_TIME ?? 0);
                            var data = new BhytHeinProcessor().GetDefaultHeinRatio(patientTypeAlter.HEIN_TREATMENT_TYPE_CODE, patyAlter.HEIN_CARD_NUMBER, branch.HEIN_LEVEL_CODE, patyAlter.RIGHT_ROUTE_CODE);
                            description += Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE_FOR_TREATMENT_FOR_TREATMENT__BUYER_INFO__HEIN_RATIO", Base.ResourceLangManager.LanguageFrmInvoiceCreateForTreatment, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture()) + ((new BhytHeinProcessor().GetDefaultHeinRatio(patientTypeAlter.HEIN_TREATMENT_TYPE_CODE, patyAlter.HEIN_CARD_NUMBER, branch.HEIN_LEVEL_CODE, patyAlter.RIGHT_ROUTE_CODE) ?? 0) * 100) + "%";
                            //}
                        }
                    }
                    txtBuyerDescription.Text = description;
                }
                else
                {
                    txtBuyerAccountNumber.Text = "";
                    txtBuyerAddress.Text = "";
                    txtBuyerDescription.Text = "";
                    txtBuyerName.Text = "";
                    txtBuyerOrganization.Text = "";
                    txtBuyerTaxCode.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadSellerInfo()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisBranchFilter filter = new HisBranchFilter();
                filter.ID = WorkPlace.GetBranchId();

                var listBranch = new BackendAdapter(param).Get<List<HIS_BRANCH>>("api/HisBranch/Get", ApiConsumers.MosConsumer, filter, param);

                if (listBranch != null && listBranch.Count > 0)
                {
                    this.branch = listBranch.FirstOrDefault();
                }

                if (this.branch != null)
                {
                    txtSellerName.Text = this.branch.BRANCH_NAME;
                    txtSellerAccountNumber.Text = this.branch.ACCOUNT_NUMBER;
                    txtSellerAddress.Text = this.branch.ADDRESS;
                    txtSellerPhone.Text = this.branch.PHONE;
                    txtSellerTaxCode.Text = this.branch.TAX_CODE;
                }
                else
                {
                    txtSellerName.Text = "";
                    txtSellerAccountNumber.Text = "";
                    txtSellerAddress.Text = "";
                    txtSellerPhone.Text = "";
                    txtSellerTaxCode.Text = "";
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
                ValidControlInvoiceBook();
                ValidControlInvoiceTime();
                ValidControlPayForm();
                ValidControlBuyerName();
                ValidControlSellerName();
                ValidControlVatRatio();
                ValidControlDiscount();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }

        }

        private void ValidControlInvoiceBook()
        {
            try
            {
                InvoiceBookValidationRule invoiceBookRule = new InvoiceBookValidationRule();
                invoiceBookRule.txtTemplateCode = txtTemplateCode;
                invoiceBookRule.cboInvoiceBook = cboInvoiceBook;
                dxValidationProvider1.SetValidationRule(txtTemplateCode, invoiceBookRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlInvoiceTime()
        {
            try
            {
                InvoiceTimeValidationRule invoiceTimeRule = new InvoiceTimeValidationRule();
                invoiceTimeRule.dtInvoiceTime = dtInvoiceTime;
                dxValidationProvider1.SetValidationRule(dtInvoiceTime, invoiceTimeRule);
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
                payFormRule.txtPayFormCode = txtPayFormCode;
                payFormRule.cboPayForm = cboPayForm;
                dxValidationProvider1.SetValidationRule(txtPayFormCode, payFormRule);
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
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlSellerName()
        {
            try
            {
                SellerNameValidationRule sellerNameRule = new SellerNameValidationRule();
                sellerNameRule.txtSellerName = txtSellerName;
                dxValidationProvider1.SetValidationRule(txtSellerName, sellerNameRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlVatRatio()
        {
            try
            {
                VatRatioValidationRule vatRule = new VatRatioValidationRule();
                vatRule.txtVatRatio = txtVatRatio;
                dxValidationProvider1.SetValidationRule(txtVatRatio, vatRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlDiscount()
        {
            try
            {
                DiscountValidationRule discountRule = new DiscountValidationRule();
                discountRule.txtDiscount = txtDiscount;
                dxValidationProvider1.SetValidationRule(txtDiscount, discountRule);
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

                var cul = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                var lang = Base.ResourceLangManager.LanguageFrmInvoiceCreateForTreatment;
                //Button
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE_FOR_TREATMENT__BTN_SAVE", lang, cul);
                this.btnNew.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE_FOR_TREATMENT__BTN_NEW", lang, cul);
                this.btnPrint.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE_FOR_TREATMENT__BTN_PRINT", lang, cul);
                this.btnCreateElectricInvoice.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE_FOR_TREATMENT__BTN_CREATE_ELECTRICT_BILL", lang, cul);

                //layout
                this.layoutAmount.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE_FOR_TREATMENT__LAYOUT_AMOUNT", lang, cul);
                this.layoutBuyerAccountNumber.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE_FOR_TREATMENT__LAYOUT_BUYER_ACCOUNT_NUMBER", lang, cul);
                this.layoutBuyerAddress.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE_FOR_TREATMENT__LAYOUT_BUYER_ADDRESS", lang, cul);
                this.layoutBuyerDescription.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE_FOR_TREATMENT__LAYOUT_BUYER_DESCRIPTION", lang, cul);
                this.layoutBuyerName.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE_FOR_TREATMENT__LAYOUT_BUYER_NAME", lang, cul);
                this.layoutBuyerOrganization.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE_FOR_TREATMENT__LAYOUT_BUYER_ORGANIZATION", lang, cul);
                this.layoutBuyerTaxCode.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE_FOR_TREATMENT__LAYOUT_BUYER_TAX_CODE", lang, cul);
                this.layoutControlBuyerInfo.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE_FOR_TREATMENT__LAYOUT_BUYER_INFO", lang, cul);
                this.layoutControlSellerInfo.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE_FOR_TREATMENT__LAYOUT_SELLER_INFO", lang, cul);
                this.layoutDiscount.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE_FOR_TREATMENT__LAYOUT_DISCOUNT", lang, cul);
                this.layoutInvoiceBook.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE_FOR_TREATMENT__LAYOUT_INVOICE_BOOK", lang, cul);
                this.layoutInvoiceTime.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE_FOR_TREATMENT__LAYOUT_INVOICE_TIME", lang, cul);
                this.layoutNumOrder.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE_FOR_TREATMENT__LAYOUT_NUM_ORDER", lang, cul);
                this.layoutPayForm.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE_FOR_TREATMENT__LAYOUT_PAY_FORM", lang, cul);
                this.layoutSellerAccountNumber.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE_FOR_TREATMENT__LAYOUT_SELLER_ACCOUNT_NUMBER", lang, cul);
                this.layoutSellerAddress.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE_FOR_TREATMENT__LAYOUT_SELLER_ADDRESS", lang, cul);
                this.layoutSellerName.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE_FOR_TREATMENT__LAYOUT_SELLER_NAME", lang, cul);
                this.layoutSellerPhone.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE_FOR_TREATMENT__LAYOUT_SELLER_PHONE", lang, cul);
                this.layoutSellerTaxCode.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE_FOR_TREATMENT__LAYOUT_SELLER_TAX_CODE", lang, cul);
                this.layoutTongTuDen.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE_FOR_TREATMENT__LAYOUT_TONG_TU_DEN", lang, cul);
                this.layoutVatRatio.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE_FOR_TREATMENT__LAYOUT_VAT_RATIO", lang, cul);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }


    }
}

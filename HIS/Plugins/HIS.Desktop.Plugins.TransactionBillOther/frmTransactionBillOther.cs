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
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.ViewInfo;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.ConfigSystem;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.TransactionBillOther.ADO;
using HIS.Desktop.Plugins.TransactionBillOther.Base;
using HIS.Desktop.Plugins.TransactionBillOther.Validation;
using HIS.Desktop.Print;
using HIS.Desktop.Utility;
using Inventec.Common.Adapter;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using Inventec.Desktop.Common.Modules;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors.Repository;
using Inventec.Common.Controls.EditorLoader;
using HIS.Desktop.Common;
using HIS.Desktop.Plugins.Library.ElectronicBill.Base;
using HIS.Desktop.Plugins.Library.ElectronicBill;
using Inventec.Common.DocumentViewer;
using System.Threading;

namespace HIS.Desktop.Plugins.TransactionBillOther
{
    public partial class frmTransactionBillOther : HIS.Desktop.Utility.FormBase
    {
        private int positionHandleControl = -1;
        private long treatmentId;
        private HIS_TREATMENT treatment = null;
        private V_HIS_PATIENT_TYPE_ALTER patientTypeAlter = null;
        private V_HIS_CASHIER_ROOM cashierRoom = null;

        private decimal totalPrice = 0;

        private bool printNow = false;

        private V_HIS_TRANSACTION currentTransaction = null;

        private List<V_HIS_ACCOUNT_BOOK> ListAccountBook = new List<V_HIS_ACCOUNT_BOOK>();
        List<V_HIS_NONE_MEDI_SERVICE> glstNoneMediService = new List<V_HIS_NONE_MEDI_SERVICE>();
        V_HIS_NONE_MEDI_SERVICE currentNoneService = new V_HIS_NONE_MEDI_SERVICE();

        List<HisBillGoodADO> ListBillGood = new List<HisBillGoodADO>();
        Inventec.Desktop.Common.Modules.Module moduleData;

        HIS.Desktop.Library.CacheClient.ControlStateWorker controlStateWorker;
        List<HIS.Desktop.Library.CacheClient.ControlStateRDO> currentControlStateRDO;
        bool isNotLoadWhileChangeControlStateInFirst;

        public frmTransactionBillOther(Module moduleData)
            : base(moduleData)
        {
            InitializeComponent();
            Base.ResourceLangManager.InitResourceLanguageManager();
            this.SetResourceKeyLanguage();
        }

        public frmTransactionBillOther(Module moduleData, long treatmentId)
            : base(moduleData)
        {
            InitializeComponent();
            this.moduleData = moduleData;
            this.treatmentId = treatmentId;
            Base.ResourceLangManager.InitResourceLanguageManager();
            this.SetResourceKeyLanguage();
        }

        private void SetResourceKeyLanguage()
        {
            try
            {
                var resources = Base.ResourceLangManager.LangFrmTransactionBillOther;
                var lang = Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture();
                btnAdd.Text = Inventec.Common.Resource.Get.Value("btnAdd.Text", resources, lang);
                btnNew.Text = Inventec.Common.Resource.Get.Value("btnNew.Text", resources, lang);
                btnPrint.Text = Inventec.Common.Resource.Get.Value("btnPrint.Text", resources, lang);
                btnSave.Text = Inventec.Common.Resource.Get.Value("btnSave.Text", resources, lang);
                btnSaveAndPrint.Text = Inventec.Common.Resource.Get.Value("btnSaveAndPrint.Text", resources, lang);
                txtTreatmentCodeSearch.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("txtTreatmentCodeSearch.NullText", resources, lang);
                repositoryItemButtonDelete.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("repositoryItemButtonDelete.Tooltip", resources, lang);

                gridColumn_BillGood_Amount.Caption = Inventec.Common.Resource.Get.Value("gridColumn_BillGood_Amount.Caption", resources, lang);
                gridColumn_BillGood_Delete.ToolTip = Inventec.Common.Resource.Get.Value("gridColumn_BillGood_Delete.Tooltip", resources, lang);
                gridColumn_BillGood_Discount.Caption = Inventec.Common.Resource.Get.Value("gridColumn_BillGood_Discount.Caption", resources, lang);
                gridColumn_BillGood_GoodName.Caption = Inventec.Common.Resource.Get.Value("gridColumn_BillGood_GoodName.Caption", resources, lang);
                gridColumn_BillGood_GoodUnitName.Caption = Inventec.Common.Resource.Get.Value("gridColumn_BillGood_GoodUnitName.Caption", resources, lang);
                gridColumn_BillGood_Price.Caption = Inventec.Common.Resource.Get.Value("gridColumn_BillGood_Price.Caption", resources, lang);
                LcTreatmentCodeSearch.Text = Inventec.Common.Resource.Get.Value("LcTreatmentCodeSearch.Text", resources, lang);
                lcChk.Text = Inventec.Common.Resource.Get.Value("lcChk.Text", resources, lang);
                lcPatientName.Text = Inventec.Common.Resource.Get.Value("lcPatientName.Text", resources, lang);
                lcBuyerTaxCode.Text = Inventec.Common.Resource.Get.Value("lcBuyerTaxCode.Text", resources, lang);
                lcBuyerAccountNumber.Text = Inventec.Common.Resource.Get.Value("lcBuyerAccountNumber.Text", resources, lang);
                lcBuyerOrganization.Text = Inventec.Common.Resource.Get.Value("lcBuyerOrganization.Text", resources, lang);
                lcPatientAddress.Text = Inventec.Common.Resource.Get.Value("lcPatientAddress.Text", resources, lang);
                lcDescription.Text = Inventec.Common.Resource.Get.Value("lcDescription.Text", resources, lang);
                layoutControlItem4.Text = Inventec.Common.Resource.Get.Value("lcGoodName.Text", resources, lang);
                lcGoodUnitName.Text = Inventec.Common.Resource.Get.Value("lcGoodUnitName.Text", resources, lang);
                lcGoodAmount.Text = Inventec.Common.Resource.Get.Value("lcGoodAmount.Text", resources, lang);
                lcGoodPrice.Text = Inventec.Common.Resource.Get.Value("lcGoodPrice.Text", resources, lang);
                lcGoodDiscount.Text = Inventec.Common.Resource.Get.Value("lcGoodDiscount.Text", resources, lang);
                lcGoodDescription.Text = Inventec.Common.Resource.Get.Value("lcGoodDescription.Text", resources, lang);
                lcAccountBookCode.Text = Inventec.Common.Resource.Get.Value("lcAccountBookCode.Text", resources, lang);
                lcPayForm.Text = Inventec.Common.Resource.Get.Value("lcPayForm.Text", resources, lang);
                lcTransactionTime.Text = Inventec.Common.Resource.Get.Value("lcTransactionTime.Text", resources, lang);
                lcExemption.Text = Inventec.Common.Resource.Get.Value("lcExemption.Text", resources, lang);
                lcExemptionReason.Text = Inventec.Common.Resource.Get.Value("lcExemptionReason.Text", resources, lang);
                lcNumOrder.Text = Inventec.Common.Resource.Get.Value("lcNumOrder.Text", resources, lang);
                lcTotalAmount.Text = Inventec.Common.Resource.Get.Value("lcTotalAmount.Text", resources, lang);
                groupBox1.Text = Inventec.Common.Resource.Get.Value("groupBox1.Text", resources, lang);
                groupBox2.Text = Inventec.Common.Resource.Get.Value("groupBox2.Text", resources, lang);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void frmTransactionBillOther_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                this.ValidControl();
                InitControlState();
                LoadDataToComboNoneMediService();
                this.LoadCashierRoom();
                this.LoadTreatment();
                this.LoadPatientTypeAlter();
                this.LoadAccountBookToLocal();
                this.LoadDataToComboPayForm();
                this.SetDefaultCommonControl();
                this.SetDefautGoodControl();
                this.SetPatientInfo();
                this.SetDefaultAccountBook();
                this.SetDefaultPayForm();
                this.ResetControlValidationGood();
                this.RefreshDataGridControl();
                this.CaluTotalPrice();
                this.SetEnableButtonByTreatment();
                this.LoadDataToComboAccountBook();
                chkCheckXD.Checked = false;
                if (txtTreatmentCodeSearch.Text.Trim().Length > 0)
                    btnSearch.Enabled = true;
                else
                    btnSearch.Enabled = false;
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadAccountBookToLocal()
        {
            try
            {
                string loginName = Inventec.UC.Login.Base.ClientTokenManagerStore.ClientTokenManager.GetLoginName();
                if (String.IsNullOrWhiteSpace(loginName))
                {
                    layoutControlGroup1.Enabled = false;
                    MessageBox.Show("Không thanh toán được, mời bạn chọn lại");
                    return;
                }

                List<long> ids = new List<long>();
                //ListUserAccountBook = new List<V_HIS_USER_ACCOUNT_BOOK>();
                HisUserAccountBookFilter useAccountBookFilter = new HisUserAccountBookFilter();
                useAccountBookFilter.LOGINNAME__EXACT = loginName;
                var userAccountBooks = new BackendAdapter(new CommonParam()).Get<List<HIS_USER_ACCOUNT_BOOK>>("api/HisUserAccountBook/Get", ApiConsumers.MosConsumer, useAccountBookFilter, null);

                HisCaroAccountBookFilter caroAccountBookFilter = new HisCaroAccountBookFilter();
                caroAccountBookFilter.CASHIER_ROOM_ID = this.cashierRoom.ID;
                var caroAccountBooks = new BackendAdapter(new CommonParam()).Get<List<HIS_CARO_ACCOUNT_BOOK>>("api/HisCaroAccountBook/Get", ApiConsumers.MosConsumer, caroAccountBookFilter, null);
                // Kiểm tra sổ còn hay k
                if (userAccountBooks != null && userAccountBooks.Count > 0)
                {
                    ids.AddRange(userAccountBooks.Select(s => s.ACCOUNT_BOOK_ID).ToList());
                }
                if (caroAccountBooks != null && caroAccountBooks.Count > 0)
                {
                    ids.AddRange(caroAccountBooks.Select(s => s.ACCOUNT_BOOK_ID).ToList());
                }
                if (ids != null && ids.Count > 0)
                {
                    ids = ids.Distinct().ToList();
                    int dem = 0;
                    while (ids.Count >= dem)
                    {
                        var idsTmp = ids.Skip(dem).Take(100).ToList();
                        dem += 100;
                        HisAccountBookViewFilter acFilter = new HisAccountBookViewFilter();
                        acFilter.IDs = idsTmp;
                        acFilter.IS_ACTIVE = IMSys.DbConfig.HIS_RS.COMMON.IS_ACTIVE__TRUE;
                        //acFilter.FOR_BILL = true;
                        acFilter.IS_OUT_OF_BILL = false;
                        acFilter.ORDER_DIRECTION = "DESC";
                        acFilter.ORDER_FIELD = "ID";
                        acFilter.FOR_OTHER_SALE = true;
                        var rsData = new BackendAdapter(new CommonParam()).Get<List<V_HIS_ACCOUNT_BOOK>>("api/HisAccountBook/GetView", ApiConsumers.MosConsumer, acFilter, null);
                        if(rsData !=null && rsData.Count > 0)
                        {
                            ListAccountBook.AddRange(rsData);
                        }
                    }
                    LoadDataToComboAccountBook();
                    SetDefaultAccountBook();
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
                if (accountBook == null) accountBook = ListAccountBook.FirstOrDefault();

                if (accountBook != null)
                {
                    txtAccountBookCode.Text = accountBook.ACCOUNT_BOOK_CODE;
                    cboAccountBook.EditValue = accountBook.ID;
                    SetDataToDicNumOrderInAccountBook(accountBook);
                }
                else
                {
                    spinNumOrder.Text = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadDataToComboNoneMediService()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisNoneMediServiceViewFilter filter = new HisNoneMediServiceViewFilter();
                filter.IS_ACTIVE = 1;
                glstNoneMediService = new BackendAdapter(param).Get<List<V_HIS_NONE_MEDI_SERVICE>>("api/HisNoneMediService/GetView", ApiConsumers.MosConsumer, filter, param);
                if (glstNoneMediService != null && glstNoneMediService.Count > 0)
                {
                    glstNoneMediService = glstNoneMediService.OrderBy(o => o.NUM_ORDER ?? 99999999).ThenBy(o => o.NONE_MEDI_SERVICE_NAME).ToList();
                }

                cboNoneMediService.Properties.DataSource = glstNoneMediService;
                cboNoneMediService.Properties.DisplayMember = "NONE_MEDI_SERVICE_NAME";
                cboNoneMediService.Properties.ValueMember = "ID";
                cboNoneMediService.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
                cboNoneMediService.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
                cboNoneMediService.Properties.ImmediatePopup = true;
                cboNoneMediService.ForceInitialize();
                cboNoneMediService.Properties.View.Columns.Clear();
                cboNoneMediService.Properties.PopupFormSize = new Size(700, 250);

                var aColumnCode = cboNoneMediService.Properties.View.Columns.AddField("NONE_MEDI_SERVICE_CODE");
                aColumnCode.Caption = "Mã";
                aColumnCode.Visible = true;
                aColumnCode.VisibleIndex = 1;
                aColumnCode.Width = 60;

                var aColumnName = cboNoneMediService.Properties.View.Columns.AddField("NONE_MEDI_SERVICE_NAME");
                aColumnName.Caption = "Tên";
                aColumnName.Visible = true;
                aColumnName.VisibleIndex = 2;
                aColumnName.Width = 340;
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
                cboPayForm.Properties.DataSource = BackendDataWorker.Get<HIS_PAY_FORM>().Where(o =>o.IS_ACTIVE ==1);
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

        private void LoadTreatment()
        {
            try
            {
                if (this.treatment == null)
                {
                    CommonParam param = new CommonParam();
                    HisTreatmentFilter treatFilter = new HisTreatmentFilter();
                    treatFilter.ID = this.treatmentId;
                    List<HIS_TREATMENT> hisTreatments = new BackendAdapter(param).Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, treatFilter, param);
                    this.treatment = hisTreatments != null ? hisTreatments.FirstOrDefault() : null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadPatientTypeAlter()
        {
            try
            {

                if (this.treatment != null)
                {
                    this.patientTypeAlter = new BackendAdapter(new CommonParam())
                    .Get<V_HIS_PATIENT_TYPE_ALTER>("api/HisPatientTypeAlter/GetViewLastByTreatmentId", ApiConsumers.MosConsumer, this.treatment.ID, null);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void LoadCashierRoom()
        {
            try
            {
                this.cashierRoom = BackendDataWorker.Get<V_HIS_CASHIER_ROOM>().FirstOrDefault(o => o.ROOM_ID == this.currentModuleBase.RoomId);
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
                ValidControlGoodName();
                ValidControlGoodUnitName();
                ValidControlGoodAmount();
                ValidControlGoodPrice();
                ValidControlGoodDiscount();
                ValidControlGoodDescription();
                ValidControlAccountBook();
                ValidControlTransactionTime();
                ValidControlBuyerAccountCode();
                ValidControlBuyerAddress();
                ValidControlBuyerOrganization();
                ValidControlBuyerTaxCode();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlGoodName()
        {
            try
            {
                GoodNameValidationRule goodNameRule = new GoodNameValidationRule();
                goodNameRule.txtGoodName = cboNoneMediService;
                dxValidationProvider1.SetValidationRule(cboNoneMediService, goodNameRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlGoodUnitName()
        {
            try
            {
                GoodUnitNameValidationRule goodUnitNameRule = new GoodUnitNameValidationRule();
                goodUnitNameRule.txtGoodUnitName = txtGoodUnitName;
                dxValidationProvider1.SetValidationRule(txtGoodUnitName, goodUnitNameRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlGoodAmount()
        {
            try
            {
                GoodAmountValidationRule goodAmountRule = new GoodAmountValidationRule();
                goodAmountRule.spinGoodAmount = spinGoodAmount;
                dxValidationProvider1.SetValidationRule(spinGoodAmount, goodAmountRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlGoodPrice()
        {
            try
            {
                GoodPriceValidationRule goodPriceRule = new GoodPriceValidationRule();
                goodPriceRule.spinGoodPrice = spinGoodPrice;
                dxValidationProvider1.SetValidationRule(spinGoodPrice, goodPriceRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlGoodDiscount()
        {
            try
            {
                GoodDiscountValidationRule goodDiscountRule = new GoodDiscountValidationRule();
                goodDiscountRule.spinGoodAmount = spinGoodAmount;
                goodDiscountRule.spinGoodDiscount = spinGoodDiscount;
                goodDiscountRule.spinGoodPrice = spinGoodPrice;
                dxValidationProvider1.SetValidationRule(spinGoodDiscount, goodDiscountRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ValidControlGoodDescription()
        {
            try
            {
                GoodDescriptionValidationRule goodDescriptionRule = new GoodDescriptionValidationRule();
                goodDescriptionRule.txtGoodDescription = txtGoodDescription;
                goodDescriptionRule.spinGoodDiscount = spinGoodDiscount;
                dxValidationProvider1.SetValidationRule(txtGoodDescription, goodDescriptionRule);
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
                AccountBookValidationRule accountBookRule = new AccountBookValidationRule();
                accountBookRule.cboAccountBook = cboAccountBook;
                accountBookRule.txtAccountBookCode = txtAccountBookCode;
                dxValidationProvider2.SetValidationRule(txtAccountBookCode, accountBookRule);
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
                dxValidationProvider2.SetValidationRule(dtTransactionTime, tranTimeRule);
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
                dxValidationProvider2.SetValidationRule(txtBuyerAddress, buyerAddressRule);
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
                buyerAccountCodeRule.txtBuyerAccountCode = txtBuyerAccountNumber;
                dxValidationProvider2.SetValidationRule(txtBuyerAccountNumber, buyerAccountCodeRule);
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
                dxValidationProvider2.SetValidationRule(txtBuyerTaxCode, buyerTaxCodeRule);
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
                dxValidationProvider2.SetValidationRule(txtBuyerOrganization, buyerOrganizationRule);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetPatientInfo()
        {
            try
            {
                if (this.treatment != null)
                {
                    txtTreatmentCodeSearch.Text = this.treatment.TREATMENT_CODE;
                    txtPatientName.Text = this.treatment.TDL_PATIENT_NAME ?? "";
                    txtBuyerAccountNumber.Text = treatment.TDL_PATIENT_ACCOUNT_NUMBER ?? "";
                    txtBuyerOrganization.Text = treatment.TDL_PATIENT_WORK_PLACE_NAME ?? treatment.TDL_PATIENT_WORK_PLACE ?? "";
                    txtBuyerAddress.Text = treatment.TDL_PATIENT_ADDRESS ?? "";
                    txtBuyerTaxCode.Text = treatment.TDL_PATIENT_TAX_CODE ?? "";
                }
                else
                {

                    txtPatientName.Text = "";
                    txtBuyerAccountNumber.Text = "";
                    txtBuyerTaxCode.Text = "";
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
                V_HIS_ACCOUNT_BOOK accountBook = null;
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
                    if (accountBook.IS_NOT_GEN_TRANSACTION_ORDER == 1)
                    {
                        spinNumOrder.Properties.ReadOnly = false;
                    }
                    else
                        spinNumOrder.Properties.ReadOnly = true;
                    txtAccountBookCode.Text = accountBook.ACCOUNT_BOOK_CODE;
                    cboAccountBook.EditValue = accountBook.ID;
                    SetDataToDicNumOrderInAccountBook(accountBook);
                }
                else
                {
                    spinNumOrder.Text = "";
                }
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
                if (cboPayForm.EditValue != null)
                {
                    return;
                }
                var ListPayForm = BackendDataWorker.Get<HIS_PAY_FORM>();
                if (ListPayForm != null && ListPayForm.Count > 0)
                {
                    var PayFormMinByCode = ListPayForm.OrderBy(o => o.PAY_FORM_CODE);
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

        private void SetDefaultCommonControl()
        {
            try
            {
                dtTransactionTime.DateTime = DateTime.Now;
                txtDescription.Text = "";
                spinExemption.Value = 0;
                spinExemptionRation.Value = 0;
                spinTotalAmount.Value = 0;
                repositoryItemButtonDelete.Buttons[0].Enabled = true;
                txtExemptionReason.Text = "";
                printNow = false;
                this.currentTransaction = null;
                this.ListBillGood = new List<HisBillGoodADO>();
                this.RefreshDataGridControl();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetDefautGoodControl()
        {
            try
            {
                cboNoneMediService.EditValue = null;
                txtGoodUnitName.Text = "";
                spinGoodAmount.Value = 0;
                spinGoodDiscount.Value = 0;
                spinGoodDiscountRatio.Value = 0;
                spinGoodPrice.Value = 0;
                txtGoodDescription.Text = "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ResetControlValidationGood()
        {
            try
            {
                dxValidationProvider1.RemoveControlError(cboNoneMediService);
                dxValidationProvider1.RemoveControlError(txtGoodUnitName);
                dxValidationProvider1.RemoveControlError(spinGoodPrice);
                dxValidationProvider1.RemoveControlError(spinGoodAmount);
                dxValidationProvider1.RemoveControlError(spinGoodDiscount);
                dxValidationProvider1.RemoveControlError(txtGoodDescription);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ResetControlValidationCommon()
        {
            try
            {
                dxValidationProvider2.RemoveControlError(txtAccountBookCode);
                dxValidationProvider2.RemoveControlError(dtTransactionTime);
                dxValidationProvider2.RemoveControlError(txtBuyerAccountNumber);
                dxValidationProvider2.RemoveControlError(txtBuyerAddress);
                dxValidationProvider2.RemoveControlError(txtBuyerOrganization);
                dxValidationProvider2.RemoveControlError(txtBuyerTaxCode);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void RefreshDataGridControl()
        {
            try
            {
                gridControlBillGoods.BeginUpdate();
                gridControlBillGoods.DataSource = this.ListBillGood;
                gridControlBillGoods.EndUpdate();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtGoodName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtGoodUnitName.Focus();
                    txtGoodUnitName.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtGoodUnitName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinGoodAmount.Focus();
                    spinGoodAmount.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinGoodAmount_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinGoodPrice.Focus();
                    spinGoodPrice.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinGoodPrice_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinGoodDiscount.Focus();
                    spinGoodDiscount.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinGoodDiscount_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (spinGoodDiscount.Value <= 0)
                    {
                        spinGoodDiscountRatio.Focus();
                        spinGoodDiscountRatio.SelectAll();
                    }
                    else
                    {
                        txtGoodDescription.Focus();
                        txtGoodDescription.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinGoodDiscount_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (spinGoodDiscount.EditValue != spinGoodDiscount.OldEditValue)
                {
                    decimal totalPrice = spinGoodAmount.Value * spinGoodPrice.Value;
                    if (totalPrice > 0)
                    {
                        spinGoodDiscountRatio.Value = (spinGoodDiscount.Value / totalPrice) * 100;
                    }
                    else
                    {
                        spinGoodDiscountRatio.Value = 0;
                    }
                }

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinGoodDiscountRatio_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

        }

        private void spinGoodDiscountRatio_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (spinGoodDiscountRatio.EditValue != spinGoodDiscountRatio.OldEditValue)
                {
                    decimal totalPrice = spinGoodAmount.Value * spinGoodPrice.Value;
                    spinGoodDiscount.Value = (spinGoodDiscountRatio.Value * totalPrice) / 100;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtGoodDescription_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

        }

        private void txtAccountBookCode_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (!String.IsNullOrWhiteSpace(txtAccountBookCode.Text))
                    {
                        var datas = this.ListAccountBook.Where(o => o.ACCOUNT_BOOK_CODE.ToLower().Contains(txtAccountBookCode.Text.ToLower().Trim())).ToList();
                        if (datas != null && datas.Count == 1)
                        {
                            txtAccountBookCode.Text = datas[0].ACCOUNT_BOOK_CODE;
                            cboAccountBook.EditValue = datas[0].ID;
                            cboPayForm.Focus();
                            cboPayForm.SelectAll();
                        }
                        else
                        {
                            cboAccountBook.Focus();
                            cboAccountBook.ShowPopup();
                        }
                    }
                    else
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
                            SetDataToDicNumOrderInAccountBook(account);
                        }
                    }
                    else
                    {
                        spinNumOrder.Text = "";
                    }
                    cboPayForm.Focus();
                    cboPayForm.SelectAll();
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
                if (cboAccountBook.EditValue != null)
                {
                    var account = ListAccountBook.FirstOrDefault(o => o.ID == Convert.ToInt64(cboAccountBook.EditValue));
                    if (account.IS_NOT_GEN_TRANSACTION_ORDER == 1)
                    {
                        spinNumOrder.Properties.ReadOnly = false;
                    }
                    else
                        spinNumOrder.Properties.ReadOnly = true;
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
                    dtTransactionTime.Focus();
                    dtTransactionTime.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void cboPayForm_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void dtTransactionTime_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinExemption.Focus();
                    spinExemption.SelectAll();
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
                    cboNoneMediService.Focus();
                    cboNoneMediService.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinNumOrder_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinTotalAmount.Focus();
                    spinTotalAmount.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinExemption_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (spinExemption.EditValue != spinExemption.OldEditValue)
                    {
                        if (totalPrice > 0)
                        {
                            spinExemptionRation.Value = (spinExemption.Value / totalPrice) * 100;
                        }
                        else
                        {
                            spinExemptionRation.Value = 0;
                        }
                        spinTotalAmount.Value = totalPrice - spinExemption.Value;
                    }
                    if (spinExemption.Value > 0)
                    {
                        spinExemptionRation.Focus();
                        spinExemptionRation.SelectAll();
                    }
                    else
                    {
                        spinExemptionRation.Focus();
                        spinExemptionRation.SelectAll();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinExemption_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                //if (spinExemption.EditValue != spinExemption.OldEditValue)
                //{
                //    if (totalPrice > 0)
                //    {                        
                //        spinExemptionRation.Value = (spinExemption.Value / totalPrice) * 100;                        
                //    }
                //    else
                //    {
                //        spinExemptionRation.Value = 0;
                //    }
                //    spinTotalAmount.Value = totalPrice - spinExemption.Value;
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinExemptionRation_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (spinExemptionRation.EditValue != spinExemptionRation.OldEditValue)
                    {
                        spinExemption.Value = (spinExemptionRation.Value * totalPrice) / 100;
                        spinTotalAmount.Value = totalPrice - spinExemption.Value;
                    }
                    if (spinExemptionRation.Value > 0)
                    {
                        txtExemptionReason.Focus();
                        txtExemptionReason.SelectAll();
                    }
                    else
                    {
                        SendKeys.Send("{TAB}");
                        SendKeys.Send("{TAB}");
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinExemptionRation_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                //if (spinExemptionRation.EditValue != spinExemptionRation.OldEditValue)
                //{
                //    spinExemption.Value = (spinExemptionRation.Value * totalPrice) / 100;
                //    spinTotalAmount.Value = totalPrice - spinExemption.Value;
                //}
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtExemptionReason_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    spinNumOrder.Focus();
                    spinNumOrder.SelectAll();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void CaluTotalPrice()
        {
            try
            {
                gridViewBillGoods.PostEditor();
                totalPrice = ListBillGood.Sum(s => s.TOTAL_PRICE_WITH_DISCOUNT);
                if (spinExemptionRation.Value > 0)
                {
                    spinExemption.Value = spinExemptionRation.Value * totalPrice / 100;
                }
                spinTotalAmount.Value = totalPrice - spinExemption.Value;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnAdd.Enabled || !dxValidationProvider1.Validate())
                    return;
                HisBillGoodADO ado = new HisBillGoodADO();
                ado.NONE_MEDI_SERVICE_ID = Convert.ToInt64(cboNoneMediService.EditValue);
                ado.SERVICE_UNIT_ID = currentNoneService.SERVICE_UNIT_ID;
                ado.AMOUNT = spinGoodAmount.Value;
                ado.DESCRIPTION = txtGoodDescription.Text;
                ado.DISCOUNT = spinGoodDiscount.Value;
                ado.GOODS_NAME = currentNoneService.NONE_MEDI_SERVICE_NAME;
                ado.GOODS_UNIT_NAME = txtGoodUnitName.Text;
                ado.PRICE = spinGoodPrice.Value;
                ado.TOTAL_PRICE = ado.AMOUNT * ado.PRICE;
                ado.TOTAL_PRICE_WITH_DISCOUNT = ado.TOTAL_PRICE - (ado.DISCOUNT ?? 0);
                this.ListBillGood.Add(ado);
                this.RefreshDataGridControl();
                this.CaluTotalPrice();
                this.SetDefautGoodControl();
                cboNoneMediService.Focus();
                cboNoneMediService.SelectAll();
                txtGoodUnitName.Enabled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void btnSaveAndPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnSaveAndPrint.Enabled || !dxValidationProvider2.Validate())
                    return;
                btnSaveAndPrint.Enabled = false;
                btnSave.Enabled = false;
                btnSaveAndSign.Enabled = false;
                CommonParam param = new CommonParam();
                WaitingManager.Show();
                bool success = this.ProcessSave(ref param);
                ResetControlValidationCommon();
                WaitingManager.Hide();
                if (success)
                {
                    this.printNow = true;
                    this.onClickPhieuThuThanhToan();
                }
                else
                {
                    btnSaveAndPrint.Enabled = true;
                    btnSave.Enabled = true;
                    btnSaveAndSign.Enabled = true;
                    MessageManager.Show(param, success);
                }
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
                if (!btnSave.Enabled || !dxValidationProvider2.Validate())
                    return;
                btnSaveAndPrint.Enabled = false;
                btnSave.Enabled = false;
                btnSaveAndSign.Enabled = false;
                printNow = false;
                CommonParam param = new CommonParam();
                WaitingManager.Show();
                bool success = this.ProcessSave(ref param);
                ResetControlValidationCommon();
                if (!success)
                {
                    btnSaveAndPrint.Enabled = true;
                    btnSave.Enabled = true;
                    btnSaveAndSign.Enabled = true;
                }
                WaitingManager.Hide();
                MessageManager.Show(this.ParentForm, param, success);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool ProcessSave(ref CommonParam param)
        {
            bool result = false;
            try
            {
                gridViewBillGoods.PostEditor();
                if (ListBillGood == null || ListBillGood.Count <= 0)
                {
                    param.Messages.Add(ResourceMessageManager.KhongCoDichVuThanhToan);
                    return false;
                }

                WaitingManager.Show();
                var listError = ListBillGood.Where(o => o.Error).ToList();
                if (listError != null && listError.Count > 0)
                {
                    foreach (var item in listError)
                    {
                        param.Messages.Add(item.ErrorText);
                    }
                    return false;
                }

                HisTransactionOtherBillSDO tranSdo = new HisTransactionOtherBillSDO();
                tranSdo.HisBillGoods = this.ListBillGood.ToList<HIS_BILL_GOODS>();
                tranSdo.HisTransaction = new HIS_TRANSACTION();
                if (chkCheckXD.Checked == false)
                {
                    if (this.treatment != null)
                        tranSdo.HisTransaction.TREATMENT_ID = this.treatment.ID;
                    else
                        tranSdo.HisTransaction.TREATMENT_ID = null;
                }
                else
                    tranSdo.HisTransaction.TREATMENT_ID = null;
                tranSdo.HisTransaction.CASHIER_ROOM_ID = this.cashierRoom.ID;
                tranSdo.RequestRoomId = this.currentModuleBase.RoomId;
                var accountBook = ListAccountBook.FirstOrDefault(o => o.ID == Convert.ToInt64(cboAccountBook.EditValue));
                if (accountBook != null)
                {
                    tranSdo.HisTransaction.ACCOUNT_BOOK_ID = accountBook.ID;
                }
                if (accountBook != null && accountBook.IS_NOT_GEN_TRANSACTION_ORDER == 1)
                {
                    tranSdo.HisTransaction.NUM_ORDER = (long)spinNumOrder.Value;
                }
                tranSdo.HisTransaction.PAY_FORM_ID = Convert.ToInt64(cboPayForm.EditValue);
                tranSdo.HisTransaction.AMOUNT = this.totalPrice;
                if (dtTransactionTime.EditValue != null && dtTransactionTime.DateTime != DateTime.MinValue)
                    tranSdo.HisTransaction.TRANSACTION_TIME = Convert.ToInt64(dtTransactionTime.DateTime.ToString("yyyyMMddHHmmss"));
                if (spinExemption.Value > 0)
                {
                    tranSdo.HisTransaction.EXEMPTION = Math.Round(spinExemption.Value, 4);
                    tranSdo.HisTransaction.EXEMPTION_REASON = txtExemptionReason.Text;
                }
                tranSdo.HisTransaction.DESCRIPTION = txtDescription.Text;
                tranSdo.HisTransaction.BUYER_ACCOUNT_NUMBER = txtBuyerAccountNumber.Text;
                tranSdo.HisTransaction.BUYER_ADDRESS = txtBuyerAddress.Text;
                tranSdo.HisTransaction.BUYER_NAME = txtPatientName.Text;
                tranSdo.HisTransaction.BUYER_ORGANIZATION = txtBuyerOrganization.Text;
                tranSdo.HisTransaction.BUYER_TAX_CODE = txtBuyerTaxCode.Text;

                var rs = new Inventec.Common.Adapter.BackendAdapter(param).Post<V_HIS_TRANSACTION>("api/HisTransaction/CreateOtherBill", ApiConsumers.MosConsumer, tranSdo, param);

                if (rs != null)
                {
                    this.currentTransaction = rs;
                    result = true;
                    btnPrint.Enabled = true;
                    btnSaveAndPrint.Enabled = false;
                    btnSave.Enabled = false;
                    btnSaveAndSign.Enabled = false;
                    btnAdd.Enabled = false;
                    repositoryItemButtonDelete.Buttons[0].Enabled = false;

                    AddLastAccountToLocal();
                    UpdateDictionaryNumOrderAccountBook(accountBook);
                }
                else
                {
                    result = false;
                    btnPrint.Enabled = false;
                    btnSaveAndPrint.Enabled = true;
                    btnSave.Enabled = true;
                    btnAdd.Enabled = true;
                    btnSaveAndSign.Enabled = true;
                    repositoryItemButtonDelete.Buttons[0].Enabled = true;
                }

                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                result = false;
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
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
                        var lstSameType = GlobalVariables.LastAccountBook.Where(o => o.IS_FOR_BILL == 1 && o.ID != accountBook.ID).ToList();// && o.BILL_TYPE_ID == accountBook.BILL_TYPE_ID).ToList();
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

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnPrint.Enabled)
                    return;
                this.printNow = false;
                this.onClickPhieuThuThanhToan();
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
                this.LoadAccountBookToLocal();
                this.LoadDataToComboPayForm();
                this.SetDefaultCommonControl();
                this.SetDefautGoodControl();
                this.SetPatientInfo();
                this.SetDefaultAccountBook();
                this.SetDefaultPayForm();
                this.ResetControlValidationGood();
                this.RefreshDataGridControl();
                this.CaluTotalPrice();
                this.SetEnableButtonByTreatment();
                txtTreatmentCodeSearch.Focus();
                txtTreatmentCodeSearch.SelectAll();
                txtTreatmentCodeSearch.Text = "";
                txtPatientName.Text = "";
                txtBuyerTaxCode.Text = "";
                txtBuyerAccountNumber.Text = "";
                txtBuyerOrganization.Text = "";
                txtBuyerAddress.Text = "";
                txtDescription.Text = "";
                chkCheckXD.Checked = false;
                this.LoadDataToComboAccountBook();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void onClickPhieuThuThanhToan()
        {
            try
            {
                if (this.currentTransaction != null)
                {
                    if (!String.IsNullOrWhiteSpace(this.currentTransaction.INVOICE_CODE))
                    {
                        onClickInHoaDonDienTu(null, null);
                    }
                    else if (this.currentTransaction.BILL_TYPE_ID == 1)
                    {
                        Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                        store.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuThuThanhToanKhac_MPS000355, InPhieuThuThanhToanHoaDon);
                    }
                    else
                    {
                        Inventec.Common.RichEditor.RichEditorStore store = new Inventec.Common.RichEditor.RichEditorStore(ApiConsumers.SarConsumer, ConfigSystems.URI_API_SAR, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetLanguage(), GlobalVariables.TemnplatePathFolder);
                        store.RunPrintTemplate(PrintTypeCodeStore.PRINT_TYPE_CODE__PhieuThuThanhToanKhac_MPS000299, InPhieuThuThanhToan);
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private bool InPhieuThuThanhToan(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                if (this.currentTransaction == null)
                    return result;

                HisBillGoodsFilter billGoodFilter = new HisBillGoodsFilter();
                billGoodFilter.BILL_ID = this.currentTransaction.ID;
                var listBillGoods = new BackendAdapter(new CommonParam()).Get<List<HIS_BILL_GOODS>>("api/HisBillGoods/Get", ApiConsumers.MosConsumer, billGoodFilter, null);

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((currentTransaction.TREATMENT_CODE ?? ""), printTypeCode, this.currentModuleBase != null ? currentModuleBase.RoomId : 0);

                MPS.Processor.Mps000299.PDO.Mps000299PDO pdo = new MPS.Processor.Mps000299.PDO.Mps000299PDO(
                    this.currentTransaction,
                    listBillGoods
                    );
                if (this.printNow)
                {
                    MPS.ProcessorBase.Core.PrintData printData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                    WaitingManager.Hide();
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                    if (result)
                    {
                        this.Close();
                    }
                }
                else
                {
                    MPS.ProcessorBase.Core.PrintData printData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    WaitingManager.Hide();
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO });
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private bool InPhieuThuThanhToanHoaDon(string printTypeCode, string fileName)
        {
            bool result = false;
            try
            {
                if (this.currentTransaction == null)
                    return result;

                HisBillGoodsFilter billGoodFilter = new HisBillGoodsFilter();
                billGoodFilter.BILL_ID = this.currentTransaction.ID;
                var listBillGoods = new BackendAdapter(new CommonParam()).Get<List<HIS_BILL_GOODS>>("api/HisBillGoods/Get", ApiConsumers.MosConsumer, billGoodFilter, null);

                string printerName = "";
                if (GlobalVariables.dicPrinter.ContainsKey(printTypeCode))
                {
                    printerName = GlobalVariables.dicPrinter[printTypeCode];
                }

                Inventec.Common.SignLibrary.ADO.InputADO inputADO = new HIS.Desktop.Plugins.Library.EmrGenerate.EmrGenerateProcessor().GenerateInputADOWithPrintTypeCode((currentTransaction.TREATMENT_CODE ?? ""), printTypeCode, this.currentModuleBase != null ? currentModuleBase.RoomId : 0);

                MPS.Processor.Mps000355.PDO.Mps000355PDO pdo = new MPS.Processor.Mps000355.PDO.Mps000355PDO(
                    this.currentTransaction,
                    listBillGoods
                    );
                if (this.printNow)
                {
                    MPS.ProcessorBase.Core.PrintData printData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, "");
                    WaitingManager.Hide();
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.PrintNow, printerName) { EmrInputADO = inputADO });
                    if (result)
                    {
                        this.Close();
                    }
                }
                else
                {
                    MPS.ProcessorBase.Core.PrintData printData = new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, "");
                    WaitingManager.Hide();
                    result = MPS.MpsPrinter.Run(new MPS.ProcessorBase.Core.PrintData(printTypeCode, fileName, pdo, MPS.ProcessorBase.PrintConfig.PreviewType.ShowDialog, printerName) { EmrInputADO = inputADO });
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        private void repositoryItemButtonDelete_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                HisBillGoodADO data = (HisBillGoodADO)gridViewBillGoods.GetFocusedRow();
                if (data != null)
                {
                    this.ListBillGood.Remove(data);
                    this.RefreshDataGridControl();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewBillGoods_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                if (e.Column.UnboundType != DevExpress.Data.UnboundColumnType.Bound)
                {
                    HisBillGoodADO data = (HisBillGoodADO)gridViewBillGoods.GetRow(e.ListSourceRowIndex);
                    if (data != null)
                    {
                        if (e.Column.FieldName == "STT")
                        {
                            e.Value = e.ListSourceRowIndex + 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewBillGoods_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                if (e.Column.FieldName == gridColumn_BillGood_Amount.FieldName
                    || e.Column.FieldName == gridColumn_BillGood_Discount.FieldName
                    || e.Column.FieldName == gridColumn_BillGood_GoodName.FieldName
                    || e.Column.FieldName == gridColumn_BillGood_GoodUnitName.FieldName
                    || e.Column.FieldName == gridColumn_BillGood_Price.FieldName)
                {
                    HisBillGoodADO data = (HisBillGoodADO)gridViewBillGoods.GetRow(e.RowHandle);
                    if (data != null)
                    {
                        this.ProcessRowChangeValue(data, e);
                        this.RefreshDataGridControl();
                        this.CaluTotalPrice();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessRowChangeValue(HisBillGoodADO data, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            try
            {
                data.TOTAL_PRICE = data.AMOUNT * data.PRICE;
                data.TOTAL_PRICE_WITH_DISCOUNT = data.TOTAL_PRICE - (data.DISCOUNT ?? 0);
                if (String.IsNullOrWhiteSpace(data.GOODS_NAME))
                {
                    data.Error = true;
                    data.ErrorText = String.Format(ResourceMessageManager.KhongDuocDeTrong, ResourceMessageManager.TenDichVu);
                    data.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                    data.ErrorColumnName = "GOODS_NAME";
                }
                else if (data.GOODS_NAME.Length >= 500)
                {
                    data.Error = true;
                    data.ErrorText = String.Format(ResourceMessageManager.DoDaiKhongDuocVuotQua, ResourceMessageManager.TenDichVu, 500);
                    data.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                    data.ErrorColumnName = "GOODS_NAME";
                }
                else if (!String.IsNullOrWhiteSpace(data.GOODS_UNIT_NAME) && data.GOODS_NAME.Length >= 500)
                {
                    data.Error = true;
                    data.ErrorText = String.Format(ResourceMessageManager.DoDaiKhongDuocVuotQua, ResourceMessageManager.DonViTinh, 500);
                    data.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                    data.ErrorColumnName = "GOODS_UNIT_NAME";
                }
                else if (data.AMOUNT <= 0)
                {
                    data.Error = true;
                    data.ErrorText = String.Format(ResourceMessageManager.KhongDuocBeHonHoacBang, ResourceMessageManager.SoLuong, 0);
                    data.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                    data.ErrorColumnName = "AMOUNT";
                }
                else if (data.PRICE <= 0)
                {
                    data.Error = true;
                    data.ErrorText = String.Format(ResourceMessageManager.KhongDuocBeHonHoacBang, ResourceMessageManager.DonGia, 0);
                    data.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                    data.ErrorColumnName = "PRICE";
                }
                else if (data.DISCOUNT < 0)
                {
                    data.Error = true;
                    data.ErrorText = String.Format(ResourceMessageManager.KhongDuocBeHonHoacBang, ResourceMessageManager.ChietKhau, 0);
                    data.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                    data.ErrorColumnName = "DISCOUNT";
                }
                else if (data.DISCOUNT > data.TOTAL_PRICE)
                {
                    data.Error = true;
                    data.ErrorText = String.Format(ResourceMessageManager.KhongDuocLonHon, ResourceMessageManager.ChietKhau, ResourceMessageManager.ThanhTien);
                    data.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning;
                    data.ErrorColumnName = "DISCOUNT";
                }
                else
                {
                    data.Error = false;
                    data.ErrorText = "";
                    data.ErrorType = DevExpress.XtraEditors.DXErrorProvider.ErrorType.None;
                    data.ErrorColumnName = "";
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void gridViewBillGoods_CustomRowColumnError(object sender, Inventec.Desktop.CustomControl.RowColumnErrorEventArgs e)
        {
            try
            {
                HisBillGoodADO data = (HisBillGoodADO)gridViewBillGoods.GetRow(e.RowHandle);
                if (data == null || !data.Error)
                {
                    e.Info.ErrorType = ErrorType.None;
                    e.Info.ErrorText = "";
                    return;
                }
                if (e.ColumnName == data.ErrorColumnName)
                {
                    e.Info.ErrorType = data.ErrorType;
                    e.Info.ErrorText = data.ErrorText;
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

        private void dxValidationProvider2_ValidationFailed(object sender, DevExpress.XtraEditors.DXErrorProvider.ValidationFailedEventArgs e)
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

        private void txtTreatmentCodeSearch_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnSearch.Focus();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SearchByTreatmentCode()
        {
            try
            {
                CommonParam param = new CommonParam();
                HisTreatmentFilter filter = new HisTreatmentFilter();
                if (!String.IsNullOrEmpty(txtTreatmentCodeSearch.Text))
                {
                    string code = txtTreatmentCodeSearch.Text.Trim();
                    if (code.Length < 12)
                    {
                        code = string.Format("{0:000000000000}", Convert.ToInt64(code));
                        txtTreatmentCodeSearch.Text = code;
                    }
                    filter.TREATMENT_CODE__EXACT = code;
                }
                var listTreatment = new BackendAdapter(param)
                        .Get<List<HIS_TREATMENT>>("api/HisTreatment/Get", ApiConsumers.MosConsumer, filter, param);
                if (listTreatment != null && listTreatment.Count == 1)
                {
                    treatment = listTreatment.FirstOrDefault();
                    treatmentId = treatment.ID;
                }
                else
                {
                    param.Messages.Add(ResourceMessageManager.KhongTimThayHoSoDieuTri);
                    return;
                }
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
                if (!btnSearch.Enabled)
                    return;
                WaitingManager.Show();
                this.treatment = null;
                this.treatmentId = 0;
                this.currentTransaction = null;
                this.patientTypeAlter = null;
                this.SearchByTreatmentCode();
                this.LoadPatientTypeAlter();
                this.SetDefaultCommonControl();
                this.SetDefautGoodControl();
                this.SetPatientInfo();
                this.SetDefaultAccountBook();
                this.SetDefaultPayForm();
                this.ResetControlValidationGood();
                this.RefreshDataGridControl();
                this.CaluTotalPrice();
                this.SetEnableButtonByTreatment();
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void SetEnableButtonByTreatment()
        {
            try
            {
                btnAdd.Enabled = true;
                btnNew.Enabled = true;
                btnPrint.Enabled = false;
                btnSave.Enabled = true;
                btnSaveAndPrint.Enabled = true;
                btnSaveAndSign.Enabled = true;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barBtnSaveAndPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnSaveAndPrint_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barBtnSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void barBtnPrint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void barBtnNew_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void barBtnSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (btnSearch.Enabled)
                    btnSearch_Click(null, null);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void barBtnAdd_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                btnAdd_Click(null, null);
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

        private void cboPayForm_EditValueChanged(object sender, ChangingEventArgs e)
        {

        }

        private void spinTotalAmount_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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

        private void SetDataNull()
        {
            try
            {
                txtTreatmentCodeSearch.Text = "";
                txtPatientName.Text = "";
                txtBuyerTaxCode.Text = "";
                txtBuyerAddress.Text = "";
                txtBuyerAccountNumber.Text = "";
                txtBuyerOrganization.Text = "";
                btnSearch.Enabled = false;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkCheckXD_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (chkCheckXD.Checked == true)
                {
                    SetDataNull();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnF2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                txtTreatmentCodeSearch.Focus();
                txtTreatmentCodeSearch.SelectAll();
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void chkCheckXD_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    txtPatientName.Focus();
                    txtPatientName.SelectAll();
                }
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtPatientName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void txtBuyerAddress_PreviewKeyDown_1(object sender, PreviewKeyDownEventArgs e)
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

        private void spinGoodDiscountRatio_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (spinGoodDiscountRatio.Value <= 0)
                    {
                        btnAdd.Focus();
                    }
                    else
                    {
                        txtGoodDescription.Focus();
                        txtGoodDescription.SelectAll();
                    }
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtGoodDescription_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    btnAdd.Focus();
                }
                e.Handled = true;
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


                        CommonParam param = new CommonParam();
                        MOS.Filter.HisAccountBookViewFilter hisAccountBookViewFilter = new HisAccountBookViewFilter();
                        hisAccountBookViewFilter.ID = accountBook.ID;
                        var accountBooks = new Inventec.Common.Adapter.BackendAdapter(param).Get<List<MOS.EFMODEL.DataModels.V_HIS_ACCOUNT_BOOK>>(HisRequestUriStore.HIS_ACCOUNT_BOOK_GETVIEW, ApiConsumer.ApiConsumers.MosConsumer, hisAccountBookViewFilter, param);
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
                            spinNumOrder.Value = (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook[accountBook.ID]) + 1;
                        }
                    }
                    else
                    {

                        spinNumOrder.Value = (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook[accountBook.ID]) + 1;
                    }
                }
                else
                {
                    spinNumOrder.Value = accountBook.CURRENT_NUM_ORDER ?? 0 + 1;

                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void spinNumOrder_Spin(object sender, SpinEventArgs e)
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

        private void UpdateDictionaryNumOrderAccountBook(V_HIS_ACCOUNT_BOOK accountBook)
        {
            try
            {
                if (HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook != null && HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.Count > 0 && HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook.ContainsKey(accountBook.ID))
                {
                    HIS.Desktop.LocalStorage.LocalData.GlobalVariables.dicNumOrderInAccountBook[accountBook.ID] = spinNumOrder.Value;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void txtPatientName_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void txtTreatmentCodeSearch_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (txtTreatmentCodeSearch.Text.Trim().Length > 0)
                    btnSearch.Enabled = true;
                else
                    btnSearch.Enabled = false;
            }
            catch (Exception ex)
            {

                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboNoneMediService_ButtonClick(object sender, ButtonPressedEventArgs e)
        {
            try
            {
                if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Plus)
                {
                    WaitingManager.Show();
                    List<object> listArgs = new List<object>();
                    listArgs.Add((RefeshReference)LoadDataToComboNoneMediService);
                    if (this.moduleData != null)
                    {
                        CallModule callModule = new CallModule(CallModule.HisNoneMediService, moduleData.RoomId, moduleData.RoomTypeId, listArgs);
                    }
                    else
                    {
                        CallModule callModule = new CallModule(CallModule.HisNoneMediService, 0, 0, listArgs);
                    }
                    WaitingManager.Hide();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void cboNoneMediService_Closed(object sender, ClosedEventArgs e)
        {
            try
            {
                if (e.CloseMode == PopupCloseMode.Normal)
                {
                    if (cboNoneMediService.EditValue != null && cboNoneMediService.EditValue != cboNoneMediService.OldEditValue)
                    {
                        currentNoneService = glstNoneMediService.Where(o => o.ID == Convert.ToInt64(cboNoneMediService.EditValue)).FirstOrDefault();
                        if (currentNoneService != null)
                        {
                            txtGoodUnitName.Enabled = false;
                            txtGoodUnitName.Text = currentNoneService.SERVICE_UNIT_NAME;
                            spinGoodPrice.Value = currentNoneService.PRICE ?? 0;
                            spinGoodAmount.Focus();
                        }
                    }
                    else
                    {
                        txtGoodUnitName.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void btnSaveAndSign_Click(object sender, EventArgs e)
        {
            try
            {
                if (!btnSaveAndSign.Enabled || !dxValidationProvider2.Validate())
                    return;

                if (chkCheckXD.Checked && (string.IsNullOrWhiteSpace(txtPatientName.Text) || string.IsNullOrWhiteSpace(txtBuyerAddress.Text)))
                {
                    if (string.IsNullOrWhiteSpace(txtPatientName.Text))
                    {
                        MessageBox.Show("Hóa đơn điện tử thiếu thông tin họ tên");
                        txtPatientName.Focus();
                        txtPatientName.SelectAll();
                    }
                    else if (string.IsNullOrWhiteSpace(txtBuyerAddress.Text))
                    {
                        MessageBox.Show("Hóa đơn điện tử thiếu thông tin địa chỉ");
                        txtBuyerAddress.Focus();
                        txtBuyerAddress.SelectAll();
                    }
                    return;
                }

                btnSaveAndPrint.Enabled = false;
                btnSave.Enabled = false;
                btnSaveAndSign.Enabled = false;
                printNow = false;
                CommonParam param = new CommonParam();
                WaitingManager.Show();
                bool success = this.ProcessSave(ref param);
                ResetControlValidationCommon();
                if (success)
                {
                    HIS_TRANSACTION tran = new HIS_TRANSACTION();
                    Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TRANSACTION>(tran, currentTransaction);
                    //tran.HIS_BILL_FUND = data.Transaction.HIS_BILL_FUND;
                    //Tao hoa don dien thu ben thu3 
                    ElectronicBillResult electronicBillResult = TaoHoaDonDienTuBenThu3CungCap(tran);
                    if (electronicBillResult == null || !electronicBillResult.Success)
                    {
                        param.Messages.Add("Tạo hóa đơn điện tử thất bại");
                        if (electronicBillResult.Messages != null && electronicBillResult.Messages.Count > 0)
                        {
                            param.Messages.AddRange(electronicBillResult.Messages);
                        }

                        param.Messages = param.Messages.Distinct().ToList();

                        //MessageManager.Show(this.ParentForm, param, success);
                    }
                    else
                    {
                        //goi api update
                        CommonParam paramUpdate = new CommonParam();
                        HisTransactionInvoiceInfoSDO sdo = new HisTransactionInvoiceInfoSDO();
                        sdo.EinvoiceLoginname = electronicBillResult.InvoiceLoginname;
                        sdo.InvoiceCode = electronicBillResult.InvoiceCode;
                        sdo.InvoiceSys = electronicBillResult.InvoiceSys;
                        sdo.EinvoiceNumOrder = electronicBillResult.InvoiceNumOrder;
                        sdo.EInvoiceTime = electronicBillResult.InvoiceTime ?? (Inventec.Common.DateTime.Get.Now() ?? 0);
                        sdo.Id = currentTransaction.ID;
                        sdo.InvoiceLookupCode = electronicBillResult.InvoiceLookupCode;
                        var apiResult = new BackendAdapter(paramUpdate).Post<bool>("api/HisTransaction/UpdateInvoiceInfo", ApiConsumers.MosConsumer, sdo, paramUpdate);
                        {
                            currentTransaction.INVOICE_CODE = electronicBillResult.InvoiceCode;
                            currentTransaction.INVOICE_SYS = electronicBillResult.InvoiceSys;
                            currentTransaction.EINVOICE_NUM_ORDER = electronicBillResult.InvoiceNumOrder;
                            currentTransaction.EINVOICE_LOGINNAME = electronicBillResult.InvoiceLoginname;
                            currentTransaction.EINVOICE_TIME = electronicBillResult.InvoiceTime ?? (Inventec.Common.DateTime.Get.Now() ?? 0);

                        }

                        if (!chkHideHddt.Checked)
                        {
                            Thread.Sleep(1000);
                            onClickInHoaDonDienTu(null, null);
                        }
                    }
                }
                else
                {
                    btnSaveAndPrint.Enabled = true;
                    btnSave.Enabled = true;
                    btnSaveAndSign.Enabled = true;
                }
                WaitingManager.Hide();
                MessageManager.Show(this.ParentForm, param, success);
            }
            catch (Exception ex)
            {
                WaitingManager.Hide();
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private ElectronicBillResult TaoHoaDonDienTuBenThu3CungCap(HIS_TRANSACTION transaction)
        {
            ElectronicBillResult result = new ElectronicBillResult();
            try
            {
                List<V_HIS_SERE_SERV_5> sereServBills = new List<V_HIS_SERE_SERV_5>();
                if (ListBillGood == null || ListBillGood.Count == 0)
                {
                    result.Success = false;
                    Inventec.Common.Logging.LogSystem.Debug("Khong co dich vu thanh toan nao duoc chon!");
                    return result;
                }

                foreach (var item in ListBillGood)
                {
                    V_HIS_SERE_SERV_5 sereServBill = new V_HIS_SERE_SERV_5();
                    sereServBill.SERVICE_ID = item.NONE_MEDI_SERVICE_ID ?? 0;
                    sereServBill.AMOUNT = item.AMOUNT;
                    sereServBill.VAT_RATIO = item.VAT_RATIO ?? 0;
                    sereServBill.TDL_SERVICE_CODE = "";
                    sereServBill.TDL_SERVICE_NAME = item.GOODS_NAME;
                    sereServBill.SERVICE_UNIT_NAME = item.GOODS_UNIT_NAME;
                    //sereServBill.PRICE = item.PRICE;
                    sereServBill.VIR_PRICE = item.PRICE - ((item.DISCOUNT ?? 0) / item.AMOUNT);
                    sereServBill.VIR_TOTAL_PATIENT_PRICE = sereServBill.VIR_PRICE * (1 + sereServBill.VAT_RATIO) * sereServBill.AMOUNT;

                    sereServBills.Add(sereServBill);
                }

                var currentTreatment = new V_HIS_TREATMENT_FEE();
                if (this.treatment != null && !chkCheckXD.Checked)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_TREATMENT_FEE>(currentTreatment, this.treatment);
                }
                else
                {
                    currentTreatment.ID = -1;//để các api trong thư viện không lấy được dữ liệu
                    currentTreatment.PATIENT_ID = -1;
                }

                currentTreatment.TDL_PATIENT_ACCOUNT_NUMBER = transaction.BUYER_ACCOUNT_NUMBER;
                currentTreatment.TDL_PATIENT_ADDRESS = transaction.BUYER_ADDRESS;
                currentTreatment.TDL_PATIENT_PHONE = transaction.BUYER_PHONE;
                currentTreatment.TDL_PATIENT_TAX_CODE = transaction.BUYER_TAX_CODE;
                currentTreatment.TDL_PATIENT_WORK_PLACE = transaction.BUYER_ORGANIZATION;
                currentTreatment.TDL_PATIENT_NAME = transaction.BUYER_NAME;

                ElectronicBillDataInput dataInput = new ElectronicBillDataInput();
                dataInput.Amount = transaction.AMOUNT;
                dataInput.Branch = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetBranchId());
                dataInput.Discount = spinExemption.Value;
                dataInput.DiscountRatio = spinExemptionRation.Value;
                dataInput.PaymentMethod = cboPayForm.Text;
                dataInput.SereServs = sereServBills;
                dataInput.Treatment = currentTreatment;
                dataInput.Currency = "VND";
                dataInput.Transaction = transaction;
                var accountBook = ListAccountBook.FirstOrDefault(o => o.ID == Convert.ToInt64(cboAccountBook.EditValue));
                if (accountBook != null)
                {
                    dataInput.SymbolCode = accountBook.SYMBOL_CODE;
                    dataInput.TemplateCode = accountBook.TEMPLATE_CODE;
                    dataInput.EinvoiceTypeId = accountBook.EINVOICE_TYPE_ID;
                }

                if (dtTransactionTime.EditValue != null && dtTransactionTime.DateTime != DateTime.MinValue)
                {
                    dataInput.TransactionTime = Convert.ToInt64(dtTransactionTime.DateTime.ToString("yyyyMMddHHmmss"));
                }

                WaitingManager.Show();
                ElectronicBillProcessor electronicBillProcessor = new ElectronicBillProcessor(dataInput);
                result = electronicBillProcessor.Run(ElectronicBillType.ENUM.CREATE_INVOICE);
                WaitingManager.Hide();
            }
            catch (Exception ex)
            {
                result.Success = false;
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return result;
        }

        private void barBtnSaveAndSign_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
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

        private void onClickInHoaDonDienTu(object sender, EventArgs e)
        {
            try
            {
                if (this.currentTransaction == null || String.IsNullOrEmpty(this.currentTransaction.INVOICE_CODE))
                {
                    //MessageBox.Show("Hóa đơn chưa thanh toán hoặc chưa cấu hình hóa đơn điện tử.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                ElectronicBillDataInput dataInput = new ElectronicBillDataInput();
                dataInput.PartnerInvoiceID = Inventec.Common.TypeConvert.Parse.ToInt64(this.currentTransaction.INVOICE_CODE);
                dataInput.InvoiceCode = currentTransaction.INVOICE_CODE;
                dataInput.NumOrder = currentTransaction.NUM_ORDER;
                dataInput.SymbolCode = currentTransaction.SYMBOL_CODE;
                dataInput.TemplateCode = currentTransaction.TEMPLATE_CODE;
                dataInput.TransactionTime = currentTransaction.EINVOICE_TIME ?? currentTransaction.TRANSACTION_TIME;
                dataInput.ENumOrder = currentTransaction.EINVOICE_NUM_ORDER;
                dataInput.EinvoiceTypeId = currentTransaction.EINVOICE_TYPE_ID;
                HIS_TRANSACTION tran = new HIS_TRANSACTION();
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_TRANSACTION>(tran, currentTransaction);
                dataInput.Transaction = tran;
                var currentTreatment = new V_HIS_TREATMENT_FEE();
                if (this.treatment != null && !chkCheckXD.Checked)
                {
                    Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_TREATMENT_FEE>(currentTreatment, this.treatment);
                }
                dataInput.Treatment = currentTreatment;
                dataInput.SereServs = new List<V_HIS_SERE_SERV_5>();

                dataInput.Branch = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<HIS_BRANCH>().FirstOrDefault(o => o.ID == HIS.Desktop.LocalStorage.LocalData.WorkPlace.GetBranchId());
                ElectronicBillProcessor electronicBillProcessor = new ElectronicBillProcessor(dataInput);
                ElectronicBillResult electronicBillResult = null;
                electronicBillResult = electronicBillProcessor.Run(ElectronicBillType.ENUM.GET_INVOICE_LINK);

                if (electronicBillResult == null || String.IsNullOrEmpty(electronicBillResult.InvoiceLink))
                {
                    if (electronicBillResult != null && electronicBillResult.Messages != null && electronicBillResult.Messages.Count > 0)
                    {
                        MessageBox.Show("Tải hóa đơn điện tử thất bại. " + string.Join(". ", electronicBillResult.Messages));
                    }
                    else
                        MessageBox.Show("Không tìm thấy link hóa đơn điện tử");
                    return;
                }

                Inventec.Common.DocumentViewer.DocumentViewerManager viewManager = new Inventec.Common.DocumentViewer.DocumentViewerManager(ViewType.ENUM.Pdf);
                viewManager.Run(electronicBillResult.InvoiceLink);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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
                HIS.Desktop.Library.CacheClient.ControlStateRDO csAddOrUpdate = (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0) ? this.currentControlStateRDO.Where(o => o.KEY == chkHideHddt.Name && o.MODULE_LINK == moduleData.ModuleLink).FirstOrDefault() : null;
                if (csAddOrUpdate != null)
                {
                    csAddOrUpdate.VALUE = (chkHideHddt.Checked ? "1" : "");
                }
                else
                {
                    csAddOrUpdate = new HIS.Desktop.Library.CacheClient.ControlStateRDO();
                    csAddOrUpdate.KEY = chkHideHddt.Name;
                    csAddOrUpdate.VALUE = (chkHideHddt.Checked ? "1" : "");
                    csAddOrUpdate.MODULE_LINK = moduleData.ModuleLink;
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
                this.currentControlStateRDO = controlStateWorker.GetData(moduleData.ModuleLink);
                if (this.currentControlStateRDO != null && this.currentControlStateRDO.Count > 0)
                {
                    foreach (var item in this.currentControlStateRDO)
                    {
                        if (item.KEY == chkHideHddt.Name)
                        {
                            chkHideHddt.Checked = item.VALUE == "1";
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
    }
}





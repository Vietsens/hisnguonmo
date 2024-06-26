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
using HIS.Desktop.Plugins.InvoiceCreate.ADO;
using HIS.Desktop.Plugins.InvoiceCreate.Validation;
using Inventec.Common.Adapter;
using Inventec.Common.ElectronicBill;
using Inventec.Core;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using MOS.Filter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.InvoiceCreate
{
    public partial class frmInvoiceCreate : HIS.Desktop.Utility.FormBase
    {
        Inventec.Desktop.Common.Modules.Module currentModule;

        HIS_BRANCH branch = null;

        List<V_HIS_USER_INVOICE_BOOK> listInvoiceBook = new List<V_HIS_USER_INVOICE_BOOK>();
        HIS_INVOICE resultInvoice = null;

        List<HisInvoiceDetailADO> ListInvoiceDetail = new List<HisInvoiceDetailADO>();

        decimal totalPatientPrice = 0;

        int positionHandleControl = -1;

        public frmInvoiceCreate(Inventec.Desktop.Common.Modules.Module module)
            : base(module)
        {
            InitializeComponent();
            try
            {
                Base.ResourceLangManager.InitResourceLanguageManager();
                this.SetIcon();
                this.currentModule = module;
                if (this.currentModule != null)
                {
                    this.Text = this.currentModule.text;
                }
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

        private void frmInvoiceCreate_Load(object sender, EventArgs e)
        {
            try
            {
                WaitingManager.Show();
                this.LoadKeyFrmLanguage();
                this.LoadInvoiceBook();
                this.ValidControl();
                this.LoadDataToComboInvoiceBook();
                this.LoadDataToComboPayForm();
                this.ResetControlValue();
                CalcuTotalPrice();
                LoadSellerInfo();
                LoadBuyerInfo();
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

        private void ResetControlValue()
        {
            try
            {
                resultInvoice = null;
                SetDefaultPayForm();
                txtTemplateCode.Text = "";
                cboInvoiceBook.EditValue = null;
                txtTongTuDen.Text = "";
                SetDefaultInvoiceBook();
                ListInvoiceDetail = new List<HisInvoiceDetailADO>();
                bindingSource1.DataSource = ListInvoiceDetail;
                gridControlInvoiceDetail.DataSource = this.bindingSource1;
                txtAmount.Value = 0;
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

        private void SetDefaultInvoiceBook()
        {
            try
            {
                if (listInvoiceBook != null && listInvoiceBook.Count == 1)
                {
                    var invoiceBook = listInvoiceBook.First();
                    cboInvoiceBook.EditValue = invoiceBook.INVOICE_BOOK_ID;
                    txtTongTuDen.Text = invoiceBook.TOTAL + "/" + invoiceBook.FROM_NUM_ORDER + "/" + Math.Round(invoiceBook.CURRENT_NUM_ORDER ?? 0, 0);
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
                string code = String.IsNullOrEmpty(ConfigApplicationWorker.Get<string>(Config.AppConfig.HFS_KEY__PAY_FORM_CODE)) ? GlobalVariables.HIS_PAY_FORM_CODE__CONSTANT : ConfigApplicationWorker.Get<string>(Config.AppConfig.HFS_KEY__PAY_FORM_CODE);
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
                var listInvoiceDetail = bindingSource1.DataSource as List<HisInvoiceDetailADO>;
                if (listInvoiceDetail != null && listInvoiceDetail.Count > 0)
                {
                    this.totalPatientPrice = listInvoiceDetail.Sum(s => s.TOTAL_PRICE);
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
                txtBuyerAccountNumber.Text = "";
                txtBuyerAddress.Text = "";
                txtBuyerDescription.Text = "";
                txtBuyerName.Text = "";
                txtBuyerOrganization.Text = "";
                txtBuyerTaxCode.Text = "";
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
                var lang = Base.ResourceLangManager.LanguageFrmInvoiceCreate;
                //Button
                this.btnCreateElectricInvoice.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE__BTN_CREATE_ELECTRICT_BILL", lang, cul);
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE__BTN_SAVE", lang, cul);
                this.btnNew.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE__BTN_NEW", lang, cul);
                this.btnPrint.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE__BTN_PRINT", lang, cul);

                //layout
                this.layoutAmount.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE__LAYOUT_AMOUNT", lang, cul);
                this.layoutBuyerAccountNumber.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE__LAYOUT_BUYER_ACCOUNT_NUMBER", lang, cul);
                this.layoutBuyerAddress.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE__LAYOUT_BUYER_ADDRESS", lang, cul);
                this.layoutBuyerDescription.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE__LAYOUT_BUYER_DESCRIPTION", lang, cul);
                this.layoutBuyerName.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE__LAYOUT_BUYER_NAME", lang, cul);
                this.layoutBuyerOrganization.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE__LAYOUT_BUYER_ORGANIZATION", lang, cul);
                this.layoutBuyerTaxCode.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE__LAYOUT_BUYER_TAX_CODE", lang, cul);
                this.layoutControlBuyerInfo.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE__LAYOUT_BUYER_INFO", lang, cul);
                this.layoutControlSellerInfo.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE__LAYOUT_SELLER_INFO", lang, cul);
                this.layoutDiscount.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE__LAYOUT_DISCOUNT", lang, cul);
                this.layoutInvoiceBook.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE__LAYOUT_INVOICE_BOOK", lang, cul);
                this.layoutInvoiceTime.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE__LAYOUT_INVOICE_TIME", lang, cul);
                this.layoutNumOrder.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE__LAYOUT_NUM_ORDER", lang, cul);
                this.layoutPayForm.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE__LAYOUT_PAY_FORM", lang, cul);
                this.layoutSellerAccountNumber.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE__LAYOUT_SELLER_ACCOUNT_NUMBER", lang, cul);
                this.layoutSellerAddress.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE__LAYOUT_SELLER_ADDRESS", lang, cul);
                this.layoutSellerName.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE__LAYOUT_SELLER_NAME", lang, cul);
                this.layoutSellerPhone.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE__LAYOUT_SELLER_PHONE", lang, cul);
                this.layoutSellerTaxCode.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE__LAYOUT_SELLER_TAX_CODE", lang, cul);
                this.layoutTongTuDen.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE__LAYOUT_TONG_TU_DEN", lang, cul);
                this.layoutVatRatio.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE__LAYOUT_VAT_RATIO", lang, cul);

                //grid InvoiceDetail
                this.gridColumn_InvoiceDetail_Amount.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE__GRID__INVOICE_DETAIL__COLUMN_AMOUNT", lang, cul);
                this.gridColumn_InvoiceDetail_Discount.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE__GRID__INVOICE_DETAIL__COLUMN_DISCOUNT", lang, cul);
                this.gridColumn_InvoiceDetail_GoodsName.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE__GRID__INVOICE_DETAIL__COLUMN_GOODS_NAME", lang, cul);
                this.gridColumn_InvoiceDetail_GoodsUnit.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE__GRID__INVOICE_DETAIL__COLUMN_GOODS_UNIT", lang, cul);
                this.gridColumn_InvoiceDetail_Price.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE__GRID__INVOICE_DETAIL__COLUMN_PRICE", lang, cul);
                this.gridColumn_InvoiceDetail_TotalPrice.Caption = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE__GRID__INVOICE_DETAIL__COLUMN_TOTAL_PRICE", lang, cul);

                //Repository
                this.repositoryItemBtnDelete.Buttons[0].ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__FRM_INVOICE_CREATE__REPOSITORY__BTN_DELETE", lang, cul);

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

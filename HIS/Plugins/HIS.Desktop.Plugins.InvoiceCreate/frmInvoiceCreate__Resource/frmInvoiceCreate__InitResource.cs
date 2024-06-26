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
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.InvoiceCreate.Resources.Lang", typeof(HIS.Desktop.Plugins.InvoiceCreate.frmInvoiceCreate__Resource).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmInvoiceCreate.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnCreateElectricInvoice.Text = Inventec.Common.Resource.Get.Value("frmInvoiceCreate.btnCreateElectricInvoice.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_InvoiceDetail_GoodsName.Caption = Inventec.Common.Resource.Get.Value("frmInvoiceCreate.gridColumn_InvoiceDetail_GoodsName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_InvoiceDetail_GoodsUnit.Caption = Inventec.Common.Resource.Get.Value("frmInvoiceCreate.gridColumn_InvoiceDetail_GoodsUnit.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_InvoiceDetail_Price.Caption = Inventec.Common.Resource.Get.Value("frmInvoiceCreate.gridColumn_InvoiceDetail_Price.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_InvoiceDetail_Amount.Caption = Inventec.Common.Resource.Get.Value("frmInvoiceCreate.gridColumn_InvoiceDetail_Amount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_InvoiceDetail_Discount.Caption = Inventec.Common.Resource.Get.Value("frmInvoiceCreate.gridColumn_InvoiceDetail_Discount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_InvoiceDetail_TotalPrice.Caption = Inventec.Common.Resource.Get.Value("frmInvoiceCreate.gridColumn_InvoiceDetail_TotalPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_InvoiceDetail_Delete.Caption = Inventec.Common.Resource.Get.Value("frmInvoiceCreate.gridColumn_InvoiceDetail_Delete.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnPrint.Text = Inventec.Common.Resource.Get.Value("frmInvoiceCreate.btnPrint.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnNew.Text = Inventec.Common.Resource.Get.Value("frmInvoiceCreate.btnNew.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmInvoiceCreate.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("frmInvoiceCreate.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlSellerInfo.Text = Inventec.Common.Resource.Get.Value("frmInvoiceCreate.layoutControlSellerInfo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutSellerName.Text = Inventec.Common.Resource.Get.Value("frmInvoiceCreate.layoutSellerName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutSellerTaxCode.Text = Inventec.Common.Resource.Get.Value("frmInvoiceCreate.layoutSellerTaxCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutSellerPhone.Text = Inventec.Common.Resource.Get.Value("frmInvoiceCreate.layoutSellerPhone.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutSellerAccountNumber.Text = Inventec.Common.Resource.Get.Value("frmInvoiceCreate.layoutSellerAccountNumber.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutSellerAddress.Text = Inventec.Common.Resource.Get.Value("frmInvoiceCreate.layoutSellerAddress.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmInvoiceCreate.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlBuyerInfo.Text = Inventec.Common.Resource.Get.Value("frmInvoiceCreate.layoutControlBuyerInfo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutBuyerName.Text = Inventec.Common.Resource.Get.Value("frmInvoiceCreate.layoutBuyerName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutBuyerTaxCode.Text = Inventec.Common.Resource.Get.Value("frmInvoiceCreate.layoutBuyerTaxCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutBuyerAccountNumber.Text = Inventec.Common.Resource.Get.Value("frmInvoiceCreate.layoutBuyerAccountNumber.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutBuyerOrganization.Text = Inventec.Common.Resource.Get.Value("frmInvoiceCreate.layoutBuyerOrganization.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutBuyerAddress.Text = Inventec.Common.Resource.Get.Value("frmInvoiceCreate.layoutBuyerAddress.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutBuyerDescription.Text = Inventec.Common.Resource.Get.Value("frmInvoiceCreate.layoutBuyerDescription.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboPayForm.Properties.NullText = Inventec.Common.Resource.Get.Value("frmInvoiceCreate.cboPayForm.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboInvoiceBook.Properties.NullText = Inventec.Common.Resource.Get.Value("frmInvoiceCreate.cboInvoiceBook.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutInvoiceBook.Text = Inventec.Common.Resource.Get.Value("frmInvoiceCreate.layoutInvoiceBook.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutTongTuDen.Text = Inventec.Common.Resource.Get.Value("frmInvoiceCreate.layoutTongTuDen.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutInvoiceTime.Text = Inventec.Common.Resource.Get.Value("frmInvoiceCreate.layoutInvoiceTime.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutAmount.Text = Inventec.Common.Resource.Get.Value("frmInvoiceCreate.layoutAmount.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutDiscount.Text = Inventec.Common.Resource.Get.Value("frmInvoiceCreate.layoutDiscount.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutNumOrder.Text = Inventec.Common.Resource.Get.Value("frmInvoiceCreate.layoutNumOrder.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutPayForm.Text = Inventec.Common.Resource.Get.Value("frmInvoiceCreate.layoutPayForm.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutVatRatio.Text = Inventec.Common.Resource.Get.Value("frmInvoiceCreate.layoutVatRatio.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmInvoiceCreate.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnRCSave.Caption = Inventec.Common.Resource.Get.Value("frmInvoiceCreate.bbtnRCSave.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnRCPrint.Caption = Inventec.Common.Resource.Get.Value("frmInvoiceCreate.bbtnRCPrint.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnRCNew.Caption = Inventec.Common.Resource.Get.Value("frmInvoiceCreate.bbtnRCNew.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmInvoiceCreate.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

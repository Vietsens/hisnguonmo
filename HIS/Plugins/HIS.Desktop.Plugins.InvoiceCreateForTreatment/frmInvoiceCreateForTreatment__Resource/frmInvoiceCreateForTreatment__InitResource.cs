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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.InvoiceCreateForTreatment.Resources.Lang", typeof(HIS.Desktop.Plugins.InvoiceCreateForTreatment.frmInvoiceCreateForTreatment__Resource).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmInvoiceCreateForTreatment.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnCreateElectricInvoice.Text = Inventec.Common.Resource.Get.Value("frmInvoiceCreateForTreatment.btnCreateElectricInvoice.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnPrint.Text = Inventec.Common.Resource.Get.Value("frmInvoiceCreateForTreatment.btnPrint.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmInvoiceCreateForTreatment.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnRCSave.Caption = Inventec.Common.Resource.Get.Value("frmInvoiceCreateForTreatment.bbtnRCSave.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnRCNew.Caption = Inventec.Common.Resource.Get.Value("frmInvoiceCreateForTreatment.bbtnRCNew.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnRCPrint.Caption = Inventec.Common.Resource.Get.Value("frmInvoiceCreateForTreatment.bbtnRCPrint.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem1.Caption = Inventec.Common.Resource.Get.Value("frmInvoiceCreateForTreatment.barButtonItem1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnNew.Text = Inventec.Common.Resource.Get.Value("frmInvoiceCreateForTreatment.btnNew.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmInvoiceCreateForTreatment.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("frmInvoiceCreateForTreatment.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlSellerInfo.Text = Inventec.Common.Resource.Get.Value("frmInvoiceCreateForTreatment.layoutControlSellerInfo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutSellerName.Text = Inventec.Common.Resource.Get.Value("frmInvoiceCreateForTreatment.layoutSellerName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutSellerTaxCode.Text = Inventec.Common.Resource.Get.Value("frmInvoiceCreateForTreatment.layoutSellerTaxCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutSellerPhone.Text = Inventec.Common.Resource.Get.Value("frmInvoiceCreateForTreatment.layoutSellerPhone.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutSellerAccountNumber.Text = Inventec.Common.Resource.Get.Value("frmInvoiceCreateForTreatment.layoutSellerAccountNumber.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutSellerAddress.Text = Inventec.Common.Resource.Get.Value("frmInvoiceCreateForTreatment.layoutSellerAddress.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmInvoiceCreateForTreatment.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlBuyerInfo.Text = Inventec.Common.Resource.Get.Value("frmInvoiceCreateForTreatment.layoutControlBuyerInfo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutBuyerName.Text = Inventec.Common.Resource.Get.Value("frmInvoiceCreateForTreatment.layoutBuyerName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutBuyerTaxCode.Text = Inventec.Common.Resource.Get.Value("frmInvoiceCreateForTreatment.layoutBuyerTaxCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutBuyerAccountNumber.Text = Inventec.Common.Resource.Get.Value("frmInvoiceCreateForTreatment.layoutBuyerAccountNumber.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutBuyerOrganization.Text = Inventec.Common.Resource.Get.Value("frmInvoiceCreateForTreatment.layoutBuyerOrganization.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutBuyerAddress.Text = Inventec.Common.Resource.Get.Value("frmInvoiceCreateForTreatment.layoutBuyerAddress.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutBuyerDescription.Text = Inventec.Common.Resource.Get.Value("frmInvoiceCreateForTreatment.layoutBuyerDescription.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboPayForm.Properties.NullText = Inventec.Common.Resource.Get.Value("frmInvoiceCreateForTreatment.cboPayForm.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboInvoiceBook.Properties.NullText = Inventec.Common.Resource.Get.Value("frmInvoiceCreateForTreatment.cboInvoiceBook.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutInvoiceBook.Text = Inventec.Common.Resource.Get.Value("frmInvoiceCreateForTreatment.layoutInvoiceBook.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutTongTuDen.Text = Inventec.Common.Resource.Get.Value("frmInvoiceCreateForTreatment.layoutTongTuDen.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutInvoiceTime.Text = Inventec.Common.Resource.Get.Value("frmInvoiceCreateForTreatment.layoutInvoiceTime.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutAmount.Text = Inventec.Common.Resource.Get.Value("frmInvoiceCreateForTreatment.layoutAmount.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutDiscount.Text = Inventec.Common.Resource.Get.Value("frmInvoiceCreateForTreatment.layoutDiscount.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutNumOrder.Text = Inventec.Common.Resource.Get.Value("frmInvoiceCreateForTreatment.layoutNumOrder.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutPayForm.Text = Inventec.Common.Resource.Get.Value("frmInvoiceCreateForTreatment.layoutPayForm.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutVatRatio.Text = Inventec.Common.Resource.Get.Value("frmInvoiceCreateForTreatment.layoutVatRatio.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmInvoiceCreateForTreatment.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

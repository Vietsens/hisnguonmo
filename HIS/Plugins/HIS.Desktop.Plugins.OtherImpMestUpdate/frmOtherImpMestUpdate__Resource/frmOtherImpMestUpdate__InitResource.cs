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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.OtherImpMestUpdate.Resources.Lang", typeof(HIS.Desktop.Plugins.OtherImpMestUpdate.frmOtherImpMestUpdate__Resource).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmOtherImpMestUpdate.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmOtherImpMestUpdate.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnUpdate.Text = Inventec.Common.Resource.Get.Value("frmOtherImpMestUpdate.btnUpdate.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnCancel.Text = Inventec.Common.Resource.Get.Value("frmOtherImpMestUpdate.btnCancel.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnPrint.Text = Inventec.Common.Resource.Get.Value("frmOtherImpMestUpdate.btnPrint.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmOtherImpMestUpdate.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkSellByImpPrice.Properties.Caption = Inventec.Common.Resource.Get.Value("frmOtherImpMestUpdate.chkSellByImpPrice.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboImpSource.Properties.NullText = Inventec.Common.Resource.Get.Value("frmOtherImpMestUpdate.cboImpSource.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboMediStock.Properties.NullText = Inventec.Common.Resource.Get.Value("frmOtherImpMestUpdate.cboMediStock.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboImpMestType.Properties.NullText = Inventec.Common.Resource.Get.Value("frmOtherImpMestUpdate.cboImpMestType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ServicePaty_PatientTypeName.Caption = Inventec.Common.Resource.Get.Value("frmOtherImpMestUpdate.gridColumn_ServicePaty_PatientTypeName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ServicePaty_ExpPrice.Caption = Inventec.Common.Resource.Get.Value("frmOtherImpMestUpdate.gridColumn_ServicePaty_ExpPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ServicePaty_VatRatio.Caption = Inventec.Common.Resource.Get.Value("frmOtherImpMestUpdate.gridColumn_ServicePaty_VatRatio.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ServicePaty_PriceAndVat.Caption = Inventec.Common.Resource.Get.Value("frmOtherImpMestUpdate.gridColumn_ServicePaty_PriceAndVat.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ServicePaty_NotSell.Caption = Inventec.Common.Resource.Get.Value("frmOtherImpMestUpdate.gridColumn_ServicePaty_NotSell.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPageMedicine.Text = Inventec.Common.Resource.Get.Value("frmOtherImpMestUpdate.xtraTabPageMedicine.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPageMaterial.Text = Inventec.Common.Resource.Get.Value("frmOtherImpMestUpdate.xtraTabPageMaterial.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ImpMestDetail_Stt.Caption = Inventec.Common.Resource.Get.Value("frmOtherImpMestUpdate.gridColumn_ImpMestDetail_Stt.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ImpMestDetail_Delete.Caption = Inventec.Common.Resource.Get.Value("frmOtherImpMestUpdate.gridColumn_ImpMestDetail_Delete.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ImpMestDetail_Edit.Caption = Inventec.Common.Resource.Get.Value("frmOtherImpMestUpdate.gridColumn_ImpMestDetail_Edit.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ImpMestDetail_TypeName.Caption = Inventec.Common.Resource.Get.Value("frmOtherImpMestUpdate.gridColumn_ImpMestDetail_TypeName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ImpMestDetail_Amount.Caption = Inventec.Common.Resource.Get.Value("frmOtherImpMestUpdate.gridColumn_ImpMestDetail_Amount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ImpMestDetail_Price.Caption = Inventec.Common.Resource.Get.Value("frmOtherImpMestUpdate.gridColumn_ImpMestDetail_Price.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ImpMestDetail_ImpVatRatio.Caption = Inventec.Common.Resource.Get.Value("frmOtherImpMestUpdate.gridColumn_ImpMestDetail_ImpVatRatio.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ImpMestDetail_ExpiredDate.Caption = Inventec.Common.Resource.Get.Value("frmOtherImpMestUpdate.gridColumn_ImpMestDetail_ExpiredDate.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ImpMestDetail_PackageNumber.Caption = Inventec.Common.Resource.Get.Value("frmOtherImpMestUpdate.gridColumn_ImpMestDetail_PackageNumber.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciAmount.Text = Inventec.Common.Resource.Get.Value("frmOtherImpMestUpdate.lciAmount.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPriceVat.Text = Inventec.Common.Resource.Get.Value("frmOtherImpMestUpdate.lciPriceVat.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPackageNumber.Text = Inventec.Common.Resource.Get.Value("frmOtherImpMestUpdate.lciPackageNumber.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciDescriptionPaty.Text = Inventec.Common.Resource.Get.Value("frmOtherImpMestUpdate.lciDescriptionPaty.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciImpMestType.Text = Inventec.Common.Resource.Get.Value("frmOtherImpMestUpdate.lciImpMestType.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciMediStock.Text = Inventec.Common.Resource.Get.Value("frmOtherImpMestUpdate.lciMediStock.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciImpSource.Text = Inventec.Common.Resource.Get.Value("frmOtherImpMestUpdate.lciImpSource.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciImpPrice.Text = Inventec.Common.Resource.Get.Value("frmOtherImpMestUpdate.lciImpPrice.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciVat.Text = Inventec.Common.Resource.Get.Value("frmOtherImpMestUpdate.lciVat.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTotalVatPrice.Text = Inventec.Common.Resource.Get.Value("frmOtherImpMestUpdate.lciTotalVatPrice.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTotalPrice.Text = Inventec.Common.Resource.Get.Value("frmOtherImpMestUpdate.lciTotalPrice.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciTotalFeePrice.Text = Inventec.Common.Resource.Get.Value("frmOtherImpMestUpdate.lciTotalFeePrice.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciExpiredDate.Text = Inventec.Common.Resource.Get.Value("frmOtherImpMestUpdate.lciExpiredDate.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciDescription.Text = Inventec.Common.Resource.Get.Value("frmOtherImpMestUpdate.lciDescription.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmOtherImpMestUpdate.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItemAdd.Caption = Inventec.Common.Resource.Get.Value("frmOtherImpMestUpdate.barButtonItemAdd.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItemCancel.Caption = Inventec.Common.Resource.Get.Value("frmOtherImpMestUpdate.barButtonItemCancel.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItemNew.Caption = Inventec.Common.Resource.Get.Value("frmOtherImpMestUpdate.barButtonItemNew.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItemPrint.Caption = Inventec.Common.Resource.Get.Value("frmOtherImpMestUpdate.barButtonItemPrint.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItemSave.Caption = Inventec.Common.Resource.Get.Value("frmOtherImpMestUpdate.barButtonItemSave.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItemUpdate.Caption = Inventec.Common.Resource.Get.Value("frmOtherImpMestUpdate.barButtonItemUpdate.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmOtherImpMestUpdate.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

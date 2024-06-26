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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("Plugins.Resources.Lang", typeof(Plugins.HIS.Desktop.Plugins.BrowseExportTicket.frmBrowseExportTicket__Resource).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmBrowseExportTicket.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmBrowseExportTicket.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage1.Text = Inventec.Common.Resource.Get.Value("frmBrowseExportTicket.xtraTabPage1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("frmBrowseExportTicket.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmBrowseExportTicket.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnSTT.Caption = Inventec.Common.Resource.Get.Value("frmBrowseExportTicket.gridColumnSTT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnMedicineTypeCode.Caption = Inventec.Common.Resource.Get.Value("frmBrowseExportTicket.gridColumnMedicineTypeCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("frmBrowseExportTicket.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColMedicineTypeName.Caption = Inventec.Common.Resource.Get.Value("frmBrowseExportTicket.gridColMedicineTypeName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnServiceUnitName.Caption = Inventec.Common.Resource.Get.Value("frmBrowseExportTicket.gridColumnServiceUnitName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnIsExpend.Caption = Inventec.Common.Resource.Get.Value("frmBrowseExportTicket.gridColumnIsExpend.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.repositoryItemPictureEdit1.NullText = Inventec.Common.Resource.Get.Value("frmBrowseExportTicket.repositoryItemPictureEdit1.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnAmount.Caption = Inventec.Common.Resource.Get.Value("frmBrowseExportTicket.gridColumnAmount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnSumInStock.Caption = Inventec.Common.Resource.Get.Value("frmBrowseExportTicket.gridColumnSumInStock.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnImpPrice.Caption = Inventec.Common.Resource.Get.Value("frmBrowseExportTicket.gridColumnImpPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnImpVatRatio.Caption = Inventec.Common.Resource.Get.Value("frmBrowseExportTicket.gridColumnImpVatRatio.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnPrice.Caption = Inventec.Common.Resource.Get.Value("frmBrowseExportTicket.gridColumnPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnExpVatRatio.Caption = Inventec.Common.Resource.Get.Value("frmBrowseExportTicket.gridColumnExpVatRatio.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnDiscount.Caption = Inventec.Common.Resource.Get.Value("frmBrowseExportTicket.gridColumnDiscount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnTotalPrice.Caption = Inventec.Common.Resource.Get.Value("frmBrowseExportTicket.gridColumnTotalPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnDescription.Caption = Inventec.Common.Resource.Get.Value("frmBrowseExportTicket.gridColumnDescription.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage2.Text = Inventec.Common.Resource.Get.Value("frmBrowseExportTicket.xtraTabPage2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("frmBrowseExportTicket.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("frmBrowseExportTicket.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnMateStt.Caption = Inventec.Common.Resource.Get.Value("frmBrowseExportTicket.gridColumnMateStt.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnMateMaterialTypeName.Caption = Inventec.Common.Resource.Get.Value("frmBrowseExportTicket.gridColumnMateMaterialTypeName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnMateMaterialTypeCode.Caption = Inventec.Common.Resource.Get.Value("frmBrowseExportTicket.gridColumnMateMaterialTypeCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnMateServiceUnitName.Caption = Inventec.Common.Resource.Get.Value("frmBrowseExportTicket.gridColumnMateServiceUnitName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnMateIsExpend.Caption = Inventec.Common.Resource.Get.Value("frmBrowseExportTicket.gridColumnMateIsExpend.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.repositoryItemPictureEdit2.NullText = Inventec.Common.Resource.Get.Value("frmBrowseExportTicket.repositoryItemPictureEdit2.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnMateAmount.Caption = Inventec.Common.Resource.Get.Value("frmBrowseExportTicket.gridColumnMateAmount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnMateTotalAmount.Caption = Inventec.Common.Resource.Get.Value("frmBrowseExportTicket.gridColumnMateTotalAmount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnMateImpPrice.Caption = Inventec.Common.Resource.Get.Value("frmBrowseExportTicket.gridColumnMateImpPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnMateImpVatRatio.Caption = Inventec.Common.Resource.Get.Value("frmBrowseExportTicket.gridColumnMateImpVatRatio.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnMatePrice.Caption = Inventec.Common.Resource.Get.Value("frmBrowseExportTicket.gridColumnMatePrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnMateDiscount.Caption = Inventec.Common.Resource.Get.Value("frmBrowseExportTicket.gridColumnMateDiscount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnMateExpVat.Caption = Inventec.Common.Resource.Get.Value("frmBrowseExportTicket.gridColumnMateExpVat.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnMateTotalPrice.Caption = Inventec.Common.Resource.Get.Value("frmBrowseExportTicket.gridColumnMateTotalPrice.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnMateDescripTion.Caption = Inventec.Common.Resource.Get.Value("frmBrowseExportTicket.gridColumnMateDescripTion.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage3.Text = Inventec.Common.Resource.Get.Value("frmBrowseExportTicket.xtraTabPage3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmBrowseExportTicket.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_Add.Caption = Inventec.Common.Resource.Get.Value("frmBrowseExportTicket.gridColumn_Add.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_Remover.Caption = Inventec.Common.Resource.Get.Value("frmBrowseExportTicket.gridColumn_Remover.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_BloodTypeName.Caption = Inventec.Common.Resource.Get.Value("frmBrowseExportTicket.gridColumn_BloodTypeName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_BloodTypeCode.Caption = Inventec.Common.Resource.Get.Value("frmBrowseExportTicket.gridColumn_BloodTypeCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ServiceUnitName.Caption = Inventec.Common.Resource.Get.Value("frmBrowseExportTicket.gridColumn_ServiceUnitName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_BloodAboCode.Caption = Inventec.Common.Resource.Get.Value("frmBrowseExportTicket.gridColumn_BloodAboCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_ExpiredDateDisplay.Caption = Inventec.Common.Resource.Get.Value("frmBrowseExportTicket.gridColumn_ExpiredDateDisplay.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_BloodRhCode.Caption = Inventec.Common.Resource.Get.Value("frmBrowseExportTicket.gridColumn_BloodRhCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_Imp_Price.Caption = Inventec.Common.Resource.Get.Value("frmBrowseExportTicket.gridColumn_Imp_Price.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_Sum_In_Stock.Caption = Inventec.Common.Resource.Get.Value("frmBrowseExportTicket.gridColumn_Sum_In_Stock.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_Amount.Caption = Inventec.Common.Resource.Get.Value("frmBrowseExportTicket.gridColumn_Amount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_Imp_Vat_Ratio_DisPlay.Caption = Inventec.Common.Resource.Get.Value("frmBrowseExportTicket.gridColumn_Imp_Vat_Ratio_DisPlay.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_Price.Caption = Inventec.Common.Resource.Get.Value("frmBrowseExportTicket.gridColumn_Price.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_Exp_Vat_Ratio_Display.Caption = Inventec.Common.Resource.Get.Value("frmBrowseExportTicket.gridColumn_Exp_Vat_Ratio_Display.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_Discount.Caption = Inventec.Common.Resource.Get.Value("frmBrowseExportTicket.gridColumn_Discount.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_Total_Price.Caption = Inventec.Common.Resource.Get.Value("frmBrowseExportTicket.gridColumn_Total_Price.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmBrowseExportTicket.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem__Save.Caption = Inventec.Common.Resource.Get.Value("frmBrowseExportTicket.barButtonItem__Save.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmBrowseExportTicket.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

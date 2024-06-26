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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.MaterialTypeCreate.Resources.Lang", typeof(HIS.Desktop.Plugins.MaterialTypeCreate.MaterialTypeCreate.frmMaterialTypeCreate__Resource).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRefresh.Text = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.btnRefresh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsAllowExportOdd.Properties.Caption = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.chkIsAllowExportOdd.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem1.Caption = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.barButtonItem1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnServicePaty.Caption = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.bbtnServicePaty.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItem2.Caption = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.barButtonItem2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsSaleEqualImpPrice.Properties.Caption = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.chkIsSaleEqualImpPrice.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsBusiness.Properties.Caption = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.chkIsBusiness.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.groupBox1.Text = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.groupBox1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem8.Text = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.layoutControlItem8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem10.Text = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.layoutControlItem10.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem11.Text = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.layoutControlItem11.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem12.Text = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.layoutControlItem12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem13.Text = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.layoutControlItem13.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem14.Text = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.layoutControlItem14.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciImpPrice.Text = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.lciImpPrice.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciImpVatRatio.Text = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.lciImpVatRatio.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciInternalPrice.Text = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.lciInternalPrice.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciCogs.Text = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.lciCogs.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnMaterialPaty.Text = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.btnMaterialPaty.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkAutoExpend.Properties.Caption = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.chkAutoExpend.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsExprireDate.Properties.Caption = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.chkIsExprireDate.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkCPNG.Properties.Caption = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.chkCPNG.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsChemiscalSubstance.Properties.Caption = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.chkIsChemiscalSubstance.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboHeinServiceType.Properties.NullText = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.cboHeinServiceType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsStent.Properties.Caption = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.chkIsStent.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboServiceUnit.Properties.NullText = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.cboServiceUnit.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboPackingType.Properties.NullText = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.cboPackingType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboNational.Properties.NullText = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.cboNational.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboManufacture.Properties.NullText = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.cboManufacture.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIsStopImp.Properties.Caption = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.chkIsStopImp.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboMaterialTypeParent.Properties.NullText = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.cboMaterialTypeParent.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciMaterialTypeCode.Text = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.lciMaterialTypeCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciMaterialTypeName.Text = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.lciMaterialTypeName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciManufacture.Text = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.lciManufacture.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciNational.Text = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.lciNational.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciServiceUnit.Text = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.lciServiceUnit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciMaterialTypeParent.Text = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.lciMaterialTypeParent.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciAlertExpiredDate.Text = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.lciAlertExpiredDate.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPackingType.Text = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.lciPackingType.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciConcentra.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.lciConcentra.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciConcentra.Text = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.lciConcentra.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciAlertMinInStock.Text = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.lciAlertMinInStock.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciHeinServiceType.Text = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.lciHeinServiceType.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.layoutControlItem3.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.layoutControlItem5.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciHeinServiceBhytName.Text = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.lciHeinServiceBhytName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciHeinServiceBhytCode.Text = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.lciHeinServiceBhytCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciIsChemicalSubstance.Text = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.lciIsChemicalSubstance.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciBhytNumOrder.Text = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.lciBhytNumOrder.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciDescription.Text = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.lciDescription.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciIsStent.Text = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.lciIsStent.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciIsStopImp.Text = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.lciIsStopImp.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciNumOrder.Text = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.lciNumOrder.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem15.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.layoutControlItem15.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem15.Text = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.layoutControlItem15.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem16.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.layoutControlItem16.OptionsToolTip.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem16.Text = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.layoutControlItem16.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem17.Text = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.layoutControlItem17.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmMaterialTypeCreate.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

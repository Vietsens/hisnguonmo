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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisBloodType.Resources.Lang", typeof(HIS.Desktop.Plugins.HisBloodType.HIS.Desktop.Plugins.HisBloodType.HisBloodType.frmHisBloodType__Resource).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("frmHisBloodType.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl7.Text = Inventec.Common.Resource.Get.Value("frmHisBloodType.layoutControl7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmHisBloodType.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn1.Caption = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn1.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn1.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn2.Caption = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn2.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn2.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn3.Caption = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn3.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn3.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn4.Caption = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn4.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn4.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn5.Caption = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn5.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn5.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn5.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn6.Caption = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn6.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn6.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn7.Caption = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn7.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn7.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn8.Caption = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn8.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn8.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn8.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn9.Caption = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn9.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn9.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn9.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn10.Caption = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn10.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn10.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn10.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn22.Caption = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn22.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn22.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn22.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn11.Caption = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn11.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn11.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn11.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn21.Caption = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn21.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn21.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn21.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn12.Caption = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn12.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn12.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn12.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn13.Caption = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn13.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn13.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn13.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn14.Caption = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn14.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn14.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn14.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn15.Caption = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn15.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn15.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn15.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn16.Caption = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn16.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn17.Caption = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn17.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn18.Caption = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn18.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn18.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn18.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn19.Caption = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn19.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn19.ToolTip = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn19.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn20.Caption = Inventec.Common.Resource.Get.Value("frmHisBloodType.treeListColumn20.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmHisBloodType.txtKeyword.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcEditorInfo.Text = Inventec.Common.Resource.Get.Value("frmHisBloodType.lcEditorInfo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmHisBloodType.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnSearch.Caption = Inventec.Common.Resource.Get.Value("frmHisBloodType.bbtnSearch.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnEdit.Caption = Inventec.Common.Resource.Get.Value("frmHisBloodType.bbtnEdit.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnAdd.Caption = Inventec.Common.Resource.Get.Value("frmHisBloodType.bbtnAdd.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnReset.Caption = Inventec.Common.Resource.Get.Value("frmHisBloodType.bbtnReset.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnFocusDefault.Caption = Inventec.Common.Resource.Get.Value("frmHisBloodType.bbtnFocusDefault.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkCPNG.Properties.Caption = Inventec.Common.Resource.Get.Value("frmHisBloodType.chkCPNG.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboParent.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisBloodType.cboParent.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboType.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisBloodType.cboType.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboPacking.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisBloodType.cboPacking.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboDVT.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisBloodType.cboDVT.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboGroup.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisBloodType.cboGroup.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboVolume.Properties.NullText = Inventec.Common.Resource.Get.Value("frmHisBloodType.cboVolume.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmHisBloodType.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRefresh.Text = Inventec.Common.Resource.Get.Value("frmHisBloodType.btnRefresh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmHisBloodType.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.dnNavigation.Text = Inventec.Common.Resource.Get.Value("frmHisBloodType.dnNavigation.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciBloodTypeCode.Text = Inventec.Common.Resource.Get.Value("frmHisBloodType.lciBloodTypeCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciBloodTypeName.Text = Inventec.Common.Resource.Get.Value("frmHisBloodType.lciBloodTypeName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciNumOrder.Text = Inventec.Common.Resource.Get.Value("frmHisBloodType.lciNumOrder.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciImpPrice.Text = Inventec.Common.Resource.Get.Value("frmHisBloodType.lciImpPrice.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciAlertExpiredDate.Text = Inventec.Common.Resource.Get.Value("frmHisBloodType.lciAlertExpiredDate.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciElement.Text = Inventec.Common.Resource.Get.Value("frmHisBloodType.lciElement.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciInternalPrice.Text = Inventec.Common.Resource.Get.Value("frmHisBloodType.lciInternalPrice.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciImpVatRatio.Text = Inventec.Common.Resource.Get.Value("frmHisBloodType.lciImpVatRatio.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem10.Text = Inventec.Common.Resource.Get.Value("frmHisBloodType.layoutControlItem10.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem11.Text = Inventec.Common.Resource.Get.Value("frmHisBloodType.layoutControlItem11.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem12.Text = Inventec.Common.Resource.Get.Value("frmHisBloodType.layoutControlItem12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem13.Text = Inventec.Common.Resource.Get.Value("frmHisBloodType.layoutControlItem13.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem14.Text = Inventec.Common.Resource.Get.Value("frmHisBloodType.layoutControlItem14.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem16.Text = Inventec.Common.Resource.Get.Value("frmHisBloodType.layoutControlItem16.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem17.Text = Inventec.Common.Resource.Get.Value("frmHisBloodType.layoutControlItem17.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem18.Text = Inventec.Common.Resource.Get.Value("frmHisBloodType.layoutControlItem18.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem19.Text = Inventec.Common.Resource.Get.Value("frmHisBloodType.layoutControlItem19.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciHeinOrder.Text = Inventec.Common.Resource.Get.Value("frmHisBloodType.lciHeinOrder.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciHeinLimitPriceOld.Text = Inventec.Common.Resource.Get.Value("frmHisBloodType.lciHeinLimitPriceOld.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciHeinLimitRatioOld.Text = Inventec.Common.Resource.Get.Value("frmHisBloodType.lciHeinLimitRatioOld.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciHeinLimitPrice.Text = Inventec.Common.Resource.Get.Value("frmHisBloodType.lciHeinLimitPrice.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciHeinLimitRatio.Text = Inventec.Common.Resource.Get.Value("frmHisBloodType.lciHeinLimitRatio.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciHeinLimitPriceInTime.Text = Inventec.Common.Resource.Get.Value("frmHisBloodType.lciHeinLimitPriceInTime.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciHeinLimitPriceIntrTime.Text = Inventec.Common.Resource.Get.Value("frmHisBloodType.lciHeinLimitPriceIntrTime.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmHisBloodType.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

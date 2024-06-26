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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.ChmsImpMestList.Resources.Lang", typeof(HIS.Desktop.Plugins.ChmsImpMestList.UCChmsImpMestList__Resource).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCChmsImpMestList.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("UCChmsImpMestList.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtImpMestCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCChmsImpMestList.txtImpMestCode.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRefresh.Text = Inventec.Common.Resource.Get.Value("UCChmsImpMestList.btnRefresh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("UCChmsImpMestList.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.navBarControlFilter.Text = Inventec.Common.Resource.Get.Value("UCChmsImpMestList.navBarControlFilter.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.navBarGroupCreateTime.Caption = Inventec.Common.Resource.Get.Value("UCChmsImpMestList.navBarGroupCreateTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("UCChmsImpMestList.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciCreateTimeFrom.Text = Inventec.Common.Resource.Get.Value("UCChmsImpMestList.lciCreateTimeFrom.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciCreateTimeTo.Text = Inventec.Common.Resource.Get.Value("UCChmsImpMestList.lciCreateTimeTo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlStatus.Text = Inventec.Common.Resource.Get.Value("UCChmsImpMestList.layoutControlStatus.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("UCChmsImpMestList.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciExpTimeFrom.Text = Inventec.Common.Resource.Get.Value("UCChmsImpMestList.lciExpTimeFrom.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciExpTimeTo.Text = Inventec.Common.Resource.Get.Value("UCChmsImpMestList.lciExpTimeTo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.navBarGroupExpTime.Caption = Inventec.Common.Resource.Get.Value("UCChmsImpMestList.navBarGroupExpTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.navBarGroupStatus.Caption = Inventec.Common.Resource.Get.Value("UCChmsImpMestList.navBarGroupStatus.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyWord.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCChmsImpMestList.txtKeyWord.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.STT.Caption = Inventec.Common.Resource.Get.Value("UCChmsImpMestList.STT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("UCChmsImpMestList.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn15.Caption = Inventec.Common.Resource.Get.Value("UCChmsImpMestList.gridColumn15.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("UCChmsImpMestList.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("UCChmsImpMestList.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn20.Caption = Inventec.Common.Resource.Get.Value("UCChmsImpMestList.gridColumn20.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("UCChmsImpMestList.gridColumn4.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("UCChmsImpMestList.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("UCChmsImpMestList.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.IMP_MEST_STT_NAME.Caption = Inventec.Common.Resource.Get.Value("UCChmsImpMestList.IMP_MEST_STT_NAME.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.repositoryItemPictureEdit1.NullText = Inventec.Common.Resource.Get.Value("UCChmsImpMestList.repositoryItemPictureEdit1.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GcExpMestCode.Caption = Inventec.Common.Resource.Get.Value("UCChmsImpMestList.GcExpMestCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GcMediStockName.Caption = Inventec.Common.Resource.Get.Value("UCChmsImpMestList.GcMediStockName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GcImpMediStockName.Caption = Inventec.Common.Resource.Get.Value("UCChmsImpMestList.GcImpMediStockName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GcReqName.Caption = Inventec.Common.Resource.Get.Value("UCChmsImpMestList.GcReqName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GcApprovalName.Caption = Inventec.Common.Resource.Get.Value("UCChmsImpMestList.GcApprovalName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GcApprovalTime.Caption = Inventec.Common.Resource.Get.Value("UCChmsImpMestList.GcApprovalTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GcExpLoginName.Caption = Inventec.Common.Resource.Get.Value("UCChmsImpMestList.GcExpLoginName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GcExpTime.Caption = Inventec.Common.Resource.Get.Value("UCChmsImpMestList.GcExpTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GcCreateTime.Caption = Inventec.Common.Resource.Get.Value("UCChmsImpMestList.GcCreateTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GcCreator.Caption = Inventec.Common.Resource.Get.Value("UCChmsImpMestList.GcCreator.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GcModifyTime.Caption = Inventec.Common.Resource.Get.Value("UCChmsImpMestList.GcModifyTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GcModifier.Caption = Inventec.Common.Resource.Get.Value("UCChmsImpMestList.GcModifier.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisAggrExpMestList.Resources.Lang", typeof(HIS.Desktop.Plugins.HisAggrExpMestList.UCHisAggrExpMestList__Resource).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCHisAggrExpMestList.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnExportList.Text = Inventec.Common.Resource.Get.Value("UCHisAggrExpMestList.btnExportList.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("UCHisAggrExpMestList.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnRefresh.Text = Inventec.Common.Resource.Get.Value("UCHisAggrExpMestList.btnRefresh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("UCHisAggrExpMestList.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.navBarControlFilter.Text = Inventec.Common.Resource.Get.Value("UCHisAggrExpMestList.navBarControlFilter.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.navBarGroupCreateTime.Caption = Inventec.Common.Resource.Get.Value("UCHisAggrExpMestList.navBarGroupCreateTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("UCHisAggrExpMestList.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciCreateTimeForm.Text = Inventec.Common.Resource.Get.Value("UCHisAggrExpMestList.lciCreateTimeForm.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciCreateTimeTo.Text = Inventec.Common.Resource.Get.Value("UCHisAggrExpMestList.lciCreateTimeTo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlStatus.Text = Inventec.Common.Resource.Get.Value("UCHisAggrExpMestList.layoutControlStatus.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("UCHisAggrExpMestList.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciExpTimeFrom.Text = Inventec.Common.Resource.Get.Value("UCHisAggrExpMestList.lciExpTimeFrom.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciExpTimeTo.Text = Inventec.Common.Resource.Get.Value("UCHisAggrExpMestList.lciExpTimeTo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.navBarGroupExpTime.Caption = Inventec.Common.Resource.Get.Value("UCHisAggrExpMestList.navBarGroupExpTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.navBarGroupStatus.Caption = Inventec.Common.Resource.Get.Value("UCHisAggrExpMestList.navBarGroupStatus.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyWord.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCHisAggrExpMestList.txtKeyWord.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.STT.Caption = Inventec.Common.Resource.Get.Value("UCHisAggrExpMestList.STT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("UCHisAggrExpMestList.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn15.Caption = Inventec.Common.Resource.Get.Value("UCHisAggrExpMestList.gridColumn15.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("UCHisAggrExpMestList.gridColumn6.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("UCHisAggrExpMestList.gridColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn20.Caption = Inventec.Common.Resource.Get.Value("UCHisAggrExpMestList.gridColumn20.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("UCHisAggrExpMestList.gridColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("UCHisAggrExpMestList.gridColumn7.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn16.Caption = Inventec.Common.Resource.Get.Value("UCHisAggrExpMestList.gridColumn16.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.IMP_MEST_STT_NAME.Caption = Inventec.Common.Resource.Get.Value("UCHisAggrExpMestList.IMP_MEST_STT_NAME.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GcExpMestCode.Caption = Inventec.Common.Resource.Get.Value("UCHisAggrExpMestList.GcExpMestCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GcMediStockName.Caption = Inventec.Common.Resource.Get.Value("UCHisAggrExpMestList.GcMediStockName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GcReqDepartmentName.Caption = Inventec.Common.Resource.Get.Value("UCHisAggrExpMestList.GcReqDepartmentName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GcUseTime.Caption = Inventec.Common.Resource.Get.Value("UCHisAggrExpMestList.GcUseTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GcExpMestTypeName.Caption = Inventec.Common.Resource.Get.Value("UCHisAggrExpMestList.GcExpMestTypeName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GcReqName.Caption = Inventec.Common.Resource.Get.Value("UCHisAggrExpMestList.GcReqName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GcApprovalName.Caption = Inventec.Common.Resource.Get.Value("UCHisAggrExpMestList.GcApprovalName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GcApprovalTime.Caption = Inventec.Common.Resource.Get.Value("UCHisAggrExpMestList.GcApprovalTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GcExpLoginName.Caption = Inventec.Common.Resource.Get.Value("UCHisAggrExpMestList.GcExpLoginName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GcExpTime.Caption = Inventec.Common.Resource.Get.Value("UCHisAggrExpMestList.GcExpTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GcCreateTime.Caption = Inventec.Common.Resource.Get.Value("UCHisAggrExpMestList.GcCreateTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GcCreator.Caption = Inventec.Common.Resource.Get.Value("UCHisAggrExpMestList.GcCreator.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GcModifyTime.Caption = Inventec.Common.Resource.Get.Value("UCHisAggrExpMestList.GcModifyTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.GcModifier.Caption = Inventec.Common.Resource.Get.Value("UCHisAggrExpMestList.GcModifier.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

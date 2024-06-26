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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("Plugins.Resources.Lang", typeof(Plugins.HIS.Desktop.Plugins.MediReactSum.frmMediReactSum__Resource).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmMediReactSum.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnNew.Text = Inventec.Common.Resource.Get.Value("frmMediReactSum.btnNew.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmMediReactSum.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtIcdText.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmMediReactSum.txtIcdText.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_MediReactSum_Stt.Caption = Inventec.Common.Resource.Get.Value("frmMediReactSum.gridColumn_MediReactSum_Stt.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_MediReactSum_Delete.Caption = Inventec.Common.Resource.Get.Value("frmMediReactSum.gridColumn_MediReactSum_Delete.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_MediReactSum_Print.Caption = Inventec.Common.Resource.Get.Value("frmMediReactSum.gridColumn_MediReactSum_Print.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_MediReactSum_ViewDetail.Caption = Inventec.Common.Resource.Get.Value("frmMediReactSum.gridColumn_MediReactSum_ViewDetail.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_MediReactSum_IcdMain.Caption = Inventec.Common.Resource.Get.Value("frmMediReactSum.gridColumn_MediReactSum_IcdMain.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_MediReactSum_IcdText.Caption = Inventec.Common.Resource.Get.Value("frmMediReactSum.gridColumn_MediReactSum_IcdText.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_MediReactSum_CreateTime.Caption = Inventec.Common.Resource.Get.Value("frmMediReactSum.gridColumn_MediReactSum_CreateTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_MediReactSum_Creator.Caption = Inventec.Common.Resource.Get.Value("frmMediReactSum.gridColumn_MediReactSum_Creator.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_MediReactSum_ModifyTime.Caption = Inventec.Common.Resource.Get.Value("frmMediReactSum.gridColumn_MediReactSum_ModifyTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn_MediReactSum_Modifier.Caption = Inventec.Common.Resource.Get.Value("frmMediReactSum.gridColumn_MediReactSum_Modifier.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutIcdText.Text = Inventec.Common.Resource.Get.Value("frmMediReactSum.layoutIcdText.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutIcdSubCode.Text = Inventec.Common.Resource.Get.Value("frmMediReactSum.layoutIcdSubCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmMediReactSum.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnRCSave.Caption = Inventec.Common.Resource.Get.Value("frmMediReactSum.bbtnRCSave.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnRCNew.Caption = Inventec.Common.Resource.Get.Value("frmMediReactSum.bbtnRCNew.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnRCChoiceIcd.Caption = Inventec.Common.Resource.Get.Value("frmMediReactSum.bbtnRCChoiceIcd.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmMediReactSum.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

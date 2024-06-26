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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisImportMestMedicine.Resources.Lang", typeof(HIS.Desktop.Plugins.HisImportMestMedicine.UC.UC_NCC__Resource).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UC_NCC.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("UC_NCC.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl6.Text = Inventec.Common.Resource.Get.Value("UC_NCC.layoutControl6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.groupBoxSoChungTu.Text = Inventec.Common.Resource.Get.Value("UC_NCC.groupBoxSoChungTu.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl7.Text = Inventec.Common.Resource.Get.Value("UC_NCC.layoutControl7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEditNoDocumentNumber.Properties.Caption = Inventec.Common.Resource.Get.Value("UC_NCC.checkEditNoDocumentNumber.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.checkEditHasDocumentNumber.Properties.Caption = Inventec.Common.Resource.Get.Value("UC_NCC.checkEditHasDocumentNumber.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem24.Text = Inventec.Common.Resource.Get.Value("UC_NCC.layoutControlItem24.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem25.Text = Inventec.Common.Resource.Get.Value("UC_NCC.layoutControlItem25.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.groupBoxNgayChungTu.Text = Inventec.Common.Resource.Get.Value("UC_NCC.groupBoxNgayChungTu.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl8.Text = Inventec.Common.Resource.Get.Value("UC_NCC.layoutControl8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem22.Text = Inventec.Common.Resource.Get.Value("UC_NCC.layoutControlItem22.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem23.Text = Inventec.Common.Resource.Get.Value("UC_NCC.layoutControlItem23.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

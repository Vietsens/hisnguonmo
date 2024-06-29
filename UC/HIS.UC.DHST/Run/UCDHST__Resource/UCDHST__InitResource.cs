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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.UC.DHST.Resources.Lang", typeof(HIS.UC.DHST.Run.UCDHST__Resource).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCDHST.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl12.Text = Inventec.Common.Resource.Get.Value("UCDHST.labelControl12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl10.Text = Inventec.Common.Resource.Get.Value("UCDHST.labelControl10.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl9.Text = Inventec.Common.Resource.Get.Value("UCDHST.labelControl9.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl8.Text = Inventec.Common.Resource.Get.Value("UCDHST.labelControl8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl7.Text = Inventec.Common.Resource.Get.Value("UCDHST.labelControl7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl6.Text = Inventec.Common.Resource.Get.Value("UCDHST.labelControl6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl5.Text = Inventec.Common.Resource.Get.Value("UCDHST.labelControl5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl4.Text = Inventec.Common.Resource.Get.Value("UCDHST.labelControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.labelControl3.Text = Inventec.Common.Resource.Get.Value("UCDHST.labelControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem1.Text = Inventec.Common.Resource.Get.Value("UCDHST.layoutControlItem1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("UCDHST.layoutControlItem2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("UCDHST.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem5.Text = Inventec.Common.Resource.Get.Value("UCDHST.layoutControlItem5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem6.Text = Inventec.Common.Resource.Get.Value("UCDHST.layoutControlItem6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem7.Text = Inventec.Common.Resource.Get.Value("UCDHST.layoutControlItem7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem8.Text = Inventec.Common.Resource.Get.Value("UCDHST.layoutControlItem8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem9.Text = Inventec.Common.Resource.Get.Value("UCDHST.layoutControlItem9.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem10.Text = Inventec.Common.Resource.Get.Value("UCDHST.layoutControlItem10.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem11.Text = Inventec.Common.Resource.Get.Value("UCDHST.layoutControlItem11.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem17.Text = Inventec.Common.Resource.Get.Value("UCDHST.layoutControlItem17.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

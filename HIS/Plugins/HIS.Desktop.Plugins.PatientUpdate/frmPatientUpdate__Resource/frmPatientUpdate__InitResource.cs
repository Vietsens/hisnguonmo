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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.PatientUpdate.Resources.Lang", typeof(HIS.Desktop.Plugins.PatientUpdate.frmPatientUpdate__Resource).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmPatientUpdate.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.groupControl1.Text = Inventec.Common.Resource.Get.Value("frmPatientUpdate.groupControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmPatientUpdate.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkUpdateNew.Properties.Caption = Inventec.Common.Resource.Get.Value("frmPatientUpdate.chkUpdateNew.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmPatientUpdate.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButtonItemSave.Caption = Inventec.Common.Resource.Get.Value("frmPatientUpdate.barButtonItemSave.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkBNManTinh.Properties.Caption = Inventec.Common.Resource.Get.Value("frmPatientUpdate.chkBNManTinh.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboEthnic.Properties.NullText = Inventec.Common.Resource.Get.Value("frmPatientUpdate.cboEthnic.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSave.Text = Inventec.Common.Resource.Get.Value("frmPatientUpdate.btnSave.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboRh.Properties.NullText = Inventec.Common.Resource.Get.Value("frmPatientUpdate.cboRh.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboBloodAbo.Properties.NullText = Inventec.Common.Resource.Get.Value("frmPatientUpdate.cboBloodAbo.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboMilitaryRank.Properties.NullText = Inventec.Common.Resource.Get.Value("frmPatientUpdate.cboMilitaryRank.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboNation.Properties.NullText = Inventec.Common.Resource.Get.Value("frmPatientUpdate.cboNation.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboCommune.Properties.NullText = Inventec.Common.Resource.Get.Value("frmPatientUpdate.cboCommune.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboDistricts.Properties.NullText = Inventec.Common.Resource.Get.Value("frmPatientUpdate.cboDistricts.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboProvince.Properties.NullText = Inventec.Common.Resource.Get.Value("frmPatientUpdate.cboProvince.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboCareer.Properties.NullText = Inventec.Common.Resource.Get.Value("frmPatientUpdate.cboCareer.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboGender1.Properties.NullText = Inventec.Common.Resource.Get.Value("frmPatientUpdate.cboGender1.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPatientName.Text = Inventec.Common.Resource.Get.Value("frmPatientUpdate.lciPatientName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciEthnic.Text = Inventec.Common.Resource.Get.Value("frmPatientUpdate.lciEthnic.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciGender.Text = Inventec.Common.Resource.Get.Value("frmPatientUpdate.lciGender.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciCareer.Text = Inventec.Common.Resource.Get.Value("frmPatientUpdate.lciCareer.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciAdress.Text = Inventec.Common.Resource.Get.Value("frmPatientUpdate.lciAdress.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciCommune.Text = Inventec.Common.Resource.Get.Value("frmPatientUpdate.lciCommune.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciDistricts.Text = Inventec.Common.Resource.Get.Value("frmPatientUpdate.lciDistricts.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciProvince.Text = Inventec.Common.Resource.Get.Value("frmPatientUpdate.lciProvince.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPhone.Text = Inventec.Common.Resource.Get.Value("frmPatientUpdate.lciPhone.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem24.Text = Inventec.Common.Resource.Get.Value("frmPatientUpdate.layoutControlItem24.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lctCmnd.Text = Inventec.Common.Resource.Get.Value("frmPatientUpdate.lctCmnd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lctDR.Text = Inventec.Common.Resource.Get.Value("frmPatientUpdate.lctDR.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lctIB.Text = Inventec.Common.Resource.Get.Value("frmPatientUpdate.lctIB.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lctRh.Text = Inventec.Common.Resource.Get.Value("frmPatientUpdate.lctRh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciBloodAbo.Text = Inventec.Common.Resource.Get.Value("frmPatientUpdate.lciBloodAbo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciPersonFamily.Text = Inventec.Common.Resource.Get.Value("frmPatientUpdate.lciPersonFamily.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciRelation.Text = Inventec.Common.Resource.Get.Value("frmPatientUpdate.lciRelation.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lctContact.Text = Inventec.Common.Resource.Get.Value("frmPatientUpdate.lctContact.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciNation.Text = Inventec.Common.Resource.Get.Value("frmPatientUpdate.lciNation.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciMilitaryRank.Text = Inventec.Common.Resource.Get.Value("frmPatientUpdate.lciMilitaryRank.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem3.Text = Inventec.Common.Resource.Get.Value("frmPatientUpdate.layoutControlItem3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciDOB.Text = Inventec.Common.Resource.Get.Value("frmPatientUpdate.lciDOB.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciUpdateNew.Text = Inventec.Common.Resource.Get.Value("frmPatientUpdate.lciUpdateNew.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmPatientUpdate.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

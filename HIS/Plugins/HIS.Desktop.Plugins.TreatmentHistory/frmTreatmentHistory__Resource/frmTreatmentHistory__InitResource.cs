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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.TreatmentHistory.Resources.Lang", typeof(HIS.Desktop.Plugins.TreatmentHistory.frmTreatmentHistory__Resource).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmTreatmentHistory.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmTreatmentHistory.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmTreatmentHistory.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnVienPhi.Text = Inventec.Common.Resource.Get.Value("frmTreatmentHistory.btnVienPhi.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnCloseLeaft.Text = Inventec.Common.Resource.Get.Value("frmTreatmentHistory.btnCloseLeaft.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnOpenLeaft.Text = Inventec.Common.Resource.Get.Value("frmTreatmentHistory.btnOpenLeaft.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColDepartment.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentHistory.treeListColDepartment.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn2.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentHistory.treeListColumn2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn1.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentHistory.treeListColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.treeListColumn3.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentHistory.treeListColumn3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColSTT.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentHistory.gridColSTT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColStatus.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentHistory.gridColStatus.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColTreatmentCode.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentHistory.gridColTreatmentCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColPatientCode.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentHistory.gridColPatientCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColPatientName.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentHistory.gridColPatientName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColDOB.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentHistory.gridColDOB.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColGenderName.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentHistory.gridColGenderName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColIcdMain.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentHistory.gridColIcdMain.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColIcdText.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentHistory.gridColIcdText.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColInTime.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentHistory.gridColInTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColOutTime.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentHistory.gridColOutTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyWord.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmTreatmentHistory.txtKeyWord.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtPatientCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmTreatmentHistory.txtPatientCode.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtTreatmentCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmTreatmentHistory.txtTreatmentCode.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmTreatmentHistory.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButton__F1.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentHistory.barButton__F1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButton__F2.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentHistory.barButton__F2.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButton__F3.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentHistory.barButton__F3.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.barButton__CrtlF.Caption = Inventec.Common.Resource.Get.Value("frmTreatmentHistory.barButton__CrtlF.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmTreatmentHistory.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

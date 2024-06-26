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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.HisExamServiceTemp.Resources.Lang", typeof(HIS.Desktop.Plugins.HisExamServiceTemp.HIS.Desktop.Plugins.HisExamServiceTemp.HisExamServiceTemp.frmHisExamServiceTemp__Resource).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl7.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.layoutControl7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSearch.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.btnSearch.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.STT.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.STT.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumnDelete.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.gridColumnDelete.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gcolGSelect.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.gcolGSelect.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExamServiceTempCode.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColExamServiceTempCode.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExamServiceTempCode.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColExamServiceTempCode.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExamServiceTempName.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColExamServiceTempName.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColExamServiceTempName.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColExamServiceTempName.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPartExamCirculation.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColPartExamCirculation.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPartExamCirculation.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColPartExamCirculation.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPartExamRespiratory.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColPartExamRespiratory.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPartExamRespiratory.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColPartExamRespiratory.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPartExamDigestion.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColPartExamDigestion.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPartExamDigestion.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColPartExamDigestion.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPartExamKidneyUrology.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColPartExamKidneyUrology.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPartExamKidneyUrology.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColPartExamKidneyUrology.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPartExamNeurological.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColPartExamNeurological.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPartExamNeurological.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColPartExamNeurological.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPartExamMuscleBone.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColPartExamMuscleBone.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPartExamMuscleBone.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColPartExamMuscleBone.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPartExamEnt.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColPartExamEnt.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPartExamEnt.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColPartExamEnt.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPartExamStomatology.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColPartExamStomatology.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPartExamStomatology.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColPartExamStomatology.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPartExamEye.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColPartExamEye.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPartExamEye.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColPartExamEye.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPartExamOend.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColPartExamOend.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPartExamOend.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColPartExamOend.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPartExamMental.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColPartExamMental.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPartExamMental.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColPartExamMental.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPartExamObstetric.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColPartExamObstetric.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPartExamObstetric.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColPartExamObstetric.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPartExamNutrition.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColPartExamNutrition.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPartExamNutrition.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColPartExamNutrition.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPartExamMotion.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColPartExamMotion.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPartExamMotion.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColPartExamMotion.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColHospitalizationReason.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColHospitalizationReason.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColHospitalizationReason.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColHospitalizationReason.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPathologicalProcess.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColPathologicalProcess.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPathologicalProcess.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColPathologicalProcess.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPathologicalHistory.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColPathologicalHistory.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPathologicalHistory.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColPathologicalHistory.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPathologicalHistoryFamily.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColPathologicalHistoryFamily.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPathologicalHistoryFamily.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColPathologicalHistoryFamily.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColFullExam.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColFullExam.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColFullExam.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColFullExam.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPartExam.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColPartExam.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColPartExam.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColPartExam.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreateTime.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColCreateTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreateTime.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColCreateTime.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreator.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColCreator.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColCreator.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColCreator.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifyTime.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColModifyTime.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifyTime.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColModifyTime.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifier.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColModifier.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.grdColModifier.ToolTip = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.grdColModifier.ToolTip", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.gridColumn1.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.txtKeyword.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lcEditorInfo.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.lcEditorInfo.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkPublic.Properties.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.chkPublic.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bar1.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.bar1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnSearch.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.bbtnSearch.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnEdit.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.bbtnEdit.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnAdd.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.bbtnAdd.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnReset.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.bbtnReset.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.bbtnFocusDefault.Caption = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.bbtnFocusDefault.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnSelect.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.btnSelect.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage__QuaTrinhBenhLy.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.xtraTabPage__QuaTrinhBenhLy.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage__TienSuBenhBenhNhan.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.xtraTabPage__TienSuBenhBenhNhan.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage__TienSuBenhGiaDinh.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.xtraTabPage__TienSuBenhGiaDinh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage4.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.xtraTabPage4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage5.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.xtraTabPage5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage6.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.xtraTabPage6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage7.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.xtraTabPage7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage8.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.xtraTabPage8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage9.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.xtraTabPage9.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage10.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.xtraTabPage10.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage11.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.xtraTabPage11.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage12.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.xtraTabPage12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage13.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.xtraTabPage13.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage14.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.xtraTabPage14.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage15.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.xtraTabPage15.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage16.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.xtraTabPage16.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage17.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.xtraTabPage17.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl5.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.layoutControl5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage18.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.xtraTabPage18.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl6.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.layoutControl6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage19.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.xtraTabPage19.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnCancel.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.btnCancel.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAdd.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.btnAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.dnNavigation.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.dnNavigation.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnEdit.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.btnEdit.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage__TomTatCLS.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.xtraTabPage__TomTatCLS.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage__PhuongPhapDieuTri.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.xtraTabPage__PhuongPhapDieuTri.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage__HuongDieuTri.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.xtraTabPage__HuongDieuTri.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciExamServiceTempCode.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.lciExamServiceTempCode.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciExamServiceTempName.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.lciExamServiceTempName.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lciHospitalizationReason.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.lciHospitalizationReason.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem13.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.layoutControlItem13.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem15.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.layoutControlItem15.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.Text = Inventec.Common.Resource.Get.Value("frmHisExamServiceTemp.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

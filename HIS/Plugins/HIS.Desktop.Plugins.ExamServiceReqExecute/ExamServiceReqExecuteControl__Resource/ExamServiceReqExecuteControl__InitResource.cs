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
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("HIS.Desktop.Plugins.ExamServiceReqExecute.Resources.Lang", typeof(HIS.Desktop.Plugins.ExamServiceReqExecute.ExamServiceReqExecuteControl__Resource).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.layoutControl1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.layoutControl2.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnTrackingCreate.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.btnTrackingCreate.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnExamServiceAdd.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.btnExamServiceAdd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnTuTruc.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.btnTuTruc.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnServiceReqList.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.btnServiceReqList.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnHospitalize.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.btnHospitalize.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAccidentHurt.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.btnAccidentHurt.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAssignPre.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.btnAssignPre.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnChooseTemplate.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.btnChooseTemplate.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnTreatmentFinish.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.btnTreatmentFinish.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnAssignService.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.btnAssignService.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnDeparment.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.btnDeparment.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.btnPrint_ExamService.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.btnPrint_ExamService.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.layoutControl3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtIcdExtraName.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.txtIcdExtraName.Properties.NullValuePrompt", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.groupBox1.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.groupBox1.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.chkIcds.Properties.Caption = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.chkIcds.Properties.Caption", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.cboIcds.Properties.NullText = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.cboIcds.Properties.NullText", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPageChung.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.xtraTabPageChung.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage3.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.xtraTabPage3.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage4.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.xtraTabPage4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage5.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.xtraTabPage5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage6.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.xtraTabPage6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage7.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.xtraTabPage7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage8.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.xtraTabPage8.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage9.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.xtraTabPage9.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage10.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.xtraTabPage10.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage11.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.xtraTabPage11.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage12.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.xtraTabPage12.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage13.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.xtraTabPage13.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.layoutControl4.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage14.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.xtraTabPage14.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl5.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.layoutControl5.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage15.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.xtraTabPage15.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl6.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.layoutControl6.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.xtraTabPage16.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.xtraTabPage16.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControl7.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.layoutControl7.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.txtSick.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.txtSick.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lblCaptionHospitalizationReason.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.lblCaptionHospitalizationReason.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lblCaptionPathologicalProcess.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.lblCaptionPathologicalProcess.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lblCaptionPathologicalHistoryFamily.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.lblCaptionPathologicalHistoryFamily.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lblCaptionPathologicalHistory.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.lblCaptionPathologicalHistory.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lblIcdMainText.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.lblIcdMainText.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lblEditIcd.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.lblEditIcd.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lblCaptionNgayThuCuaBenh.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.lblCaptionNgayThuCuaBenh.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lblSick.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.lblSick.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lblCaptionConclude.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.lblCaptionConclude.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lblIcdText.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.lblIcdText.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem20.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.layoutControlItem20.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lblInformationExam.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.lblInformationExam.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem22.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.layoutControlItem22.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lblCaptionDiagnostic.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.lblCaptionDiagnostic.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.lblCaptionResultNote.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.lblCaptionResultNote.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
                this.layoutControlItem19.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.layoutControlItem19.Text", Resources.ResourceLanguageManager.LanguageResource, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

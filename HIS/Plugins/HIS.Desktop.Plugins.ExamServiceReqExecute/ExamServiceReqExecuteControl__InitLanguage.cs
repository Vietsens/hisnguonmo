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
using HIS.Desktop.Plugins.ExamServiceReqExecute.Base;
using HIS.Desktop.Utility;
using Inventec.Desktop.Common.LanguageManager;
using System;

namespace HIS.Desktop.Plugins.ExamServiceReqExecute
{
    public partial class ExamServiceReqExecuteControl : UserControlBase
    {


        /// <summary>
        ///Hàm xét ngôn ngữ cho giao diện ExamServiceReqExecuteControl
        /// </summary>
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                ResourceLangManager.InitResourceLanguageManager();

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.layoutControl1.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.layoutControl2.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.btnFastTrackingCreate.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.btnFastTrackingCreate.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.btnVoBenhAn.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.btnVoBenhAn.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.btnMedisoftHistory.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.btnMedisoftHistory.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.btnMedisoftHistory.ToolTip = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.btnMedisoftHistory.ToolTip", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.btnAssignPaan.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.btnAssignPaan.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.btnAssignPaan.ToolTip = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.btnAssignPaan.ToolTip", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.btnContentSubclinical.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.btnContentSubclinical.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.btnKeDonYHCT.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.btnKeDonYHCT.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.btnKeDonYHCT.ToolTip = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.btnKeDonYHCT.ToolTip", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.btnAggrExam.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.btnAggrExam.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.btnServiceReqList.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.btnServiceReqList.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.drBtnOther.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.drBtnOther.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.btnTuTruc.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.btnTuTruc.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.btnAssignPre.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.btnAssignPre.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.btnChooseTemplate.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.btnChooseTemplate.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.btnSaveFinish.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.btnSaveFinish.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.btnAssignService.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.btnAssignService.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.btnPrint_ExamService.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.btnPrint_ExamService.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.layoutControl3.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.layoutControl21.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.layoutControl21.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.gridColumn17.Caption = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.gridColumn17.Caption", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.gridColumn16.Caption = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.gridColumn16.Caption", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.gridColumn12.Caption = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.gridColumn12.Caption", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.txtInfomationExecute.ToolTip = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.txtInfomationExecute.ToolTip", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.btnConnectBloodPressure.ToolTip = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.btnConnectBloodPressure.ToolTip", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.layoutControl19.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.layoutControl19.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.gridColumn11.Caption = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.gridColumn11.Caption", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.labelControl12.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.labelControl12.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.layoutControl18.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.layoutControl18.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.layoutControl17.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.layoutControl17.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.btnContentSubclinicalEdit.ToolTip = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.btnContentSubclinicalEdit.ToolTip", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                //toolTipItem1.Text = Inventec.Common.Resource.Get.Value("toolTipItem1.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.labelControl10.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.labelControl10.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.labelControl2.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.labelControl2.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.labelControl6.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.labelControl6.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.labelControl9.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.labelControl9.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.labelControl8.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.labelControl8.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.labelControl5.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.labelControl5.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.labelControl7.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.labelControl7.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.cboPatientCase.Properties.NullText = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.cboPatientCase.Properties.NullText", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.labelControl4.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.labelControl4.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.xtraTabPageDiUng.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.xtraTabPageDiUng.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.layoutControl22.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.layoutControl22.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.gridColumn19.Caption = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.gridColumn19.Caption", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.gridColumn4.Caption = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.gridColumn4.Caption", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.gridColumn5.Caption = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.gridColumn5.Caption", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.gridColumn6.Caption = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.gridColumn6.Caption", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.gridColumn7.Caption = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.gridColumn7.Caption", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.gridColumn8.Caption = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.gridColumn8.Caption", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.xtraTabPageExamHistory.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.xtraTabPageExamHistory.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.gridColumn10.Caption = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.gridColumn10.Caption", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.gridColumn9.Caption = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.gridColumn9.Caption", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.gridColumn1.Caption = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.gridColumn1.Caption", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.gridColumn2.Caption = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.gridColumn2.Caption", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.gridColumn3.Caption = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.gridColumn3.Caption", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.xtraTabPageExamExecute.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.xtraTabPageExamExecute.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.layoutControlTabExamExecute.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.layoutControlTabExamExecute.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.groupControlTreatmentFinish.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.groupControlTreatmentFinish.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.layoutControl8.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.layoutControl8.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.layoutControl11.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.layoutControl11.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.chkTreatmentFinish.Properties.Caption = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.chkTreatmentFinish.Properties.Caption", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.chkExamFinish.Properties.Caption = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.chkExamFinish.Properties.Caption", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.chkExamServiceAdd.Properties.Caption = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.chkExamServiceAdd.Properties.Caption", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.chkHospitalize.Properties.Caption = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.chkHospitalize.Properties.Caption", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.labelControl1.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.labelControl1.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.xtraTabPage1.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.xtraTabPage1.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.layoutControl14.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.layoutControl14.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.STT.Caption = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.STT.Caption", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.grdColTime.Caption = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.grdColTime.Caption", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.gridColumn13.Caption = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.gridColumn13.Caption", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.gridColumn14.Caption = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.gridColumn14.Caption", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.gridColumn15.Caption = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.gridColumn15.Caption", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.xtraTabPageContraindication.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.xtraTabPageContraindication.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.layoutControl16.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.layoutControl16.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.cboContraindication.Properties.NullText = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.cboContraindication.Properties.NullText", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.layoutControlItem59.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.layoutControlItem59.OptionsToolTip.ToolTip", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.layoutControlItem59.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.layoutControlItem59.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.layoutControl12.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.layoutControl12.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.cboNextTreatmentInstructions.Properties.NullText = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.cboNextTreatmentInstructions.Properties.NullText", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.chkEditNextTreatmentInstruction.Properties.Caption = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.chkEditNextTreatmentInstruction.Properties.Caption", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.lciNextTreatmentInstructionText.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.lciNextTreatmentInstructionText.OptionsToolTip.ToolTip", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.lciNextTreatmentInstructionText.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.lciNextTreatmentInstructionText.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.labelControl3.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.labelControl3.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.layoutControl15.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.layoutControl15.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.btnChooseIcdText.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.btnChooseIcdText.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.lblCDPhu.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.lblCDPhu.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.txtIcdText.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.txtIcdText.Properties.NullValuePrompt", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.lcCause.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.lcCause.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.cboIcdsCause.Properties.NullText = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.cboIcdsCause.Properties.NullText", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.chkEditIcdCause.Properties.Caption = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.chkEditIcdCause.Properties.Caption", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.lciIcdTextCause.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.lciIcdTextCause.OptionsToolTip.ToolTip", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.lciIcdTextCause.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.lciIcdTextCause.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.lciIcdSubCode.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.lciIcdSubCode.OptionsToolTip.ToolTip", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.lciIcdSubCode.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.lciIcdSubCode.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.cboKskCode.Properties.NullText = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.cboKskCode.Properties.NullText", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.layoutControl13.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.layoutControl13.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.cboIcds.Properties.NullText = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.cboIcds.Properties.NullText", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.chkEditIcd.Properties.Caption = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.chkEditIcd.Properties.Caption", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.lciIcdText.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.lciIcdText.OptionsToolTip.ToolTip", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.lciIcdText.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.lciIcdText.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                //this.xtraTabPageChung.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.xtraTabPageChung.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                //this.xtraTabTuanHoan.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.xtraTabTuanHoan.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                //this.xtraTabHoHap.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.xtraTabHoHap.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                //this.xtraTabTieuHoa.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.xtraTabTieuHoa.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                //this.xtraTabThanTietNieu.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.xtraTabThanTietNieu.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                //this.xtraTabThanKinh.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.xtraTabThanKinh.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                //this.xtraTabCoXuongKhop.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.xtraTabCoXuongKhop.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                //this.xtraTabTaiMuiHong.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.xtraTabTaiMuiHong.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.layoutControl10.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.layoutControl10.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.labelControl22.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.labelControl22.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.labelControl21.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.labelControl21.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.labelControl20.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.labelControl20.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.labelControl19.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.labelControl19.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.layoutControlItem30.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.layoutControlItem30.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.layoutControlItem31.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.layoutControlItem31.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.layoutControlItem32.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.layoutControlItem32.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.layoutControlItem74.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.layoutControlItem74.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.layoutControlItem77.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.layoutControlItem77.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.layoutControlItem76.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.layoutControlItem76.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.layoutControlItem75.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.layoutControlItem75.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                //this.xtraTabRangHamMat.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.xtraTabRangHamMat.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.layoutControl20.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.layoutControl20.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.layoutControlItem78.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.layoutControlItem78.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.layoutControlItem79.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.layoutControlItem79.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.layoutControlItem80.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.layoutControlItem80.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                //this.xtraTabMat.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.xtraTabMat.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.layoutControl9.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.layoutControl9.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.labelControl18.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.labelControl18.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.chkPART_EXAM_EYE_BLIND_COLOR__MMV.Properties.Caption = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.chkPART_EXAM_EYE_BLIND_COLOR__MMV.Properties.Caption", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.chkPART_EXAM_EYE_BLIND_COLOR__MMXLC.Properties.Caption = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.chkPART_EXAM_EYE_BLIND_COLOR__MMXLC.Properties.Caption", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.chkPART_EXAM_EYE_BLIND_COLOR__MMTB.Properties.Caption = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.chkPART_EXAM_EYE_BLIND_COLOR__MMTB.Properties.Caption", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.chkPART_EXAM_EYE_BLIND_COLOR__MMD.Properties.Caption = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.chkPART_EXAM_EYE_BLIND_COLOR__MMD.Properties.Caption", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.chkPART_EXAM_EYE_BLIND_COLOR__BT.Properties.Caption = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.chkPART_EXAM_EYE_BLIND_COLOR__BT.Properties.Caption", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.lcForTabpageMat__ThiTruong.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.lcForTabpageMat__ThiTruong.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.chkPART_EXAM_VERTICAL_SIGHT__HC.Properties.Caption = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.chkPART_EXAM_VERTICAL_SIGHT__HC.Properties.Caption", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.chkPART_EXAM_HORIZONTAL_SIGHT__HC.Properties.Caption = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.chkPART_EXAM_HORIZONTAL_SIGHT__HC.Properties.Caption", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.chkPART_EXAM_VERTICAL_SIGHT__BT.Properties.Caption = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.chkPART_EXAM_VERTICAL_SIGHT__BT.Properties.Caption", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.chkPART_EXAM_HORIZONTAL_SIGHT__BT.Properties.Caption = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.chkPART_EXAM_HORIZONTAL_SIGHT__BT.Properties.Caption", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.labelControl17.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.labelControl17.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.labelControl16.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.labelControl16.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.labelControl15.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.labelControl15.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.labelControl14.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.labelControl14.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.labelControl13.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.labelControl13.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.labelControl11.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.labelControl11.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.layoutControlItem20.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.layoutControlItem20.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.layoutControlItem22.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.layoutControlItem22.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.layoutControlItem21.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.layoutControlItem21.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.layoutControlItem26.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.layoutControlItem26.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.layoutControlItem23.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.layoutControlItem23.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.layoutControlItem27.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.layoutControlItem27.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                //this.xtraTabNoiTiet.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.xtraTabNoiTiet.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                //this.xtraTabTamThan.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.xtraTabTamThan.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.layoutControl4.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                //this.xtraTabDinhDuong.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.xtraTabDinhDuong.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.layoutControl5.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.layoutControl5.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                //this.xtraTabVanDong.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.xtraTabVanDong.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.layoutControl6.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.layoutControl6.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                //this.xtraTabSanPhuKhoa.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.xtraTabSanPhuKhoa.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.layoutControl7.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.layoutControl7.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                //this.xtraTabDaLieu.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.xtraTabDaLieu.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.txtSick.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.txtSick.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.lblCaptionPathologicalProcess.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.lblCaptionPathologicalProcess.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.lblInformationExam.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.lblInformationExam.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.lblCaptionDiagnostic.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.lblCaptionDiagnostic.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.lblCaptionConclude.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.lblCaptionConclude.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.lblCaptionNgayThuCuaBenh.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.lblCaptionNgayThuCuaBenh.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.lblSick.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.lblSick.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.lciPatientCase.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.lciPatientCase.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.lblCaptionHospitalizationReason.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.lblCaptionHospitalizationReason.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.lciKskCode.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.lciKskCode.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.lciChuY.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.lciChuY.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.lciProvisionalDianosis.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.lciProvisionalDianosis.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.lcgDHST.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.lcgDHST.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.lciExecuteTime.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.lciExecuteTime.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.lciPulse.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.lciPulse.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.lciBloodPressure.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.lciBloodPressure.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.lciWeight.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.lciWeight.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.lciHeight.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.lciHeight.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.lciNote.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.lciNote.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                //this.lciBMIDisplay.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.lciBMIDisplay.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                //this.lciLeatherArea.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.lciLeatherArea.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.lciMLCT.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.lciMLCT.OptionsToolTip.ToolTip", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.lciMLCT.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.lciMLCT.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.lciTemperature.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.lciTemperature.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.lciSpo2.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.lciSpo2.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.lciBreathRate.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.lciBreathRate.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.lciChest.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.lciChest.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.lciBelly.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.lciBelly.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.lciBMI.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.lciBMI.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.lciKhamToanThan.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.lciKhamToanThan.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.lblCaptionPathologicalHistory.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.lblCaptionPathologicalHistory.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.lblCaptionPathologicalHistoryFamily.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.lblCaptionPathologicalHistoryFamily.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.gridColumn18.Caption = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.gridColumn18.Caption", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.customGridViewWithFilterMultiColumn3.OptionsFind.FindNullPrompt = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.customGridViewWithFilterMultiColumn3.OptionsFind.FindNullPrompt", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.customGridViewWithFilterMultiColumn1.OptionsFind.FindNullPrompt = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.customGridViewWithFilterMultiColumn1.OptionsFind.FindNullPrompt", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.customGridViewWithFilterMultiColumn2.OptionsFind.FindNullPrompt = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.customGridViewWithFilterMultiColumn2.OptionsFind.FindNullPrompt", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());

                this.xtraTabPageChung.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.xtraTabPageChung.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.xtraTabTuanHoan.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.xtraTabPage3.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.xtraTabHoHap.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.xtraTabPage4.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.xtraTabTieuHoa.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.xtraTabPage5.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.xtraTabThanTietNieu.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.xtraTabPage6.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.xtraTabThanKinh.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.xtraTabPage7.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.xtraTabCoXuongKhop.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.xtraTabPage8.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.xtraTabTaiMuiHong.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.xtraTabPage9.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.xtraTabRangHamMat.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.xtraTabPage10.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.xtraTabMat.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.xtraTabPage11.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.xtraTabNoiTiet.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.xtraTabPage12.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.xtraTabTamThan.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.xtraTabPage13.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.layoutControl4.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.xtraTabDinhDuong.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.xtraTabPage14.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.layoutControl5.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.layoutControl5.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.xtraTabVanDong.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.xtraTabPage15.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.layoutControl6.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.layoutControl6.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.xtraTabSanPhuKhoa.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.xtraTabPage16.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.xtraTabDaLieu.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.xtraTabPage17.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.layoutControl7.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.layoutControl7.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }


        public void InitLanguage()
        {
            try
            {
                ////Khoi tao doi tuong resource
                ResourceLangManager.InitResourceLanguageManager();

                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.layoutControl1.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.layoutControl2.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                //this.btnTrackingCreate.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.btnTrackingCreate.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());

                this.btnTuTruc.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.btnTuTruc.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                //this.btnServiceReqList.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.btnServiceReqList.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());

                //this.btnAccidentHurt.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.btnAccidentHurt.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.drBtnOther.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.drBtnOther.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.btnAssignPre.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.btnAssignPre.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.btnAssignPaan.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.btnAssignPaan.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.btnAssignPaan.ToolTip = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.btnAssignPaan.ToolTip", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.btnChooseTemplate.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.btnChooseTemplate.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.btnSaveFinish.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.btnSaveFinish.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.btnAssignService.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.btnAssignService.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());

                this.btnPrint_ExamService.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.btnPrint_ExamService.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());

                this.btnVoBenhAn.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.btnVoBenhAn.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());

                this.layoutControl3.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.layoutControl3.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.lcgDHST.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.groupBox1.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());

                this.xtraTabPageChung.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.xtraTabPageChung.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.xtraTabTuanHoan.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.xtraTabPage3.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.xtraTabHoHap.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.xtraTabPage4.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.xtraTabTieuHoa.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.xtraTabPage5.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.xtraTabThanTietNieu.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.xtraTabPage6.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.xtraTabThanKinh.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.xtraTabPage7.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.xtraTabCoXuongKhop.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.xtraTabPage8.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.xtraTabTaiMuiHong.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.xtraTabPage9.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.xtraTabRangHamMat.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.xtraTabPage10.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.xtraTabMat.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.xtraTabPage11.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.xtraTabNoiTiet.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.xtraTabPage12.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.xtraTabTamThan.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.xtraTabPage13.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.layoutControl4.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.layoutControl4.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.xtraTabDinhDuong.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.xtraTabPage14.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.layoutControl5.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.layoutControl5.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.xtraTabVanDong.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.xtraTabPage15.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.layoutControl6.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.layoutControl6.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.xtraTabSanPhuKhoa.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.xtraTabPage16.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.xtraTabDaLieu.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.xtraTabPage17.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.layoutControl7.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.layoutControl7.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.txtSick.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.txtSick.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.lblCaptionHospitalizationReason.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.lblCaptionHospitalizationReason.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.lblCaptionPathologicalProcess.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.lblCaptionPathologicalProcess.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.lblCaptionPathologicalHistoryFamily.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.lblCaptionPathologicalHistoryFamily.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.lblCaptionPathologicalHistory.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.lblCaptionPathologicalHistory.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.lblCaptionNgayThuCuaBenh.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.lblCaptionNgayThuCuaBenh.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.lciPatientCase.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.lciPatientCase.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.lblSick.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.lblSick.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.lblCaptionConclude.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.lblCaptionConclude.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.lciChuY.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.layoutControlItem20.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.lblInformationExam.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.lblInformationExam.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.lciKhamToanThan.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.layoutControlItem22.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.lblCaptionDiagnostic.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.lblCaptionDiagnostic.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());

                this.lciIcdTextCause.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.lblIcdCauseText.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.lciKskCode.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.lciKskCode.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.lciKskCode.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.lciKskCode.ToolTip", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.lciIcdTextCause.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.lblIcdCauseText.Tooltip", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.chkEditIcdCause.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_ICD__LCI_CHECK_EDIT_ICD", ResourceLangManager.LanguageUCExamServiceReqExecute, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                this.lciIcdText.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_ICD__LCI_ICD_MAIN", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.lciProvisionalDianosis.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_ICD__LCI_PROVISIONAL_DIAGNOSIS", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.lciProvisionalDianosis.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_ICD__LCI_PROVISIONAL_DIAGNOSIS_TOOLTIP", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.chkEditIcd.Text = Inventec.Common.Resource.Get.Value("IVT_LANGUAGE_KEY__UC_ICD__LCI_CHECK_EDIT_ICD", ResourceLangManager.LanguageUCExamServiceReqExecute, Inventec.Desktop.Common.LanguageManager.LanguageManager.GetCulture());

                this.lblCDPhu.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.layoutControlItem19.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.txtIcdText.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.txtIcdExtraName.Properties.NullValuePrompt", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());

                this.lciMLCT.Text = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.layoutControlItem72.Text", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.lciMLCT.OptionsToolTip.ToolTip = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.llayoutControlItem72.OptionsToolTip.ToolTip", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
                this.lblIsToCalculateEgfr.ToolTip = Inventec.Common.Resource.Get.Value("ExamServiceReqExecuteControl.lblIsToCalculateEgfr.ToolTip", ResourceLangManager.LanguageUCExamServiceReqExecute, LanguageManager.GetCulture());
            
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

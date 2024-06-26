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
using HIS.Desktop.ADO;
using HIS.Desktop.LibraryMessage;
using HIS.Desktop.LocalStorage.BackendData;
using HIS.Desktop.LocalStorage.LocalData;
using HIS.Desktop.Plugins.TreatmentList.Properties;
using HIS.Desktop.Utility;
using Inventec.Desktop.Common.LanguageManager;
using Inventec.Desktop.Common.Message;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.TreatmentList
{
    public partial class UCTreatmentList : UserControlBase
    {
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("Plugins.Resources.Lang", typeof(HIS.Desktop.Plugins.TreatmentList.UCTreatmentList).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                
                this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCTreatmentList.layoutControl1.Text", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.txtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCTreatmentList.txtKeyword.Properties.NullValuePrompt", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.txtPatient.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCTreatmentList.txtPatient.Properties.NullValuePrompt", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.btnRefresh.Text = Inventec.Common.Resource.Get.Value("UCTreatmentList.btnRefresh.Text", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.btnFind.Text = Inventec.Common.Resource.Get.Value("UCTreatmentList.btnFind.Text", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.n.Text = Inventec.Common.Resource.Get.Value("UCTreatmentList.n.Text", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.AREA.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.AREA.Caption", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("UCTreatmentList.layoutControl2.Text", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.InDepartment.Text = Inventec.Common.Resource.Get.Value("UCTreatmentList.InDepartment.Text", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.radioButton1.Text = Inventec.Common.Resource.Get.Value("UCTreatmentList.radioButton1.Text", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.layoutControl5.Text = Inventec.Common.Resource.Get.Value("UCTreatmentList.layoutControl5.Text", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.layoutCreateTimeFrom.Text = Inventec.Common.Resource.Get.Value("UCTreatmentList.layoutCreateTimeFrom.Text", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.layoutCreateTimeTo.Text = Inventec.Common.Resource.Get.Value("UCTreatmentList.layoutCreateTimeTo.Text", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.navBarGroupCreateTime.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.navBarGroupCreateTime.Caption", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.txtTreatment.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCTreatmentList.txtTreatment.Properties.NullValuePrompt", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.grTdlHeinCardNumber.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.gridColumn1.Caption", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.grTdlHeinCardNumber.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentList.gridColumn1.ToolTip", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.grOrder.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.ORDER.Caption", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.grOrder.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentList.ORDER.ToolTip", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.grStatus.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.STATUST.Caption", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.grStatus.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentList.STATUST.ToolTip", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.repositoryItembtnStatus.Caption.Text = Inventec.Common.Resource.Get.Value("UCTreatmentList.repositoryItemPictureEdit1.Caption.Text", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.grTimeLine.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentList.TIME_LINE.ToolTip", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.grBedRoomIn.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.BedRoomIn.Caption", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.grServicePackgeView.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.ServicePackgeView.Caption", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.grFixTreatment.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.fixTreatment.Caption", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.grServiceReq.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.ServiceReq.Caption", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.grServiceReq.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentList.ServiceReq.ToolTip", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.grServiceReqList.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.ServiceReqList.Caption", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.grEditTreatment.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentList.EDIT.ToolTip", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.grMergePatient.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.MergePatient.Caption", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.grMergePatient.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentList.MergePatient.ToolTip", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.grInfantInformation.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.InfantInformation.Caption", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.grAccidentHurt.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.AccidentHurt.Caption", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.grAccidentHurt.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentList.AccidentHurt.ToolTip", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.grFinish.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.finish.Caption", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.grTreatmentSumary.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.TreatmentSumary.Caption", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.grTreatmentRecord.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.TreatmentRecord.Caption", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.grTreatmentRecord.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentList.TreatmentRecord.ToolTip", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.grFeeInfo.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.FEE_INFO.Caption", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.grUnfinish.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.Unfinish.Caption", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.grSarPrintList.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.SarPrintList.Caption", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.grPaySereServ.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.gridColumn2.Caption", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.grPaySereServ.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentList.gridColumn2.ToolTip", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.grTreatmentCode.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.gridColumn6.Caption", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.grTreatmentCode.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentList.gridColumn6.ToolTip", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.grPatientCode.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.PATIENT_CODE.Caption", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.grPatientCode.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentList.PATIENT_CODE.ToolTip", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.grTdlHeinCardNumber.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.TDL_HEIN_CARD_NUMBER.Caption", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.grVirPatientName.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.TDL_PATIENT_NAME.Caption", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.grVirPatientName.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentList.TDL_PATIENT_NAME.ToolTip", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.grDobSt.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.DOB_ST.Caption", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.grDobSt.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentList.DOB_ST.ToolTip", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.grGenderName.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.GENDER_NAME.Caption", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.grGenderName.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentList.GENDER_NAME.ToolTip", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.grVirAddress.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.VIR_ADDRESS.Caption", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.grVirAddress.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentList.VIR_ADDRESS.ToolTip", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.grInTimeStr.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.IN_TIME_ST.Caption", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.grInTimeStr.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentList.IN_TIME_ST.ToolTip", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.grClinicalInTimeSt.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.CLINICAL_IN_TIME_ST.Caption", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.grClinicalInTimeSt.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentList.CLINICAL_IN_TIME_ST.ToolTip", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.grOutTimeSt.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.OUT_TIME_ST.Caption", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.grOutTimeSt.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentList.OUT_TIME_ST.ToolTip", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.grIcdMainText.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.ICD_MAIN_TEXT.Caption", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.grIcdMainText.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentList.ICD_MAIN_TEXT.ToolTip", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.grIcdText.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.ICD_TEXT.Caption", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.grIcdText.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentList.ICD_TEXT.ToolTip", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.grCreateTimeSt.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.CREATE_TIME_ST.Caption", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.grCreateTimeSt.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentList.CREATE_TIME_ST.ToolTip", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.grCreator.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.CREATOR.Caption", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.grCreator.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentList.CREATOR.ToolTip", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.grModifyTimeSt.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.MODIFY_TIME_ST.Caption", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.grModifyTimeSt.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentList.MODIFY_TIME_ST.ToolTip", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.frModifier.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.MODIFIER.Caption", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.frModifier.ToolTip = Inventec.Common.Resource.Get.Value("UCTreatmentList.MODIFIER.ToolTip", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.grColPrint.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.gridColPrint.Caption", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.layoutViewColumn1.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.layoutViewColumn1.Caption", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.layoutViewColumn2.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.layoutViewColumn2.Caption", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.layoutViewColumn6.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.layoutViewColumn6.Caption", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.layoutViewColumn7.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.layoutViewColumn7.Caption", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.layoutViewColumn8.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.layoutViewColumn8.Caption", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.layoutViewColumn9.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.layoutViewColumn9.Caption", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.layoutViewColumn10.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.layoutViewColumn10.Caption", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.layoutViewColumn11.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.layoutViewColumn11.Caption", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.layoutViewColumn12.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.layoutViewColumn12.Caption", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.layoutViewColumn13.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.layoutViewColumn13.Caption", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.layoutViewColumn14.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.layoutViewColumn14.Caption", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.layoutViewColumn15.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.layoutViewColumn15.Caption", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.layoutViewColumn16.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.layoutViewColumn16.Caption", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.layoutViewColumn17.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.layoutViewColumn17.Caption", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.layoutViewColumn18.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.layoutViewColumn18.Caption", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.layoutViewColumn19.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.layoutViewColumn19.Caption", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.layoutViewColumn20.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.layoutViewColumn20.Caption", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("UCTreatmentList.layoutControlItem2.Text", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.layoutControlItem13.Text = Inventec.Common.Resource.Get.Value("UCTreatmentList.layoutControlItem13.Text", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.a.Text = Inventec.Common.Resource.Get.Value("UCTreatmentList.a.Text", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.layoutControlItem12.Text = Inventec.Common.Resource.Get.Value("UCTreatmentList.layoutControlItem12.Text", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                //this.bar4.Text = Inventec.Common.Resource.Get.Value("UCTreatmentList.bar4.Text", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                //this.bar6.Text = Inventec.Common.Resource.Get.Value("UCTreatmentList.bar6.Text", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
                this.grTreatmentEndTypeName.Caption = Inventec.Common.Resource.Get.Value("UCTreatmentList.gridColumn4.Caption", Base.ResourceLangManager.LanguageUCTreatmentList, LanguageManager.GetCulture());
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

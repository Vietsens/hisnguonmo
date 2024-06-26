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
using HIS.Desktop.Plugins.HisContactPointList.Properties;
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

namespace HIS.Desktop.Plugins.HisContactPointList
{
    public partial class UCHisContactPointList : UserControlBase
    {
        private void SetCaptionByLanguageKey()
        {
            try
            {
                ////Khoi tao doi tuong resource
                Resources.ResourceLanguageManager.LanguageResource = new ResourceManager("Plugins.Resources.Lang", typeof(HIS.Desktop.Plugins.HisContactPointList.UCHisContactPointList).Assembly);

                ////Gan gia tri cho cac control editor co Text/Caption/ToolTip/NullText/NullValuePrompt/FindNullPrompt
                
                //this.layoutControl1.Text = Inventec.Common.Resource.Get.Value("UCHisContactPointList.layoutControl1.Text", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.txtKeyword.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCHisContactPointList.txtKeyword.Properties.NullValuePrompt", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.txtPatientCode.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCHisContactPointList.txtPatient.Properties.NullValuePrompt", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.btnRefresh.Text = Inventec.Common.Resource.Get.Value("UCHisContactPointList.btnRefresh.Text", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.btnFind.Text = Inventec.Common.Resource.Get.Value("UCHisContactPointList.btnFind.Text", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.n.Text = Inventec.Common.Resource.Get.Value("UCHisContactPointList.n.Text", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.AREA.Caption = Inventec.Common.Resource.Get.Value("UCHisContactPointList.AREA.Caption", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.layoutControl2.Text = Inventec.Common.Resource.Get.Value("UCHisContactPointList.layoutControl2.Text", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //thirdBenhNhannt.Text = Inventec.Common.Resource.Get.Value("UCHisContactPointList.InDepartment.Text", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //thirdNhanVienn1.Text = Inventec.Common.Resource.Get.Value("UCHisContactPointList.radioButton1.Text", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                ////this.layoutControl5.Text = Inventec.Common.Resource.Get.Value("UCHisContactPointList.layoutControl5.Text", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                ////this.layoutCreateTimeFrom.Text = Inventec.Common.Resource.Get.Value("UCHisContactPointList.layoutCreateTimeFrom.Text", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                ////this.layoutCreateTimeTo.Text = Inventec.Common.Resource.Get.Value("UCHisContactPointList.layoutCreateTimeTo.Text", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                ////this.navBarGroupCreateTime.Caption = Inventec.Common.Resource.Get.Value("UCHisContactPointList.navBarGroupCreateTime.Caption", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                ////this.txtTreatment.Properties.NullValuePrompt = Inventec.Common.Resource.Get.Value("UCHisContactPointList.txtTreatment.Properties.NullValuePrompt", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.grTdlHeinCardNumber.Caption = Inventec.Common.Resource.Get.Value("UCHisContactPointList.gridColumn1.Caption", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.grTdlHeinCardNumber.ToolTip = Inventec.Common.Resource.Get.Value("UCHisContactPointList.gridColumn1.ToolTip", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.grOrder.Caption = Inventec.Common.Resource.Get.Value("UCHisContactPointList.ORDER.Caption", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.grOrder.ToolTip = Inventec.Common.Resource.Get.Value("UCHisContactPointList.ORDER.ToolTip", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.grStatus.Caption = Inventec.Common.Resource.Get.Value("UCHisContactPointList.STATUST.Caption", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.grStatus.ToolTip = Inventec.Common.Resource.Get.Value("UCHisContactPointList.STATUST.ToolTip", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.repositoryItembtnStatus.Caption.Text = Inventec.Common.Resource.Get.Value("UCHisContactPointList.repositoryItemPictureEdit1.Caption.Text", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.grTimeLine.ToolTip = Inventec.Common.Resource.Get.Value("UCHisContactPointList.TIME_LINE.ToolTip", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.grBedRoomIn.Caption = Inventec.Common.Resource.Get.Value("UCHisContactPointList.BedRoomIn.Caption", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.grServicePackgeView.Caption = Inventec.Common.Resource.Get.Value("UCHisContactPointList.ServicePackgeView.Caption", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.grFixTreatment.Caption = Inventec.Common.Resource.Get.Value("UCHisContactPointList.fixTreatment.Caption", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.grServiceReq.Caption = Inventec.Common.Resource.Get.Value("UCHisContactPointList.ServiceReq.Caption", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.grServiceReq.ToolTip = Inventec.Common.Resource.Get.Value("UCHisContactPointList.ServiceReq.ToolTip", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.grServiceReqList.Caption = Inventec.Common.Resource.Get.Value("UCHisContactPointList.ServiceReqList.Caption", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.grEditTreatment.ToolTip = Inventec.Common.Resource.Get.Value("UCHisContactPointList.EDIT.ToolTip", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.grMergePatient.Caption = Inventec.Common.Resource.Get.Value("UCHisContactPointList.MergePatient.Caption", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.grMergePatient.ToolTip = Inventec.Common.Resource.Get.Value("UCHisContactPointList.MergePatient.ToolTip", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.grInfantInformation.Caption = Inventec.Common.Resource.Get.Value("UCHisContactPointList.InfantInformation.Caption", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.grAccidentHurt.Caption = Inventec.Common.Resource.Get.Value("UCHisContactPointList.AccidentHurt.Caption", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.grAccidentHurt.ToolTip = Inventec.Common.Resource.Get.Value("UCHisContactPointList.AccidentHurt.ToolTip", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.grFinish.Caption = Inventec.Common.Resource.Get.Value("UCHisContactPointList.finish.Caption", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.grTreatmentSumary.Caption = Inventec.Common.Resource.Get.Value("UCHisContactPointList.TreatmentSumary.Caption", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.grTreatmentRecord.Caption = Inventec.Common.Resource.Get.Value("UCHisContactPointList.TreatmentRecord.Caption", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.grTreatmentRecord.ToolTip = Inventec.Common.Resource.Get.Value("UCHisContactPointList.TreatmentRecord.ToolTip", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.grFeeInfo.Caption = Inventec.Common.Resource.Get.Value("UCHisContactPointList.FEE_INFO.Caption", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.grUnfinish.Caption = Inventec.Common.Resource.Get.Value("UCHisContactPointList.Unfinish.Caption", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.grSarPrintList.Caption = Inventec.Common.Resource.Get.Value("UCHisContactPointList.SarPrintList.Caption", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.grPaySereServ.Caption = Inventec.Common.Resource.Get.Value("UCHisContactPointList.gridColumn2.Caption", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.grPaySereServ.ToolTip = Inventec.Common.Resource.Get.Value("UCHisContactPointList.gridColumn2.ToolTip", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.grTreatmentCode.Caption = Inventec.Common.Resource.Get.Value("UCHisContactPointList.gridColumn6.Caption", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.grTreatmentCode.ToolTip = Inventec.Common.Resource.Get.Value("UCHisContactPointList.gridColumn6.ToolTip", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.grPatientCode.Caption = Inventec.Common.Resource.Get.Value("UCHisContactPointList.PATIENT_CODE.Caption", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.grPatientCode.ToolTip = Inventec.Common.Resource.Get.Value("UCHisContactPointList.PATIENT_CODE.ToolTip", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.grTdlHeinCardNumber.Caption = Inventec.Common.Resource.Get.Value("UCHisContactPointList.TDL_HEIN_CARD_NUMBER.Caption", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.grVirPatientName.Caption = Inventec.Common.Resource.Get.Value("UCHisContactPointList.TDL_PATIENT_NAME.Caption", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.grVirPatientName.ToolTip = Inventec.Common.Resource.Get.Value("UCHisContactPointList.TDL_PATIENT_NAME.ToolTip", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.grDobSt.Caption = Inventec.Common.Resource.Get.Value("UCHisContactPointList.DOB_ST.Caption", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.grDobSt.ToolTip = Inventec.Common.Resource.Get.Value("UCHisContactPointList.DOB_ST.ToolTip", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.grGenderName.Caption = Inventec.Common.Resource.Get.Value("UCHisContactPointList.GENDER_NAME.Caption", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.grGenderName.ToolTip = Inventec.Common.Resource.Get.Value("UCHisContactPointList.GENDER_NAME.ToolTip", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.grVirAddress.Caption = Inventec.Common.Resource.Get.Value("UCHisContactPointList.VIR_ADDRESS.Caption", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.grVirAddress.ToolTip = Inventec.Common.Resource.Get.Value("UCHisContactPointList.VIR_ADDRESS.ToolTip", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.grInTimeStr.Caption = Inventec.Common.Resource.Get.Value("UCHisContactPointList.IN_TIME_ST.Caption", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.grInTimeStr.ToolTip = Inventec.Common.Resource.Get.Value("UCHisContactPointList.IN_TIME_ST.ToolTip", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.grClinicalInTimeSt.Caption = Inventec.Common.Resource.Get.Value("UCHisContactPointList.CLINICAL_IN_TIME_ST.Caption", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.grClinicalInTimeSt.ToolTip = Inventec.Common.Resource.Get.Value("UCHisContactPointList.CLINICAL_IN_TIME_ST.ToolTip", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.grOutTimeSt.Caption = Inventec.Common.Resource.Get.Value("UCHisContactPointList.OUT_TIME_ST.Caption", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.grOutTimeSt.ToolTip = Inventec.Common.Resource.Get.Value("UCHisContactPointList.OUT_TIME_ST.ToolTip", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.grIcdMainText.Caption = Inventec.Common.Resource.Get.Value("UCHisContactPointList.ICD_MAIN_TEXT.Caption", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.grIcdMainText.ToolTip = Inventec.Common.Resource.Get.Value("UCHisContactPointList.ICD_MAIN_TEXT.ToolTip", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.grIcdText.Caption = Inventec.Common.Resource.Get.Value("UCHisContactPointList.ICD_TEXT.Caption", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.grIcdText.ToolTip = Inventec.Common.Resource.Get.Value("UCHisContactPointList.ICD_TEXT.ToolTip", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.grCreateTimeSt.Caption = Inventec.Common.Resource.Get.Value("UCHisContactPointList.CREATE_TIME_ST.Caption", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.grCreateTimeSt.ToolTip = Inventec.Common.Resource.Get.Value("UCHisContactPointList.CREATE_TIME_ST.ToolTip", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.grCreator.Caption = Inventec.Common.Resource.Get.Value("UCHisContactPointList.CREATOR.Caption", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.grCreator.ToolTip = Inventec.Common.Resource.Get.Value("UCHisContactPointList.CREATOR.ToolTip", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.grModifyTimeSt.Caption = Inventec.Common.Resource.Get.Value("UCHisContactPointList.MODIFY_TIME_ST.Caption", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.grModifyTimeSt.ToolTip = Inventec.Common.Resource.Get.Value("UCHisContactPointList.MODIFY_TIME_ST.ToolTip", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.frModifier.Caption = Inventec.Common.Resource.Get.Value("UCHisContactPointList.MODIFIER.Caption", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.frModifier.ToolTip = Inventec.Common.Resource.Get.Value("UCHisContactPointList.MODIFIER.ToolTip", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.grColPrint.Caption = Inventec.Common.Resource.Get.Value("UCHisContactPointList.gridColPrint.Caption", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.layoutViewColumn1.Caption = Inventec.Common.Resource.Get.Value("UCHisContactPointList.layoutViewColumn1.Caption", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.layoutViewColumn2.Caption = Inventec.Common.Resource.Get.Value("UCHisContactPointList.layoutViewColumn2.Caption", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.layoutViewColumn6.Caption = Inventec.Common.Resource.Get.Value("UCHisContactPointList.layoutViewColumn6.Caption", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.layoutViewColumn7.Caption = Inventec.Common.Resource.Get.Value("UCHisContactPointList.layoutViewColumn7.Caption", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.layoutViewColumn8.Caption = Inventec.Common.Resource.Get.Value("UCHisContactPointList.layoutViewColumn8.Caption", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.layoutViewColumn9.Caption = Inventec.Common.Resource.Get.Value("UCHisContactPointList.layoutViewColumn9.Caption", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.layoutViewColumn10.Caption = Inventec.Common.Resource.Get.Value("UCHisContactPointList.layoutViewColumn10.Caption", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.layoutViewColumn11.Caption = Inventec.Common.Resource.Get.Value("UCHisContactPointList.layoutViewColumn11.Caption", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.layoutViewColumn12.Caption = Inventec.Common.Resource.Get.Value("UCHisContactPointList.layoutViewColumn12.Caption", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.layoutViewColumn13.Caption = Inventec.Common.Resource.Get.Value("UCHisContactPointList.layoutViewColumn13.Caption", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.layoutViewColumn14.Caption = Inventec.Common.Resource.Get.Value("UCHisContactPointList.layoutViewColumn14.Caption", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.layoutViewColumn15.Caption = Inventec.Common.Resource.Get.Value("UCHisContactPointList.layoutViewColumn15.Caption", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.layoutViewColumn16.Caption = Inventec.Common.Resource.Get.Value("UCHisContactPointList.layoutViewColumn16.Caption", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.layoutViewColumn17.Caption = Inventec.Common.Resource.Get.Value("UCHisContactPointList.layoutViewColumn17.Caption", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.layoutViewColumn18.Caption = Inventec.Common.Resource.Get.Value("UCHisContactPointList.layoutViewColumn18.Caption", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.layoutViewColumn19.Caption = Inventec.Common.Resource.Get.Value("UCHisContactPointList.layoutViewColumn19.Caption", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.layoutViewColumn20.Caption = Inventec.Common.Resource.Get.Value("UCHisContactPointList.layoutViewColumn20.Caption", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.layoutControlItem2.Text = Inventec.Common.Resource.Get.Value("UCHisContactPointList.layoutControlItem2.Text", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.layoutControlItem13.Text = Inventec.Common.Resource.Get.Value("UCHisContactPointList.layoutControlItem13.Text", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.a.Text = Inventec.Common.Resource.Get.Value("UCHisContactPointList.a.Text", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.layoutControlItem12.Text = Inventec.Common.Resource.Get.Value("UCHisContactPointList.layoutControlItem12.Text", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());
                //this.grTreatmentEndTypeName.Caption = Inventec.Common.Resource.Get.Value("UCHisContactPointList.gridColumn4.Caption", Base.ResourceLangManager.LanguageUCHisContactPointList, LanguageManager.GetCulture());

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

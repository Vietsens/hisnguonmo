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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using MOS.EFMODEL.DataModels;
using HIS.UC.Death;
using HIS.UC.TranPati;
using HIS.UC.TranPati.ADO;
using HIS.UC.Death.ADO;
using HIS.UC.ExamTreatmentFinish.ADO;
using MOS.SDO;
using HIS.UC.SecondaryIcd.ADO;
using HIS.UC.SurgeryAppointment.ADO;
using Inventec.Common.Logging;

namespace HIS.UC.ExamTreatmentFinish.Run
{
    public partial class UCExamTreatmentFinish : UserControl
    {
        public object GetValue()
        {
            ExamTreatmentFinishResult ExamTreatmentFinish = null;
            try
            {
                LogSystem.Debug("UCExamTreatmentFinish.GetValue. Begin");
                LogSystem.Debug("CboTreatmentEndType.EditValue: " + cboTreatmentEndType.EditValue);
                LogSystem.Debug("CboTreatmentEndTypeExt.Text: " + cboTreatmentEndTypeExt.EditValue);
                this.positionHandle = -1;
                var valid = dxValidationProvider1.Validate();
                if (ucSecondaryIcd != null)
                    valid = valid && (bool)subIcdProcessor.GetValidate(ucSecondaryIcd);
                if (!valid)
                    return null;
                ExamTreatmentFinish = new ExamTreatmentFinishResult();
                HisTreatmentFinishSDO ExamTreatmentFinishSDO = new HisTreatmentFinishSDO();
                if (cboCareer.EditValue != null)
                {
                    long? careerId = Inventec.Common.TypeConvert.Parse.ToInt64(cboCareer.EditValue.ToString());
                    ExamTreatmentFinishSDO.CareerId = careerId > 0 ? careerId : null;
                }
                if (cboTreatmentEndType.EditValue != null)
                {
                    ExamTreatmentFinishSDO.TreatmentEndTypeId = Inventec.Common.TypeConvert.Parse.ToInt64(cboTreatmentEndType.EditValue.ToString());
                }

                if (cboTreatmentResult.EditValue != null)
                {
                    ExamTreatmentFinishSDO.TreatmentResultId = Inventec.Common.TypeConvert.Parse.ToInt64(cboTreatmentResult.EditValue.ToString());
                }

                if (cboTreatmentEndTypeExt.EditValue != null)
                {
                    ExamTreatmentFinishSDO.TreatmentEndTypeExtId = Inventec.Common.TypeConvert.Parse.ToInt64(cboTreatmentEndTypeExt.EditValue.ToString());
                }

                if (ExamTreatmentFinishSDO.TreatmentEndTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHUYEN)
                {
                    //HisTreatmentFinishSDO tranPati = tranPatiProcessor.GetValue(this.ucTranPati) as HisTreatmentFinishSDO;
                    if (currentTreatmentFinishSDO == null)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceMessage.ChuaNhapThongTinChuyenVien,
                           Resources.ResourceMessage.ThongBao,
                           MessageBoxButtons.OK);

                        if (treatment == null)
                        {
                            treatment = ExamTreatmentFinishInitADO.Treatment;
                        }

                        EndTypeForm.FormTransfer form = new EndTypeForm.FormTransfer(this.moduleData, treatment, UpdateExamTreatmentFinish,this._treatmentext);
                        form.ShowDialog();
                        return null;
                    }

                    AutoMapper.Mapper.CreateMap<HisTreatmentFinishSDO, HisTreatmentFinishSDO>();
                    ExamTreatmentFinishSDO = AutoMapper.Mapper.Map<HisTreatmentFinishSDO, HisTreatmentFinishSDO>(currentTreatmentFinishSDO);

                    if (cboTreatmentEndTypeExt.EditValue != null)
                    {
                        ExamTreatmentFinishSDO.TreatmentEndTypeExtId = Inventec.Common.TypeConvert.Parse.ToInt64(cboTreatmentEndTypeExt.EditValue.ToString());
                    }
                }
                else if (ExamTreatmentFinishSDO.TreatmentEndTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHET)
                {
                    HisTreatmentFinishSDO death = deathSdoResult; //deathProcessor.GetValue(this.ucDeath) as HisTreatmentFinishSDO;
                    if (death == null)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceMessage.ChuaNhapThongTinTuVong,
                           Resources.ResourceMessage.ThongBao,
                           MessageBoxButtons.OK);
                        return null;
                    }

                    AutoMapper.Mapper.CreateMap<HisTreatmentFinishSDO, HisTreatmentFinishSDO>();
                    ExamTreatmentFinishSDO = AutoMapper.Mapper.Map<HisTreatmentFinishSDO, HisTreatmentFinishSDO>(death);
                }
                else if (ExamTreatmentFinishSDO.TreatmentEndTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__HEN)
                {
                    //ExamTreatmentFinishSDO.AppointmentTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtTimeAppointment.DateTime);
                    //if (this._ucAdvise != null)
                    //{
                    //    if (!this._ucAdvise.GetValidate())
                    //        return null;
                    //    var adoAd = (AdviseADO)this._ucAdvise.GetValue();
                    //    ExamTreatmentFinishSDO.Advise = adoAd.Advise;
                    //    ExamTreatmentFinishSDO.AppointmentExamRoomIds = adoAd.ExamRoomIds;
                    //}

                    if (currentTreatmentFinishSDO == null)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(Resources.ResourceMessage.ChuaNhapThoiGianHenKham,
                           Resources.ResourceMessage.ThongBao,
                           MessageBoxButtons.OK);

                        if (treatment == null)
                        {
                            treatment = ExamTreatmentFinishInitADO.Treatment;
                        }

                        //check mặc định phòng khám lần sau là phòng hiện tại
                        if (String.IsNullOrWhiteSpace(treatment.APPOINTMENT_EXAM_ROOM_IDS))
                        {
                            treatment.APPOINTMENT_EXAM_ROOM_IDS = this.ExamTreatmentFinishInitADO.moduleData.RoomId.ToString();
                        }

                        treatment.OUT_TIME = Inventec.Common.TypeConvert.Parse.ToInt64(dtEndTime.DateTime.ToString("yyyyMMddHHmmss"));
                        EndTypeForm.FormAppointment form = new EndTypeForm.FormAppointment(treatment, UpdateExamTreatmentFinish, ExamTreatmentFinishInitADO.IsBlockNumOrder);
                        form.ShowDialog();
                        return null;
                    }

                    AutoMapper.Mapper.CreateMap<HisTreatmentFinishSDO, HisTreatmentFinishSDO>();
                    ExamTreatmentFinishSDO = AutoMapper.Mapper.Map<HisTreatmentFinishSDO, HisTreatmentFinishSDO>(currentTreatmentFinishSDO);
                    ExamTreatmentFinishSDO.NumOrderBlockId = currentTreatmentFinishSDO.NumOrderBlockId;
                    if (cboTreatmentEndTypeExt.EditValue != null)
                    {
                        ExamTreatmentFinishSDO.TreatmentEndTypeExtId = Inventec.Common.TypeConvert.Parse.ToInt64(cboTreatmentEndTypeExt.EditValue.ToString());
                    }
                }

                if (ExamTreatmentFinishSDO.TreatmentEndTypeExtId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__NGHI_DUONG_THAI)
                {
                    HisTreatmentFinishSDO sick = sickSdoResult; //sickProcessor.GetValue(this.ucSick) as HisTreatmentFinishSDO;
                    // sick.TreatmentEndTypeExtId = ExamTreatmentFinishSDO.TreatmentEndTypeExtId;
                    sick.TreatmentEndTypeExtId = ExamTreatmentFinishSDO.TreatmentEndTypeExtId;
                    //sick.TreatmentEndTypeId = ExamTreatmentFinishSDO.TreatmentEndTypeId;
                    //AutoMapper.Mapper.CreateMap<HisTreatmentFinishSDO, HisTreatmentFinishSDO>();
                    //ExamTreatmentFinishSDO = AutoMapper.Mapper.Map<HisTreatmentFinishSDO, HisTreatmentFinishSDO>(sick);
                    ExamTreatmentFinishSDO.SickHeinCardNumber = sick.SickHeinCardNumber;
                    ExamTreatmentFinishSDO.SickLeaveDay = sick.SickLeaveDay;
                    ExamTreatmentFinishSDO.SickLeaveFrom = sick.SickLeaveFrom;
                    ExamTreatmentFinishSDO.SickLeaveTo = sick.SickLeaveTo;
                    ExamTreatmentFinishSDO.PatientRelativeName = sick.PatientRelativeName;
                    ExamTreatmentFinishSDO.PatientRelativeType = sick.PatientRelativeType;
                    ExamTreatmentFinishSDO.PatientWorkPlace = sick.PatientWorkPlace;
                    ExamTreatmentFinishSDO.Babies = sick.Babies;
                    ExamTreatmentFinishSDO.SickLoginname = sick.SickLoginname;
                    ExamTreatmentFinishSDO.SickUsername = sick.SickUsername;
                    ExamTreatmentFinishSDO.WorkPlaceId = sick.WorkPlaceId;
                    ExamTreatmentFinishSDO.SocialInsuranceNumber = sick.SocialInsuranceNumber;
                    ExamTreatmentFinishSDO.EndTypeExtNote = sick.EndTypeExtNote;
                }
                else if (ExamTreatmentFinishSDO.TreatmentEndTypeExtId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__HEN_MO)
                {
                    SurgAppointmentADO surgery = surSdoResult;// this.surgeryAppointmentProcessor.GetValuePlus(this.ucSurgeryAppointment) as SurgAppointmentADO;
                    //surgery.TreatmentEndTypeExtId = ExamTreatmentFinishSDO.TreatmentEndTypeExtId;
                    //surgery.TreatmentEndTypeId = ExamTreatmentFinishSDO.TreatmentEndTypeId;

                    //AutoMapper.Mapper.CreateMap<SurgAppointmentADO, HisTreatmentFinishSDO>();
                    //ExamTreatmentFinishSDO = AutoMapper.Mapper.Map<SurgAppointmentADO, HisTreatmentFinishSDO>(surgery);
                    ExamTreatmentFinishSDO.AppointmentSurgery = surgery.AppointmentSurgery;
                    ExamTreatmentFinishSDO.Surgery = surgery.Surgery;
                    ExamTreatmentFinishSDO.SurgeryAppointmentTime = surgery.SurgeryAppointmentTime;
                    ExamTreatmentFinish.IsPrintSurgAppoint = surgery.IsPrintSurgAppoint;
                    ExamTreatmentFinishSDO.SocialInsuranceNumber = surgery.SocialInsuranceNumber;
                }
                else if (ExamTreatmentFinishSDO.TreatmentEndTypeExtId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE_EXT.ID__NGHI_OM)
                {
                    HisTreatmentFinishSDO sick = sickSdoResult;// sickProcessor.GetValue(this.ucSick) as HisTreatmentFinishSDO;
                    //treatment sick
                    ExamTreatmentFinishSDO.SickLeaveDay = sick.SickLeaveDay;
                    ExamTreatmentFinishSDO.SickLeaveFrom = sick.SickLeaveFrom;
                    ExamTreatmentFinishSDO.SickLeaveTo = sick.SickLeaveTo;
                    ExamTreatmentFinishSDO.PatientRelativeName = sick.PatientRelativeName;
                    ExamTreatmentFinishSDO.PatientRelativeType = sick.PatientRelativeType;
                    //ExamTreatmentFinishSDO.TreatmentEndTypeExtId = sick.TreatmentEndTypeExtId;
                    ExamTreatmentFinishSDO.Babies = sick.Babies;
                    ExamTreatmentFinishSDO.PatientWorkPlace = sick.PatientWorkPlace;
                    ExamTreatmentFinishSDO.SickHeinCardNumber = sick.SickHeinCardNumber;
                    ExamTreatmentFinishSDO.SickLoginname = sick.SickLoginname;
                    ExamTreatmentFinishSDO.SickUsername = sick.SickUsername;
                    ExamTreatmentFinishSDO.DocumentBookId = sick.DocumentBookId;
                    ExamTreatmentFinishSDO.WorkPlaceId = sick.WorkPlaceId;
                    ExamTreatmentFinishSDO.SocialInsuranceNumber = sick.SocialInsuranceNumber;
                    ExamTreatmentFinishSDO.EndTypeExtNote = sick.EndTypeExtNote;
                    ExamTreatmentFinishSDO.IsPregnancyTermination = sick.IsPregnancyTermination;
                    ExamTreatmentFinishSDO.PregnancyTerminationReason = sick.PregnancyTerminationReason;
                    ExamTreatmentFinishSDO.TreatmentMethod = sick.TreatmentMethod;
                    ExamTreatmentFinishSDO.GestationalAge = sick.GestationalAge;
                    ExamTreatmentFinishSDO.PregnancyTerminationTime = sick.PregnancyTerminationTime;
                    ExamTreatmentFinishSDO.MotherName = sick.MotherName;
                    ExamTreatmentFinishSDO.FatherName = sick.FatherName;
                }

                ExamTreatmentFinishSDO.TreatmentFinishTime = Inventec.Common.DateTime.Convert.SystemDateTimeToTimeNumber(dtEndTime.DateTime) ?? 0;
                ExamTreatmentFinishSDO.TreatmentId = this.ExamTreatmentFinishInitADO.Treatment != null ? this.ExamTreatmentFinishInitADO.Treatment.ID : 0;
                ExamTreatmentFinishSDO.TreatmentEndTypeId = Inventec.Common.TypeConvert.Parse.ToInt64(cboTreatmentEndType.EditValue.ToString());
                if (cboTreatmentResult.EditValue != null)
                {
                    ExamTreatmentFinishSDO.TreatmentResultId = Inventec.Common.TypeConvert.Parse.ToInt64(cboTreatmentResult.EditValue.ToString());
                }
                if (this.currentShowICDInformation != null && !this.ExamTreatmentFinishInitADO.IsAutoSetIcdWhenFinishInOtherExam)
                {
                    ExamTreatmentFinishSDO.ShowIcdCode = this.currentShowICDInformation.SHOW_ICD_CODE;
                    ExamTreatmentFinishSDO.ShowIcdName = this.currentShowICDInformation.SHOW_ICD_NAME;
                    ExamTreatmentFinishSDO.ShowIcdSubCode = this.currentShowICDInformation.SHOW_ICD_SUB_CODE;
                    ExamTreatmentFinishSDO.ShowIcdText = this.currentShowICDInformation.SHOW_ICD_TEXT;
                }

                if (chkIsExpXml4210Collinear.Checked)
                {
                    ExamTreatmentFinishSDO.IsExpXml4210Collinear = true;
                }
                ExamTreatmentFinish.SevereIllNessInfo = causeResult != null ? causeResult.SevereIllNessInfo : null;
                ExamTreatmentFinish.ListEventsCausesDeath = causeResult != null ? causeResult.ListEventsCausesDeath : null;
                if (cboHospSubs.EditValue != null)
                {
                    ExamTreatmentFinishSDO.HospSubsDirectorLoginname = cboHospSubs.EditValue.ToString();
                    ExamTreatmentFinishSDO.HospSubsDirectorUsername = cboHospSubs.Text.ToString();
                }

                if (cboEndDeptSubs.EditValue != null)
                {
                    ExamTreatmentFinishSDO.EndDeptSubsHeadLoginname = cboEndDeptSubs.EditValue.ToString();
                    ExamTreatmentFinishSDO.EndDeptSubsHeadUsername = cboEndDeptSubs.Text.ToString();
                }
                ExamTreatmentFinish.TreatmentFinishSDO = ExamTreatmentFinishSDO;
                ExamTreatmentFinish.IsPrintBANT = chkBANT.Checked;
                ExamTreatmentFinishSDO.CreateOutPatientMediRecord = chkCapSoLuuTruBA.CheckState == CheckState.Checked;
                if (chkSignExam.Checked)
                {
                    ExamTreatmentFinish.IsSignExam = true;
                }
                if (chkPrintExam.Checked)
                {
                    ExamTreatmentFinish.IsPrintExam = true;
                }

                if (chkPrintAppoinment.Checked)
                {
                    ExamTreatmentFinish.IsPrintAppoinment = true;
                }

                if (chkPrintBordereau.Checked)
                {
                    ExamTreatmentFinish.IsPrintBordereau = true;
                }
                if (chkSignAppoinment.Checked)
                {
                    ExamTreatmentFinish.IsSignAppoinment = true;
                }

                if (chkSignBordereau.Checked)
                {
                    ExamTreatmentFinish.IsSignBordereau = true;
                }

                if (chkPrintBHXH.Checked)
                {
                    ExamTreatmentFinish.IsPrintBHXH = true;
                }

                if (chkSignBHXH.Checked)
                {
                    ExamTreatmentFinish.IsSignBHXH = true;
                }

                if (chkKyPhieuTrichLuc.Checked)
                {
                    ExamTreatmentFinish.IsSignTrichPhuLuc = true;
                }
                if (chkInPhieuTrichLuc.Checked)
                {
                    ExamTreatmentFinish.IsPrintTrichPhuLuc = true;
                }
                if (chkPrintPrescription.Checked)
                {
                    ExamTreatmentFinish.IsPrintPrescription = true;
                }
                if (chkPrintHosTransfer.Checked && IsVisibleHosTransfer)
                {
                    ExamTreatmentFinish.IsPrintHosTransfer = true;
                }

                if (cboProgram.EditValue != null)
                {
                    ExamTreatmentFinishSDO.ProgramId = Inventec.Common.TypeConvert.Parse.ToInt64(cboProgram.EditValue.ToString());
                }

                ExamTreatmentFinish.icdADOInTreatment = this.UcIcdGetValue() as HIS.UC.Icd.ADO.IcdInputADO;
                ExamTreatmentFinish.icdSubADOInTreatment = this.UcSecondaryIcdGetValue() as SecondaryIcdDataADO;
                ExamTreatmentFinish.traditionalIcdTreatment = this.UcTraditionalIcdGetValue() as HIS.UC.Icd.ADO.IcdInputADO;
                ExamTreatmentFinish.traditionInIcdSub = this.UcSecondaryIcdGetValue() as SecondaryIcdDataADO;
                ExamTreatmentFinish.Advise = txtAdviseNew.Text;
                ExamTreatmentFinish.Conclusion = txtConclusionNew.Text;
                ExamTreatmentFinish.Note = memNote.Text.Trim();
                //ExamTreatmentFinish.TreatmentFinishSDO.ClinicalNote = this._treatmentext.CLINICAL_NOTE;
                if (ucSecondaryIcd != null)
                {
                    var subIcd = subIcdProcessor.GetValue(ucSecondaryIcd);
                    if (subIcd != null && subIcd is SecondaryIcdDataADO)
                    {
                        ExamTreatmentFinish.TreatmentFinishSDO.IcdSubCode = ((SecondaryIcdDataADO)subIcd).ICD_SUB_CODE;
                        ExamTreatmentFinish.TreatmentFinishSDO.IcdText = ((SecondaryIcdDataADO)subIcd).ICD_TEXT;
                    }
                }
                LogSystem.Debug("UCExamTreatmentFinish.GetValue. End: \n" + LogUtil.TraceData("ExamTreatmentFinish", ExamTreatmentFinish));
            }
            catch (Exception ex)
            {
                ExamTreatmentFinish = null;
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return ExamTreatmentFinish;
        }
    }
}

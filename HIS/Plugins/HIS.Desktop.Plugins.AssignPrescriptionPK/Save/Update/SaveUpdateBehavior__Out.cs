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
using DevExpress.XtraEditors;
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.AssignPrescriptionPK.AssignPrescription;
using HIS.Desktop.Plugins.AssignPrescriptionPK.Resources;
using Inventec.Common.Logging;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignPrescriptionPK.Save.Update
{
    partial class SaveUpdateBehavior : SaveAbstract, ISave
    {
        object Run__Out()
        {
            OutPatientPresResultSDO result = null;
            if (this.CheckValid() && OldServiceReq != null)
            {
                frmAssignPrescription.VerifyWarningOverCeiling();
                this.InitBase();

                OutPatientPresSDO prescriptionSDO = new OutPatientPresSDO();
                ProcessData(ref prescriptionSDO, (UseTimes != null && UseTimes.Count > 0) ? this.UseTimes.FirstOrDefault() : 0);                
                if (prescriptionSDO.TreatmentFinishSDO != null && prescriptionSDO.TreatmentFinishSDO.TreatmentEndTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHET && (string.IsNullOrEmpty(prescriptionSDO.TreatmentFinishSDO.MainCause) || prescriptionSDO.TreatmentFinishSDO.DeathCauseId == null || prescriptionSDO.TreatmentFinishSDO.DeathTime == null || prescriptionSDO.TreatmentFinishSDO.DeathWithinId == null))
                {
                    frmAssignPrescription.msgTuVong = "Bạn chưa nhập đủ thông tin tử vong, vui lòng kiểm tra lại";
                    return result;
                }


                LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => prescriptionSDO), prescriptionSDO));
                //LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("UPDATE____" + Inventec.Common.Logging.LogUtil.GetMemberName(() => listInput), listInput));
                result = new Inventec.Common.Adapter.BackendAdapter(Param).Post<OutPatientPresResultSDO>(RequestUriStore.HIS_SERVICE_REQ__OUTPATIENT_PRES_UPDATE, ApiConsumers.MosConsumer, prescriptionSDO, Param);
                if (result == null
                    || result.ServiceReqs == null || result.ServiceReqs.Count == 0
                    || ((result.ServiceReqMaties == null || result.ServiceReqMaties.Count == 0)
                        && (result.ServiceReqMeties == null || result.ServiceReqMeties.Count == 0)
                        && (result.Materials == null || result.Materials.Count == 0)
                        && (result.Medicines == null || result.Medicines.Count == 0))
                    )
                {
                    Inventec.Common.Logging.LogSystem.Debug("Goi api sua don thuoc that bai. Du lieu dau vao____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => prescriptionSDO), prescriptionSDO) + ". Du lieu dau ra____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => result), result) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Param), Param));
                    result = null;
                }
                else if (this.ProgramId > 0 && result.MediRecord != null)
                {
                    string storeCode = result.MediRecord.STORE_CODE;
                    frmAssignPrescription.treatmentFinishProcessor.UpdateStoreCode(frmAssignPrescription.ucTreatmentFinish, storeCode);
                }

                Inventec.Common.Logging.LogSystem.Debug("Goi api sua don thuoc. Du lieu dau vao____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => prescriptionSDO), prescriptionSDO) + ". Du lieu dau ra____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Param), Param) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => result), result));

            }

            return result;
        }

        private void ProcessData(ref OutPatientPresSDO prescriptionSDO,long useTime = 0)
		{
			try
			{
                OutPatientPresMedicineSDOs = new List<PresMedicineSDO>();
                foreach (var item in OutPatientPresMedicineADOs)
                {
                    if (item.useTime == useTime)
                    {
                        PresMedicineSDO sdo = new PresMedicineSDO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<PresMedicineSDO>(sdo, item);
                        sdo.InstructionTimes = item.InstructionTimes;
                        sdo.MedicineBeanIds = item.MedicineBeanIds;
                        sdo.MedicineInfoSdos = item.MedicineInfoSdos;
                        OutPatientPresMedicineSDOs.Add(sdo);
                    }
                }
                OutPatientPresMaterialSDOs = new List<PresMaterialSDO>();
                foreach (var item in OutPatientPresMaterialADOs)
                {
                    if (item.useTime == useTime)
                    {
                        PresMaterialSDO sdo = new PresMaterialSDO();
                        Inventec.Common.Mapper.DataObjectMapper.Map<PresMaterialSDO>(sdo, item);
                        sdo.InstructionTimes = item.InstructionTimes;
                        sdo.MaterialBeanIds = item.MaterialBeanIds;
                        OutPatientPresMaterialSDOs.Add(sdo);
                    }
                }
                prescriptionSDO.Medicines = this.OutPatientPresMedicineSDOs;
                prescriptionSDO.Materials = this.OutPatientPresMaterialSDOs;
                prescriptionSDO.ServiceReqMaties = this.PresOutStockMatySDOs;
                prescriptionSDO.ServiceReqMeties = this.PresOutStockMetySDOs;
                prescriptionSDO.PrescriptionTypeId = PrescriptionType.NEW;
                prescriptionSDO.Id = OldServiceReq.ID;
                prescriptionSDO.TreatmentId = this.TreatmentId;
                prescriptionSDO.ClientSessionKey = GlobalStore.ClientSessionKey;
                prescriptionSDO.PrescriptionPhaseNum = (short?)this.PrescriptionPhaseNum;
                prescriptionSDO.HisGfrAlertLogs = AlertLogSDOs;
                if (this.ParentServiceReqId > 0)
                    prescriptionSDO.ParentServiceReqId = this.ParentServiceReqId;

                this.ProcessPrescriptionUpdateSDO(prescriptionSDO, useTime);
                this.ProcessPrescriptionUpdateSDOICD(prescriptionSDO);
                this.ProcessPrescriptionSDOForSereServInKip(prescriptionSDO);
                this.ProcessPrescriptionSDOForTreatmentFinish(prescriptionSDO);

            }
            catch (Exception ex)
			{
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
		}            

        private void ProcessPrescriptionUpdateSDO(OutPatientPresSDO prescriptionSDO,long useTime = 0)
        {
            try
            {
                prescriptionSDO.DrugStoreId = this.DrugStoreId;
                prescriptionSDO.IsCabinet = GlobalStore.IsCabinet;
                if (this.RemedyCount > 0)
                    prescriptionSDO.RemedyCount = this.RemedyCount;
                prescriptionSDO.Advise = this.Advise;
                prescriptionSDO.RequestRoomId = this.RequestRoomId;
                prescriptionSDO.RequestLoginName = this.RequestLoginname;
                prescriptionSDO.RequestUserName = this.RequestUserName;
                prescriptionSDO.ProvisionalDiagnosis = this.ProvisionalDiagnosis;
                //prescriptionSDO.ExpMestReasonId = this.ExpMestReasonId;
                if (this.ParentServiceReqId > 0)
                {
                    prescriptionSDO.ParentServiceReqId = this.ParentServiceReqId;
                }

                prescriptionSDO.InstructionTime = this.InstructionTimes.OrderByDescending(o => o).First();
                if (useTime > 0)
                {
                    prescriptionSDO.UseTime = useTime;
                }
                //else
                //{
                //    prescriptionSDO.UseTime = this.InstructionTimes.OrderByDescending(o => o).First();
                //}
                prescriptionSDO.InteractionReason = this.InteractionReason;

                //prescriptionSDO.UseTimes = this.UseTimes;

                if (frmAssignPrescription.lciPhieuDieuTri.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always
                    //&& frmAssignPrescription.Listtrackings != null && frmAssignPrescription.Listtrackings.Count > 0)
                    && frmAssignPrescription.cboPhieuDieuTri.EditValue != null)
                {
                    prescriptionSDO.TrackingId = Inventec.Common.TypeConvert.Parse.ToInt64(frmAssignPrescription.cboPhieuDieuTri.EditValue.ToString());
                   // prescriptionSDO.TrackingId = frmAssignPrescription.Listtrackings.FirstOrDefault().ID;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessPrescriptionUpdateSDOICD(OutPatientPresSDO prescriptionSDO)
        {
            try
            {
                prescriptionSDO.IcdName = this.IcdName;
                prescriptionSDO.IcdCode = this.IcdCode;
                prescriptionSDO.IcdCauseName = this.IcdCauseName;
                prescriptionSDO.IcdCauseCode = this.IcdCauseCode;
                prescriptionSDO.IcdText = this.IcdText;
                prescriptionSDO.IcdSubCode = this.IcdSubCode;
                prescriptionSDO.TraditionalIcdCode = this.IcdTranditionalCode;
                prescriptionSDO.TraditionalIcdName = this.IcdTranditionalName;
                prescriptionSDO.TraditionalIcdSubCode = this.IcdTranditionalSubCode;
                prescriptionSDO.TraditionalIcdText = this.IcdTranditionalText;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessPrescriptionSDOForSereServInKip(OutPatientPresSDO prescriptionSDO)
        {
            try
            {
                long? sereServParentId = null;
                if (prescriptionSDO.Medicines != null && prescriptionSDO.Medicines.Count > 0)
                    sereServParentId = prescriptionSDO.Medicines.Exists(o => o.SereServParentId != null) ? prescriptionSDO.Medicines.FirstOrDefault(o => o.SereServParentId != null).SereServParentId : null;
                if (prescriptionSDO.Materials != null && prescriptionSDO.Materials.Count > 0)
                    sereServParentId = prescriptionSDO.Materials.Exists(o => o.SereServParentId != null) ? prescriptionSDO.Materials.FirstOrDefault(o => o.SereServParentId != null).SereServParentId : null;
                if (prescriptionSDO.Materials.Count > 0
                    || prescriptionSDO.Medicines.Count > 0
                    )
                {
                    foreach (var item in prescriptionSDO.Materials)
                    {
                        item.SereServParentId = sereServParentId;
                    }

                    foreach (var item in prescriptionSDO.Medicines)
                    {
                        item.SereServParentId = sereServParentId;
                    }

                    foreach (var item in frmAssignPrescription.mediMatyTypeADOs)
                    {
                        item.SereServParentId = sereServParentId;
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessPrescriptionSDOForTreatmentFinish(OutPatientPresSDO prescriptionSDO)
        {
            try
            {
                if (this.IsAutoTreatmentEnd)
                {
                    prescriptionSDO.TreatmentFinishSDO = this.TreatmentFinishSDO;
                    if (prescriptionSDO.TreatmentFinishSDO == null)
                        prescriptionSDO.TreatmentFinishSDO = new HisTreatmentFinishSDO();

                    prescriptionSDO.TreatmentFinishSDO.DoctorLoginname = this.RequestLoginname;
                    prescriptionSDO.TreatmentFinishSDO.DoctorUsernname = this.RequestUserName;
                    prescriptionSDO.TreatmentFinishSDO.EndRoomId = this.RequestRoomId;
                    prescriptionSDO.TreatmentFinishSDO.IcdCode = this.IcdCode;
                    prescriptionSDO.TreatmentFinishSDO.IcdCauseName = this.IcdCauseName;
                    prescriptionSDO.TreatmentFinishSDO.IcdCauseCode = this.IcdCauseCode;
                    prescriptionSDO.TreatmentFinishSDO.IcdName = this.IcdName;
                    prescriptionSDO.TreatmentFinishSDO.IcdSubCode = this.IcdSubCode;
                    prescriptionSDO.TreatmentFinishSDO.IcdText = this.IcdText;
                    prescriptionSDO.TreatmentFinishSDO.TraditionalIcdCode = this.IcdTranditionalCode;
                    prescriptionSDO.TreatmentFinishSDO.TraditionalIcdName = this.IcdTranditionalName;
                    prescriptionSDO.TreatmentFinishSDO.TraditionalIcdSubCode = this.IcdTranditionalSubCode;
                    prescriptionSDO.TreatmentFinishSDO.TraditionalIcdText = this.IcdTranditionalText;
                    prescriptionSDO.TreatmentFinishSDO.TreatmentId = this.TreatmentId;
                    prescriptionSDO.TreatmentFinishSDO.TreatmentEndTypeExtId = this.TreatmentEndTypeExtId;
                    prescriptionSDO.TreatmentFinishSDO.TreatmentEndTypeId = this.TreatmentEndTypeId;
                    if (this.AppointmentTime > 0)
                        prescriptionSDO.TreatmentFinishSDO.AppointmentTime = this.AppointmentTime;
                    prescriptionSDO.TreatmentFinishSDO.AppointmentExamRoomIds = this.AppointmentNextRoomIds;
                    prescriptionSDO.TreatmentFinishSDO.TreatmentFinishTime = this.EndTime;
                    prescriptionSDO.TreatmentFinishSDO.ServiceReqId = this.OldServiceReq.ID;
                    prescriptionSDO.TreatmentFinishSDO.Advise = this.AdviseFinish;
                    prescriptionSDO.TreatmentFinishSDO.SurgeryAppointmentTime = this.SurgeryAppointmentTime;
                    prescriptionSDO.TreatmentFinishSDO.AppointmentSurgery = this.AppointmentSurgery;
                    if (this.CreateOutPatientMediRecord && this.ProgramId > 0)
                        prescriptionSDO.TreatmentFinishSDO.ProgramId = this.ProgramId;
                    prescriptionSDO.TreatmentFinishSDO.CreateOutPatientMediRecord = this.CreateOutPatientMediRecord;
                    prescriptionSDO.TreatmentFinishSDO.SickHeinCardNumber = this.SickHeinCardNumber;
                    prescriptionSDO.TreatmentFinishSDO.DocumentBookId = this.DocumentBookId;
                    prescriptionSDO.TreatmentFinishSDO.SickLoginname = this.SickLoginname;
                    prescriptionSDO.TreatmentFinishSDO.SickUsername = this.SickUsername;
                    prescriptionSDO.TreatmentFinishSDO.SickLeaveDay = this.SickLeaveDay;
                    prescriptionSDO.TreatmentFinishSDO.SickLeaveFrom = this.SickLeaveFrom;
                    prescriptionSDO.TreatmentFinishSDO.SickLeaveTo = this.SickLeaveTo;
                    prescriptionSDO.TreatmentFinishSDO.WorkPlaceId = this.SickWorkplaceId;
                    prescriptionSDO.TreatmentFinishSDO.NumOrderBlockId = this.NumOrderBlockId;
                    prescriptionSDO.TreatmentFinishSDO.ShowIcdCode = this.ShowIcdCode;
                    prescriptionSDO.TreatmentFinishSDO.ShowIcdName = this.ShowIcdName;
                    prescriptionSDO.TreatmentFinishSDO.ShowIcdSubCode = this.ShowIcdSubCode;
                    prescriptionSDO.TreatmentFinishSDO.ShowIcdText = this.ShowIcdText;
                    prescriptionSDO.TreatmentFinishSDO.IsExpXml4210Collinear = this.IsExpXml4210Collinear;
                    prescriptionSDO.TreatmentFinishSDO.CareerId = this.CareerId;
                    prescriptionSDO.TreatmentFinishSDO.EndDeptSubsHeadLoginname = this.EndDeptSubsHeadLoginname;
                    prescriptionSDO.TreatmentFinishSDO.EndDeptSubsHeadUsername = this.EndDeptSubsHeadUsername;
                    prescriptionSDO.TreatmentFinishSDO.HospSubsDirectorLoginname = this.HospSubsDirectorLoginname;
                    prescriptionSDO.TreatmentFinishSDO.HospSubsDirectorUsername = this.HospSubsDirectorUsername;
                    prescriptionSDO.TreatmentFinishSDO.TranPatiHospitalLoginname = this.TranPatiHospitalLoginname;
                    prescriptionSDO.TreatmentFinishSDO.TranPatiHospitalUsername = this.TranPatiHospitalUsername;
                    prescriptionSDO.TreatmentFinishSDO.TranPatiReasonId = this.TranPatiReasonId;
                    prescriptionSDO.TreatmentFinishSDO.TranPatiFormId = this.TranPatiFormId;
                    prescriptionSDO.TreatmentFinishSDO.TranPatiTechId = this.TranPatiTechId;
                    prescriptionSDO.TreatmentFinishSDO.TransferOutMediOrgCode = this.TransferOutMediOrgCode;
                    prescriptionSDO.TreatmentFinishSDO.TransferOutMediOrgName = this.TransferOutMediOrgName;
                    prescriptionSDO.TreatmentFinishSDO.ClinicalNote = this.ClinicalNote;
                    prescriptionSDO.TreatmentFinishSDO.SubclinicalResult = this.SubclinicalResult;
                    prescriptionSDO.TreatmentFinishSDO.PatientCondition = this.PatientCondition;
                    prescriptionSDO.TreatmentFinishSDO.TransportVehicle = this.TransportVehicle;
                    prescriptionSDO.TreatmentFinishSDO.Transporter = this.Transporter;
                    prescriptionSDO.TreatmentFinishSDO.TreatmentMethod = this.TreatmentMethod;
                    prescriptionSDO.TreatmentFinishSDO.TransporterLoginnames = this.TransporterLoginnames;
                    prescriptionSDO.TreatmentFinishSDO.TreatmentDirection = this.TreatmentDirection;
                    prescriptionSDO.TreatmentFinishSDO.MainCause = this.MainCause;
                    prescriptionSDO.TreatmentFinishSDO.Surgery = this.Surgery;
                    prescriptionSDO.TreatmentFinishSDO.DeathTime = this.DeathTime;
                    prescriptionSDO.TreatmentFinishSDO.IsHasAupopsy = this.IsHasAupopsy;
                    prescriptionSDO.TreatmentFinishSDO.DeathCauseId = this.DeathCauseId;
                    prescriptionSDO.TreatmentFinishSDO.DeathWithinId = this.DeathWithinId;
                    prescriptionSDO.TreatmentFinishSDO.EndTypeExtNote = this.EndTypeExtNote;
                    prescriptionSDO.TreatmentFinishSDO.MotherName = this.MotherName;
                    prescriptionSDO.TreatmentFinishSDO.FatherName = this.FatherName;
                }
                else
                {
                    prescriptionSDO.TreatmentFinishSDO = null;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }
    }
}

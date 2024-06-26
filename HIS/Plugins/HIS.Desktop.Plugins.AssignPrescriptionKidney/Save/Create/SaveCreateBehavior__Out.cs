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
using HIS.Desktop.ApiConsumer;
using HIS.Desktop.Plugins.AssignPrescriptionKidney.AssignPrescription;
using Inventec.Common.Logging;
using MOS.SDO;
using System;
using System.Linq;

namespace HIS.Desktop.Plugins.AssignPrescriptionKidney.Save.Create
{
    partial class SaveCreateBehavior : SaveAbstract, ISave
    {
        object Run__Out()
        {
            OutPatientPresResultSDO result = null;
            //LogSystem.Debug("CheckValid => 1");
            if (this.CheckValid())
            {
                frmAssignPrescription.VerifyWarningOverCeiling();
                //LogSystem.Debug("InitBase => 2");
                this.InitBase();

                OutPatientPresSDO prescriptionSDO = new OutPatientPresSDO();
                prescriptionSDO.Medicines = this.OutPatientPresMedicineSDOs;
                prescriptionSDO.Materials = this.OutPatientPresMaterialSDOs;
                prescriptionSDO.ServiceReqMaties = this.ServiceReqMaties;
                prescriptionSDO.ServiceReqMeties = this.ServiceReqMeties;
                prescriptionSDO.SerialNumbers = this.PatientPresMaterialBySerialNumberSDOs;
                prescriptionSDO.TreatmentId = this.TreatmentId;
                prescriptionSDO.PrescriptionTypeId = PrescriptionType.NEW;
                prescriptionSDO.ClientSessionKey = GlobalStore.ClientSessionKey;
                if (this.ParentServiceReqId > 0)
                    prescriptionSDO.ParentServiceReqId = this.ParentServiceReqId;

                this.ProcessPrescriptionUpdateSDO(prescriptionSDO);
                this.ProcessPrescriptionUpdateSDOICD(prescriptionSDO);
                this.ProcessPrescriptionSDOForSereServInKip(prescriptionSDO);
                this.ProcessPrescriptionSDOForTreatmentFinish(prescriptionSDO);
                
                Inventec.Common.Logging.LogSystem.Debug("Goi api ke don thuoc. Du lieu dau vao____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => prescriptionSDO), prescriptionSDO) + ". Du lieu dau ra____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Param), Param));
                LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => prescriptionSDO), prescriptionSDO));
                result = new Inventec.Common.Adapter.BackendAdapter(Param).Post<OutPatientPresResultSDO>(RequestUriStore.HIS_SERVICE_REQ__OUTPATIENT_PRES_CREATE, ApiConsumers.MosConsumer, prescriptionSDO, Param);
                if (result == null
                    || result.ServiceReqs == null || result.ServiceReqs.Count == 0
                    || ((result.ServiceReqMaties == null || result.ServiceReqMaties.Count == 0)
                        && (result.ServiceReqMeties == null || result.ServiceReqMeties.Count == 0)
                        && (result.Materials == null || result.Materials.Count == 0)
                        && (result.Medicines == null || result.Medicines.Count == 0))
                    )
                {
                    Inventec.Common.Logging.LogSystem.Debug("Goi api ke don thuoc that bai. Du lieu dau vao____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => prescriptionSDO), prescriptionSDO) + ". Du lieu dau ra____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => result), result) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Param), Param));
                    result = null;
                }

                Inventec.Common.Logging.LogSystem.Debug("Du lieu dau vao____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => prescriptionSDO), prescriptionSDO) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Param), Param));
                //LogSystem.Debug("Call api => 4");
            }

            return result;
        }

        private void ProcessPrescriptionUpdateSDO(OutPatientPresSDO prescriptionSDO)
        {
            try
            {
                if (this.RemedyCount > 0)
                    prescriptionSDO.RemedyCount = this.RemedyCount;
                prescriptionSDO.Advise = this.Advise;
                prescriptionSDO.RequestRoomId = this.RequestRoomId;
                prescriptionSDO.RequestLoginName = this.RequestLoginname;
                prescriptionSDO.RequestUserName = this.RequestUserName;
                if (this.ParentServiceReqId > 0)
                    prescriptionSDO.ParentServiceReqId = this.ParentServiceReqId;
                prescriptionSDO.InstructionTime = this.InstructionTimes.OrderByDescending(o => o).First();
                prescriptionSDO.UseTime = this.InstructionTimes.OrderByDescending(o => o).First();

                //Set numofday của đươn ngoài kho
                //Kiem tra khong có đơn trong kho thì set numofday cho đơn ngoài kho
                if ((prescriptionSDO.Medicines == null || prescriptionSDO.Medicines.Count == 0)
                    && prescriptionSDO.ServiceReqMeties != null && prescriptionSDO.ServiceReqMeties.Count > 0
                    && this.MediMatyTypeADOs != null && this.MediMatyTypeADOs.Count > 0)
                {
                    long numOfDay = 0;
                    foreach (var item in this.MediMatyTypeADOs)
                    {
                        long? numOfDayTemp = GetNumOfDays(item);
                        if (numOfDayTemp.HasValue && numOfDay < numOfDayTemp)
                            numOfDay = numOfDayTemp.Value;
                    }
                    if (numOfDay > 0)
                    {
                        prescriptionSDO.NumOfDays = numOfDay;
                    }
                }

                if (frmAssignPrescription.lciPhieuDieuTri.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always
                    && frmAssignPrescription.cboPhieuDieuTri.EditValue != null)
                {
                    prescriptionSDO.TrackingId = Inventec.Common.TypeConvert.Parse.ToInt64(frmAssignPrescription.cboPhieuDieuTri.EditValue.ToString());
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
                if (prescriptionSDO.Materials.Count > 0
                    || prescriptionSDO.Medicines.Count > 0
                    )
                {
                    if (frmAssignPrescription.currentSereServ != null)
                    {
                        foreach (var item in prescriptionSDO.Materials)
                        {
                            item.SereServParentId = frmAssignPrescription.currentSereServ.ID;
                        }

                        foreach (var item in prescriptionSDO.Medicines)
                        {
                            item.SereServParentId = frmAssignPrescription.currentSereServ.ID;
                        }
                    }

                    if (frmAssignPrescription.currentSereServInEkip != null)
                    {
                        foreach (var item in prescriptionSDO.Materials)
                        {
                            item.SereServParentId = frmAssignPrescription.currentSereServInEkip.ID;
                        }

                        foreach (var item in prescriptionSDO.Medicines)
                        {
                            item.SereServParentId = frmAssignPrescription.currentSereServInEkip.ID;
                        }
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
                    //if (this.IcdId > 0)
                    //    prescriptionSDO.TreatmentFinishSDO.IcdId = this.IcdId;
                    prescriptionSDO.TreatmentFinishSDO.IcdName = this.IcdName;
                    //prescriptionSDO.TreatmentFinishSDO.IcdCauseName = this.IcdCauseName;
                    //prescriptionSDO.TreatmentFinishSDO.IcdCauseCode = this.IcdCauseCode;
                    prescriptionSDO.TreatmentFinishSDO.IcdSubCode = this.IcdSubCode;
                    prescriptionSDO.TreatmentFinishSDO.IcdText = this.IcdText;
                    prescriptionSDO.TreatmentFinishSDO.TreatmentId = this.TreatmentId;
                    prescriptionSDO.TreatmentFinishSDO.TreatmentEndTypeId = this.TreatmentEndTypeId;
                    prescriptionSDO.TreatmentFinishSDO.AppointmentTime = this.AppointmentTime;
                    prescriptionSDO.TreatmentFinishSDO.TreatmentFinishTime = this.EndTime;
                    prescriptionSDO.TreatmentFinishSDO.Advise = this.AdviseFinish;
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

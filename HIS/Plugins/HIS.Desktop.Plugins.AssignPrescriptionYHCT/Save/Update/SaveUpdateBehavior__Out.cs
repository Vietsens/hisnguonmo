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
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.ADO;
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.AssignPrescription;
using HIS.Desktop.Plugins.AssignPrescriptionYHCT.Resources;
using Inventec.Common.Logging;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIS.Desktop.Plugins.AssignPrescriptionYHCT.Save.Update
{
    partial class SaveUpdateBehavior : SaveAbstract, ISave
    {
        object Run__Out()
        {
            OutPatientPresResultSDO result = null;
            LogSystem.Debug("CheckValid => 1");
            if (this.CheckValid())
            {
                frmAssignPrescription.VerifyWarningOverCeiling();
                LogSystem.Debug("InitBase => 2");
                this.InitBase();

                OutPatientPresSDO prescriptionSDO = new OutPatientPresSDO();
                prescriptionSDO.Medicines = this.OutPatientPresMedicineSDOs;
                prescriptionSDO.Materials = this.OutPatientPresMaterialSDOs;
                prescriptionSDO.ServiceReqMaties = this.ServiceReqMaties;
                prescriptionSDO.ServiceReqMeties = this.ServiceReqMeties;
                prescriptionSDO.PrescriptionTypeId = PrescriptionType.TRADITIONAL;
                prescriptionSDO.Id = OldServiceReq.ID;
                prescriptionSDO.TreatmentId = this.TreatmentId;
                prescriptionSDO.ClientSessionKey = GlobalStore.ClientSessionKey;
                if (this.ParentServiceReqId > 0)
                    prescriptionSDO.ParentServiceReqId = this.ParentServiceReqId;

                this.ProcessPrescriptionUpdateSDO(prescriptionSDO);
                this.ProcessPrescriptionUpdateSDOICD(prescriptionSDO);
                this.ProcessPrescriptionSDOForSereServInKip(prescriptionSDO);
                this.ProcessPrescriptionSDOForTreatmentFinish(prescriptionSDO);

                // minhnq
                if (prescriptionSDO.TreatmentFinishSDO != null && prescriptionSDO.TreatmentFinishSDO.TreatmentEndTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__CHET && (string.IsNullOrEmpty(prescriptionSDO.TreatmentFinishSDO.MainCause) || prescriptionSDO.TreatmentFinishSDO.DeathCauseId == null || prescriptionSDO.TreatmentFinishSDO.DeathTime == null || prescriptionSDO.TreatmentFinishSDO.DeathWithinId == null))
                {
                    frmAssignPrescription.msgTuVong = "Bạn chưa nhập đủ thông tin tử vong, vui lòng kiểm tra lại";
                    return result;
                }

                LogSystem.Debug("Process data => 3");
                Inventec.Common.Logging.LogSystem.Debug("Goi api sua don thuoc. Du lieu dau vao____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => prescriptionSDO), prescriptionSDO) + ". Du lieu dau ra____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Param), Param));
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

                Inventec.Common.Logging.LogSystem.Debug("Du lieu dau vao____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => result), result) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Param), Param));
                LogSystem.Debug("Call api => 4");
            }

            return result;
        }

        private void ProcessPrescriptionUpdateSDO(OutPatientPresSDO prescriptionSDO)
        {
            try
            {
                prescriptionSDO.DrugStoreId = this.DrugStoreId;
                if (this.RemedyCount > 0)
                    prescriptionSDO.RemedyCount = this.RemedyCount;
                prescriptionSDO.Advise = this.Advise;
                prescriptionSDO.RequestRoomId = this.RequestRoomId;
                prescriptionSDO.RequestLoginName = this.RequestLoginname;
                prescriptionSDO.RequestUserName = this.RequestUserName;
                //prescriptionSDO.ExpMestReasonId = this.ExpMestReasonId;
                prescriptionSDO.InstructionTime = this.InstructionTimes.OrderByDescending(o => o).First();
                //prescriptionSDO.UseTime = this.InstructionTimes.OrderByDescending(o => o).First();
                prescriptionSDO.UseTime = this.USE_TIMES.FirstOrDefault();
                
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
                long? sereServParentId = null;
                if(prescriptionSDO.Medicines != null && prescriptionSDO.Medicines.Count > 0)
                    sereServParentId = prescriptionSDO.Medicines.Exists(o => o.SereServParentId != null) ? prescriptionSDO.Medicines.FirstOrDefault(o => o.SereServParentId != null).SereServParentId : null;
                if (prescriptionSDO.Materials != null && prescriptionSDO.Materials.Count > 0)
                    sereServParentId = prescriptionSDO.Materials.Exists(o => o.SereServParentId != null) ? prescriptionSDO.Materials.FirstOrDefault(o => o.SereServParentId != null).SereServParentId : null;
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

                    foreach (var item in prescriptionSDO.Materials)
                    {
                        item.SereServParentId = sereServParentId;
                    }

                    foreach (var item in prescriptionSDO.Medicines)
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
                    //if (this.IcdId > 0)
                    //    prescriptionSDO.TreatmentFinishSDO.IcdId = this.IcdId;
                    prescriptionSDO.TreatmentFinishSDO.IcdName = this.IcdName;
                    prescriptionSDO.TreatmentFinishSDO.IcdSubCode = this.IcdSubCode;
                    prescriptionSDO.TreatmentFinishSDO.IcdText = this.IcdText;
                    prescriptionSDO.TreatmentFinishSDO.TreatmentId = this.TreatmentId;
                    prescriptionSDO.TreatmentFinishSDO.TreatmentEndTypeId = this.TreatmentEndTypeId;
                    prescriptionSDO.TreatmentFinishSDO.AppointmentTime = this.AppointmentTime;
                    prescriptionSDO.TreatmentFinishSDO.TreatmentFinishTime = this.EndTime;
                    prescriptionSDO.TreatmentFinishSDO.ServiceReqId = this.OldServiceReq.ID;
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

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

namespace HIS.Desktop.Plugins.AssignPrescriptionYHCT.Save.Create
{
    partial class SaveCreateBehavior : SaveAbstract, ISave
    {
        object Run__In()
        {
            InPatientPresResultSDO result = null;
            LogSystem.Debug("CheckValid => 1");
            if (this.CheckValid())
            {
                frmAssignPrescription.VerifyWarningOverCeiling();
                LogSystem.Debug("InitBase => 2");
                this.InitBase();

                InPatientPresSDO prescriptionSDO = new InPatientPresSDO();
                prescriptionSDO.Medicines = this.InPatientPresMedicineSDOs;
                prescriptionSDO.Materials = this.InPatientPresMaterialSDOs;
                prescriptionSDO.ServiceReqMaties = this.ServiceReqMaties;
                prescriptionSDO.ServiceReqMeties = this.ServiceReqMeties;
                prescriptionSDO.PrescriptionTypeId = PrescriptionType.TRADITIONAL;
                prescriptionSDO.TreatmentId = this.TreatmentId;

                
                
                prescriptionSDO.UseTimes = this.USE_TIMES;
                this.ProcessPrescriptionUpdateSDO(prescriptionSDO);
                this.ProcessPrescriptionUpdateSDOICD(prescriptionSDO);
                this.ProcessPrescriptionSDOForSereServInKip(prescriptionSDO);
                LogSystem.Debug("Process data => 3");

                Inventec.Common.Logging.LogSystem.Info(Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => prescriptionSDO), prescriptionSDO));
                result = new Inventec.Common.Adapter.BackendAdapter(Param).Post<InPatientPresResultSDO>(RequestUriStore.HIS_SERVICE_REQ__IN_PATIENT_PRES_CREATE, ApiConsumers.MosConsumer, prescriptionSDO, Param);
                if (result == null
                    || result.ServiceReqs == null || result.ServiceReqs.Count == 0
                    || ((result.ServiceReqMaties == null || result.ServiceReqMaties.Count == 0)
                        && (result.ServiceReqMeties == null || result.ServiceReqMeties.Count == 0)
                        && (result.Materials == null || result.Materials.Count == 0)
                        && (result.Medicines == null || result.Medicines.Count == 0))
                    )
                {
                    Inventec.Common.Logging.LogSystem.Error("Goi api ke don thuoc that bai. Du lieu dau vao____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => prescriptionSDO), prescriptionSDO) + ". Du lieu dau ra____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => result), result) + "____" + Inventec.Common.Logging.LogUtil.TraceData(Inventec.Common.Logging.LogUtil.GetMemberName(() => Param), Param));
                    result = null;
                }
                LogSystem.Debug("Call api => 4");
            }
            return result;
        }

        private void ProcessPrescriptionUpdateSDO(InPatientPresSDO prescriptionSDO)
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
                if (this.ParentServiceReqId > 0)
                    prescriptionSDO.ParentServiceReqId = this.ParentServiceReqId;
                prescriptionSDO.InstructionTimes = this.InstructionTimes;
                prescriptionSDO.AssignTimeTo = this.TIME_TO;
                if (frmAssignPrescription.lciPhieuDieuTri.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always
                    && frmAssignPrescription.cboPhieuDieuTri.EditValue!=null)
                {
                    prescriptionSDO.TrackingId = Inventec.Common.TypeConvert.Parse.ToInt64(frmAssignPrescription.cboPhieuDieuTri.EditValue.ToString());
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessPrescriptionUpdateSDOICD(InPatientPresSDO prescriptionSDO)
        {
            try
            {
                prescriptionSDO.IcdName = this.IcdName;
                prescriptionSDO.IcdCode = this.IcdCode;
                prescriptionSDO.IcdCauseName = this.IcdCauseName;
                prescriptionSDO.IcdCauseCode = this.IcdCauseCode;
                prescriptionSDO.IcdText = this.IcdText;
                prescriptionSDO.IcdSubCode = this.IcdSubCode;
                prescriptionSDO.TraditionalIcdCode = this.IcdCodeYHCT;
                prescriptionSDO.TraditionalIcdName = this.IcdNameYHCT;
                prescriptionSDO.TraditionalIcdSubCode = this.IcdSubCodeYHCT;
                prescriptionSDO.TraditionalIcdText = this.IcdTextYHCT;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
        }

        private void ProcessPrescriptionSDOForSereServInKip(InPatientPresSDO prescriptionSDO)
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

    }
}

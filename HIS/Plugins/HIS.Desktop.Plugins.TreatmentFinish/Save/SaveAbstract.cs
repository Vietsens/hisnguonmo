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
using HIS.UC.Icd.ADO;
using HIS.UC.SecondaryIcd.ADO;
using Inventec.Core;
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.TreatmentFinish.Save
{
    abstract class SaveAbstract : EntityBase
    {
        protected long RoomId { get; set; }
        protected HisTreatmentFinishSDO hisTreatmentFinishSDO_process { get; set; }
        protected HIS_TREATMENT currentHisTreatment { get; set; }

        protected string DoctorLoginname { get; set; }
        protected string DoctorUsernname { get; set; }
        protected long EndRoomId { get; set; }
        protected string IcdCode { get; set; }
        protected long? IcdId { get; set; }
        protected string IcdName { get; set; }
        protected string IcdSubCode { get; set; }
        protected string IcdText { get; set; }
        protected bool? IsChronic { get; set; }
        protected bool IsTemporary { get; set; }
        protected long TreatmentEndTypeId { get; set; }
        protected long TreatmentFinishTime { get; set; }
        protected long TreatmentId { get; set; }
        protected long? TreatmentResultId { get; set; }
        protected string Treatment_Method { get; set; }
        protected string Advised { get; set; }
        protected long? ServiceReqId { get; set; }
        protected string ClinicalNote { get; set; }
        protected string Subclinical { get; set; }

        protected FormTreatmentFinish Form { get; set; }

        protected SaveAbstract(long RoomId,
            long? ServiceReqId,
            bool isSave,
            HIS_TREATMENT currentHisTreatment,
            HisTreatmentFinishSDO hisTreatmentFinishSDO_process,
            FormTreatmentFinish Form)
            : base()
        {
            try
            {
                this.RoomId = RoomId;
                this.currentHisTreatment = currentHisTreatment;
                this.hisTreatmentFinishSDO_process = hisTreatmentFinishSDO_process;
                this.TreatmentId = currentHisTreatment.ID;
                this.EndRoomId = RoomId;
                this.IsTemporary = !isSave;
                this.ServiceReqId = ServiceReqId;
                if (Form != null)
                {
                    this.Form = Form;
                    ProcessFinishData(Form);
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private void ProcessFinishData(FormTreatmentFinish Form)
        {
            try
            {
                String dt = Form.dtEndTime.DateTime.ToString("yyyyMMddHHmmss");
                this.TreatmentFinishTime = Inventec.Common.TypeConvert.Parse.ToInt64(dt);

                var treatmentEndType = Base.GlobalStore.HisTreatmentEndTypes.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((Form.cboTreatmentEndType.EditValue ?? 0).ToString()));
                if (treatmentEndType != null)
                {
                    this.TreatmentEndTypeId = treatmentEndType.ID;
                }
                else
                {
                    this.TreatmentEndTypeId = 0;
                }

                var treatmentResultId = Base.GlobalStore.HisTreatmentResults.FirstOrDefault(o => o.ID == Inventec.Common.TypeConvert.Parse.ToInt64((Form.cboResult.EditValue ?? 0).ToString()));
                if (treatmentResultId != null)
                {
                    this.TreatmentResultId = treatmentResultId.ID;
                }
                else
                {
                    this.TreatmentResultId = null;
                }

                if (Form.ucIcd != null)
                {
                    var icdValue = Form.icdProcessor.GetValue(Form.ucIcd);
                    if (icdValue != null && icdValue is IcdInputADO)
                    {
                        this.IcdCode = ((IcdInputADO)icdValue).ICD_CODE;
                        this.IcdName = ((IcdInputADO)icdValue).ICD_NAME;
                    }
                }
                if (Form.ucSecondaryIcd != null)
                {
                    var subIcd = Form.subIcdProcessor.GetValue(Form.ucSecondaryIcd);
                    if (subIcd != null && subIcd is SecondaryIcdDataADO)
                    {
                        this.IcdSubCode = ((SecondaryIcdDataADO)subIcd).ICD_SUB_CODE;
                        this.IcdText = ((SecondaryIcdDataADO)subIcd).ICD_TEXT;
                    }
                }

                if (Form.chkChronic.Checked)
                {
                    this.IsChronic = true;
                }

                if (Form.lciDoctorName.Visibility == DevExpress.XtraLayout.Utils.LayoutVisibility.Always && !String.IsNullOrEmpty(Form.txtDoctorLogginName.Text.Trim()))
                {
                    this.DoctorLoginname = Form.txtDoctorLogginName.Text.Trim();
                    //this.DoctorUsernname = Form.cboDoctorUserName.EditValue.ToString();
                    var VhisEmployee = HIS.Desktop.LocalStorage.BackendData.BackendDataWorker.Get<MOS.EFMODEL.DataModels.V_HIS_EMPLOYEE>().FirstOrDefault(o => o.LOGINNAME == Form.txtDoctorLogginName.Text && o.IS_ACTIVE == 1);
                    if (VhisEmployee != null)
                    {
                        this.DoctorUsernname = VhisEmployee.TDL_USERNAME;
                    }
                }

                this.Treatment_Method = Form.txtMethod.Text;
                this.Advised = Form.txtAdvised.Text;
                this.ClinicalNote = Form.txtDauHieuLamSang.Text;
                this.Subclinical = Form.txtKetQuaXetNghiem.Text;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        protected bool CheckDeath()
        {
            bool result = true;
            try
            {
                if (hisTreatmentFinishSDO_process != null)
                {
                    if (hisTreatmentFinishSDO_process.DeathCauseId == null ||
                        hisTreatmentFinishSDO_process.DeathCauseId <= 0) result = false;
                    else if (hisTreatmentFinishSDO_process.DeathTime == null ||
                        hisTreatmentFinishSDO_process.DeathTime <= 0) result = false;
                    else if (hisTreatmentFinishSDO_process.DeathWithinId == null ||
                        hisTreatmentFinishSDO_process.DeathWithinId <= 0) result = false;
                    else if (string.IsNullOrEmpty(hisTreatmentFinishSDO_process.MainCause)) result = false;
                }
                else
                    result = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        protected bool CheckSick()
        {
            bool result = true;
            try
            {
                if (hisTreatmentFinishSDO_process != null)
                {
                }
                else
                    result = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        protected bool CheckTranPati()
        {
            bool result = true;
            try
            {
                if (hisTreatmentFinishSDO_process != null)
                {
                    if (hisTreatmentFinishSDO_process.TranPatiFormId == null ||
                        hisTreatmentFinishSDO_process.TranPatiFormId <= 0) result = false;
                    else if (hisTreatmentFinishSDO_process.TranPatiReasonId == null ||
                        hisTreatmentFinishSDO_process.TranPatiReasonId <= 0) result = false;
                    else if (String.IsNullOrEmpty(hisTreatmentFinishSDO_process.TransferOutMediOrgCode)) result = false;
                }
                else
                    result = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }

        protected bool CheckAppointment()
        {
            bool result = true;
            try
            {
                if (hisTreatmentFinishSDO_process != null)
                {
                    if (hisTreatmentFinishSDO_process.AppointmentTime == null || hisTreatmentFinishSDO_process.AppointmentTime <= 0) result = false;
                }
                else
                    result = false;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = false;
            }
            return result;
        }
    }
}

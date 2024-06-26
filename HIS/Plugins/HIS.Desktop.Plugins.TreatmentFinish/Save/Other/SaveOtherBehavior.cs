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
using MOS.EFMODEL.DataModels;
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.TreatmentFinish.Save.Other
{
    class SaveOtherBehavior : SaveAbstract, ISave
    {
        internal SaveOtherBehavior(long RoomId,
            long? ServiceReqId,
            bool isSave,
            HIS_TREATMENT currentVHisTreatment,
            HisTreatmentFinishSDO hisTreatmentFinishSDO_process,
            FormTreatmentFinish Form)
            : base(RoomId, ServiceReqId, isSave, currentVHisTreatment, hisTreatmentFinishSDO_process, Form)
        { }

        object ISave.Run()
        {
            HisTreatmentFinishSDO result = null;
            try
            {
                result = new HisTreatmentFinishSDO();

                result.TreatmentFinishTime = this.TreatmentFinishTime;
                result.TreatmentId = this.TreatmentId;
                result.EndRoomId = this.RoomId;
                result.ServiceReqId = this.ServiceReqId;
                result.TreatmentEndTypeId = this.TreatmentEndTypeId;
                result.TreatmentResultId = this.TreatmentResultId;
                result.IcdCode = this.IcdCode;
                result.IcdName = this.IcdName;
                result.IcdSubCode = this.IcdSubCode;
                result.IcdText = this.IcdText;
                result.IsChronic = this.IsChronic;
                result.IsTemporary = this.IsTemporary;
                result.DoctorLoginname = this.DoctorLoginname;
                result.DoctorUsernname = this.DoctorUsernname;

                result.Advise = this.Advised;
                result.TreatmentMethod = this.Treatment_Method;

                result.ClinicalNote = this.ClinicalNote;
                result.SubclinicalResult = this.Subclinical;

                //treatment sick
                result.SickLeaveDay = hisTreatmentFinishSDO_process.SickLeaveDay;
                result.SickLeaveFrom = hisTreatmentFinishSDO_process.SickLeaveFrom;
                result.SickLeaveTo = hisTreatmentFinishSDO_process.SickLeaveTo;
                result.MotherName = hisTreatmentFinishSDO_process.MotherName;
                result.FatherName = hisTreatmentFinishSDO_process.FatherName;
                result.PatientRelativeName = hisTreatmentFinishSDO_process.PatientRelativeName;
                result.PatientRelativeType = hisTreatmentFinishSDO_process.PatientRelativeType;
                result.SickLoginname = hisTreatmentFinishSDO_process.SickLoginname;
                result.SickUsername = hisTreatmentFinishSDO_process.SickUsername;
                result.DocumentBookId = hisTreatmentFinishSDO_process.DocumentBookId;

                result.TreatmentEndTypeExtId = hisTreatmentFinishSDO_process.TreatmentEndTypeExtId;
                result.Babies = hisTreatmentFinishSDO_process.Babies;
                result.PatientWorkPlace = hisTreatmentFinishSDO_process.PatientWorkPlace;
                result.WorkPlaceId = hisTreatmentFinishSDO_process.WorkPlaceId;
                result.EndTypeExtNote = hisTreatmentFinishSDO_process.EndTypeExtNote;
                result.IsPregnancyTermination = hisTreatmentFinishSDO_process.IsPregnancyTermination;
                result.GestationalAge = hisTreatmentFinishSDO_process.GestationalAge;
                result.PregnancyTerminationReason = hisTreatmentFinishSDO_process.PregnancyTerminationReason;
                result.PregnancyTerminationTime = hisTreatmentFinishSDO_process.PregnancyTerminationTime;


                if ((hisTreatmentFinishSDO_process.TreatmentEndTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__RAVIEN
                    || hisTreatmentFinishSDO_process.TreatmentEndTypeId == IMSys.DbConfig.HIS_RS.HIS_TREATMENT_END_TYPE.ID__XINRAVIEN)
                    && hisTreatmentFinishSDO_process.AppointmentTime.HasValue)
                {
                    result.AppointmentExamRoomIds = hisTreatmentFinishSDO_process.AppointmentExamRoomIds;
                    result.AppointmentPeriodId = hisTreatmentFinishSDO_process.AppointmentPeriodId;
                    result.AppointmentTime = hisTreatmentFinishSDO_process.AppointmentTime;
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = new HisTreatmentFinishSDO();
            }
            return result;
        }
    }
}

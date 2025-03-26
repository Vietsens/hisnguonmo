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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.TreatmentFinish.ADO
{
    public class DataOutputADO
    {
        public DataOutputADO() { }
        public bool IsSignExam { get; set; }
        public bool IsPrintExam { get; set; }
        public bool IsAutoTreatmentFinish { get; set; }
        public bool IsAutoPrintGHK { get; set; }
        public bool IsSignGHK { get; set; }
        public bool IsAutoBK { get; set; }
        public bool IsSignBK { get; set; }
        public bool IsAutoPrintTL { get; set; }
        public bool IsSignTL { get; set; }
        public bool IsAutoBANT { get; set; }
        public bool IsCreateBANT { get; set; }
        public DateTime dtEndTime { get; set; }
        public long TreatmentEndTypeId { get; set; }
        public long? TreatmentEndTypeExtId { get; set; }
        public DateTime dtAppointmentTime { get; set; }
        public string Advise { get; set; }
        public long? SurgeryAppointmentTime { get; set; }
        public string AppointmentSurgery { get; set; }
        public List<long> AppointmentNextRoomIds { get; set; }
        public bool IsIssueOutPatientStoreCode { get; set; }
        public string StoreCode { get; set; }
        public long? ProgramId { get; set; }
        public string SickHeinCardNumber { get; set; }
        public string SickLoginname { get; set; }
        public string SickUsername { get; set; }
        public long? SickLeaveTo { get; set; }
        public long? SickLeaveFrom { get; set; }
        public decimal? SickLeaveDay { get; set; }
        public long? SickWorkplaceId { get; set; } 
        public long? DocumentBookId { get; set; }
        public long CareeId { get; set; }

        public long? NumOrderBlockId { get; set; }
        public long? NumOrderBlockNumOrder { get; set; }

        public bool IsPrintBHXH { get; set; }
        public bool IsSignBHXH { get; set; }
        public bool IsExpXml4210Collinear { get; set; }

        public string icdName { get; set; }
        public string icdCode { get; set; }
        public string icdSubCode { get; set; }
        public string icdText { get; set; }
        public string EndDeptSubsHeadLoginname { get; set; }
        public string EndDeptSubsHeadUsername { get; set; }
        public string HospSubsDirectorLoginname { get; set; }
        public string HospSubsDirectorUsername { get; set; }
        public string TranPatiHospitalLoginname { get; set; }
        public string TranPatiHospitalUsername { get; set; }
        public long? TranPatiReasonId { get; set; }
        public long? TranPatiFormId { get; set; }
        public long? TranPatiTechId { get; set; }
        public string TransferOutMediOrgCode { get; set; }
        public string TransferOutMediOrgName { get; set; }
        public string ClinicalNote { get; set; }
        public string SubclinicalResult { get; set; }
        public string PatientCondition { get; set; }
        public string TransportVehicle { get; set; }
        public string Transporter { get; set; }
        public string TreatmentMethod { get; set; }
        public string TreatmentDirection { get; set; }
        public string MainCause { get; set; }
        public string Surgery { get; set; }
        public long? DeathTime { get; set; }
        public short? IsHasAupopsy { get; set; }
        public long? DeathCauseId { get; set; }
        public long? DeathWithinId { get; set; }
        public string EndTypeExtNote { get; set; }
        public string MotherName { get; set; }
        public string FatherName { get; set; }
        public string TransporterLoginnames { get; set; }
        public string HospitalizeReasonName { get; set; }
        public string HospitalizeReasonCode { get; set; }
        public string SurgeryName { get; set; }
        public long? SurgeryBeginTime { get; set; }
        public long? SurgeryEndTime { get; set; }
        public string UsedMedicine { get; set; }

    }
}

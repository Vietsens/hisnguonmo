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
using System;
using System.Collections.Generic;

namespace MPS.Processor.Mps000504.PDO
{
    /// <summary>
    /// PTTK 2883 - muc 2: input cho pipeline gom nhom chi phi theo khoa/phong xu ly (ExeRoom),
    /// port tu Mps000304 (temp 6556) de temp bang ke theo KHOA dung duoc cac key:
    /// ReqExeDepaRoom, ReqExeRoom, HeinServiceTypeExeRoom, MedicineLineExeRoom,
    /// HeinServiceTypeBedExeRoom, ServiceExeRoom, PatyAlterBHYTExeRoom.
    /// Cac property nay la TUY CHON — khi null thi Mps000504 in nhu cu (chi danh sach phang SereServs).
    /// </summary>
    public partial class Mps000504PDO
    {
        /// <summary>V_HIS_TREATMENT day du (khac property Treatment hien co la V_HIS_TREATMENT_FEE)</summary>
        public V_HIS_TREATMENT TreatmentView { get; set; }
        /// <summary>HIS_SERE_SERV DA LOC theo khoang thoi gian [fromDateReq, toDateReq] (TDL_INTRUCTION_TIME)</summary>
        public List<HIS_SERE_SERV> SereServs { get; set; }
        public List<HIS_SERE_SERV_EXT> SereServExts { get; set; }
        public List<HIS_HEIN_SERVICE_TYPE> HeinServiceTypes { get; set; }
        public List<V_HIS_SERVICE> Services { get; set; }
        public List<V_HIS_ROOM> Rooms { get; set; }
        public List<HIS_DEPARTMENT> Departments { get; set; }
        public List<HIS_MEDICINE_TYPE> medicineTypes { get; set; }
        public List<HIS_MEDICINE_LINE> MedicineLines { get; set; }
        public List<HIS_MATERIAL_TYPE> materialTypes { get; set; }
        public List<HIS_SERVICE_REQ> ServiceReqs { get; set; }
        public List<HIS_PATIENT_TYPE_ALTER> PatientTypeAlterAlls { get; set; }
        public V_HIS_PATIENT_TYPE_ALTER CurrentPatyAlter { get; set; }
        public HIS_BRANCH Branch { get; set; }
        public List<HIS_TREATMENT_TYPE> TreatmentTypes { get; set; }
        public PatientTypeCFG PatientTypeCFG { get; set; }
        public HisConfigValue HisConfigValue { get; set; }
        public List<HIS_SERVICE_UNIT> HisServiceUnit { get; set; }
        public List<HIS_OTHER_PAY_SOURCE> ListOtherPaySource { get; set; }
    }

    public class HisConfigValue
    {
        public bool IsPriceWithDifference { get; set; }
        public bool IsNotSameDepartment { get; set; }
        public bool IsGroupReqDepartment { get; set; }
        public bool IsGroupHeinServiceByUseTime { get; set; }
    }

    public class PatientTypeCFG
    {
        public long? PATIENT_TYPE__BHYT { get; set; }
        public long? PATIENT_TYPE__FEE { get; set; }
    }

    public class HeinServiceTypeCFG
    {
        public long? HEIN_SERVICE_TYPE__HIGHTECH_ID { get; set; }
        public long? HEIN_SERVICE_TYPE__MATERIAL_VTTT_ID { get; set; }
        public long? HEIN_SERVICE_TYPE__EXAM_ID { get; set; }
        public long? HEIN_SERVICE_TYPE__SURG_MISU_ID { get; set; }
        public long? HEIN_SERVICE_TYPE__MEDI_MATE_FROM_CABINET_ID { get; set; }
    }
}

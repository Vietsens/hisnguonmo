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
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000484.PDO
{
    public class Mps000484PDO : RDOBase
    {
        public HIS_SEVERE_ILLNESS_INFO severeIllnessInfo { get; set; }
        public List<HIS_EVENTS_CAUSES_DEATH> lstEventsCausesDeath { get; set; }
        public HIS_TREATMENT treatment { get; set; }
        public HIS_PATIENT_TYPE_ALTER patientTypeAlter { get; set; }
        public HIS_PATIENT patient { get; set; }
        public HIS_DEPARTMENT_TRAN departmentTran { get; set; }
        public HIS_DEPARTMENT department { get; set; }
        public HIS_BRANCH branch { get; set; }
        public List<HIS_ICD> Icds { get; set; }
        public List<HIS_TREATMENT_END_TYPE> treatmentEndType { get; set; }
        public List<HIS_TREATMENT_RESULT> treatmentResult { get; set; }
        public Mps000484PDO()
        {

        }
        public Mps000484PDO(HIS_SEVERE_ILLNESS_INFO _SevereIllnessInfo, List<HIS_EVENTS_CAUSES_DEATH> _LstEventsCausesDeath, HIS_TREATMENT _Treatment, HIS_PATIENT_TYPE_ALTER _PatientTypeAlter, HIS_PATIENT _Patient, HIS_DEPARTMENT_TRAN _DepartmentTran, HIS_DEPARTMENT _Department, HIS_BRANCH _Branch,List<HIS_ICD> Icds, List<HIS_TREATMENT_END_TYPE> treatmentEndType, List<HIS_TREATMENT_RESULT> treatmentResult)
        {
            try
            {
                this.severeIllnessInfo = _SevereIllnessInfo;
                this.lstEventsCausesDeath = _LstEventsCausesDeath;
                this.treatment = _Treatment;
                this.patientTypeAlter = _PatientTypeAlter;
                this.patient = _Patient;
                this.departmentTran = _DepartmentTran;
                this.department = _Department;
                this.branch = _Branch;
                this.Icds = Icds;
                this.treatmentEndType = treatmentEndType;
                this.treatmentResult = treatmentResult;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Debug(ex);
            }
        }

    }

}

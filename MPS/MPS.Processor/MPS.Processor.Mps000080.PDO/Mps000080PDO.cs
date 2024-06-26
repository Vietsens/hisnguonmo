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

namespace MPS.Processor.Mps000080.PDO
{
     public partial class Mps000080PDO : RDOBase
    {        
        public string departmentName;
        public List<SereServGroupPlusADO> sereServNotHiTechs { get; set; }
        public List<SereServGroupPlusADO> sereServHitechs { get; set; }
        public List<SereServGroupPlusADO> sereServHitechADOs { get; set; }
        public List<SereServGroupPlusADO> sereServVTTTs { get; set; }
        public List<MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN> departmentTrans;

        public List<HIS_HEIN_SERVICE_TYPE> ServiceReports { get; set; }

        public List<SereServGroupPlusADO> ServiceGroups { get; set; }
        public List<SereServGroupPlusADO> DepartmentGroups { get; set; }
        public List<SereServGroupPlusADO> ServiceHitechGroups { get; set; }
        public List<SereServGroupPlusADO> HitechDepartmentGroups { get; set; }

        public MOS.EFMODEL.DataModels.V_HIS_TRAN_PATI hisTranPati { get; set; }
        public List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_FEE> treatmentFees { get; set; }
        public long totalDay { get; set; }

        public Mps000080PDO(
           PatientADO patientADO,
           PatyAlterBhytADO patyAlterBhytADO,
            string departmentName,
            List<SereServGroupPlusADO> sereServNotHiTechs,
            List<SereServGroupPlusADO> sereServHitechs,
            List<SereServGroupPlusADO> sereServVTTTs,
            List<MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN> departmentTrans,
            V_HIS_TREATMENT currentHisTreatment,

            List<HIS_HEIN_SERVICE_TYPE> ServiceReports,

            MOS.EFMODEL.DataModels.V_HIS_TRAN_PATI hisTranPati,
            List<MOS.EFMODEL.DataModels.V_HIS_TREATMENT_FEE> treatmentFees,
            long totalDay
            
            )
        {
            try
            {
                this.patientADO = patientADO;
                this.patyAlterBhytADO = patyAlterBhytADO;
                this.departmentName = departmentName;
                this.sereServNotHiTechs = sereServNotHiTechs;
                this.sereServHitechs = sereServHitechs;
                this.sereServVTTTs = sereServVTTTs;
                this.departmentTrans = departmentTrans;
                this.currentHisTreatment = currentHisTreatment;
                this.ServiceReports = ServiceReports;
                this.hisTranPati = hisTranPati;
                this.treatmentFees = treatmentFees;
                this.totalDay = totalDay;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
     }
}

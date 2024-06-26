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

namespace MPS.Processor.Mps000102.PDO
{
    public partial class Mps000102PDO : RDOBase
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
        public List<SereServGroupPlusADO> HightTechDepartmentGroups { get; set; }
        public List<SereServGroupPlusADO> ServiceVTTTDepartmentGroup { get; set; }
        public List<HIS_SERE_SERV_DEPOSIT> dereDetails { get; set; }

        public long? totalDay { get; set; }
        public string ratio { get; set; }
        public decimal thuChenhLech, thuDongChiTra;

        public Mps000102PDO() { }

        public Mps000102PDO(
            PatientADO patientADO,
            V_HIS_PATIENT_TYPE_ALTER patyAlterBhytADO,
            string departmentName,
            List<SereServGroupPlusADO> sereServNotHiTechs,
            List<SereServGroupPlusADO> sereServHitechs,
            List<SereServGroupPlusADO> sereServVTTTs,
            List<MOS.EFMODEL.DataModels.V_HIS_DEPARTMENT_TRAN> departmentTrans,
            V_HIS_TREATMENT_FEE currentHisTreatment,
            List<HIS_HEIN_SERVICE_TYPE> ServiceReports,
            MOS.EFMODEL.DataModels.V_HIS_TRANSACTION HisDeposit,
            List<MOS.EFMODEL.DataModels.HIS_SERE_SERV_DEPOSIT> _dereDetails,
            long? totalDay,
            string ratio,
            V_HIS_SERVICE_REQ hisServiceReq
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
                this.totalDay = totalDay;
                this.deposit = HisDeposit;
                this.dereDetails = _dereDetails;
                this.ratio = ratio;
                this.hisServiceReq = hisServiceReq;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000314.PDO
{
    public class Mps000314PDO : MPS.ProcessorBase.Core.RDOBase
    {
        public SingleKeyValue SingleKeyValue { get; set; }
        public PatientTypeCFG PatientTypeCFG { get; set; }
        public List<HIS_HEIN_SERVICE_TYPE> HeinServiceTypes { get; set; }
        public List<V_HIS_SERVICE> Services { get; set; }
        public List<V_HIS_ROOM> Rooms { get; set; }
        public HIS_BRANCH Branch { get; set; }
        public List<HIS_MEDICINE_TYPE> medicineTypes { get; set; }
        public List<HIS_MATERIAL_TYPE> materialTypes { get; set; }
        public List<HIS_TREATMENT_TYPE> TreatmentTypes;
        public V_HIS_PATIENT_TYPE_ALTER CurrentPatyAlter { get; set; }
        public List<HIS_PATIENT_TYPE_ALTER> PatientTypeAlterAlls { get; set; }
        public List<HIS_SERE_SERV_EXT> SereServExts { get; set; }
        public List<HIS_SERVICE_REQ> ServiceReqs { get; set; }
        public List<HIS_DEPARTMENT> Departments { get; set; }
        public V_HIS_PATIENT Patient { get; set; }
        public List<HIS_SERVICE_UNIT> HisServiceUnit { get; set; }
        public List<V_HIS_DEPARTMENT_TRAN> DepartmentTrans { get; set; }
        public V_HIS_TREATMENT_FEE TreatmentFee { get; set; }
        public List<HIS_SERE_SERV> SereServs { get; set; }
        public V_HIS_TREATMENT Treatment { get; set; }
        public List<HIS_FUND> ListFund { get; set; }
        public List<HIS_MEDI_ORG> ListMediOrg { get; set; }

        public Mps000314PDO(
            V_HIS_PATIENT_TYPE_ALTER _currentPatyAlter,
            List<HIS_PATIENT_TYPE_ALTER> _patientTypeAlterAll,
            List<V_HIS_DEPARTMENT_TRAN> _departmentTrans,
            V_HIS_TREATMENT_FEE _treatmentFee,
            PatientTypeCFG _patientTypeCfg,
            List<HIS_SERE_SERV> _sereServ,
            List<HIS_SERE_SERV_EXT> _sereServExts,
            V_HIS_TREATMENT _treatment,
            V_HIS_PATIENT _patient,
            List<HIS_HEIN_SERVICE_TYPE> _heinServiceTypes,
            List<V_HIS_ROOM> _rooms,
            List<V_HIS_SERVICE> _services,
            List<HIS_TREATMENT_TYPE> _treatmentType,
            HIS_BRANCH _branch,
            List<HIS_MATERIAL_TYPE> _materialTypes,
            List<HIS_DEPARTMENT> _departments,
            SingleKeyValue _singleKeyValue,
            List<HIS_SERVICE_UNIT> _hisServiceUnit,
            List<HIS_FUND> _listFund,
            List<HIS_MEDI_ORG> _listMediOrg
            )
        {
            try
            {
                this.SereServs = _sereServ;
                this.Treatment = _treatment;
                this.DepartmentTrans = _departmentTrans;
                this.TreatmentFee = _treatmentFee;
                this.SingleKeyValue = _singleKeyValue;
                this.HeinServiceTypes = _heinServiceTypes;
                this.Services = _services;
                this.Rooms = _rooms;
                this.PatientTypeCFG = _patientTypeCfg;
                this.Branch = _branch;
                this.TreatmentTypes = _treatmentType;
                this.CurrentPatyAlter = _currentPatyAlter;
                this.SereServExts = _sereServExts;
                this.materialTypes = _materialTypes;
                this.Departments = _departments;
                this.PatientTypeAlterAlls = _patientTypeAlterAll;
                this.Patient = _patient;
                this.HisServiceUnit = _hisServiceUnit;
                this.ListFund = _listFund;
                this.ListMediOrg = _listMediOrg;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }

    public class PatientTypeCFG
    {
        public long? PATIENT_TYPE__BHYT { get; set; }
        public long? PATIENT_TYPE__FEE { get; set; }
    }

    public class SingleKeyValue
    {
        public string ratio { get; set; }
        public long today { get; set; }
        public string departmentName { get; set; }
        public string currentDateSeparateFullTime { get; set; }
        public string userNameReturnResult { get; set; }
        public string statusTreatmentOut { get; set; }
        public string mediStockName { get; set; }
        public string roomName { get; set; }
    }
}

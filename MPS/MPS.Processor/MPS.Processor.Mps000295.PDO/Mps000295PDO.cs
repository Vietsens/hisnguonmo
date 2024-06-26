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
using MPS.Processor.Mps000295.PDO.Config;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000295.PDO
{
    public partial class Mps000295PDO : RDOBase
    {
        public SingleKeyValue SingleKeyValue { get; set; }
        public HeinServiceTypeCFG HeinServiceTypeCFG { get; set; }
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
        public List<HIS_MEDICINE_LINE> MedicineLines { get; set; }
        public List<HIS_SERVICE_REQ> ServiceReqs { get; set; }
        public List<HIS_DEPARTMENT> Departments { get; set; }
        public V_HIS_PATIENT Patient { get; set; }
        public List<HIS_MEDI_ORG> ListMediOrg { get; set; }

        public Mps000295PDO(
            V_HIS_PATIENT_TYPE_ALTER _currentPatyAlter,
            List<HIS_PATIENT_TYPE_ALTER> _patientTypeAlterAll,
            List<V_HIS_DEPARTMENT_TRAN> _departmentTrans,
            List<V_HIS_TREATMENT_FEE> _treatmentFees,
            HeinServiceTypeCFG _heinServiceTypeCfg,
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
            List<HIS_MEDI_ORG> _listMediOrg
            )
        {
            try
            {
                this.CurrentPatyAlter = _currentPatyAlter;
                this.PatientTypeAlterAlls = _patientTypeAlterAll;
                this.DepartmentTrans = _departmentTrans;
                this.TreatmentFees = _treatmentFees;
                this.HeinServiceTypeCFG = _heinServiceTypeCfg;
                this.PatientTypeCFG = _patientTypeCfg;
                this.SereServs = _sereServ;
                this.SereServExts = _sereServExts;
                this.Treatment = _treatment;
                this.Patient = _patient;
                this.HeinServiceTypes = _heinServiceTypes;
                this.Rooms = _rooms;
                this.Services = _services;
                this.TreatmentTypes = _treatmentType;
                this.Branch = _branch;
                this.materialTypes = _materialTypes;
                this.Departments = _departments;
                this.SingleKeyValue = _singleKeyValue;
                this.ListMediOrg = _listMediOrg;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        /// <summary>
        /// Contructor này có _medicineLine, mục đích hiển thị gom nhóm của thuốc: Tân dược, chế phẩm
        /// </summary>
        public Mps000295PDO(
            V_HIS_PATIENT_TYPE_ALTER _currentPatyAlter,
            List<HIS_PATIENT_TYPE_ALTER> _patientTypeAlterAll,
            List<V_HIS_DEPARTMENT_TRAN> _departmentTrans,
            List<V_HIS_TREATMENT_FEE> _treatmentFees,
            HeinServiceTypeCFG _heinServiceTypeCfg,
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
            List<HIS_MEDICINE_TYPE> _medicineTypes,
            List<HIS_MATERIAL_TYPE> _materialTypes,
            List<HIS_MEDICINE_LINE> _medicineLine,
            List<HIS_SERVICE_REQ> _serviceReqs,
            List<HIS_DEPARTMENT> _departments,
            SingleKeyValue _singleKeyValue,
            List<HIS_MEDI_ORG> _listMediOrg
            )
        {
            try
            {
                this.CurrentPatyAlter = _currentPatyAlter;
                this.PatientTypeAlterAlls = _patientTypeAlterAll;
                this.DepartmentTrans = _departmentTrans;
                this.TreatmentFees = _treatmentFees;
                this.HeinServiceTypeCFG = _heinServiceTypeCfg;
                this.PatientTypeCFG = _patientTypeCfg;
                this.SereServs = _sereServ;
                this.SereServExts = _sereServExts;
                this.Treatment = _treatment;
                this.Patient = _patient;
                this.HeinServiceTypes = _heinServiceTypes;
                this.Rooms = _rooms;
                this.Services = _services;
                this.TreatmentTypes = _treatmentType;
                this.Branch = _branch;
                this.medicineTypes = _medicineTypes;
                this.materialTypes = _materialTypes;
                this.MedicineLines = _medicineLine;
                this.ServiceReqs = _serviceReqs;
                this.Departments = _departments;
                this.SingleKeyValue = _singleKeyValue;
                this.ListMediOrg = _listMediOrg;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

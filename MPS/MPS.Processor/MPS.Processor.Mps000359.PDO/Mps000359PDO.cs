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

namespace MPS.Processor.Mps000359.PDO
{
    public class Mps000359PDO : RDOBase
    {
        public SingleKeyValue SingleKeyValue { get; set; }
        public PatientTypeCFG PatientTypeCFG { get; set; }
        public HisConfigValue HisConfigValue { get; set; }
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
        public List<V_HIS_TREATMENT_FEE> TreatmentFees { get; set; }
        public List<HIS_SERE_SERV> SereServs { get; set; }
        public V_HIS_TREATMENT Treatment { get; set; }
        public List<HIS_PATIENT_TYPE> HisPatientType { get; set; }
        public List<HIS_OTHER_PAY_SOURCE> ListOtherPaySource { get; set; }
        public List<HIS_SERE_SERV_BILL> ListSereServBills { get; set; }
        public List<HIS_MEDI_ORG> ListMediOrg { get; set; }
        public List<HIS_SERE_SERV_DEPOSIT> ListSereServDeposits { get; set; }
        public List<HIS_SESE_DEPO_REPAY> ListSeseDepoRepays { get; set; }
        public List<HIS_CONFIG> lstConfig { get; set; }
        public HIS_TRANS_REQ transReq { get; set; }
        public Mps000359PDO(
            V_HIS_PATIENT_TYPE_ALTER _currentPatyAlter,
            List<HIS_PATIENT_TYPE_ALTER> _patientTypeAlterAll,
            List<V_HIS_DEPARTMENT_TRAN> _departmentTrans,
            List<V_HIS_TREATMENT_FEE> _treatmentFees,
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
            HisConfigValue _hisConfigValue,
            List<HIS_SERVICE_UNIT> _hisServiceUnit,
            List<HIS_PATIENT_TYPE> _hisPatientType,
            List<HIS_OTHER_PAY_SOURCE> _listOtherPaySource,
            List<HIS_SERE_SERV_BILL> _listSereServBills,
            List<HIS_MEDI_ORG> _listMediOrg
            )
        {
            try
            {
                this.SereServs = _sereServ;
                this.Treatment = _treatment;
                this.DepartmentTrans = _departmentTrans;
                this.TreatmentFees = _treatmentFees;
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
                this.HisConfigValue = _hisConfigValue;
                this.Patient = _patient;
                this.HisServiceUnit = _hisServiceUnit;
                this.HisPatientType = _hisPatientType;
                this.ListOtherPaySource = _listOtherPaySource;
                this.ListSereServBills = _listSereServBills;
                this.ListMediOrg = _listMediOrg;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000359PDO(
            V_HIS_PATIENT_TYPE_ALTER _currentPatyAlter,
            List<HIS_PATIENT_TYPE_ALTER> _patientTypeAlterAll,
            List<V_HIS_DEPARTMENT_TRAN> _departmentTrans,
            List<V_HIS_TREATMENT_FEE> _treatmentFees,
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
            HisConfigValue _hisConfigValue,
            List<HIS_SERVICE_UNIT> _hisServiceUnit,
            List<HIS_PATIENT_TYPE> _hisPatientType,
            List<HIS_OTHER_PAY_SOURCE> _listOtherPaySource,
            List<HIS_SERE_SERV_BILL> _listSereServBills,
            List<HIS_MEDI_ORG> _listMediOrg,
            List<HIS_SERE_SERV_DEPOSIT> _listSereServDeposits,
            List<HIS_SESE_DEPO_REPAY> _listSeseDepoRepays
            )
        {
            try
            {
                this.SereServs = _sereServ;
                this.Treatment = _treatment;
                this.DepartmentTrans = _departmentTrans;
                this.TreatmentFees = _treatmentFees;
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
                this.HisConfigValue = _hisConfigValue;
                this.Patient = _patient;
                this.HisServiceUnit = _hisServiceUnit;
                this.HisPatientType = _hisPatientType;
                this.ListOtherPaySource = _listOtherPaySource;
                this.ListSereServBills = _listSereServBills;
                this.ListMediOrg = _listMediOrg;
                this.ListSereServDeposits = _listSereServDeposits;
                this.ListSeseDepoRepays = _listSeseDepoRepays;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public Mps000359PDO(
    V_HIS_PATIENT_TYPE_ALTER _currentPatyAlter,
    List<HIS_PATIENT_TYPE_ALTER> _patientTypeAlterAll,
    List<V_HIS_DEPARTMENT_TRAN> _departmentTrans,
    List<V_HIS_TREATMENT_FEE> _treatmentFees,
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
    HisConfigValue _hisConfigValue,
    List<HIS_SERVICE_UNIT> _hisServiceUnit,
    List<HIS_PATIENT_TYPE> _hisPatientType,
    List<HIS_OTHER_PAY_SOURCE> _listOtherPaySource,
    List<HIS_SERE_SERV_BILL> _listSereServBills,
    List<HIS_MEDI_ORG> _listMediOrg,
            List<HIS_CONFIG> lstConfig,
            HIS_TRANS_REQ transReq
    )
        {
            try
            {
                this.SereServs = _sereServ;
                this.Treatment = _treatment;
                this.DepartmentTrans = _departmentTrans;
                this.TreatmentFees = _treatmentFees;
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
                this.HisConfigValue = _hisConfigValue;
                this.Patient = _patient;
                this.HisServiceUnit = _hisServiceUnit;
                this.HisPatientType = _hisPatientType;
                this.ListOtherPaySource = _listOtherPaySource;
                this.ListSereServBills = _listSereServBills;
                this.ListMediOrg = _listMediOrg; 
                this.lstConfig = lstConfig;
                this.transReq = transReq;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000359PDO(
            V_HIS_PATIENT_TYPE_ALTER _currentPatyAlter,
            List<HIS_PATIENT_TYPE_ALTER> _patientTypeAlterAll,
            List<V_HIS_DEPARTMENT_TRAN> _departmentTrans,
            List<V_HIS_TREATMENT_FEE> _treatmentFees,
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
            HisConfigValue _hisConfigValue,
            List<HIS_SERVICE_UNIT> _hisServiceUnit,
            List<HIS_PATIENT_TYPE> _hisPatientType,
            List<HIS_OTHER_PAY_SOURCE> _listOtherPaySource,
            List<HIS_SERE_SERV_BILL> _listSereServBills,
            List<HIS_MEDI_ORG> _listMediOrg,
            List<HIS_SERE_SERV_DEPOSIT> _listSereServDeposits,
            List<HIS_SESE_DEPO_REPAY> _listSeseDepoRepays,
            List<HIS_CONFIG> lstConfig,
            HIS_TRANS_REQ transReq
            )
        {
            try
            {
                this.SereServs = _sereServ;
                this.Treatment = _treatment;
                this.DepartmentTrans = _departmentTrans;
                this.TreatmentFees = _treatmentFees;
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
                this.HisConfigValue = _hisConfigValue;
                this.Patient = _patient;
                this.HisServiceUnit = _hisServiceUnit;
                this.HisPatientType = _hisPatientType;
                this.ListOtherPaySource = _listOtherPaySource;
                this.ListSereServBills = _listSereServBills;
                this.ListMediOrg = _listMediOrg;
                this.ListSereServDeposits = _listSereServDeposits;
                this.ListSeseDepoRepays = _listSeseDepoRepays;
                this.lstConfig = lstConfig;
                this.transReq = transReq;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

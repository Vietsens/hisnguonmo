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
using MPS.Processor.Mps000302.PDO.Config;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000302.PDO
{
    public partial class Mps000302PDO : RDOBase
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
        public HisConfigValue HisConfigValue { get; set; }
        public V_HIS_PATIENT Patient { get; set; }
        public List<HIS_SERVICE_UNIT> HisServiceUnit { get; set; }
        public List<HIS_TRANSACTION> ListTransactionBill { get; set; }
        public List<HIS_MEDI_ORG> ListMediOrg { get; set; }
        public List<HIS_PATIENT_TYPE> ListPatientType { get; set; }
        public List<HIS_SERE_SERV_BILL> ListSereServBills { get; set; }
        public List<HIS_SERE_SERV_DEPOSIT> ListSereServDeposits { get; set; }
        public List<HIS_SESE_DEPO_REPAY> ListSeseDepoRepays { get; set; }
        public HIS_TRANS_REQ TransReq { get; set; }
        public List<HIS_CONFIG> ListHisConfigPaymentQrCode { get; set; }

        public Mps000302PDO() { }

        public Mps000302PDO(
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
            HisConfigValue _hisConfigValue,
            List<HIS_SERVICE_UNIT> _hisServiceUnit,
            List<HIS_MEDI_ORG> _listMediOrg,
            List<HIS_PATIENT_TYPE> _listPatientType,
            List<HIS_SERE_SERV_BILL> _listSereServBills,
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
                this.HeinServiceTypeCFG = _heinServiceTypeCfg;
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
                this.ListMediOrg = _listMediOrg;
                this.ListPatientType = _listPatientType;
                this.ListSereServBills = _listSereServBills;
                this.ListSereServDeposits = _listSereServDeposits;
                this.ListSeseDepoRepays = _listSeseDepoRepays;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        public Mps000302PDO(
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
            HisConfigValue _hisConfigValue,
            List<HIS_SERVICE_UNIT> _hisServiceUnit,
            List<HIS_MEDI_ORG> _listMediOrg,
            List<HIS_PATIENT_TYPE> _listPatientType,
            List<HIS_SERE_SERV_BILL> _listSereServBills,
            List<HIS_SERE_SERV_DEPOSIT> _listSereServDeposits,
            List<HIS_SESE_DEPO_REPAY> _listSeseDepoRepays,
            HIS_TRANS_REQ transReq,
            List<HIS_CONFIG> listHisConfigPaymentQrCode
            )
        {
            try
            {
                this.SereServs = _sereServ;
                this.Treatment = _treatment;
                this.DepartmentTrans = _departmentTrans;
                this.TreatmentFees = _treatmentFees;
                this.HeinServiceTypeCFG = _heinServiceTypeCfg;
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
                this.ListMediOrg = _listMediOrg;
                this.ListPatientType = _listPatientType;
                this.ListSereServBills = _listSereServBills;
                this.ListSereServDeposits = _listSereServDeposits;
                this.ListSeseDepoRepays = _listSeseDepoRepays;
                this.TransReq = transReq;
                this.ListHisConfigPaymentQrCode = listHisConfigPaymentQrCode;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

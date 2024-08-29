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

namespace MPS.Processor.Mps000321.PDO
{
    public class Mps000321PDO : RDOBase
    {
        public SingleKeyValue SingleKeyValue { get; set; }
        public PatientTypeCFG PatientTypeCFG { get; set; }
        public List<HIS_HEIN_SERVICE_TYPE> HeinServiceTypes { get; set; }
        public List<V_HIS_SERVICE> Services { get; set; }
        public List<V_HIS_ROOM> Rooms { get; set; }
        public HIS_BRANCH Branch { get; set; }
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
        public HisConfigValue HisConfigValue { get; set; }
        public List<HIS_PATIENT_TYPE> HisPatientType { get; set; }
        public List<HIS_MEDI_ORG> ListMediOrg { get; set; }
        public List<HIS_OTHER_PAY_SOURCE> ListOtherPaySource { get; set; }
        public List<V_HIS_TRANSACTION> ListTransaction { get; set; }
        public List<HIS_CONFIG> listConfigPaymentQrCode { get; set; }
        public HIS_TRANS_REQ tranReq { get; set; }

        public Mps000321PDO() { }

        public Mps000321PDO(
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
            List<HIS_PATIENT_TYPE> _patientType,
            List<HIS_MEDI_ORG> _listMediOrg,
            List<HIS_OTHER_PAY_SOURCE> _listOtherPaySource,
            List<V_HIS_TRANSACTION> _ListTransaction
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
                this.HisPatientType = _patientType;
                this.ListMediOrg = _listMediOrg;
                this.ListOtherPaySource = _listOtherPaySource;
                this.ListTransaction = _ListTransaction;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        // contructor co danh sach cau hinh in QR
        public Mps000321PDO(
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
            List<HIS_PATIENT_TYPE> _patientType,
            List<HIS_MEDI_ORG> _listMediOrg,
            List<HIS_OTHER_PAY_SOURCE> _listOtherPaySource,
            List<V_HIS_TRANSACTION> _ListTransaction,
            List<HIS_CONFIG> config,
            HIS_TRANS_REQ trans
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
                this.HisPatientType = _patientType;
                this.ListMediOrg = _listMediOrg;
                this.ListOtherPaySource = _listOtherPaySource;
                this.ListTransaction = _ListTransaction;
                this.listConfigPaymentQrCode = config;
                this.tranReq = trans;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

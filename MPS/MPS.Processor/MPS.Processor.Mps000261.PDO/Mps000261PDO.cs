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
using MPS.Processor.Mps000261.PDO.Config;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000261.PDO
{
    public partial class Mps000261PDO : RDOBase
    {
        public SingleKeyValue SingleKeyValue { get; set; }
        public HeinServiceTypeCFG HeinServiceTypeCFG { get; set; }
        public PatientTypeCFG PatientTypeCFG { get; set; }
        public TransactionTypeCFG TransactionTypeCFG { get; set; }
        public List<HIS_HEIN_SERVICE_TYPE> HeinServiceTypes { get; set; }
        public List<V_HIS_SERVICE> Services { get; set; }
        public List<V_HIS_ROOM> Rooms { get; set; }
        public List<HIS_TRANSACTION> Transactions { get; set; }
        public List<HIS_MATERIAL_TYPE> MaterialTypes { get; set; }
        public List<HIS_MEDICINE_TYPE> medicineTypes { get; set; }
        public List<HIS_MEDICINE_LINE> MedicineLines { get; set; }
        public List<HIS_SERVICE_REQ> ServiceReqs { get; set; }
        public List<HIS_CONFIG> listConfig { get; set; }
        public HIS_TRANS_REQ trans{ get; set; }

        public Mps000261PDO(
            List<V_HIS_DEPARTMENT_TRAN> _departmentTrans,
            List<V_HIS_TREATMENT_FEE> _treatmentFees,
            HeinServiceTypeCFG _heinServiceTypeCfg,
            PatientTypeCFG _patientTypeCFG,
            TransactionTypeCFG _transactionTypeCFG,
            List<HIS_SERE_SERV> _sereServ,
            List<HIS_TRANSACTION> _transactions,
            V_HIS_TREATMENT _treatment,
            List<HIS_HEIN_SERVICE_TYPE> _heinServiceTypes,
            List<V_HIS_ROOM> _rooms,
            List<V_HIS_SERVICE> _services,
            List<HIS_MEDICINE_TYPE> _medicineTypes,
            List<HIS_MATERIAL_TYPE> _materialTypes,
            List<HIS_MEDICINE_LINE> _medicineLines,
            List<HIS_SERVICE_REQ> _serviceReqs,
            SingleKeyValue _singleKeyValue
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
                this.PatientTypeCFG = _patientTypeCFG;
                this.Transactions = _transactions;
                this.TransactionTypeCFG = _transactionTypeCFG;
                this.MaterialTypes = _materialTypes;
                this.MedicineLines = _medicineLines;
                this.medicineTypes = _medicineTypes;
                this.ServiceReqs = _serviceReqs;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000261PDO(
            List<V_HIS_DEPARTMENT_TRAN> _departmentTrans,
            List<V_HIS_TREATMENT_FEE> _treatmentFees,
            HeinServiceTypeCFG _heinServiceTypeCfg,
            PatientTypeCFG _patientTypeCFG,
            TransactionTypeCFG _transactionTypeCFG,
            List<HIS_SERE_SERV> _sereServ,
            List<HIS_TRANSACTION> _transactions,
            V_HIS_TREATMENT _treatment,
            List<HIS_HEIN_SERVICE_TYPE> _heinServiceTypes,
            List<V_HIS_ROOM> _rooms,
            List<V_HIS_SERVICE> _services,
            List<HIS_MATERIAL_TYPE> _materialTypes,
            SingleKeyValue _singleKeyValue
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
                this.PatientTypeCFG = _patientTypeCFG;
                this.Transactions = _transactions;
                this.TransactionTypeCFG = _transactionTypeCFG;
                this.MaterialTypes = _materialTypes;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        //
        public Mps000261PDO(
            List<V_HIS_DEPARTMENT_TRAN> _departmentTrans,
            List<V_HIS_TREATMENT_FEE> _treatmentFees,
            HeinServiceTypeCFG _heinServiceTypeCfg,
            PatientTypeCFG _patientTypeCFG,
            TransactionTypeCFG _transactionTypeCFG,
            List<HIS_SERE_SERV> _sereServ,
            List<HIS_TRANSACTION> _transactions,
            V_HIS_TREATMENT _treatment,
            List<HIS_HEIN_SERVICE_TYPE> _heinServiceTypes,
            List<V_HIS_ROOM> _rooms,
            List<V_HIS_SERVICE> _services,
            List<HIS_MEDICINE_TYPE> _medicineTypes,
            List<HIS_MATERIAL_TYPE> _materialTypes,
            List<HIS_MEDICINE_LINE> _medicineLines,
            List<HIS_SERVICE_REQ> _serviceReqs,
            SingleKeyValue _singleKeyValue,
            List<HIS_CONFIG> listConfig,
            HIS_TRANS_REQ trans
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
                this.PatientTypeCFG = _patientTypeCFG;
                this.Transactions = _transactions;
                this.TransactionTypeCFG = _transactionTypeCFG;
                this.MaterialTypes = _materialTypes;
                this.MedicineLines = _medicineLines;
                this.medicineTypes = _medicineTypes;
                this.ServiceReqs = _serviceReqs;
                this.listConfig = listConfig;
                this.trans = trans;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000261PDO(
            List<V_HIS_DEPARTMENT_TRAN> _departmentTrans,
            List<V_HIS_TREATMENT_FEE> _treatmentFees,
            HeinServiceTypeCFG _heinServiceTypeCfg,
            PatientTypeCFG _patientTypeCFG,
            TransactionTypeCFG _transactionTypeCFG,
            List<HIS_SERE_SERV> _sereServ,
            List<HIS_TRANSACTION> _transactions,
            V_HIS_TREATMENT _treatment,
            List<HIS_HEIN_SERVICE_TYPE> _heinServiceTypes,
            List<V_HIS_ROOM> _rooms,
            List<V_HIS_SERVICE> _services,
            List<HIS_MATERIAL_TYPE> _materialTypes,
            SingleKeyValue _singleKeyValue,
            List<HIS_CONFIG> listConfig,
            HIS_TRANS_REQ trans
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
                this.PatientTypeCFG = _patientTypeCFG;
                this.Transactions = _transactions;
                this.TransactionTypeCFG = _transactionTypeCFG;
                this.MaterialTypes = _materialTypes;
                this.listConfig = listConfig;
                this.trans = trans;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

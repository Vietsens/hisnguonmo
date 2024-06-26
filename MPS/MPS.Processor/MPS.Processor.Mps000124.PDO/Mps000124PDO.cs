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
using MPS.Processor.Mps000124.PDO.Config;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000124.PDO
{
    public partial class Mps000124PDO : RDOBase
    {
        public SingleKeyValue SingleKeyValue { get; set; }
        public HeinServiceTypeCFG HeinServiceTypeCFG { get; set; }
        public PatientTypeCFG PatientTypeCFG { get; set; }
        public List<HIS_HEIN_SERVICE_TYPE> HeinServiceTypes { get; set; }
        public List<V_HIS_SERVICE> Services { get; set; }
        public List<V_HIS_ROOM> Rooms { get; set; }
        public List<HIS_SERE_SERV_DEPOSIT> SereServDeposit { get; set; }
        public List<HIS_TRANSACTION> Bills { get; set; }
        public List<HIS_DEPARTMENT> Department { get; set; }
        public TransactionTypeCFG transactionTypeCFG { get; set; }
        public List<HIS_SESE_DEPO_REPAY> SeseDepoRepay { get; set; }
        public List<HIS_MATERIAL_TYPE> MaterialTypes { get; set; }

        public Mps000124PDO(
            HIS_PATIENT_TYPE_ALTER _patyAlter,
            List<V_HIS_DEPARTMENT_TRAN> _departmentTrans,
            List<V_HIS_TREATMENT_FEE> _treatmentFees,
            List<HIS_DEPARTMENT> _department,
            HeinServiceTypeCFG _heinServiceTypeCfg,
            PatientTypeCFG _patientTypeCFG,
            List<HIS_SERE_SERV> _sereServ,
            List<HIS_SERE_SERV_DEPOSIT> _sereServDeposit,
            List<HIS_SESE_DEPO_REPAY> _seseDepoRepay,
            List<HIS_TRANSACTION> _bills,
            TransactionTypeCFG _transactionTypeCFG,
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
                this.PatyAlter = _patyAlter;
                this.SereServs = _sereServ;
                this.Treatment = _treatment;
                this.DepartmentTrans = _departmentTrans;
                this.TreatmentFees = _treatmentFees;
                this.HeinServiceTypeCFG = _heinServiceTypeCfg;
                this.SingleKeyValue = _singleKeyValue;
                this.HeinServiceTypes = _heinServiceTypes;
                this.Services = _services;
                this.Rooms = _rooms;
                this.SereServDeposit = _sereServDeposit;
                this.Bills = _bills;
                this.Department = _department;
                this.PatientTypeCFG = _patientTypeCFG;
                this.transactionTypeCFG = _transactionTypeCFG;
                this.SeseDepoRepay = _seseDepoRepay;
                this.MaterialTypes = _materialTypes;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

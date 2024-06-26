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

namespace MPS.Processor.Mps000110.PDO
{
    public partial class Mps000110PDO : RDOBase
    {
        public const string printTypeCode = "Mps000110";

        public string departmentName;
        public long? totalDay { get; set; }

        public List<HIS_SERE_SERV_DEPOSIT> ListSsDeposit { get; set; }
        public List<V_HIS_TRANSACTION> ListDeposit { get; set; }

        public Mps000110PDO() { }

        public Mps000110PDO(
             PatientADO patientADO,
             V_HIS_PATIENT_TYPE_ALTER V_HIS_PATIENT_TYPE_ALTER,
             string departmentName,
             List<HIS_SESE_DEPO_REPAY> _dereDetails,
             List<V_HIS_DEPARTMENT_TRAN> departmentTrans,
             V_HIS_TREATMENT_1 currentHisTreatment,
             MOS.EFMODEL.DataModels.V_HIS_TRANSACTION _repay,
             long? totalDay,
            V_HIS_DEPARTMENT_TRAN departmentTran
             )
        {
            try
            {
                this.patientADO = patientADO;
                this.V_HIS_PATIENT_TYPE_ALTER = V_HIS_PATIENT_TYPE_ALTER;
                this.departmentName = departmentName;
                this.departmentTrans = departmentTrans;
                this.currentHisTreatment = currentHisTreatment;
                this.totalDay = totalDay;
                repay = _repay;
                this.dereDetails = _dereDetails;
                this.DepartmentTran = departmentTran;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        public Mps000110PDO(
            PatientADO patientADO,
            V_HIS_PATIENT_TYPE_ALTER V_HIS_PATIENT_TYPE_ALTER,
            string departmentName,
            List<HIS_SESE_DEPO_REPAY> _dereDetails,
            List<V_HIS_DEPARTMENT_TRAN> departmentTrans,
            V_HIS_TREATMENT_1 currentHisTreatment,
            MOS.EFMODEL.DataModels.V_HIS_TRANSACTION _repay,
            long? totalDay,
            V_HIS_DEPARTMENT_TRAN departmentTran,
            List<HIS_SERE_SERV_DEPOSIT> listSsDeposit,
            List<V_HIS_TRANSACTION> listDeposit
            )
        {
            try
            {
                this.patientADO = patientADO;
                this.V_HIS_PATIENT_TYPE_ALTER = V_HIS_PATIENT_TYPE_ALTER;
                this.departmentName = departmentName;
                this.departmentTrans = departmentTrans;
                this.currentHisTreatment = currentHisTreatment;
                this.totalDay = totalDay;
                repay = _repay;
                this.dereDetails = _dereDetails;
                this.DepartmentTran = departmentTran;
                this.ListSsDeposit = listSsDeposit;
                this.ListDeposit = listDeposit;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

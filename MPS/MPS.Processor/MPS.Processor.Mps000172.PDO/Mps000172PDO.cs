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

namespace MPS.Processor.Mps000172.PDO
{
    public partial class Mps000172PDO : RDOBase
    {
        public decimal ratio = 0;
        public Mps000172PDO() { }

        public Mps000172PDO(V_HIS_TRANSACTION transaction, V_HIS_PATIENT_TYPE_ALTER patyAlterBhyt, V_HIS_DEPARTMENT_TRAN departmentTran, decimal ratio)
        {
            this._BatyAlterBhyt = patyAlterBhyt;
            this._Transaction = transaction;
            this._DepartmentTran = departmentTran;
            this.ratio = ratio;
        }

        public Mps000172PDO(V_HIS_TRANSACTION transaction, V_HIS_PATIENT_TYPE_ALTER patyAlterBhyt, List<V_HIS_DEPARTMENT_TRAN> departmentTrans, decimal ratio)
        {
            this._BatyAlterBhyt = patyAlterBhyt;
            this._Transaction = transaction;
            this.DepartmentTranAllByTreatments = departmentTrans;
            this.ratio = ratio;
        }

        public Mps000172PDO(V_HIS_TRANSACTION transaction, V_HIS_PATIENT_TYPE_ALTER patyAlterBhyt, List<V_HIS_DEPARTMENT_TRAN> departmentTrans, decimal ratio, HIS_PATIENT patient)
        {
            this._BatyAlterBhyt = patyAlterBhyt;
            this._Transaction = transaction;
            this.DepartmentTranAllByTreatments = departmentTrans;
            this.ratio = ratio;
            this._Patient = patient;
        }

      
    }
}

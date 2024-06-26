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

namespace MPS.Processor.Mps000113.PDO
{
    public partial class Mps000113PDO : RDOBase
    {
        public const string printTypeCode = "Mps000113";
        public decimal ratio = 0;
        public List<V_HIS_TRANSACTION> ListBill;
        public V_HIS_TREATMENT_FEE Treatment;

        public Mps000113PDO() { }

        public Mps000113PDO(V_HIS_TRANSACTION transactionRepay, V_HIS_PATIENT patient, decimal ratio, V_HIS_PATIENT_TYPE_ALTER patyAlterBHYT, V_HIS_DEPARTMENT_TRAN departmentTran, V_HIS_TREATMENT_FEE _treatment, List<V_HIS_TRANSACTION> _listBill)
        {
            this._Patient = patient;
            this._Transaction = transactionRepay;
            this.ratio = ratio;
            this._PatyAlterBHYT = patyAlterBHYT;
            this._DepartmentTran = departmentTran;
            this.ListBill = _listBill;
            this.Treatment = _treatment;
        }
    }
}

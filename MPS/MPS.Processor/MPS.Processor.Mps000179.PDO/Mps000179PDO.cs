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

namespace MPS.Processor.Mps000179.PDO
{
    public class Mps000179PDO : RDOBase
    {
        public V_HIS_PATIENT currentPatient { get; set; }
        public V_HIS_TREATMENT currentTreatment { get; set; }
        public V_HIS_PATIENT_TYPE_ALTER currentPatyAlterBhyt { get; set; }
        public V_HIS_DEPARTMENT_TRAN currentDepartmentTran { get; set; }

        public Mps000179PDO() { }

        public Mps000179PDO(
            V_HIS_PATIENT currentPatient,
            V_HIS_TREATMENT currentTreatment,
            V_HIS_PATIENT_TYPE_ALTER currentPatyAlterBhyt,
            V_HIS_DEPARTMENT_TRAN currentDepartmentTran
            )
        {
            this.currentPatient = currentPatient;
            this.currentTreatment = currentTreatment;
            this.currentPatyAlterBhyt = currentPatyAlterBhyt;
            this.currentDepartmentTran = currentDepartmentTran;
        }
    }
}

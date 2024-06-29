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

namespace MPS.Processor.Mps000178.PDO
{
    public class Mps000178PDO : RDOBase
    {
        public V_HIS_PATIENT currentPatient { get; set; }
        public V_HIS_PATIENT_TYPE_ALTER patientTypeAlter { get; set; }
        public V_HIS_TREATMENT_4 treatment4 { get; set; }
        public V_HIS_DEPARTMENT_TRAN departmentTran { get; set; }
        public Mps000178PDO() { }

        public Mps000178PDO(V_HIS_PATIENT currentPatient)
        {
            this.currentPatient = currentPatient;
        }

        public Mps000178PDO(V_HIS_PATIENT currentPatient, V_HIS_PATIENT_TYPE_ALTER _patientTypeAlter, V_HIS_TREATMENT_4 _treatment4, V_HIS_DEPARTMENT_TRAN departmentTran)
        {
            this.currentPatient = currentPatient;
            this.patientTypeAlter = _patientTypeAlter;
            this.treatment4 = _treatment4;
            this.departmentTran = departmentTran;
        }
    }
}

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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.ADO
{
    public class PatientTypeDepartmentADO
    {
        public V_HIS_DEPARTMENT_TRAN departmentTran { get; set; }
        public V_HIS_PATIENT_TYPE_ALTER patientTypeAlter { get; set; }
        public V_HIS_CO_TREATMENT coTreatment { get; set; }
        public object HisPatyAlterDetail { get; set; }
        public long type { get; set; }
        public short? IsActiveDepartmentTran { get; set; }
        public short? IS_ACTIVE { get; set; }
        public long? CREATE_TIME { get; set; }
        public long? MODIFY_TIME { get; set; }
        public long LOG_TIME { get; set; }
        public long TREATMENT_ID { get; set; }
        public long PATIENT_ID { get; set; }
        public string CREATOR { get; set; }
        public string MODIFIER { get; set; }
        public string Id { get; set; }
        public string ParentId { get; set; }
        public long? HEIN_CARD_FROM_TIME
        {
            get;
            set;
        }

        public long? HEIN_CARD_TO_TIME
        {
            get;
            set;
        }

    }
}

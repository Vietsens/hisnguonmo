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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ServiceReqPatient.ADO
{
    class Printf
    {
        public string PARENT_ORGANIZATION_NAME{get;set;}
        public string TDL_PATIENT_CODE { get; set; }
        public string ORGANIZATION_NAME { get; set; }
        public string TREATMENT_CODE { get; set; }
        public string DEPARTMENT_NAME { get; set; }
        public string PATIENT_NAME { get; set; }
        public string AGE { get; set; }
        public string PATIENT_GENDER_NAME { get; set; }
        public string TDL_HEIN_CARD_NUMBER { get; set; }
        public string HEIN_CARD_TO_TIME { get; set; }
        public string BED_ROOM_NAME { get; set; }
        public string BED_NAME { get; set; }
        public string IN_TIME_STR { get; set; }
        public string ICD_NAME { get; set; }
        public string ICD_TEXT { get; set; }
        public string TABLE { get; set; }
    }
}

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

namespace MPS.Processor.Mps000306.ADO
{
    public class GroupDepartmentADO
    {
        public string KEY_PATY_ALTER { get; set; }
        public long GROUP_DEPARTMENT_ID { get; set; }
        public long GROUP_DEPARTMENT_ID__DepaRoom { get; set; }
        public long GROUP_ROOM_ID__ExeRoom { get; set; }
        public decimal? TOTAL_PRICE_BHYT_HEIN_SERVICE_TYPE { get; set; }
        public decimal? TOTAL_PRICE_HEIN_SERVICE_TYPE { get; set; }
        public decimal? TOTAL_HEIN_PRICE_HEIN_SERVICE_TYPE { get; set; }
        public decimal? TOTAL_PATIENT_PRICE_VIR_HEIN_SERVICE_TYPE { get; set; }
        public decimal? TOTAL_PATIENT_PRICE_HEIN_SERVICE_TYPE { get; set; }
        public decimal? TOTAL_PATIENT_PRICE_SELF_HEIN_SERVICE_TYPE { get; set; }
        public string DEPARTMENT_CODE { get; set; }
        public string DEPARTMENT_NAME { get; set; }
        public string ROOM_CODE { get; set; }
        public string ROOM_NAME { get; set; }

        public decimal TOTAL_PATIENT_PRICE_LEFT { get; set; }
        public decimal TOTAL_PRICE_VP { get; set; }
        public decimal? OTHER_SOURCE_PRICE { get; set; }

        public short? IS_CLINICAL { get; set; }
        public decimal TOTAL_PATIENT_LEFT { get; set; }
        public string GROUP_ROOM_CODE { get; set; }
        public string GROUP_ROOM_NAME { get; set; }
    }
}

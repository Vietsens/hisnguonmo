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

namespace MPS.ADO
{
    public class ExeInfusionDetailSDO : MOS.EFMODEL.DataModels.V_HIS_INFUSION
    {
        public string CREATE_TIME_STR { get; set; }
        public string EXPIRED_DATE_STR { get; set; }
        public string START_TIME_STR { get; set; }
        public string FINISH_TIME_STR { get; set; }
        public decimal? SPEED { get; set; }
        public string EXECUTE_LOGINNAME { get; set; }
        public string EXECUTE_USERNAME { get; set; }
        public string REQUEST_LOGINNAME { get; set; }
        public string REQUEST_USERNAME { get; set; }
        public string EXECUTE_DEPARTMENT_CODE { get; set; }
        public long? EXECUTE_DEPARTMENT_ID { get; set; }
        public string EXECUTE_DEPARTMENT_NAME { get; set; }
        public string EXECUTE_ROOM_CODE { get; set; }
        public long? EXECUTE_ROOM_ID { get; set; }
        public string EXECUTE_ROOM_NAME { get; set; }
    }
}

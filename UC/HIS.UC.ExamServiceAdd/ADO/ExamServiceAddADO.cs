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
using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.HisExamServiceAdd.ADO
{
    public class ExamServiceAddADO : HisServiceReqExamAdditionSDO
    {
        public long? FinishTime { get; set; }
        public bool IsPrintExamAdd { get; set; }
        public bool IsSignExamAdd { get; set; }
        public bool IsAppointment { get; set; }
        public bool IsPrintAppointment { get; set; }
        public long? AppointmentTime { get; set; }
        public string Advise { get; set; }
        public bool IsBlockNumOrder { get; set; }
        public long? DefaultIdRoom { get; set; }
        public long? NumOrderBlockId { get; set; }
        public long? RoomApointmentId { get; set; }
        public long? ServiceApointmentId { get; set; }
        public string Note { get; set; }
    }
}

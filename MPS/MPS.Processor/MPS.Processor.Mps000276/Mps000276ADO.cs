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

namespace MPS.Processor.Mps000276
{
    public class Mps000276ADO : HIS_SERE_SERV
    {
        public int RowNum { get; set; }
        public long? CashierRoomId { get; set; }
        public string CashierRoomCode { get; set; }
        public string CashierRoomName { get; set; }
        public string CashierRoomAddress { get; set; }

        public long? ParentServiceId { get; set; }
        public string ParentServiceCode { get; set; }
        public string ParentServiceName { get; set; }

        public long ExecuteRoomId { get; set; }
        public string ExecuteRoomCode { get; set; }
        public string ExecuteRoomName { get; set; }
        public string ExecuteRoomAddress { get; set; }

        public long RequestRoomId { get; set; }
        public string RequestRoomCode { get; set; }
        public string RequestRoomName { get; set; }
        public string RequestRoomAddress { get; set; }

        public long ServiceId { get; set; }
        public string ServiceCode { get; set; }
        public string ServiceName { get; set; }
        public long ServiceTypeId { get; set; }
        public string ServiceTypeCode { get; set; }
        public string ServiceTypeName { get; set; }

        public long? ResultRoomId { get; set; }
        public string ResultRoomCode { get; set; }
        public string ResultRoomName { get; set; }
        public string ResultRoomAddress { get; set; }

        public long? ResultDeskId { get; set; }
        public string ResultDeskCode { get; set; }
        public string ResultDeskName { get; set; }

        public bool IsResultInDiffDay { get; set; }
         
        public long ServiceReqNumOrder { get; set; }        
        public long? ServiceNumOrder { get; set; }
        public long? ExecuteNumOrder { get; set; }

        public long InstructionTime { get; set; }
        public long InstructionDate { get; set; }

        public long SereServId { get; set; }
        public long ServiceReqId { get; set; }

        public long? CallSampleOrder { get; set; }
        public string SampleRoomCode { get; set; }
        public string SampleRoomName { get; set; }
        public string ASSIGN_TURN_CODE { get; set; }

        public Mps000276ADO() { }
    }
}

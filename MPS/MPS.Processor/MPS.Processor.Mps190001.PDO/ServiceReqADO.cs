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

namespace MPS.Processor.Mps190001.PDO
{
    public class ServiceReqADO : HIS_SERVICE_REQ
    {
        public bool isCheck { get; set; }
        public string EXECUTE_DEPARTMENT_CODE { get; set; }
        public string EXECUTE_DEPARTMENT_NAME { get; set; }
        public string EXECUTE_ROOM_CODE { get; set; }
        public string EXECUTE_ROOM_NAME { get; set; }
        public string EXECUTE_ROOM_ADDRESS { get; set; }
        public string REQUEST_DEPARTMENT_CODE { get; set; }
        public string REQUEST_DEPARTMENT_NAME { get; set; }
        public string REQUEST_ROOM_CODE { get; set; }
        public string REQUEST_ROOM_NAME { get; set; }
        public string SERVICE_REQ_STT_CODE { get; set; }
        public string SERVICE_REQ_STT_NAME { get; set; }
        public string SERVICE_REQ_TYPE_CODE { get; set; }
        public string SERVICE_REQ_TYPE_NAME { get; set; }
        public bool DeleteCheck { get; set; }
        public bool AddInforPTTT { get; set; }

        // Thoi gian vao vien cua ho so dieu tri - key <#lstServiceReq.IN_TIME;> tren mau in
        public long? IN_TIME { get; set; }

        public string SAMPLE_ROOM_CODE { get; set; }
        public string SAMPLE_ROOM_NAME { get; set; }
        public string LIST_SERVICE_NAME { get; set; }

        public ServiceReqADO()
        {

        }
    }
}

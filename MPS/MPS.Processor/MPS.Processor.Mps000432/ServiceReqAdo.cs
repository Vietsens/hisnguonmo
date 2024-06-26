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

namespace MPS.Processor.Mps000432
{
    class ServiceReqAdo : V_HIS_SERVICE_REQ
    {
        public long CURRENT_EXECUTE_ROOM_NUM_ORDER { get; set; }
        public byte[] SERVICE_REQ_CODE_BAR { get; set; }
        public byte[] ASSIGN_TURN_CODE_BARCODE { get; set; }
        public byte[] TEST_SERVICE_REQ_BAR { get; set; }
        public byte[] QRCODE_PATIENT { get; set; }

        public ServiceReqAdo(V_HIS_SERVICE_REQ data)
        {
            if (data != null)
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<ServiceReqAdo>(this, data);
            }
        }
    }
}

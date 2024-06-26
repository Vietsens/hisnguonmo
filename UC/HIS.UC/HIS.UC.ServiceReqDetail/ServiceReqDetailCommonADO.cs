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
using ACS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.ServiceReqDetail
{
    public class ServiceReqDetailCommonADO : MOS.EFMODEL.DataModels.HIS_SERVICE_REQ
    {
        public ServiceReqDetailCommonADO() { }
        public ServiceReqDetailCommonADO(MOS.EFMODEL.DataModels.HIS_SERVICE_REQ data)
        {
            if (data != null)
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<ServiceReqDetailCommonADO>(this, data);
            }
        }

        public string EXP_MEST_CODE { get; set; }
        public string MEDI_STOCK_NAME { get; set; }
        public string EXP_LOGINNAME { get; set; }
        public string EXP_TIME { get; set; }
        public string EXP_MEST_REASON_NAME { get; set; }
        public string EXP_MEST_STT_NAME { get; set; }
        public string APPROVAL_USER_NAME { get; set; }
        public long EXP_MEST_ID { get; set; }
    }
}

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
    public class ServiceReqDetailADO : MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MEDICINE
    {
        public ServiceReqDetailADO() { }
        public ServiceReqDetailADO(MOS.EFMODEL.DataModels.V_HIS_EXP_MEST_MEDICINE data)
        {
            if (data != null)
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<ServiceReqDetailADO>(this, data);
            }
        }

        public bool check2 { get; set; }
        public bool isKeyChoose1 { get; set; }
        public bool radio2 { get; set; }

        public long STT { get; set; }

        public long TYPE { get; set; }// 1 thuốc - 2 vật tư - 3 máu
        public string CODE { get; set; }
        public string NAME { get; set; }
        public decimal AMOUNT { get; set; }
        public string SERVICE_UNIT_NAME { get; set; }
        public string TUTORIAL { get; set; }


    }
}

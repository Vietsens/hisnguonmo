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

namespace HIS.UC.ServiceGroup
{
    public class ServiceGroupADO : MOS.EFMODEL.DataModels.HIS_SERVICE_GROUP
    {
        public ServiceGroupADO() { }
        public ServiceGroupADO(HIS_SERVICE_GROUP data)
        {
            if (data != null)
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<ServiceGroupADO>(this, data);
                IsPublic = (data.IS_PUBLIC == 1);
                LOCK_UNLOCK = data.IS_ACTIVE;
                DELETE_ITEM = data.IS_ACTIVE;
            }
        }
        public bool checkGroup { get; set; }
        public bool isKeyChooseGroup { get; set; }
        public bool radioGroup { get; set; }
        public decimal AMOUNT { get; set; }
        public bool IsPublic { get; set; }
        public short? LOCK_UNLOCK { get; set; }
        public short? DELETE_ITEM { get; set; }
    }
}

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

namespace HIS.UC.RoomTypeList
{
    public class RoomTypeListADO : MOS.EFMODEL.DataModels.HIS_ROOM_TYPE
    {
        public RoomTypeListADO() { }
        public RoomTypeListADO(HIS_ROOM_TYPE data)
        {
            if (data != null)
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<RoomTypeListADO>(this, data);
            }
        }
        public bool check1 { get; set; }
        public bool isKeyChoose { get; set; }
        public bool radio1 { get; set; }
    }
}

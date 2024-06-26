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

namespace HIS.Desktop.Plugins.HisServicePatyList
{
    public class RoomRequiredADO : V_HIS_ROOM
    {
        public bool Check { get; set; }

        public RoomRequiredADO()
        {
        }

        public RoomRequiredADO(V_HIS_ROOM roomRequired)
        {
            if (roomRequired != null)
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<RoomRequiredADO>(this, roomRequired);
            }
        }

        public RoomRequiredADO(V_HIS_ROOM roomRequired, List<long> roomRequiredIds)
        {
            if (roomRequired != null)
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<RoomRequiredADO>(this, roomRequired);
                if (roomRequiredIds.Contains(roomRequired.ID))
                    this.Check = true;
            }
        }
    }
}

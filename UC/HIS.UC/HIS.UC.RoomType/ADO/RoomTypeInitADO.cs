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
using HIS.Desktop.ADO;
using HIS.UC.Room;
using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.UC.RoomType.ADO
{
    public class RoomTypeInitADO
    {
        public List<RoomTypeADO> listRoomTypeADO { get; set; }
        public List<V_HIS_ROOM_TYPE_MODULE> ListRoomType { get; set; }
        public List<RoomTypeColumn> ListRoomTypeColumn { get; set; }

        public bool? IsShowSearchPanel { get; set; }

        public Grid_CustomUnboundColumnData RoomTypeGrid_CustomUnboundColumnData { get; set; }
        public Grid_RowCellClick RoomTypeGrid_RowCellClick { get; set; }
    }
}

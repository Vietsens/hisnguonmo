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
using System.Collections.Generic;

namespace HIS.Desktop.Plugins.DashboardTreatmentBedRoom.ADO
{
    /// <summary>
    /// Một phòng điều trị = một thẻ trong lưới 4 cột.
    /// </summary>
    public class RoomInfoADO
    {
        public RoomInfoADO()
        {
            BEDS = new List<BedInfoADO>();
        }

        /// <summary>HIS_BED_ROOM.ID. Để 0 nếu nguồn dữ liệu không có ID, khi đó khóa theo ROOM_NAME.</summary>
        public long ROOM_ID { get; set; }

        /// <summary>Tên phòng hiển thị trên đầu thẻ</summary>
        public string ROOM_NAME { get; set; }

        public List<BedInfoADO> BEDS { get; set; }

        /// <summary>Khóa đối chiếu khi làm mới. Ưu tiên ID thật, không có thì rơi về tên phòng.</summary>
        public string RoomKey
        {
            get { return ROOM_ID != 0 ? "#" + ROOM_ID : (ROOM_NAME ?? string.Empty); }
        }
    }
}

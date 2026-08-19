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
using System.Drawing;

namespace HIS.Desktop.Plugins.DashboardTreatmentBedRoom.ADO
{
    /// <summary>
    /// Một ô trong cụm "Chế độ chăm sóc".
    ///
    /// Số ô bằng đúng số bản ghi HIS_CARE_LEVEL đang hoạt động, không cố định.
    /// Tên và màu lấy từ danh mục, số lượng lấy từ TreatmentBedRoomCareLevelSDO;
    /// cấp nào không có bệnh nhân thì vẫn hiện ô với số 0.
    /// </summary>
    public class CareLevelSummaryADO
    {
        /// <summary>HIS_CARE_LEVEL.ID</summary>
        public long CARE_LEVEL_ID { get; set; }

        public string CARE_LEVEL_CODE { get; set; }

        /// <summary>HIS_CARE_LEVEL.CARE_LEVEL_NAME — nhãn dưới con số</summary>
        public string CARE_LEVEL_NAME { get; set; }

        /// <summary>Màu con số, từ DISPLAY_COLOR của danh mục. Null thì dùng màu mặc định.</summary>
        public Color? DISPLAY_COLOR { get; set; }

        /// <summary>Số bệnh nhân đang ở cấp này</summary>
        public long TOTAL { get; set; }
    }
}

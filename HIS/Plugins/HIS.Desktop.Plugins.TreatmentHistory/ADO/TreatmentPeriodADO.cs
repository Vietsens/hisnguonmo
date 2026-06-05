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
namespace HIS.Desktop.Plugins.TreatmentHistory.ADO
{
    /// <summary>
    /// Dữ liệu gắn vào node của cây popup "Đợt điều trị cần gộp" (2 cấp: Năm -> Đợt).
    /// IsYear = true => node năm (cha); IsYear = false => node đợt điều trị (con).
    /// </summary>
    public class TreatmentPeriodADO
    {
        /// <summary>true: node năm (cha); false: node đợt điều trị (lá).</summary>
        public bool IsYear { get; set; }

        /// <summary>Năm (lấy từ IN_TIME). Dùng cho cả node năm lẫn node đợt.</summary>
        public int Year { get; set; }

        /// <summary>Mã điều trị (chỉ có ở node đợt). Năm = 0.</summary>
        public long TreatmentId { get; set; }

        /// <summary>Chuỗi hiển thị: node năm = "{Năm}"; node đợt = "Mã đợt · TG vào · Khoa cuối · Trạng thái".</summary>
        public string Display { get; set; }
    }
}

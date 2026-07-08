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
using Newtonsoft.Json;

namespace HIS.Desktop.Plugins.Library.EmrToolkitImport.Models
{
    /// <summary>
    /// Trường Data của phản hồi API MaHoaJson (POST /api/EMR/MaHoaJson).
    /// Là đầu vào cho API Import.
    /// </summary>
    public class MaHoaJsonResultADO
    {
        /// <summary>Chuỗi dữ liệu đã mã hóa (GUID tham chiếu)</summary>
        [JsonProperty("DuLieu")]
        public string DuLieu { get; set; }

        /// <summary>Key giải mã tương ứng với DuLieu</summary>
        [JsonProperty("KeyGiaiMa")]
        public string KeyGiaiMa { get; set; }
    }
}

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
    /// Cấu trúc phản hồi chung của EMRTOOLKIT (Output&lt;T&gt;).
    /// Áp dụng cho cả 3 API: CreateToken, MaHoaJson, Import.
    /// </summary>
    /// <typeparam name="T">Kiểu dữ liệu của trường Data</typeparam>
    public class EmrOutput<T>
    {
        /// <summary>true = thành công, false = thất bại</summary>
        [JsonProperty("Success")]
        public bool Success { get; set; }

        /// <summary>Thông báo lỗi (null nếu thành công)</summary>
        [JsonProperty("Message")]
        public string Message { get; set; }

        /// <summary>Dữ liệu trả về (đã giải mã với API Import)</summary>
        [JsonProperty("Data")]
        public T Data { get; set; }
    }
}

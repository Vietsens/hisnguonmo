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
    /// Trường Data của phản hồi API CreateToken.
    /// Chỉ khai báo các trường cần dùng; các trường khác của EMRTOOLKIT bỏ qua.
    /// </summary>
    public class TokenResultADO
    {
        /// <summary>Token dùng cho header tokencode ở các API tiếp theo</summary>
        [JsonProperty("Token")]
        public string Token { get; set; }

        /// <summary>Tên đăng nhập</summary>
        [JsonProperty("UserName")]
        public string UserName { get; set; }

        /// <summary>Ngày tạo token</summary>
        [JsonProperty("DayCreate")]
        public string DayCreate { get; set; }

        /// <summary>Ngày hết hạn token</summary>
        [JsonProperty("DayExpired")]
        public string DayExpired { get; set; }

        /// <summary>Mã cơ sở khám chữa bệnh gắn với token</summary>
        [JsonProperty("MaCSKCB")]
        public string MaCSKCB { get; set; }
    }
}

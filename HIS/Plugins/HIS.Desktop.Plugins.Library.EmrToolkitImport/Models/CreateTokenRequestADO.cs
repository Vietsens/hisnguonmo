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
    /// Request body cho API POST /api/Token/CreateToken.
    /// </summary>
    public class CreateTokenRequestADO
    {
        /// <summary>Tên đăng nhập EMRTOOLKIT</summary>
        [JsonProperty("TenDangNhap")]
        public string TenDangNhap { get; set; }

        /// <summary>Mật khẩu EMRTOOLKIT</summary>
        [JsonProperty("MatKhau")]
        public string MatKhau { get; set; }
    }
}

/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 */
using Newtonsoft.Json;

namespace HIS.Desktop.Plugins.KskSyncListQD831.Sync.Model
{
    /// <summary>
    /// Đầu ra API /get-token (thành công 200) theo tài liệu HSSK 831:
    /// { "success": bool, "message": string, "type": string, "time": number, "token": string }.
    /// </summary>
    internal class Ksk831TokenResponse
    {
        /// <summary>Biến trạng thái.</summary>
        [JsonProperty("success")]
        internal bool Success { get; set; }

        /// <summary>Thông điệp trả về.</summary>
        [JsonProperty("message")]
        internal string Message { get; set; }

        /// <summary>Loại token – Bearer Token.</summary>
        [JsonProperty("type")]
        internal string Type { get; set; }

        /// <summary>Thời gian hiệu lực của token (đơn vị: phút).</summary>
        [JsonProperty("time")]
        internal int Time { get; set; }

        /// <summary>Chuỗi token.</summary>
        [JsonProperty("token")]
        internal string Token { get; set; }
    }
}

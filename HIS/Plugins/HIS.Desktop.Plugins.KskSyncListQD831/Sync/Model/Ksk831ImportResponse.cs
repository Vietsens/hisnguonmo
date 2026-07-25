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
    /// Đầu ra API /CSDLYTE831/import-csdlyte831-mobile (thành công 200) theo tài liệu HSSK 831:
    /// { "ContentType": null, "SerializerSettings": null, "StatusCode": null, "Value": "Cập nhật hồ sơ sức khỏe thành công!" }.
    /// </summary>
    internal class Ksk831ImportResponse
    {
        [JsonProperty("ContentType")]
        internal string ContentType { get; set; }

        [JsonProperty("SerializerSettings")]
        internal object SerializerSettings { get; set; }

        [JsonProperty("StatusCode")]
        internal object StatusCode { get; set; }

        /// <summary>Thông điệp kết quả cập nhật hồ sơ.</summary>
        [JsonProperty("Value")]
        internal string Value { get; set; }
    }
}

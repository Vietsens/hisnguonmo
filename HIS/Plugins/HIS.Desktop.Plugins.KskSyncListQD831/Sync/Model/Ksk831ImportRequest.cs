/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 */
namespace HIS.Desktop.Plugins.KskSyncListQD831.Sync.Model
{
    /// <summary>
    /// Đầu vào API đồng bộ /CSDLYTE831/import-csdlyte831-mobile (multipart/form-data) theo tài liệu HSSK 831:
    /// { "xmlFile": File XML (QĐ 831/QĐ-BYT), "nguoi_gui": Thông tin người đồng bộ dữ liệu (ghi log) }.
    /// Header: Authorization: bearer {token}.
    /// </summary>
    internal class Ksk831ImportRequest
    {
        /// <summary>xmlFile — nội dung file XML (bytes UTF-8).</summary>
        internal byte[] XmlFile { get; set; }

        /// <summary>Tên file đính kèm.</summary>
        internal string FileName { get; set; }

        /// <summary>nguoi_gui — Chuỗi, tối đa 100 (ghi log).</summary>
        internal string NguoiGui { get; set; }
    }
}

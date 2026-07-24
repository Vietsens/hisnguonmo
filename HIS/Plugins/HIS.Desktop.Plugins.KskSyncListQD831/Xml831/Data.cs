/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 */
using System.Xml.Serialization;

namespace HIS.Desktop.Plugins.KskSyncListQD831.Xml831
{
    /// <summary>Gốc XML hồ sơ sức khỏe QĐ831 (&lt;DATA&gt;). Bước 1: chỉ mô hình cấu trúc, chưa map dữ liệu.</summary>
    [XmlRoot("DATA")]
    public class Data
    {
        [XmlElement("HEADER")]
        public Header Header { get; set; }

        [XmlElement("BODY")]
        public Body Body { get; set; }

        [XmlElement("SECURITY")]
        public Security Security { get; set; }
    }
}

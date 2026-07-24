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
    /// <summary>&lt;DSYEUTO&gt; — 1 yếu tố nguy cơ (LOAI 1=hút thuốc,2=rượu bia,3=ma túy,4=thể lực).</summary>
    public class DsYeuTo
    {
        [XmlElement("LOAI")]
        public string Loai { get; set; }

        [XmlElement("TENLOAI")]
        public string TenLoai { get; set; }

        [XmlElement("TRANGTHAI")]
        public string TrangThai { get; set; }

        [XmlElement("SOLUONG")]
        public string SoLuong { get; set; }

        [XmlElement("DABO")]
        public string DaBo { get; set; }
    }
}

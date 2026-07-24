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
    /// <summary>&lt;QUANHE_GIADINH&gt; — một thành viên hộ gia đình.</summary>
    public class QuanHeGiaDinh
    {
        [XmlElement("LOAI_QUANHE")]
        public string LoaiQuanHe { get; set; }

        [XmlElement("TEN_QUANHE")]
        public string TenQuanHe { get; set; }

        [XmlElement("MADINHDANH")]
        public string MaDinhDanh { get; set; }

        [XmlElement("HOTEN")]
        public string HoTen { get; set; }

        [XmlElement("DIENTHOAI")]
        public string DienThoai { get; set; }

        [XmlElement("DIDONG")]
        public string DiDong { get; set; }

        [XmlElement("GIAMHO")]
        public string GiamHo { get; set; }
    }
}

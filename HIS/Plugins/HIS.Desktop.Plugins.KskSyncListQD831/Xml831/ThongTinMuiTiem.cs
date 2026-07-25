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
    /// <summary>&lt;THONGTINMUITIEM&gt; — 1 mũi tiêm chủng.</summary>
    public class ThongTinMuiTiem
    {
        [XmlElement("MAVACXIN")]
        public string MaVacXin { get; set; }

        [XmlElement("TENVACXIN")]
        public string TenVacXin { get; set; }

        [XmlElement("LOAIVACXIN")]
        public string LoaiVacXin { get; set; }

        [XmlElement("TRANGTHAI")]
        public string TrangThai { get; set; }

        [XmlElement("NGAYTIEM")]
        public string NgayTiem { get; set; }

        [XmlElement("THANGTHAI")]
        public string ThangThai { get; set; }

        [XmlElement("PHANUNGSAUTIEM")]
        public string PhanUngSauTiem { get; set; }

        [XmlElement("NGAYHENTIEM")]
        public string NgayHenTiem { get; set; }

        [XmlElement("SOMUIUONVANMETIEM")]
        public string SoMuiUongVanMeTiem { get; set; }
    }
}

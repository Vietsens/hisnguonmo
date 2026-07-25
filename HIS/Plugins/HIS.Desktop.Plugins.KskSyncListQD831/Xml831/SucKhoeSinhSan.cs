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
    /// <summary>&lt;SUCKHOE_SINHSAN&gt; — sức khỏe sinh sản.</summary>
    public class SucKhoeSinhSan
    {
        [XmlElement("BIENPHAP_TRANHTHAI")]
        public string BienPhapTranhThai { get; set; }

        [XmlElement("KYTHAICUOI")]
        public string KyThaiCuoi { get; set; }

        [XmlElement("SOLAN_COTHAI")]
        public string SoLanCoThai { get; set; }

        [XmlElement("SOLAN_SAYTHAI")]
        public string SoLanSayThai { get; set; }

        [XmlElement("SOLAN_PHATHAI")]
        public string SoLanPhaThai { get; set; }

        [XmlElement("SOLAN_SINHDE")]
        public string SoLanSinhDe { get; set; }

        [XmlElement("SOLAN_DETHUONG")]
        public string SoLanDeThuong { get; set; }

        [XmlElement("SOLAN_DEMO")]
        public string SoLanDeMo { get; set; }

        [XmlElement("SOLANDEKHO")]
        public string SoLanDeKho { get; set; }

        [XmlElement("SOLANDE_DUTHANG")]
        public string SoLanDeDuThang { get; set; }

        [XmlElement("SOLAN_DENON")]
        public string SoLanDeNon { get; set; }

        [XmlElement("SOCON_HIENSONG")]
        public string SoConHienSong { get; set; }

        [XmlElement("BENH_PHUKHOA")]
        public string BenhPhuKhoa { get; set; }
    }
}

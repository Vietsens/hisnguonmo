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
    /// <summary>&lt;TINHTRANG_LUCSINH&gt; — tình trạng lúc sinh.</summary>
    public class TinhTrangLucSinh
    {
        [XmlElement("LOAIDE")]
        public string LoaiDe { get; set; }

        [XmlElement("DETHIEUTHANG")]
        public string DeThieuThang { get; set; }

        [XmlElement("BINGAT_LUCDE")]
        public string BiNgatLucDe { get; set; }

        [XmlElement("CANNANG")]
        public string CanNang { get; set; }

        [XmlElement("CHIEUDAI")]
        public string ChieuDai { get; set; }

        [XmlElement("DITAT_BAMSINH")]
        public string DiTatBamSinh { get; set; }

        [XmlElement("KHAC")]
        public string Khac { get; set; }
    }
}

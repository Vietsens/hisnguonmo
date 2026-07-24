/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 */
using System.Collections.Generic;
using System.Xml.Serialization;

namespace HIS.Desktop.Plugins.KskSyncListQD831.Xml831
{
    /// <summary>&lt;YEUTO_SUCKHOE&gt; — danh sách yếu tố nguy cơ + phơi nhiễm/hố xí/khác.</summary>
    public class YeuToSucKhoe
    {
        // DSYEUTO lap truc tiep trong YEUTO_SUCKHOE (khong boc trong element bao).
        [XmlElement("DSYEUTO")]
        public List<DsYeuTo> DsYeuTo { get; set; }

        [XmlElement("YEUTO_TIEPXUC")]
        public string YeuToTiepXuc { get; set; }

        [XmlElement("THOIGIAN_TIEPXUC")]
        public string ThoiGianTiepXuc { get; set; }

        [XmlElement("LOAIHOXIGD")]
        public string LoaiHoXiGd { get; set; }

        [XmlElement("KHAC")]
        public string Khac { get; set; }
    }
}

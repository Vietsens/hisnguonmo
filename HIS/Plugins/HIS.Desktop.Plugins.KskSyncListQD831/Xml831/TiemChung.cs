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
    /// <summary>&lt;TIEMCHUNG&gt; — danh sách mũi tiêm + tổng số mũi uống/tiêm vắc-xin.</summary>
    public class TiemChung
    {
        [XmlElement("THONGTINMUITIEM")]
        public List<ThongTinMuiTiem> ThongTinMuiTiem { get; set; }

        [XmlElement("SOMUIUONVANMETIEM")]
        public string SoMuiUongVanMeTiem { get; set; }
    }
}

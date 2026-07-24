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
    /// <summary>&lt;HOSOSUCKHOE&gt; — 4 khối: thông tin chung, tiền sử, tiêm chủng, danh sách hồ sơ KCB.</summary>
    public class HoSoSucKhoe
    {
        [XmlElement("THONGTINCHUNG")]
        public ThongTinChung ThongTinChung { get; set; }

        [XmlElement("TIENSU")]
        public TienSu TienSu { get; set; }

        [XmlElement("TIEMCHUNG")]
        public TiemChung TiemChung { get; set; }

        [XmlArray("DANHSACHHOSOKHAMCHUABENH")]
        [XmlArrayItem("HOSOKHAMCHUABENH")]
        public List<HoSoKhamChuaBenh> DanhSachHoSoKhamChuaBenh { get; set; }
    }
}

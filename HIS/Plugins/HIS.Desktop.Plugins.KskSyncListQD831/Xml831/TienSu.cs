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
    /// <summary>&lt;TIENSU&gt; — tiền sử: lúc sinh, yếu tố sức khỏe, khuyết tật, bệnh tật/dị ứng, gia đình, sinh sản.</summary>
    public class TienSu
    {
        [XmlElement("TINHTRANG_LUCSINH")]
        public TinhTrangLucSinh TinhTrangLucSinh { get; set; }

        [XmlElement("YEUTO_SUCKHOE")]
        public YeuToSucKhoe YeuToSucKhoe { get; set; }

        [XmlArray("KHUYETTAT")]
        [XmlArrayItem("DSKHUYETTAT")]
        public List<DsKhuyetTat> KhuyetTat { get; set; }

        // TIEUSU_BENHTAT va TIENSU_GIADINH cung cau truc (DIUNG + BENHTAT) -> dung chung class.
        [XmlElement("TIEUSU_BENHTAT")]
        public TieuSuBenhTat TieuSuBenhTat { get; set; }

        [XmlElement("TIENSU_PHAUTHUAT")]
        public string TienSuPhauThuat { get; set; }

        [XmlElement("TIENSU_GIADINH")]
        public TieuSuBenhTat TienSuGiaDinh { get; set; }

        [XmlElement("SUCKHOE_SINHSAN")]
        public SucKhoeSinhSan SucKhoeSinhSan { get; set; }

        [XmlElement("VANDEKHAC")]
        public string VanDeKhac { get; set; }
    }
}

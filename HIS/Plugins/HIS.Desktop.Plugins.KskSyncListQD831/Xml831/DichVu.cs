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
    /// <summary>&lt;DICHVU&gt; — 1 dịch vụ cận lâm sàng + kết quả (MANHOM: 1=Xét nghiệm, 2=Chẩn đoán hình ảnh).</summary>
    public class DichVu
    {
        [XmlElement("MADICHVU")]
        public string MaDichVu { get; set; }

        [XmlElement("MANHOM")]
        public string MaNhom { get; set; }

        [XmlElement("TENDICHVU")]
        public string TenDichVu { get; set; }

        [XmlElement("KETQUA")]
        public string KetQua { get; set; }
    }
}

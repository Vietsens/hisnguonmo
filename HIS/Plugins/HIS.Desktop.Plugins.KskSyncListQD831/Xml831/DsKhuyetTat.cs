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
    /// <summary>&lt;DSKHUYETTAT&gt; — 1 khuyết tật (LOAI 1=thính lực,2=thị lực,3=tay,4=chân,5=cột sống,6=môi/hàm ếch,7=khác).</summary>
    public class DsKhuyetTat
    {
        [XmlElement("LOAI")]
        public string Loai { get; set; }

        [XmlElement("TENLOAI")]
        public string TenLoai { get; set; }

        [XmlElement("MOTA")]
        public string MoTa { get; set; }
    }
}

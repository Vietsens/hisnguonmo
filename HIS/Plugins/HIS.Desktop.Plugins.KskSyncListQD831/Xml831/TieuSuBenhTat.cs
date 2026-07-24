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
    /// <summary>
    /// Khối tiền sử dị ứng + bệnh tật (dùng cho &lt;TIEUSU_BENHTAT&gt; bản thân và &lt;TIENSU_GIADINH&gt;).
    /// Với tiền sử gia đình, mỗi mục có thêm NGUOIMAC (xem DsDiUng / DsBenh).
    /// </summary>
    public class TieuSuBenhTat
    {
        [XmlArray("DIUNG")]
        [XmlArrayItem("DSDIUNG")]
        public List<DsDiUng> DiUng { get; set; }

        [XmlArray("BENHTAT")]
        [XmlArrayItem("DSBENH")]
        public List<DsBenh> BenhTat { get; set; }
    }
}

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
    /// <summary>&lt;CHANDOANBENH&gt; — 1 chẩn đoán/kết luận (MABENH = mã ICD-10).</summary>
    public class ChanDoanBenh
    {
        [XmlElement("MABENH")]
        public string MaBenh { get; set; }

        [XmlElement("TENBENH")]
        public string TenBenh { get; set; }

        [XmlElement("KETLUAN")]
        public string KetLuan { get; set; }
    }
}

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
    /// <summary>&lt;DSDIUNG&gt; — 1 dị ứng (LOAI 1=thuốc,2=hóa chất,3=thực phẩm,4=khác). NGUOIMAC chỉ dùng cho tiền sử gia đình (null -&gt; bỏ thẻ).</summary>
    public class DsDiUng
    {
        [XmlElement("LOAI")]
        public string Loai { get; set; }

        [XmlElement("MASO")]
        public string MaSo { get; set; }

        [XmlElement("TEN")]
        public string Ten { get; set; }

        [XmlElement("MOTA")]
        public string MoTa { get; set; }

        [XmlElement("NGUOIMAC")]
        public string NguoiMac { get; set; }
    }
}

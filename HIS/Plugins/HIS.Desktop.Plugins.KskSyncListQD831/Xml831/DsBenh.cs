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
    /// <summary>
    /// &lt;DSBENH&gt; — 1 bệnh tật (LOAIBENH 1=tim mạch,2=THA,3=ĐTĐ,4=dạ dày,5=phổi mạn,6=hen,7=bướu cổ,
    /// 8=viêm gan,9=tim bẩm sinh,10=tâm thần,11=tự kỷ,12=động kinh,13=ung thư,14=lao,15=khác).
    /// NGUOIMAC chỉ dùng cho tiền sử gia đình (null -&gt; bỏ thẻ).
    /// </summary>
    public class DsBenh
    {
        [XmlElement("LOAIBENH")]
        public string LoaiBenh { get; set; }

        [XmlElement("TENBENH")]
        public string TenBenh { get; set; }

        [XmlElement("TRANGTHAI")]
        public string TrangThai { get; set; }

        [XmlElement("MOTA")]
        public string MoTa { get; set; }

        [XmlElement("NGUOIMAC")]
        public string NguoiMac { get; set; }
    }
}

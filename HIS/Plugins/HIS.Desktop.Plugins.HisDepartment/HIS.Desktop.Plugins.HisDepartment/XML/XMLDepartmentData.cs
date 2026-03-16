/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 */
using System.Xml.Serialization;

namespace HIS.Desktop.Plugins.HisDepartment.XML
{
    [XmlRoot("HSDANHMUC")]
    public class XMLDepartmentData
    {
        [XmlElement("DANHSACH_DMBOPHANCHUYENMON", Order = 1)]
        public XMLDepartmentList DanhSach { get; set; }

        [XmlElement("CHUKYDONVI", Order = 2)]
        public string ChuKyDonVi { get; set; }
    }
}

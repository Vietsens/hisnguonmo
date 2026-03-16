/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 */
using System;
using System.Xml;
using System.Xml.Serialization;

namespace HIS.Desktop.Plugins.HisDepartment.XML
{
    [Serializable]
    public class XMLDepartmentDetailData
    {
        [XmlElement(Order = 1)]
        public int STT { get; set; }

        [XmlElement(Order = 2)]
        public string MA_KHOA { get; set; }

        [XmlElement(Order = 3)]
        public XmlCDataSection TEN_KHOA { get; set; }

        [XmlElement(Order = 4)]
        public string BAN_KHAM { get; set; }

        [XmlElement(Order = 5)]
        public string GIUONG_PD { get; set; }

        [XmlElement(Order = 6)]
        public string GIUONG_TK { get; set; }

        [XmlElement(Order = 7)]
        public string GIUONG_HSTC { get; set; }

        [XmlElement(Order = 8)]
        public string GIUONG_HSCC { get; set; }

        [XmlElement(Order = 9)]
        public string TU_NGAY { get; set; }

        [XmlElement(Order = 10)]
        public string DEN_NGAY { get; set; }

        [XmlElement(Order = 11)]
        public string MA_CSKCB { get; set; }
    }
}

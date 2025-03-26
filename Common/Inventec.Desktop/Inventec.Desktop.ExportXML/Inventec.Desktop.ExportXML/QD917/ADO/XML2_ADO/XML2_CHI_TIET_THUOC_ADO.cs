/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *  
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *  
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *  
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Inventec.Desktop.ExportXML.QD917.ADO.XML2_ADO
{
    [Serializable]
    public class XML2_CHI_TIET_THUOC_ADO
    {
        [XmlElement(Order = 1)]
        public XmlCDataSection MA_LK { get; set; }

        [XmlElement(Order = 2)]
        public int STT { get; set; }

        [XmlElement(Order = 3)]
        public XmlCDataSection MA_THUOC { get; set; }

        [XmlElement(Order = 4)]
        public XmlCDataSection MA_NHOM { get; set; }

        [XmlElement(Order = 5)]
        public XmlCDataSection TEN_THUOC { get; set; }

        [XmlElement(Order = 6)]
        public XmlCDataSection DON_VI_TINH { get; set; }

        [XmlElement(Order = 7)]
        public XmlCDataSection HAM_LUONG { get; set; }

        [XmlElement(Order = 8)]
        public XmlCDataSection DUONG_DUNG { get; set; }

        [XmlElement(Order = 9)]
        public XmlCDataSection LIEU_DUNG { get; set; }

        [XmlElement(Order = 10)]
        public XmlCDataSection SO_DANG_KY { get; set; }

        [XmlElement(Order = 11)]
        public decimal SO_LUONG { get; set; }

        [XmlElement(Order = 12)]
        public decimal? DON_GIA { get; set; }

        [XmlElement(Order = 13)]
        public decimal? TYLE_TT { get; set; }

        [XmlElement(Order = 14)]
        public decimal? THANH_TIEN { get; set; }

        [XmlElement(Order = 15)]
        public XmlCDataSection MA_KHOA { get; set; }

        [XmlElement(Order = 16)]
        public XmlCDataSection MA_BAC_SI { get; set; }

        [XmlElement(Order = 17)]
        public XmlCDataSection MA_BENH { get; set; }

        [XmlElement(Order = 18)]
        public XmlCDataSection NGAY_YL { get; set; }

        [XmlElement(Order = 19)]
        public XmlCDataSection MA_PTTT { get; set; }
    }
}

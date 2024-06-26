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

namespace HIS.Desktop.Plugins.ImportRegisterByXml.XML.XML3
{
    //[Serializable, XmlRoot("GIAMDINHHS", Namespace = "xmlConvertToObj", ElementName = "MyGroupName", DataType = "string", IsNullable = true)]
    public class XML3DetailData
    {
        [XmlElement("MA_LK")]
        public string MA_LK { get; set; }

        [XmlElement("STT")]
        public string STT { get; set; }

        [XmlElement("MA_DICH_VU")]
        public string MA_DICH_VU { get; set; }

        [XmlElement("MA_VAT_TU")]
        public string MA_VAT_TU { get; set; }

        [XmlElement("MA_NHOM")]
        public string MA_NHOM { get; set; }

        [XmlElement("GOI_VTYT")]
        public string GOI_VTYT { get; set; }

        [XmlElement("TEN_VAT_TU")]
        public string TEN_VAT_TU { get; set; }

        [XmlElement("TEN_DICH_VU")]
        public string TEN_DICH_VU { get; set; }

        [XmlElement("DON_VI_TINH")]
        public string DON_VI_TINH { get; set; }

        [XmlElement("PHAM_VI")]
        public string PHAM_VI { get; set; }

        [XmlElement("SO_LUONG")]
        public decimal SO_LUONG { get; set; }

        [XmlElement("DON_GIA")]
        public decimal DON_GIA { get; set; }

        [XmlElement("TT_THAU")]
        public string TT_THAU { get; set; }

        [XmlElement("TYLE_TT")]
        public string TYLE_TT { get; set; }

        [XmlElement("THANH_TIEN")]
        public decimal THANH_TIEN { get; set; }

        [XmlElement("T_TRANTT")]
        public decimal T_TRANTT { get; set; }

        [XmlElement("MUC_HUONG")]
        public decimal MUC_HUONG { get; set; }

        [XmlElement("T_NGUONKHAC")]
        public decimal T_NGUONKHAC { get; set; }

        [XmlElement("T_BNTT")]
        public decimal T_BNTT { get; set; }

        [XmlElement("T_BHTT")]
        public decimal T_BHTT { get; set; }

        [XmlElement("T_BNCCT")]
        public decimal T_BNCCT { get; set; }

        [XmlElement("T_NGOAIDS")]
        public decimal T_NGOAIDS { get; set; }

        [XmlElement("MA_KHOA")]
        public string MA_KHOA { get; set; }

        [XmlElement("MA_GIUONG")]
        public string MA_GIUONG { get; set; }

        [XmlElement("MA_BAC_SI")]
        public string MA_BAC_SI { get; set; }

        [XmlElement("MA_BENH")]
        public string MA_BENH { get; set; }

        [XmlElement("NGAY_YL")]
        public string NGAY_YL { get; set; }

        [XmlElement("NGAY_KQ")]
        public string NGAY_KQ { get; set; }

        [XmlElement("MA_PTTT")]
        public string MA_PTTT { get; set; }
    }
}

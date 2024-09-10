using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Inventec.Desktop.ExportXML.QD917.ADO.XML3_ADO
{
    [Serializable]
    public class XML3_CHI_TIET_DVKT_ADO
    {
        [XmlElement(Order = 1)]
        public XmlCDataSection MA_LK { get; set; }

        [XmlElement(Order = 2)]
        public int STT { get; set; }

        [XmlElement(Order = 3)]
        public XmlCDataSection MA_DICH_VU { get; set; }

        [XmlElement(Order = 4)]
        public XmlCDataSection MA_VAT_TU { get; set; }

        [XmlElement(Order = 5)]
        public XmlCDataSection MA_NHOM { get; set; }

        [XmlElement(Order = 6)]
        public XmlCDataSection TEN_DICH_VU { get; set; }

        [XmlElement(Order = 7)]
        public XmlCDataSection DON_VI_TINH { get; set; }

        [XmlElement(Order = 8)]
        public decimal SO_LUONG { get; set; }

        [XmlElement(Order = 9)]
        public decimal? DON_GIA { get; set; }

        [XmlElement(Order = 10)]
        public decimal TYLE_TT { get; set; }

        [XmlElement(Order = 11)]
        public decimal? THANH_TIEN { get; set; }

        [XmlElement(Order = 12)]
        public XmlCDataSection MA_KHOA { get; set; }

        [XmlElement(Order = 13)]
        public XmlCDataSection MA_BAC_SI { get; set; }

        [XmlElement(Order = 14)]
        public XmlCDataSection MA_BENH { get; set; }

        [XmlElement(Order = 15)]
        public XmlCDataSection NGAY_YL { get; set; }

        [XmlElement(Order = 16)]
        public XmlCDataSection NGAY_KQ { get; set; }

        [XmlElement(Order = 17)]
        public XmlCDataSection MA_PTTT { get; set; }
    }
}

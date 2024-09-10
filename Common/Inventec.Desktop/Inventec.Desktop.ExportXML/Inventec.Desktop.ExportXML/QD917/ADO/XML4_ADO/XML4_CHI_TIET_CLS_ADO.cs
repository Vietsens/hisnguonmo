using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inventec.Desktop.ExportXML.QD917.ADO.XML4_ADO
{
    public class XML4_CHI_TIET_CLS_ADO
    {
        [XmlElement(Order = 1)]
        public string MA_LK { get; set; }

        [XmlElement(Order = 2)]
        public string STT { get; set; }

        [XmlElement(Order = 3)]
        public string MA_DICH_VU { get; set; }

        [XmlElement(Order = 4)]
        public string MA_CHI_SO { get; set; }

        [XmlElement(Order = 5)]
        public string TEN_CHI_SO { get; set; }

        [XmlElement(Order = 6)]
        public string GIA_TRI { get; set; }

        [XmlElement(Order = 7)]
        public string MA_MAY { get; set; }

        [XmlElement(Order = 8)]
        public string MO_TA { get; set; }

        [XmlElement(Order = 9)]
        public string KET_LUAN { get; set; }

        [XmlElement(Order = 10)]
        public string NGAY_KQ { get; set; }
    }
}

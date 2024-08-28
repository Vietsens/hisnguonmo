using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inventec.Desktop.ExportXML.QD917.ADO.XML5_ADO
{
    public class XML5_CHI_TIET_DIEN_BIEN_BENH_ADO
    {
        [XmlElement(Order = 1)]
        public string MA_LK { get; set; }

        [XmlElement(Order = 2)]
        public string STT { get; set; }

        [XmlElement(Order = 3)]
        public string DIEN_BIEN { get; set; }

        [XmlElement(Order = 4)]
        public string HOI_CHAN { get; set; }

        [XmlElement(Order = 5)]
        public string PHAU_THUAT { get; set; }

        [XmlElement(Order = 6)]
        public string NGAY_YL { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Inventec.Desktop.ExportXML.QD917.ADO.HoSoBenhNhan_ADO
{
    [XmlRoot("THONGTINHOSO")]
    public class HS_THONGTINHOSO_ADO
    {
        [XmlElement("NGAYLAP")]
        public XmlCDataSection NGAYLAP { get; set; }

        [XmlElement("SOLUONGHOSO")]
        public long SOLUONGHOSO { get; set; }

        [XmlElement("DANHSACHHOSO")]
        public HS_DANHSACHHOSO_ADO DANHSACHHOSO { get; set; }
    }
}

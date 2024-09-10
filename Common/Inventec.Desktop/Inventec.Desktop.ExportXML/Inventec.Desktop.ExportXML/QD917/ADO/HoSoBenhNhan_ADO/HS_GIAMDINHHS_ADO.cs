using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Inventec.Desktop.ExportXML.QD917.ADO.HoSoBenhNhan_ADO
{
    [XmlRoot("GIAMDINHHS")]
    public class HS_GIAMDINHHS_ADO
    {
        [XmlElement("THONGTINDONVI")]
        public HS_THONGTINDONVI_ADO THONGTINDONVI { get; set; }

        [XmlElement("THONGTINHOSO")]
        public HS_THONGTINHOSO_ADO THONGTINHOSO { get; set; }

        [XmlElement("CHUKYDONVI")]
        public XmlCDataSection CHUKYDONVI { get; set; }
    }
}

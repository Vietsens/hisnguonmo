using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Inventec.Desktop.ExportXML.QD917.ADO.HoSoBenhNhan_ADO
{
    [XmlRoot("THONGTINDONVI")]
    public class HS_THONGTINDONVI_ADO
    {
        [XmlElement("MACSKCB")]
        public XmlCDataSection MACSKCB { get; set; }
    }
}

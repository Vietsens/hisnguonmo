using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inventec.Desktop.ExportXML.QD917.ADO.HoSoBenhNhan_ADO
{
    [XmlRoot("DANHSACHHOSO")]
    public class HS_DANHSACHHOSO_ADO
    {
        [XmlElement("HOSO")]
        public HS_HOSO_ADO HOSO { get; set; }
    }
}

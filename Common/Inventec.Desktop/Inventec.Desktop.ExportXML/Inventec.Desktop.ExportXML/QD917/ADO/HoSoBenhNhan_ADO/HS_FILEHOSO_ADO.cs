using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inventec.Desktop.ExportXML.QD917.ADO.HoSoBenhNhan_ADO
{
    [XmlRoot("FILEHOSO")]
    public class HS_FILEHOSO_ADO
    {
        [XmlElement("LOAIHOSO")]
        public string LOAIHOSO { get; set; }

        [XmlElement("NOIDUNGFILE")]
        public string NOIDUNGFILE { get; set; }
    }
}

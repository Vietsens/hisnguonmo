using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inventec.Desktop.ExportXML.QD917.ADO.HoSoBenhNhan_ADO
{
    [XmlRoot("HOSO")]
    public class HS_HOSO_ADO
    {
        [XmlElement("FILEHOSO")]
        public List<HS_FILEHOSO_ADO> FILEHOSO { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inventec.Desktop.ExportXML.QD917.ADO.XML2_ADO
{
    [XmlRoot("DSACH_CHI_TIET_THUOC")]
    public class XML2_DSACH_CHI_TIET_THUOC_ADO
    {
        [XmlElement("CHI_TIET_THUOC")]
        public List<XML2_CHI_TIET_THUOC_ADO> CHI_TIET_THUOC { get; set; }
    }
}

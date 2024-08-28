using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inventec.Desktop.ExportXML.QD917.ADO.XML3_ADO
{
    [XmlRoot("DSACH_CHI_TIET_DVKT")]
    public class XML3_DSACH_CHI_TIET_DVKT_ADO
    {
        [XmlElement("CHI_TIET_DVKT")]
        public List<XML3_CHI_TIET_DVKT_ADO> CHI_TIET_DVKT { get; set; }
    }
}

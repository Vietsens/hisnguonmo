using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inventec.Desktop.ExportXML.QD917.ADO.XML4_ADO
{
    [XmlRoot("DSACH_CHI_TIET_CLS")]
    public class XML4_DSACH_CHI_TIET_CLS_ADO
    {
        [XmlElement("CHI_TIET_CLS")]
        public List<XML4_CHI_TIET_CLS_ADO> CHI_TIET_CLS { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inventec.Desktop.ExportXML.QD917.ADO.XML5_ADO
{
    [XmlRoot("DSACH_CHI_TIET_DIEN_BIEN_BENH")]
    public class XML5_DSACH_CHI_TIET_DIEN_BIEN_BENH_ADO
    {
        [XmlElement("CHI_TIET_DIEN_BIEN_BENH")]
        public List<XML5_CHI_TIET_DIEN_BIEN_BENH_ADO> CHI_TIET_DIEN_BIEN_BENH { get; set; }
    }
}

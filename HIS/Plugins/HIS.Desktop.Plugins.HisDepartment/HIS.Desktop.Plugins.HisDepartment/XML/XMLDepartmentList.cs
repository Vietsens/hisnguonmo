using System.Collections.Generic;
using System.Xml.Serialization;

namespace HIS.Desktop.Plugins.HisDepartment.XML
{
    public class XMLDepartmentList
    {
        [XmlAttribute("Id")]
        public string Id { get; set; }

        [XmlElement("DMBOPHANCHUYENMON")]
        public List<XMLDepartmentDetailData> Items { get; set; }
    }
}

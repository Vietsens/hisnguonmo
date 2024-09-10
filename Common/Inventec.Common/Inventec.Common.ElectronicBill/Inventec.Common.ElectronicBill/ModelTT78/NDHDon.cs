using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inventec.Common.ElectronicBill.ModelTT78
{
    [XmlType(TypeName = "NDHDon")]
    public class NDHDon
    {
        [XmlElement("NBan")]
        public NBan nBan { get; set; }

        [XmlElement("NMua")]
        public NMua nMua { get; set; }

        [XmlArray("DSHHDVu")]
        [XmlArrayItem("HHDVu")]
        public List<HHDVu> hHDVu { get; set; }

        [XmlElement("TToan")]
        public TToan tToan { get; set; }
    }
}

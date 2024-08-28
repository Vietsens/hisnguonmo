using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inventec.Common.ElectronicBill.ModelTT78
{
    [XmlType(TypeName = "DLHDon")]
    public class DLHDon
    {
        [XmlElement("TTChung")]
        public TTChung tTChung { get; set; }

        [XmlElement("NDHDon")]
        public NDHDon nDHDon { get; set; }
    }
}

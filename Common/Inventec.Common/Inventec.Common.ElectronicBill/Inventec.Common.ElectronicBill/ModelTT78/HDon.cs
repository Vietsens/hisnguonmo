using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inventec.Common.ElectronicBill.ModelTT78
{
    [XmlType(TypeName = "HDon")]
    public class HDon
    {
        [XmlElement("key")]
        public string Key { get; set; }

        [XmlElement("DLHDon")]
        public DLHDon dLHDon { get; set; }
    }
}

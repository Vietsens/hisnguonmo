using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inventec.Common.ElectronicBill.MD
{
    public class Extras
    {
        [XmlElement("Extra_Name")]
        public string Extra_Name { get; set; }

        [XmlElement("Extra_Value")]
        public string Extra_Value { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inventec.Common.EBillSoftDreams.ModelXml
{
    [XmlType(TypeName = "Inv")]
    public class Inv
    {
        [XmlElement("Invoice")]
        public Invoice Invoice { get; set; }
    }
}

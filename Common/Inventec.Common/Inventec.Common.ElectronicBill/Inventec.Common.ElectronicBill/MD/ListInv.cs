using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inventec.Common.ElectronicBill.MD
{
    [XmlRoot("Data")]
    public class ListInv
    {
        [XmlElement("Item")]
        public List<InvByCusFkey> Item { get; set; }
    }
}

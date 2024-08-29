using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inventec.Common.ElectronicBill.MD
{
    [XmlType(TypeName = "Inv")]
    public class Invoice
    {
        [XmlElement("key")]
        public string Key { get; set; }

        [XmlElement("Invoice")]
        public InvoiceDetail InvoiceDetail { get; set; }
    }
}

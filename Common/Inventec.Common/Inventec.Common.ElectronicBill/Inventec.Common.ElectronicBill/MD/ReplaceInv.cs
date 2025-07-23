using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inventec.Common.ElectronicBill.MD
{
    [XmlType(TypeName = "ReplaceInv")]
    public class ReplaceInvoice: InvoiceDetail
    {
        [XmlElement("key")]
        public string key { get; set; }
    }
}

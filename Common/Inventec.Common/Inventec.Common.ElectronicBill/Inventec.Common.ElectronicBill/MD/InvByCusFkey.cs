using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Inventec.Common.ElectronicBill.MD
{
    [XmlRoot("Item")]
    public class InvByCusFkey
    {
        [XmlElement("index")]
        public string index { get; set; }

        [XmlElement("cusCode")]
        public string cusCode { get; set; }

        [XmlElement("month")]
        public string month { get; set; }

        [XmlElement("name")]
        public string name { get; set; }

        [XmlElement("publishDate")]
        public string publishDate { get; set; }

        [XmlElement("signStatus")]
        public string signStatus { get; set; }

        [XmlElement("pattern")]
        public string pattern { get; set; }

        [XmlElement("serial")]
        public string serial { get; set; }

        [XmlElement("invNum")]
        public string invNum { get; set; }

        [XmlElement("amount")]
        public string amount { get; set; }

        [XmlElement("status")]
        public string status { get; set; }

        [XmlElement("cusname")]
        public XmlCDataSection cusname { get; set; }

        [XmlElement("payment")]
        public string payment { get; set; }

        [XmlElement("converted")]
        public string converted { get; set; }
    }
}

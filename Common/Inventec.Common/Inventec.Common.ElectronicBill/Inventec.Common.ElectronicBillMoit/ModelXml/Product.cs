using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inventec.Common.ElectronicBillMoit.ModelXml
{
    public class Product
    {
        [XmlElement("Code")]
        public string Code { get; set; }

        [XmlElement("ProdName")]
        public string ProdName { get; set; }

        [XmlElement("ProdUnit")]
        public string ProdUnit { get; set; }

        [XmlElement("ProdQuantity")]
        public string ProdQuantity { get; set; }

        [XmlElement("ProdPrice")]
        public string ProdPrice { get; set; }

        [XmlElement("VATRate")]
        public string VATRate { get; set; }

        [XmlElement("VATAmount")]
        public string VATAmount { get; set; }

        [XmlElement("Total")]
        public string Total { get; set; }

        [XmlElement("Amount")]
        public string Amount { get; set; }
    }
}

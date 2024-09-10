using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Inventec.Common.ElectronicBill.MD
{
    public class InvoiceDetail
    {
        [XmlElement("Buyer")]
        public string Buyer { get; set; }

        [XmlElement("CusCode")]
        public string CusCode { get; set; }

        [XmlElement("CusName")]
        public string CusName { get; set; }

        [XmlElement("CusAddress")]
        public string CusAddress { get; set; }

        [XmlElement("CusPhone")]
        public string CusPhone { get; set; }

        [XmlElement("CusTaxCode")]
        public string CusTaxCode { get; set; }

        [XmlElement("PaymentMethod")]
        public string PaymentMethod { get; set; }

        [XmlElement("KindOfService")]
        public string KindOfService { get; set; }

        [XmlArray("Products")]
        [XmlArrayItem("Product")]
        public List<Product> Products { get; set; }

        [XmlElement("Total")]
        public string Total { get; set; }

        [XmlElement("DiscountAmount")]
        public string DiscountAmount { get; set; }

        [XmlElement("VATRate")]
        public string VATRate { get; set; }

        [XmlElement("VATAmount")]
        public string VATAmount { get; set; }

        [XmlElement("Amount")]
        public string Amount { get; set; }

        [XmlElement("AmountInWords")]
        public string AmountInWords { get; set; }

        [XmlElement("ArisingDate")]
        public string ArisingDate { get; set; }

        [XmlElement("Extra")]
        public string Extra { get; set; }

        [XmlElement("CurrencyUnit")]
        public string CurrencyUnit { get; set; }

        [XmlElement("CusBankNo")]
        public string CusBankNo { get; set; }
    }
}

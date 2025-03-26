/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *  
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *  
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *  
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Inventec.Common.ElectronicBillMoit.ModelXml
{
    public class InvoiceDetail
    {
        [XmlElement("CusCode")]
        public string CusCode { get; set; }

        [XmlElement("Buyer")]
        public string Buyer { get; set; }

        [XmlElement("CusName")]
        public string CusName { get; set; }

        [XmlElement("CusEmail")]
        public string CusEmail { get; set; }

        [XmlElement("CusAddress")]
        public string CusAddress { get; set; }

        [XmlElement("CusPhone")]
        public string CusPhone { get; set; }

        [XmlElement("CusTaxCode")]
        public string CusTaxCode { get; set; }

        [XmlElement("PaymentMethod")]
        public string PaymentMethod { get; set; }

        [XmlElement("ArisingDate")]
        public string ArisingDate { get; set; }

        [XmlArray("Products")]
        [XmlArrayItem("Product")]
        public List<Product> Products { get; set; }

        [XmlElement("Total")]
        public string Total { get; set; }

        [XmlElement("VATRate")]
        public string VATRate { get; set; }

        [XmlElement("VATAmount")]
        public string VATAmount { get; set; }

        [XmlElement("Amount")]
        public string Amount { get; set; }

        [XmlElement("AmountInWords")]
        public string AmountInWords { get; set; }
    }
}

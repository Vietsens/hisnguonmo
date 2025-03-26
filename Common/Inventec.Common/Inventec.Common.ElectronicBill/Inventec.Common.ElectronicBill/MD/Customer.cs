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

namespace Inventec.Common.ElectronicBill.MD
{
    [XmlType(TypeName = "Customer")]
    public class Customer
    {
        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("Code")]
        public string Code { get; set; }

        [XmlElement("TaxCode")]
        public string TaxCode { get; set; }

        [XmlElement("Address")]
        public string Address { get; set; }

        [XmlElement("BankAccountName")]
        public string BankAccountName { get; set; }

        [XmlElement("BankName")]
        public string BankName { get; set; }

        [XmlElement("BankNumber")]
        public string BankNumber { get; set; }

        [XmlElement("Email")]
        public string Email { get; set; }

        [XmlElement("Fax")]
        public string Fax { get; set; }

        [XmlElement("Phone")]
        public string Phone { get; set; }

        [XmlElement("ContactPerson")]
        public string ContactPerson { get; set; }

        [XmlElement("RepresentPerson")]
        public string RepresentPerson { get; set; }

        [XmlElement("CusType")]
        public long CusType { get; set; }
    }
}

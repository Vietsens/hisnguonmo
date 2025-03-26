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

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
    public class Product
    {
        [XmlElement("ProdName")]
        public string ProdName { get; set; }

        [XmlElement("ProdUnit")]
        public string ProdUnit { get; set; }

        [XmlElement("ProdQuantity")]
        public string ProdQuantity { get; set; }

        [XmlElement("ProdPrice")]
        public string ProdPrice { get; set; }

        [XmlElement("Amount")]
        public string Amount { get; set; }

        [XmlElement("VATRate")]
        public string VATRate { get; set; }

        [XmlElement("VATAmount")]
        public string VATAmount { get; set; }

        [XmlElement("Total")]
        public string Total { get; set; }

        ///// <summary>
        ///// Tính chất * (1-Hàng hóa, dịch vụ; 2-Khuyến mại; 3-Chiết khấu thương mại (trong trường hợp muốn thể hiện thông tin chiết khấu theo dòng); 4-Ghi chú/diễn giải)
        ///// </summary>
        //[XmlElement("IsSum")]
        //public string IsSum { get; set; }
    }
}

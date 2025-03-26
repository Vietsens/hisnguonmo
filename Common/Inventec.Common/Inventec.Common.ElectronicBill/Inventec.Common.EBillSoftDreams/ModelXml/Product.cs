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

namespace Inventec.Common.EBillSoftDreams.ModelXml
{
    public class Product
    {
        /// <summary>
        /// Mã sản phẩm
        /// </summary>
        [XmlElement("Code")]
        public string Code { get; set; }

        /// <summary>
        /// Tên sản phẩm
        /// </summary>
        [XmlElement("ProdName")]
        public string ProdName { get; set; }

        /// <summary>
        /// Đơn vị tính
        /// </summary>
        [XmlElement("ProdUnit")]
        public string ProdUnit { get; set; }

        /// <summary>
        /// Số lượng
        /// </summary>
        [XmlElement("ProdQuantity")]
        public decimal? ProdQuantity { get; set; }

        /// <summary>
        /// Đơn giá
        /// </summary>
        [XmlElement("ProdPrice")]
        public decimal? ProdPrice { get; set; }

        /// <summary>
        /// Tổng tiền trước thuế
        /// </summary>
        [XmlElement("Total")]
        public decimal Total { get; set; }

        /// <summary>
        /// Thuế suất 
        /// </summary>
        [XmlElement("VATRate")]
        public decimal? VATRate { get; set; }

        /// <summary>
        /// Tiền thuế
        /// </summary>
        [XmlElement("VATAmount")]
        public decimal? VATAmount { get; set; }

        /// <summary>
        /// Tổng tiền
        /// </summary>
        [XmlElement("Amount")]
        public decimal Amount { get; set; }

        /// <summary>
        /// {"Pos":"Số thứ tự"}
        /// </summary>
        [XmlElement("Extra")]
        public string Extra { get; set; }
    }
}

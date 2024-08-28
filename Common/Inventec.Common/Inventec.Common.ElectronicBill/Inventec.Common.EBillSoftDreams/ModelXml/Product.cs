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

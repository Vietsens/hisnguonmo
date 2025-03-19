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
    public class Invoice
    {
        /// <summary>
        /// Giá trị khóa duy nhất của hóa đơn
        /// </summary>
        [XmlElement("Ikey")]
        public string Ikey { get; set; }

        ///// <summary>
        ///// Số hoá đơn, kiểu số nguyên dương
        ///// </summary>
        //[XmlElement("InvNo")]
        //public long? InvNo { get; set; }

        /// <summary>
        /// Mã khách hàng
        /// Buyer và CusName không được đồng thời bỏ trống
        /// </summary>
        [XmlElement("CusCode")]
        public string CusCode { get; set; }

        /// <summary>
        /// Tên người mua hàng
        /// Buyer và CusName không được đồng thời bỏ trống
        /// </summary>
        [XmlElement("Buyer")]
        public string Buyer { get; set; }

        /// <summary>
        /// Tên khách hàng
        /// </summary>
        [XmlElement("CusName")]
        public string CusName { get; set; }

        /// <summary>
        /// Email của khách nhận thông báo phát hành hoá đơn
        /// </summary>
        [XmlElement("Email")]
        public string Email { get; set; }

        /// <summary>
        /// Danh sách email CC (ngăn cách bởi dấu phẩy) nhận thông báo phát hành hoá đơn
        /// </summary>
        [XmlElement("EmailCC")]
        public string EmailCC { get; set; }

        /// <summary>
        /// Địa chỉ khách hàng
        /// </summary>
        [XmlElement("CusAddress")]
        public string CusAddress { get; set; }

        /// <summary>
        /// Tên ngân hàng của khách hàng
        /// </summary>
        [XmlElement("CusBankName")]
        public string CusBankName { get; set; }

        /// <summary>
        /// Số tài khoản ngân hàng của khách hàng
        /// </summary>
        [XmlElement("CusBankNo")]
        public string CusBankNo { get; set; }

        /// <summary>
        /// Điện thoại khách hàng
        /// </summary>
        [XmlElement("CusPhone")]
        public string CusPhone { get; set; }

        /// <summary>
        /// Mã số thuế (Bắt buộc với KHDoanh nghiệp)
        /// </summary>
        [XmlElement("CusTaxCode")]
        public string CusTaxCode { get; set; }

        /// <summary>
        /// Phương thức thanh toán
        /// •	T/M: Tiền mặt
        /// •	C/K: Thanh toán chuyển khoản
        /// •	TM/CK: Thanh toán tiền mặt hoặc chuyển khoản
        /// •	TT/D: Thanh toán thẻ tín dụng
        /// •	Bù trừ: Thanh toán bù trừ
        /// </summary>
        [XmlElement("PaymentMethod")]
        public string PaymentMethod { get; set; }

        /// <summary>
        /// Ngày phát sinh hóa đơn (mặc định là ngày hiện tại, chuỗi định dạng dd/MM/yyyy)
        /// </summary>
        [XmlElement("ArisingDate")]
        public string ArisingDate { get; set; }

        /// <summary>
        /// Tỷ giá chuyển đổi
        /// </summary>
        [XmlElement("ExchangeRate")]
        public string ExchangeRate { get; set; }

        /// <summary>
        /// Đơn vị tiền tệ (ví dụ VND, USD)
        /// </summary>
        [XmlElement("CurrencyUnit")]
        public string CurrencyUnit { get; set; }

        /// <summary>
        /// Thông tin bổ sung (định dạng json có các thuộc tính theo quy ước riêng nếu có phát sinh)
        /// </summary>
        [XmlElement("Extra")]
        public string Extra { get; set; }

        [XmlArray("Products")]
        [XmlArrayItem("Product")]
        public List<Product> Products { get; set; }

        /// <summary>
        /// Tổng tiền trước thuế
        /// </summary>
        [XmlElement("Total")]
        public decimal Total { get; set; }

        /// <summary>
        /// Thuế GTGT
        /// •	-1: Không tính thuế
        /// •	0: Thuế suất 0%
        /// •	5: Thuế suất 5%
        /// •	10: Thuế suất 10%
        /// </summary>
        [XmlElement("VATRate")]
        public int VATRate { get; set; }

        /// <summary>
        /// Tiền thuế GTGT
        /// </summary>
        [XmlElement("VATAmount")]
        public decimal VATAmount { get; set; }

        /// <summary>
        /// Tổng tiền
        /// </summary>
        [XmlElement("Amount")]
        public decimal Amount { get; set; }

        /// <summary>
        /// Số tiền viết bằng chữ
        /// </summary>
        [XmlElement("AmountInWords")]
        public string AmountInWords { get; set; }
    }
}

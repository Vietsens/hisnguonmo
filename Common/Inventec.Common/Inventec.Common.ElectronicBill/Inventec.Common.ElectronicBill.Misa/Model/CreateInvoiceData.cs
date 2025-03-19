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

namespace Inventec.Common.ElectronicBill.Misa.Model
{
    public class CreateInvoiceData
    {
        /// <summary>
        /// ID của hóa đơn trên Client App
        /// </summary>
        public string RefID { get; set; }

        /// <summary>
        /// Mã loại hóa đơn, gồm các giá trị sau:
        /// - 01GTKT
        /// - 02GTTT
        /// - 07KPTQ
        /// - 03XKNB
        /// - 04HGDL
        /// </summary>
        public string InvoiceType { get; set; }

        /// <summary>
        /// Mẫu số hóa đơn
        /// </summary>
        public string TemplateCode { get; set; }

        /// <summary>
        /// Ký hiệu hóa đơn
        /// </summary>
        public string InvoiceSeries { get; set; }

        /// <summary>
        /// Mã loại tiền tệ
        /// </summary>
        public string CurrencyCode { get; set; }

        /// <summary>
        /// Ghi chú của hóa đơn
        /// </summary>
        public string InvoiceNote { get; set; }

        /// <summary>
        /// Trạng thái điều chỉnh của hóa đơn.
        /// 1: Hóa đơn gốc
        /// 3: Hóa đơn thay thế
        /// 5: Hóa đơn điều chỉnh
        /// </summary>
        public int AdjustmentType { get; set; }

        /// <summary>
        /// Phương thức thanh toán
        /// </summary>
        public string PaymentMethodName { get; set; }

        /// <summary>
        /// Mã số thuế người bán
        /// </summary>
        public string SellerTaxCode { get; set; }

        /// <summary>
        /// Tên người bán hàng
        /// </summary>
        public string SellerLegalName { get; set; }

        /// <summary>
        /// Địa chỉ người bán
        /// </summary>
        public string SellerAddressLine { get; set; }

        /// <summary>
        /// Số TK ngân hàng của người bán
        /// </summary>
        public string SellerBankAccount { get; set; }

        /// <summary>
        /// Tên ngân hàng của người bán
        /// </summary>
        public string SellerBankName { get; set; }

        /// <summary>
        /// Tên người mua.Trường hợp người mua là Doanh nghiệp thì là tên đăng ký kinh doanh của người mua
        /// </summary>
        public string BuyerLegalName { get; set; }

        /// <summary>
        /// Tên người liên hệ
        /// </summary>
        public string BuyerDisplayName { get; set; }

        /// <summary>
        /// Mã số thuế người mua
        /// </summary>
        public string BuyerTaxCode { get; set; }

        /// <summary>
        /// Địa chỉ người mua
        /// </summary>
        public string BuyerAddressLine { get; set; }

        /// <summary>
        /// Số điện thoại người mua
        /// </summary>
        public string BuyerPhoneNumber { get; set; }

        /// <summary>
        /// Địa chỉ Email người mua
        /// </summary>
        public string BuyerEmail { get; set; }

        /// <summary>
        /// Số tài khoản ngân hàng người mua
        /// </summary>
        public string BuyerBankAccount { get; set; }

        /// <summary>
        /// Tên ngân hàng của người mua
        /// </summary>
        public string BuyerBankName { get; set; }

        /// <summary>
        /// Tỷ giá chuyển đổi
        /// </summary>
        public decimal ExchangeRate { get; set; }

        /// <summary>
        /// Mức thuế GTGT.
        /// - 0: 0%
        /// - 5: 5%
        /// - 10: 10%
        /// - -1: Không chịu thuế
        /// </summary>
        public decimal VatPercentage { get; set; }

        /// <summary>
        /// Tổng tiền hàng chưa thuế GTGT
        /// </summary>
        public decimal TotalAmountWithoutVAT { get; set; }

        /// <summary>
        /// Tổng tiền thuế GTGT
        /// </summary>
        public decimal TotalVATAmount { get; set; }

        /// <summary>
        /// Tổng tiền thanh toán nguyên tệ (đã bao gồm thuế GTGT)
        /// </summary>
        public decimal TotalAmountWithVAT { get; set; }

        /// <summary>
        /// Tổng tiền thanh toán quy đổi (đã bao gồm thuế GTGT)
        /// </summary>
        public decimal TotalAmountWithVATFrn { get; set; }

        /// <summary>
        /// Tổng tiền thanh toán (viết bằng chữ)
        /// </summary>
        public string TotalAmountWithVATInWords { get; set; }

        /// <summary>
        /// Tổng tiền chiết khấu
        /// </summary>
        public decimal DiscountAmount { get; set; }

        /// <summary>
        /// Dữ liệu chi tiết các mặt hàng của 1 hóa đơn có định dạng JSON. Chi tiết các thông tin xem bảng phía dưới
        /// </summary>
        public List<InvoiceDetail> OriginalInvoiceDetail { get; set; }

        /// <summary>
        /// Object chứa tùy chọn để định dạng số các thông tin trên hóa đơn
        /// </summary>
        public UserDefined OptionUserDefined { get; set; }

        public string CustomField1 { get; set; }
        public string CustomField2 { get; set; }
        public string CustomField3 { get; set; }
        public string CustomField4 { get; set; }
        public string CustomField5 { get; set; }
        public string CustomField6 { get; set; }
        public string CustomField7 { get; set; }
        public string CustomField8 { get; set; }
        public string CustomField9 { get; set; }
        public string CustomField10 { get; set; }
    }
}

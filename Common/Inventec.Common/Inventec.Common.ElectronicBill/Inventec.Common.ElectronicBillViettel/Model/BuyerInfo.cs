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

namespace Inventec.Common.ElectronicBillViettel.Model
{
    /// <summary>
    /// Thông tin người mua trên hóa đơn
    /// </summary>
    [Serializable]
    public class BuyerInfo
    {
        /// <summary>
        /// Tên người mua trong trường hợp là người mua lẻ, cá nhân. Tên người mua hoặc tên đơn vị là bắt buộc.
        /// Maxlength: 255
        /// </summary>
        public string buyerName { get; set; }

        /// <summary>
        /// Mã khách hàng, chỉ cho phép các ký tự 
        /// Maxlength: 100
        /// </summary>
        public string buyerCode { get; set; }

        /// <summary>
        /// Tên đơn vị (đăng ký kinh doanh trong trường hợp là doanh nghiệp) của người mua. Tên người mua hoặc tên đơn vị là bắt buộc.
        /// Maxlength: 255
        /// </summary>
        public string buyerLegalName { get; set; }

        /// <summary>
        /// Mã số thuế người mua, có thể là mã số thuế Việt Nam hoặc mã số thuế nước ngoài
        /// Maxlength: 20
        /// </summary>
        public string buyerTaxCode { get; set; }

        /// <summary>
        /// Địa chỉ xuất hóa đơn của người mua
        /// Maxlength: 255
        /// </summary>
        public string buyerAddressLine { get; set; }

        /// <summary>
        /// Số điện thoại người mua, số điện thoại sẽ được dùng để gửi tin nhắn trong trường hợp bên bán đăng ký dịch vụ SMS Brandname. Nếu có nhiều số điện thoại thì cách nhau bởi dấu (;)
        /// Maxlength: 20
        /// </summary>
        public string buyerPhoneNumber { get; set; }

        /// <summary>
        /// Số fax người mua
        /// Maxlength: 20
        /// </summary>
        public string buyerFaxNumber { get; set; }

        /// <summary>
        /// Email người mua, sử dụng để gửi hóa đơn cho người mua. Nếu có nhiều email thì cách nhau bởi dấu chấm phẩy (;).
        /// Maxlength: 500
        /// </summary>
        public string buyerEmail { get; set; }

        /// <summary>
        /// Tên trụ sở chính ngân hàng nơi người mua mở tài khoản giao dịch. Nếu có nhiều tên ngân hàng thì cách nhau bởi dấu chấm phẩy (;)
        /// Maxlength: 100
        /// </summary>
        public string buyerBankName { get; set; }

        /// <summary>
        /// Tài khoản ngân hàng của người mua. Nếu có nhiều số tài khoản thì cách nhau bởi dấu chấm phẩy (;)
        /// Maxlength: 20
        /// </summary>
        public string buyerBankAccount { get; set; }

        /// <summary>
        /// Tên Quận Huyện
        /// Maxlength: 50
        /// </summary>
        public string buyerDistrictName { get; set; }

        /// <summary>
        /// Tên Tỉnh/Thành phố
        /// Maxlength: 25
        /// </summary>
        public string buyerCityName { get; set; }

        /// <summary>
        /// Mã quốc gia người mua
        /// Maxlength: 15
        /// </summary>
        public string buyerCountryCode { get; set; }

        /// <summary>
        /// Loại giấy tờ của người mua
        /// Maxlength: 15
        /// </summary>
        public string buyerIdType { get; set; }

        /// <summary>
        /// Số giấy tờ của người mua, có thể là chứng minh thư, giấy phép kinh doanh, hộ chiếu.
        /// Maxlength: 15
        /// </summary>
        public string buyerIdNo { get; set; }

        /// <summary>
        /// Ngày sinh của người mua
        /// Maxlength: 15
        /// </summary>
        public string buyerBirthDay { get; set; }
    }
}

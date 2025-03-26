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
    /// Thông tin người bán trên hóa đơn, có thể được truyền sang hoặc lấy tự động trên hệ thống hóa đơn điện tử. Trong trường hợp sellerTaxCode không được truyền sang thì dữ liệu sẽ được lấy từ hệ thống hóa đơn điện tử.
    /// </summary>
    [Serializable]
    public class SellerInfo
    {
        /// <summary>
        /// Tên (đăng ký kinh doanh trong trường hợp là doanh nghiệp) của người bán
        /// Bắt buộc
        /// Maxlength: 255
        /// </summary>
        public string sellerLegalName { get; set; }

        /// <summary>
        /// Mã số thuế người bán được cấp bởi TCT Việt Nam.
        /// Maxlength: 20
        /// </summary>
        public string sellerTaxCode { get; set; }

        /// <summary>
        /// Mã chi nhánh, trong trường hợp nhiều chi nhánh mà cần có trường thông tin để đánh dấu riêng về chi nhánh.
        /// Maxlength: 20
        /// </summary>
        public string sellerCode { get; set; }

        /// <summary>
        /// Địa chỉ của bên bán được thể hiện trên hóa đơn.
        /// Maxlength: 255
        /// </summary>
        public string sellerAddressLine { get; set; }

        /// <summary>
        /// Số điện thoại người bán
        /// Maxlength: 20
        /// </summary>
        public string sellerPhoneNumber { get; set; }

        /// <summary>
        /// Số fax người bán
        /// Maxlength: 20
        /// </summary>
        public string sellerFaxNumber { get; set; }

        /// <summary>
        /// Địa chỉ thư điện tử người bán
        /// Maxlength: 50
        /// </summary>
        public string sellerEmail { get; set; }

        /// <summary>
        /// Tên ngân hàng nơi người bán mở tài khoản giao dịch, Nếu có nhiều các thông tin cách nhau bởi dấu chấm phẩy (;)
        /// Maxlength: 100
        /// </summary>
        public string sellerBankName { get; set; }

        /// <summary>
        /// Tài khoản ngân hàng của người bán, Nếu có nhiều các thông tin cách nhau bởi dấu chấm phẩy (;)
        /// Maxlength: 20
        /// </summary>
        public string sellerBankAccount { get; set; }

        /// <summary>
        /// Tên Quận Huyện
        /// Maxlength: 50
        /// </summary>
        public string sellerDistrictName { get; set; }

        /// <summary>
        /// Tên Tỉnh/Thành phố
        /// Maxlength: 25
        /// </summary>
        public string sellerCityName { get; set; }

        /// <summary>
        /// Mã quốc gia 
        /// Maxlength: 15
        /// </summary>
        public string sellerCountryCode { get; set; }

        /// <summary>
        /// Website của người bán
        /// Maxlength: 100
        /// </summary>
        public string sellerWebsite { get; set; }
    }
}

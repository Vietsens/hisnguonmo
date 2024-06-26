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

namespace HIS.Desktop.Plugins.Library.ElectronicBill.ProxyBehavior.CTO.Model
{
    class EBill
    {
        /// <summary>
        /// Mã duy nhất từ hệ thống tích hợp để xác định hóa đơn
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// Trạng hóa phát thành hóa đơn, mặc định = “issued”
        /// </summary>
        public string status { get; set; }

        /// <summary>
        /// Mẫu hóa đơn phát hành
        /// </summary>
        public EbTemplate template { get; set; }

        /// <summary>
        /// Thông tin người mua trong trường hợp là người mua lẻ, cá nhân. Nếu có thông tin này thì không cần truyền company
        /// </summary>
        public EbInfo buyer { get; set; }

        /// <summary>
        /// Thông tin đơn vị (đăng ký kinh doanh trong trường hợp là doanh nghiệp) của người mua. Nếu có thông tin này thì không cần truyền buyer
        /// </summary>
        public EbInfo company { get; set; }

        /// <summary>
        /// Tên người dùng của phần mềm tích hợp khi tạo hóa đơn
        /// </summary>
        public string creator { get; set; }

        /// <summary>
        /// Ngày lập hóa đơn, tuân theo nguyên tắc đảm bảo về trình tự thời gian trong 1 ký hiệu hóa đơn của một mẫu hóa đơn với một mã số thuế cụ thể:
        /// số hóa đơn sau phải được lập với thời gian lớn hơn hoặc bằng số hóa đơn trước.
        /// yyyy-MM-ddTHH:mm:sszzz
        /// </summary>
        public string issued_date { get; set; }

        /// <summary>
        /// Mã tiền tệ dùng cho hóa đơn có chiều dài 3 ký tự theo quy định của NHNN Việt Nam. Ví dụ: USD, VND, EUR
        /// </summary>
        public string currency { get; set; }

        /// <summary>
        /// Hình thức thanh toán ví dụ: TM, CK,...
        /// </summary>
        public string method { get; set; }

        /// <summary>
        /// Danh sách chi tiết dịch vụ hàng hóa để lập hóa đơn
        /// </summary>
        public List<EbProduct> items { get; set; }

        /// <summary>
        /// Tổng số tiền bằng chữ
        /// </summary>
        public string amount_inwords { get; set; }

        /// <summary>
        /// Thông tin thuế của hóa đơn
        /// </summary>
        public EbVat vat { get; set; }

        /// <summary>
        /// Là tổng tiền chưa bao gồm VAT của hóa đơn
        /// </summary>
        public decimal total_net { get; set; }

        /// <summary>
        /// Là tổng tiền đã bao gồm VAT của hóa đơn
        /// </summary>
        public decimal total_gross { get; set; }

        /// <sumary>
        /// Mã số bệnh nhân của hóa đơn phát hành hóa đơn
        /// </sumary>
        public string patient_id { get; set; }

        /// <sumary>
        /// Mã số điều trị của hóa đơn phát hành hóa đơn
        /// </sumary>
        public string encounter { get; set; }
    }
}

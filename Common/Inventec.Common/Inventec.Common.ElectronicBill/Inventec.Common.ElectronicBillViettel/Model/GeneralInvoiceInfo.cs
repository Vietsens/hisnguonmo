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
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.ElectronicBillViettel.Model
{
    [Serializable]
    public class GeneralInvoiceInfo
    {
        /// <summary>
        /// Mã loại hóa đơn chỉ nhận các giá trị sau: 01GTKT, 02GTTT, 07KPTQ, 03XKNB, 04HGDL, 01BLP
        /// Maxlength: 50
        /// </summary>
        public string invoiceType { get; set; }

        /// <summary>
        /// Ký hiệu mẫu hóa đơn, tuân thủ theo quy định ký hiệu mẫu hóa đơn của Thông tư hướng dẫn thi hành nghị định số 51/2010/NĐ-CP
        /// Maxlength: 20
        /// Ví dụ 01GTKT0/001, trong đó 
        /// 01GTKT: ký hiệu loại hóa đơn
        /// 0: số liên, đối với hóa đơn điện tử luôn là 0
        /// 001: số thứ tự tăng dần theo số lượng mẫu DN đăng ký với cơ quan thuế
        /// </summary>
        public string templateCode { get; set; }

        /// <summary>
        /// Là “Ký hiệu hóa đơn” tuân thủ theo quy tắc tạo ký hiệu hóa đơn của Thông tư hướng dẫn thi hành nghị định số 51/2010/NĐ-CP.
        /// Ví dụ AA/16E.
        /// Minlength : 6
        /// Maxlength : 6
        /// </summary>
        public string invoiceSeries { get; set; }

        /// <summary>
        /// Ngày lập hóa đơn, tuân theo nguyên tắc đảm bảo về trình tự thời gian trong 1 ký hiệu hóa đơn của một mẫu hóa đơn với một mã số thuế cụ thể: số hóa đơn sau phải được lập với thời gian lớn hơn hoặc bằng số hóa đơn trước.
        /// yyyy-MM-ddTHH:mm:sszzz
        /// </summary>
        public string invoiceIssuedDate { get; set; }

        /// <summary>
        /// Mã tiền tệ dùng cho hóa đơn có chiều dài 3 ký tự theo quy định của NHNN Việt Nam. Ví dụ: USD, VND, EUR…
        /// Minlength: 3
        /// Maxlength: 3
        /// </summary>
        public string currencyCode { get; set; }

        /// <summary>
        /// Trạng thái điều chỉnh hóa đơn: 
        /// 1: Hóa đơn gốc 
        /// 3: Hóa đơn thay thế 
        /// 5: Hóa đơn điều chỉnh (dự kiến sẽ bỏ theo NĐ119)
        /// Maxlength: 1
        /// </summary>
        public string adjustmentType { get; set; }

        /// <summary>
        /// Loại điều chỉnh đối với hóa đơn điều chỉnh
        /// 1: Hóa đơn điều chỉnh tiền (dự kiến sẽ bỏ theo NĐ119)
        /// 2: Hóa đơn điều chỉnh thông tin (dự kiến sẽ bỏ theo NĐ119)
        /// Maxlength: 1
        /// </summary>
        public string adjustmentInvoiceType { get; set; }

        /// <summary>
        /// Số hóa đơn. Mặc định là hệ thống tự sinh. 
        /// Minlength: 7
        /// Maxlength: 13
        /// </summary>
        public string invoiceNo { get; set; }

        /// <summary>
        /// Số hóa đơn của hóa đơn gốc trong trường hợp lập hóa đơn là: 
        ///     Hóa đơn thay thế
        ///     Hóa đơn điều chỉnh
        ///Số hóa đơn có dạng AA/18E0000000, trong đó
        ///     AA/18E: ký hiệu hóa đơn
        ///     0000000: số thứ tự tăng dần
        ///Minlength: 7
        ///Maxlength: 13
        /// </summary>
        public string originalInvoiceId { get; set; }

        /// <summary>
        /// Thời gian lập hóa đơn gốc, bắt buộc trong trường hợp lập hóa đơn thay thế và hóa đơn điều chỉnh. Dùng để tìm kiếm hóa đơn gốc của hóa đơn thay thế, điều chỉnh
        /// Maxlength: 50
        /// Format: yyyy-MM-dd  
        /// </summary>
        public string originalInvoiceIssueDate { get; set; }

        /// <summary>
        /// Thông tin tham khào nếu có kèm theo của hóa đơn: văn bản thỏa thuận giữa bên mua và bên bán về việc thay thế, điều chỉnh hóa đơn. Bắt buộc khi lập hóa đơn thay thế, hóa đơn điều chỉnh.
        /// Maxlength : 225
        /// </summary>
        public string additionalReferenceDesc { get; set; }

        /// <summary>
        /// Thời gian phát sinh văn bản thỏa thuận giữa bên mua và bên bán, bắt buộc khi lập hóa đơn thay thế, hóa đơn điều chỉnh.
        /// yyyy-MM-ddTHH:mm:sszzz
        /// </summary>
        public string additionalReferenceDate { get; set; }

        /// <summary>
        /// Trạng thái thanh toán của hóa đơn
        /// True: Đã thanh toán
        /// False: Chưa thanh toán
        /// </summary>
        public bool paymentStatus { get; set; }

        /// <summary>
        /// Mặc định true.
        /// </summary>
        public bool cusGetInvoiceRight { get; set; }

        public Decimal? exchangeRate { get; set; }

        /// <summary>
        /// ID để kiểm trùng giao dịch lập hóa đơn, được sinh ra từ hệ thống của bên đối tác, là duy nhất với mỗi hóa đơn.
        /// Trong trường hợp gửi transactionUuid thì bên hệ thống đối tác sẽ tự quản lý để đảm bảo tính duy nhất của transactionUuid.
        /// Với mỗi transactionUuid, khi đã gửi một transactionUuid với một hóa đơn A thì mọi request lập hóa đơn với cùng transactionUuid sẽ trả về hóa đơn A chứ không lập hóa đơn khác.
        /// Thời gian hiệu lực của transactionUuid là 3 ngày.
        /// Khuyến cáo: sử dụng UUID V4 để tránh bị trùng số.
        /// Minlength: 10
        /// Maxlength: 36
        /// </summary>
        public string transactionUuid { get; set; }

        /// <summary>
        /// Tên người lập hóa đơn. Nếu không truyền sang, hệ thống sẽ tự động lấy user được dùng để xác thực để lưu vào
        /// Maxlength: 100
        /// </summary>
        public string userName { get; set; }

        /// <summary>
        /// Được sử dụng khi lập hóa đơn sử dụng USB Token.
        /// Serial Number của chứng thư số của doanh nghiệp, chứng thư số này đã được doanh nghiệp đẩy lên trên hệ thống khi đăng ký sử dụng USB Token.
        /// Định dạng Hex. Ví dụ: 5404FFFEB7033FB316D672201B7BA4FE
        /// </summary>
        public string certificateSerial { get; set; }

        /// <summary>
        /// Hình thức thanh toán
        /// </summary>
        public string paymentTypeName { get; set; }

        /// <summary>
        /// lý do điều chỉnh
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string adjustedNote { get; set; }
    }
}

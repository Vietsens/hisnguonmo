using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.ElectronicBillViettel.Model
{
    public class Response
    {
        /// <summary>
        /// Mã lỗi (giá trị là null nếu lập hóa đơn thành công)
        /// </summary>
        public string errorCode { get; set; }

        /// <summary>
        /// Mô tả lỗi (giá trị là null nếu lập hóa đơn thành công)
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// kết quả tạo hóa đơn
        /// </summary>
        public ResponseResult result { get; set; }

        /// <summary>
        /// trường động theo mẫu
        /// </summary>
        public List<CustomFields> customFields { get; set; }

        /// <summary>
        /// Tên file tải về
        /// </summary>
        public string fileName { get; set; }

        /// <summary>
        /// Nội dung file được chuyển thành kiểu byte
        /// FileUtils.writeByteArrayToFile(newFile("D:/viettel/fileName.zip"), output.getFileToBytes());
        /// </summary>
        public byte[] fileToBytes { get; set; }

        /// <summary>
        /// Trạng thái thanh toán 
        /// </summary>
        public bool? paymentStatus { get; set; }

        /// <summary>
        /// Tổng số dòng trong trường hợp tìm kiếm
        /// </summary>
        public long totalRow { get; set; }

        /// <summary>
        /// Danh sách thông tin hóa đơn trong trường hợp tìm kiếm
        /// Lấy thông tin issueDate
        /// </summary>
        public List<InvoiceInfoData> invoices { get; set; }

        /// <summary>
        /// Mã duy nhất do his gửi lên
        /// </summary>
        public string transactionUuid { get; set; }
    }
}

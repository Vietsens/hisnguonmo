using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.ElectronicBill.Misa.Model
{
    public class Response
    {
        /// <summary>
        /// Mô tả lỗi (giá trị là null nếu lập hóa đơn thành công)
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// kết quả tạo hóa đơn
        /// </summary>
        public List<InvoiceResult> result { get; set; }

        /// <summary>
        /// kết quả tạo hóa đơn V2
        /// </summary>
        public List<InvoiceResultV2> resultV2 { get; set; }

        /// <summary>
        /// Nội dung file được chuyển thành kiểu byte
        /// FileUtils.writeByteArrayToFile(newFile("D:/viettel/fileName.zip"), output.getFileToBytes());
        /// </summary>
        public byte[] fileToBytes { get; set; }

        /// <summary>
        /// đường dẫn tải file pdf
        /// </summary>
        public string fileDownload { get; set; }

        /// <summary>
        /// dữ liệu xml kết quả
        /// </summary>
        public string XmlData { get; set; }
    }
}

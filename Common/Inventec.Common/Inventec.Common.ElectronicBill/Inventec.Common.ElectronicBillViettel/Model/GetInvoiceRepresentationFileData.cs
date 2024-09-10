using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.ElectronicBillViettel.Model
{
    public class GetInvoiceRepresentationFileData
    {
        /// <summary>
        /// Mã số thuế của doanh nghiệp
        /// Bắt buộc
        /// </summary>
        public string supplierTaxCode { get; set; }

        /// <summary>
        /// Số hóa đơn, bao gồm ký hiệu hóa đơn và số thứ tự hóa đơn
        /// </summary>
        public string invoiceNo { get; set; }

        /// <summary>
        /// Mã mẫu hóa đơn
        /// </summary>
        public string templateCode { get; set; }

        /// <summary>
        /// Chuỗi kiểm tra dữ liệu (fkey) được truyền vào khi lập hóa đơn
        /// </summary>
        public string transactionUuid { get; set; }

        /// <summary>
        /// Loại file muốn tải về, các định dạng được phép ZIP, PDF
        /// </summary>
        public string fileType { get; set; }

        /// <summary>
        /// true – Đã thanh toán
        /// false – Chưa thanh toán
        /// </summary>
        public bool? paid { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.ElectronicBill.Misa.Model
{
    public class InvoiceResultV2
    {
        /// <summary>
        /// ID của hóa đơn gốc
        /// </summary>
        public string RefID { get; set; }

        /// <summary>
        /// Mã tra cứu hóa đơn
        /// </summary>
        public string TransactionID { get; set; }

        /// <summary>
        /// Số hóa đơn
        /// </summary>
        public string InvNo { get; set; }

        /// <summary>
        /// Ngày hóa đơn
        /// </summary>
        public DateTime? InvDate { get; set; }

        /// <summary>
        /// Nội dung hóa đơn (định dạng XML)
        /// </summary>
        public string InvoiceData { get; set; }

        /// <summary>
        /// Dữ liệu file PDF
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// Mã lỗi trả về nếu việc chuyển hóa đơn thành định dạng hóa đơn điện tử không thành công.
        /// </summary>
        public string ErrorCode { get; set; }
    }
}

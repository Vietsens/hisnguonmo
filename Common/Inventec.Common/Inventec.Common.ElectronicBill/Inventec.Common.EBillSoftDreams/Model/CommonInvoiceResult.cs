using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.EBillSoftDreams.Model
{
    public class CommonInvoiceResult
    {
        /// <summary>
        /// Trạng thái hoá đơn
        /// •	-1: Hoá đơn không tồn tại trong hệ thống
        /// •	0: Hoá đơn chưa có chữ ký số
        /// •	1: Hoá đơn có chữ ký số
        /// •	2: Hoá đơn đã khai báo thuế
        /// •	3: Hoá đơn bị thay thế
        /// •	4: Hoá đơn bị điều chỉnh
        /// •	5: Hoá đơn bị huỷ
        /// •	6: Hoá đơn đã duyệt (dành cho khách hàng sử dụng quy trình duyệt hoá đơn trước ký)
        /// </summary>
        public long InvoiceStatus { get; set; }

        /// <summary>
        /// Mẫu
        /// </summary>
        public string Pattern { get; set; }

        /// <summary>
        /// Ký hiệu
        /// </summary>
        public string Serial { get; set; }

        /// <summary>
        /// Số hoá đơn
        /// </summary>
        public string No { get; set; }

        /// <summary>
        /// Mã tra cứu
        /// </summary>
        public string LookupCode { get; set; }

        /// <summary>
        /// khoá tích hợp ikey
        /// </summary>
        public string Ikey { get; set; }

        /// <summary>
        /// Ngày tạo lập
        /// </summary>
        public string ArisingDate { get; set; }

        /// <summary>
        /// Ngày phát hành
        /// </summary>
        public string IssueDate { get; set; }

        /// <summary>
        /// Tên khách hàng
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// Mã khách hàng
        /// </summary>
        public string CustomerCode { get; set; }

        /// <summary>
        /// Người mua
        /// </summary>
        public string Buyer { get; set; }

        /// <summary>
        /// Giá trị hoá đơn
        /// </summary>
        public string Amount { get; set; }
    }
}

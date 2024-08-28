using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.ElectronicBill.Misa.Model
{
    public class DeleteInvoiceData
    {
        /// <summary>
        /// Mã tra cứu của hóa đơn bị xóa bỏ
        /// </summary>
        public string TransactionID { get; set; }

        /// <summary>
        /// Ngày chứng từ xóa bỏ hóa đơn
        /// </summary>
        public DateTime RefDate { get; set; }

        /// <summary>
        /// Số chứng từ xóa bỏ hóa đơn
        /// </summary>
        public string RefNo { get; set; }

        /// <summary>
        /// Lý do xóa bỏ
        /// </summary>
        public string DeletedReason { get; set; }

        /// <summary>
        /// Ngày của biên bản thỏa thuận xóa bỏ hóa đơn giữa người mua và người bán
        /// </summary>
        public string MinutesDate { get; set; }

        /// <summary>
        /// Số của biên bản thỏa thuận xóa bỏ hóa đơn giữa người mua và người bán
        /// </summary>
        public string MinutesNo { get; set; }

        /// <summary>
        /// Lý do ghi trên biên bản thỏa thuận xóa bỏ hóa đơn giữa người mua và người bán
        /// </summary>
        public string MinutesReason { get; set; }

        /// <summary>
        /// Tên tệp biên bản thỏa thuận
        /// </summary>
        public string MinutesFileName { get; set; }

        /// <summary>
        /// Nội dung biên bản thỏa thuận đã encode base64
        /// </summary>
        public string MinutesFileContent { get; set; }
    }
}

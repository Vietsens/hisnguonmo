using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.ElectronicBill.Misa.Model
{
    public class ReleaseInvoiceV2
    {
        /// <summary>
        /// ID của hóa đơn trên Client App
        /// </summary>
        public string RefID { get; set; }

        /// <summary>
        /// Mã tra cứu của hóa đơn
        /// </summary>
        public string TransactionID { get; set; }

        /// <summary>
        /// Nội dung hóa đơn đã được ký điện tử
        /// SignResult.PayLoad
        /// </summary>
        public string InvoiceData { get; set; }

        /// <summary>
        /// Có gửi Email sau khi phát hành hay không
        /// </summary>
        public bool IsSendEmail { get; set; }

        /// <summary>
        /// Tên người nhận Email.Bắt buộc khi IsSendEmail = true
        /// </summary>
        public string ReceiverName { get; set; }

        /// <summary>
        /// Danh sách các Email nhận cách nhau bởi dấu ;
        /// Bắt buộc khi IsSendEmail = true
        /// </summary>
        public string ReceiverEmail { get; set; }
    }
}

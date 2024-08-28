using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.EBillSoftDreams
{
    public class Response
    {
        public Response()
        {
            Messages = new List<string>();
        }

        public bool Success { get; set; }

        /// <summary>
        /// Mô tả lỗi (giá trị là null nếu lập hóa đơn thành công)
        /// </summary>
        public List<string> Messages { get; set; }

        /// <summary>
        /// Số hóa đơn
        /// </summary>
        public string invoiceNo { get; set; }

        public string Ikey { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string fileName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public byte[] fileToBytes { get; set; }

        /// <summary>
        /// Mã tra cứu
        /// </summary>
        public string LookupCode { get; set; }

        /// <summary>
        /// Ngày phát hành
        /// </summary>
        public string IssueDate { get; set; }
    }
}

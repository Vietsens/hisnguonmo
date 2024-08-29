using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.EHoaDon
{
    [Serializable]
    public class InvoiceResult
    {
        public long PartnerInvoiceID { get; set; }
        public string PartnerInvoiceStringID { get; set; }

        public Guid InvoiceGUID { get; set; }

        public string InvoiceForm { get; set; }
        public string InvoiceSerial { get; set; }
        public int InvoiceNo { get; set; }

        /// <summary>
        /// Trạng thái xử lý: 0 - thêm mới thành công, 1 - lỗi
        /// </summary>
        public int Status { get; set; }
        public string MessLog { get; set; }

        /// <summary>
        /// Mã tra cứu
        /// </summary>
        public string MTC { get; set; }
    }
}

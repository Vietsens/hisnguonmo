using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.ElectronicBillViettel.Model
{
    public class ResponseResult
    {
        /// <summary>
        /// Số hóa đơn ví dụ: AA/16E0000001 
        /// </summary>
        public string invoiceNo { get; set; }

        /// <summary>
        /// Mã số bí mật dùng để tra khách hàng tra cứu
        /// </summary>
        public string reservationCode { get; set; }

        /// <summary>
        /// Mã số thuế người bán (doanh nghiệp phát hành hóa đơn)
        /// </summary>
        public string supplierTaxCode { get; set; }

        /// <summary>
        /// Id của giao dịch
        /// </summary>
        public string transactionID { get; set; }

        /// <summary>
        /// thời gian giao dịch
        /// milliseconds
        /// 1587797116843
        /// </summary>
        public double issueDate { get; set; }

        /// <summary>
        /// trạng thái
        /// </summary>
        public string status { get; set; }
    }
}

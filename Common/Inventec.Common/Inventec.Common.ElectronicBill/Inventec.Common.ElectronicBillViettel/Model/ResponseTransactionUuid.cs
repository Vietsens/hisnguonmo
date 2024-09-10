using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.ElectronicBillViettel.Model
{
    class ResponseTransactionUuid
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
        public List<ResponseResult> result { get; set; }
    }
}

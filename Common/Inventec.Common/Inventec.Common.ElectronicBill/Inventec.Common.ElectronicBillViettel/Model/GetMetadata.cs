using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.ElectronicBillViettel.Model
{
    public class GetMetadata
    {
        /// <summary>
        /// Mã số thuế
        /// Maxlength : 11
        /// </summary>
        public string taxCode { get; set; }

        /// <summary>
        /// Mã mẫu hóa đơn, tuân thủ theo quy định ký hiệu mẫu hóa đơn của Thông tư hướng dẫn thi hành nghị định số 51/2010/NĐ-CP
        /// Maxlength : 11
        /// </summary>
        public string templateCode { get; set; }
    }
}

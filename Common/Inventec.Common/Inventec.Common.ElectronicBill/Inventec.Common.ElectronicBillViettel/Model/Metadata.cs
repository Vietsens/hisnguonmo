using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.ElectronicBillViettel.Model
{
    public class Metadata
    {
        /// <summary>
        /// ID của trường động
        /// Maxlength : 10
        /// </summary>
        public long invoiceCustomFieldId { get; set; }

        /// <summary>
        /// Tên của trường động khi lưu vào dữ liệu
        /// </summary>
        public string keyTag { get; set; }

        /// <summary>
        /// Kiểu dữ liệu của trường động. Chỉ bao gồm các giá trị: “text”,  “date”, “number”
        /// </summary>
        public string valueType { get; set; }

        /// <summary>
        /// Giá trị dữ liệu khi kiểu là text
        /// Maxlength: 13
        /// </summary>
        public string stringValue { get; set; }

        /// <summary>
        /// Giá trị của trường dữ liệu trong trường hợp valueType = date
        /// Đinh dạng: yyyy-MM-ddTHH:mm:sszzz 
        /// </summary>
        public string dateValue { get; set; }

        /// <summary>
        /// Giá trị dữ liệu khi kiểu là number
        /// Maxlength: 6
        /// </summary>
        public long? numberValue { get; set; }

        /// <summary>
        /// Tên hiển thị của trường động, Hiển thị trên giao diện nhập liệu khi lập hóa đơn
        /// </summary>
        public string keyLabel { get; set; }

        /// <summary>
        /// Trường có bắt buộc hay không
        /// </summary>
        public bool? isRequired { get; set; }

        /// <summary>
        /// isSeller = true: Trường dữ liệu thuộc bên bán
        /// isSeller = false: Trường dữ liệu thuộc bên mua
        /// </summary>
        public bool? isSeller { get; set; }
    }
}

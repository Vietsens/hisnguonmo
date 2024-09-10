using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.ElectronicBill.Misa.Model
{
    public class SignXml
    {
        /// <summary>
        /// PinCode
        /// Mật khẩu cài đặt chứng thư số
        /// </summary>
        public string PinCode { get; set; }

        /// <summary>
        /// Xml hóa đơn điện tử
        /// nhận được khi tạo hóa đơn
        /// </summary>
        public string XmlContent { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.ElectronicBill.Misa.Model
{
    /// <summary>
    /// Tham số để lấy token
    /// </summary>
    class GetTokenParameter
    {
        /// <summary>
        /// ID của ứnng dụng tích hợp
        /// </summary>
        public string AppID { get; set; }

        /// <summary>
        /// Mã số thuế
        /// </summary>
        public string TaxCode { get; set; }

        /// <summary>
        /// Tên đăng nhập
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Mật khẩu
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// ID thiết bị xác thực 2 lớp MISAID
        /// </summary>
        public string DeviceID { get; set; }
    }
}

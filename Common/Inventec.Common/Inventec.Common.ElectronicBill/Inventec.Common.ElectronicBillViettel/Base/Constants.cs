using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.ElectronicBillViettel.Base
{
    class Constants
    {
        internal static int TIME_OUT = 90;
        internal const string ErrorCode = "ERROR_APP";
        internal const string keyHeaderToken = "Cookie";
        internal const string valueHeaderToken = "access_token={0}";
    }

    /// <summary>
    /// Lớp chứa các key định nghĩa phương thức gọi request lên meInvoice Cloud
    /// </summary>
    class HttpMethod
    {
        /// <summary>
        /// Phương thức GET
        /// </summary>
        public const string GET = "GET";

        /// <summary>
        /// Phương thức POST
        /// </summary>
        public const string POST = "POST";

        /// <summary>
        /// Phương thức PUT
        /// </summary>
        public const string PUT = "PUT";

        /// <summary>
        /// Phương thức Xóa
        /// </summary>
        public const string DELETE = "DELETE";
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.ElectronicBill.Misa.Model
{
    public class SignResult
    {
        /// <summary>
        /// Chuỗi xml invoice đã được ký số
        /// </summary>
        public string PayLoad { get; set; }

        /// <summary>
        /// Trống
        /// </summary>
        public string Message { get; set; }
    }
}

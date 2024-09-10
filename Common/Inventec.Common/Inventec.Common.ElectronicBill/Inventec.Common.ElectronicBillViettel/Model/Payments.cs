using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.ElectronicBillViettel.Model
{
    /// <summary>
    /// Theo quy định thì 1 hóa đơn có thể có 1 hoặc nhiều hình thức thanh toán
    /// </summary>
    [Serializable]
    public class Payments
    {
        /// <summary>
        /// Tên phương thức thanh toán. Có thể nhập giá trị tùy ý.
        /// Maxlength: 30
        /// Bắt buộc
        /// </summary>
        public string paymentMethodName { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.ElectronicBill.Misa.Model
{
    public class UserDefined
    {
        /// <summary>
        /// Đồng tiền hạch toán. VD: VND
        /// </summary>
        public string MainCurrency { get; set; }

        /// <summary>
        /// Số chữ số phần thập phân của tiền quy đổi, mặc định = 0
        /// </summary>
        public string AmountDecimalDigits { get; set; }

        /// <summary>
        /// Số chữ số phần thập phân của tiền nguyên tệ, mặc định = 0
        /// </summary>
        public string AmountOCDecimalDigits { get; set; }

        /// <summary>
        /// Số chữ số phần thập phân của đơn giá nguyên tệ, mặc định = 0
        /// </summary>
        public string UnitPriceOCDecimalDigits { get; set; }

        /// <summary>
        /// Số chữ số phần thập phân của đơn giá quy đổi, mặc định = 0
        /// </summary>
        public string UnitPriceDecimalDigits { get; set; }

        /// <summary>
        /// Số chữ số phần thập phân của số lượng, mặc định = 0
        /// </summary>
        public string QuantityDecimalDigits { get; set; }

        /// <summary>
        /// Số chữ số phần thập phân của hệ số/tỷ lệ, mặc định = 0
        /// </summary>
        public string CoefficientDecimalDigits { get; set; }

        /// <summary>
        /// Số chữ số phần thập phân của tỷ giá, mặc định = 0
        /// </summary>
        public string ExchangRateDecimalDigits { get; set; }
    }
}

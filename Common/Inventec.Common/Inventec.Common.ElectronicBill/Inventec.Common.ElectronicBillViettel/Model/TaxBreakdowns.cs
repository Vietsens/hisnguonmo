/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 *  
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *  
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 *  
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.ElectronicBillViettel.Model
{
    /// <summary>
    /// Dùng để tổng hợp thuế suất theo mức cho hóa đơn.
    /// </summary>
    [Serializable]
    public class TaxBreakdowns
    {
        /// <summary>
        /// Mức thuế: khai báo giá trị như sau 
        /// -2: không thuế
        /// -1: không kê khai tính/nộp thuế 
        /// 0: 0%
        /// 5: 5%
        /// 10: 10%
        /// </summary>
        public long? taxPercentage { get; set; }

        /// <summary>
        /// Tổng tiền chịu thuế của mức thuế tương ứng, tổng tiền chịu thuế không có số âm.
        /// Bằng tổng của itemTotalAmountWithoutTax của tất cả các itemInfo có mức thuế suất giống với mức thuế suất tổng hợp.
        /// Trong trường hợp dòng hàng hóa là chiết khấu thì trừ đi.
        /// </summary>
        public decimal? taxableAmount { get; set; }

        /// <summary>
        /// Tổng tiền thuế của mức thuế tương ứng, tổng tiền thuế không có số âm.
        /// Bằng tổng của taxAmount của tất cả các itemInfo có mức thuế suất giống với mức thuế suất tổng hợp.
        /// Trong trường hợp dòng hàng hóa là chiết khấu thì trừ đi.
        /// </summary>
        public decimal? taxAmount { get; set; }

        /// <summary>
        /// Dùng để biểu thị tổng tiền chịu thuế của mức thuế là âm hay dương.
        /// - null/true: Tổng tiền đánh thuế dương. Được sử dụng đối với các hàng hóa thông thường. 
        /// - false: Tổng tiền đánh thuế âm, được sử dụng với hóa đơn điều chỉnh giảm hoặc hóa đơn có hàng hóa là chiết khấu mà tổng tiền của hàng hóa và chiết khấu của mức thuế là âm
        /// </summary>
        public bool? taxableAmountPos { get; set; }

        /// <summary>
        /// Dùng để biểu thị tổng tiền thuế của mức thuế là âm hay dương. Giá trị của taxAmountPos luôn giống với giá trị của taxableAmountPos. 
        /// - null/true: Tổng tiền thuế dương 
        /// - false: Tổng tiền thuế âm
        /// </summary>
        public bool? taxAmountPos { get; set; }

        /// <summary>
        /// Lý do miễn giảm thuế
        /// Maxlength: 255
        /// </summary>
        public string taxExemptionReason { get; set; }
    }
}

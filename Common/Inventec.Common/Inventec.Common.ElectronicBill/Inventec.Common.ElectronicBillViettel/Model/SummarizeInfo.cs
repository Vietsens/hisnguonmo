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
    /// Dùng để tổng hợp tiền hàng cho toàn bộ hóa đơn.
    /// </summary>
    /// 
    [Serializable]
    public class SummarizeInfo
    {
        /// <summary>
        /// Tổng thành tiền itemTotalAmountWithoutTax cộng gộp của tất cả các dòng hóa đơn chưa bao gồm VAT. Tổng thành tiền không có số âm
        /// - Hóa đơn thường: Tổng tiền HHDV trên các dòng HĐ. 
        /// - Hóa đơn điều chỉnh: Tổng tiền điều chỉnh của các dòng HĐ.
        /// Maxlength: 13
        /// </summary>
        public decimal sumOfTotalLineAmountWithoutTax { get; set; }

        /// <summary>
        /// Tổng tiền hóa đơn chưa bao gồm VAT (đã tính chiết khấu (nếu có)). Tổng tiền hóa đơn không có số âm.
        /// - Hóa đơn thường: Tổng tiền HHDV trên các dòng HĐ và các khoản tăng/giảm khác trên toàn HĐ. 
        /// - Hóa đơn điều chỉnh: Tổng tiền điều chỉnh của các dòng HĐ và các khoản tăng/giảm khác trên toàn HĐ.
        /// Maxlength: 15
        /// </summary>
        public decimal totalAmountWithoutTax { get; set; }

        /// <summary>
        /// Tổng tiền thuế trên toàn hóa đơn. Tổng tiền thuế không có số âm.
        /// - Hóa đơn thường: Tổng tiền VAT trên các dòng HĐ và các khoản thuế khác trên toàn HĐ.
        /// - Hóa đơn điều chỉnh: Tổng tiền VAT điều chỉnh của các dòng HĐ và các khoản tăng/giảm VAT khác trên toàn HĐ.
        /// Maxlength: 13
        /// </summary>
        public decimal? totalTaxAmount { get; set; }

        /// <summary>
        /// Tổng tiền trên hóa đơn đã bao gồm VAT. Tổng tiền sau thuế không có số âm.
        /// - Hóa đơn thường: Tổng tiền HHDV trên các dòng HĐ và các khoản tăng/giảm khác trên toàn HĐ đã bao gồm cả VAT.
        /// - Hóa đơn điều chỉnh: Tổng tiền điều chỉnh của các dòng HĐ và các khoản tăng/giảm khác trên toàn HĐ đã bao gồm cả VAT
        /// Maxlength: 13
        /// </summary>
        public decimal totalAmountWithTax { get; set; }

        /// <summary>
        /// Tổng tiền ngoại tệ của hóa đơn đã bao gồm VAT. Dùng trong trường hợp hóa đơn không chọn loại tiền là VND
        /// - Hóa đơn thường: Tổng tiền HHDV trên các dòng HĐ và các khoản tăng/giảm khác trên toàn HĐ đã bao gồm cả VAT.
        /// - Hóa đơn điều chỉnh: Tổng tiền điều chỉnh của các dòng HĐ và các khoản tăng/giảm khác trên toàn HĐ đã bao gồm cả VAT
        /// Maxlength: 13
        /// </summary>
        public decimal? totalAmountWithTaxFrn { get; set; }

        /// <summary>
        /// Số tiền hóa đơn bao gồm VAT viết bằng chữ. Hệ thống hóa đơn điện tử sẽ tự động sinh lại dữ liệu này để đảm bảo đúng theo dữ liệu hệ thống.
        /// Maxlength: 255
        /// </summary>
        public string totalAmountWithTaxInWords { get; set; }

        /// <summary>
        /// Để đánh dấu tổng tiền sau thuế là âm hay dương
        /// - null/True: Tổng tiền là số dương, sử dụng đối với các hóa đơn thông thường hoặc hóa đơn có chiết khấu nhưng tổng thuế vẫn là dương sau khi trừ chiết khấu.
        /// - False: Tổng tiền âm, sử dụng đối với hóa đơn điều chỉnh giảm hoặc có chiết khấu mà tiền chiết khấu lớn hơn tiền hàng hóa thông thường.
        /// </summary>
        public bool? isTotalAmountPos { get; set; }

        /// <summary>
        /// Để đánh dấu tổng tiền thuế là âm hay dương
        /// - null/true: tổng tiền thuế là dương
        /// - false: tổng tiền thuế là âm
        /// </summary>
        public bool? isTotalTaxAmountPos { get; set; }

        /// <summary>
        /// Để đánh dấu tổng tiền trước thuế là âm hay dương
        /// - null/true: tổng tiền trước thuế là dương
        /// - false: tổng tiền trước thuế là âm
        /// </summary>
        public bool? isTotalAmtWithoutTaxPos { get; set; }

        /// <summary>
        /// Tổng tiền chiết khấu thương mại trên toàn hóa đơn trước khi tính thuế. Chú ý: Khi tính chiết khấu, toàn hóa đơn chỉ sử dụng một mức thuế.
        /// Maxlength: 13
        /// </summary>
        public decimal discountAmount { get; set; }

        /// <summary>
        /// Tổng tiền chiết khấu thanh toán trên toàn hóa đơn sau khi tính thuế. Chú ý: Khi tính chiết khấu, toàn hóa đơn chỉ sử dụng một mức thuế. 
        /// Maxlength: 13
        /// </summary>
        public decimal settlementDiscountAmount { get; set; }

        /// <summary>
        /// Trường nhận biết tổng tiền chiết khấu là số dương hay âm
        /// - null/true: tổng tiền trước thuế là dương
        /// - false: tổng tiền trước thuế là âm
        /// </summary>
        public bool? isDiscountAmtPos { get; set; }
        /// <summary>
        /// Tổng tiền sau khi chiết khấu. Tổng tiền chưa thuế của chi tiết dịch vụ(amountWithoutTax) trừ đi tổng tiền miễn giảm(discountAmount)
        /// </summary>
        public decimal? totalAmountAfterDiscount { get; set; }
    }
}

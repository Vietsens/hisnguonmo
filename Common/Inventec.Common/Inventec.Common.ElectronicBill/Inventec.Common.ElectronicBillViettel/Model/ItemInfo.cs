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
    /// Là một danh sách các hàng hóa/dịch vụ được hiển thị trên hóa đơn
    /// </summary>
    [Serializable]
    public class ItemInfo
    {
        /// <summary>
        /// Thứ tự dòng hóa đơn, bắt đầu từ 1
        /// Maxlength: 5
        /// </summary>
        public int? lineNumber { get; set; }

        /// <summary>
        /// Đánh dấu loại hàng hóa/dịch vụ
        /// null hoặc 1- Hàng Hóa
        /// 2: Ghi chú
        /// 3: Chiết khấu
        /// 4: Bảng kê
        /// 5: Phí khác.
        /// Maxlength: 1
        /// </summary>
        public int? selection { get; set; }

        /// <summary>
        /// Mã hàng hóa, dịch vụ
        /// Maxlength: 50
        /// </summary>
        public string itemCode { get; set; }

        /// <summary>
        /// Tên hàng hóa, dịch vụ
        /// Maxlength: 300
        /// Bắt buộc
        /// </summary>
        public string itemName { get; set; }

        /// <summary>
        /// Mã đơn vị tính
        /// Maxlength: 10
        /// </summary>
        public string unitCode { get; set; }

        /// <summary>
        /// Tên đơn vị tính hàng hóa, dịch vụ
        /// Maxlength: 50
        /// </summary>
        public string unitName { get; set; }

        /// <summary>
        /// Đơn giá của hàng hóa, không có số âm. Trong trường hợp không muốn hiển thị lên hóa đơn thì không truyền sang (truyền null sang).
        /// Maxlength: 13
        /// Bắt buộc khi selection = null/1/5
        /// </summary>
        public decimal? unitPrice { get; set; }

        /// <summary>
        /// Số lượng của hàng hóa, luôn là số dương. Trong trường hợp không muốn hiển thị lên hóa đơn thì không truyền sang (truyền null sang).
        /// Maxlength: 13
        /// Bắt buộc khi selection = null/1/5
        /// </summary>
        public decimal? quantity { get; set; }

        /// <summary>
        /// Là tổng tiền chưa bao gồm VAT của hàng hóa/dịch vụ. Tổng tiền không có số âm. itemTotalAmountWithoutTax = (1 - discount) *quantity * unitPrice
        /// Maxlength: 13
        /// Bắt buộc khi selection = null/1/3/4/5
        /// </summary>
        public decimal? itemTotalAmountWithoutTax { get; set; }

        /// <summary>
        /// Trong trường hợp thuế tổng/hóa đơn bán hàng (theo chuẩn hóa đơn xác thực là phải có)
        /// 0: 0% / 5: 5% / 10: 10%
        /// Maxlength: 13
        /// </summary>
        public long? taxPercentage { get; set; }

        /// <summary>
        /// Tổng tiền thuế, không có số âm
        /// Maxlength: 13
        /// </summary>
        public decimal? taxAmount { get; set; }

        /// <summary>
        /// Dùng để biểu thị tiền trên hóa đơn là số âm trong trường hợp hàng hóa là chiết khấu, hoặc dòng hàng hóa biểu thị điều chỉnh giảm tiền hàng.
        /// Maxlength: 20
        /// Hóa đơn bình thường: có giá trị là null.
        /// Hóa đơn điều chỉnh:
        /// - false: dòng hàng hóa/dịch vụ bị điều chỉnh giảm 
        /// - true: dòng hàng hóa/dịch vụ bị điều chỉnh tăng
        /// </summary>
        public bool? isIncreaseItem { get; set; }

        /// <summary>
        /// Ghi chú cho từng dòng hàng hóa
        /// Maxlength: 50
        /// </summary>
        public string itemNote { get; set; }

        /// <summary>
        /// Số lô, thường dùng cho các hàng hóa là thuốc, có thể sử dụng để hiển thị thêm thông tin trong trường hợp cần thiết.
        /// Maxlength: 50
        /// </summary>
        public string batchNo { get; set; }

        /// <summary>
        /// Hạn sử dụng của hàng hóa, thường dùng cho các hàng hóa là thuốc, có thể sử dụng để hiển thị thêm thông tin trong trường hợp cần thiết.
        /// Maxlength: 50
        /// </summary>
        public string expDate { get; set; }

        /// <summary>
        /// % chiết khấu trên dòng sản phẩm, tính trên đơn giá của sản phẩm. Trong trường hợp không có thì truyền null
        /// Maxlength: 13
        /// </summary>
        public decimal? discount { get; set; }

        /// <summary>
        /// Giá trị chiết khấu trên dòng sản phẩm, sau khi nhân với số lượng và % chiết khấu
        /// Maxlength: 13
        /// </summary>
        public decimal? itemDiscount { get; set; }

        /// <summary>
        /// Là tổng tiền đã bao gồm VAT của hàng hóa/dịch vụ. Tổng tiền không có số âm.
        /// Maxlength: 13
        /// </summary>
        public decimal? itemTotalAmountWithTax { get; set; }

        /// <summary>
        /// Là tổng tiền sau khi trừ chiết khấu, giảm giá.
        /// Tổng tiền không có số âm.
        /// </summary>
        public decimal? itemTotalAmountAfterDiscount { get; set; }
    }
}

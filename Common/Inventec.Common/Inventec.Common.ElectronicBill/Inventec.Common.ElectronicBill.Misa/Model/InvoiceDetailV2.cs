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

namespace Inventec.Common.ElectronicBill.Misa.Model
{
    public class InvoiceDetailV2
    {
        /// <summary>
        /// Tính chất (1: HHDV; 2: khuyến mại; 3: chiết khẩu; 4: ghi chú/diễn giải)
        /// </summary>
        public int ItemType { get; set; }

        /// <summary>
        /// Số thứ tự dòng mặt hàng, bắt đầu từ 1.
        /// </summary>
        public int LineNumber { get; set; }

        /// <summary>
        /// Mã mặt hàng
        /// </summary>
        public string ItemCode { get; set; }

        /// <summary>
        /// Tên mặt hàng
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// Đơn vị tính
        /// </summary>
        public string UnitName { get; set; }

        /// <summary>
        /// Số lượng mặt hàng
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// Đơn giá mặt hàng không bao gồm thuế
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// Tỷ lệ chiết khấu
        /// </summary>
        public decimal? DiscountRate { get; set; }

        /// <summary>
        /// Tiền chiết khấu
        /// </summary>
        public decimal? DiscountAmountOC { get; set; }

        /// <summary>
        /// Tiền chiết khấu quy đổi
        /// </summary>
        public decimal? DiscountAmount { get; set; }

        /// <summary>
        /// Thành tiền
        /// </summary>
        public decimal? AmountOC { get; set; }

        /// <summary>
        /// Tiền hàng nguyên tệ không bao gồm thuế
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Thành tiền chưa thuế
        /// </summary>
        public decimal? AmountWithoutVATOC { get; set; }

        /// <summary>
        /// Đơn giá sau thuế
        /// </summary>
        public decimal? UnitPriceAfterTax { get; set; }

        /// <summary>
        /// Thành tiền sau thuế quy đổi
        /// </summary>
        public decimal? AmountAfterTax { get; set; }

        /// <summary>
        /// Tên loại thuế suất
        /// </summary>
        public string VATRateName { get; set; }

        /// <summary>
        /// Tiền thuế
        /// </summary>
        public decimal? VATAmountOC { get; set; }

        ///// <summary>
        ///// Mức thuế GTGT.
        ///// - 0: 0%
        ///// - 5: 5%
        ///// - 10: 10%
        ///// - -1: Không chịu thuế
        ///// </summary>
        //public decimal VatPercentage { get; set; }

        /// <summary>
        /// Tiền thuế GTGT
        /// </summary>
        public decimal VatAmount { get; set; }

        /// <summary>
        /// Thứ tự hiển thị lên mẫu
        /// </summary>
        public int? SortOrder { get; set; }

        /// <summary>
        /// Diễn giải hàng hóa
        /// </summary>
        public string InventoryItemNote { get; set; }

        /// <summary>
        /// Loại hàng hóa:
        /// 0: Hàng hóa vật tư
        /// 1: Hàng hóa thành phẩm
        /// 2: Hàng hóa dịch vụ
        /// 3: Đánh dấu là dòng diễn giải
        /// 4: Đánh dấu là dòng chiết khấu
        /// </summary>
        //public int InventoryItemType { get; set; }

        ///// <summary>
        ///// Có phải là hàng khuyến mại hay không.
        ///// 0: Không phải là hàng khuyến mại
        ///// 1: Là hàng khuyến mại
        ///// </summary>
        //public bool Promotion { get; set; }
    }
}

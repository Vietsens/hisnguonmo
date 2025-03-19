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
    /// Dữ liệu đầu vào đối với các API lập hóa đơn, điều chỉnh hóa đơn, thay thế hóa đơn, lập hóa đơn nháp, lập hóa đơn usb token, xem trước hóa đơn nháp các trường dữ liệu truyền vào sẽ có dạng chung
    /// </summary>
    [Serializable]
    public class DataCreateInvoice
    {
        /// <summary>
        /// Thông tin chung của hóa đơn
        /// </summary>
        public GeneralInvoiceInfo generalInvoiceInfo { get; set; }

        /// <summary>
        /// Thông tin người mua
        /// </summary>
        public BuyerInfo buyerInfo { get; set; }

        /// <summary>
        /// Thông tin người bán
        /// </summary>
        public SellerInfo sellerInfo { get; set; }

        /// <summary>
        /// thông tin thanh toán
        /// </summary>
        public List<Payments> payments { get; set; }

        /// <summary>
        /// thông tin hàng hóa
        /// </summary>
        public List<ItemInfo> itemInfo { get; set; }

        /// <summary>
        /// thông tin tổng hợp tiền của hóa đơn
        /// </summary>
        public SummarizeInfo summarizeInfo { get; set; }

        /// <summary>
        /// thông tin gom nhóm tiền hóa đơn theo thuế suất
        /// </summary>
        public List<TaxBreakdowns> taxBreakdowns { get; set; }

        /// <summary>
        /// thông tin trường động
        /// </summary>
        public List<Metadata> metadata { get; set; }
    }
}

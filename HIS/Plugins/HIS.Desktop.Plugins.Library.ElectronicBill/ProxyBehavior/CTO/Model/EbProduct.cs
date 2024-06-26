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

namespace HIS.Desktop.Plugins.Library.ElectronicBill.ProxyBehavior.CTO.Model
{
    class EbProduct
    {
        /// <summary>
        /// Tên hàng hóa, dịch vụ
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Tên đơn vị tính hàng hóa, dịch vụ
        /// </summary>
        public string unit { get; set; }

        /// <summary>
        /// Số lượng của hàng hóa, luôn là số dương
        /// </summary>
        public double quantity { get; set; }

        /// <summary>
        /// Đơn giá của hàng hóa, không có số âm
        /// </summary>
        public decimal price { get; set; }

        /// <summary>
        /// Là tổng tiền chưa bao gồm VAT của hàng hóa/dịch vụ
        /// </summary>
        public decimal amount { get; set; }

        /// <summary>
        /// Thuế suất của hàng hóa, dịch vụ
        /// </summary>
        public int tax { get; set; }

        /// <summary>
        /// Là tổng tiền đã bao gồm VAT của hàng hóa/dịch vụ. Tổng tiền không có số âm
        /// </summary>
        public decimal total { get; set; }
    }
}

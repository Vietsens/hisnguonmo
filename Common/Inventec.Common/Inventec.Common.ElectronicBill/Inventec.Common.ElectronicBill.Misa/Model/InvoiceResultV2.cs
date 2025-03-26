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
    public class InvoiceResultV2
    {
        /// <summary>
        /// ID của hóa đơn gốc
        /// </summary>
        public string RefID { get; set; }

        /// <summary>
        /// Mã tra cứu hóa đơn
        /// </summary>
        public string TransactionID { get; set; }

        /// <summary>
        /// Số hóa đơn
        /// </summary>
        public string InvNo { get; set; }

        /// <summary>
        /// Ngày hóa đơn
        /// </summary>
        public DateTime? InvDate { get; set; }

        /// <summary>
        /// Nội dung hóa đơn (định dạng XML)
        /// </summary>
        public string InvoiceData { get; set; }

        /// <summary>
        /// Dữ liệu file PDF
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// Mã lỗi trả về nếu việc chuyển hóa đơn thành định dạng hóa đơn điện tử không thành công.
        /// </summary>
        public string ErrorCode { get; set; }
    }
}

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
    public class DeleteInvoiceData
    {
        /// <summary>
        /// Mã tra cứu của hóa đơn bị xóa bỏ
        /// </summary>
        public string TransactionID { get; set; }

        /// <summary>
        /// Ngày chứng từ xóa bỏ hóa đơn
        /// </summary>
        public DateTime RefDate { get; set; }

        /// <summary>
        /// Số chứng từ xóa bỏ hóa đơn
        /// </summary>
        public string RefNo { get; set; }

        /// <summary>
        /// Lý do xóa bỏ
        /// </summary>
        public string DeletedReason { get; set; }

        /// <summary>
        /// Ngày của biên bản thỏa thuận xóa bỏ hóa đơn giữa người mua và người bán
        /// </summary>
        public string MinutesDate { get; set; }

        /// <summary>
        /// Số của biên bản thỏa thuận xóa bỏ hóa đơn giữa người mua và người bán
        /// </summary>
        public string MinutesNo { get; set; }

        /// <summary>
        /// Lý do ghi trên biên bản thỏa thuận xóa bỏ hóa đơn giữa người mua và người bán
        /// </summary>
        public string MinutesReason { get; set; }

        /// <summary>
        /// Tên tệp biên bản thỏa thuận
        /// </summary>
        public string MinutesFileName { get; set; }

        /// <summary>
        /// Nội dung biên bản thỏa thuận đã encode base64
        /// </summary>
        public string MinutesFileContent { get; set; }
    }
}

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
    public class ReleaseInvoiceData
    {
        /// <summary>
        /// ID của hóa đơn trên Client App
        /// </summary>
        public string RefID { get; set; }

        /// <summary>
        /// Mã tra cứu của hóa đơn
        /// </summary>
        public string TransactionID { get; set; }

        /// <summary>
        /// Nội dung hóa đơn đã được ký điện tử
        /// SignResult.PayLoad
        /// </summary>
        public string InvoiceData { get; set; }

        /// <summary>
        /// Có gửi Email sau khi phát hành hay không
        /// </summary>
        public bool IsSendEmail { get; set; }

        /// <summary>
        /// Tên người nhận Email.Bắt buộc khi IsSendEmail = true
        /// </summary>
        public string ReceiverName { get; set; }

        /// <summary>
        /// Danh sách các Email nhận cách nhau bởi dấu ;
        /// Bắt buộc khi IsSendEmail = true
        /// </summary>
        public string ReceiverEmail { get; set; }
    }
}

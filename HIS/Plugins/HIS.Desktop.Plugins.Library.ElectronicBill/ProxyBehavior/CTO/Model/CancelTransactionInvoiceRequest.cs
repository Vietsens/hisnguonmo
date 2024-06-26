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
    class CancelTransactionInvoiceRequest
    {
        /// <summary>
        /// Mã duy nhất từ hệ thống tích hợp để xác định hóa đơn, chính là id trong phát hành hóa đơn
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// Mã số thuế của doanh nghiệp/chi nhánh phát hành hóa đơn.
        /// </summary>
        public string supplierTaxCode { get; set; }

        /// <summary>
        /// Số hóa đơn, bao gồm ký hiệu hóa đơn và số thứ tự hóa đơn
        /// </summary>
        public string invoiceNo { get; set; }

        /// <summary>
        /// Mã mẫu hóa đơn
        /// </summary>
        public string templateCode { get; set; }

        /// <summary>
        /// Ngày lập hóa đơn, chính là ngày issued_date trong phát hành hóa đơn.
        /// Định dạng ngày: yyyyMMddHHmmss
        /// </summary>
        public string strIssueDate { get; set; }

        /// <summary>
        /// Tên văn bản thỏa thuận hủy hóa đơn
        /// </summary>
        public string additionalReferenceDesc { get; set; }

        /// <summary>
        /// Ngày thỏa thuận (không vượt quá ngày hiện tại).
        /// Định dạng ngày: yyyyMMddHHmmss
        /// </summary>
        public string additionalReferenceDate { get; set; }
    }
}

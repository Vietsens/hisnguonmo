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
    public class GetInvoiceRepresentationFileData
    {
        /// <summary>
        /// Mã số thuế của doanh nghiệp
        /// Bắt buộc
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
        /// Chuỗi kiểm tra dữ liệu (fkey) được truyền vào khi lập hóa đơn
        /// </summary>
        public string transactionUuid { get; set; }

        /// <summary>
        /// Loại file muốn tải về, các định dạng được phép ZIP, PDF
        /// </summary>
        public string fileType { get; set; }

        /// <summary>
        /// true – Đã thanh toán
        /// false – Chưa thanh toán
        /// </summary>
        public bool? paid { get; set; }
    }
}

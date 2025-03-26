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
    public class ResponseResult
    {
        /// <summary>
        /// Số hóa đơn ví dụ: AA/16E0000001 
        /// </summary>
        public string invoiceNo { get; set; }

        /// <summary>
        /// Mã số bí mật dùng để tra khách hàng tra cứu
        /// </summary>
        public string reservationCode { get; set; }

        /// <summary>
        /// Mã số thuế người bán (doanh nghiệp phát hành hóa đơn)
        /// </summary>
        public string supplierTaxCode { get; set; }

        /// <summary>
        /// Id của giao dịch
        /// </summary>
        public string transactionID { get; set; }

        /// <summary>
        /// thời gian giao dịch
        /// milliseconds
        /// 1587797116843
        /// </summary>
        public double issueDate { get; set; }

        /// <summary>
        /// trạng thái
        /// </summary>
        public string status { get; set; }
    }
}

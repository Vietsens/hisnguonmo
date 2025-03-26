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

namespace Inventec.Common.EHoaDon
{
    [Serializable]
    public class InvoiceResult
    {
        public long PartnerInvoiceID { get; set; }
        public string PartnerInvoiceStringID { get; set; }

        public Guid InvoiceGUID { get; set; }

        public string InvoiceForm { get; set; }
        public string InvoiceSerial { get; set; }
        public int InvoiceNo { get; set; }

        /// <summary>
        /// Trạng thái xử lý: 0 - thêm mới thành công, 1 - lỗi
        /// </summary>
        public int Status { get; set; }
        public string MessLog { get; set; }

        /// <summary>
        /// Mã tra cứu
        /// </summary>
        public string MTC { get; set; }
    }
}

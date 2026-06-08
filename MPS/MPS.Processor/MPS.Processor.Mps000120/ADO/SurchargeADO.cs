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

namespace MPS.Processor.Mps000120.ADO
{
    /// <summary>
    /// PTTK 2656 - mục 4.2.8: Dòng phụ phí in thêm sau danh sách dịch vụ trên bảng kê.
    /// Mỗi dòng = 1 bản ghi HIS_TRANSACTION_PAYFORM có SURCHARGE_AMOUNT > 0.
    /// Bind vào template qua region "Surcharge".
    /// </summary>
    public class SurchargeADO
    {
        /// <summary>Số thứ tự dòng phụ phí</summary>
        public int STT { get; set; }

        /// <summary>Tên phụ phí (snapshot SURCHARGE_NAME) - in cột Tên</summary>
        public string SURCHARGE_NAME { get; set; }

        /// <summary>Số lượng - luôn = 1 theo PTTK</summary>
        public decimal AMOUNT { get; set; }

        /// <summary>Thành tiền phụ phí (SURCHARGE_AMOUNT)</summary>
        public decimal SURCHARGE_AMOUNT { get; set; }

        /// <summary>Thứ tự hiển thị gốc</summary>
        public long? SORT_ORDER { get; set; }
    }
}

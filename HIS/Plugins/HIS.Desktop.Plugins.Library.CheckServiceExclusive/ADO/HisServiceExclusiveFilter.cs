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

namespace HIS.Desktop.Plugins.Library.CheckServiceExclusive.ADO
{
    /// <summary>
    /// Filter local cho api/HisServiceExclusive/Get|GetView (viec 57452).
    /// Chua co trong MOS.Filter.dll dang ban giao; khi backend cap nhat thi thay bang
    /// MOS.Filter.HisServiceExclusiveFilter / HisServiceExclusiveViewFilter.
    /// Ten truong bam theo mau HisServiceSameFilter de backend nhan dien duoc.
    /// </summary>
    public class HisServiceExclusiveFilter
    {
        public long? ID { get; set; }
        public List<long> IDs { get; set; }
        public short? IS_ACTIVE { get; set; }

        public long? SERVICE_ID { get; set; }
        public List<long> SERVICE_IDs { get; set; }
        public long? EXCLUSIVE_ID { get; set; }
        public List<long> EXCLUSIVE_IDs { get; set; }

        /// <summary>Tra 2 chieu: lay moi ban ghi co SERVICE_ID hoac EXCLUSIVE_ID bang gia tri nay</summary>
        public long? SERVICE_ID__OR__EXCLUSIVE_ID { get; set; }

        /// <summary>Tra 2 chieu theo danh sach</summary>
        public List<long> SERVICE_ID__OR__EXCLUSIVE_IDs { get; set; }

        public string KEY_WORD { get; set; }
        public string ORDER_FIELD { get; set; }
        public string ORDER_DIRECTION { get; set; }
    }
}

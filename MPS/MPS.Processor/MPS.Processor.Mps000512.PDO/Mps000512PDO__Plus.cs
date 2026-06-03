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
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MPS.Processor.Mps000512.PDO
{
    /// <summary>
    /// Kiểu gom nhóm dịch vụ. Thay vì tách hẳn 1 MPS riêng cho "bảng kê theo khoa",
    /// MPS này gói cả 3 trường hợp vào 1 cờ cấu hình (giống Mps000510).
    /// </summary>
    public enum GroupServiceType
    {
        /// <summary>Bảng kê tổng hợp thường (không gom theo khoa/phòng) - như Mps000302.</summary>
        None = 0,
        /// <summary>Gom nhóm theo khoa xử lý.</summary>
        Department = 1,
        /// <summary>Gom nhóm theo phòng xử lý.</summary>
        Room = 2
    }

    public partial class Mps000512PDO : RDOBase
    {
        /// <summary>Cách gom nhóm hiển thị (template group theo cột GROUP_*). Mặc định None = hành vi Mps000302.</summary>
        public GroupServiceType GroupType { get; set; }

        /// <summary>
        /// Nếu có giá trị: chỉ lấy dịch vụ thuộc khoa xử lý này (lọc bảng kê theo 1 khoa).
        /// Để null = lấy toàn bộ (bảng kê tổng hợp).
        /// </summary>
        public long? FilterDepartmentId { get; set; }
    }
}

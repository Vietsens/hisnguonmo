/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 */
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MPS.Processor.Mps000510.PDO
{
    /// <summary>
    /// Kiểu gom nhóm dịch vụ. Thay vì tách hẳn 1 MPS riêng như Mps000306,
    /// MPS này gói cả 3 trường hợp vào 1 cờ cấu hình.
    /// </summary>
    public enum GroupServiceType
    {
        /// <summary>Bảng kê thường (không gom theo khoa/phòng) - như Mps000281.</summary>
        None = 0,
        /// <summary>Gom nhóm theo khoa xử lý.</summary>
        Department = 1,
        /// <summary>Gom nhóm theo phòng xử lý.</summary>
        Room = 2
    }

    public partial class Mps000510PDO : RDOBase
    {
        /// <summary>Cách gom nhóm hiển thị (template group theo cột GROUP_*). Mặc định None.</summary>
        public GroupServiceType GroupType { get; set; }

        /// <summary>
        /// Nếu có giá trị: chỉ lấy dịch vụ thuộc khoa xử lý này (lọc bảng kê theo 1 khoa).
        /// Để null = lấy toàn bộ.
        /// </summary>
        public long? FilterDepartmentId { get; set; }
    }
}

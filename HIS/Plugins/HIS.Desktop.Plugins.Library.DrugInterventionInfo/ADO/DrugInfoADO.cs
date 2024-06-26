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

namespace HIS.Desktop.Plugins.Library.DrugInterventionInfo.ADO
{
    class DrugInfoADO
    {
        /// <summary>
        /// Mã định danh của thuốc
        /// Không truyền vào
        /// </summary>
        public int? medicationId { get; set; }

        /// <summary>
        /// Tên thuốc
        /// </summary>
        public string medicationName { get; set; }

        /// <summary>
        /// Tổng số đề nghị theo đơn vị đóng gói
        /// </summary>
        public decimal? totalQuantity { get; set; }

        /// <summary>
        /// Chỉ định đường dùng
        /// </summary>
        public string drugRoute { get; set; }

        /// <summary>
        /// Ngày bắt đầu uống thuốc
        /// </summary>
        public DateTime? prescribeDate { get; set; }

        // sáng uống 1 viên tối uống 2 viên
        // dua sang ngay 2 lan, moi lan 2 vien

        /// <summary>
        /// Số lần mỗi ngày
        /// </summary>
        public decimal? timePerDay { get; set; }

        /// <summary>
        /// Số đơn vị thuốc mỗi lần
        /// </summary>
        public decimal? quantityPerTime { get; set; }

        /// <summary>
        /// Ghi chú của bác sĩ
        /// </summary>
        public string note { get; set; }

        /// <summary>
        /// Đơn vị sử dụng
        /// Đơn vị của loại thuốc ml
        /// </summary>
        public string usageUnit { get; set; }

        /// <summary>
        /// Đơn vị đóng gói
        /// Đơn vị phát cho bệnh nhân - chai/lọ
        /// </summary>
        public string packageUnit { get; set; }

        /// <summary>
        /// Thứ tự hiển thị trong toa
        /// </summary>
        public int index { get; set; }
    }
}

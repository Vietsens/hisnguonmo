/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
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

namespace HIS.Desktop.Plugins.MedicineMediStockSummaryVertical.ADO
{
    /// <summary>
    /// Đối tượng bind 1 dòng tồn kho toàn viện (theo chiều dọc) lên GridControl.
    /// Tùy chế độ lọc thời gian mà hiển thị cột "Tồn hiện tại" hoặc bộ cột đầu kỳ/xuất/cuối kỳ.
    /// </summary>
    public class MediStockSummaryVerticalADO
    {
        /// <summary>Số thứ tự</summary>
        public int Stt { get; set; }

        /// <summary>Mã kho</summary>
        public string MediStockCode { get; set; }

        /// <summary>Tên kho</summary>
        public string MediStockName { get; set; }

        /// <summary>Tồn hiện tại (khi không chọn khoảng thời gian)</summary>
        public decimal? Amount { get; set; }

        /// <summary>Tồn đầu kỳ (khi chọn khoảng thời gian)</summary>
        public decimal? BeginAmount { get; set; }

        /// <summary>Số lượng nhập trong kỳ (khi chọn khoảng thời gian)</summary>
        public decimal? InAmount { get; set; }

        /// <summary>Số lượng xuất trong kỳ (khi chọn khoảng thời gian)</summary>
        public decimal? ExportAmount { get; set; }

        /// <summary>Tồn cuối kỳ (khi chọn khoảng thời gian)</summary>
        public decimal? EndAmount { get; set; }
    }
}

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

namespace HIS.Desktop.Plugins.ExpMestAggregate.ADO
{
    /// <summary>
    /// ADO cho combo lọc "Ca chạy thận" (KIDNEY_SHIFT) ở màn Tổng hợp phiếu lĩnh.
    /// SHIFT_VALUE = giá trị ca (1..5) — map vào HisExpMestViewFilter.KIDNEY_SHIFT.
    /// </summary>
    public class KidneyShiftADO
    {
        /// <summary>Giá trị ca chạy thận (1..5)</summary>
        public long SHIFT_VALUE { get; set; }

        /// <summary>Tên hiển thị: "Ca 1".."Ca 5"</summary>
        public string SHIFT_NAME { get; set; }
    }
}

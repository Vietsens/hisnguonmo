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

namespace HIS.Desktop.Plugins.AssignService.ADO
{
    /// <summary>
    /// Một loại bảng kê có thể in qua thư viện PrintBordereau (hiển thị trong popup tích chọn).
    /// </summary>
    public class BangKeInADO
    {
        /// <summary>Mã Mps của bảng kê (Tag lấy từ menu của PrintBordereau) - dùng để gọi Print.</summary>
        public string PrintTypeCode { get; set; }
        /// <summary>Tên hiển thị của bảng kê.</summary>
        public string Name { get; set; }
        /// <summary>Người dùng có tích chọn in bảng kê này hay không.</summary>
        public bool Check { get; set; }

        public BangKeInADO() { }

        public BangKeInADO(string printTypeCode, string name, bool check = false)
        {
            this.PrintTypeCode = printTypeCode;
            this.Name = name;
            this.Check = check;
        }
    }
}

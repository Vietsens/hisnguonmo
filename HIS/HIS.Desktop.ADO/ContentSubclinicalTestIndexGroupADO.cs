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

namespace HIS.Desktop.ADO
{
    /// <summary>
    /// Đối tượng trả về thông tin chỉ số xét nghiệm theo nhóm chỉ số.
    /// Dùng cho luồng tự động lấy kết quả xét nghiệm (auto test index) giữa
    /// ContentSubclinical và các plugin gọi đến (vd EnterKskInfomantionVer2).
    /// </summary>
    public class ContentSubclinicalTestIndexGroupADO
    {
        /// <summary>Mã nhóm chỉ số xét nghiệm (HIS_TEST_INDEX_GROUP.TEST_INDEX_GROUP_CODE).</summary>
        public string TEST_INDEX_GROUP_CODE { get; set; }

        /// <summary>Giá trị chỉ số đã được format (giống logic VALUE hiện tại của ContentSubclinicalADO).</summary>
        public string VALUE { get; set; }

        /// <summary>Mã chỉ số xét nghiệm gốc (tham chiếu, hỗ trợ debug/đối chiếu).</summary>
        public string TEST_INDEX_CODE { get; set; }

        /// <summary>Tên chỉ số xét nghiệm gốc (tham chiếu, hỗ trợ debug/đối chiếu).</summary>
        public string TEST_INDEX_NAME { get; set; }
    }
}

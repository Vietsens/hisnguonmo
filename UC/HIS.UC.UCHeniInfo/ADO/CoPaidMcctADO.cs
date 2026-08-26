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

namespace HIS.UC.UCHeniInfo.ADO
{
    /// <summary>
    /// Ba giá trị suy ra từ một lượt tra cứu cùng chi trả trên cổng BHXH,
    /// kèm ngữ cảnh cần thiết để giải thích cho người dùng.
    /// </summary>
    public class CoPaidMcctADO
    {
        public CoPaidMcctADO() { }

        /// <summary>
        /// True khi cổng trả về ít nhất một bản ghi và suy được số tiền lũy kế.
        /// False nghĩa là: giữ nguyên mọi giá trị trên form.
        /// </summary>
        public bool HasAccumulate { get; set; }

        /// <summary>
        /// True khi HIS_BHYT_PARAM cung cấp được lương cơ sở hợp lệ, tức đã xét được
        /// ngưỡng 06 tháng. False nghĩa là: chỉ điền số tiền lũy kế.
        /// </summary>
        public bool HasThreshold { get; set; }

        /// <summary>Tiền cùng chi trả lũy kế trong năm tài chính, đã làm tròn.</summary>
        public long CoPaidAccumulateAmount { get; set; }

        /// <summary>Số lũy kế lớn hơn 06 tháng lương cơ sở.</summary>
        public bool IsPaid6Month { get; set; }

        /// <summary>
        /// Thời điểm miễn cùng chi trả dạng yyyyMMdd — ngày ra viện của đợt KCB đầu tiên
        /// đẩy số lũy kế vượt ngưỡng. Null khi không suy ra được.
        /// </summary>
        public long? FreeCoPaidTime { get; set; }

        /// <summary>
        /// Đã vượt ngưỡng nhưng không đợt nào có ngày ra viện dùng được, nên thời điểm
        /// miễn vẫn chưa xác định và người dùng phải nhập theo giấy chứng nhận của BHXH.
        /// </summary>
        public bool IsMissingFreeCoPaidTime { get; set; }

        /// <summary>
        /// Tổng số tiền thuộc diện được miễn trên các đợt KCB trả về.
        /// Chỉ hiển thị tham khảo — HIS không có cột lưu.
        /// </summary>
        public decimal TotalMcctAmount { get; set; }

        /// <summary>Nguồn và mốc thời gian dữ liệu cổng, hiển thị cho người dùng.</summary>
        public string DataNote { get; set; }
    }
}

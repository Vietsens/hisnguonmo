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

namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.ADO
{
    /// <summary>
    /// 1 dòng datasource cột "Giá trị mặc định" — tương ứng 1 RadioGroupItem của RadioGroup
    /// <see cref="FIELD_NAME"/>. Lưới lọc theo FIELD_NAME nên chưa chọn "Nội dung" thì ô này rỗng.
    /// </summary>
    public class KskDefaultValueADO
    {
        /// <summary>Khóa RadioGroup chứa lựa chọn này — dùng để lọc theo ô "Nội dung" cùng dòng.</summary>
        public string FIELD_NAME { get; set; }

        /// <summary>
        /// Khóa ghép "FIELD_NAME|VALUE" dùng làm ValueMember.
        /// BẮT BUỘC ghép chứ không dùng riêng <see cref="VALUE"/>: cùng số 1 nhưng ô này nghĩa
        /// "Bình thường", ô kia nghĩa "Có" — nếu lấy VALUE làm khóa chung thì lưới hiển thị sai nhãn.
        /// </summary>
        public string VALUE_KEY { get; set; }

        /// <summary>Giá trị thật của RadioGroupItem — cái được ghi xuống DB.</summary>
        public long VALUE { get; set; }

        /// <summary>Nhãn item hiển thị cho người dùng (DisplayMember). VD "Không", "Bình thường".</summary>
        public string VALUE_CAPTION { get; set; }
    }
}

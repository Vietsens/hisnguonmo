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
    /// 1 dòng datasource cột "Nội dung" — tương ứng 1 RadioGroup nằm trong nhóm <see cref="KskDefaultGroupADO"/>.
    /// Lưới lọc theo <see cref="GROUP_NAME"/> nên chưa chọn "Mục" thì cột "Nội dung" không có dữ liệu.
    /// </summary>
    public class KskDefaultFieldADO
    {
        /// <summary>Khóa nhóm cha — dùng để lọc theo ô "Mục" đã chọn ở cùng dòng. VD "lcgMat8".</summary>
        public string GROUP_NAME { get; set; }

        /// <summary>Tên RadioGroup — khóa lưu/đối chiếu (ValueMember). VD "rdoStrabismus8".</summary>
        public string FIELD_NAME { get; set; }

        /// <summary>Nhãn LayoutControlItem hiển thị cho người dùng (DisplayMember). VD "Lác mắt".</summary>
        public string FIELD_CAPTION { get; set; }
    }
}

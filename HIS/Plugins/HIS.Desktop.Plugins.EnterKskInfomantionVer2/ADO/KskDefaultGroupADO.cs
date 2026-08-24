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
    /// 1 dòng datasource cột "Mục" của lưới thiết lập mặc định — tương ứng 1 LayoutControlGroup
    /// trong mục VI. KHÁM LÂM SÀNG (tab "Trẻ em dưới 6 tuổi").
    /// Danh sách dựng ĐỘNG bằng cách duyệt layout (xem BuildUnderSixDefaultCatalog), KHÔNG hardcode:
    /// thêm nhóm mới vào Designer thì nó tự xuất hiện ở đây.
    /// </summary>
    public class KskDefaultGroupADO
    {
        /// <summary>Tên LayoutControlGroup — khóa lưu/đối chiếu (ValueMember). VD "lcgMat8".</summary>
        public string GROUP_NAME { get; set; }

        /// <summary>Tiêu đề nhóm hiển thị cho người dùng (DisplayMember). VD "2.2. Khám mắt".</summary>
        public string GROUP_CAPTION { get; set; }
    }
}

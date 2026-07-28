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

namespace HIS.Desktop.Plugins.HisTreatmentType
{
    /// <summary>
    /// PT-48590 B.4.1.1: che do lam viec cua man hinh Dien dieu tri.
    /// Hieu luc cua nut luu phu thuoc che do + quyen, khong con phu thuoc viec da chon dong hay chua.
    /// </summary>
    internal enum EnumTreatmentTypeFormMode
    {
        /// <summary>Che do Them moi — o Ma / Ten mo cho nhap, nut Them hieu luc</summary>
        Add = 0,

        /// <summary>Che do Sua — o Ma / Ten chi doc, nut Sua hieu luc theo dong dang chon</summary>
        Edit = 1
    }
}

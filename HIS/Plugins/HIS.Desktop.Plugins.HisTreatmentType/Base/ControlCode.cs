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
    /// PT-48590 B.2.2: ma dieu khien phan quyen cap nut cua man hinh Dien dieu tri.
    /// Ma la chuoi duy nhat TOAN UNG DUNG (bang ACS_CONTROL khong co cot module) —
    /// truoc khi trien khai phai tra bang ma dieu khien thuc te cua benh vien de tranh trung ma.
    /// </summary>
    internal class ControlCode
    {
        /// <summary>Nut Them dien dieu tri</summary>
        internal const string BtnAdd = "HIS000057";

        /// <summary>Bieu tuong Khoa / mo khoa dien dieu tri tren luoi</summary>
        internal const string BtnChangeLock = "HIS000058";
    }
}

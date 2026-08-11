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

namespace HIS.Desktop.Plugins.HisServiceSpeciality
{
    /// <summary>
    /// Che do combo "Chon theo" cua man hinh thiet lap Dich vu - Pham vi chuyen mon (PTTK 3142).
    /// Gia tri map voi Status.id nap vao cboChoose.
    /// </summary>
    public enum EnumChooseBy
    {
        /// <summary>Chon theo Dich vu: grid dich vu radio chon 1, grid pham vi chuyen mon checkbox chon nhieu</summary>
        Service = 1,

        /// <summary>Chon theo Pham vi chuyen mon: grid pham vi chuyen mon radio chon 1, grid dich vu checkbox chon nhieu</summary>
        Speciality = 2
    }
}

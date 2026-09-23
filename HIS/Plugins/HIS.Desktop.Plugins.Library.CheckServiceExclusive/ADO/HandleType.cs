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

namespace HIS.Desktop.Plugins.Library.CheckServiceExclusive.ADO
{
    /// <summary>
    /// Muc xu ly khi phat hien 2 dich vu loai tru nhau (cot HANDLE_TYPE_ID cua HIS_SERVICE_EXCLUSIVE).
    /// Rang buoc CSDL: HIS_SERVICE_EXCLUSIVE_CHK2 CHECK (HANDLE_TYPE_ID IN (1, 2)), mac dinh 1.
    ///
    /// LUU Y quy uoc 1/2 cua he thong KHONG nhat quan (SIMULTANEITY 1=chan/2=canh bao) nen
    /// bang nay bam theo tai lieu 3342: 1 = Canh bao, 2 = Chan.
    /// </summary>
    public enum HandleType : short
    {
        /// <summary>Canh bao: nhac nguoi dung nhung van cho chi dinh</summary>
        Warning = 1,

        /// <summary>Chan: khong cho chi dinh</summary>
        Block = 2
    }
}

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
    /// </summary>
    public enum HandleType : short
    {
        /// <summary>Canh bao: nhac nguoi dung nhung van cho chi dinh</summary>
        Warning = 1,

        /// <summary>Chan: khong cho chi dinh</summary>
        Block = 2
    }

    /// <summary>
    /// Ban ghi danh muc "dich vu khong duoc phep chi dinh dong thoi" (viec 57452 / TTMB-TK-56258).
    ///
    /// LUU Y: day la lop CUC BO cua FE. Bang HIS_SERVICE_EXCLUSIVE chua co trong MOS.EFMODEL.dll
    /// dang ban giao, nen FE tu khai de build duoc truoc. Ten property phai TRUNG TUYET DOI voi
    /// ten cot cua entity phia backend thi JSON moi map dung.
    /// Khi backend ban giao MOS.EFMODEL moi: thay lop nay bang MOS.EFMODEL.DataModels.HIS_SERVICE_EXCLUSIVE.
    /// </summary>
    public class HIS_SERVICE_EXCLUSIVE
    {
        public long ID { get; set; }
        public Nullable<long> CREATE_TIME { get; set; }
        public Nullable<long> MODIFY_TIME { get; set; }
        public string CREATOR { get; set; }
        public string MODIFIER { get; set; }
        public string APP_CREATOR { get; set; }
        public string APP_MODIFIER { get; set; }
        public Nullable<short> IS_ACTIVE { get; set; }
        public Nullable<short> IS_DELETE { get; set; }
        public string GROUP_CODE { get; set; }

        /// <summary>Dich vu goc</summary>
        public long SERVICE_ID { get; set; }

        /// <summary>Dich vu khong duoc chi dinh cung dich vu goc</summary>
        public long EXCLUSIVE_ID { get; set; }

        /// <summary>1 = Canh bao, 2 = Chan (xem HandleType)</summary>
        public short HANDLE_TYPE_ID { get; set; }

        /// <summary>Ghi chu / dien giai ly do loai tru (tuy chon) - hien kem noi dung thong bao neu co (tai lieu 3342)</summary>
        public string NOTE { get; set; }
    }
}

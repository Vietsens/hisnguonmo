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

namespace HIS.Desktop.Plugins.HisImport.Message
{
    class MessageImport
    {
        internal const string Maxlength = "{0} vượt quá maxlength|";
        internal const string KhongHopLe = "{0} không hợp lệ|";
        internal const string ThieuTruongDL = "Thiếu trường dữ liệu bắt buộc {0}|";
        internal const string DaTonTai = " {0} đã tồn tại dữ liệu nhân viên |";
        internal const string TonTaiTrungNhauTrongFileImport = "Tồn tại {0} trùng nhau trong file import|";
        internal const string CoThiPhaiNhap = "Có {0} thì phải nhập {1}|";
        internal const string MaKhoaDaKhoa = "Mã khoa {0} đã bị khóa| ";
        internal const string FileImportDaTonTai = "File import tồn tại tên đăng nhập giống nhau {0} | ";
        internal const string SaiDinhDang = " {0} sai định dạng (X)| ";
        internal const string KhongtonTai = " {0} không tồn tại trong danh sách người dùng( ACS_USER) |";
    }
}

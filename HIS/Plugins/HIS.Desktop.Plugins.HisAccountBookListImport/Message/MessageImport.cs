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

namespace HIS.Desktop.Plugins.HisAccountBookListImport.Message
{
    class MessageImport
    {
        internal const string Maxlength = "{0} vượt quá độ dài cho phép";
        internal const string KhongHopLe = "{0}không hợp lệ";
        internal const string ThieuTruongDL = "Thiếu trường {0}";
        internal const string TrungMaSo = " Trùng mã sổ trong file import";
        internal const string DaTonTaiLoaiSo = " {0} đã tồn tại trong loại sổ";
        internal const string TonTaiTrungNhauTrongFileImport = "Tồn tại sổ có cùng các thông số trong file import";
        internal const string CoThiPhaiNhap = "Có {0} thì phải nhập {1}";
        internal const string MaSoDaKhoa = "Mã sổ đã bị khóa";
        internal const string MaLoaiSoDaKhoa = "Mã loại sổ đã bị khóa";
        internal const string TenLoaiSoDaKhoa = "Tên loại sổ đã bị khóa";
        internal const string DBDaTonTai = "Mã sổ \"{0}\" đã tồn tại trong cơ sở dữ liệu";

        internal const string NguonKinhPhiTrongKhoang13 = "Nguồn kinh phí chỉ cho phép nhập các giá trị 1,2,3";
        internal const string DanhSachMaPhongNganCachBangDauPhay = "Danh sách các mã phòng phải được ngăn cách với nhau bằng dấu phẩy";
    }
}

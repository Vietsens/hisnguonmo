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

namespace HIS.Desktop.Plugins.ImportHisCashierAddCfg.Message
{
    class MessageImport
    {
        internal const string Maxlength = "{0} vượt quá độ dài cho phép|";
        internal const string KhongHopLe = "{0} không hợp lệ|";
        internal const string ThieuTruongDL = "Thiếu trường {0}|";
        internal const string DaTonTai = "{0} đã tồn tại|";
        internal const string TonTaiTrungNhauTrongFileImport = "Tồn tại {0} trùng nhau trong file import|";
        internal const string ThieuMaPhongYeuCauVaMaPhongXuLy = "Thiếu cả hai thông tin Mã phòng yêu cầu, mã phòng xử lý|";
        internal const string ThieuMaPhongThuNgan = "Thiếu thông tin mã phòng thu ngân|";
        internal const string MaPhongKhongTonTai = "Mã phòng {0} không tồn tại|";
        internal const string NgayKhongChinhXac = "Ngày từ/đến không chính xác|";
        internal const string NgayTuNhoHonNgayDen = "Ngày từ không được lớn hơn ngày đến|";
        internal const string DinhDangThoiGian = "Thời gian từ/đến không đúng định dạng|";
        internal const string ThoiGianKhongChinhXac = "Thời gian từ/đến không chính xác|";
        internal const string ThoiGianTuNhoHonNgayDen = "Thời gian từ không được lớn hơn thời gian đến|";
        internal const string MaPhongXuLy = "Mã phòng xử lý ";
        internal const string MaPhongYeuCau = "Mã phòng têu cầu ";
        internal const string MaPhongThuNgan = "Mã phòng thu ngân ";
    }
}

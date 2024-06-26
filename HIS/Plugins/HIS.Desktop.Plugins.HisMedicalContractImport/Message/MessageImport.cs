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

namespace HIS.Desktop.Plugins.HisMedicalContractImport.Message
{
    class MessageImport
    {
        internal const string Maxlength = "{0} vượt quá độ dài cho phép";
        internal const string KhongHopLe = "{0} không hợp lệ";
        internal const string ThieuTruongDL = "Thiếu trường {0}";
        internal const string TonTaiTrungMaThuocTrongFileImport = "Hợp đồng {0} tồn tại trùng mã thuốc {1} trong file import";
        internal const string TonTaiTrungMaVatTuTrongFileImport = "Hợp đồng {0} tồn tại trùng mã vật tư {1} trong file import";
        internal const string DuLieuDaKhoa = "{0} đã bị khóa";
        internal const string DulieuHopDongKhongHopLe = "Đã tồn tại dữ liệu hợp đồng với mã {0} nhưng khác dữ liệu: {1}";
        internal const string ThauKhongCoMaThuocVT = "Quyết định thầu {0} không có dữ liệu {1}";
        internal const string DBDaTonTai = "Số hợp đồng {0} đã tồn tại";
    }
}

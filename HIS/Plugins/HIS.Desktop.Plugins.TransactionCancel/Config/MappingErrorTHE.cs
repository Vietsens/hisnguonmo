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

namespace HIS.Desktop.Plugins.TransactionCancel.Config
{
    public class MappingErrorTHE
    {
        public Dictionary<string, string> dicMapping { get; set; }
        public MappingErrorTHE()
        {
            dicMapping = new Dictionary<string, string>();
            dicMapping.Add("00", "Giao dịch thành công");
            dicMapping.Add("02", "Giao dịch thanh toán đã được hủy");
            dicMapping.Add("03", "Giao dịch thanh toán đã được hoàn");
            dicMapping.Add("04", "Giao dịch thanh toán không thành công");
            dicMapping.Add("10", "Giao dịch không chính xác");
            dicMapping.Add("11", "Giao dịch không tồn tại");
            dicMapping.Add("12", "Giao dịch đang bị tạm khóa");
            dicMapping.Add("14", "Số tiền hủy (hoàn) không bằng số tiền thanh toán");
            dicMapping.Add("17", "Số tiền không chính xác");
            dicMapping.Add("18", "Mã giao dịch không tồn tại");
            dicMapping.Add("19", "Không tạo được giao dịch");
            dicMapping.Add("20", "Tài khoản thanh toán không tồn tại");
            dicMapping.Add("21", "Tài khoản thanh toán đang bị tạm khóa");
            dicMapping.Add("22", "Tài khoản thanh toán không đủ số dư");
            dicMapping.Add("23", "Loại tài khoản không chính xác");
            dicMapping.Add("24", "Mã pin không chính xác");
            dicMapping.Add("25", "Tài khoản nhận không tồn tại");
            dicMapping.Add("26", "Tài khoản nhận đang bị tạm khóa");
            dicMapping.Add("29", "Tài khoản không chính xác");
            dicMapping.Add("32", "Thẻ không tồn tại");
            dicMapping.Add("33", "Thẻ đang bị tạm khóa");
            dicMapping.Add("34", "Mã thẻ không tồn tại");
            dicMapping.Add("36", "Tài khoản thực hiện giao dịch không tồn tại");
            dicMapping.Add("37", "Tài khoản thực hiện giao dịch đang bị tạm khóa");
            dicMapping.Add("38", "Tài khoản thực hiện giao dịch không chính xác");
            dicMapping.Add("39", "Mã client (HIS – ClientTrace) không chính xác");
            dicMapping.Add("41", "Không có số CMND");
            dicMapping.Add("42", "Giao dịch timeout");
            dicMapping.Add("43", "Thẻ đã hết hạn sử dụng");
            dicMapping.Add("44", "Thẻ không chính xác");
            dicMapping.Add("45", "Dữ liệu đầu vào không hợp lệ");
            dicMapping.Add("46", "Giao dịch đã được chốt quyết toán");
            dicMapping.Add("50", "Lỗi kết nối");
            dicMapping.Add("99", "Lỗi không xác định");
            dicMapping.Add("89", "Lỗi phần mềm Onelink");
            dicMapping.Add("88", "Lỗi phần mềm Onelink");
        }
    }
}

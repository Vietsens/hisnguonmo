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

namespace Inventec.Common.ElectronicBill
{
    public class MappingError
    {
        public Dictionary<string, string> dicMapping;
        public MappingError()
        {
            dicMapping = new Dictionary<string, string>();
            dicMapping.Add("ERR:1", "Tài khoản đăng nhập sai hoặc không có quyền thêm khách hàng");
            dicMapping.Add("ERR:2", "Chuỗi token không chính xác");
            dicMapping.Add("ERR:3", "Dữ liệu xml đầu vào không đúng quy định");
            dicMapping.Add("ERR:5", "Không phát hành được hóa đơn");
            dicMapping.Add("ERR:6", "Không tìm thấy hóa đơn tương ứng chuỗi đưa vào");
            dicMapping.Add("ERR:7", "User name không phù hợp, không tìm thấy company tương ứng cho user.");
            dicMapping.Add("ERR:8", "Hóa đơn cần điều chỉnh đã bị thay thế. Không thể điều chỉnh được nữa.");
            dicMapping.Add("ERR:8.1", "Hóa đơn đã được chuyển đổi");
            dicMapping.Add("ERR:9", "Trạng thái hóa đơn không được điều chỉnh");
            dicMapping.Add("ERR:10", "Lô có số hóa đơn vượt quá max cho phép");
            dicMapping.Add("ERR:11", "Hóa đơn chưa được thanh toán");
            dicMapping.Add("ERR:13", "Hóa đơn đã gạch nợ rồi");
            dicMapping.Add("ERR:20", "Pattern và serial không phù hợp, hoặc không tồn tại hóa đơn đã đăng kí có sử dụng Pattern và serial truyền vào");
        }
    }
}

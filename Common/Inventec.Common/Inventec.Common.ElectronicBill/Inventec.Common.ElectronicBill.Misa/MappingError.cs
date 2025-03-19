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

namespace Inventec.Common.ElectronicBill.Misa
{
    internal class MappingError
    {
        public static Dictionary<string, string> DicMapping;
        public static void CreateMappingError()
        {
            DicMapping = new Dictionary<string, string>();
            DicMapping.Add("InvalidParameter", "Thông tin tham số gửi lên service không đúng");
            DicMapping.Add("InvalidAppID", "Thông tin AppID không đúng");
            DicMapping.Add("InvalidTaxCode", "Mã số thuế không đúng hoặc Mã số thuế trên hóa đơn, trên chứng thư số không khớp nhau");
            DicMapping.Add("InvoiceNotExist", "Hóa đơn không tồn tại");
            DicMapping.Add("InvoiceDeleted", "Hóa đơn đã bị xóa bỏ");
            DicMapping.Add("LicenseNotExist", "Chưa có giấy phép sử dụng dịch vụ meInvoice");
            DicMapping.Add("InvoiceQuantityTooLarge", "Số lượng hóa đơn quá lớn");
            DicMapping.Add("RequireInfo_", "Thông tin bắt buộc");
            DicMapping.Add("InvalidInvNo", "Số hóa đơn không hợp lệ. Lỗi này cũng xảy ra khi phát hành hóa đơn có số không liên tiếp");
            DicMapping.Add("InvalidXMLData", "Định dạng dữ liệu hóa đơn không hợp lệ");
            DicMapping.Add("InvalidVatPercentage", "Thông tin thuế suất thuế GTGT không hợp lệ. Các thuế suất hợp lệ bao gồm: -0: 0%  -5: 5%  -10: 10%  -1: Không chịu thuế");
            DicMapping.Add("InvoiceIssuedDate", "Ngày hóa đơn không hợp lệ. Ngày hóa đơn đúng phải lớn hơn hoặc bằng ngày ngày của hóa đơn liền kề");
            DicMapping.Add("InvoiceNumberNotContinuous", "Số hóa đơn không liên tục");
            DicMapping.Add("SignatureEmpty", "Không có thông tin chữ ký số trên hóa đơn điện tử");
            DicMapping.Add("InvalidSignature", "Chữ ký số không hợp lệ. Lỗi có thể do dữ liệu hóa đơn đã bị thay đổi sau khi ký điện tử");
            DicMapping.Add("InvalidTransactionID", "Mã tra cứu hóa đơn không đúng");
            DicMapping.Add("InvalidAdjustmentType", "Loại hóa đơn không đúng. Loại hóa đơn sẽ chỉ gồm các giá trị sau: 1: Hóa đơn gốc 3: Hóa đơn thay thế 5: Hóa đơn điều chỉnh");
            DicMapping.Add("InvoicePublishNotExist", "Không có thông báo phát hành hóa đơn cho mẫu số, ký hiệu tương ứng của hóa đơn");
            DicMapping.Add("InvoiceNumberNotCotinuous", "Số hóa đơn không liên tục");
        }
    }
}

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
namespace HIS.Desktop.Plugins.Library.ElectronicBill.Base
{
    /// <summary>
    /// Nguồn gây lỗi khi phát hành/tra cứu hóa đơn điện tử.
    /// Dùng để báo cho người dùng biết lỗi thuộc về bên nào và cần liên hệ ai (vCong 62522).
    /// </summary>
    public enum ElectronicBillErrorSource
    {
        /// <summary>
        /// Không kết nối được tới cổng nhà cung cấp: mất mạng, timeout, DNS, tường lửa, SSL/TLS, cổng gián đoạn.
        /// Liên hệ: IT bệnh viện kiểm tra mạng máy trạm; nếu mạng bình thường thì liên hệ nhà cung cấp.
        /// </summary>
        Connection = 1,

        /// <summary>
        /// Cổng nhà cung cấp nhận được yêu cầu nhưng từ chối (sai tài khoản, hết số hóa đơn, dữ liệu không hợp lệ theo quy tắc của họ, HTTP 4xx/5xx...).
        /// Liên hệ: nhà cung cấp hóa đơn điện tử, kèm nội dung lỗi.
        /// </summary>
        Provider = 2,

        /// <summary>
        /// Lỗi trên máy trạm: không ghi được file, thiếu quyền, USB token/chứng thư số, thư viện thiếu, hết bộ nhớ.
        /// Liên hệ: IT bệnh viện kiểm tra máy trạm.
        /// </summary>
        Workstation = 3,

        /// <summary>
        /// Thiếu hoặc sai cấu hình tích hợp hóa đơn điện tử trong HIS (địa chỉ cổng, tài khoản, tham số mẫu/ký hiệu).
        /// Liên hệ: quản trị HIS tại viện hoặc đội triển khai.
        /// </summary>
        Config = 4,

        /// <summary>
        /// Dữ liệu hóa đơn/giao dịch chưa hợp lệ về nghiệp vụ (giao dịch khác hồ sơ, tổng tiền bằng 0, giao dịch thay thế...).
        /// Liên hệ: người dùng tự kiểm tra lại dữ liệu; nếu đã đúng thì liên hệ hỗ trợ HIS.
        /// </summary>
        Business = 5,

        /// <summary>
        /// Lỗi xử lý bên trong phần mềm HIS (null reference, ép kiểu, thiếu/lệch phiên bản thư viện...).
        /// Liên hệ: hỗ trợ phần mềm HIS kèm nội dung lỗi và thời điểm thao tác.
        /// </summary>
        Software = 6
    }
}

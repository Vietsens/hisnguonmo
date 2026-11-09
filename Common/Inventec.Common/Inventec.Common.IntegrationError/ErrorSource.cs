namespace Inventec.Common.IntegrationError
{
    /// <summary>
    /// Nguồn gây lỗi khi gọi một hệ thống tích hợp (hóa đơn điện tử, BHXH, ký số, ngân hàng, cổng y tế, LIS, PACS, thiết bị...).
    /// Dùng để báo cho người dùng biết lỗi thuộc về bên nào và cần liên hệ ai (vCong 62522).
    /// </summary>
    public enum ErrorSource
    {
        /// <summary>Không kết nối được: mất mạng, timeout, DNS, tường lửa, SSL/TLS, cổng đối tác gián đoạn (HTTP 502/503/504).</summary>
        Connection = 1,

        /// <summary>Hệ thống đối tác nhận yêu cầu nhưng trả lỗi (mã lỗi/mô tả của họ, HTTP 400/500, dữ liệu trả về sai định dạng).</summary>
        Partner = 2,

        /// <summary>Lỗi trên máy trạm: quyền ghi file, ổ đĩa, USB token/chứng thư số, thiết bị, dịch vụ cục bộ.</summary>
        Workstation = 3,

        /// <summary>Thiết lập/cấu hình tại viện chưa đúng: địa chỉ, tài khoản, mật khẩu, mã đơn vị, khóa bí mật (HTTP 401/403/404).</summary>
        Config = 4,

        /// <summary>Dữ liệu nghiệp vụ chưa hợp lệ, người dùng tự kiểm tra được.</summary>
        Business = 5,

        /// <summary>Lỗi xử lý bên trong phần mềm HIS (null reference, ép kiểu, lệch phiên bản thư viện...).</summary>
        Software = 6
    }
}

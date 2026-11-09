using Inventec.Common.IntegrationError;
using System;
using System.Text.RegularExpressions;

namespace HIS.Desktop.Plugins.ExportXmlQD130.Base
{
    /// <summary>
    /// Phân loại nguồn lỗi cho các worker đẩy cổng phụ khi xuất XML 130
    /// (CSDL 4750, CSDL Cần Thơ 3176, Trung tâm điều hành y tế HOC, cổng tiếp nhận Vĩnh Long, tiền giám định) — vCong 62522.
    /// Các worker vốn tự dựng chuỗi lỗi ("HTTP 500 - ...", "đăng nhập ... thất bại", "Lỗi ngoại lệ khi gửi: ...");
    /// lớp này chuyển chuỗi đó thành thông báo có nguồn lỗi + liên hệ, giữ nguyên nội dung gốc ở phần Chi tiết.
    /// </summary>
    internal static class PortalErrorHelper
    {
        private static readonly Regex HTTP_CODE = new Regex(@"HTTP\s*(\d{3})", RegexOptions.IgnoreCase);

        /// <summary>Chuỗi lỗi worker → thông báo phân loại. fallbackSummary dùng khi lỗi thuộc phía cổng.</summary>
        internal static string FromRaw(IntegrationPartner partner, string raw, string fallbackSummary)
        {
            try
            {
                if (IntegrationErrorMessage.IsBuilt(raw)) return raw;
                if (String.IsNullOrWhiteSpace(raw)) return IntegrationErrorMessage.NoResponse(partner, null);
                string m = raw.ToLower();
                if (IntegrationErrorClassifier.IsConnectionMessage(raw))
                    return IntegrationErrorMessage.Build(ErrorSource.Connection, partner, "Không kết nối được tới " + IntegrationErrorMessage.GetName(partner), raw);
                if (m.Contains("cấu hình") || m.Contains("khóa cấu hình"))
                    return IntegrationErrorMessage.ConfigError(partner, raw);
                if (m.Contains("không có dữ liệu xml") || m.Contains("vượt giới hạn") || m.Contains("ký số thất bại"))
                    return IntegrationErrorMessage.BusinessError(partner, raw);
                if (m.Contains("đăng nhập") || m.Contains("token") || m.Contains("401") || m.Contains("403"))
                    return IntegrationErrorMessage.LoginError(partner, raw);
                Match http = HTTP_CODE.Match(raw);
                if (http.Success)
                {
                    int code = Int32.Parse(http.Groups[1].Value);
                    ErrorSource src = IntegrationErrorClassifier.SourceFromHttpStatus(code);
                    string summary = src == ErrorSource.Partner ? fallbackSummary
                        : (src == ErrorSource.Config ? "Cổng từ chối truy cập (HTTP " + code + "): kiểm tra địa chỉ/tài khoản thiết lập"
                        : "Cổng không phục vụ được (HTTP " + code + ")");
                    return IntegrationErrorMessage.Build(src, partner, summary, raw);
                }
                return IntegrationErrorMessage.PartnerError(partner, raw, fallbackSummary);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return raw;
            }
        }

        /// <summary>Đăng nhập cổng thất bại: dùng exception nếu có (mạng/timeout), không thì lý do cổng trả về (tài khoản sai...).</summary>
        internal static string Login(IntegrationPartner partner, string reason, Exception ex)
        {
            if (ex != null) return IntegrationErrorMessage.FromException(ex, partner, "đăng nhập " + IntegrationErrorMessage.GetName(partner));
            return IntegrationErrorMessage.LoginError(partner, reason);
        }
    }
}

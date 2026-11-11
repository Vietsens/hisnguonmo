using Inventec.Common.IntegrationError;
using System;
using System.Net.Http;

namespace HIS.Desktop.Plugins.Library.NationalPharmacyConnect
{
    /// <summary>
    /// (vCong 62522) Dựng nội dung TinNhan khi liên thông cổng đơn thuốc quốc gia thất bại, có phân loại nguồn lỗi
    /// (kết nối / cổng đơn thuốc trả lỗi / tài khoản liên thông tại viện / lỗi code HIS) và giữ nguyên văn nội dung cổng trả về.
    /// Trường Ma giữ nguyên như cũ (mã HTTP hoặc mã cổng trả về) để không đổi logic kiểm tra thành công.
    /// </summary>
    public static class PharmacyErrorHelper
    {
        public static IntegrationPartner Partner
        {
            get { return IntegrationPartner.Pharmacy; }
        }

        /// <summary>Cổng trả HTTP không thành công (401/403 = tài khoản liên thông, 404 = địa chỉ, 5xx = phía cổng/kết nối).</summary>
        public static string HttpError(HttpResponseMessage response, string body, string action)
        {
            try
            {
                if (response == null) return IntegrationErrorMessage.NoResponse(Partner, action);
                string uri = response.RequestMessage != null && response.RequestMessage.RequestUri != null ? response.RequestMessage.RequestUri.ToString() : "";
                IntegrationException ex = IntegrationHttpError.FromStatus(Partner, uri, (int)response.StatusCode, response.ReasonPhrase, body);
                return IntegrationErrorMessage.FromException(ex, Partner, action);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                return body;
            }
        }

        /// <summary>Cổng nhận yêu cầu nhưng trả mã lỗi nghiệp vụ (Ma khác 200) kèm tin nhắn.</summary>
        public static string ApiError(string ma, string tinNhan, string action)
        {
            if (IntegrationErrorMessage.IsBuilt(tinNhan)) return tinNhan;
            string apiMessage = String.IsNullOrWhiteSpace(ma) ? tinNhan : ("Mã " + ma + (String.IsNullOrWhiteSpace(tinNhan) ? "" : ". " + tinNhan));
            return IntegrationErrorMessage.PartnerError(Partner, apiMessage, "Cổng đơn thuốc quốc gia từ chối " + action);
        }

        /// <summary>Cổng trả HTTP 200 nhưng nội dung không đọc được.</summary>
        public static string ParseError(string body, string action)
        {
            return IntegrationErrorMessage.FromException(IntegrationHttpError.ParseError(Partner, action, body, null), Partner, null);
        }

        /// <summary>Exception khi gọi cổng (mất mạng, timeout, địa chỉ sai định dạng...).</summary>
        public static string ExceptionError(Exception ex, string action)
        {
            return IntegrationErrorMessage.FromException(ex, Partner, action);
        }

        /// <summary>Đăng nhập cổng thất bại → thường do tài khoản liên thông thiết lập tại viện.</summary>
        public static string LoginError(string ma, string tinNhan)
        {
            if (IntegrationErrorMessage.IsBuilt(tinNhan)) return tinNhan;
            string apiMessage = String.IsNullOrWhiteSpace(tinNhan) ? (String.IsNullOrWhiteSpace(ma) ? null : "Mã " + ma) : (String.IsNullOrWhiteSpace(ma) ? tinNhan : "Mã " + ma + ". " + tinNhan);
            return IntegrationErrorMessage.LoginError(Partner, apiMessage);
        }
    }
}

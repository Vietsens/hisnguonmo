using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace Inventec.Common.IntegrationError
{
    /// <summary>
    /// Hỗ trợ các lớp gọi API (HttpClient / HttpWebRequest) ném exception đã gắn nguồn lỗi,
    /// thay cho kiểu cũ throw new Exception("Loi khi goi API ... StatusCode") làm mất nội dung đối tác trả về.
    /// </summary>
    public static class IntegrationHttpError
    {
        /// <summary>
        /// Gói exception phát sinh khi gọi HTTP (HttpClient .Result, HttpWebRequest.GetResponse) thành IntegrationException có nguồn.
        /// WebException có response (HTTP lỗi) sẽ đọc body; đã là IntegrationException thì trả nguyên.
        /// </summary>
        public static Exception WrapCall(Exception ex, IntegrationPartner partner, string uri)
        {
            if (ex == null) return new IntegrationException(ErrorSource.Software, partner, "Lỗi không xác định khi gọi " + IntegrationErrorMessage.GetName(partner));
            if (IntegrationErrorClassifier.Flatten(ex).Any(o => o is IntegrationException)) return ex;

            WebException webEx = IntegrationErrorClassifier.Flatten(ex).OfType<WebException>().FirstOrDefault();
            if (webEx != null && webEx.Response != null)
            {
                return FromWebException(webEx, partner, uri);
            }

            ErrorSource source = IntegrationErrorClassifier.Classify(ex);
            Exception root = IntegrationErrorClassifier.GetRoot(ex);
            string name = IntegrationErrorMessage.GetName(partner);
            string msg;
            if (source == ErrorSource.Connection)
            {
                msg = IntegrationErrorClassifier.IsTimeout(root)
                    ? string.Format("Hết thời gian chờ phản hồi từ {0} ({1}). {2}", name, uri, root.Message)
                    : string.Format("Không kết nối được tới {0} ({1}): {2}", name, uri, root.Message);
            }
            else
            {
                msg = string.Format("Lỗi khi gọi {0} ({1}): {2}", name, uri, root.Message);
            }
            return new IntegrationException(source, partner, msg, ex);
        }

        /// <summary>WebException có response: lấy mã HTTP + body để báo đúng nguồn.</summary>
        public static IntegrationException FromWebException(WebException ex, IntegrationPartner partner, string uri)
        {
            HttpWebResponse httpResponse = ex.Response as HttpWebResponse;
            if (httpResponse == null)
            {
                return new IntegrationException(ErrorSource.Connection, partner,
                    string.Format("Không kết nối được tới {0} ({1}): {2}", IntegrationErrorMessage.GetName(partner), uri, ex.Message), ex);
            }
            string body = "";
            try
            {
                using (Stream s = httpResponse.GetResponseStream())
                using (StreamReader reader = new StreamReader(s))
                {
                    body = reader.ReadToEnd();
                }
            }
            catch { }
            return FromStatus(partner, uri, (int)httpResponse.StatusCode, httpResponse.StatusDescription, body);
        }

        /// <summary>HttpClient trả HTTP không thành công: đọc body rồi báo đúng nguồn.</summary>
        public static IntegrationException FromStatus(IntegrationPartner partner, string uri, HttpResponseMessage resp)
        {
            if (resp == null) return NoResponse(partner, "gọi " + uri);
            return FromStatus(partner, uri, (int)resp.StatusCode, resp.ReasonPhrase, ReadBody(resp));
        }

        /// <summary>
        /// Đối tác trả HTTP không thành công: 401/403/404/405 = thiết lập tại viện, 408/502/503/504 = kết nối, còn lại = phía đối tác.
        /// </summary>
        public static IntegrationException FromStatus(IntegrationPartner partner, string uri, int statusCode, string reason, string body)
        {
            string name = IntegrationErrorMessage.GetName(partner);
            string detail = IntegrationErrorMessage.Shorten(String.IsNullOrWhiteSpace(body) ? reason : body);
            ErrorSource source = IntegrationErrorClassifier.SourceFromHttpStatus(statusCode);
            string summary;
            switch (statusCode)
            {
                case 401:
                case 403:
                    summary = string.Format("{0} từ chối xác thực (mã {1}): tài khoản, mật khẩu, token, khóa bí mật hoặc mã đơn vị thiết lập trong HIS không đúng hoặc đã hết hạn", name, statusCode);
                    break;
                case 404:
                case 405:
                    summary = string.Format("Không tìm thấy đường dẫn API trên {0} (mã {1}, {2}): kiểm tra địa chỉ trong cấu hình; nếu địa chỉ đúng thì đối tác đã đổi API", name, statusCode, uri);
                    break;
                case 408:
                case 502:
                case 503:
                case 504:
                    summary = string.Format("{0} không phục vụ được (mã {1}, {2})", name, statusCode, uri);
                    break;
                default:
                    summary = string.Format("{0} trả về mã {1} ({2})", name, statusCode, uri);
                    break;
            }
            return new IntegrationException(source, partner, statusCode, String.IsNullOrWhiteSpace(detail) ? summary : summary + ": " + detail);
        }

        /// <summary>Đối tác trả 200 nhưng nội dung không đọc được theo cấu trúc mong đợi.</summary>
        public static IntegrationException ParseError(IntegrationPartner partner, string uri, string body, Exception inner)
        {
            return new IntegrationException(ErrorSource.Partner, partner,
                string.Format("{0} ({1}) trả về dữ liệu không đúng định dạng: {2}", IntegrationErrorMessage.GetName(partner), uri, IntegrationErrorMessage.Shorten(body)), inner);
        }

        /// <summary>Không nhận được phản hồi (null / rỗng).</summary>
        public static IntegrationException NoResponse(IntegrationPartner partner, string action)
        {
            return new IntegrationException(ErrorSource.Connection, partner,
                String.IsNullOrWhiteSpace(action)
                    ? "Không nhận được phản hồi từ " + IntegrationErrorMessage.GetName(partner)
                    : string.Format("Không nhận được phản hồi từ {0} khi {1}", IntegrationErrorMessage.GetName(partner), action));
        }

        /// <summary>Ném IntegrationException nếu response null hoặc HTTP không thành công.</summary>
        public static void EnsureSuccess(HttpResponseMessage resp, IntegrationPartner partner, string uri)
        {
            if (resp == null) throw NoResponse(partner, "gọi " + uri);
            if (!resp.IsSuccessStatusCode) throw FromStatus(partner, uri, resp);
        }

        /// <summary>Đọc body an toàn (không ném lỗi).</summary>
        public static string ReadBody(HttpResponseMessage resp)
        {
            try
            {
                return resp != null && resp.Content != null ? resp.Content.ReadAsStringAsync().Result : "";
            }
            catch
            {
                return "";
            }
        }
    }
}

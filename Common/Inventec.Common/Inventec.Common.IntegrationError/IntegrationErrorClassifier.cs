using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Inventec.Common.IntegrationError
{
    /// <summary>
    /// Xác định nguồn lỗi từ exception hoặc chuỗi mô tả lỗi.
    /// Thứ tự: IntegrationException (đã biết nguồn) → loại exception cụ thể nhất → nội dung message.
    /// </summary>
    public static class IntegrationErrorClassifier
    {
        private const string OWN_NAMESPACE = "Inventec.Common.IntegrationError";

        /// <summary>
        /// Xác định nguồn lỗi từ exception (đi hết chuỗi InnerException / AggregateException).
        /// </summary>
        public static ErrorSource Classify(Exception ex)
        {
            if (ex == null) return ErrorSource.Software;

            List<Exception> chain = Flatten(ex);
            foreach (Exception e in chain)
            {
                IntegrationException known = e as IntegrationException;
                if (known != null) return known.ErrorSource;
            }

            for (int i = chain.Count - 1; i >= 0; i--)
            {
                ErrorSource? byType = ClassifyByType(chain[i]);
                if (byType.HasValue) return byType.Value;
            }

            return ClassifyByMessage(ex.Message);
        }

        private static ErrorSource? ClassifyByType(Exception e)
        {
            if (e == null) return null;

            WebException webEx = e as WebException;
            if (webEx != null)
            {
                if (webEx.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse httpResponse = webEx.Response as HttpWebResponse;
                    int code = httpResponse != null ? (int)httpResponse.StatusCode : 0;
                    return SourceFromHttpStatus(code);
                }
                return ErrorSource.Connection;
            }
            if (e is SocketException || e is TimeoutException || e is TaskCanceledException || e is OperationCanceledException
                || e is HttpRequestException || e is System.Security.Authentication.AuthenticationException)
                return ErrorSource.Connection;

            // WCF / SOAP / Json: so theo tên kiểu để không phải tham chiếu System.ServiceModel, System.Web.Services, Newtonsoft
            string typeName = e.GetType().FullName ?? "";
            if (typeName.StartsWith("System.ServiceModel.FaultException") || typeName.StartsWith("System.Web.Services.Protocols.Soap"))
                return ErrorSource.Partner;
            if (typeName.StartsWith("System.ServiceModel."))
                return ErrorSource.Connection; // EndpointNotFound, Communication, ServerTooBusy, MessageSecurity...
            if (IsTypeOrSubclassOf(e, "Newtonsoft.Json.JsonException") || e is System.Xml.XmlException)
                return ErrorSource.Partner; // dữ liệu đối tác trả về không đúng định dạng
            if (e is InvalidOperationException)
            {
                string m = (e.Message ?? "").ToLower();
                if (m.Contains("response content type") || m.Contains("error in xml document") || m.Contains("client found response"))
                    return ErrorSource.Partner; // SOAP client: đối tác/proxy trả HTML thay vì XML
            }

            if (e is System.Security.Cryptography.CryptographicException || e is UnauthorizedAccessException
                || e is System.Runtime.InteropServices.COMException || e is OutOfMemoryException
                || e is DriveNotFoundException || e is DirectoryNotFoundException || e is FileNotFoundException || e is PathTooLongException)
                return ErrorSource.Workstation;
            if (e is IOException && !(e is FileLoadException)) return ErrorSource.Workstation;

            if (e is UriFormatException) return ErrorSource.Config;
            if (e is IndexOutOfRangeException || e is FormatException || e is OverflowException)
                return IsConfigStage(e) ? ErrorSource.Config : ErrorSource.Software;

            if (e is TypeLoadException || e is FileLoadException || e is MissingMemberException || e is DllNotFoundException || e is BadImageFormatException
                || e is NullReferenceException || e is InvalidCastException || e is ArgumentException || e is InvalidOperationException
                || e is KeyNotFoundException || e is NotImplementedException || e is NotSupportedException || e is ArithmeticException)
                return ErrorSource.Software;

            if (e.GetType() == typeof(Exception) || e is AggregateException) return null;
            return ErrorSource.Software;
        }

        /// <summary>Nguồn lỗi theo mã HTTP trả về.</summary>
        public static ErrorSource SourceFromHttpStatus(int statusCode)
        {
            if (statusCode == 401 || statusCode == 403 || statusCode == 404 || statusCode == 405) return ErrorSource.Config;
            if (statusCode == 408 || statusCode == 502 || statusCode == 503 || statusCode == 504) return ErrorSource.Connection;
            return ErrorSource.Partner;
        }

        /// <summary>
        /// Đoán nguồn lỗi từ nội dung message (Exception trần, thư viện ngoài gói lại, chuỗi lỗi đối tác trả về).
        /// </summary>
        public static ErrorSource ClassifyByMessage(string message)
        {
            if (String.IsNullOrWhiteSpace(message)) return ErrorSource.Software;
            string m = message.ToLower();

            string[] softwareKeys = { "object reference", "specified cast", "index was outside", "sequence contains", "input string was not", "nullable object",
                "key was not present", "method not found", "could not load file or assembly", "could not load type", "chưa hỗ trợ", "chưa tích hợp", "chua ho tro" };
            if (softwareKeys.Any(k => m.Contains(k))) return ErrorSource.Software;

            string[] workstationKeys = { "access to the path", "being used by another process", "there is not enough space", "disk full", "usb", "token not found",
                "chứng thư", "chung thu", "smart card", "thẻ thông minh", "keyset does not exist", "private key", "could not find file", "could not find a part of the path",
                "không tìm thấy thiết bị", "khong tim thay thiet bi", "cổng com", "com port", "serial port" };
            if (workstationKeys.Any(k => m.Contains(k))) return ErrorSource.Workstation;

            if (IsConnectionMessage(message)) return ErrorSource.Connection;
            string[] connectionKeys = { "kết nối", "ket noi", "network", "endpoint", "không trả về dữ liệu" };
            if (connectionKeys.Any(k => m.Contains(k))) return ErrorSource.Connection;

            string[] configKeys = { "(401)", "(403)", "(404)", "unauthorized", "forbidden", "cấu hình", "cau hinh", "webservice url", "config",
                "sai tên đăng nhập", "sai mật khẩu", "invalid username", "invalid password", "tài khoản không", "mật khẩu không", "invalid_client", "invalid_grant",
                "client secret", "clientid", "client_id", "khóa bí mật", "khoa bi mat", "đăng nhập thất bại", "dang nhap that bai", "xác thực thất bại", "chưa khai báo", "chưa khai tài khoản" };
            if (configKeys.Any(k => m.Contains(k))) return ErrorSource.Config;

            // Mã HTTP nằm trong chuỗi ("HTTP 500 - ...", "(HTTP 400 ...)")
            System.Text.RegularExpressions.Match http = System.Text.RegularExpressions.Regex.Match(message, @"HTTP\s*(\d{3})", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            if (http.Success)
            {
                int code;
                if (Int32.TryParse(http.Groups[1].Value, out code) && code >= 400) return SourceFromHttpStatus(code);
            }

            if (m.Contains("ký số") || m.Contains("ky so") || m.Contains("chữ ký số")) return ErrorSource.Workstation;
            if (m.Contains("không dựng được") || m.Contains("khong dung duoc")) return ErrorSource.Software;
            if (m.Contains("từ chối") || m.Contains("tu choi") || m.Contains("không đọc được phản hồi") || m.Contains("không tiếp nhận")) return ErrorSource.Partner;

            string[] partnerKeys = { "statuscode", "status code", "loi khi goi api", "lỗi khi gọi api", "trả về lỗi", "bad request", "internal server error",
                "(400)", "(500)", "soap", "fault" };
            if (partnerKeys.Any(k => m.Contains(k))) return ErrorSource.Partner;

            // Message tiếng Việt có dấu do code HIS tự ném khi kiểm tra dữ liệu → dữ liệu nghiệp vụ.
            // Còn lại (thường tiếng Anh từ thư viện) → hỗ trợ HIS phân tích log, không đoán bừa.
            return HasVietnameseDiacritics(message) ? ErrorSource.Business : ErrorSource.Software;
        }

        /// <summary>
        /// Chuỗi mô tả lỗi có phải lỗi kết nối kỹ thuật không (chỉ bắt từ khóa kỹ thuật rõ ràng).
        /// </summary>
        public static bool IsConnectionMessage(string message)
        {
            if (String.IsNullOrWhiteSpace(message)) return false;
            string m = message.ToLower();
            string[] keys = { "timed out", "time out", "timeout", "hết thời gian chờ", "unable to connect", "could not be resolved", "no such host",
                "actively refused", "secure channel", "ssl/tls", "the ssl connection", "could not establish trust", "network is unreachable",
                "(502)", "(503)", "(504)", "bad gateway", "service unavailable", "gateway timeout", "underlying connection was closed",
                "không nhận được phản hồi", "không kết nối được", "khong ket noi duoc", "connection was forcibly closed", "name resolution" };
            return keys.Any(k => m.Contains(k));
        }

        public static bool IsTimeout(Exception root)
        {
            if (root == null) return false;
            if (root is TimeoutException || root is TaskCanceledException || root is OperationCanceledException) return true;
            WebException webEx = root as WebException;
            if (webEx != null && webEx.Status == WebExceptionStatus.Timeout) return true;
            string m = (root.Message ?? "").ToLower();
            return m.Contains("timed out") || m.Contains("timeout") || m.Contains("hết thời gian chờ");
        }

        public static bool IsSecureChannel(Exception root)
        {
            if (root is System.Security.Authentication.AuthenticationException) return true;
            WebException webEx = root as WebException;
            return webEx != null && (webEx.Status == WebExceptionStatus.SecureChannelFailure || webEx.Status == WebExceptionStatus.TrustFailure);
        }

        private static bool HasVietnameseDiacritics(string text)
        {
            const string marks = "àáạảãâầấậẩẫăằắặẳẵèéẹẻẽêềếệểễìíịỉĩòóọỏõôồốộổỗơờớợởỡùúụủũưừứựửữỳýỵỷỹđ";
            string t = text.ToLower();
            return t.Any(c => marks.IndexOf(c) >= 0);
        }

        private static bool IsTypeOrSubclassOf(Exception e, string fullName)
        {
            Type t = e.GetType();
            while (t != null)
            {
                if (t.FullName == fullName) return true;
                t = t.BaseType;
            }
            return false;
        }

        /// <summary>
        /// Exception phát sinh ở bước đọc cấu hình (method Run/Check/...Config/Init/GetProvider) hay không,
        /// dựa vào frame đầu tiên ngoài framework trong stack trace. Không có stack trace → coi như bước cấu hình.
        /// </summary>
        private static bool IsConfigStage(Exception e)
        {
            string method = GetFirstAppMethodName(e);
            if (method == null) return true;
            return method.EndsWith("Run") || method.Contains("Check") || method.Contains("Config") || method.Contains("GetProvider") || method.StartsWith("Init") || method == ".ctor";
        }

        /// <summary>Tên method đầu tiên không thuộc framework (System/Microsoft/Newtonsoft) trong stack trace.</summary>
        public static string GetFirstAppMethodName(Exception e)
        {
            if (e == null || String.IsNullOrWhiteSpace(e.StackTrace)) return null;
            foreach (string raw in e.StackTrace.Split('\n'))
            {
                string line = raw.Trim();
                if (line.StartsWith("at ")) line = line.Substring(3);
                else
                {
                    int sp = line.IndexOf(' ');
                    if (sp < 0) continue;
                    line = line.Substring(sp + 1); // ngôn ngữ khác: "tại ..." / "bei ..."
                }
                if (line.StartsWith("System.") || line.StartsWith("Microsoft.") || line.StartsWith("Newtonsoft.") || line.StartsWith(OWN_NAMESPACE)) continue;
                int paren = line.IndexOf('(');
                if (paren > 0) line = line.Substring(0, paren);
                return line.Substring(line.LastIndexOf('.') + 1);
            }
            return null;
        }

        /// <summary>Danh sách exception từ ngoài vào trong (AggregateException lấy nhánh đầu tiên).</summary>
        public static List<Exception> Flatten(Exception ex)
        {
            List<Exception> result = new List<Exception>();
            Exception cur = ex;
            int guard = 0;
            while (cur != null && guard++ < 20)
            {
                result.Add(cur);
                AggregateException agg = cur as AggregateException;
                if (agg != null && agg.InnerExceptions != null && agg.InnerExceptions.Count > 0)
                {
                    cur = agg.InnerExceptions[0];
                }
                else
                {
                    cur = cur.InnerException;
                }
            }
            return result;
        }

        /// <summary>Exception gốc (trong cùng).</summary>
        public static Exception GetRoot(Exception ex)
        {
            if (ex == null) return null;
            List<Exception> chain = Flatten(ex);
            return chain.Count > 0 ? chain[chain.Count - 1] : ex;
        }

        /// <summary>Nguyên văn lỗi kỹ thuật cho phần "Chi tiết": message ngoài cùng + message gốc nếu khác.</summary>
        public static string GetDetail(Exception ex)
        {
            if (ex == null) return "";
            Exception root = GetRoot(ex);
            string outer = ex is AggregateException && ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            if (root != null && root != ex && !String.IsNullOrWhiteSpace(root.Message) && !String.Equals(root.Message, outer) && (outer == null || !outer.Contains(root.Message)))
            {
                return string.Format("{0} ({1})", outer, root.Message);
            }
            return outer;
        }
    }
}

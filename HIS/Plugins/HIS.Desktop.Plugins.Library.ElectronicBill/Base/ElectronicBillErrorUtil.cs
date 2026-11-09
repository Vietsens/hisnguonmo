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
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.ElectronicBill.Base
{
    /// <summary>
    /// Phân loại nguồn lỗi hóa đơn điện tử và dựng thông báo cho người dùng:
    /// [NGUỒN LỖI] mô tả. Liên hệ: ai. Chi tiết: ex.Message (vCong 62522).
    /// </summary>
    public class ElectronicBillErrorUtil
    {
        /// <summary>Số ký tự tối đa của phần "Chi tiết" đưa lên màn hình (phần còn lại vẫn có trong log).</summary>
        private const int DETAIL_MAX_LENGTH = 600;

        #region Nhãn, liên hệ

        public static string GetLabel(ElectronicBillErrorSource source)
        {
            switch (source)
            {
                case ElectronicBillErrorSource.Connection: return "LỖI KẾT NỐI MẠNG/CỔNG HÓA ĐƠN";
                case ElectronicBillErrorSource.Provider: return "LỖI PHÍA NHÀ CUNG CẤP HÓA ĐƠN ĐIỆN TỬ";
                case ElectronicBillErrorSource.Workstation: return "LỖI TẠI MÁY TRẠM";
                case ElectronicBillErrorSource.Config: return "THIẾT LẬP/CẤU HÌNH TẠI VIỆN CHƯA ĐÚNG";
                case ElectronicBillErrorSource.Business: return "DỮ LIỆU GIAO DỊCH CHƯA HỢP LỆ";
                default: return "LỖI CODE PHẦN MỀM HIS";
            }
        }

        public static string GetContact(ElectronicBillErrorSource source, string providerName)
        {
            string ncc = String.IsNullOrWhiteSpace(providerName) ? "nhà cung cấp hóa đơn điện tử" : "nhà cung cấp " + providerName;
            switch (source)
            {
                case ElectronicBillErrorSource.Connection:
                    return "IT bệnh viện kiểm tra mạng/Internet, tường lửa, proxy của máy trạm này (thử mở địa chỉ cổng hóa đơn bằng trình duyệt). Nếu mạng bình thường thì liên hệ " + ncc + " vì cổng có thể đang gián đoạn. Lưu ý: kiểm tra trên cổng xem hóa đơn đã được tạo chưa trước khi phát hành lại";
                case ElectronicBillErrorSource.Provider:
                    return ncc + " (gửi kèm nguyên văn nội dung lỗi bên dưới). Nếu lỗi nói về dữ liệu hóa đơn (mã số thuế, tên người mua, số tiền, ký hiệu...) thì sửa lại dữ liệu trên HIS rồi phát hành lại";
                case ElectronicBillErrorSource.Workstation:
                    return "IT bệnh viện kiểm tra máy trạm này: quyền ghi file/thư mục tạm, ổ đĩa còn trống, USB token/chứng thư số đã cắm và còn hạn, thư viện ký số đã cài. Lỗi chỉ xảy ra trên máy này, thử lại trên máy khác để đối chiếu";
                case ElectronicBillErrorSource.Config:
                    return "quản trị HIS tại viện hoặc đội triển khai kiểm tra cấu hình hóa đơn điện tử (địa chỉ cổng, tài khoản, mã đơn vị, mẫu số/ký hiệu, tham số kèm theo)";
                case ElectronicBillErrorSource.Business:
                    return "người dùng kiểm tra lại giao dịch/dữ liệu theo nội dung lỗi. Nếu dữ liệu đã đúng mà vẫn lỗi thì liên hệ hỗ trợ phần mềm HIS";
                default:
                    return "hỗ trợ phần mềm HIS (gửi kèm nội dung lỗi, tên máy và thời điểm thao tác để tra log). Người dùng không cần liên hệ " + ncc;
            }
        }

        /// <summary>
        /// Tên hiển thị của nhà cung cấp từ mã ProviderType.*
        /// </summary>
        public static string GetProviderName(string provider)
        {
            if (String.IsNullOrWhiteSpace(provider)) return "";
            switch (provider.Trim().ToUpper())
            {
                case ProviderType.VNPT: return "VNPT";
                case ProviderType.VIETTEL: return "Viettel";
                case ProviderType.BKAV: return "BKAV";
                case ProviderType.MISA: return "MISA";
                case ProviderType.SoftDream: return "SoftDreams (EasyInvoice)";
                case ProviderType.CongThuong: return "Bộ Công Thương";
                case ProviderType.safecert: return "SafeCert (TS24)";
                case ProviderType.CYBERBILL: return "CyberBill";
                case ProviderType.MINVOICE: return "M-Invoice";
                case ProviderType.VNINVOICE: return "VNInvoice";
                case ProviderType.MOBIFONE: return "MobiFone";
                case ProviderType.CTO: return "cổng trung gian CTO";
                case ProviderType.BACH_MAI: return "cổng trung gian Bạch Mai";
                case ProviderType.VIETSENS: return "VietSens";
                default: return provider;
            }
        }

        #endregion

        #region Dựng thông báo

        /// <summary>
        /// Dựng 1 dòng thông báo đầy đủ cho người dùng.
        /// </summary>
        /// <param name="source">Nguồn lỗi</param>
        /// <param name="provider">Mã ProviderType.* (có thể null)</param>
        /// <param name="summary">Mô tả ngắn, tiếng Việt, dễ hiểu</param>
        /// <param name="detail">Nguyên văn lỗi kỹ thuật (ex.Message / nội dung API trả về)</param>
        public static string BuildMessage(ElectronicBillErrorSource source, string provider, string summary, string detail)
        {
            string providerName = GetProviderName(provider);
            StringBuilder sb = new StringBuilder();
            sb.Append("[").Append(GetLabel(source));
            if (!String.IsNullOrWhiteSpace(providerName))
            {
                sb.Append(" - ").Append(providerName);
            }
            sb.Append("] ");
            if (!String.IsNullOrWhiteSpace(summary))
            {
                sb.Append(summary.Trim().TrimEnd('.')).Append(". ");
            }
            sb.Append("Liên hệ: ").Append(GetContact(source, providerName)).Append(".");
            detail = Shorten(detail);
            if (!String.IsNullOrWhiteSpace(detail))
            {
                sb.Append(" Chi tiết: ").Append(detail);
            }
            return sb.ToString();
        }

        private static string Shorten(string detail)
        {
            if (String.IsNullOrWhiteSpace(detail)) return "";
            detail = detail.Trim();
            if (detail.Length > DETAIL_MAX_LENGTH)
            {
                detail = detail.Substring(0, DETAIL_MAX_LENGTH) + "... (xem đầy đủ trong log)";
            }
            return detail;
        }

        /// <summary>
        /// Mô tả ngắn mặc định theo nguồn lỗi và loại exception.
        /// </summary>
        public static string GetDefaultSummary(ElectronicBillErrorSource source, Exception ex)
        {
            Exception root = GetRoot(ex);
            switch (source)
            {
                case ElectronicBillErrorSource.Connection:
                    if (IsTimeout(root)) return "Hết thời gian chờ phản hồi từ cổng hóa đơn điện tử";
                    if (root is System.Security.Authentication.AuthenticationException || (root is WebException && IsSecureChannel((WebException)root)))
                        return "Không thiết lập được kết nối bảo mật (SSL/TLS) tới cổng hóa đơn điện tử";
                    return "Không kết nối được tới cổng hóa đơn điện tử";
                case ElectronicBillErrorSource.Provider:
                    if (root is Newtonsoft.Json.JsonException) return "Cổng hóa đơn điện tử trả về dữ liệu không đúng định dạng";
                    return "Cổng hóa đơn điện tử trả về lỗi";
                case ElectronicBillErrorSource.Workstation:
                    if (root is System.Security.Cryptography.CryptographicException) return "Lỗi chứng thư số/USB token trên máy trạm";
                    if (root is OutOfMemoryException) return "Máy trạm hết bộ nhớ";
                    if (root is UnauthorizedAccessException) return "Máy trạm không có quyền ghi file";
                    return "Lỗi đọc/ghi file hoặc thiết bị trên máy trạm";
                case ElectronicBillErrorSource.Config:
                    if (root is IndexOutOfRangeException) return "Chuỗi cấu hình hóa đơn điện tử thiếu tham số";
                    if (root is UriFormatException) return "Địa chỉ cổng hóa đơn điện tử trong cấu hình sai định dạng";
                    if (root is FormatException) return "Tham số cấu hình hóa đơn điện tử sai định dạng";
                    return "Cấu hình hóa đơn điện tử chưa đúng";
                case ElectronicBillErrorSource.Business:
                    return "";
                default:
                    if (root is TypeLoadException || root is FileLoadException || root is MissingMemberException || root is DllNotFoundException || root is BadImageFormatException)
                        return "Thiếu hoặc lệch phiên bản thư viện của phần mềm trên máy trạm";
                    return "Phần mềm gặp lỗi xử lý nội bộ";
            }
        }

        #endregion

        #region Phân loại exception

        /// <summary>
        /// Xác định nguồn lỗi từ exception (đi hết chuỗi InnerException / AggregateException).
        /// </summary>
        public static ElectronicBillErrorSource Classify(Exception ex)
        {
            if (ex == null) return ElectronicBillErrorSource.Software;

            foreach (Exception e in Flatten(ex))
            {
                ElectronicBillException ebEx = e as ElectronicBillException;
                if (ebEx != null) return ebEx.Source;
            }

            // Ưu tiên theo loại exception cụ thể nhất (từ trong ra ngoài)
            List<Exception> chain = Flatten(ex);
            chain.Reverse();
            foreach (Exception e in chain)
            {
                ElectronicBillErrorSource? byType = ClassifyByType(e);
                if (byType.HasValue) return byType.Value;
            }

            // Exception "trần" (System.Exception) do code tự ném, hoặc thư viện ngoài gói lại → đoán theo nội dung
            return ClassifyByMessage(ex.Message);
        }

        private static ElectronicBillErrorSource? ClassifyByType(Exception e)
        {
            if (e == null) return null;

            // Kết nối / HTTP (thư viện SOAP của VNPT, BKAV... ném WebException)
            WebException webEx = e as WebException;
            if (webEx != null)
            {
                if (webEx.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse httpResponse = webEx.Response as HttpWebResponse;
                    int code = httpResponse != null ? (int)httpResponse.StatusCode : 0;
                    if (code == 401 || code == 403 || code == 404) return ElectronicBillErrorSource.Config;
                    if (code == 502 || code == 503 || code == 504) return ElectronicBillErrorSource.Connection;
                    return ElectronicBillErrorSource.Provider;
                }
                return ElectronicBillErrorSource.Connection;
            }
            if (e is SocketException || e is TimeoutException || e is TaskCanceledException || e is OperationCanceledException
                || e is HttpRequestException || e is System.Security.Authentication.AuthenticationException)
                return ElectronicBillErrorSource.Connection;

            // WCF / SOAP: so theo tên kiểu để không phải tham chiếu System.ServiceModel, System.Web.Services
            string typeName = e.GetType().FullName ?? "";
            if (typeName.StartsWith("System.ServiceModel.FaultException") || typeName.StartsWith("System.Web.Services.Protocols.Soap"))
                return ElectronicBillErrorSource.Provider; // cổng nhận yêu cầu và trả SOAP Fault
            if (typeName.StartsWith("System.ServiceModel."))
                return ElectronicBillErrorSource.Connection; // EndpointNotFound, Communication, ServerTooBusy, MessageSecurity...

            // Nhà cung cấp trả dữ liệu không đúng định dạng
            if (e is Newtonsoft.Json.JsonException || e is System.Xml.XmlException) return ElectronicBillErrorSource.Provider;
            if (e is InvalidOperationException)
            {
                string m = (e.Message ?? "").ToLower();
                // SoapHttpClientProtocol: cổng/proxy trả HTML thay vì XML, hoặc XML sai cấu trúc
                if (m.Contains("response content type") || m.Contains("error in xml document") || m.Contains("client found response"))
                    return ElectronicBillErrorSource.Provider;
            }

            // Máy trạm
            if (e is System.Security.Cryptography.CryptographicException || e is UnauthorizedAccessException
                || e is System.Runtime.InteropServices.COMException || e is OutOfMemoryException
                || e is DriveNotFoundException || e is DirectoryNotFoundException || e is FileNotFoundException || e is PathTooLongException)
                return ElectronicBillErrorSource.Workstation;
            if (e is IOException && !(e is FileLoadException)) return ElectronicBillErrorSource.Workstation;

            // Địa chỉ cổng trong cấu hình sai định dạng
            if (e is UriFormatException) return ElectronicBillErrorSource.Config;
            // Chuỗi cấu hình split thiếu phần tử / tham số sai định dạng: chỉ coi là cấu hình khi phát sinh ở bước đọc cấu hình (Run/Check),
            // còn phát sinh khi dựng dữ liệu hóa đơn hoặc đọc kết quả thì là lỗi code
            if (e is IndexOutOfRangeException || e is FormatException || e is OverflowException)
                return IsConfigStage(e) ? ElectronicBillErrorSource.Config : ElectronicBillErrorSource.Software;

            // Phần mềm
            if (e is TypeLoadException || e is FileLoadException || e is MissingMemberException || e is DllNotFoundException || e is BadImageFormatException
                || e is NullReferenceException || e is InvalidCastException || e is ArgumentException || e is InvalidOperationException
                || e is KeyNotFoundException || e is NotImplementedException || e is NotSupportedException || e is ArithmeticException)
                return ElectronicBillErrorSource.Software;

            if (e.GetType() == typeof(Exception) || e is AggregateException) return null; // để ClassifyByMessage quyết định
            return ElectronicBillErrorSource.Software;
        }

        /// <summary>
        /// Đoán nguồn lỗi từ nội dung message (dùng cho Exception trần, thư viện ngoài gói lại).
        /// </summary>
        public static ElectronicBillErrorSource ClassifyByMessage(string message)
        {
            if (String.IsNullOrWhiteSpace(message)) return ElectronicBillErrorSource.Software;
            string m = message.ToLower();

            // Thứ tự quan trọng: lỗi code / máy trạm có từ khóa đặc trưng, kiểm trước để không bị "token", "connection" nuốt nhầm
            string[] softwareKeys = { "object reference", "specified cast", "index was outside", "sequence contains", "input string was not", "nullable object",
                "key was not present", "method not found", "could not load file or assembly", "could not load type", "chưa hỗ trợ", "chưa tích hợp", "chua ho tro" };
            if (softwareKeys.Any(k => m.Contains(k))) return ElectronicBillErrorSource.Software;

            string[] workstationKeys = { "access to the path", "being used by another process", "there is not enough space", "disk full", "usb", "token not found",
                "chứng thư", "smart card", "thẻ thông minh", "keyset does not exist", "private key", "could not find file", "could not find a part of the path" };
            if (workstationKeys.Any(k => m.Contains(k))) return ElectronicBillErrorSource.Workstation;

            string[] connectionKeys = { "timed out", "time out", "timeout", "hết thời gian chờ", "unable to connect", "could not be resolved", "no such host",
                "actively refused", "underlying connection", "kết nối", "ket noi", "ssl", "tls", "secure channel", "network", "name resolution", "unreachable",
                "gateway timeout", "service unavailable", "bad gateway", "(502)", "(503)", "(504)", "không trả về dữ liệu", "không nhận được phản hồi", "endpoint" };
            if (connectionKeys.Any(k => m.Contains(k))) return ElectronicBillErrorSource.Connection;

            // Cổng từ chối xác thực / không có đường dẫn = thiết lập tài khoản, địa chỉ tại viện chưa đúng
            string[] configKeys = { "(401)", "(403)", "(404)", "unauthorized", "forbidden", "cấu hình", "cau hinh", "webservice url", "config",
                "sai tên đăng nhập", "sai mật khẩu", "invalid username", "invalid password", "tài khoản không", "mật khẩu không" };
            if (configKeys.Any(k => m.Contains(k))) return ElectronicBillErrorSource.Config;

            string[] providerKeys = { "statuscode", "status code", "loi khi goi api", "lỗi khi gọi api", "trả về lỗi", "bad request", "internal server error",
                "(400)", "(500)", "soap", "fault" };
            if (providerKeys.Any(k => m.Contains(k))) return ElectronicBillErrorSource.Provider;

            // Message tiếng Việt có dấu do code HIS tự ném khi kiểm tra dữ liệu → dữ liệu giao dịch.
            // Message không nhận diện được còn lại (thường tiếng Anh từ thư viện) → chuyển hỗ trợ HIS phân tích log, không đoán bừa.
            return HasVietnameseDiacritics(message) ? ElectronicBillErrorSource.Business : ElectronicBillErrorSource.Software;
        }

        private static bool HasVietnameseDiacritics(string text)
        {
            const string marks = "àáạảãâầấậẩẫăằắặẳẵèéẹẻẽêềếệểễìíịỉĩòóọỏõôồốộổỗơờớợởỡùúụủũưừứựửữỳýỵỷỹđ";
            string t = text.ToLower();
            return t.Any(c => marks.IndexOf(c) >= 0);
        }

        /// <summary>
        /// Exception phát sinh ở bước đọc cấu hình (hàm Run/Check/GetProvider/...Config) của thư viện hay không,
        /// dựa vào frame đầu tiên thuộc thư viện này trong stack trace.
        /// Không có stack trace (exception chưa từng được ném) → coi như bước cấu hình.
        /// </summary>
        private static bool IsConfigStage(Exception e)
        {
            string method = GetOwnMethodName(e);
            if (method == null) return true;
            return method.EndsWith("Run") || method.Contains("Check") || method.Contains("Config") || method.Contains("GetProvider");
        }

        /// <summary>Tên method đầu tiên thuộc thư viện ElectronicBill trong stack trace (null nếu không có).</summary>
        public static string GetOwnMethodName(Exception e)
        {
            if (e == null || String.IsNullOrWhiteSpace(e.StackTrace)) return null;
            const string ns = "HIS.Desktop.Plugins.Library.ElectronicBill.";
            foreach (string raw in e.StackTrace.Split('\n'))
            {
                string line = raw.Trim();
                int idx = line.IndexOf(ns);
                if (idx < 0 || line.Contains("ElectronicBillErrorUtil") || line.Contains("ApiConsumer")) continue;
                string frame = line.Substring(idx);
                int paren = frame.IndexOf('(');
                if (paren > 0) frame = frame.Substring(0, paren);
                return frame.Substring(frame.LastIndexOf('.') + 1);
            }
            return null;
        }

        /// <summary>
        /// Chuỗi mô tả lỗi (thường là ex.Message do thư viện ngoài gói lại) có phải lỗi kết nối kỹ thuật không.
        /// Chỉ bắt từ khóa kỹ thuật rõ ràng để không nhầm với lỗi nghiệp vụ nhà cung cấp mô tả bằng tiếng Việt.
        /// </summary>
        public static bool IsConnectionMessage(string message)
        {
            if (String.IsNullOrWhiteSpace(message)) return false;
            string m = message.ToLower();
            string[] keys = { "timed out", "time out", "timeout", "hết thời gian chờ", "unable to connect", "could not be resolved", "no such host",
                "actively refused", "secure channel", "ssl/tls", "the ssl connection", "could not establish trust", "network is unreachable",
                "the remote server returned an error: (502", "(503)", "(504)", "underlying connection was closed", "không nhận được phản hồi", "không kết nối được" };
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

        private static bool IsSecureChannel(WebException webEx)
        {
            return webEx.Status == WebExceptionStatus.SecureChannelFailure || webEx.Status == WebExceptionStatus.TrustFailure;
        }

        /// <summary>Exception gốc (trong cùng).</summary>
        public static Exception GetRoot(Exception ex)
        {
            if (ex == null) return null;
            List<Exception> chain = Flatten(ex);
            return chain.Count > 0 ? chain[chain.Count - 1] : ex;
        }

        /// <summary>Danh sách exception từ ngoài vào trong (AggregateException lấy nhánh đầu tiên).</summary>
        private static List<Exception> Flatten(Exception ex)
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

        /// <summary>
        /// Nguyên văn lỗi kỹ thuật để đưa vào phần "Chi tiết": message ngoài cùng + message gốc nếu khác.
        /// </summary>
        public static string GetDetail(Exception ex)
        {
            if (ex == null) return "";
            Exception root = GetRoot(ex);
            string outer = ex is AggregateException && ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            if (root != null && root != ex && !String.IsNullOrWhiteSpace(root.Message) && !String.Equals(root.Message, outer))
            {
                return string.Format("{0} ({1})", outer, root.Message);
            }
            return outer;
        }

        #endregion

        #region Dựng thông báo từ exception

        /// <summary>
        /// Dựng thông báo đầy đủ từ exception (phân loại + mô tả + liên hệ + chi tiết).
        /// </summary>
        public static string BuildFromException(Exception ex, string provider, string action)
        {
            ElectronicBillErrorSource source = Classify(ex);
            string summary = GetDefaultSummary(source, ex);
            string detail = GetDetail(ex);
            ElectronicBillException known = ex as ElectronicBillException;
            if (known != null)
            {
                // Exception do ApiConsumer dựng sẵn: message đã là mô tả cụ thể (mã HTTP, URL, lý do) → dùng làm câu chính
                summary = known.Message;
                Exception root = GetRoot(known);
                detail = known.InnerException != null && root != null && !summary.Contains(root.Message ?? "")
                    ? GetDetail(known.InnerException)
                    : null;
                if (String.IsNullOrWhiteSpace(provider)) provider = known.Provider;
            }
            else if (source == ElectronicBillErrorSource.Business)
            {
                // Exception trần do code tự ném để báo dữ liệu chưa hợp lệ → message chính là mô tả
                summary = detail;
                detail = null;
            }
            if (!String.IsNullOrWhiteSpace(action) && !String.IsNullOrWhiteSpace(summary) && known == null)
            {
                summary = string.Format("{0} khi {1}", summary, action);
            }
            return BuildMessage(source, provider, summary, detail);
        }

        /// <summary>Chuỗi đã là thông báo do bộ này dựng (tránh bọc 2 lần).</summary>
        public static bool IsBuiltMessage(string message)
        {
            return !String.IsNullOrWhiteSpace(message) && message.StartsWith("[") && message.Contains("] ") && message.Contains("Liên hệ: ");
        }

        #endregion

        #region Lỗi bị nuốt khi dựng dữ liệu (vCong 62522)

        // Các hàm dựng dữ liệu hóa đơn (người mua, dòng hàng, tổng tiền, mẫu in...) khi lỗi chỉ ghi log rồi trả null/rỗng,
        // request vẫn được gửi đi thiếu dữ liệu → cổng từ chối → người dùng tưởng lỗi nhà cung cấp.
        // Ghi lại lỗi đầu tiên trong luồng để cuối ElectronicBillProcessor.Run báo đúng nguồn là lỗi code HIS / máy trạm.
        // ThreadStatic vì 1 lượt Run chạy đồng bộ trên 1 luồng; các luồng phụ (lưu file nền) không ảnh hưởng luồng chính.
        [ThreadStatic]
        private static Exception silentError;
        [ThreadStatic]
        private static string silentErrorStage;

        public static void ResetSilentError()
        {
            silentError = null;
            silentErrorStage = null;
        }

        /// <summary>
        /// Gọi trong catch chỉ ghi log của hàm dựng dữ liệu. Chỉ giữ lỗi đầu tiên (thường là gốc).
        /// </summary>
        public static void NoteSilentError(Exception ex, string stage)
        {
            try
            {
                if (ex != null && silentError == null)
                {
                    silentError = ex;
                    silentErrorStage = stage;
                }
            }
            catch { }
        }

        public static Exception GetSilentError() { return silentError; }

        public static string GetSilentErrorStage() { return silentErrorStage; }

        /// <summary>
        /// Thông báo cho lỗi bị nuốt: máy trạm nếu là lỗi file/thiết bị, còn lại là lỗi code HIS.
        /// </summary>
        public static string BuildSilentErrorMessage(string provider)
        {
            Exception ex = silentError;
            if (ex == null) return null;
            ElectronicBillErrorSource source = Classify(ex);
            if (source != ElectronicBillErrorSource.Workstation) source = ElectronicBillErrorSource.Software;
            string stage = String.IsNullOrWhiteSpace(silentErrorStage) ? "dựng dữ liệu hóa đơn" : silentErrorStage;
            string summary = source == ElectronicBillErrorSource.Workstation
                ? string.Format("{0} khi {1}", GetDefaultSummary(source, ex), stage)
                : string.Format("Phần mềm HIS gặp lỗi khi {0} nên dữ liệu gửi sang cổng hóa đơn có thể thiếu hoặc sai. Lỗi phía cổng (nếu có) bên dưới có thể là hệ quả của lỗi này", stage);
            return BuildMessage(source, provider, summary, GetDetail(ex));
        }

        #endregion

        #region Hỗ trợ cho ApiConsumer (gói exception HTTP về đúng nguồn)

        /// <summary>
        /// Gói exception phát sinh khi gọi HTTP (HttpClient .Result, HttpWebRequest) thành ElectronicBillException có nguồn.
        /// Nếu đã là ElectronicBillException thì trả về nguyên.
        /// </summary>
        public static Exception WrapHttpCall(Exception ex, string provider, string uri)
        {
            if (ex == null) return new ElectronicBillException(ElectronicBillErrorSource.Software, provider, "Lỗi không xác định khi gọi cổng hóa đơn điện tử", null);
            if (Flatten(ex).Any(o => o is ElectronicBillException)) return ex;

            ElectronicBillErrorSource source = Classify(ex);
            Exception root = GetRoot(ex);
            string msg;
            if (source == ElectronicBillErrorSource.Connection)
            {
                msg = IsTimeout(root)
                    ? string.Format("Hết thời gian chờ phản hồi từ cổng hóa đơn điện tử ({0}). {1}", uri, root.Message)
                    : string.Format("Lỗi kết nối cổng hóa đơn điện tử ({0}): {1}", uri, root.Message);
            }
            else
            {
                msg = string.Format("Lỗi khi gọi cổng hóa đơn điện tử ({0}): {1}", uri, root.Message);
            }
            return new ElectronicBillException(source, provider, msg, ex);
        }

        /// <summary>
        /// Cổng trả HTTP status không thành công → lỗi phía nhà cung cấp, kèm body để người dùng gửi cho họ.
        /// </summary>
        public static ElectronicBillException HttpStatusError(string provider, string uri, int statusCode, string reason, string body)
        {
            string detail = String.IsNullOrWhiteSpace(body) ? reason : body;
            ElectronicBillErrorSource source;
            string summary;
            switch (statusCode)
            {
                case 502:
                case 503:
                case 504:
                    // Cổng/proxy của nhà cung cấp không phục vụ được
                    source = ElectronicBillErrorSource.Connection;
                    summary = string.Format("Cổng hóa đơn điện tử không phục vụ được (mã {0}, {1})", statusCode, uri);
                    break;
                case 401:
                case 403:
                    // Cổng từ chối xác thực: tài khoản/mật khẩu/token/mã đơn vị thiết lập trong HIS không đúng
                    source = ElectronicBillErrorSource.Config;
                    summary = string.Format("Cổng hóa đơn điện tử từ chối xác thực (mã {0}): tài khoản, mật khẩu, token hoặc mã đơn vị thiết lập trong HIS không đúng", statusCode);
                    break;
                case 404:
                    // Sai địa chỉ cổng/đường dẫn API trong cấu hình (hoặc nhà cung cấp đổi API)
                    source = ElectronicBillErrorSource.Config;
                    summary = string.Format("Không tìm thấy đường dẫn API trên cổng hóa đơn điện tử (mã 404, {0}): kiểm tra địa chỉ cổng trong cấu hình; nếu địa chỉ đúng thì nhà cung cấp đã đổi API", uri);
                    break;
                default:
                    source = ElectronicBillErrorSource.Provider;
                    summary = string.Format("Cổng hóa đơn điện tử trả về mã {0} ({1})", statusCode, uri);
                    break;
            }
            return new ElectronicBillException(source, provider, statusCode, string.Format("{0}: {1}", summary, Shorten(detail)));
        }

        /// <summary>
        /// Cổng trả 200 nhưng nội dung không đọc được theo cấu trúc mong đợi.
        /// </summary>
        public static ElectronicBillException ParseError(string provider, string uri, string body, Exception inner)
        {
            return new ElectronicBillException(ElectronicBillErrorSource.Provider, provider,
                string.Format("Cổng hóa đơn điện tử ({0}) trả về dữ liệu không đúng định dạng: {1}", uri, Shorten(body)), inner);
        }

        /// <summary>
        /// Không nhận được phản hồi (null) từ thư viện/cổng.
        /// </summary>
        public static ElectronicBillException NoResponse(string provider, string action)
        {
            return new ElectronicBillException(ElectronicBillErrorSource.Connection, provider,
                string.Format("Không nhận được phản hồi từ cổng hóa đơn điện tử khi {0}", action), null);
        }

        #endregion
    }
}

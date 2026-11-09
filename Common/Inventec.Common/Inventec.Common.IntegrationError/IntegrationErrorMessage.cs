using System;
using System.IO;
using System.Text;

namespace Inventec.Common.IntegrationError
{
    /// <summary>
    /// Dựng thông báo lỗi cho người dùng theo khuôn:
    /// [NGUỒN LỖI - tên đối tác] mô tả. Liên hệ: ai. Chi tiết: nguyên văn lỗi kỹ thuật (ex.Message / nội dung đối tác trả về).
    /// </summary>
    public static class IntegrationErrorMessage
    {
        /// <summary>Số ký tự tối đa của phần "Chi tiết" đưa lên màn hình (bản đầy đủ vẫn có trong log).</summary>
        private const int DETAIL_MAX_LENGTH = 600;
        private const string DEFAULT_PARTNER_NAME = "hệ thống đối tác";

        #region Nhãn, liên hệ

        public static string GetLabel(ErrorSource source, PartnerKind kind)
        {
            switch (source)
            {
                case ErrorSource.Connection: return "LỖI KẾT NỐI";
                case ErrorSource.Partner: return GetPartnerLabel(kind);
                case ErrorSource.Workstation: return "LỖI TẠI MÁY TRẠM";
                case ErrorSource.Config: return "THIẾT LẬP/CẤU HÌNH TẠI VIỆN CHƯA ĐÚNG";
                case ErrorSource.Business: return "DỮ LIỆU CHƯA HỢP LỆ";
                default: return "LỖI CODE PHẦN MỀM HIS";
            }
        }

        private static string GetPartnerLabel(PartnerKind kind)
        {
            switch (kind)
            {
                case PartnerKind.EInvoice: return "LỖI PHÍA NHÀ CUNG CẤP HÓA ĐƠN ĐIỆN TỬ";
                case PartnerKind.Bhxh: return "LỖI PHÍA CỔNG GIÁM ĐỊNH BHXH";
                case PartnerKind.DigitalSign: return "LỖI PHÍA NHÀ CUNG CẤP CHỮ KÝ SỐ";
                case PartnerKind.Bank: return "LỖI PHÍA NGÂN HÀNG";
                case PartnerKind.HealthPortal: return "LỖI PHÍA CỔNG DỮ LIỆU Y TẾ";
                case PartnerKind.Pharmacy: return "LỖI PHÍA CỔNG ĐƠN THUỐC QUỐC GIA";
                case PartnerKind.Lis: return "LỖI PHÍA HỆ THỐNG XÉT NGHIỆM (LIS)";
                case PartnerKind.Pacs: return "LỖI PHÍA HỆ THỐNG PACS";
                case PartnerKind.Device: return "LỖI THIẾT BỊ";
                case PartnerKind.AiService: return "LỖI PHÍA DỊCH VỤ AI/TRA CỨU";
                case PartnerKind.FileStorage: return "LỖI MÁY CHỦ LƯU TRỮ FILE";
                case PartnerKind.LocalService: return "LỖI DỊCH VỤ TRÊN MÁY TRẠM";
                case PartnerKind.Emr: return "LỖI PHÍA HỆ THỐNG BỆNH ÁN ĐIỆN TỬ";
                default: return "LỖI PHÍA HỆ THỐNG ĐỐI TÁC";
            }
        }

        /// <summary>Cách gọi đơn vị chịu trách nhiệm phía đối tác, VD "nhà cung cấp chữ ký số Viettel-CA".</summary>
        public static string GetPartnerRole(IntegrationPartner partner)
        {
            string name = GetName(partner);
            PartnerKind kind = partner != null ? partner.Kind : PartnerKind.Other;
            switch (kind)
            {
                case PartnerKind.EInvoice: return "nhà cung cấp hóa đơn điện tử " + name;
                case PartnerKind.Bhxh: return "cơ quan BHXH (bộ phận giám định)";
                case PartnerKind.DigitalSign: return "nhà cung cấp chữ ký số " + name;
                case PartnerKind.Bank: return name.StartsWith("ngân hàng", StringComparison.OrdinalIgnoreCase) ? name : "ngân hàng " + name;
                case PartnerKind.HealthPortal: return "đơn vị quản lý " + name + " (Sở/Bộ Y tế)";
                case PartnerKind.Pharmacy: return "đơn vị quản lý cổng liên thông đơn thuốc";
                case PartnerKind.Lis: return "nhà cung cấp " + name;
                case PartnerKind.Pacs: return "nhà cung cấp " + name;
                case PartnerKind.Device: return "hãng/đơn vị bảo trì " + name;
                case PartnerKind.AiService: return "nhà cung cấp " + name;
                case PartnerKind.FileStorage: return "IT bệnh viện (quản trị máy chủ lưu trữ file)";
                case PartnerKind.LocalService: return "IT bệnh viện";
                case PartnerKind.Emr: return "nhà cung cấp " + name;
                default: return "đơn vị cung cấp " + name;
            }
        }

        public static string GetContact(ErrorSource source, IntegrationPartner partner)
        {
            string name = GetName(partner);
            string role = GetPartnerRole(partner);
            PartnerKind kind = partner != null ? partner.Kind : PartnerKind.Other;
            switch (source)
            {
                case ErrorSource.Connection:
                    if (kind == PartnerKind.LocalService || kind == PartnerKind.Device)
                        return string.Format("IT bệnh viện kiểm tra {0} trên máy trạm này: đã cài, đang chạy, không bị tường lửa hoặc phần mềm diệt virus chặn", name);
                    if (kind == PartnerKind.FileStorage)
                        return "IT bệnh viện kiểm tra mạng nội bộ và máy chủ lưu trữ file (FSS) đang hoạt động";
                    string conn = string.Format("IT bệnh viện kiểm tra mạng/Internet, tường lửa, proxy của máy trạm này (thử mở địa chỉ {0} bằng trình duyệt). Nếu mạng bình thường thì liên hệ {1} vì hệ thống của họ có thể đang gián đoạn", name, role);
                    if (IsSendKind(kind))
                        conn += ". Lưu ý: kiểm tra trên hệ thống đối tác xem yêu cầu đã được ghi nhận chưa trước khi thực hiện lại, tránh gửi trùng";
                    return conn;
                case ErrorSource.Partner:
                    if (partner != null && !String.IsNullOrWhiteSpace(partner.Contact)) return partner.Contact;
                    return role + " (gửi kèm nguyên văn nội dung lỗi bên dưới)" + GetPartnerDataHint(kind);
                case ErrorSource.Workstation:
                    if (kind == PartnerKind.Device)
                        return string.Format("IT bệnh viện kiểm tra {0} tại máy trạm này: thiết bị đã bật, cáp/cổng COM/USB, driver, ứng dụng kết nối thiết bị đang chạy. Lỗi chỉ xảy ra trên máy này, thử lại trên máy khác để đối chiếu", name);
                    if (kind == PartnerKind.LocalService)
                        return string.Format("IT bệnh viện kiểm tra {0} tại máy trạm này: đã cài đúng đường dẫn, đang chạy, không bị phần mềm diệt virus chặn", name);
                    return "IT bệnh viện kiểm tra máy trạm này: quyền ghi file/thư mục tạm, ổ đĩa còn trống, USB token/chứng thư số đã cắm và còn hạn, thiết bị và driver, thư viện ký số đã cài. Lỗi chỉ xảy ra trên máy này, thử lại trên máy khác để đối chiếu";
                case ErrorSource.Config:
                    string cfg = string.Format("quản trị HIS tại viện hoặc đội triển khai kiểm tra thiết lập kết nối {0} (địa chỉ, tài khoản, mật khẩu, mã đơn vị, khóa bí mật, tham số kèm theo)", name);
                    if (kind == PartnerKind.DigitalSign)
                        cfg += ", và thông tin tài khoản ký số của người ký (mã định danh, mã PIN, chứng thư)";
                    return cfg;
                case ErrorSource.Business:
                    return "người dùng kiểm tra lại dữ liệu theo nội dung lỗi. Nếu dữ liệu đã đúng mà vẫn lỗi thì liên hệ hỗ trợ phần mềm HIS";
                default:
                    return string.Format("hỗ trợ phần mềm HIS (gửi kèm nội dung lỗi, tên máy và thời điểm thao tác để tra log). Người dùng không cần liên hệ {0}", role);
            }
        }

        private static string GetPartnerDataHint(PartnerKind kind)
        {
            switch (kind)
            {
                case PartnerKind.EInvoice: return ". Nếu lỗi nói về dữ liệu hóa đơn (mã số thuế, tên người mua, số tiền, ký hiệu...) thì sửa lại dữ liệu trên HIS rồi phát hành lại";
                case PartnerKind.Bhxh: return ". Nếu lỗi nói về dữ liệu hồ sơ (thẻ BHYT, mã bệnh, dịch vụ, thuốc, chi phí...) thì sửa hồ sơ trên HIS rồi gửi lại";
                case PartnerKind.DigitalSign: return ". Nếu lỗi nói về phiên ký, chứng thư hết hạn hoặc chưa xác nhận trên điện thoại thì người ký xử lý trên ứng dụng ký số rồi ký lại";
                case PartnerKind.HealthPortal:
                case PartnerKind.Pharmacy: return ". Nếu lỗi nói về dữ liệu hồ sơ/đơn thuốc thì sửa lại trên HIS rồi gửi lại";
                case PartnerKind.Bank: return ". Nếu lỗi nói về số tiền, tài khoản hoặc trạng thái giao dịch thì kiểm tra lại giao dịch trên HIS và với ngân hàng trước khi thực hiện lại";
                default: return "";
            }
        }

        private static bool IsSendKind(PartnerKind kind)
        {
            return kind == PartnerKind.EInvoice || kind == PartnerKind.Bank || kind == PartnerKind.Bhxh || kind == PartnerKind.HealthPortal
                || kind == PartnerKind.Pharmacy || kind == PartnerKind.Lis || kind == PartnerKind.Pacs;
        }

        public static string GetName(IntegrationPartner partner)
        {
            return partner == null || String.IsNullOrWhiteSpace(partner.Name) ? DEFAULT_PARTNER_NAME : partner.Name.Trim();
        }

        #endregion

        #region Dựng thông báo

        /// <summary>Dựng 1 dòng thông báo đầy đủ.</summary>
        public static string Build(ErrorSource source, IntegrationPartner partner, string summary, string detail)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[").Append(GetLabel(source, partner != null ? partner.Kind : PartnerKind.Other));
            if (partner != null && !String.IsNullOrWhiteSpace(partner.Name))
            {
                sb.Append(" - ").Append(partner.Name.Trim());
            }
            sb.Append("] ");
            if (!String.IsNullOrWhiteSpace(summary))
            {
                sb.Append(summary.Trim().TrimEnd('.')).Append(". ");
            }
            sb.Append("Liên hệ: ").Append(GetContact(source, partner)).Append(".");
            detail = Shorten(detail);
            if (!String.IsNullOrWhiteSpace(detail) && (summary == null || !summary.Contains(detail)))
            {
                sb.Append(" Chi tiết: ").Append(detail);
            }
            return sb.ToString();
        }

        /// <summary>Thông báo từ exception: tự phân loại nguồn, mô tả mặc định, kèm ex.Message ở phần Chi tiết.</summary>
        /// <param name="action">Thao tác đang làm, VD "gửi hồ sơ", "ký văn bản" (tùy chọn)</param>
        public static string FromException(Exception ex, IntegrationPartner partner, string action)
        {
            ErrorSource source = IntegrationErrorClassifier.Classify(ex);
            string summary = GetDefaultSummary(source, ex, partner);
            string detail = IntegrationErrorClassifier.GetDetail(ex);
            IntegrationException known = FindKnown(ex);
            if (known != null)
            {
                // Exception dựng sẵn ở lớp gọi API: message đã cụ thể (mã HTTP, địa chỉ, lý do) → dùng làm câu chính
                summary = known.Message;
                if (partner == null) partner = known.Partner;
                Exception root = IntegrationErrorClassifier.GetRoot(known);
                detail = known.InnerException != null && root != null && root != known && !summary.Contains(root.Message ?? "")
                    ? IntegrationErrorClassifier.GetDetail(known.InnerException)
                    : null;
                if (!String.IsNullOrWhiteSpace(action)) summary = string.Format("Lỗi khi {0}: {1}", action, summary);
            }
            else if (source == ErrorSource.Business)
            {
                summary = detail; // message do code tự ném để báo dữ liệu chưa hợp lệ chính là mô tả
                detail = null;
            }
            else if (!String.IsNullOrWhiteSpace(action) && !String.IsNullOrWhiteSpace(summary))
            {
                summary = string.Format("{0} khi {1}", summary, action);
            }
            return Build(source, partner, summary, detail);
        }

        /// <summary>
        /// Đối tác nhận yêu cầu nhưng trả về lỗi nghiệp vụ (mã lỗi + mô tả của họ).
        /// apiMessage rỗng → coi như không nhận được phản hồi; apiMessage là lỗi kết nối kỹ thuật → kết nối; đã dựng sẵn → giữ nguyên.
        /// </summary>
        public static string PartnerError(IntegrationPartner partner, string apiMessage, string fallbackSummary)
        {
            if (IsBuilt(apiMessage)) return apiMessage;
            if (String.IsNullOrWhiteSpace(apiMessage))
            {
                return Build(ErrorSource.Connection, partner,
                    (String.IsNullOrWhiteSpace(fallbackSummary) ? "Thao tác thất bại" : fallbackSummary.Trim().TrimEnd('.')) + " và " + GetName(partner) + " không trả về mô tả lỗi", null);
            }
            ErrorSource source = IntegrationErrorClassifier.IsConnectionMessage(apiMessage) ? ErrorSource.Connection : ErrorSource.Partner;
            if (String.IsNullOrWhiteSpace(fallbackSummary))
            {
                fallbackSummary = source == ErrorSource.Connection ? "Không kết nối được tới " + GetName(partner) : GetName(partner) + " trả về lỗi";
            }
            return Build(source, partner, fallbackSummary, apiMessage);
        }

        /// <summary>
        /// Đăng nhập/lấy token đối tác thất bại: thường do tài khoản, mật khẩu, khóa bí mật thiết lập tại viện → THIẾT LẬP;
        /// không phản hồi → KẾT NỐI.
        /// </summary>
        public static string LoginError(IntegrationPartner partner, string apiMessage)
        {
            if (IsBuilt(apiMessage)) return apiMessage;
            if (String.IsNullOrWhiteSpace(apiMessage)) return NoResponse(partner, "đăng nhập/lấy mã xác thực");
            if (IntegrationErrorClassifier.IsConnectionMessage(apiMessage))
                return Build(ErrorSource.Connection, partner, "Không kết nối được tới " + GetName(partner) + " khi đăng nhập", apiMessage);
            return Build(ErrorSource.Config, partner,
                "Đăng nhập " + GetName(partner) + " thất bại: kiểm tra tài khoản, mật khẩu, mã đơn vị, khóa bí mật đã thiết lập trong HIS; nếu tất cả đã đúng thì liên hệ " + GetPartnerRole(partner), apiMessage);
        }

        public static string ConfigError(IntegrationPartner partner, string summary)
        {
            return Build(ErrorSource.Config, partner, summary, null);
        }

        public static string BusinessError(IntegrationPartner partner, string summary)
        {
            return Build(ErrorSource.Business, partner, summary, null);
        }

        public static string SoftwareError(IntegrationPartner partner, string summary, string detail)
        {
            return Build(ErrorSource.Software, partner, summary, detail);
        }

        public static string WorkstationError(IntegrationPartner partner, string summary, string detail)
        {
            return Build(ErrorSource.Workstation, partner, summary, detail);
        }

        public static string NoResponse(IntegrationPartner partner, string action)
        {
            string summary = String.IsNullOrWhiteSpace(action)
                ? "Không nhận được phản hồi từ " + GetName(partner)
                : string.Format("Không nhận được phản hồi từ {0} khi {1}", GetName(partner), action);
            return Build(ErrorSource.Connection, partner, summary, null);
        }

        #region Nhãn gọn (cho chuỗi lỗi ghép nhiều cổng / cột trạng thái lưu DB)

        /// <summary>Liên hệ ngắn theo nguồn lỗi, dùng trong nhãn gọn.</summary>
        public static string GetShortContact(ErrorSource source, IntegrationPartner partner)
        {
            switch (source)
            {
                case ErrorSource.Connection: return "IT viện kiểm tra mạng, sau đó báo " + GetPartnerRole(partner);
                case ErrorSource.Partner: return "báo " + GetPartnerRole(partner);
                case ErrorSource.Workstation: return "IT viện kiểm tra máy trạm";
                case ErrorSource.Config: return "quản trị HIS/triển khai kiểm tra thiết lập";
                case ErrorSource.Business: return "kiểm tra lại dữ liệu hồ sơ";
                default: return "báo hỗ trợ phần mềm HIS";
            }
        }

        /// <summary>Nhãn gọn: "[NGUỒN LỖI → liên hệ ai]".</summary>
        public static string Tag(ErrorSource source, IntegrationPartner partner)
        {
            return "[" + GetLabel(source, partner != null ? partner.Kind : PartnerKind.Other) + " → " + GetShortContact(source, partner) + "]";
        }

        /// <summary>Đã có nhãn gọn / thông báo đầy đủ ở đầu chuỗi.</summary>
        public static bool IsTagged(string text)
        {
            if (String.IsNullOrWhiteSpace(text)) return false;
            string t = text.TrimStart();
            return IsBuilt(t) || (t.StartsWith("[") && t.Contains(" → ") && t.IndexOf(']') > 0);
        }

        /// <summary>Gắn nhãn gọn vào trước một chuỗi lỗi (tự phân loại theo nội dung). Chuỗi đã gắn nhãn thì giữ nguyên.</summary>
        public static string TagText(IntegrationPartner partner, string raw)
        {
            if (String.IsNullOrWhiteSpace(raw) || IsTagged(raw)) return raw;
            return Tag(IntegrationErrorClassifier.ClassifyByMessage(raw), partner) + " " + raw.Trim();
        }

        /// <summary>
        /// Thông báo đầy đủ (nhãn + liên hệ) từ một chuỗi lỗi có sẵn do code tự dựng (vd "Cổng trả lỗi (HTTP 500) — ...",
        /// "Đăng nhập cổng thất bại — ..."): tự phân loại theo nội dung, giữ nguyên chuỗi làm mô tả. Đã dựng/gắn nhãn thì giữ nguyên.
        /// </summary>
        public static string FromText(IntegrationPartner partner, string raw)
        {
            if (String.IsNullOrWhiteSpace(raw) || IsBuilt(raw)) return raw;
            return Build(IntegrationErrorClassifier.ClassifyByMessage(raw), partner, raw.Trim(), null);
        }

        /// <summary>Nhãn gọn từ exception: "[NGUỒN → liên hệ] ex.Message".</summary>
        public static string TagException(Exception ex, IntegrationPartner partner)
        {
            return Tag(IntegrationErrorClassifier.Classify(ex), partner) + " " + IntegrationErrorClassifier.GetDetail(ex);
        }

        #endregion

        /// <summary>
        /// Bản rút gọn để lưu DB / cột trạng thái hẹp: bỏ phần "Liên hệ: ...", giữ nhãn nguồn lỗi + mô tả + chi tiết,
        /// rồi cắt theo số byte UTF-8 (cột Oracle VARCHAR2 tính theo byte).
        /// </summary>
        public static string ToShort(string message, int maxBytes)
        {
            if (String.IsNullOrEmpty(message)) return message;
            string s = message;
            int iContact = s.IndexOf(" Liên hệ: ", StringComparison.Ordinal);
            if (iContact < 0 && s.StartsWith("Liên hệ: ")) iContact = 0;
            if (iContact >= 0)
            {
                int iDetail = s.IndexOf(" Chi tiết: ", iContact, StringComparison.Ordinal);
                s = s.Substring(0, iContact) + (iDetail >= 0 ? s.Substring(iDetail) : "");
            }
            return TruncateUtf8(s, maxBytes);
        }

        /// <summary>Cắt chuỗi để số byte UTF-8 không vượt maxBytes (không cắt giữa ký tự).</summary>
        public static string TruncateUtf8(string s, int maxBytes)
        {
            if (String.IsNullOrEmpty(s) || maxBytes <= 0) return s;
            if (Encoding.UTF8.GetByteCount(s) <= maxBytes) return s;
            const string ELLIPSIS = "...";
            int budget = maxBytes - Encoding.UTF8.GetByteCount(ELLIPSIS);
            if (budget <= 0) return "";
            StringBuilder sb = new StringBuilder();
            int used = 0;
            foreach (char c in s)
            {
                int n = Encoding.UTF8.GetByteCount(new[] { c });
                if (used + n > budget) break;
                sb.Append(c);
                used += n;
            }
            return sb.ToString() + ELLIPSIS;
        }

        /// <summary>Chuỗi đã là thông báo do bộ này dựng (tránh bọc 2 lần).</summary>
        public static bool IsBuilt(string message)
        {
            return !String.IsNullOrWhiteSpace(message) && message.StartsWith("[") && message.Contains("] ") && message.Contains("Liên hệ: ");
        }

        /// <summary>Mô tả ngắn mặc định theo nguồn lỗi và loại exception.</summary>
        public static string GetDefaultSummary(ErrorSource source, Exception ex, IntegrationPartner partner)
        {
            Exception root = IntegrationErrorClassifier.GetRoot(ex);
            string name = GetName(partner);
            switch (source)
            {
                case ErrorSource.Connection:
                    if (IntegrationErrorClassifier.IsTimeout(root)) return "Hết thời gian chờ phản hồi từ " + name;
                    if (IntegrationErrorClassifier.IsSecureChannel(root)) return "Không thiết lập được kết nối bảo mật (SSL/TLS) tới " + name;
                    return "Không kết nối được tới " + name;
                case ErrorSource.Partner:
                    if (root is System.Xml.XmlException || (root != null && (root.GetType().FullName ?? "").StartsWith("Newtonsoft.Json")))
                        return name + " trả về dữ liệu không đúng định dạng";
                    return name + " trả về lỗi";
                case ErrorSource.Workstation:
                    if (root is System.Security.Cryptography.CryptographicException) return "Lỗi chứng thư số/USB token trên máy trạm";
                    if (root is OutOfMemoryException) return "Máy trạm hết bộ nhớ";
                    if (root is UnauthorizedAccessException) return "Máy trạm không có quyền ghi file";
                    return "Lỗi đọc/ghi file hoặc thiết bị trên máy trạm";
                case ErrorSource.Config:
                    if (root is IndexOutOfRangeException) return "Chuỗi cấu hình kết nối " + name + " thiếu tham số";
                    if (root is UriFormatException) return "Địa chỉ " + name + " trong cấu hình sai định dạng";
                    if (root is FormatException) return "Tham số cấu hình kết nối " + name + " sai định dạng";
                    return "Thiết lập kết nối " + name + " chưa đúng";
                case ErrorSource.Business:
                    return "";
                default:
                    if (root is TypeLoadException || root is FileLoadException || root is MissingMemberException || root is DllNotFoundException || root is BadImageFormatException)
                        return "Thiếu hoặc lệch phiên bản thư viện của phần mềm trên máy trạm";
                    return "Phần mềm gặp lỗi xử lý nội bộ";
            }
        }

        private static IntegrationException FindKnown(Exception ex)
        {
            foreach (Exception e in IntegrationErrorClassifier.Flatten(ex))
            {
                IntegrationException known = e as IntegrationException;
                if (known != null) return known;
            }
            return null;
        }

        internal static string Shorten(string detail)
        {
            if (String.IsNullOrWhiteSpace(detail)) return "";
            detail = detail.Trim();
            if (detail.Length > DETAIL_MAX_LENGTH)
            {
                detail = detail.Substring(0, DETAIL_MAX_LENGTH) + "... (xem đầy đủ trong log)";
            }
            return detail;
        }

        #endregion
    }
}

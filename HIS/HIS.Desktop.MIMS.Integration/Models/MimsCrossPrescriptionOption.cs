namespace HIS.Desktop.MIMS.Integration.Models
{
    /// <summary>
    /// Cấu hình kiểm tra tương tác chéo đơn (việc 52540) — plugin dựng và truyền xuống thư viện.
    /// Thư viện KHÔNG tự đọc HisConfigs để giữ đúng phân tách: config thuộc plugin.
    /// </summary>
    public class MimsCrossPrescriptionOption
    {
        /// <summary>Cách gộp thuốc đơn khác vào Prescribing.</summary>
        public const int REQUEST_MODE__MERGE_PRESCRIBING = 1;

        /// <summary>Cách sinh khối Prescribed riêng.</summary>
        public const int REQUEST_MODE__PRESCRIBED_BLOCK = 2;

        /// <summary>Tiêu đề mặc định khối HTML khi plugin không truyền (thiếu satellite resource).</summary>
        public const string DEFAULT_BANNER_TITLE =
            "Thuốc đang dùng từ đơn khác trong hồ sơ (đã đưa vào kiểm tra tương tác):";

        /// <summary>Mẫu mặc định dòng mô tả nguồn đơn: {0} = ngày kê, {1} = ngày dùng đến.</summary>
        public const string DEFAULT_SOURCE_FORMAT = "đơn ngày {0}, dùng đến {1}";

        /// <summary>
        /// 1 = gộp thuốc đơn khác vào &lt;Prescribing&gt; (mặc định, theo MIMS API Guide:
        /// khối Prescribing là "Current and Past Medications").
        /// 2 = sinh khối &lt;Prescribed&gt; riêng (dự phòng, theo Request55.xsd).
        /// </summary>
        public int RequestMode { get; set; }

        /// <summary>
        /// true = gửi tham số form "alertfilterbydrug" chứa GUID thuốc đơn hiện tại
        /// → MIMS chỉ trả cảnh báo liên quan thuốc đang kê (QT-16).
        /// </summary>
        public bool UseAlertFilterByDrug { get; set; }

        /// <summary>
        /// Giá trị tham số form "alertfilterbyseverity" (dạng &lt;WARNINGIDS&gt;...).
        /// null/rỗng = không gửi, MIMS trả về mọi mức như hiện tại.
        /// </summary>
        public string AlertFilterBySeverity { get; set; }

        /// <summary>Tiêu đề khối HTML "Thuốc đang dùng từ đơn khác" — plugin lấy từ resx.</summary>
        public string BannerTitle { get; set; }

        /// <summary>Mẫu dòng mô tả nguồn đơn: {0} = ngày kê, {1} = ngày dùng đến.</summary>
        public string SourceFormat { get; set; }

        public MimsCrossPrescriptionOption()
        {
            RequestMode = REQUEST_MODE__MERGE_PRESCRIBING;
            UseAlertFilterByDrug = true;
        }

        /// <summary>Tiêu đề dùng thực tế — rơi về mặc định khi plugin không truyền được.</summary>
        public string GetBannerTitle()
        {
            return string.IsNullOrWhiteSpace(BannerTitle) ? DEFAULT_BANNER_TITLE : BannerTitle;
        }

        /// <summary>Mẫu dòng nguồn đơn dùng thực tế — rơi về mặc định khi plugin không truyền được.</summary>
        public string GetSourceFormat()
        {
            return string.IsNullOrWhiteSpace(SourceFormat) ? DEFAULT_SOURCE_FORMAT : SourceFormat;
        }
    }
}

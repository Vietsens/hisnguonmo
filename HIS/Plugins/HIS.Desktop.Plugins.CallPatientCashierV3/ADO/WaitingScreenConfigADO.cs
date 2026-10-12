using System.Collections.Generic;

namespace HIS.Desktop.Plugins.CallPatientCashierV3
{
    /// <summary>
    /// Cấu hình màn chờ thu ngân 3, lưu dạng JSON trong ControlState theo máy + module.
    /// Gồm điều kiện lọc tivi (R1, R7, R8), tùy chọn làm mới và tùy chỉnh hiển thị (R14, R17, R18).
    /// </summary>
    public class WaitingScreenConfigADO
    {
        /// <summary>Phiên bản bố cục hiển thị. Cấu hình lưu từ bản cũ hơn thì cỡ chữ / màu được đưa về mặc định theo mockup.</summary>
        public const int CURRENT_DISPLAY_VERSION = 2;
        public int DisplayVersion { get; set; }

        /// <summary>
        /// Phiên bản điều kiện lọc. Bản 1 tự điền khoa của phòng thu ngân và so với khoa kết thúc (luôn trống với hồ sơ chưa kết thúc)
        /// → tivi trống. Cấu hình lưu từ bản 1 được xóa ô Khoa khi nạp. Bản 2: ô Khoa so với khoa hiện tại của bệnh nhân.
        /// </summary>
        public const int CURRENT_FILTER_VERSION = 2;
        public int FilterVersion { get; set; }

        // Điều kiện lọc
        public bool AlwaysToday { get; set; }
        public long? InDateFrom { get; set; }      // yyyyMMdd000000
        public long? InDateTo { get; set; }        // yyyyMMdd235959
        public List<long> DepartmentIds { get; set; }
        public List<long> TreatmentTypeIds { get; set; }
        public List<long> PatientTypeIds { get; set; }
        /// <summary>0 = Tất cả, 1 = Còn nợ viện phí, 2 = Không nợ</summary>
        public int DebtStatus { get; set; }
        /// <summary>Khóa nhóm dịch vụ được chọn: "T{service_type_id}" hoặc "E{execute_group_id}". Rỗng = tất cả</summary>
        public List<string> ServiceGroupKeys { get; set; }
        public bool SplitCdhaByExecuteGroup { get; set; }

        // Làm mới
        public int RefreshSeconds { get; set; }
        public int MaxRows { get; set; }
        public int PageSeconds { get; set; }

        // Hiển thị
        public float TitleFontSize { get; set; }
        public float ListFontSize { get; set; }
        public float CallingFontSize { get; set; }
        public string TitleColor { get; set; }
        public string ListColor { get; set; }
        public string CallingColor { get; set; }
        public string BackColor { get; set; }
        // Màu thẻ "Danh sách chờ"
        public string ListTitleBackColor { get; set; }
        public string ColumnHeaderBackColor { get; set; }
        public string RowAltBackColor { get; set; }
        // Màu thanh "Đang gọi"
        public string CallBarBackColor { get; set; }
        public string CallBarBackColor2 { get; set; }
        public string CallTitleColor { get; set; }
        public string CallRoomColor { get; set; }
        public string DepartmentTitle { get; set; }
        public bool ShowOnExtendMonitor { get; set; }
        public bool AutoOpen { get; set; }

        public static WaitingScreenConfigADO CreateDefault(string departmentTitle, long? defaultDepartmentId)
        {
            WaitingScreenConfigADO cfg = new WaitingScreenConfigADO();
            cfg.AlwaysToday = true;
            // Mặc định không lọc khoa (08/10/2026): phòng thu ngân thường thuộc khoa hành chính, không phải khoa của bệnh nhân
            cfg.DepartmentIds = new List<long>();
            cfg.FilterVersion = CURRENT_FILTER_VERSION;
            cfg.TreatmentTypeIds = new List<long>();
            cfg.PatientTypeIds = new List<long>();
            cfg.DebtStatus = 0;
            cfg.ServiceGroupKeys = new List<string>();
            cfg.SplitCdhaByExecuteGroup = true;
            cfg.RefreshSeconds = 10;
            cfg.MaxRows = 10;
            cfg.PageSeconds = 8;
            cfg.TitleFontSize = 34;
            cfg.ListFontSize = 34;
            cfg.CallingFontSize = 34;
            cfg.TitleColor = DEFAULT_TITLE_COLOR;
            cfg.ListColor = DEFAULT_LIST_COLOR;
            cfg.CallingColor = DEFAULT_CALLING_COLOR;
            cfg.BackColor = DEFAULT_BACK_COLOR;
            cfg.DepartmentTitle = departmentTitle;
            cfg.ShowOnExtendMonitor = true;
            cfg.AutoOpen = false;
            cfg.DisplayVersion = CURRENT_DISPLAY_VERSION;
            return cfg;
        }

        /// <summary>Bổ sung giá trị mặc định cho cấu hình đọc từ bản lưu cũ / thiếu trường.</summary>
        public void Normalize()
        {
            if (this.DisplayVersion < CURRENT_DISPLAY_VERSION)
            {
                this.ResetDisplayToDefault();
                this.DisplayVersion = CURRENT_DISPLAY_VERSION;
            }
            if (this.DepartmentIds == null) this.DepartmentIds = new List<long>();
            if (this.FilterVersion < CURRENT_FILTER_VERSION)
            {
                this.DepartmentIds.Clear();
                this.FilterVersion = CURRENT_FILTER_VERSION;
            }
            if (this.TreatmentTypeIds == null) this.TreatmentTypeIds = new List<long>();
            if (this.PatientTypeIds == null) this.PatientTypeIds = new List<long>();
            if (this.ServiceGroupKeys == null) this.ServiceGroupKeys = new List<string>();
            if (this.RefreshSeconds < 5 || this.RefreshSeconds > 60) this.RefreshSeconds = 10;
            if (this.MaxRows < 5 || this.MaxRows > 30) this.MaxRows = 10;
            if (this.PageSeconds < 5 || this.PageSeconds > 60) this.PageSeconds = 8;
            if (this.TitleFontSize < 20 || this.TitleFontSize > 60) this.TitleFontSize = 34;
            if (this.ListFontSize < 20 || this.ListFontSize > 60) this.ListFontSize = 34;
            if (this.CallingFontSize < 20 || this.CallingFontSize > 60) this.CallingFontSize = 34;
            if (string.IsNullOrEmpty(this.TitleColor)) this.TitleColor = DEFAULT_TITLE_COLOR;
            if (string.IsNullOrEmpty(this.ListColor)) this.ListColor = DEFAULT_LIST_COLOR;
            if (string.IsNullOrEmpty(this.CallingColor)) this.CallingColor = DEFAULT_CALLING_COLOR;
            if (string.IsNullOrEmpty(this.BackColor)) this.BackColor = DEFAULT_BACK_COLOR;
            if (string.IsNullOrEmpty(this.ListTitleBackColor)) this.ListTitleBackColor = DEFAULT_LIST_TITLE_BACK_COLOR;
            if (string.IsNullOrEmpty(this.ColumnHeaderBackColor)) this.ColumnHeaderBackColor = DEFAULT_COLUMN_HEADER_BACK_COLOR;
            if (string.IsNullOrEmpty(this.RowAltBackColor)) this.RowAltBackColor = DEFAULT_ROW_ALT_BACK_COLOR;
            if (string.IsNullOrEmpty(this.CallBarBackColor)) this.CallBarBackColor = DEFAULT_CALL_BAR_BACK_COLOR;
            if (string.IsNullOrEmpty(this.CallBarBackColor2)) this.CallBarBackColor2 = DEFAULT_CALL_BAR_BACK_COLOR2;
            if (string.IsNullOrEmpty(this.CallTitleColor)) this.CallTitleColor = DEFAULT_CALL_TITLE_COLOR;
            if (string.IsNullOrEmpty(this.CallRoomColor)) this.CallRoomColor = DEFAULT_CALL_ROOM_COLOR;
        }

        /// <summary>Màu mặc định theo mockup của viện: chữ thanh tiêu đề trắng, chữ danh sách navy, tên đang gọi vàng, nền navy.</summary>
        public const string DEFAULT_TITLE_COLOR = "#FFFFFF";
        public const string DEFAULT_LIST_COLOR = "#0B2545";
        public const string DEFAULT_CALLING_COLOR = "#FFD400";
        public const string DEFAULT_BACK_COLOR = "#0E3A5B";
        public const string DEFAULT_LIST_TITLE_BACK_COLOR = "#0E3A5B";
        public const string DEFAULT_COLUMN_HEADER_BACK_COLOR = "#3A6B8F";
        public const string DEFAULT_ROW_ALT_BACK_COLOR = "#D8EEF0";
        public const string DEFAULT_CALL_BAR_BACK_COLOR = "#0B4F6C";
        public const string DEFAULT_CALL_BAR_BACK_COLOR2 = "#1B7F8E";
        public const string DEFAULT_CALL_TITLE_COLOR = "#F5B700";
        public const string DEFAULT_CALL_ROOM_COLOR = "#FFFFFF";

        /// <summary>Đưa phần hiển thị (cỡ chữ, màu) về mặc định mockup, giữ nguyên điều kiện lọc và làm mới.</summary>
        public void ResetDisplayToDefault()
        {
            this.TitleFontSize = 34;
            this.ListFontSize = 34;
            this.CallingFontSize = 34;
            this.TitleColor = DEFAULT_TITLE_COLOR;
            this.ListColor = DEFAULT_LIST_COLOR;
            this.CallingColor = DEFAULT_CALLING_COLOR;
            this.BackColor = DEFAULT_BACK_COLOR;
            this.ListTitleBackColor = DEFAULT_LIST_TITLE_BACK_COLOR;
            this.ColumnHeaderBackColor = DEFAULT_COLUMN_HEADER_BACK_COLOR;
            this.RowAltBackColor = DEFAULT_ROW_ALT_BACK_COLOR;
            this.CallBarBackColor = DEFAULT_CALL_BAR_BACK_COLOR;
            this.CallBarBackColor2 = DEFAULT_CALL_BAR_BACK_COLOR2;
            this.CallTitleColor = DEFAULT_CALL_TITLE_COLOR;
            this.CallRoomColor = DEFAULT_CALL_ROOM_COLOR;
        }
    }
}

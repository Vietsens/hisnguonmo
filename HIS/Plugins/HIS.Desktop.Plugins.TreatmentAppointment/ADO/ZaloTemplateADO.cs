/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 */
namespace HIS.Desktop.Plugins.TreatmentAppointment.ADO
{
    /// <summary>
    /// Template Zalo OA trả về từ API GetZaloTemplates.
    /// Đại diện cho 1 template ZNS đã được duyệt phía gateway (OneSMS / FNS).
    /// </summary>
    public class ZaloTemplateADO
    {
        /// <summary>ID template ZNS phía gateway</summary>
        public string TemplateId { get; set; }

        /// <summary>Tên hiển thị template</summary>
        public string TemplateName { get; set; }

        /// <summary>Trạng thái template (ENABLE / DISABLE / PENDING)</summary>
        public string Status { get; set; }

        /// <summary>Chất lượng template do Zalo đánh giá (HIGH / MEDIUM / LOW)</summary>
        public string Quality { get; set; }

        /// <summary>Nội dung mẫu hiển thị preview cho user</summary>
        public string PreviewContent { get; set; }
    }
}

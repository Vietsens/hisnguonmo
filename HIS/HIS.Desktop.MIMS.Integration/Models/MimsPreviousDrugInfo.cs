namespace HIS.Desktop.MIMS.Integration.Models
{
    /// <summary>
    /// Thông tin nguồn gốc của 1 thuốc thuộc đơn KHÁC trong hồ sơ (việc 52540).
    /// Chỉ phục vụ dựng khối HTML "Thuốc đang dùng từ đơn khác" đầu popup cảnh báo —
    /// KHÔNG tham gia vào XML request gửi MIMS.
    /// </summary>
    public class MimsPreviousDrugInfo
    {
        /// <summary>Mã thuốc trong danh mục HIS (MEDICINE_TYPE_CODE).</summary>
        public string HisDrugCode { get; set; }

        /// <summary>Tên thuốc (MEDICINE_TYPE_NAME).</summary>
        public string DrugName { get; set; }

        /// <summary>Mã phiếu xuất/đơn thuốc (EXP_MEST_CODE) — có thể rỗng.</summary>
        public string ExpMestCode { get; set; }

        /// <summary>Tên bác sĩ kê đơn (REQ_USERNAME) — có thể rỗng.</summary>
        public string RequestUserName { get; set; }

        /// <summary>Tên khoa kê đơn — có thể rỗng.</summary>
        public string DepartmentName { get; set; }

        /// <summary>Thời gian chỉ định của đơn đó, dạng yyyyMMddHHmmss.</summary>
        public long? IntructionTime { get; set; }

        /// <summary>Thời gian dùng đến của thuốc, dạng yyyyMMddHHmmss.</summary>
        public long? UseTimeTo { get; set; }
    }
}

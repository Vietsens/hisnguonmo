namespace HIS.Desktop.MIMS.Integration.Models
{
    /// <summary>
    /// Chi tiết một cảnh báo Drug-Lactation (phụ nữ cho con bú)
    /// được parse từ Result XML: Interaction/(GGPI|Product|GenericItem)/Route/Lactation.
    /// </summary>
    public class DrugLactationAlertDetail
    {
        /// <summary>
        /// Tên thuốc trong Interaction (GGPI|Product|GenericItem/@name).
        /// </summary>
        public string DrugName { get; set; }

        /// <summary>
        /// Mã tham chiếu thuốc (@reference).
        /// </summary>
        public string DrugReference { get; set; }

        /// <summary>
        /// Tên đường dùng (Route/@name).
        /// </summary>
        public string RouteName { get; set; }

        /// <summary>
        /// Lớp tương tác (InteractionClass/@name).
        /// </summary>
        public string InteractionClassName { get; set; }

        /// <summary>
        /// Hoạt chất (InteractionClass/Molecule/@name).
        /// </summary>
        public string MoleculeName { get; set; }

        /// <summary>
        /// Mức độ raw (Severity/@name): Contraindicated / Avoid if possible / Caution.
        /// </summary>
        public string Severity { get; set; }

        /// <summary>
        /// Mức độ dạng enum, parse từ <see cref="Severity"/>.
        /// </summary>
        public LactationSeverity SeverityLevel { get; set; }

        /// <summary>
        /// Thứ hạng hiển thị (Severity/@ranking) nếu MIMS trả về.
        /// </summary>
        public string Ranking { get; set; }

        /// <summary>
        /// Nội dung khuyến cáo (Comment).
        /// </summary>
        public string Comment { get; set; }
    }
}

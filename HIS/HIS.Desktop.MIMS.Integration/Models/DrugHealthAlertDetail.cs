namespace HIS.Desktop.MIMS.Integration.Models
{
    /// <summary>
    /// Chi tiết một cảnh báo Drug-Health Alert
    /// được parse từ MIMS DRUG-HEALTH ALERT Result XML.
    /// </summary>
    public class DrugHealthAlertDetail
    {
        /// <summary>
        /// Tên thuốc trong Interaction (Product/GGPI/@name).
        /// </summary>
        public string DrugName { get; set; }

        /// <summary>
        /// Mã tham chiếu thuốc (Product/GGPI/@reference).
        /// </summary>
        public string DrugReference { get; set; }

        /// <summary>
        /// Tên đường dùng (Route/@name), ví dụ: "Systemic".
        /// </summary>
        public string RouteName { get; set; }

        /// <summary>
        /// Mã ICD10 (HealthIssueCode/@code).
        /// </summary>
        public string HealthIssueCode { get; set; }

        /// <summary>
        /// Loại mã bệnh (HealthIssueCode/@codeType), hiện tại là "ICD10".
        /// </summary>
        public string HealthIssueCodeType { get; set; }

        /// <summary>
        /// Tên bệnh lý (HealthIssueCode/@name), ví dụ: "Asthma".
        /// </summary>
        public string HealthIssueName { get; set; }

        /// <summary>
        /// Lớp dược lý của thuốc (PrescribingInteractionClass/@name), ví dụ "Salicylates".
        /// </summary>
        public string PrescribingClassName { get; set; }

        /// <summary>
        /// Mô tả lớp dược lý (PrescribingInteractionClass/@description).
        /// </summary>
        public string PrescribingClassDescription { get; set; }

        /// <summary>
        /// Tên hoạt chất kê đơn (PrescribingMolecule/@name).
        /// </summary>
        public string PrescribingMoleculeName { get; set; }

        /// <summary>
        /// Mức độ nghiêm trọng (Severity/@name), ví dụ: "Contraindicated", "Extreme Caution".
        /// Raw text từ MIMS.
        /// </summary>
        public string Severity { get; set; }

        /// <summary>
        /// Mức độ nghiêm trọng dạng enum, được parse từ <see cref="Severity"/>.
        /// </summary>
        public DrugHealthSeverity SeverityLevel { get; set; }

        /// <summary>
        /// Khả năng xảy ra (Likelihood/@name).
        /// </summary>
        public string Likelihood { get; set; }

        /// <summary>
        /// Mức độ chứng cứ (Documentation/@name).
        /// </summary>
        public string Documentation { get; set; }

        /// <summary>
        /// Nội dung mô tả cảnh báo dành cho chuyên môn (Interaction/Professional).
        /// </summary>
        public string ProfessionalText { get; set; }
    }
}

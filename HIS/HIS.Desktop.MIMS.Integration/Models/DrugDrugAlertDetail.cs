using System.Collections.Generic;

namespace HIS.Desktop.MIMS.Integration.Models
{
    /// <summary>
    /// Chi tiết một cảnh báo tương tác thuốc (CDS Drug-Drug Alert)
    /// được parse từ MIMS DRUG-DRUG Alert XML.
    /// </summary>
    public class DrugDrugAlertDetail
    {
        /// <summary>
        /// Tên thuốc chính (GGPI đầu tiên trong Interaction)
        /// </summary>
        public string PrimaryDrugName { get; set; }

        /// <summary>
        /// Mã tham chiếu GGPI của thuốc chính
        /// </summary>
        public string PrimaryDrugReference { get; set; }

        /// <summary>
        /// Tên thuốc/hoạt chất tương tác (Molecule trong InteractionClass)
        /// </summary>
        public string InteractingDrugName { get; set; }

        /// <summary>
        /// Mã tham chiếu GGPI hoặc Molecule của thuốc/hoạt chất tương tác
        /// </summary>
        public string InteractingDrugReference { get; set; }

        /// <summary>
        /// Lớp của thuốc kê đơn (PrescribingInteractionClass/@name)
        /// </summary>
        public string PrescribingClassName { get; set; }

        /// <summary>
        /// Lớp của thuốc tương tác (InteractionClass/@name)
        /// </summary>
        public string InteractingClassName { get; set; }

        /// <summary>
        /// Mức độ nghiêm trọng (Severity) - raw text từ MIMS (ví dụ: "Severe", "Moderate").
        /// </summary>
        public string Severity { get; set; }

        /// <summary>
        /// Mức độ nghiêm trọng dạng enum, được parse từ <see cref="Severity"/>.
        /// </summary>
        public DrugInteractionSeverity SeverityLevel { get; set; }

        /// <summary>
        /// Khả năng xảy ra (Likelihood)
        /// </summary>
        public string Likelihood { get; set; }

        /// <summary>
        /// Mức độ chứng cứ (Documentation)
        /// </summary>
        public string Documentation { get; set; }

        /// <summary>
        /// Nội dung mô tả tương tác dành cho chuyên môn (Interaction/Professional)
        /// </summary>
        public string ProfessionalText { get; set; }

        /// <summary>
        /// Danh sách khuyến cáo/Precaution (mỗi Precaution/Professional)
        /// </summary>
        public List<string> Precautions { get; set; }

        public DrugDrugAlertDetail()
        {
            Precautions = new List<string>();
        }
    }
}

namespace HIS.Desktop.MIMS.Integration.Models
{
    /// <summary>
    /// Thông tin chi tiết Drug Information (GGPI) được parse từ Result/Content/GGPI.
    /// Tập trung vào các trường quan trọng cho FE.
    /// </summary>
    public class DrugInformationGgpiDetail
    {
        /// <summary>
        /// Tên hiển thị của GGPI (GGPI/@name)
        /// </summary>
        public string DrugName { get; set; }

        /// <summary>
        /// Mã tham chiếu GGPI (GGPI/@reference)
        /// </summary>
        public string Reference { get; set; }

        /// <summary>
        /// Tên generic (MONOGRAPH/GENMONO)
        /// </summary>
        public string GenericName { get; set; }

        /// <summary>
        /// Nhóm điều trị (MONOGRAPH/GCLS)
        /// </summary>
        public string TherapeuticClass { get; set; }

        /// <summary>
        /// Phân loại/GPCAT (ví dụ nhóm dùng đường uống, phân loại an toàn...)
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Chống chỉ định (MONOGRAPH/GCI)
        /// </summary>
        public string Contraindications { get; set; }

        /// <summary>
        /// Thận trọng đặc biệt (MONOGRAPH/GSP)
        /// </summary>
        public string SpecialPrecautions { get; set; }

        /// <summary>
        /// Tác dụng không mong muốn/ADR (MONOGRAPH/GAR)
        /// </summary>
        public string AdverseReactions { get; set; }

        /// <summary>
        /// Tương tác thuốc (MONOGRAPH/GDI)
        /// </summary>
        public string DrugInteractions { get; set; }

        /// <summary>
        /// Liều dùng và cách dùng (MONOGRAPH/GDOSE)
        /// </summary>
        public string DosageAndAdministration { get; set; }

        /// <summary>
        /// Dược lực/Dược động (MONOGRAPH/GACTION)
        /// </summary>
        public string Pharmacology { get; set; }
    }
}

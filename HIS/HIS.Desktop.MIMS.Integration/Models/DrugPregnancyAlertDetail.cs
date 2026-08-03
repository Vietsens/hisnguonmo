namespace HIS.Desktop.MIMS.Integration.Models
{
    /// <summary>
    /// Chi tiết một cảnh báo Drug-Pregnancy (hoặc WOCBA — phụ nữ tuổi sinh đẻ)
    /// được parse từ Result XML: Interaction/(GGPI|Product|GenericItem)/Route/Pregnancy|WOCBA.
    /// </summary>
    public class DrugPregnancyAlertDetail
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
        /// true = node WOCBA (phụ nữ tuổi sinh đẻ), false = node Pregnancy (đang mang thai).
        /// </summary>
        public bool IsWocba { get; set; }

        /// <summary>
        /// Lớp tương tác (InteractionClass/@name).
        /// </summary>
        public string InteractionClassName { get; set; }

        /// <summary>
        /// Mô tả lớp tương tác (InteractionClass/@description).
        /// </summary>
        public string InteractionClassDescription { get; set; }

        /// <summary>
        /// Hoạt chất (InteractionClass/Molecule/@name).
        /// </summary>
        public string MoleculeName { get; set; }

        /// <summary>
        /// Phân loại thai kỳ raw (Category/@name): A, B, C, D, X hoặc "+" (comment MIMS).
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Phân loại thai kỳ dạng enum, parse từ <see cref="Category"/>.
        /// </summary>
        public PregnancyCategory CategoryLevel { get; set; }

        /// <summary>
        /// Tam cá nguyệt áp dụng (Category/@Trimester), ví dụ "1st Trimester".
        /// </summary>
        public string Trimester { get; set; }

        /// <summary>
        /// Nguồn phân loại (Category/@Source): FDA hoặc MIMS.
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// Nội dung khuyến cáo (Category/Comment).
        /// </summary>
        public string Comment { get; set; }
    }
}

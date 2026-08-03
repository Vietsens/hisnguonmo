namespace HIS.Desktop.MIMS.Integration.Models
{
    /// <summary>
    /// Mức độ nghiêm trọng cho cảnh báo tương tác thuốc (Drug-Drug Alert).
    /// Theo tài liệu MIMS: Severe, Moderate, Minor, Caution.
    /// </summary>
    public enum DrugInteractionSeverity
    {
        Unknown = 0,
        Severe,
        Moderate,
        Minor,
        Caution
    }

    /// <summary>
    /// Mức độ nghiêm trọng cho cảnh báo Drug-Health Alert.
    /// Theo tài liệu MIMS: Contraindicated, Extreme Caution.
    /// </summary>
    public enum DrugHealthSeverity
    {
        Unknown = 0,
        Contraindicated,
        ExtremeCaution
    }

    /// <summary>
    /// Phân loại thai kỳ cho cảnh báo Drug-Pregnancy (Route/Pregnancy|WOCBA/Category/@name).
    /// A/B/C/D/X theo FDA; Plus ("+") là khuyến cáo nguồn MIMS.
    /// Giá trị enum tăng dần theo mức nghiêm trọng (A nhẹ nhất, X nặng nhất).
    /// </summary>
    public enum PregnancyCategory
    {
        Unknown = 0,
        /// <summary>FDA A — nghiên cứu có kiểm soát không thấy nguy cơ</summary>
        A = 1,
        /// <summary>FDA B — chưa có bằng chứng nguy cơ trên người</summary>
        B = 2,
        /// <summary>Khuyến cáo nguồn MIMS ("+") — có comment chuyên môn kèm theo</summary>
        Plus = 3,
        /// <summary>FDA C — chưa loại trừ được nguy cơ</summary>
        C = 4,
        /// <summary>FDA D — có bằng chứng nguy cơ</summary>
        D = 5,
        /// <summary>FDA X — chống chỉ định trong thai kỳ</summary>
        X = 6
    }

    /// <summary>
    /// Mức độ cho cảnh báo Drug-Lactation (Route/Lactation/Severity/@name).
    /// Giá trị enum tăng dần theo mức nghiêm trọng.
    /// </summary>
    public enum LactationSeverity
    {
        Unknown = 0,
        /// <summary>Caution — dùng thận trọng, theo dõi trẻ bú mẹ</summary>
        Caution = 1,
        /// <summary>Avoid if possible — tránh dùng nếu có thể</summary>
        AvoidIfPossible = 2,
        /// <summary>Contraindicated — chống chỉ định khi cho con bú</summary>
        Contraindicated = 3
    }
}
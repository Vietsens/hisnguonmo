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
}
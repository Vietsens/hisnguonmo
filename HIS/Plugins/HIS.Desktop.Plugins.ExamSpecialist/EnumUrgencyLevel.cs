namespace HIS.Desktop.Plugins.ExamSpecialist
{
    /// <summary>
    /// Mức độ của yêu cầu mời khám chuyên khoa.
    /// Mapping với cột URGENCY_LEVEL trong bảng HIS_SPECIALIST_EXAM.
    /// Yêu cầu mời không chọn mức độ thì cột này để NULL — không có giá trị enum tương ứng.
    /// </summary>
    public enum EnumUrgencyLevel
    {
        /// <summary>Thường — khoa được mời sắp xếp công việc đến khám trong ngày</summary>
        Normal = 1,

        /// <summary>Khẩn — khoa được mời cần đến khám ngay</summary>
        Emergency = 2
    }

    /// <summary>
    /// Lựa chọn của trường lọc "Mức độ". Value = null là "Tất cả" — không áp điều kiện lọc.
    /// </summary>
    public class UrgencyModel
    {
        public short? Value { get; set; }
        public string Display { get; set; }
    }
}

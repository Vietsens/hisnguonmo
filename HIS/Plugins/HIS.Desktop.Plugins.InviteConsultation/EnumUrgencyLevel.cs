namespace HIS.Desktop.Plugins.InviteConsultation
{
    /// <summary>
    /// Mức độ của yêu cầu mời hội chẩn.
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
}

namespace HIS.Desktop.MIMS.Integration.Models
{
    /// <summary>
    /// Thông tin bệnh nhân gửi kèm request MIMS (khối &lt;PatientProfile&gt;)
    /// phục vụ cảnh báo Drug Pregnancy / Drug Lactation.
    /// PatientProfile là con trực tiếp của &lt;Request&gt;, ngang hàng &lt;Interaction&gt;.
    /// </summary>
    public class MimsPatientProfile
    {
        /// <summary>
        /// Mã giới tính theo MIMS: "F" = nữ, "M" = nam.
        /// </summary>
        public string GenderCode { get; set; }

        /// <summary>
        /// Tuổi (năm) của bệnh nhân — map vào &lt;Age&gt;&lt;Year&gt;.
        /// </summary>
        public int? AgeYear { get; set; }

        /// <summary>
        /// Đang mang thai — khi true và PregnancyMonth có giá trị thì sinh &lt;Pregnancy&gt;&lt;Month&gt;.
        /// </summary>
        public bool IsPregnant { get; set; }

        /// <summary>
        /// Số tháng mang thai (1..9) — MIMS tự suy trimester từ giá trị này.
        /// </summary>
        public int? PregnancyMonth { get; set; }

        /// <summary>
        /// Đang cho con bú — khi true thì sinh &lt;Nursing&gt;true&lt;/Nursing&gt;.
        /// </summary>
        public bool IsNursing { get; set; }

        /// <summary>
        /// Có ít nhất 1 trạng thái được đánh dấu — điều kiện để chèn PatientProfile vào request.
        /// </summary>
        public bool HasAnyFlag()
        {
            return IsPregnant || IsNursing;
        }
    }
}

namespace HIS.Desktop.MIMS.Integration.Models
{
    /// <summary>
    /// Bản ghi HIS_MIMS_PATIENT_PROFILE (trạng thái mang thai / cho con bú của bệnh nhân).
    /// DTO độc lập với MOS.EFMODEL — property đặt tên trùng cột DB để serialize/deserialize
    /// JSON tương thích API api/HisMimsPatientProfile/* (backend gencode theo pattern HIS_MIMS_*).
    /// 1 bệnh nhân chỉ có 1 bản ghi active — cập nhật tại chỗ, không tạo bản ghi mới theo lần khám.
    /// </summary>
    public class MimsPatientProfileRecord
    {
        public long ID { get; set; }
        public long? CREATE_TIME { get; set; }
        public long? MODIFY_TIME { get; set; }
        public string CREATOR { get; set; }
        public string MODIFIER { get; set; }
        public string APP_CREATOR { get; set; }
        public string APP_MODIFIER { get; set; }
        public short? IS_ACTIVE { get; set; }
        public short? IS_DELETE { get; set; }
        public string GROUP_CODE { get; set; }

        /// <summary>
        /// Bệnh nhân — bắt buộc, duy nhất 1 bản ghi active / bệnh nhân.
        /// </summary>
        public long PATIENT_ID { get; set; }

        /// <summary>
        /// Lần điều trị đánh dấu gần nhất (tham chiếu, nullable).
        /// </summary>
        public long? TREATMENT_ID { get; set; }

        /// <summary>
        /// 1 = đang mang thai.
        /// </summary>
        public short? IS_PREGNANT { get; set; }

        /// <summary>
        /// Số tháng mang thai (1..9) — bắt buộc khi IS_PREGNANT = 1.
        /// </summary>
        public short? PREGNANT_MONTH { get; set; }

        /// <summary>
        /// 1 = đang cho con bú.
        /// </summary>
        public short? IS_LACTATING { get; set; }

        /// <summary>
        /// Số tháng cho con bú (tham khảo hồ sơ — MIMS chỉ cần Nursing=true).
        /// </summary>
        public short? LACTATING_MONTH { get; set; }
    }

    /// <summary>
    /// Filter cho api/HisMimsPatientProfile/Get — property đặt tên theo pattern FilterQuery MOS.
    /// </summary>
    public class MimsPatientProfileFilter
    {
        public long? ID { get; set; }
        public long? PATIENT_ID { get; set; }
        public long? TREATMENT_ID { get; set; }
        public short? IS_ACTIVE { get; set; }
        public string ORDER_FIELD { get; set; }
        public string ORDER_DIRECTION { get; set; }
    }
}

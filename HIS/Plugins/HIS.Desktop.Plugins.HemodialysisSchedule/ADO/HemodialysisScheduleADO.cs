/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 */
namespace HIS.Desktop.Plugins.HemodialysisSchedule.ADO
{
    /// <summary>
    /// DTO một bản ghi slot lịch chạy thận (HIS_HEMODIALYSIS_SCHEDULE) trả về từ API
    /// HisHemodialysisSchedule/Get. Các trường TDL_* / *_NAME là dữ liệu join hiển thị
    /// (bệnh nhân, điều trị, đối tượng, gói vật tư) do backend đổ sang, KHÔNG lưu bảng.
    /// </summary>
    public class HemodialysisScheduleADO
    {
        // Khóa + trường lưu bảng
        public long ID { get; set; }
        public long TREATMENT_ID { get; set; }
        public long PATIENT_ID { get; set; }
        public long ROOM_ID { get; set; }
        /// <summary>Ngày xếp lịch, dạng số yyyyMMdd</summary>
        public long SCHEDULE_DATE { get; set; }
        /// <summary>Ca chạy thận 1..5</summary>
        public short KIDNEY_SHIFT { get; set; }
        /// <summary>Gói vật tư (HIS_EXP_MEST_TEMPLATE.ID) — nullable, inline edit trên grid</summary>
        public long? EXP_MEST_TEMPLATE_ID { get; set; }
        public string NOTE { get; set; }

        // Trường join hiển thị (read-only)
        public string TREATMENT_CODE { get; set; }
        public string TDL_PATIENT_NAME { get; set; }
        public string TDL_PATIENT_CODE { get; set; }
        public long? TDL_PATIENT_DOB { get; set; }
        public short? TDL_PATIENT_IS_HAS_NOT_DAY_DOB { get; set; }
        public long? TDL_PATIENT_GENDER_ID { get; set; }
        public string TDL_PATIENT_GENDER_NAME { get; set; }
        /// <summary>Ngày vào điều trị (dạng số yyyyMMddHHmmss)</summary>
        public long? IN_TIME { get; set; }
        /// <summary>Đối tượng (diện điều trị / patient type) — hiển thị từ HIS_TREATMENT, read-only</summary>
        public string TDL_PATIENT_TYPE_NAME { get; set; }
        /// <summary>Tên gói vật tư (HIS_EXP_MEST_TEMPLATE.EXP_MEST_TEMPLATE_NAME) để hiển thị</summary>
        public string EXP_MEST_TEMPLATE_NAME { get; set; }

        // Audit
        public long? CREATE_TIME { get; set; }
        public string CREATOR { get; set; }
        public long? MODIFY_TIME { get; set; }
        public string MODIFIER { get; set; }
    }
}

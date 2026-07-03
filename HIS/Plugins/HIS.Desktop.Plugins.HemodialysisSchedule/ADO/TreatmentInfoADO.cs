/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 */
namespace HIS.Desktop.Plugins.HemodialysisSchedule.ADO
{
    /// <summary>
    /// DTO bệnh nhân đang điều trị (vùng dưới) — ánh xạ theo các cột của V_HIS_TREATMENT_4.
    /// Deserialize trực tiếp từ JSON trả về của API, thêm IsSelected cho cột checkbox multi-select.
    /// </summary>
    public class TreatmentInfoADO
    {
        /// <summary>HIS_TREATMENT.ID</summary>
        public long ID { get; set; }
        public long PATIENT_ID { get; set; }
        public string TREATMENT_CODE { get; set; }
        public string TDL_PATIENT_NAME { get; set; }
        public string TDL_PATIENT_CODE { get; set; }
        public long? TDL_PATIENT_DOB { get; set; }
        public short? TDL_PATIENT_IS_HAS_NOT_DAY_DOB { get; set; }
        public long? TDL_PATIENT_GENDER_ID { get; set; }
        public string TDL_PATIENT_GENDER_NAME { get; set; }
        public long? IN_TIME { get; set; }
        /// <summary>Diện điều trị</summary>
        public string TDL_TREATMENT_TYPE_NAME { get; set; }
        /// <summary>Số thẻ BHYT</summary>
        public string TDL_PATIENT_HEIN_CARD_NUMBER { get; set; }
        /// <summary>Chẩn đoán chính</summary>
        public string ICD_NAME { get; set; }
        public long? TREATMENT_TYPE_ID { get; set; }

        /// <summary>Đánh dấu tick để "Đưa vào lịch"</summary>
        public bool IsSelected { get; set; }
    }
}

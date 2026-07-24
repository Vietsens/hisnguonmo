/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 */
namespace MPS.Processor.Mps000519.ADO
{
    /// <summary>
    /// Dòng danh sách tiêm chủng (mục C) — GỘP danh mục vắc xin (HIS_VACCINE_TYPE)
    /// với dữ liệu đã lưu của bệnh nhân (HIS_HEALTH_VACCINATION), mỗi loại vắc xin = 1 dòng.
    /// Dùng cho band lặp trên template: {Vaccine1.FIELD;}, {Vaccine2.FIELD;}, {Vaccine3.FIELD;}.
    /// Nhóm (VACCINE_GROUP / TYPE_VACCINE): 1 = TC cơ bản cho trẻ em, 2 = TC ngoài TCMR, 3 = UV cho phụ nữ có thai.
    /// </summary>
    public class VaccinationAdo
    {
        /// <summary>Nhóm vắc xin: 1/2/3.</summary>
        public int VACCINE_GROUP { get; set; }
        /// <summary>Mã loại vắc xin (HIS_VACCINE_TYPE.VACCINE_TYPE_CODE).</summary>
        public string VACCINE_CODE { get; set; }
        /// <summary>Tên loại vắc xin (hiển thị cột "Loại vắc xin"/"Nội dung").</summary>
        public string VACCINE_NAME { get; set; }
        /// <summary>Số thứ tự dòng trong nhóm (1..n).</summary>
        public int STT { get; set; }

        /// <summary>Cờ "Chưa chủng ngừa"/"Chưa tiêm": "x" nếu IS_NOT_VACCINATED = 1.</summary>
        public string NOT_VACCINATED_X { get; set; }
        /// <summary>Cờ "Đã chủng ngừa"/"Đã tiêm": "x" nếu đã tiêm (có ngày hoặc không phải "chưa").</summary>
        public string VACCINATED_X { get; set; }
        /// <summary>Ngày đã chủng ngừa (dd/MM/yyyy) — nhóm 1/2.</summary>
        public string VACCINATED_TIME_STR { get; set; }
        /// <summary>Tháng thai — nhóm 3 (UV cho phụ nữ có thai).</summary>
        public string PREGNANCY_MONTH { get; set; }
        /// <summary>Phản ứng sau tiêm.</summary>
        public string REACTION { get; set; }
        /// <summary>Ngày hẹn tiêm (dd/MM/yyyy).</summary>
        public string APPOINTMENT_TIME_STR { get; set; }
    }
}

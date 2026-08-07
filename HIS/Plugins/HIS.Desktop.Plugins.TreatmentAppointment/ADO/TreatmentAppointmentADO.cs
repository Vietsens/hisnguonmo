/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 */
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.Plugins.TreatmentAppointment.ADO
{
    /// <summary>
    /// Mở rộng HIS_TREATMENT thêm trường IsSelected để bind checkbox multi-select trên grid.
    /// </summary>
    public class TreatmentAppointmentADO : HIS_TREATMENT
    {
        /// <summary>Đánh dấu chọn dòng để gửi tin nhắn Zalo nhắc tái khám</summary>
        public bool IsSelected { get; set; }

        /// <summary>Tên phòng hẹn khám — giải mã từ CSV APPOINTMENT_EXAM_ROOM_IDS, pre-computed khi map ADO</summary>
        public string APPOINTMENT_EXAM_ROOM_NAMES { get; set; }

        /// <summary>Thời gian sửa cuối định dạng hiển thị — pre-computed từ MODIFY_TIME</summary>
        public string MODIFY_TIME_STR { get; set; }
    }
}

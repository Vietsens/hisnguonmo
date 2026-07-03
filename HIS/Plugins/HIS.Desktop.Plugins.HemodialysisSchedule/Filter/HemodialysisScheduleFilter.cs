/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 */
namespace HIS.Desktop.Plugins.HemodialysisSchedule.Filter
{
    /// <summary>
    /// Filter tải lịch chạy thận theo Phòng + Ngày + Ca (vùng trên).
    /// </summary>
    public class HemodialysisScheduleFilter
    {
        public long? ROOM_ID { get; set; }
        /// <summary>Ngày xếp lịch dạng số yyyyMMdd</summary>
        public long? SCHEDULE_DATE { get; set; }
        public short? KIDNEY_SHIFT { get; set; }
        /// <summary>Tìm theo tên/mã BN, mã điều trị</summary>
        public string KEY_WORD { get; set; }

        public string ORDER_FIELD { get; set; }
        public string ORDER_DIRECTION { get; set; }
    }
}

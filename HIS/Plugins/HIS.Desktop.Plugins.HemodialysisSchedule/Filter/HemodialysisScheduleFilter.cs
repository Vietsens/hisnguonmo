/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 */
namespace HIS.Desktop.Plugins.HemodialysisSchedule.Filter
{
    /// <summary>
    /// Filter tải lịch chạy thận theo Phòng + Ngày + Ca (vùng trên).
    /// Kế thừa MOS.Filter.FilterBase để dùng đúng cơ chế chuẩn MOS
    /// (KEY_WORD, CN_WORD, ORDER_FIELD, ORDER_DIRECTION, phân trang... nằm ở base).
    /// </summary>
    public class HemodialysisScheduleFilter : MOS.Filter.FilterBase
    {
        public long? ROOM_ID { get; set; }
        /// <summary>Ngày xếp lịch dạng số yyyyMMdd</summary>
        public long? SCHEDULE_DATE { get; set; }
        public short? KIDNEY_SHIFT { get; set; }
    }
}

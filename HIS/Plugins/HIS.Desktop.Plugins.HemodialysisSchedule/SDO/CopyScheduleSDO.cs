/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 */
namespace HIS.Desktop.Plugins.HemodialysisSchedule.SDO
{
    /// <summary>
    /// SDO gửi lên API CopySchedule: đọc toàn bộ bản ghi (phòng nguồn, ngày nguồn) →
    /// INSERT vào ngày đích, skip BN trùng theo unique key (TREATMENT_ID, SCHEDULE_DATE, KIDNEY_SHIFT).
    /// KHÔNG xóa slot đã có, KHÔNG sinh y lệnh.
    /// </summary>
    public class CopyScheduleSDO
    {
        public long ROOM_ID { get; set; }
        /// <summary>Ngày nguồn (yyyyMMdd)</summary>
        public long SOURCE_DATE { get; set; }
        /// <summary>Ngày đích (yyyyMMdd)</summary>
        public long TARGET_DATE { get; set; }
    }
}

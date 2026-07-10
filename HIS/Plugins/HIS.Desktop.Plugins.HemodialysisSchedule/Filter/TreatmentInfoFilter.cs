/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 */
namespace HIS.Desktop.Plugins.HemodialysisSchedule.Filter
{
    /// <summary>
    /// Filter tải danh sách bệnh nhân đang điều trị (vùng dưới), nguồn V_HIS_TREATMENT_4.
    /// Kế thừa MOS.Filter.FilterBase để dùng đúng cơ chế chuẩn MOS
    /// (KEY_WORD, CN_WORD, ORDER_FIELD, ORDER_DIRECTION, phân trang... nằm ở base).
    /// </summary>
    public class TreatmentInfoFilter : MOS.Filter.FilterBase
    {
        /// <summary>Khoa (mặc định khoa hiện tại); null khi tích "Toàn khoa"</summary>
        public long? DEPARTMENT_ID { get; set; }
        /// <summary>Ngày vào từ (yyyyMMddHHmmss)</summary>
        public long? IN_TIME_FROM { get; set; }
        /// <summary>Ngày vào đến (yyyyMMddHHmmss)</summary>
        public long? IN_TIME_TO { get; set; }
        /// <summary>Chỉ lấy BN đang điều trị (chưa kết thúc)</summary>
        public bool IS_IN_TREATMENT { get; set; }
    }
}

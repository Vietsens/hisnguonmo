/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 */
using System.Collections.Generic;

namespace HIS.Desktop.Plugins.HemodialysisSchedule.Filter
{
    /// <summary>
    /// Filter tải danh sách bệnh nhân đang điều trị (vùng dưới), nguồn V_HIS_TREATMENT_4.
    /// </summary>
    public class TreatmentInfoFilter
    {
        /// <summary>Khoa (mặc định khoa hiện tại); null khi tích "Toàn khoa"</summary>
        public long? DEPARTMENT_ID { get; set; }
        /// <summary>Ngày vào từ (yyyyMMddHHmmss)</summary>
        public long? IN_TIME_FROM { get; set; }
        /// <summary>Ngày vào đến (yyyyMMddHHmmss)</summary>
        public long? IN_TIME_TO { get; set; }
        public string KEY_WORD { get; set; }
        /// <summary>Chỉ lấy BN đang điều trị (chưa kết thúc)</summary>
        public bool IS_IN_TREATMENT { get; set; }

        public string ORDER_FIELD { get; set; }
        public string ORDER_DIRECTION { get; set; }
    }
}

/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2017 INVENTEC
 */
using System.Collections.Generic;

namespace HIS.Desktop.Plugins.HemodialysisSchedule.ADO
{
    /// <summary>
    /// Kết quả sao chép lịch (R6): số bản ghi thêm mới + số/ danh sách BN bị skip do trùng
    /// unique key (TREATMENT_ID, SCHEDULE_DATE, KIDNEY_SHIFT).
    /// </summary>
    public class CopyScheduleResultADO
    {
        public int AddedCount { get; set; }
        public int SkippedCount { get; set; }
        public List<CopyScheduleSkippedItem> SkippedItems { get; set; }

        public CopyScheduleResultADO()
        {
            this.SkippedItems = new List<CopyScheduleSkippedItem>();
        }
    }

    public class CopyScheduleSkippedItem
    {
        public long TREATMENT_ID { get; set; }
        public string PATIENT_NAME { get; set; }
        public short KIDNEY_SHIFT { get; set; }
    }
}

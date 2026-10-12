using System.Collections.Generic;
using System.ComponentModel;

namespace HIS.Desktop.Plugins.CallPatientCashierV3
{
    /// <summary>Một dòng trên bảng "DANH SÁCH CHỜ" của tivi.</summary>
    public class WaitingPatientADO
    {
        public long TREATMENT_ID { get; set; }
        public string TREATMENT_CODE { get; set; }
        public string PATIENT_NAME { get; set; }
        public string YEAR_OF_BIRTH { get; set; }
        public string SERVICE_GROUPS { get; set; }
        public long IN_TIME { get; set; }
        /// <summary>"BN cần nộp thêm" tính theo công thức màn Viện phí (R4)</summary>
        public decimal RECEIVE_MORE { get; set; }
        /// <summary>Khóa nhóm để lọc; ẩn khỏi lưới (tránh DevExpress coi là bảng con và hiện nút mở rộng).</summary>
        [Browsable(false)]
        public List<string> GroupKeys { get; set; }
    }
}

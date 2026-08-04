/* IVT — HIS.Desktop.Plugins.InfectiousDiseaseSyncList
 * 1 dòng trên grid danh sách đồng bộ: dữ liệu điều trị (từ V_HIS_TREATMENT) + trạng thái đẩy đối soát.
 */
namespace HIS.Desktop.Plugins.InfectiousDiseaseSyncList.ADO
{
    public class EcdsSyncGridRowADO
    {
        public int STT { get; set; }
        public long TREATMENT_ID { get; set; }
        public long PATIENT_ID { get; set; }
        public string TREATMENT_CODE { get; set; }
        public string PATIENT_CODE { get; set; }
        public string PATIENT_NAME { get; set; }
        public string ICD_CODE { get; set; }
        public string IN_TIME_STR { get; set; }

        /// <summary>0=chưa đẩy, 1=đã đẩy, 2=lỗi.</summary>
        public int PUSH_STATE { get; set; }
        public string PUSH_STATE_STR { get; set; }
        public string ECDS_CASE_CODE { get; set; }

        /// <summary>Nhãn cột thao tác "Xem" (mở form chi tiết).</summary>
        public string VIEW_ACTION { get { return "Xem"; } }
        /// <summary>Nhãn cột thao tác "Đẩy" (đẩy riêng 1 ca).</summary>
        public string PUSH_ACTION { get { return "Đẩy"; } }

        /// <summary>Bản gốc điều trị — dùng để build DTO / mở form chi tiết.</summary>
        public MOS.EFMODEL.DataModels.V_HIS_TREATMENT Source { get; set; }
    }
}

/* IVT — HIS.Desktop.Plugins.InfectiousDiseaseSyncList
 * 1 dòng kết quả đẩy gửi backend MOS để cập nhật HIS_ECDS_DISEASE_CASE (§21).
 */
namespace HIS.Desktop.Plugins.InfectiousDiseaseSyncList.ADO
{
    public class HisEcdsPushResultADO
    {
        public long TREATMENT_ID { get; set; }
        public string ECDS_CASE_ID { get; set; }
        public string ECDS_CASE_CODE { get; set; }
        /// <summary>0=chưa đẩy, 1=đã đẩy, 2=lỗi.</summary>
        public int PUSH_STATE { get; set; }
        public long LAST_PUSH_TIME { get; set; }
        public string PUSH_MESSAGE { get; set; }
    }
}

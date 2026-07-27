/* IVT — HIS.Desktop.Plugins.InfectiousDiseaseReport
 * DTO gửi backend MOS để lưu bản ghi HIS_ECDS_DISEASE_CASE (đối soát + dữ liệu ca đã đẩy).
 * Backend map sang HisEcdsDiseaseCaseSDO (§20).
 */
namespace HIS.Desktop.Plugins.InfectiousDiseaseReport.ADO
{
    /// <summary>Bản ghi HIS_ECDS_DISEASE_CASE + dữ liệu ca bệnh đã đẩy.</summary>
    public class HisEcdsDiseaseCaseSaveADO
    {
        /// <summary>ID bản ghi HIS; 0 = tạo mới, &gt;0 = sửa.</summary>
        public long ID { get; set; }
        public long TREATMENT_ID { get; set; }
        public long? PATIENT_ID { get; set; }
        public string ECDS_CASE_ID { get; set; }
        public string ECDS_CASE_CODE { get; set; }
        /// <summary>0=chưa đẩy, 1=đã đẩy, 2=lỗi (EcdsPushState).</summary>
        public int PUSH_STATE { get; set; }
        public long LAST_PUSH_TIME { get; set; }
        public string PUSH_MESSAGE { get; set; }
        /// <summary>Dữ liệu ca bệnh đã đẩy lên cổng (để backend lưu chi tiết).</summary>
        public EcdsDiseaseCaseDto CASE_DATA { get; set; }
    }

    /// <summary>Điều kiện lấy bản ghi theo điều trị.</summary>
    public class HisEcdsDiseaseCaseFilterADO
    {
        public long TREATMENT_ID { get; set; }
    }
}

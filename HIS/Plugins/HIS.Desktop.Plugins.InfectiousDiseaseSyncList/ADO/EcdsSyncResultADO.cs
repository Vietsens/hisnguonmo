/* IVT — HIS.Desktop.Plugins.InfectiousDiseaseSyncList
 * Kết quả đồng bộ 1 ca bệnh (hiển thị trong dialog tổng hợp).
 */
namespace HIS.Desktop.Plugins.InfectiousDiseaseSyncList.ADO
{
    public class EcdsSyncResultADO
    {
        public int Stt { get; set; }
        public long TreatmentId { get; set; }
        public string TreatmentCode { get; set; }
        public string PatientName { get; set; }
        public string IcdCode { get; set; }
        public string StatusText { get; set; }
        public bool Success { get; set; }
        public string MaCaBenh { get; set; }
        public string Message { get; set; }
    }
}

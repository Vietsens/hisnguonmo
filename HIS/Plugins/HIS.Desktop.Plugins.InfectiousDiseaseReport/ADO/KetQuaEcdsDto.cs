/* IVT — HIS.Desktop.Plugins.InfectiousDiseaseReport
 * Bao gói response chuẩn của cổng ECDS: { thanhCong, maLoi, thongDiep, duLieu }.
 */
namespace HIS.Desktop.Plugins.InfectiousDiseaseReport.ADO
{
    /// <summary>Kết quả trả về từ cổng ECDS.</summary>
    public class KetQuaEcdsDto<T>
    {
        public bool thanhCong { get; set; }
        public string maLoi { get; set; }
        public string thongDiep { get; set; }
        public T duLieu { get; set; }
    }
}

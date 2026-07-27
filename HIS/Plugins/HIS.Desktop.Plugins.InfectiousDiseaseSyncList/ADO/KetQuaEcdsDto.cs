/* IVT — HIS.Desktop.Plugins.InfectiousDiseaseSyncList
 * Bao gói response chuẩn của cổng ECDS: { thanhCong, maLoi, thongDiep, duLieu }.
 */
namespace HIS.Desktop.Plugins.InfectiousDiseaseSyncList.ADO
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

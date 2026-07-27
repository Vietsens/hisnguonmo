/* IVT — HIS.Desktop.Plugins.InfectiousDiseaseReport
 * Điều kiện tra danh mục ECDS (POST /api/fast/v1/danh-muc/*).
 */
namespace HIS.Desktop.Plugins.InfectiousDiseaseReport.ADO
{
    public class SearchDanhMucFastDto
    {
        public string tuKhoa { get; set; }
        public string maTinh { get; set; }
        public string maXa { get; set; }
        public string maIcd10Benh { get; set; }
        public int? trangSo { get; set; }
        public int? kichThuocTrang { get; set; }
    }
}

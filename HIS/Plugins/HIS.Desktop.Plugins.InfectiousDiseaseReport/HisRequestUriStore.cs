/* IVT — HIS.Desktop.Plugins.InfectiousDiseaseReport
 * Endpoint backend MOS lưu/đối soát ca bệnh ECDS (§20, §21 tài liệu thiết kế).
 */
namespace HIS.Desktop.Plugins.InfectiousDiseaseReport
{
    internal class HisRequestUriStore
    {
        /// <summary>Tạo bản ghi HIS_ECDS_DISEASE_CASE (CRUD entity trực tiếp).</summary>
        internal const string HIS_ECDS_CREATE = "api/HisEcdsDiseaseCase/Create";
        /// <summary>Sửa bản ghi HIS_ECDS_DISEASE_CASE (đẩy lại).</summary>
        internal const string HIS_ECDS_UPDATE = "api/HisEcdsDiseaseCase/Update";
        /// <summary>Lấy bản ghi theo điều trị (đối soát khi mở form).</summary>
        internal const string HIS_ECDS_GET = "api/HisEcdsDiseaseCase/Get";
        /// <summary>Lấy đầy đủ ca bệnh theo TREATMENT_CODE: cha + 2 danh sách con (§20b).</summary>
        internal const string HIS_ECDS_GET_FULL = "api/HisEcdsDiseaseCase/GetFull";
        /// <summary>Lấy thông tin hành chính bệnh nhân (V_HIS_PATIENT) để điền tab Hành chính.</summary>
        internal const string HIS_PATIENT_GETVIEW = "api/HisPatient/GetView";
    }
}

/* IVT — HIS.Desktop.Plugins.InfectiousDiseaseReport
 * Endpoint backend MOS lưu/đối soát ca bệnh ECDS (§20, §21 tài liệu thiết kế).
 */
namespace HIS.Desktop.Plugins.InfectiousDiseaseReport
{
    internal class HisRequestUriStore
    {
        /// <summary>Tạo bản ghi HIS_ECDS_DISEASE_CASE (aggregate save).</summary>
        internal const string HIS_ECDS_SAVE_CREATE = "api/HisEcdsDiseaseCase/SaveCreate";
        /// <summary>Sửa bản ghi (đẩy lại).</summary>
        internal const string HIS_ECDS_SAVE_UPDATE = "api/HisEcdsDiseaseCase/SaveUpdate";
        /// <summary>Lấy bản ghi theo điều trị (đối soát khi mở form).</summary>
        internal const string HIS_ECDS_GET = "api/HisEcdsDiseaseCase/Get";
        /// <summary>Lấy thông tin hành chính bệnh nhân (V_HIS_PATIENT) để điền tab Hành chính.</summary>
        internal const string HIS_PATIENT_GETVIEW = "api/HisPatient/GetView";
    }
}

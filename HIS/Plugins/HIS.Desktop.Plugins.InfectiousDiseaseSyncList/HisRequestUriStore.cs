/* IVT — HIS.Desktop.Plugins.InfectiousDiseaseSyncList
 * Endpoint backend MOS (§21 tài liệu thiết kế).
 */
namespace HIS.Desktop.Plugins.InfectiousDiseaseSyncList
{
    internal class HisRequestUriStore
    {
        /// <summary>Cập nhật danh sách kết quả đẩy (batch push result).</summary>
        internal const string HIS_ECDS_UPDATE_PUSH_RESULT = "api/HisEcdsDiseaseCase/UpdatePushResultList";
        /// <summary>Đối soát trạng thái đẩy theo danh sách điều trị (để tô cột trạng thái).</summary>
        internal const string HIS_ECDS_GET = "api/HisEcdsDiseaseCase/Get";
        /// <summary>Lấy view ca bệnh (V_HIS_ECDS_DISEASE_CASE) để đối soát trạng thái đẩy.</summary>
        internal const string HIS_ECDS_GET_VIEW = "api/HisEcdsDiseaseCase/GetView";
    }
}

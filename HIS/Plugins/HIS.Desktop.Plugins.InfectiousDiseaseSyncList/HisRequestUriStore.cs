/* IVT — HIS.Desktop.Plugins.InfectiousDiseaseSyncList
 * Endpoint backend MOS (§21 tài liệu thiết kế).
 */
namespace HIS.Desktop.Plugins.InfectiousDiseaseSyncList
{
    internal class HisRequestUriStore
    {
        /// <summary>Cập nhật danh sách kết quả đẩy (batch push result).</summary>
        internal const string HIS_ECDS_UPDATE_PUSH_RESULT = "api/HisEcdsDiseaseCase/UpdatePushResultList";
    }
}

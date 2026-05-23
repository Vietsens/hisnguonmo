/* IVT
 * @Project : hisnguonmo
 * Việc 45072 — Plugin ID strings (ModuleLink) dùng trong inter-plugin communication.
 */
namespace HIS.Desktop.Plugins.SurgServiceReqExecute2
{
    internal class ModuleLinkString
    {
        /// <summary>Plugin Danh sách y lệnh — mở từ btnDanhSachYLenh_v45072.</summary>
        internal const string ServiceReqList = "HIS.Desktop.Plugins.ServiceReqList";

        /// <summary>Plugin SurgServiceReqExecute (cũ) — chứa FormPtttTemp dùng cho Lưu mẫu PTTT.</summary>
        internal const string SurgServiceReqExecute = "HIS.Desktop.Plugins.SurgServiceReqExecute";

        /// <summary>Plugin SurgServiceReqExecute2 — chính plugin này, dùng cho ControlState key.</summary>
        internal const string SurgServiceReqExecute2 = "HIS.Desktop.Plugins.SurgServiceReqExecute2";
    }
}

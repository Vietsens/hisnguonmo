/* IVT
 * @Project : hisnguonmo
 * Việc 45072 — Tập trung URI endpoints dùng trong plugin SurgServiceReqExecute2.
 * Class này SHADOW HIS.Desktop.ApiConsumer.HisRequestUriStore khi có `using HIS.Desktop.ApiConsumer;`
 * → BẮT BUỘC re-declare các URI cũ đang được code hiện hữu sử dụng để tránh break.
 *
 * URI đã verify khớp với HIS.Desktop.ApiConsumer.HisRequestUriStore (lib chung) + ExecuteRoom plugin:
 *  - HIS_SERVICE_REQ_UNSTART  = "api/HisServiceReq/UnStart"   (chữ S hoa — chuẩn lib chung)
 *  - HIS_SERVICE_REQ_UNFINISH = "/api/HisServiceReq/Unfinish" (chữ f thường, có leading slash — chuẩn lib chung)
 */
namespace HIS.Desktop.Plugins.SurgServiceReqExecute2
{
    internal class HisRequestUriStore
    {
        #region Việc 45072 — URI mới

        /// <summary>Hủy bắt đầu y lệnh — POST id (long).</summary>
        internal const string MOSHIS_HIS_SERVICE_REQ_UNSTART = "api/HisServiceReq/UnStart";

        /// <summary>Hủy kết thúc y lệnh — POST id (long).</summary>
        internal const string MOSHIS_HIS_SERVICE_REQ_UNFINISH = "/api/HisServiceReq/Unfinish";

        /// <summary>Lấy HIS_SERE_SERV_EXT theo filter (hỗ trợ SERE_SERV_IDs để batch).</summary>
        internal const string MOSHIS_HIS_SERE_SERV_EXT_GET = "api/HisSereServExt/Get";

        /// <summary>Lấy V_EMR_DOCUMENT theo filter (xem EMR.Filter.EmrDocumentViewFilter).</summary>
        internal const string EMR_DOCUMENT_GET_VIEW = "api/EmrDocument/GetView";

        /// <summary>Xóa văn bản EMR đã ký theo ID.</summary>
        internal const string EMR_DOCUMENT_DELETE = "api/EmrDocument/Delete";

        /// <summary>Lấy V_HIS_SERE_SERV_PTTT theo filter.</summary>
        internal const string MOSHIS_HIS_SERE_SERV_PTTT_GET = "api/HisSereServPttt/Get";

        #endregion

        #region Re-declare các URI từ HIS.Desktop.ApiConsumer.HisRequestUriStore (lib chung) để tránh shadow

        /// <summary>Re-declare từ lib chung — code cũ trong _Left.cs sử dụng.</summary>
        internal const string HIS_SERVICE_REQ_START = "api/HisServiceReq/Start";

        /// <summary>Re-declare từ lib chung — code cũ trong _Left.cs sử dụng.</summary>
        internal const string HIS_TREATMENT_GETFEEVIEW = "api/HisTreatment/GetFeeView";

        /// <summary>Re-declare từ lib chung — code cũ trong _Left.cs sử dụng.</summary>
        internal const string HIS_PATIENT_GETVIEW = "api/HisPatient/GetView";

        #endregion
    }
}

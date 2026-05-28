namespace HIS.Desktop.Plugins.SurgServiceReqExecute2
{
    internal class HisRequestUriStore
    {
        #region URI mới

        internal const string MOSHIS_HIS_SERVICE_REQ_UNSTART = "api/HisServiceReq/UnStart";

        internal const string MOSHIS_HIS_SERVICE_REQ_UNFINISH = "/api/HisServiceReq/Unfinish";

        internal const string MOSHIS_HIS_SERE_SERV_EXT_GET = "api/HisSereServExt/Get";

        internal const string EMR_DOCUMENT_GET_VIEW = "api/EmrDocument/GetView";

        internal const string EMR_DOCUMENT_DELETE = "api/EmrDocument/Delete";

        internal const string MOSHIS_HIS_SERE_SERV_PTTT_GET = "api/HisSereServPttt/Get";

        #endregion

        #region Re-declare các URI từ HIS.Desktop.ApiConsumer.HisRequestUriStore (lib chung) để tránh shadow

        internal const string HIS_SERVICE_REQ_START = "api/HisServiceReq/Start";

        internal const string HIS_TREATMENT_GETFEEVIEW = "api/HisTreatment/GetFeeView";

        internal const string HIS_PATIENT_GETVIEW = "api/HisPatient/GetView";

        #endregion
    }
}

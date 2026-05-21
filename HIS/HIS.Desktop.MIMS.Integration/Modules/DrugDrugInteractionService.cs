using System.Collections.Generic;
using System.Xml.Linq;
using HIS.Desktop.MIMS.Integration.Models;
using HIS.Desktop.MIMS.Integration.Core;
using HIS.Desktop.MIMS.Integration.View;
using MOS.EFMODEL.DataModels;

namespace HIS.Desktop.MIMS.Integration.Modules
{
    public class DrugDrugInteractionService : BaseService
    {
        public DrugDrugInteractionService()
        {
            NameText = "Kiểm tra tương tác thuốc";
        }

        public MimsResult Check(List<DrugItem> current, List<DrugItem> previous)
        {
            Inventec.Common.Logging.LogSystem.Debug(
                "DrugDrugInteractionService.Check - start"
                + Inventec.Common.Logging.LogUtil.TraceData(
                    Inventec.Common.Logging.LogUtil.GetMemberName(() => current), current)
                + Inventec.Common.Logging.LogUtil.TraceData(
                    Inventec.Common.Logging.LogUtil.GetMemberName(() => previous), previous));

            current = this.MappingMIMS(current);
            previous = this.MappingMIMS(previous);
            string xmlRequest = MimsRequestBuilder.BuildDrugDrugInteractionRequest(current, previous, true);
            Inventec.Common.Logging.LogSystem.Debug(string.Format(
                "DrugDrugInteractionService.Check - requestLength={0}", xmlRequest == null ? 0 : xmlRequest.Length));

            bool isTimeout;
            string xmlResponse = MimsClient.PostXml(MimsConfig.CdsApiUrl, xmlRequest, out isTimeout);
            Inventec.Common.Logging.LogSystem.Debug(string.Format(
                "DrugDrugInteractionService.Check - isTimeout={0}, responseLength={1}",
                isTimeout, xmlResponse == null ? 0 : xmlResponse.Length));

            var result = new MimsResult
            {
                RawXml = xmlResponse,
                IsTimeout = isTimeout
            };

            if (isTimeout)
            {
                result.Success = false;
                result.Message = "Kiểm tra kết nối MIMS";
                result.Html = BuildSimpleHtml(result.Message);
                return result;
            }

            if (string.IsNullOrEmpty(xmlResponse))
            {
                result.Success = false;
                result.Message = "No response from MIMS API";
                result.Html = BuildSimpleHtml(result.Message);
                return result;
            }

            var trimmed = xmlResponse.TrimStart();
            if (trimmed.StartsWith("<Error", System.StringComparison.OrdinalIgnoreCase))
            {
                result.IsErrorResponse = true;
                try
                {
                    var doc = XDocument.Parse(xmlResponse);
                    result.ErrorMessage = (string)doc.Root.Element("Message");
                }
                catch
                {
                    // ignore parse error, just fall back to raw xml
                    result.ErrorMessage = xmlResponse;
                }

                result.Success = false;
                result.Message = result.ErrorMessage;
                result.Html = BuildSimpleHtml(result.ErrorMessage ?? "MIMS trả về lỗi.");
                return result;
            }

            // Normal successful case
            result.Html = MimsResponseTransformer.XmlToHtml(xmlResponse);
            result.Success = !string.IsNullOrEmpty(result.Html);

            // Parse chi tiết CDS Drug–Drug Alert
            result.DrugDrugAlertDetails = MimsResultDetailParser.ParseDrugDrugAlerts(xmlResponse);

            Inventec.Common.Logging.LogSystem.Debug(string.Format(
                "DrugDrugInteractionService.Check - Success={0}, DrugDrugAlertDetails.Count={1}",
                result.Success,
                result.DrugDrugAlertDetails == null ? 0 : result.DrugDrugAlertDetails.Count));

            return result;
        }

        public void ShowResultAsync(List<DrugItem> current, List<DrugItem> previous)
        {
            WebViewHelper.ShowResultAsync(() => CheckWithVnFallback(current, previous), NameText);
        }

        /// <summary>
        /// Helper cho ShowResultAsync: CDS Drug-Drug check trước, fallback VN nếu không có alert.
        /// </summary>
        private MimsResult CheckWithVnFallback(List<DrugItem> current, List<DrugItem> previous)
        {
            MimsResult cdsResult = Check(current, previous);

            bool hasCdsAlert = cdsResult != null && cdsResult.Success
                && cdsResult.DrugDrugAlertDetails != null
                && cdsResult.DrugDrugAlertDetails.Count > 0;

            Inventec.Common.Logging.LogSystem.Debug(string.Format(
                "DrugDrugInteractionService.CheckWithVnFallback - hasCdsAlert={0}", hasCdsAlert));

            if (hasCdsAlert) return cdsResult;

            MimsResult vnResult = CheckVnContraindication(current);
            bool hasVnAlert = vnResult != null
                && vnResult.VnContraindicationDetails != null
                && vnResult.VnContraindicationDetails.Count > 0
                && !string.IsNullOrEmpty(vnResult.Html);

            Inventec.Common.Logging.LogSystem.Debug(string.Format(
                "DrugDrugInteractionService.CheckWithVnFallback - hasVnAlert={0}", hasVnAlert));

            return hasVnAlert ? vnResult : cdsResult;
        }

        public bool ShowDialog(List<DrugItem> drugs, List<DrugItem> previous)
        {
            return ShowDialog(drugs, previous, null, null, null, null);
        }

        /// <summary>
        /// Overload có log audit: nếu user accept dialog (CDS hoặc VN fallback) thì ghi log.
        /// </summary>
        public bool ShowDialog(List<DrugItem> drugs, List<DrugItem> previous,
            HIS_MIMS_INTERACTION_LOG interactionLog,
            long? treatmentId, long? serviceReqId, long? patientId)
        {
            try
            {
                Inventec.Common.Logging.LogSystem.Debug(
                    "DrugDrugInteractionService.ShowDialog - start"
                    + Inventec.Common.Logging.LogUtil.TraceData(
                        Inventec.Common.Logging.LogUtil.GetMemberName(() => drugs), drugs));

                MimsResult result = Check(drugs, previous);

                bool hasCdsAlert = result != null && result.Success
                    && result.DrugDrugAlertDetails != null
                    && result.DrugDrugAlertDetails.Count > 0;

                Inventec.Common.Logging.LogSystem.Debug(string.Format(
                    "DrugDrugInteractionService.ShowDialog - hasCdsAlert={0}", hasCdsAlert));

                if (hasCdsAlert && !string.IsNullOrEmpty(result.Html))
                {
                    Inventec.Common.Logging.LogSystem.Debug(
                        "DrugDrugInteractionService.ShowDialog - showing CDS dialog");
                    bool rs = WebViewHelper.ShowDialog(result.Html, NameText);
                    // Note: CDS Drug-Drug log không lưu ở đây (chưa có SaveDataInteractionLog cho service này).
                    // Plugin có thể tự log sau khi nhận return value.
                    return rs;
                }

                Inventec.Common.Logging.LogSystem.Debug(
                    "DrugDrugInteractionService.ShowDialog - no CDS alert -> fallback to VN Contraindication");
                return CheckAndShowVnContraindication(drugs, interactionLog, treatmentId, serviceReqId, patientId);
            }
            catch (System.Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return true;
        }
    }
}

using System.Collections.Generic;
using System.Xml.Linq;
using HIS.Desktop.MIMS.Integration.Models;
using HIS.Desktop.MIMS.Integration.Core;
using HIS.Desktop.MIMS.Integration.View;

namespace HIS.Desktop.MIMS.Integration.Modules
{
    public class VnContraindicationService : BaseService
    {
        public VnContraindicationService()
        {
            NameText = "Kiểm tra tương tác thuốc.";
        }

        public MimsResult Check(List<string> hisDrugCodes)
        {
            Inventec.Common.Logging.LogSystem.Debug(
                "VnContraindicationService.Check - start"
                + Inventec.Common.Logging.LogUtil.TraceData(
                    Inventec.Common.Logging.LogUtil.GetMemberName(() => hisDrugCodes), hisDrugCodes));

            string xmlRequest = MimsRequestBuilder.BuildVnContraindicationRequest(hisDrugCodes);
            Inventec.Common.Logging.LogSystem.Debug("VnContraindicationService.Check - requestXml: " + xmlRequest);

            bool isTimeout;
            string xmlResponse = MimsClient.PostXml(MimsConfig.VnContraApiUrl, xmlRequest, out isTimeout);
            Inventec.Common.Logging.LogSystem.Debug(string.Format(
                "VnContraindicationService.Check - isTimeout={0}, responseLength={1}",
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
                    result.ErrorMessage = xmlResponse;
                }

                result.Success = false;
                result.Message = result.ErrorMessage;
                result.Html = BuildSimpleHtml(result.ErrorMessage ?? "MIMS trả về lỗi.");
                return result;
            }

            result.Html = MimsResponseTransformer.XmlToHtml(xmlResponse);
            result.Success = !string.IsNullOrEmpty(result.Html);

            // Parse chi tiết CAP_TUONG_TAC cho VN Contraindication Alert (theo mẫu "VN Contraindication Alert Result 02")
            result.VnContraindicationDetails = MimsResultDetailParser.ParseVnContraindicationInteractions(xmlResponse);

            Inventec.Common.Logging.LogSystem.Debug(string.Format(
                "VnContraindicationService.Check - Success={0}, VnContraindicationDetails.Count={1}",
                result.Success,
                result.VnContraindicationDetails == null ? 0 : result.VnContraindicationDetails.Count));

            return result;
        }

        public void ShowResultAsync(List<string> hisDrugCodes)
        {
            WebViewHelper.ShowResultAsync(() => Check(hisDrugCodes), NameText);
        }

        public MimsResult Check(List<DrugItem> drugs)
        {
            Inventec.Common.Logging.LogSystem.Debug(
                "VnContraindicationService.Check(List<DrugItem>) - start"
                + Inventec.Common.Logging.LogUtil.TraceData(
                    Inventec.Common.Logging.LogUtil.GetMemberName(() => drugs), drugs));

            var atcCodes = ExtractAtcCodes(drugs);
            if (atcCodes.Count == 0)
            {
                Inventec.Common.Logging.LogSystem.Debug(
                    "VnContraindicationService.Check(List<DrugItem>) - ABORT: ExtractAtcCodes returned empty");
                var r = new MimsResult { Success = false };
                r.Message = "Không tìm thấy mã ATC cho các thuốc được chọn";
                r.Html = BuildSimpleHtml(r.Message);
                return r;
            }
            return Check(atcCodes);
        }

        public void ShowResultAsync(List<DrugItem> drugs)
        {
            WebViewHelper.ShowResultAsync(() => Check(drugs), NameText);
        }
    }
}

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
            string xmlRequest = MimsRequestBuilder.BuildVnContraindicationRequest(hisDrugCodes);

            bool isTimeout;
            string xmlResponse = MimsClient.PostXml(MimsConfig.VnContraApiUrl, xmlRequest, out isTimeout);

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

            return result;
        }

        public void ShowResultAsync(List<string> hisDrugCodes)
        {
            WebViewHelper.ShowResultAsync(() => Check(hisDrugCodes), NameText);
        }
	}
}

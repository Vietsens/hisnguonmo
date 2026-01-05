using System.Collections.Generic;
using System.Xml.Linq;
using HIS.Desktop.MIMS.Integration.Models;
using HIS.Desktop.MIMS.Integration.Core;
using HIS.Desktop.MIMS.Integration.View;

namespace HIS.Desktop.MIMS.Integration.Modules
{
    public class DrugInfomationService
    {
        private static string BuildSimpleHtml(string message)
        {
            string safe = System.Security.SecurityElement.Escape(message ?? string.Empty);
            return "<html><head><meta charset=\"utf-8\"/></head><body><h3>" + safe + "</h3></body></html>";
        }

        public MimsResult Check(DrugItem drug)
        {
            string xmlRequest = MimsRequestBuilder.BuildDrugInformationRequest(drug);

            bool isTimeout;
            string xmlResponse = MimsClient.PostXml(MimsConfig.CdsApiUrl, xmlRequest, out isTimeout);

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

            // Normal successful case
            result.Html = MimsResponseTransformer.XmlToHtml(xmlResponse);
            result.Success = !string.IsNullOrEmpty(result.Html);

            return result;
        }
        public void ShowResult(MimsResult result)
        {
            if (result != null && !string.IsNullOrEmpty(result.Html))
            {
                WebViewHelper.ShowHtml(result.Html, "Thông tin thuốc");
            }
        }

        public void ShowResultAsync(DrugItem drug)
        {
            WebViewHelper.ShowResultAsync(() => Check(drug), "Thông tin thuốc");
        }
    }
}

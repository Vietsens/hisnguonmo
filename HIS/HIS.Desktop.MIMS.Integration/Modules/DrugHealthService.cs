using System.Collections.Generic;
using System.Xml.Linq;
using HIS.Desktop.MIMS.Integration.Core;
using HIS.Desktop.MIMS.Integration.Models;
using HIS.Desktop.MIMS.Integration.View;

namespace HIS.Desktop.MIMS.Integration.Modules
{
    public class DrugHealthService
    {
        private static string BuildSimpleHtml(string message)
        {
            string safe = System.Security.SecurityElement.Escape(message ?? string.Empty);
            return "<html><head><meta charset=\"utf-8\"/></head><body><h3>" + safe + "</h3></body></html>";
        }

        /// <summary>
        /// Kiểm tra Drug-Health Alert theo tài liệu MIMS (Prescribing + HealthIssueCodes ICD10).
        /// </summary>
        public MimsResult Check(List<DrugItem> drugs, List<string> icd10Codes)
        {
            var result = new MimsResult();
            if (drugs == null || drugs.Count == 0 || !drugs.Exists(o=>o.MimsGuid!=null))
            {
                result.Success = false;
                result.Message = "Không có thông tin thuốc kiểm tra tương tác";
                result.Html = BuildSimpleHtml(result.Message);
                ShowResult(result);
                return result;
            }
            string xmlRequest = MimsRequestBuilder.BuildDrugHealthAlertRequest(drugs, icd10Codes);

            bool isTimeout;
            string xmlResponse = MimsClient.PostXml(MimsConfig.CdsApiUrl, xmlRequest, out isTimeout);

            result.RawXml = xmlResponse;
            result.IsTimeout = isTimeout;

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

            // Parse chi tiết Drug-Health Alert theo Result XML trong tài liệu MIMS.
            result.DrugHealthAlertDetails = MimsResultDetailParser.ParseDrugHealthAlerts(xmlResponse);

            return result;
        }

        public void ShowResult(MimsResult result)
        {
            if (result != null && !string.IsNullOrEmpty(result.Html))
            {
                WebViewHelper.ShowHtml(result.Html, "Kiểm tra bệnh lý nền (Drug-Health Alert)");
            }
        }

        public void ShowResultAsync(List<DrugItem> drugs, List<string> icd10Codes)
        {
            WebViewHelper.ShowResultAsync(() => Check(drugs, icd10Codes), "Kiểm tra bệnh lý nền (Drug-Health Alert)");
        }
    }
}

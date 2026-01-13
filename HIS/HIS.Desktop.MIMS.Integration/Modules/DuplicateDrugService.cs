using System.Collections.Generic;
using System.Xml.Linq;
using HIS.Desktop.MIMS.Integration.Models;
using HIS.Desktop.MIMS.Integration.Core;
using HIS.Desktop.MIMS.Integration.View;

namespace HIS.Desktop.MIMS.Integration.Modules
{
	public class DuplicateDrugService : BaseService
	{
        public DuplicateDrugService()
        {
            NameText = "Kiểm tra trùng lặp thuốc";
        }

        public MimsResult Check(List<DrugItem> drugs)
        {
            return this.Check(drugs, null);
        }

        public MimsResult Check(List<DrugItem> current, List<DrugItem> previous)
		{
            this.MappingMIMS(current);
            this.MappingMIMS(previous);
            string xmlRequest = MimsRequestBuilder.BuildDrugInteractionRequest(current, true);

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

			result.Html = MimsResponseTransformer.XmlToHtml(xmlResponse);
			result.Success = !string.IsNullOrEmpty(result.Html);
			return result;
		}

        public void ShowResultAsync(List<DrugItem> drugs)
        {
            WebViewHelper.ShowResultAsync(() => Check(drugs), NameText);
        }
        public void ShowResultAsync(List<DrugItem> current, List<DrugItem> previous)
        {
            WebViewHelper.ShowResultAsync(() => Check(current, previous), NameText);
        }
	}
}

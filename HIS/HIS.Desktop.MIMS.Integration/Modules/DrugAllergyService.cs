using System.Collections.Generic;
using System.Xml.Linq;
using HIS.Desktop.MIMS.Integration.Models;
using HIS.Desktop.MIMS.Integration.Core;
using HIS.Desktop.MIMS.Integration.View;

namespace HIS.Desktop.MIMS.Integration.Modules
{
	public class DrugAllergyService : BaseService
	{
        public DrugAllergyService()
        {
            NameText = "Kiểm tra dị ứng thuốc";
        }
		public MimsResult Check(List<DrugItem> drugs, List<AllergyItem> allergies)
		{
			string xmlRequest = MimsRequestBuilder.BuildDrugAllergyRequest(drugs, allergies);

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

            // Parse chi tiết Drug-Allergy Alert (theo XML Result trong tài liệu MIMS).
            result.DrugAllergyAlertDetails = MimsResultDetailParser.ParseDrugAllergyAlerts(xmlResponse);

			return result;
		}

        public void ShowResultAsync(List<DrugItem> drugs, List<AllergyItem> allergies)
        {
            WebViewHelper.ShowResultAsync(() => Check(drugs, allergies), NameText);
        }
	}
}

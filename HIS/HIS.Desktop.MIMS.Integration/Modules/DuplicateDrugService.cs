using System.Collections.Generic;
using HIS.Desktop.MIMS.Integration.Models;
using HIS.Desktop.MIMS.Integration.Core;
using HIS.Desktop.MIMS.Integration.View;

namespace HIS.Desktop.MIMS.Integration.Modules
{
	public class DuplicateDrugService
	{
		public MimsResult Check(List<DrugItem> drugs)
		{
			string xmlRequest = MimsRequestBuilder.BuildDrugInteractionRequest(drugs);
			string xmlResponse = MimsClient.PostXml(MimsConfig.CdsApiUrl, xmlRequest);
			var result = new MimsResult
			{
				RawXml = xmlResponse,
                Html = MimsResponseTransformer.XmlToHtml(xmlResponse),
				Success = !string.IsNullOrEmpty(xmlResponse),
				Message = string.IsNullOrEmpty(xmlResponse) ? "No response from MIMS API" : null
			};
			return result;
		}

		public void ShowResult(MimsResult result)
		{
			if (result != null && !string.IsNullOrEmpty(result.Html))
			{
				WebViewHelper.ShowHtml(result.Html, "Kiểm tra trùng lặp thuốc");
			}
		}
	}
}

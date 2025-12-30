using System.Collections.Generic;
using HIS.Desktop.MIMS.Integration.Models;
using HIS.Desktop.MIMS.Integration.Core;
using HIS.Desktop.MIMS.Integration.View;

namespace HIS.Desktop.MIMS.Integration.Modules
{
    public class VnContraindicationService
    {
        public MimsResult Check(List<string> hisDrugCodes)
        {
            string xmlRequest = MimsRequestBuilder.BuildVnContraindicationRequest(hisDrugCodes);
            string xmlResponse = MimsClient.PostXml(MimsConfig.VnContraApiUrl, xmlRequest);
            var result = new MimsResult
            {
                RawXml = xmlResponse,
                Html = MimsResponseTransformer.XmlToHtml(xmlResponse),
                Success = !string.IsNullOrEmpty(xmlResponse),
                Message = string.IsNullOrEmpty(xmlResponse) ? "No response from MIMS API" : null
            };

            // Parse chi tiết CAP_TUONG_TAC cho VN Contraindication Alert (theo mẫu "VN Contraindication Alert Result 02")
            result.VnContraindicationDetails = MimsResultDetailParser.ParseVnContraindicationInteractions(xmlResponse);

            return result;
        }

        public void ShowResult(MimsResult result)
		{
			if (result != null && !string.IsNullOrEmpty(result.Html))
			{
				WebViewHelper.ShowHtml(result.Html, "Kiểm tra chống chỉ định Việt Nam");
			}
		}
    }
}

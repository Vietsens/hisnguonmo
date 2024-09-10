using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.ElectronicBillViettel.Base
{
    class RequestUriStore
    {
        /// <summary>
        /// {supplierTaxCode}: mã số thuế của doanh nghiệp/chi nhánh phát hành
        /// {1}= {supplierTaxCode}
        /// Mẫu 1: 0312770607 
        /// Mẫu 2: 0312770607-001
        /// </summary>
        internal const string CreateInvoiceV1 = "/InvoiceAPI/InvoiceWS/createInvoice/{0}";
        internal const string CreateInvoiceV2 = "/services/einvoiceapplication/api/InvoiceAPI/InvoiceWS/createInvoice/{0}";

        internal const string CancelInvoiceV1 = "/InvoiceAPI/InvoiceWS/cancelTransactionInvoice";
        internal const string CancelInvoiceV2 = "/services/einvoiceapplication/api/InvoiceAPI/InvoiceWS/cancelTransactionInvoice";

        internal const string GetFileInvoiceV1 = "/InvoiceAPI/InvoiceWS/createExchangeInvoiceFile";
        internal const string GetFileInvoiceV2 = "/services/einvoiceapplication/api/InvoiceAPI/InvoiceWS/createExchangeInvoiceFile";

        internal const string GetCustomFieldsV1 = "/InvoiceAPI/InvoiceWS/getCustomFields";
        internal const string GetCustomFieldsV2 = "/services/einvoiceapplication/api/InvoiceAPI/InvoiceWS/getCustomFields";

        internal const string GetInvoiceRepresentationFileV1 = "/InvoiceAPI/InvoiceUtilsWS/getInvoiceRepresentationFile";
        internal const string GetInvoiceRepresentationFileV2 = "/services/einvoiceapplication/api/InvoiceAPI/InvoiceUtilsWS/getInvoiceRepresentationFile";

        //internal const string SearchInvoiceByTransactionUuidV1 = "/InvoiceAPI/InvoiceWS/searchInvoiceByTransactionUuid";
        internal const string SearchInvoiceByTransactionUuidV2 = "/services/einvoiceapplication/api/InvoiceAPI/InvoiceWS/searchInvoiceByTransactionUuid";

        internal const string Login = "/auth/login";

        /// <summary>
        /// {supplierTaxCode}: mã số thuế của doanh nghiệp/chi nhánh phát hành
        /// {1}= {supplierTaxCode}
        /// Mẫu 1: 0312770607 
        /// Mẫu 2: 0312770607-001
        /// </summary>
        internal const string GetInvoicesV1 = "/InvoiceAPI/InvoiceUtilsWS/getInvoices/{0}";
        internal const string GetInvoicesV2 = "/services/einvoiceapplication/api/InvoiceAPI/InvoiceUtilsWS/getInvoices/{0}";

        internal static string CombileUrl(params string[] data)
        {
            string result = "";
            List<string> pathUrl = new List<string>();
            for (int i = 0; i < data.Length; i++)
            {
                pathUrl.Add(data[i].Trim('/'));
            }

            result = string.Join("/", pathUrl);

            //Inventec.Common.Logging.LogSystem.Debug("CombileUrl:" + result);
            return result;
        }
    }
}

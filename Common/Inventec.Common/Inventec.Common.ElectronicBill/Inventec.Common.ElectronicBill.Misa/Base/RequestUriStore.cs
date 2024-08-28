using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.ElectronicBill.Misa.Base
{
    class RequestUriStore
    {
        internal const string CreateInvoice = "/invoice/invoicing";

        internal const string ReleaseInvoice = "/invoice";

        internal const string CancelInvoice = "/invoice/deleted";

        internal const string GetFileInvoice = "/invoice/download?downloadDatatype=pdf";

        internal const string ConvertFileInvoice = "/invoice/voucher-paper?converter={0}";

        internal const string ApiLogin = "/user/token?appid={0}&taxcode={1}&username={2}&password={3}";

        internal const string SignXml = "/api/SignXML";

        internal const string PreviewInvoice = "/invoice/view?viewType=Pdf";

        internal const string ApiLoginV2 = "/auth/token";
        internal const string CreateInvoiceV2 = "/itg/invoicepublishing/createinvoice";
        internal const string PreviewInvoiceV2 = "/itg/invoicepublishing/invoicelinkview?type=1";
        internal const string ReleaseInvoiceV2 = "/itg/invoicepublishing";

        internal const string CancelInvoiceV2 = "/invoiceprocessing/cancelvouchers";

        internal const string GetFileInvoiceV2 = "/DownloadHandler.ashx?Type=pdf&Code={0}";
        //internal const string GetFileInvoiceV2 = "/itg/invoicepublished/linkview";
        internal const string ConvertFileInvoiceV2 = "/itg/invoicepublished/voucher-paper";
    }
}

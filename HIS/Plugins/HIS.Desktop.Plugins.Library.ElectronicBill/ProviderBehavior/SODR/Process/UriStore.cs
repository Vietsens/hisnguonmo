using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.ElectronicBill.ProviderBehavior.SODR.Process
{
    internal class UriStore
    {
        internal static string LoginUrl = "/api/authen-v2/login";

        internal static string EinvoiceIssueUrl = "/api/einvoice/issue";

        internal static string EinvoiceCancel = "/api/einvoice/cancel";

        internal static string EinvoiceDownload = "/api/einvoice/download";
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.EBillSoftDreams.Base
{
    class RequestUriStore
    {
        internal const string CreateInvoice = "api/publish/importAndIssueInvoice";
        internal const string ImportInvoice = "api/publish/importInvoice";

        internal const string CancelInvoice = "/api/business/cancelInvoice";

        internal const string GetFileInvoice = "/api/publish/getInvoicePdf";
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.ElectronicBillMoit.Base
{
    class RequestUriStore
    {
        internal const string CreateInvoice = "/api/hoadon/xuathoadon";

        internal const string CancelInvoice = "/api/business/cancelinv";

        internal const string GetFileInvoice = "/api/hoadon/chuyendoihoadon";

        internal const string CreateInvoiceTT78 = "/api/tt78/hoadon/xuathoadon";

        internal const string DeleteInvoiceTT78 = "/api/tt78/business/deleteinv";

        internal const string GetFileInvoiceTT78 = "/api/tt78/business/invoicebykey";
    }
}

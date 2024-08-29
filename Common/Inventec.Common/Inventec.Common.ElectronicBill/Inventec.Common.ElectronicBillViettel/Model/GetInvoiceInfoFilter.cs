using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.ElectronicBillViettel.Model
{
    public class GetInvoiceInfoFilter
    {
        public string invoiceNo { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public long rowPerPage { get; set; }
        public long pageNum { get; set; }

        public string buyerTaxCode { get; set; }
        public string invoiceType { get; set; }
        public string templateCode { get; set; }
        public string invoiceSeri { get; set; }
        public bool getAll { get; set; }
    }
}

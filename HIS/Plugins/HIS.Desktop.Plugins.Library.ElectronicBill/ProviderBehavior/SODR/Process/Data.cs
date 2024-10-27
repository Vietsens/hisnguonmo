using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.ElectronicBill.ProviderBehavior.SODR.Process
{
    public class IssueCreateV2
    {
        public string Pattern { get; set; }

        public List<InvoiceV2> Invoices { get; set; }
    }
    public class InvoiceRs
    {
        public string Pattern { get; set; }

        public string Ikey { get; set; }

        public string No { get; set; }

        public string TaxAuthorityCode { get; set; }
    }

    public class InvoiceV2
    {
        public string TaxAuthorityCode { get; set; }

        public string ArisingDate { get; set; }

        public string CusAddress { get; set; }

        public string CusName { get; set; }

        public string CusPhone { get; set; }

        public string CusTaxCode { get; set; }

        public string CusBankName { get; set; }

        public string CusBankNo { get; set; }

        public string Email { get; set; }

        public string Extra { get; set; }

        public string Ikey { get; set; }

        public string PaymentMethod { get; set; }

        public List<ProductV2> Products { get; set; }
    }
    public class ProductV2
    {
        public string Code { get; set; }

        public string Feature { get; set; }

        public string No { get; set; }

        public string Name { get; set; }

        public decimal Price { get; set; }

        public decimal Quantity { get; set; }

        public string Unit { get; set; }

        public decimal Discount { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal Total { get; set; }

        public decimal VATAmount { get; set; }

        public float VATRate { get; set; }

        public decimal Amount { get; set; }

        public string Extra { get; set; }
    }

    public class ResultDataV2
    {
        public string Status { get; set; }

        public string Message { get; set; }

        public string Data { get; set; }

        public List<InvoiceRs> InvoiceResult { get; set; }

        public string AccessToken { get; set; }
    }
}

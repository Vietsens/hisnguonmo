using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.ElectronicBill.ProviderBehavior.VNInvoice.Model
{
    class InputCreateInvoice
    {
        public string erpId { get; set; }
        public string creatorErp { get; set; }
        public string transactionId { get; set; }
        public DateTime invoiceDate { get; set; }
        public string note { get; set; }
        public string storeCode { get; set; }
        public string storeName { get; set; }
        public bool isFinancialLeaseInvoice { get; set; }
        public string budgetUnitCode { get; set; }
        public string buyerIDNumber { get; set; }
        public string paymentMethod { get; set; }
        public string currency { get; set; }
        public double exchangeRate { get; set; }
        public short discountType { get; set; }
        public decimal totalAmount { get; set; }
        public decimal totalVatAmount { get; set; }
        public decimal totalPaymentAmount { get; set; }
        public decimal totalDiscountAmountBeforeTax { get; set; }
        public decimal totalDiscountAmountAfterTax { get; set; }
        public string buyerCode { get; set; }
        public string buyerEmail { get; set; }
        public string buyerFullName { get; set; }
        public string buyerLegalName { get; set; }
        public string buyerTaxCode { get; set; }
        public string buyerAddressLine { get; set; }
        public string buyerDistrictName { get; set; }
        public string buyerCityName { get; set; }
        public string buyerCountryCode { get; set; }
        public string buyerPhoneNumber { get; set; }
        public string buyerFaxNumber { get; set; }
        public string buyerBankAccount { get; set; }
        public string buyerBankName { get; set; }
        public List<InvoiceDetail> invoiceDetails { get; set; }
        public List<InvoiceHeaderExtra> invoiceHeaderExtras { get; set; }
        public List<InvoiceDetailExtra> invoiceDetailExtras { get; set; }
        public List<InvoiceTaxBreakdown> invoiceTaxBreakdowns { get; set; }
        public List<InvoiceSpecificProductExtra> invoiceSpecificProductExtras { get; set; }
    }
    public class InvoiceSpecificProductExtra
    {
        public int type { get; set; }
        public string fieldName { get; set; }
        public string fieldValue { get; set; }
    }
    public class OutputCreateInvoice
    {
        public string id { get; set; }             // id hóa đơn bên VNIs
        public string erpId { get; set; }          // id hóa đơn bên ERP
        public string transactionId { get; set; }  // Mã giao dịch
        public int templateNo { get; set; }        // Mẫu số hóa đơn
        public string serialNo { get; set; }       // Ký hiệu hóa đơn
        public string invoiceNo { get; set; }      // Số hóa đơn
        public int invoiceStatus { get; set; }     // Trạng thái hóa đơn
        public int signStatus { get; set; }        // Trạng thái ký
        public bool succeeded { get; set; }
        public int code { get; set; }
        public string message { get; set; }
    }
}

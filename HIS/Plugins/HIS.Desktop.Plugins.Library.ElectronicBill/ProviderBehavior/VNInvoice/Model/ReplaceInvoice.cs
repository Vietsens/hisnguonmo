using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.ElectronicBill.ProviderBehavior.VNInvoice.Model
{
    public class InputReplaceInvoice
    {
        public string erpIdReference { get; set; }
        public int templateNo { get; set; }
        public string serialNo { get; set; }          // Ký hiệu hóa đơn cần thay thế
        public string invoiceNo { get; set; }         // Số hóa đơn cần thay thế
        public string erpId { get; set; }             // id hóa đơn bên ERP
        public string creatorErp { get; set; }        // Người tạo hóa đơn trên ERP
        public string invoiceDate { get; set; }     // Ngày hóa đơn (yyyy-MM-dd)
        public string note { get; set; }              // Ghi chú của toàn hóa đơn
        public string paymentMethod { get; set; }     // Phương thức thanh toán (TM/CK; Tiền mặt; Chuyển khoản…)
        public string currency { get; set; }          // Tiền tệ (VD: VND)
        public double exchangeRate { get; set; }
        public short discountType { get; set; }
        public decimal totalAmount { get; set; }
        public decimal totalVatAmount { get; set; }
        public decimal totalPaymentAmount { get; set; }          
        public double totalDiscountAmountBeforeTax { get; set; }  
        public string buyerCode { get; set; }                  
        public string buyerEmail { get; set; }                   
        public string buyerFullName { get; set; }                
        public string buyerLegalName { get; set; }                
        public string buyerTaxCode { get; set; }
        public string buyerAddressLine { get; set; }    // Địa chỉ khách hàng
        public string buyerDistrictName { get; set; }   // Quận/huyện khách hàng
        public string buyerCityName { get; set; }       // Tỉnh/thành phố khách hàng
        public string buyerCountryCode { get; set; }    // Mã quốc gia khách hàng
        public string buyerPhoneNumber { get; set; }    // Số điện thoại khách hàng
        public string buyerFaxNumber { get; set; }      // Số Fax khách hàng
        public string buyerBankAccount { get; set; }    // Số tài khoản ngân hàng
        public string buyerBankName { get; set; }       // Chi nhánh ngân hàng của khách hàng
        public List<VNInvoiceDetail> invoiceDetails { get; set; }
        public List<InvoiceDetailExtra> invoiceDetailExtras { get; set; }
        public List<InvoiceHeaderExtra> invoiceHeaderExtras { get; set; }
        public List<InvoiceTaxBreakdown> invoiceTaxBreakdowns { get; set; }
        public InvoicePrintNote invoicePrintNote { get; set; }
    }
    public class VNInvoiceDetail
    {
        public int index { get; set; }                        // Số thứ tự chi tiết trong hóa đơn
        public decimal discountAmountBeforeTax { get; set; }  // Tiền chiết khấu cho dòng chi tiết (trước thuế)
        public double discountPercentBeforeTax { get; set; }  // Phần trăm chiết khấu cho dòng chi tiết (trước thuế)
        public decimal? paymentAmount { get; set; }            // Thành tiền sau VAT của dòng chi tiết
        public string productCode { get; set; }                  // Mã sản phẩm
        public int productType { get; set; }
        public string productName { get; set; }                  // Tên sản phẩm
        public string unitName { get; set; }                     // Đơn vị tính
        public decimal? unitPrice { get; set; }                   // Đơn giá
        public double quantity { get; set; }                     // Số lượng
        public decimal? amount { get; set; }                      // Thành tiền chưa VAT
        public int vatPercent { get; set; }                      // Phần trăm thuế
        public decimal vatAmount { get; set; }
        public string note { get; set; }
    }
    public class InvoiceHeaderExtra
    {
        public string fieldName { get; set; }    // Tên trường mở rộng
        public string fieldValue { get; set; }   // Giá trị trường mở rộng
    }
    public class InvoiceDetailExtra
    {
        public string fieldName { get; set; }    // Tên trường mở rộng
        public string fieldValue { get; set; }   // Giá trị trường mở rộng
    }
    public class InvoiceTaxBreakdown
    {
        public decimal vatAmount { get; set; }
        public int vatPercent { get; set; }
    }
    public class InvoicePrintNote
    {
        public string note { get; set; }
        public bool isShowNote { get; set; }
    }
    public class OutputReplaceInvoice
    {
        public List<OutPutDataReplace> data { get; set; }
        public bool succeeded { get; set; }       
        public int code { get; set; }       
        public string message { get; set; }
        public string errors { get; set; }
    }
    public class OutPutDataReplace
    {

        public string id { get; set; }             // id hóa đơn bên VNIs
        public string erpId { get; set; }          // id hóa đơn bên ERP
        public string transactionId { get; set; }  // Mã giao dịch
        public int templateNo { get; set; }        // Mẫu số hóa đơn
        public string serialNo { get; set; }       // Ký hiệu hóa đơn
        public string invoiceNo { get; set; }      // Số hóa đơn
        public int invoiceStatus { get; set; }     // Trạng thái hóa đơn
        public int signStatus { get; set; }        // Trạng thái ký
    }

}


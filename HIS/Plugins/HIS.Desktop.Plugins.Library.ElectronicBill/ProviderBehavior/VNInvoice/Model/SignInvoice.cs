using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.ElectronicBill.ProviderBehavior.VNInvoice.Model
{
    public class OutputSignInvoice
    {
        public List<OutPutDataSign> data { get; set; }
        public bool succeeded { get; set; }
        public int code { get; set; }
        public string message { get; set; }
        public string errors { get; set; }
    }
    public class OutPutDataSign
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

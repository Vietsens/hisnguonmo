using Inventec.Common.ElectronicBill.MD;
using Inventec.Common.ElectronicBill.ModelTT78;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.ElectronicBill.Base
{
    public class ElectronicBillInput
    {
        public string serviceUrl { get; set; }
        public string account { get; set; }
        public string acPass { get; set; }
        public List<Invoice> invoices { get; set; }
        public List<Invoice_BM> invoicesBm { get; set; }
        public string pattern { get; set; }
        public string serial { get; set; }
        public string userName { get; set; }
        public string passWord { get; set; }
        public int convert { get; set; }
        public string fKey { get; set; }
        public string DataXmlStringPlus { get; set; }
        public List<HDon> invoiceTT78s { get; set; }
    }
}

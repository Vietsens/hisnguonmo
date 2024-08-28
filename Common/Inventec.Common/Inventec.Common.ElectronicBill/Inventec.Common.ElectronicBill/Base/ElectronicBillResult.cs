using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.ElectronicBill.Base
{
    public class ElectronicBillResult
    {
        public bool Success { get; set; }
        public List<string> Messages { get; set; }
        public string Data { get; set; }
        public int Status { get; set; }
        public string MessLog { get; set; }
        public string InvoiceLink { get; set; }
    }
}

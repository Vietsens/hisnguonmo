using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.ElectronicBillMoit
{
    public class InvoiceResult
    {
        public bool Status { get; set; }
        public string MessLog { get; set; }
        public string Key { get; set; }
        public string fKey { get; set; }
        public string NumOrder { get; set; }

        public string PdfFile { get; set; }
    }
}

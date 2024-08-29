using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.EBillSoftDreams.Model
{
    internal class CreateInvoiceResult : CommonInvoice
    {
        public List<CommonInvoiceResult> Invoices { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, string> KeyInvoiceMsg { get; set; }
    }
}

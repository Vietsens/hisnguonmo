using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.ElectronicBillMoit.Model
{
    public class CreateInvoice
    {
        public string xmlData { get; set; }
        public string pattern { get; set; }
        public string serial { get; set; }
        public bool convert { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.ElectronicBill.Misa.Model
{
    class VoucherPaperADO
    {
        public List<string> TransactionIDList { get; set; }
        public string Converter { get; set; }
        public DateTime ConvertDate { get; set; }
    }
}

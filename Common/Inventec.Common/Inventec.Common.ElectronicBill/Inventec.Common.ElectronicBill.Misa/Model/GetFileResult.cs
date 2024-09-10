using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.ElectronicBill.Misa.Model
{
    public class GetFileResult
    {
        public string TransactionID { get; set; }
        public string Data { get; set; }
        public string ErrorCode { get; set; }
    }
}

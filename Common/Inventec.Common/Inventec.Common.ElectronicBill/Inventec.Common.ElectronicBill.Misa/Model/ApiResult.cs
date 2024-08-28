using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.ElectronicBill.Misa.Model
{
    public class ApiResult
    {
        public string Data { get; set; }
        public bool Success { get; set; }
        public List<string> Errors { get; set; }
        public string ErrorCode { get; set; }
        public string NewData { get; set; }
        public string CustomData { get; set; }
    }
}

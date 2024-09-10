using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.ElectronicBillMoit.Model
{
    public class ApiResultData
    {
        public bool success { get; set; }
        public string error { get; set; }
        public string messages { get; set; }
        public object data { get; set; }
    }
}

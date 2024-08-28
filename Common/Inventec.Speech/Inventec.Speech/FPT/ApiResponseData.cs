using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Speech.FPT
{
    class ApiResponseData
    {
        public string async { get; set; }
        public string error { get; set; }
        public string message { get; set; }
        public string request_id { get; set; }
    }
}

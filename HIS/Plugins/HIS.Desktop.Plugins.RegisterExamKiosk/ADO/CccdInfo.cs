using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.RegisterExamKiosk.ADO
{
    public class CccdInfo
    {
        public string identifyNumber { get; set; }
        public string previousNumber { get; set; }
        public string name { get; set; }
        public string sex { get; set; }
        public string dateOfBirth { get; set; }
        public string address { get; set; }
        public string issueDate { get; set; }
    }

    public class ResultWrapper
    {
        public CccdInfo data { get; set; }
    }

    public class RootResponse
    {
        public bool success { get; set; }
        public string serverTime { get; set; }
        public ResultWrapper result { get; set; }
    }
}

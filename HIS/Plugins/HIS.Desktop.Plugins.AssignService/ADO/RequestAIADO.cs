using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AssignService.ADO
{
    public class RequestAIADO
    {
        public string icd_code { get; set; }
        public string gender_name { get; set; }
        public int age { get; set; }
        public int top_n { get; set; }
    }
}

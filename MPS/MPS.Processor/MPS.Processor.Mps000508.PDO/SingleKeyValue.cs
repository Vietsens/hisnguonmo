using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000508.PDO
{
    public class SingleKeyValue
    {
        public string departmentName { get; set; }
        public string roomName { get; set; }

        public string RATIO_STR { get; set; }
        public long TOTAL_DAY { get; set; }
        public string CURRENT_DATE_SEPARATE_FULL_STR { get; set; }
        public string USERNAME_RETURN_RESULT { get; set; }
        public string STATUS_TREATMENT_OUT { get; set; }
        public string PAY_VIEW_OPTION { get; set; }
        public long? CURRENT_SERVER_TIME { get; set; }
    }
}

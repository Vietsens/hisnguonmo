using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps190001
{
    class SereServGroupPlusADO : V_HIS_SERE_SERV
    {
        public string SERVICE_CODE { get; set; }
        public string SERVICE_NAME { get; set; }
        public string SERVICE_REQ_CODE { get; set; }
        public string VIR_TOTAL_PRICE_OTHER { get; set; } //them moi

        public long? BEGIN_TIME { get; set; }
        public long? END_TIME { get; set; }
        public string INSTRUCTION_NOTE { get; set; }
        public string NOTE { get; set; }
        public string CONCLUDE { get; set; }
        public string DESCRIPTION { get; set; }

        public long SERVICE_PARENT_ID { get; set; }
        public long? NUM_ORDER { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace MOS.Filter
{
    public class HisHeinApprovalFilter : FilterBase
    {
        public long? EXECUTE_TIME_FROM { get; set; }
        public long? EXECUTE_TIME_TO { get; set; }
        public long? TREATMENT_ID { get; set; }
        public List<long> TREATMENT_IDs { get; set; }
        public bool? IS_DELETE { get; set; }

        public List<string> HEIN_CARD_NUMBER_PREFIXs { get; set; }
        public List<string> HEIN_CARD_NUMBER_PREFIX__NOT_INs { get; set; }

        public string LEVEL_CODE__EXACT { get; set; }
        public string JOIN_5_YEAR__EXACT { get; set; }
        public string PAID_6_MONTH__EXACT { get; set; }
        public string RIGHT_ROUTE_CODE__EXACT { get; set; }
        public string LIVE_AREA_CODE__EXACT { get; set; }
        public string HEIN_MEDI_ORG_CODE__EXACT { get; set; }
        public string HEIN_CARD_NUMBER__EXACT { get; set; }
        public string HEIN_CARD_NUMBER { get; set; }
        
        public HisHeinApprovalFilter()
            : base()
        {
        }
    }
}


using System.Collections.Generic;
namespace MOS.Filter
{
    public class HisImpMestBloodViewFilter : FilterBase
    {
        public long? IMP_MEST_ID { get; set; }
        public long? BLOOD_ID { get; set; }
        public long? BLOOD_TYPE_ID { get; set; }
        public List<long> IMP_MEST_IDs { get; set; }
        public List<long> BLOOD_IDs { get; set; }
        public long? IMP_MEST_STT_ID { get; set; }
        public List<long> IMP_MEST_STT_IDs { get; set; }
        public long? IMP_TIME_FROM { get; set; }
        public long? IMP_TIME_TO { get; set; }
        public long? MEDI_STOCK_ID { get; set; }
        public List<long> MEDI_STOCK_IDs { get; set; }

        public HisImpMestBloodViewFilter()
            : base()
        {
        }
    }
}


using System.Collections.Generic;
namespace MOS.Filter
{
    public class HisBidBloodTypeFilter : FilterBase
    {
        public long? BID_ID { get; set; }
        public long? BLOOD_TYPE_ID { get; set; }
        public long? SUPPLIER_ID { get; set; }
        public List<long> SUPPLIER_IDs { get; set; }
        public List<long> BID_IDs { get; set; }
        public List<long> BLOOD_TYPE_IDs { get; set; }

        public HisBidBloodTypeFilter()
            : base()
        {
        }
    }
}

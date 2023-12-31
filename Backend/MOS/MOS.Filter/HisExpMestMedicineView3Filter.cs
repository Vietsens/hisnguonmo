using System.Collections.Generic;

namespace MOS.Filter
{
    public class HisExpMestMedicineView3Filter : FilterBase
    {
        public List<long> EXP_MEST_IDs { get; set; }
        public List<long> MEDICINE_IDs { get; set; }
        public List<long> VACCINATION_RESULT_IDs { get; set; }
        public List<long> TDL_VACCINATION_IDs { get; set; }
        public long? VACCINATION_RESULT_ID { get; set; }
        public long? TDL_VACCINATION_ID { get; set; }
        public long? EXP_MEST_ID { get; set; }
        public long? MEDICINE_TYPE_ID { get; set; }
        public long? EXP_MEST_TYPE_ID { get; set; }
        public long? MEDICINE_ID { get; set; }
        public long? EXP_TIME_FROM { get; set; }
        public long? EXP_TIME_TO { get; set; }

        public HisExpMestMedicineView3Filter()
            : base()
        {
        }
    }
}

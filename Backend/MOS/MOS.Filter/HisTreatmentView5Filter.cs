
using System.Collections.Generic;
namespace MOS.Filter
{
    public class HisTreatmentView5Filter : FilterBase
    {
        public List<long> ICD_IDs { get; set; }
        public List<long> END_ROOM_IDs { get; set; }
        public List<long> DATA_STORE_ID_NULL__OR__INs { get; set; }
        public List<long> TDL_PATIENT_TYPE_IDs { get; set; }

        public long? CAREER_ID { get; set; }
        public long? FEE_LOCK_TIME_FROM { get; set; }
        public long? FEE_LOCK_TIME_TO { get; set; }
        public long? IN_TIME_FROM { get; set; }
        public long? IN_TIME_TO { get; set; }
        public long? OUT_TIME_FROM { get; set; }
        public long? OUT_TIME_TO { get; set; }
        public bool? IS_LOCK_HEIN { get; set; }
        public bool? IS_PAUSE { get; set; }
        public bool? IS_OUT { get; set; }
        public long? PATIENT_ID { get; set; }
        public long? TREATMENT_END_TYPE_ID { get; set; }
        public string TREATMENT_CODE__EXACT { get; set; }
        public string PATIENT_CODE__EXACT { get; set; }

        public HisTreatmentView5Filter()
            : base()
        {
        }
    }
}

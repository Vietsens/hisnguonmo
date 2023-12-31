using System.Collections.Generic;

namespace MOS.Filter
{
    public class HisExpMestMedicineView6Filter : FilterBase
    {
        public List<long> TDL_MEDICINE_TYPE_IDs { get; set; }
        public List<long> EXP_MEST_TYPE_IDs { get; set; }
        public List<long> MEDI_STOCK_IDs { get; set; }
        public List<long> MEDI_STOCK_PERIOD_IDs { get; set; }
        public List<long> EXP_MEST_STT_IDs { get; set; }
        public List<long> REQ_ROOM_IDs { get; set; }
        public List<long> REQ_DEPARTMENT_IDs { get; set; }
        public List<long> BID_IDs { get; set; }
        public List<long> MEDICINE_TYPE_IDs { get; set; }
        public List<long> SUPPLIER_IDs { get; set; }
        public List<long> SERVICE_IDs { get; set; }
        public List<long> MANUFACTURER_IDs { get; set; }
        public List<long> MEMA_GROUP_IDs { get; set; }
        public List<long> SERVICE_UNIT_IDs { get; set; }
        public List<long> EXP_MEST_IDs { get; set; }
        public List<long> MEDICINE_IDs { get; set; }
        public List<long> TDL_MEDI_STOCK_IDs { get; set; }
        public List<long> TDL_TREATMENT_IDs { get; set; }
        public List<long> VACCINATION_RESULT_IDs { get; set; }
        public List<long> TDL_VACCINATION_IDs { get; set; }
        public List<long> TDL_SERVICE_REQ_IDs { get; set; }
        public List<long> PRESCRIPTION_IDs { get; set; }

        public long? TDL_SERVICE_REQ_ID { get; set; }
        public long? TDL_MEDICINE_TYPE_ID { get; set; }
        public long? EXP_MEST_TYPE_ID { get; set; }
        public long? MEDI_STOCK_ID { get; set; }
        public long? MEDI_STOCK_PERIOD_ID { get; set; }
        public long? EXP_MEST_STT_ID { get; set; }
        public long? REQ_ROOM_ID { get; set; }
        public long? REQ_DEPARTMENT_ID { get; set; }
        public long? BID_ID { get; set; }
        public long? MEDICINE_TYPE_ID { get; set; }
        public long? SUPPLIER_ID { get; set; }
        public long? SERVICE_ID { get; set; }
        public long? MANUFACTURER_ID { get; set; }
        public long? MEMA_GROUP_ID { get; set; }
        public long? SERVICE_UNIT_ID { get; set; }
        public long? EXP_MEST_ID { get; set; }
        public long? MEDICINE_ID { get; set; }
        public long? TDL_MEDI_STOCK_ID { get; set; }
        public long? TDL_TREATMENT_ID { get; set; }
        public long? VACCINATION_RESULT_ID { get; set; }
        public long? TDL_VACCINATION_ID { get; set; }
        public long? PRESCRIPTION_ID { get; set; }

        public long? EXP_TIME_FROM { get; set; }
        public long? EXP_DATE_FROM { get; set; }
        public long? EXPIRED_DATE_FROM { get; set; }
        public long? IMP_TIME_FROM { get; set; }
        public long? APPROVAL_TIME_FROM { get; set; }
        public long? APPROVAL_DATE_FROM { get; set; }
        public long? TDL_INTRUCTION_DATE_FROM { get; set; }

        public long? EXP_TIME_TO { get; set; }
        public long? EXP_DATE_TO { get; set; }
        public long? EXPIRED_DATE_TO { get; set; }
        public long? IMP_TIME_TO { get; set; }
        public long? APPROVAL_TIME_TO { get; set; }
        public long? APPROVAL_DATE_TO { get; set; }
        public long? TDL_INTRUCTION_DATE_TO { get; set; }

        public bool? IS_EXPEND { get; set; }
        public bool? IS_EXPORT { get; set; }
        public bool? IS_FUNCTIONAL_FOOD { get; set; }
        public bool? HAS_MEDI_STOCK_PERIOD { get; set; }
        public bool? IS_NOT_TAKEN { get; set; }

        public long? TDL_INTRUCTION_DATE__EQUAL { get; set; }

        public HisExpMestMedicineView6Filter()
            : base()
        {
        }
    }
}

using System.Collections.Generic;

namespace MOS.Filter
{
    public class HisExpMestMaterialViewFilter : FilterBase
    {
        public List<long> TDL_MATERIAL_TYPE_IDs { get; set; }
        public List<long> EXP_MEST_TYPE_IDs { get; set; }
        public List<long> AGGR_EXP_MEST_IDs { get; set; }
        public List<long> MEDI_STOCK_IDs { get; set; }
        public List<long> MEDI_STOCK_PERIOD_IDs { get; set; }
        public List<long> EXP_MEST_STT_IDs { get; set; }
        public List<long> REQ_ROOM_IDs { get; set; }
        public List<long> REQ_DEPARTMENT_IDs { get; set; }
        public List<long> BID_IDs { get; set; }
        public List<long> MATERIAL_TYPE_IDs { get; set; }
        public List<long> SUPPLIER_IDs { get; set; }
        public List<long> SERVICE_IDs { get; set; }
        public List<long> MANUFACTURER_IDs { get; set; }
        public List<long> MEMA_GROUP_IDs { get; set; }
        public List<long> SERVICE_UNIT_IDs { get; set; }
        public List<long> EXP_MEST_IDs { get; set; }
        public List<long> MATERIAL_IDs { get; set; }
        public List<long> TDL_MEDI_STOCK_IDs { get; set; }
        public List<long> PATIENT_TYPE_IDs { get; set; }
        public List<long> TDL_TREATMENT_IDs { get; set; }
        public List<long> TDL_SERVICE_REQ_IDs { get; set; }
        public List<long> TDL_AGGR_EXP_MEST_ID__OR__EXP_MEST_IDs { get; set; }

        public long? TDL_INTRUCTION_TIME_FROM { get; set; }
        public long? TDL_INTRUCTION_TIME_TO { get; set; }
        public long? PATIENT_TYPE_ID { get; set; }
        public long? TDL_MATERIAL_TYPE_ID { get; set; }
        public long? EXP_MEST_TYPE_ID { get; set; }
        public long? AGGR_EXP_MEST_ID { get; set; }
        public long? MEDI_STOCK_ID { get; set; }
        public long? MEDI_STOCK_PERIOD_ID { get; set; }
        public long? EXP_MEST_STT_ID { get; set; }
        public long? REQ_ROOM_ID { get; set; }
        public long? REQ_DEPARTMENT_ID { get; set; }
        public long? BID_ID { get; set; }
        public long? MATERIAL_TYPE_ID { get; set; }
        public long? SUPPLIER_ID { get; set; }
        public long? SERVICE_ID { get; set; }
        public long? MANUFACTURER_ID { get; set; }
        public long? MEMA_GROUP_ID { get; set; }
        public long? SERVICE_UNIT_ID { get; set; }
        public long? EXP_MEST_ID { get; set; }
        public long? MATERIAL_ID { get; set; }
        public long? TDL_MEDI_STOCK_ID { get; set; }
        public long? TDL_AGGR_EXP_MEST_ID__OR__EXP_MEST_ID { get; set; }
        public long? TDL_TREATMENT_ID { get; set; }

        public long? EXP_TIME_FROM { get; set; }
        public long? EXP_DATE_FROM { get; set; }
        public long? EXPIRED_DATE_FROM { get; set; }
        public long? IMP_TIME_FROM { get; set; }
        public long? APPROVAL_TIME_FROM { get; set; }
        public long? APPROVAL_DATE_FROM { get; set; }

        public long? EXP_TIME_TO { get; set; }
        public long? EXP_DATE_TO { get; set; }
        public long? EXPIRED_DATE_TO { get; set; }
        public long? IMP_TIME_TO { get; set; }
        public long? APPROVAL_TIME_TO { get; set; }
        public long? APPROVAL_DATE_TO { get; set; }

        public bool? IS_EXPEND { get; set; }
        public bool? IS_EXPORT { get; set; }
        public bool? IS_CHEMICAL_SUBSTANCE { get; set; }
        public bool? HAS_MEDI_STOCK_PERIOD { get; set; }

        public string SERIAL_NUMBER__EXACT { get; set; }
        public string PACKAGE_NUMBER__EXACT { get; set; }

        public HisExpMestMaterialViewFilter()
            : base()
        {
        }
    }
}

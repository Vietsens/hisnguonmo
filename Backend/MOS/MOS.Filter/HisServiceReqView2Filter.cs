
using System.Collections.Generic;
namespace MOS.Filter
{
    public class HisServiceReqView2Filter : FilterBase
    {
        public string SERVICE_REQ_CODE { get; set; }
        public string TREATMENT_CODE__EXACT { get; set; }
        public string SESSION_CODE__EXACT { get; set; }
        public bool? IS_NO_EXECUTE { get; set; }
        public long? EXECUTE_ROOM_ID { get; set; }
        public long? REQUEST_ROOM_ID { get; set; }
        public long? INTRUCTION_TIME_FROM { get; set; }
        public long? INTRUCTION_TIME_TO { get; set; }
        public long? TREATMENT_ID { get; set; }
        public long? TDL_PATIENT_ID { get; set; }
        public long? SERVICE_REQ_TYPE_ID { get; set; }
        public long? TDL_KSK_CONTRACT_ID { get; set; }
        public List<long> SERVICE_REQ_STT_IDs { get; set; }
        public List<long> TREATMENT_IDs { get; set; }
        public List<long> SERVICE_REQ_TYPE_IDs { get; set; }
        public List<long> EXECUTE_DEPARTMENT_IDs { get; set; }
        public List<long> REQUEST_DEPARTMENT_IDs { get; set; }
        public List<long> TRACKING_IDs { get; set; }
        public List<long> EXECUTE_ROOM_IDS { get; set; }
        public string TDL_PATIENT_CODE__EXACT { get; set; }
        public string SERVICE_REQ_CODE__EXACT { get; set; }
        


        public string ORDER_FIELD1 { get; set; }
        public string ORDER_DIRECTION1 { get; set; }
        public string ORDER_FIELD2 { get; set; }
        public string ORDER_DIRECTION2 { get; set; }
        public string ORDER_FIELD3 { get; set; }
        public string ORDER_DIRECTION3 { get; set; }
        public string ORDER_FIELD4 { get; set; }
        public string ORDER_DIRECTION4 { get; set; }
        public long? EXECUTE_DEPARTMENT_ID { get; set; }
        public long? FINISH_TIME_FROM { get; set; }
        public long? FINISH_TIME_TO { get; set; }
        public long? START_TIME_FROM { get; set; }
        public long? START_TIME_TO { get; set; }
        public long? REQUEST_DEPARTMENT_ID { get; set; }
        public long? TRACKING_ID { get; set; }
        public long? INTRUCTION_DATE_FROM { get; set; }
        public long? INTRUCTION_DATE_TO { get; set; }
        public string PATIENT_NAME { get; set; }

        public HisServiceReqView2Filter()
            : base()
        {
        }
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MOS.EFMODEL.DataModels
{
    using System;
    using System.Collections.Generic;
    
    public partial class V_HIS_EXME_REASON_CFG
    {
        public long ID { get; set; }
        public Nullable<long> CREATE_TIME { get; set; }
        public Nullable<long> MODIFY_TIME { get; set; }
        public string CREATOR { get; set; }
        public string MODIFIER { get; set; }
        public string APP_CREATOR { get; set; }
        public string APP_MODIFIER { get; set; }
        public Nullable<short> IS_ACTIVE { get; set; }
        public Nullable<short> IS_DELETE { get; set; }
        public string GROUP_CODE { get; set; }
        public long EXP_MEST_REASON_ID { get; set; }
        public long PATIENT_CLASSIFY_ID { get; set; }
        public long TREATMENT_TYPE_ID { get; set; }
        public Nullable<long> PATIENT_TYPE_ID { get; set; }
        public Nullable<long> OTHER_PAY_SOURCE_ID { get; set; }
        public string EXP_MEST_REASON_CODE { get; set; }
        public string EXP_MEST_REASON_NAME { get; set; }
        public string PATIENT_CLASSIFY_CODE { get; set; }
        public string PATIENT_CLASSIFY_NAME { get; set; }
        public string TREATMENT_TYPE_CODE { get; set; }
        public string TREATMENT_TYPE_NAME { get; set; }
        public string PATIENT_TYPE_CODE { get; set; }
        public string PATIENT_TYPE_NAME { get; set; }
        public string OTHER_PAY_SOURCE_CODE { get; set; }
        public string OTHER_PAY_SOURCE_NAME { get; set; }
    }
}

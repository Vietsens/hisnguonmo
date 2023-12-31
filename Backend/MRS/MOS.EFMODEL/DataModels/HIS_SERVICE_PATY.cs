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
    
    public partial class HIS_SERVICE_PATY
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
        public long SERVICE_ID { get; set; }
        public long PATIENT_TYPE_ID { get; set; }
        public decimal PRICE { get; set; }
        public decimal VAT_RATIO { get; set; }
        public Nullable<long> PRIORITY { get; set; }
        public Nullable<long> FROM_TIME { get; set; }
        public Nullable<long> TO_TIME { get; set; }
        public Nullable<long> TREATMENT_FROM_TIME { get; set; }
        public Nullable<long> TREATMENT_TO_TIME { get; set; }
        public long BRANCH_ID { get; set; }
        public Nullable<long> INTRUCTION_NUMBER_FROM { get; set; }
        public Nullable<long> INTRUCTION_NUMBER_TO { get; set; }
        public string REQUEST_ROOM_IDS { get; set; }
        public Nullable<decimal> OVERTIME_PRICE { get; set; }
        public string EXECUTE_ROOM_IDS { get; set; }
        public Nullable<short> DAY_FROM { get; set; }
        public Nullable<short> DAY_TO { get; set; }
        public string HOUR_FROM { get; set; }
        public string HOUR_TO { get; set; }
        public string REQUEST_DEPARMENT_IDS { get; set; }
        public Nullable<long> INSTR_NUM_BY_TYPE_FROM { get; set; }
        public Nullable<long> INSTR_NUM_BY_TYPE_TO { get; set; }
        public Nullable<long> PACKAGE_ID { get; set; }
        public Nullable<long> SERVICE_CONDITION_ID { get; set; }
        public Nullable<long> PATIENT_CLASSIFY_ID { get; set; }
        public Nullable<decimal> ACTUAL_PRICE { get; set; }
        public Nullable<long> RATION_TIME_ID { get; set; }
    
        public virtual HIS_BRANCH HIS_BRANCH { get; set; }
        public virtual HIS_PACKAGE HIS_PACKAGE { get; set; }
        public virtual HIS_PATIENT_CLASSIFY HIS_PATIENT_CLASSIFY { get; set; }
        public virtual HIS_PATIENT_TYPE HIS_PATIENT_TYPE { get; set; }
        public virtual HIS_RATION_TIME HIS_RATION_TIME { get; set; }
        public virtual HIS_SERVICE HIS_SERVICE { get; set; }
        public virtual HIS_SERVICE_CONDITION HIS_SERVICE_CONDITION { get; set; }
    }
}

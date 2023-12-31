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
    
    public partial class HIS_PREPARE_METY
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
        public long PREPARE_ID { get; set; }
        public long MEDICINE_TYPE_ID { get; set; }
        public decimal REQ_AMOUNT { get; set; }
        public Nullable<decimal> APPROVAL_AMOUNT { get; set; }
        public long TDL_TREATMENT_ID { get; set; }
    
        public virtual HIS_MEDICINE_TYPE HIS_MEDICINE_TYPE { get; set; }
        public virtual HIS_PREPARE HIS_PREPARE { get; set; }
    }
}

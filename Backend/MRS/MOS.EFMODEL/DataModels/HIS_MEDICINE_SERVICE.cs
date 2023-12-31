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
    
    public partial class HIS_MEDICINE_SERVICE
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
        public string ICD_CODE { get; set; }
        public string ICD_NAME { get; set; }
        public Nullable<long> SERVICE_ID { get; set; }
        public Nullable<long> MEDICINE_TYPE_ID { get; set; }
        public Nullable<long> TEST_INDEX_ID { get; set; }
        public Nullable<short> DATA_TYPE { get; set; }
        public Nullable<decimal> VALUE_SERVICE_FROM { get; set; }
        public Nullable<decimal> VALUE_SERVICE_TO { get; set; }
        public Nullable<decimal> AMOUNT_INDAY_FROM { get; set; }
        public Nullable<decimal> AMOUNT_INDAY_TO { get; set; }
        public string WARNING_CONTENT { get; set; }
    }
}

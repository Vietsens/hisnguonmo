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
    
    public partial class HIS_MIXED_MEDICINE
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
        public Nullable<long> MEDICINE_ID { get; set; }
        public long INFUSION_ID { get; set; }
        public Nullable<long> MEDICINE_TYPE_ID { get; set; }
        public string PACKAGE_NUMBER { get; set; }
        public string MEDICINE_TYPE_NAME { get; set; }
        public Nullable<decimal> VOLUME { get; set; }
        public Nullable<decimal> AMOUNT { get; set; }
        public string SERVICE_UNIT_NAME { get; set; }
    
        public virtual HIS_INFUSION HIS_INFUSION { get; set; }
        public virtual HIS_MEDICINE HIS_MEDICINE { get; set; }
        public virtual HIS_MEDICINE_TYPE HIS_MEDICINE_TYPE { get; set; }
    }
}

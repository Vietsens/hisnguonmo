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
    
    public partial class HIS_EKIP_TEMP
    {
        public HIS_EKIP_TEMP()
        {
            this.HIS_EKIP_TEMP_USER = new HashSet<HIS_EKIP_TEMP_USER>();
        }
    
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
        public string EKIP_TEMP_NAME { get; set; }
        public Nullable<short> IS_PUBLIC { get; set; }
        public Nullable<short> IS_PUBLIC_IN_DEPARTMENT { get; set; }
        public Nullable<long> DEPARTMENT_ID { get; set; }
    
        public virtual HIS_DEPARTMENT HIS_DEPARTMENT { get; set; }
        public virtual ICollection<HIS_EKIP_TEMP_USER> HIS_EKIP_TEMP_USER { get; set; }
    }
}

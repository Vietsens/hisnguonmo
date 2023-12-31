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
    
    public partial class HIS_TREATMENT_BED_ROOM
    {
        public HIS_TREATMENT_BED_ROOM()
        {
            this.HIS_BED_LOG = new HashSet<HIS_BED_LOG>();
            this.HIS_PATIENT_OBSERVATION = new HashSet<HIS_PATIENT_OBSERVATION>();
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
        public long TREATMENT_ID { get; set; }
        public long BED_ROOM_ID { get; set; }
        public string ADD_LOGINNAME { get; set; }
        public string ADD_USERNAME { get; set; }
        public long ADD_TIME { get; set; }
        public string REMOVE_LOGINNAME { get; set; }
        public string REMOVE_USERNAME { get; set; }
        public Nullable<long> REMOVE_TIME { get; set; }
        public Nullable<long> BED_ID { get; set; }
        public Nullable<long> CO_TREATMENT_ID { get; set; }
        public Nullable<long> TREATMENT_ROOM_ID { get; set; }
        public Nullable<long> TDL_OBSERVED_TIME_FROM { get; set; }
        public Nullable<long> TDL_OBSERVED_TIME_TO { get; set; }
    
        public virtual HIS_BED HIS_BED { get; set; }
        public virtual ICollection<HIS_BED_LOG> HIS_BED_LOG { get; set; }
        public virtual HIS_BED_ROOM HIS_BED_ROOM { get; set; }
        public virtual HIS_CO_TREATMENT HIS_CO_TREATMENT { get; set; }
        public virtual ICollection<HIS_PATIENT_OBSERVATION> HIS_PATIENT_OBSERVATION { get; set; }
        public virtual HIS_TREATMENT HIS_TREATMENT { get; set; }
    }
}

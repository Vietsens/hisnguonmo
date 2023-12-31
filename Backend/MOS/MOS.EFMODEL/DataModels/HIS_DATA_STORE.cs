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
    
    public partial class HIS_DATA_STORE
    {
        public HIS_DATA_STORE()
        {
            this.HIS_DATA_STORE1 = new HashSet<HIS_DATA_STORE>();
            this.HIS_MEDI_RECORD = new HashSet<HIS_MEDI_RECORD>();
            this.HIS_PROGRAM = new HashSet<HIS_PROGRAM>();
            this.HIS_TREATMENT = new HashSet<HIS_TREATMENT>();
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
        public string DATA_STORE_CODE { get; set; }
        public string DATA_STORE_NAME { get; set; }
        public long ROOM_ID { get; set; }
        public Nullable<long> PARENT_ID { get; set; }
        public Nullable<long> STORED_DEPARTMENT_ID { get; set; }
        public Nullable<long> STORED_ROOM_ID { get; set; }
        public string TREATMENT_END_TYPE_IDS { get; set; }
        public string TREATMENT_TYPE_IDS { get; set; }
    
        public virtual HIS_ROOM HIS_ROOM { get; set; }
        public virtual ICollection<HIS_DATA_STORE> HIS_DATA_STORE1 { get; set; }
        public virtual HIS_DATA_STORE HIS_DATA_STORE2 { get; set; }
        public virtual HIS_ROOM HIS_ROOM1 { get; set; }
        public virtual HIS_DEPARTMENT HIS_DEPARTMENT { get; set; }
        public virtual ICollection<HIS_MEDI_RECORD> HIS_MEDI_RECORD { get; set; }
        public virtual ICollection<HIS_PROGRAM> HIS_PROGRAM { get; set; }
        public virtual ICollection<HIS_TREATMENT> HIS_TREATMENT { get; set; }
    }
}

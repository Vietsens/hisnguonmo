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
    
    public partial class HIS_MACHINE
    {
        public HIS_MACHINE()
        {
            this.HIS_EXP_MEST = new HashSet<HIS_EXP_MEST>();
            this.HIS_MACHINE_SERV_MATY = new HashSet<HIS_MACHINE_SERV_MATY>();
            this.HIS_QC_NORMATION = new HashSet<HIS_QC_NORMATION>();
            this.HIS_SERE_SERV_EXT = new HashSet<HIS_SERE_SERV_EXT>();
            this.HIS_SERE_SERV_TEIN = new HashSet<HIS_SERE_SERV_TEIN>();
            this.HIS_SERVICE_MACHINE = new HashSet<HIS_SERVICE_MACHINE>();
            this.HIS_SERVICE_REQ = new HashSet<HIS_SERVICE_REQ>();
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
        public string MACHINE_CODE { get; set; }
        public string MACHINE_NAME { get; set; }
        public string SERIAL_NUMBER { get; set; }
        public string SOURCE_CODE { get; set; }
        public string MACHINE_GROUP_CODE { get; set; }
        public string INTEGRATE_ADDRESS { get; set; }
        public Nullable<long> ROOM_ID { get; set; }
        public Nullable<short> IS_KIDNEY { get; set; }
        public string ROOM_IDS { get; set; }
        public Nullable<long> MAX_SERVICE_PER_DAY { get; set; }
        public string MANUFACTURER_NAME { get; set; }
        public string NATIONAL_NAME { get; set; }
        public Nullable<short> MANUFACTURED_YEAR { get; set; }
        public Nullable<short> USED_YEAR { get; set; }
        public string CIRCULATION_NUMBER { get; set; }
        public string SYMBOL { get; set; }
        public Nullable<long> DEPARTMENT_ID { get; set; }
    
        public virtual ICollection<HIS_EXP_MEST> HIS_EXP_MEST { get; set; }
        public virtual HIS_ROOM HIS_ROOM { get; set; }
        public virtual ICollection<HIS_MACHINE_SERV_MATY> HIS_MACHINE_SERV_MATY { get; set; }
        public virtual ICollection<HIS_QC_NORMATION> HIS_QC_NORMATION { get; set; }
        public virtual ICollection<HIS_SERE_SERV_EXT> HIS_SERE_SERV_EXT { get; set; }
        public virtual ICollection<HIS_SERE_SERV_TEIN> HIS_SERE_SERV_TEIN { get; set; }
        public virtual ICollection<HIS_SERVICE_MACHINE> HIS_SERVICE_MACHINE { get; set; }
        public virtual ICollection<HIS_SERVICE_REQ> HIS_SERVICE_REQ { get; set; }
    }
}

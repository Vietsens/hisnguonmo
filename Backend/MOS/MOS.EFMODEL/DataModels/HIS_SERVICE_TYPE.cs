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
    
    public partial class HIS_SERVICE_TYPE
    {
        public HIS_SERVICE_TYPE()
        {
            this.HIS_SERE_SERV_TEMP = new HashSet<HIS_SERE_SERV_TEMP>();
            this.HIS_SERVICE = new HashSet<HIS_SERVICE>();
            this.HIS_SURG_REMUNERATION = new HashSet<HIS_SURG_REMUNERATION>();
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
        public string SERVICE_TYPE_CODE { get; set; }
        public string SERVICE_TYPE_NAME { get; set; }
        public Nullable<long> NUM_ORDER { get; set; }
        public Nullable<long> EXE_SERVICE_MODULE_ID { get; set; }
        public Nullable<short> IS_AUTO_SPLIT_REQ { get; set; }
        public Nullable<short> IS_NOT_DISPLAY_ASSIGN { get; set; }
        public Nullable<short> IS_SPLIT_REQ_BY_SAMPLE_TYPE { get; set; }
        public Nullable<short> IS_REQUIRED_SAMPLE_TYPE { get; set; }
    
        public virtual HIS_EXE_SERVICE_MODULE HIS_EXE_SERVICE_MODULE { get; set; }
        public virtual ICollection<HIS_SERE_SERV_TEMP> HIS_SERE_SERV_TEMP { get; set; }
        public virtual ICollection<HIS_SERVICE> HIS_SERVICE { get; set; }
        public virtual ICollection<HIS_SURG_REMUNERATION> HIS_SURG_REMUNERATION { get; set; }
    }
}

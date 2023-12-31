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
    
    public partial class HIS_WORKING_SHIFT
    {
        public HIS_WORKING_SHIFT()
        {
            this.HIS_ACCOUNT_BOOK = new HashSet<HIS_ACCOUNT_BOOK>();
            this.HIS_SERVICE_REQ = new HashSet<HIS_SERVICE_REQ>();
            this.HIS_SERVICE_REQ1 = new HashSet<HIS_SERVICE_REQ>();
            this.HIS_SERVICE_REQ2 = new HashSet<HIS_SERVICE_REQ>();
            this.HIS_TRANSACTION = new HashSet<HIS_TRANSACTION>();
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
        public string WORKING_SHIFT_CODE { get; set; }
        public string WORKING_SHIFT_NAME { get; set; }
        public string FROM_TIME { get; set; }
        public string TO_TIME { get; set; }
    
        public virtual ICollection<HIS_ACCOUNT_BOOK> HIS_ACCOUNT_BOOK { get; set; }
        public virtual ICollection<HIS_SERVICE_REQ> HIS_SERVICE_REQ { get; set; }
        public virtual ICollection<HIS_SERVICE_REQ> HIS_SERVICE_REQ1 { get; set; }
        public virtual ICollection<HIS_SERVICE_REQ> HIS_SERVICE_REQ2 { get; set; }
        public virtual ICollection<HIS_TRANSACTION> HIS_TRANSACTION { get; set; }
    }
}

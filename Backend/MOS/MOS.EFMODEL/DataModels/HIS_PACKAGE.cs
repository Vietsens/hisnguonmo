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
    
    public partial class HIS_PACKAGE
    {
        public HIS_PACKAGE()
        {
            this.HIS_PACKAGE_DETAIL = new HashSet<HIS_PACKAGE_DETAIL>();
            this.HIS_SERE_SERV = new HashSet<HIS_SERE_SERV>();
            this.HIS_SERVICE = new HashSet<HIS_SERVICE>();
            this.HIS_SERVICE_PATY = new HashSet<HIS_SERVICE_PATY>();
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
        public string PACKAGE_CODE { get; set; }
        public string PACKAGE_NAME { get; set; }
        public Nullable<short> IS_NOT_FIXED_SERVICE { get; set; }
    
        public virtual ICollection<HIS_PACKAGE_DETAIL> HIS_PACKAGE_DETAIL { get; set; }
        public virtual ICollection<HIS_SERE_SERV> HIS_SERE_SERV { get; set; }
        public virtual ICollection<HIS_SERVICE> HIS_SERVICE { get; set; }
        public virtual ICollection<HIS_SERVICE_PATY> HIS_SERVICE_PATY { get; set; }
    }
}

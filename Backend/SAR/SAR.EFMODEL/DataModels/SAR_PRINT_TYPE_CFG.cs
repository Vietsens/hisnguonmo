//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SAR.EFMODEL.DataModels
{
    using System;
    using System.Collections.Generic;
    
    public partial class SAR_PRINT_TYPE_CFG
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
        public string APP_CODE { get; set; }
        public string MODULE_LINK { get; set; }
        public string CONTROL_CODE { get; set; }
        public string CONTROL_PATH { get; set; }
        public string BRANCH_CODE { get; set; }
        public long PRINT_TYPE_ID { get; set; }
        public string CAPTION { get; set; }
    
        public virtual SAR_PRINT_TYPE SAR_PRINT_TYPE { get; set; }
    }
}

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
    
    public partial class HIS_TRANSACTION_EXP
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
        public string TDL_EXP_MEST_CODE { get; set; }
        public long TDL_MEDI_STOCK_ID { get; set; }
        public long TRANSACTION_ID { get; set; }
        public Nullable<long> EXP_MEST_ID { get; set; }
    
        public virtual HIS_EXP_MEST HIS_EXP_MEST { get; set; }
        public virtual HIS_TRANSACTION HIS_TRANSACTION { get; set; }
    }
}

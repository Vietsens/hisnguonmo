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
    
    public partial class V_HIS_MEDI_STOCK_METY_1
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
        public long MEDI_STOCK_ID { get; set; }
        public long MEDICINE_TYPE_ID { get; set; }
        public Nullable<decimal> ALERT_MIN_IN_STOCK { get; set; }
        public Nullable<decimal> ALERT_MAX_IN_STOCK { get; set; }
        public Nullable<short> IS_PREVENT_MAX { get; set; }
        public Nullable<short> IS_GOODS_RESTRICT { get; set; }
        public Nullable<short> IS_PREVENT_EXP { get; set; }
        public Nullable<long> EXP_MEDI_STOCK_ID { get; set; }
        public string MEDI_STOCK_CODE { get; set; }
        public string MEDI_STOCK_NAME { get; set; }
        public string MEDICINE_TYPE_CODE { get; set; }
        public string MEDICINE_TYPE_NAME { get; set; }
        public string EXP_MEDI_STOCK_CODE { get; set; }
        public string EXP_MEDI_STOCK_NAME { get; set; }
        public Nullable<decimal> AMOUT_EXP_MEDI_STOCK { get; set; }
    }
}

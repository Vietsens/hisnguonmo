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
    
    public partial class V_HIS_EXP_MEST_METY_REQ_1
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
        public long EXP_MEST_ID { get; set; }
        public long MEDICINE_TYPE_ID { get; set; }
        public decimal AMOUNT { get; set; }
        public Nullable<long> NUM_ORDER { get; set; }
        public string DESCRIPTION { get; set; }
        public long TDL_MEDI_STOCK_ID { get; set; }
        public Nullable<decimal> DD_AMOUNT { get; set; }
        public Nullable<long> TREATMENT_ID { get; set; }
        public Nullable<decimal> BCS_REQ_AMOUNT { get; set; }
        public Nullable<short> IS_ALLOW_ODD { get; set; }
    }
}

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
    
    public partial class V_HIS_MEST_PERIOD_BLTY
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
        public long MEDI_STOCK_PERIOD_ID { get; set; }
        public long BLOOD_TYPE_ID { get; set; }
        public decimal BEGIN_AMOUNT { get; set; }
        public decimal IN_AMOUNT { get; set; }
        public decimal OUT_AMOUNT { get; set; }
        public Nullable<decimal> VIR_END_AMOUNT { get; set; }
        public Nullable<decimal> INVENTORY_AMOUNT { get; set; }
        public string MEDI_STOCK_PERIOD_NAME { get; set; }
        public Nullable<long> MEDI_STOCK_PERIOD_TIME { get; set; }
        public Nullable<long> TO_TIME { get; set; }
        public string BLOOD_TYPE_CODE { get; set; }
        public string BLOOD_TYPE_NAME { get; set; }
        public string SERVICE_UNIT_CODE { get; set; }
        public string SERVICE_UNIT_NAME { get; set; }
        public Nullable<decimal> VOLUME { get; set; }
        public string PACKING_TYPE_NAME { get; set; }
    }
}

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
    
    public partial class D_HIS_MEDICINE_TYPE_1
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
        public string MEDICINE_TYPE_CODE { get; set; }
        public string MEDICINE_TYPE_NAME { get; set; }
        public long SERVICE_ID { get; set; }
        public Nullable<long> PARENT_ID { get; set; }
        public Nullable<short> IS_LEAF { get; set; }
        public Nullable<long> NUM_ORDER { get; set; }
        public string CONCENTRA { get; set; }
        public string ACTIVE_INGR_BHYT_CODE { get; set; }
        public string ACTIVE_INGR_BHYT_NAME { get; set; }
        public string REGISTER_NUMBER { get; set; }
        public Nullable<long> PACKING_TYPE_ID__DELETE { get; set; }
        public Nullable<long> MANUFACTURER_ID { get; set; }
        public Nullable<long> MEDICINE_USE_FORM_ID { get; set; }
        public Nullable<long> MEDICINE_LINE_ID { get; set; }
        public Nullable<long> MEDICINE_GROUP_ID { get; set; }
        public long TDL_SERVICE_UNIT_ID { get; set; }
        public string NATIONAL_NAME { get; set; }
        public string TUTORIAL { get; set; }
        public Nullable<decimal> IMP_PRICE { get; set; }
        public Nullable<decimal> IMP_VAT_RATIO { get; set; }
        public Nullable<decimal> INTERNAL_PRICE { get; set; }
        public Nullable<decimal> ALERT_MAX_IN_TREATMENT { get; set; }
        public Nullable<long> ALERT_EXPIRED_DATE { get; set; }
        public Nullable<decimal> ALERT_MIN_IN_STOCK { get; set; }
        public Nullable<short> IS_STOP_IMP { get; set; }
        public Nullable<short> IS_STAR_MARK { get; set; }
        public Nullable<short> IS_ALLOW_ODD { get; set; }
        public Nullable<short> IS_ALLOW_EXPORT_ODD { get; set; }
        public Nullable<short> IS_FUNCTIONAL_FOOD { get; set; }
        public Nullable<short> IS_REQUIRE_HSD { get; set; }
        public Nullable<short> IS_SALE_EQUAL_IMP_PRICE { get; set; }
        public Nullable<short> IS_BUSINESS { get; set; }
        public Nullable<decimal> USE_ON_DAY { get; set; }
        public string DESCRIPTION { get; set; }
        public Nullable<long> MEMA_GROUP_ID { get; set; }
        public string BYT_NUM_ORDER { get; set; }
        public string TCY_NUM_ORDER { get; set; }
        public string MEDICINE_TYPE_PROPRIETARY_NAME { get; set; }
        public string PACKING_TYPE_NAME { get; set; }
        public Nullable<short> IS_RAW_MEDICINE { get; set; }
        public Nullable<short> IS_AUTO_EXPEND { get; set; }
        public string MANUFACTURER_CODE { get; set; }
        public string MANUFACTURER_NAME { get; set; }
        public string MEDI_STOCK_CODE { get; set; }
        public string MEDI_STOCK_NAME { get; set; }
        public Nullable<decimal> AMOUNT { get; set; }
        public Nullable<long> MEDI_STOCK_ID { get; set; }
        public string MEDICINE_REGISTER_NUMBER { get; set; }
        public long SERVICE_UNIT_ID { get; set; }
        public string SERVICE_UNIT_CODE { get; set; }
        public string SERVICE_UNIT_NAME { get; set; }
    }
}

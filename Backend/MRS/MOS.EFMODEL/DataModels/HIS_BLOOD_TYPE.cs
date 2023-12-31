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
    
    public partial class HIS_BLOOD_TYPE
    {
        public HIS_BLOOD_TYPE()
        {
            this.HIS_ANTICIPATE_BLTY = new HashSet<HIS_ANTICIPATE_BLTY>();
            this.HIS_BID_BLOOD_TYPE = new HashSet<HIS_BID_BLOOD_TYPE>();
            this.HIS_BLOOD = new HashSet<HIS_BLOOD>();
            this.HIS_BLOOD_TYPE1 = new HashSet<HIS_BLOOD_TYPE>();
            this.HIS_BLTY_SERVICE = new HashSet<HIS_BLTY_SERVICE>();
            this.HIS_BLTY_VOLUME = new HashSet<HIS_BLTY_VOLUME>();
            this.HIS_EXP_BLTY_SERVICE = new HashSet<HIS_EXP_BLTY_SERVICE>();
            this.HIS_EXP_MEST_BLTY_REQ = new HashSet<HIS_EXP_MEST_BLTY_REQ>();
            this.HIS_MEDI_STOCK_BLTY = new HashSet<HIS_MEDI_STOCK_BLTY>();
            this.HIS_MEST_PERIOD_BLTY = new HashSet<HIS_MEST_PERIOD_BLTY>();
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
        public string BLOOD_TYPE_CODE { get; set; }
        public string BLOOD_TYPE_NAME { get; set; }
        public long SERVICE_ID { get; set; }
        public Nullable<long> PARENT_ID { get; set; }
        public Nullable<short> IS_LEAF { get; set; }
        public Nullable<long> NUM_ORDER { get; set; }
        public long TDL_SERVICE_UNIT_ID { get; set; }
        public long BLOOD_VOLUME_ID { get; set; }
        public Nullable<long> BLOOD_GROUP_ID { get; set; }
        public Nullable<long> PACKING_TYPE_ID { get; set; }
        public Nullable<decimal> IMP_PRICE { get; set; }
        public Nullable<decimal> IMP_VAT_RATIO { get; set; }
        public Nullable<decimal> INTERNAL_PRICE { get; set; }
        public Nullable<long> ALERT_EXPIRED_DATE { get; set; }
        public string ELEMENT { get; set; }
        public Nullable<short> IS_RED_BLOOD_CELLS { get; set; }
        public Nullable<long> WARNING_DAY { get; set; }
    
        public virtual ICollection<HIS_ANTICIPATE_BLTY> HIS_ANTICIPATE_BLTY { get; set; }
        public virtual ICollection<HIS_BID_BLOOD_TYPE> HIS_BID_BLOOD_TYPE { get; set; }
        public virtual ICollection<HIS_BLOOD> HIS_BLOOD { get; set; }
        public virtual HIS_BLOOD_GROUP HIS_BLOOD_GROUP { get; set; }
        public virtual HIS_SERVICE HIS_SERVICE { get; set; }
        public virtual HIS_BLOOD_VOLUME HIS_BLOOD_VOLUME { get; set; }
        public virtual ICollection<HIS_BLOOD_TYPE> HIS_BLOOD_TYPE1 { get; set; }
        public virtual HIS_BLOOD_TYPE HIS_BLOOD_TYPE2 { get; set; }
        public virtual HIS_PACKING_TYPE HIS_PACKING_TYPE { get; set; }
        public virtual ICollection<HIS_BLTY_SERVICE> HIS_BLTY_SERVICE { get; set; }
        public virtual ICollection<HIS_BLTY_VOLUME> HIS_BLTY_VOLUME { get; set; }
        public virtual ICollection<HIS_EXP_BLTY_SERVICE> HIS_EXP_BLTY_SERVICE { get; set; }
        public virtual ICollection<HIS_EXP_MEST_BLTY_REQ> HIS_EXP_MEST_BLTY_REQ { get; set; }
        public virtual ICollection<HIS_MEDI_STOCK_BLTY> HIS_MEDI_STOCK_BLTY { get; set; }
        public virtual ICollection<HIS_MEST_PERIOD_BLTY> HIS_MEST_PERIOD_BLTY { get; set; }
    }
}

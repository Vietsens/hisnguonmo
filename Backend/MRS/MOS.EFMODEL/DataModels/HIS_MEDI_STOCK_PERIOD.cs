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
    
    public partial class HIS_MEDI_STOCK_PERIOD
    {
        public HIS_MEDI_STOCK_PERIOD()
        {
            this.HIS_EXP_MEST = new HashSet<HIS_EXP_MEST>();
            this.HIS_EXP_MEST_BLOOD = new HashSet<HIS_EXP_MEST_BLOOD>();
            this.HIS_EXP_MEST_MATERIAL = new HashSet<HIS_EXP_MEST_MATERIAL>();
            this.HIS_EXP_MEST_MEDICINE = new HashSet<HIS_EXP_MEST_MEDICINE>();
            this.HIS_IMP_MEST = new HashSet<HIS_IMP_MEST>();
            this.HIS_IMP_MEST1 = new HashSet<HIS_IMP_MEST>();
            this.HIS_MEDI_STOCK_PERIOD1 = new HashSet<HIS_MEDI_STOCK_PERIOD>();
            this.HIS_MEST_INVENTORY = new HashSet<HIS_MEST_INVENTORY>();
            this.HIS_MEST_PERIOD_BLOOD = new HashSet<HIS_MEST_PERIOD_BLOOD>();
            this.HIS_MEST_PERIOD_BLTY = new HashSet<HIS_MEST_PERIOD_BLTY>();
            this.HIS_MEST_PERIOD_MATE = new HashSet<HIS_MEST_PERIOD_MATE>();
            this.HIS_MEST_PERIOD_MATY = new HashSet<HIS_MEST_PERIOD_MATY>();
            this.HIS_MEST_PERIOD_MEDI = new HashSet<HIS_MEST_PERIOD_MEDI>();
            this.HIS_MEST_PERIOD_METY = new HashSet<HIS_MEST_PERIOD_METY>();
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
        public string MEDI_STOCK_PERIOD_NAME { get; set; }
        public long MEDI_STOCK_ID { get; set; }
        public Nullable<long> PREVIOUS_ID { get; set; }
        public Nullable<long> COUNT_MEDICINE_TYPE { get; set; }
        public Nullable<long> COUNT_MATERIAL_TYPE { get; set; }
        public Nullable<long> COUNT_IMP_MEST { get; set; }
        public Nullable<long> COUNT_EXP_MEST { get; set; }
        public Nullable<long> TO_TIME { get; set; }
        public Nullable<short> IS_APPROVE { get; set; }
        public Nullable<short> IS_AUTO_PERIOD { get; set; }
        public string VIR_UNIQUE_AUTO_PERIOD { get; set; }
    
        public virtual ICollection<HIS_EXP_MEST> HIS_EXP_MEST { get; set; }
        public virtual ICollection<HIS_EXP_MEST_BLOOD> HIS_EXP_MEST_BLOOD { get; set; }
        public virtual ICollection<HIS_EXP_MEST_MATERIAL> HIS_EXP_MEST_MATERIAL { get; set; }
        public virtual ICollection<HIS_EXP_MEST_MEDICINE> HIS_EXP_MEST_MEDICINE { get; set; }
        public virtual ICollection<HIS_IMP_MEST> HIS_IMP_MEST { get; set; }
        public virtual ICollection<HIS_IMP_MEST> HIS_IMP_MEST1 { get; set; }
        public virtual HIS_MEDI_STOCK HIS_MEDI_STOCK { get; set; }
        public virtual ICollection<HIS_MEDI_STOCK_PERIOD> HIS_MEDI_STOCK_PERIOD1 { get; set; }
        public virtual HIS_MEDI_STOCK_PERIOD HIS_MEDI_STOCK_PERIOD2 { get; set; }
        public virtual ICollection<HIS_MEST_INVENTORY> HIS_MEST_INVENTORY { get; set; }
        public virtual ICollection<HIS_MEST_PERIOD_BLOOD> HIS_MEST_PERIOD_BLOOD { get; set; }
        public virtual ICollection<HIS_MEST_PERIOD_BLTY> HIS_MEST_PERIOD_BLTY { get; set; }
        public virtual ICollection<HIS_MEST_PERIOD_MATE> HIS_MEST_PERIOD_MATE { get; set; }
        public virtual ICollection<HIS_MEST_PERIOD_MATY> HIS_MEST_PERIOD_MATY { get; set; }
        public virtual ICollection<HIS_MEST_PERIOD_MEDI> HIS_MEST_PERIOD_MEDI { get; set; }
        public virtual ICollection<HIS_MEST_PERIOD_METY> HIS_MEST_PERIOD_METY { get; set; }
    }
}

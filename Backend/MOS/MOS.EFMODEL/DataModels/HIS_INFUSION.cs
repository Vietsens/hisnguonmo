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
    
    public partial class HIS_INFUSION
    {
        public HIS_INFUSION()
        {
            this.HIS_MIXED_MEDICINE = new HashSet<HIS_MIXED_MEDICINE>();
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
        public long INFUSION_SUM_ID { get; set; }
        public Nullable<long> MEDICINE_ID { get; set; }
        public Nullable<long> SERVICE_UNIT_ID { get; set; }
        public Nullable<long> START_TIME { get; set; }
        public Nullable<long> FINISH_TIME { get; set; }
        public decimal AMOUNT { get; set; }
        public Nullable<decimal> SPEED { get; set; }
        public string REQUEST_LOGINNAME { get; set; }
        public string REQUEST_USERNAME { get; set; }
        public string EXECUTE_LOGINNAME { get; set; }
        public string EXECUTE_USERNAME { get; set; }
        public string NOTE { get; set; }
        public string MEDICINE_TYPE_NAME { get; set; }
        public string PACKAGE_NUMBER { get; set; }
        public Nullable<long> EXPIRED_DATE { get; set; }
        public string SERVICE_UNIT_NAME { get; set; }
        public Nullable<long> SPEED_UNIT_ID { get; set; }
        public Nullable<decimal> VOLUME { get; set; }
        public Nullable<decimal> CONVERT_TIME_RATIO { get; set; }
        public Nullable<decimal> CONVERT_VOLUME_RATIO { get; set; }
        public string MIXED_MEDICINE { get; set; }
        public Nullable<long> EMR_DOCUMENT_STT_ID { get; set; }
        public string EMR_DOCUMENT_URL { get; set; }
        public string EMR_DOCUMENT_CODE { get; set; }
    
        public virtual HIS_SERVICE_UNIT HIS_SERVICE_UNIT { get; set; }
        public virtual HIS_MEDICINE HIS_MEDICINE { get; set; }
        public virtual HIS_INFUSION_SUM HIS_INFUSION_SUM { get; set; }
        public virtual ICollection<HIS_MIXED_MEDICINE> HIS_MIXED_MEDICINE { get; set; }
    }
}

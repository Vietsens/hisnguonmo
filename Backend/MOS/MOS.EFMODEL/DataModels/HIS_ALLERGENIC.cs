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
    
    public partial class HIS_ALLERGENIC
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
        public string ALLERGENIC_NAME { get; set; }
        public long ALLERGY_CARD_ID { get; set; }
        public Nullable<short> IS_DOUBT { get; set; }
        public Nullable<short> IS_SURE { get; set; }
        public string CLINICAL_EXPRESSION { get; set; }
        public Nullable<long> MEDICINE_TYPE_ID { get; set; }
        public long TDL_PATIENT_ID { get; set; }
    
        public virtual HIS_ALLERGY_CARD HIS_ALLERGY_CARD { get; set; }
        public virtual HIS_MEDICINE_TYPE HIS_MEDICINE_TYPE { get; set; }
    }
}

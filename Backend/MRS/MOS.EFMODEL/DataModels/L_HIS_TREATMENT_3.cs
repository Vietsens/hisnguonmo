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
    
    public partial class L_HIS_TREATMENT_3
    {
        public long ID { get; set; }
        public string TREATMENT_CODE { get; set; }
        public string TDL_PATIENT_CODE { get; set; }
        public string TDL_PATIENT_NAME { get; set; }
        public long TDL_PATIENT_DOB { get; set; }
        public long TDL_PATIENT_GENDER_ID { get; set; }
        public long IN_DATE { get; set; }
        public Nullable<long> OUT_DATE { get; set; }
        public long IN_TIME { get; set; }
        public Nullable<long> OUT_TIME { get; set; }
        public Nullable<long> END_DEPARTMENT_ID { get; set; }
        public string TDL_PATIENT_ADDRESS { get; set; }
        public Nullable<short> IS_ACTIVE { get; set; }
        public Nullable<short> IS_LOCK_HEIN { get; set; }
        public Nullable<short> IS_PAUSE { get; set; }
        public Nullable<long> TDL_PATIENT_TYPE_ID { get; set; }
        public Nullable<long> MEDI_RECORD_ID { get; set; }
        public Nullable<long> TDL_TREATMENT_TYPE_ID { get; set; }
        public Nullable<long> FEE_LOCK_TIME { get; set; }
        public string TDL_HEIN_CARD_NUMBER { get; set; }
        public string ICD_CODE { get; set; }
        public string ICD_NAME { get; set; }
        public Nullable<long> END_ROOM_ID { get; set; }
        public Nullable<long> TREATMENT_END_TYPE_ID { get; set; }
        public Nullable<long> MEDI_RECORD_TYPE_ID { get; set; }
        public string REJECT_STORE_REASON { get; set; }
        public Nullable<long> APPROVAL_STORE_STT_ID { get; set; }
        public Nullable<short> TDL_PATIENT_IS_HAS_NOT_DAY_DOB { get; set; }
        public Nullable<long> LAST_DEPARTMENT_ID { get; set; }
        public string RECORD_INSPECTION_REJECT_NOTE { get; set; }
        public Nullable<long> CLINICAL_IN_TIME { get; set; }
        public Nullable<long> TDL_KSK_CONTRACT_ID { get; set; }
        public Nullable<short> TDL_KSK_CONTRACT_IS_RESTRICTED { get; set; }
        public string APPROVAL_LOGINNAME { get; set; }
        public string APPROVAL_USERNAME { get; set; }
        public Nullable<long> APPROVAL_TIME { get; set; }
        public string UNAPPROVAL_LOGINNAME { get; set; }
        public string UNAPPROVAL_USERNAME { get; set; }
        public Nullable<long> UNAPPROVAL_TIME { get; set; }
        public string GENDER_NAME { get; set; }
        public string DEPARTMENT_NAME { get; set; }
        public string TREATMENT_END_TYPE_NAME { get; set; }
        public string ROOM_NAME { get; set; }
        public string LAST_DEPARTMENT_NAME { get; set; }
    }
}

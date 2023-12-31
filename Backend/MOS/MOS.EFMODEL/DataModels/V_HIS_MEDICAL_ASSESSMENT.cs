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
    
    public partial class V_HIS_MEDICAL_ASSESSMENT
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
        public long TREATMENT_ID { get; set; }
        public long ASSESSMENT_OBJECT_ID { get; set; }
        public Nullable<long> ASSESSMENT_TYPE_ID { get; set; }
        public string WELFARE_TYPE_IDS { get; set; }
        public long ASSESSMENT_TIME_FROM { get; set; }
        public string REPORT_NUMBER { get; set; }
        public Nullable<decimal> PREVIOUS_INJURY_RATE { get; set; }
        public string REFERRAL_CODE { get; set; }
        public Nullable<long> REQUEST_TIME { get; set; }
        public string REQUEST_ORG_CODE { get; set; }
        public string REQUEST_ORG_NAME { get; set; }
        public string EXAMINATION_RESULT { get; set; }
        public string LEGAL_GROUND_NUMBERS { get; set; }
        public Nullable<decimal> INJURY_RATE { get; set; }
        public Nullable<decimal> INJURY_RATE_TOTAL { get; set; }
        public Nullable<long> DISABILITY_TYPE_ID { get; set; }
        public Nullable<long> DISABILITY_STATUS_ID { get; set; }
        public string REQUEST_AFTER_ASSESSMENT { get; set; }
        public string CONCLUSION { get; set; }
        public string ASSESSMENT_BOARD_NAME { get; set; }
        public Nullable<long> ASSESSMENT_TIME_TO { get; set; }
        public string ASSESSMENT_PURPOSE { get; set; }
        public string LEGAL_GROUND_DOCUMENTS { get; set; }
        public string ASSESSMENT_REQUEST_CONTENT { get; set; }
        public string PATHOLOGICAL_HISTORY { get; set; }
        public string DISCUSSION { get; set; }
        public string TREATMENT_CODE { get; set; }
        public string TDL_PATIENT_CODE { get; set; }
        public string TDL_PATIENT_NAME { get; set; }
        public long TDL_PATIENT_DOB { get; set; }
        public string TDL_PATIENT_GENDER_NAME { get; set; }
        public long IN_TIME { get; set; }
        public Nullable<long> OUT_TIME { get; set; }
        public string TDL_HEIN_CARD_NUMBER { get; set; }
        public string TDL_PATIENT_NATIONAL_CODE { get; set; }
        public Nullable<long> TDL_PATIENT_CCCD_DATE { get; set; }
        public string TDL_PATIENT_CCCD_NUMBER { get; set; }
        public string TDL_PATIENT_CCCD_PLACE { get; set; }
        public Nullable<long> TDL_PATIENT_CMND_DATE { get; set; }
        public string TDL_PATIENT_CMND_NUMBER { get; set; }
        public string TDL_PATIENT_CMND_PLACE { get; set; }
        public Nullable<long> TDL_PATIENT_PASSPORT_DATE { get; set; }
        public string TDL_PATIENT_PASSPORT_NUMBER { get; set; }
        public string TDL_PATIENT_PASSPORT_PLACE { get; set; }
        public string TDL_PATIENT_ADDRESS { get; set; }
        public string TDL_PATIENT_PROVINCE_CODE { get; set; }
        public string TDL_PATIENT_DISTRICT_CODE { get; set; }
        public string TDL_PATIENT_COMMUNE_CODE { get; set; }
        public Nullable<long> NUMBER_OF_PREMATURE_BIRTH { get; set; }
        public long BRANCH_ID { get; set; }
        public string TDL_SOCIAL_INSURANCE_NUMBER { get; set; }
        public string TDL_PATIENT_COMMUNE_NAME { get; set; }
        public string TDL_PATIENT_DISTRICT_NAME { get; set; }
        public string TDL_PATIENT_NATIONAL_NAME { get; set; }
        public string TDL_PATIENT_PROVINCE_NAME { get; set; }
        public string TDL_PATIENT_CAREER_NAME { get; set; }
        public string TDL_PATIENT_MOBILE { get; set; }
        public string ASSESSMENT_OBJECT_CODE { get; set; }
        public string ASSESSMENT_OBJECT_NAME { get; set; }
        public string WELFARE_TYPE_NAMES { get; set; }
        public string PRESIDENT_USERNAME { get; set; }
        public string ON_BEHALF_USERNAME { get; set; }
    }
}

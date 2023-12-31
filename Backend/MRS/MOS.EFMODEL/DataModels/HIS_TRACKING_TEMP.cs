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
    
    public partial class HIS_TRACKING_TEMP
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
        public string MEDICAL_INSTRUCTION { get; set; }
        public string CONTENT { get; set; }
        public Nullable<decimal> TEMPERATURE { get; set; }
        public Nullable<decimal> BREATH_RATE { get; set; }
        public Nullable<decimal> WEIGHT { get; set; }
        public Nullable<decimal> HEIGHT { get; set; }
        public Nullable<decimal> CHEST { get; set; }
        public Nullable<decimal> BELLY { get; set; }
        public Nullable<long> BLOOD_PRESSURE_MAX { get; set; }
        public Nullable<long> BLOOD_PRESSURE_MIN { get; set; }
        public Nullable<long> PULSE { get; set; }
        public Nullable<decimal> VIR_BMI { get; set; }
        public Nullable<decimal> VIR_BODY_SURFACE_AREA { get; set; }
        public string TRACKING_TEMP_CODE { get; set; }
        public string TRACKING_TEMP_NAME { get; set; }
        public Nullable<short> IS_PUBLIC { get; set; }
        public Nullable<long> DEPARTMENT_ID { get; set; }
        public Nullable<short> IS_PUBLIC_IN_DEPARTMENT { get; set; }
    
        public virtual HIS_DEPARTMENT HIS_DEPARTMENT { get; set; }
    }
}

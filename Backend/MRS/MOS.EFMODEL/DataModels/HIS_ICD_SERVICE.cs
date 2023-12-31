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
    
    public partial class HIS_ICD_SERVICE
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
        public string ICD_CODE { get; set; }
        public string ICD_NAME { get; set; }
        public long ICD_ID__DELETE { get; set; }
        public Nullable<long> SERVICE_ID { get; set; }
        public Nullable<long> ACTIVE_INGREDIENT_ID { get; set; }
        public Nullable<short> IS_INDICATION { get; set; }
        public Nullable<short> IS_CONTRAINDICATION { get; set; }
        public string CONTRAINDICATION_CONTENT { get; set; }
        public Nullable<short> IS_WARNING { get; set; }
        public Nullable<long> MIN_DURATION { get; set; }
    
        public virtual HIS_ACTIVE_INGREDIENT HIS_ACTIVE_INGREDIENT { get; set; }
        public virtual HIS_SERVICE HIS_SERVICE { get; set; }
    }
}

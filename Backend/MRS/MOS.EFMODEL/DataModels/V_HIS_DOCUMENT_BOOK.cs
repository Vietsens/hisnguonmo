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
    
    public partial class V_HIS_DOCUMENT_BOOK
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
        public string DOCUMENT_BOOK_CODE { get; set; }
        public string DOCUMENT_BOOK_NAME { get; set; }
        public long TOTAL_NUM_ORDER { get; set; }
        public long FROM_NUM_ORDER { get; set; }
        public Nullable<long> YEAR { get; set; }
        public Nullable<short> IS_SICK_BHXH { get; set; }
        public Nullable<long> CURRENT_NUM_ORDER { get; set; }
    }
}

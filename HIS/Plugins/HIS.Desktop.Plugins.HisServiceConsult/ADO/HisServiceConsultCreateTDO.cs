/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 */
using System.Collections.Generic;

namespace HIS.Desktop.Plugins.HisServiceConsult.ADO
{
    /// <summary>
    /// TDO gui len BE khi tao moi ket qua tu van.
    /// </summary>
    public class HisServiceConsultCreateTDO
    {
        public long TREATMENT_ID { get; set; }
        public string ConsultantLoginName { get; set; }
        public string ConsultantUserName { get; set; }
        public long CONSULT_RESULT_TYPE_ID { get; set; }
        public string REASON { get; set; }
        public string DESCRIPTION { get; set; }
        public long? CONSULT_TIME { get; set; }
        public List<long> PACKAGE_IDS { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.WCF.DCO
{
    [DataContract]
    public class ResultSaleDCO
    {
        [DataMember]
        public decimal Amount { get; set; }

        [DataMember]
        public string AuditKey { get; set; }

        [DataMember]
        public string CdaTraceCode { get; set; }

        [DataMember]
        public decimal Fee { get; set; }

        [DataMember]
        public string PgwResultCode { get; set; }

        [DataMember]
        public string ResultCode { get; set; }

        [DataMember]
        public decimal SendAccountBalance { get; set; }

        [DataMember]
        public string ServiceCode { get; set; }
    }
}

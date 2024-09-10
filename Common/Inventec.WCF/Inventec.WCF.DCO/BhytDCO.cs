using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.WCF.DCO
{
    [DataContract()]
    public class BhytDCO
    {
        [DataMember]
        public string Address { get; set; }

        [DataMember]
        public string BhytNumber { get; set; }

        [DataMember]
        public long FromTime { get; set; }

        [DataMember]
        public string LiveCode { get; set; }

        [DataMember]
        public string MediOrgCode { get; set; }

        [DataMember]
        public string MediOrgName { get; set; }

        [DataMember]
        public long ToTime { get; set; }
    }
}

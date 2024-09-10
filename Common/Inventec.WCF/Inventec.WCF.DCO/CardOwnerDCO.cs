using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.WCF.DCO
{
    [DataContract()]
    public class CardOwnerDCO
    {
        [DataMember]
        public string BankCardCode { get; set; }

        [DataMember]
        public List<BhytDCO> Bhyts { get; set; }

        [DataMember]
        public string CardCode { get; set; }

        [DataMember]
        public string CardNumber { get; set; }

        [DataMember]
        public PeopleDCO People { get; set; }

        [DataMember]
        public string ServiceCode { get; set; }
    }
}

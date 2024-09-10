using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.WCF.DCO
{
    [DataContract]
    public class SaleDCO
    {
        [DataMember]
        public string SaleId { get; set; }

        [DataMember]
        public decimal Amount { get; set; }

        [DataMember]
        public string[] ListServiceCode { get; set; }
    }
}

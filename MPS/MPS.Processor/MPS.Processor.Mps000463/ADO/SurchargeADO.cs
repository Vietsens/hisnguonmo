using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000463.ADO
{
    public class SurchargeADO
    {
        public int STT { get; set; }
        public string SURCHARGE_NAME { get; set; }
        public decimal AMOUNT { get; set; }
        public decimal SURCHARGE_AMOUNT { get; set; }
        public long? SORT_ORDER { get; set; }
    }
}

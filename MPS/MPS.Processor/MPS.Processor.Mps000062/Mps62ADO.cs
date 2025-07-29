using MPS.Processor.Mps000062.PDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000062
{
    public class Mps62ADO
    {
        public string MedicineTypeCode { get; set; }
        public List<ExpMestMetyReqADO> List { get; set; }
        public string Dates { get; set; }
        public long TotalDateNumber { get; set; }
    }
}

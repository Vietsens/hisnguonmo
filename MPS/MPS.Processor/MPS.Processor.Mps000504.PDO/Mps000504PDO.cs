using MOS.EFMODEL.DataModels;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000504.PDO
{
    public class Mps000504PDO : RDOBase
    {
        public V_HIS_TREATMENT_FEE Treatment { get; set; }
        public List<V_HIS_SERE_SERV> HisSereServ { get; set; }
        public long fromDateReq { get; set; }
        public long toDateReq { get; set; }
        public Mps000504PDO(V_HIS_TREATMENT_FEE treatment, List<V_HIS_SERE_SERV> hisSereServ, long fromDateReq, long toDateReq)
        {
            this.HisSereServ = hisSereServ;
            this.Treatment = treatment;
            this.fromDateReq = fromDateReq;
            this.toDateReq = toDateReq;
        }
    }
}

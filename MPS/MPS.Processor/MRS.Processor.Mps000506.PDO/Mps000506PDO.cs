using MOS.EFMODEL.DataModels;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000506.PDO
{
    public class Mps000506PDO : RDOBase
    {
        public HIS_SERVICE_REQ ServiceReq;
        public HIS_SERE_SERV_VIEX sereServViex;

        public Mps000506PDO(HIS_SERVICE_REQ serviceReq)
        {
            this.ServiceReq = serviceReq;
        }
        public Mps000506PDO(HIS_SERVICE_REQ serviceReq, HIS_SERE_SERV_VIEX sereServViex)
        {
            this.ServiceReq = serviceReq;
            this.sereServViex = sereServViex;
        }
    }
}

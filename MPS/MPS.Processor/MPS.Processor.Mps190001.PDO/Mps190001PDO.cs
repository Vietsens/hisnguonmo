using MOS.EFMODEL.DataModels;
using MPS.ProcessorBase.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps190001.PDO
{
    public partial class Mps190001PDO : RDOBase
    {
        public Mps190001PDO(
           List<MPS.Processor.Mps190001.PDO.ServiceReqADO> serviceReqList)
        {
            try
            {
                this.lstServiceReq = serviceReqList;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

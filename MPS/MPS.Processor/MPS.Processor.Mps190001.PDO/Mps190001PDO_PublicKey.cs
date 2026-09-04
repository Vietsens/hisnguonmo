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
        public List<MPS.Processor.Mps190001.PDO.ServiceReqADO> lstServiceReq { get; set; }
    }

    public class Mps190001ADO
    {
        public string bebRoomName { get; set; }
        public string departmentName { get; set; }
        public decimal ratio { get; set; }
        public string REQUEST_USER_MOBILE { get; set; }
    }
}

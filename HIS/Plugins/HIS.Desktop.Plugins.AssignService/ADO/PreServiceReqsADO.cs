using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AssignService.ADO
{
    class PreServiceReqsADO : V_HIS_SERVICE_REQ_6
    {
        public bool IsReqPicked { get; set; } = false;  // Trạng thái checkbox

    
        public PreServiceReqsADO(V_HIS_SERVICE_REQ_6 source)
        {
            if (source == null) return;
            Inventec.Common.Mapper.DataObjectMapper.Map<PreServiceReqsADO>(this, source);
        }
    }
}

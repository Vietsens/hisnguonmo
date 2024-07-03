using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.CallPatientTypeAlter.ADO
{
    public class ServiceConditionADO : HIS_SERVICE_CONDITION
    {
        public long SERVICE_CONDITION_ID { get; set; }
        public ServiceConditionADO(HIS_SERVICE_CONDITION data)
        {
            if (data != null)
            {
                Inventec.Common.Mapper.DataObjectMapper.Map<HIS_SERVICE_CONDITION>(this, data);
                if(HEIN_RATIO != null)
                    HEIN_RATIO = this.HEIN_RATIO * 100;
            }
        }
    }
}

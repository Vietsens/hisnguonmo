using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ListSurgMisuByTreatment.ADO
{
    class SurgMisuADO : V_HIS_SERE_SERV_1
    {
        public bool IS_SELECTED { get; set; }

        public SurgMisuADO(V_HIS_SERE_SERV_1 sereServ)
        {
            Inventec.Common.Mapper.DataObjectMapper.Map<V_HIS_SERE_SERV_1>(this, sereServ);
            this.IS_SELECTED = false;
        }
    }
}

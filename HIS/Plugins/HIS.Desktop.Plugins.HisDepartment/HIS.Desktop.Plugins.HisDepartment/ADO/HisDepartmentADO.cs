using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisDepartment.ADO
{
    public class HisDepartmentADO : MOS.EFMODEL.DataModels.HIS_DEPARTMENT
    {
        public bool IS_CHECK_DEPT { get; set; }

        public HisDepartmentADO() { }
       
        public HisDepartmentADO(HIS_DEPARTMENT dept)
        {
            Inventec.Common.Mapper.DataObjectMapper.Map<HIS_DEPARTMENT>(this, dept);
        }
    }
}

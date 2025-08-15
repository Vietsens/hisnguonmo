using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.MedicineTypeCreate.ADO
{
    class DepartmentADO : HIS_DEPARTMENT
    {
        public bool IsRadioChecked { get; set; }
        public bool IsCheckBoxChecked { get; set; }

        public DepartmentADO() { }

        public DepartmentADO(HIS_DEPARTMENT data)
        {
            Inventec.Common.Mapper.DataObjectMapper.Map<DepartmentADO>(this, data);
        }
    }
}

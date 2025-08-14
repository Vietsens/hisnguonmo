using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.MaterialTypeCreate.ADO
{
    class PatientTypeADO: HIS_PATIENT_TYPE
    {
        public bool IsRadioChecked { get; set; }
        public bool IsCheckBoxChecked { get; set; }

        public PatientTypeADO() { }

        public PatientTypeADO(HIS_PATIENT_TYPE data)
        {
            Inventec.Common.Mapper.DataObjectMapper.Map<PatientTypeADO>(this, data);
        }
    }
}

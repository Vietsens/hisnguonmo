using MOS.SDO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AssignPrescriptionPK.ADO
{
    public class AlertLogADO : HisGfrAlertLogSDO
    {
        public string IdGuild { get; set; }
        public AlertLogADO(HisGfrAlertLogSDO data, MediMatyTypeADO ado)
        {
            Inventec.Common.Mapper.DataObjectMapper.Map<AlertLogADO>(this, data);
            IdGuild = ado.PrimaryKey;
        }
    }
}

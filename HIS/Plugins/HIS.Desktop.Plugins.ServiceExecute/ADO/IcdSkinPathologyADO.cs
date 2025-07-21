using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.ServiceExecute.ADO
{
    public class IcdSkinPathologyADO
    {
        public long Id { get; set; }
        public string ICD_SKIN_PATHOLOGY_CODE {  get; set; }
        public string ICD_SKIN_PATHOLOGY_NAME { get; set; }
        public bool IS_CHECK {  get; set; } 
    }
}

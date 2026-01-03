using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AssignService.ADO
{
    public class GuaranteeInfoADO
    {
        public string GUARANTEE_CODE { get; set; }
        public decimal GUARANTEE_REGISTER { get; set; }     
        public decimal GUARANTEE_USED { get; set; }       
        public decimal GUARANTEE_BALANCE { get; set; }
    }
}

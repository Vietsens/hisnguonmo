using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.EInvoiceCreate.ADO
{
    public class BranchADO
    {
        public long ID { get; set; }
        public string BRANCH_CODE { get; set; }
        public string BRANCH_NAME { get; set; }
        public bool IS_ALL { get; set; }
        public bool IS_SELECTED { get; set; }
    }
}

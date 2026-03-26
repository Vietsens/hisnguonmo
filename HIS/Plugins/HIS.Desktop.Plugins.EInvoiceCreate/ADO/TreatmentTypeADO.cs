using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.EInvoiceCreate.ADO
{
    public class TreatmentTypeADO
    {
        public long ID { get; set; }
        public string TREATMENT_TYPE_NAME { get; set; }
        public bool IS_ALL { get; set; }
        public bool IS_SELECTED { get; set; }
    }
}

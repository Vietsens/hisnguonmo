using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.MedicalExpenseGuarantee
{
    public class MedicalExpenseGuaranteeProcessor
    {
        string address {  get; set; }
        string code {  get; set; }
        string limit {  get; set; }

        public MedicalExpenseGuaranteeProcessor(string address, string code, string limit)
        {
            this.address = address;
            this.code = code;
            this.limit = limit;
        }
    }
}

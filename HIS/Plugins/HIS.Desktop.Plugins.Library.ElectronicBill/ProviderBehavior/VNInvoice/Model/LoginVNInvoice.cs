using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.Library.ElectronicBill.ProviderBehavior.VNInvoice.Model
{
    public class InputLoginVNInvoice
    {
        public string taxCode { get; set; }
        public string username { get; set; }
        public string password { get; set; }
    }
    public class OutputLoginVNInvoice
    {
        public string accessToken { get; set; }
        public string expireIn { get; set; }
    }
}

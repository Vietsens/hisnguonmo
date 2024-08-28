using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.ElectronicBillViettel
{
    public class DataInitApi
    {
        public string VIETTEL_Address { get; set; }
        public string User { get; set; }
        public string Pass { get; set; }
        public string SupplierTaxCode { get; set; }
        public Version Version { get; set; }
    }

    public enum Version
    {
        v1,
        v2
    }
}

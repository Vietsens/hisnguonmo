using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.ElectronicBill.Misa
{
    public class DataInit
    {
        public string BaseUrl { get; set; }
        public string User { get; set; }
        public string Pass { get; set; }
        public string AppID { get; set; }
        public string TaxCode { get; set; }
        public string SignUrl { get; set; }
        public string DownloadUrl { get; set; }

        public object DataCreate { get; set; }

        public object DataPreview { get; set; }

        public object DataGet { get; set; }

        public object DataDelete { get; set; }

        public object DataSign { get; set; }

        public object DataRelease { get; set; }
    }
}

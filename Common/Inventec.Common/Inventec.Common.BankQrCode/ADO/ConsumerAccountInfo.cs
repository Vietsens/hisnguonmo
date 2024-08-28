using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.BankQrCode.ADO
{
    public class ConsumerAccountInfo
    {
        public string pointOTMethod { get; set; }
        public string bnbID { get; set; }
        public string ConsumerID { get; set; }
        public string transferType { get; set; }
        public string ccy { get; set; }
        public string countryCode { get; set; }
    }
}

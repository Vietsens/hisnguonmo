using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.BankQrCode.Provider.LPBANK.Model
{
    public class LpInfoData
    {
        public string payLoad { get; set; }
        public string methodCode { get; set; }
        public string guid { get; set; }
        public string acquierOrBnb { get; set; }
        public string merchantOrConsumer { get; set; }
        public string qrType { get; set; }
        public string ccy { get; set; }
        public string CounttryCode { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.BankQrCode.Provider.VIETTINBANK
{
    class ConfigADO
    {
        public string payLoad { get; set; }
        public string pointOTMethod { get; set; }
        public string masterMerchant { get; set; }
        public string merchantCode { get; set; }
        public string merchantCC { get; set; }
        public string merchantName { get; set; }
        public string merchantCity { get; set; }
        public string ccy { get; set; }
        public string CounttryCode { get; set; }
        public string terminalId { get; set; }
        public string storeID { get; set; }
        public string expDate { get; set; }
    }
}

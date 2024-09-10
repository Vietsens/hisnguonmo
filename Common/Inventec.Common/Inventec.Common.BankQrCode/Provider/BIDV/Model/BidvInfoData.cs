using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.BankQrCode.Provider.BIDV.Model
{
    public class BidvInfoData
    {
        public string PayLoad { get; set; }
        public string PointOTMethod { get; set; }
        public string Guid { get; set; }
        public string MerchantCode { get; set; }
        public string MCC { get; set; }
        public string MerchantName { get; set; }
        public string MerchantCity { get; set; }
        public string Ccy { get; set; }
        public string CountryCode { get; set; }
        public string TerminalLabel { get; set; }
        public string StoreLabel { get; set; }
        public string PostalCode { get; set; }
    }
}

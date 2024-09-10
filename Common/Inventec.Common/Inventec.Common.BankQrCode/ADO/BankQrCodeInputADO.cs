using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventec.Common.BankQrCode.ADO
{
    public class BankQrCodeInputADO
    {
        public string SystemConfig { get; set; }
        public string AppliactionConfig { get; set; }
        public string TransactionCode { get; set; }
        public decimal Amount { get; set; }
        public string Purpose { get; set; }
        public ConsumerAccountInfo ConsumerInfo { get; set; }
    }
}

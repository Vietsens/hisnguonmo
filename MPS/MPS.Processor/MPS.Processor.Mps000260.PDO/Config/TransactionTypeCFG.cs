using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPS.Processor.Mps000260.PDO.Config
{
    public class TransactionTypeCFG
    {
        public long? TRANSACTION_TYPE_ID__BILL { get; set; }

        public long? TRANSACTION_TYPE_ID__REPAY { get; set; }

        public long? TRANSACTION_TYPE_ID__DEPOSIT { get; set; }
    }
}
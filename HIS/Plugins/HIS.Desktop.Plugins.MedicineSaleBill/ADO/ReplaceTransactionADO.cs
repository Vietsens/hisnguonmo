using MOS.EFMODEL.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.MedicineSaleBill.ADO
{
    internal class ReplaceTransactionADO
    {
        public long ID { get; set; }
        public string TRANSACTION_CODE { get; set; }
        public string TRANSACTION_TIME { get; set; }
        public long NUM_ORDER { get; set; }

        public ReplaceTransactionADO(HIS_TRANSACTION data)
        {
            this.ID = data.ID;
            this.TRANSACTION_CODE = data.TRANSACTION_CODE;
            this.TRANSACTION_TIME = Inventec.Common.DateTime.Convert.TimeNumberToTimeString(data.TRANSACTION_TIME);
            this.NUM_ORDER = data.NUM_ORDER;
        }
    }
}

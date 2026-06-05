/* IVT
 * @Project : hisnguonmo
 * Copyright (C) 2026 INVENTEC
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HIS.UC.TransactionPayformGrid.ADO
{
    /// <summary>
    /// Mot ngan hang kha dung cho cot "Ngan hang".
    /// Form cha map tu HIS_BANK.
    /// </summary>
    public class BankItemADO
    {
        /// <summary>ID ngan hang (HIS_BANK.ID)</summary>
        public long BANK_ID { get; set; }

        /// <summary>Ma ngan hang</summary>
        public string BANK_CODE { get; set; }

        /// <summary>Ten ngan hang (hien thi)</summary>
        public string BANK_NAME { get; set; }
    }
}

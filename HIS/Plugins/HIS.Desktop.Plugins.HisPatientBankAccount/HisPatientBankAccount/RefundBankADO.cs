using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisPatientBankAccount.HisPatientBankAccount
{
    /// <summary>
    /// Doi tuong dai dien cau hinh ngan hang hoan tien
    /// lay tu HIS_CONFIG co KEY bat dau bang "HIS.Desktop.Plugins.RefundByTransfer".
    /// Vd: KEY = "HIS.Desktop.Plugins.RefundByTransfer.PVCBInfo" => BankCode = "PVCB".
    /// </summary>
    internal class RefundBankADO
    {
        /// <summary>Ma ngan hang (dung de truyen vao BankHub)</summary>
        public string BankCode { get; set; }

        /// <summary>KEY goc cua cau hinh trong HIS_CONFIG</summary>
        public string ConfigKey { get; set; }
    }
}

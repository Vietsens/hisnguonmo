using HIS.Desktop.LocalStorage.HisConfig;
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.RefundByTransfer.Base
{
    class HisConfigCFG
    {
        internal const string REFUND_BY_TRANSFER_MBB__CONFIG = "HIS.Desktop.Plugins.RefundByTransfer.MBBInfo";

        internal static string IsSplitTotalReceivePrice
        {
            get
            {
                return GetValue("HIS.Desktop.Plugins.Transaction.IsSplitTotalReceivePrice");
            }
        }

        private static string GetValue(string code)
        {
            string result = null;
            try
            {
                return HisConfigs.Get<string>(code);
            }
            catch (Exception ex)
            {
                LogSystem.Warn(ex);
                result = null;
            }
            return result;
        }
    }
}

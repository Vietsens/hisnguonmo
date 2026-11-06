using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.SupplyOrderCreate.Base
{
    class SupplyConstant
    {
        /// <summary>
        /// HIS_SUPPLY_ORDER.SUPPLY_ORDER_STT
        /// </summary>
        internal const short ORDER_STT__DRAFT = 1;
        internal const short ORDER_STT__CONFIRMED = 2;
        internal const short ORDER_STT__CANCELED = 3;

        /// <summary>
        /// HIS_SUPPLY_ORDER.MAIL_STT (null = not sent)
        /// </summary>
        internal const short MAIL_STT__NOT_SENT = 1;
        internal const short MAIL_STT__SENDING = 2;
        internal const short MAIL_STT__SUCCESS = 3;
        internal const short MAIL_STT__ERROR = 4;

        /// <summary>
        /// Medi stock codes in the supply process, comma separated. Empty = feature off
        /// </summary>
        internal const string CONFIG_KEY__MEDI_STOCK_CODES = "MOS.HIS_ANTICIPATE.SUPPLY.MEDI_STOCK_CODES";

        internal static List<string> GetSupplyMediStockCodes()
        {
            List<string> result = new List<string>();
            try
            {
                string value = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CONFIG_KEY__MEDI_STOCK_CODES);
                if (!String.IsNullOrWhiteSpace(value))
                {
                    result = value.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(o => o.Trim()).Where(o => o != "").Distinct().ToList();
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
            }
            return result;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Desktop.Plugins.SupplyAnticipateCreate.Base
{
    class SupplyConstant
    {
        /// <summary>
        /// HIS_ANTICIPATE.SUPPLY_STT
        /// </summary>
        internal const short STT__DRAFT = 1;
        internal const short STT__WAIT_PHARMACY = 2;
        internal const short STT__WAIT_ACCOUNTANT = 3;
        internal const short STT__RETURNED = 4;
        internal const short STT__APPROVED = 5;

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

        /// <summary>
        /// Period yyyyMM01000000 -> MM/yyyy
        /// </summary>
        internal static string PeriodToString(long? period)
        {
            if (!period.HasValue || period.Value < 10000000000000)
            {
                return "";
            }
            string s = period.Value.ToString();
            return s.Substring(4, 2) + "/" + s.Substring(0, 4);
        }

        internal static long DateToPeriod(DateTime date)
        {
            return date.Year * 10000000000L + date.Month * 100000000L + 1000000L;
        }
    }
}

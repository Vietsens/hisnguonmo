using HIS.Desktop.LocalStorage.HisConfig;
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.MonitorMediStockDataError.Config
{
    class HisConfig
    {
        public const string HIS_STOCK_DATA_ERROR_MEDI_STOCK_CODE = "HIS_STOCK_DATA_ERROR.MEDI_STOCK_CODE";
        internal static string MediStockCode;
        internal static void LoadConfig()
        {
            try
            {
                MediStockCode = GetValue(HIS_STOCK_DATA_ERROR_MEDI_STOCK_CODE);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
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

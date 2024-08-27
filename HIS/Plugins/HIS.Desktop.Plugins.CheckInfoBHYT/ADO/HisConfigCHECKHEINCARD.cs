using HIS.Desktop.LocalStorage.HisConfig;
using Inventec.Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.CheckInfoBHYT.ADO
{
    class HisConfigCHECKHEINCARD
    {
        internal const string HIS_CHECK_HEIN_CARD_BHXH__API = "HIS.CHECK_HEIN_CARD.BHXH__API";

        internal static string CHECK_HEIN_CARD_BHXH__API;

        internal static void LoadConfig()
        {
            try
            {
                CHECK_HEIN_CARD_BHXH__API = GetValue(HIS_CHECK_HEIN_CARD_BHXH__API);
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
                LogSystem.Error(ex);
                result = null;
            }
            return result;
        }
    }
}

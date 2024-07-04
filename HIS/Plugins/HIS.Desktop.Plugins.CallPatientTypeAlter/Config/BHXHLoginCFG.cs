using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HIS.Desktop.LocalStorage.HisConfig;

namespace HIS.Desktop.Plugins.CallPatientTypeAlter.Config
{
    class BHXHLoginCFG
    {
        private const string CONFIG_KEY = "HIS.CHECK_HEIN_CARD.BHXH.LOGIN.USER_PASS";
        private const string CONFIG_KEY_ADDRESS = "HIS.CHECK_HEIN_CARD.BHXH__ADDRESS";

        public static string USERNAME;
        public static string PASSWORD;
        public static string ADDRESS;

        public static void LoadConfig()
        {
            try
            {
                USERNAME = Get(HisConfigs.Get<string>(CONFIG_KEY), 0);
                PASSWORD = Get(HisConfigs.Get<string>(CONFIG_KEY), 1);
                ADDRESS = HisConfigs.Get<string>(CONFIG_KEY_ADDRESS).Trim();
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }

        private static string Get(string value, int index)
        {
            string user = "";
            try
            {
                if (!String.IsNullOrEmpty(value))
                {
                    var data = value.Split(':');
                    if (data != null && data.Length >= index)
                    {
                        user = data[index].Trim();
                    }
                }
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                user = "";
            }
            return user;
        }
    }
}

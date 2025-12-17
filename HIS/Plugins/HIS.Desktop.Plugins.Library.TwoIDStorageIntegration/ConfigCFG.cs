using HIS.Desktop.LocalStorage.HisConfig;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using HIS.Desktop.LocalStorage.EmrConfig;
using Inventec.Common.SignLibrary;
namespace HIS.Desktop.Plugins.Library.TwoIDStorageIntegration
{
    internal class ConfigCFG
    {
        private const string CONFIG_KEY_EMR_2ID_STORAGE_INFO = "EMR.2ID.STORAGE_INFO";
        internal static string emr2IdStorageInfo;

        public class StorageConfig
        {
            public string ApiBaseUrl { get; set; }
            public string ApiKey { get; set; }
            public string ApiSecret { get; set; }
          
        }
        internal static void LoadConfig()
        {
            try
            {
                HIS.Desktop.LocalStorage.EmrConfig.ConfigLoader.Refresh();
                emr2IdStorageInfo = GetValueFromEmr(CONFIG_KEY_EMR_2ID_STORAGE_INFO);
                Inventec.Common.Logging.LogSystem.Debug(Inventec.Common.Logging.LogUtil.TraceData("emr2IdStorageInfo  input:", emr2IdStorageInfo));

            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
        private static string GetValueFromEmr(string key)
        {
            try
            {
                return GlobalStore.EmrConfigs
                    .FirstOrDefault(o => o.KEY == key)
                    ?.VALUE ?? "";
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return "";
        }
        private static string GetValue(string key)
        {
            try
            {
                return HisConfigs.Get<string>(key);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
            return "";
        }
        public static StorageConfig GetStorageConfig()
        {
            if (string.IsNullOrEmpty(emr2IdStorageInfo))
                LoadConfig();

            if (!string.IsNullOrEmpty(emr2IdStorageInfo) && emr2IdStorageInfo.TrimStart().StartsWith("{"))
            {
                try
                {
                    return JsonConvert.DeserializeObject<StorageConfig>(emr2IdStorageInfo);
                }
                catch (Exception ex)
                {
                    Inventec.Common.Logging.LogSystem.Error(ex);
                }
            }
            else if (!string.IsNullOrEmpty(emr2IdStorageInfo))
            {
                var parts = emr2IdStorageInfo.Split('|');
                return new StorageConfig
                {
                    ApiBaseUrl = parts.Length > 0 ? parts[0] : "",
                    ApiKey = parts.Length > 1 ? parts[1] : "",
                    ApiSecret = parts.Length > 2 ? parts[2] : ""
                };
            }
            return new StorageConfig();
        }
    }
}


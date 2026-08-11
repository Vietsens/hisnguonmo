using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisTreatmentFile.Config
{
    class ConfigKey
    {
        private const string IS_HAS_CONNECTION_EMR = "MOS.HAS_CONNECTION_EMR";
        private const string CON_FIG___TREATMENT_FILE_VIEW_EMR_DOCUMENT = "HIS.Desktop.Plugins.HisTreatmentFile.ViewEmrDocument";

        internal static bool IsHasConnectionEmr;
        internal static bool ViewEmrDocument;
        internal static void GetConfigKey()
        {
            try
            {
                IsHasConnectionEmr = GetValueHis(IS_HAS_CONNECTION_EMR) == "1";
                ViewEmrDocument = GetValueHis(CON_FIG___TREATMENT_FILE_VIEW_EMR_DOCUMENT) == "1";
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
                return HIS.Desktop.LocalStorage.EmrConfig.EmrConfigs.Get<string>(code);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                result = null;
            }
            return result;
        }
        private static string GetValueHis(string code)
        {
            string result = null;
            try
            {
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(code);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Warn(ex);
                result = null;
            }
            return result;
        }
    }
}

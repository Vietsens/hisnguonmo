using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AttackFile.Config
{
    class ConfigKey
    {
        private const string CONFIG_KEY__EMR_PATIENT_SIGN_OPTION = "EMR.EMR_DOCUMENT.PATIENT_SIGN.OPTION";
        private const string IS_HAS_CONNECTION_EMR = "MOS.HAS_CONNECTION_EMR";
        private const string IS_STORED_MUST_REQ_TO_VIEW = "EMR.EMR_TREATMENT.STORED_MUST_REQ_TO_VIEW";
        private const string IS_DELETE_FILE_OPTION = "EMR.EMR_DOCUMENT.DELETE_FILE_OPTION";
        internal static bool IsStoredMustReqToView;
        internal static bool IsHasConnectionEmr;
        internal static string patientSignOption;
        internal static string deleteFileOption;

        internal static void GetConfigKey()
        {
            try
            {
                IsStoredMustReqToView = GetValue(IS_STORED_MUST_REQ_TO_VIEW) == "1";
                IsHasConnectionEmr = GetValueHis(IS_HAS_CONNECTION_EMR) == "1";
                patientSignOption = GetValue(CONFIG_KEY__EMR_PATIENT_SIGN_OPTION);
                deleteFileOption = GetValue(IS_DELETE_FILE_OPTION);
                Inventec.Common.Logging.LogSystem.Debug(patientSignOption);
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
                return LocalStorage.EmrConfig.EmrConfigs.Get<string>(code);
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

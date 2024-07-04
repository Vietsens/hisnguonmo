using Inventec.Common.LocalStorage.SdaConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.CallPatientTypeAlter
{
    class CheckPreviousPrescriptionCFG
    {
        private const string CONFIG_KEY = "MOS.HIS_TREATMENT.IS_CHECK_PREVIOUS_PRESCRIPTION";
        private const string DEFAULT__IS_CHECK_PREVIOUS_PRESCRIPTION = "1";

        private static bool? isCheckPreviousPrescription;
        public static bool IsCheckPreviousPrescription
        {
            get
            {
                if (!isCheckPreviousPrescription.HasValue)
                {
                    isCheckPreviousPrescription = GetIsCheck(SdaConfigs.Get<string>(CONFIG_KEY));
                }
                return isCheckPreviousPrescription.Value;
            }
        }

        private static bool GetIsCheck(string value)
        {
            return (value == DEFAULT__IS_CHECK_PREVIOUS_PRESCRIPTION);
        }
    }
}

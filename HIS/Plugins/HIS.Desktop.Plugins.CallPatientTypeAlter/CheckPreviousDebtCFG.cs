using Inventec.Common.LocalStorage.SdaConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.CallPatientTypeAlter
{
    class CheckPreviousDebtCFG
    {
        private const string CONFIG_KEY = "MOS.HIS_TREATMENT.IS_CHECK_PREVIOUS_DEBT";
        private const string DEFAULT__IS_CHECK_PREVIOUS_DEBT = "1";

        private static bool? isCheckPreviousDebt;
        public static bool IsCheckPreviousDebt
        {
            get
            {
                if (!isCheckPreviousDebt.HasValue)
                {
                    isCheckPreviousDebt = GetIsCheck(SdaConfigs.Get<string>(CONFIG_KEY));
                }
                return isCheckPreviousDebt.Value;
            }
        }

        private static bool GetIsCheck(string value)
        {
            return (value == DEFAULT__IS_CHECK_PREVIOUS_DEBT);
        }
    }
}

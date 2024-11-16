using HIS.Desktop.LocalStorage.HisConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.EnterKskInfomantionVer2.Config
{
    public class HisConfigCFG
    {
        private const string KEY__DisablePartExamByExecutor = "HIS.Desktop.Plugins.EnterKskInfomantionVer2.DisablePartExamByExecutor";

        internal static string DisablePartExamByExecutor;

        internal static void LoadConfig()
        {
            try
            {
                DisablePartExamByExecutor = HisConfigs.Get<string>(KEY__DisablePartExamByExecutor);
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
            }
        }
    }
}

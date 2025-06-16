using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.AnalyzeMedicalImage.Config
{
    class AppConfigKeys
    {
        internal const string CONFIG_KEY__ConnectionInfo = "HIS.Desktop.AI.ConnectionInfo";

        internal static string AIConnectionInfo
        {
            get
            {
                return HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(CONFIG_KEY__ConnectionInfo);
            }
        }
    }
}

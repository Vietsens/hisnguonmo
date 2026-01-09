using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.EnterKskInfomantion.Config
{
    class HisConfig
    {
        private const string HiddenTabOptionKey = "HIS.Desktop.Plugins.EnterKskInfomantion.HiddenTabOption";

        internal static int HiddenTabOption
        {
            get
            {
                var value = HIS.Desktop.LocalStorage.HisConfig.HisConfigs.Get<string>(HiddenTabOptionKey);

                return int.TryParse(value, out var opt) ? opt : 0;
            }
        }
    }
}

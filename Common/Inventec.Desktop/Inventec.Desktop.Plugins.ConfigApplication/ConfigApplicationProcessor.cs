using Inventec.Core;
using Inventec.Desktop.Common;
using Inventec.Desktop.Common.Core;
using Inventec.Desktop.Core;
using Inventec.Desktop.Common.Modules;
using Inventec.Desktop.Plugins.ConfigApplication.ConfigApplication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.Desktop.Plugins.ConfigApplication
{
    [ExtensionOf(typeof(DesktopRootExtensionPoint),
       "Inventec.Desktop.Plugins.ConfigApplication",
       "Cấu hình phần mềm",
       "System",
       8,
       "properties_32x32.png",
       "A",
       Module.MODULE_TYPE_ID__UC,
       true,
       true)
    ]
    public class ConfigApplicationProcessor : ModuleBase, IDesktopRoot
    {
        CommonParam param;
        public ConfigApplicationProcessor()
        {
            param = new CommonParam();
        }
        public ConfigApplicationProcessor(CommonParam paramBusiness)
        {
            param = (paramBusiness != null ? paramBusiness : new CommonParam());
        }

        public object Run(object[] args)
        {
            object result = null;
            try
            {
                IConfigApplication behavior = ConfigApplicationFactory.MakeIConfigApplication(param, args);
                result = behavior != null ? (behavior.Run()) : null;
            }
            catch (Exception ex)
            {
                Inventec.Common.Logging.LogSystem.Error(ex);
                result = null;
            }
            return result;
        }

        public override bool IsEnable()
        {
            return true;
        }
    }
}

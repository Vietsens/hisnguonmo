using Inventec.Core;
using Inventec.Desktop.Common.Core;
using Inventec.Desktop.Core;
using Inventec.Desktop.Common.Modules;
using Inventec.Desktop.Plugins.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.Desktop.Plugins.Plugin
{
    [ExtensionOf(typeof(DesktopRootExtensionPoint),
       "Inventec.Desktop.Plugins.Plugin",
       "Quản lý phiên bản chức năng",
       "System",
       35,
       "packageproduct_32x32.png",
       "A",
       Module.MODULE_TYPE_ID__FORM,
       true,
       true)
    ]
    public class PluginProcessor : ModuleBase, IDesktopRoot
    {
        CommonParam param;
        public PluginProcessor()
        {
            param = new CommonParam();
        }
        public PluginProcessor(CommonParam paramBusiness)
        {
            param = (paramBusiness != null ? paramBusiness : new CommonParam());
        }

        /// <summary>
        /// </summary>
        /// <param name="args">Dau vao co 3 tham so: SdaConsumer, NumPageSize, Inventec.UC.PluginControl.ProcessHasException</param>
        /// <returns></returns>
        public object Run(object[] args)
        {
            object result = null;
            try
            {
                IPlugin behavior = PluginFactory.MakeIPlugin(param, args);
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

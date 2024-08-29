using Inventec.Core;
using Inventec.Desktop.Common.Core;
using Inventec.Desktop.Core;
using Inventec.Desktop.Common.Modules;
using Inventec.Desktop.Plugins.Updater;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.Desktop.Plugins.Updater
{
    [ExtensionOf(typeof(DesktopRootExtensionPoint),
       "Inventec.Desktop.Plugins.Updater",
       "Cập nhật phiên bản",
       "System",
       6,
       "convert_32x32.png",
       "A",
       Module.MODULE_TYPE_ID__FORM,
       true,
       true)
    ]
    public class VersionUpdateProcessor : ModuleBase, IDesktopRoot
    {
        CommonParam param;
        public VersionUpdateProcessor()
        {
            param = new CommonParam();
        }
        public VersionUpdateProcessor(CommonParam paramBusiness)
        {
            param = (paramBusiness != null ? paramBusiness : new CommonParam());
        }

        /// <summary>
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public object Run(object[] args)
        {
            object result = null;
            try
            {
                IVersionUpdate behavior = VersionUpdateFactory.MakeIVersionUpdate(param, args);
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

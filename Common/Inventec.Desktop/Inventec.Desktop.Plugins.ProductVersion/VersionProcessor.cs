using Inventec.Core;
using Inventec.Desktop.Common.Core;
using Inventec.Desktop.Core;
using Inventec.Desktop.Common.Modules;
using Inventec.Desktop.Plugins.ProductVersion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.Desktop.Plugins.ProductVersion
{
    [ExtensionOf(typeof(DesktopRootExtensionPoint),
       "Inventec.Desktop.Plugins.ProductVersion",
       "Phiên bản phần mềm",
       "System",
       5,
       "version_32x32.png",
       "A",
       Module.MODULE_TYPE_ID__FORM,
       true,
       true)
    ]
    public class VersionProcessor : ModuleBase, IDesktopRoot
    {
        CommonParam param;
        public VersionProcessor()
        {
            param = new CommonParam();
        }
        public VersionProcessor(CommonParam paramBusiness)
        {
            param = (paramBusiness != null ? paramBusiness : new CommonParam());
        }

        /// <summary>
        /// </summary>
        /// <param name="args">Dau vao co 3 tham so: SdaConsumer, NumPageSize, Inventec.UC.VersionControl.ProcessHasException</param>
        /// <returns></returns>
        public object Run(object[] args)
        {
            Form result = null;
            try
            {
                IVersion behavior = VersionFactory.MakeIVersion(param, args);
                result = behavior != null ? (Form)(behavior.Run()) : null;
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

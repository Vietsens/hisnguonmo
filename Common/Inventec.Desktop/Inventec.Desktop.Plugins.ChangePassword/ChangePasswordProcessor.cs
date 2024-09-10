using Inventec.Core;
using Inventec.Desktop.Common.Core;
using Inventec.Desktop.Core;
using Inventec.Desktop.Common.Modules;
using Inventec.Desktop.Plugins.ChangePassword;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inventec.Desktop.Plugins.ChangePassword
{
    [ExtensionOf(typeof(DesktopRootExtensionPoint),
       "Inventec.Desktop.Plugins.ChangePassword",
       "Đổi mật khẩu",
       "System",
       51,
       "role_32x32.png",
       "A",
       Module.MODULE_TYPE_ID__FORM,
       true,
       true)
    ]
    public class ChangePasswordProcessor : ModuleBase, IDesktopRoot
    {
        CommonParam param;
        public ChangePasswordProcessor()
        {
            param = new CommonParam();
        }
        public ChangePasswordProcessor(CommonParam paramBusiness)
        {
            param = (paramBusiness != null ? paramBusiness : new CommonParam());
        }

        /// <summary>
        /// </summary>
        /// <param name="args">Dau vao co 3 tham so: SdaConsumer, Inventec.UC.ChangePassword.HasExceptionApi, Icon</param>
        /// <returns></returns>
        public object Run(object[] args)
        {
            object result = null;
            try
            {
                IEventLog behavior = EventLogFactory.MakeIChangePassword(param, args);
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

using Inventec.Core;
using Inventec.Desktop.Common.Modules;
using Inventec.Desktop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIS.Desktop.Plugins.HisAlertDepartment.Config
{

    [ExtensionOf(typeof(DesktopRootExtensionPoint), "HIS.Desktop.Plugins.HisAlertDepartment", "Khác", "Bussiness", 8, "canh-bao.png", "", Module.MODULE_TYPE_ID__UC, true, true)]
    class HisAlertDepartmentProcessor : ModuleBase, IDesktopRoot
    {
        CommonParam param;
        public HisAlertDepartmentProcessor()
        {
            param = new CommonParam();
        }
        public HisAlertDepartmentProcessor(CommonParam paramBussiness)
        {
            param = paramBussiness == null ? new CommonParam() : paramBussiness;
        }
        public object Run(object[] args)
        {
            object result = null;
            try
            {
                HisAlertDepartmentStore behavior = HisAlertDepartmentFactory.MakeIControl(param, args);
                result = behavior != null ? (object)(behavior.Run()) : null;
                return result;
            }
            catch (Exception e)
            {
                Inventec.Common.Logging.LogSystem.Warn(e);
                return result;
            }
        }
        public override bool IsEnable()
        {
            bool result = false;
            try
            {
                result = true;
            }
            catch (Exception e)
            {

                return result;
            }
            return result;
        }
    }
}
